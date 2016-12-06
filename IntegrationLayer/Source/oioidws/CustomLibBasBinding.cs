using System.ServiceModel.Channels;
using System.ServiceModel.Security.Tokens;
using Digst.OioIdws.LibBas.StrCustomization;
using Digst.OioIdws.LibBas.Bindings;
using System;

namespace Organisation.IntegrationLayer
{
    /* TODO: This is the old STS implementation that uses tokens
    internal class CustomLibBasBinding : LibBasBinding
    {
        public override BindingElementCollection CreateBindingElements()
        {
            BindingElementCollection result = base.CreateBindingElements();

            foreach (BindingElement element in result)
            {
                if (element is AsymmetricSecurityBindingElement)
                {
                    ((AsymmetricSecurityBindingElement)element).DefaultAlgorithmSuite = new CustomAlgorithmSuite();
                }
                else if (element is HttpTransportBindingElement)
                {
                    ((HttpTransportBindingElement)element).MaxReceivedMessageSize = Int32.MaxValue;
                }
            }

            return result;
        }
        */

    // this is the binding needed for AuthorityContext on SP
    internal class CustomLibBasBinding : CustomBinding
    {
        public override BindingElementCollection CreateBindingElements()
        {
            var transport = new HttpsTransportBindingElement();
            transport.RequireClientCertificate = true;

            var encoding = new TextMessageEncodingBindingElement();
            encoding.MessageVersion = MessageVersion.Soap11;

            var elements = new BindingElementCollection();
            elements.Add(encoding);
            elements.Add(transport);

            return elements;
        }
    }
}
