namespace Roblox.Api.ControlPlane;

using System.Collections.Generic;

using Service.ApiControlPlane;

/// <summary>
/// Represents an interface for validating whether or not an API key has access to an operation.
/// </summary>
public interface IAuthorizationVerifier
{
    /// <summary>
    /// Does the specified <see cref="IApiClient"/> have access to the <see cref="IOperation"/> within the <see cref="IService"/>?
    /// </summary>
    /// <param name="apiClient">The <see cref="IApiClient"/>.</param>
    /// <param name="operation">The <see cref="IOperation"/>.</param>
    /// <returns>True if the client has access to the <see cref="IOperation"/> within the <see cref="IService"/></returns>
    bool HasAccess(IApiClient apiClient, IOperation operation);

    /// <summary>
    /// Does the specified <see cref="IApiClient"/> have access to the <see cref="IOperation"/> within the <see cref="IService"/>?
    /// </summary>
    /// <param name="apiClient">The <see cref="IApiClient"/>.</param>
    /// <param name="serviceName">The name of the <see cref="IService"/>.</param>
    /// <param name="operationName">The name of the <see cref="IOperation"/></param>
    /// <returns>True if the client has access to the <see cref="IOperation"/> within the <see cref="IService"/></returns>
    bool HasAccess(IApiClient apiClient, string serviceName, string operationName);

    /// <summary>
    /// Does the specified <see cref="IAuthorization"/> array have any authorizations for the <see cref="IOperation"/> within the <see cref="IService"/>?
    /// </summary>
    /// <param name="authorizations">The <see cref="IAuthorization"/>s.</param>
    /// <param name="serviceName">The name of the <see cref="IService"/>.</param>
    /// <param name="operationName">The name of the <see cref="IOperation"/></param>
    /// <returns>True if the client has access to the <see cref="IOperation"/> within the <see cref="IService"/></returns>
    public bool HasAccess(ICollection<IAuthorization> authorizations, string serviceName, string operationName);
}
