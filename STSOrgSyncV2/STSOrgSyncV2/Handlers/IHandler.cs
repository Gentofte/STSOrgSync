using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace STSOrgSyncV2
{
    // ================================================================================
    public interface IHandler
    {
        // -----------------------------------------------------------------------------
        Task HandleEvent(ADX.DTO.ADEvent adEvent, CancellationToken cancellationToken);
    }
}
