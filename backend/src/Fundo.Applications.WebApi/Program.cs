using DotNetEnv;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System;

namespace Fundo.Applications.WebApi
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                Env.TraversePath().Load();
                CreateWebHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unhandled WebApi exception: {ex.Message}");
            }
            finally
            {
                Console.WriteLine("Application shutting down.");
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
        }
    }
}
