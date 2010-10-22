using System;
using System.ServiceModel;
using Wing.Soa.Interop;

namespace Wing.Server.Soa
{
    public interface ISoaServiceHostBuilderStrategy
    {
        void Execute(SoaServiceDescriptor descriptor, ref ServiceHost serviceHost, ref Object singletonInstance, ref Uri serviceDefaultUri);
    }
}
