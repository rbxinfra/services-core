namespace Roblox.Service.ApiControlPlane;

/// <summary>
/// <c>Status Type</c>
/// </summary>
public enum StatusTypeEnum : byte
{
    /// <summary>
    /// The service, API Client or Operation is enabled.
    /// </summary>
    Enabled = 1,

    /// <summary>
    /// The service, API Client or Operation is disabled.
    /// </summary>
    Disabled = 2
}
