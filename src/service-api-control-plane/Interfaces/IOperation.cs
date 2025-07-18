namespace Roblox.Service.ApiControlPlane;

using System;

/// <summary>
/// Represents the model for an Operation returned by the service.
/// </summary>
public interface IOperation
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
    /// Gets the Service.
    /// </summary>
    IService Service { get; set; }

    /// <summary>
    /// Gets the value that determines if this operation is enabled.
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
    /// Disable the operation.
    /// </summary>
    void Disable();

    /// <summary>
    /// Enable the operation.
    /// </summary>
    void Enable();

    /// <summary>
    /// Delete the operation from the Database or Service.
    /// </summary>
    void Delete();

    /// <summary>
    /// Save the operation.
    /// </summary>
    void Save();
}
