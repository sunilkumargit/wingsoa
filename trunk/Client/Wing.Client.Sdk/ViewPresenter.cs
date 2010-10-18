using System;
using System.Windows;
using Wing.Composite.Regions;
using Wing.ServiceLocation;
using Wing.Client.Sdk.Controls;

namespace Wing.Client.Sdk
{
    public abstract class ViewPresenter : IViewPresenter
    {
        private IViewPresentationModel _presentationModel;
        private object _view;
        private IRegionManager _regionManager;
        private bool _isActive;

        protected ViewPresenter(IViewPresentationModel model, Object view, IRegionManager regionManager)
        {
            _presentationModel = model;
            _view = view;
            if (view is HeaderedPage)
            {
                var page = (HeaderedPage)view;
                page.PageTitle = model.Caption;
                page.SubTitle = model.Title;
            }
            _regionManager = regionManager ?? ServiceLocator.Current.GetInstance<IRegionManager>();
            Wing.Composite.Presentation.Regions.RegionManager.SetRegionManager((DependencyObject)view, _regionManager);
        }

        public bool IsActive { get { return _isActive; } }

        internal void SetActiveState(bool isActive)
        {
            if (isActive == _isActive)
                return;
            _isActive = isActive;
            ActiveStateChanged();
        }

        protected virtual void ActiveStateChanged() { }

        public virtual IViewPresentationModel GetPresentationModel()
        {
            return _presentationModel;
        }

        public virtual object GetView()
        {
            return _view;
        }

        public IViewPresenter Parent { get; private set; }

        public virtual IRegionManager RegionManager { get { return _regionManager; } }

        public virtual string Caption
        {
            get { return GetPresentationModel().Caption; }
        }

        public void SetParent(IViewPresenter parent)
        {
            if (parent != null && this.Parent != null && this.Parent != parent)
                throw new Exception("The presenter has a parent already.");
            Parent = parent;
        }
    }

    public abstract class ViewPresenter<TModel> : ViewPresenter, IViewPresenter<TModel> where TModel : IViewPresentationModel
    {
        protected ViewPresenter(IViewPresentationModel model, Object view, IRegionManager regionManager)
            : base(model, view, regionManager) { }

        public TModel Model
        {
            get { return (TModel)GetPresentationModel(); }
        }
    }

}