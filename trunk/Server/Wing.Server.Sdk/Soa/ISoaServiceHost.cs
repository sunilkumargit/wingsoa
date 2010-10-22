using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using Wing.Soa.Interop;

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
        SoaServiceConnectionInfo GetConnectionInfo();
    }
}
