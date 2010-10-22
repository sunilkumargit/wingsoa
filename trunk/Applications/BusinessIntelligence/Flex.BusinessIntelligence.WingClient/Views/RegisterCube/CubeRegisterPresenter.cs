using System;
using Flex.BusinessIntelligence.Data;
using Flex.BusinessIntelligence.Interop.Services;
using Wing.Client.Sdk;
using Wing.Client.Sdk.Controls;
using Wing.ServiceLocation;

namespace Flex.BusinessIntelligence.WingClient.Views.RegisterCube
{
    public class CubeRegisterPresenter : PopupWindowPresenter<CubeRegisterPresentationModel>
    {
        public CubeRegisterPresenter()
            : base(new CubeRegisterPresentationModel() { Caption = "Registrar um novo cubo" }, new CubeRegisterView(), null)
        {
            Model.CubeInfo = new CubeRegistrationInfo()
            {
                CubeId = Guid.NewGuid()
            };
        }

        protected override void ActiveStateChanged()
        {
            base.ActiveStateChanged();
            if (IsActive)
            {
                WindowHandler.SetDialogStyle(ModalDialogStyles.OKCancel);
                ((CubeRegisterView)GetView()).SetModel(Model);
            }
        }

        protected override void WindowClosing(DialogResultArgs args)
        {
            base.WindowClosing(args);
            if (args.Result == DialogResult.OK)
            {
                args.Cancel = true;
                if (((CubeRegisterView)GetView()).Validate())
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
                ((CubeRegisterView)GetView()).CancelEdit();
        }
    }
}
