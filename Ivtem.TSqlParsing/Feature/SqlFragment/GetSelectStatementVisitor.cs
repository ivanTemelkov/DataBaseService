using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Ivtem.TSqlParsing.Feature.SqlFragment;

public class GetSelectStatementVisitor : GetDataVisitor<SelectStatement?>
{
    private SelectStatement? SelectStatement { get; set; }
    
    protected override SelectStatement? GetVisitorData()
        => SelectStatement;
    
    public override void ExplicitVisit(TSqlScript node)
    {
        IsVisitCalled = true;

        foreach (var batch in node.Batches)
        {
            batch.Accept(this);
            if (SelectStatement is not null)
                return;
        }
    }

    public override void ExplicitVisit(TSqlBatch node)
    {
        var selectStatements = node.Statements
            .OfType<SelectStatement>();
        
        SelectStatement = selectStatements
            .FirstOrDefault();
    }
}