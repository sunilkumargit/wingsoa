namespace Telerik.WebUI
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls.Primitives;

    public static class PopupTracker
    {
        private static Dictionary<int, WeakReference> s_childWindows = new Dictionary<int, WeakReference>();
        private static Dictionary<int, WeakReference> s_popups = new Dictionary<int, WeakReference>();

        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public static ReadOnlyCollection<FrameworkElement> GetChildWindows()
        {
            List<FrameworkElement> windows = new List<FrameworkElement>();
            List<int> toRemove = new List<int>();
            foreach (KeyValuePair<int, WeakReference> kvp in s_childWindows)
            {
                WeakReference wref = kvp.Value;
                int key = kvp.Key;
                try
                {
                    FrameworkElement target = wref.Target as FrameworkElement;
                    if (target != null)
                    {
                        windows.Add(target);
                    }
                    else
                    {
                        toRemove.Add(key);
                    }
                    continue;
                }
                catch (InvalidOperationException)
                {
                    toRemove.Add(key);
                    continue;
                }
            }
            foreach (int k in toRemove)
            {
                s_childWindows.Remove(k);
            }
            return windows.AsReadOnly();
        }

        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate"), SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId="Popups")]
        public static ReadOnlyCollection<Popup> GetPopups()
        {
            List<Popup> popups = new List<Popup>();
            List<int> toRemove = new List<int>();
            foreach (KeyValuePair<int, WeakReference> kvp in s_popups)
            {
                int key = kvp.Key;
                WeakReference wref = kvp.Value;
                try
                {
                    Popup target = wref.Target as Popup;
                    if (target != null)
                    {
                        popups.Add(target);
                    }
                    else
                    {
                        toRemove.Add(key);
                    }
                    continue;
                }
                catch (InvalidOperationException)
                {
                    toRemove.Add(key);
                    continue;
                }
            }
            foreach (int k in toRemove)
            {
                s_popups.Remove(k);
            }
            return popups.AsReadOnly();
        }

        public static void Track(Popup popup)
        {
            if (popup == null)
            {
                throw new ArgumentNullException("popup");
            }
            if (!s_popups.ContainsKey(popup.GetHashCode()))
            {
                s_popups.Add(popup.GetHashCode(), new WeakReference(popup));
            }
        }

        public static void Track(FrameworkElement childWindow)
        {
            if (childWindow == null)
            {
                throw new ArgumentNullException("childWindow");
            }
            Type windowType = childWindow.GetType();
            while ((windowType != null) && !string.Equals(windowType.FullName, "System.Windows.Controls.ChildWindow"))
            {
                windowType = windowType.BaseType;
            }
            if (windowType == null)
            {
                throw new ArgumentException("Tracked element is not a subclass of ChildWindow", "childWindow");
            }
            if (!s_childWindows.ContainsKey(childWindow.GetHashCode()))
            {
                s_childWindows.Add(childWindow.GetHashCode(), new WeakReference(childWindow));
            }
        }
    }
}

