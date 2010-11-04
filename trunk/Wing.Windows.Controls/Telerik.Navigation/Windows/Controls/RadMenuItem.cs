namespace Telerik.Windows.Controls
{
    using System;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Automation.Peers;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Threading;
    using Telerik.Windows;
    using Telerik.Windows.Controls.Animation;
    using Telerik.Windows.Controls.Design;
    using Telerik.Windows.Controls.Primitives;

    [TemplatePart(Name="PART_Popup", Type=typeof(System.Windows.Controls.Primitives.Popup)), DefaultProperty("StaysOpenOnClick"), StyleTypedProperty(Property="ItemContainerStyle", StyleTargetType=typeof(RadMenuItem)), DefaultEvent("Click"), TemplateVisualState(Name="Checked", GroupName="CheckStateGroup"), TemplateVisualState(Name="Unchecked", GroupName="CheckStateGroup"), TemplateVisualState(Name="HideIcon", GroupName="CheckStateGroup"), TemplateVisualState(Name="Highlighted", GroupName="CommonStateGroup"), TemplateVisualState(Name="Focused", GroupName="CommonStateGroup")]
    public class RadMenuItem : HeaderedItemsControl, ICommandSource
    {
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        public static readonly Telerik.Windows.RoutedEvent CheckedEvent = EventManager.RegisterRoutedEvent("Checked", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(RadMenuItem));
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        public static readonly Telerik.Windows.RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble, typeof(RadRoutedEventHandler), typeof(RadMenuItem));
        private DispatcherTimer closeTimer;
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(RadMenuItem), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadMenuItem.OnCommandParameterChanged)));
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(RadMenuItem), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadMenuItem.OnCommandChanged)));
        public static readonly DependencyProperty CommandTargetProperty = DependencyProperty.Register("CommandTarget", typeof(UIElement), typeof(RadMenuItem), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadMenuItem.OnCommandTargetChanged)));
        private RadMenuItem currentSelection;
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(object), typeof(RadMenuItem), new System.Windows.PropertyMetadata(null, new PropertyChangedCallback(RadMenuItem.OnIconChanged)));
        public static readonly DependencyProperty IconTemplateProperty = DependencyProperty.Register("IconTemplate", typeof(DataTemplate), typeof(RadMenuItem), null);
        public static readonly DependencyProperty IsCheckableProperty = DependencyProperty.Register("IsCheckable", typeof(bool), typeof(RadMenuItem), new System.Windows.PropertyMetadata(false, new PropertyChangedCallback(RadMenuItem.OnIsCheckableChanged)));
        public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register("IsChecked", typeof(bool), typeof(RadMenuItem), new System.Windows.PropertyMetadata(false, new PropertyChangedCallback(RadMenuItem.OnIsCheckedChanged)));
        public static readonly DependencyProperty IsHighlightedProperty;
        private static readonly DependencyPropertyKey IsHighlightedPropertyKey = DependencyPropertyExtensions.RegisterReadOnly("IsHighlighted", typeof(bool), typeof(RadMenuItem), new Telerik.Windows.PropertyMetadata(false, new PropertyChangedCallback(RadMenuItem.OnIsHighlighted)));
        internal static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(RadMenuItem), new System.Windows.PropertyMetadata(false, new PropertyChangedCallback(RadMenuItem.OnIsSelectedChanged)));
        public static readonly DependencyProperty IsSeparatorProperty = DependencyProperty.Register("IsSeparator", typeof(bool), typeof(RadMenuItem), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(RadMenuItem.OnIsSeparatorChanged)));
        public static readonly DependencyProperty IsSubmenuOpenProperty;
        private static readonly DependencyPropertyKey IsSubmenuOpenPropertyKey = DependencyPropertyExtensions.RegisterReadOnly("IsSubmenuOpen", typeof(bool), typeof(RadMenuItem), new Telerik.Windows.PropertyMetadata(false, new PropertyChangedCallback(RadMenuItem.OnIsSubmenuOpenChanged)));
        private RadMenuItem lastSubMenuHeaderSelected;
        private bool loaded;
        public static readonly DependencyProperty MenuProperty;
        private static readonly DependencyPropertyKey MenuPropertyKey = DependencyPropertyExtensions.RegisterReadOnly("Menu", typeof(MenuBase), typeof(RadMenuItem), null);
        private bool mouseOver;
        private DispatcherTimer openTimer;
        private static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(RadMenuItem), new Telerik.Windows.PropertyMetadata(Orientation.Horizontal, new PropertyChangedCallback(RadMenuItem.OnOrientationChanged)));
        public static readonly DependencyProperty RoleProperty;
        private static readonly DependencyPropertyKey RolePropertyKey = DependencyPropertyExtensions.RegisterReadOnly("Role", typeof(MenuItemRole), typeof(RadMenuItem), new Telerik.Windows.PropertyMetadata(MenuItemRole.SubmenuItem, new PropertyChangedCallback(RadMenuItem.OnRoleChanged)));
        public static readonly DependencyProperty SeparatorTemplateKeyProperty = DependencyProperty.Register("SeparatorTemplateKey", typeof(ControlTemplate), typeof(RadMenuItem), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadMenuItem.OnTemplateChanged)));
        public static readonly DependencyProperty StaysOpenOnClickProperty = DependencyProperty.Register("StaysOpenOnClick", typeof(bool), typeof(RadMenuItem), new Telerik.Windows.PropertyMetadata(false));
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        public static readonly Telerik.Windows.RoutedEvent SubmenuClosedEvent = EventManager.RegisterRoutedEvent("SubmenuClosed", RoutingStrategy.Bubble, typeof(RadRoutedEventHandler), typeof(RadMenuItem));
        public static readonly DependencyProperty SubmenuHeaderTemplateKeyProperty = DependencyProperty.Register("SubmenuHeaderTemplateKey", typeof(ControlTemplate), typeof(RadMenuItem), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadMenuItem.OnTemplateChanged)));
        public static readonly DependencyProperty SubmenuItemTemplateKeyProperty = DependencyProperty.Register("SubmenuItemTemplateKey", typeof(ControlTemplate), typeof(RadMenuItem), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadMenuItem.OnTemplateChanged)));
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        public static readonly Telerik.Windows.RoutedEvent SubmenuOpenedEvent = EventManager.RegisterRoutedEvent("SubmenuOpened", RoutingStrategy.Bubble, typeof(RadRoutedEventHandler), typeof(RadMenuItem));
        private System.Windows.Controls.Primitives.Popup submenuPopup;
        public static readonly DependencyProperty TopLevelHeaderTemplateKeyProperty = DependencyProperty.Register("TopLevelHeaderTemplateKey", typeof(ControlTemplate), typeof(RadMenuItem), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadMenuItem.OnTemplateChanged)));
        public static readonly DependencyProperty TopLevelItemTemplateKeyProperty = DependencyProperty.Register("TopLevelItemTemplateKey", typeof(ControlTemplate), typeof(RadMenuItem), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadMenuItem.OnTemplateChanged)));
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        public static readonly Telerik.Windows.RoutedEvent UncheckedEvent = EventManager.RegisterRoutedEvent("Unchecked", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(RadMenuItem));
        private PopupWrapper wrapper;

        [Telerik.Windows.Controls.SRCategory("Behavior")]
        public event RoutedEventHandler Checked
        {
            add
            {
                this.AddHandler(CheckedEvent, value);
            }
            remove
            {
                this.RemoveHandler(CheckedEvent, value);
            }
        }

        [Telerik.Windows.Controls.SRCategory("Behavior")]
        public event RadRoutedEventHandler Click
        {
            add
            {
                this.AddHandler(ClickEvent, value);
            }
            remove
            {
                this.RemoveHandler(ClickEvent, value);
            }
        }

        [Telerik.Windows.Controls.SRCategory("Behavior")]
        public event RadRoutedEventHandler SubmenuClosed
        {
            add
            {
                this.AddHandler(SubmenuClosedEvent, value);
            }
            remove
            {
                this.RemoveHandler(SubmenuClosedEvent, value);
            }
        }

        [Telerik.Windows.Controls.SRCategory("Behavior")]
        public event RadRoutedEventHandler SubmenuOpened
        {
            add
            {
                this.AddHandler(SubmenuOpenedEvent, value);
            }
            remove
            {
                this.RemoveHandler(SubmenuOpenedEvent, value);
            }
        }

        [Telerik.Windows.Controls.SRCategory("Behavior")]
        public event RoutedEventHandler Unchecked
        {
            add
            {
                this.AddHandler(UncheckedEvent, value);
            }
            remove
            {
                this.RemoveHandler(UncheckedEvent, value);
            }
        }

        static RadMenuItem()
        {
            EventManager.RegisterClassHandler(typeof(RadMenuItem), MenuBase.IsSelectedChangedEvent, new RadRoutedPropertyChangedEventHandler<bool>(RadMenuItem.OnIsSelectedChanged));
            RoleProperty = RolePropertyKey.DependencyProperty;
            IsSubmenuOpenProperty = IsSubmenuOpenPropertyKey.DependencyProperty;
            IsHighlightedProperty = IsHighlightedPropertyKey.DependencyProperty;
            MenuProperty = MenuPropertyKey.DependencyProperty;
        }

        public RadMenuItem()
        {
            base.DefaultStyleKey = typeof(RadMenuItem);
            base.Loaded += new RoutedEventHandler(this.OnLoaded);
        }

        private void BindEvents()
        {
            if (this.submenuPopup != null)
            {
                this.submenuPopup.Opened += new EventHandler(this.OnPopupOpened);
                this.submenuPopup.Closed += new EventHandler(this.OnPopupClosed);
                if (this.submenuPopup.Child != null)
                {
                    this.submenuPopup.Child.MouseEnter += new MouseEventHandler(this.PopupMouseEnter);
                    this.submenuPopup.Child.MouseLeave += new MouseEventHandler(this.PopupMouseLeave);
                }
            }
        }

        private void CanExecuteApply()
        {
            if (this.Command != null)
            {
                RoutedCommand routedCommand = this.Command as RoutedCommand;
                if (routedCommand == null)
                {
                    base.IsEnabled = this.Command.CanExecute(this.CommandParameter);
                }
                else
                {
                    base.IsEnabled = routedCommand.CanExecute(this.CommandParameter, this.CommandTarget ?? this);
                }
            }
        }

        private void CanExecuteChanged(object sender, EventArgs e)
        {
            this.CanExecuteApply();
        }

        private void ChangeCommand(ICommand oldCommand, ICommand newCommand)
        {
            if (oldCommand != null)
            {
                oldCommand.CanExecuteChanged -= new EventHandler(this.CanExecuteChanged);
            }
            if (newCommand != null)
            {
                newCommand.CanExecuteChanged += new EventHandler(this.CanExecuteChanged);
            }
            this.CanExecuteApply();
        }

        private void ChangeTemplate(MenuItemRole role)
        {
            ControlTemplate newTemplate = null;
            switch (role)
            {
                case MenuItemRole.TopLevelItem:
                    newTemplate = this.TopLevelItemTemplateKey;
                    break;

                case MenuItemRole.TopLevelHeader:
                    newTemplate = this.TopLevelHeaderTemplateKey;
                    break;

                case MenuItemRole.SubmenuItem:
                    newTemplate = this.SubmenuItemTemplateKey;
                    break;

                case MenuItemRole.SubmenuHeader:
                    newTemplate = this.SubmenuHeaderTemplateKey;
                    break;

                case MenuItemRole.Separator:
                    newTemplate = this.SeparatorTemplateKey;
                    break;
            }
            this.Role = role;
            if (newTemplate != null)
            {
                base.Template = newTemplate;
                base.ApplyTemplate();
            }
        }

        protected internal override void ChangeVisualState(bool useTransitions)
        {
            if (this.IsHighlighted || this.IsSubmenuOpen)
            {
                this.GoToState(useTransitions, new string[] { "Highlighted", "Normal" });
            }
            else
            {
                this.GoToState(useTransitions, new string[] { "Normal" });
            }
            if (this.IsChecked)
            {
                this.GoToState(useTransitions, new string[] { "Checked", "Normal" });
            }
            else if (this.Icon == null)
            {
                this.GoToState(useTransitions, new string[] { "HideIcon", "Normal" });
            }
            else
            {
                this.GoToState(useTransitions, new string[] { "Unchecked" });
            }
            if (this.IsSelected && base.IsEnabled)
            {
                this.GoToState(useTransitions, new string[] { "Focused", "Unfocused" });
            }
            else
            {
                this.GoToState(useTransitions, new string[] { "Unfocused" });
            }
            if (!base.IsEnabled)
            {
                this.GoToState(useTransitions, new string[] { "Disabled", "Normal" });
            }
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

        private void ClickHeader(MenuItemRole role)
        {
            this.FocusOrSelect();
            if (((role == MenuItemRole.TopLevelHeader) && this.ClickToOpen) && this.IsSubmenuOpen)
            {
                this.CloseMenu();
            }
            else if (!this.IsSubmenuOpen)
            {
                this.OpenMenu();
            }
        }

        private void ClickItem()
        {
            this.OnClick();
        }

        internal void CloseMenu()
        {
            if (this.lastSubMenuHeaderSelected != null)
            {
                this.lastSubMenuHeaderSelected.CloseMenu();
                this.lastSubMenuHeaderSelected = null;
            }
            this.StopOpenTimer();
            this.StopCloseTimer();
            if (this.IsSubmenuOpen)
            {
                this.IsSubmenuOpen = false;
            }
        }

        private void CloseSubMenu()
        {
            if (this.closeTimer == null)
            {
                this.closeTimer = new DispatcherTimer();
                this.closeTimer.Tick += new EventHandler(this.OnCloseTimerTick);
            }
            StartCloseTimer(this.closeTimer, this.GetDuration(false));
        }

        private void ExecuteCommand()
        {
            if (this.Command != null)
            {
                RoutedCommand routedCommand = this.Command as RoutedCommand;
                if (routedCommand == null)
                {
                    this.Command.Execute(this.CommandParameter);
                }
                else
                {
                    routedCommand.Execute(this.CommandParameter, this.CommandTarget ?? this);
                }
            }
        }

        internal void FocusOrSelect()
        {
            base.Focus();
            if (!this.IsSelected)
            {
                this.IsSelected = true;
            }
            if (this.IsSelected && !this.IsHighlighted)
            {
                this.IsHighlighted = true;
            }
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new RadMenuItem();
        }

        private Duration GetDuration(bool open)
        {
            RadMenu radMenu = this.Menu as RadMenu;
            if ((radMenu != null) && this.IsTopHeader)
            {
                if (!open)
                {
                    return radMenu.TopLevelHideDelay;
                }
                return radMenu.TopLevelShowDelay;
            }
            if (!open)
            {
                return this.HideDelay;
            }
            return this.ShowDelay;
        }

        private static object GetItemOrContainerFromContainer(DependencyObject container)
        {
            object itemOrContainer = container;
            Telerik.Windows.Controls.ItemsControl owner = Telerik.Windows.Controls.ItemsControl.ItemsControlFromItemContainer(container);
            if (owner != null)
            {
                itemOrContainer = owner.ItemContainerGenerator.ItemFromContainer(container);
            }
            return itemOrContainer;
        }

        private void GetTemplateParts()
        {
            if (this.submenuPopup != null)
            {
                if (this.wrapper != null)
                {
                    if (this.IsSubmenuOpen)
                    {
                        this.CloseMenu();
                        this.wrapper.HidePopup();
                    }
                    this.wrapper.ClickedOutsidePopup -= new EventHandler(this.OnClickedOutsidePopup);
                    this.wrapper.ClearReferences();
                    this.wrapper = null;
                }
                this.submenuPopup.Child = null;
            }
            this.submenuPopup = base.GetTemplateChild("PART_Popup") as System.Windows.Controls.Primitives.Popup;
            if (this.submenuPopup == null)
            {
                Telerik.Windows.Controls.Primitives.Popup radPopup = base.GetTemplateChild("PART_Popup") as Telerik.Windows.Controls.Primitives.Popup;
                if (radPopup != null)
                {
                    this.submenuPopup = radPopup.RealPopup;
                    this.submenuPopup.HorizontalOffset = radPopup.HorizontalOffset;
                    this.submenuPopup.VerticalOffset = radPopup.VerticalOffset;
                }
            }
            if (this.submenuPopup != null)
            {
                if (this.IsTopHeader)
                {
                    this.wrapper = new PopupWrapper(this.submenuPopup, this);
                    this.wrapper.ClipAroundElement = this.Menu;
                    this.wrapper.CatchClickOutsidePopup = true;
                    this.wrapper.ClickedOutsidePopup += new EventHandler(this.OnClickedOutsidePopup);
                    this.wrapper.Placement = Telerik.Windows.Controls.PlacementMode.Bottom;
                    this.wrapper.HorizontalOffset = this.submenuPopup.HorizontalOffset;
                    this.wrapper.VerticalOffset = this.submenuPopup.VerticalOffset;
                    RadMenu menu = Telerik.Windows.Controls.ItemsControl.ItemsControlFromItemContainer(this) as RadMenu;
                    if (menu != null)
                    {
                        base.SetBinding(OrientationProperty, new Binding("Orientation") { Source = menu });
                    }
                }
                else if (this.IsSubHeader)
                {
                    this.wrapper = new PopupWrapper(this.submenuPopup, this);
                    this.wrapper.HorizontalOffset = this.submenuPopup.HorizontalOffset;
                    this.wrapper.VerticalOffset = this.submenuPopup.VerticalOffset;
                    this.wrapper.Placement = Telerik.Windows.Controls.PlacementMode.Right;
                }
            }
        }

        private static RadMenuItem GetTopLevelItem(RadMenuItem item)
        {
            RadMenuItem topLevelItem = item;
            while (((topLevelItem != null) && (topLevelItem.Role != MenuItemRole.TopLevelHeader)) && (topLevelItem.Role != MenuItemRole.TopLevelItem))
            {
                topLevelItem = Telerik.Windows.Controls.ItemsControl.ItemsControlFromItemContainer(topLevelItem) as RadMenuItem;
            }
            return topLevelItem;
        }

        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId="stateNames"), SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId="useTransitions"), SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        private void GoToState(bool useTransitions, params string[] stateNames)
        {
            if (stateNames != null)
            {
                foreach (string str in stateNames)
                {
                    if (VisualStateManager.GoToState(this, str, useTransitions))
                    {
                        break;
                    }
                }
            }
        }

        private bool HandleDownKey(MenuItemRole role)
        {
            bool handled = false;
            if (((role == MenuItemRole.SubmenuHeader) && this.IsSubmenuOpen) && (this.CurrentSelection == null))
            {
                this.NavigateToIndex(0, 1);
                handled = true;
            }
            return handled;
        }

        private bool HandleEnterKey(MenuItemRole role)
        {
            bool handled = false;
            switch (role)
            {
                case MenuItemRole.TopLevelItem:
                case MenuItemRole.SubmenuItem:
                    break;

                case MenuItemRole.TopLevelHeader:
                case MenuItemRole.SubmenuHeader:
                    if (!this.Menu.NotifyOnHeaderClick)
                    {
                        goto Label_0031;
                    }
                    break;

                default:
                    goto Label_0031;
            }
            this.ClickItem();
            handled = true;
        Label_0031:
            if (((role != MenuItemRole.TopLevelHeader) || (this.Menu.CurrentSelection == null)) && ((role != MenuItemRole.SubmenuHeader) || this.IsSubmenuOpen))
            {
                return handled;
            }
            this.OpenSubmenuWithKeyboard();
            return true;
        }

        private bool HandleEscapeKey()
        {
            bool handled = false;
            RadMenuItem parent = this.ParentItem;
            if ((parent != null) && parent.IsSubmenuOpen)
            {
                parent.FocusOrSelect();
                parent.CloseMenu();
                parent.IsHighlighted = true;
                parent.ChangeVisualState(true);
                handled = true;
            }
            return handled;
        }

        private bool HandleLeftKey()
        {
            bool handled = false;
            if ((this.Role == MenuItemRole.SubmenuHeader) && this.IsSubmenuOpen)
            {
                this.CloseMenu();
                this.IsHighlighted = true;
                return true;
            }
            RadMenuItem parent = this.ParentItem;
            if (((parent != null) && parent.IsSubmenuOpen) && (parent.Role == MenuItemRole.SubmenuHeader))
            {
                parent.CloseMenu();
                parent.IsHighlighted = true;
                parent.ChangeVisualState(true);
                handled = true;
            }
            return handled;
        }

        internal void HandleMouseDown()
        {
            MenuItemRole role = this.Role;
            switch (role)
            {
                case MenuItemRole.TopLevelHeader:
                case MenuItemRole.SubmenuHeader:
                    this.ClickHeader(role);
                    break;
            }
            this.ChangeVisualState(true);
        }

        internal void HandleMouseEnter()
        {
            this.IsMouseOver = true;
            this.IsHighlighted = true;
            switch (this.Role)
            {
                case MenuItemRole.TopLevelItem:
                case MenuItemRole.TopLevelHeader:
                    if (!this.ClickToOpen || this.MenuIsSelected)
                    {
                        this.OpenSubMenu();
                    }
                    break;

                case MenuItemRole.SubmenuItem:
                case MenuItemRole.SubmenuHeader:
                    this.OpenSubMenu();
                    break;
            }
            this.ChangeVisualState(true);
        }

        internal void HandleMouseLeave()
        {
            this.IsMouseOver = false;
            MenuItemRole role = this.Role;
            switch (role)
            {
                case MenuItemRole.TopLevelHeader:
                    if (!this.IsSubmenuOpen)
                    {
                        this.UnSelectOrUnHighlight();
                    }
                    else if (!this.ClickToOpen)
                    {
                        this.CloseSubMenu();
                    }
                    goto Label_0085;

                case MenuItemRole.TopLevelItem:
                    if (!this.ClickToOpen)
                    {
                        this.UnSelectOrUnHighlight();
                    }
                    else if (!this.MenuIsSelected)
                    {
                        this.IsHighlighted = false;
                    }
                    goto Label_0085;

                case MenuItemRole.SubmenuItem:
                    this.UnSelectOrUnHighlight();
                    goto Label_0085;

                case MenuItemRole.SubmenuHeader:
                    if (!this.IsSubmenuOpen)
                    {
                        this.UnSelectOrUnHighlight();
                    }
                    else
                    {
                        if (this.ClickToOpen)
                        {
                            return;
                        }
                        this.CloseSubMenu();
                    }
                    break;
            }
        Label_0085:
            if ((role == MenuItemRole.TopLevelHeader) || (role == MenuItemRole.TopLevelItem))
            {
                this.StopOpenTimer();
                this.IsHighlighted = false;
                this.ChangeVisualState(true);
            }
        }

        internal void HandleMouseUp()
        {
            switch (this.Role)
            {
                case MenuItemRole.TopLevelItem:
                case MenuItemRole.SubmenuItem:
                    break;

                case MenuItemRole.TopLevelHeader:
                case MenuItemRole.SubmenuHeader:
                    if (!this.Menu.NotifyOnHeaderClick)
                    {
                        return;
                    }
                    break;

                default:
                    return;
            }
            this.ClickItem();
            this.ChangeVisualState(true);
        }

        private bool HandleRightKey(MenuItemRole role)
        {
            bool handled = false;
            if ((role == MenuItemRole.SubmenuHeader) && !this.IsSubmenuOpen)
            {
                this.OpenSubmenuWithKeyboard();
                return true;
            }
            if ((role == MenuItemRole.SubmenuHeader) && this.IsSubmenuOpen)
            {
                this.NavigateToIndex(0, 1);
                handled = true;
            }
            return handled;
        }

        private bool HandleUpKey(MenuItemRole role)
        {
            bool handled = false;
            if (((role == MenuItemRole.SubmenuHeader) && this.IsSubmenuOpen) && (this.CurrentSelection == null))
            {
                this.NavigateToIndex(base.Items.Count - 1, -1);
                handled = true;
            }
            return handled;
        }

        private void InitializeComponent()
        {
            this.UnBindEvents();
            this.GetTemplateParts();
            this.BindEvents();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return (item is RadMenuItem);
        }

        private static bool IsMouseOverParent(RadMenuItem item)
        {
            for (RadMenuItem currentItem = item; currentItem != null; currentItem = currentItem.ParentItem)
            {
                if (currentItem.IsMouseOver)
                {
                    return true;
                }
            }
            return false;
        }

        private static FocusNavigationDirection KeyToTraversalDirection(Key key)
        {
            switch (key)
            {
                case Key.Left:
                    return FocusNavigationDirection.Left;

                case Key.Up:
                    return FocusNavigationDirection.Up;

                case Key.Right:
                    return FocusNavigationDirection.Right;

                case Key.Down:
                    return FocusNavigationDirection.Down;
            }
            throw new NotSupportedException();
        }

        private bool MenuItemNavigate(Key key)
        {
            if (((key == Key.Left) || (key == Key.Right)) || ((key == Key.Up) || (key == Key.Down)))
            {
                Telerik.Windows.Controls.ItemsControl control = Telerik.Windows.Controls.ItemsControl.ItemsControlFromItemContainer(this);
                if (control != null)
                {
                    if (!control.HasItems)
                    {
                        return false;
                    }
                    if (((control.Items.Count == 1) && !(control is RadMenu)) && ((key == Key.Up) || (key == Key.Down)))
                    {
                        return true;
                    }
                    object focusedElement = FocusManager.GetFocusedElement();
                    RadMenuItem menuItem = control as RadMenuItem;
                    if (menuItem != null)
                    {
                        menuItem.NavigateTo(KeyToTraversalDirection(key), this);
                    }
                    object obj3 = FocusManager.GetFocusedElement();
                    if ((obj3 != focusedElement) && (obj3 != this))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void NavigateTo(FocusNavigationDirection direction, RadMenuItem menuItem)
        {
            int index = -1;
            RadMenuItem topLevelItem = null;
            RadMenu menu = null;
            switch (direction)
            {
                case FocusNavigationDirection.Left:
                    topLevelItem = GetTopLevelItem(this);
                    if (topLevelItem == null)
                    {
                        break;
                    }
                    menu = topLevelItem.Menu as RadMenu;
                    if ((menu == null) || (menu.CurrentSelection == null))
                    {
                        break;
                    }
                    index = menu.Items.IndexOf(GetItemOrContainerFromContainer(topLevelItem));
                    menu.MenuNavigate(index - 1, -1, true);
                    return;

                case FocusNavigationDirection.Right:
                    topLevelItem = GetTopLevelItem(this);
                    if (topLevelItem == null)
                    {
                        break;
                    }
                    menu = topLevelItem.Menu as RadMenu;
                    if ((menu == null) || (menu.CurrentSelection == null))
                    {
                        break;
                    }
                    index = menu.Items.IndexOf(GetItemOrContainerFromContainer(topLevelItem));
                    menu.MenuNavigate(index + 1, 1, true);
                    return;

                case FocusNavigationDirection.Up:
                    index = base.Items.IndexOf(GetItemOrContainerFromContainer(menuItem));
                    this.NavigateToIndex(index - 1, -1);
                    return;

                case FocusNavigationDirection.Down:
                    index = base.Items.IndexOf(GetItemOrContainerFromContainer(menuItem));
                    this.NavigateToIndex(index + 1, 1);
                    break;

                default:
                    return;
            }
        }

        private bool NavigateToIndex(int index, int direction)
        {
            DependencyObject control = MenuBase.NavigateToItem(this, index, direction);
            if (!this.SetCurrentSelection(control))
            {
                RadMenuItem item = control as RadMenuItem;
                if (item.IsSeparator)
                {
                    int separatorIndex = base.ItemContainerGenerator.IndexFromContainer(item);
                    control = MenuBase.NavigateToItem(this, separatorIndex + direction, direction);
                    return this.SetCurrentSelection(control);
                }
                return false;
            }
            return true;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.InitializeComponent();
            if (this.IsSelected)
            {
                this.FocusOrSelect();
            }
            this.ChangeVisualState(true);
        }

        protected virtual void OnChecked(RadRoutedEventArgs e)
        {
            this.RaiseEvent(e);
        }

        protected virtual void OnClick()
        {
            this.OnClickImpl();
        }

        private void OnClickedOutsidePopup(object sender, EventArgs e)
        {
            this.IsSelected = false;
            if (this.InsideContextMenu && (this.Menu != null))
            {
                this.Menu.CloseAll();
            }
        }

        internal void OnClickImpl()
        {
            this.FocusOrSelect();
            if (this.IsCheckable)
            {
                this.IsChecked = !this.IsChecked;
            }
            this.RaiseEvent(new RadRoutedEventArgs(ClickEvent, this));
            this.ExecuteCommand();
            MenuBase menu = this.Menu;
            if (!this.StaysOpenOnClick && ((this.Role == MenuItemRole.SubmenuItem) || (this.Role == MenuItemRole.TopLevelItem)))
            {
                this.IsMouseOver = false;
                this.IsHighlighted = false;
                this.ChangeVisualState(true);
                if (menu != null)
                {
                    menu.CloseAll();
                }
            }
            if (AutomationPeer.ListenerExists(AutomationEvents.InvokePatternOnInvoked))
            {
                AutomationPeer peer = FrameworkElementAutomationPeer.CreatePeerForElement(this);
                if (peer != null)
                {
                    peer.RaiseAutomationEvent(AutomationEvents.InvokePatternOnInvoked);
                }
            }
        }

        private void OnCloseTimerTick(object sender, EventArgs e)
        {
            this.StopCloseTimer();
            if ((!this.ClickToOpen && (this.Menu != null)) && !this.MouseOverPopup)
            {
                RadMenuItem topLevelItem = GetTopLevelItem(this);
                if ((topLevelItem == null) || !topLevelItem.IsMouseOver)
                {
                    RadMenu menu = this.Menu as RadMenu;
                    if ((menu != null) && menu.WaitForTopLevelHeaderHideDuration)
                    {
                        if (topLevelItem.currentSelection != null)
                        {
                            topLevelItem.currentSelection.CloseMenu();
                            topLevelItem.CloseSubMenu();
                        }
                    }
                    else
                    {
                        this.Menu.CloseAll();
                    }
                }
            }
            if (!this.MouseOverPopup)
            {
                this.CloseMenu();
                this.IsSelected = false;
            }
            else if ((this.ParentItem != null) && (this.ParentItem.CurrentSelection != this))
            {
                this.CloseMenu();
                this.IsSelected = false;
            }
        }

        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadMenuItem menuItem = d as RadMenuItem;
            if (menuItem != null)
            {
                menuItem.ChangeCommand((ICommand) e.OldValue, (ICommand) e.NewValue);
            }
        }

        private static void OnCommandParameterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadMenuItem menuItem = d as RadMenuItem;
            if (menuItem != null)
            {
                menuItem.CanExecuteApply();
            }
        }

        private static void OnCommandTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadMenuItem menuItem = d as RadMenuItem;
            if (menuItem != null)
            {
                menuItem.CanExecuteApply();
            }
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new RadMenuItemAutomationPeer(this);
        }

        private static void OnIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as RadMenuItem).ChangeVisualState(true);
        }

        private static void OnIsCheckableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadMenuItem item = d as RadMenuItem;
            if (item.IsLoaded)
            {
                item.UpdateRole();
            }
        }

        private static void OnIsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadMenuItem item = (RadMenuItem) d;
            if (item.IsLoaded)
            {
                item.UpdateRole();
            }
            if ((bool) e.NewValue)
            {
                item.OnChecked(new RadRoutedEventArgs(CheckedEvent, item));
            }
            else
            {
                item.OnUnchecked(new RadRoutedEventArgs(UncheckedEvent, item));
            }
            item.ChangeVisualState(true);
        }

        private static void OnIsHighlighted(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RadMenuItem) d).ChangeVisualState(true);
        }

        private static void OnIsSelectedChanged(object sender, Telerik.Windows.Controls.RadRoutedPropertyChangedEventArgs<Boolean> e)
        {
            RadMenuItem item = (RadMenuItem) sender;
            RadMenuItem originalSource = e.OriginalSource as RadMenuItem;
            if ((sender != e.OriginalSource) && (originalSource != null))
            {
                if (e.NewValue)
                {
                    if (item.CurrentSelection == originalSource)
                    {
                        item.StopCloseTimer();
                    }
                    if ((item.CurrentSelection != originalSource) && (originalSource.ParentItem == item))
                    {
                        if (item.CurrentSelection != null)
                        {
                            item.CurrentSelection.CloseSubMenu();
                        }
                        item.CurrentSelection = originalSource;
                        item.CurrentSelection.StopCloseTimer();
                        item.StopCloseTimer();
                        RadMenuItem parentMenuItem = item.ParentItem;
                        if (parentMenuItem != null)
                        {
                            parentMenuItem.StopCloseTimer();
                        }
                        if (!item.IsSelected)
                        {
                            item.IsSelected = true;
                        }
                    }
                }
                else if (item.CurrentSelection == originalSource)
                {
                    item.CurrentSelection = null;
                    RadMenuItem parentItem = item.ParentItem;
                    if ((parentItem != null) && !parentItem.IsSelected)
                    {
                        item.CloseMenu();
                    }
                    else if ((!item.ClickToOpen && item.IsSubmenuOpen) && !item.IsMouseOver)
                    {
                        item.CloseSubMenu();
                    }
                }
                e.Handled = true;
            }
        }

        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadMenuItem item = (RadMenuItem) d;
            bool newValue = (bool) e.NewValue;
            if (!newValue)
            {
                item.IsMouseOver = false;
            }
            item.IsHighlighted = newValue;
            item.ChangeVisualState(true);
            if ((bool) e.OldValue)
            {
                item.StopOpenTimer();
                if (item.IsSubmenuOpen && (item.IsTopHeader || item.InsideContextMenu))
                {
                    item.CloseMenu();
                }
            }
            item.RaiseEvent(new Telerik.Windows.Controls.RadRoutedPropertyChangedEventArgs<Boolean>((bool) e.OldValue, (bool) e.NewValue, MenuBase.IsSelectedChangedEvent));
        }

        private static void OnIsSeparatorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadMenuItem item = d as RadMenuItem;
            if (item != null)
            {
                item.IsTabStop = !((bool) e.NewValue);
            }
        }

        private static void OnIsSubmenuOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadMenuItem menuItem = (RadMenuItem) d;
            menuItem.StopOpenTimer();
            menuItem.StopCloseTimer();
            bool newValue = (bool) e.NewValue;
            bool oldValue = (bool) e.OldValue;
            if (newValue)
            {
                menuItem.IsSelected = true;
                menuItem.CurrentSelection = null;
                EventHandler animateOnPopupOpen = null;
                animateOnPopupOpen = delegate (object sender, EventArgs args) {
                    RunExpandAnimation(menuItem);
                    menuItem.wrapper.Opened -= animateOnPopupOpen;
                };
                if (!menuItem.submenuPopup.IsOpen)
                {
                    menuItem.wrapper.ShowPopup();
                    menuItem.wrapper.Opened += animateOnPopupOpen;
                }
                else
                {
                    RunExpandAnimation(menuItem);
                }
            }
            else
            {
                if (menuItem.CurrentSelection != null)
                {
                    menuItem.Focus();
                    if (menuItem.CurrentSelection.IsSubmenuOpen)
                    {
                        menuItem.CurrentSelection.CloseMenu();
                    }
                }
                menuItem.CurrentSelection = null;
                if (!menuItem.IsMouseOver)
                {
                    menuItem.IsHighlighted = false;
                }
                AnimationManager.StopIfRunning(menuItem, "Expand");
                AnimationManager.Play(menuItem, "Collapse", delegate {
                    if (((menuItem.submenuPopup != null) && (menuItem.wrapper != null)) && (menuItem.submenuPopup.IsOpen && !menuItem.IsSubmenuOpen))
                    {
                        menuItem.wrapper.HidePopup();
                    }
                }, new object[0]);
            }
            menuItem.ChangeVisualState(true);
            RadMenuItemAutomationPeer peer = FrameworkElementAutomationPeer.FromElement(menuItem) as RadMenuItemAutomationPeer;
            if (peer != null)
            {
                peer.RaiseExpandCollapseAutomationEvent(oldValue, newValue);
            }
        }

        private void OnItemContainerGeneratorStatusChanged(object sender, EventArgs e)
        {
            if (base.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
            {
                base.ItemContainerGenerator.StatusChanged -= new EventHandler(this.OnItemContainerGeneratorStatusChanged);
                this.NavigateToIndex(0, 1);
            }
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            if (this.IsLoaded || (base.ReadLocalValue(RoleProperty) != DependencyProperty.UnsetValue))
            {
                this.UpdateRole();
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (!e.Handled)
            {
                e.Handled = this.ProcessKey(e.Key);
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.loaded = true;
            this.UpdateRole();
            this.CanExecuteApply();
            this.ChangeVisualState(true);
        }

        private void OnMenuPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if ((e.PropertyName == "Orientation") && (this.wrapper != null))
            {
                RadMenu menu = this.Menu as RadMenu;
                this.wrapper.Placement = (menu.Orientation == Orientation.Horizontal) ? Telerik.Windows.Controls.PlacementMode.Bottom : Telerik.Windows.Controls.PlacementMode.Right;
            }
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            this.HandleMouseEnter();
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            this.HandleMouseLeave();
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (!e.Handled)
            {
                this.HandleMouseDown();
                e.Handled = true;
            }
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            if (!e.Handled)
            {
                this.HandleMouseUp();
                e.Handled = true;
            }
        }

        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            if (!e.Handled)
            {
                this.HandleMouseDown();
                e.Handled = true;
            }
            base.OnMouseRightButtonDown(e);
        }

        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            if (!e.Handled)
            {
                this.HandleMouseUp();
                e.Handled = true;
            }
            base.OnMouseRightButtonUp(e);
        }

        private void OnOpenTimerTick(object sender, EventArgs e)
        {
            if (this.IsMouseOver)
            {
                this.OpenMenu();
            }
            else
            {
                this.IsHighlighted = false;
            }
            this.StopOpenTimer();
        }

        private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadMenuItem radMenuItem = d as RadMenuItem;
            if ((radMenuItem != null) && (radMenuItem.wrapper != null))
            {
                radMenuItem.wrapper.Placement = (((Orientation) e.NewValue) == Orientation.Horizontal) ? Telerik.Windows.Controls.PlacementMode.Bottom : Telerik.Windows.Controls.PlacementMode.Right;
            }
        }

        private void OnPopupClosed(object source, EventArgs e)
        {
            RadMenuItemAutomationPeer peer = FrameworkElementAutomationPeer.CreatePeerForElement(this) as RadMenuItemAutomationPeer;
            if (peer != null)
            {
                peer.RaiseExpandCollapseAutomationEvent(true, false);
            }
            this.OnSubmenuClosed(new RadRoutedEventArgs(SubmenuClosedEvent, this));
        }

        private void OnPopupOpened(object sender, EventArgs e)
        {
            RadMenuItemAutomationPeer peer = FrameworkElementAutomationPeer.CreatePeerForElement(this) as RadMenuItemAutomationPeer;
            if (peer != null)
            {
                peer.RaiseExpandCollapseAutomationEvent(false, true);
            }
            this.OnSubmenuOpened(new RadRoutedEventArgs(SubmenuOpenedEvent, this));
        }

        private static void OnRoleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadMenuItem item = d as RadMenuItem;
            if (((MenuItemRole) e.OldValue) == MenuItemRole.TopLevelHeader)
            {
                item.ClearValue(OrientationProperty);
            }
        }

        protected virtual void OnSubmenuClosed(RadRoutedEventArgs e)
        {
            this.RaiseEvent(e);
        }

        protected virtual void OnSubmenuOpened(RadRoutedEventArgs e)
        {
            this.RaiseEvent(e);
        }

        private void OnSubmenuOpened(object sender, RoutedEventArgs e)
        {
            this.SubmenuOpened -= new RadRoutedEventHandler(this.OnSubmenuOpened);
            this.NavigateToIndex(0, 1);
        }

        private static void OnTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadMenuItem item = d as RadMenuItem;
            if ((item != null) && item.IsLoaded)
            {
                item.ChangeTemplate(item.Role);
            }
        }

        protected virtual void OnUnchecked(RadRoutedEventArgs e)
        {
            this.RaiseEvent(e);
        }

        internal void OpenMenu()
        {
            if (this.IsTopHeader || this.IsSubHeader)
            {
                bool hasParent = this.ParentItem != null;
                if ((this.IsSubHeader && hasParent) && ((this.ParentItem.lastSubMenuHeaderSelected != null) && (this.ParentItem.lastSubMenuHeaderSelected != this)))
                {
                    this.ParentItem.lastSubMenuHeaderSelected.CloseMenu();
                    this.ParentItem.lastSubMenuHeaderSelected.IsSelected = false;
                    this.ParentItem.lastSubMenuHeaderSelected = null;
                }
                if (!hasParent && this.MenuIsSelected)
                {
                    this.Menu.CurrentSelection.CloseMenu();
                    this.Menu.CurrentSelection.IsSelected = false;
                }
                if ((hasParent && this.ParentItem.IsSubmenuOpen) || (this.IsTopHeader || (!hasParent && this.InsideContextMenu)))
                {
                    this.FocusOrSelect();
                    if (!this.IsSubmenuOpen)
                    {
                        this.IsSubmenuOpen = true;
                    }
                    if (hasParent)
                    {
                        this.ParentItem.lastSubMenuHeaderSelected = this;
                    }
                }
            }
        }

        private void OpenSubMenu()
        {
            if (this.IsSubmenuOpen)
            {
                this.StopCloseTimer();
                this.FocusOrSelect();
            }
            else if (this.IsSubHeader || this.IsTopHeader)
            {
                if ((this.ParentItem != null) && (this.ParentItem.CurrentSelection == null))
                {
                    this.FocusOrSelect();
                }
                if (this.openTimer == null)
                {
                    this.openTimer = new DispatcherTimer();
                    this.openTimer.Tick += new EventHandler(this.OnOpenTimerTick);
                }
                else
                {
                    this.openTimer.Stop();
                }
                StartOpenTimer(this.openTimer, this.GetDuration(true));
            }
            else if ((this.Role == MenuItemRole.TopLevelItem) || (this.Role == MenuItemRole.SubmenuItem))
            {
                this.FocusOrSelect();
            }
        }

        internal void OpenSubmenuWithKeyboard()
        {
            if (base.ItemContainerGenerator.Status == GeneratorStatus.NotStarted)
            {
                base.ItemContainerGenerator.StatusChanged += new EventHandler(this.OnItemContainerGeneratorStatusChanged);
            }
            else if ((base.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated) && !this.IsSubmenuOpen)
            {
                this.SubmenuOpened += new RadRoutedEventHandler(this.OnSubmenuOpened);
            }
            if (this.IsSubmenuOpen)
            {
                this.NavigateToIndex(0, 1);
            }
            else
            {
                this.OpenMenu();
            }
        }

        private void PopupMouseEnter(object sender, MouseEventArgs e)
        {
            if (this.Menu != null)
            {
                this.Menu.MouseOverPopup = true;
            }
        }

        private void PopupMouseLeave(object sender, MouseEventArgs e)
        {
            if (this.Menu != null)
            {
                this.Menu.MouseOverPopup = false;
            }
            if (!this.ClickToOpen)
            {
                this.CloseSubMenu();
            }
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            StyleManager.SetThemeFromParent(element, this);
            RadMenuItem menuItem = element as RadMenuItem;
            if (menuItem != null)
            {
                menuItem.Menu = this.Menu;
                menuItem.UpdateRole();
            }
        }

        internal bool ProcessKey(Key key)
        {
            bool handled = false;
            MenuItemRole role = this.Role;
            if (base.FlowDirection == FlowDirection.RightToLeft)
            {
                switch (key)
                {
                    case Key.Left:
                        key = Key.Right;
                        break;

                    case Key.Right:
                        key = Key.Left;
                        break;
                }
            }
            switch (key)
            {
                case Key.Left:
                    handled = this.HandleLeftKey();
                    break;

                case Key.Up:
                    handled = this.HandleUpKey(role);
                    break;

                case Key.Right:
                    handled = this.HandleRightKey(role);
                    break;

                case Key.Down:
                    handled = this.HandleDownKey(role);
                    break;

                case Key.Escape:
                    handled = this.HandleEscapeKey();
                    break;

                case Key.Enter:
                    handled = this.HandleEnterKey(role);
                    break;
            }
            if (!handled)
            {
                handled = this.MenuItemNavigate(key);
            }
            if (handled)
            {
                this.ChangeVisualState(true);
            }
            return handled;
        }

        private static void RunExpandAnimation(RadMenuItem menuItem)
        {
            AnimationManager.StopIfRunning(menuItem, "Collapse");
            RadMenu menu = menuItem.Menu as RadMenu;
            Orientation orientation = Orientation.Horizontal;
            SlideMode slideMode = SlideMode.Top;
            if (menu != null)
            {
                orientation = (menuItem.IsTopHeader && (menu.Orientation == Orientation.Horizontal)) ? Orientation.Vertical : Orientation.Horizontal;
                Telerik.Windows.Controls.PlacementMode placementMode = Telerik.Windows.Controls.PlacementMode.Absolute;
                if (menuItem.wrapper != null)
                {
                    placementMode = menuItem.wrapper.ActtualPlacement;
                }
                switch (placementMode)
                {
                    case Telerik.Windows.Controls.PlacementMode.Left:
                    case Telerik.Windows.Controls.PlacementMode.Top:
                        if (menuItem.FlowDirection == FlowDirection.LeftToRight)
                        {
                            slideMode = SlideMode.Bottom;
                        }
                        break;
                }
            }
            AnimationManager.Play(menuItem, "Expand", null, new object[] { orientation, slideMode });
        }

        private bool SetCurrentSelection(DependencyObject control)
        {
            RadMenuItem menuItem = control as RadMenuItem;
            if ((menuItem != null) && !menuItem.IsSeparator)
            {
                this.CurrentSelection = menuItem;
                menuItem.FocusOrSelect();
                menuItem.ChangeVisualState(true);
                return true;
            }
            return false;
        }

        private static void StartCloseTimer(DispatcherTimer timer, Duration interval)
        {
            timer.Interval = interval.TimeSpan;
            timer.Start();
        }

        private static void StartOpenTimer(DispatcherTimer timer, Duration interval)
        {
            timer.Interval = interval.TimeSpan;
            timer.Start();
        }

        private void StopCloseTimer()
        {
            if (this.closeTimer != null)
            {
                this.closeTimer.Tick -= new EventHandler(this.OnCloseTimerTick);
            }
            StopTimer(ref this.closeTimer);
        }

        private void StopOpenTimer()
        {
            if (this.openTimer != null)
            {
                this.openTimer.Tick -= new EventHandler(this.OnOpenTimerTick);
            }
            StopTimer(ref this.openTimer);
        }

        private static void StopTimer(ref DispatcherTimer timer)
        {
            if (timer != null)
            {
                timer.Stop();
                timer = null;
            }
        }

        private void UnBindEvents()
        {
            if (this.submenuPopup != null)
            {
                this.submenuPopup.Opened -= new EventHandler(this.OnPopupOpened);
                this.submenuPopup.Closed -= new EventHandler(this.OnPopupClosed);
                if (this.submenuPopup.Child != null)
                {
                    this.submenuPopup.Child.MouseEnter -= new MouseEventHandler(this.PopupMouseEnter);
                    this.submenuPopup.Child.MouseLeave -= new MouseEventHandler(this.PopupMouseLeave);
                }
            }
        }

        private void UnSelectOrUnHighlight()
        {
            if (this.IsSelected)
            {
                this.IsSelected = false;
            }
            else
            {
                this.IsHighlighted = false;
            }
        }

        internal void UpdateRole()
        {
            MenuItemRole role;
            RadMenu parentMenu = Telerik.Windows.Controls.ItemsControl.ItemsControlFromItemContainer(this) as RadMenu;
            if (this.IsSeparator)
            {
                role = MenuItemRole.Separator;
            }
            else if (!this.IsCheckable && base.HasItems)
            {
                if (parentMenu != null)
                {
                    role = MenuItemRole.TopLevelHeader;
                }
                else
                {
                    role = MenuItemRole.SubmenuHeader;
                }
            }
            else if (parentMenu != null)
            {
                role = MenuItemRole.TopLevelItem;
            }
            else
            {
                role = MenuItemRole.SubmenuItem;
            }
            if (role != this.Role)
            {
                this.ChangeTemplate(role);
            }
        }

        private bool ClickToOpen
        {
            get
            {
                MenuBase menu = this.Menu;
                return ((menu != null) && menu.ClickToOpen);
            }
        }

        [TypeConverter(typeof(CommandConverter))]
        public ICommand Command
        {
            get
            {
                return (ICommand) base.GetValue(CommandProperty);
            }
            set
            {
                base.SetValue(CommandProperty, value);
            }
        }

        public object CommandParameter
        {
            get
            {
                return base.GetValue(CommandParameterProperty);
            }
            set
            {
                base.SetValue(CommandParameterProperty, value);
            }
        }

        public UIElement CommandTarget
        {
            get
            {
                return (UIElement) base.GetValue(CommandTargetProperty);
            }
            set
            {
                base.SetValue(CommandTargetProperty, value);
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
                if (this.currentSelection != value)
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
        }

        private Duration HideDelay
        {
            get
            {
                if (this.Menu != null)
                {
                    return this.Menu.HideDelay;
                }
                return MenuBase.DefaultHideDuration;
            }
        }

        [Telerik.Windows.Controls.SRCategory("Appearance"), TypeConverter(typeof(ImageConverter))]
        public object Icon
        {
            get
            {
                return base.GetValue(IconProperty);
            }
            set
            {
                base.SetValue(IconProperty, value);
            }
        }

        public DataTemplate IconTemplate
        {
            get
            {
                return (DataTemplate) base.GetValue(IconTemplateProperty);
            }
            set
            {
                base.SetValue(IconTemplateProperty, value);
            }
        }

        internal bool InsideContextMenu
        {
            get
            {
                return RadContextMenu.GetInsideContextMenu(this);
            }
        }

        [Telerik.Windows.Controls.SRCategory("Behavior")]
        public bool IsCheckable
        {
            get
            {
                return (bool) base.GetValue(IsCheckableProperty);
            }
            set
            {
                base.SetValue(IsCheckableProperty, value);
            }
        }

        [Telerik.Windows.Controls.SRCategory("Appearance")]
        public bool IsChecked
        {
            get
            {
                return (bool) base.GetValue(IsCheckedProperty);
            }
            set
            {
                base.SetValue(IsCheckedProperty, value);
            }
        }

        [Browsable(false), Telerik.Windows.Controls.SRCategory("Appearance")]
        public bool IsHighlighted
        {
            get
            {
                return (bool) base.GetValue(IsHighlightedProperty);
            }
            protected set
            {
                this.SetValue(IsHighlightedPropertyKey, value);
            }
        }

        internal bool IsLoaded
        {
            get
            {
                return this.loaded;
            }
        }

        internal bool IsMouseOver
        {
            get
            {
                return this.mouseOver;
            }
            private set
            {
                this.mouseOver = value;
            }
        }

        internal bool IsSelected
        {
            get
            {
                return (bool) base.GetValue(IsSelectedProperty);
            }
            set
            {
                base.SetValue(IsSelectedProperty, value);
            }
        }

        public bool IsSeparator
        {
            get
            {
                return (bool) base.GetValue(IsSeparatorProperty);
            }
            set
            {
                base.SetValue(IsSeparatorProperty, value);
            }
        }

        internal bool IsSubHeader
        {
            get
            {
                return (this.Role == MenuItemRole.SubmenuHeader);
            }
        }

        internal bool IsSubItem
        {
            get
            {
                return (this.Role == MenuItemRole.SubmenuItem);
            }
        }

        [Telerik.Windows.Controls.SRCategory("Appearance"), Browsable(false)]
        public bool IsSubmenuOpen
        {
            get
            {
                return (bool) base.GetValue(IsSubmenuOpenProperty);
            }
            internal set
            {
                this.SetValue(IsSubmenuOpenPropertyKey, BooleanBoxes.Box(value));
            }
        }

        internal bool IsTopHeader
        {
            get
            {
                return (this.Role == MenuItemRole.TopLevelHeader);
            }
        }

        internal bool IsTopItem
        {
            get
            {
                return (this.Role == MenuItemRole.TopLevelItem);
            }
        }

        public MenuBase Menu
        {
            get
            {
                return (MenuBase) base.GetValue(MenuProperty);
            }
            internal set
            {
                this.SetValue(MenuPropertyKey, value);
            }
        }

        private bool MenuIsSelected
        {
            get
            {
                RadMenu menu = this.Menu as RadMenu;
                if (menu == null)
                {
                    return false;
                }
                return ((menu.CurrentSelection != null) && (menu.CurrentSelection != this));
            }
        }

        private bool MouseOverPopup
        {
            get
            {
                return ((this.Menu != null) && this.Menu.MouseOverPopup);
            }
        }

        internal RadMenuItem ParentItem
        {
            get
            {
                return (Telerik.Windows.Controls.ItemsControl.ItemsControlFromItemContainer(this) as RadMenuItem);
            }
        }

        [Telerik.Windows.Controls.SRCategory("Behavior")]
        public MenuItemRole Role
        {
            get
            {
                return (MenuItemRole) base.GetValue(RoleProperty);
            }
            private set
            {
                this.SetValue(RolePropertyKey, value);
            }
        }

        public ControlTemplate SeparatorTemplateKey
        {
            get
            {
                return (ControlTemplate) base.GetValue(SeparatorTemplateKeyProperty);
            }
            set
            {
                base.SetValue(SeparatorTemplateKeyProperty, value);
            }
        }

        private Duration ShowDelay
        {
            get
            {
                if (this.Menu != null)
                {
                    return this.Menu.ShowDelay;
                }
                return MenuBase.DefaultShowDuration;
            }
        }

        [Telerik.Windows.Controls.SRCategory("Behavior")]
        public bool StaysOpenOnClick
        {
            get
            {
                return (bool) base.GetValue(StaysOpenOnClickProperty);
            }
            set
            {
                base.SetValue(StaysOpenOnClickProperty, value);
            }
        }

        public ControlTemplate SubmenuHeaderTemplateKey
        {
            get
            {
                return (ControlTemplate) base.GetValue(SubmenuHeaderTemplateKeyProperty);
            }
            set
            {
                base.SetValue(SubmenuHeaderTemplateKeyProperty, value);
            }
        }

        public ControlTemplate SubmenuItemTemplateKey
        {
            get
            {
                return (ControlTemplate) base.GetValue(SubmenuItemTemplateKeyProperty);
            }
            set
            {
                base.SetValue(SubmenuItemTemplateKeyProperty, value);
            }
        }

        public ControlTemplate TopLevelHeaderTemplateKey
        {
            get
            {
                return (ControlTemplate) base.GetValue(TopLevelHeaderTemplateKeyProperty);
            }
            set
            {
                base.SetValue(TopLevelHeaderTemplateKeyProperty, value);
            }
        }

        public ControlTemplate TopLevelItemTemplateKey
        {
            get
            {
                return (ControlTemplate) base.GetValue(TopLevelItemTemplateKeyProperty);
            }
            set
            {
                base.SetValue(TopLevelItemTemplateKeyProperty, value);
            }
        }

        private enum FocusNavigationDirection
        {
            Next,
            Previous,
            First,
            Last,
            Left,
            Right,
            Up,
            Down
        }
    }
}

