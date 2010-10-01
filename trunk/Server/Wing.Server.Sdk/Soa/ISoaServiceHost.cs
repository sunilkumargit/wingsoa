using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wing.Soa.Interop;
using System.ServiceModel;
using System.Collections.ObjectModel;

namespace Wing.Server.Soa
{
    public interface ISoaServiceHost
    {
        SoaServiceDescriptor Descriptor { get; }
        Uri ServiceAddress { get; }
        ServiceHost ServiceHost { get; }
        ReadOnlyObservableCollection<Exception> Exceptions { get; }
        bool HasErrors { get; }
        SoaServiceState State { get; }
        Object SingletonInstance { get; }
        void Stop();
        void Start();
        SoaServiceHostInfo GetStateInfo();
    }
}
