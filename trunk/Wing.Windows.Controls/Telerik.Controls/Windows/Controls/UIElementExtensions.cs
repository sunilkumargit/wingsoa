namespace Telerik.Windows.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    public static class UIElementExtensions
    {
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public static IList<T> ChildrenOfType<T>(this UIElement element) where T: UIElement
        {
            List<T> list = new List<T>();
            if (element != null)
            {
                element.ChildrenOfType<T>(ref list);
            }
            return list;
        }

        private static IList<T> ChildrenOfType<T>(this UIElement element, ref List<T> list) where T: UIElement
        {
            int count = VisualTreeHelper.GetChildrenCount(element);
            for (int i = 0; i < count; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(element, i);
                if (child is T)
                {
                    list.Add((T) child);
                }
                UIElement uiElementChild = child as UIElement;
                if (uiElementChild != null)
                {
                    uiElementChild.ChildrenOfType<T>(ref list);
                }
            }
            return list;
        }

        internal static void GoToState(this Control element, bool useTransitions, params string[] stateNames)
        {
            if (stateNames != null)
            {
                foreach (string str in stateNames)
                {
                    if (VisualStateManager.GoToState(element, str, useTransitions))
                    {
                        break;
                    }
                }
            }
        }

        internal static bool IsAncestorOf(this UIElement target, DependencyObject element)
        {
            if (target == element)
            {
                return true;
            }
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            if (!(element is UIElement))
            {
                throw new InvalidOperationException("element is not UIElement");
            }
            DependencyObject parent = element;
            do
            {
                DependencyObject currentParent = VisualTreeHelper.GetParent(parent);
                FrameworkElement feParent = parent as FrameworkElement;
                if ((currentParent == null) && (feParent != null))
                {
                    parent = feParent.Parent;
                }
                else
                {
                    parent = currentParent;
                }
                if (parent == target)
                {
                    return true;
                }
            }
            while (parent != null);
            return false;
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters"), SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily"), SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public static T ParentOfType<T>(this UIElement element) where T: DependencyObject
        {
            if (element == null)
            {
                return default(T);
            }
            DependencyObject parent = VisualTreeHelper.GetParent(element);
            while ((parent != null) && !(parent is T))
            {
                DependencyObject newVisualParent = VisualTreeHelper.GetParent(parent);
                if (newVisualParent != null)
                {
                    parent = newVisualParent;
                }
                else
                {
                    if (!(parent is FrameworkElement))
                    {
                        break;
                    }
                    parent = (parent as FrameworkElement).Parent;
                }
            }
            return (parent as T);
        }
    }
}

