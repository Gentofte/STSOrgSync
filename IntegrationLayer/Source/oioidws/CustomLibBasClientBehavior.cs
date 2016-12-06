using System;
using System.IdentityModel.Tokens;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Security;
using System.Xml;
using Digst.OioIdws.Common.Constants;
using Digst.OioIdws.LibBas.StrCustomization;
using Digst.OioIdws.LibBas.Behaviors;

namespace Organisation.IntegrationLayer
{
    // This class is a copy of LibBasClientBehaviour from the OIOIDWS framework, which has
    // been updated to use the CustomLibBasMessageInspector class instead of the LibBasMessageInspector class
    // found in the OIOIDWS framework... this is needed to change the implementation of the MessageInspector,
    // and nothing else.
    internal class CustomLibBasClientBehavior : IEndpointBehavior
    {
        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
            /* TODO: only there when using tokens, and SP does not use tokens until 1/2-17
            var requirements = bindingParameters.Find<ChannelProtectionRequirements>();
            requirements.IncomingSignatureParts.AddParts(MessagePartSpecificationWsc());
            requirements.OutgoingSignatureParts.AddParts(LibBasServiceBehavior.MessagePartSpecificationWsp());
            */

            var clientCredentials = bindingParameters.Find<ClientCredentials>();
            clientCredentials.UseIdentityConfiguration = true;

            var securityTokenHandlerCollectionManager = clientCredentials.SecurityTokenHandlerCollectionManager[SecurityTokenHandlerCollectionManager.Usage.Default];
            securityTokenHandlerCollectionManager.AddOrReplace(new StrReferenceSaml2SecurityTokenHandler());
        }

        public static MessagePartSpecification MessagePartSpecificationWsc()
        {
            var libertyFrameworkQualifiedName = new XmlQualifiedName(LibBas.HeaderName, LibBas.HeaderNameSpace);
            var wsAddressingMessageIdQualifiedName = new XmlQualifiedName(WsAdressing.WsAdressingMessageId, WsAdressing.WsAdressing10NameSpace);
            var wsAddressingActionQualifiedName = new XmlQualifiedName(WsAdressing.WsAdressingAction, WsAdressing.WsAdressing10NameSpace);
            var wsAddressingRelatesToQualifiedName = new XmlQualifiedName(WsAdressing.WsAdressingRelatesTo, WsAdressing.WsAdressing10NameSpace);
            var wsAddressingToQualifiedName = new XmlQualifiedName(WsAdressing.WsAdressingTo, WsAdressing.WsAdressing10NameSpace); // This one is optional according to [LIB-BAS]

            var part = new MessagePartSpecification(libertyFrameworkQualifiedName, wsAddressingMessageIdQualifiedName, wsAddressingActionQualifiedName, wsAddressingRelatesToQualifiedName, wsAddressingToQualifiedName);
            part.IsBodyIncluded = true;

            return part;
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.MessageInspectors.Add(new CustomLibBasMessageInspector());
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            throw new NotImplementedException();
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }
    }
}