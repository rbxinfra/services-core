namespace Roblox.Api.ControlPlane.Entities;

using System;
using System.Diagnostics;
using System.Collections.Generic;

using Data;
using Caching;
using Data.Interfaces;
using Caching.Interfaces;

using Roblox.Service.ApiControlPlane;

/// <summary>
/// Business Logic Layer for <see cref="ServiceAuthorization"/>
/// </summary>
[DebuggerDisplay("{Service.Name,nq}: {ApiClient.Note}, {AuthorizationType,nq}")]
internal class ServiceAuthorization : IRobloxEntity<int, ServiceAuthorizationDAL>, IServiceAuthorization
{
    private ServiceAuthorizationDAL _EntityDAL;

    /// <inheritdoc cref="IServiceAuthorization.ID"/>
    public int ID => _EntityDAL.ID;

    internal int ServiceID
    {
        get => _EntityDAL.ServiceID;
        set => _EntityDAL.ServiceID = value;
    }

    /// <inheritdoc cref="IServiceAuthorization.Service"/>
    public IService Service
    {
        get => Entities.Service.Get(ServiceID);
        set => ServiceID = value.ID;
    }

    internal int ApiClientID
    {
        get => _EntityDAL.ApiClientID;
        set => _EntityDAL.ApiClientID = value;
    }

    /// <inheritdoc cref="IServiceAuthorization.ApiClient"/>
    public IApiClient ApiClient
    {
        get => Entities.ApiClient.Get(ApiClientID);
        set => ApiClientID = value.ID;
    }

    internal byte AuthorizationTypeID
    {
        get => _EntityDAL.AuthorizationTypeID;
        set => _EntityDAL.AuthorizationTypeID = value;
    }

    /// <inheritdoc cref="IServiceAuthorization.AuthorizationType"/>
    public AuthorizationTypeEnum AuthorizationType
    {
        get { return Entities.AuthorizationType.Get(AuthorizationTypeID); }
        set { AuthorizationTypeID = (byte)value; }
    }

    /// <inheritdoc cref="IServiceAuthorization.Created"/>
    public DateTime Created => _EntityDAL.Created;

    /// <inheritdoc cref="IServiceAuthorization.Updated"/>
    public DateTime Updated => _EntityDAL.Updated;

    /// <summary>
    /// Initializes a new instance of <see cref="ServiceAuthorization"/>.
    /// </summary>
    public ServiceAuthorization()
    {
        _EntityDAL = new ServiceAuthorizationDAL();
    }

    /// <inheritdoc cref="IServiceAuthorization.IsDisabled"/>
    public bool IsDisabled() => AuthorizationType == AuthorizationTypeEnum.None;

    /// <inheritdoc cref="IServiceAuthorization.IsEnabled"/>
    public bool IsEnabled() => AllowsFullServiceAccess() || HasOperationAuthorizations();

    /// <inheritdoc cref="IServiceAuthorization.HasOperationAuthorizations"/>
    public bool HasOperationAuthorizations() => AuthorizationType == AuthorizationTypeEnum.Partial;

    /// <inheritdoc cref="IServiceAuthorization.AllowsFullServiceAccess"/>
    public bool AllowsFullServiceAccess() => AuthorizationType == AuthorizationTypeEnum.Full;

    /// <summary>
    /// Create a new instance of <see cref="ServiceAuthorization"/>.
    /// </summary>
    /// <param name="serviceID">The ID of the <see cref="Service"/>.</param>
    /// <param name="apiClientID">The ID of the <see cref="ApiClient"/>.</param>
    /// <param name="authorizationTypeID">The ID of the <see cref="AuthorizationType"/>.</param>
    public static IServiceAuthorization CreateNew(int serviceID, int apiClientID, byte authorizationTypeID)
    {
        var serviceAuthorization = new ServiceAuthorization();
        serviceAuthorization.ServiceID = serviceID;
        serviceAuthorization.ApiClientID = apiClientID;
        serviceAuthorization.AuthorizationTypeID = authorizationTypeID;
        serviceAuthorization.Save();

        return serviceAuthorization;
    }

