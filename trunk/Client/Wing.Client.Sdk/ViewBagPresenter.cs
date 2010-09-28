using System;
using System.Collections.ObjectModel;
using System.Linq;
using Wing.Client.Sdk.Events;
using Wing.Composite.Presentation.Regions;
using Wing.Composite.Regions;
using Wing.Events;
using Wing.ServiceLocation;

namespace Wing.Client.Sdk
{
    public abstract class ViewBagPresenter : ViewPresenter, IViewBagPresenter
    {
        private ObservableCollection<IViewPresenter> _views = new ObservableCollection<IViewPresenter>();
        private ReadOnlyObservableCollection<IViewPresenter> _readOnlyCollection;
        private string _contentRegion;
        private IEventAggregator _eventAggregator;
        private INavigationHistoryService _history;
        private IRegionManager _regionManager;

        protected ViewBagPresenter(IViewPresentationModel model, Object view, IRegionManager regionManager, String contentRegionName)
            : base(model, view, regionManager)
        {
            _history = ServiceLocator.Current.GetInstance<INavigationHistoryService>();
            _eventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
            _readOnlyCollection = new ReadOnlyObservableCollection<IViewPresenter>(_views);
            _regionManager = regionManager ?? ServiceLocator.Current.GetInstance<IRegionManager>();
            _contentRegion = contentRegionName;
            regionManager.Regions[_contentRegion].ActiveViews.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(ActiveViews_CollectionChanged);
        }

        void ActiveViews_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateActiveView();
        }

        protected ViewBagPresenter(IViewPresentationModel model, Object view, IRegionManager regionManager)
            : this(model, view, regionManager, "Content") { }

        protected ViewBagPresenter(IViewPresentationModel model, Object view)
            : this(model, view, null, "Content") { }

        public ReadOnlyObservableCollection<IViewPresenter> Views { get; private set; }
        public void Navigate(IViewPresenter presenter, bool addToHistory)
        {
            if (presenter.Parent == this || presenter.Parent == null)
            {
                if (ActivePresenter != presenter)
                {
                    if (ActivePresenter != null)
                    {
                        if (addToHistory)
                            _history.Push(ActivePresenter);
                        RegionManager.Regions[_contentRegion].Deactivate(ActivePresenter.GetView());
                    }
                    presenter.SetParent(this);
                    if (_views.Contains(presenter))
                        RegionManager.Regions[_contentRegion].Activate(presenter.GetView());
                    else
                    {
                        _views.Add(presenter);
                        RegionManager.Regions[_contentRegion].Add(presenter.GetView(), presenter.RegionManager);
                        RegionManager.Regions[_contentRegion].Activate(presenter.GetView());
                    }
                    UpdateActiveView();
                }
            }
            else if (presenter.Parent != null && presenter.Parent is IViewBagPresenter)
            {
                var activePresenter = ActivePresenter;
                var parent = presenter.Parent as IViewBagPresenter;
                Navigate(presenter.Parent, false);
                parent.Navigate(presenter, false);
                if (ActivePresenter != activePresenter && activePresenter != null && addToHistory)
                    _history.Push(activePresenter);
            }
        }

        public void Navigate(IViewPresenter presenter)
        {
            Navigate(presenter, true);
        }

        private void UpdateActiveView()
        {
            var activeView = RegionManager.Regions[_contentRegion].ActiveViews.FirstOrDefault();
            if (activeView != null)
            {
                var presenter = _views.FirstOrDefault(p => p.GetView() == activeView);
                if (presenter == null)
                    throw new Exception("Active view has no presenter");
                ActivePresenter = presenter;
            }
            else
                ActivePresenter = null;
            _eventAggregator.GetEvent<ViewBagActiveViewChangedEvent>().Publish(new ViewBagActiveViewChangedEventArgs(this));
        }

        public IViewPresenter ActivePresenter { get; private set; }
    }

    public abstract class ViewBagPresenter<TModel> : ViewBagPresenter, IViewBagPresenter<TModel> where TModel : IViewPresentationModel
    {
        protected ViewBagPresenter(IViewPresentationModel model, Object view, IRegionManager regionManager, String contentRegionName)
            : base(model, view, regionManager, contentRegionName) { }

        protected ViewBagPresenter(IViewPresentationModel model, Object view, IRegionManager regionManager)
            : base(model, view, regionManager) { }

        protected ViewBagPresenter(IViewPresentationModel model, Object view)
            : base(model, view) { }

        public TModel Model
        {
            get { return (TModel)GetPresentationModel(); }
        }
    }
}
