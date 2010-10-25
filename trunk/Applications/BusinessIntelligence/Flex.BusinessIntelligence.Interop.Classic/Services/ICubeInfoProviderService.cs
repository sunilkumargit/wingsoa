using System;
using System.Collections.Generic;
using System.ServiceModel;
using Flex.BusinessIntelligence.Data;
using Wing.Soa.Interop;

namespace Flex.BusinessIntelligence.Interop.Services
{
    [ServiceContract]
    [ServiceKnownType(typeof(CubeRegistrationInfo))]
    [ServiceKnownType(typeof(CubeQueryGroup))]
    [ServiceKnownType(typeof(CubeQueryInfo))]
    [ServiceKnownType(typeof(OperationResult))]
    public interface ICubeInfoProviderService
    {
#if !SILVERLIGHT
        [OperationContract]
        List<CubeRegistrationInfo> GetCubesInfo();

        [OperationContract]
        CubeRegistrationInfo GetCubeInfo(Guid cubeId);

        [OperationContract]
        OperationResult SaveCubeInfo(CubeRegistrationInfo info);

        [OperationContract]
        OperationResult DeleteCubeInfo(Guid cubeId);

        [OperationContract]
        List<CubeQueryInfo> GetQueriesInfo();

        [OperationContract]
        OperationResult SaveQueryInfo(CubeQueryInfo query);

        [OperationContract]
        OperationResult DeleteQueryInfo(Guid queryId);
#else
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetCubesInfo(AsyncCallback callback, Object state);
        List<CubeRegistrationInfo> EndGetCubesInfo(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetCubeInfo(Guid cubeId, AsyncCallback callback, Object state);
        CubeRegistrationInfo EndGetCubeInfo(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSaveCubeInfo(CubeRegistrationInfo info, AsyncCallback callback, Object state);
        OperationResult EndSaveCubeInfo(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteCubeInfo(Guid cubeId, AsyncCallback callback, Object state);
        OperationResult EndDeleteCubeInfo(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetQueriesInfo(AsyncCallback callback, Object state);
        List<CubeQueryInfo> EndGetQueriesInfo(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSaveQueryInfo(CubeQueryInfo query, AsyncCallback callback, Object state);
        OperationResult EndSaveQueryInfo(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteQueryInfo(Guid queryId, AsyncCallback callback, Object state);
        OperationResult EndDeleteQueryInfo(IAsyncResult result);
#endif

    }
}
