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

namespace Wing.Server.Modules.SoaServicesManager
{
    internal class CreateSingletonInstanceStrategy : ISoaServiceHostBuilderStrategy
    {
        #region ISoaServiceHostBuilderStrategy Members

        public void Execute(SoaServiceDescriptor descriptor, ref ServiceHost serviceHost, ref object singletonInstance, ref Uri serviceDefaultAddress)
        {
            if (descriptor.IsSingletonInstance && singletonInstance == null)
            {
                var type = Type.GetType(descriptor.ServiceTypeName);
                singletonInstance = Activator.CreateInstance(type);
            }
        }

        #endregion
    }
}
