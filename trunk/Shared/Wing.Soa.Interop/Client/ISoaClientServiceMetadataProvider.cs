using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Wing.Soa.Interop.Client
{
    public interface ISoaClientServiceMetadataProvider
    {
        SoaServiceDescriptor GetServiceDescriptorByContractType(Type contractType);
        SoaServiceConnectionInfo GetServiceHostInfo(String serviceName);
    }
}
