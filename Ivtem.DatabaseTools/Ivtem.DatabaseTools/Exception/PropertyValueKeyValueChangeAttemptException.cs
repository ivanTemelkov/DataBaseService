namespace Ivtem.DatabaseTools.Exception;

public sealed class PropertyValueKeyValueChangeAttemptException : System.Exception
{
    public PropertyValueKeyValueChangeAttemptException(string key) : base($"Key property {key} is read-only!")
    {

    }
}