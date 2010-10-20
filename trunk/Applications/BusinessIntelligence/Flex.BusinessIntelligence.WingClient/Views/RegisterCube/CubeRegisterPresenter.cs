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
using Wing.Client.Sdk.Controls;
using Wing.Client.Sdk;
using Flex.BusinessIntelligence.Data;
using Wing.ServiceLocation;
using Flex.BusinessIntelligence.Interop.Services;

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
