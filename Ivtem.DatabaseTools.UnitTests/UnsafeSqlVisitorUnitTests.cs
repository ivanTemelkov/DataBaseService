using Ivtem.TSqlParsing.Feature;
using Ivtem.TSqlParsing.Feature.CompatibilityLevel;
using Ivtem.TSqlParsing.Feature.SqlFragment;
using Ivtem.TSqlParsing.Feature.SqlGenerator;

namespace Ivtem.DatabaseTools.UnitTests;

[TestFixture]
public class UnsafeSqlVisitorUnitTests
{
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

}