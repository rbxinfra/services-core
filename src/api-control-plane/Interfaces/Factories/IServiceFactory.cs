namespace Roblox.Api.ControlPlane;

using System.Collections.Generic;

using Service.ApiControlPlane;

/// <summary>
/// Interface for a factory that returns <see cref="IService"/> instances.
/// </summary>
public interface IServiceFactory
{
    /// <summary>
    /// Create a new <see cref="IService"/>
    /// </summary>
    /// <param name="name">The name of the <see cref="IService"/></param>
    /// <param name="isEnabled">Is this service enabled?</param>
    /// <returns>The new <see cref="IService"/></returns>
    IService CreateNew(string name, bool isEnabled);

    /// <summary>
    /// Get an <see cref="IService"/> by ID.
    /// </summary>
    /// <param name="id">The ID of the service.</param>
    /// <returns>The <see cref="IService"/>.</returns>
    IService GetByID(int id);

    /// <summary>
    /// Get an <see cref="IService"/> by Name.
    /// </summary>
    /// <param name="name">The name of the service.</param>
    /// <returns>The <see cref="IService"/>.</returns>
    IService GetByName(string name);

    /// <summary>
    /// Gets the total number of services.
    /// </summary>
    /// <returns>The total number of <see cref="IService"/></returns>
    int GetTotalNumber();

    /// <summary>
    /// Get a collection of all services
    /// </summary>
    /// <returns>The collection of <see cref="IService"/></returns>
    ICollection<IService> GetAll();

    /// <summary>
    /// Get a collection of all services with pagination.
    /// </summary>
    /// <param name="startRowIndex">The row to start indexing (page)</param>
    /// <param name="maximumRows">The maximum amount of rows to return (pageSize)</param>
    /// <returns>The collection of <see cref="IService"/></returns>
    ICollection<IService> GetAll_Paged(int startRowIndex, int maximumRows);
}
