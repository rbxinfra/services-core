namespace Roblox.Api.ControlPlane.Entities;

using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;

using Roblox.MssqlDatabases;
using Roblox.Entities.Mssql;

/// <summary>
/// Data access layer for <see cref="Operation"/>
/// </summary>
[Serializable]
internal class OperationDAL
{
    private const Roblox.MssqlDatabases.RobloxDatabase _Database = global::Roblox.MssqlDatabases.RobloxDatabase.RobloxServices;

    public int ID { get; set; }
    public string Name { get; set; }
    public int ServiceID { get; set; }
    public byte StatusTypeID { get; set; }
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }

    private static OperationDAL BuildDAL(IDictionary<string, object> record)
    {
        var dal = new OperationDAL();
        dal.ID = (int)record["ID"];
        dal.Name = (string)record["Name"];
        dal.ServiceID = (int)record["ServiceID"];
        dal.StatusTypeID = (byte)record["StatusTypeID"];
        dal.Created = (DateTime)record["Created"];
        dal.Updated = (DateTime)record["Updated"];

        return dal;
    }

    internal void Delete()
    {
        _Database.Delete("Operations_DeleteOperationByID", ID);
    }

    internal void Insert()
    {
        var queryParameters = new SqlParameter[]
        {
            new SqlParameter("@ID", ID) { Direction = ParameterDirection.Output },
            new SqlParameter("@Name", Name),
            new SqlParameter("@ServiceID", ServiceID),
            new SqlParameter("@StatusTypeID", StatusTypeID),
            new SqlParameter("@Created", Created),
            new SqlParameter("@Updated", Updated),
        };

        ID = _Database.Insert<int>("Operations_InsertOperation", queryParameters);
    }

    internal void Update()
    {
        var queryParameters = new SqlParameter[]
        {
            new SqlParameter("@ID", ID),
            new SqlParameter("@Name", Name),
            new SqlParameter("@ServiceID", ServiceID),
            new SqlParameter("@StatusTypeID", StatusTypeID),
            new SqlParameter("@Created", Created),
            new SqlParameter("@Updated", Updated),
        };

        _Database.Update("Operations_UpdateOperationByID", queryParameters);
    }

    internal static OperationDAL Get(int id)
    {
        return _Database.Get(
            "Operations_GetOperationByID",
            id,
            BuildDAL
        );
    }

    public static OperationDAL GetByServiceIDAndName(int serviceID, string name)
    {
        if (serviceID == default(int))
            return null;
        if (string.IsNullOrEmpty(name))
            return null;

        var queryParameters = new SqlParameter[]
        {
            new SqlParameter("@ServiceID", serviceID),
            new SqlParameter("@Name", name),
        };

        return _Database.Lookup(
            "Operations_GetOperationByServiceIDAndName",
            BuildDAL,
            queryParameters
        );
    }

    public static int GetTotalNumberOfOperationsByServiceID(int serviceID)
    {
        var queryParameters = new SqlParameter[]
        {
            new SqlParameter("@ServiceID", serviceID),
        };

        return _Database.GetCount<int>(
            "Operations_GetTotalNumberOfOperationsByServiceID",
            queryParameters: queryParameters
        );
    }

    public static ICollection<int> GetOperationsByServiceID(int serviceID, long startRowIndex, long maximumRows)
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
            "Operations_GetOperationIDsByServiceID_Paged",
            queryParameters
        );
    }
}
