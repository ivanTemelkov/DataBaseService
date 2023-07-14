using System.Collections.Immutable;
using Ivtem.TSqlParsing.Feature.SqlFragment;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Ivtem.TSqlParsing.Feature.ColumnNames;

public class SelectColumnNamesProvider : ISelectColumnNamesProvider
{
    public ImmutableArray<string> GetColumnNames(TSqlFragment sqlFragment)
    {
        var visitor = new SelectStatementVisitor();
        sqlFragment.Accept(visitor);
        return visitor.GetColumnNames();
    }
}