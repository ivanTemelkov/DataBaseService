using Ivtem.TSqlParsing.Feature.CompatibilityLevel;
using Ivtem.TSqlParsing.Feature.SqlGenerator;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Ivtem.TSqlParsing.Feature;

public interface ISqlFragmentAndGeneratorProvider
{
    Task<TSqlCompatibilityLevel> GetCompatibilityLevel();

    Task<DefaultSqlScriptGenerator> GetSqlGenerator();
    Task<TSqlFragment> TryGetSqlFragment(string sql);
}