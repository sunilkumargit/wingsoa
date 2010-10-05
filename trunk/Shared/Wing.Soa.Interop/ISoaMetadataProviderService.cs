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
    [ServiceKnownType(typeof(SoaServiceConnectionInfo))]
    public interface ISoaMetadataProviderService
    {
#if !SILVERLIGHT
        [OperationContract]
        List<SoaServiceDescriptor> GetRegisteredServices();

        [OperationContract]
        SoaServiceHostInfo GetServiceState(String serviceName);

        [OperationContract]
        SoaServiceDescriptor GetServiceDescriptorByContractRefTypeName(String refTypeName);

        [OperationContract]
        SoaServiceConnectionInfo GetConnectionInfo(String serviceName);

        [OperationContract]
        SoaServiceConnectionInfo GetServiceConnectionInfoByContractRefTypeName(String refTypeName);

        [OperationContract]
        List<SoaServiceHostInfo> GetServicesStates();
#else
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetRegisteredServices(AsyncCallback callback, Object state);
        void EndGetRegisteredServices(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetServiceState(String serviceName, AsyncCallback callback, Object state);
        SoaServiceHostInfo EndGetServiceState(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetServiceDescriptorByContractRefTypeName(String refTypeName, AsyncCallback callback, Object state);
        SoaServiceDescriptor EndGetServiceDescriptorByContractRefTypeName(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetServiceConnectionInfoByContractRefTypeName(String refTypeName, AsyncCallback callback, Object state);
        SoaServiceConnectionInfo EndGetServiceConnectionInfoByContractRefTypeName(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetConnectionInfo(String serviceName, AsyncCallback callback, Object state);
        SoaServiceConnectionInfo EndGetConnectionInfo(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetServicesStates(AsyncCallback callback, Object state);
        List<SoaServiceHostInfo> EndGetServicesStates(IAsyncResult result);
#endif
    }
}