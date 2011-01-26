using System;
using System.Collections.ObjectModel;
using Wing.Soa.Interop;

namespace Wing.Server.Soa
{
    public interface ISoaServiceHostBuilder
    {
        ISoaServiceHost BuildServiceHost(SoaServiceDescriptor descriptor, Object singletonInstance);
        ObservableCollection<ISoaServiceHostBuilderStrategy> Strategies { get; }
    }
}