using System.Collections.Immutable;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Ivtem.TSqlParsing.Feature.SqlFragment;

public class GetColumnNamesVisitor : GetDataVisitor<ImmutableArray<string>>
{
    public string? ColumnName { get; private set; }

    private ImmutableArray<string> ColumnNames { get; set; } = ImmutableArray<string>.Empty;
    
    public override void ExplicitVisit(SelectStatement node)
    {
        IsVisitCalled = true;

        
        var queryExpression = node.QueryExpression;

        IEnumerable<SelectScalarExpression>? selectElements;
        if (queryExpression is BinaryQueryExpression binaryQueryExpression)
        {
            selectElements = (binaryQueryExpression.FirstQueryExpression as QuerySpecification)?
                .SelectElements
                .OfType<SelectScalarExpression>();
        }
        else
        {
            selectElements = (queryExpression as QuerySpecification)?
                .SelectElements
                .OfType<SelectScalarExpression>();
        }

        if (selectElements is null)
            return;

        var columnNames = new List<string>();

        foreach (var selectElement in selectElements)
        {
            selectElement.Accept(this);

            if (string.IsNullOrWhiteSpace(ColumnName))
                continue;

            columnNames.Add(ColumnName);
        }

        ColumnNames = [..columnNames];
    }
    

    public override void ExplicitVisit(SelectScalarExpression node)
    {
        ColumnName = node.ColumnName?.Value ?? EvaluateColumnName(node);
    }
    
    protected override ImmutableArray<string> GetVisitorData()
    {
        return ColumnNames;
    }

    private static string EvaluateColumnName(SelectScalarExpression node) =>
        (node.Expression as ColumnReferenceExpression)?.MultiPartIdentifier.Identifiers.Last().Value ?? string.Empty;
}
