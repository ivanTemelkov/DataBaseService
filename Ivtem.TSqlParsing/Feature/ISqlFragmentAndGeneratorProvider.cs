using Ivtem.TSqlParsing.Feature.CompatibilityLevel;
using Ivtem.TSqlParsing.Feature.SqlGenerator;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Ivtem.TSqlParsing.Feature;

public interface ISqlFragmentAndGeneratorProvider
{
    Task<TSqlCompatibilityLevel> GetCompatibilityLevel();

    Task<DefaultSqlScriptGenerator> GetSqlGenerator();

    [Obsolete($"Use {nameof(GetSqlFragment)} instead.")]
    Task<TSqlFragment> TryGetSqlFragment(string sql);

    Task<TSqlFragment> GetSqlFragment(string sql);
}