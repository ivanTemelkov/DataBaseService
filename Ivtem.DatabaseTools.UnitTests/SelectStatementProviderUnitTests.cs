using Ivtem.TSqlParsing.Feature;
using Ivtem.TSqlParsing.Feature.CompatibilityLevel;
using Ivtem.TSqlParsing.Feature.SelectQuery;
using Ivtem.TSqlParsing.Feature.SqlFragment;
using Ivtem.TSqlParsing.Feature.SqlGenerator;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Ivtem.DatabaseTools.UnitTests;

[TestFixture]
public class SelectStatementProviderUnitTests
{
    private static readonly string SelectStatementWithUnion = """
                                                              /*
                                                              Some metadata
                                                              */
                                                              
                                                              SELECT *
                                                              FROM (
                                                                  VALUES
                                                                      ('M001', 'Lathe Machine',    'Active',    3600),
                                                                      ('M002', 'Drill Press',      'Idle',      1800),
                                                                      ('M003', 'Milling Machine',  'InFailure', 0),
                                                                      ('M004', 'Grinder',          'Active',    5400),
                                                                      ('M005', 'CNC Router',       'Idle',      2400),
                                                                      ('M006', '3D Printer',       'Active',    7200),
                                                                      ('M007', 'Welder',           'InFailure', 300),
                                                                      ('M008', 'Laser Cutter',     'Idle',      1200),
                                                                      ('M009', 'Hydraulic Press',  'Active',    4000),
                                                                      ('M010', 'Waterjet Cutter',  'InFailure', 0)
                                                              ) AS Machine(MachineId, MachineDescription, MachineState, MachineUptime)
                                                              
                                                              UNION
                                                              
                                                              SELECT *
                                                              FROM (
                                                                  VALUES
                                                                      ('M011', 'Plasma Cutter',    'Active',    6000),
                                                                      ('M012', 'Surface Grinder',  'Idle',      900),
                                                                      ('M013', 'Sander',           'InFailure', 120),
                                                                      ('M014', 'Punch Press',      'Active',    4800),
                                                                      ('M015', 'Lathe #2',         'Idle',      2100)
                                                              ) AS Machine2(MachineId, MachineDescription, MachineState, MachineUptime);
                                                              """;
    
    private static readonly string SelectStatementWithUnionAll = """
                                                              /*
                                                              Some metadata
                                                              */
                                                              
                                                              SELECT *
                                                              FROM (
                                                                  VALUES
                                                                      ('M001', 'Lathe Machine',    'Active',    3600),
                                                                      ('M002', 'Drill Press',      'Idle',      1800),
                                                                      ('M003', 'Milling Machine',  'InFailure', 0),
                                                                      ('M004', 'Grinder',          'Active',    5400),
                                                                      ('M005', 'CNC Router',       'Idle',      2400),
                                                                      ('M006', '3D Printer',       'Active',    7200),
                                                                      ('M007', 'Welder',           'InFailure', 300),
                                                                      ('M008', 'Laser Cutter',     'Idle',      1200),
                                                                      ('M009', 'Hydraulic Press',  'Active',    4000),
                                                                      ('M010', 'Waterjet Cutter',  'InFailure', 0)
                                                              ) AS Machine(MachineId, MachineDescription, MachineState, MachineUptime)
                                                              
                                                              UNION ALL
                                                              
                                                              SELECT *
                                                              FROM (
                                                                  VALUES
                                                                      ('M011', 'Plasma Cutter',    'Active',    6000),
                                                                      ('M012', 'Surface Grinder',  'Idle',      900),
                                                                      ('M013', 'Sander',           'InFailure', 120),
                                                                      ('M014', 'Punch Press',      'Active',    4800),
                                                                      ('M015', 'Lathe #2',         'Idle',      2100)
                                                              ) AS Machine2(MachineId, MachineDescription, MachineState, MachineUptime);
                                                              """;
    
