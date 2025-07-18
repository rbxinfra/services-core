namespace Roblox.Service.ApiControlPlane;

using System;

/// <summary>
/// Represents the model for a Service returned by the service.
/// </summary>
public interface IService
{
    /// <summary>
    /// Gets the ID.
    /// </summary>
    int ID { get; }

    /// <summary>
    /// Gets or sets the Name.
    /// </summary>
    string Name { get; set; }

    /// <summary>
    /// Gets the value that determines if this service is enabled.
    /// </summary>
    bool IsEnabled { get; }

    /// <summary>
    /// Gets the created date.
    /// </summary>
    DateTime Created { get; }

    /// <summary>
    /// Gets the created date.
    /// </summary>
    DateTime Updated { get; }

    /// <summary>
    /// Disable the service.
    /// </summary>
    void Disable();

    /// <summary>
    /// Enable the service.
    /// </summary>
    void Enable();

    /// <summary>
    /// Delete the service from the Database or Service.
    /// </summary>
    void Delete();

    /// <summary>
    /// Save the service.
    /// </summary>
    void Save();
}
