using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wing.Server.Soa;
using Wing.Soa.Interop;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.Web;
using System.ServiceModel.Description;
using System.ServiceModel.Channels;
using Wing.ServiceLocation;
using Wing.Server.Core;

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
