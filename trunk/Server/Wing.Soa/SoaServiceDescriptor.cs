using System;
using System.ServiceModel;
using System.Collections;
using System.Collections.Generic;
using Wing.Utils;

namespace Wing.Soa
{
    public class SoaServiceDescriptor
    {
        public SoaServiceDescriptor(String serviceName, Type contractType, Type serviceType)
        {
            Assert.NullArgument(contractType, "contractType");
            Assert.NullArgument(serviceType, "serviceType");
            ResolveServiceName(ref serviceName, contractType);
            ServiceName = serviceName;
            ContractType = contractType;
            ServiceType = serviceType;
        }

        public SoaServiceDescriptor(Type contractType, Type serviceType)
            : this("", contractType, serviceType) { }

        public SoaServiceDescriptor(String serviceName, Type contractType, Object singletonInstance)
        {
            Assert.NullArgument(contractType, "contractType");
            Assert.NullArgument(singletonInstance, "singletonInstance");
            ResolveServiceName(ref serviceName, contractType);
            ServiceName = serviceName;
            ContractType = contractType;
            SingletonInstance = singletonInstance;
        }

        public SoaServiceDescriptor(Type contractType, Object singletonInstance)
            : this("", contractType, singletonInstance) { }

        private static void ResolveServiceName(ref string serviceName, Type contractType)
        {
            if (serviceName == null)
                serviceName = contractType.Name;
        }

        public string ServiceName { get; private set; }
        public Type ContractType { get; private set; }
        public Object SingletonInstance { get; private set; }
        public Type ServiceType { get; private set; }

        public bool IsSingletonInstance { get { return SingletonInstance != null; } }
    }
}
