namespace Ivtem.DatabaseTools.Model.Properties;

public record PropertyValue
{
    public required string Name { get; init; }

    public Type Type { get; init; } = typeof(string);

    public string? Caption { get; init; }

    public string? Value { get; init; }
}