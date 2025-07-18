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
/// Business logic for <see cref="AuthorizationType"/>
/// </summary>
[DebuggerDisplay("Roblox.Api.ControlPlane.AuthorizationType.{Value,nq}")]
internal class AuthorizationType : IRobloxEntity<byte, AuthorizationTypeDAL>
{
    private AuthorizationTypeDAL _EntityDAL;

    /// <summary>
    /// Gets the <see cref="ID"/> of this <see cref="AuthorizationType"/>.
    /// </summary>
    public byte ID => _EntityDAL.ID;

    /// <summary>
    /// Gets or sets the <see cref="Value"/> of this <see cref="AuthorizationType"/>.
    /// </summary>
    public string Value
    {
        get => _EntityDAL.Value;
        set => _EntityDAL.Value = value;
    }

    /// <summary>
    /// Gets the <see cref="Created"/> of this <see cref="AuthorizationType"/>.
    /// </summary>
    public DateTime Created => _EntityDAL.Created;

    /// <summary>
    /// Gets the <see cref="Updated"/> of this <see cref="AuthorizationType"/>.
    /// </summary>
    public DateTime Updated => _EntityDAL.Updated;

    /// <summary>
    /// The ID of the <see cref="AuthorizationTypeEnum.None"/>.
    /// </summary>
    public static readonly byte NoneID;

    /// <summary>
    /// The Value of the <see cref="AuthorizationTypeEnum.None"/>.
    /// </summary>
    public static readonly string NoneValue = "None";

    /// <summary>
    /// The ID of the <see cref="AuthorizationTypeEnum.Partial"/>.
    /// </summary>
    public static readonly byte PartialID;

    /// <summary>
    /// The Value of the <see cref="AuthorizationTypeEnum.Partial"/>.
    /// </summary>
    public static readonly string PartialValue = "Partial";

    /// <summary>
    /// The ID of the <see cref="AuthorizationTypeEnum.Full"/>.
    /// </summary>
    public static readonly byte FullID;

    /// <summary>
    /// The Value of the <see cref="AuthorizationTypeEnum.Full"/>.
    /// </summary>
    public static readonly string FullValue = "Full";

    /// <summary>
    /// The supported <see cref="AuthorizationType"/>s.
    /// </summary>
    public static HashSet<string> SupportedAuthorizationTypes = new() { NoneValue, PartialValue, FullValue };

    /// <summary>
    /// Construct a new instance of <see cref="AuthorizationType"/>.
    /// </summary>
    public AuthorizationType()
    {
        _EntityDAL = new AuthorizationTypeDAL();
    }

    /// <summary>
    /// The static construct for <see cref="AuthorizationType"/>.
    /// </summary>
    static AuthorizationType()
    {
        NoneID = MustGet(NoneValue).ID;
        PartialID = MustGet(PartialValue).ID;
        FullID = MustGet(FullValue).ID;
    }

    /// <summary>
    /// Convert the <see cref="AuthorizationType"/> to an <see cref="AuthorizationTypeEnum"/>.
    /// </summary>
    /// <param name="self">The <see cref="AuthorizationType"/>.</param>
    public static implicit operator AuthorizationTypeEnum(AuthorizationType self) => (AuthorizationTypeEnum)self.ID;

    /// <summary>
    /// Delete the <see cref="AuthorizationType"/>.
    /// </summary>
    public void Delete()
    {
        EntityHelper.DeleteEntity(
            this,
            _EntityDAL.Delete
        );
    }

    /// <summary>
    /// Save the <see cref="AuthorizationType"/>.
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
    /// Get an <see cref="AuthorizationType"/> by ID.
    /// </summary>
    /// <param name="id">The ID of the <see cref="AuthorizationType"/>.</param>
    /// <returns>The <see cref="AuthorizationType"/>.</returns>
    public static AuthorizationType Get(byte id)
    {
        return EntityHelper.GetEntity<byte, AuthorizationTypeDAL, AuthorizationType>(
            EntityCacheInfo,
            id,
            () => AuthorizationTypeDAL.Get(id)
        );
    }

    /// <summary>
    /// Get an <see cref="AuthorizationType"/> ny Value.
    /// </summary>
    /// <param name="value">The Value of the <see cref="AuthorizationType"/>.</param>
    /// <returns>The <see cref="AuthorizationType"/>.</returns>
    public static AuthorizationType Get(string value)
    {
        return EntityHelper.GetEntityByLookup<byte, AuthorizationTypeDAL, AuthorizationType>(
            EntityCacheInfo,
            String.Format("AuthorizationTypeValue:{0}", value),
            () => AuthorizationTypeDAL.GetByValue(value)
        );
    }

    /// <summary>
    /// Get an <see cref="AuthorizationType"/> by Value skipping caches.
    /// </summary>
    /// <param name="authorizationTypeValue">The Value of the <see cref="AuthorizationType"/>.</param>
    /// <returns></returns>
    /// <exception cref="ApplicationException">The <see cref="AuthorizationType"/> with the value of is not supported.</exception>
    /// <exception cref="DataIntegrityException">Failed to load <see cref="AuthorizationType"/>.</exception>
    public static AuthorizationType MustGet(string authorizationTypeValue)
    {
        if (!SupportedAuthorizationTypes.Contains(authorizationTypeValue))
            throw new ApplicationException(string.Format("The AuthorizationType with the value of {0} is not supported.", authorizationTypeValue));

        try
        {
            return EntityHelper.MustGet<string, AuthorizationType>(
                authorizationTypeValue,
                Get
            );
        }
        catch (DataIntegrityException)
        {
            throw new DataIntegrityException(String.Format("Failed to load AuthorizationType {0}.", authorizationTypeValue));
        }
    }

    /// <summary>
    /// Get an <see cref="AuthorizationType"/> by ID skipping caches.
    /// </summary>
    /// <param name="authorizationTypeID">The ID of the <see cref="AuthorizationType"/>.</param>
    /// <returns></returns>
    /// <exception cref="DataIntegrityException">Failed to load <see cref="AuthorizationType"/>.</exception>
    public static AuthorizationType MustGet(byte authorizationTypeID)
    {
        try
        {
            return EntityHelper.MustGet<byte, AuthorizationType>(
                authorizationTypeID,
                Get
            );
        }
        catch (DataIntegrityException)
        {
            throw new DataIntegrityException(String.Format("Failed to load AuthorizationType {0}.", authorizationTypeID));
        }
    }

    #region IRobloxEntity Members

    /// <inheritdoc cref="IRobloxEntity{TIndex, TDal}.Construct(TDal)"/>
    public void Construct(AuthorizationTypeDAL dal)
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
    /// The <see cref="ICacheInfo"/> for the <see cref="AuthorizationType"/>.
    /// </summary>
    public static CacheInfo EntityCacheInfo = new CacheInfo(
        new CacheabilitySettings(false, false, true, true, true, true),
        typeof(AuthorizationType).ToString(),
        false
    );

    /// <inheritdoc cref="ICacheableObject.BuildEntityIDLookups"/>
    public IEnumerable<string> BuildEntityIDLookups()
    {
        if (_EntityDAL != null)
            yield return string.Format("AuthorizationTypeValue:{0}", Value);
        yield break;
    }

    /// <inheritdoc cref="ICacheableObject.BuildStateTokenCollection"/>
    public IEnumerable<StateToken> BuildStateTokenCollection()
    {
        yield break;
    }

    #endregion ICacheableObject Members
}

