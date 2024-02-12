using System.Data.SqlClient;

namespace Ivtem.TSqlParsing.Feature.CompatibilityLevel;

public class SqlCompatibilityLevelProvider : ISqlCompatibilityLevelProvider
{
    public TSqlCompatibilityLevel? CompatibilityLevel { get; private set; }

    private SqlConnectionStringBuilder ConnectionStringBuilder { get; }

    private string InitialCatalog => ConnectionStringBuilder.InitialCatalog;

    private string ConnectionString => ConnectionStringBuilder.ConnectionString;

    public SqlCompatibilityLevelProvider(string connectionString)
    {
        ArgumentException.ThrowIfNullOrEmpty(connectionString);
        ConnectionStringBuilder = new SqlConnectionStringBuilder(connectionString);
    }


    public TSqlCompatibilityLevel GetCompatibilityLevel()
    {
        if (CompatibilityLevel.HasValue) return CompatibilityLevel.Value;

        var sql = @$"
SELECT TOP 1 compatibility_level AS [CompatibilityLevel]
FROM sys.databases WHERE name = '{InitialCatalog}';
";
        using var connection = new SqlConnection(ConnectionString);

        connection.Open();

        var command = new SqlCommand(sql, connection);

        using var sqlReader = command.ExecuteReader();

        if (sqlReader.Read() && sqlReader[0] is byte value)
        {
            connection.Close();
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