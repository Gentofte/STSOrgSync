using STSOrgSyncV2.Configuration;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.HttpSys;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace STSOrgSyncV2
{
    // ================================================================================
    public class Startup
    {
        IHostEnvironment Environment { get; }
        IConfiguration Configuration { get; }

        // -----------------------------------------------------------------------------
        public Startup(IHostEnvironment env, IConfiguration configuration)
        {
            Environment = env;
            Configuration = configuration;
        }

        // -----------------------------------------------------------------------------
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddAuthentication(HttpSysDefaults.AuthenticationScheme);
            services.AddAuthorization();

            AddCORS(services);

            // Register clients libs if any ...
            //services.Add_POSTADEventHandler_HttpClient();

            // Remeber this - it's GK machinery
            services.AddAppStuff();
        }

        // -----------------------------------------------------------------------------
        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();

            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors();

            // Remeber this - it's GK machinery
            app.UseAppStuff();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        // -----------------------------------------------------------------------------
        private void AddCORS(IServiceCollection services)
        {
            bool allowAllOrigins = Configuration.GetValue<string>("xxxCorsAllowAll", "false").ToLower() == "true";

            if (allowAllOrigins)
            {
                services.AddCors(options =>
                {
                    options.AddPolicy("xxxCorsPolicy",
                        builder => builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
                });
            }
            else
            {
                string[] acceptedOrigins = Configuration.GetSection("xxxCorsAllowedOrigins").Get<string[]>();

                if (acceptedOrigins != null)
                {
                    services.AddCors(options =>
                    {
                        options.AddPolicy(name: "xxxCorsPolicy",
                            builder =>
                                builder.WithOrigins(acceptedOrigins)
                                .AllowAnyMethod()
                                .AllowAnyHeader()
                                .AllowCredentials());
                    });
                }
            }
        }
    }
}
