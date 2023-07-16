using System.Diagnostics.CodeAnalysis;
using Ivtem.TSqlParsing.Feature.SqlFragment;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Ivtem.TSqlParsing.Feature.SelectQuery;

public class SelectStatementProvider : ISelectStatementProvider
{
    public bool TryGetStatement(TSqlFragment sqlFragment, [NotNullWhen(true)] out SelectStatement? selectStatement)
    {
        var selectStatementVisitor = new GetSelectStatementVisitor();
        sqlFragment.Accept(selectStatementVisitor);

        selectStatement = selectStatementVisitor.GetData();

        return selectStatement != null;
    }
}