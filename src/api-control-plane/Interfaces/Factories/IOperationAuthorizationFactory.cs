namespace Roblox.Api.ControlPlane;

using System.Collections.Generic;

using Service.ApiControlPlane;

/// <summary>
/// Interface for a factory that returns <see cref="IOperationAuthorization"/> instances.
/// </summary>
public interface IOperationAuthorizationFactory
{
    /// <summary>
    /// Create a new <see cref="IOperationAuthorization"/>
    /// </summary>
    /// <param name="apiClient">The service of the service.</param>
    /// <param name="operation">The name of the operation.</param>
    /// <param name="authorizationType">The authorization type.</param>
    /// <returns>The newly created <see cref="IOperationAuthorization"/></returns>
    IOperationAuthorization CreateNew(IApiClient apiClient, IOperation operation, AuthorizationTypeEnum authorizationType = AuthorizationTypeEnum.Full);

    /// <summary>
    /// Get an <see cref="IOperationAuthorization"/> by ID.
    /// </summary>
    /// <param name="id">The ID of the operation authorization.</param>
    /// <returns>The <see cref="IOperationAuthorization"/>.</returns>
    IOperationAuthorization GetByID(int id);

    /// <summary>
    /// Get an <see cref="IOperation"/> by <see cref="IApiClient"/> and <see cref="IOperation"/>.
    /// </summary>
    /// <param name="apiClient">The service of the operation.</param>
    /// <param name="operation">The name of the operation.</param>
    /// <returns>The <see cref="IOperationAuthorization"/>.</returns>
    IOperationAuthorization GetByApiClientAndOperation(IApiClient apiClient, IOperation operation);

    /// <summary>
    /// Gets the total number of operation authorizations by <see cref="IOperation"/>
    /// </summary>
    /// <param name="operation">The operation</param>
    /// <returns>The total number of <see cref="IOperationAuthorization"/></returns>
    int GetTotalNumberByOperation(IOperation operation);

    /// <summary>
    /// Get a collection of all operation authorizations by <see cref="IOperation"/>.
    /// </summary>
    /// <param name="operation">The operation.</param>
    /// <returns>The collection of <see cref="IOperationAuthorization"/></returns>
    ICollection<IOperationAuthorization> GetAllByOperation(IOperation operation);

    /// <summary>
    /// Get a collection of all operation authorizations by <see cref="IOperation"/> with pagination.
    /// </summary>
    /// <param name="operation">The operation.</param>
    /// <param name="startRowIndex">The row to start indexing (page)</param>
    /// <param name="maximumRows">The maximum amount of rows to return (pageSize)</param>
    /// <returns>The collection of <see cref="IOperationAuthorization"/></returns>
    ICollection<IOperationAuthorization> GetAllByOperation_Paged(IOperation operation, int startRowIndex, int maximumRows);

    /// <summary>
    /// Gets the total number of operation authorizations by <see cref="IApiClient"/>
    /// </summary>
    /// <param name="apiClient">The api client</param>
    /// <returns>The total number of <see cref="IOperationAuthorization"/></returns>
    int GetTotalNumberByApiClient(IApiClient apiClient);

    /// <summary>
    /// Get a collection of all operation authorizations by <see cref="IApiClient"/>.
    /// </summary>
    /// <param name="apiClient">The operation.</param>
    /// <returns>The collection of <see cref="IOperationAuthorization"/></returns>
    ICollection<IOperationAuthorization> GetAllByApiClient(IApiClient apiClient);

    /// <summary>
    /// Get a collection of all operation authorizations by <see cref="IApiClient"/> with pagination.
    /// </summary>
    /// <param name="apiClient">The api client.</param>
    /// <param name="startRowIndex">The row to start indexing (page)</param>
    /// <param name="maximumRows">The maximum amount of rows to return (pageSize)</param>
    /// <returns>The collection of <see cref="IOperationAuthorization"/></returns>
    ICollection<IOperationAuthorization> GetAllByApiClient_Paged(IApiClient apiClient, int startRowIndex, int maximumRows);
}
