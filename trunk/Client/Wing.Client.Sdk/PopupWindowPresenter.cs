using System;
using System.Windows;
using Wing.Composite.Regions;
using Wing.ServiceLocation;
using Wing.Client.Sdk.Controls;

namespace Wing.Client.Sdk
{
    public abstract class PopupWindowPresenter : ViewPresenter, IPopupWindowPresenter
    {
        private IPopupWindowHandler _handler;

        public PopupWindowPresenter(IViewPresentationModel model, Object view, IRegionManager regionManager)
            : base(model, view, regionManager)
        {

        }

        public void CallbackSetWindowHandler(IPopupWindowHandler handler)
        {
            WindowHandler = handler;
        }

        public IPopupWindowHandler WindowHandler
        {
            get { return _handler; }
            set
            {
                if (_handler != null)
                    throw new Exception("This presenter already has a window handler");
                _handler = value;
                if (_handler != null)
                {
                    SetActiveState(true);
                    _handler.Closing += new EventHandler<DialogResultArgs>(_handler_Closing);
                    _handler.Closed += new EventHandler<DialogResultArgs>(_handler_Closed);
                    _handler.SetTitle(this.Caption);
                }
            }
        }

        void _handler_Closed(object sender, DialogResultArgs e)
        {
            WindowClosed(e.Result);
        }

        void _handler_Closing(object sender, DialogResultArgs e)
        {
            WindowClosing(e);
        }

        protected virtual void WindowClosing(DialogResultArgs args) { }
        protected virtual void WindowClosed(DialogResult result) { }
    }

    public abstract class PopupWindowPresenter<TModel> : PopupWindowPresenter, IPopupWindowPresenter<TModel> where TModel : IViewPresentationModel
    {
        protected PopupWindowPresenter(TModel model, Object view, IRegionManager regionManager)
            : base(model, view, regionManager) { }

        public TModel Model
        {
            get { return (TModel)GetPresentationModel(); }
        }
    }


}