    /// <inheritdoc cref="IServiceAuthorization.Delete"/>
    public void Delete()
    {
        EntityHelper.DeleteEntity(
            this,
            _EntityDAL.Delete
        );
    }

    /// <inheritdoc cref="IServiceAuthorization.Save"/>
    public void Save()
    {
        EntityHelper.SaveEntity(
            this,
            () =>
            {
                _EntityDAL.Created = DateTime.Now;
                _EntityDAL.Updated = _EntityDAL.Created;
                _EntityDAL.Insert();
            },
            () =>
            {
                _EntityDAL.Updated = DateTime.Now;
                _EntityDAL.Update();
            }
        );
    }

    /// <summary>
    /// Get an <see cref="ServiceAuthorization"/> by ID.
    /// </summary>
    /// <param name="id">The ID of the <see cref="ServiceAuthorization"/>.</param>
    /// <returns>The <see cref="ServiceAuthorization"/>.</returns>
    public static IServiceAuthorization Get(int id)
    {
        return EntityHelper.GetEntity<int, ServiceAuthorizationDAL, ServiceAuthorization>(
            EntityCacheInfo,
            id,
            () => ServiceAuthorizationDAL.Get(id)
        );
    }

    /// <summary>
    /// Get an <see cref="ServiceAuthorization"/> by ServiceID and ApiClientID.
    /// </summary>
    /// <param name="serviceID">The ID of the <see cref="Service"/>.</param>
    /// <param name="apiClientID">The ID of the <see cref="ApiClient"/>.</param>
    /// <returns>The <see cref="ServiceAuthorization"/>.</returns>
    public static IServiceAuthorization GetByServiceIDAndApiClientID(int serviceID, int apiClientID)
    {
        return EntityHelper.GetEntityByLookup<int, ServiceAuthorizationDAL, ServiceAuthorization>(
            EntityCacheInfo,
            string.Format("ServiceAuthorizationServiceID:{0}_ServiceAuthorizationApiClientID:{1}", serviceID, apiClientID),
            () => ServiceAuthorizationDAL.GetByServiceIDAndApiClientID(serviceID, apiClientID)
        );
    }

    /// <summary>
    /// Gets the total number of <see cref="ServiceAuthorization"/>s by <see cref="Service.ID"/>.
    /// </summary>
    /// <param name="apiClientID">The <see cref="Service.ID"/>.</param>
    /// <returns>The total number of <see cref="ServiceAuthorization"/>s.</returns>
    public static int GetTotalNumberOfServiceAuthorizationsByServiceID(int apiClientID)
    {
        var countId = string.Format("GetTotalNumberOfServiceAuthorizationsByServiceID_ServiceID:{0}", apiClientID);

        return EntityHelper.GetEntityCount<int>(
            EntityCacheInfo,
            CacheManager.BuildQualifiedCachePolicy(string.Format("ServiceID:{0}", apiClientID)),
            countId,
            () => ServiceAuthorizationDAL.GetTotalNumberOfServiceAuthorizationsByServiceID(apiClientID)
        );
    }

    /// <summary>
    /// Gets the total number of <see cref="ServiceAuthorization"/>s by <see cref="ApiClient.ID"/>.
    /// </summary>
    /// <param name="apiClientID">The <see cref="ApiClient.ID"/>.</param>
    /// <returns>The total number of <see cref="ServiceAuthorization"/>s.</returns>
    public static int GetTotalNumberOfServiceAuthorizationsByApiClientID(int apiClientID)
    {
        var countId = string.Format("GetTotalNumberOfServiceAuthorizationsByApiClientID_ApiClientID:{0}", apiClientID);

        return EntityHelper.GetEntityCount<int>(
            EntityCacheInfo,
            CacheManager.BuildQualifiedCachePolicy(string.Format("ApiClientID:{0}", apiClientID)),
            countId,
            () => ServiceAuthorizationDAL.GetTotalNumberOfServiceAuthorizationsByApiClientID(apiClientID)
        );
    }

