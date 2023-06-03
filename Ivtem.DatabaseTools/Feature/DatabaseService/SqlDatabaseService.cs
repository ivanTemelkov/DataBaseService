using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Ivtem.DatabaseTools.Feature.DatabaseService;

public class SqlDatabaseService : IDatabaseService
{
    private SqlConnectionStringBuilder ConnectionStringBuilder { get; }

    private TSqlParserFactory ParserFactory { get; } = new(initialQuotedIdentifiers: true, SqlEngineType.Standalone);

    public string DataSource => ConnectionStringBuilder.DataSource;

    public string InitialCatalog => ConnectionStringBuilder.InitialCatalog;

    public string ConnectionString => ConnectionStringBuilder.ConnectionString;

    public TSqlCompatibilityLevel CompatibilityLevel { get; private set; }

    private SqlDatabaseService(SqlConnectionStringBuilder builder)
    {
        ConnectionStringBuilder = builder;
    }

    public static async Task<SqlDatabaseService> Create(string connectionString)
    {
        ArgumentNullException.ThrowIfNull(connectionString);

        var connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);

        var result = new SqlDatabaseService(connectionStringBuilder);

        await result.Initialize();

        return result;
    }

    public bool TryValidateSqlQuery(string query, [NotNullWhen(false)] out ParseError[]? parseErrors)
    {
        parseErrors = default;

        var sqlParser = ParserFactory.GetParser(CompatibilityLevel);

        using var reader = new StringReader(query);
        _ = sqlParser.Parse(reader, out var errors);

        parseErrors = errors?.ToArray();

        return parseErrors is null || parseErrors.Length == 0;
    }

    private async Task Initialize()
    {
        CompatibilityLevel = await GetCompatibilityLevel();
    }

    private async Task<TSqlCompatibilityLevel> GetCompatibilityLevel()
    {
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
            return FromInt(value);
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