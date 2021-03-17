using ADX.CTRL;
using ADX.DTO;

using GK.AppCore.Abstractions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace STSOrgSyncV2
{
    // ================================================================================
    public class ADXEventHandler : IADXEventHandler
    {
        readonly IServiceProvider _serviceProvider;

        readonly ILogger _logger;
        readonly ISTSOSYNCV2Config _config;

        readonly IOUHandler _eventHandlerOU;
        readonly IGroupHandler _eventHandlerGroup;
        readonly IUserHandler _eventHandlerUser;

        // -----------------------------------------------------------------------------
        public ADXEventHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            _logger = _serviceProvider.GetService<ILogger<ADXEventHandler>>();
            _config = _serviceProvider.GetService<ISTSOSYNCV2Config>();

            _eventHandlerOU = _serviceProvider.GetService<IOUHandler>();
            _eventHandlerGroup = _serviceProvider.GetService<IGroupHandler>();
            _eventHandlerUser = _serviceProvider.GetService<IUserHandler>();
        }

        // -----------------------------------------------------------------------------
        public async Task<MessageResult> HandleMessageAsync(byte[] messageBody, CancellationToken cancellationToken)
        {
            //string methodTag = $"{GetType().Name}.{nameof(HandleMessageAsync)}()";

            cancellationToken.ThrowIfCancellationRequested();

            var adEvent = ADX.DTO.Serializer.FromByteArrayToADEvent(messageBody);

            //LogTrace($"Inside FILTERED handler => ({methodTag}), ev => [{adEvent.ToString()}]");
            LogTrace($"Inside FILTERED handler, ev => {adEvent.ToString2()}");

            try
            {
                if (!_config.NOPLoopMode)
                {
                    switch (adEvent.EventClass)
                    {
                        case EventClass.user:
                            await _eventHandlerUser.HandleEvent(adEvent, cancellationToken);
                            break;

                        case EventClass.group:
                            await _eventHandlerGroup.HandleEvent(adEvent, cancellationToken);
                            break;

                        case EventClass.organizationalUnit:
                            await _eventHandlerOU.HandleEvent(adEvent, cancellationToken);
                            break;

                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"HANDLING EVENT for OBJ => [{adEvent.GetObjectIDs()}] FAILED! Ex => [{ex.Message}]. EVENT IGNORED!");
            }

            await Task.Yield();

            return new MessageResult { StatusCode = StatusCode.OK, StatusMessage = "" };
        }

        // -----------------------------------------------------------------------------
        void LogTrace(string msg)
        {
            if (_config.LogTrace_ADX_Incoming_FILTERED_Events)
            {
                _logger.LogTrace(msg);
            }
        }
    }
}
