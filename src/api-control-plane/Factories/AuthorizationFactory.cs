namespace Roblox.Api.ControlPlane.Factories;

using System;
using System.Collections.Generic;

using IApiClient = Service.ApiControlPlane.IApiClient;
using Authorization = Service.ApiControlPlane.Authorization;
using IAuthorization = Service.ApiControlPlane.IAuthorization;

/// <inheritdoc cref="IServiceFactory"/>
public class AuthorizationFactory : IAuthorizationFactory
{
    private readonly IOperationAuthorizationFactory _OperationAuthorizationFactory;
    private readonly IServiceAuthorizationFactory _ServiceAuthorizationFactory;

    /// <summary>
    /// Construct a new instance of <see cref="AuthorizationFactory"/>
    /// </summary>
    /// <param name="operationAuthorizationFactory">The operation authorization factory.</param>
    /// <param name="serviceAuthorizationFactory">The service authorization factory.</param>
    /// <exception cref="ArgumentNullException">
    /// - <paramref name="operationAuthorizationFactory"/> cannot be null.
    /// - <paramref name="serviceAuthorizationFactory"/> cannot be null.
    /// </exception>
    public AuthorizationFactory(
        IOperationAuthorizationFactory operationAuthorizationFactory,
        IServiceAuthorizationFactory serviceAuthorizationFactory
    )
    {
        _OperationAuthorizationFactory = operationAuthorizationFactory ?? throw new ArgumentNullException(nameof(operationAuthorizationFactory));
        _ServiceAuthorizationFactory = serviceAuthorizationFactory ?? throw new ArgumentNullException(nameof(serviceAuthorizationFactory));
    }

    /// <inheritdoc cref="IServiceAuthorizationFactory.GetTotalNumberByApiClient(IApiClient)"/>
    public int GetTotalNumberByApiClient(IApiClient apiClient)
    {
        if (apiClient == null)
            throw new ArgumentNullException(nameof(apiClient));

        return _ServiceAuthorizationFactory.GetTotalNumberByApiClient(apiClient) + _OperationAuthorizationFactory.GetTotalNumberByApiClient(apiClient);
    }

    /// <inheritdoc cref="IServiceAuthorizationFactory.GetAllByApiClient(IApiClient)"/>
    public ICollection<IAuthorization> GetAllByApiClient(IApiClient apiClient)
        => GetAllByApiClient_Paged(apiClient, 1, GetTotalNumberByApiClient(apiClient));

    /// <inheritdoc cref="IAuthorizationFactory.GetAllByApiClient_Paged(IApiClient, int, int)"/>
    public ICollection<IAuthorization> GetAllByApiClient_Paged(IApiClient apiClient, int startRowIndex, int maximumRows)
    {
        if (apiClient == null)
            throw new ArgumentNullException(nameof(apiClient));

        var authorizations = new List<IAuthorization>();

        foreach (var serviceAuthorization in _ServiceAuthorizationFactory.GetAllByApiClient_Paged(apiClient, startRowIndex, maximumRows))
            authorizations.Add(new Authorization(serviceAuthorization));
        foreach (var operationAuthorization in _OperationAuthorizationFactory.GetAllByApiClient_Paged(apiClient, startRowIndex, maximumRows))
            authorizations.Add(new Authorization(operationAuthorization));

        return authorizations;
    }
}
