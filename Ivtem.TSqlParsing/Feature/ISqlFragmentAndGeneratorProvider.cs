using Ivtem.TSqlParsing.Feature.CompatibilityLevel;
using Ivtem.TSqlParsing.Feature.SqlGenerator;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Ivtem.TSqlParsing.Feature;

public interface ISqlFragmentAndGeneratorProvider
{
    TSqlCompatibilityLevel GetCompatibilityLevel();

    DefaultSqlScriptGenerator GetSqlGenerator();
    
    TSqlFragment GetSqlFragment(string sql);
}