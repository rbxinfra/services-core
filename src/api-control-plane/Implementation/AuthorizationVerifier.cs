namespace Roblox.Api.ControlPlane;

using System;
using System.Linq;
using System.Collections.Generic;

using Service.ApiControlPlane;

/// <inheritdoc cref="IAuthorizationVerifier"/>
public class AuthorizationVerifier : IAuthorizationVerifier
{
    private readonly IAuthorizationFactory _AuthorizationFactory;

    /// <summary>
    /// Construct a new instance of <see cref="AuthorizationVerifier"/>
    /// </summary>
    public AuthorizationVerifier()
    {
    }

    /// <summary>
    /// Construct a new instance of <see cref="AuthorizationVerifier"/>
    /// </summary>
    /// <param name="authorizationFactory">The <see cref="IAuthorizationFactory"/></param>
    /// <exception cref="ArgumentNullException"><paramref name="authorizationFactory"/> cannot be null.</exception>
    public AuthorizationVerifier(IAuthorizationFactory authorizationFactory)
    {
        _AuthorizationFactory = authorizationFactory ?? throw new ArgumentNullException(nameof(authorizationFactory));
    }

    /// <inheritdoc cref="IAuthorizationVerifier.HasAccess(IApiClient, IOperation)"/>
    public bool HasAccess(IApiClient apiClient, IOperation operation)
    {
        if (_AuthorizationFactory == null) return false;
        if (apiClient == null) return false;
        if (operation == null) return false;
        if (!apiClient.IsValid) return false;
        if (!operation.IsEnabled) return false;

        return HasAccess(apiClient, operation.Service.Name, operation.Name);
    }

    /// <inheritdoc cref="IAuthorizationVerifier.HasAccess(IApiClient, string, string)"/>
    public bool HasAccess(IApiClient apiClient, string serviceName, string operationName)
    {
        if (_AuthorizationFactory == null) return false;
        if (apiClient == null) return false;
        if (!apiClient.IsValid) return false;

        var consol = _AuthorizationFactory.GetAllByApiClient(apiClient);

        return HasAccess(consol, serviceName, operationName);
    }

    /// <inheritdoc cref="IAuthorizationVerifier.HasAccess(ICollection{IAuthorization}, string, string)"/>
    public bool HasAccess(ICollection<IAuthorization> consol, string serviceName, string operationName)
    {
        /* 
        
        Information on AuthorizationTypes

        ServiceAuthorizations:
        None --> The client is completely denied from the service, even if it has operation authorizations.
        Partial --> Usually this is set by apicontrolplane service, it is neither disabled nor full, but should have operation authorizations.
        Full --> The client has full access to the service, it can access any operation in the service, even those explicitly set as None.

        OperationAuthorizations:
        None --> The client is completely denied from the operation, usually this can be handled by just deleting the operation authorization, 
                 hence why updating this to none from the apicontrolplane dashboard will delete the authorization instead of updating it.
        Partial --> Means nothing for operation authorizations, acts the same as Full.
        Full --> The client has access to execute the specified operation.

        */

        var authorizations = consol.Where(auth => auth.ServiceName == serviceName);

        var isCompletelyDisabled = authorizations.Any(
            auth => auth.IsServiceAuthorization() && 
                    auth.AuthorizationType == AuthorizationTypeEnum.None
        );
        if (isCompletelyDisabled) return false;
        
        var isForService = authorizations.Any(
            auth => auth.IsServiceAuthorization() && 
                    auth.AuthorizationType == AuthorizationTypeEnum.Full
        );
        if (isForService) return true;

        var deniedAccessToOperation = authorizations.Any(
            auth => auth.IsOperationAuthorization() &&
                    auth.Operation.Name == operationName &&
                    auth.AuthorizationType == AuthorizationTypeEnum.None
        );
        if (deniedAccessToOperation) return false;

        return authorizations.Any(
            auth => auth.IsOperationAuthorization() && 
                    auth.Operation.Name == operationName && 
                    auth.AuthorizationType != AuthorizationTypeEnum.None
        );
    }
}
