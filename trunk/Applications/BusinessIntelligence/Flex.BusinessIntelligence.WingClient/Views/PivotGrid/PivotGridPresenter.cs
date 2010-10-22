using Wing.Client.Sdk;

namespace Flex.BusinessIntelligence.WingClient.Views.PivotGrid
{
    public class PivotGridPresenter : ViewPresenter<PivotGridPresentationModel>
    {
        public PivotGridPresenter(PivotGridPresentationModel model)
            : base(model, new PivotGridView(), null)
        {
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