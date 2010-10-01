using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wing.Soa.Interop;
using System.Collections.ObjectModel;

namespace Wing.Server.Soa
{
    public interface ISoaServicesManager
    {
        ISoaServiceHost RegisterService(SoaServiceDescriptor descriptor, bool autoStart = true, Object singletonInstance = null);
        ISoaServiceHost GetService(String serviceName);
        ISoaServiceHost RemoveService(String serviceName);
        ReadOnlyObservableCollection<ISoaServiceHost> Services { get; }
        ISoaServiceHostBuilder HostBuilder { get; }
    }
}