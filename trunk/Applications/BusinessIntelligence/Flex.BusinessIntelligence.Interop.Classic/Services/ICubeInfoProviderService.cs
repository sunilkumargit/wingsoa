using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Flex.BusinessIntelligence.Data;
using Wing.Soa.Interop;

namespace Flex.BusinessIntelligence.Interop.Services
{
    [ServiceContract]
    [ServiceKnownType(typeof(CubeRegistrationInfo))]
    [ServiceKnownType(typeof(CubeQueryGroup))]
    [ServiceKnownType(typeof(CubeQueryInfo))]
    public interface ICubeInfoProviderService
    {
        [OperationContract]
        List<CubeRegistrationInfo> GetCubesInfo();

        [OperationContract]
        CubeRegistrationInfo GetCubeInfo(Guid cubeId);

        [OperationContract]
        OperationResult SaveCubeInfo(CubeRegistrationInfo info);

        [OperationContract]
        OperationResult DeleteCubeInfo(Guid cubeId);
    }
}
