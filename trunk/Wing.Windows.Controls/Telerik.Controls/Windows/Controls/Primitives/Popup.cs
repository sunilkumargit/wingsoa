namespace Telerik.Windows.Controls.Primitives
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using Telerik.Windows;
    using Telerik.Windows.Controls;
    using Telerik.Windows.Controls.Animation;

    public class Popup : ContentControl
    {
        public static readonly DependencyProperty CatchClickOutsidePopupProperty = DependencyProperty.Register("CatchClickOutsidePopup", typeof(bool), typeof(Telerik.Windows.Controls.Primitives.Popup), new Telerik.Windows.PropertyMetadata(false, new PropertyChangedCallback(Telerik.Windows.Controls.Primitives.Popup.OnPopupPropertyChanged)));
        private bool changingContent;
        public static readonly DependencyProperty ClipAroundElementProperty = DependencyProperty.Register("ClipAroundElement", typeof(FrameworkElement), typeof(Telerik.Windows.Controls.Primitives.Popup), new Telerik.Windows.PropertyMetadata(null, new PropertyChangedCallback(Telerik.Windows.Controls.Primitives.Popup.OnPopupPropertyChanged)));
        public static readonly Telerik.Windows.RoutedEvent ClosedEvent = EventManager.RegisterRoutedEvent("Closed", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Telerik.Windows.Controls.Primitives.Popup));
        public static readonly DependencyProperty CloseOnOutsideClickProperty = DependencyProperty.Register("CloseOnOutsideClick", typeof(bool), typeof(Telerik.Windows.Controls.Primitives.Popup), new Telerik.Windows.PropertyMetadata(false, new PropertyChangedCallback(Telerik.Windows.Controls.Primitives.Popup.OnPopupPropertyChanged)));
        public static readonly DependencyProperty HorizontalOffsetProperty = DependencyProperty.Register("HorizontalOffset", typeof(double), typeof(Telerik.Windows.Controls.Primitives.Popup), new Telerik.Windows.PropertyMetadata(0.0));
        private bool isLoaded;
        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register("IsOpen", typeof(bool), typeof(Telerik.Windows.Controls.Primitives.Popup), new Telerik.Windows.PropertyMetadata(false, new PropertyChangedCallback(Telerik.Windows.Controls.Primitives.Popup.OnIsOpenPropertyChanged)));
        public static readonly Telerik.Windows.RoutedEvent OpenedEvent = EventManager.RegisterRoutedEvent("Opened", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Telerik.Windows.Controls.Primitives.Popup));
        public static readonly DependencyProperty OwnerProperty = DependencyProperty.Register("Owner", typeof(FrameworkElement), typeof(Telerik.Windows.Controls.Primitives.Popup), new Telerik.Windows.PropertyMetadata(null, new PropertyChangedCallback(Telerik.Windows.Controls.Primitives.Popup.OnPopupPropertyChanged)));
        public static readonly DependencyProperty PlacementProperty = DependencyProperty.Register("Placement", typeof(Telerik.Windows.Controls.PlacementMode), typeof(Telerik.Windows.Controls.Primitives.Popup), new Telerik.Windows.PropertyMetadata(Telerik.Windows.Controls.PlacementMode.Bottom, new PropertyChangedCallback(Telerik.Windows.Controls.Primitives.Popup.OnPopupPropertyChanged)));
        public static readonly DependencyProperty PlacementRectangleProperty = DependencyProperty.Register("PlacementRectangle", typeof(Rect), typeof(Telerik.Windows.Controls.Primitives.Popup), new Telerik.Windows.PropertyMetadata(Rect.Empty, new PropertyChangedCallback(Telerik.Windows.Controls.Primitives.Popup.OnPopupPropertyChanged)));
        public static readonly DependencyProperty PlacementTargetProperty = DependencyProperty.Register("PlacementTarget", typeof(UIElement), typeof(Telerik.Windows.Controls.Primitives.Popup), new Telerik.Windows.PropertyMetadata(null, new PropertyChangedCallback(Telerik.Windows.Controls.Primitives.Popup.OnPopupPropertyChanged)));
        private System.Windows.Controls.Primitives.Popup popup = new System.Windows.Controls.Primitives.Popup();
        public static readonly DependencyProperty UsePlacementTargetAsClipElementProperty = DependencyProperty.Register("UsePlacementTargetAsClipElement", typeof(bool), typeof(Telerik.Windows.Controls.Primitives.Popup), new Telerik.Windows.PropertyMetadata(true, new PropertyChangedCallback(Telerik.Windows.Controls.Primitives.Popup.OnPopupPropertyChanged)));
        public static readonly DependencyProperty VerticalOffsetProperty = DependencyProperty.Register("VerticalOffset", typeof(double), typeof(Telerik.Windows.Controls.Primitives.Popup), new Telerik.Windows.PropertyMetadata(0.0));
        private PopupWrapper wrapper;

        public event EventHandler ClickedOutsidePopup
        {
            add
            {
                this.wrapper.ClickedOutsidePopup += value;
            }
            remove
            {
                this.wrapper.ClickedOutsidePopup -= value;
            }
        }

        public event RoutedEventHandler Closed
        {
            add
            {
                this.AddHandler(ClosedEvent, value);
            }
            remove
            {
                this.RemoveHandler(ClosedEvent, value);
            }
        }

        public event RoutedEventHandler Opened
        {
            add
            {
                this.AddHandler(OpenedEvent, value);
            }
            remove
            {
                this.RemoveHandler(OpenedEvent, value);
            }
        }

        public Popup()
        {
            this.popup.Closed += new EventHandler(this.OnPopupClosed);
            this.wrapper = new PopupWrapper(this.popup, null);
            this.wrapper.Opened += new EventHandler(this.WrapperOpened);
            base.Loaded += new RoutedEventHandler(this.OnLoaded);
            base.Unloaded += new RoutedEventHandler(this.OnUnloaded);
        }

        public void AdjustLocation()
        {
            if (this.IsOpen && this.popup.IsOpen)
            {
                this.wrapper.AdjustPopupLocation();
            }
        }

        private SlideMode GetSlideMode()
        {
            switch (this.ActualPlacement)
            {
                case Telerik.Windows.Controls.PlacementMode.Absolute:
                case Telerik.Windows.Controls.PlacementMode.Center:
                case Telerik.Windows.Controls.PlacementMode.Right:
                case Telerik.Windows.Controls.PlacementMode.Left:
                case Telerik.Windows.Controls.PlacementMode.Top:
                    return SlideMode.Bottom;

                case Telerik.Windows.Controls.PlacementMode.Bottom:
                    return SlideMode.Top;
            }
            return SlideMode.Bottom;
        }

        private void HandleIsOpenChange()
        {
            if (this.isLoaded)
            {
                if (ApplicationHelper.GetRootVisual(this) == ApplicationHelper.GetRootVisual(this.Owner))
                {
                    if (this.IsOpen)
                    {
                        this.wrapper.ShowPopup();
                    }
                    else
                    {
                        AnimationManager.Stop(this.Owner, "Expand");
                        object[] slideData = new object[2];
                        slideData[1] = this.GetSlideMode();
                        AnimationManager.Play(this.Owner, "Collapse", delegate {
                            this.wrapper.HidePopup();
                        }, slideData);
                    }
                }
                else
                {
                    this.wrapper.HidePopup();
                    this.IsOpen = false;
                    base.ClearValue(IsOpenProperty);
                }
            }
        }

        private void InvlidateWrapperProperties()
        {
            this.wrapper.ClipAroundElement = this.ClipAroundElement;
            this.wrapper.PlacementTarget = this.PlacementTarget;
            this.wrapper.PlacementRectangle = this.PlacementRectangle;
            this.wrapper.Placement = this.Placement;
            this.wrapper.UsePlacementTargetAsClipElement = this.UsePlacementTargetAsClipElement;
            this.wrapper.CloseOnOutsideClick = this.CloseOnOutsideClick;
            this.wrapper.CatchClickOutsidePopup = this.CatchClickOutsidePopup;
            this.wrapper.PopupOwner = this.Owner;
            this.wrapper.HorizontalOffset = this.HorizontalOffset;
            this.wrapper.VerticalOffset = this.VerticalOffset;
            this.AdjustLocation();
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
            if (!this.changingContent)
            {
                UIElement content = newContent as UIElement;
                if (content != null)
                {
                    try
                    {
                        this.changingContent = true;
                        base.Content = this.popup;
                        this.popup.Child = content;
                        return;
                    }
                    finally
                    {
                        this.changingContent = false;
                    }
                }
                this.popup.Child = null;
            }
        }

        private static void OnIsOpenPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Telerik.Windows.Controls.Primitives.Popup popup = d as Telerik.Windows.Controls.Primitives.Popup;
            if (popup != null)
            {
                popup.HandleIsOpenChange();
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.isLoaded = true;
            if (this.IsOpen)
            {
                this.HandleIsOpenChange();
            }
        }

        private void OnPopupClosed(object sender, EventArgs e)
        {
            this.IsOpen = false;
            this.popup.RaiseEvent(new RadRoutedEventArgs(ClosedEvent));
        }

        private static void OnPopupPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Telerik.Windows.Controls.Primitives.Popup popup = d as Telerik.Windows.Controls.Primitives.Popup;
            if (popup != null)
            {
                popup.InvlidateWrapperProperties();
            }
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            this.isLoaded = false;
            if (this.IsOpen)
            {
                this.HandleIsOpenChange();
            }
        }

        private void WrapperOpened(object sender, EventArgs e)
        {
            this.RaiseEvent(new RadRoutedEventArgs(OpenedEvent));
            AnimationManager.Stop(this.Owner, "Collapse");
            object[] playOptions = new object[2];
            playOptions[1] = this.GetSlideMode();
            AnimationManager.Play(this.Owner, "Expand", null, playOptions);
        }

        private Telerik.Windows.Controls.PlacementMode ActualPlacement
        {
            get
            {
                return this.wrapper.ActtualPlacement;
            }
        }

        public bool CatchClickOutsidePopup
        {
            get
            {
                return (bool) base.GetValue(CatchClickOutsidePopupProperty);
            }
            set
            {
                base.SetValue(CatchClickOutsidePopupProperty, value);
            }
        }

        public FrameworkElement ClipAroundElement
        {
            get
            {
                return (FrameworkElement) base.GetValue(ClipAroundElementProperty);
            }
            set
            {
                base.SetValue(ClipAroundElementProperty, value);
            }
        }

        public bool CloseOnOutsideClick
        {
            get
            {
                return (bool) base.GetValue(CloseOnOutsideClickProperty);
            }
            set
            {
                base.SetValue(CloseOnOutsideClickProperty, value);
            }
        }

        public double HorizontalOffset
        {
            get
            {
                return (double) base.GetValue(HorizontalOffsetProperty);
            }
            set
            {
                base.SetValue(HorizontalOffsetProperty, value);
            }
        }

        public bool IsOpen
        {
            get
            {
                return (bool) base.GetValue(IsOpenProperty);
            }
            set
            {
                base.SetValue(IsOpenProperty, value);
            }
        }

        public FrameworkElement Owner
        {
            get
            {
                return (FrameworkElement) base.GetValue(OwnerProperty);
            }
            set
            {
                base.SetValue(OwnerProperty, value);
            }
        }

        public Telerik.Windows.Controls.PlacementMode Placement
        {
            get
            {
                return (Telerik.Windows.Controls.PlacementMode) base.GetValue(PlacementProperty);
            }
            set
            {
                base.SetValue(PlacementProperty, value);
            }
        }

        public Rect PlacementRectangle
        {
            get
            {
                return (Rect) base.GetValue(PlacementRectangleProperty);
            }
            set
            {
                base.SetValue(PlacementRectangleProperty, value);
            }
        }

        public UIElement PlacementTarget
        {
            get
            {
                return (UIElement) base.GetValue(PlacementTargetProperty);
            }
            set
            {
                base.SetValue(PlacementTargetProperty, value);
            }
        }

        public System.Windows.Controls.Primitives.Popup RealPopup
        {
            get
            {
                return this.popup;
            }
        }

        public bool UsePlacementTargetAsClipElement
        {
            get
            {
                return (bool) base.GetValue(UsePlacementTargetAsClipElementProperty);
            }
            set
            {
                base.SetValue(UsePlacementTargetAsClipElementProperty, value);
            }
        }

        public double VerticalOffset
        {
            get
            {
                return (double) base.GetValue(VerticalOffsetProperty);
            }
            set
            {
                base.SetValue(VerticalOffsetProperty, value);
            }
        }
    }
}

