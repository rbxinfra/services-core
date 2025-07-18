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
/// Business Logic Layer for <see cref="OperationAuthorization"/>
/// </summary>
[DebuggerDisplay("{Operation.Name,nq}: {ApiClient.Note}, {AuthorizationType,nq}")]
internal class OperationAuthorization : IRobloxEntity<int, OperationAuthorizationDAL>, IOperationAuthorization
{
    private OperationAuthorizationDAL _EntityDAL;

    /// <inheritdoc cref="IOperationAuthorization.ID"/>
    public int ID => _EntityDAL.ID;

    internal int OperationID
    {
        get => _EntityDAL.OperationID;
        set => _EntityDAL.OperationID = value;
    }

    /// <inheritdoc cref="IOperationAuthorization.Operation"/>
    public IOperation Operation
    {
        get => Entities.Operation.Get(OperationID);
        set => OperationID = value.ID;
    }

    internal int ApiClientID
    {
        get => _EntityDAL.ApiClientID;
        set => _EntityDAL.ApiClientID = value;
    }

    /// <inheritdoc cref="IOperationAuthorization.ApiClient"/>
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

    /// <inheritdoc cref="IOperationAuthorization.AuthorizationType"/>
    public AuthorizationTypeEnum AuthorizationType
    {
        get => Entities.AuthorizationType.Get(AuthorizationTypeID);
        set => AuthorizationTypeID = (byte)value;
    }

    /// <inheritdoc cref="IOperationAuthorization.Created"/>
    public DateTime Created => _EntityDAL.Created;

    /// <inheritdoc cref="IOperationAuthorization.Updated"/>
    public DateTime Updated => _EntityDAL.Updated;

    /// <summary>
    /// Construct a new instance of <see cref="OperationAuthorization"/>
    /// </summary>
    public OperationAuthorization()
    {
        _EntityDAL = new OperationAuthorizationDAL();
    }

    /// <inheritdoc cref="IOperationAuthorization.IsDisabled"/>
    public bool IsDisabled() => AuthorizationType == AuthorizationTypeEnum.None;

    /// <inheritdoc cref="IOperationAuthorization.IsEnabled"/>
    public bool IsEnabled() => AuthorizationType == AuthorizationTypeEnum.Full || AuthorizationType == AuthorizationTypeEnum.Partial;

    /// <inheritdoc cref="IOperationAuthorization.IsOwnedByService(IService)"/>
    public bool IsOwnedByService(IService service) => IsOwnedByService(service.ID);
    internal bool IsOwnedByService(int serviceID) => Operation.Service.ID == serviceID;

    /// <summary>
    /// Creates a new <see cref="OperationAuthorization"/> for the given <see cref="Operation"/> and <see cref="ApiClient"/>.
    /// </summary>
    /// <param name="operationID">The <see cref="Operation.ID"/>.</param>
    /// <param name="apiClientID">The <see cref="ApiClient.ID"/>.</param>
    /// <param name="authorizationTypeID">The <see cref="AuthorizationType.ID"/>.</param>
    /// <returns>A new <see cref="OperationAuthorization"/>.</returns>
    public static IOperationAuthorization CreateNew(int operationID, int apiClientID, byte authorizationTypeID)
    {
        var operationAuthorization = new OperationAuthorization();
        operationAuthorization.OperationID = operationID;
        operationAuthorization.ApiClientID = apiClientID;
        operationAuthorization.AuthorizationTypeID = authorizationTypeID;

        operationAuthorization.Save();
        return operationAuthorization;
    }

    /// <inheritdoc cref="IOperationAuthorization.Delete"/>
    public void Delete()
    {
        EntityHelper.DeleteEntity(
            this,
            _EntityDAL.Delete
        );
    }

    /// <inheritdoc cref="IOperationAuthorization.Save"/>
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
    /// Get an <see cref="OperationAuthorization"/> by ID.
    /// </summary>
    /// <param name="id">The ID of the <see cref="OperationAuthorization"/>.</param>
    /// <returns>The <see cref="OperationAuthorization"/>.</returns>
    public static IOperationAuthorization Get(int id)
    {
        return EntityHelper.GetEntity<int, OperationAuthorizationDAL, OperationAuthorization>(
            EntityCacheInfo,
            id,
            () => OperationAuthorizationDAL.Get(id)
        );
    }

    /// <summary>
    /// Get an <see cref="OperationAuthorization"/> by <see cref="Operation.ID"/> and <see cref="ApiClient.ID"/>.
    /// </summary>
    /// <param name="operationID">The <see cref="Operation.ID"/>.</param>
    /// <param name="apiClientID">The <see cref="ApiClient.ID"/>.</param>
    /// <returns>The <see cref="OperationAuthorization"/>.</returns>
    public static IOperationAuthorization GetByOperationIDAndApiClientID(int operationID, int apiClientID)
    {
        return EntityHelper.GetEntityByLookup<int, OperationAuthorizationDAL, OperationAuthorization>(
            EntityCacheInfo,
            string.Format("OperationAuthorizationOperationID:{0}_OperationAuthorizationApiClientID:{1}", operationID, apiClientID),
            () => OperationAuthorizationDAL.GetByOperationIDAndApiClientID(operationID, apiClientID)
        );
    }

