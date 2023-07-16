using System.Collections.Immutable;
using Ivtem.TSqlParsing.Feature.SqlFragment;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Ivtem.TSqlParsing.Feature.SelectQuery;

public class SelectColumnNamesProvider : ISelectColumnNamesProvider
{
    public ImmutableArray<string> GetColumnNames(TSqlFragment sqlFragment)
    {
        var visitor = new GetColumnNamesVisitor();
        sqlFragment.Accept(visitor);
        return visitor.GetData();
    }
}