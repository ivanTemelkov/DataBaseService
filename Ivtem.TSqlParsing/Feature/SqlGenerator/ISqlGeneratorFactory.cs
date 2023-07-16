using Ivtem.TSqlParsing.Feature.CompatibilityLevel;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Ivtem.TSqlParsing.Feature.SqlGenerator;

public interface ISqlGeneratorFactory
{
    SqlGenerator GetGenerator(TSqlCompatibilityLevel compatibilityLevel);
}