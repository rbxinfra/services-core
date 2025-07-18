namespace Roblox.Api.ControlPlane;

using System;

using Service.ApiControlPlane;

using Service = Entities.Service;
using ApiClient = Entities.ApiClient;
using Operation = Entities.Operation;
using ServiceAuthorization = Entities.ServiceAuthorization;
using OperationAuthorization = Entities.OperationAuthorization;

/// <summary>
/// <see cref="IAuthority"/> instance for direct MSSQL access.
/// </summary>
public class Authority : IAuthority
{
    /// <inheritdoc cref="IAuthority.Authenticate(Guid)" />
    public bool Authenticate(Guid apiKey)
    {
        var apiClient = ApiClient.GetByApiKey(apiKey);

        return apiClient != null && apiClient.IsValid;
    }

    /// <inheritdoc cref="IAuthority.IsAuthorized(Guid, string, string, out IApiClient)"/>
    public bool IsAuthorized(Guid apiKey, string serviceName, string operationName, out IApiClient apiClient)
    {
        apiClient = ApiClient.GetByApiKey(apiKey);
        if (apiClient == null || !apiClient.IsValid) return false;

        var service = Service.GetByName(serviceName);
        if (service == null)
        {
            Service.CreateNew(serviceName, true);
            return false;
        }

        if (!service.IsEnabled) return false;

        var serviceAuthorization = ServiceAuthorization.GetByServiceIDAndApiClientID(service.ID, apiClient.ID);
        if (serviceAuthorization == null || !serviceAuthorization.IsEnabled()) return false;

        if (serviceAuthorization.AllowsFullServiceAccess()) return true;

        // The only other ID it can be is Partial

        var operation = Operation.GetByServiceIDAndName(service.ID, operationName);
        if (operation == null)
        {
            Operation.CreateNew(operationName, service.ID, true);
            return false;
        }

        if (!operation.IsEnabled) return false;

        var operationAuthorization = OperationAuthorization.GetByOperationIDAndApiClientID(operation.ID, apiClient.ID);
        if (operationAuthorization == null || !operationAuthorization.IsEnabled()) return false;

        // The only other IDs it can be is Partial or Full
        // TODO: See if partial in this context is actually something or if it can do anything special

        return true;
    }

    /// <inheritdoc cref="IAuthority.IsAuthorized(Guid, string, out IApiClient)"/>
    public bool IsAuthorized(Guid apiKey, string serviceName, out IApiClient apiClient)
    {
        apiClient = ApiClient.GetByApiKey(apiKey);
        if (apiClient == null || !apiClient.IsValid) return false;


        var service = Service.GetByName(serviceName);
        if (service == null)
        {
            Service.CreateNew(serviceName, true);
            return false;
        }

        if (!service.IsEnabled) return false;

        var serviceAuthorization = ServiceAuthorization.GetByServiceIDAndApiClientID(service.ID, apiClient.ID);
        if (serviceAuthorization == null || !serviceAuthorization.IsEnabled()) return false;

        return true;
    }

    /// <inheritdoc cref="IAuthority.OperationIsEnabled(string, string)"/>
    public bool OperationIsEnabled(string serviceName, string operationName)
    {
        var service = Service.GetByName(serviceName);
        if (service == null)
        {
            Service.CreateNew(serviceName, true);
            return false;
        }
        if (!service.IsEnabled) return false;

        var operation = Operation.GetByServiceIDAndName(service.ID, operationName);
        if (operation == null || !operation.IsEnabled)
        {
            Operation.CreateNew(operationName, service.ID, true);
            return true;
        }
        return operation.IsEnabled;
    }

    /// <inheritdoc cref="IAuthority.ServiceIsEnabled(string)"/>
    public bool ServiceIsEnabled(string serviceName)
    {
        var service = Service.GetByName(serviceName);
        if (service == null)
        {
            Service.CreateNew(serviceName, true);
            return true;
        }
        return service.IsEnabled;
    }
}
