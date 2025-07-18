namespace Roblox.Api.ControlPlane.Factories;

using System;
using System.Linq;
using System.Collections.Generic;

using ApplicationTelemetry;
using Service.ApiControlPlane;
using Http.Client.ApiControlPlane;

using IApiClient = Service.ApiControlPlane.IApiClient;
using ServiceApiClient = Service.ApiControlPlane.ApiClient;
using ServiceOperation = Service.ApiControlPlane.Operation;
using Authorization = Service.ApiControlPlane.Authorization;
using IAuthorization = Service.ApiControlPlane.IAuthorization;
using ServiceRegistration = Service.ApiControlPlane.ServiceRegistration;

/// <inheritdoc cref="IServiceFactory"/>
public class ServiceRegistrationFactory : IServiceRegistrationFactory
{
    private readonly IServiceFactory _ServiceFactory;
    private readonly IOperationFactory _OperationFactory;
    private readonly IServiceAuthorizationFactory _ServiceAuthorizationFactory;
    private readonly IOperationAuthorizationFactory _OperationAuthorizationFactory;

    private const string _CallerName = "apicontrolplane-service";

    private readonly ITelemetry _Telemetry;
    private readonly IApiControlPlaneClient _ApiClient;

    /// <summary>
    /// Construct a new instance of <see cref="AuthorizationFactory"/>
    /// </summary>
    /// <param name="serviceFactory">The service factory.</param>
    /// <param name="operationFactory">The operation factory.</param>
    /// <param name="serviceAuthorizationFactory">The service authorization factory.</param>
    /// <param name="operationAuthorizationFactory">The oepration authorization factory.</param>
    /// <exception cref="ArgumentNullException">
    /// - <paramref name="serviceFactory"/> cannot be null.
    /// - <paramref name="operationFactory"/> cannot be null.
    /// - <paramref name="serviceAuthorizationFactory"/> cannot be null.
    /// - <paramref name="operationAuthorizationFactory"/> cannot be null.
    /// </exception>
    public ServiceRegistrationFactory(
        IServiceFactory serviceFactory,
        IOperationFactory operationFactory,
        IServiceAuthorizationFactory serviceAuthorizationFactory,
        IOperationAuthorizationFactory operationAuthorizationFactory
    )
    {
        _ServiceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));
        _OperationFactory = operationFactory ?? throw new ArgumentNullException(nameof(operationFactory));
        _ServiceAuthorizationFactory = serviceAuthorizationFactory ?? throw new ArgumentNullException(nameof(serviceAuthorizationFactory));
        _OperationAuthorizationFactory = operationAuthorizationFactory ?? throw new ArgumentNullException(nameof(operationAuthorizationFactory));
    }

    /// <summary>
    /// Construct a new instance of <see cref="AuthorizationFactory"/>
    /// </summary>
    /// <param name="apiClient">The API Control Plane client.</param>
    /// <param name="telemetry">The API Control Plane telemetry.</param>
    /// <exception cref="ArgumentNullException"><paramref name="apiClient"/> cannot be null.</exception>
    public ServiceRegistrationFactory(
        IApiControlPlaneClient apiClient,
        ITelemetry telemetry = null
    )
    {
        _ApiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));

        if (_ApiClient != null) _Telemetry = telemetry ?? new Telemetry();
    }

    /// <inheritdoc cref="IServiceRegistrationFactory.GetServiceRegistration(string)"/>
    public IServiceRegistration GetServiceRegistration(string serviceName)
    {
        if (string.IsNullOrEmpty(serviceName)) throw new ArgumentNullException(nameof(serviceName));

        if (_ApiClient == null)
        {
            var service = _ServiceFactory.GetByName(serviceName);
            if (service == null) return null;

            var apiClients = new List<IApiClient>();
            var authorizations = new List<IAuthorization>();

            var operations = _OperationFactory.GetAllByService(service);

            foreach (var serviceAuthorization in _ServiceAuthorizationFactory.GetAllByService(service))
            {
                if (!apiClients.Contains(serviceAuthorization.ApiClient)) apiClients.Add(serviceAuthorization.ApiClient);

                authorizations.Add(new Authorization(serviceAuthorization));
            }
            foreach (var operation in operations)
                foreach (var operationAuthorization in _OperationAuthorizationFactory.GetAllByOperation(operation))
                {
                    if (!apiClients.Contains(operationAuthorization.ApiClient)) apiClients.Add(operationAuthorization.ApiClient);

                    authorizations.Add(new Authorization(operationAuthorization));
                }

            return new ServiceRegistration
            {
                ServiceName = service.Name,
                IsEnabled = service.IsEnabled,
                ApiClients = apiClients,
                Authorizations = authorizations,
                Operations = operations
            };
        }

        var serviceRegistration = new ServiceRegistration();
        var serviceRegistrationResponse = _Telemetry.WrapSync(TelemetryType.http_client, _CallerName, "/v1/GetServiceRegistration", () => _ApiClient.GetServiceRegistration(serviceName)).Data;

        if (serviceRegistrationResponse == null) return null;

        serviceRegistration.ServiceName = serviceRegistrationResponse.ServiceName;
        serviceRegistration.IsEnabled = serviceRegistrationResponse.IsEnabled;

        serviceRegistration.ApiClients = serviceRegistrationResponse.ApiClients.Select(client => new ServiceApiClient(client)).ToArray();
        serviceRegistration.Operations = serviceRegistrationResponse.Operations.Select(operation => new ServiceOperation(operation)).ToArray();
        serviceRegistration.Authorizations = serviceRegistrationResponse.Authorizations.Select(auth => new Authorization(auth)).ToArray();

        return serviceRegistration;
    }
}
