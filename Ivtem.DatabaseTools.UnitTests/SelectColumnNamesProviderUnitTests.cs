using Ivtem.TSqlParsing.Feature;
using Ivtem.TSqlParsing.Feature.SelectQuery;
using Ivtem.TSqlParsing.Feature.SqlFragment;

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
    private static string SelectCountStatement => @"
SELECT COUNT(Field1) AS Count FROM SomeTable GROUP BY Field1;
";

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
        new InputResultData(SelectFieldsAsStatement, new [] { "NewName1", "Field2", "NewName3" })
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
    public void ItParsesCorrectly(int index)
    {
        var testData = TestData[index];
        var sql = testData.SqlQuery;
        var expectedColumnNames = testData.ColumnNames;

        var isParsed = FragmentProvider.TryGetSqlFragment(sql, out var sqlFragment, out _);

        Assert.That(isParsed, Is.True);

        var sut = new SelectColumnNamesProvider();

        var actualColumnNames = sut.GetColumnNames(sqlFragment!);

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