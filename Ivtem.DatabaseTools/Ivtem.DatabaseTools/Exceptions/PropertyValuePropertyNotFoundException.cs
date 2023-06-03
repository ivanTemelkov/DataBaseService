namespace Ivtem.DatabaseTools.Exceptions;

public sealed class PropertyValuePropertyNotFoundException : Exception
{
    public PropertyValuePropertyNotFoundException(string propertyName) : base($"Property {propertyName} NOT found!")
    {

    }
}