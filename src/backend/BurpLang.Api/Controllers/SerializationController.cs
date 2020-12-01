using System;

using BurpLang.Common.Entities;
using BurpLang.Exceptions;

using Microsoft.AspNetCore.Mvc;

namespace BurpLang.Api.Controllers
{
    [ApiController]
    [Route("")]
    public class SerializationController : ControllerBase
    {
        [HttpPost("deserialize")]
        public object Deserialize([FromBody] string body)
        {
            try
            {
                var deserialized = Converter.Deserialize<Entity>(body);

                return deserialized;
            }
            catch (FormatException formatException)
            {
                return new { formatException.Message };
            }
            catch (RootObjectNotFoundException rootObjectNotFoundException)
            {
                return new { rootObjectNotFoundException.Message };
            }
        }
    }
}