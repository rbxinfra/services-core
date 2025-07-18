namespace Roblox.Api.ControlPlane.Entities;

using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;

using Roblox.MssqlDatabases;
using Roblox.Entities.Mssql;

/// <summary>
/// Data access layer for <see cref="StatusType"/>
/// </summary>
[Serializable]
internal class StatusTypeDAL
{
    private const Roblox.MssqlDatabases.RobloxDatabase _Database = global::Roblox.MssqlDatabases.RobloxDatabase.RobloxServices;

    public byte ID { get; set; }
    public string Value { get; set; }
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }

    private static StatusTypeDAL BuildDAL(IDictionary<string, object> record)
    {
        var dal = new StatusTypeDAL();
        dal.ID = (byte)record["ID"];
        dal.Value = (string)record["Value"];
        dal.Created = (DateTime)record["Created"];
        dal.Updated = (DateTime)record["Updated"];

        return dal;
    }

    internal void Delete()
    {
        _Database.Delete("StatusTypes_DeleteStatusTypeByID", ID);
    }

    internal void Insert()
    {
        var queryParameters = new SqlParameter[]
        {
            new SqlParameter("@ID", ID) { Direction = ParameterDirection.Output },
            new SqlParameter("@Value", Value),
            new SqlParameter("@Created", Created),
            new SqlParameter("@Updated", Updated),
        };

        ID = _Database.Insert<byte>("StatusTypes_InsertStatusType", queryParameters);
    }

    internal void Update()
    {
        var queryParameters = new SqlParameter[]
        {
            new SqlParameter("@ID", ID),
            new SqlParameter("@Value", Value),
            new SqlParameter("@Created", Created),
            new SqlParameter("@Updated", Updated),
        };

        _Database.Update("StatusTypes_UpdateStatusTypeByID", queryParameters);
    }

    internal static StatusTypeDAL Get(byte id)
    {
        return _Database.Get(
            "StatusTypes_GetStatusTypeByID",
            id,
            BuildDAL
        );
    }

    public static StatusTypeDAL GetByValue(string value)
    {
        if (string.IsNullOrEmpty(value))
            return null;

        var queryParameters = new SqlParameter[]
        {
            new SqlParameter("@Value", value),
        };

        return _Database.Lookup(
            "StatusTypes_GetStatusTypeByValue",
            BuildDAL,
            queryParameters
        );
    }
}
