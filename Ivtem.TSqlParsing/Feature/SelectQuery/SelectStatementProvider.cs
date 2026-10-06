using Ivtem.TSqlParsing.Feature.CompatibilityLevel;
using Ivtem.TSqlParsing.Feature.SqlFragment;
using Ivtem.TSqlParsing.Feature.SqlGenerator;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using System.Diagnostics.CodeAnalysis;

namespace Ivtem.TSqlParsing.Feature.SelectQuery;

public class SelectStatementProvider : ISelectStatementProvider
{
    private ISqlGeneratorFactory SqlGeneratorFactory { get; }
    
    private ISqlCompatibilityLevelProvider CompatibilityLevelProvider { get; }

    private DefaultSqlScriptGenerator? SqlScriptGenerator { get; set; }

    public SelectStatementProvider(ISqlGeneratorFactory sqlGeneratorFactory, ISqlCompatibilityLevelProvider compatibilityLevelProvider)
    {
        SqlGeneratorFactory = sqlGeneratorFactory;
        CompatibilityLevelProvider = compatibilityLevelProvider;
    }
    
    public bool TryGetStatement(TSqlFragment sqlFragment, [NotNullWhen(true)] out SelectStatement? selectStatement)
    {
        var selectStatementVisitor = new GetSelectStatementVisitor();
        sqlFragment.Accept(selectStatementVisitor);

        selectStatement = selectStatementVisitor.GetData();

        return selectStatement != null;
    }

    public bool TryGetStatement(TSqlFragment sqlFragment, [NotNullWhen(true)] out SelectStatement? selectStatement, [NotNullWhen(true)] out string? sql)
    {
        sql = null;
        
        if (TryGetStatement(sqlFragment, out selectStatement) == false)
        {
            return false;
        }

        SqlScriptGenerator ??= SqlGeneratorFactory.GetGenerator(CompatibilityLevelProvider.GetCompatibilityLevel());
        
        sql = SqlScriptGenerator.Generate(sqlFragment);

        return true;
    }

    
}