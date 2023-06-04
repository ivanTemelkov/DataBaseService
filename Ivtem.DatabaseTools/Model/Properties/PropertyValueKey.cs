namespace Ivtem.DatabaseTools.Model.Properties;

public sealed record PropertyValueKey : PropertyValue
{
    public PropertyValueKey(string name, string value, string? caption = null, Type? type = null) : base(name, caption, type, value)
    {
        ArgumentException.ThrowIfNullOrEmpty(value);
        Value = value;
    }

    public PropertyValueKey(string keyValue, PropertyValueSchema schema) : this(schema.KeyPropertyName, keyValue, schema.KeyPropertyCaption, schema.KeyPropertyType)
    {
    }
}