using System.Data.Common;
using System.Data.SqlClient;
using Ivtem.DatabaseTools.Exceptions;
using Ivtem.DatabaseTools.Model.Properties;

namespace Ivtem.DatabaseTools.Feature.DatabaseService;

public class QueryExecutor
{
    private IDatabaseService DatabaseService { get; }

    private string ConnectionString => DatabaseService.ConnectionString;

    public QueryExecutor(IDatabaseService databaseService)
    {
        DatabaseService = databaseService;
    }


    public async Task<PropertyValueList> Execute(string query, PropertyValueList schema)
    {
        if (schema.RowCount > 0) throw new PropertyValueListNotEmptyException();

        var key = schema.Key;

        await using var connection = new SqlConnection(ConnectionString);

        await connection.OpenAsync();

        var command = new SqlCommand(query, connection);

        await using var sqlReader = await command.ExecuteReaderAsync();

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

                    if (name.Equals(key.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        keyValue = value ?? throw new PropertyValueKeyIsNullException(name);
                        continue;
                    }

                    // var dataTypeName = dataRecord.GetDataTypeName(i);
            
                    fields.Add((name, value));
                }

                if (string.IsNullOrWhiteSpace(keyValue)) throw new PropertyValueKeyNotFoundException(key.Name);

                schema.AddOrUpdate(keyValue, fields);
            }
        }

        return schema;
    }
}
