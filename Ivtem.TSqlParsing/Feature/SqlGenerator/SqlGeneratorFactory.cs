using Ivtem.TSqlParsing.Feature.CompatibilityLevel;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Ivtem.TSqlParsing.Feature.SqlGenerator;

public class SqlGeneratorFactory : ISqlGeneratorFactory
{
    private SqlScriptGeneratorOptions GeneratorOptions { get; }

    public SqlGeneratorFactory(SqlScriptGeneratorOptions? generatorOptions = null)
    {
        GeneratorOptions = generatorOptions ?? new SqlScriptGeneratorOptions
        {
            KeywordCasing = KeywordCasing.Uppercase,
            SqlEngineType = SqlEngineType.Standalone,
            IndentationSize = 4,
            AlignClauseBodies = true,
            MultilineSelectElementsList = true,
            MultilineWherePredicatesList = true,
            IndentViewBody = true,
            MultilineViewColumnsList = false,
            AsKeywordOnOwnLine = false,
            IndentSetClause = true,
            AlignSetClauseItem = true,
            MultilineSetClauseItems = false,
            MultilineInsertTargetsList = false,
            MultilineInsertSourcesList = false,
            NewLineBeforeOpenParenthesisInMultilineList = false,
            NewLineBeforeCloseParenthesisInMultilineList = false,
            AllowExternalLibraryPaths = false,
            AllowExternalLanguagePaths = false,
            IncludeSemicolons = true,
            AlignColumnDefinitionFields = false,
            NewLineBeforeFromClause = true,
            NewLineBeforeWhereClause = true,
            NewLineBeforeGroupByClause = true,
            NewLineBeforeOrderByClause = true,
            NewLineBeforeHavingClause = true,
            NewLineBeforeWindowClause = true,
            NewLineBeforeJoinClause = true,
            NewLineBeforeOffsetClause = true,
            NewLineBeforeOutputClause = true,

        };
    }

    public DefaultSqlScriptGenerator GetGenerator(TSqlCompatibilityLevel compatibilityLevel)
        => compatibilityLevel switch
        {
            TSqlCompatibilityLevel.TSql80 => new DefaultSqlScriptGenerator(new Sql80ScriptGenerator(GeneratorOptions)),
            TSqlCompatibilityLevel.TSql90 => new DefaultSqlScriptGenerator(new Sql90ScriptGenerator(GeneratorOptions)),
            TSqlCompatibilityLevel.TSql100 => new DefaultSqlScriptGenerator(new Sql100ScriptGenerator(GeneratorOptions)),
            TSqlCompatibilityLevel.TSql110 => new DefaultSqlScriptGenerator(new Sql110ScriptGenerator(GeneratorOptions)),
            TSqlCompatibilityLevel.TSql120 => new DefaultSqlScriptGenerator(new Sql120ScriptGenerator(GeneratorOptions)),
            TSqlCompatibilityLevel.TSql130 => new DefaultSqlScriptGenerator(new Sql130ScriptGenerator(GeneratorOptions)),
            TSqlCompatibilityLevel.TSql140 => new DefaultSqlScriptGenerator(new Sql140ScriptGenerator(GeneratorOptions)),
            TSqlCompatibilityLevel.TSql150 => new DefaultSqlScriptGenerator(new Sql150ScriptGenerator(GeneratorOptions)),
            TSqlCompatibilityLevel.TSql160 => new DefaultSqlScriptGenerator(new Sql160ScriptGenerator(GeneratorOptions)),
            _ => throw new ArgumentOutOfRangeException(nameof(compatibilityLevel), compatibilityLevel, null)
        };
}