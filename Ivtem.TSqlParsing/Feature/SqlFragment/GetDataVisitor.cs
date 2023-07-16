using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Ivtem.TSqlParsing.Feature.SqlFragment;

public abstract class GetDataVisitor<T> : TSqlConcreteFragmentVisitor
{
    protected bool IsVisitCalled { get; set; }
    
    public T GetData()
    {
        if (IsVisitCalled == false)
            throw new InvalidOperationException($"Cannot call {nameof(GetData)}() before calling {nameof(TSqlFragment.Accept)}()!");

        return GetVisitorData();
    }

    protected abstract T GetVisitorData();
}