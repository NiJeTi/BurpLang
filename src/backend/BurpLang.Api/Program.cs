using System;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

using Serilog;

namespace BurpLang.Api
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                CreateHost(args).Run();
            }
            catch (Exception exception)
            {
                Log.Fatal(exception, "Application terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static IHost CreateHost(string[] args) =>
            Host
               .CreateDefaultBuilder(args)
               .UseSerilog()
               .ConfigureWebHostDefaults(builder => builder.UseStartup<Startup>())
               .Build();
    }
}