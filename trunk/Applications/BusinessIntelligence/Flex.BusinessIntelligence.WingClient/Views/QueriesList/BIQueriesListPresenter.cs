using Flex.BusinessIntelligence.Client.Interop;
using Flex.BusinessIntelligence.Interop.Services;
using Wing.Client.Sdk;
using Wing.ServiceLocation;

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
