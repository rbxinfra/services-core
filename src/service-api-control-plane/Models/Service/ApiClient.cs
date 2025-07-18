namespace Roblox.Service.ApiControlPlane;

using System;

using ApplicationTelemetry;

using Http.Client.ApiControlPlane;

using HttpClientApiClientModel = Http.Client.ApiControlPlane.ApiClientModel;

/// <inheritdoc cref="IApiClient"/>
internal class ApiClient : IApiClient
{
    private const string _CallerName = "apicontrolplane-service";

    private readonly ITelemetry _Telemetry;
    private readonly IApiControlPlaneClient _ApiClient;

    /// <summary>
    /// Internal constructor used by service registration.
    /// </summary>
    /// <param name="initialData">The initial service client model fetched from the service. If null it is initalized as a default client.</param>
    /// <exception cref="ArgumentNullException"><paramref name="initialData"/> cannot be null.</exception>
    internal ApiClient(HttpClientApiClientModel initialData)
    {
        if (initialData == null) throw new ArgumentNullException(nameof(initialData));

        ID = initialData.Id;

        Guid.TryParse(initialData.Key, out var apiKey);
        ApiKey = apiKey;

        Note = initialData.Note;
        IsValid = initialData.IsValid;
        Created = initialData.Created;
        Updated = initialData.Updated;
    }

    /// <summary>
    /// Construct a new instance of <see cref="ApiClient"/>
    /// </summary>
    /// <param name="telemetry">A telemetry to wrap requests.</param>
    /// <param name="apiClient">The service client to call on API Control Plane.</param>
    /// <param name="initialData">The initial service client model fetched from the service. If null it is initalized as a default client.</param>
    /// <exception cref="ArgumentNullException">
    /// - <paramref name="telemetry"/> cannot be null.
    /// - <paramref name="apiClient"/> cannot be null.
    /// </exception>
    public ApiClient(ITelemetry telemetry, IApiControlPlaneClient apiClient, HttpClientApiClientModel initialData)
    {
        initialData ??= new HttpClientApiClientModel();

        _Telemetry = telemetry ?? throw new ArgumentNullException(nameof(telemetry));
        _ApiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));

        UpdateProperties(initialData);
    }

    /// <inheritdoc cref="IApiClient.ID"/>
    public int ID { get; private set; }

    /// <inheritdoc cref="IApiClient.ApiKey"/>
    public Guid ApiKey { get; set; }

    /// <inheritdoc cref="IApiClient.Note"/>
    public string Note { get; set; }

    /// <inheritdoc cref="IApiClient.IsValid"/>
    public bool IsValid { get; internal set; }

    /// <inheritdoc cref="IApiClient.Created"/>
    public DateTime Created { get; private set; }

    /// <inheritdoc cref="IApiClient.Updated"/>
    public DateTime Updated { get; private set; }

    private void UpdateProperties(HttpClientApiClientModel client)
    {
        ID = client.Id;
        ApiKey = Guid.Parse(client.Key);
        Note = client.Note;
        IsValid = client.IsValid;
        Created = client.Created;
        Updated = client.Updated;
    }


    /// <inheritdoc cref="IApiClient.SetInvalid"/>
    public void SetInvalid()
    {
        if (IsValid)
        {
            IsValid = false;
            Save();
        }
    }

    /// <inheritdoc cref="IApiClient.SetValid"/>
    public void SetValid()
    {
        if (!IsValid)
        {
            IsValid = true;
            Save();
        }
    }

    /// <inheritdoc cref="IApiClient.Delete"/>
    public void Delete()
    {
        if (_ApiClient == null) return;

        _Telemetry.WrapSync(TelemetryType.http_client, _CallerName, "/v1/RemoveClient", () => _ApiClient.RemoveClient(new() { Id = ID }));
    }

    /// <inheritdoc cref="IApiClient.Save"/>
    public void Save()
    {
        if (_ApiClient == null) return;

        if (ID == default(int))
            UpdateProperties(_Telemetry.WrapSync(TelemetryType.http_client, _CallerName, "/v1/AddClient", () => _ApiClient.AddClient(new() { Key = ApiKey, Note = Note, IsValid = IsValid })).Data);
        else
            UpdateProperties(_Telemetry.WrapSync(TelemetryType.http_client, _CallerName, "/v1/UpdateClient", () => _ApiClient.UpdateClient(new() { Id = ID, Key = ApiKey, Note = Note, IsValid = IsValid })).Data);
    }
}
