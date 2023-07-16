using Ivtem.TSqlParsing.Feature.CompatibilityLevel;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Ivtem.TSqlParsing.Feature.SqlParser;

public interface ITSqlParserFactory
{
    TSqlParser GetParser(TSqlCompatibilityLevel compatibilityLevel);
}