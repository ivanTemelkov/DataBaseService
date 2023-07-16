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

    public async Task<TSqlCompatibilityLevel> GetCompatibilityLevel()
    {
        CompatibilityLevel ??= await CompatibilityLevelProvider.GetCompatibilityLevelWithTimeout();
        return CompatibilityLevel.Value;
    }

    public async Task<TSqlFragment> GetSqlFragment(string sql)
    {
        CompatibilityLevel ??= await CompatibilityLevelProvider.GetCompatibilityLevelWithTimeout();
        if (SqlFragmentProvider.TryGetSqlFragment(sql, CompatibilityLevel.Value, out var sqlFragment, out var parseErrors) ==
            false)
        {
            throw new InvalidOperationException($"Parser failed! Sql: {sql}\nErrors: {parseErrors}");
        }

        return sqlFragment;
    }

    [Obsolete($"Use {nameof(GetSqlFragment)} instead.")]
    public Task<TSqlFragment> TryGetSqlFragment(string sql)
    {
        return GetSqlFragment(sql);
    }

    public async Task<DefaultSqlScriptGenerator> GetSqlGenerator()
    {
        CompatibilityLevel ??= await CompatibilityLevelProvider.GetCompatibilityLevelWithTimeout();
        return SqlGeneratorFactory.GetGenerator(CompatibilityLevel.Value);
    }

}
