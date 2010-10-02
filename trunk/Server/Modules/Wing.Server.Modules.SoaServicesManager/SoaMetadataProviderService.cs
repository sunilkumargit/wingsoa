using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wing.Soa.Interop;
using Wing.Server.Soa;
using Wing.ServiceLocation;
using System.ServiceModel;

namespace Wing.Server.Modules.SoaServicesManager
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class SoaMetadataProviderService : ISoaMetadataProviderService
    {

        private ISoaServicesManager ServicesManager
        {
            get { return ServiceLocator.Current.GetInstance<ISoaServicesManager>(); }
        }

        public List<SoaServiceDescriptor> GetRegisteredServices()
        {
            return ServicesManager.Services.Select(s => s.Descriptor).ToList();
        }

        public SoaServiceHostInfo GetServiceState(string serviceName)
        {
            var service = ServicesManager.GetService(serviceName);
            return service.GetStateInfo();
        }

        public List<SoaServiceHostInfo> GetServicesStates()
        {
            return ServicesManager.Services.Select(s => s.GetStateInfo()).ToList();
        }

        public SoaServiceDescriptor GetServiceDescriptorByContractRefTypeName(string refTypeName)
        {
            return ServicesManager.Services
                .Where(s => s.Descriptor.ContractTypeRefName == refTypeName)
                .Select(s => s.Descriptor)
                .FirstOrDefault();
        }

        public SoaServiceConnectionInfo GetServiceConnectionInfoByContractRefTypeName(string refTypeName)
        {
            return ServicesManager.Services
                .Where(s => s.Descriptor.ContractTypeRefName == refTypeName)
                .Select(s => s.GetConnectionInfo())
                .FirstOrDefault();
        }

        public SoaServiceConnectionInfo GetConnectionInfo(string serviceName)
        {
            return ServicesManager.Services
                .Where(s => s.Descriptor.ServiceName == serviceName)
                .Select(s => s.GetConnectionInfo())
                .FirstOrDefault();
        }
    }
}
