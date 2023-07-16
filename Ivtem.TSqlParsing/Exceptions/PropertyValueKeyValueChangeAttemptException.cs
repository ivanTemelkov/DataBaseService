namespace Ivtem.TSqlParsing.Exceptions;

public sealed class PropertyValueKeyValueChangeAttemptException : Exception
{
    public PropertyValueKeyValueChangeAttemptException(string key) : base($"Key property {key} is read-only!")
    {

    }
}