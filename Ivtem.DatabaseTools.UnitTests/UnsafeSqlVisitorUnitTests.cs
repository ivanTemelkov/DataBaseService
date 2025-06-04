using Ivtem.TSqlParsing.Feature;
using Ivtem.TSqlParsing.Feature.CompatibilityLevel;
using Ivtem.TSqlParsing.Feature.SqlFragment;
using Ivtem.TSqlParsing.Feature.SqlGenerator;

namespace Ivtem.DatabaseTools.UnitTests;

[TestFixture]
public class UnsafeSqlVisitorUnitTests
{
    private static readonly string UnSuspiciousQuery = """
        SELECT e.ent_id as MachineId,
        	   REPLACE(e.ent_name,'-','_') as MachineName,
               OperLogIn.Operator1,
               OperLogIn.Operator2,
               OperLogIn.Operator3,
        	   CASE
        		WHEN CONCAT(OperLogIn.Operator1,',',OperLogIn.Operator2,',',OperLogIn.Operator3) = ',,' THEN NULL
        		ELSE CONCAT(OperLogIn.Operator1,',',OperLogIn.Operator2,',',OperLogIn.Operator3)
        	   END AS Operators,
        	   round(J.qty_prod,0) AS [Qta],
               J.qty_reqd AS [QtaReq],
               CASE WHEN J.qty_reqd = 0 THEN '0' WHEN J.qty_prod > j.qty_reqd THEN '100' ELSE FORMAT((J.qty_prod / J.qty_reqd) * 100, 'N2', 'en-US') END AS [QtaPerc]
        FROM  LM_TerminalPlacement AS T
        INNER JOIN ent AS E
        	ON T.ent_id = E.ent_id
        LEFT JOIN LM_MachinePlacement MP (NOLOCK) ON MP.ent_id = e.ent_id
        LEFT OUTER JOIN Job AS J ON J.wo_id = MP.wo_Id AND J.oper_id = MP.oper_Id
        left join [LMV_LHP_Operators] AS OperLogIn on OperLogIn.Ent_Id  = e.ent_id
        WHERE T.terminalId = '(@Global.TerminalId@)'
        """;

    [TestCaseSource(typeof(SuspiciousSelectTestCases), nameof(SuspiciousSelectTestCases.All))]
    public void ItDetectsSuspiciousQueries(string suspiciousQuery)
    {
        var sqlFragmentProvider = new TSqlFragmentProvider();

        var sqlParserGeneratorProvider = new SqlFragmentAndGeneratorProvider(
            PredefinedSqlCompatibilityLevelProvider.TSql160Provider,
            sqlFragmentProvider,
            new SqlGeneratorFactory());

        var sqlFragment = sqlParserGeneratorProvider.GetSqlFragment(suspiciousQuery);

        var unsafeSqlInspector = new UnsafeSqlVisitor();

        sqlFragment.Accept(unsafeSqlInspector);

        Assert.That(unsafeSqlInspector.IsUnsafe, Is.True);
    }
    
    [Test]
    public void ItDoesNotDetectUnSuspiciousQueryAsUnsafe()
    {
        var sqlFragmentProvider = new TSqlFragmentProvider();
        var sqlParserGeneratorProvider = new SqlFragmentAndGeneratorProvider(
            PredefinedSqlCompatibilityLevelProvider.TSql160Provider,
            sqlFragmentProvider,
            new SqlGeneratorFactory());
        var sqlFragment = sqlParserGeneratorProvider.GetSqlFragment(UnSuspiciousQuery);
        var unsafeSqlInspector = new UnsafeSqlVisitor();
        sqlFragment.Accept(unsafeSqlInspector);
        Assert.That(unsafeSqlInspector.IsUnsafe, Is.False);
    }

}