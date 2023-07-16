using System.Collections.Immutable;
using Ivtem.TSqlParsing.Feature.SqlFragment;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Ivtem.TSqlParsing.Feature.SelectQuery;

public class TableNamesProvider
{
    public ImmutableArray<TableReferenceInfo> GetTableNames(TSqlFragment sqlFragment)
    {
        var visitor = new FromClauseVisitor();
        sqlFragment.Accept(visitor);

        return visitor.GetTables();
    }

}

public record TableReferenceInfo(string? ServerIdentifier, string? DatabaseIdentifier, string? SchemaIdentifier,
    string? BaseIdentifier);