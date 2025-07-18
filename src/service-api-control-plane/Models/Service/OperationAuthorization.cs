namespace Roblox.Service.ApiControlPlane;

using System;

using ApplicationTelemetry;

using Http.Client.ApiControlPlane;

using HttpClientAuthorizationType = Http.Client.ApiControlPlane.AuthorizationType;
using HttpClientOperationAuthorizationModel = Http.Client.ApiControlPlane.OperationAuthorizationModel;

/// <inheritdoc cref="IApiClient"/>
internal class OperationAuthorization : IOperationAuthorization
{
    private const string _CallerName = "apicontrolplane-service";

    private readonly ITelemetry _Telemetry;
    private readonly IApiControlPlaneClient _ApiClient;

    /// <summary>
    /// Construct a new instance of <see cref="OperationAuthorization"/>
    /// </summary>
    /// <param name="telemetry">A telemetry to wrap requests.</param>
    /// <param name="apiClient">The service client to call on API Control Plane.</param>
    /// <param name="initialData">The initial service client model fetched from the service. If null it is initalized as a default client.</param>
    /// <exception cref="ArgumentNullException">
    /// - <paramref name="telemetry"/> cannot be null.
    /// - <paramref name="apiClient"/> cannot be null.
    /// </exception>
    public OperationAuthorization(ITelemetry telemetry, IApiControlPlaneClient apiClient, HttpClientOperationAuthorizationModel initialData)
    {
        initialData ??= new HttpClientOperationAuthorizationModel();

        _Telemetry = telemetry ?? throw new ArgumentNullException(nameof(telemetry));
        _ApiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));

        UpdateProperties(initialData);
    }

    /// <inheritdoc cref="IOperationAuthorization.ID"/>
    public int ID { get; private set; }

    /// <inheritdoc cref="IOperationAuthorization.Operation"/>
    public IOperation Operation { get; set; }

    /// <inheritdoc cref="IOperationAuthorization.ApiClient"/>
    public IApiClient ApiClient { get; set; }

    /// <inheritdoc cref="IOperationAuthorization.AuthorizationType"/>
    public AuthorizationTypeEnum AuthorizationType { get; set; }

    /// <inheritdoc cref="IOperationAuthorization.Created"/>
    public DateTime Created { get; private set; }

    /// <inheritdoc cref="IOperationAuthorization.Updated"/>
    public DateTime Updated { get; private set; }

    private void UpdateProperties(HttpClientOperationAuthorizationModel data)
    {
        ID = data.Id;

        if (data.OperationId != default(int))
        {
            var remoteOperation = _Telemetry.WrapSync(TelemetryType.http_client, _CallerName, "/v1/GetOperationByID", () => _ApiClient.GetOperationByID(data.OperationId)).Data;

            Operation = new Operation(_Telemetry, _ApiClient, remoteOperation);
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

    /// <inheritdoc cref="IOperationAuthorization.IsDisabled"/>
    public bool IsDisabled() => AuthorizationType == AuthorizationTypeEnum.None;

    /// <inheritdoc cref="IOperationAuthorization.IsEnabled"/>
    public bool IsEnabled()
        => AuthorizationType == AuthorizationTypeEnum.Full || AuthorizationType == AuthorizationTypeEnum.Partial;

    /// <inheritdoc cref="IOperationAuthorization.IsOwnedByService"/>
    public bool IsOwnedByService(IService service) => Operation.Service.ID == service.ID;

    /// <inheritdoc cref="IApiClient.Delete"/>
    public void Delete()
    {
        _Telemetry.WrapSync(TelemetryType.http_client, _CallerName, "/v1/AddOperationAuthorization", () => _ApiClient.AddOperationAuthorization(
            new()
            {
                Key = ApiClient.ApiKey,
                ServiceName = Operation.Service.Name,
                OperationName = Operation.Name,
                AuthorizationType = HttpClientAuthorizationType.None
            }
        ));
    }

    /// <inheritdoc cref="IApiClient.Save"/>
    public void Save()
    {
        if (Operation == null)
            throw new ApplicationException("OperationAuthorization.Operation must be set before saving.");
        if (ApiClient == null)
            throw new ApplicationException("OperationAuthorization.ApiClient must be set before saving.");
        if (AuthorizationType == AuthorizationTypeEnum.None)
            throw new ApplicationException("OperationAuthorization.AuthorizationType cannot be None, please use Delete!");

        if (ID == default(int))
            UpdateProperties(
                _Telemetry.WrapSync(TelemetryType.http_client, _CallerName, "/v1/AddOperationAuthorization", () => _ApiClient.AddOperationAuthorization(
                    new()
                    {
                        Key = ApiClient.ApiKey,
                        ServiceName = Operation.Service.Name,
                        OperationName = Operation.Name,
                        AuthorizationType = (HttpClientAuthorizationType)AuthorizationType
                    }
                )).Data
            );
        else
            UpdateProperties(
                _Telemetry.WrapSync(TelemetryType.http_client, _CallerName, "/v1/UpdateOperationAuthorization", () => _ApiClient.AddOperationAuthorization(
                    new()
                    {
                        Key = ApiClient.ApiKey,
                        ServiceName = Operation.Service.Name,
                        OperationName = Operation.Name,
                        AuthorizationType = (HttpClientAuthorizationType)AuthorizationType
                    }
                )).Data
            );
    }
}
