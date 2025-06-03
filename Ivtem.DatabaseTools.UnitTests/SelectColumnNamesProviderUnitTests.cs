using Ivtem.TSqlParsing.Feature;
using Ivtem.TSqlParsing.Feature.CompatibilityLevel;
using Ivtem.TSqlParsing.Feature.SelectQuery;
using Ivtem.TSqlParsing.Feature.SqlFragment;
using NUnit.Framework.Constraints;
using System.Reflection.PortableExecutable;

namespace Ivtem.DatabaseTools.UnitTests;

public class SelectColumnNamesProviderUnitTests
{
    private static string SelectStatementWithCte => @"
;WITH [MachinePhases] AS
(
	SELECT [ma_codice] AS [MachineId],
	       [fa_id] AS [PhaseId]
    FROM [Piazzamenti] (NOLOCK)
    WHERE [ma_codice] IN (@@machineIds@@)
),
[Production] AS 
(
	SELECT [MP].[MachineId],
	       [Prod].[fa_id] AS [PhaseId],
	       [Prod].[dp_azione] AS [ActivityType],
	       [Prod].[dp_datain] AS [ActivityStart],
	       [Prod].[dp_datafi] AS [ActivityEnd],
	       [Prod].[dp_oremacc] * 60.0 AS [PreviousSetupMinutes]
    FROM Produzione (NOLOCK) AS [Prod]
    INNER JOIN [MachinePhases] AS [MP] ON [Prod].[ma_codice] = [MP].[MachineId]
    WHERE [dp_datafi] > DATEADD(month, -2, GETDATE())
),
[LastActivity] AS
(
	SELECT [M].[MachineId],
	       [M].[PhaseId],
		   MAX([PRD].[ActivityEnd]) AS [ActivityEnd]
	FROM [Production] AS [PRD]
	INNER JOIN [MachinePhases] AS [M] ON [M].[MachineId] = [PRD].[MachineId] AND [PRD].[ActivityType] = '02'
	GROUP BY [M].[MachineId], [M].[PhaseId]
),
[LastSetups] AS 
(
	SELECT [P].[MachineId],
	       [P].[PhaseId],
    	   [P].[PreviousSetupMinutes],
    	   [P].[ActivityEnd]
	FROM [Production] AS [P]
	LEFT JOIN [LastActivity] AS [LA] ON [P].[MachineId] = [LA].[MachineId] AND [P].[PhaseId] = [LA].[PhaseId]
	WHERE [P].[ActivityStart] > [LA].[ActivityEnd] AND [P].[ActivityType] = '04'
),
[SumLastSetups] AS
(
	SELECT [MachineId],
	       [PhaseId],
    	   SUM([PreviousSetupMinutes]) AS [PreviousSetupMinutes],
    	   MAX([ActivityEnd]) AS [SetupEnd]
	FROM [LastSetups]
	GROUP BY [MachineId], [PhaseId]
)
SELECT 
	  [MP].[MachineId],
	  [MP].[PhaseId],
	  [F].[fa_oremvrs] * 60 AS [PreviousRunningMachineTimeMinutes],
	  [F].[fa_orelvrs] * 60 AS [PreviousRunningOperatorTimeMinutes],
	  [LS].[PreviousSetupMinutes],
	  [LS].[SetupEnd]
FROM [MachinePhases] AS [MP]
INNER JOIN [Fasi] AS [F] (NOLOCK) ON [MP].[PhaseId] = [F].[fa_id]
LEFT JOIN [SumLastSetups] AS [LS] ON [MP].[MachineId] = [LS].[MachineId] AND [MP].[PhaseId] = [LS].[PhaseId];
";

    private static string SelectAllStatement => @"
SELECT * FROM SomeTable;
";

    private static string SelectFieldsStatement => @"
SELECT Field1, Field2, Field3 FROM SomeTable;
";

    private static string SelectTop1FieldsStatement => @"
SELECT TOP 1 Field1, Field2, Field3 FROM SomeTable;
";