    /// <summary>
    /// Get all <see cref="ServiceAuthorization"/>s by ServiceID paged.
    /// </summary>
    /// <param name="serviceID">The ID of the <see cref="Service"/>.</param>
    /// <param name="startRowIndex">The start row index.</param>
    /// <param name="maximumRows">The maximum rows.</param>
    /// <returns>The <see cref="ServiceAuthorization"/>s.</returns>
    public static ICollection<IServiceAuthorization> GetServiceAuthorizationsByServiceIDPaged(int serviceID, int startRowIndex, int maximumRows)
    {
        string collectionId = string.Format("GetServiceAuthorizationsByServiceIDPaged_ServiceID:{0}_StartRowIndex:{1}_MaximumRows:{2}", serviceID, startRowIndex, maximumRows);
        return EntityHelper.GetEntityCollection<IServiceAuthorization, int>(
            EntityCacheInfo,
            CacheManager.UnqualifiedNonExpiringCachePolicy,
            collectionId,
            () =>
            {
                return ServiceAuthorizationDAL.GetServiceAuthorizationsByServiceID(
                    serviceID,
                    startRowIndex + 1,
                    maximumRows
                );
            },
            Get
        );
    }

    /// <summary>
    /// Get all <see cref="ServiceAuthorization"/>s by ApiClientID paged.
    /// </summary>
    /// <param name="apiClientID">The ID of the <see cref="ApiClient"/>.</param>
    /// <param name="startRowIndex">The start row index.</param>
    /// <param name="maximumRows">The maximum rows.</param>
    /// <returns>The <see cref="ServiceAuthorization"/>s.</returns>
    public static ICollection<IServiceAuthorization> GetServiceAuthorizationsByApiClientIDPaged(int apiClientID, int startRowIndex, int maximumRows)
    {
        string collectionId = string.Format("GetServiceAuthorizationsByApiClientIDPaged_ApiClientID:{0}_StartRowIndex:{1}_MaximumRows:{2}", apiClientID, startRowIndex, maximumRows);
        return EntityHelper.GetEntityCollection<IServiceAuthorization, int>(
            EntityCacheInfo,
            CacheManager.UnqualifiedNonExpiringCachePolicy,
            collectionId,
            () =>
            {
                return ServiceAuthorizationDAL.GetServiceAuthorizationsByApiClientID(
                    apiClientID,
                    startRowIndex + 1,
                    maximumRows
                );
            },
            Get
        );
    }

    #region IRobloxEntity Members

    /// <inheritdoc cref="IRobloxEntity{TIndex, TDal}.Construct(TDal)"/>
    public void Construct(ServiceAuthorizationDAL dal)
    {
        _EntityDAL = dal;
    }

    #endregion IRobloxEntity Members

    #region ICacheableObject Members

    /// <inheritdoc cref="ICacheableObject.CacheInfo"/>
    public CacheInfo CacheInfo
    {
        get { return EntityCacheInfo; }
    }

    /// <summary>
    /// The <see cref="ICacheInfo"/> for the <see cref="ServiceAuthorization"/>.
    /// </summary>
    public static CacheInfo EntityCacheInfo = new CacheInfo(
        new CacheabilitySettings(true, true, true, true, false, true),
        typeof(ServiceAuthorization).ToString(),
        false
    );

    /// <inheritdoc cref="ICacheableObject.BuildEntityIDLookups"/>
    public IEnumerable<string> BuildEntityIDLookups()
    {
        if (_EntityDAL != null)
            yield return string.Format("ServiceAuthorizationServiceID:{0}_ServiceAuthorizationApiClientID:{1}", Service.ID, ApiClient.ID);

        yield break;
    }

    /// <inheritdoc cref="ICacheableObject.BuildStateTokenCollection"/>
    public IEnumerable<StateToken> BuildStateTokenCollection()
    {
        yield return new StateToken(string.Format("ServiceID:{0}", ServiceID));
        yield return new StateToken(string.Format("ApiClientID:{0}", ApiClientID));

        yield break;
    }

    #endregion ICacheableObject Members
}
