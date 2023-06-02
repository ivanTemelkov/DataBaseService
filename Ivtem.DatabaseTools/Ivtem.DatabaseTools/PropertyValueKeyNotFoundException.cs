namespace Ivtem.DatabaseTools;

public sealed class PropertyValueKeyNotFoundException : Exception
{
    public PropertyValueKeyNotFoundException(string key) : base($"Key {key} NOT found!")
    {

    }
}

public sealed class PropertyValueMissingKeyValueException : Exception
{
    public PropertyValueMissingKeyValueException(string key) : base($"Value of Key property {key} NOT found!")
    {

    }
}

public sealed class PropertyValueKeyValueChangeAttemptException : Exception
{
    public PropertyValueKeyValueChangeAttemptException(string key) : base($"Key property {key} is read-only!")
    {

    }
}