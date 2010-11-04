namespace Telerik.Windows.Controls
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Threading;

    internal static class Extensions
    {
        internal static void CopyValue(this DependencyObject destination, DependencyObject source, DependencyProperty property)
        {
            if (source.ReadLocalValue(property) != DependencyProperty.UnsetValue)
            {
                destination.SetValue(property, source.GetValue(property));
            }
        }

        internal static void DesignerSafeBeginInvoke(this Dispatcher dispatcher, Action action)
        {
            if (RadControl.IsInDesignMode)
            {
                action();
            }
            else
            {
                dispatcher.BeginInvoke(action);
            }
        }

        internal static void SetPlacementTarget(this RadContextMenu contextMenu, FrameworkElement element)
        {
            UIElement actualPlacementTarget = contextMenu.PlacementTarget;
            PlacementMode actualPlacement = contextMenu.Placement;
            contextMenu.PlacementTarget = element;
            contextMenu.Placement = PlacementMode.Bottom;
            RoutedEventHandler closeHandler = null;
            closeHandler = delegate {
                contextMenu.Closed -= closeHandler;
                contextMenu.PlacementTarget = actualPlacementTarget;
                contextMenu.Placement = actualPlacement;
            };
            contextMenu.Closed += closeHandler;
        }
    }
}

