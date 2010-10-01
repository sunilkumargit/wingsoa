using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace Wing.Soa.Interop
{
    [ServiceContract]
    [ServiceKnownType(typeof(SoaServiceDescriptor))]
    [ServiceKnownType(typeof(SoaServiceState))]
    [ServiceKnownType(typeof(SoaServiceHostInfo))]
    public interface ISoaMetadataProviderService
    {
        [OperationContract]
        List<SoaServiceDescriptor> GetRegisteredServices();

        [OperationContract]
        SoaServiceHostInfo GetServiceState(String serviceName);

        [OperationContract]
        List<SoaServiceHostInfo> GetServicesStates();
    }
}