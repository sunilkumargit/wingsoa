using System;
using System.ServiceModel;
using System.Collections;
using System.Collections.Generic;
using Wing.Utils;

namespace Wing.Soa
{
    public interface ISoaServiceHostBuilder
    {
        ServiceHost BuildServiceHost(Type contractType, Type serviceType);
        ServiceHost BuildServiceHost(Type contractType, Object singletonInstance);
        IEnumerable<ISoaServiceHostBuildPolicy> Policies { get; }
        void AddPolicy(ISoaServiceHostBuildPolicy policy);
        void RemovePolicy(ISoaServiceHostBuildPolicy policy);
    }
}
