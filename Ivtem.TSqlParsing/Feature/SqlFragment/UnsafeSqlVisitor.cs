using System.Collections.Immutable;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Ivtem.TSqlParsing.Feature.SqlFragment;

public sealed class UnsafeSqlVisitor : TSqlFragmentVisitor
{
    public string? Warning { get; private set; }

    public bool IsUnsafe => Warning is not null;
    
    public override void Visit(FunctionCall node)
    {
        if (IsUnsafe)
            return;
        
        var functionName = node.FunctionName?.Value?.ToLowerInvariant();
        
        if (functionName is null)
            return;
        
        if (functionName is not ("xp_cmdshell" or "sp_executesql" or "openrowset" or "openquery"
            or "opendatasource"))
            return;
        
        Warning = $"Dangerous function detected: {functionName}";
    }

    public override void Visit(ExecuteStatement node)
    {
        if (IsUnsafe)
            return;
        
        Warning = "EXEC/EXECUTE statement detected.";
    }

    public override void Visit(SchemaObjectName node)
    {
        if (IsUnsafe)
            return;
        
        foreach (var id in node.Identifiers)
        {
            var val = id.Value;
            if (val.Equals("sys", StringComparison.OrdinalIgnoreCase) ||
                val.Equals("information_schema", StringComparison.OrdinalIgnoreCase))
            {
                Warning = $"Reference to system schema: {val} at line {node.StartLine}";
                return;
            }
        }
    }

    public override void Visit(NamedTableReference node)
    {
        if (IsUnsafe)
            return;
        
        var identifiers = node.SchemaObject?.Identifiers;
        if (identifiers == null || identifiers.Count == 0)
            return;

        // Detect sys.* or INFORMATION_SCHEMA.*
        var schema = identifiers.Count > 1 ? identifiers[^2].Value.ToLowerInvariant() : "";

        if (schema is "sys" or "information_schema")
        {
            Warning = $"System object access: {schema} at line {node.StartLine}";
        }
    }

    public override void Visit(ProcedureReferenceName node)
    {
        if (IsUnsafe)
            return;
        
        if (node.ProcedureReference.Name.BaseIdentifier?.Value.ToLowerInvariant().StartsWith("xp_") == true)
        {
            Warning = $"Extended stored procedure: {node.ProcedureReference.Name}";
        }
    }
}