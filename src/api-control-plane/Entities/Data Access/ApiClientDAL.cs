namespace Roblox.Api.ControlPlane.Entities;

using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;

using Roblox.MssqlDatabases;
using Roblox.Entities.Mssql;

/// <summary>
/// Data access layer for <see cref="ApiClient"/>
/// </summary>
[Serializable]
internal class ApiClientDAL
{
    private const Roblox.MssqlDatabases.RobloxDatabase _Database = global::Roblox.MssqlDatabases.RobloxDatabase.RobloxServices;

    public int ID { get; set; }
    public Guid ApiKey { get; set; }
    public string Note { get; set; }
    public byte StatusTypeID { get; set; }
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }

    private static ApiClientDAL BuildDAL(IDictionary<string, object> record)
    {
        var dal = new ApiClientDAL();
        dal.ID = (int)record["ID"];
        dal.ApiKey = (Guid)record["ApiKey"];
        dal.Note = (string)record["Note"];
        dal.StatusTypeID = (byte)record["StatusTypeID"];
        dal.Created = (DateTime)record["Created"];
        dal.Updated = (DateTime)record["Updated"];

        return dal;
    }

    internal void Delete()
    {
        _Database.Delete("ApiClients_DeleteApiClientByID", ID);
    }

    internal void Insert()
    {
        var queryParameters = new SqlParameter[]
        {
            new SqlParameter("@ID", ID) { Direction = ParameterDirection.Output },
            new SqlParameter("@ApiKey", ApiKey),
            new SqlParameter("@Note", Note),
            new SqlParameter("@StatusTypeID", StatusTypeID),
            new SqlParameter("@Created", Created),
            new SqlParameter("@Updated", Updated),
        };

        ID = _Database.Insert<int>("ApiClients_InsertApiClient", queryParameters);
    }

    internal void Update()
    {
        var queryParameters = new SqlParameter[]
        {
            new SqlParameter("@ID", ID),
            new SqlParameter("@ApiKey", ApiKey),
            new SqlParameter("@Note", Note),
            new SqlParameter("@StatusTypeID", StatusTypeID),
            new SqlParameter("@Created", Created),
            new SqlParameter("@Updated", Updated),
        };

        _Database.Update("ApiClients_UpdateApiClientByID", queryParameters);
    }

    internal static ApiClientDAL Get(int id)
    {
        return _Database.Get(
            "ApiClients_GetApiClientByID",
            id,
            BuildDAL
        );
    }

    public static ApiClientDAL GetByApiKey(Guid apiKey)
    {
        if (apiKey == default(Guid))
            return null;

        var queryParameters = new SqlParameter[]
        {
            new SqlParameter("@ApiKey", apiKey),
        };

        return _Database.Lookup(
            "ApiClients_GetApiClientByApiKey",
            BuildDAL,
            queryParameters
        );
    }

    public static ApiClientDAL GetByNote(string note)
    {
        if (string.IsNullOrEmpty(note))
            return null;

        var queryParameters = new SqlParameter[]
        {
            new SqlParameter("@Note", note),
        };

        return _Database.Lookup(
            "ApiClients_GetApiClientByNote",
            BuildDAL,
            queryParameters
        );
    }

    public static int GetTotalNumberOfApiClients()
    {
        return _Database.GetCount<int>(
            "ApiClients_GetTotalNumberOfApiClients"
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
            "ApiClients_GetAllApiClientIDs_Paged",
            queryParameters
        );
    }
}
