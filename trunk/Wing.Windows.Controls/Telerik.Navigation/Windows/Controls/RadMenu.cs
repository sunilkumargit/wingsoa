namespace Telerik.Windows.Controls
{
    using System;
    using System.ComponentModel;
    using System.Threading;
    using System.Windows;
    using System.Windows.Automation.Peers;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using Telerik.Windows;

    [DefaultProperty("ClickToOpen"), StyleTypedProperty(Property="ItemContainerStyle", StyleTargetType=typeof(RadMenuItem))]
    public class RadMenu : MenuBase
    {
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(System.Windows.Controls.Orientation), typeof(RadMenu), new System.Windows.PropertyMetadata(System.Windows.Controls.Orientation.Horizontal, new PropertyChangedCallback(RadMenu.OnOrientationChanged)));
        private Panel panel;
        public static readonly DependencyProperty TopLevelHideDelayProperty = DependencyProperty.Register("TopLevelHideDelay", typeof(Duration), typeof(RadMenu), new FrameworkPropertyMetadata(MenuBase.DefaultHideDuration));
        public static readonly DependencyProperty TopLevelShowDelayProperty = DependencyProperty.Register("TopLevelShowDelay", typeof(Duration), typeof(RadMenu), new FrameworkPropertyMetadata(MenuBase.DefaultShowDuration));
        public static readonly DependencyProperty WaitForTopLevelHeaderHideDurationProperty = DependencyProperty.Register("WaitForTopLevelHeaderHideDuration", typeof(bool), typeof(RadMenu), new FrameworkPropertyMetadata(false));

        public event PropertyChangedEventHandler PropertyChanged;

        public RadMenu()
        {
            
            base.DefaultStyleKey = typeof(RadMenu);
            base.TabNavigation = KeyboardNavigationMode.Once;
        }

        private System.Windows.Controls.Orientation? GetPanelOrientation()
        {
            if (this.panel != null)
            {
                if (this.panel is StackPanel)
                {
                    return new System.Windows.Controls.Orientation?((System.Windows.Controls.Orientation) this.panel.GetValue(StackPanel.OrientationProperty));
                }
                if (this.panel is RadWrapPanel)
                {
                    return new System.Windows.Controls.Orientation?((System.Windows.Controls.Orientation) this.panel.GetValue(RadWrapPanel.OrientationProperty));
                }
            }
            return null;
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new RadMenuAutomationPeer(this);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (!e.Handled)
            {
                e.Handled = this.ProcessKey(e.Key);
            }
        }

        private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadMenu menu = d as RadMenu;
            if ((menu != null) && (menu.panel != null))
            {
                if (menu.panel is StackPanel)
                {
                    menu.panel.SetValue(StackPanel.OrientationProperty, menu.Orientation);
                }
                else if (menu.panel is RadWrapPanel)
                {
                    menu.panel.SetValue(RadWrapPanel.OrientationProperty, menu.Orientation);
                }
                menu.OnPropertyChanged("Orientation");
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            if (this.panel == null)
            {
                this.panel = VisualTreeHelper.GetParent(element) as Panel;
                if (this.panel is StackPanel)
                {
                    this.panel.SetValue(StackPanel.OrientationProperty, this.Orientation);
                }
                else if (this.panel is RadWrapPanel)
                {
                    this.panel.SetValue(RadWrapPanel.OrientationProperty, this.Orientation);
                }
            }
        }

        internal bool ProcessKey(Key key)
        {
            int index = -1;
            switch (key)
            {
                case Key.Left:
                case Key.Right:
                {
                    if (base.CurrentSelection == null)
                    {
                        goto Label_00FD;
                    }
                    System.Windows.Controls.Orientation? orientation = this.GetPanelOrientation();
                    if (!orientation.HasValue || (((System.Windows.Controls.Orientation) orientation.Value) != System.Windows.Controls.Orientation.Vertical))
                    {
                        int direction = (key == Key.Left) ? -1 : 1;
                        index = base.ItemContainerGenerator.IndexFromContainer(base.CurrentSelection);
                        base.MenuNavigate(index + direction, direction, base.CurrentSelection.IsSubmenuOpen);
                        break;
                    }
                    base.CurrentSelection.OpenSubmenuWithKeyboard();
                    break;
                }
                case Key.Up:
                case Key.Down:
                {
                    if (base.CurrentSelection == null)
                    {
                        goto Label_00FD;
                    }
                    System.Windows.Controls.Orientation? orientation = this.GetPanelOrientation();
                    if (!orientation.HasValue || (((System.Windows.Controls.Orientation) orientation) != System.Windows.Controls.Orientation.Vertical))
                    {
                        base.CurrentSelection.OpenSubmenuWithKeyboard();
                    }
                    else
                    {
                        int direction = (key == Key.Up) ? -1 : 1;
                        index = base.ItemContainerGenerator.IndexFromContainer(base.CurrentSelection);
                        base.MenuNavigate(index + direction, direction, base.CurrentSelection.IsSubmenuOpen);
                    }
                    return true;
                }
                default:
                    goto Label_00FD;
            }
            return true;
        Label_00FD:
            return false;
        }

        public System.Windows.Controls.Orientation Orientation
        {
            get
            {
                return (System.Windows.Controls.Orientation) base.GetValue(OrientationProperty);
            }
            set
            {
                base.SetValue(OrientationProperty, value);
            }
        }

        [TypeConverter(typeof(DurationConverter))]
        public Duration TopLevelHideDelay
        {
            get
            {
                return (Duration) base.GetValue(TopLevelHideDelayProperty);
            }
            set
            {
                base.SetValue(TopLevelHideDelayProperty, value);
            }
        }

        [TypeConverter(typeof(DurationConverter))]
        public Duration TopLevelShowDelay
        {
            get
            {
                return (Duration) base.GetValue(TopLevelShowDelayProperty);
            }
            set
            {
                base.SetValue(TopLevelShowDelayProperty, value);
            }
        }

        public bool WaitForTopLevelHeaderHideDuration
        {
            get
            {
                return (bool) base.GetValue(WaitForTopLevelHeaderHideDurationProperty);
            }
            set
            {
                base.SetValue(WaitForTopLevelHeaderHideDurationProperty, value);
            }
        }
    }
}

