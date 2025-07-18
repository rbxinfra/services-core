namespace Roblox.Api.ControlPlane.Entities;

using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;

using Roblox.MssqlDatabases;
using Roblox.Entities.Mssql;

/// <summary>
/// Data access layer for <see cref="ServiceAuthorization"/>
/// </summary>
[Serializable]
internal class ServiceAuthorizationDAL
{
    private const Roblox.MssqlDatabases.RobloxDatabase _Database = global::Roblox.MssqlDatabases.RobloxDatabase.RobloxServices;

    public int ID { get; set; }
    public int ServiceID { get; set; }
    public int ApiClientID { get; set; }
    public byte AuthorizationTypeID { get; set; }
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }

    private static ServiceAuthorizationDAL BuildDAL(IDictionary<string, object> record)
    {
        var dal = new ServiceAuthorizationDAL();
        dal.ID = (int)record["ID"];
        dal.ServiceID = (int)record["ServiceID"];
        dal.ApiClientID = (int)record["ApiClientID"];
        dal.AuthorizationTypeID = (byte)record["AuthorizationTypeID"];
        dal.Created = (DateTime)record["Created"];
        dal.Updated = (DateTime)record["Updated"];

        return dal;
    }

    internal void Delete()
    {
        _Database.Delete("ServiceAuthorizations_DeleteServiceAuthorizationByID", ID);
    }

    internal void Insert()
    {
        var queryParameters = new SqlParameter[]
        {
            new SqlParameter("@ID", ID) { Direction = ParameterDirection.Output },
            new SqlParameter("@ServiceID", ServiceID),
            new SqlParameter("@ApiClientID", ApiClientID),
            new SqlParameter("@AuthorizationTypeID", AuthorizationTypeID),
            new SqlParameter("@Created", Created),
            new SqlParameter("@Updated", Updated),
        };

        ID = _Database.Insert<int>("ServiceAuthorizations_InsertServiceAuthorization", queryParameters);
    }

    internal void Update()
    {
        var queryParameters = new SqlParameter[]
        {
            new SqlParameter("@ID", ID),
            new SqlParameter("@ServiceID", ServiceID),
            new SqlParameter("@ApiClientID", ApiClientID),
            new SqlParameter("@AuthorizationTypeID", AuthorizationTypeID),
            new SqlParameter("@Created", Created),
            new SqlParameter("@Updated", Updated),
        };

        _Database.Update("ServiceAuthorizations_UpdateServiceAuthorizationByID", queryParameters);
    }

    internal static ServiceAuthorizationDAL Get(int id)
    {
        return _Database.Get(
            "ServiceAuthorizations_GetServiceAuthorizationByID",
            id,
            BuildDAL
        );
    }

    public static ServiceAuthorizationDAL GetByServiceIDAndApiClientID(int serviceID, int apiClientID)
    {
        if (serviceID == default(int))
            return null;
        if (apiClientID == default(int))
            return null;

        var queryParameters = new SqlParameter[]
        {
            new SqlParameter("@ServiceID", serviceID),
            new SqlParameter("@ApiClientID", apiClientID),
        };

        return _Database.Lookup(
            "ServiceAuthorizations_GetServiceAuthorizationByServiceIDAndApiClientID",
            BuildDAL,
            queryParameters
        );
    }

    public static int GetTotalNumberOfServiceAuthorizationsByServiceID(int serviceID)
    {
        var queryParameters = new SqlParameter[]
        {
            new SqlParameter("@ServiceID", serviceID),
        };

        return _Database.GetCount<int>(
            "ServiceAuthorizations_GetTotalNumberOfServiceAuthorizationsByServiceID",
            queryParameters: queryParameters
        );
    }

    public static ICollection<int> GetServiceAuthorizationsByServiceID(int serviceID, long startRowIndex, long maximumRows)
    {
        if (serviceID == default(int))
            throw new ArgumentException("Parameter 'serviceID' cannot be null, empty or the default value.");
        if (startRowIndex < 1)
            throw new ApplicationException("Required value not specified: StartRowIndex.");
        if (maximumRows < 1)
            throw new ApplicationException("Required value not specified: MaximumRows.");

        var queryParameters = new SqlParameter[]
        {
            new SqlParameter("@ServiceID", serviceID),
            new SqlParameter("@StartRowIndex", startRowIndex),
            new SqlParameter("@MaximumRows", maximumRows)
        };

        return _Database.GetIDCollection<int>(
            "ServiceAuthorizations_GetServiceAuthorizationIDsByServiceID_Paged",
            queryParameters
        );
    }

    public static int GetTotalNumberOfServiceAuthorizationsByApiClientID(int apiClientID)
    {
        var queryParameters = new SqlParameter[]
        {
            new SqlParameter("@ApiClientID", apiClientID),
        };

        return _Database.GetCount<int>(
            "ServiceAuthorizations_GetTotalNumberOfServiceAuthorizationsByApiClientID",
            queryParameters: queryParameters
        );
    }

    public static ICollection<int> GetServiceAuthorizationsByApiClientID(int apiClientID, long startRowIndex, long maximumRows)
    {
        if (apiClientID == default(int))
            throw new ArgumentException("Parameter 'apiClientID' cannot be null, empty or the default value.");
        if (startRowIndex < 1)
            throw new ApplicationException("Required value not specified: StartRowIndex.");
        if (maximumRows < 1)
            throw new ApplicationException("Required value not specified: MaximumRows.");

        var queryParameters = new SqlParameter[]
        {
            new SqlParameter("@ApiClientID", apiClientID),
            new SqlParameter("@StartRowIndex", startRowIndex),
            new SqlParameter("@MaximumRows", maximumRows)
        };

        return _Database.GetIDCollection<int>(
            "ServiceAuthorizations_GetServiceAuthorizationIDsByApiClientID_Paged",
            queryParameters
        );
    }
}
