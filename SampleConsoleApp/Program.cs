using Ivtem.DatabaseTools.Feature.DatabaseService;
using Ivtem.DatabaseTools.Model.Properties;

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

var key = new PropertyValue
{
    Name = "MachineId",
    Caption = "Machine Id",
    Type = typeof(string),
    Value = null
};

IEnumerable<(string PropertyName, string PropertyCaption, Type PropertyType)> properties = new []
{
    (key.Name, key.Caption, key.Type),
    ("MachineDescription", "Machine Description", typeof(string)),
};

var propertyValueList = new PropertyValueList(key, properties);

var queryExecutor = new QueryExecutor(dataBaseService);

propertyValueList = await queryExecutor.Execute(sql, propertyValueList);

foreach (var row in propertyValueList)
{
    foreach (var propertyValue in row)
    {
        Console.WriteLine($"{propertyValue.Name}({propertyValue.Caption}) = {propertyValue.Value}");
    }
}