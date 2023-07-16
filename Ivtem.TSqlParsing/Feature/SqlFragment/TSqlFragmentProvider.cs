using Ivtem.TSqlParsing.Feature.CompatibilityLevel;
using Ivtem.TSqlParsing.Feature.SqlParser;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using System.Diagnostics.CodeAnalysis;

namespace Ivtem.TSqlParsing.Feature.SqlFragment;

public class TSqlFragmentProvider : ISqlFragmentProvider
{
    private TSqlParserFactory ParserFactory { get; } = new(initialQuotedIdentifiers: true, SqlEngineType.Standalone);

    public bool TryGetSqlFragment(string query, TSqlCompatibilityLevel compatibilityLevel,
        [NotNullWhen(true)] out TSqlFragment? sqlFragment, [NotNullWhen(false)] out ParseError[]? parseErrors)
    {
        sqlFragment = default;
        parseErrors = default;

        var sqlParser = ParserFactory.GetParser(compatibilityLevel);

        using var reader = new StringReader(query);
        sqlFragment = sqlParser.Parse(reader, out var errors);

        parseErrors = errors?.ToArray();

        return sqlFragment is not null;
    }

    public bool TryGetSqlFragment(string query, [NotNullWhen(true)] out TSqlFragment? sqlFragment,
        [NotNullWhen(false)] out ParseError[]? parseErrors)
        => TryGetSqlFragment(query, TSqlCompatibilityLevel.TSql160, out sqlFragment, out parseErrors);
}

