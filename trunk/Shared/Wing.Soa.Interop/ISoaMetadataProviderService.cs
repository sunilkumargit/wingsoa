using System;
using System.Collections.Generic;
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
        List<SoaServiceConnectionInfo> GetServicesConnectionInfo();

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
        IAsyncResult BeginGetServicesConnectionInfo(AsyncCallback callback, Object state);
        List<SoaServiceConnectionInfo> EndGetServicesConnectionInfo(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetServicesStates(AsyncCallback callback, Object state);
        List<SoaServiceHostInfo> EndGetServicesStates(IAsyncResult result);
#endif
    }
}