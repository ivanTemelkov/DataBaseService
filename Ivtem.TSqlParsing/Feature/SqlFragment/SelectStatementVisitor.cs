using System.Collections.Immutable;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Ivtem.TSqlParsing.Feature.SqlFragment;

public class SelectStatementVisitor : TSqlConcreteFragmentVisitor
{
    private ImmutableArray<string>? ColumnNames { get; set; }

    private bool IsVisitCalled { get; set; }

    public override void Visit(SelectStatement node)
    {
        IsVisitCalled = true;

        var queryExpression = node.QueryExpression;

        var selectElements = (queryExpression as QuerySpecification)?.SelectElements.OfType<SelectScalarExpression>();

        if (selectElements is null) return;

        var columnNames = new List<string>();

        foreach (var selectElement in selectElements)
        {
            var visitor = new SelectScalarExpressionVisitor();
            selectElement.Accept(visitor);

            if (string.IsNullOrWhiteSpace(visitor.ColumnName)) continue;

            columnNames.Add(visitor.ColumnName);
        }

        ColumnNames = columnNames.ToImmutableArray();
    }

    public ImmutableArray<string> GetColumnNames()
    {
        if (IsVisitCalled == false)
            throw new InvalidOperationException($"Cannot get {nameof(ColumnNames)} if {nameof(Visit)} was not called!");

        return ColumnNames ?? throw new InvalidOperationException($"{nameof(ColumnNames)} is null!");
    }
}