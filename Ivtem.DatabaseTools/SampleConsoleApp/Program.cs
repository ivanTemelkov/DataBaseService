using Ivtem.DatabaseTools;

var connectionString =
    @"Initial Catalog=operaSacet;Data Source=LM-NBK-44\DEV19;User ID=operasa;Password=*****;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";


Console.WriteLine("Initializing the SqlDataBaseService ...");
var dataBaseService = await SqlDataBaseService.Create(connectionString);

Console.WriteLine("Done.");
Console.WriteLine($"The Compatibility Level is {dataBaseService.CompatibilityLevel}");

var sql = @"
SELECT ma_codice AS [MachineId], ma_desmac AS [MachineDescription] FROM FMACCHINE WHERE ma_codice='MAC_001' ORDER BY ma_codice;
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

Console.WriteLine("The query is valid.");

