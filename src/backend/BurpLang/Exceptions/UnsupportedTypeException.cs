using System;

namespace BurpLang.Exceptions
{
    public class UnsupportedTypeException : Exception
    {
        public UnsupportedTypeException(Type type) : base($"Found unsupported property of type \"{type}\"") { }
    }
}