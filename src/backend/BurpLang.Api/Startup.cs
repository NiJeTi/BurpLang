using BurpLang.Api.Formatters;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Serilog;

namespace BurpLang.Api
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services
               .AddControllers(options => options.InputFormatters.Add(new TextPlainFormatter()))
               .AddNewtonsoftJson();
        }

        public void Configure(IApplicationBuilder application, IWebHostEnvironment environment)
        {
            application.UseRouting();

            if (environment.IsDevelopment())
                application.UseCors(builder =>
                {
                    builder.AllowAnyOrigin();
                    builder.AllowAnyHeader();
                    builder.AllowAnyMethod();
                });

            application.UseEndpoints(endpoints => endpoints.MapControllers());
            application.UseSerilogRequestLogging();
        }
    }
}