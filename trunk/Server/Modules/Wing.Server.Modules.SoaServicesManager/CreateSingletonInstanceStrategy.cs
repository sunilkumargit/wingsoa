using System;
using System.ServiceModel;
using Wing.Server.Soa;
using Wing.Soa.Interop;

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
