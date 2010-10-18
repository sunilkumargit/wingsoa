using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Wing.Client.Sdk;
using Flex.BusinessIntelligence.Client.Interop;
using Wing.Client.Sdk.Controls;
using Wing.Soa.Interop.Client;
using Flex.BusinessIntelligence.Interop.Services;
using Flex.BusinessIntelligence.Data;
using System.Collections.Generic;
using Wing.Utils;
using System.Collections.ObjectModel;

namespace Flex.BusinessIntelligence.WingClient.Views.CubesConfig
{
    public class BICubesConfigPresenter : ViewPresenter<BICubesConfigPresentationModel>
    {
        public BICubesConfigPresenter(BICubesConfigPresentationModel presentationModel, BICubesConfigView view)
            : base(presentationModel, view, null)
        {
        }

        protected override void ActiveStateChanged()
        {
            base.ActiveStateChanged();
            if (IsActive)
            {
                //refresh nos cubos registrados
                WorkContext.Async(() =>
                {
                    SoaClientManager.InvokeService<ICubeInfoProviderService>((channel, broker) =>
                    {
                        var cubes = broker.CallSync<List<CubeRegistrationInfo>>(channel.BeginGetCubesInfo, channel.EndGetCubesInfo);
                        CollectionUtils.SyncCollections<CubeRegistrationInfo>(Model.Cubes, cubes, (a, b) => a.CubeId == b.CubeId);
                        ((BICubesConfigView)GetView()).SetCubesInfo(Model.Cubes);
                    });
                });
            }
        }

    }
}