using GK.AppCore.Abstractions;
using GK.AppCore.Configuration;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace STSOrgSyncV2.Configuration
{
    // ================================================================================
    public static class DotNetCoreExtensions
    {
        // -----------------------------------------------------------------------------
        public static IIoCConfig AddAppStuff(this IServiceCollection services)
        {
            IoCConfig.Instance.ConfigureIoCStuff(services);

            services.AddGKAppCoreStuff();

            return IoCConfig.Instance;
        }

        // -----------------------------------------------------------------------------
        public static IApplicationBuilder UseAppStuff(this IApplicationBuilder app)
        {
            var appLifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();
            var serviceControl = app.ApplicationServices.GetRequiredService<IApplicationControl>();

            appLifetime.ApplicationStopping.Register(() => { serviceControl.Stop(); });
            appLifetime.ApplicationStarted.Register(() => { serviceControl.Start(); });

            app.UseGKAppCoreStuff();

            serviceControl.Init();

            return app;
        }
    }
}
