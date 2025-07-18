namespace Roblox.Service.ApiControlPlane;

using System;

/// <summary>
/// Represents the model for an Service Authorization returned by the service.
/// </summary>
public interface IServiceAuthorization
{
    /// <summary>
    /// Gets or sets the ID.
    /// </summary>
    int ID { get; }

    /// <summary>
    /// Gets the Service.
    /// </summary>
    IService Service { get; set; }

    /// <summary>
    /// Gets the ApiClient.
    /// </summary>
    IApiClient ApiClient { get; set; }

    /// <summary>
    /// Gets the AuthorizationType.
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
    /// Is the service authorization disabled? <see cref="AuthorizationType"/> has to be <see cref="AuthorizationTypeEnum.None"/>
    /// </summary>
    /// <returns>True if the service authorization is disabled.</returns>
    bool IsDisabled();

    /// <summary>
    /// Is the service authorization enabled? 
    /// <see cref="AuthorizationType"/> has to be <see cref="AuthorizationTypeEnum.Full"/> or <see cref="AuthorizationTypeEnum.Partial"/>
    /// </summary>
    /// <returns>True if the service authorization is enabled.</returns>
    bool IsEnabled();

    /// <summary>
    /// Does this service authorization have child operation authorizations? 
    /// <see cref="AuthorizationType"/> has to be <see cref="AuthorizationTypeEnum.Partial"/>
    /// </summary>
    /// <returns>True if the service authorization has child operation authorizations.</returns>
    bool HasOperationAuthorizations();

    /// <summary>
    /// Does this service authorization allow full access to the service?
    /// <see cref="AuthorizationType"/> has to be <see cref="AuthorizationTypeEnum.Full"/>
    /// </summary>
    /// <returns>True if the service authorization has full access.</returns>
    bool AllowsFullServiceAccess();

    /// <summary>
    /// Delete the service authoriation.
    /// </summary>
    void Delete();

    /// <summary>
    /// Save the service authorization to the database or service.
    /// </summary>
    void Save();
}
