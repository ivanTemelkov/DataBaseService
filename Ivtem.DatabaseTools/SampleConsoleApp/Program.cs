using System.Data.Common;
using System.Data.SqlClient;
using Ivtem.DatabaseTools;
using Ivtem.DatabaseTools.Feature.DatabaseService;

var connectionString =
    @"Initial Catalog=operaSacet;Data Source=LM-NBK-44\DEV19;User ID=operasa;Password=*****;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";


Console.WriteLine("Initializing the SqlDatabaseService ...");
var dataBaseService = await SqlDatabaseService.Create(connectionString);

Console.WriteLine("Done.");
Console.WriteLine($"The Compatibility Level is {dataBaseService.CompatibilityLevel}");

var sql = @"
SELECT ma_codice AS [MachineId], ma_desmac AS [MachineDescription] FROM FMacchine -- WHERE ma_codice='MAC_001' ORDER BY ma_codice;
";

Console.WriteLine($"sql: {sql}");

Console.WriteLine($"Testing for errors ...");

if (dataBaseService.TryValidateSqlQuery(sql, out var parseErrors) == false)
{
    foreach (var parseError in parseErrors)
    {
        Console.WriteLine(parseError.Message);
    }

    return;
}

Console.WriteLine("The query is valid.\n");


await using var connection = new SqlConnection(connectionString);

await connection.OpenAsync();

var command = new SqlCommand(sql, connection);

await using var sqlReader = await command.ExecuteReaderAsync();

while (await sqlReader.ReadAsync())
{
    foreach (DbDataRecord dataRecord in sqlReader)
    {
        for (int i = 0; i < dataRecord.FieldCount; i++)
        {
            Console.WriteLine(dataRecord.GetName(i));
        }
        
    }
}