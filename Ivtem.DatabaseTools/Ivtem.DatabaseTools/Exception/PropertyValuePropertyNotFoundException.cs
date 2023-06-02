namespace Ivtem.DatabaseTools.Exception;

public sealed class PropertyValuePropertyNotFoundException : System.Exception
{
    public PropertyValuePropertyNotFoundException(string propertyName) : base($"Property {propertyName} NOT found!")
    {

    }
}