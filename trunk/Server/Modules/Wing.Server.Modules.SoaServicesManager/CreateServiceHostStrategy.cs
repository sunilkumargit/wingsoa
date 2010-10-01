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
using Wing.Server.Core;
using Wing.ServiceLocation;

namespace Wing.Server.Modules.SoaServicesManager
{
    internal class CreateServiceHostStrategy : ISoaServiceHostBuilderStrategy
    {
        #region ISoaServiceHostBuilderStrategy Members

        public void Execute(SoaServiceDescriptor descriptor, ref ServiceHost serviceHost, ref object singletonInstance, ref Uri serviceDefaultAddress)
        {
            var settings = ServiceLocator.Current.GetInstance<BootstrapSettings>();
            serviceDefaultAddress = new Uri(serviceDefaultAddress.ToString() + "/" + descriptor.ServiceName);
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
