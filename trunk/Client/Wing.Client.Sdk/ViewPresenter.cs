using System;
using System.Collections.Generic;
using Wing.Composite.Regions;
using Wing.Composite.Presentation.Regions;
using System.Windows;
using System.Collections.ObjectModel;

namespace Wing.Client.Sdk
{
    public abstract class ViewPresenter : IViewPresenter
    {
        private IViewPresentationModel _presentationModel;
        private object _view;
        private IRegionManager _regionManager;

        protected ViewPresenter(IViewPresentationModel model, Object view, IRegionManager regionManager)
        {
            _presentationModel = model;
            _view = view;
            _regionManager = regionManager;
            Wing.Composite.Presentation.Regions.RegionManager.SetRegionManager((DependencyObject)view, _regionManager);
        }

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