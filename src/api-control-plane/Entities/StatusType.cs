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
/// Business logic for <see cref="StatusType"/>
/// </summary>
[DebuggerDisplay("Roblox.Api.ControlPlane.StatusType.{Value,nq}")]
internal class StatusType : IRobloxEntity<byte, StatusTypeDAL>
{
    private StatusTypeDAL _EntityDAL;

    /// <summary>
    /// Gets the <see cref="ID"/> of this <see cref="StatusType"/>.
    /// </summary>
    public byte ID => _EntityDAL.ID;

    /// <summary>
    /// Gets or sets the <see cref="Value"/> of this <see cref="StatusType"/>.
    /// </summary>
    public string Value
    {
        get => _EntityDAL.Value;
        set => _EntityDAL.Value = value;
    }

    /// <summary>
    /// Gets the <see cref="Created"/> of this <see cref="StatusType"/>.
    /// </summary>
    public DateTime Created => _EntityDAL.Created;

    /// <summary>
    /// Gets the <see cref="Updated"/> of this <see cref="StatusType"/>.
    /// </summary>
    public DateTime Updated => _EntityDAL.Updated;

    /// <summary>
    /// The ID of the <see cref="StatusTypeEnum.Disabled"/>.
    /// </summary>
    public static readonly byte DisabledID;

    /// <summary>
    /// The Value of the <see cref="StatusTypeEnum.Disabled"/>.
    /// </summary>
    public static readonly string DisabledValue = "Disabled";

    /// <summary>
    /// The ID of the <see cref="StatusTypeEnum.Enabled"/>.
    /// </summary>
    public static readonly byte EnabledID;

    /// <summary>
    /// The Value of the <see cref="StatusTypeEnum.Enabled"/>.
    /// </summary>
    public static readonly string EnabledValue = "Enabled";

    /// <summary>
    /// The supported <see cref="StatusType"/>s.
    /// </summary>
    public static HashSet<string> SupportedStatusTypes = new() { DisabledValue, EnabledValue };

    /// <summary>
    /// Construct a new instance of <see cref="StatusType"/>.
    /// </summary>
    public StatusType()
    {
        _EntityDAL = new StatusTypeDAL();
    }

    /// <summary>
    /// The static construct for <see cref="StatusType"/>.
    /// </summary>
    static StatusType()
    {
        DisabledID = MustGet(DisabledValue).ID;
        EnabledID = MustGet(EnabledValue).ID;
    }

    /// <summary>
    /// Convert the <see cref="StatusType"/> to an <see cref="StatusTypeEnum"/>.
    /// </summary>
    /// <param name="self">The <see cref="StatusType"/>.</param>
    public static implicit operator StatusTypeEnum(StatusType self) => (StatusTypeEnum)self.ID;

    /// <summary>
    /// Delete the <see cref="StatusType"/>.
    /// </summary>
    public void Delete()
    {
        EntityHelper.DeleteEntity(
            this,
            _EntityDAL.Delete
        );
    }

    /// <summary>
    /// Save the <see cref="StatusType"/>.
    /// </summary>
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
    /// Get an <see cref="StatusType"/> by ID.
    /// </summary>
    /// <param name="id">The ID of the <see cref="StatusType"/>.</param>
    /// <returns>The <see cref="StatusType"/>.</returns>
    public static StatusType Get(byte id)
    {
        return EntityHelper.GetEntity<byte, StatusTypeDAL, StatusType>(
            EntityCacheInfo,
            id,
            () => StatusTypeDAL.Get(id)
        );
    }

    /// <summary>
    /// Get an <see cref="StatusType"/> ny Value.
    /// </summary>
    /// <param name="value">The Value of the <see cref="StatusType"/>.</param>
    /// <returns>The <see cref="StatusType"/>.</returns>
    public static StatusType Get(string value)
    {
        return EntityHelper.GetEntityByLookup<byte, StatusTypeDAL, StatusType>(
            EntityCacheInfo,
            String.Format("StatusTypeValue:{0}", value),
            () => StatusTypeDAL.GetByValue(value)
        );
    }

    /// <summary>
    /// Get an <see cref="StatusType"/> by Value skipping caches.
    /// </summary>
    /// <param name="StatusTypeValue">The Value of the <see cref="StatusType"/>.</param>
    /// <returns></returns>
    /// <exception cref="ApplicationException">The <see cref="StatusType"/> with the value of is not supported.</exception>
    /// <exception cref="DataIntegrityException">Failed to load <see cref="StatusType"/>.</exception>
    public static StatusType MustGet(string StatusTypeValue)
    {
        if (!SupportedStatusTypes.Contains(StatusTypeValue))
            throw new ApplicationException(string.Format("The StatusType with the value of {0} is not supported.", StatusTypeValue));

        try
        {
            return EntityHelper.MustGet<string, StatusType>(
                StatusTypeValue,
                Get
            );
        }
        catch (DataIntegrityException)
        {
            throw new DataIntegrityException(String.Format("Failed to load StatusType {0}.", StatusTypeValue));
        }
    }

    /// <summary>
    /// Get an <see cref="StatusType"/> by ID skipping caches.
    /// </summary>
    /// <param name="StatusTypeID">The ID of the <see cref="StatusType"/>.</param>
    /// <returns></returns>
    /// <exception cref="DataIntegrityException">Failed to load <see cref="StatusType"/>.</exception>
    public static StatusType MustGet(byte StatusTypeID)
    {
        try
        {
            return EntityHelper.MustGet<byte, StatusType>(
                StatusTypeID,
                Get
            );
        }
        catch (DataIntegrityException)
        {
            throw new DataIntegrityException(String.Format("Failed to load StatusType {0}.", StatusTypeID));
        }
    }

    #region IRobloxEntity Members

    /// <inheritdoc cref="IRobloxEntity{TIndex, TDal}.Construct(TDal)"/>
    public void Construct(StatusTypeDAL dal)
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
    /// The <see cref="ICacheInfo"/> for the <see cref="StatusType"/>.
    /// </summary>
    public static CacheInfo EntityCacheInfo = new CacheInfo(
        new CacheabilitySettings(false, false, true, true, true, true),
        typeof(StatusType).ToString(),
        false
    );

    /// <inheritdoc cref="ICacheableObject.BuildEntityIDLookups"/>
    public IEnumerable<string> BuildEntityIDLookups()
    {
        if (_EntityDAL != null)
            yield return string.Format("StatusTypeValue:{0}", Value);
        yield break;
    }

    /// <inheritdoc cref="ICacheableObject.BuildStateTokenCollection"/>
    public IEnumerable<StateToken> BuildStateTokenCollection()
    {
        yield break;
    }

    #endregion ICacheableObject Members
}

