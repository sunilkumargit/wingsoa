using System;
using System.Collections.ObjectModel;
using Wing.Soa.Interop;

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