namespace Roblox.Service.ApiControlPlane;

using System;

/// <summary>
/// Represents the model for an Operation Authorization returned by the service.
/// </summary>
public interface IOperationAuthorization
{
    /// <summary>
    /// Gets the ID.
    /// </summary>
    int ID { get; }

    /// <summary>
    /// Gets the Operation.
    /// </summary>
    IOperation Operation { get; set; }

    /// <summary>
    /// Gets the ApiClient.
    /// </summary>
    IApiClient ApiClient { get; set; }

    /// <summary>
    /// Gets or sets the AuhthorizationType.
    /// </summary>
    AuthorizationTypeEnum AuthorizationType { get; set; }

    /// <summary>
    /// Gets the created date.
    /// </summary>
    DateTime Created { get; }

    /// <summary>
    /// Gets the created date.
    /// </summary>
    DateTime Updated { get; }

    /// <summary>
    /// Is the operation authorization disabled? <see cref="AuthorizationType"/> has to be <see cref="AuthorizationTypeEnum.None"/>
    /// </summary>
    /// <returns>True if the operation authorization is disabled.</returns>
    bool IsDisabled();

    /// <summary>
    /// Is the operation authorization enabled? 
    /// <see cref="AuthorizationType"/> has to be <see cref="AuthorizationTypeEnum.Full"/> or <see cref="AuthorizationTypeEnum.Partial"/>
    /// </summary>
    /// <returns>True if the operation authorization is enabled.</returns>
    bool IsEnabled();

    /// <summary>
    /// Is this operation authorization owned by the following service?
    /// </summary>
    /// <param name="service">The service.</param>
    /// <returns>True if the operation authorization is owned by the service.</returns>
    bool IsOwnedByService(IService service);

    /// <summary>
    /// Delete the operation authorization from the database or service.
    /// </summary>
    void Delete();

    /// <summary>
    /// Save the operation authorization to the database or service.
    /// </summary>
    void Save();
}
