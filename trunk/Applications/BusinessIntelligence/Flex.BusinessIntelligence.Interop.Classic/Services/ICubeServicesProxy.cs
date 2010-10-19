using System;
using System.Collections.ObjectModel;
using Flex.BusinessIntelligence.Data;
using Wing.Soa.Interop;

namespace Flex.BusinessIntelligence.Interop.Services
{
    public interface ICubeServicesProxy
    {
        ObservableCollection<CubeRegistrationInfo> Cubes { get; }
        void RefreshCubesInfo();
        OperationResult SaveCube(CubeRegistrationInfo cube, Action<OperationResult> callback);
        OperationResult DeleteCube(CubeRegistrationInfo cube, Action<OperationResult> callback);
    }
}
