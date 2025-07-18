namespace Roblox.Api.ControlPlane.Factories;

using System;
using System.Linq;
using System.Collections.Generic;

using ApplicationTelemetry;

using Service.ApiControlPlane;
using Http.Client.ApiControlPlane;

using IService = Service.ApiControlPlane.IService;
using IApiClient = Service.ApiControlPlane.IApiClient;
using ServiceAuthorizationEntity = Entities.ServiceAuthorization;
using IServiceAuthorization = Service.ApiControlPlane.IServiceAuthorization;
using HttpClientAuthorizationType = Http.Client.ApiControlPlane.AuthorizationType;
using ServiceServiceAuthorization = Service.ApiControlPlane.ServiceAuthorization;

/// <inheritdoc cref="IServiceAuthorizationFactory"/>
public class ServiceAuthorizationFactory : IServiceAuthorizationFactory
{
    private const string _CallerName = "apicontrolplane-service";

    private readonly ITelemetry _Telemetry;
    private readonly IApiControlPlaneClient _ApiClient;

    /// <summary>
    /// Construct a new instance of <see cref="ServiceAuthorizationFactory"/>
    /// </summary>
    /// <param name="apiClient">The API Control Plane client.</param>
    /// <param name="telemetry">The API Control Plane telemetry.</param>
    public ServiceAuthorizationFactory(IApiControlPlaneClient apiClient = null, ITelemetry telemetry = null)
    {
        _ApiClient = apiClient;

        if (_ApiClient != null) _Telemetry = telemetry ?? new Telemetry();
    }

    /// <inheritdoc cref="IServiceAuthorizationFactory.CreateNew(IApiClient, IService, AuthorizationTypeEnum)"/>
    public IServiceAuthorization CreateNew(IApiClient apiClient, IService service, AuthorizationTypeEnum authorizationType)
    {
        if (apiClient == null)
            throw new ArgumentNullException(nameof(apiClient));
        if (service == null)
            throw new ArgumentNullException(nameof(service));

        if (_ApiClient == null)
            return ServiceAuthorizationEntity.CreateNew(service.ID, apiClient.ID, (byte)authorizationType);

        var serviceAuthorization = _Telemetry.WrapSync(
            TelemetryType.http_client, 
            _CallerName,
            "/v1/AddServiceAuthorization",
            () => _ApiClient.AddServiceAuthorization(
                new()
                {
                    Key = apiClient.ApiKey,
                    ServiceName = service.Name,
                    AuthorizationType = (HttpClientAuthorizationType)authorizationType
                }
            )
        ).Data;

        return new ServiceServiceAuthorization(_Telemetry, _ApiClient, serviceAuthorization);
    }

    /// <inheritdoc cref="IServiceAuthorizationFactory.GetByID(int)"/>
    public IServiceAuthorization GetByID(int id)
    {
        if (_ApiClient == null)
            return ServiceAuthorizationEntity.Get(id);

        throw new NotImplementedException();
    }

    /// <inheritdoc cref="IServiceAuthorizationFactory.GetByApiClientAndService(IApiClient, IService)"/>
    public IServiceAuthorization GetByApiClientAndService(IApiClient apiClient, IService service)
    {
        if (apiClient == null)
            throw new ArgumentNullException(nameof(apiClient));
        if (service == null)
            throw new ArgumentNullException(nameof(service));

        if (_ApiClient == null)
            return ServiceAuthorizationEntity.GetByServiceIDAndApiClientID(service.ID, apiClient.ID);

        throw new NotImplementedException();
    }

    /// <inheritdoc cref="IServiceAuthorizationFactory.GetTotalNumberByService(IService)"/>
    public int GetTotalNumberByService(IService service)
    {
        if (service == null)
            throw new ArgumentNullException(nameof(service));

        if (_ApiClient == null)
            return ServiceAuthorizationEntity.GetTotalNumberOfServiceAuthorizationsByServiceID(service.ID);

        return _Telemetry.WrapSync(
            TelemetryType.http_client,
            _CallerName,
            "/v1/GetTotalNumberOfServiceAuthorizationsByService",
            () => _ApiClient.GetTotalNumberOfServiceAuthorizationsByService(service.Name)
        ).Data;
    }

