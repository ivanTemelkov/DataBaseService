namespace Ivtem.TSqlParsing.Exceptions;

public sealed class PropertyValueKeyNotFoundException : Exception
{
    public PropertyValueKeyNotFoundException(string key) : base($"Key {key} NOT found!")
    {

    }
}


public sealed class PropertyValueKeyIsNullException : Exception
{
    public PropertyValueKeyIsNullException(string propertyName) : base($"A record with null key value found! Key Property Name: {propertyName}")
    {
        
    }
}