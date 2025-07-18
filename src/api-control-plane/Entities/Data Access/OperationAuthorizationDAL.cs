namespace Roblox.Api.ControlPlane.Entities;

using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;

using Roblox.MssqlDatabases;
using Roblox.Entities.Mssql;

/// <summary>
/// Data access layer for <see cref="OperationAuthorization"/>
/// </summary>
[Serializable]
internal class OperationAuthorizationDAL
{
    private const Roblox.MssqlDatabases.RobloxDatabase _Database = global::Roblox.MssqlDatabases.RobloxDatabase.RobloxServices;

    public int ID { get; set; }
    public int OperationID { get; set; }
    public int ApiClientID { get; set; }
    public byte AuthorizationTypeID { get; set; }
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }

    private static OperationAuthorizationDAL BuildDAL(IDictionary<string, object> record)
    {
        var dal = new OperationAuthorizationDAL();
        dal.ID = (int)record["ID"];
        dal.OperationID = (int)record["OperationID"];
        dal.ApiClientID = (int)record["ApiClientID"];
        dal.AuthorizationTypeID = (byte)record["AuthorizationTypeID"];
        dal.Created = (DateTime)record["Created"];
        dal.Updated = (DateTime)record["Updated"];

        return dal;
    }

    internal void Delete()
    {
        _Database.Delete("OperationAuthorizations_DeleteOperationAuthorizationByID", ID);
    }

    internal void Insert()
    {
        var queryParameters = new SqlParameter[]
        {
            new SqlParameter("@ID", ID) { Direction = ParameterDirection.Output },
            new SqlParameter("@OperationID", OperationID),
            new SqlParameter("@ApiClientID", ApiClientID),
            new SqlParameter("@AuthorizationTypeID", AuthorizationTypeID),
            new SqlParameter("@Created", Created),
            new SqlParameter("@Updated", Updated),
        };

        ID = _Database.Insert<int>("OperationAuthorizations_InsertOperationAuthorization", queryParameters);
    }

    internal void Update()
    {
        var queryParameters = new SqlParameter[]
        {
            new SqlParameter("@ID", ID),
            new SqlParameter("@OperationID", OperationID),
            new SqlParameter("@ApiClientID", ApiClientID),
            new SqlParameter("@AuthorizationTypeID", AuthorizationTypeID),
            new SqlParameter("@Created", Created),
            new SqlParameter("@Updated", Updated),
        };

        _Database.Update("OperationAuthorizations_UpdateOperationAuthorizationByID", queryParameters);
    }

    internal static OperationAuthorizationDAL Get(int id)
    {
        return _Database.Get(
            "OperationAuthorizations_GetOperationAuthorizationByID",
            id,
            BuildDAL
        );
    }

    public static OperationAuthorizationDAL GetByOperationIDAndApiClientID(int operationID, int apiClientID)
    {
        if (operationID == default(int))
            return null;
        if (apiClientID == default(int))
            return null;

        var queryParameters = new SqlParameter[]
        {
            new SqlParameter("@OperationID", operationID),
            new SqlParameter("@ApiClientID", apiClientID),
        };

        return _Database.Lookup(
            "OperationAuthorizations_GetOperationAuthorizationByOperationIDAndApiClientID",
            BuildDAL,
            queryParameters
        );
    }

    public static int GetTotalNumberOfOperationAuthorizationsByOperationID(int operationID)
    {
        var queryParameters = new SqlParameter[]
        {
            new SqlParameter("@OperationID", operationID),
        };

        return _Database.GetCount<int>(
            "OperationAuthorizations_GetTotalNumberOfOperationAuthorizationsByOperationID",
            queryParameters: queryParameters
        );
    }

    public static ICollection<int> GetOperationAuthorizationsByOperationID(int operationID, long startRowIndex, long maximumRows)
    {
        if (operationID == default(int))
            throw new ArgumentException("Parameter 'operationID' cannot be null, empty or the default value.");
        if (startRowIndex < 1)
            throw new ApplicationException("Required value not specified: StartRowIndex.");
        if (maximumRows < 1)
            throw new ApplicationException("Required value not specified: MaximumRows.");

        var queryParameters = new SqlParameter[]
        {
            new SqlParameter("@OperationID", operationID),
            new SqlParameter("@StartRowIndex", startRowIndex),
            new SqlParameter("@MaximumRows", maximumRows)
        };

        return _Database.GetIDCollection<int>(
            "OperationAuthorizations_GetOperationAuthorizationIDsByOperationID_Paged",
            queryParameters
        );
    }

    public static int GetTotalNumberOfOperationAuthorizationsByApiClientID(int apiClientID)
    {
        var queryParameters = new SqlParameter[]
        {
            new SqlParameter("@ApiClientID", apiClientID),
        };

        return _Database.GetCount<int>(
            "OperationAuthorizations_GetTotalNumberOfOperationAuthorizationsByApiClientID",
            queryParameters: queryParameters
        );
    }

    public static ICollection<int> GetOperationAuthorizationsByApiClientID(int apiClientID, long startRowIndex, long maximumRows)
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
            "OperationAuthorizations_GetOperationAuthorizationIDsByApiClientID_Paged",
            queryParameters
        );
    }
}
