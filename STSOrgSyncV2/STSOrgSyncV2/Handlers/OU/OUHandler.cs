using GK.AD;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace STSOrgSyncV2
{
    // ================================================================================
    public class OUHandler : IOUHandler
    {
        readonly IServiceProvider _serviceProvider;
        readonly ILogger _logger;
        readonly ISTSOSYNCV2Config _config;

        readonly IADObjectFactory _adObjectFactory;

        // -----------------------------------------------------------------------------
        public OUHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _logger = _serviceProvider.GetService<ILogger<OUHandler>>();
            _config = _serviceProvider.GetService<ISTSOSYNCV2Config>();

            _adObjectFactory = _serviceProvider.GetService<IADObjectFactory>();
        }

        // -----------------------------------------------------------------------------
        public async Task HandleEvent(ADX.DTO.ADEvent adEvent, CancellationToken cancellationToken)
        {
            await DoOUStuff(adEvent, cancellationToken);
        }

        // -----------------------------------------------------------------------------
        async Task DoOUStuff(ADX.DTO.ADEvent adEvent, CancellationToken cancellationToken)
        {
            var ouDTO = adEvent.GetADObject() as ADX.DTO.OU;
            if (ouDTO != null)
            {
                var serverName = adEvent.Sender.ServerName;

                var ou = _adObjectFactory.TryGetOU(ouDTO.objectGuid, serverName);
                if (ou != null)
                {
                    // Do something with OU ...
                    //_logger.LogTrace($"OU handler => [{ou.ToString()}]");
                }
            }

            await Task.Yield();
        }
    }
}
