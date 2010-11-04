namespace Telerik.Windows.Controls
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls.Primitives;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;
    using Telerik.Windows;
    using Telerik.Windows.Input;

    internal class PopupWrapper
    {
        private Storyboard openState;
        private PlacementHelper placementHelper;
        private Popup popup;
        private static WeakReferenceList<Popup> registeredPopups;
        private Popup rootPopup;
        private Rectangle rootRectangle;

        public event EventHandler ClickedOutsidePopup;

        public event EventHandler Opened;

        public PopupWrapper(Popup popupControl, FrameworkElement popupOwner)
        {
            this.UsePlacementTargetAsClipElement = true;
            this.rootPopup = new Popup();
            this.popup = popupControl;
            this.PopupOwner = popupOwner;
            this.Placement = Telerik.Windows.Controls.PlacementMode.Bottom;
            this.PlacementRectangle = Rect.Empty;
            this.rootRectangle = new Rectangle();
            this.rootRectangle.Fill = new SolidColorBrush(Colors.Transparent);
            this.rootPopup.Child = this.rootRectangle;
            Mouse.AddMouseDownHandler(this.rootRectangle, new EventHandler<MouseButtonEventArgs>(this.OnRectMouseLeftButtonDown));
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        internal void AdjustPopupLocation()
        {
            Rect rect = new Rect();
            Point topLeft = new Point();
            this.popup.UpdateLayout();
            double zoomFactor = ApplicationHelper.ZoomFactor;
            FlowDirection flowDirection = (this.PopupOwner != null) ? this.PopupOwner.FlowDirection : (this.popup.Child as FrameworkElement).FlowDirection;
            if (!this.PlacementRectangle.IsEmpty)
            {
                rect = this.PlacementRectangle;
            }
            else
            {
                UIElement element = this.PopupOwner;
                if (this.PlacementTarget != null)
                {
                    element = this.PlacementTarget;
                }
                try
                {
                    topLeft = element.TransformToVisual(null).Transform(topLeft);
                    if (ApplicationHelper.RootVisual.IsAncestorOf(element))
                    {
                        topLeft.X /= zoomFactor;
                        topLeft.Y /= zoomFactor;
                    }
                }
                catch
                {
                }
                rect = new Rect(topLeft, element.RenderSize);
            }
            this.placementHelper = new PlacementHelper(rect, this.popup.Child.RenderSize, this.HorizontalOffset, this.VerticalOffset, flowDirection);
            Point offset = this.placementHelper.GetPlacementOrigin(this.Placement);
            this.ActtualPlacement = this.placementHelper.ActualPlacement;
            if (!this.PlacementRectangle.IsEmpty || (this.PlacementTarget != null))
            {
                Point popupGlobalOffset = new Point();
                if (PopupIsInTheVisualTree(this.popup))
                {
                    try
                    {
                        popupGlobalOffset = this.popup.TransformToVisual(null).Transform(new Point());
                    }
                    catch
                    {
                    }
                    this.popup.HorizontalOffset = offset.X - popupGlobalOffset.X;
                    this.popup.VerticalOffset = offset.Y - popupGlobalOffset.Y;
                }
                else
                {
                    this.popup.HorizontalOffset = offset.X;
                    this.popup.VerticalOffset = offset.Y;
                }
            }
            else
            {
                this.popup.HorizontalOffset = offset.X - topLeft.X;
                this.popup.VerticalOffset = offset.Y - topLeft.Y;
            }
        }

        internal void ClearReferences()
        {
            Mouse.RemoveMouseDownHandler(this.rootRectangle, new EventHandler<MouseButtonEventArgs>(this.OnRectMouseLeftButtonDown));
            this.rootRectangle.Fill = null;
            this.rootPopup.Child = null;
            this.CloseOnOutsideClick = false;
            this.CatchClickOutsidePopup = false;
            this.ClipAroundElement = null;
            this.PopupOwner = null;
            this.popup = null;
        }

        private void CloseRootPopup()
        {
            ApplicationHelper.RemoveResizedHandler(new EventHandler(this.OnRootSizeChanged));
            ApplicationHelper.RemoveZoomedHandler(new EventHandler(this.OnRootSizeChanged));
            if (this.ShowRectangle)
            {
                this.rootPopup.IsOpen = false;
                Unregister(this.rootPopup);
            }
            this.popup.Closed -= new EventHandler(this.OnPopupClosed);
        }

        private void CreateOpenStateAnimation()
        {
            this.openState = new Storyboard();
            DoubleAnimation da = new DoubleAnimation();
            Storyboard.SetTarget(da, this.popup.Child);
            Storyboard.SetTargetProperty(da, new PropertyPath("Opacity", new object[0]));
            da.From = 0.0;
            da.To = 1.0;
            da.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 130));
            this.openState.Children.Add(da);
        }

        private Geometry GenerateClippingRegion()
        {
            if (!this.CatchClickOutsidePopup && !this.CloseOnOutsideClick)
            {
                return null;
            }
            UIElement rootVisual = Application.Current.RootVisual;
            UIElement clipAround = this.PopupOwner;
            if (this.ClipAroundElement != null)
            {
                clipAround = this.ClipAroundElement;
            }
            else if ((this.PlacementTarget != null) && this.UsePlacementTargetAsClipElement)
            {
                clipAround = this.PlacementTarget;
            }
            GeneralTransform transform = null;
            try
            {
                transform = clipAround.TransformToVisual(ApplicationHelper.RootVisual);
            }
            catch (ArgumentException)
            {
                return new GeometryGroup();
            }
            Point topLeft = transform.Transform(new Point(0.0, 0.0));
            Point topRight = transform.Transform(new Point(clipAround.RenderSize.Width, 0.0));
            Point bottomLeft = transform.Transform(new Point(0.0, clipAround.RenderSize.Height));
            Point bottomRight = transform.Transform(new Point(clipAround.RenderSize.Width, clipAround.RenderSize.Height));
            double zoomFactor = ApplicationHelper.ZoomFactor;
            topLeft.X /= zoomFactor;
            topLeft.Y /= zoomFactor;
            topRight.X /= zoomFactor;
            topRight.Y /= zoomFactor;
            bottomLeft.X /= zoomFactor;
            bottomLeft.Y /= zoomFactor;
            bottomRight.X /= zoomFactor;
            bottomRight.Y /= zoomFactor;
            if (!this.PlacementRectangle.IsEmpty)
            {
                topLeft.X = this.PlacementRectangle.Left;
                topLeft.Y = this.PlacementRectangle.Top;
                topRight.X = this.PlacementRectangle.Right;
                topRight.Y = this.PlacementRectangle.Top;
                bottomLeft.X = this.PlacementRectangle.Left;
                bottomLeft.Y = this.PlacementRectangle.Bottom;
                bottomRight.X = this.PlacementRectangle.Right;
                bottomRight.Y = this.PlacementRectangle.Bottom;
            }
            GeometryGroup geometryGroup = new GeometryGroup();
            RectangleGeometry rectangleGeometry = new RectangleGeometry();
            Rect rect = new Rect {
                X = 0.0,
                Y = 0.0,
                Width = Math.Max(0.0, SilvelrightActualWidth),
                Height = Math.Max(0.0, topLeft.Y)
            };
            rectangleGeometry.Rect = rect;
            geometryGroup.Children.Add(rectangleGeometry);
            rectangleGeometry = new RectangleGeometry();
            rect = new Rect {
                X = 0.0,
                Y = bottomLeft.Y,
                Width = Math.Max(0.0, SilvelrightActualWidth),
                Height = Math.Max((double) 0.0, (double) (SilvelrightActualHeight - bottomLeft.Y))
            };
            rectangleGeometry.Rect = rect;
            geometryGroup.Children.Add(rectangleGeometry);
            rectangleGeometry = new RectangleGeometry();
            rect = new Rect {
                X = 0.0,
                Y = topLeft.Y,
                Width = Math.Max(0.0, topLeft.X),
                Height = Math.Max((double) 0.0, (double) (bottomLeft.Y - topLeft.Y))
            };
            rectangleGeometry.Rect = rect;
            geometryGroup.Children.Add(rectangleGeometry);
            rectangleGeometry = new RectangleGeometry();
            rect = new Rect {
                X = topRight.X,
                Y = topRight.Y,
                Width = Math.Max((double) 0.0, (double) (SilvelrightActualWidth - topRight.X)),
                Height = Math.Max((double) 0.0, (double) (bottomRight.Y - topRight.Y))
            };
            rectangleGeometry.Rect = rect;
            geometryGroup.Children.Add(rectangleGeometry);
            return geometryGroup;
        }

        internal void HidePopup()
        {
            Unregister(this.popup);
            this.CloseRootPopup();
            PopupManager.Close(this.popup, PopupType.Popup, false);
        }

        private void OnClickedOutsidePopup()
        {
            if (this.ClickedOutsidePopup != null)
            {
                this.ClickedOutsidePopup(this, EventArgs.Empty);
            }
            if (this.CloseOnOutsideClick)
            {
                this.HidePopup();
            }
        }

        private void OnPopupClosed(object sender, EventArgs e)
        {
            this.HidePopup();
        }

        private void OnPopupOpened(object sender, EventArgs e)
        {
            this.popup.Opened -= new EventHandler(this.OnPopupOpened);
            if (this.popup.IsOpen)
            {
                if (this.ShowRectangle)
                {
                    this.rootRectangle.Clip = this.GenerateClippingRegion();
                }
                this.AdjustPopupLocation();
                this.popup.Dispatcher.BeginInvoke(delegate {
                    if ((this.popup != null) && (this.popup.Child != null))
                    {
                        this.popup.Child.IsHitTestVisible = true;
                    }
                });
                if (this.openState == null)
                {
                    this.CreateOpenStateAnimation();
                }
                this.openState.Stop();
                this.openState.Begin();
                if (this.Opened != null)
                {
                    this.Opened(this, EventArgs.Empty);
                }
            }
            else
            {
                this.HidePopup();
            }
        }

        private void OnRectMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.OnClickedOutsidePopup();
            e.Handled = true;
        }

        private void OnRootSizeChanged(object sender, EventArgs e)
        {
            this.rootRectangle.Width = SilvelrightActualWidth;
            this.rootRectangle.Height = SilvelrightActualHeight;
            this.rootRectangle.Clip = this.GenerateClippingRegion();
            this.AdjustPopupLocation();
        }

        private static bool PopupIsInTheVisualTree(Popup popup)
        {
            if (popup.Parent == null)
            {
                return (VisualTreeHelper.GetParent(popup) != null);
            }
            return true;
        }

        private static void Register(Popup popup)
        {
            if (!RegisteredPopups.Contains(popup))
            {
                RegisteredPopups.Add(popup);
            }
        }

        internal void ShowPopup()
        {
            ApplicationHelper.AddResizedHandler(new EventHandler(this.OnRootSizeChanged));
            ApplicationHelper.AddZoomedHandler(new EventHandler(this.OnRootSizeChanged));
            if (this.ShowRectangle)
            {
                this.rootRectangle.Width = SilvelrightActualWidth;
                this.rootRectangle.Height = SilvelrightActualHeight;
                this.rootPopup.IsOpen = true;
                Register(this.rootPopup);
            }
            Register(this.popup);
            this.popup.Closed += new EventHandler(this.OnPopupClosed);
            if (this.popup.Child != null)
            {
                this.popup.Child.IsHitTestVisible = false;
                this.popup.Child.Opacity = 0.0;
            }
            this.popup.Opened += new EventHandler(this.OnPopupOpened);
            PopupManager.Open(this.popup, PopupType.Popup, false);
        }

        private static void Unregister(Popup popup)
        {
            RegisteredPopups.Remove(popup);
        }

        internal Telerik.Windows.Controls.PlacementMode ActtualPlacement { get; set; }

        public bool CatchClickOutsidePopup { get; set; }

        public FrameworkElement ClipAroundElement { get; set; }

        public bool CloseOnOutsideClick { get; set; }

        public double HorizontalOffset { get; set; }

        public Telerik.Windows.Controls.PlacementMode Placement { get; set; }

        public Rect PlacementRectangle { get; set; }

        public UIElement PlacementTarget { get; set; }

        public FrameworkElement PopupOwner { get; set; }

        internal static WeakReferenceList<Popup> RegisteredPopups
        {
            get
            {
                if (registeredPopups == null)
                {
                    registeredPopups = new WeakReferenceList<Popup>();
                }
                return registeredPopups;
            }
        }

        private bool ShowRectangle
        {
            get
            {
                if (!this.CatchClickOutsidePopup)
                {
                    return this.CloseOnOutsideClick;
                }
                return true;
            }
        }

        private static double SilvelrightActualHeight
        {
            get
            {
                return ApplicationHelper.ApplicationSize.Height;
            }
        }

        private static double SilvelrightActualWidth
        {
            get
            {
                return ApplicationHelper.ApplicationSize.Width;
            }
        }

        public bool UsePlacementTargetAsClipElement { get; set; }

        public double VerticalOffset { get; set; }
    }
}

