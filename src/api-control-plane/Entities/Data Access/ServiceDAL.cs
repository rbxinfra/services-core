namespace Roblox.Api.ControlPlane.Entities;

using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;

using Roblox.MssqlDatabases;
using Roblox.Entities.Mssql;

/// <summary>
/// Data access layer for <see cref="Service"/>
/// </summary>
[Serializable]
internal class ServiceDAL
{
    private const Roblox.MssqlDatabases.RobloxDatabase _Database = global::Roblox.MssqlDatabases.RobloxDatabase.RobloxServices;

    public int ID { get; set; }
    public string Name { get; set; }
    public byte StatusTypeID { get; set; }
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }

    private static ServiceDAL BuildDAL(IDictionary<string, object> record)
    {
        var dal = new ServiceDAL();
        dal.ID = (int)record["ID"];
        dal.Name = (string)record["Name"];
        dal.StatusTypeID = (byte)record["StatusTypeID"];
        dal.Created = (DateTime)record["Created"];
        dal.Updated = (DateTime)record["Updated"];

        return dal;
    }

    internal void Delete()
    {
        _Database.Delete("Services_DeleteServiceByID", ID);
    }

    internal void Insert()
    {
        var queryParameters = new SqlParameter[]
        {
            new SqlParameter("@ID", ID) { Direction = ParameterDirection.Output },
            new SqlParameter("@Name", Name),
            new SqlParameter("@StatusTypeID", StatusTypeID),
            new SqlParameter("@Created", Created),
            new SqlParameter("@Updated", Updated),
        };

        ID = _Database.Insert<int>("Services_InsertService", queryParameters);
    }

    internal void Update()
    {
        var queryParameters = new SqlParameter[]
        {
            new SqlParameter("@ID", ID),
            new SqlParameter("@Name", Name),
            new SqlParameter("@StatusTypeID", StatusTypeID),
            new SqlParameter("@Created", Created),
            new SqlParameter("@Updated", Updated),
        };

        _Database.Update("Services_UpdateServiceByID", queryParameters);
    }

    internal static ServiceDAL Get(int id)
    {
        return _Database.Get(
            "Services_GetServiceByID",
            id,
            BuildDAL
        );
    }

    public static ServiceDAL GetByName(string name)
    {
        if (string.IsNullOrEmpty(name))
            return null;

        var queryParameters = new SqlParameter[]
        {
            new SqlParameter("@Name", name),
        };

        return _Database.Lookup(
            "Services_GetServiceByName",
            BuildDAL,
            queryParameters
        );
    }

    public static int GetTotalNumberOfServices()
    {
        return _Database.GetCount<int>(
            "Services_GetTotalNumberOfServices"
        );
    }

    public static ICollection<int> GetAllPaged(long startRowIndex, long maximumRows)
    {
        if (startRowIndex < 1)
            throw new ApplicationException("Required value not specified: StartRowIndex.");
        if (maximumRows < 1)
            throw new ApplicationException("Required value not specified: MaximumRows.");

        var queryParameters = new SqlParameter[]
        {
            new SqlParameter("@StartRowIndex", startRowIndex),
            new SqlParameter("@MaximumRows", maximumRows)
        };

        return _Database.GetIDCollection<int>(
            "Services_GetAllServiceIDs_Paged",
            queryParameters
        );
    }
}
