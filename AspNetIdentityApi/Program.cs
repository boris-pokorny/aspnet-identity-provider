using System;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AspNetIdentityApi {
    public class Program {
        public static void Main (string[] args) {

            var seed = args.Contains ("/seed");

            if (seed) {
                args = args.Except (new [] { "/seed" }).ToArray ();
            }

            var host = CreateHostBuilder (args).Build ();

            if (seed) {
                var config = host.Services.GetRequiredService<IConfiguration> ();
                var connectionString = config.GetConnectionString ("DefaultConnection");
                SeedData.EnsureSeedData (connectionString);
                return;
            }

            host.Run ();
        }

        public static IHostBuilder CreateHostBuilder (string[] args) =>
            Host.CreateDefaultBuilder (args)
            .ConfigureWebHostDefaults (webBuilder => {
                webBuilder.UseStartup<Startup> ();
            });
    }
}