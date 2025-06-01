using System.Diagnostics.CodeAnalysis;
using Ivtem.TSqlParsing.Feature.SqlFragment;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Ivtem.TSqlParsing.Feature.SelectQuery;

public sealed class UnsafeSelectQueryDetector
{
    private SelectStatement SelectStatement { get; }

    public UnsafeSelectQueryDetector(SelectStatement selectStatement)
    {
        SelectStatement = selectStatement;
    }
    
    public bool IsSafe([NotNullWhen(returnValue: false)] out string? warning)
    {
        var visitor = new UnsafeSqlVisitor();
        SelectStatement.Accept(visitor);

        warning = visitor.Warning;
        return visitor.IsUnsafe == false;
    }
}