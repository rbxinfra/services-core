namespace Roblox.Service.ApiControlPlane;

using System;

using ApplicationTelemetry;

using Http.Client.ApiControlPlane;

using HttpClientOperationModel = Http.Client.ApiControlPlane.OperationModel;
using HttpClientApiClientModel = Http.Client.ApiControlPlane.ApiClientModel;
using HttpClientAuthorizationModel = Http.Client.ApiControlPlane.AuthorizationModel;

/// <inheritdoc cref="IAuthorization"/>
public class Authorization : IAuthorization
{
    private const string _CallerName = "apicontrolplane-service";

    /// <summary>
    /// Construct a new instance of <see cref="Authorization"/>.
    /// </summary>
    /// <param name="serviceAuthorization">The service authorization.</param>
    /// <exception cref="ArgumentNullException"><paramref name="serviceAuthorization"/> cannot be null.</exception>
    public Authorization(IServiceAuthorization serviceAuthorization)
    {
        if (serviceAuthorization == null)
            throw new ArgumentNullException(nameof(serviceAuthorization));

        ServiceName = serviceAuthorization.Service.Name;
        ApiClient = serviceAuthorization.ApiClient;
        Operation = null;
        AuthorizationType = serviceAuthorization.AuthorizationType;
    }

    /// <summary>
    /// Construct a new instance of <see cref="Authorization"/>.
    /// </summary>
    /// <param name="operationAuthorization">The operation authorization.</param>
    /// <exception cref="ArgumentNullException"><paramref name="operationAuthorization"/> cannot be null.</exception>
    public Authorization(IOperationAuthorization operationAuthorization)
    {
        if (operationAuthorization == null)
            throw new ArgumentNullException(nameof(operationAuthorization));

        ServiceName = operationAuthorization.Operation.Service.Name;
        ApiClient = operationAuthorization.ApiClient;
        Operation = operationAuthorization.Operation;
        AuthorizationType = operationAuthorization.AuthorizationType;
    }

    /// <summary>
    /// Internal constructor used by service registration.
    /// </summary>
    /// <param name="initialData">The initial service client model fetched from the service. If null it is initalized as a default client.</param>
    /// <exception cref="ArgumentNullException"><paramref name="initialData"/> cannot be null.</exception>
    internal Authorization(HttpClientAuthorizationModel initialData)
    {
        if (initialData == null) throw new ArgumentNullException(nameof(initialData));

        ServiceName = initialData.ServiceName;
        ApiClient = new ApiClient(new HttpClientApiClientModel { Id = initialData.ApiClientId, Note = initialData.ApiClientNote });

        if (initialData.OperationName != null) Operation = new Operation(new HttpClientOperationModel { Name = initialData.OperationName });

        AuthorizationType = (AuthorizationTypeEnum)initialData.AuthorizationType;
    }

    /// <summary>
    /// Construct a new instance of <see cref="Authorization"/>
    /// </summary>
    /// <param name="telemetry">The <see cref="ITelemetry"/>.</param>
    /// <param name="apiClient">The <see cref="IApiControlPlaneClient"/>.</param>
    /// <param name="authorization">The authorization from service.</param>
    /// <exception cref="ArgumentNullException">
    /// - <paramref name="telemetry"/> cannot be null.
    /// - <paramref name="apiClient"/> cannot be null.
    /// - <paramref name="authorization"/> cannot be null.
    /// </exception>
    /// <exception cref="ApplicationException">
    /// - Service returned null client. This is NOT normal.
    /// - Service returned null operation. This is NOT normal.
    /// </exception>
    public Authorization(ITelemetry telemetry, IApiControlPlaneClient apiClient, HttpClientAuthorizationModel authorization)
    {
        if (telemetry == null) throw new ArgumentNullException(nameof(telemetry));
        if (apiClient == null) throw new ArgumentNullException(nameof(apiClient));
        if (authorization == null) throw new ArgumentNullException(nameof(authorization));

        ServiceName = authorization.ServiceName;

        var client = telemetry.WrapSync(_CallerName, "/v1/GetClientByID", () => apiClient.GetClientByID(authorization.ApiClientId)).Data;
        if (client == null) throw new ApplicationException("Service returned null client. This is NOT normal.");

        ApiClient = new ApiClient(telemetry, apiClient, client);

        var operation = telemetry.WrapSync(_CallerName, "/v1/GetOperationByService", () => apiClient.GetOperationByService(authorization.ServiceName, authorization.OperationName)).Data;
        if (operation == null) throw new ApplicationException("Service returned null operation. This is NOT normal.");

        Operation = new Operation(telemetry, apiClient, operation);

        AuthorizationType = (AuthorizationTypeEnum)authorization.AuthorizationType;
    }

    /// <inheritdoc cref="IAuthorization.ServiceName"/>
    public string ServiceName { get; private set; }

    /// <inheritdoc cref="IAuthorization.ApiClient"/>
    public IApiClient ApiClient { get; private set; }

    /// <inheritdoc cref="IAuthorization.Operation"/>
    public IOperation Operation { get; private set; }

    /// <inheritdoc cref="IAuthorization.AuthorizationType"/>
    public AuthorizationTypeEnum AuthorizationType { get; private set; }

    /// <inheritdoc cref="IAuthorization.IsOperationAuthorization"/>
    public bool IsOperationAuthorization() => Operation != null;

    /// <inheritdoc cref="IAuthorization.IsServiceAuthorization"/>
    public bool IsServiceAuthorization() => Operation == null;
}
