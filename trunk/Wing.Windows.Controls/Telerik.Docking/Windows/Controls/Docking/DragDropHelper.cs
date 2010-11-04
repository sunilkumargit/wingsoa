namespace Telerik.Windows.Controls.Docking
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media;
    using Telerik.Windows;
    using Telerik.Windows.Controls;

    internal static class DragDropHelper
    {
        private static readonly DependencyProperty IsPopupDragRootProperty = DependencyProperty.RegisterAttached("IsPopupDragRoot", typeof(bool), typeof(DragDropHelper), null);

        public static void DragDelta(this UIElement target, MouseEventArgs e)
        {
            target.DragDelta(target.GetDragRoot(), e);
        }

        public static void DragDelta(this UIElement target, UIElement dragRoot, MouseEventArgs e)
        {
            target.RaiseEvent(new DragInfoEventArgs(RadPane.DragDeltaEvent, e.GetPosition(dragRoot)));
        }

        public static void EndDrag(this UIElement target, MouseEventArgs e)
        {
            target.EndDrag(target.GetDragRoot(), e);
        }

        public static void EndDrag(this UIElement target, UIElement dragRoot, MouseEventArgs e)
        {
            target.RaiseEvent(new DragInfoEventArgs(RadPane.DragCompletedEvent, e.GetPosition(dragRoot)));
        }

        private static UIElement GetDragRoot(this UIElement target)
        {
            for (UIElement root = target; root != null; root = VisualTreeHelper.GetParent(root) as UIElement)
            {
                target = root;
                if (GetIsPopupDragRoot(target))
                {
                    return target;
                }
            }
            return target;
        }

        internal static bool GetIsPopupDragRoot(DependencyObject obj)
        {
            return (bool) obj.GetValue(IsPopupDragRootProperty);
        }

        internal static void SetIsPopupDragRoot(DependencyObject obj, bool value)
        {
            obj.SetValue(IsPopupDragRootProperty, value);
        }

        public static void StartDrag(this UIElement target, MouseEventArgs e)
        {
            target.StartDrag(target.GetDragRoot(), e);
        }

        public static void StartDrag(this UIElement target, UIElement dragRoot, MouseEventArgs e)
        {
            target.RaiseEvent(new DragInfoEventArgs(RadPane.DragStartedEvent, e.GetPosition(dragRoot)));
        }

        public static void StopDrag(this UIElement target)
        {
            bool canceled = true;
            target.RaiseEvent(new DragInfoEventArgs(RadPane.DragCompletedEvent, new Point(), canceled));
        }
    }
}

