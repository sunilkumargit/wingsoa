using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using Wing.ServiceLocation;

namespace Wing.Soa
{
    public class SoaServicesProvider : ISoaServicesProvider
    {
        public Uri BaseAdress { get; set; }

        public SoaServicesProvider(String baseAdress)
        {
            BaseAdress = new Uri(baseAdress);
        }

        public void RegisterService<TContract, TService>(String serviceName, String baseAddress)
        {
            var serviceAddress = String.IsNullOrEmpty(baseAddress) ? BaseAdress : new Uri(baseAddress);
            serviceName = String.IsNullOrEmpty(serviceName) ? typeof(TContract).Name : serviceName;
            var address = new Uri(serviceAddress, serviceName);
            var host = new ServiceHost(typeof(TService));
            host.Description.Behaviors.Add(new ServiceMetadataBehavior());
            host.AddServiceEndpoint(typeof(TContract), new WSHttpBinding(), address);
            BindingElement bindingElement = new HttpTransportBindingElement();
            CustomBinding binding = new CustomBinding(bindingElement);
            host.AddServiceEndpoint(typeof(IMetadataExchange), binding, new Uri(address, "MEX"));
            host.Open();
        }

        public void RegisterService<TContract, TService>()
        {
            RegisterService<TContract, TService>(null, null);
        }

        #region ISoaServicesProvider Members


        SoaServiceHostInfo ISoaServicesProvider.RegisterService<TContract, TService>(string serviceName, string baseAddress)
        {
            throw new NotImplementedException();
        }

        SoaServiceHostInfo ISoaServicesProvider.RegisterService<TContract, TService>()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<SoaServiceHostInfo> GetRegisteredServices()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