    /// <inheritdoc cref="IServiceAuthorizationFactory.GetAllByService(IService)"/>
    public ICollection<IServiceAuthorization> GetAllByService(IService service)
        => GetAllByService_Paged(service, 1, GetTotalNumberByService(service));

    /// <inheritdoc cref="IServiceAuthorizationFactory.GetAllByService_Paged(IService, int, int)"/>
    public ICollection<IServiceAuthorization> GetAllByService_Paged(IService service, int startRowIndex, int maximumRows)
    {
        if (maximumRows < 1) return Array.Empty<IServiceAuthorization>();
    
        if (service == null)
            throw new ArgumentNullException(nameof(service));

        if (_ApiClient == null)
            return ServiceAuthorizationEntity.GetServiceAuthorizationsByServiceIDPaged(service.ID, startRowIndex, maximumRows);

        var serviceAuthorizations = _Telemetry.WrapSync(
            TelemetryType.http_client, 
            _CallerName,
            "/v1/GetServiceAuthorizationsByServicePaged",
            () => _ApiClient.GetServiceAuthorizationsByServicePaged(service.Name, startRowIndex, maximumRows)
        ).Data;

        return (
             from authorization in serviceAuthorizations
             select (new ServiceServiceAuthorization(_Telemetry, _ApiClient, authorization) as IServiceAuthorization)
        ).ToList();
    }

    /// <inheritdoc cref="IServiceAuthorizationFactory.GetTotalNumberByApiClient(IApiClient)"/>
    public int GetTotalNumberByApiClient(IApiClient apiClient)
    {
        if (apiClient == null)
            throw new ArgumentNullException(nameof(apiClient));

        if (_ApiClient == null)
            return ServiceAuthorizationEntity.GetTotalNumberOfServiceAuthorizationsByApiClientID(apiClient.ID);

        return _Telemetry.WrapSync(
            TelemetryType.http_client,
            _CallerName,
            "/v1/GetTotalNumberOfServiceAuthorizationsByClient",
            () => _ApiClient.GetTotalNumberOfServiceAuthorizationsByClient(apiClient.ApiKey)
        ).Data;
    }

    /// <inheritdoc cref="IServiceAuthorizationFactory.GetAllByApiClient(IApiClient)"/>
    public ICollection<IServiceAuthorization> GetAllByApiClient(IApiClient apiClient)
        => GetAllByApiClient_Paged(apiClient, 1, GetTotalNumberByApiClient(apiClient));

    /// <inheritdoc cref="IServiceAuthorizationFactory.GetAllByApiClient_Paged(IApiClient, int, int)"/>
    public ICollection<IServiceAuthorization> GetAllByApiClient_Paged(IApiClient apiClient, int startRowIndex, int maximumRows)
    {
        if (maximumRows < 1) return Array.Empty<IServiceAuthorization>();
    
        if (apiClient == null)
            throw new ArgumentNullException(nameof(apiClient));

        if (_ApiClient == null)
            return ServiceAuthorizationEntity.GetServiceAuthorizationsByApiClientIDPaged(apiClient.ID, startRowIndex, maximumRows);

        var serviceAuthorizations = _Telemetry.WrapSync(
            TelemetryType.http_client, 
            _CallerName,
            "/v1/GetServiceAuthorizationsByClientPaged",
            () => _ApiClient.GetServiceAuthorizationsByClientPaged(apiClient.ApiKey, startRowIndex, maximumRows)
        ).Data;

        return (
             from authorization in serviceAuthorizations
             select (new ServiceServiceAuthorization(_Telemetry, _ApiClient, authorization) as IServiceAuthorization)
        ).ToList();
    }
}
