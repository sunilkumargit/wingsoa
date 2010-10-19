using System;
using Wing.Composite.Regions;
using Wing.Client.Sdk.Controls;

namespace Wing.Client.Sdk
{
    public interface IPopupWindowPresenter : IViewPresenter
    {
        void CallbackSetWindowHandler(IPopupWindowHandler handler);
        IPopupWindowHandler WindowHandler { get; }
    }

    public interface IPopupWindowPresenter<TModel> : IPopupWindowPresenter, IViewPresenter<TModel>
        where TModel : IViewPresentationModel
    {
    }
}
