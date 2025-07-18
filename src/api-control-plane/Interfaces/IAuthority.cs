namespace Roblox.Api.ControlPlane;

using System;

using Service.ApiControlPlane;

/// <summary>
/// Authority for the API Control Plane
/// </summary>
public interface IAuthority
{
    /// <summary>
    /// Checks if there is an existing <see cref="IApiClient"/> with the specified API key,
    /// as well as if the client is enabled or not.
    /// </summary>
    /// <param name="apiKey">The API key of the <see cref="IApiClient"/></param>
    /// <returns>True if the <see cref="IApiClient"/> exists, false otherwise.</returns>
    bool Authenticate(Guid apiKey);

    /// <summary>
    /// Is the specified <see cref="IApiClient"/> authorized to the <see cref="IService"/>?
    /// </summary>
    /// <param name="apiKey">The API key of the <see cref="IApiClient"/>.</param>
    /// <param name="serviceName">The name of the <see cref="IService"/>.</param>
    /// <param name="apiClient">The <see cref="IApiClient"/>.</param>
    /// <returns>True if the <see cref="IApiClient"/> is authorized to the <see cref="IService"/>.</returns>
    bool IsAuthorized(Guid apiKey, string serviceName, out IApiClient apiClient);

    /// <summary>
    /// Is the specified <see cref="IApiClient"/> authorized to the <see cref="IOperation"/> in the <see cref="IService"/>?
    /// </summary>
    /// <param name="apiKey">The API key of the <see cref="IApiClient"/>.</param>
    /// <param name="serviceName">The name of the <see cref="IService"/>.</param>
    /// <param name="operationName">The name of the <see cref="IOperation"/>.</param>
    /// <param name="apiClient">The <see cref="IApiClient"/>.</param>
    /// <returns>True if the <see cref="IApiClient"/> is authorized to the <see cref="IOperation"/> in the <see cref="IService"/>.</returns>
    bool IsAuthorized(Guid apiKey, string serviceName, string operationName, out IApiClient apiClient);

    /// <summary>
    /// Is the specified <see cref="IOperation"/> enabled?
    /// </summary>
    /// <param name="serviceName">The name of the <see cref="IService"/>.</param>
    /// <param name="operationName">The name of the <see cref="IOperation"/>.</param>
    /// <returns>True if the <see cref="IOperation"/> is enabled.</returns>
    bool OperationIsEnabled(string serviceName, string operationName);

    /// <summary>
    /// Is the specified <see cref="IService"/> enabled?
    /// </summary>
    /// <param name="serviceName">The name of the <see cref="IService"/>.</param>
    /// <returns>True if the <see cref="IService"/> is enabled.</returns>
    bool ServiceIsEnabled(string serviceName);
}
