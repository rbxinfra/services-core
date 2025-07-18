namespace Roblox.Api.ControlPlane;

using System.Collections.Generic;

using Service.ApiControlPlane;

/// <summary>
/// Interface for a factory that returns <see cref="IAuthorization"/> instances.
/// </summary>
public interface IServiceRegistrationFactory
{
    /// <summary>
    /// Gets the registration for a service.
    /// </summary>
    /// <remarks>If using API client instead of data access, the information returned will be majorly cut down.</remarks>
    /// <param name="serviceName">The name of the <see cref="IService"/>.</param>
    /// <returns>The <see cref="IServiceRegistration"/></returns>
    IServiceRegistration GetServiceRegistration(string serviceName);
}
