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
        [HttpPost("parse")]
        public IActionResult Parse([FromBody] string data)
        {
            var response = new ParseResponse();

            try
            {
                var deserializer = new Parser<Entity>(data);

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