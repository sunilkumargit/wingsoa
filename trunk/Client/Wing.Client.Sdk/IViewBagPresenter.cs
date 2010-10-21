using System.Collections.ObjectModel;

namespace Wing.Client.Sdk
{
    public interface IViewBagPresenter : IViewPresenter
    {
        ReadOnlyObservableCollection<IViewPresenter> Views { get; }
        IViewPresenter ActivePresenter { get; }
        void Navigate(IViewPresenter presenter);
        void Navigate(IViewPresenter presenter, bool addToHistory);
        void RemoveView(IViewPresenter presenter);
    }

    public interface IViewBagPresenter<TModel> : IViewBagPresenter, IViewPresenter<TModel>
        where TModel : IViewPresentationModel
    { }
}
