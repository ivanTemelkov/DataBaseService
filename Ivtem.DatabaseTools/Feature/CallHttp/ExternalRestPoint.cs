using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Ivtem.DatabaseTools.Feature.CallHttp;


public sealed partial class ExternalRestPoint
{
    public Task<ExternalRestEndpointStatus> IsAvailable([NotNull] string connectionString)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        var sqlConnectionString = new SqlConnectionStringBuilder(connectionString);
        // TODO Add Caching by Data Source
        var dataSource = sqlConnectionString.DataSource;




    }
}

public sealed partial class ExternalRestPoint
{
    

}

public sealed class RestEndPointCaller
{
    private bool IsEnableCache { get; }

    public RestEndPointCaller(bool isEnableCache)
    {
        IsEnableCache = isEnableCache;
    }

    public async Task<RestEndpointResult> Call([NotNull] string connectionString, [NotNull] RestEnpointRequest request, CancellationToken token = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        var sqlConnectionString = new SqlConnectionStringBuilder(connectionString);

        // TODO Add Caching by Data Source
        var dataSource = sqlConnectionString.DataSource;

        await using var connection = new SqlConnection(connectionString);
        await using var transaction = await connection.BeginTransactionAsync(token);

        ExternalRestEndpointStatus externalEndpointStatus;
        if (IsEnableCache)
        {
            // TODO
            externalEndpointStatus = await connection.GetExternalRestEndpointStatus((SqlTransaction)transaction, token);
        }
        else
        {
            externalEndpointStatus = await connection.GetExternalRestEndpointStatus((SqlTransaction)transaction, token);
        }

        if (externalEndpointStatus is ExternalRestEndpointStatus.Ready)
        {
            // Use External Rest Endpoint implementation
        }
        else
        {
            // Use OLE Automation Implementation
        }
    }
}

public sealed record RestEndpointResult(HttpStatusCode StatusCode, string body);

public sealed record RestEnpointRequest(Uri Uri, HttpMethod Method);



internal static class SqlConnectionExtensions
{
    private const string ExternalRestPointQuery = """
        SET NOCOUNT ON;

        DECLARE @Status int;

        -- 1 = NotSupported
        -- The configuration option doesn't exist on SQL Server versions
        -- that don't support the native external REST endpoint feature.
        IF NOT EXISTS
        (
            SELECT 1
            FROM sys.configurations
            WHERE name = N'external rest endpoint enabled'
        )
        BEGIN
            SET @Status = 1;
        END

        -- 2 = Disabled
        ELSE IF NOT EXISTS
        (
            SELECT 1
            FROM sys.configurations
            WHERE name = N'external rest endpoint enabled'
              AND CONVERT(int, value_in_use) = 1
        )
        BEGIN
            SET @Status = 2;
        END

        -- 3 = PermissionDenied
        ELSE IF ISNULL(
            HAS_PERMS_BY_NAME(
                DB_NAME(),
                N'DATABASE',
                N'EXECUTE ANY EXTERNAL ENDPOINT'
            ),
            0
        ) <> 1
        BEGIN
            SET @Status = 3;
        END

        -- 0 = Ready
        ELSE
        BEGIN
            SET @Status = 0;
        END;

        SELECT @Status;
        """;

    internal static async Task<ExternalRestEndpointStatus> GetExternalRestEndpointStatus(
        [NotNull] this SqlConnection connection, 
        SqlTransaction? transaction = null, 
        CancellationToken cancellationToken = default)
    {
        await using var command = connection.CreateCommand();
        command.CommandType = CommandType.Text;
        command.CommandText = ExternalRestPointQuery;
        command.Transaction = transaction;

        var result = await command.ExecuteScalarAsync(cancellationToken);

        if (result is null or DBNull)
        {
            throw new InvalidOperationException(
                "SQL Server did not return an external REST endpoint status.");
        }

        var value = Convert.ToInt32(result);

        if (Enum.IsDefined(typeof(ExternalRestEndpointStatus), value) == false)
        {
            throw new InvalidOperationException(
                $"SQL Server returned an unknown external REST endpoint status: {value}.");
        }

        return (ExternalRestEndpointStatus)value;
    }
}


public enum ExternalRestEndpointStatus
{
    Ready = 0,
    NotSupported = 1,
    Disabled = 2,
    PermissionDenied = 3
}
