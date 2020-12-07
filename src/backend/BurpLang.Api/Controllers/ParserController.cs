using BurpLang.Api.Models;
using BurpLang.Common.Entities;
using BurpLang.Exceptions;

using Microsoft.AspNetCore.Mvc;

using Serilog;

namespace BurpLang.Api.Controllers
{
    [ApiController]
    [Route("")]
    public class ParserController : ControllerBase
    {
        [HttpPost("parse")]
        public IActionResult Parse([FromBody] string data)
        {
            var response = new ParseResponse();

            try
            {
                var deserializer = new Parser<Entity>(data);

                response.Entity = deserializer.GetObject();
                Log.Information("Entity parse: SUCCESS.");
            }
            catch (ParsingException exception)
            {
                response.Error = exception;
                Log.Information("Entity parse: ERROR.");
            }

            return Ok(response);
        }
    }
}