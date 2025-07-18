namespace Roblox.Service.ApiControlPlane;

/// <summary>
/// <c>Àuthorization Type</c>
/// </summary>
public enum AuthorizationTypeEnum : byte
{
    /// <summary>
    /// The client has no authorization over the operation or service.
    /// </summary>
    None = 1,

    /// <summary>
    /// The client has partial authorization over the service, normally paired with OperationAuthorizations of type Full.
    /// </summary>
    Partial = 2,

    /// <summary>
    /// The client has full authorization over the operation or service.
    /// </summary>
    Full = 3
}
