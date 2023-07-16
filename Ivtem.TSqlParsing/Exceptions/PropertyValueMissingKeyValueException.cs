namespace Ivtem.TSqlParsing.Exceptions;

public sealed class PropertyValueMissingKeyValueException : Exception
{
    public PropertyValueMissingKeyValueException(string key) : base($"Value of Key property {key} NOT found!")
    {

    }
}