namespace Telerik.Windows.Controls.TreeView
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using Telerik.Windows;

    internal static class TreeViewPanelExtensions
    {
        private static FrameworkElement bringIntoView;

        public static void BringIntoView(this FrameworkElement visual)
        {
            if ((bringIntoView != null) && (bringIntoView != visual))
            {
                (Application.Current.RootVisual as FrameworkElement).LayoutUpdated -= new EventHandler(TreeViewPanelExtensions.OnElementLayoutUpdated);
            }
            (Application.Current.RootVisual as FrameworkElement).LayoutUpdated += new EventHandler(TreeViewPanelExtensions.OnElementLayoutUpdated);
            bringIntoView = visual;
            visual.InvalidateMeasure();
        }

        public static void BringIntoView(this FrameworkElement visual, Rect targetRectangle)
        {
            RequestBringIntoViewEventArgs e = new RequestBringIntoViewEventArgs(visual, targetRectangle) {
                RoutedEvent = TreeViewPanel.RequestBringIntoViewEvent
            };
            visual.RaiseEvent(e);
        }

        public static bool IntersectsWith(this Rect rect1, Rect rect)
        {
            if (rect1.IsEmpty || rect.IsEmpty)
            {
                return false;
            }
            return ((((rect.Left <= rect1.Right) && (rect.Right >= rect1.Left)) && (rect.Top <= rect1.Bottom)) && (rect.Bottom >= rect1.Top));
        }

        public static bool IsAncestorOf(this UIElement target, UIElement descendant)
        {
            for (UIElement parent = VisualTreeHelper.GetParent(descendant) as UIElement; parent != null; parent = VisualTreeHelper.GetParent(parent) as UIElement)
            {
                if (parent == target)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsKeyboardFocusWithin(this UIElement element)
        {
            return (FocusManager.GetFocusedElement() == element);
        }

        public static void MoveVisualChild(this UIElementCollection collection, UIElement visual, UIElement destination)
        {
            int visualIndex = collection.IndexOf(visual);
            int index = (destination != null) ? collection.IndexOf(destination) : collection.Count;
            if (visualIndex != index)
            {
                if (visualIndex < index)
                {
                    index--;
                }
                collection.Remove(visual);
                collection.Insert(index, visual);
            }
        }

        private static void OnElementLayoutUpdated(object sender, EventArgs e)
        {
            (Application.Current.RootVisual as FrameworkElement).LayoutUpdated -= new EventHandler(TreeViewPanelExtensions.OnElementLayoutUpdated);
            if (bringIntoView != null)
            {
                bringIntoView.BringIntoView(Rect.Empty);
            }
            bringIntoView = null;
        }
    }
}

