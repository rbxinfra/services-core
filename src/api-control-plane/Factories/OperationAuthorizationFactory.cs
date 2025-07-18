namespace Roblox.Api.ControlPlane.Factories;

using System;
using System.Linq;
using System.Collections.Generic;

using ApplicationTelemetry;

using Service.ApiControlPlane;
using Http.Client.ApiControlPlane;

using IOperation = Service.ApiControlPlane.IOperation;
using IApiClient = Service.ApiControlPlane.IApiClient;
using OperationAuthorizationEntity = Entities.OperationAuthorization;
using IOperationAuthorization = Service.ApiControlPlane.IOperationAuthorization;
using HttpClientAuthorizationType = Http.Client.ApiControlPlane.AuthorizationType;
using ServiceOperationAuthorization = Service.ApiControlPlane.OperationAuthorization;

/// <inheritdoc cref="IOperationAuthorizationFactory"/>
public class OperationAuthorizationFactory : IOperationAuthorizationFactory
{
    private const string _CallerName = "apicontrolplane-service";

    private readonly ITelemetry _Telemetry;
    private readonly IApiControlPlaneClient _ApiClient;

    /// <summary>
    /// Construct a new instance of <see cref="OperationAuthorizationFactory"/>
    /// </summary>
    /// <param name="apiClient">The API Control Plane client.</param>
    /// <param name="telemetry">The API Control Plane telemetry.</param>
    public OperationAuthorizationFactory(IApiControlPlaneClient apiClient = null, ITelemetry telemetry = null)
    {
        _ApiClient = apiClient;

        if (_ApiClient != null) _Telemetry = telemetry ?? new Telemetry();
    }

    /// <inheritdoc cref="IOperationAuthorizationFactory.CreateNew(IApiClient, IOperation, AuthorizationTypeEnum)"/>
    public IOperationAuthorization CreateNew(IApiClient apiClient, IOperation operation, AuthorizationTypeEnum authorizationType)
    {
        if (apiClient == null)
            throw new ArgumentNullException(nameof(apiClient));
        if (operation == null)
            throw new ArgumentNullException(nameof(operation));

        if (_ApiClient == null)
            return OperationAuthorizationEntity.CreateNew(operation.ID, apiClient.ID, (byte)authorizationType);

        var operationAuthorization = _Telemetry.WrapSync(
            TelemetryType.http_client, 
            _CallerName,
            "/v1/AddOperationAuthorization",
            () => _ApiClient.AddOperationAuthorization(
                new()
                {
                    Key = apiClient.ApiKey,
                    ServiceName = operation.Service.Name,
                    OperationName = operation.Name,
                    AuthorizationType = (HttpClientAuthorizationType)authorizationType
                }
            )
        ).Data;

        return new ServiceOperationAuthorization(_Telemetry, _ApiClient, operationAuthorization);
    }

    /// <inheritdoc cref="IOperationAuthorizationFactory.GetByID(int)"/>
    public IOperationAuthorization GetByID(int id)
    {
        if (_ApiClient == null)
            return OperationAuthorizationEntity.Get(id);

        throw new NotImplementedException();
    }

    /// <inheritdoc cref="IOperationAuthorizationFactory.GetByApiClientAndOperation(IApiClient, IOperation)"/>
    public IOperationAuthorization GetByApiClientAndOperation(IApiClient apiClient, IOperation operation)
    {
        if (apiClient == null)
            throw new ArgumentNullException(nameof(apiClient));
        if (operation == null)
            throw new ArgumentNullException(nameof(operation));

        if (_ApiClient == null)
            return OperationAuthorizationEntity.GetByOperationIDAndApiClientID(operation.ID, apiClient.ID);

        throw new NotImplementedException();
    }

    /// <inheritdoc cref="IOperationAuthorizationFactory.GetTotalNumberByOperation(IOperation)"/>
    public int GetTotalNumberByOperation(IOperation operation)
    {
        if (operation == null)
            throw new ArgumentNullException(nameof(operation));

        if (_ApiClient == null)
            return OperationAuthorizationEntity.GetTotalNumberOfOperationAuthorizationsByOperationID(operation.ID);

        return _Telemetry.WrapSync(
            TelemetryType.http_client, 
            _CallerName,
            "/v1/GetTotalNumberOfOperationAuthorizationsByOperation", 
            () => _ApiClient.GetTotalNumberOfOperationAuthorizationsByOperation(operation.Service.Name, operation.Name)
        ).Data;
    }

