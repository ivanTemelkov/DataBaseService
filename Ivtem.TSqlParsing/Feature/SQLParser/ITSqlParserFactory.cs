using Ivtem.TSqlParsing.Feature.CompatibilityLevel;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Ivtem.TSqlParsing.Feature.SQLParser;

public interface ITSqlParserFactory
{
    TSqlParser GetParser(TSqlCompatibilityLevel compatibilityLevel);
}