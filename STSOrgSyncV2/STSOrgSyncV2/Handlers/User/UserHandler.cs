using GK.AD;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace STSOrgSyncV2
{
    // ================================================================================
    public class UserHandler : IUserHandler
    {
        readonly IServiceProvider _serviceProvider;
        readonly ILogger _logger;
        readonly ISTSOSYNCV2Config _config;

        readonly IADObjectFactory _adObjectFactory;

        // -----------------------------------------------------------------------------
        public UserHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _logger = _serviceProvider.GetService<ILogger<UserHandler>>();
            _config = _serviceProvider.GetService<ISTSOSYNCV2Config>();

            _adObjectFactory = _serviceProvider.GetService<IADObjectFactory>();
        }

        // -----------------------------------------------------------------------------
        public async Task HandleEvent(ADX.DTO.ADEvent adEvent, CancellationToken cancellationToken)
        {
            await DoUserStuff(adEvent, cancellationToken);
        }

        // -----------------------------------------------------------------------------
        async Task DoUserStuff(ADX.DTO.ADEvent adEvent, CancellationToken cancellationToken)
        {
            try
            {
                var userDTO = adEvent.GetADObject() as ADX.DTO.User;

                if (userDTO != null)
                {
                    var serverName = adEvent.Sender.ServerName;

                    var user = _adObjectFactory.TryGetUser(userDTO.objectGuid, serverName);
                    if (user != null)
                    {
                        // Do something with user ...
                        //_logger.LogTrace($"User handler => [{user.ToString()}], title => [{user.title}], primary dept => [{user.department}]");
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            await Task.Yield();
        }
    }
}