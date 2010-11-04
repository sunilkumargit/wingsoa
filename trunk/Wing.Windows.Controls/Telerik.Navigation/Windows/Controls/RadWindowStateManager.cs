namespace Telerik.Windows.Controls
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Windows;

    internal class RadWindowStateManager
    {
        private bool firstTimeMinimized;
        private Rect maximized;
        private Rect minimized;
        private Rect normal;

        public RadWindowStateManager(RadWindow window)
        {
            this.Window = window;
            this.normal = new Rect(Math.Round(this.Window.Left, 0), Math.Round(this.Window.Top, 0), Math.Round(this.Window.Width, 0), Math.Round(this.Window.Height, 0));
            this.minimized.Height = double.NaN;
            this.minimized.Width = double.NaN;
            this.firstTimeMinimized = true;
            this.Window.SizeChanged += new SizeChangedEventHandler(this.Window_SizeChanged);
            this.Window.LocationChanged += new RoutedEventHandler(this.Window_LocationChanged);
        }

        internal void UpdateLayout()
        {
            if (this.Window.WindowState == WindowState.Normal)
            {
                this.normal.Width = this.Window.Width;
                this.normal.Height = this.Window.Height;
            }
        }

        public void UpdateLeft(double left)
        {
            left = Math.Round(left, 0);
            if (this.Window.WindowState == WindowState.Normal)
            {
                this.normal.X = left;
            }
            if (WindowState.Minimized == this.Window.WindowState)
            {
                this.minimized.X = left;
                if (!this.Window.RestoreMinimizedLocation)
                {
                    this.normal.X = left;
                }
            }
        }

        private void UpdateMaximize()
        {
            if (this.Window.Popup != null)
            {
                Size appSize = this.Window.Popup.GetRootSize();
                this.maximized.X = this.Window.RestrictedAreaMargin.Left;
                this.maximized.Y = this.Window.RestrictedAreaMargin.Top;
                this.maximized.Width = Math.Min(this.Window.MaxWidth, (appSize.Width - this.Window.RestrictedAreaMargin.Left) - this.Window.RestrictedAreaMargin.Right);
                this.maximized.Height = Math.Min(this.Window.MaxHeight, (appSize.Height - this.Window.RestrictedAreaMargin.Top) - this.Window.RestrictedAreaMargin.Bottom);
            }
        }

        private void UpdateMinimized()
        {
            if (!this.Window.RestoreMinimizedLocation || this.firstTimeMinimized)
            {
                this.minimized.Y = this.normal.Y;
                this.minimized.X = this.normal.X;
                this.firstTimeMinimized = false;
            }
        }

        public void UpdateTop(double top)
        {
            top = Math.Round(top, 0);
            if (this.Window.WindowState == WindowState.Normal)
            {
                this.normal.Y = top;
            }
            if (WindowState.Minimized == this.Window.WindowState)
            {
                this.minimized.Y = top;
                if (!this.Window.RestoreMinimizedLocation)
                {
                    this.normal.Y = top;
                }
            }
        }

        private void Window_LocationChanged(object sender, RoutedEventArgs e)
        {
            this.UpdateLayout();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.UpdateLayout();
        }

        public Rect SizeAndPosition
        {
            get
            {
                switch (this.Window.WindowState)
                {
                    case WindowState.Normal:
                        return this.normal;

                    case WindowState.Minimized:
                        this.UpdateMinimized();
                        return this.minimized;

                    case WindowState.Maximized:
                        this.UpdateMaximize();
                        return this.maximized;
                }
                return new Rect(0.0, 0.0, 0.0, 0.0);
            }
        }

        private RadWindow Window { get; set; }
    }
}

