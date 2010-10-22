using Flex.BusinessIntelligence.Interop.Services;
using Wing.Client.Sdk;
using Wing.Client.Sdk.Controls;
using Wing.ServiceLocation;

namespace Flex.BusinessIntelligence.WingClient.Views.CubeProperties
{
    public class CubePropertiesPresenter : PopupWindowPresenter<CubePropertiesPresentationModel>
    {
        private bool _closing;
        public CubePropertiesPresenter()
            : base(new CubePropertiesPresentationModel() { Caption = "Propriedades do cubo" }, new CubePropertiesView(), null)
        {
        }

        protected override void ActiveStateChanged()
        {
            base.ActiveStateChanged();
            WindowHandler.SetDialogStyle(ModalDialogStyles.OKCancel);
            ((CubePropertiesView)GetView()).SetModel(Model);
        }

        protected override void WindowClosing(DialogResultArgs args)
        {
            base.WindowClosing(args);
            if (args.Result == DialogResult.OK)
            {
                args.Cancel = true;
                if (((CubePropertiesView)GetView()).Validate())
                {
                    WorkContext.Sync(() =>
                    {
                        ServiceLocator.GetInstance<ICubeServicesProxy>()
                            .SaveCube(Model.CubeInfo, (res) =>
                            {
                                if (res.Status == Wing.Soa.Interop.OperationStatus.Error)
                                    ShellService.Alert(res.Message);
                                else
                                    WindowHandler.Close(false);
                            });
                    });
                }
            }
            else
                ((CubePropertiesView)GetView()).CancelEdit();
        }
    }
}
