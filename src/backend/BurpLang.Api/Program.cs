using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace BurpLang.Api
{
    internal static class Program
    {
        private static void Main(string[] args) => CreateHost(args).Run();

        private static IHost CreateHost(string[] args) =>
            Host
               .CreateDefaultBuilder(args)
               .ConfigureWebHostDefaults(builder => builder.UseStartup<Startup>())
               .Build();
    }
}