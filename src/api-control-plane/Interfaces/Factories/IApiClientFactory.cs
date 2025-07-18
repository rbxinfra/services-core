namespace Roblox.Api.ControlPlane;

using System;
using System.Collections.Generic;

using Service.ApiControlPlane;

/// <summary>
/// Interface for a factory that returns <see cref="IApiClient"/> instances.
/// </summary>
public interface IApiClientFactory
{
    /// <summary>
    /// Create a new <see cref="IApiClient"/>
    /// </summary>
    /// <param name="apiKey">The key of the API Client</param>
    /// <param name="note">The optional note.</param>
    /// <param name="isValid">Is the API Client enabled?</param>
    /// <returns>The newly created <see cref="IApiClient"/></returns>
    IApiClient CreateNew(Guid? apiKey, string note = null, bool isValid = true);

    /// <summary>
    /// Get an <see cref="IApiClient"/> by ID.
    /// </summary>
    /// <param name="id">The ID of the API client.</param>
    /// <returns>The <see cref="IApiClient"/>.</returns>
    IApiClient GetByID(int id);

    /// <summary>
    /// Get an <see cref="IApiClient"/> by API key.
    /// </summary>
    /// <param name="key">The API key of the API client.</param>
    /// <returns>The <see cref="IApiClient"/>.</returns>
    IApiClient GetByKey(Guid key);

    /// <summary>
    /// Get an <see cref="IApiClient"/> by note.
    /// </summary>
    /// <param name="note">The note of the API client.</param>
    /// <returns>The <see cref="IApiClient"/>.</returns>
    IApiClient GetByNote(string note);

    /// <summary>
    /// Gets the total number of API clients.
    /// </summary>
    /// <returns>The total number of <see cref="IApiClient"/></returns>
    int GetTotalNumber();

    /// <summary>
    /// Get a collection of all API Clients
    /// </summary>
    /// <returns>The collection of <see cref="IApiClient"/></returns>
    ICollection<IApiClient> GetAll();

    /// <summary>
    /// Get a collection of all API Clients with pagination.
    /// </summary>
    /// <param name="startRowIndex">The row to start indexing (page)</param>
    /// <param name="maximumRows">The maximum amount of rows to return (pageSize)</param>
    /// <returns>The collection of <see cref="IApiClient"/></returns>
    ICollection<IApiClient> GetAll_Paged(int startRowIndex, int maximumRows);
}
