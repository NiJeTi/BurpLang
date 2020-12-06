using System;

using BurpLang.Common.Entities;
using BurpLang.Exceptions;

namespace BurpLang.Api.Models
{
    [Serializable]
    public class ParseResponse
    {
        public Entity? Entity { get; set; }

        public ParsingException? Error { get; set; }
    }
}