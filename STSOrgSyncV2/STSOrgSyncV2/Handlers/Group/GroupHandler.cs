using GK.AD;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace STSOrgSyncV2
{
    // ================================================================================
    public class GroupHandler : IGroupHandler
    {
        readonly IServiceProvider _serviceProvider;
        readonly ILogger _logger;
        readonly ISTSOSYNCV2Config _config;

        readonly IADObjectFactory _adObjectFactory;

        // -----------------------------------------------------------------------------
        public GroupHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _logger = _serviceProvider.GetService<ILogger<GroupHandler>>();
            _config = _serviceProvider.GetService<ISTSOSYNCV2Config>();

            _adObjectFactory = _serviceProvider.GetService<IADObjectFactory>();
        }

        // -----------------------------------------------------------------------------
        public async Task HandleEvent(ADX.DTO.ADEvent adEvent, CancellationToken cancellationToken)
        {
            await DoGroupStuff(adEvent, cancellationToken);
        }

        // -----------------------------------------------------------------------------
        async Task DoGroupStuff(ADX.DTO.ADEvent adEvent, CancellationToken cancellationToken)
        {
            var groupDTO = adEvent.GetADObject() as ADX.DTO.Group;
            if (groupDTO != null)
            {
                var serverName = adEvent.Sender.ServerName;

                var group = _adObjectFactory.TryGetGroup(groupDTO.objectGuid, serverName);

                if (group != null)
                {
                    // Do something with group ...
                    //_logger.LogTrace($"Group handler => [{group.ToString()}]");
                }
            }

            await Task.Yield();
        }
    }
}
