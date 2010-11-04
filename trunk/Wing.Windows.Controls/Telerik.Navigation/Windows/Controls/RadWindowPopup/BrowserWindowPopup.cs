namespace Telerik.Windows.Controls.RadWindowPopup
{
    using System;
    using System.Windows;
    using Telerik.Windows.Controls;

    internal abstract class BrowserWindowPopup : WindowPopup
    {
        protected BrowserWindowPopup()
        {
        }

        public override Size GetRootSize()
        {
            return ApplicationHelper.ApplicationSize;
        }

        private void OnApplicationLayoutUpdated(object sender, EventArgs e)
        {
            FrameworkElement app = ApplicationHelper.RootVisual;
            if ((app.ActualWidth > 0.0) && (app.ActualHeight > 0.0))
            {
                app.LayoutUpdated -= new EventHandler(this.OnApplicationLayoutUpdated);
                this.PositionWindow();
            }
        }

        protected override void OnOpened(EventArgs args)
        {
            base.OnOpened(args);
            if ((base.WindowStartupLocation == Telerik.Windows.Controls.WindowStartupLocation.CenterOwner) || (base.WindowStartupLocation == Telerik.Windows.Controls.WindowStartupLocation.CenterScreen))
            {
                this.PositionWindow();
            }
        }

        private void OnWindowLayoutUpdated(object sender, EventArgs e)
        {
            RadWindow window = base.Child as RadWindow;
            if (((window != null) && (window.ActualWidth > 0.0)) && (window.ActualHeight > 0.0))
            {
                window.LayoutUpdated -= new EventHandler(this.OnWindowLayoutUpdated);
                this.PositionWindow();
            }
        }

        private void OnWindowOwnerLayoutUpdated(object sender, EventArgs e)
        {
            if ((base.Owner.ActualWidth > 0.0) && (base.Owner.ActualHeight > 0.0))
            {
                base.Owner.LayoutUpdated -= new EventHandler(this.OnWindowOwnerLayoutUpdated);
                this.PositionWindow();
            }
        }

        protected override void OpenPopup()
        {
            RadWindow owner = base.Owner as RadWindow;
            if ((owner != null) && !owner.IsOpen)
            {
                throw new InvalidOperationException("Cannot set Owner property to a RadWindow that has not been shown previously.");
            }
        }

        private void PositionWindow()
        {
            RadWindow window = base.Child as RadWindow;
            if (window != null)
            {
                Size rootSize = this.GetRootSize();
                if ((rootSize.Width <= 0.0) || (rootSize.Height <= 0.0))
                {
                    ApplicationHelper.RootVisual.LayoutUpdated += new EventHandler(this.OnApplicationLayoutUpdated);
                }
                else if ((window.ActualWidth <= 0.0) || (window.ActualHeight <= 0.0))
                {
                    window.LayoutUpdated += new EventHandler(this.OnWindowLayoutUpdated);
                }
                else if (((base.WindowStartupLocation == Telerik.Windows.Controls.WindowStartupLocation.CenterOwner) && (base.Owner != null)) && ((base.Owner.ActualWidth <= 0.0) || (base.Owner.ActualHeight <= 0.0)))
                {
                    base.Owner.LayoutUpdated += new EventHandler(this.OnWindowOwnerLayoutUpdated);
                }
                else
                {
                    Point parentStart = new Point(window.LeftOffset, window.TopOffset);
                    Size parentSize = this.GetRootSize();
                    if ((base.WindowStartupLocation == Telerik.Windows.Controls.WindowStartupLocation.CenterOwner) && (base.Owner != null))
                    {
                        parentStart = base.Owner.TransformToVisual(ApplicationHelper.RootVisual).Transform(parentStart);
                        parentSize = new Size(base.Owner.ActualWidth, base.Owner.ActualHeight);
                    }
                    Rect parentRect = new Rect(parentStart, parentSize);
                    Point newLocation = new PlacementHelper(parentRect, new Size(window.ActualWidth, window.ActualHeight), 0.0, 0.0, this.GetRootSize(), window.FlowDirection).GetPlacementOrigin(PlacementMode.Center);
                    window.Left = newLocation.X;
                    window.Top = newLocation.Y;
                    window.UpdateLayout();
                }
            }
        }
    }
}

