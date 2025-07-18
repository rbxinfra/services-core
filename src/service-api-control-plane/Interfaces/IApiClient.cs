namespace Roblox.Service.ApiControlPlane;

using System;

/// <summary>
/// Represents the model for an API Client returned by the service.
/// </summary>
public interface IApiClient
{
    /// <summary>
    /// Gets the ID of this API Client.
    /// </summary>
    int ID { get; }

    /// <summary>
    /// Gets or sets the API Key of this API Client.
    /// </summary>
    Guid ApiKey { get; set; }

    /// <summary>
    /// Gets or sets the note or name of this API Client.
    /// </summary>
    string Note { get; set; }

    /// <summary>
    /// Gets the value that determines if this API Client is enabled or not.
    /// </summary>
    bool IsValid { get; }

    /// <summary>
    /// Gets or sets the created date of this API Client.
    /// </summary>
    DateTime Created { get; }

    /// <summary>
    /// Gets or sets the updated date of this API Client.
    /// </summary>
    DateTime Updated { get; }

    /// <summary>
    /// Disable the API Client.
    /// </summary>
    void SetInvalid();

    /// <summary>
    /// Enable the API Client.
    /// </summary>
    void SetValid();

    /// <summary>
    /// Delete the API Client from the Database or Service.
    /// </summary>
    void Delete();

    /// <summary>
    /// Save the API Client.
    /// </summary>
    void Save();
}
