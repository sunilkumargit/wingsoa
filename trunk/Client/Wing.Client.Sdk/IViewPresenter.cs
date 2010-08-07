using System;
using Wing.Composite.Regions;

namespace Wing.Client.Sdk
{
    public interface IViewPresenter
    {
        void SetParent(IViewPresenter parent);
        IViewPresenter Parent { get; }
        IViewPresentationModel GetPresentationModel();
        Object GetView();
        String Caption { get; }
        IRegionManager RegionManager { get; }
    }

    public interface IViewPresenter<TModel> : IViewPresenter where TModel : IViewPresentationModel
    {
        TModel Model { get; }
    }
}