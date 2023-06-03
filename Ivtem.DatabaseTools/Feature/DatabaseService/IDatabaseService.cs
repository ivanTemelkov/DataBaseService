using System.Diagnostics.CodeAnalysis;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Ivtem.DatabaseTools.Feature.DatabaseService;

public interface IDatabaseService
{
    string DataSource { get; }
    string InitialCatalog { get; }
    string ConnectionString { get; }
    TSqlCompatibilityLevel CompatibilityLevel { get; }
    bool TryValidateSqlQuery(string query, [NotNullWhen(false)] out ParseError[]? parseErrors);
}