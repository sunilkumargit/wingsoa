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
using Wing.Client.Sdk.Controls;
using Wing.Olap.Controls;

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
        }
    }
}