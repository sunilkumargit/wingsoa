namespace Telerik.Windows.Controls.RadWindowPopup
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using Telerik.Windows.Controls;

    internal abstract class WindowPopup
    {
        private bool isConfigured;
        private ContentControl owner;

        public event EventHandler Opened;

        protected WindowPopup()
        {
        }

        public abstract void BringToFront();
        public void Close()
        {
            this.ClosePopup();
            this.IsOpen = false;
        }

        protected abstract void ClosePopup();
        public void Configure(UIElement child, UIElement modalCanvas, bool isTopMost)
        {
            if (this.isConfigured)
            {
                throw new InvalidOperationException("You cannot reconfigure WindowPopup");
            }
            this.IsTopMost = isTopMost;
            this.Child = child;
            this.ModalCanvas = modalCanvas;
            this.isConfigured = true;
        }

        public virtual Point GetRootPosition()
        {
            return new Point();
        }

        public abstract Size GetRootSize();
        public abstract UIElement GetVisual();
        public abstract int GetZIndex();
        protected void InvalidateArrange()
        {
            if ((this.Child != null) && this.IsOpen)
            {
                this.Child.InvalidateArrange();
            }
        }

        public abstract void Move(double left, double top);
        protected void OnOpened()
        {
            this.IsOpen = true;
            this.OnOpened(EventArgs.Empty);
        }

        protected virtual void OnOpened(EventArgs args)
        {
            if (this.Opened != null)
            {
                this.Opened(this, args);
            }
        }

        public void Open(bool isModal)
        {
            if (!this.IsOpen)
            {
                this.IsModal = isModal;
                this.OpenPopup();
            }
        }

        protected abstract void OpenPopup();

        public UIElement Child { get; private set; }

        public bool IsModal { get; private set; }

        public bool IsOpen { get; private set; }

        public bool IsTopMost { get; private set; }

        public UIElement ModalCanvas { get; private set; }

        public ContentControl Owner
        {
            get
            {
                return this.owner;
            }
            set
            {
                this.owner = value as RadWindow;
            }
        }

        public Telerik.Windows.Controls.WindowStartupLocation WindowStartupLocation { get; set; }
    }
}

