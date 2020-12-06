using BurpLang.Api.Models;
using BurpLang.Common.Entities;
using BurpLang.Exceptions;

using Microsoft.AspNetCore.Mvc;

namespace BurpLang.Api.Controllers
{
    [ApiController]
    [Route("")]
    public class ParsingController : ControllerBase
    {
        [HttpPost("deserialize")]
        public IActionResult Deserialize([FromBody] string data)
        {
            var response = new ParseResponse();
            var deserializer = new Parser<Entity>(data);

            try
            {
                response.Entity = deserializer.GetObject();
            }
            catch (ParsingException exception)
            {
                response.Error = exception;
            }

            return Ok(response);
        }
    }
}