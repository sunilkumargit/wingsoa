using System;
using Wing.Composite.Regions;
using Wing.Composite.Presentation.Regions;
using System.Windows;
using System.Collections.ObjectModel;

namespace Wing.Client.Sdk
{
    public interface IViewBagPresenter : IViewPresenter
    {
        ReadOnlyObservableCollection<IViewPresenter> Views { get; }
        IViewPresenter ActivePresenter { get; }
        void Navigate(IViewPresenter presenter);
    }

    public interface IViewBagPresenter<TModel> : IViewBagPresenter, IViewPresenter<TModel>
        where TModel : IViewPresentationModel
    { }
}
