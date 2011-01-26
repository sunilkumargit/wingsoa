using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using Wing.Server.Soa;
using Wing.Soa.Interop;

namespace Wing.Server.Modules.SoaServicesManager
{
    internal class CreateBasicHttpBindingStrategy : ISoaServiceHostBuilderStrategy
    {

        #region ISoaServiceHostBuilderStrategy Members

        public void Execute(SoaServiceDescriptor descriptor, ref ServiceHost serviceHost, ref object singletonInstance, ref Uri serviceDefaultAddress)
        {
            var contractType = Type.GetType(descriptor.ContractTypeName);

            var metadataBehavior = new ServiceMetadataBehavior();
            metadataBehavior.HttpGetEnabled = true;
            serviceHost.Description.Behaviors.Add(metadataBehavior);
            serviceHost.AddServiceEndpoint(contractType, new BasicHttpBinding(), "");
            BindingElement bindingElement = new HttpTransportBindingElement();
            CustomBinding binding = new CustomBinding(bindingElement);
            serviceHost.AddServiceEndpoint(typeof(IMetadataExchange), binding, new Uri(serviceDefaultAddress.ToString() + "/MEX"));
        }

        #endregion
    }
}
