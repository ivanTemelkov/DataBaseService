using Ivtem.DatabaseTools.Feature.DatabaseService;
using Ivtem.TSqlParsing.Feature;
using Ivtem.TSqlParsing.Feature.CompatibilityLevel;
using Ivtem.TSqlParsing.Feature.SelectQuery;
using Ivtem.TSqlParsing.Feature.SqlFragment;
using Ivtem.TSqlParsing.Feature.SqlGenerator;
using Ivtem.TSqlParsing.Model.Properties;
using Microsoft.SqlServer.TransactSql.ScriptDom;

var connectionString =
    @"Initial Catalog=operaSacet;Data Source=LM-NBK-44\DEV19;User ID=operasa;Password=*****;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";


Console.WriteLine("Initializing the SqlDatabaseService ...");
var dataBaseService = await SqlDatabaseService.Create(connectionString);

Console.WriteLine("Done.");
Console.WriteLine($"The Compatibility Level is {dataBaseService.CompatibilityLevel}");

//var sql = @"
//SELECT ma_codice AS [MachineId], ma_desmac AS [MachineDescription] FROM FMacchine -- WHERE ma_codice='MAC_001' ORDER BY ma_codice;
//";

var sql = @"
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


sql = @"
IF @ma_codice = 0
BEGIN
    SELECT [ma_codice] AS [MachineId],
	       [fa_id] AS [PhaseId]
    FROM [Piazzamenti] (NOLOCK)
    WHERE [ma_codice] IN (@@machineIds@@)
END;
";

sql = @"

/*
**	Initial Comment Block
**
**
**
*/

INSERT INTO [Piazzamenti] (id)
VALUES (1);


/*
**	Middle Comment Block
**
**
**
*/

SELECT [ma_codice] AS [MachineId],
	   [fa_id] AS [PhaseId]
FROM [Piazzamenti] (NOLOCK)
WHERE [ma_codice] IN (@@machineIds@@);


/*
**	End Comment Block
**
**
**
*/
";


Console.WriteLine($"sql: {sql}");

Console.WriteLine($"Testing for errors ...");



var sqlFragmentProvider = new TSqlFragmentProvider();

var sqlParserGeneratorProvider = new SqlFragmentAndGeneratorProvider(new SqlCompatibilityLevelProvider(connectionString), sqlFragmentProvider, new SqlGeneratorFactory());

TSqlFragment? sqlFragment = default;

try
{
    sqlFragment = sqlParserGeneratorProvider.GetSqlFragment(sql);
}
catch (Exception e)
{
    Console.WriteLine(e);
}

if (sqlFragment is null)
{
    return;
}


var blockCommentProvider = new BlockCommentProvider();

if (blockCommentProvider.TryGetBlockComments(sqlFragment, out var commentBlocks))
{
    foreach (var commentBlock in commentBlocks)
    {
        Console.WriteLine(commentBlock);
    }
}


Console.WriteLine("The query is valid.\n");

var selectQueryProvider = new SelectStatementProvider();

if (selectQueryProvider.TryGetStatement(sqlFragment, out var selectStatement) == false)
{
    Console.WriteLine("SELECT statement NOT found!");
    return;
}

var sqlScriptGenerator = sqlParserGeneratorProvider.GetSqlGenerator();

var script = sqlScriptGenerator.Generate(selectStatement);

Console.WriteLine(script);
return;


var tableNamesProvider = new TableNamesProvider();
var tableNames = tableNamesProvider.GetTableNames(selectStatement);

foreach (var tableName in tableNames)
{
    Console.WriteLine(tableName);
}

return;


var columnNamesProvider = new SelectColumnNamesProvider();

var columnNames = columnNamesProvider.GetColumnNames(sqlFragment);

Console.WriteLine("Column Names:");
foreach (var columnName in columnNames)
{
    Console.WriteLine(columnName);
}

return;

var key = new PropertyValue("MachineId", "Machine Id");

var properties = new []
{
    new PropertyValue("MachineDescription", "Machine Description", typeof(string))
};


var schema = new PropertyValueSchema(key, properties);

var queryExecutor = new QueryExecutor(dataBaseService);

var propertyValueList = await queryExecutor.Execute(sql, schema);

foreach (var row in propertyValueList)
{
    foreach (var propertyValue in row)
    {
        Console.WriteLine($"{propertyValue.Name}({propertyValue.Caption}) = {propertyValue.Value}");
    }
}