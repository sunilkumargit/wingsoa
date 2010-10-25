using System;
using System.Collections.Generic;
using System.ServiceModel;
using Flex.BusinessIntelligence.Data;
using Flex.BusinessIntelligence.Interop.Services;
using Wing.Server;
using Wing.ServiceLocation;
using Wing.Soa.Interop;

namespace Flex.BusinessIntelligence.Server.Core
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class CubeInfoProviderService : ICubeInfoProviderService
    {
        private IServerEntityStoreService _entityStore;

        public CubeInfoProviderService()
        {
            _entityStore = ServiceLocator.GetInstance<IServerEntityStoreService>();
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
            try
            {
                if (!_entityStore.Save(info))
                {
                    result.Status = OperationStatus.Error;
                    result.Message = "Não foi possível salvar as configurações do cubo";
                }
            }
            catch (Exception ex)
            {
                result.Status = OperationStatus.Error;
                result.Message = ex.ToString();
            }
            return result;
        }

        public OperationResult DeleteCubeInfo(Guid cubeId)
        {
            var info = GetCubeInfo(cubeId);
            var result = new OperationResult();
            try
            {
                if (info != null)
                    _entityStore.Remove(info);
                result.Message = "O cubo foi excluído";
            }
            catch (Exception ex)
            {
                result.Status = OperationStatus.Error;
                result.Message = ex.ToString();
            }
            return result;
        }

        public List<CubeQueryInfo> GetQueriesInfo()
        {
            var query = _entityStore.CreateQuery<CubeQueryInfo>();
            return query.Find();
        }


        public OperationResult SaveQueryInfo(CubeQueryInfo query)
        {
            var result = new OperationResult();
            try
            {
                if (!_entityStore.Save(query))
                {
                    result.Status = OperationStatus.Error;
                    result.Message = "Não foi possível salvar a consulta";
                }
            }
            catch (Exception ex)
            {
                result.Status = OperationStatus.Error;
                result.Message = ex.ToString();
            }
            return result;
        }

        public OperationResult DeleteQueryInfo(Guid queryId)
        {
            var query = _entityStore.CreateQuery<CubeQueryInfo>();
            query.AddFilterEqual("QueryId", queryId);
            var info = query.FindFirst();
            var result = new OperationResult();
            try
            {
                if (info != null)
                    _entityStore.Remove(info);
                result.Message = "A consulta foi excluída";
            }
            catch (Exception ex)
            {
                result.Status = OperationStatus.Error;
                result.Message = ex.ToString();
            }
            return result;
        }
    }
}
