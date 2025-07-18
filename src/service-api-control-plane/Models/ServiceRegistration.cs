namespace Roblox.Service.ApiControlPlane;

using System.Collections.Generic;

/// <inheritdoc cref="IServiceRegistration"/>
internal class ServiceRegistration : IServiceRegistration
{
    /// <inheritdoc cref="IServiceRegistration.ServiceName"/>
    public string ServiceName { get; set; }

    /// <inheritdoc cref="IServiceRegistration.IsEnabled"/>
    public bool IsEnabled { get; set; }

    /// <inheritdoc cref="IServiceRegistration.ApiClients"/>
    public ICollection<IApiClient> ApiClients { get; set; }

    /// <inheritdoc cref="IServiceRegistration.Operations"/>
    public ICollection<IOperation> Operations { get; set; }

    /// <inheritdoc cref="IServiceRegistration.Authorizations"/>
    public ICollection<IAuthorization> Authorizations { get; set; }
}
