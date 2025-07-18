namespace Roblox.Service.ApiControlPlane;

/*

IAuthorizationFactory consolidates this together based on client SAs and OAs only in the case of DAA.
IAuthorizationVerifier checks this based on this condition:

var consol = _AuthorizationFactory.GetAllClientAuthorizations(apiKey);
var authorizations = consol.Where(auth => auth.ServiceName == serviceName);
var isForOperation = authorizations.FirstOrDefault(
                        auth => 
                            auth.IsOperationAuthorization() 
                                ? auth.OperationName == operationName && 
                                  auth.AuthorizationType != AuthorizationTypeEnum.None
                                : false
                     ) != null;
var isForService = authorizations.FirstOrDefault(
                        auth => 
                            auth.IsServiceAuthorization()
                                ? auth.AuthorizationType == AuthorizationTypeEnum.Full
                                : false
                    ) != null;

return isForOperation || isForService;
 
*/

/// <summary>
/// Represents the Authorization returned by the service
/// </summary>
public interface IAuthorization
{
    /// <summary>
    /// Gets the service name.
    /// </summary>
    string ServiceName { get; }

    /// <summary>
    /// Gets the API Client.
    /// </summary>
    IApiClient ApiClient { get; }

    /// <summary>
    /// Gets or sets the operation name, if this is null then it's a service authorization.
    /// </summary>
    IOperation Operation { get; }

    /// <summary>
    /// Gets or sets the authorization type of the authorization
    /// </summary>
    AuthorizationTypeEnum AuthorizationType { get; }

    /// <summary>
    /// Is this authorization for an operation? <see cref="Operation"/> must not be <c>null</c>.
    /// </summary>
    /// <returns>True if the authorization is for an operation.</returns>
    bool IsOperationAuthorization();
    
    /// <summary>
    /// Is this authorization for a service? <see cref="Operation"/> must be <c>null</c>.
    /// </summary>
    /// <returns>True if the authorization is for a service.</returns>
    bool IsServiceAuthorization();
}
