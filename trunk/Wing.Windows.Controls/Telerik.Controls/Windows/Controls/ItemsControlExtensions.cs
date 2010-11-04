namespace Telerik.Windows.Controls
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    internal static class ItemsControlExtensions
    {
        internal static void ForEachContainerItem<T>(this Telerik.Windows.Controls.ItemsControl itemsControl, Action<T> work) where T : class
        {
            for (int i = 0; i < itemsControl.Items.Count; i++)
            {
                T itemContainer = itemsControl.ItemContainerGenerator.ContainerFromIndex(i) as T;
                if (itemContainer != null)
                {
                    work(itemContainer);
                }
            }
        }

        internal static IEnumerable<T> GetContainers<T>(this Telerik.Windows.Controls.ItemsControl target) where T : class
        {
            return new List<T>();
        }

        internal static Panel GetItemsPanel(this DependencyObject itemsControl)
        {
            return GetItemsPanelRecursive(itemsControl);
        }

        internal static TPanel GetItemsPanel<TPanel>(this DependencyObject itemsControl) where TPanel : Panel
        {
            return GetItemsPanelRecursive<TPanel>(itemsControl);
        }

        private static Panel GetItemsPanelRecursive(DependencyObject control)
        {
            Control element = control as Control;
            if (element != null)
            {
                element.ApplyTemplate();
            }
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(control); i++)
            {
                UIElement child = VisualTreeHelper.GetChild(control, i) as UIElement;
                if (child != null)
                {
                    Panel panel = child as Panel;
                    if ((panel != null) && (VisualTreeHelper.GetParent(child) is ItemsPresenter))
                    {
                        return panel;
                    }
                    panel = GetItemsPanelRecursive(child);
                    if (panel != null)
                    {
                        return panel;
                    }
                }
            }
            return null;
        }

        private static TPanel GetItemsPanelRecursive<TPanel>(DependencyObject control) where TPanel : Panel
        {
            Control element = control as Control;
            if (element != null)
            {
                element.ApplyTemplate();
            }
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(control); i++)
            {
                UIElement child = VisualTreeHelper.GetChild(control, i) as UIElement;
                if (child != null)
                {
                    TPanel panel = child as TPanel;
                    if ((panel != null) && (VisualTreeHelper.GetParent(child) is ItemsPresenter))
                    {
                        return panel;
                    }
                    panel = GetItemsPanelRecursive<TPanel>(child);
                    if (panel != null)
                    {
                        return panel;
                    }
                }
            }
            return default(TPanel);
        }
    }
}

