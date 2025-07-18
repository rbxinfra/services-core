namespace Roblox.Service.ApiControlPlane;

using System;

using ApplicationTelemetry;

using Http.Client.ApiControlPlane;

using HttpClientServiceModel = Http.Client.ApiControlPlane.ServiceModel;

/// <inheritdoc cref="IApiClient"/>
internal class Service : IService
{
    private const string _CallerName = "apicontrolplane-service";

    private readonly ITelemetry _Telemetry;
    private readonly IApiControlPlaneClient _ApiClient;

    /// <summary>
    /// Construct a new instance of <see cref="Service"/>
    /// </summary>
    /// <param name="telemetry">A telemetry to wrap requests.</param>
    /// <param name="apiClient">The service client to call on API Control Plane.</param>
    /// <param name="initialData">The initial service client model fetched from the service. If null it is initalized as a default client.</param>
    /// <exception cref="ArgumentNullException">
    /// - <paramref name="telemetry"/> cannot be null.
    /// - <paramref name="apiClient"/> cannot be null.
    /// </exception>
    public Service(ITelemetry telemetry, IApiControlPlaneClient apiClient, HttpClientServiceModel initialData)
    {
        initialData ??= new HttpClientServiceModel();

        _Telemetry = telemetry ?? throw new ArgumentNullException(nameof(telemetry));
        _ApiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));

        UpdateProperties(initialData);
    }

    /// <inheritdoc cref="IOperation.ID"/>
    public int ID { get; private set; }

    /// <inheritdoc cref="IOperation.Name"/>
    public string Name { get; set; }

    /// <inheritdoc cref="IOperation.IsEnabled"/>
    public bool IsEnabled { get; internal set; }

    /// <inheritdoc cref="IApiClient.Created"/>
    public DateTime Created { get; private set; }

    /// <inheritdoc cref="IApiClient.Updated"/>
    public DateTime Updated { get; private set; }

    private void UpdateProperties(HttpClientServiceModel data)
    {
        ID = data.Id;
        Name = data.Name;
        IsEnabled = data.IsEnabled;
        Created = data.Created;
        Updated = data.Updated;
    }

    /// <inheritdoc cref="IService.Disable"/>
    public void Disable()
    {
        if (IsEnabled)
        {
            IsEnabled = false;
            Save();
        }
    }

    /// <inheritdoc cref="IService.Enable"/>
    public void Enable()
    {
        if (!IsEnabled)
        {
            IsEnabled = true;
            Save();
        }
    }

    /// <inheritdoc cref="IService.Delete"/>
    public void Delete()
    {
        _Telemetry.WrapSync(TelemetryType.http_client, _CallerName, "/v1/RemoveService", () => _ApiClient.RemoveService(new() { Id = ID }));
    }

    /// <inheritdoc cref="IService.Save"/>
    public void Save()
    {
        if (ID == default(int))
            UpdateProperties(_Telemetry.WrapSync(TelemetryType.http_client, _CallerName, "/v1/AddService", () => _ApiClient.AddService(new() { Name = Name, IsEnabled = IsEnabled })).Data);
        else
            UpdateProperties(_Telemetry.WrapSync(TelemetryType.http_client, _CallerName, "/v1/UpdateService", () => _ApiClient.UpdateService(new() { Id = ID, Name = Name, IsEnabled = IsEnabled })).Data);
    }
}
