using Ivtem.TSqlParsing.Feature.CompatibilityLevel;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Ivtem.TSqlParsing.Feature.SQLParser;

public class SqlScriptGeneratorFactory : ISqlScriptGeneratorFactory
{
    private SqlScriptGeneratorOptions GeneratorOptions { get; }

    public SqlScriptGeneratorFactory(SqlScriptGeneratorOptions? generatorOptions)
    {
        GeneratorOptions = generatorOptions ?? new SqlScriptGeneratorOptions
        {
            AlignClauseBodies = true,
            IncludeSemicolons = true
        };
    }

    public SqlScriptGenerator GetGenerator(TSqlCompatibilityLevel compatibilityLevel)
        => compatibilityLevel switch
        {
            TSqlCompatibilityLevel.TSql80 => new Sql80ScriptGenerator(GeneratorOptions),
            TSqlCompatibilityLevel.TSql90 => new Sql90ScriptGenerator(GeneratorOptions),
            TSqlCompatibilityLevel.TSql100 => new Sql100ScriptGenerator(GeneratorOptions),
            TSqlCompatibilityLevel.TSql110 => new Sql110ScriptGenerator(GeneratorOptions),
            TSqlCompatibilityLevel.TSql120 => new Sql120ScriptGenerator(GeneratorOptions),
            TSqlCompatibilityLevel.TSql130 => new Sql130ScriptGenerator(GeneratorOptions),
            TSqlCompatibilityLevel.TSql140 => new Sql140ScriptGenerator(GeneratorOptions),
            TSqlCompatibilityLevel.TSql150 => new Sql150ScriptGenerator(GeneratorOptions),
            TSqlCompatibilityLevel.TSql160 => new Sql160ScriptGenerator(GeneratorOptions),
            _ => throw new ArgumentOutOfRangeException(nameof(compatibilityLevel), compatibilityLevel, null)
        };
}