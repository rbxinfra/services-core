namespace Roblox.Api.ControlPlane.Factories;

using System;
using System.Linq;
using System.Collections.Generic;

using ApplicationTelemetry;

using Http.Client.ApiControlPlane;

using OperationEntity = Entities.Operation;
using IService = Service.ApiControlPlane.IService;
using IOperation = Service.ApiControlPlane.IOperation;
using ServiceOperation = Service.ApiControlPlane.Operation;

/// <inheritdoc cref="IOperationFactory"/>
public class OperationFactory : IOperationFactory
{
    private const string _CallerName = "apicontrolplane-service";

    private readonly ITelemetry _Telemetry;
    private readonly IApiControlPlaneClient _ApiClient;

    /// <summary>
    /// Construct a new instance of <see cref="OperationFactory"/>
    /// </summary>
    /// <param name="apiClient">The API Control Plane client.</param>
    /// <param name="telemetry">The API Control Plane telemetry.</param>
    public OperationFactory(IApiControlPlaneClient apiClient = null, ITelemetry telemetry = null)
    {
        _ApiClient = apiClient;

        if (_ApiClient != null) _Telemetry = telemetry ?? new Telemetry();
    }

    /// <inheritdoc cref="IOperationFactory.CreateNew(string, IService, bool)"/>
    public IOperation CreateNew(string name, IService service, bool isEnabled)
    {
        if (service == null)
            throw new ArgumentNullException(nameof(service));

        if (_ApiClient == null)
            return OperationEntity.CreateNew(name, service.ID, isEnabled);

        var operation = _Telemetry.WrapSync(
            TelemetryType.http_client, 
            _CallerName,
            "/v1/AddOperation",
            () => _ApiClient.AddOperation(new() { Name = name, ServiceName = service.Name, IsEnabled = isEnabled })
        ).Data;

        return new ServiceOperation(_Telemetry, _ApiClient, operation);
    }

    /// <inheritdoc cref="IOperationFactory.GetByID(int)"/>
    public IOperation GetByID(int id)
    {
        if (_ApiClient == null)
            return OperationEntity.Get(id);

        var operation = _Telemetry.WrapSync(TelemetryType.http_client, _CallerName, "/v1/GetOperationByID", () => _ApiClient.GetOperationByID(id)).Data;

        return new ServiceOperation(_Telemetry, _ApiClient, operation);
    }

    /// <inheritdoc cref="IOperationFactory.GetByName(IService, string)"/>
    public IOperation GetByName(IService service, string name)
    {
        if (service == null)
            throw new ArgumentNullException(nameof(service));

        if (_ApiClient == null)
            return OperationEntity.GetByServiceIDAndName(service.ID, name);

        var operation = _Telemetry.WrapSync(TelemetryType.http_client, _CallerName, "/v1/GetOperationByService", () => _ApiClient.GetOperationByService(service.Name, name)).Data;

        return new ServiceOperation(_Telemetry, _ApiClient, operation);
    }

    /// <inheritdoc cref="IOperationFactory.GetTotalNumberByService(IService)"/>
    public int GetTotalNumberByService(IService service)
    {
        if (service == null)
            throw new ArgumentNullException(nameof(service));

        if (_ApiClient == null)
            return OperationEntity.GetTotalNumberOfOperationsByServiceID(service.ID);

        return _Telemetry.WrapSync(
            TelemetryType.http_client,
            _CallerName,
            "/v1/GetTotalNumberOfOperationsByService",
            () => _ApiClient.GetTotalNumberOfOperationsByService(service.Name)
        ).Data;
    }

    /// <inheritdoc cref="IOperationFactory.GetAllByService(IService)"/>
    public ICollection<IOperation> GetAllByService(IService service)
        => GetAllByService_Paged(service, 1, GetTotalNumberByService(service));

    /// <inheritdoc cref="IOperationFactory.GetAllByService_Paged(IService, int, int)"/>
    public ICollection<IOperation> GetAllByService_Paged(IService service, int startRowIndex, int maximumRows)
    {
        if (maximumRows < 1) return Array.Empty<IOperation>();    

        if (service == null)
            throw new ArgumentNullException(nameof(service));

        if (_ApiClient == null)
            return OperationEntity.GetOperationsByServiceIDPaged(service.ID, startRowIndex, maximumRows);

        var operations = _Telemetry.WrapSync(TelemetryType.http_client, _CallerName, "/v1/GetOperationByService", () => _ApiClient.GetOperationsByServicePaged(service.Name, startRowIndex, maximumRows)).Data;

        return (
            from operation in operations
            select (new ServiceOperation(_Telemetry, _ApiClient, operation) as IOperation)
        ).ToList();
    }
}
