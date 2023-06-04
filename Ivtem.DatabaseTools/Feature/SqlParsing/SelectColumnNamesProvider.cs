using System.Collections.Immutable;
using Ivtem.DatabaseTools.Feature.SqlParse;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Ivtem.DatabaseTools.Feature.SqlParsing;

public class SelectColumnNamesProvider
{
    public ImmutableArray<string> GetColumnNames(TSqlFragment sqlFragment)
    {
        var visitor = new SelectStatementVisitor();
        sqlFragment.Accept(visitor);
        return visitor.GetColumnNames();
    }
}