using Ivtem.TSqlParsing.Feature.CompatibilityLevel;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Ivtem.TSqlParsing.Feature.SQLParser;

public interface ITSqlParserFactory
{
    TSqlCompatibilityLevel CompatibilityLevel { get; }
    string DataSource { get; }
    string InitialCatalog { get; }
    string ConnectionString { get; }
    bool IsInitialized { get; }

    Task Initialize();
    Task<TSqlParser> GetParser();
}