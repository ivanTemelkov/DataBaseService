using Ivtem.TSqlParsing.Feature.CompatibilityLevel;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Ivtem.TSqlParsing.Feature.SqlParser;

public class TSqlParserFactory : ITSqlParserFactory
{
    private bool InitialQuotedIdentifiers { get; }
    private SqlEngineType SqlEngineType { get; }

    public TSqlParserFactory(bool initialQuotedIdentifiers, SqlEngineType sqlEngineType)
    {
        InitialQuotedIdentifiers = initialQuotedIdentifiers;
        SqlEngineType = sqlEngineType;
    }

    public TSqlParser GetParser(TSqlCompatibilityLevel compatibilityLevel)
        => compatibilityLevel switch
        {
            TSqlCompatibilityLevel.TSql80 => new TSql80Parser(InitialQuotedIdentifiers),
            TSqlCompatibilityLevel.TSql90 => new TSql90Parser(InitialQuotedIdentifiers),
            TSqlCompatibilityLevel.TSql100 => new TSql100Parser(InitialQuotedIdentifiers),
            TSqlCompatibilityLevel.TSql110 => new TSql110Parser(InitialQuotedIdentifiers),
            TSqlCompatibilityLevel.TSql120 => new TSql120Parser(InitialQuotedIdentifiers),
            TSqlCompatibilityLevel.TSql130 => new TSql130Parser(InitialQuotedIdentifiers, SqlEngineType),
            TSqlCompatibilityLevel.TSql140 => new TSql140Parser(InitialQuotedIdentifiers, SqlEngineType),
            TSqlCompatibilityLevel.TSql150 => new TSql150Parser(InitialQuotedIdentifiers, SqlEngineType),
            TSqlCompatibilityLevel.TSql160 => new TSql160Parser(InitialQuotedIdentifiers, SqlEngineType),
            _ => throw new ArgumentOutOfRangeException(nameof(compatibilityLevel), compatibilityLevel,
                $"Compatibility Level {compatibilityLevel} NOT supported!")
        };
}