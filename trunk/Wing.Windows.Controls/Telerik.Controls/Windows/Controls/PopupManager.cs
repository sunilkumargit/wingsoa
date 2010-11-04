namespace Telerik.Windows.Controls
{
    using System;
    using System.Globalization;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using Telerik.WebUI;

    public static class PopupManager
    {
        private static int defaultDockWindowZIndex = 0xf1b30;
        private static int defaultPopupZIndex = 0x493e0;
        private static int defaultTopWindowZIndex = 0x186a0;
        private static int defaultWindowZIndex;
        private const int MAXZINDEX = 0xf4240;
        private static int nextDockWindowZIndex;
        private static int nextPopupZIndex;
        private static int nextTopWindowZIndex;
        private static int nextWindowZIndex;
        private static int openedDockWindows;
        private static int openedPopups;
        private static int openedTopWindows;
        private static int openedWindows;

        internal static bool BringToFront(Popup popup, PopupType type)
        {
            if (CanBringToFront(popup))
            {
                switch (type)
                {
                    case PopupType.Window:
                        BringToFrontWindow(popup);
                        goto Label_0046;

                    case PopupType.TopMostWindow:
                        BringToFrontTopMostWindow(popup);
                        goto Label_0046;

                    case PopupType.Popup:
                        BringToFrontPopup(popup);
                        goto Label_0046;

                    case PopupType.DockWindow:
                        BringToFrontDockWindow(popup);
                        goto Label_0046;
                }
            }
            return false;
        Label_0046:
            return true;
        }

        private static void BringToFrontDockWindow(Popup popup)
        {
            int currentZIndex = Canvas.GetZIndex(popup);
            if ((currentZIndex < defaultDockWindowZIndex) || (currentZIndex > 0xf4240))
            {
                throw new InvalidOperationException(Telerik.Windows.Controls.SR.GetString("CannotBringToFrontDifferentTypes"));
            }
            if (currentZIndex < (NextDockWindowZIndex - 1))
            {
                CoerceNextDockWindowZIndex();
                Canvas.SetZIndex(popup, NextDockWindowZIndex);
                nextDockWindowZIndex++;
            }
        }

        private static void BringToFrontPopup(Popup popup)
        {
            int currentZIndex = Canvas.GetZIndex(popup);
            if ((currentZIndex < defaultPopupZIndex) || (currentZIndex >= defaultDockWindowZIndex))
            {
                throw new InvalidOperationException(Telerik.Windows.Controls.SR.GetString("CannotBringToFrontDifferentTypes"));
            }
            if (currentZIndex < (NextPopupZIndex - 1))
            {
                CoerceNextPopupZIndex();
                Canvas.SetZIndex(popup, NextPopupZIndex);
                nextPopupZIndex++;
            }
        }

        private static void BringToFrontTopMostWindow(Popup popup)
        {
            Close(popup, true);
            Open(popup, true);
            int currentZIndex = Canvas.GetZIndex(popup);
            if (currentZIndex < defaultTopWindowZIndex)
            {
                DecreaseNextWindowZIndex(currentZIndex);
                openedTopWindows++;
            }
            else if (currentZIndex >= defaultPopupZIndex)
            {
                throw new InvalidOperationException(Telerik.Windows.Controls.SR.GetString("CannotBringToFrontDifferentTypes"));
            }
            if (currentZIndex < (NextTopWindowZIndex - 1))
            {
                CoerceNextTopMostWindowZIndex();
                Canvas.SetZIndex(popup, NextTopWindowZIndex);
                nextTopWindowZIndex++;
            }
        }

        private static void BringToFrontWindow(Popup popup)
        {
            int currentZIndex = Canvas.GetZIndex(popup);
            if (currentZIndex >= defaultTopWindowZIndex)
            {
                if (currentZIndex >= defaultPopupZIndex)
                {
                    throw new InvalidOperationException(Telerik.Windows.Controls.SR.GetString("CannotBringToFrontDifferentTypes"));
                }
                DecreaseNextTopMostZIndex(currentZIndex);
                openedWindows++;
                currentZIndex = defaultWindowZIndex - 2;
            }
            if (currentZIndex < (NextWindowZIndex - 1))
            {
                Close(popup, false);
                Open(popup, false);
                CoerceNextWindowZIndex();
                Canvas.SetZIndex(popup, NextWindowZIndex);
                nextWindowZIndex++;
            }
        }

        private static bool CanBringToFront(Popup popup)
        {
            return (((popup != null) && (popup.Child != null)) && popup.IsOpen);
        }

        private static bool CanPopupClose(Popup popup)
        {
            return (((popup != null) && (popup.Child != null)) && popup.IsOpen);
        }

        private static bool CanPopupOpen(Popup popup)
        {
            return (((popup != null) && (popup.Child != null)) && !popup.IsOpen);
        }

        private static void Close(Popup popup, bool detachFromVisualRoot)
        {
            popup.IsOpen = false;
            PopupWrapper.RegisteredPopups.Remove(popup);
            if (detachFromVisualRoot)
            {
                HACKS.RemovePopupFromVisualTree(popup);
            }
        }

        internal static bool Close(Popup popup, PopupType type)
        {
            return Close(popup, type, true);
        }

        internal static bool Close(Popup popup, PopupType type, bool detachFromVisualRoot)
        {
            if (CanPopupClose(popup))
            {
                switch (type)
                {
                    case PopupType.Window:
                        if (openedWindows != 0)
                        {
                            CloseWindow(popup, detachFromVisualRoot);
                            goto Label_006E;
                        }
                        return false;

                    case PopupType.TopMostWindow:
                        if (openedTopWindows != 0)
                        {
                            CloseTopMostWindow(popup, detachFromVisualRoot);
                            goto Label_006E;
                        }
                        return false;

                    case PopupType.Popup:
                        if (openedPopups != 0)
                        {
                            ClosePopup(popup, detachFromVisualRoot);
                            goto Label_006E;
                        }
                        return false;

                    case PopupType.DockWindow:
                        if (openedDockWindows != 0)
                        {
                            CloseDockWindow(popup, detachFromVisualRoot);
                            goto Label_006E;
                        }
                        return false;
                }
            }
            return false;
        Label_006E:
            return true;
        }

        private static void CloseDockWindow(Popup popup, bool detachFromVisualRoot)
        {
            Close(popup, detachFromVisualRoot);
            int zindex = Canvas.GetZIndex(popup);
            Canvas.SetZIndex(popup, 0);
            openedDockWindows--;
            if (openedDockWindows == 0)
            {
                nextDockWindowZIndex = 0;
            }
            else if (zindex == (NextDockWindowZIndex - 1))
            {
                nextDockWindowZIndex--;
            }
        }

        private static void ClosePopup(Popup popup, bool detachFromVisualRoot)
        {
            Close(popup, detachFromVisualRoot);
            int zindex = Canvas.GetZIndex(popup);
            Canvas.SetZIndex(popup, 0);
            openedPopups--;
            if (openedPopups == 0)
            {
                nextPopupZIndex = 0;
            }
            else if (zindex == (NextPopupZIndex - 1))
            {
                nextPopupZIndex--;
            }
        }

        private static void CloseTopMostWindow(Popup popup, bool detachFromVisualRoot)
        {
            Close(popup, detachFromVisualRoot);
            int zindex = Canvas.GetZIndex(popup);
            Canvas.SetZIndex(popup, 0);
            DecreaseNextTopMostZIndex(zindex);
        }

        private static void CloseWindow(Popup popup, bool detachFromVisualRoot)
        {
            Close(popup, detachFromVisualRoot);
            int zindex = Canvas.GetZIndex(popup);
            Canvas.SetZIndex(popup, 0);
            DecreaseNextWindowZIndex(zindex);
        }

        private static void CoerceDockWindowStartingIndex(int index)
        {
            CoercePopupIndex(index, defaultPopupZIndex, 0xf4240);
        }

        private static void CoerceNextDockWindowZIndex()
        {
            int maxDockWindowZIndex = 0xf4240 - defaultDockWindowZIndex;
            if (nextDockWindowZIndex > maxDockWindowZIndex)
            {
                nextDockWindowZIndex = maxDockWindowZIndex;
            }
        }

        private static void CoerceNextPopupZIndex()
        {
            int maxPopupZIndex = defaultDockWindowZIndex - defaultPopupZIndex;
            if (nextPopupZIndex > maxPopupZIndex)
            {
                nextPopupZIndex = maxPopupZIndex;
            }
        }

        private static void CoerceNextTopMostWindowZIndex()
        {
            int maxTopWindowZIndex = defaultPopupZIndex - defaultTopWindowZIndex;
            if (nextTopWindowZIndex > maxTopWindowZIndex)
            {
                nextTopWindowZIndex = maxTopWindowZIndex;
            }
        }

        private static void CoerceNextWindowZIndex()
        {
            int maxWindowZIndex = defaultTopWindowZIndex - defaultWindowZIndex;
            if (nextWindowZIndex > maxWindowZIndex)
            {
                nextWindowZIndex = maxWindowZIndex;
            }
        }

        private static void CoercePopupIndex(int index, int minimum, int maximum)
        {
            if ((index < minimum) || (index > maximum))
            {
                string err = string.Format(CultureInfo.CurrentCulture, Telerik.Windows.Controls.SR.GetString("PopupIndexOutOfRange"), new object[] { minimum, maximum });
                throw new ArgumentOutOfRangeException("index", err);
            }
        }

        private static void CoercePopupStartingIndex(int index)
        {
            CoercePopupIndex(index, defaultTopWindowZIndex, defaultDockWindowZIndex);
        }

        private static void CoerceTopWindowStartingIndex(int index)
        {
            CoercePopupIndex(index, defaultWindowZIndex, defaultPopupZIndex);
        }

        private static void CoerceWindowStartingIndex(int index)
        {
            CoercePopupIndex(index, 0, defaultTopWindowZIndex);
        }

        private static void DecreaseNextTopMostZIndex(int zindex)
        {
            openedTopWindows--;
            if (openedTopWindows == 0)
            {
                nextTopWindowZIndex = 0;
            }
            else if (zindex == (NextTopWindowZIndex - 1))
            {
                nextTopWindowZIndex--;
            }
        }

        private static void DecreaseNextWindowZIndex(int zindex)
        {
            openedWindows--;
            if (openedWindows == 0)
            {
                nextWindowZIndex = 0;
            }
            else if (zindex == (NextWindowZIndex - 1))
            {
                nextWindowZIndex--;
            }
        }

        internal static int MaxZIndex(PopupType type)
        {
            switch (type)
            {
                case PopupType.Window:
                    return (nextWindowZIndex - 1);

                case PopupType.TopMostWindow:
                    return (nextTopWindowZIndex - 1);

                case PopupType.Popup:
                    return (nextPopupZIndex - 1);

                case PopupType.DockWindow:
                    return (nextDockWindowZIndex - 1);
            }
            return 0;
        }

        private static void Open(Popup popup, bool attachToVisualRoot)
        {
            if (attachToVisualRoot)
            {
                HACKS.AttachPopupToVisualTree(popup);
            }
            if (!PopupWrapper.RegisteredPopups.Contains(popup))
            {
                PopupWrapper.RegisteredPopups.Add(popup);
            }
            PopupTracker.Track(popup);
            popup.IsOpen = true;
        }

        internal static bool Open(Popup popup, PopupType type)
        {
            return Open(popup, type, true);
        }

        internal static bool Open(Popup popup, PopupType type, bool attachToVisualRoot)
        {
            if (CanPopupOpen(popup))
            {
                switch (type)
                {
                    case PopupType.Window:
                        OpenWindow(popup, attachToVisualRoot);
                        goto Label_004A;

                    case PopupType.TopMostWindow:
                        OpenTopMostWindow(popup, attachToVisualRoot);
                        goto Label_004A;

                    case PopupType.Popup:
                        OpenPopup(popup, attachToVisualRoot);
                        goto Label_004A;

                    case PopupType.DockWindow:
                        OpenDockWindow(popup, attachToVisualRoot);
                        goto Label_004A;
                }
            }
            return false;
        Label_004A:
            return true;
        }

        private static void OpenDockWindow(Popup popup, bool attachToVisualRoot)
        {
            CoerceNextDockWindowZIndex();
            Canvas.SetZIndex(popup, NextDockWindowZIndex);
            nextDockWindowZIndex++;
            Open(popup, attachToVisualRoot);
            openedDockWindows++;
        }

        private static void OpenPopup(Popup popup, bool attachToVisualRoot)
        {
            CoerceNextPopupZIndex();
            Canvas.SetZIndex(popup, NextPopupZIndex);
            nextPopupZIndex++;
            Open(popup, attachToVisualRoot);
            openedPopups++;
        }

        private static void OpenTopMostWindow(Popup popup, bool attachToVisualRoot)
        {
            CoerceNextTopMostWindowZIndex();
            Canvas.SetZIndex(popup, NextTopWindowZIndex);
            nextTopWindowZIndex++;
            Open(popup, attachToVisualRoot);
            openedTopWindows++;
        }

        private static void OpenWindow(Popup popup, bool attachToVisualRoot)
        {
            CoerceNextWindowZIndex();
            Canvas.SetZIndex(popup, NextWindowZIndex);
            nextWindowZIndex++;
            Open(popup, attachToVisualRoot);
            openedWindows++;
        }

        internal static void Reset()
        {
            openedWindows = 0;
            openedTopWindows = 0;
            openedPopups = 0;
            openedDockWindows = 0;
            nextWindowZIndex = 0;
            nextTopWindowZIndex = 0;
            nextPopupZIndex = 0;
            nextDockWindowZIndex = 0;
            DockWindowStartingZIndex = 0xf1b30;
            PopupStartingZIndex = 0x493e0;
            TopWindowStartingZIndex = 0x186a0;
            WindowStartingZIndex = 0;
        }

        public static int DockWindowStartingZIndex
        {
            get
            {
                return defaultDockWindowZIndex;
            }
            set
            {
                if (openedDockWindows != 0)
                {
                    throw new InvalidOperationException(Telerik.Windows.Controls.SR.GetString("CannotChangeStartingIndexOnOpenPopups"));
                }
                CoerceDockWindowStartingIndex(value);
                defaultDockWindowZIndex = value;
            }
        }

        internal static int NextDockWindowZIndex
        {
            get
            {
                return (nextDockWindowZIndex + defaultDockWindowZIndex);
            }
        }

        internal static int NextPopupZIndex
        {
            get
            {
                return (nextPopupZIndex + defaultPopupZIndex);
            }
        }

        internal static int NextTopWindowZIndex
        {
            get
            {
                return (nextTopWindowZIndex + defaultTopWindowZIndex);
            }
        }

        internal static int NextWindowZIndex
        {
            get
            {
                return (nextWindowZIndex + defaultWindowZIndex);
            }
        }

        internal static int OpenedDockWindowsCount
        {
            get
            {
                return openedDockWindows;
            }
        }

        internal static int OpenedPopupsCount
        {
            get
            {
                return openedPopups;
            }
        }

        internal static int OpenedTopWindowsCount
        {
            get
            {
                return openedTopWindows;
            }
        }

        internal static int OpenedWindowsCount
        {
            get
            {
                return openedWindows;
            }
        }

        public static int PopupStartingZIndex
        {
            get
            {
                return defaultPopupZIndex;
            }
            set
            {
                if (openedPopups != 0)
                {
                    throw new InvalidOperationException(Telerik.Windows.Controls.SR.GetString("CannotChangeStartingIndexOnOpenPopups"));
                }
                CoercePopupStartingIndex(value);
                defaultPopupZIndex = value;
            }
        }

        public static int TopWindowStartingZIndex
        {
            get
            {
                return defaultTopWindowZIndex;
            }
            set
            {
                if (openedTopWindows != 0)
                {
                    throw new InvalidOperationException(Telerik.Windows.Controls.SR.GetString("CannotChangeStartingIndexOnOpenPopups"));
                }
                CoerceTopWindowStartingIndex(value);
                defaultTopWindowZIndex = value;
            }
        }

        public static int WindowStartingZIndex
        {
            get
            {
                return defaultWindowZIndex;
            }
            set
            {
                if (openedWindows != 0)
                {
                    throw new InvalidOperationException(Telerik.Windows.Controls.SR.GetString("CannotChangeStartingIndexOnOpenPopups"));
                }
                CoerceWindowStartingIndex(value);
                defaultWindowZIndex = value;
            }
        }
    }
}

