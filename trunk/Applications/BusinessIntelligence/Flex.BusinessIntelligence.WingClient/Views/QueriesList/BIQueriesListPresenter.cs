using Flex.BusinessIntelligence.Client.Interop;
using Flex.BusinessIntelligence.Interop.Services;
using Wing.Client.Sdk;
using Wing.ServiceLocation;
using Flex.BusinessIntelligence.Data;

namespace Flex.BusinessIntelligence.WingClient.Views.QueriesList
{
    public class BIQueriesListPresenter : ViewPresenter<BIQueriesListPresentationModel>
    {
        public BIQueriesListPresenter(BIQueriesListPresentationModel model, BIQueriesListView view)
            : base(model, view, null)
        {
            view.ItemTriggered += new SingleEventHandler<CubeQueryInfo>(view_ItemTriggered);
        }

        void view_ItemTriggered(CubeQueryInfo sender)
        {
            CommandsManager.GetCommand(BICommandNames.OpenCubeQuery).Execute(sender);
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
