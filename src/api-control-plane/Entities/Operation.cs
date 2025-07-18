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
/// Business Logic Layer for <see cref="Operation"/>
/// </summary>
[DebuggerDisplay("{Service.Name,nq}.{Name,nq}")]
internal class Operation : IRobloxEntity<int, OperationDAL>, IOperation
{
    private OperationDAL _EntityDAL;

    /// <inheritdoc cref="IOperation.ID"/>
    public int ID => _EntityDAL.ID;

    /// <inheritdoc cref="IOperation.Name"/>
    public string Name
    {
        get => _EntityDAL.Name;
        set => _EntityDAL.Name = value;
    }

    internal int ServiceID
    {
        get => _EntityDAL.ServiceID;
        set => _EntityDAL.ServiceID = value;
    }

    /// <inheritdoc cref="IOperation.Service"/>
    public IService Service
    {
        get => Entities.Service.Get(ServiceID);
        set => ServiceID = value.ID;
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

    /// <inheritdoc cref="IOperation.IsEnabled"/>
    public bool IsEnabled => StatusType == StatusTypeEnum.Enabled;

    /// <inheritdoc cref="IOperation.Created"/>
    public DateTime Created => _EntityDAL.Created;

    /// <inheritdoc cref="IOperation.Updated"/>
    public DateTime Updated => _EntityDAL.Updated;

    /// <summary>
    /// Construct a new instance of <see cref="Operation"/>
    /// </summary>
    public Operation()
    {
        _EntityDAL = new OperationDAL();
    }

    /// <inheritdoc cref="IOperation.Disable"/>
    public void Disable()
    {
        if (!IsEnabled)
        {
            StatusType = StatusTypeEnum.Disabled;
            Save();
        }
    }

    /// <inheritdoc cref="IOperation.Enable"/>
    public void Enable()
    {
        if (IsEnabled)
        {
            StatusType = StatusTypeEnum.Enabled;
            Save();
        }
    }

    /// <summary>
    /// Create a new <see cref="Operation"/> with the specified <paramref name="name"/>, <paramref name="serviceID"/>, and <paramref name="isEnabled"/>.
    /// </summary>
    /// <param name="name">The name of the <see cref="Operation"/>.</param>
    /// <param name="serviceID">The ID of the <see cref="Service"/> the <see cref="Operation"/> belongs to.</param>
    /// <param name="isEnabled">Whether or not the <see cref="Operation"/> is enabled.</param>
    /// <returns>The new <see cref="Operation"/>.</returns>
    public static IOperation CreateNew(string name, int serviceID, bool isEnabled)
    {
        var operation = new Operation();
        operation.Name = name;
        operation.ServiceID = serviceID;
        operation.StatusType = isEnabled ? StatusTypeEnum.Enabled : StatusTypeEnum.Disabled;
        operation.Save();

        return operation;
    }

    /// <inheritdoc cref="IOperation.Delete"/>
    public void Delete()
    {
        EntityHelper.DeleteEntity(
            this,
            _EntityDAL.Delete
        );
    }

    /// <inheritdoc cref="IOperation.Save"/>
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
    /// Get an <see cref="Operation"/> by ID.
    /// </summary>
    /// <param name="id">The ID of the <see cref="Operation"/>.</param>
    /// <returns>The <see cref="Operation"/>.</returns>
    public static IOperation Get(int id)
    {
        return EntityHelper.GetEntity<int, OperationDAL, Operation>(
            EntityCacheInfo,
            id,
            () => OperationDAL.Get(id)
        );
    }

    /// <summary>
    /// Get an <see cref="Operation"/> by <paramref name="serviceID"/> and <paramref name="name"/>.
    /// </summary>
    /// <param name="serviceID">The ID of the <see cref="Service"/> the <see cref="Operation"/> belongs to.</param>
    /// <param name="name">The name of the <see cref="Operation"/>.</param>
    /// <returns>The <see cref="Operation"/>.</returns>
    public static IOperation GetByServiceIDAndName(int serviceID, string name)
    {
        return EntityHelper.GetEntityByLookup<int, OperationDAL, Operation>(
            EntityCacheInfo,
            string.Format("OperationServiceID:{0}_OperationName:{1}", serviceID, name),
            () => OperationDAL.GetByServiceIDAndName(serviceID, name)
        );
    }

    /// <summary>
    /// Gets the total number of <see cref="Operation"/>s by <paramref name="serviceID"/>
    /// </summary>
    /// <param name="serviceID">The ID of the <see cref="Service"/> the <see cref="Operation"/>s belong to.</param>
    /// <returns>The total number of <see cref="Operation"/></returns>
    public static int GetTotalNumberOfOperationsByServiceID(int serviceID)
    {
        var countId = string.Format("GetTotalNumberOfOperationsByServiceID_ServiceID:{0}", serviceID);

        return EntityHelper.GetEntityCount<int>(
            EntityCacheInfo,
            CacheManager.BuildQualifiedCachePolicy(string.Format("ServiceID:{0}", serviceID)),
            countId,
            () => OperationDAL.GetTotalNumberOfOperationsByServiceID(serviceID)
        );
    }

    /// <summary>
    /// Get all <see cref="Operation"/>s by <paramref name="serviceID"/> paged.
    /// </summary>
    /// <param name="serviceID">The ID of the <see cref="Service"/> the <see cref="Operation"/>s belong to.</param>
    /// <param name="startRowIndex">The start row index.</param>
    /// <param name="maximumRows">The maximum rows.</param>
    /// <returns>The <see cref="Operation"/>s.</returns>
    public static ICollection<IOperation> GetOperationsByServiceIDPaged(int serviceID, int startRowIndex, int maximumRows)
    {
        string collectionId = string.Format("GetOperationsByServiceIDPaged_ServiceID:{0}_StartRowIndex:{1}_MaximumRows:{2}", serviceID, startRowIndex, maximumRows);
        return EntityHelper.GetEntityCollection<IOperation, int>(
            EntityCacheInfo,
            CacheManager.BuildQualifiedCachePolicy(string.Format("ServiceID:{0}", serviceID)),
            collectionId,
            () =>
            {
                return OperationDAL.GetOperationsByServiceID(
                    serviceID,
                    startRowIndex + 1,
                    maximumRows
                );
            },
            Get
        );
    }

    #region IRobloxEntity Members

    /// <inheritdoc cref="IRobloxEntity{TIndex, TDal}.Construct(TDal)"/>
    public void Construct(OperationDAL dal)
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
    /// The <see cref="ICacheInfo"/> for <see cref="Operation"/>.
    /// </summary>
    public static CacheInfo EntityCacheInfo = new CacheInfo(
        new CacheabilitySettings(true, true, true, true, false, true),
        typeof(Operation).ToString(),
        false
    );

    /// <inheritdoc cref="ICacheableObject.BuildEntityIDLookups"/>
    public IEnumerable<string> BuildEntityIDLookups()
    {
        if (_EntityDAL != null)
        {
            yield return string.Format("OperationServiceID:{0}_OperationName:{1}", Service.ID, Name);
        }
        yield break;
    }

    /// <inheritdoc cref="ICacheableObject.BuildStateTokenCollection"/>
    public IEnumerable<StateToken> BuildStateTokenCollection()
    {
        yield return new StateToken(string.Format("ServiceID:{0}", ServiceID));
        yield break;
    }

    #endregion ICacheableObject Members
}
