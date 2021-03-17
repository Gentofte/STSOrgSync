using ADX.CTRL.Configuration;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace STSOrgSyncV2
{
    // ================================================================================
    public class STSOSYNCV2Config : ISTSOSYNCV2Config
    {
        // -----------------------------------------------------------------------------
        public STSOSYNCV2Config(IConfiguration configuration, IADXCTRLConfig adxCtrlConfig)
        {
            ADX_STSOSYNCV2_IncomingEventsQueueRootNameUNFILTERED = adxCtrlConfig.IncomingEventsQueueRootNameUNFILTERED;
            ADX_STSOSYNCV2_IncomingEventsQueueRootNameFILTERED = adxCtrlConfig.IncomingEventsQueueRootNameFILTERED;

            LogTrace_ADX_Incoming_FILTERED_Events = configuration.GetValue<bool>(nameof(LogTrace_ADX_Incoming_FILTERED_Events), false);
            LogTrace_ADX_EventsFilteredAway = configuration.GetValue<bool>(nameof(LogTrace_ADX_EventsFilteredAway), false);
            NOPLoopMode = configuration.GetValue<bool>(nameof(NOPLoopMode), false);
        }

        // -----------------------------------------------------------------------------
        public string ADX_STSOSYNCV2_IncomingEventsQueueRootNameUNFILTERED { get; set; }

        // -----------------------------------------------------------------------------
        public string ADX_STSOSYNCV2_IncomingEventsQueueRootNameFILTERED { get; set; }

        // -----------------------------------------------------------------------------
        public bool LogTrace_ADX_Incoming_FILTERED_Events { get; set; } = false;

        // -----------------------------------------------------------------------------
        public bool LogTrace_ADX_EventsFilteredAway { get; set; } = false;

        // -----------------------------------------------------------------------------
        public bool NOPLoopMode { get; set; } = false;
    }
}
