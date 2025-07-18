namespace Roblox.Api.ControlPlane;

using System.Collections.Generic;

using Service.ApiControlPlane;

/// <summary>
/// Interface for a factory that returns <see cref="IAuthorization"/> instances.
/// </summary>
public interface IAuthorizationFactory
{
    /// <summary>
    /// Get the total number of authorizations by <see cref="IApiClient"/>.
    /// </summary>
    /// <param name="apiClient">The <see cref="IApiClient"/></param>
    /// <returns>The total number of <see cref="IAuthorization"/></returns>
    int GetTotalNumberByApiClient(IApiClient apiClient);

    /// <summary>
    /// Get a collection of all authorizations by <see cref="IApiClient"/>.
    /// </summary>
    /// <param name="apiClient">The <see cref="IApiClient"/>.</param>
    /// <returns>The collection of <see cref="IAuthorization"/></returns>
    ICollection<IAuthorization> GetAllByApiClient(IApiClient apiClient);

    /// <summary>
    /// Get a collection of all authorizations by <see cref="IApiClient"/> with pagination.
    /// </summary>
    /// <param name="apiClient">The <see cref="IApiClient"/>.</param>
    /// <param name="startRowIndex">The row to start indexing (page)</param>
    /// <param name="maximumRows">The maximum amount of rows to return (pageSize)</param>
    /// <returns>The collection of <see cref="IAuthorization"/></returns>
    ICollection<IAuthorization> GetAllByApiClient_Paged(IApiClient apiClient, int startRowIndex, int maximumRows);
}
