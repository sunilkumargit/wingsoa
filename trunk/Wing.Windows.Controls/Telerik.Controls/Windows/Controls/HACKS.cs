namespace Telerik.Windows.Controls
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Media;

    internal static class HACKS
    {
        internal static void AttachPopupToVisualTree(Popup popup)
        {
            UIElement root = Application.Current.RootVisual;
            if (root != null)
            {
                Panel panel = root as Panel;
                if (panel == null)
                {
                    int childrenCount = VisualTreeHelper.GetChildrenCount(root);
                    for (int i = 0; i < childrenCount; i++)
                    {
                        panel = VisualTreeHelper.GetChild(root, i) as Panel;
                        if (panel != null)
                        {
                            break;
                        }
                    }
                }
                if ((panel != null) && (popup != null))
                {
                    DependencyObject visualParent = VisualTreeHelper.GetParent(popup);
                    DependencyObject logicalParent = popup.Parent;
                    if ((visualParent == null) && (logicalParent == null))
                    {
                        panel.Children.Insert(0, popup);
                        popup.UpdateLayout();
                    }
                }
            }
        }

        internal static bool RemoveParent(FrameworkElement child)
        {
            if (child.Parent == null)
            {
                return false;
            }
            if ((!RemoveParent(child, child.Parent as Popup) && !RemoveParent(child, child.Parent as Border)) && (!RemoveParent(child, child.Parent as ContentControl) && !RemoveParent(child, child.Parent as ContentPresenter)))
            {
                return RemoveParent(child, child.Parent as Panel);
            }
            return true;
        }

        private static bool RemoveParent(FrameworkElement child, Border parent)
        {
            if ((parent != null) && (parent.Child == child))
            {
                parent.Child = null;
                return true;
            }
            return false;
        }

        private static bool RemoveParent(FrameworkElement child, ContentControl parent)
        {
            if ((parent != null) && (parent.Content == child))
            {
                parent.ClearValue(ContentControl.ContentProperty);
                return true;
            }
            return false;
        }

        private static bool RemoveParent(FrameworkElement child, ContentPresenter parent)
        {
            if ((parent != null) && (parent.Content == child))
            {
                parent.ClearValue(ContentPresenter.ContentProperty);
                return true;
            }
            return false;
        }

        private static bool RemoveParent(FrameworkElement child, Panel parent)
        {
            if ((parent != null) && parent.Children.Contains(child))
            {
                parent.Children.Remove(child);
                return true;
            }
            return false;
        }

        private static bool RemoveParent(FrameworkElement child, Popup parent)
        {
            if ((parent != null) && (parent.Child == child))
            {
                parent.ClearValue(Popup.ChildProperty);
                return true;
            }
            return false;
        }

        internal static void RemovePopupFromVisualTree(Popup popup)
        {
            Panel panel = popup.Parent as Panel;
            if (panel != null)
            {
                panel.Children.Remove(popup);
            }
        }
    }
}

