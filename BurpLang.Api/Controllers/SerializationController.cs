using System;

using BurpLang.Common.Entities;

using Microsoft.AspNetCore.Mvc;

namespace BurpLang.Api.Controllers
{
    [ApiController]
    [Route("")]
    public class SerializationController : ControllerBase
    {
        [HttpPost("deserialize")]
        public IActionResult Deserialize([FromBody] string body)
        {
            try
            {
                var deserialized = Converter.Deserialize<Entity>(body);

                return Ok(deserialized);
            }
            catch (FormatException formatException)
            {
                return Ok(formatException.Message);
            }
        }
    }
}