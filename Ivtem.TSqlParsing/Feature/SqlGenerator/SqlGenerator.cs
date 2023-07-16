using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Ivtem.TSqlParsing.Feature.SqlGenerator;

public class SqlGenerator
{
    private SqlScriptGenerator ScriptGenerator { get; }

    public SqlGenerator(SqlScriptGenerator scriptGenerator)
    {
        ScriptGenerator = scriptGenerator;
    }

    public string Generate(TSqlFragment fragment)
    {
        ScriptGenerator.GenerateScript(fragment, out var script, out var errors);

        if (errors is not null && errors.Count > 0)
        {
            throw new InvalidOperationException(errors.ToString());
        }

        if (string.IsNullOrWhiteSpace(script))
        {
            throw new InvalidOperationException("Failed to generate script!");
        }

        return script;
    }

}