namespace Roblox.Api.ControlPlane.Factories;

using System;
using System.Linq;
using System.Collections.Generic;

using ApplicationTelemetry;

using Http.Client.ApiControlPlane;

using ApiClientEntity = Entities.ApiClient;
using IApiClient = Service.ApiControlPlane.IApiClient;
using ServiceApiClient = Service.ApiControlPlane.ApiClient;

/// <inheritdoc cref="IApiClientFactory"/>
public class ApiClientFactory : IApiClientFactory
{
    private const string _CallerName = "apicontrolplane-service";

    private readonly ITelemetry _Telemetry;
    private readonly IApiControlPlaneClient _ApiClient;

    /// <summary>
    /// Construct a new instance of <see cref="ApiClientFactory"/>
    /// </summary>
    /// <param name="apiClient">The API Control Plane client.</param>
    /// <param name="telemetry">The API Control Plane telemetry.</param>
    public ApiClientFactory(IApiControlPlaneClient apiClient = null, ITelemetry telemetry = null)
    {
        _ApiClient = apiClient;

        if (_ApiClient != null) _Telemetry = telemetry ?? new Telemetry();
    }

    /// <inheritdoc cref="IApiClientFactory.CreateNew(Guid?, string, bool)"/>
    public IApiClient CreateNew(Guid? apiKey, string note = null, bool isValid = true)
    {
        if (_ApiClient == null)
            return ApiClientEntity.CreateNew(apiKey ?? Guid.NewGuid(), note, isValid);

        var apiClient = new ServiceApiClient(_Telemetry, _ApiClient, null);

        apiClient.ApiKey = apiKey ?? Guid.NewGuid();
        apiClient.Note = note;
        apiClient.IsValid = isValid;

        apiClient.Save();

        return apiClient;
    }

    /// <inheritdoc cref="IApiClientFactory.GetByID(int)"/>
    public IApiClient GetByID(int id)
    {
        if (_ApiClient == null)
            return ApiClientEntity.Get(id);

        var apiClient = _Telemetry.WrapSync(TelemetryType.http_client, _CallerName, "/v1/GetClientByID", () => _ApiClient.GetClientByID(id)).Data;

        return new ServiceApiClient(_Telemetry, _ApiClient, apiClient);
    }

    /// <inheritdoc cref="IApiClientFactory.GetByKey(Guid)"/>
    public IApiClient GetByKey(Guid key)
    {
        if (_ApiClient == null)
            return ApiClientEntity.GetByApiKey(key);

        var apiClient = _Telemetry.WrapSync(TelemetryType.http_client, _CallerName, "/v1/GetClientByKey", () => _ApiClient.GetClientByKey(key)).Data;

        return new ServiceApiClient(_Telemetry, _ApiClient, apiClient);
    }

    /// <inheritdoc cref="IApiClientFactory.GetByNote(string)"/>
    public IApiClient GetByNote(string note)
    {
        if (_ApiClient == null)
            return ApiClientEntity.GetByNote(note);

        var apiClient = _Telemetry.WrapSync(TelemetryType.http_client, _CallerName, "/v1/GetClientByNote", () => _ApiClient.GetClientByNote(note)).Data;

        return new ServiceApiClient(_Telemetry, _ApiClient, apiClient);
    }

    /// <inheritdoc cref="IApiClientFactory.GetTotalNumber"/>
    public int GetTotalNumber()
    {
        if (_ApiClient == null)
            return ApiClientEntity.GetTotalNumberOfApiClients();

        return _Telemetry.WrapSync(TelemetryType.http_client, _CallerName, "/v1/GetTotalNumberOfClients", () => _ApiClient.GetTotalNumberOfClients()).Data;
    }

    /// <inheritdoc cref="IApiClientFactory.GetAll"/>
    public ICollection<IApiClient> GetAll()
        => GetAll_Paged(1, GetTotalNumber());

    /// <inheritdoc cref="IApiClientFactory.GetAll_Paged(int, int)"/>
    public ICollection<IApiClient> GetAll_Paged(int startRowIndex, int maximumRows)
    {
        if (maximumRows < 1) return Array.Empty<IApiClient>();
    
        if (_ApiClient == null)
            return ApiClientEntity.GetApiClientsPaged(startRowIndex, maximumRows);

        var apiClients = _Telemetry.WrapSync(TelemetryType.http_client, _CallerName, "/v1/GetClientByNote", () => _ApiClient.GetClientsPaged(startRowIndex, maximumRows)).Data;

        return (from client in apiClients select (new ServiceApiClient(_Telemetry, _ApiClient, client) as IApiClient)).ToList();
    }
}
