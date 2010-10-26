using Flex.BusinessIntelligence.Interop.Services;
using Wing.Client.Sdk;
using Wing.Client.Sdk.Controls;
using Wing.ServiceLocation;
using Flex.BusinessIntelligence.Data;

namespace Flex.BusinessIntelligence.WingClient.Views.SaveQuery
{
    public class SaveQueryPresenter : PopupWindowPresenter<SaveQueryPresentationModel>
    {
        private bool _closing;
        public SaveQueryPresenter(CubeQueryInfo queryInfo)
            : base(new SaveQueryPresentationModel(queryInfo) { Caption = "Salvar consulta" }, new SaveQueryView(), null)
        {
        }

        protected override void ActiveStateChanged()
        {
            base.ActiveStateChanged();
            WindowHandler.SetDialogStyle(ModalDialogStyles.OKCancel);
            ((SaveQueryView)GetView()).SetModel(Model);
        }

        protected override void WindowClosing(DialogResultArgs args)
        {
            base.WindowClosing(args);
            if (args.Result == DialogResult.OK)
            {
                args.Cancel = true;
                if (((SaveQueryView)GetView()).Validate())
                {
                    WorkContext.Sync(() =>
                    {
                        ServiceLocator.GetInstance<ICubeServicesProxy>()
                            .SaveQuery(Model.QueryInfo, (res) =>
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
                ((SaveQueryView)GetView()).CancelEdit();
        }
    }
}
