namespace Roblox.Api.ControlPlane;

using System.Collections.Generic;

using Service.ApiControlPlane;

/// <summary>
/// Interface for a factory that returns <see cref="IOperation"/> instances.
/// </summary>
public interface IOperationFactory
{
    /// <summary>
    /// Create a new <see cref="IOperation"/>
    /// </summary>
    /// <param name="name">The name of the operation.</param>
    /// <param name="service">The <see cref="IService"/></param>
    /// <param name="isEnabled">Is this operation enabled?</param>
    /// <returns>The newly created <see cref="IOperation"/></returns>
    IOperation CreateNew(string name, IService service, bool isEnabled);

    /// <summary>
    /// Get an <see cref="IOperation"/> by ID.
    /// </summary>
    /// <param name="id">The ID of the operation.</param>
    /// <returns>The <see cref="IOperation"/>.</returns>
    IOperation GetByID(int id);

    /// <summary>
    /// Get an <see cref="IOperation"/> by Name.
    /// </summary>
    /// <param name="service">The service of the operation.</param>
    /// <param name="name">The name of the operation.</param>
    /// <returns>The <see cref="IOperation"/>.</returns>
    IOperation GetByName(IService service, string name);

    /// <summary>
    /// Gets the total number of operations by <see cref="IService"/>
    /// </summary>
    /// <param name="service">The service</param>
    /// <returns>The total number of <see cref="IServiceAuthorization"/></returns>
    int GetTotalNumberByService(IService service);

    /// <summary>
    /// Get a collection of all operations by <see cref="IService"/>.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <returns>The collection of <see cref="IServiceAuthorization"/></returns>
    ICollection<IOperation> GetAllByService(IService service);

    /// <summary>
    /// Get a collection of all operations by <see cref="IService"/> with pagination.
    /// </summary>
    /// <param name="service">The name of the <see cref="IService"/></param>
    /// <param name="startRowIndex">The row to start indexing (page)</param>
    /// <param name="maximumRows">The maximum amount of rows to return (pageSize)</param>
    /// <returns>The collection of <see cref="IOperation"/></returns>
    ICollection<IOperation> GetAllByService_Paged(IService service, int startRowIndex, int maximumRows);
}
