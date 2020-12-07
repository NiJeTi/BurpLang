using System;

using BurpLang.Exceptions;

namespace BurpLang.Api.Models
{
    [Serializable]
    public class ParsingError
    {
        public ParsingError(string message)
        {
            Message = message;
        }

        public string Message { get; set; }

        public int StartIndex { get; set; }
        public int EndIndex { get; set; }

        public static implicit operator ParsingError(ParsingException exception) =>
            new ParsingError(exception.Message)
            {
                Message = exception.Message,
                StartIndex = exception.StartIndex,
                EndIndex = exception.EndIndex
            };
    }
}