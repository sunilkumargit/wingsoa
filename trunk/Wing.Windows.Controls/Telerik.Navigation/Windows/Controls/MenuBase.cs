namespace Telerik.Windows.Controls
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Telerik.Windows;

    [DefaultEvent("ItemClick"), StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(RadMenuItem))]
    public abstract class MenuBase : Telerik.Windows.Controls.ItemsControl
    {
        public static readonly DependencyProperty ClickToOpenProperty = DependencyProperty.Register("ClickToOpen", typeof(bool), typeof(MenuBase), null);
        private RadMenuItem currentSelection;
        internal static readonly Duration DefaultHideDuration = new Duration(TimeSpan.FromMilliseconds(250.0));
        internal static readonly Duration DefaultShowDuration = new Duration(TimeSpan.FromMilliseconds(150.0));
        public static readonly DependencyProperty HideDelayProperty = DependencyProperty.Register("HideDelay", typeof(Duration), typeof(MenuBase), new FrameworkPropertyMetadata(DefaultHideDuration));
        public static readonly DependencyProperty IconColumnWidthProperty = DependencyProperty.Register("IconColumnWidth", typeof(double), typeof(MenuBase), new FrameworkPropertyMetadata(0.0));
        private static readonly DependencyProperty IsLoadedProperty = DependencyProperty.Register("IsLoaded", typeof(bool), typeof(MenuBase), new Telerik.Windows.PropertyMetadata(false));
        internal static readonly Telerik.Windows.RoutedEvent IsSelectedChangedEvent = EventManager.RegisterRoutedEvent("IsSelectedChanged", RoutingStrategy.Bubble, typeof(RadRoutedPropertyChangedEventHandler<bool>), typeof(MenuBase));
        private bool mouseOverPopup;
        public static readonly DependencyProperty NotifyOnHeaderClickProperty = DependencyProperty.Register("NotifyOnHeaderClick", typeof(bool), typeof(MenuBase), null);
        public static readonly DependencyProperty ShowDelayProperty = DependencyProperty.Register("ShowDelay", typeof(Duration), typeof(MenuBase), new FrameworkPropertyMetadata(DefaultShowDuration));

        public event RadRoutedEventHandler ItemClick;

        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static MenuBase()
        {
            EventManager.RegisterClassHandler(typeof(MenuBase), IsSelectedChangedEvent, new RadRoutedPropertyChangedEventHandler<bool>(MenuBase.OnIsSelectedChanged));
            EventManager.RegisterClassHandler(typeof(MenuBase), RadMenuItem.ClickEvent, new RadRoutedEventHandler(MenuBase.OnItemClick));
        }

        protected MenuBase()
        {
            base.Loaded += new RoutedEventHandler(this.OnMenuLoaded);
        }

        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            base.ClearContainerForItemOverride(element, item);
            RadMenuItem menuItem = element as RadMenuItem;
            if (menuItem != null)
            {
                menuItem.Menu = null;
            }
        }

        internal virtual void CloseAll()
        {
            this.CurrentSelection = null;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new RadMenuItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return (item is RadMenuItem);
        }

        internal bool MenuNavigate(int index, int direction, bool open)
        {
            DependencyObject control = NavigateToItem(this, index, direction);
            if (control == null)
            {
                return false;
            }
            if (!this.SetCurrentSelection(control, open))
            {
                RadMenuItem item = control as RadMenuItem;
                if ((item != null) && item.IsSeparator)
                {
                    int separatorIndex = base.ItemContainerGenerator.IndexFromContainer(item);
                    control = NavigateToItem(this, separatorIndex + direction, direction);
                    return this.SetCurrentSelection(control, open);
                }
                return false;
            }
            return true;
        }

        internal static DependencyObject NavigateToItem(Telerik.Windows.Controls.ItemsControl itemsControl, int index, int direction)
        {
            int coercedIndex = -1;
            UIElement container = null;
            int count = itemsControl.Items.Count - 1;
            if ((itemsControl.Items.Count > index) && (index >= 0))
            {
                coercedIndex = index;
            }
            else if (index < 0)
            {
                coercedIndex = count;
            }
            else
            {
                coercedIndex = 0;
            }
            for (int i = 0; i < itemsControl.Items.Count; i++)
            {
                bool enabled = false;
                bool visible = false;
                container = itemsControl.ItemContainerGenerator.ContainerFromIndex(coercedIndex) as UIElement;
                if (container != null)
                {
                    Control control = container as Control;
                    if (control != null)
                    {
                        enabled = control.IsEnabled && control.IsTabStop;
                    }
                    visible = container.Visibility == Visibility.Visible;
                }
                if (((container != null) && enabled) && visible)
                {
                    return container;
                }
                coercedIndex += direction;
                if (coercedIndex < 0)
                {
                    coercedIndex = count;
                }
                if (coercedIndex > count)
                {
                    coercedIndex = 0;
                }
            }
            return container;
        }

        private static void OnIsSelectedChanged(object sender, Telerik.Windows.Controls.RadRoutedPropertyChangedEventArgs<Boolean> e)
        {
            RadMenuItem originalSource = e.OriginalSource as RadMenuItem;
            if (originalSource != null)
            {
                MenuBase menu = (MenuBase)sender;
                if (e.NewValue)
                {
                    if ((menu.CurrentSelection != originalSource) && (originalSource.ParentItem == null))
                    {
                        bool submenuOpen = false;
                        if (menu.CurrentSelection != null)
                        {
                            submenuOpen = menu.CurrentSelection.IsSubmenuOpen;
                            menu.CurrentSelection.CloseMenu();
                        }
                        menu.CurrentSelection = originalSource;
                        if ((menu.CurrentSelection != null) && submenuOpen)
                        {
                            MenuItemRole role = menu.CurrentSelection.Role;
                            if (((role == MenuItemRole.SubmenuHeader) || (role == MenuItemRole.TopLevelHeader)) && (menu.CurrentSelection.IsSubmenuOpen != submenuOpen))
                            {
                                if (submenuOpen)
                                {
                                    menu.CurrentSelection.OpenMenu();
                                }
                                else
                                {
                                    menu.CurrentSelection.CloseMenu();
                                }
                            }
                        }
                    }
                }
                else if (menu.CurrentSelection == originalSource)
                {
                    menu.CurrentSelection = null;
                }
                e.Handled = true;
            }
        }

        protected virtual void OnItemClick(RadRoutedEventArgs args)
        {
            if (this.ItemClick != null)
            {
                this.ItemClick(this, args);
            }
        }

        private static void OnItemClick(object sender, RadRoutedEventArgs args)
        {
            (sender as MenuBase).OnItemClick(args);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == Key.Escape)
            {
                this.CloseAll();
                e.Handled = true;
            }
        }

        protected virtual void OnLoaded(RoutedEventArgs e)
        {
            base.SetValue(IsLoadedProperty, true);
        }

        private void OnMenuLoaded(object sender, RoutedEventArgs e)
        {
            this.OnLoaded(e);
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            StyleManager.SetThemeFromParent(element, this);
            RadMenuItem menuItem = element as RadMenuItem;
            if (menuItem != null)
            {
                menuItem.Menu = this;
                menuItem.UpdateRole();
            }
        }

        private bool SetCurrentSelection(DependencyObject control, bool open)
        {
            RadMenuItem menuItem = control as RadMenuItem;
            if ((menuItem == null) || menuItem.IsSeparator)
            {
                return false;
            }
            this.CurrentSelection = menuItem;
            menuItem.FocusOrSelect();
            menuItem.ChangeVisualState(true);
            if (open)
            {
                menuItem.OpenSubmenuWithKeyboard();
            }
            return true;
        }

        public bool ClickToOpen
        {
            get
            {
                return (bool)base.GetValue(ClickToOpenProperty);
            }
            set
            {
                base.SetValue(ClickToOpenProperty, value);
            }
        }

        internal RadMenuItem CurrentSelection
        {
            get
            {
                return this.currentSelection;
            }
            set
            {
                if (this.currentSelection != null)
                {
                    this.currentSelection.IsSelected = false;
                }
                this.currentSelection = value;
                if (this.currentSelection != null)
                {
                    this.currentSelection.IsSelected = true;
                }
            }
        }

        [TypeConverter(typeof(DurationConverter))]
        public Duration HideDelay
        {
            get
            {
                return (Duration)base.GetValue(HideDelayProperty);
            }
            set
            {
                base.SetValue(HideDelayProperty, value);
            }
        }

        public double IconColumnWidth
        {
            get
            {
                return (double)base.GetValue(IconColumnWidthProperty);
            }
            set
            {
                base.SetValue(IconColumnWidthProperty, value);
            }
        }

        internal bool IsKeyboardFocusWithin
        {
            get
            {
                DependencyObject focusedElement = FocusManager.GetFocusedElement() as DependencyObject;
                return ((focusedElement != null) && this.IsAncestorOf(focusedElement));
            }
        }

        internal bool IsLoaded
        {
            get
            {
                return (bool)base.GetValue(IsLoadedProperty);
            }
        }

        internal bool MouseOverPopup
        {
            get
            {
                return this.mouseOverPopup;
            }
            set
            {
                this.mouseOverPopup = value;
            }
        }

        public bool NotifyOnHeaderClick
        {
            get
            {
                return (bool)base.GetValue(NotifyOnHeaderClickProperty);
            }
            set
            {
                base.SetValue(NotifyOnHeaderClickProperty, value);
            }
        }

        [TypeConverter(typeof(DurationConverter))]
        public Duration ShowDelay
        {
            get
            {
                return (Duration)base.GetValue(ShowDelayProperty);
            }
            set
            {
                base.SetValue(ShowDelayProperty, value);
            }
        }
    }
}

