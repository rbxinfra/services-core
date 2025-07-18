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
/// Business Logic Layer for <see cref="ApiClient"/>
/// </summary>
[DebuggerDisplay("{Note}")]
internal class ApiClient : IRobloxEntity<int, ApiClientDAL>, IApiClient
{
    private ApiClientDAL _EntityDAL;

    /// <inheritdoc cref="IApiClient.ID"/>
    public int ID => _EntityDAL.ID;

    /// <inheritdoc cref="IApiClient.ApiKey"/>
    public Guid ApiKey
    {
        get => _EntityDAL.ApiKey;
        set => _EntityDAL.ApiKey = value;
    }

    /// <inheritdoc cref="IApiClient.Note"/>
    public string Note
    {
        get => _EntityDAL.Note;
        set => _EntityDAL.Note = value;
    }

    internal byte StatusTypeID
    {
        get => _EntityDAL.StatusTypeID;
        set => _EntityDAL.StatusTypeID = value;
    }

    internal StatusTypeEnum StatusType
    {
        get => Entities.StatusType.Get(StatusTypeID);
        set => StatusTypeID = (byte)value;
    }

    /// <inheritdoc cref="IApiClient.IsValid"/>
    public bool IsValid => StatusType == StatusTypeEnum.Enabled;

    /// <inheritdoc cref="IApiClient.Created"/>
    public DateTime Created => _EntityDAL.Created;

    /// <inheritdoc cref="IApiClient.Updated"/>
    public DateTime Updated => _EntityDAL.Updated;

    /// <summary>
    /// Construct a new instance of <see cref="ApiClient"/>
    /// </summary>
    public ApiClient()
    {
        _EntityDAL = new ApiClientDAL();
    }

    /// <inheritdoc cref="IApiClient.SetValid"/>
    public void SetValid()
    {
        if (!IsValid)
        {
            StatusType = StatusTypeEnum.Enabled;
            Save();
        }
    }

    /// <inheritdoc cref="IApiClient.SetInvalid"/>
    public void SetInvalid()
    {
        if (IsValid)
        {
            StatusType = StatusTypeEnum.Disabled;
            Save();
        }
    }

    /// <summary>
    /// Creates a new <see cref="ApiClient"/>.
    /// </summary>
    /// <param name="apiKey">The <see cref="Guid"/> of the <see cref="ApiClient"/>.</param>
    /// <param name="note">The note of the <see cref="ApiClient"/>.</param>
    /// <param name="isValid">The validity of the <see cref="ApiClient"/>.</param>
    /// <returns>The new <see cref="ApiClient"/>.</returns>
    public static IApiClient CreateNew(Guid apiKey, string note, bool isValid)
    {
        var apiClient = new ApiClient();
        apiClient.ApiKey = apiKey;
        apiClient.Note = note;
        apiClient.StatusType = isValid ? StatusTypeEnum.Enabled : StatusTypeEnum.Disabled;
        apiClient.Save();

        return apiClient;
    }

    /// <inheritdoc cref="IApiClient.Delete"/>
    public void Delete()
    {
        EntityHelper.DeleteEntity(
            this,
            _EntityDAL.Delete
        );
    }

    /// <inheritdoc cref="IApiClient.Save"/>
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
    /// Get an <see cref="ApiClient"/> by ID.
    /// </summary>
    /// <param name="id">The ID of the <see cref="ApiClient"/>.</param>
    /// <returns>The <see cref="ApiClient"/>.</returns>
    public static IApiClient Get(int id)
    {
        return EntityHelper.GetEntity<int, ApiClientDAL, ApiClient>(
            EntityCacheInfo,
            id,
            () => ApiClientDAL.Get(id)
        );
    }

    /// <summary>
    /// Get an <see cref="ApiClient"/> by <paramref name="apiKey"/>.
    /// </summary>
    /// <param name="apiKey">The <see cref="Guid"/> of the <see cref="ApiClient"/>.</param>
    /// <returns>The <see cref="ApiClient"/>.</returns>
    public static IApiClient GetByApiKey(Guid apiKey)
    {
        return EntityHelper.GetEntityByLookup<int, ApiClientDAL, ApiClient>(
            EntityCacheInfo,
            string.Format("ApiClientApiKey:{0}", apiKey),
            () => ApiClientDAL.GetByApiKey(apiKey)
        );
    }

    /// <summary>
    /// Get an <see cref="ApiClient"/> by <paramref name="note"/>.
    /// </summary>
    /// <param name="note">The note of the <see cref="ApiClient"/>.</param>
    /// <returns>The <see cref="ApiClient"/>.</returns>
    public static IApiClient GetByNote(string note)
    {
        return EntityHelper.GetEntityByLookup<int, ApiClientDAL, ApiClient>(
            EntityCacheInfo,
            string.Format("ApiClientNote:{0}", note),
            () => ApiClientDAL.GetByNote(note)
        );
    }

    /// <summary>
    /// Gets the total number of <see cref="ApiClient"/>
    /// </summary>
    /// <returns>The total number of <see cref="ApiClient"/></returns>
    public static int GetTotalNumberOfApiClients()
    {
        var countId = "GetTotalNumberOfApiClients";

        return EntityHelper.GetEntityCount<int>(
            EntityCacheInfo,
            CacheManager.UnqualifiedNonExpiringCachePolicy,
            countId,
            () => ApiClientDAL.GetTotalNumberOfApiClients()
        );
    }

    /// <summary>
    /// Get all <see cref="ApiClient"/>s paged.
    /// </summary>
    /// <param name="startRowIndex">The start row index.</param>
    /// <param name="maximumRows">The maximum rows.</param>
    /// <returns>All <see cref="ApiClient"/>s paged.</returns>
    public static ICollection<IApiClient> GetApiClientsPaged(int startRowIndex, int maximumRows)
    {
        string collectionId = string.Format("GetApiClientsPaged_StartRowIndex:{0}_MaximumRows:{1}", startRowIndex, maximumRows);
        return EntityHelper.GetEntityCollection<IApiClient, int>(
            EntityCacheInfo,
            CacheManager.UnqualifiedNonExpiringCachePolicy,
            collectionId,
            () =>
            {
                return ApiClientDAL.GetAllPaged(
                    startRowIndex + 1,
                    maximumRows
                );
            },
            Get
        );
    }

    #region IRobloxEntity Members

    /// <inheritdoc cref="IRobloxEntity{TIndex, TDal}.Construct(TDal)"/>
    public void Construct(ApiClientDAL dal)
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
    /// The <see cref="ICacheInfo"/> for <see cref="ApiClient"/>.
    /// </summary>
    public static CacheInfo EntityCacheInfo = new CacheInfo(
        new CacheabilitySettings(true, true, true, true, true, true),
        typeof(ApiClient).ToString(),
        false
    );

    /// <inheritdoc cref="ICacheableObject.BuildEntityIDLookups"/>
    public IEnumerable<string> BuildEntityIDLookups()
    {
        if (_EntityDAL != null)
        {
            yield return string.Format("ApiClientApiKey:{0}", ApiKey);
            yield return string.Format("ApiClientNote:{0}", Note);
        }
        yield break;
    }

    /// <inheritdoc cref="ICacheableObject.BuildStateTokenCollection"/>
    public IEnumerable<StateToken> BuildStateTokenCollection()
    {
        yield break;
    }

    #endregion ICacheableObject Members
}
