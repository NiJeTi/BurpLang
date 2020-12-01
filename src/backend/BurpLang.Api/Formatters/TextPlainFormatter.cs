using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc.Formatters;

namespace BurpLang.Api.Formatters
{
    public class TextPlainFormatter : TextInputFormatter
    {
        public TextPlainFormatter()
        {
            SupportedMediaTypes.Add("text/plain");
        }

        public override async Task<InputFormatterResult> ReadAsync(InputFormatterContext context)
        {
            using var streamReader = new StreamReader(context.HttpContext.Request.Body);
            var data = await streamReader.ReadToEndAsync();

            return await InputFormatterResult.SuccessAsync(data);
        }

        public override Task<InputFormatterResult> ReadRequestBodyAsync(
            InputFormatterContext context, Encoding encoding) =>
            ReadAsync(context);

        protected override bool CanReadType(Type type) => type == typeof(string);
    }
}