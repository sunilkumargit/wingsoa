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
using Wing.ServiceLocation;
using Flex.BusinessIntelligence.Interop.Services;

namespace Flex.BusinessIntelligence.WingClient.Views.QueriesList
{
    public class BIQueriesListPresenter : ViewPresenter<BIQueriesListPresentationModel>
    {
        public BIQueriesListPresenter(BIQueriesListPresentationModel model, BIQueriesListView view)
            : base(model, view, null)
        {

        }

        protected override void ActiveStateChanged()
        {
            base.ActiveStateChanged();
            if (IsActive)
            {
                if (Model.Queries == null)
                {
                    WorkContext.Async(() =>
                    {
                        var proxy = ServiceLocator.GetInstance<ICubeServicesProxy>();
                        Model.Queries = proxy.Queries;
                        Model.Cubes = proxy.Cubes;
                        ((BIQueriesListView)GetView()).SetModel(Model);
                    });
                }
            }
        }
    }
}
