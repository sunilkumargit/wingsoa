namespace Telerik.Windows.Controls.RadWindowPopup
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using Telerik.Windows;
    using Telerik.Windows.Controls;

    internal class WindowPopupSilverlightFactory : WindowPopupFactory
    {
        protected override WindowPopup CreatePopup(UIElement content)
        {
            return new WindowPopupSilverlightImpl();
        }

        private class WindowPopupSilverlightImpl : BrowserWindowPopup
        {
            private bool areEventsAttached;
            private Panel content;
            private bool isPopupLoaded;
            private Popup popup = new Popup();
            private bool showOnLoad;

            public WindowPopupSilverlightImpl()
            {
                if (Application.Current.RootVisual != null)
                {
                    this.isPopupLoaded = true;
                }
                else
                {
                    WeakEventListener<WindowPopupSilverlightFactory.WindowPopupSilverlightImpl, object, RoutedEventArgs> weakListener = new WeakEventListener<WindowPopupSilverlightFactory.WindowPopupSilverlightImpl, object, RoutedEventArgs>(this) {
                        OnEventAction = delegate (WindowPopupSilverlightFactory.WindowPopupSilverlightImpl popup, object sender, RoutedEventArgs args) {
                            popup.OnPopupLoaded(sender, args);
                        }
                    };
                    this.popup.Loaded += new RoutedEventHandler(weakListener.OnEvent);
                }
                this.content = new Canvas();
                this.popup.Child = this.content;
            }

            private void AttachEvents()
            {
                if (!this.areEventsAttached)
                {
                    this.areEventsAttached = true;
                    WeakEventListener<WindowPopupSilverlightFactory.WindowPopupSilverlightImpl, object, EventArgs> weakListener = new WeakEventListener<WindowPopupSilverlightFactory.WindowPopupSilverlightImpl, object, EventArgs>(this) {
                        OnEventAction = delegate (WindowPopupSilverlightFactory.WindowPopupSilverlightImpl popup, object sender, EventArgs args) {
                            popup.InvalidateArrange();
                        },
                        OnDetachAction = delegate (WeakEventListener<WindowPopupSilverlightFactory.WindowPopupSilverlightImpl, object, EventArgs> listener) {
                            ApplicationHelper.RootVisual.SizeChanged -= new SizeChangedEventHandler(listener.OnEvent);
                        }
                    };
                    ApplicationHelper.RootVisual.SizeChanged += new SizeChangedEventHandler(weakListener.OnEvent);
                }
            }

            public override void BringToFront()
            {
                PopupManager.BringToFront(this.popup, base.IsTopMost ? PopupType.TopMostWindow : PopupType.Window);
            }

            protected override void ClosePopup()
            {
                if (!this.showOnLoad)
                {
                    PopupManager.Close(this.popup, base.IsTopMost ? PopupType.TopMostWindow : PopupType.Window);
                }
                else
                {
                    this.showOnLoad = false;
                }
                this.content.Children.Clear();
            }

            public override UIElement GetVisual()
            {
                return this.content;
            }

            public override int GetZIndex()
            {
                return Canvas.GetZIndex(this.popup);
            }

            public override void Move(double left, double top)
            {
                Canvas.SetLeft(base.Child, left);
                Canvas.SetTop(base.Child, top);
            }

            private void OnPopupLoaded(object sender, RoutedEventArgs e)
            {
                this.isPopupLoaded = true;
                if (this.showOnLoad)
                {
                    this.showOnLoad = false;
                    this.Show();
                }
                this.popup.Loaded -= new RoutedEventHandler(this.OnPopupLoaded);
            }

            protected override void OpenPopup()
            {
                base.OpenPopup();
                if (base.IsModal)
                {
                    this.content.Children.Add(base.ModalCanvas);
                }
                this.content.Children.Add(base.Child);
                RadWindow window = base.Child as RadWindow;
                if (window != null)
                {
                    this.Move(window.Left, window.Top);
                }
                if (this.isPopupLoaded)
                {
                    this.Show();
                }
                else
                {
                    this.showOnLoad = true;
                }
            }

            private void Show()
            {
                PopupManager.Open(this.popup, base.IsTopMost ? PopupType.TopMostWindow : PopupType.Window);
                this.AttachEvents();
                base.OnOpened();
            }
        }
    }
}

