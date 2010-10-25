using System;
using System.Collections.ObjectModel;
using Flex.BusinessIntelligence.Data;
using Wing.Soa.Interop;

namespace Flex.BusinessIntelligence.Interop.Services
{
    public interface ICubeServicesProxy
    {
        ObservableCollection<CubeRegistrationInfo> Cubes { get; }
        ObservableCollection<CubeQueryInfo> Queries { get; }
        void RefreshCubesInfo();
        void RefreshQueriesInfo();
        OperationResult SaveCube(CubeRegistrationInfo cube, Action<OperationResult> callback);
        OperationResult DeleteCube(CubeRegistrationInfo cube, Action<OperationResult> callback);
        OperationResult SaveQuery(CubeQueryInfo query, Action<OperationResult> callback);
        OperationResult DeleteQuery(CubeQueryInfo query, Action<OperationResult> callback);
    }
}
