namespace Ivtem.DatabaseTools.Model.Properties;

public record PropertyValue
{
    public string Name { get; }

    public Type Type { get; }

    public string Caption { get; }

    public string? Value { get; init; }

    public PropertyValue(string name, string? caption = null, Type? type = null, string? value = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);

        Name = name;
        Caption = caption ?? name;
        Type = type ?? typeof(string);
        Value = value;
    }
}