using System.Collections;
using System.Collections.Concurrent;

namespace Ivtem.DatabaseTools;

public class PropertyValueList : IEnumerable<PropertyValueRow>
{
    /// <summary>
    /// Key = KeyPropertyValue
    /// Value = PropertyValueRow
    /// </summary>
    private ConcurrentDictionary<string, PropertyValueRow> PropertyValueRows { get; } = new();

    public PropertyValue Key { get; }

    public string KeyPropertyName => Key.Name;

    public string KeyPropertyCaption => Key.Caption ?? Key.Name;

    public Type KeyPropertyType => Key.Type;

    /// <summary>
    /// Dictionary of propertyName, propertyCaption
    /// </summary>
    public Dictionary<string, string> PropertyCaptions { get; } = new();

    public Dictionary<string, Type> PropertyTypes { get; } = new();

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

    public PropertyValueList(PropertyValue key, IEnumerable<(string propertyName, string? propertyCaption, Type? propertyType)> properties)
    {
        ArgumentException.ThrowIfNullOrEmpty(key.Name, $"{nameof(key)}.{nameof(key.Name)}");
        ArgumentException.ThrowIfNullOrEmpty(key.Value, $"{nameof(key)}.{nameof(key.Value)}");

        Key = key;

        foreach (var (propertyName, propertyCaption, propertyType) in properties)
        {
            PropertyCaptions[propertyName] = propertyCaption ?? propertyName;
            PropertyTypes[propertyName] = propertyType ?? typeof(string);
        }

        PropertyCaptions[KeyPropertyName] = KeyPropertyCaption;
        PropertyTypes[KeyPropertyName] = KeyPropertyType;
    }

    public void AddOrUpdate(string keyValue, IEnumerable<(string propertyName, string? propertyValue)> properties)
    {
        ArgumentException.ThrowIfNullOrEmpty(keyValue, nameof(keyValue));

        var key = new PropertyValue
        {
            Name = KeyPropertyName,
            Caption = KeyPropertyCaption,
            Type = KeyPropertyType,
            Value = keyValue
        };

        var propertyValueRow = PropertyValueRow.CreateRow(key, PropertyCaptions, PropertyTypes, properties);

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