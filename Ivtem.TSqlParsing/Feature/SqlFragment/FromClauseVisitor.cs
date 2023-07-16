using System.Collections.Immutable;
using Ivtem.TSqlParsing.Feature.SelectQuery;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Ivtem.TSqlParsing.Feature.SqlFragment;

public class FromClauseVisitor : TSqlConcreteFragmentVisitor
{
    private HashSet<TableReferenceInfo> Tables { get; } = new();

    public override void Visit(FromClause node)
    {
        foreach (var tableReference in node.TableReferences)
        {
            tableReference.Accept(this);
        }
    }

    public override void Visit(NamedTableReference node)
    {
        var schemaObject = node.SchemaObject;

        var serverIdentifier = schemaObject.ServerIdentifier?.Value;
        var databaseIdentifier = schemaObject.DatabaseIdentifier?.Value;
        var schemaIdentifier = schemaObject.SchemaIdentifier?.Value;
        var baseIdentifier = schemaObject.BaseIdentifier?.Value;

        var tableReference = new TableReferenceInfo(serverIdentifier, databaseIdentifier, schemaIdentifier, baseIdentifier);

        Tables.Add(tableReference);
    }

    public ImmutableArray<TableReferenceInfo> GetTables() 
        => Tables.ToImmutableArray();
}