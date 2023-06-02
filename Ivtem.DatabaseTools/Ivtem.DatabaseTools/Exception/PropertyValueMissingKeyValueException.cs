namespace Ivtem.DatabaseTools.Exception;

public sealed class PropertyValueMissingKeyValueException : System.Exception
{
    public PropertyValueMissingKeyValueException(string key) : base($"Value of Key property {key} NOT found!")
    {

    }
}