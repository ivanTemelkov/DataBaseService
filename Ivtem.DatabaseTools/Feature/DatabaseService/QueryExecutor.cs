using System.Data.Common;
using System.Data.SqlClient;
using Ivtem.TSqlParsing.Exceptions;
using Ivtem.TSqlParsing.Model.Properties;

namespace Ivtem.DatabaseTools.Feature.DatabaseService;

public class QueryExecutor
{
    private IDatabaseService DatabaseService { get; }

    private string ConnectionString => DatabaseService.ConnectionString;

    public QueryExecutor(IDatabaseService databaseService)
    {
        DatabaseService = databaseService;
    }


    public async Task<PropertyValueList> Execute(string query, PropertyValueSchema schema)
    {
        await using var connection = new SqlConnection(ConnectionString);

        await connection.OpenAsync();

        var command = new SqlCommand(query, connection);

        await using var sqlReader = await command.ExecuteReaderAsync();

        var keyName = schema.KeyPropertyName;
        var result = new PropertyValueList(schema);

        while (await sqlReader.ReadAsync())
        {
            foreach (DbDataRecord dataRecord in sqlReader)
            {
                var fields = new List<(string PropertyName, string? PropertyValue)>();

                string? keyValue = default;
                for (int i = 0; i < dataRecord.FieldCount; i++)
                {
                    var name = dataRecord.GetName(i);
                    var value = dataRecord.IsDBNull(i) ? null : dataRecord.GetValue(i).ToString();

                    if (name.Equals(keyName, StringComparison.OrdinalIgnoreCase))
                    {
                        keyValue = value ?? throw new PropertyValueKeyIsNullException(name);
                        continue;
                    }

                    // var dataTypeName = dataRecord.GetDataTypeName(i);
            
                    fields.Add((name, value));
                }

                if (string.IsNullOrWhiteSpace(keyValue)) throw new PropertyValueKeyNotFoundException(keyName);

                result.AddOrUpdate(keyValue, fields);
            }
        }

        return result;
    }
}
