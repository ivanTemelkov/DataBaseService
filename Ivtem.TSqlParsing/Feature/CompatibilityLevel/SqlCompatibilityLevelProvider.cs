using System.Data.SqlClient;
using Ivtem.TSqlParsing.Extensions;

namespace Ivtem.TSqlParsing.Feature.CompatibilityLevel;

public class SqlCompatibilityLevelProvider : ISqlCompatibilityLevelProvider
{
    public TSqlCompatibilityLevel? CompatibilityLevel { get; private set; }

    private SqlConnectionStringBuilder ConnectionStringBuilder { get; }

    public string DataSource => ConnectionStringBuilder.DataSource;

    public string InitialCatalog => ConnectionStringBuilder.InitialCatalog;

    public string ConnectionString => ConnectionStringBuilder.ConnectionString;

    public SqlCompatibilityLevelProvider(string connectionString)
    {
        ArgumentException.ThrowIfNullOrEmpty(connectionString);
        ConnectionStringBuilder = new SqlConnectionStringBuilder(connectionString);
    }

    public Task<TSqlCompatibilityLevel> GetCompatibilityLevelWithTimeout(TimeSpan? timeoutInterval = null)
    {
        var timeout = timeoutInterval ?? TimeSpan.FromSeconds(1);
        return GetCompatibilityLevel().WithTimeout(timeout);
    }

    public async Task<TSqlCompatibilityLevel> GetCompatibilityLevel()
    {
        if (CompatibilityLevel.HasValue) return CompatibilityLevel.Value;

        var sql = @$"
SELECT TOP 1 compatibility_level AS [CompatibilityLevel]
FROM sys.databases WHERE name = '{InitialCatalog}';
";
        await using var connection = new SqlConnection(ConnectionString);

        await connection.OpenAsync();

        var command = new SqlCommand(sql, connection);

        await using var sqlReader = await command.ExecuteReaderAsync();

        if (await sqlReader.ReadAsync() && sqlReader[0] is byte value)
        {
            await connection.CloseAsync();
            CompatibilityLevel = FromInt(value);
            return CompatibilityLevel.Value;
        }

        throw new InvalidOperationException("Failed to get Sql Compatibility Level!");
    }

    private static TSqlCompatibilityLevel FromInt(int value) => value switch
    {
        80 => TSqlCompatibilityLevel.TSql80,
        90 => TSqlCompatibilityLevel.TSql90,
        100 => TSqlCompatibilityLevel.TSql100,
        110 => TSqlCompatibilityLevel.TSql110,
        120 => TSqlCompatibilityLevel.TSql120,
        130 => TSqlCompatibilityLevel.TSql130,
        140 => TSqlCompatibilityLevel.TSql140,
        150 => TSqlCompatibilityLevel.TSql150,
        160 => TSqlCompatibilityLevel.TSql160,
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, $"Unsupported compatibility level {value}!")
    };
}