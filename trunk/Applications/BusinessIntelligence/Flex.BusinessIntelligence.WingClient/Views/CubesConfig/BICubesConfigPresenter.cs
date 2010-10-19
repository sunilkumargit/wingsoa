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
using Wing.ServiceLocation;

namespace Flex.BusinessIntelligence.WingClient.Views.CubesConfig
{
    public class BICubesConfigPresenter : ViewPresenter<BICubesConfigPresentationModel>
    {
        public BICubesConfigPresenter(BICubesConfigPresentationModel presentationModel, BICubesConfigView view)
            : base(presentationModel, view, null)
        {
            var _view = (BICubesConfigView)GetView();
            _view.ItemTriggered += new SingleEventHandler<CubeRegistrationInfo>(_view_ItemTriggered);
        }

        void _view_ItemTriggered(CubeRegistrationInfo sender)
        {
            CommandsManager.GetCommand(BICommandNames.CubeShowProperties).Execute(sender);
        }

        protected override void ActiveStateChanged()
        {
            base.ActiveStateChanged();
            if (IsActive)
            {
                //refresh nos cubos registrados
                WorkContext.Async(() =>
                {
                    var cubeProxy = ServiceLocator.Current.GetInstance<ICubeServicesProxy>();
                    if (Model.Cubes == null)
                    {
                        Model.Cubes = cubeProxy.Cubes;
                        ((BICubesConfigView)GetView()).SetCubesInfo(Model.Cubes);
                    }
                });
            }
        }

    }
}