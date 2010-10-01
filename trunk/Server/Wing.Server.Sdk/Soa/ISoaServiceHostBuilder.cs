using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wing.Soa.Interop;
using System.Collections.ObjectModel;
using System.ServiceModel;

namespace Wing.Server.Soa
{
    public interface ISoaServiceHostBuilder
    {
        ISoaServiceHost BuildServiceHost(SoaServiceDescriptor descriptor, Object singletonInstance);
        ObservableCollection<ISoaServiceHostBuilderStrategy> Strategies { get; }
    }
}