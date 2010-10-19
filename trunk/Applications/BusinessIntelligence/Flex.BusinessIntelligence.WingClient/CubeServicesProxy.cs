﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Flex.BusinessIntelligence.Data;
using Wing.ServiceLocation;
using Wing.Client.Sdk;
using Wing.Soa.Interop.Client;
using Flex.BusinessIntelligence.Interop.Services;
using Wing.Utils;
using Wing.Soa.Interop;

namespace Flex.BusinessIntelligence.WingClient
{
    public class CubeServicesProxy : ICubeServicesProxy
    {
        private ObservableCollection<CubeRegistrationInfo> _cubes;
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
                _cubes.Clear();
                _cubes.AddRange(cubes);
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
    }
}