using System.Collections;
using System.Collections.Concurrent;

namespace Ivtem.DatabaseTools;

public class PropertyValueRow : IEnumerable<PropertyValue>
{
    /// <summary>
    /// Key = propertyName
    /// Value = PropertyValue
    /// </summary>
    private ConcurrentDictionary<string, PropertyValue> PropertyValues { get; } = new();

    public PropertyValue Key { get; }

    public string KeyPropertyName => Key.Name;

    public string KeyPropertyValue => Key.Value ?? throw new PropertyValueMissingKeyValueException(KeyPropertyName);

    public string KeyPropertyCaption => Key.Caption ?? Key.Name;

    public Type KeyPropertyType => Key.Type;

    /// <summary>
    /// Dictionary of propertyName, propertyCaption
    /// </summary>
    public Dictionary<string, string> PropertyCaptions { get; private set; } = new();

    public Dictionary<string, Type> PropertyTypes { get; private set; } = new();

    public string? this[string propertyName]
    {
        get => GetValue(propertyName);
        set => SetValue(propertyName, value);
    }

    private PropertyValueRow(PropertyValue key)
    {
        Key = key;
    }

    public PropertyValueRow(PropertyValue key, IEnumerable<(string propertyName, string? propertyCaption, Type? propertyType)> rowSchema)
    {
        ArgumentException.ThrowIfNullOrEmpty(key.Name);
        ArgumentException.ThrowIfNullOrEmpty(key.Value);

        Key = key;

        PropertyValues[KeyPropertyName] = key;

        foreach (var (propertyName, propertyCaption, propertyType) in rowSchema)
        {
            PropertyCaptions[propertyName] = propertyCaption ?? propertyName;
            PropertyTypes[propertyName] = propertyType ?? typeof(string);
        }

        PropertyCaptions[KeyPropertyName] = KeyPropertyCaption;
        PropertyTypes[KeyPropertyName] = KeyPropertyType;
    }

    internal static PropertyValueRow CreateRow(PropertyValue key, Dictionary<string, string> propertyCaptions,
        Dictionary<string, Type> propertyTypes, IEnumerable<(string PropertyName, string? PropertyValue)> properties)
    {
        ArgumentException.ThrowIfNullOrEmpty(key.Name);
        ArgumentException.ThrowIfNullOrEmpty(key.Value);

        var schema = new PropertyValueRow(key)
        {
            PropertyCaptions = propertyCaptions,
            PropertyTypes = propertyTypes,
        };

        var propertyValues = properties
            .DistinctBy(x => x.PropertyName)
            .ToDictionary(x => x.PropertyName, x => x.PropertyValue);

        return CreateRow(schema, propertyValues);
    }

    public PropertyValueRow CreateRow(string keyValue, IEnumerable<(string PropertyName, string? PropertyValue)> properties)
    {
        ArgumentException.ThrowIfNullOrEmpty(keyValue);

        var propertyValues = properties
            .DistinctBy(x => x.PropertyName)
            .ToDictionary(x => x.PropertyName, x => x.PropertyValue);

        return CreateRow(keyValue, propertyValues);
    }

    public PropertyValueRow CreateRow(string keyValue, IReadOnlyDictionary<string, string?> properties)
    {
        ArgumentException.ThrowIfNullOrEmpty(keyValue);

        var key = Key with { Value = keyValue };

        var result = new PropertyValueRow(key)
        {
            PropertyCaptions = PropertyCaptions,
            PropertyTypes = PropertyTypes
        };

        return CreateRow(result, properties);
    }

    private static PropertyValueRow CreateRow(PropertyValueRow schema, IReadOnlyDictionary<string, string?> properties)
    {
        foreach (var propertyName in schema.PropertyCaptions.Keys)
        {
            if (propertyName.Equals(schema.KeyPropertyName, StringComparison.OrdinalIgnoreCase)) continue;

            var caption = schema.PropertyCaptions[propertyName];
            var type = schema.PropertyTypes[propertyName];

            if (properties.TryGetValue(propertyName, out var value) == false) continue;

            schema.PropertyValues[propertyName] = new PropertyValue
            {
                Name = propertyName,
                Caption = caption,
                Type = type,
                Value = value
            };
        }

        return schema;
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
            : new PropertyValue
            {
                Name = propertyName,
                Caption = caption,
                Type = type,
                Value = value
            };


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
                yield return new PropertyValue
                {
                    Name = propertyName,
                    Caption = PropertyCaptions[propertyName],
                    Value = null
                };
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}