using ADX.CTRL;

using GK.AppCore.Abstractions;

using Microsoft.Extensions.DependencyInjection;

using System;

namespace STSOrgSyncV2.Configuration
{
    // ================================================================================
    public sealed class IoCConfig : IIoCConfig
    {
        static readonly Lazy<IoCConfig> lazy = new Lazy<IoCConfig>(() => new IoCConfig());

        static readonly object _lock = new object();
        static bool _isConfigured = false;

        // -----------------------------------------------------------------------------
        public static IoCConfig Instance { get { return lazy.Value; } }

        // -----------------------------------------------------------------------------
        IoCConfig()
        {
        }

        // -----------------------------------------------------------------------------
        public void ConfigureIoCStuff(IServiceCollection services)
        {
            lock (_lock) { if (_isConfigured) return; _isConfigured = true; }

            // Register EXTERNAL services =>

            GK.AppCore.Configuration.IoCConfig.Instance.ConfigureIoCStuff(services);
            ADX.CTRL.Configuration.IoCConfig.Instance.ConfigureIoCStuff(services);

            // Register INTERNAL services =>

            services.AddSingleton<ISTSOSYNCV2Config, STSOSYNCV2Config>();

            // ServiceControl reregister (sort of override) to enable instance in this project ...
            services.AddSingleton<IApplicationControl, ApplicationControl>();

            // Add filter instance to filter OUT unwanted events from unfiltered queue before going into filtered queue
            // Note: This IOC registration overwrites default in ADX.CTRL assembly
            // NO override registration here means, that NULL filter is used in ADX CTRL. Normally this is what you want! 20210228/SDE
            //services.AddSingleton<IUnfilteredEventFilter, ADXEventFilter>();

            // Add handler to receive filtered events.
            // NOTE: This IOC registration overwrites default BUT EMPTY NOP handler in ADX.CTRL assembly
            services.AddSingleton<IADXEventHandler, ADXEventHandler>();

            // Register handlers for each type of AD objects
            services.AddTransient<IOUHandler, OUHandler>();
            services.AddTransient<IGroupHandler, GroupHandler>();
            services.AddTransient<IUserHandler, UserHandler>();
        }

        // -----------------------------------------------------------------------------
        public bool IsConfigured() => _isConfigured;
    }
}
