namespace Telerik.Windows.Controls
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using System.Windows.Media;
    using Telerik.Windows.Input;

    public static class ScrollViewerExtensions
    {
        public static readonly DependencyProperty AttachedHorizontalScrollBarProperty = DependencyProperty.RegisterAttached("AttachedHorizontalScrollBar", typeof(ScrollBar), typeof(ScrollViewerExtensions), new PropertyMetadata(null, new PropertyChangedCallback(ScrollViewerExtensions.OnAttachedHorizontalScrollBarPropertyChanged)));
        public static readonly DependencyProperty AttachedVerticalScrollBarProperty = DependencyProperty.RegisterAttached("AttachedVerticalScrollBar", typeof(ScrollBar), typeof(ScrollViewerExtensions), new PropertyMetadata(null, new PropertyChangedCallback(ScrollViewerExtensions.OnAttachedVerticalScrollBarPropertyChanged)));
        public static readonly DependencyProperty EnableMouseWheelProperty = DependencyProperty.RegisterAttached("EnableMouseWheel", typeof(bool), typeof(ScrollViewerExtensions), new PropertyMetadata(new PropertyChangedCallback(ScrollViewerExtensions.OnEnableMouseWheelChanged)));

        public static ScrollBar GetAttachedHorizontalScrollBar(DependencyObject obj)
        {
            return (ScrollBar) obj.GetValue(AttachedHorizontalScrollBarProperty);
        }

        public static ScrollBar GetAttachedVerticalScrollBar(DependencyObject obj)
        {
            return (ScrollBar) obj.GetValue(AttachedVerticalScrollBarProperty);
        }

        public static bool GetEnableMouseWheel(DependencyObject obj)
        {
            return (bool) obj.GetValue(EnableMouseWheelProperty);
        }

        private static bool HandleMouseWheel(ScrollViewer scrollViewer, double scrollDelta)
        {
            if (scrollViewer != null)
            {
                double absoluteDelta = Math.Abs(scrollDelta);
                double newOffset = 0.0;
                if (scrollDelta > 0.0)
                {
                    newOffset = Math.Max((double) 0.0, (double) (scrollViewer.VerticalOffset - absoluteDelta));
                }
                else
                {
                    newOffset = Math.Min(scrollViewer.ScrollableHeight, scrollViewer.VerticalOffset + absoluteDelta);
                }
                if (newOffset != scrollViewer.VerticalOffset)
                {
                    scrollViewer.ScrollToVerticalOffset(newOffset);
                    return true;
                }
            }
            return false;
        }

        private static void OnAttachedHorizontalScrollBarPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScrollViewer scrollViewer = d as ScrollViewer;
            ScrollBar scrollBar = e.NewValue as ScrollBar;
            if (scrollBar != null)
            {
                scrollBar.ValueChanged += delegate (object s, RoutedPropertyChangedEventArgs<double> args) {
                    scrollViewer.ScrollToHorizontalOffset(args.NewValue);
                };
            }
            scrollViewer.ApplyTemplate();
            ScrollBar internalScrollBar = ((FrameworkElement) VisualTreeHelper.GetChild(scrollViewer, 0)).FindName("HorizontalScrollBar") as ScrollBar;
            internalScrollBar.ValueChanged += delegate (object s, RoutedPropertyChangedEventArgs<double> args) {
                scrollBar.Value = args.NewValue;
            };
        }

        private static void OnAttachedVerticalScrollBarPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScrollViewer scrollViewer = d as ScrollViewer;
            ScrollBar scrollBar = e.NewValue as ScrollBar;
            if (scrollBar != null)
            {
                scrollBar.ValueChanged += delegate (object s, RoutedPropertyChangedEventArgs<double> args) {
                    scrollViewer.ScrollToVerticalOffset(args.NewValue);
                };
            }
            scrollViewer.ApplyTemplate();
            ScrollBar internalScrollBar = ((FrameworkElement) VisualTreeHelper.GetChild(scrollViewer, 0)).FindName("VerticalScrollBar") as ScrollBar;
            internalScrollBar.ValueChanged += delegate (object s, RoutedPropertyChangedEventArgs<double> args) {
                if (scrollBar != null)
                {
                    scrollBar.Value = args.NewValue;
                }
            };
        }

        private static void OnEnableMouseWheelChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if (!RadControl.IsInDesignMode)
            {
                ScrollViewer scollViewer = d as ScrollViewer;
                if ((bool) args.NewValue)
                {
                    if (Mouse.IsMouseWheelSupported)
                    {
                        scollViewer.MouseWheel += new MouseWheelEventHandler(ScrollViewerExtensions.OnScrollViewerMouseWheel);
                    }
                    else
                    {
                        Mouse.AddMouseWheelHandler(scollViewer, new EventHandler<Telerik.Windows.Input.MouseWheelEventArgs>(ScrollViewerExtensions.OnScrollViewerMouseWheel));
                    }
                }
                else if (Mouse.IsMouseWheelSupported)
                {
                    scollViewer.MouseWheel -= new MouseWheelEventHandler(ScrollViewerExtensions.OnScrollViewerMouseWheel);
                }
            }
        }

        private static void OnHorizontalOffsetChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ((ScrollViewer) obj).ScrollToHorizontalOffset((double) args.NewValue);
        }

        private static void OnScrollViewerMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs args)
        {
            args.Handled = HandleMouseWheel(sender as ScrollViewer, (double) args.Delta);
        }

        private static void OnScrollViewerMouseWheel(object sender, Telerik.Windows.Input.MouseWheelEventArgs args)
        {
            args.Handled = HandleMouseWheel(sender as ScrollViewer, (double) args.Delta);
        }

        private static void OnVerticalOffsetChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ((ScrollViewer) obj).ScrollToVerticalOffset((double) args.NewValue);
        }

        public static void SetAttachedHorizontalScrollBar(DependencyObject obj, ScrollBar value)
        {
            obj.SetValue(AttachedHorizontalScrollBarProperty, value);
        }

        public static void SetAttachedVerticalScrollBar(DependencyObject obj, ScrollBar value)
        {
            obj.SetValue(AttachedVerticalScrollBarProperty, value);
        }

        public static void SetEnableMouseWheel(DependencyObject obj, bool value)
        {
            obj.SetValue(EnableMouseWheelProperty, value);
        }
    }
}

