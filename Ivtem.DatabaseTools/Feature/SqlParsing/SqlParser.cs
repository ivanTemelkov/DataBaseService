using System.Diagnostics.CodeAnalysis;
using Ivtem.DatabaseTools.Feature.DatabaseService;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Ivtem.DatabaseTools.Feature.SqlParsing;

public class SqlParser
{
    private TSqlCompatibilityLevel CompatibilityLevel { get; }

    private TSqlParserFactory ParserFactory { get; } = new(initialQuotedIdentifiers: true, SqlEngineType.Standalone);

    public SqlParser(TSqlCompatibilityLevel compatibilityLevel)
    {
        CompatibilityLevel = compatibilityLevel;
    }

    public bool TryGetSqlFragment(string query, [NotNullWhen(true)] out TSqlFragment? sqlFragment, [NotNullWhen(false)] out ParseError[]? parseErrors)
    {
        sqlFragment = default;
        parseErrors = default;

        var sqlParser = ParserFactory.GetParser(CompatibilityLevel);

        using var reader = new StringReader(query);
        sqlFragment = sqlParser.Parse(reader, out var errors);

        parseErrors = errors?.ToArray();

        return sqlFragment is not null;
    }
}