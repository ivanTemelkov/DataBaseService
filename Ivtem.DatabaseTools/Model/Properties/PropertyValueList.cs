using System.Collections;
using System.Collections.Concurrent;
using Ivtem.DatabaseTools.Exceptions;

namespace Ivtem.DatabaseTools.Model.Properties;

public class PropertyValueList : IEnumerable<PropertyValueRow>
{
    /// <summary>
    /// Key = KeyPropertyValue
    /// Value = PropertyValueRow
    /// </summary>
    private ConcurrentDictionary<string, PropertyValueRow> PropertyValueRows { get; } = new();

    public PropertyValueSchema Schema { get; }

    public string KeyPropertyName => Schema.KeyPropertyName;

    public string KeyPropertyCaption => Schema.KeyPropertyCaption;

    public Type KeyPropertyType => Schema.KeyPropertyType;

    public Dictionary<string, string> PropertyCaptions => Schema.PropertyCaptions;

    public Dictionary<string, Type> PropertyTypes => Schema.PropertyTypes;

    public long RowCount => PropertyValueRows.Count;

    public long PropertiesCount => PropertyCaptions.Count;

    public string? this[string key, string propertyName]
    {
        get => PropertyValueRows.TryGetValue(key, out var propertyValueRow)
            ? propertyValueRow[propertyName]
            : throw new PropertyValueKeyNotFoundException(key);

        set
        {
            if (PropertyValueRows.TryGetValue(key, out var propertyValueRow) == false)
                throw new PropertyValueKeyNotFoundException(key);
            propertyValueRow[propertyName] = value;
        }
    }

    public PropertyValueList(PropertyValueSchema schema)
    {
        Schema = schema;
    }

    public void AddOrUpdate(string keyValue, IEnumerable<(string propertyName, string? propertyValue)> properties)
    {
        ArgumentException.ThrowIfNullOrEmpty(keyValue, nameof(keyValue));
        
        var propertyValueRow = PropertyValueRow.CreateRow(keyValue, Schema, properties);

        PropertyValueRows[keyValue] = propertyValueRow;
    }

    public IEnumerator<PropertyValueRow> GetEnumerator()
    {
        return PropertyValueRows.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}