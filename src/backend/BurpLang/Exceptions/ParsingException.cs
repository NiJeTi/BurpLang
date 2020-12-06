using System;

namespace BurpLang.Exceptions
{
    public class ParsingException : Exception
    {
        public ParsingException(string? message) : base(message) { }
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
    }
}