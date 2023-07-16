using System.Collections;
using System.Collections.Concurrent;
using Ivtem.TSqlParsing.Exceptions;

namespace Ivtem.TSqlParsing.Model.Properties;

public class PropertyValueRow : IEnumerable<PropertyValue>
{
    /// <summary>
    /// Key = propertyName
    /// Value = PropertyValue
    /// </summary>
    private ConcurrentDictionary<string, PropertyValue> PropertyValues { get; } = new();

    public PropertyValueKey Key { get; }

    public string KeyPropertyName => Key.Name;

    public string KeyPropertyValue => Key.Value!;

    public string KeyPropertyCaption => Key.Caption;

    public Type KeyPropertyType => Key.Type;

    private PropertyValueSchema Schema { get; }

    public Dictionary<string, string> PropertyCaptions => Schema.PropertyCaptions;

    public Dictionary<string, Type> PropertyTypes => Schema.PropertyTypes;

    public string? this[string propertyName]
    {
        get => GetValue(propertyName);
        set => SetValue(propertyName, value);
    }

    private PropertyValueRow(PropertyValueKey key, PropertyValueSchema schema)
    {
        Key = key;
        PropertyValues[KeyPropertyName] = key;
        Schema = schema;
    }

    public static PropertyValueRow CreateRow(string keyValue, PropertyValueSchema schema, IEnumerable<(string PropertyName, string? PropertyValue)> properties)
    {
        ArgumentException.ThrowIfNullOrEmpty(keyValue, nameof(keyValue));

        var key = new PropertyValueKey(keyValue, schema);

        var row = new PropertyValueRow(key, schema);

        var propertyValues = properties
            .DistinctBy(x => x.PropertyName)
            .ToDictionary(x => x.PropertyName, x => x.PropertyValue);

        return CreateRow(row, propertyValues);
    }

    public PropertyValueRow CreateRow(string keyValue, IEnumerable<(string PropertyName, string? PropertyValue)> properties)
    {
        var propertyValues = properties
            .DistinctBy(x => x.PropertyName)
            .ToDictionary(x => x.PropertyName, x => x.PropertyValue);

        return CreateRow(keyValue, propertyValues);
    }

    public PropertyValueRow CreateRow(string keyValue, IReadOnlyDictionary<string, string?> properties)
    {
        var key = new PropertyValueKey(Schema.KeyPropertyName, keyValue, Schema.KeyPropertyCaption,
            Schema.KeyPropertyType);

        var result = new PropertyValueRow(key, Schema);

        return CreateRow(result, properties);
    }

    private static PropertyValueRow CreateRow(PropertyValueRow emptyRow, IReadOnlyDictionary<string, string?> properties)
    {
        foreach (var propertyName in emptyRow.PropertyCaptions.Keys)
        {
            var caption = emptyRow.PropertyCaptions[propertyName];
            var type = emptyRow.PropertyTypes[propertyName];

            if (properties.TryGetValue(propertyName, out var value) == false)
            {
                // Value is not provided in the properties, try get it from the Key or leave it undefined
                if (propertyName.Equals(emptyRow.KeyPropertyName, StringComparison.OrdinalIgnoreCase) == false) continue;

                value = emptyRow.KeyPropertyValue;
            }

            emptyRow.PropertyValues[propertyName] = new PropertyValue(propertyName, caption, type, value);
        }

        return emptyRow;
    }

    public void AddOrUpdateRow(PropertyValueRow other)
    {
        foreach (var propertyName in PropertyCaptions.Keys)
        {
            if (propertyName.Equals(KeyPropertyName, StringComparison.InvariantCultureIgnoreCase)) continue;

            PropertyValues[propertyName] = PropertyValues[propertyName] with { Value = other[propertyName] };
        }
    }

    public string? GetValue(string propertyName)
    {
        ArgumentException.ThrowIfNullOrEmpty(propertyName, nameof(propertyName));

        if (PropertyCaptions.TryGetValue(propertyName, out _) == false)
            throw new PropertyValuePropertyNotFoundException(propertyName);

        return PropertyValues.TryGetValue(propertyName, out var propertyValue)
            ? propertyValue.Value
            : default;
    }

    public void SetValue(string propertyName, string? value)
    {
        ArgumentException.ThrowIfNullOrEmpty(propertyName, nameof(propertyName));

        if (PropertyCaptions.TryGetValue(propertyName, out var caption) == false)
            throw new PropertyValuePropertyNotFoundException(propertyName);

        if (PropertyTypes.TryGetValue(propertyName, out var type) == false)
            throw new PropertyValuePropertyNotFoundException(propertyName);

        if (propertyName.Equals(KeyPropertyName, StringComparison.OrdinalIgnoreCase))
            throw new PropertyValueKeyValueChangeAttemptException(KeyPropertyName);


        var newPropertyValue = PropertyValues.TryGetValue(propertyName, out var propertyValue)
            ? propertyValue with { Value = value }
            : new PropertyValue(propertyName, caption, type, value);


        PropertyValues[propertyName] = newPropertyValue;
    }

    public IEnumerator<PropertyValue> GetEnumerator()
    {
        foreach (var propertyName in PropertyCaptions.Keys)
        {
            if (PropertyValues.TryGetValue(propertyName, out var value))
            {
                yield return value;
            }
            else
            {
                yield return new PropertyValue(propertyName, PropertyCaptions[propertyName]);
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}