using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Ivtem.DatabaseTools.Feature.SqlParse;

public class SelectScalarExpressionVisitor : TSqlConcreteFragmentVisitor
{
    public string? ColumnName { get; private set; }

    public override void Visit(SelectScalarExpression node)
    {
        ColumnName = node.ColumnName?.Value ?? EvaluateColumnName(node);
    }

    private static string EvaluateColumnName(SelectScalarExpression node) =>
        (node.Expression as ColumnReferenceExpression)?.MultiPartIdentifier.Identifiers.Last().Value ?? throw new NotImplementedException($"node.Expression of type: {node.Expression.GetType().Name} NOT supported!");
}