using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wing.Soa.Interop;
using System.Collections.ObjectModel;
using System.ServiceModel;

namespace Wing.Server.Soa
{
    public interface ISoaServiceHostBuilderStrategy
    {
        void Execute(SoaServiceDescriptor descriptor, ref ServiceHost serviceHost, ref Object singletonInstance, ref Uri serviceDefaultUri);
    }
}
