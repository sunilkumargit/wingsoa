using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Flex.BusinessIntelligence.Interop.Services;
using Wing.Server;
using Flex.BusinessIntelligence.Data;
using Wing.Soa.Interop;
using Wing.ServiceLocation;
using System.ServiceModel;

namespace Flex.BusinessIntelligence.Server.Core
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class CubeInfoProviderService : ICubeInfoProviderService
    {
        private IServerEntityStoreService _entityStore;

        public CubeInfoProviderService()
        {
            _entityStore = ServiceLocator.Current.GetInstance<IServerEntityStoreService>();
            // ler os cubos, para testar
            var cubes = GetCubesInfo();
        }

        public List<CubeRegistrationInfo> GetCubesInfo()
        {
            var query = _entityStore.CreateQuery<CubeRegistrationInfo>();
            return query.Find();
        }

        public CubeRegistrationInfo GetCubeInfo(Guid cubeId)
        {
            var query = _entityStore.CreateQuery<CubeRegistrationInfo>();
            query.AddFilterEqual("CubeId", cubeId);
            return query.FindFirst();
        }

        public OperationResult SaveCubeInfo(CubeRegistrationInfo info)
        {
            var result = new OperationResult();
            if (!_entityStore.Save(info))
            {
                result.Status = OperationStatus.Error;
                result.Message = "Não foi possível salvar as configurações do cube";
            }
            return result;
        }

        public OperationResult DeleteCubeInfo(Guid cubeId)
        {
            var info = GetCubeInfo(cubeId);
            if (info != null)
                _entityStore.Remove(info);
            return new OperationResult() { Message = "O cubo foi excluído" };
        }
    }
}
