using System;

namespace BurpLang.Exceptions
{
    public class RootObjectNotFoundException : Exception
    {
        public RootObjectNotFoundException(string input) :
            base($"Root object was not found in:{Environment.NewLine}{input}") { }
    }
}