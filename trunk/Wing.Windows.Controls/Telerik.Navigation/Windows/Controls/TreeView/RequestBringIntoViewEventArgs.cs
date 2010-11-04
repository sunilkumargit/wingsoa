namespace Telerik.Windows.Controls.TreeView
{
    using System;
    using System.Windows;
    using Telerik.Windows;

    public class RequestBringIntoViewEventArgs : RadRoutedEventArgs
    {
        private DependencyObject target;
        private Rect targetRectangle;

        internal RequestBringIntoViewEventArgs(DependencyObject target, Rect targetRect)
        {
            this.target = target;
            this.targetRectangle = targetRect;
        }

        protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget)
        {
            RequestBringIntoViewEventHandler handler = (RequestBringIntoViewEventHandler) genericHandler;
            handler(genericTarget, this);
        }

        public DependencyObject TargetObject
        {
            get
            {
                return this.target;
            }
        }

        public Rect TargetRect
        {
            get
            {
                return this.targetRectangle;
            }
        }
    }
}

