namespace Ivtem.DatabaseTools.Exception;

public sealed class PropertyValueKeyNotFoundException : System.Exception
{
    public PropertyValueKeyNotFoundException(string key) : base($"Key {key} NOT found!")
    {

    }
}


public sealed class PropertyValueKeyIsNullException : System.Exception
{
    public PropertyValueKeyIsNullException(string propertyName) : base($"A record with null key value found! Key Property Name: {propertyName}")
    {
        
    }
}