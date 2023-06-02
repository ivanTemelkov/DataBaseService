namespace Ivtem.DatabaseTools.Exception;

public sealed class PropertyValueKeyNotFoundException : System.Exception
{
    public PropertyValueKeyNotFoundException(string key) : base($"Key {key} NOT found!")
    {

    }
}