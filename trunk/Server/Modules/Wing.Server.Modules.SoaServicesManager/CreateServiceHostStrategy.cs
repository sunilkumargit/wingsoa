using System;
using System.ServiceModel;
using Wing.Server.Core;
using Wing.Server.Soa;
using Wing.ServiceLocation;
using Wing.Soa.Interop;

namespace Wing.Server.Modules.SoaServicesManager
{
    internal class CreateServiceHostStrategy : ISoaServiceHostBuilderStrategy
    {
        #region ISoaServiceHostBuilderStrategy Members

        public void Execute(SoaServiceDescriptor descriptor, ref ServiceHost serviceHost, ref object singletonInstance, ref Uri serviceDefaultAddress)
        {
            var settings = ServiceLocator.GetInstance<BootstrapSettings>();
            serviceDefaultAddress = new Uri(serviceDefaultAddress, descriptor.ServiceName);
            if (descriptor.IsSingletonInstance)
            {
                serviceHost = new ServiceHost(singletonInstance, serviceDefaultAddress);
            }
            else
                serviceHost = new ServiceHost(Type.GetType(descriptor.ServiceTypeName), serviceDefaultAddress);
        }

        #endregion
    }
}
