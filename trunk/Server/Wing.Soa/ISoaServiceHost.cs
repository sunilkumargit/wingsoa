using System;
using System.ServiceModel;
using System.Collections;
using System.Collections.Generic;
using Wing.Utils;

namespace Wing.Soa
{
    public interface ISoaServiceHost
    {
        SoaServiceDescriptor Descriptor { get; }
        ServiceHost Host { get; }
    }

    internal class SoaServiceHost : ISoaServiceHost
    {
        public SoaServiceHost(SoaServiceDescriptor descriptor, ServiceHost host)
        {
            host.Open();
        }

        #region ISoaServiceHost Members

        public SoaServiceDescriptor Descriptor
        {
            get { throw new NotImplementedException(); }
        }

        public ServiceHost Host
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }

    /*
var address = new Uri(serviceAddress, serviceName);
var host = new ServiceHost(typeof(TService));
host.Description.Behaviors.Add(new ServiceMetadataBehavior());
host.AddServiceEndpoint(typeof(TContract), new WSHttpBinding(), address);
BindingElement bindingElement = new HttpTransportBindingElement();
CustomBinding binding = new CustomBinding(bindingElement);
host.AddServiceEndpoint(typeof(IMetadataExchange), binding, new Uri(address, "MEX"));
host.Open();
 */

}
