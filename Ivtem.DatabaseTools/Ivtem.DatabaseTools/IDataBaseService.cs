using System.Diagnostics.CodeAnalysis;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Ivtem.DatabaseTools;

public interface IDataBaseService
{
    string DataSource { get; }
    string InitialCatalog { get; }
    string ConnectionString { get; }
    TSqlCompatibilityLevel CompatibilityLevel { get; }
    bool TryValidateSqlQuery(string query, [NotNullWhen(false)] out ParseError[]? parseErrors);
}