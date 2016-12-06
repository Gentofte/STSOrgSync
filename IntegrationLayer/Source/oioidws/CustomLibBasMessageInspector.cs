using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using Digst.OioIdws.Common;
using Digst.OioIdws.Common.Constants;

namespace Organisation.IntegrationLayer
{
    // slightly modified version of the officiel OIOIDWS class, it just does not enforce validation of headers
    // this is required as the KMD services does not follow the specs, and send the correct headers
    internal class CustomLibBasMessageInspector : IClientMessageInspector, IDispatchMessageInspector
    {
        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            return null;
        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {

        }

        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            return null;
        }

        public void AfterReceiveReply(ref Message reply, object correlationState)
        {

        }
    }
}
