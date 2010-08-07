using System;
using System.Collections.Generic;
using Wing.Composite.Regions;
using Wing.Composite.Presentation.Regions;
using System.Windows;
using System.Linq;
using System.Collections.ObjectModel;
using Wing.ServiceLocation;
using Wing.Events;
using Wing.Client.Sdk.Events;

namespace Wing.Client.Sdk
{
    public abstract class ViewBagPresenter : ViewPresenter, IViewBagPresenter
    {
        private ObservableCollection<IViewPresenter> _views = new ObservableCollection<IViewPresenter>();
        private ReadOnlyObservableCollection<IViewPresenter> _readOnlyCollection;
        private string _contentRegion;
        private IEventAggregator _eventAggregator;

        protected ViewBagPresenter(IViewPresentationModel model, Object view, IRegionManager regionManager, IEventAggregator eventAggregator, String contentRegionName)
            : base(model, view, regionManager)
        {
            _readOnlyCollection = new ReadOnlyObservableCollection<IViewPresenter>(_views);
            _contentRegion = contentRegionName;
            _eventAggregator = eventAggregator;
            regionManager.Regions[_contentRegion].ActiveViews.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(ActiveViews_CollectionChanged);
        }

        void ActiveViews_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateActiveView();
        }

        protected ViewBagPresenter(IViewPresentationModel model, Object view, IRegionManager regionManager, IEventAggregator eventAggregator)
            : this(model, view, regionManager, eventAggregator, "Content") { }

        public ReadOnlyObservableCollection<IViewPresenter> Views { get; private set; }
        public void Navigate(IViewPresenter presenter)
        {
            if (presenter.Parent == this || presenter.Parent == null)
            {
                if (ActivePresenter != presenter)
                {
                    presenter.SetParent(this);
                    if (_views.Contains(presenter))
                        RegionManager.Regions[_contentRegion].Activate(presenter.GetView());
                    else
                    {
                        _views.Add(presenter);
                        RegionManager.Regions[_contentRegion].Add(presenter.GetView(), presenter.RegionManager);
                    }
                    UpdateActiveView();
                }
            }
            else if (presenter.Parent != null && presenter.Parent is IViewBagPresenter)
            {
                var parent = presenter.Parent as IViewBagPresenter;
                Navigate(presenter.Parent);
                parent.Navigate(presenter);
            }
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
        protected ViewBagPresenter(IViewPresentationModel model, Object view, IRegionManager regionManager, IEventAggregator eventAggregator, String contentRegionName)
            : base(model, view, regionManager, eventAggregator, contentRegionName) { }

        protected ViewBagPresenter(IViewPresentationModel model, Object view, IRegionManager regionManager, IEventAggregator eventAggregator)
            : base(model, view, regionManager, eventAggregator) { }

        public TModel Model
        {
            get { return (TModel)GetPresentationModel(); }
        }
    }
}
