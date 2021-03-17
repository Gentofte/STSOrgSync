using ADX.CTRL;
using ADX.DTO;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using System;

namespace STSOrgSyncV2
{
    // ================================================================================
    public class ADXEventFilter : IUnfilteredEventFilter
    {
        readonly IObjectFilter _specialFilter;
        readonly IObjectFilter _ugoFilter;
        readonly IInCacheFilter _cacheFilter;

        readonly ISTSOSYNCV2Config _config;
        readonly ILogger _logger;

        // -----------------------------------------------------------------------------
        public ADXEventFilter(IServiceProvider serviceProvider)
        {
            _specialFilter = serviceProvider.GetService<SpecialObjectsFilter>();
            _ugoFilter = serviceProvider.GetService<UserGroupOUFilter>();
            _cacheFilter = serviceProvider.GetService<IInCacheFilter>();

            _config = serviceProvider.GetService<ISTSOSYNCV2Config>();
            _logger = serviceProvider.GetService<ILogger<ADXEventFilter>>();
        }

        // -----------------------------------------------------------------------------
        public IADObject FilterObject(IADObject adObj)
        {
            if (adObj == null) return null;

            // Just return object, iff no filtering is wanted ...
            //return adObj;

            var logCopy = adObj;

            adObj = _specialFilter.FilterObject(adObj);
            if (adObj == null) { WriteLogFilteredAway(logCopy, nameof(SpecialObjectsFilter)); return null; }

            adObj = _ugoFilter.FilterObject(adObj);
            if (adObj == null) { WriteLogFilteredAway(logCopy, nameof(UserGroupOUFilter)); return null; }

            adObj = _cacheFilter.FilterObject(adObj);
            if (adObj == null) { WriteLogFilteredAway(logCopy, nameof(IInCacheFilter)); return null; }

            return adObj;
        }

        // -----------------------------------------------------------------------------
        void WriteLogFilteredAway(IADObject adObj, string filterType)
        {
            if (_config.LogTrace_ADX_EventsFilteredAway)
            {
                _logger.LogTrace($"Object is FILTERED OUT => {adObj.ToString()} by filtertype => [{filterType}]");
            }
        }
    }
}
