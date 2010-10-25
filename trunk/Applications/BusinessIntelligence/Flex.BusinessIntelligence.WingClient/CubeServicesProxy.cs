using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Flex.BusinessIntelligence.Data;
using Flex.BusinessIntelligence.Interop.Services;
using Wing.Client.Sdk;
using Wing.Soa.Interop;
using Wing.Soa.Interop.Client;
using Wing.Utils;

namespace Flex.BusinessIntelligence.WingClient
{
    public class CubeServicesProxy : ICubeServicesProxy
    {
        private ObservableCollection<CubeRegistrationInfo> _cubes;
        private ObservableCollection<CubeQueryInfo> _queries;
        private IShellService _shellService;

        public CubeServicesProxy(IShellService shellService)
        {
            _shellService = shellService;
        }

        public ObservableCollection<CubeRegistrationInfo> Cubes
        {
            get
            {
                if (_cubes == null)
                    LoadCubesFromServer();
                return _cubes;
            }
        }

        public ObservableCollection<CubeQueryInfo> Queries
        {
            get
            {
                if (_queries == null)
                    LoadQueriesFromServer();
                return _queries;
            }
        }

        public void RefreshCubesInfo()
        {
            LoadCubesFromServer();
        }

        private void LoadCubesFromServer()
        {
            if (_cubes == null)
                _cubes = new ObservableCollection<CubeRegistrationInfo>();
            _shellService.DisplayWorkingStatus("Consultando o servidor...");
            //carregar os cubos do servidor.
            SoaClientManager.InvokeService<ICubeInfoProviderService>((channel, broker) =>
            {
                var cubes = broker.CallSync<List<CubeRegistrationInfo>>(channel.BeginGetCubesInfo, channel.EndGetCubesInfo);
                VisualContext.Async(() =>
                {
                    _cubes.Clear();
                    _cubes.AddRange(cubes);
                });
            });
            _shellService.HideWorkingStatus();
        }

        public OperationResult SaveCube(CubeRegistrationInfo cube, Action<OperationResult> callback)
        {
            _shellService.DisplayWorkingStatus("Gravando as informações do cubo...");
            OperationResult result = null;
            SoaClientManager.InvokeService<ICubeInfoProviderService>((channel, broker) =>
            {
                result = broker.CallSync<OperationResult, CubeRegistrationInfo>(channel.BeginSaveCubeInfo, channel.EndSaveCubeInfo, cube);
            });
            _shellService.HideWorkingStatus();
            if (result.Status == OperationStatus.Success)
                RefreshCubesInfo();
            if (callback != null)
                callback(result);
            return result;
        }

        public OperationResult DeleteCube(CubeRegistrationInfo cube, Action<OperationResult> callback)
        {
            _shellService.DisplayWorkingStatus("Excluindo as informações do cubo...");
            OperationResult result = null;
            SoaClientManager.InvokeService<ICubeInfoProviderService>((channel, broker) =>
            {
                result = broker.CallSync<OperationResult, Guid>(channel.BeginDeleteCubeInfo, channel.EndDeleteCubeInfo, cube.CubeId);
            });
            _shellService.HideWorkingStatus();
            if (result.Status == OperationStatus.Success)
                RefreshCubesInfo();
            if (callback != null)
                callback(result);
            return result;
        }

        private void LoadQueriesFromServer()
        {
            if (_queries == null)
                _queries = new ObservableCollection<CubeQueryInfo>();
            _shellService.DisplayWorkingStatus("Consultando o servidor...");
            SoaClientManager.InvokeService<ICubeInfoProviderService>((channel, broker) =>
            {
                var queries = broker.CallSync<List<CubeQueryInfo>>(channel.BeginGetQueriesInfo, channel.EndGetQueriesInfo);
                VisualContext.Async(() =>
                {
                    _queries.Clear();
                    _queries.AddRange(queries);
                });
            });
            _shellService.HideWorkingStatus();
        }


        public void RefreshQueriesInfo()
        {
            LoadQueriesFromServer();
        }

        public OperationResult SaveQuery(CubeQueryInfo query, Action<OperationResult> callback)
        {
            _shellService.DisplayWorkingStatus("Excluindo as informações da consulta...");
            OperationResult result = null;
            SoaClientManager.InvokeService<ICubeInfoProviderService>((channel, broker) =>
            {
                result = broker.CallSync<OperationResult, CubeQueryInfo>(channel.BeginSaveQueryInfo, channel.EndSaveQueryInfo, query);
            });
            _shellService.HideWorkingStatus();
            if (result.Status == OperationStatus.Success)
                RefreshQueriesInfo();
            if (callback != null)
                callback(result);
            return result;
        }

        public OperationResult DeleteQuery(CubeQueryInfo query, Action<OperationResult> callback)
        {
            _shellService.DisplayWorkingStatus("Excluindo as informações do cubo...");
            OperationResult result = null;
            SoaClientManager.InvokeService<ICubeInfoProviderService>((channel, broker) =>
            {
                result = broker.CallSync<OperationResult, Guid>(channel.BeginDeleteQueryInfo, channel.EndDeleteQueryInfo, query.CubeId);
            });
            _shellService.HideWorkingStatus();
            if (result.Status == OperationStatus.Success)
                RefreshQueriesInfo();
            if (callback != null)
                callback(result);
            return result;
        }
    }
}
