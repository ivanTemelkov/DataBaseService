using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Ivtem.TSqlParsing.Feature.SqlFragment;

public sealed class UnsafeSqlVisitor : TSqlFragmentVisitor
{
    public static readonly HashSet<string> DangerousBuiltInFunctions = new(StringComparer.OrdinalIgnoreCase)
    {
        // Security & Role context
        "suser_name",
        "suser_sname",
        "user_name",
        "original_login",
        "session_user",
        "system_user",
        "current_user",
        "is_srvrolemember",
        "is_member",
        "has_perms_by_name",
        "loginproperty",

        // Metadata & Discovery
        "object_id",
        "object_name",
        "col_name",
        "type_name",
        "schema_name",
        "db_name",
        "database_principal_id",

        // Server/environment context
        "host_name",
        "app_name",
        "@@version",     // technically a global variable, but often string-matched
        "@@servername",

        // Execution trace
        "fn_trace_gettable",  // exposes trace files

        // Extended stored procedures via UDF-style syntax (precaution)
        "xp_cmdshell",
        "xp_dirtree",
        "xp_fileexist",
        "xp_regread",
        "xp_regwrite",
        "sp_executesql"
    };

    private static readonly HashSet<string> ProtectedSystemSchema = new(StringComparer.OrdinalIgnoreCase)
    {
        "sys", "information_schema"
    };
    
    private static readonly HashSet<string> LegacySystemViews = new(StringComparer.OrdinalIgnoreCase)
    {
        "sysobjects",
        "syscolumns",
        "sysindexes",
        "sysreferences",
        "sysfiles",
        "sysusers",
        "sysforeignkeys"
    };
    
    public string? Warning { get; private set; }

    public bool IsUnsafe => Warning is not null;
    
    public override void Visit(OpenRowsetTableReference node)
    {
        Warning = $"OpenRowset is considered unsafe! Detected at line {node.StartLine}";
    }
    
    public override void Visit(OpenQueryTableReference node)
    {
        Warning = $"OpenQuery is considered unsafe! Detected at line {node.StartLine}";
    }
    
    public override void Visit(AdHocTableReference node)
    {
        Warning = $"OpenDataSource (ad hoc) is considered unsafe! Detected at line {node.StartLine}";
    }
    
    public override void Visit(SchemaObjectFunctionTableReference node)
    {
        if (IsUnsafe)
            return;
        
        var functionName = node.SchemaObject.BaseIdentifier?.Value;
        
        if (functionName is null)
            return;
        
        if (DangerousBuiltInFunctions.Contains(functionName) == false)
            return;
        
        Warning = $"Table-valued function call {functionName} is considered unsafe! Detected at line {node.StartLine}";
    }
    
    public override void Visit(FunctionCall node)
    {
        if (IsUnsafe)
            return;
        
        var functionName = node.FunctionName?.Value;
        
        if (functionName is null)
            return;
        
        if (DangerousBuiltInFunctions.Contains(functionName) == false)
            return;
        
        Warning = $"Function call {functionName} is considered unsafe! Detected at line {node.StartLine}";
    }

    public override void Visit(SchemaObjectName node)
    {
        if (IsUnsafe)
            return;
        
        foreach (var id in node.Identifiers)
        {
            var val = id.Value;
            
            if (ProtectedSystemSchema.Contains(val) == false)
                continue;
            
            Warning = $"Reference to system schema {val} is considered unsafe! Detected at line {node.StartLine}";
            return;
        }
    }

    public override void Visit(NamedTableReference node)
    {
        if (IsUnsafe)
            return;
        
        var objectName = node.SchemaObject.BaseIdentifier?.Value;
        if (string.IsNullOrWhiteSpace(objectName) == false && LegacySystemViews.Contains(objectName))
        {
            Warning = $"Use of deprecated system view {objectName} is considered unsafe! Detected at line {node.StartLine}";
            return;
        }
        
        var identifiers = node.SchemaObject?.Identifiers;
        if (identifiers == null || identifiers.Count == 0)
            return;

        // Detect sys.* or INFORMATION_SCHEMA.*
        var schema = identifiers.Count > 1
            ? identifiers[^2].Value
            : string.Empty;

        if (ProtectedSystemSchema.Contains(schema) == false)
            return;
        
        Warning = $"Access to system object {schema} is considered unsafe! Deteced at line {node.StartLine}";
    }
}