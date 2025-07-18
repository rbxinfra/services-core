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
/// Business Logic Layer for <see cref="Service"/>
/// </summary>
[DebuggerDisplay("{Name,nq}: {StatusType,nq}")]
internal class Service : IRobloxEntity<int, ServiceDAL>, IService
{
    private ServiceDAL _EntityDAL;

    /// <inheritdoc cref="IService.ID"/>
    public int ID => _EntityDAL.ID;

    /// <inheritdoc cref="IService.Name"/>
    public string Name
    {
        get => _EntityDAL.Name;
        set => _EntityDAL.Name = value;
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

    /// <inheritdoc cref="IService.IsEnabled"/>
    public bool IsEnabled => StatusType == StatusTypeEnum.Enabled;

    /// <inheritdoc cref="IService.Created"/>
    public DateTime Created => _EntityDAL.Created;

    /// <inheritdoc cref="IService.Updated"/>
    public DateTime Updated => _EntityDAL.Updated;

    /// <summary>
    /// Construct a new instance of <see cref="Service"/>
    /// </summary>
    public Service()
    {
        _EntityDAL = new ServiceDAL();
    }

    /// <inheritdoc cref="IService.Disable"/>
    public void Disable()
    {
        if (!IsEnabled)
        {
            StatusType = StatusTypeEnum.Disabled;
            Save();
        }
    }

    /// <inheritdoc cref="IService.Enable"/>
    public void Enable()
    {
        if (IsEnabled)
        {
            StatusType = StatusTypeEnum.Enabled;
            Save();
        }
    }

    /// <summary>
    /// Creates a new <see cref="Service"/>.
    /// </summary>
    /// <param name="name">The name of the <see cref="Service"/>.</param>
    /// <param name="isEnabled">Whether the <see cref="Service"/> is enabled.</param>
    /// <returns>The new <see cref="Service"/>.</returns>
    public static IService CreateNew(string name, bool isEnabled)
    {
        var service = new Service();
        service.Name = name;
        service.StatusType = isEnabled ? StatusTypeEnum.Enabled : StatusTypeEnum.Disabled;
        service.Save();

        return service;
    }

    /// <inheritdoc cref="IService.Delete"/>
    public void Delete()
    {
        EntityHelper.DeleteEntity(
            this,
            _EntityDAL.Delete
        );
    }

    /// <inheritdoc cref="IService.Save"/>
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
    /// Get a <see cref="Service"/> by ID.
    /// </summary>
    /// <param name="id">The ID of the <see cref="Service"/>.</param>
    /// <returns>The <see cref="Service"/>.</returns>
    public static IService Get(int id)
    {
        return EntityHelper.GetEntity<int, ServiceDAL, Service>(
            EntityCacheInfo,
            id,
            () => ServiceDAL.Get(id)
        );
    }

    /// <summary>
    /// Get a <see cref="Service"/> by name.
    /// </summary>
    /// <param name="name">The name of the <see cref="Service"/>.</param>
    /// <returns>The <see cref="Service"/>.</returns>
    public static IService GetByName(string name)
    {
        return EntityHelper.GetEntityByLookup<int, ServiceDAL, Service>(
            EntityCacheInfo,
            string.Format("ServiceName:{0}", name),
            () => ServiceDAL.GetByName(name)
        );
    }

    /// <summary>
    /// Gets the total number of <see cref="Service"/>
    /// </summary>
    /// <returns>The total number of <see cref="Service"/></returns>
    public static int GetTotalNumberOfServices()
    {
        var countId = "GetTotalNumberOfServices";

        return EntityHelper.GetEntityCount<int>(
            EntityCacheInfo,
            CacheManager.UnqualifiedNonExpiringCachePolicy,
            countId,
            () => ServiceDAL.GetTotalNumberOfServices()
        );
    }

    /// <summary>
    /// Get a paged collection of <see cref="Service"/>s.
    /// </summary>
    /// <param name="startRowIndex">The start row index.</param>
    /// <param name="maximumRows">The maximum rows.</param>
    /// <returns>A paged collection of <see cref="Service"/>s.</returns>
    public static ICollection<IService> GetServicesPaged(int startRowIndex, int maximumRows)
    {
        string collectionId = string.Format("GetServicesPaged_StartRowIndex:{0}_MaximumRows:{1}", startRowIndex, maximumRows);
        return EntityHelper.GetEntityCollection<IService, int>(
            EntityCacheInfo,
            CacheManager.UnqualifiedNonExpiringCachePolicy,
            collectionId,
            () =>
            {
                return ServiceDAL.GetAllPaged(
                    startRowIndex + 1,
                    maximumRows
                );
            },
            Get
        );
    }

    #region IRobloxEntity Members

    /// <inheritdoc cref="IRobloxEntity{TIndex, TDal}.Construct(TDal)"/>
    public void Construct(ServiceDAL dal)
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
    /// The <see cref="ICacheInfo"/> for the <see cref="Service"/>.
    /// </summary>
    public static CacheInfo EntityCacheInfo = new CacheInfo(
        new CacheabilitySettings(true, true, true, true, true, true),
        typeof(Service).ToString(),
        false
    );

    /// <inheritdoc cref="ICacheableObject.BuildEntityIDLookups"/>
    public IEnumerable<string> BuildEntityIDLookups()
    {
        if (_EntityDAL != null)
            yield return string.Format("ServiceName:{0}", Name);
        yield break;
    }

    /// <inheritdoc cref="ICacheableObject.BuildEntityIDLookups"/>
    public IEnumerable<StateToken> BuildStateTokenCollection()
    {
        yield break;
    }

    #endregion ICacheableObject Members
}
