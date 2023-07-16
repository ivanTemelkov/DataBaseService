﻿using System.Collections.Immutable;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Ivtem.TSqlParsing.Feature.SqlFragment;

public class GetColumnNamesVisitor : GetDataVisitor<ImmutableArray<string>>
{
    public string? ColumnName { get; private set; }

    private ImmutableArray<string>? ColumnNames { get; set; }
    
    public override void Visit(SelectStatement node)
    {
        IsVisitCalled = true;

        var queryExpression = node.QueryExpression;

        var selectElements = (queryExpression as QuerySpecification)?.SelectElements.OfType<SelectScalarExpression>();

        if (selectElements is null) return;

        var columnNames = new List<string>();

        foreach (var selectElement in selectElements)
        {
            selectElement.Accept(this);

            if (string.IsNullOrWhiteSpace(ColumnName)) continue;

            columnNames.Add(ColumnName);
        }

        ColumnNames = columnNames.ToImmutableArray();
    }
    
    protected override ImmutableArray<string> GetVisitorData()
    {
        return ColumnNames ?? throw new InvalidOperationException($"{nameof(ColumnNames)} is null!");
    }

    public override void Visit(SelectScalarExpression node)
    {
        ColumnName = node.ColumnName?.Value ?? EvaluateColumnName(node);
    }

    private static string EvaluateColumnName(SelectScalarExpression node) =>
        (node.Expression as ColumnReferenceExpression)?.MultiPartIdentifier.Identifiers.Last().Value ?? throw new NotImplementedException($"node.Expression of type: {node.Expression.GetType().Name} NOT supported!");
}