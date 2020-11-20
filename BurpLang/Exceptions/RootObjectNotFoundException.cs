using System;

namespace BurpLang.Exceptions
{
    public class RootObjectNotFoundException : Exception
    {
        public string Input { get; }
        
        public RootObjectNotFoundException(string input) :
            base($"Root object was not found in:{Environment.NewLine}{input}")
        {
            Input = input;
        }
    }
}