    /// <inheritdoc cref="IOperationAuthorizationFactory.GetAllByOperation(IOperation)"/>
    public ICollection<IOperationAuthorization> GetAllByOperation(IOperation operation)
        => GetAllByOperation_Paged(operation, 1, GetTotalNumberByOperation(operation));

    /// <inheritdoc cref="IOperationAuthorizationFactory.GetAllByOperation_Paged(IOperation, int, int)"/>
    public ICollection<IOperationAuthorization> GetAllByOperation_Paged(IOperation operation, int startRowIndex, int maximumRows)
    {
        if (maximumRows < 1) return Array.Empty<IOperationAuthorization>();
    
        if (operation == null)
            throw new ArgumentNullException(nameof(operation));

        if (_ApiClient == null)
            return OperationAuthorizationEntity.GetOperationAuthorizationsByOperationIDPaged(operation.ID, startRowIndex, maximumRows);

        var operationAuthorizations = _Telemetry.WrapSync(
            TelemetryType.http_client, 
            _CallerName,
            "/v1/GetOperationAuthorizationsByOperationPaged",
            () => _ApiClient.GetOperationAuthorizationsByOperationPaged(operation.Service.Name, operation.Name, startRowIndex, maximumRows)
        ).Data;

        return (
             from authorization in operationAuthorizations
             select (new ServiceOperationAuthorization(_Telemetry, _ApiClient, authorization) as IOperationAuthorization)
        ).ToList();
    }

    /// <inheritdoc cref="IOperationAuthorizationFactory.GetTotalNumberByApiClient(IApiClient)"/>
    public int GetTotalNumberByApiClient(IApiClient apiClient)
    {
        if (apiClient == null)
            throw new ArgumentNullException(nameof(apiClient));

        if (_ApiClient == null)
            return OperationAuthorizationEntity.GetTotalNumberOfOperationAuthorizationsByApiClientID(apiClient.ID);

        return _Telemetry.WrapSync(
            TelemetryType.http_client,
            _CallerName,
            "/v1/GetTotalNumberOfOperationAuthorizationsByClient",
            () => _ApiClient.GetTotalNumberOfOperationAuthorizationsByClient(apiClient.ApiKey)
        ).Data;
    }

    /// <inheritdoc cref="IOperationAuthorizationFactory.GetAllByApiClient(IApiClient)"/>
    public ICollection<IOperationAuthorization> GetAllByApiClient(IApiClient apiClient)
        => GetAllByApiClient_Paged(apiClient, 1, GetTotalNumberByApiClient(apiClient));

    /// <inheritdoc cref="IOperationAuthorizationFactory.GetAllByApiClient_Paged(IApiClient, int, int)"/>
    public ICollection<IOperationAuthorization> GetAllByApiClient_Paged(IApiClient apiClient, int startRowIndex, int maximumRows)
    {
        if (maximumRows < 1) return Array.Empty<IOperationAuthorization>();
    
        if (apiClient == null)
            throw new ArgumentNullException(nameof(apiClient));

        if (_ApiClient == null)
            return OperationAuthorizationEntity.GetOperationAuthorizationsByApiClientIDPaged(apiClient.ID, startRowIndex, maximumRows);

        var operationAuthorizations = _Telemetry.WrapSync(
            TelemetryType.http_client, 
            _CallerName,
            "/v1/GetOperationAuthorizationsByClientPaged",
            () => _ApiClient.GetOperationAuthorizationsByClientPaged(apiClient.ApiKey, startRowIndex, maximumRows)
        ).Data;

        return (
             from authorization in operationAuthorizations
             select (new ServiceOperationAuthorization(_Telemetry, _ApiClient, authorization) as IOperationAuthorization)
        ).ToList();
    }
}
