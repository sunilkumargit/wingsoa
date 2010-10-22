using Flex.BusinessIntelligence.Client.Interop;
using Flex.BusinessIntelligence.Data;
using Flex.BusinessIntelligence.Interop.Services;
using Wing.Client.Sdk;
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
                    var cubeProxy = ServiceLocator.GetInstance<ICubeServicesProxy>();
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