    private static string SelectFieldsAsStatement => @"
SELECT Field1 AS [NewName1], Field2, Field3 AS [NewName3] FROM SomeTable;
";

    // Without AS Count this breaks because the column Expression is seen as FunctionCall
    private static string SelectCountStatement => """
                                                  SELECT COUNT(Field1) AS Count FROM SomeTable GROUP BY Field1;
                                                  """;
    
    private static string SelectUnionStatement => """
                                                  SELECT Field1, Field2, Field3 FROM SomeTable
                                                  UNION
                                                  SELECT Field1, Field2, Field3 FROM SomeOtherTable;
                                                  """;
    
    private static string SelectUnionAllStatement => """
                                                     SELECT Field1, Field2, Field3 FROM SomeTable
                                                     UNION ALL
                                                     SELECT Field1, Field2, Field3 FROM SomeOtherTable;
                                                     """;
    
    private static string SelectComplexUnionStatement => """
        SELECT
            MachineId,
            MachineName,
            GoodParts,
            Scraps,
            Operator,
            ProductId,
            ProductName,
            OrderId,
            CASE WHEN ROW_NUMBER() OVER (ORDER BY MachineId) % 2 = 0 THEN 1 ELSE 0 END AS ShowInput
        FROM
        (
            VALUES
            ('MAC006', 'Piegatrice Lamiere', 320, 10, 1800, 22, 4, 2, 'Alessandro Ferrari', 'PL10', 'Rivestimento Metallo', 'OR1544', '5em'),
            ('MAC007', 'Forno Industriale', 0, 0, 0, 0, 0, 0, 'Paolo Russo', 'FI33', 'Braccio Supporto', 'OR1374', '5rem'),
            ('MAC008', 'Presse Idrauliche', 5, 0, 2500, 5, 10, 5, 'Davide Ricci', 'PH17', 'Telaio Macchina', 'OR1364', '2vh'),
            ('MAC009', 'Taglierina Plasma', 210, 7, 1700, 20, 12, 2, 'Giulia Moretti', 'TP14', 'Disco Metallico', 'OR1244', '2vw'),
            ('MAC010', 'Stampante 3D', 455, 2, 500, 20, 10, 6, 'Federico Conti', 'S3D7', 'Supporto Prototipo', 'OR1366', '0'),
            ('MAC011', 'Burattatrice', 44, 2, 1500, 24, 10, 1, 'Maurizio Lugli', 'OU33', 'Corpo Motore', 'OR1153', '0px'),
            ('MAC012', 'Incisore Laserr', 0, 0, 0, 0, 11, 5, 'Marco Rossi', 'K311', 'Braccetto Alluminio', 'OR1224', '5em'),
            ('MAC013', 'Trapano Vortex', 11, 5, 1200, 27, 33, 30, 'Franco Bianchi', 'FA13', 'Pannello KM', 'OR1233', '5em'),
            ('MAC014', 'Saldatrice TIG', 33, 1, 2000, 5, 3, 15, 'Marco Marras', 'SA19', 'Supporto Trasmissione', 'OR1377', '5em'),
            ('MAC015', 'Fresa Verticale', 75, 3, 1000, 7, 5, 0, 'Luca Lutelli', 'TO22', 'Braccetto Tergicristallo', 'OR1324', '5em')
        )
        AS MockData (
            MachineId,
            MachineName,
            GoodParts,    
            Scraps,
            OrderTarget,
            ActiveTime,
            InSetupTime,
            InFailureTime,
            Operator,
            ProductId,    
            ProductName,    
            OrderId,    
            Width    
        )
        
        UNION ALL 
        
        SELECT
            MachineId,
            MachineName,
            GoodParts,
            Scraps,
            Operator,
            ProductId,
            ProductName,
            OrderId,
            CASE WHEN ROW_NUMBER() OVER (ORDER BY MachineId) % 2 = 0 THEN 1 ELSE 0 END AS ShowInput
        FROM
        (
            VALUES
            ('MAC001', 'Pressa Rotativa', 22, 2, 1500, 30, 10, 4, 'Maurizio Paolini', 'PC11', 'Braccio Motore', 'OR1344', '*'),
            ('MAC002', 'Taglierina Laser', 0, 0, 0, 0, 22, 3, 'Giovanni Rossi', 'TL21', 'Supporto Alluminio', 'OR1214', '1*'),
            ('MAC003', 'Fresa CNC', 150, 5, 1200, 11, 20, 30, 'Elena Bianchi', 'FC13', 'Pannello Controllo', 'OR1351', '1 *'),
            ('MAC004', 'Saldatrice Automatica', 33, 1, 2000, 12, 4, 15, 'Marco Verdi', 'SA19', 'Telaio Strutturale', 'OR1444', '5 px'),
            ('MAC005', 'Tornio Automatico', 75, 3, 1000, 12, 5, 0, 'Laura Neri', 'TA22', 'Albero Trasmissione', 'OR1576', '15px')
        )
        AS MockData (
            MachineId,
            MachineName,
            GoodParts,    
            Scraps,
            OrderTarget,
            ActiveTime,
            InSetupTime,
            InFailureTime,
            Operator,
            ProductId,    
            ProductName,    
            OrderId,    
            Width    
        )
        """;

