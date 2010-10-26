using Wing.Client.Sdk;
using Flex.BusinessIntelligence.WingClient.Views.SaveQuery;
using Flex.BusinessIntelligence.Data;
using Wing.ServiceLocation;
using Flex.BusinessIntelligence.Interop.Services;
using System;

namespace Flex.BusinessIntelligence.WingClient.Views.PivotGrid
{
    public class PivotGridPresenter : ViewPresenter<PivotGridPresentationModel>
    {
        public PivotGridPresenter(PivotGridPresentationModel model)
            : base(model, new PivotGridView(), null)
        {
            PivotGridView view = (PivotGridView)GetView();
            view.SaveQueryTriggered += new SingleEventHandler<string>(view_SaveQueryTriggered);
        }

        void view_SaveQueryTriggered(string arg)
        {
            if (Model.QueryInfo == null)
            {
                var queryInfo = new CubeQueryInfo()
                {
                    QueryId = Guid.NewGuid(),
                    CubeId = Model.CubeInfo.CubeId,
                    QueryData = arg
                };
                var presenter = new SaveQueryPresenter(queryInfo);
                ShellService.ShowPopup(presenter);
            }
            else
            {
                Model.QueryInfo.QueryData = arg;
                WorkContext.Sync(() =>
                {
                    ServiceLocator.GetInstance<ICubeServicesProxy>()
                        .SaveQuery(Model.QueryInfo, (res) =>
                        {
                            if (res.Status == Wing.Soa.Interop.OperationStatus.Error)
                                ShellService.Alert(res.Message);
                        });
                });
            }
        }

        protected override void ActiveStateChanged()
        {
            base.ActiveStateChanged();
            // remover a consulta quando ela não estiver mais ativa no contexto do BI.
            if (!IsActive && Parent is IViewBagPresenter)
                ((IViewBagPresenter)Parent).RemoveView(this);

            if (IsActive)
            {
                var view = (PivotGridView)GetView();
                view.SetModel(Model);
            }
        }
    }


}