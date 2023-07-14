using Ivtem.TSqlParsing.Feature.CompatibilityLevel;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Ivtem.TSqlParsing.Feature.SQLParser;

public interface ISqlScriptGeneratorFactory
{
    SqlScriptGenerator GetGenerator(TSqlCompatibilityLevel compatibilityLevel);
}