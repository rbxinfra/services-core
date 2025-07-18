namespace Roblox.Service.ApiControlPlane;

using System;

using ApplicationTelemetry;

using Http.Client.ApiControlPlane;

using HttpClientOperationModel = Http.Client.ApiControlPlane.OperationModel;

/// <inheritdoc cref="IApiClient"/>
internal class Operation : IOperation
{
    private const string _CallerName = "apicontrolplane-service";

    private readonly ITelemetry _Telemetry;
    private readonly IApiControlPlaneClient _ApiClient;

    /// <summary>
    /// Internal constructor used by service registration.
    /// </summary>
    /// <param name="initialData">The initial service client model fetched from the service. If null it is initalized as a default client.</param>
    /// <exception cref="ArgumentNullException"><paramref name="initialData"/> cannot be null.</exception>
    internal Operation(HttpClientOperationModel initialData)
    {
        if (initialData == null) throw new ArgumentNullException(nameof(initialData));

        ID = initialData.Id;
        Name = initialData.Name;
        IsEnabled = initialData.IsEnabled;
        Created = initialData.Created;
        Updated = initialData.Updated;
    }

    /// <summary>
    /// Construct a new instance of <see cref="Operation"/>
    /// </summary>
    /// <param name="telemetry">A telemetry to wrap requests.</param>
    /// <param name="apiClient">The service client to call on API Control Plane.</param>
    /// <param name="initialData">The initial service client model fetched from the service. If null it is initalized as a default client.</param>
    /// <exception cref="ArgumentNullException">
    /// - <paramref name="telemetry"/> cannot be null.
    /// - <paramref name="apiClient"/> cannot be null.
    /// </exception>
    public Operation(ITelemetry telemetry, IApiControlPlaneClient apiClient, HttpClientOperationModel initialData)
    {
        initialData ??= new HttpClientOperationModel();

        _Telemetry = telemetry ?? throw new ArgumentNullException(nameof(telemetry));
        _ApiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));

        UpdateProperties(initialData);
    }

    /// <inheritdoc cref="IOperation.ID"/>
    public int ID { get; private set; }

    /// <inheritdoc cref="IOperation.Name"/>
    public string Name { get; set; }

    /// <inheritdoc cref="IOperation.Service"/>
    public IService Service { get; set; }

    /// <inheritdoc cref="IOperation.IsEnabled"/>
    public bool IsEnabled { get; internal set; }

    /// <inheritdoc cref="IApiClient.Created"/>
    public DateTime Created { get; private set; }

    /// <inheritdoc cref="IApiClient.Updated"/>
    public DateTime Updated { get; private set; }

    private void UpdateProperties(HttpClientOperationModel data)
    {
        ID = data.Id;
        Name = data.Name;
        
        if (data.ServiceId != default(int))
        {
            var remoteService = _Telemetry.WrapSync(TelemetryType.http_client, _CallerName, "/v1/GetServiceByID", () => _ApiClient.GetServiceByID(data.ServiceId)).Data;

            Service = new Service(_Telemetry, _ApiClient, remoteService);
        }

        IsEnabled = data.IsEnabled;
        Created = data.Created;
        Updated = data.Updated;
    }

    /// <inheritdoc cref="IOperation.Disable"/>
    public void Disable()
    {
        if (IsEnabled)
        {
            IsEnabled = false;
            Save();
        }
    }

    /// <inheritdoc cref="IOperation.Enable"/>
    public void Enable()
    {
        if (!IsEnabled)
        {
            IsEnabled = true;
            Save();
        }
    }

    /// <inheritdoc cref="IApiClient.Delete"/>
    public void Delete()
    {
        if (_ApiClient == null) return;

        _Telemetry.WrapSync(TelemetryType.http_client, _CallerName, "/v1/RemoveOperation", () => _ApiClient.RemoveOperation(new() { Id = ID }));
    }

    /// <inheritdoc cref="IApiClient.Save"/>
    public void Save()
    {
        if (_ApiClient == null) return;

        if (Service == null)
            throw new ApplicationException("Operation.Service must be set before saving.");

        if (ID == default(int))
            UpdateProperties(_Telemetry.WrapSync(TelemetryType.http_client, _CallerName, "/v1/AddOperation", () => _ApiClient.AddOperation(new() {  Name = Name, ServiceName = Service.Name, IsEnabled = IsEnabled })).Data);
        else
            UpdateProperties(_Telemetry.WrapSync(TelemetryType.http_client, _CallerName, "/v1/UpdateOperation", () => _ApiClient.UpdateOperation(new() { Id = ID, Name = Name, ServiceName = Service.Name, IsEnabled = IsEnabled })).Data);
    }
}
