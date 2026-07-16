using Fundo.Applications.WebApi.Configurations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Fundo.Applications.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration) { }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddPersistence();
            services.AddApplication();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (!env.IsEnvironment("Testing"))
            {
                app.RunDatabaseMigrations();
            }

            app.UseExceptionHandler();
            app.UseRouting();
            app.UseCors(ServiceRegistration.FrontendCorsPolicy);
            app.UseAuthorization();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
