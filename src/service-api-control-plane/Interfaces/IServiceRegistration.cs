namespace Roblox.Service.ApiControlPlane;

using System.Collections.Generic;

/// <summary>
/// Represents the registration of a service
/// </summary>
public interface IServiceRegistration
{
    /// <summary>
    /// Gets the service name.
    /// </summary>
    string ServiceName { get; }

    /// <summary>
    /// Determines if the service is enabled.
    /// </summary>
    bool IsEnabled { get; }

    /// <summary>
    /// Gets the API Clients that have authorizations to this service.
    /// </summary>
    ICollection<IApiClient> ApiClients { get; }

    /// <summary>
    /// Gets the operations this service owns.
    /// </summary>
    ICollection<IOperation> Operations { get; }

    /// <summary>
    /// Gets the authorizations that this service has.
    /// </summary>
    ICollection<IAuthorization> Authorizations { get; }
}
