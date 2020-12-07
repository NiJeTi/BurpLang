using System;

using BurpLang.Common.Entities;

namespace BurpLang.Api.Models
{
    [Serializable]
    public class ParseResponse
    {
        public Entity? Entity { get; set; }

        public ParsingError? Error { get; set; }
    }
}