namespace Telerik.Windows.Controls
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    public static class DependencyObjectExtensions
    {
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public static T FindChildByType<T>(this DependencyObject source) where T : DependencyObject
        {
            return source.FindChildrenByType<T>().FirstOrDefault<T>();
        }

        internal static IEnumerable<T> FindChildrenByType<T>(this DependencyObject source) where T : DependencyObject
        {
            var queue = new Queue<DependencyObject>();
            queue.Enqueue(source);
            while (queue.Count > 0)
            {
                var currItem = queue.Dequeue();
                var childrenCount = VisualTreeHelper.GetChildrenCount(currItem);
                for (var i = 0; i < childrenCount; i++)
                {
                    var childItem = VisualTreeHelper.GetChild(currItem, i);
                    var typedChild = childItem as T;
                    if (typedChild == null)
                    {
                        queue.Enqueue(childItem);
                        continue;
                    }
                    else
                        yield return typedChild;
                }
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        internal static bool Focus(this DependencyObject dependencyObject)
        {
            return ((dependencyObject is Control) && ((Control)dependencyObject).Focus());
        }

        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public static T GetVisualParent<T>(this DependencyObject dependencyObject) where T : DependencyObject
        {
            return dependencyObject.GetVisualParent<T>(false);
        }

        internal static TParent GetVisualParent<TParent, TRestriction>(this DependencyObject dependencyObject) where TParent : DependencyObject
        {
            DependencyObject parent = null;
            if (dependencyObject != null)
            {
                parent = VisualTreeHelper.GetParent(dependencyObject);
            }
            while ((parent != null) && (!(parent is TParent) || !(parent is TRestriction)))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }
            return (TParent)parent;
        }

        [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        internal static TParent GetVisualParent<TParent>(this DependencyObject dependencyObject, bool includeSelf) where TParent : DependencyObject
        {
            if (includeSelf && (dependencyObject is TParent))
            {
                return (dependencyObject as TParent);
            }
            return dependencyObject.GetVisualParent<TParent, TParent>();
        }

        internal static void SetCurrentValue(this DependencyObject dependencyObject, DependencyProperty dependencyProperty, object value)
        {
            dependencyObject.SetValue(dependencyProperty, value);
        }
    }
}

