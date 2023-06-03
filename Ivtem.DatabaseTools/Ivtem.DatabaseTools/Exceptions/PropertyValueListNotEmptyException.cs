using Ivtem.DatabaseTools.Model.Properties;

namespace Ivtem.DatabaseTools.Exceptions;

public sealed class PropertyValueListNotEmptyException : Exception
{
    public PropertyValueListNotEmptyException() : base($"Please provide an empty {nameof(PropertyValueList)}")
    {
        
    }
}