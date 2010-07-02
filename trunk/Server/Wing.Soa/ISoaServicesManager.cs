using System;
using System.ServiceModel;
using System.Collections;
using System.Collections.Generic;
using Wing.Utils;

namespace Wing.Soa
{
    public interface ISoaServicesManager
    {
        ISoaServiceHostBuilder Builder { get; }
        void RegisterService(SoaServiceDescriptor serviceDescriptor);
    }
}
