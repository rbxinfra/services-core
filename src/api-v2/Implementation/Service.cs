namespace Roblox.ApiV2;

using System;

using Api.ControlPlane;

/// <summary>
/// Default implementation for <see cref="IService"/>
/// </summary>
public abstract class Service : IService
{
    private readonly IAuthority _Authority;

    /// <summary>
    /// Construct a new instance of <see cref="Service"/>
    /// </summary>
    /// <param name="authority">The <see cref="IAuthority"/></param>
    /// <exception cref="ArgumentNullException"><paramref name="authority"/> cannot be null.</exception>
    public Service(IAuthority authority)
    {
        _Authority = authority ?? throw new ArgumentNullException(nameof(authority));
    }

    /// <inheritdoc cref="IService.Name"/>
    public abstract string Name { get; }

    /// <inheritdoc cref="IService.IsEnabled()"/>
    public virtual bool IsEnabled()
        => _Authority.ServiceIsEnabled(Name);

    /// <inheritdoc cref="IService.ShouldThrottleSystem()"/>
    public virtual bool ShouldThrottleSystem() => false;

    /// <inheritdoc cref="IService.ShouldThrottleClient(Guid)"/>
    public virtual bool ShouldThrottleClient(Guid key) => false;
}
