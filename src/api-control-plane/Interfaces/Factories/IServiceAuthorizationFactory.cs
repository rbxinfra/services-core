namespace Roblox.Api.ControlPlane;

using System.Collections.Generic;

using Service.ApiControlPlane;

/// <summary>
/// Interface for a factory that returns <see cref="IServiceAuthorization"/> instances.
/// </summary>
public interface IServiceAuthorizationFactory
{
    /// <summary>
    /// Create a new <see cref="IServiceAuthorization"/>
    /// </summary>
    /// <param name="apiClient">The service of the service.</param>
    /// <param name="service">The name of the service.</param>
    /// <param name="authorizationType">The authorization type.</param>
    /// <returns>The newly created <see cref="IServiceAuthorization"/></returns>
    IServiceAuthorization CreateNew(IApiClient apiClient, IService service, AuthorizationTypeEnum authorizationType = AuthorizationTypeEnum.Full);

    /// <summary>
    /// Get an <see cref="IServiceAuthorization"/> by ID.
    /// </summary>
    /// <param name="id">The ID of the service authorization.</param>
    /// <returns>The <see cref="IServiceAuthorization"/>.</returns>
    IServiceAuthorization GetByID(int id);

    /// <summary>
    /// Get an <see cref="IService"/> by <see cref="IApiClient"/> and <see cref="IService"/>.
    /// </summary>
    /// <param name="apiClient">The service of the service.</param>
    /// <param name="service">The name of the service.</param>
    /// <returns>The <see cref="IServiceAuthorization"/>.</returns>
    IServiceAuthorization GetByApiClientAndService(IApiClient apiClient, IService service);

    /// <summary>
    /// Gets the total number of service authorizations by <see cref="IService"/>
    /// </summary>
    /// <param name="service">The service</param>
    /// <returns>The total number of <see cref="IServiceAuthorization"/></returns>
    int GetTotalNumberByService(IService service);

    /// <summary>
    /// Get a collection of all service authorizations by <see cref="IService"/>.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <returns>The collection of <see cref="IServiceAuthorization"/></returns>
    ICollection<IServiceAuthorization> GetAllByService(IService service);

    /// <summary>
    /// Get a collection of all service authorizations by <see cref="IService"/> with pagination.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <param name="startRowIndex">The row to start indexing (page)</param>
    /// <param name="maximumRows">The maximum amount of rows to return (pageSize)</param>
    /// <returns>The collection of <see cref="IServiceAuthorization"/></returns>
    ICollection<IServiceAuthorization> GetAllByService_Paged(IService service, int startRowIndex, int maximumRows);

    /// <summary>
    /// Gets the total number of service authorizations by <see cref="IApiClient"/>
    /// </summary>
    /// <param name="apiClient">The api client</param>
    /// <returns>The total number of <see cref="IServiceAuthorization"/></returns>
    int GetTotalNumberByApiClient(IApiClient apiClient);

    /// <summary>
    /// Get a collection of all service authorizations by <see cref="IApiClient"/>.
    /// </summary>
    /// <param name="apiClient">The service.</param>
    /// <returns>The collection of <see cref="IServiceAuthorization"/></returns>
    ICollection<IServiceAuthorization> GetAllByApiClient(IApiClient apiClient);

    /// <summary>
    /// Get a collection of all service authorizations by <see cref="IApiClient"/> with pagination.
    /// </summary>
    /// <param name="apiClient">The api client.</param>
    /// <param name="startRowIndex">The row to start indexing (page)</param>
    /// <param name="maximumRows">The maximum amount of rows to return (pageSize)</param>
    /// <returns>The collection of <see cref="IServiceAuthorization"/></returns>
    ICollection<IServiceAuthorization> GetAllByApiClient_Paged(IApiClient apiClient, int startRowIndex, int maximumRows);
}
