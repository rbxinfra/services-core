namespace Roblox.Api.ControlPlane;

using System;
using System.Linq;

using Caching;
using EventLog;
using ApplicationContext;
using Service.ApiControlPlane;

/// <summary>
/// <see cref="IAuthority"/> for serving authority through the API Control Plane Service.
/// </summary>
public class ServiceAuthority : IAuthority
{
    private static readonly TimeSpan _DefaultServiceRegistrationRefreshInterval = TimeSpan.FromMinutes(30);

    private readonly ILogger _Logger;
    private readonly IApplicationContext _ApplicationContext;
    private readonly IAuthorizationVerifier _AuthorizationVerifier;
    private readonly IServiceRegistrationFactory _ServiceRegistrationFactory;

    private readonly LazyWithRetry<RefreshAhead<IServiceRegistration>> _CurrentServiceRegistration;

    private IServiceRegistration CurrentServiceRegistration => _CurrentServiceRegistration.Value.Value;

    /// <summary>
    /// Construct a new instance of <see cref="ServiceAuthority"/>
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/></param>
    /// <param name="applicationContext">The <see cref="IApplicationContext"/></param>
    /// <param name="authorizationVerifier">The <see cref="IAuthorizationVerifier"/></param>
    /// <param name="serviceRegistrationFactory">The <see cref="IServiceRegistrationFactory"/></param>
    /// <param name="serviceRegistrationRefreshInterval">The refresh interval for the service registration.</param>
    /// <exception cref="ArgumentNullException">
    /// - <paramref name="logger"/> cannot be null.
    /// - <paramref name="applicationContext"/> cannot be null.
    /// - <paramref name="authorizationVerifier"/> cannot be null.
    /// - <paramref name="serviceRegistrationFactory"/> cannot be null.
    /// </exception>
    public ServiceAuthority(
        ILogger logger,
        IApplicationContext applicationContext,
        IAuthorizationVerifier authorizationVerifier,
        IServiceRegistrationFactory serviceRegistrationFactory,
        TimeSpan serviceRegistrationRefreshInterval = default
    )
    {
        _Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _ApplicationContext = applicationContext ?? throw new ArgumentNullException(nameof(applicationContext));
        _AuthorizationVerifier = authorizationVerifier ?? throw new ArgumentNullException(nameof(authorizationVerifier));
        _ServiceRegistrationFactory = serviceRegistrationFactory ?? throw new ArgumentNullException(nameof(serviceRegistrationFactory));

        if (serviceRegistrationRefreshInterval == default) serviceRegistrationRefreshInterval = _DefaultServiceRegistrationRefreshInterval;

        _CurrentServiceRegistration = new LazyWithRetry<RefreshAhead<IServiceRegistration>>(
            () => RefreshAhead<IServiceRegistration>.ConstructAndPopulate(serviceRegistrationRefreshInterval, RefreshCurrentServiceRegistration)
        );
    }

    private IServiceRegistration RefreshCurrentServiceRegistration(IServiceRegistration current)
    {
        try
        {
            var serviceName = _ApplicationContext.Name;

            return _ServiceRegistrationFactory.GetServiceRegistration(serviceName);
        }
        catch (Exception ex)
        {
            _Logger.Error(ex);

            return current;
        }
    }

    /// <inheritdoc cref="IAuthority.Authenticate(Guid)" />
    public bool Authenticate(Guid apiKey)
        => CurrentServiceRegistration.ApiClients.Any(client => client.ApiKey == apiKey && client.IsValid);

    /// <inheritdoc cref="IAuthority.IsAuthorized(Guid, string, string, out IApiClient)"/>
    public bool IsAuthorized(Guid apiKey, string serviceName, string operationName, out IApiClient apiClient)
    {
        apiClient = null;

        if (CurrentServiceRegistration == null) return false;

        var currentApiClient = CurrentServiceRegistration.ApiClients.FirstOrDefault(client => client.ApiKey == apiKey);
        apiClient = currentApiClient;

        if (apiClient == null) return false;

        var authorizations = CurrentServiceRegistration.Authorizations.Where(auth => auth.ApiClient.ID == currentApiClient.ID).ToArray();

        return _AuthorizationVerifier.HasAccess(authorizations, serviceName, operationName);
    }

    /// <inheritdoc cref="IAuthority.IsAuthorized(Guid, string, out IApiClient)"/>
    public bool IsAuthorized(Guid apiKey, string serviceName, out IApiClient apiClient)
    {
        apiClient = null;

        if (CurrentServiceRegistration == null) return false;

        var currentApiClient = CurrentServiceRegistration?.ApiClients.FirstOrDefault(client => client.ApiKey == apiKey);
        apiClient = currentApiClient;

        if (apiClient == null) return false;

        var authorizations = CurrentServiceRegistration?.Authorizations.Where(auth => auth.ApiClient.ID == currentApiClient.ID).ToArray();

        return _AuthorizationVerifier.HasAccess(authorizations, serviceName, null);
    }

    /// <inheritdoc cref="IAuthority.OperationIsEnabled(string, string)"/>
    public bool OperationIsEnabled(string serviceName, string operationName)
    {
        if (CurrentServiceRegistration == null) return false;

        var operation = CurrentServiceRegistration.Operations.FirstOrDefault(operation => operation.Name == operationName);

        return operation != null && operation.IsEnabled;
    }

    /// <inheritdoc cref="IAuthority.ServiceIsEnabled(string)"/>
    public bool ServiceIsEnabled(string serviceName)
    {
        if (CurrentServiceRegistration == null) return false;

        return CurrentServiceRegistration.IsEnabled;
    }
}
