namespace Telerik.Windows.Controls
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls.Primitives;
    using System.Windows.Media;

    internal static class VisualTreeHelperExtensions
    {
        internal static IEnumerable<T> GetAncestors<T>(this DependencyObject target) where T : class
        {
            //.TODO.
            return new List<T>();
        }

        internal static List<T> GetChildren<T>(this DependencyObject parent) where T : FrameworkElement
        {
            List<T> collection = new List<T>();
            if (parent == null)
            {
                return null;
            }
            int count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                DependencyObject obj = VisualTreeHelper.GetChild(parent, i);
                if (obj.GetType() == typeof(T))
                {
                    collection.Add((T)obj);
                }
                if (VisualTreeHelper.GetChildrenCount(obj) > 0)
                {
                    foreach (T newElement in obj.GetChildren<T>())
                    {
                        collection.Add(newElement);
                    }
                }
            }
            return collection;
        }

        internal static IEnumerable<T> GetElementsInHostCoordinates<T>(this UIElement subtree, Point position) where T : UIElement
        {
            if (subtree == null)
            {
                return Enumerable.Empty<T>();
            }
            UIElement testElement = subtree;
            Popup popup = subtree as Popup;
            if (popup != null)
            {
                testElement = (VisualTreeHelper.GetParent(popup) == null) ? popup.Child : ApplicationHelper.GetRootVisual(popup);
                Popup rootPopup = testElement as Popup;
                if (rootPopup != null)
                {
                    testElement = rootPopup.Child;
                }
            }
            return VisualTreeHelper.FindElementsInHostCoordinates(ApplicationHelper.TransformToScreenRoot(testElement).Transform(position), subtree).OfType<T>();
        }

        internal static IEnumerable<T> GetElementsInScreenCoordinates<T>(Point mousePosition) where T : UIElement
        {
            return GetElementsInScreenCoordinates<T>(mousePosition, ApplicationHelper.RootVisual);
        }

        internal static IEnumerable<T> GetElementsInScreenCoordinates<T>(this FrameworkElement relativeTo, Point mousePosition) where T : UIElement
        {
            return GetElementsInScreenCoordinates<T>(mousePosition, ApplicationHelper.GetRootVisual(relativeTo));
        }

        private static IEnumerable<T> GetElementsInScreenCoordinates<T>(Point mousePosition, FrameworkElement rootVisual) where T : UIElement
        {
            return GetElementsInScreenCoordinatesFromOpenedPopups<T>(mousePosition).Concat<T>(rootVisual.GetElementsInHostCoordinates<T>(mousePosition));
        }

        internal static IEnumerable<T> GetElementsInScreenCoordinatesFromOpenedPopups<T>(Point position) where T : UIElement
        {
            IEnumerable<Popup> popups = Enumerable.Empty<Popup>();
            return PopupWrapper.RegisteredPopups
                .Reverse<Popup>()
                .SelectMany(p => p.GetElementsInHostCoordinates<T>(position));
        }

        internal static T GetParent<T>(this DependencyObject child) where T : FrameworkElement
        {
            return (child as UIElement).ParentOfType<T>();
        }

    }
}

