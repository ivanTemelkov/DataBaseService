namespace Ivtem.TSqlParsing.Model.Properties;

public record PropertyValueSchema
{
    public string KeyPropertyName { get; }

    public string KeyPropertyCaption { get; }

    public Type KeyPropertyType { get; }

    public string? DefaultFormatting { get; init; }

    /// <summary>
    /// Dictionary of propertyName, propertyCaption
    /// </summary>
    public Dictionary<string, string> PropertyCaptions { get; } = new();

    public Dictionary<string, Type> PropertyTypes { get; } = new();

    public PropertyValueSchema(PropertyValue key, IEnumerable<PropertyValue> properties)
    {
        KeyPropertyName = key.Name;
        KeyPropertyCaption = key.Caption;
        KeyPropertyType = key.Type;

        PropertyCaptions[KeyPropertyName] = KeyPropertyCaption;
        PropertyTypes[KeyPropertyName] = KeyPropertyType;

        foreach (var propertyValue in properties)
        {
            var name = propertyValue.Name;
            PropertyCaptions[name] = propertyValue.Caption;
            PropertyTypes[name] = propertyValue.Type;
        }
    }
}