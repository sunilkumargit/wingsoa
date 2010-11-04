namespace Telerik.Windows.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Windows;
    using Telerik.Windows.Controls.DragDrop;

    public class RadWindowManager
    {
        private static RadWindowManager instnace;
        private List<RadWindow> windows = new List<RadWindow>();

        private RadWindowManager()
        {
            RadDragAndDropManager.WindowsCollection = this.windows;
        }

        internal void AddWindow(RadWindow window)
        {
            this.windows.Add(window);
            this.MarkWindowAsActive(window);
        }

        internal void BringToFront(RadWindow window)
        {
            if (!window.IsActiveWindow)
            {
                this.MarkWindowAsActive(window);
                window.Popup.BringToFront();
            }
        }

        public void CloseAllWindows()
        {
            this.Go(w => w.IsOpen, delegate (RadWindow w) {
                w.Close();
            });
        }

        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public IList<RadWindow> GetWindows()
        {
            return new ReadOnlyCollection<RadWindow>(this.Windows.ToList<RadWindow>());
        }

        private void Go(Func<RadWindow, bool> filter, Action<RadWindow> method)
        {
            if (this.Windows != null)
            {
                (from w in this.Windows
                    where filter(w)
                    select w).ToList<RadWindow>().ForEach(method);
            }
        }

        private void MarkWindowAsActive(RadWindow window)
        {
            window.IsActiveWindow = true;
            (from w in this.Windows
                where w != window
                select w).ToList<RadWindow>().ForEach(delegate (RadWindow w) {
                w.IsActiveWindow = false;
            });
        }

        public void MaximizeAllWindows()
        {
            this.Go(w => w.IsOpen && (w.ResizeMode == ResizeMode.CanResize), delegate (RadWindow w) {
                w.WindowState = WindowState.Maximized;
            });
        }

        public void MinimizeAllWindows()
        {
            this.Go(w => w.IsOpen && (w.ResizeMode != ResizeMode.NoResize), delegate (RadWindow w) {
                w.WindowState = WindowState.Minimized;
            });
        }

        public void NormalAllWindows()
        {
            this.Go(w => w.IsOpen, delegate (RadWindow w) {
                w.WindowState = WindowState.Normal;
            });
        }

        internal void RemoveWindow(RadWindow window)
        {
            this.windows.Remove(window);
            if (window.IsActiveWindow)
            {
                RadWindow wnd = (from w in this.Windows
                    orderby w.Z
                    select w).LastOrDefault<RadWindow>();
                if (wnd != null)
                {
                    wnd.BringToFront();
                }
                window.IsActiveWindow = false;
            }
        }

        public static RadWindowManager Current
        {
            get
            {
                if (instnace == null)
                {
                    instnace = new RadWindowManager();
                }
                return instnace;
            }
        }

        internal IEnumerable<RadWindow> Windows
        {
            get
            {
                return (from w in this.windows
                    where w != null
                    select w);
            }
        }
    }
}