    private static readonly string SelectStatementWithUnionAllAndCte = """
                                                              /*
                                                              Some metadata
                                                              */
                                                              
                                                              ;WITH MachineData AS (
                                                                  SELECT *
                                                                  FROM (
                                                                      VALUES
                                                                          ('M001', 'Lathe Machine',    'Active',    3600),
                                                                          ('M002', 'Drill Press',      'Idle',      1800),
                                                                          ('M003', 'Milling Machine',  'InFailure', 0),
                                                                          ('M004', 'Grinder',          'Active',    5400),
                                                                          ('M005', 'CNC Router',       'Idle',      2400),
                                                                          ('M006', '3D Printer',       'Active',    7200),
                                                                          ('M007', 'Welder',           'InFailure', 300),
                                                                          ('M008', 'Laser Cutter',     'Idle',      1200),
                                                                          ('M009', 'Hydraulic Press',  'Active',    4000),
                                                                          ('M010', 'Waterjet Cutter',  'InFailure', 0)
                                                                  ) AS A(MachineId, MachineDescription, MachineState, MachineUptime)
                                                              
                                                                  UNION ALL
                                                              
                                                                  SELECT *
                                                                  FROM (
                                                                      VALUES
                                                                          ('M011', 'Plasma Cutter',    'Active',    6000),
                                                                          ('M012', 'Surface Grinder',  'Idle',      900),
                                                                          ('M013', 'Sander',           'InFailure', 120),
                                                                          ('M014', 'Punch Press',      'Active',    4800),
                                                                          ('M015', 'Lathe #2',         'Idle',      2100)
                                                                  ) AS B(MachineId, MachineDescription, MachineState, MachineUptime)
                                                              ),
                                                              
                                                              AdditionalData AS (
                                                                  SELECT *
                                                                  FROM (
                                                                      VALUES
                                                                          ('X001', 'Test Machine A', 'Idle',      100),
                                                                          ('X002', 'Test Machine B', 'Active',    250),
                                                                          ('X003', 'Test Machine C', 'InFailure', 0)
                                                                  ) AS C(MachineId, MachineDescription, MachineState, MachineUptime)
                                                              )
                                                              
                                                              -- Final result: combine both datasets
                                                              SELECT * FROM MachineData
                                                              UNION ALL
                                                              SELECT * FROM AdditionalData;
                                                              
                                                              """;

    private static readonly string SelectStatementWithOuterApply = """
                                                                   /*
                                                                   Some metadata
                                                                   */
                                                                   
                                                                   ;WITH UptimeMapping AS (
                                                                       SELECT * FROM (
                                                                           VALUES
                                                                               ('M001', 3600),
                                                                               ('M002', 1800),
                                                                               ('M003', 0),
                                                                               ('M004', 5400),
                                                                               ('M005', 2400)
                                                                       ) AS u(MachineId, MachineUptime)
                                                                   )
                                                                   SELECT 
                                                                       m.MachineId,
                                                                       m.MachineDescription,
                                                                       m.MachineState,
                                                                       u.MachineUptime
                                                                   FROM (
                                                                       VALUES
                                                                           ('M001', 'Lathe Machine',    'Active'),
                                                                           ('M002', 'Drill Press',      'Idle'),
                                                                           ('M003', 'Milling Machine',  'InFailure'),
                                                                           ('M004', 'Grinder',          'Active'),
                                                                           ('M005', 'CNC Router',       'Idle')
                                                                   ) AS m(MachineId, MachineDescription, MachineState)
                                                                   OUTER APPLY (
                                                                       SELECT MachineUptime
                                                                       FROM UptimeMapping u
                                                                       WHERE u.MachineId = m.MachineId
                                                                   ) AS u;
                                                                   """;

