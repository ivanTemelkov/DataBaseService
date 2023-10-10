using Ivtem.TSqlParsing.Feature.CompatibilityLevel;
using Ivtem.TSqlParsing.Feature.SqlFragment;
using Ivtem.TSqlParsing.Feature.SqlGenerator;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Ivtem.TSqlParsing.Feature;

public class SqlFragmentAndGeneratorProvider : ISqlFragmentAndGeneratorProvider
{
    private ISqlCompatibilityLevelProvider CompatibilityLevelProvider { get; }

    private ISqlFragmentProvider SqlFragmentProvider { get; }

    private ISqlGeneratorFactory SqlGeneratorFactory { get; }

    private TSqlCompatibilityLevel? CompatibilityLevel { get; set; }


    public SqlFragmentAndGeneratorProvider(ISqlCompatibilityLevelProvider compatibilityLevelProvider,
        ISqlFragmentProvider sqlFragmentProvider,
        ISqlGeneratorFactory sqlGeneratorFactory)
    {
        CompatibilityLevelProvider = compatibilityLevelProvider;
        SqlFragmentProvider = sqlFragmentProvider;
        SqlGeneratorFactory = sqlGeneratorFactory;
    }

    public TSqlCompatibilityLevel GetCompatibilityLevel()
    {
        CompatibilityLevel ??= CompatibilityLevelProvider.GetCompatibilityLevel();
        return CompatibilityLevel.Value;
    }

    public TSqlFragment GetSqlFragment(string sql)
    {
        CompatibilityLevel ??= CompatibilityLevelProvider.GetCompatibilityLevel();
        if (SqlFragmentProvider.TryGetSqlFragment(sql, CompatibilityLevel.Value, out var sqlFragment, out var parseErrors) ==
            false)
        {
            throw new InvalidOperationException($"Parser failed! Sql: {sql}\nErrors: {parseErrors}");
        }

        return sqlFragment;
    }
    
    public DefaultSqlScriptGenerator GetSqlGenerator()
    {
        CompatibilityLevel ??= CompatibilityLevelProvider.GetCompatibilityLevel();
        return SqlGeneratorFactory.GetGenerator(CompatibilityLevel.Value);
    }

}
