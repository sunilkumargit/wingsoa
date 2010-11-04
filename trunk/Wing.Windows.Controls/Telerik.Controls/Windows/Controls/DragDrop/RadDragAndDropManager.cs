namespace Telerik.Windows.Controls.DragDrop
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Shapes;
    using System.Windows.Threading;
    using Telerik.Windows;
    using Telerik.Windows.Controls;
    using Telerik.Windows.Controls.Common;

    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "It may be decoupled later.")]
    public static class RadDragAndDropManager
    {
        public static readonly DependencyProperty AllowDragProperty = DependencyProperty.RegisterAttached("AllowDrag", typeof(bool), typeof(RadDragAndDropManager), new Telerik.Windows.PropertyMetadata(false, new PropertyChangedCallback(RadDragAndDropManager.OnAllowDragChanged)));
        public static readonly DependencyProperty AllowDropProperty = DependencyProperty.RegisterAttached("AllowDrop", typeof(bool), typeof(RadDragAndDropManager), new Telerik.Windows.PropertyMetadata(false, null));
        private static Popup coverPopup = new Popup();
        private static WeakReference coverRectangle = new WeakReference(null);
        private static Popup dragCuePopup = new Popup();
        public static readonly Telerik.Windows.RoutedEvent DragInfoEvent = EventManager.RegisterRoutedEvent("DragInfo", RoutingStrategy.Bubble, typeof(EventHandler<DragDropEventArgs>), typeof(RadDragAndDropManager));
        public static readonly Telerik.Windows.RoutedEvent DragQueryEvent = EventManager.RegisterRoutedEvent("DragQuery", RoutingStrategy.Bubble, typeof(EventHandler<DragDropQueryEventArgs>), typeof(RadDragAndDropManager));
        private static double dragStartThreshold = 4.0;
        public static readonly Telerik.Windows.RoutedEvent DropInfoEvent = EventManager.RegisterRoutedEvent("DropInfo", RoutingStrategy.Bubble, typeof(EventHandler<DragDropEventArgs>), typeof(RadDragAndDropManager));
        public static readonly Telerik.Windows.RoutedEvent DropQueryEvent = EventManager.RegisterRoutedEvent("DropQuery", RoutingStrategy.Bubble, typeof(EventHandler<DragDropQueryEventArgs>), typeof(RadDragAndDropManager));
        internal static readonly DependencyProperty HideDuringLayoutChangeProperty = DependencyProperty.RegisterAttached("HideDuringLayoutChange", typeof(bool), typeof(RadDragAndDropManager), new Telerik.Windows.PropertyMetadata(false, new PropertyChangedCallback(HideDuringLayoutChangeHelper.OnHideDuringLayoutChangeChanged)));
        private static bool isDragCuePositioned;
        private static bool isInitialized;
        private static bool isMouseDown;
        private static FrameworkElement lastApprovedDestination;
        private static Point lastDropTargetMovePoint;
        private static Point mouseClickPoint;
        private static FrameworkElement previousDropDestination;
        private static Point previousScrollAdjustPosition;
        private static Point relativeCLick;
        private static WeakReference rootVisual = new WeakReference(null);
        private static DispatcherTimer scrollViewerScrollTimer;
        private static IList<ScrollViewer> scrollViewersToAdjust;

        public static void AddDragInfoHandler(DependencyObject target, EventHandler<DragDropEventArgs> handler)
        {
            (target as UIElement).AddHandler(DragInfoEvent, handler);
        }

        public static void AddDragQueryHandler(DependencyObject target, EventHandler<DragDropQueryEventArgs> handler)
        {
            (target as UIElement).AddHandler(DragQueryEvent, handler);
        }

        public static void AddDropInfoHandler(DependencyObject target, EventHandler<DragDropEventArgs> handler)
        {
            (target as UIElement).AddHandler(DropInfoEvent, handler);
        }

        public static void AddDropQueryHandler(DependencyObject target, EventHandler<DragDropQueryEventArgs> handler)
        {
            (target as UIElement).AddHandler(DropQueryEvent, handler);
        }

        private static void AddRootVisualHandlers(FrameworkElement root)
        {
            if (root != null)
            {
                root.KeyDown += new KeyEventHandler(RadDragAndDropManager.OnRootVisualKeyDown);
                root.KeyUp += new KeyEventHandler(RadDragAndDropManager.OnRootVisualKeyUp);
                root.AddHandler(UIElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(RadDragAndDropManager.OnCoverRectangleMouseLeftButtonUp), true);
                root.MouseMove += new MouseEventHandler(RadDragAndDropManager.OnCoverRectangleMouseMove);
            }
        }

        private static void AdjustScrollViewer(ScrollViewer viewer, Point currentPoint)
        {
            Point p = currentPoint;
            Point topLeft = viewer.TransformToVisual(RootVisual).Transform(new Point(0.0, 0.0));
            Point relative = new Point(p.X - topLeft.X, p.Y - topLeft.Y);
            if ((relative.Y > 0.0) && (relative.Y < 40.0))
            {
                viewer.ScrollToVerticalOffset(viewer.VerticalOffset - (20.0 * ((40.0 - relative.Y) / 40.0)));
            }
            if ((relative.Y > (viewer.ActualHeight - 40.0)) && (relative.Y < viewer.ActualHeight))
            {
                viewer.ScrollToVerticalOffset(viewer.VerticalOffset + (20.0 * ((40.0 - (viewer.ActualHeight - relative.Y)) / 40.0)));
            }
            if ((relative.X > 0.0) && (relative.X < 40.0))
            {
                viewer.ScrollToHorizontalOffset(viewer.HorizontalOffset - (20.0 * ((40.0 - relative.X) / 40.0)));
            }
            if ((relative.X > (viewer.ActualWidth - 40.0)) && (relative.X < viewer.ActualWidth))
            {
                viewer.ScrollToHorizontalOffset(viewer.HorizontalOffset + (20.0 * ((40.0 - (viewer.ActualWidth - relative.X)) / 40.0)));
            }
        }

        private static bool ArePointsNear(Point currentPoint, Point mouseClickPoint)
        {
            return ArePointsNear(currentPoint, mouseClickPoint, DragStartThreshold);
        }

        private static bool ArePointsNear(Point currentPoint, Point mouseClickPoint, double threshold)
        {
            return ((Math.Abs((double)(currentPoint.X - mouseClickPoint.X)) < threshold) && (Math.Abs((double)(currentPoint.Y - mouseClickPoint.Y)) < threshold));
        }

        public static void CancelDrag()
        {
            if (IsDragging)
            {
                CancelDragging();
            }
            else
            {
                ResetEverything();
            }
        }

        private static void CancelDragging()
        {
            IsDragging = false;
            NotifyPreviousApprovedDestination();
            PopupManager.Close(dragCuePopup, PopupType.DockWindow);
            PopupManager.Close(coverPopup, PopupType.DockWindow);
            if (Options.SourceCueHostRollback != null)
            {
                DetachFromParent(Options.SourceCueHost);
                Options.SourceCueHostRollback.Execute();
            }
            if (Options.SourceCueRollback != null)
            {
                DetachFromParent(Options.SourceCue);
                Options.SourceCueRollback.Execute();
            }
            if (Options.DragCueRollback != null)
            {
                Options.DragCueHost.Content = null;
                Options.DragCueRollback.Execute();
            }
            Options.Status = DragStatus.DragCancel;
            RaiseDragInfo();
            Options.Status = DragStatus.None;
            ResetEverything();
        }

        private static void DetachFromParent(FrameworkElement target)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(target);
            ContentControl contentControl = parent as ContentControl;
            if (contentControl != null)
            {
                contentControl.Content = null;
            }
            else if (parent is ContentPresenter)
            {
                ContentControl contentConrol = target.Parent as ContentControl;
                contentConrol.Content = null;
            }
            else
            {
                Panel panel = parent as Panel;
                if (panel != null)
                {
                    panel.Children.Remove(target);
                }
            }
        }

        private static void FindDropZones(out IList<FrameworkElement> dropZones, ref IList<ScrollViewer> scrollViewers)
        {
            List<UIElement> elements = new List<UIElement>(100);
            if (Options.ParticipatingVisualRoots != null)
            {
                foreach (UIElement root in Options.ParticipatingVisualRoots)
                {
                    elements.AddRange(VisualTreeHelper.FindElementsInHostCoordinates(Options.CurrentDragPoint, root));
                }
            }
            elements.AddRange(VisualTreeHelper.FindElementsInHostCoordinates(Options.CurrentDragPoint, RootVisual));
            List<FrameworkElement> results = new List<FrameworkElement>(4);
            scrollViewers = new List<ScrollViewer>(3);
            foreach (UIElement element in elements)
            {
                if (GetAllowDrop(element))
                {
                    results.Add(element as FrameworkElement);
                }
                ScrollViewer viewer = element as ScrollViewer;
                if (AutoBringIntoView && (viewer != null))
                {
                    scrollViewers.Add(viewer);
                }
                if (element.GetType().Name == "RadWindow")
                {
                    break;
                }
            }
            dropZones = results;
        }

        private static void FindRootVisual()
        {
            if (RootVisual == null)
            {
                RootVisual = Application.Current.RootVisual as FrameworkElement;
            }
        }

        public static ContentControl GenerateArrowCue()
        {
            DragArrow dragArrowElement = new DragArrow
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Height = 30.0
            };
            TransformGroup group = new TransformGroup
            {
                Children = { new ScaleTransform(), new RotateTransform(), new TranslateTransform() }
            };
            dragArrowElement.RenderTransform = group;
            dragArrowElement.Padding = new Thickness(30.0, 0.0, 30.0, 0.0);
            dragArrowElement.RenderTransformOrigin = new Point(0.0, 0.5);
            return dragArrowElement;
        }

        public static DragVisualCue GenerateVisualCue()
        {
            return GenerateVisualCue(null);
        }

        public static DragVisualCue GenerateVisualCue(FrameworkElement source)
        {
            DragVisualCue result = new DragVisualCue();
            if (source != null)
            {
                result.Width = source.ActualWidth;
                result.Height = source.ActualHeight;
                result.MinWidth = source.MinWidth;
                result.MaxWidth = source.MaxWidth;
                result.MinHeight = source.MinHeight;
                result.MaxHeight = source.MaxHeight;
                result.Margin = source.Margin;
            }
            return result;
        }

        public static bool GetAllowDrag(DependencyObject obj)
        {
            return (bool)obj.GetValue(AllowDragProperty);
        }

        public static bool GetAllowDrop(DependencyObject obj)
        {
            return (bool)obj.GetValue(AllowDropProperty);
        }

        private static DragDropOptions GetDefaultOptions()
        {
            return new DragDropOptions();
        }

        internal static bool GetHideDuringLayoutChange(DependencyObject obj)
        {
            return (bool)obj.GetValue(HideDuringLayoutChangeProperty);
        }

        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "sourceElement")]
        private static void Initialize(FrameworkElement sourceElement)
        {
            isInitialized = true;
            ArrowVisibilityMinimumThreshold = 60.0;
            isDragCuePositioned = false;
            if (!RadControl.IsInDesignMode)
            {
                AutoBringIntoView = true;
                Options = GetDefaultOptions();
                FindRootVisual();
                Deployment.Current.Dispatcher.BeginInvoke(new Action(RadDragAndDropManager.FindRootVisual));
                Deployment.Current.Dispatcher.BeginInvoke(new Action(RadDragAndDropManager.FindRootVisual));
                CoverRectangle = new Grid { Background = new SolidColorBrush { Color = Color.FromArgb(0, 100, 0, 0) } };
                CoverRectangle.Opacity = 1.0;
                coverPopup.Child = CoverRectangle;
                CoverRectangle.MouseMove += new MouseEventHandler(RadDragAndDropManager.OnCoverRectangleMouseMove);
                CoverRectangle.MouseLeftButtonUp += new MouseButtonEventHandler(RadDragAndDropManager.OnCoverRectangleMouseLeftButtonUp);
                if (RootVisual != null)
                {
                    CoverRectangle.Width = RootVisual.ActualWidth;
                    CoverRectangle.Height = RootVisual.ActualHeight;
                }
                scrollViewerScrollTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(0.02) };
                scrollViewerScrollTimer.Tick += new EventHandler(RadDragAndDropManager.OnScrollViewerScrollTimerCompleted);
            }
        }

        private static void NotifyDestination(FrameworkElement destination)
        {
            if (destination != null)
            {
                Options.Status = DragStatus.DropImpossible;
                FrameworkElement originalDestination = Options.Destination;
                DragStatus originalStatus = Options.Status;
                Options.Destination = destination;
                RaiseDropInfo();
                RaiseDragInfo();
                Options.Destination = originalDestination;
                if ((originalDestination != null) && (originalDestination != destination))
                {
                    Options.Status = originalStatus;
                    RaiseDropInfo();
                }
            }
        }

        private static void NotifyPreviousApprovedDestination()
        {
            if (lastApprovedDestination != null)
            {
                NotifyDestination(lastApprovedDestination);
                lastApprovedDestination = null;
            }
        }

        internal static void OnAllowDragChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!RadControl.IsInDesignMode)
            {
                if (!isInitialized)
                {
                    Initialize(sender as FrameworkElement);
                }
                UIElement element = sender as UIElement;
                if ((bool)e.NewValue)
                {
                    element.LostMouseCapture += new MouseEventHandler(RadDragAndDropManager.OnSourceLostMouseCapture);
                    element.AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(RadDragAndDropManager.OnTrackedPartMouseLeftButtonDown), true);
                }
                else
                {
                    element.LostMouseCapture -= new MouseEventHandler(RadDragAndDropManager.OnSourceLostMouseCapture);
                    element.RemoveHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(RadDragAndDropManager.OnTrackedPartMouseLeftButtonDown));
                }
            }
        }

        private static void OnCoverRectangleMouseLeftButtonUp(object sender, EventArgs e)
        {
            OnCoverRectangleMouseLeftButtonUpInternal();
        }

        internal static void OnCoverRectangleMouseLeftButtonUpInternal()
        {
            isMouseDown = false;
            if (Options.Status != DragStatus.DropPossible)
            {
                if (IsDragging)
                {
                    CancelDragging();
                }
                else
                {
                    ResetEverything();
                }
            }
            else
            {
                OnDrop();
            }
        }

        private static void OnCoverRectangleMouseMove(object sender, MouseEventArgs e)
        {
            if (IsMouseDown)
            {
                if (IsDragging)
                {
                    OnRealDrag(new TestableMouseEventArgs(e));
                }
                else
                {
                    OnTrackedElementMouseMoveInternal(new TestableMouseEventArgs(e));
                }
            }
        }

        internal static void OnCoverRectangleMouseMoveInternal(IMouseEventArgs e)
        {
            if (IsMouseDown)
            {
                if (IsDragging)
                {
                    OnRealDrag(e);
                }
                else
                {
                    OnTrackedElementMouseMoveInternal(e);
                }
            }
        }

        private static void OnDrop()
        {
            if (Options.SourceCueHostRollback != null)
            {
                DetachFromParent(Options.SourceCueHost);
                Options.SourceCueHostRollback.Execute();
            }
            if (Options.SourceCueRollback != null)
            {
                DetachFromParent(Options.SourceCue);
                Options.SourceCueRollback.Execute();
            }
            if (Options.DragCueRollback != null)
            {
                Options.DragCueHost.Content = null;
                Options.DragCueRollback.Execute();
            }
            Options.Status = DragStatus.DragComplete;
            RaiseDragInfo();
            Options.Status = DragStatus.DropComplete;
            RaiseDropInfo();
            IsDragging = false;
            PopupManager.Close(dragCuePopup, PopupType.DockWindow);
            PopupManager.Close(coverPopup, PopupType.DockWindow);
            Options.Status = DragStatus.None;
            ResetEverything();
        }

        private static void OnDropImpossible()
        {
            NotifyDestination(previousDropDestination);
            previousDropDestination = null;
        }

        private static void OnDropPossible()
        {
            if ((Options.Destination != lastApprovedDestination) && (lastApprovedDestination != null))
            {
                NotifyPreviousApprovedDestination();
            }
            if (Options.DestinationCueHost != null)
            {
                DeferredActionBase destinationCueHostRollback;
                DeferredActionBase destinationCueRollback;
                TemporaryReplaceElementInVisualTree(Options.DestinationCueHost, Options.DestinationCue, out destinationCueHostRollback, out destinationCueRollback);
                Options.DestinationCueHostRollback = destinationCueHostRollback;
                Options.DestinationCueRollback = destinationCueRollback;
            }
            lastApprovedDestination = Options.Destination;
        }

        private static void OnDropTargetMouseMove(object sender, MouseEventArgs e)
        {
            Point currentPoint = e.GetPosition(null);
            if (currentPoint != lastDropTargetMovePoint)
            {
                lastDropTargetMovePoint = currentPoint;
            }
        }

        private static void OnRealDrag(IMouseEventArgs e)
        {
            IList<FrameworkElement> dropZones;
            DragStatus originalStatus = Options.Status;
            Options.CurrentDragPoint = e.GetPosition(null);
            PositionPopup(e);
            PositionArrow();
            FindDropZones(out dropZones, ref scrollViewersToAdjust);
            bool isDropPossible = false;
            foreach (FrameworkElement zone in dropZones)
            {
                Options.Status = DragStatus.DropDestinationQuery;
                Options.Destination = zone;
                Options.RelativeDragPoint = e.GetPosition(zone);
                bool? destinationApproval = RaiseDropQuery();
                bool? sourceApproval = null;
                previousDropDestination = Options.Destination;
                if (destinationApproval == true)
                {
                    Options.Status = DragStatus.DropSourceQuery;
                    sourceApproval = RaiseDragQuery();
                }
                if ((destinationApproval == true) && (sourceApproval == true))
                {
                    Options.Status = DragStatus.DropPossible;
                    RaiseDropInfo();
                    RaiseDragInfo();
                    OnDropPossible();
                    isDropPossible = true;
                    break;
                }
                Options.Status = DragStatus.DropImpossible;
                if (originalStatus != DragStatus.DropImpossible)
                {
                    OnDropImpossible();
                }
                if ((destinationApproval == false) || (sourceApproval == false))
                {
                    break;
                }
            }
            foreach (ScrollViewer viewer in scrollViewersToAdjust)
            {
                AdjustScrollViewer(viewer, Options.CurrentDragPoint);
            }
            if (scrollViewersToAdjust.Any<ScrollViewer>())
            {
                previousScrollAdjustPosition = Options.CurrentDragPoint;
                if (scrollViewerScrollTimer != null)
                {
                    scrollViewerScrollTimer.Start();
                }
            }
            else
            {
                scrollViewerScrollTimer.Stop();
            }
            if (!isDropPossible && (dropZones.Count == 0))
            {
                OnDropImpossible();
            }
        }

        private static void OnRootVisualKeyDown(object sender, KeyEventArgs e)
        {
            if (IsDragging)
            {
                if (e.Key == Key.Escape)
                {
                    CancelDragging();
                }
                e.Handled = true;
            }
        }

        private static void OnRootVisualKeyUp(object sender, KeyEventArgs e)
        {
            if (IsDragging)
            {
                e.Handled = true;
            }
        }

        private static void OnScrollViewerScrollTimerCompleted(object sender, EventArgs e)
        {
            if ((IsDragging && ArePointsNear(previousScrollAdjustPosition, Options.CurrentDragPoint, 5.0)) && (scrollViewersToAdjust != null))
            {
                foreach (ScrollViewer scrollViewer in scrollViewersToAdjust)
                {
                    if (scrollViewer != null)
                    {
                        AdjustScrollViewer(scrollViewer, previousScrollAdjustPosition);
                    }
                }
            }
            else
            {
                scrollViewerScrollTimer.Stop();
                scrollViewersToAdjust = null;
            }
        }

        private static void OnSourceLostMouseCapture(object sender, MouseEventArgs e)
        {
            if (IsDragging)
            {
                CancelDragging();
            }
            else
            {
                isMouseDown = false;
            }
        }

        private static void OnTrackedElementMouseLeave(object sender, MouseEventArgs e)
        {
            OnTrackedElementMouseLeaveInternal();
        }

        private static void OnTrackedElementMouseLeaveInternal()
        {
            if (!IsDragging)
            {
                ResetEverything();
            }
        }

        private static void OnTrackedElementMouseMove(object sender, MouseEventArgs e)
        {
            OnTrackedElementMouseMoveInternal(new TestableMouseEventArgs(e));
        }

        private static void OnTrackedElementMouseMoveInternal(IMouseEventArgs e)
        {
            if ((!IsDragging && IsMouseDown) && !IsDragging)
            {
                TryStartDrag(e);
            }
        }

        private static void OnTrackedPartMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OnTrackedPartMouseLeftButtonDownInternal(sender, new TestableMouseButtonEventArgs(e));
        }

        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "sender", Justification = "Used in the WPF version"), SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "GetPosition can throw unexpected errors")]
        internal static void OnTrackedPartMouseLeftButtonDownInternal(object sender, IMouseButtonEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            if (!isMouseDown)
            {
                element.MouseLeave += new MouseEventHandler(RadDragAndDropManager.OnTrackedElementMouseLeave);
                element.MouseMove += new MouseEventHandler(RadDragAndDropManager.OnTrackedElementMouseMove);
                element.AddHandler(UIElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(RadDragAndDropManager.OnTrackedPartMouseLeftButtonUp), true);
                isMouseDown = true;
                Options = GetDefaultOptions();
                Options.Source = element;
                try
                {
                    mouseClickPoint = e.GetPosition(null);
                    relativeCLick = e.GetPosition(e.OriginalSource as UIElement);
                }
                catch
                {
                    ResetEverything();
                }
                Options.CurrentDragPoint = mouseClickPoint;
            }
        }

        private static void OnTrackedPartMouseLeftButtonUp(object sender, EventArgs e)
        {
            OnCoverRectangleMouseLeftButtonUpInternal();
        }

        private static void PositionArrow()
        {
            if ((Options.ArrowCue != null) && (CoverRectangle != null))
            {
                ContentControl dragArrowElement = Options.ArrowCue;
                if (!CoverRectangle.Children.Contains(dragArrowElement))
                {
                    CoverRectangle.Children.Add(dragArrowElement);
                }
                double horizontalDif = mouseClickPoint.X - Options.CurrentDragPoint.X;
                double verticalDif = mouseClickPoint.Y - Options.CurrentDragPoint.Y;
                double distance = Math.Sqrt((horizontalDif * horizontalDif) + (verticalDif * verticalDif));
                if (distance > ArrowVisibilityMinimumThreshold)
                {
                    TransformGroup group = dragArrowElement.RenderTransform as TransformGroup;
                    ScaleTransform scale = group.Children[0] as ScaleTransform;
                    RotateTransform rotate = group.Children[1] as RotateTransform;
                    TranslateTransform translate = group.Children[2] as TranslateTransform;
                    translate.X = mouseClickPoint.X;
                    translate.Y = mouseClickPoint.Y - 15.0;
                    dragArrowElement.Width = distance;
                    if (horizontalDif != 0.0)
                    {
                        rotate.Angle = (Math.Atan(verticalDif / horizontalDif) * 180.0) / 3.1415926535897931;
                    }
                    else
                    {
                        rotate.Angle = (verticalDif < 0.0) ? ((double)90) : ((double)(-90));
                    }
                    if (horizontalDif > 0.0)
                    {
                        rotate.Angle += 180.0;
                        scale.ScaleY = -1.0;
                    }
                    else
                    {
                        scale.ScaleY = 1.0;
                    }
                    dragArrowElement.Visibility = Visibility.Visible;
                }
                else
                {
                    dragArrowElement.Visibility = Visibility.Collapsed;
                }
            }
        }

        private static void PositionPopup(IMouseEventArgs e)
        {
            FrameworkElement element = dragCuePopup.Child as FrameworkElement;
            if (element != null)
            {
                if ((!isDragCuePositioned && (element.ActualHeight != 0.0)) && (element.ActualWidth != 0.0))
                {
                    isDragCuePositioned = true;
                    element.Opacity = 1.0;
                    Point position = e.GetPosition(element);
                    if ((position.X < 0.0) || (position.X > element.ActualWidth))
                    {
                        relativeCLick.X = element.ActualWidth / 2.0;
                    }
                    if ((position.Y < 0.0) || (position.Y > element.ActualHeight))
                    {
                        relativeCLick.Y = element.ActualHeight / 2.0;
                    }
                }
                TranslateTransform renderTransform = element.RenderTransform as TranslateTransform;
                if (renderTransform == null)
                {
                    renderTransform = new TranslateTransform();
                    element.RenderTransform = renderTransform;
                }
                renderTransform.Y = Options.CurrentDragPoint.Y - relativeCLick.Y;
                renderTransform.X = Options.CurrentDragPoint.X - relativeCLick.X;
            }
        }

        private static void RaiseDragInfo()
        {
            if (Options.Source != null)
            {
                Options.Source.RaiseEvent(new DragDropEventArgs(DragInfoEvent, Options.Source, Options));
            }
        }

        private static bool? RaiseDragQuery()
        {
            if (Options.Source != null)
            {
                DragDropQueryEventArgs args = new DragDropQueryEventArgs(DragQueryEvent, Options.Source, Options);
                Options.Source.RaiseEvent(args);
                return args.QueryResult;
            }
            return null;
        }

        private static void RaiseDropInfo()
        {
            if (Options.Destination != null)
            {
                Options.Destination.RaiseEvent(new DragDropEventArgs(DropInfoEvent, Options.Destination, Options));
            }
        }

        private static bool? RaiseDropQuery()
        {
            if (Options.Destination != null)
            {
                DragDropQueryEventArgs args = new DragDropQueryEventArgs(DropQueryEvent, Options.Destination, Options);
                Options.Destination.RaiseEvent(args);
                return args.QueryResult;
            }
            return null;
        }

        public static void RemoveDragInfoHandler(DependencyObject target, EventHandler<DragDropEventArgs> handler)
        {
            (target as UIElement).RemoveHandler(DragInfoEvent, handler);
        }

        public static void RemoveDragQueryHandler(DependencyObject target, EventHandler<DragDropQueryEventArgs> handler)
        {
            (target as UIElement).RemoveHandler(DragQueryEvent, handler);
        }

        public static void RemoveDropInfoHandler(DependencyObject target, EventHandler<DragDropEventArgs> handler)
        {
            (target as UIElement).RemoveHandler(DropInfoEvent, handler);
        }

        public static void RemoveDropQueryHandler(DependencyObject target, EventHandler<DragDropQueryEventArgs> handler)
        {
            (target as UIElement).RemoveHandler(DropQueryEvent, handler);
        }

        private static void RemoveRootVisualHandlers(FrameworkElement root)
        {
            root.KeyDown -= new KeyEventHandler(RadDragAndDropManager.OnRootVisualKeyDown);
            root.KeyUp -= new KeyEventHandler(RadDragAndDropManager.OnRootVisualKeyUp);
            root.MouseMove -= new MouseEventHandler(RadDragAndDropManager.OnCoverRectangleMouseMove);
            root.RemoveHandler(UIElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(RadDragAndDropManager.OnCoverRectangleMouseLeftButtonUp));
        }

        private static void ResetEverything()
        {
            FrameworkElement trackedElement = Options.Source;
            if (trackedElement != null)
            {
                trackedElement.MouseMove -= new MouseEventHandler(RadDragAndDropManager.OnTrackedElementMouseMove);
                trackedElement.MouseLeave -= new MouseEventHandler(RadDragAndDropManager.OnTrackedElementMouseLeave);
                trackedElement.RemoveHandler(UIElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(RadDragAndDropManager.OnTrackedPartMouseLeftButtonUp));
            }
            if (Options.ArrowCue != null)
            {
                CoverRectangle.Children.Remove(Options.ArrowCue);
            }
            if (Options.ParticipatingVisualRoots != null)
            {
                foreach (UIElement item in Options.ParticipatingVisualRoots)
                {
                    if (item != null)
                    {
                        item.MouseMove -= new MouseEventHandler(RadDragAndDropManager.OnCoverRectangleMouseMove);
                    }
                }
                Options.ParticipatingVisualRoots = null;
            }
            Options = GetDefaultOptions();
            lastApprovedDestination = null;
            isMouseDown = false;
            isDragCuePositioned = false;
            dragCuePopup.Child = null;
        }

        public static void SetAllowDrag(DependencyObject obj, bool value)
        {
            obj.SetValue(AllowDragProperty, value);
        }

        public static void SetAllowDrop(DependencyObject obj, bool value)
        {
            obj.SetValue(AllowDropProperty, value);
        }

        internal static void SetHideDuringLayoutChange(DependencyObject obj, bool value)
        {
            obj.SetValue(HideDuringLayoutChangeProperty, value);
        }

        public static void StartDrag(FrameworkElement dragSource, object payload, object dragCue)
        {
            if (!IsDragging)
            {
                if (dragSource == null)
                {
                    throw new ArgumentNullException("dragSource");
                }
                if (!isInitialized)
                {
                    Initialize(dragSource);
                }
                Options.Source = dragSource;
                Options.Payload = payload;
                Options.DragCue = dragCue;
                StartDragging();
            }
        }

        private static void StartDragging()
        {
            IsDragging = true;
            Options.Status = DragStatus.DragInProgress;
            RaiseDragInfo();
            PopupManager.Open(coverPopup, PopupType.DockWindow);
            if (RootVisual == null)
            {
                FindRootVisual();
            }
            if (CoverRectangle != null)
            {
                CoverRectangle.Width = RootVisual.ActualWidth;
                CoverRectangle.Height = RootVisual.ActualHeight;
            }
            if (WindowsCollection != null)
            {
                foreach (IWindow item in (from win in WindowsCollection.OfType<IWindow>()
                                          orderby win.Z
                                          select win).Reverse<IWindow>())
                {
                    UIElement element = item as UIElement;
                    if ((element != null) && item.IsOpen)
                    {
                        Options.ParticipatingVisualRoots.Add(element);
                        element.MouseMove += new MouseEventHandler(RadDragAndDropManager.OnCoverRectangleMouseMove);
                    }
                }
            }
            if (Options.SourceCueHost != null)
            {
                DeferredActionBase sourceCueHostRollback;
                DeferredActionBase sourceCueRollback;
                TemporaryReplaceElementInVisualTree(Options.SourceCueHost, Options.SourceCue, out sourceCueHostRollback, out sourceCueRollback);
                Options.SourceCueHostRollback = sourceCueHostRollback;
                Options.SourceCueRollback = sourceCueRollback;
            }
            if (Options.DragCue != null)
            {
                Grid wrapper = new Grid
                {
                    RenderTransform = new TranslateTransform(),
                    Opacity = 0.0
                };
                dragCuePopup.Child = wrapper;
                FrameworkElement visualDragCue = Options.DragCue as FrameworkElement;
                if (visualDragCue != null)
                {
                    Options.DragCueRollback = TemporaryDetachFromParent(visualDragCue);
                }
                ContentControl dragCueHost = new ContentControl
                {
                    Content = Options.DragCue
                };
                wrapper.Children.Add(dragCueHost);
                Options.DragCueHost = dragCueHost;
                wrapper.Children.Add(new Rectangle { Fill = new SolidColorBrush { Color = Colors.Transparent } });
                PositionPopup(null);
                PopupManager.Open(dragCuePopup, PopupType.DockWindow);
                dragCuePopup.IsHitTestVisible = false;
                dragCuePopup.Child.IsHitTestVisible = false;
            }
        }

        private static DeferredActionBase TemporaryDetachFromParent(FrameworkElement target)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(target);
            ContentControl contentControl = parent as ContentControl;
            if (contentControl != null)
            {
                contentControl.Content = null;
                return new DeferredAction<ContentControl, FrameworkElement>(delegate(ContentControl defParent, FrameworkElement defTarget)
                {
                    if (defTarget.Parent == null)
                    {
                        defParent.Content = defTarget;
                    }
                }, contentControl, target);
            }
            Panel panel = parent as Panel;
            if (panel == null)
            {
                return null;
            }
            int oldIndex = panel.Children.IndexOf(target);
            panel.Children.Remove(target);
            return new DeferredAction<Panel, FrameworkElement, int>(delegate(Panel defParent, FrameworkElement defTarget, int defIndex)
            {
                if (defTarget.Parent == null)
                {
                    defParent.Children.Insert(defIndex, defTarget);
                }
            }, panel, target, oldIndex);
        }

        private static void TemporaryReplaceElementInVisualTree(FrameworkElement target, FrameworkElement replacement, out DeferredActionBase targetRollback, out DeferredActionBase replacementRollback)
        {
            replacementRollback = TemporaryDetachFromParent(replacement);
            DependencyObject parent = VisualTreeHelper.GetParent(target);
            if (parent == null)
            {
                targetRollback = null;
            }
            else
            {
                Panel panelParent = parent as Panel;
                if (panelParent == null)
                {
                    throw new NotImplementedException();
                }
                int replaceIndex = panelParent.Children.IndexOf(target);
                panelParent.Children[replaceIndex] = replacement;
                targetRollback = new DeferredAction<Panel, FrameworkElement, int>(delegate(Panel defPanel, FrameworkElement defTarget, int defIndex)
                {
                    defPanel.Children[defIndex] = defTarget;
                }, panelParent, target, replaceIndex);
            }
        }

        private static void TryStartDrag(IMouseEventArgs e)
        {
            if (!IsDragging)
            {
                isDragCuePositioned = false;
                Options.ParticipatingVisualRoots = new List<UIElement>(10);
                Options.CurrentDragPoint = e.GetPosition(null);
                if (!ArePointsNear(Options.CurrentDragPoint, mouseClickPoint))
                {
                    Options.MouseClickPoint = mouseClickPoint;
                    Options.SourceCue = GenerateVisualCue(Options.Source);
                    Options.ArrowCue = null;
                    Options.DragCue = null;
                    Options.Status = DragStatus.DragQuery;
                    FrameworkElement source = Options.Source;
                    if (RaiseDragQuery() == true)
                    {
                        StartDragging();
                    }
                }
            }
        }

        public static double ArrowVisibilityMinimumThreshold { get; set; }

        public static bool AutoBringIntoView { get; set; }

        private static Grid CoverRectangle
        {
            get
            {
                return (coverRectangle.Target as Grid);
            }
            set
            {
                coverRectangle = new WeakReference(value);
            }
        }

        public static double DragStartThreshold
        {
            get
            {
                return dragStartThreshold;
            }
            set
            {
                if (value < 0.0)
                {
                    throw new ArgumentOutOfRangeException("value", "DragStartThreshold cannot be smaller than 0");
                }
                dragStartThreshold = value;
            }
        }

        public static bool IsDragging { get; set; }

        internal static bool IsMouseDown
        {
            get
            {
                return isMouseDown;
            }
            set
            {
                isMouseDown = value;
            }
        }

        public static DragDropOptions Options { get; set; }

        internal static FrameworkElement RootVisual
        {
            get
            {
                return (rootVisual.Target as FrameworkElement);
            }
            set
            {
                if ((rootVisual.Target != null) && rootVisual.IsAlive)
                {
                    RemoveRootVisualHandlers(rootVisual.Target as FrameworkElement);
                }
                rootVisual = new WeakReference(value);
                if (value != null)
                {
                    AddRootVisualHandlers(value);
                }
            }
        }

        internal static IList WindowsCollection { get; set; }
    }
}

