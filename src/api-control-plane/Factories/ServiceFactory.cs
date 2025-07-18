namespace Roblox.Api.ControlPlane.Factories;

using System;
using System.Linq;
using System.Collections.Generic;

using ApplicationTelemetry;

using Http.Client.ApiControlPlane;

using ServiceEntity = Entities.Service;
using Service = Service.ApiControlPlane.Service;
using IService = Service.ApiControlPlane.IService;

/// <inheritdoc cref="IServiceFactory"/>
public class ServiceFactory : IServiceFactory
{
    private const string _CallerName = "apicontrolplane-service";

    private readonly ITelemetry _Telemetry;
    private readonly IApiControlPlaneClient _ApiClient;

    /// <summary>
    /// Construct a new instance of <see cref="ServiceFactory"/>
    /// </summary>
    /// <param name="apiClient">The API Control Plane client.</param>
    /// <param name="telemetry">The API Control Plane telemetry.</param>
    public ServiceFactory(IApiControlPlaneClient apiClient = null, ITelemetry telemetry = null)
    {
        _ApiClient = apiClient;

        if (_ApiClient != null) _Telemetry = telemetry ?? new Telemetry();
    }

    /// <inheritdoc cref="IServiceFactory.CreateNew(string, bool)"/>
    public IService CreateNew(string name, bool isEnabled)
    {
        if (_ApiClient == null)
            return ServiceEntity.CreateNew(name, isEnabled);

        var service = new Service(_Telemetry, _ApiClient, null);
        service.Name = name;
        service.IsEnabled = isEnabled;

        service.Save();

        return service;
    }

    /// <inheritdoc cref="IServiceFactory.GetByID(int)"/>
    public IService GetByID(int id)
    {
        if (_ApiClient == null)
            return ServiceEntity.Get(id);

        var service = _Telemetry.WrapSync(TelemetryType.http_client, _CallerName, "/v1/GetServiceByID", () => _ApiClient.GetServiceByID(id)).Data;

        return new Service(_Telemetry, _ApiClient, service);
    }

    /// <inheritdoc cref="IServiceFactory.GetByName(string)"/>
    public IService GetByName(string name)
    {
        if (_ApiClient == null)
            return ServiceEntity.GetByName(name);

        var service = _Telemetry.WrapSync(TelemetryType.http_client, _CallerName, "/v1/GetServiceByName", () => _ApiClient.GetServiceByName(name)).Data;

        return new Service(_Telemetry, _ApiClient, service);
    }

    /// <inheritdoc cref="IServiceFactory.GetTotalNumber"/>
    public int GetTotalNumber()
    {
        if (_ApiClient == null)
            return ServiceEntity.GetTotalNumberOfServices();

        return _Telemetry.WrapSync(TelemetryType.http_client, _CallerName, "/v1/GetTotalNumberOfServices", () => _ApiClient.GetTotalNumberOfServices()).Data;
    }

    /// <inheritdoc cref="IServiceFactory.GetAll"/>
    public ICollection<IService> GetAll()
        => GetAll_Paged(1, GetTotalNumber());

    /// <inheritdoc cref="IServiceFactory.GetAll_Paged(int, int)"/>
    public ICollection<IService> GetAll_Paged(int startRowIndex, int maximumRows)
    {
        if (maximumRows < 1) return Array.Empty<IService>();
    
        if (_ApiClient == null)
            return ServiceEntity.GetServicesPaged(startRowIndex, maximumRows);

        var services = _Telemetry.WrapSync(TelemetryType.http_client, _CallerName, "/v1/GetServicesPaged", () => _ApiClient.GetServicesPaged(startRowIndex, maximumRows)).Data;

        return (from service in services select (new Service(_Telemetry, _ApiClient, service) as IService)).ToList();
    }
}
