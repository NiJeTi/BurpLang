using System;

namespace BurpLang.Exceptions
{
    public class PropertyParsingException : Exception
    {
        public PropertyParsingException(string propertyName, string expectedPattern, string actualValue) :
            base($"Error parsing property \"{propertyName}\". " +
                $"Expected pattern \"{expectedPattern}\" but was \"{actualValue}\"") { }
    }
}