    private static InputResultData[] TestData => new[]
    {
        new InputResultData(SelectStatementWithCte, new []
        {
            "MachineId", "PhaseId", "PreviousRunningMachineTimeMinutes", "PreviousRunningOperatorTimeMinutes", "PreviousSetupMinutes", "SetupEnd"
        }),
        new InputResultData(SelectAllStatement, Array.Empty<string>()),
        new InputResultData(SelectFieldsStatement, new [] { "Field1", "Field2", "Field3" }),
        new InputResultData(SelectTop1FieldsStatement, new [] { "Field1", "Field2", "Field3" }),
        new InputResultData(SelectCountStatement, new [] { "Count" }),
        new InputResultData(SelectFieldsAsStatement, new [] { "NewName1", "Field2", "NewName3" }),
        new InputResultData(SelectUnionStatement, new [] { "Field1", "Field2", "Field3" }),
        new InputResultData(SelectUnionAllStatement, new [] { "Field1", "Field2", "Field3" }),
        new InputResultData(SelectComplexUnionStatement, new []
        {
            "MachineId", "MachineName", "GoodParts", "Scraps", "Operator", "ProductId", "ProductName", "OrderId", "ShowInput"
        })
    };

    private TSqlFragmentProvider FragmentProvider { get; } = new();

    [SetUp]
    public void Setup()
    {
    }

    [Test]
    [TestCase(0)]
    [TestCase(1)]
    [TestCase(2)]
    [TestCase(3)]
    [TestCase(4)]
    [TestCase(5)]
    [TestCase(6)]
    [TestCase(7)]
    [TestCase(8)]
    public void ItParsesCorrectly(int index)
    {
        var testData = TestData[index];
        var sql = testData.SqlQuery;
        var expectedColumnNames = testData.ColumnNames;
                
        var isParsed = FragmentProvider.TryGetSqlFragment(sql, TSqlCompatibilityLevel.TSql110, out var sqlFragment, out _);

        Assert.That(isParsed, Is.True);

        isParsed = new SelectStatementProvider().TryGetStatement(sqlFragment!, out var selectStatement);
        
        Assert.That(isParsed, Is.True);
        
        var sut = new SelectColumnNamesProvider();

        var actualColumnNames = sut.GetColumnNames(selectStatement!);

        Assert.That(actualColumnNames, Is.EquivalentTo(expectedColumnNames));

    }


    private class InputResultData
    {
        public string SqlQuery { get; }

        public string[] ColumnNames { get; }

        public InputResultData(string sqlQuery, string[] columnNames)
        {
            SqlQuery = sqlQuery;
            ColumnNames = columnNames;
        }
    }
}