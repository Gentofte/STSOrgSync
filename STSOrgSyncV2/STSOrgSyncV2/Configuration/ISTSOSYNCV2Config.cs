namespace STSOrgSyncV2
{
    // ================================================================================
    public interface ISTSOSYNCV2Config
    {
        // -----------------------------------------------------------------------------
        string ADX_STSOSYNCV2_IncomingEventsQueueRootNameUNFILTERED { get; }

        // -----------------------------------------------------------------------------
        string ADX_STSOSYNCV2_IncomingEventsQueueRootNameFILTERED { get; }

        // -----------------------------------------------------------------------------
        bool LogTrace_ADX_Incoming_FILTERED_Events { get; set; }

        // -----------------------------------------------------------------------------
        bool LogTrace_ADX_EventsFilteredAway { get; set; }

        // -----------------------------------------------------------------------------
        bool NOPLoopMode { get; set; }
    }
}