    /// <summary>
    /// Gets the total number of <see cref="OperationAuthorization"/>s by <see cref="Operation.ID"/>.
    /// </summary>
    /// <param name="apiClientID">The <see cref="Operation.ID"/>.</param>
    /// <returns>The total number of <see cref="OperationAuthorization"/>s.</returns>
    public static int GetTotalNumberOfOperationAuthorizationsByOperationID(int apiClientID)
    {
        var countId = string.Format("GetTotalNumberOfOperationAuthorizationsByOperationID_OperationID:{0}", apiClientID);

        return EntityHelper.GetEntityCount<int>(
            EntityCacheInfo,
            CacheManager.BuildQualifiedCachePolicy(string.Format("OperationID:{0}", apiClientID)),
            countId,
            () => OperationAuthorizationDAL.GetTotalNumberOfOperationAuthorizationsByOperationID(apiClientID)
        );
    }

    /// <summary>
    /// Get all <see cref="OperationAuthorization"/>s by <see cref="Operation.ID"/> paged.
    /// </summary>
    /// <param name="operationID">The <see cref="Operation.ID"/>.</param>
    /// <param name="startRowIndex">The start row index.</param>
    /// <param name="maximumRows">The maximum rows.</param>
    /// <returns>The <see cref="OperationAuthorization"/>s.</returns>
    public static ICollection<IOperationAuthorization> GetOperationAuthorizationsByOperationIDPaged(int operationID, int startRowIndex, int maximumRows)
    {
        string collectionId = string.Format("GetOperationAuthorizationsByOperationIDPaged_OperationID:{0}_StartRowIndex:{1}_MaximumRows:{2}", operationID, startRowIndex, maximumRows);
        return EntityHelper.GetEntityCollection<IOperationAuthorization, int>(
            EntityCacheInfo,
            CacheManager.BuildQualifiedCachePolicy(string.Format("OperationID:{0}", operationID)),
            collectionId,
            () =>
            {
                return OperationAuthorizationDAL.GetOperationAuthorizationsByOperationID(
                    operationID,
                    startRowIndex + 1,
                    maximumRows
                );
            },
            Get
        );
    }

    /// <summary>
    /// Gets the total number of <see cref="OperationAuthorization"/>s by <see cref="ApiClient.ID"/>.
    /// </summary>
    /// <param name="apiClientID">The <see cref="ApiClient.ID"/>.</param>
    /// <returns>The total number of <see cref="OperationAuthorization"/>s.</returns>
    public static int GetTotalNumberOfOperationAuthorizationsByApiClientID(int apiClientID)
    {
        var countId = string.Format("GetTotalNumberOfOperationAuthorizationsByApiClientID_ApiClientID:{0}", apiClientID);

        return EntityHelper.GetEntityCount<int>(
            EntityCacheInfo,
            CacheManager.BuildQualifiedCachePolicy(string.Format("ApiClientID:{0}", apiClientID)),
            countId,
            () => OperationAuthorizationDAL.GetTotalNumberOfOperationAuthorizationsByApiClientID(apiClientID)
        );
    }

    /// <summary>
    /// Get all <see cref="OperationAuthorization"/>s by <see cref="ApiClient.ID"/> paged.
    /// </summary>
    /// <param name="apiClientID">The <see cref="ApiClient.ID"/>.</param>
    /// <param name="startRowIndex">The start row index.</param>
    /// <param name="maximumRows">The maximum rows.</param>
    /// <returns>The <see cref="OperationAuthorization"/>s.</returns>
    public static ICollection<IOperationAuthorization> GetOperationAuthorizationsByApiClientIDPaged(int apiClientID, int startRowIndex, int maximumRows)
    {
        string collectionId = string.Format("GetOperationAuthorizationsByApiClientIDPaged_ApiClientID:{0}_StartRowIndex:{1}_MaximumRows:{2}", apiClientID, startRowIndex, maximumRows);
        return EntityHelper.GetEntityCollection<IOperationAuthorization, int>(
            EntityCacheInfo,
            CacheManager.BuildQualifiedCachePolicy(string.Format("ApiClientID:{0}", apiClientID)),
            collectionId,
            () =>
            {
                return OperationAuthorizationDAL.GetOperationAuthorizationsByApiClientID(
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
    public void Construct(OperationAuthorizationDAL dal)
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
    /// The <see cref="ICacheInfo"/> for the <see cref="OperationAuthorization"/>.
    /// </summary>
    public static CacheInfo EntityCacheInfo = new CacheInfo(
        new CacheabilitySettings(true, true, true, true, false, true),
        typeof(OperationAuthorization).ToString(),
        false
    );

    /// <inheritdoc cref="ICacheableObject.BuildEntityIDLookups"/>
    public IEnumerable<string> BuildEntityIDLookups()
    {
        if (_EntityDAL != null)
            yield return string.Format("OperationAuthorizationOperationID:{0}_OperationAuthorizationApiClientID:{1}", Operation.ID, ApiClient.ID);
        yield break;
    }

    /// <inheritdoc cref="ICacheableObject.BuildStateTokenCollection"/>
    public IEnumerable<StateToken> BuildStateTokenCollection()
    {
        yield return new StateToken(string.Format("OperationID:{0}", OperationID));
        yield return new StateToken(string.Format("ApiClientID:{0}", ApiClientID));
        yield break;
    }

    #endregion ICacheableObject Members
}
