// See https://aka.ms/new-console-template for more information

using Ivtem.TSqlParsing.Feature;
using Ivtem.TSqlParsing.Feature.CompatibilityLevel;
using Ivtem.TSqlParsing.Feature.SelectQuery;
using Ivtem.TSqlParsing.Feature.SqlFragment;
using Ivtem.TSqlParsing.Feature.SqlGenerator;
using Microsoft.SqlServer.TransactSql.ScriptDom;

var selectStatementWithUnion = """
                               /*
                               Some metadata
                               */

                               DROP TABLE LNK_TableName;
                               
                               -- This is considered unsafe
                               -- SELECT * FROM sys.databases;
                               
                               SELECT MachineId, MachineDescription, MachineState, MachineUptime
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

                               SELECT MachineId, MachineDescription, MachineState, MachineUptime
                               FROM (
                                   VALUES
                                       ('M011', 'Plasma Cutter',    'Active',    6000),
                                       ('M012', 'Surface Grinder',  'Idle',      900),
                                       ('M013', 'Sander',           'InFailure', 120),
                                       ('M014', 'Punch Press',      'Active',    4800),
                                       ('M015', 'Lathe #2',         'Idle',      2100)
                               ) AS Machine2(MachineId, MachineDescription, MachineState, MachineUptime);
                               """;
    
    
var sqlFragmentProvider = new TSqlFragmentProvider();

var sqlParserGeneratorProvider = new SqlFragmentAndGeneratorProvider(
    PredefinedSqlCompatibilityLevelProvider.TSql160Provider,
    sqlFragmentProvider,
    new SqlGeneratorFactory());

var scriptGenerator = sqlParserGeneratorProvider.GetSqlGenerator();
var sqlFragment = sqlParserGeneratorProvider.GetSqlFragment(selectStatementWithUnion);

var selectQueryProvider = new SelectStatementProvider();

if (selectQueryProvider.TryGetStatement(sqlFragment, out var selectStatement))
{
    var detector = new UnsafeSelectQueryDetector(selectStatement);
    if (detector.IsSafe(out var warning) == false)
    {
        Console.WriteLine(warning);
        return;
    }

    var selectColumnNamesProvider = new SelectColumnNamesProvider();

    var columnNames = selectColumnNamesProvider.GetColumnNames(selectStatement);

    foreach (var columnName in columnNames)
    {
        Console.WriteLine(columnName);
    }

    var innerQuery = selectStatement.QueryExpression;

    var derivedTable = new QueryDerivedTable
    {
        QueryExpression = innerQuery,
        Alias = new Identifier { Value = "[__inner_query__]" }
    };

    var whereCondition = new BooleanComparisonExpression
    {
        ComparisonType = BooleanComparisonType.Equals,
        FirstExpression = new ColumnReferenceExpression
        {
            MultiPartIdentifier = new MultiPartIdentifier
            {
                Identifiers =
                {
                    new Identifier { Value = "[__inner_query__]" },
                    new Identifier { Value = "MachineId" }
                }
            }
        },
        SecondExpression = new StringLiteral
        {
            Value = "M001"
        }
    };
    
    var outerQuerySpec = new QuerySpecification
    {
        FromClause = new FromClause
        {
            TableReferences = { derivedTable }
        },
        WhereClause = new WhereClause
        {
            SearchCondition = whereCondition
        }
    };
    
    outerQuerySpec.SelectElements.Add(new SelectStarExpression());

    outerQuerySpec.TopRowFilter = new TopRowFilter
    {
        Expression = new IntegerLiteral { Value = "3" },
        Percent = false,
        WithTies = false
    };

    outerQuerySpec.OrderByClause = new OrderByClause
    {
        OrderByElements =
        {
            new ExpressionWithSortOrder
            {
                Expression = new ColumnReferenceExpression
                {
                    MultiPartIdentifier = new MultiPartIdentifier
                    {
                        Identifiers =
                        {
                            new Identifier { Value = "[__inner_query__]" },
                            new Identifier { Value = "MachineId" }
                        }
                    }
                },
                SortOrder = SortOrder.Descending
            },
            new ExpressionWithSortOrder
            {
                Expression = new ColumnReferenceExpression
                {
                    MultiPartIdentifier = new MultiPartIdentifier
                    {
                        Identifiers =
                        {
                            new Identifier { Value = "[__inner_query__]" },
                            new Identifier { Value = "MachineState" }
                        }
                    }
                },
                SortOrder = SortOrder.Ascending
            }
        }
    };
    
    var newSelectStatement = new SelectStatement
    {
        QueryExpression = outerQuerySpec
    };

    var script = scriptGenerator.Generate(newSelectStatement);
    Console.WriteLine(script);
}