    [Test]
    public void ItFindsSelectStatementWithUnion()
    {
        var sqlFragmentProvider = new TSqlFragmentProvider();

        var sqlParserGeneratorProvider = new SqlFragmentAndGeneratorProvider(
            PredefinedSqlCompatibilityLevelProvider.TSql160Provider,
            sqlFragmentProvider,
            new SqlGeneratorFactory());

        var sqlFragment = sqlParserGeneratorProvider.GetSqlFragment(SelectStatementWithUnion);
        
        var selectQueryProvider = new SelectStatementProvider();

        if (selectQueryProvider.TryGetStatement(sqlFragment, out var selectStatement))
        {
            if (selectStatement.QueryExpression is BinaryQueryExpression binaryQueryExpression)
            {
                var actual = binaryQueryExpression.BinaryQueryExpressionType;
                var expected = BinaryQueryExpressionType.Union;
                Assert.That(actual, Is.EqualTo(expected));
                return;
            }
        }
        
        Assert.Fail();
    }
    
    [Test]
    public void ItFindsSelectStatementWithUnionAll()
    {
        var sqlFragmentProvider = new TSqlFragmentProvider();

        var sqlParserGeneratorProvider = new SqlFragmentAndGeneratorProvider(
            PredefinedSqlCompatibilityLevelProvider.TSql160Provider,
            sqlFragmentProvider,
            new SqlGeneratorFactory());

        var sqlFragment = sqlParserGeneratorProvider.GetSqlFragment(SelectStatementWithUnionAll);
        
        var selectQueryProvider = new SelectStatementProvider();

        if (selectQueryProvider.TryGetStatement(sqlFragment, out var selectStatement))
        {
            if (selectStatement.QueryExpression is BinaryQueryExpression binaryQueryExpression)
            {
                var actual = binaryQueryExpression.BinaryQueryExpressionType;
                var expected = BinaryQueryExpressionType.Union;
                Assert.That(actual, Is.EqualTo(expected));
                return;
            }
        }
        
        Assert.Fail();
    }
    
    [Test]
    public void ItFindsSelectStatementWithUnionAllAndCte()
    {
        var sqlFragmentProvider = new TSqlFragmentProvider();

        var sqlParserGeneratorProvider = new SqlFragmentAndGeneratorProvider(
            PredefinedSqlCompatibilityLevelProvider.TSql160Provider,
            sqlFragmentProvider,
            new SqlGeneratorFactory());

        var sqlFragment = sqlParserGeneratorProvider.GetSqlFragment(SelectStatementWithUnionAllAndCte);
        
        var selectQueryProvider = new SelectStatementProvider();

        if (selectQueryProvider.TryGetStatement(sqlFragment, out var selectStatement))
        {
            if (selectStatement.QueryExpression is BinaryQueryExpression binaryQueryExpression)
            {
                var actual = binaryQueryExpression.BinaryQueryExpressionType;
                var expected = BinaryQueryExpressionType.Union;
                Assert.That(actual, Is.EqualTo(expected));
                return;
            }
        }
        
        Assert.Fail();
    }
    
    [Test]
    public void ItFindsSelectStatementWithOuterApply()
    {
        var sqlFragmentProvider = new TSqlFragmentProvider();

        var sqlParserGeneratorProvider = new SqlFragmentAndGeneratorProvider(
            PredefinedSqlCompatibilityLevelProvider.TSql160Provider,
            sqlFragmentProvider,
            new SqlGeneratorFactory());

        var scriptGenerator = sqlParserGeneratorProvider.GetSqlGenerator();
        var sqlFragment = sqlParserGeneratorProvider.GetSqlFragment(SelectStatementWithOuterApply);
        
        var script = scriptGenerator.Generate(sqlFragment);
        
        var selectQueryProvider = new SelectStatementProvider();

        if (selectQueryProvider.TryGetStatement(sqlFragment, out var selectStatement))
        {
            
            if (selectStatement.QueryExpression is BinaryQueryExpression binaryQueryExpression)
            {
                var actual = binaryQueryExpression.BinaryQueryExpressionType;
                var expected = BinaryQueryExpressionType.Union;
                Assert.That(actual, Is.EqualTo(expected));
                return;
            }
        }
        
        Assert.Fail();
    }
    
}