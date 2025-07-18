namespace Roblox.Service.ApiControlPlane;

using System;

using ApplicationTelemetry;

using Http.Client.ApiControlPlane;

using HttpClientAuthorizationType = Http.Client.ApiControlPlane.AuthorizationType;
using HttpClientServiceAuthorizationModel = Http.Client.ApiControlPlane.ServiceAuthorizationModel;

/// <inheritdoc cref="IApiClient"/>
internal class ServiceAuthorization : IServiceAuthorization
{
    private const string _CallerName = "apicontrolplane-service";

    private readonly ITelemetry _Telemetry;
    private readonly IApiControlPlaneClient _ApiClient;

    /// <summary>
    /// Construct a new instance of <see cref="ServiceAuthorization"/>
    /// </summary>
    /// <param name="telemetry">A telemetry to wrap requests.</param>
    /// <param name="apiClient">The service client to call on API Control Plane.</param>
    /// <param name="initialData">The initial service client model fetched from the service. If null it is initalized as a default client.</param>
    /// <exception cref="ArgumentNullException">
    /// - <paramref name="telemetry"/> cannot be null.
    /// - <paramref name="apiClient"/> cannot be null.
    /// </exception>
    public ServiceAuthorization(ITelemetry telemetry, IApiControlPlaneClient apiClient, HttpClientServiceAuthorizationModel initialData)
    {
        initialData ??= new HttpClientServiceAuthorizationModel();

        _Telemetry = telemetry ?? throw new ArgumentNullException(nameof(telemetry));
        _ApiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));

        UpdateProperties(initialData);
    }

    /// <inheritdoc cref="IServiceAuthorization.ID"/>
    public int ID { get; private set; }

    /// <inheritdoc cref="IServiceAuthorization.Service"/>
    public IService Service { get; set; }

    /// <inheritdoc cref="IServiceAuthorization.ApiClient"/>
    public IApiClient ApiClient { get; set; }

    /// <inheritdoc cref="IServiceAuthorization.AuthorizationType"/>
    public AuthorizationTypeEnum AuthorizationType { get; set; }

    /// <inheritdoc cref="IServiceAuthorization.Created"/>
    public DateTime Created { get; private set; }

    /// <inheritdoc cref="IServiceAuthorization.Updated"/>
    public DateTime Updated { get; private set; }

    private void UpdateProperties(HttpClientServiceAuthorizationModel data)
    {
        ID = data.Id;
        
        if (data.ServiceId != default(int))
        {
            var remoteService = _Telemetry.WrapSync(TelemetryType.http_client, _CallerName, "/v1/GetServiceByID", () => _ApiClient.GetServiceByID(data.ServiceId)).Data;

            Service = new Service(_Telemetry, _ApiClient, remoteService);
        }

        if (data.ApiClientId != default(int))
        {
            var remoteApiClient = _Telemetry.WrapSync(TelemetryType.http_client, _CallerName, "/v1/GetClientByID", () => _ApiClient.GetClientByID(data.ApiClientId)).Data;

            ApiClient = new ApiClient(_Telemetry, _ApiClient, remoteApiClient);
        }

        AuthorizationType = (AuthorizationTypeEnum)data.AuthorizationType;
        Created = data.Created;
        Updated = data.Updated;
    }

    /// <inheritdoc cref="IServiceAuthorization.IsDisabled"/>
    public bool IsDisabled() => AuthorizationType == AuthorizationTypeEnum.None;

    /// <inheritdoc cref="IService.Enable"/>
    public bool IsEnabled() 
        => AllowsFullServiceAccess() || HasOperationAuthorizations();

    /// <inheritdoc cref="IServiceAuthorization.HasOperationAuthorizations"/>
    public bool HasOperationAuthorizations() => AuthorizationType == AuthorizationTypeEnum.Partial;

    /// <inheritdoc cref="IServiceAuthorization.AllowsFullServiceAccess"/>
    public bool AllowsFullServiceAccess() => AuthorizationType == AuthorizationTypeEnum.Full;

    /// <inheritdoc cref="IServiceAuthorization.Delete"/>
    /// <inheritdoc cref="IApiClient.Delete"/>
    public void Delete()
    {
        _Telemetry.WrapSync(TelemetryType.http_client, _CallerName, "/v1/AddServiceAuthorization", () => _ApiClient.AddServiceAuthorization(
            new()
            {
                Key = ApiClient.ApiKey,
                ServiceName = Service.Name,
                AuthorizationType = HttpClientAuthorizationType.None
            }
        ));
    }

    /// <inheritdoc cref="IApiClient.Save"/>
    public void Save()
    {
        if (Service == null)
            throw new ApplicationException("ServiceAuthorization.Service must be set before saving.");
        if (ApiClient == null)
            throw new ApplicationException("ServiceAuthorization.ApiClient must be set before saving.");
        if (AuthorizationType == AuthorizationTypeEnum.None)
            throw new ApplicationException("ServiceAuthorization.AuthorizationType cannot be None, please use Delete!");

        if (ID == default(int))
            UpdateProperties(
                _Telemetry.WrapSync(TelemetryType.http_client, _CallerName, "/v1/AddServiceAuthorization", () => _ApiClient.AddServiceAuthorization(
                    new()
                    {
                        Key = ApiClient.ApiKey,
                        ServiceName = Service.Name,
                        AuthorizationType = (HttpClientAuthorizationType)AuthorizationType
                    }
                )).Data
            );
        else
            UpdateProperties(
                _Telemetry.WrapSync(TelemetryType.http_client, _CallerName, "/v1/AddServiceAuthorization", () => _ApiClient.AddServiceAuthorization(
                    new()
                    {
                        Key = ApiClient.ApiKey,
                        ServiceName = Service.Name,
                        AuthorizationType = (HttpClientAuthorizationType)AuthorizationType
                    }
                )).Data
            );
    }
}
