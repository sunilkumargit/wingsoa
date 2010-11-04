namespace Telerik.Windows.Controls
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Automation.Peers;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Shapes;
    using Telerik.Windows;
    using Telerik.Windows.Controls.Primitives;

    [TemplateVisualState(GroupName="PlacementStates", Name="HorizontalBottom"), TemplateVisualState(GroupName="PlacementStates", Name="HorizontalRight"), TemplateVisualState(GroupName="PlacementStates", Name="HorizontalTop"), TemplatePart(Name="FocusRectangleElement", Type=typeof(Rectangle)), TemplateVisualState(GroupName="PlacementStates", Name="HorizontalLeft"), DefaultProperty("IsSelected"), TemplateVisualState(GroupName="PlacementStates", Name="VerticalRight"), TemplateVisualState(GroupName="PlacementStates", Name="VerticalTop"), TemplateVisualState(GroupName="PlacementStates", Name="VerticalBottom"), TemplateVisualState(GroupName="PlacementStates", Name="VerticalLeft"), TemplateVisualState(Name="Selected", GroupName="CommonStates"), TemplateVisualState(Name="MouseOver", GroupName="CommonStates"), TemplateVisualState(Name="SelectedMouseOver", GroupName="CommonStates")]
    public class RadTabItem : HeaderedContentControl
    {
        public static readonly DependencyProperty BottomTemplateProperty = DependencyProperty.Register("BottomTemplate", typeof(ControlTemplate), typeof(RadTabItem), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadTabItem.OnBottomTemplateChanged)));
        public static readonly DependencyProperty DropDownContentProperty = DependencyProperty.Register("DropDownContent", typeof(object), typeof(RadTabItem), null);
        public static readonly DependencyProperty DropDownContentTemplateProperty = DependencyProperty.Register("DropDownContentTemplate", typeof(DataTemplate), typeof(RadTabItem), null);
        public static readonly DependencyProperty DropDownContentTemplateSelectorProperty = DependencyProperty.Register("DropDownContentTemplateSelector", typeof(DataTemplateSelector), typeof(RadTabItem), null);
        private StateFlags flags = new StateFlags();
        private bool hasHeaderElement;
        private ContentPresenter headerElementPresenter;
        private TabItemContentPresenter headerElementTabItemPresenter;
        private FrameworkElement headerElementWrapper;
        public static readonly DependencyProperty IsBreakProperty = DependencyProperty.Register("IsBreak", typeof(bool), typeof(RadTabItem), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadTabItem.OnIsBreakChanged)));
        private static readonly DependencyPropertyKey IsMouseOverPropertyKey = DependencyPropertyExtensions.RegisterReadOnly("IsMouseOver", typeof(bool), typeof(RadTabItem), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadTabItem.OnIsMouseOverChanged)));
        public static readonly DependencyProperty IsMouseOverProperty = IsMouseOverPropertyKey.DependencyProperty;
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(RadTabItem), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadTabItem.OnIsSelectedChanged)));
        public static readonly DependencyProperty LeftTemplateProperty = DependencyProperty.Register("LeftTemplate", typeof(ControlTemplate), typeof(RadTabItem), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadTabItem.OnLeftTemplateChanged)));
        public static readonly DependencyProperty RightTemplateProperty = DependencyProperty.Register("RightTemplate", typeof(ControlTemplate), typeof(RadTabItem), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadTabItem.OnRightTemplateChanged)));
        private static readonly DependencyPropertyKey TabOrientationPropertyKey = DependencyPropertyExtensions.RegisterReadOnly("TabOrientation", typeof(Orientation), typeof(RadTabItem), new Telerik.Windows.PropertyMetadata(Orientation.Horizontal, new PropertyChangedCallback(RadTabItem.OnTabOrientationChanged)));
        public static readonly DependencyProperty TabOrientationProperty = TabOrientationPropertyKey.DependencyProperty;
        private static readonly DependencyPropertyKey TabStripPlacementPropertyKey = DependencyPropertyExtensions.RegisterReadOnly("TabStripPlacement", typeof(Dock), typeof(RadTabItem), new Telerik.Windows.PropertyMetadata(Dock.Top, new PropertyChangedCallback(RadTabItem.OnTabStripPlacementChanged)));
        public static readonly DependencyProperty TabStripPlacementProperty = TabStripPlacementPropertyKey.DependencyProperty;
        public static readonly DependencyProperty TopTemplateProperty = DependencyProperty.Register("TopTemplate", typeof(ControlTemplate), typeof(RadTabItem), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadTabItem.OnTopTemplateChanged)));

        public RadTabItem()
        {
            base.DefaultStyleKey = typeof(RadTabItem);
            base.Loaded += new RoutedEventHandler(this.OnLoaded);
        }

        protected internal override void ChangeVisualState(bool useTransitions)
        {
            if (base.IsFocused)
            {
                this.GoToState(useTransitions, new string[] { "Focused", "Unfocused" });
            }
            else
            {
                this.GoToState(useTransitions, new string[] { "Unfocused" });
            }
            string newPositionState = this.TabOrientation.ToString() + this.TabStripPlacement.ToString();
            this.GoToState(useTransitions, new string[] { newPositionState });
            if (!base.IsEnabled)
            {
                this.GoToState(useTransitions, new string[] { "Disabled", "Normal" });
            }
            else if (this.IsSelected && this.IsMouseOver)
            {
                this.GoToState(useTransitions, new string[] { "SelectedMouseOver", "Selected", "MouseOver", "Normal" });
            }
            else if (this.IsSelected)
            {
                this.GoToState(useTransitions, new string[] { "Selected", "MouseOver", "Normal" });
            }
            else if (this.IsMouseOver)
            {
                this.GoToState(useTransitions, new string[] { "MouseOver", "Normal" });
            }
            else
            {
                this.GoToState(useTransitions, new string[] { "Normal" });
            }
        }

        protected virtual ControlTemplate FindTemplateFromPosition(Dock position)
        {
            switch (position)
            {
                case Dock.Left:
                    return this.LeftTemplate;

                case Dock.Top:
                    return this.TopTemplate;

                case Dock.Right:
                    return this.RightTemplate;

                case Dock.Bottom:
                    return this.BottomTemplate;
            }
            return null;
        }

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

        protected override Size MeasureOverride(Size availableSize)
        {
            if (this.Flags.HasPresenter)
            {
                this.headerElementTabItemPresenter.InvalidateMeasure();
            }
            return base.MeasureOverride(availableSize);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (this.headerElementPresenter != null)
            {
                this.headerElementPresenter.Content = null;
            }
            this.headerElementTabItemPresenter = base.GetTemplateChild("HeaderElement") as TabItemContentPresenter;
            this.headerElementPresenter = base.GetTemplateChild("HeaderElement") as ContentPresenter;
            this.UpdateHeaderPresenterContent();
            this.Flags.HasPresenter = this.headerElementTabItemPresenter != null;
            if (this.hasHeaderElement)
            {
                this.headerElementWrapper.MouseLeftButtonDown -= new MouseButtonEventHandler(this.OnHeaderMouseLeftButtonDown);
            }
            this.headerElementWrapper = base.GetTemplateChild("wrapper") as FrameworkElement;
            this.hasHeaderElement = this.headerElementWrapper != null;
            if (this.hasHeaderElement)
            {
                this.headerElementWrapper.MouseLeftButtonDown += new MouseButtonEventHandler(this.OnHeaderMouseLeftButtonDown);
            }
            if (this.Flags.HasPresenter)
            {
                this.headerElementTabItemPresenter.TabItemOwner = this;
            }
            this.ChangeVisualState();
            this.Flags.IsLoaded = true;
        }

        private static void OnBottomTemplateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            RadTabItem tabItem = sender as RadTabItem;
            if (tabItem.TabStripPlacement == Dock.Bottom)
            {
                tabItem.UpdateTemplate();
            }
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
            if (this.HasOwner)
            {
                this.Owner.NotifyChildContentChanged(this);
            }
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new RadTabItemAutomationPeer(this);
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            if (e.OriginalSource == this)
            {
                AutomationPeer peer = FrameworkElementAutomationPeer.FromElement(this);
                if (peer != null)
                {
                    peer.RaiseAutomationEvent(AutomationEvents.AutomationFocusChanged);
                }
            }
        }

        protected override void OnHeaderChanged(object oldHeader, object newHeader)
        {
            base.OnHeaderChanged(oldHeader, newHeader);
            this.UpdateHeaderPresenterContent();
        }

        [SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers", Justification="There is no need to make the method private.")]
        protected virtual void OnHeaderMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if ((this.HasOwner && !e.Handled) && base.IsEnabled)
            {
                if (this.IsSelected)
                {
                    base.Focus();
                }
                else
                {
                    this.IsSelected = true;
                    RadTabItem focusedItem = FocusManager.GetFocusedElement() as RadTabItem;
                    if (((focusedItem != null) && (Telerik.Windows.Controls.ItemsControl.ItemsControlFromItemContainer(focusedItem) == this.Owner)) && (this.Owner != null))
                    {
                        this.Owner.Focus();
                    }
                }
                e.Handled = true;
            }
        }

        private static void OnIsBreakChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            RadTabItem tabItem = sender as RadTabItem;
            if (tabItem.Owner != null)
            {
                tabItem.Owner.NotifyChildIsBreakChanged();
            }
            tabItem.ChangeVisualState();
        }

        private static void OnIsMouseOverChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as RadTabItem).ChangeVisualState();
        }

        protected virtual void OnIsSelectedChanged(bool oldValue, bool newValue)
        {
            if (this.HasOwner)
            {
                this.Owner.NotifyChildIsSelectedChanged(this);
            }
            bool isSelected = newValue;
            RadTabItemAutomationPeer peer = FrameworkElementAutomationPeer.FromElement(this) as RadTabItemAutomationPeer;
            if (peer != null)
            {
                peer.RaiseAutomationIsSelectedChanged(isSelected);
            }
            if (this.HasOwner && this.Owner.ShouldSetIsTabStop)
            {
                base.IsTabStop = newValue;
            }
            this.ChangeVisualState();
        }

        private static void OnIsSelectedChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as RadTabItem).OnIsSelectedChanged((bool) e.OldValue, (bool) e.NewValue);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (this.HasOwner)
            {
                e.Handled = this.Owner.OnKeyDownInternal(e.Key);
            }
        }

        private static void OnLeftTemplateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            RadTabItem tabItem = sender as RadTabItem;
            if (tabItem.TabStripPlacement == Dock.Left)
            {
                tabItem.UpdateTemplate();
            }
        }

        [SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers", Justification="There is no need to make the method private.")]
        protected virtual void OnLoaded(object sender, RoutedEventArgs e)
        {
            if ((this.HasOwner && this.IsSelected) && ((this.Owner.ContentElement != null) && (this.Owner.ContentElement.Content != this.Owner.SelectedContent)))
            {
                this.Owner.NotifyChildContentChanged(this);
            }
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            if (e.OriginalSource == this)
            {
                AutomationPeer peer = FrameworkElementAutomationPeer.FromElement(this);
                if (peer != null)
                {
                    peer.RaiseAutomationEvent(AutomationEvents.AutomationFocusChanged);
                }
            }
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            this.IsMouseOver = true;
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            this.IsMouseOver = false;
        }

        private static void OnRightTemplateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            RadTabItem tabItem = sender as RadTabItem;
            if (tabItem.TabStripPlacement == Dock.Right)
            {
                tabItem.UpdateTemplate();
            }
        }

        private static void OnTabOrientationChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            RadTabItem tabItem = sender as RadTabItem;
            if (tabItem.headerElementTabItemPresenter != null)
            {
                tabItem.headerElementTabItemPresenter.InvalidateMeasure();
            }
            tabItem.ChangeVisualState();
        }

        private static void OnTabStripPlacementChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            RadTabItem tabItem = sender as RadTabItem;
            tabItem.ChangeVisualState();
            tabItem.UpdateTemplate();
        }

        private static void OnTopTemplateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            RadTabItem tabItem = sender as RadTabItem;
            if (tabItem.TabStripPlacement == Dock.Top)
            {
                tabItem.UpdateTemplate();
            }
        }

        internal void UpdateHeaderPresenterContent()
        {
            if (this.headerElementPresenter != null)
            {
                this.headerElementPresenter.Content = base.Header;
            }
        }

        protected internal void UpdateTemplate()
        {
            ControlTemplate newTemplate = this.FindTemplateFromPosition(this.TabStripPlacement);
            if (newTemplate != null)
            {
                base.Template = newTemplate;
                if (this.headerElementTabItemPresenter != null)
                {
                    this.headerElementTabItemPresenter.ClearValue(ContentControl.ContentProperty);
                }
                if (this.Flags.IsLoaded)
                {
                    base.ApplyTemplate();
                }
            }
        }

        [Telerik.Windows.Controls.SRDescription("TabItemBottomTemplateDescription"), DefaultValue((string) null), Telerik.Windows.Controls.SRCategory("Appearance")]
        public virtual ControlTemplate BottomTemplate
        {
            get
            {
                return (ControlTemplate) base.GetValue(BottomTemplateProperty);
            }
            set
            {
                base.SetValue(BottomTemplateProperty, value);
            }
        }

        public object DropDownContent
        {
            get
            {
                return base.GetValue(DropDownContentProperty);
            }
            set
            {
                base.SetValue(DropDownContentProperty, value);
            }
        }

        public DataTemplate DropDownContentTemplate
        {
            get
            {
                return (DataTemplate) base.GetValue(DropDownContentTemplateProperty);
            }
            set
            {
                base.SetValue(DropDownContentTemplateProperty, value);
            }
        }

        public DataTemplateSelector DropDownContentTemplateSelector
        {
            get
            {
                return (DataTemplateSelector) base.GetValue(DropDownContentTemplateSelectorProperty);
            }
            set
            {
                base.SetValue(DropDownContentTemplateSelectorProperty, value);
            }
        }

        private StateFlags Flags
        {
            get
            {
                return this.flags;
            }
        }

        private bool HasOwner
        {
            get
            {
                return (this.Owner != null);
            }
        }

        [Category("Appearance"), Description("Gets or sets whether the next tab would be displayed on the next row"), DefaultValue(false)]
        public virtual bool IsBreak
        {
            get
            {
                return (bool) base.GetValue(IsBreakProperty);
            }
            set
            {
                base.SetValue(IsBreakProperty, value);
            }
        }

        [Browsable(false)]
        public bool IsMouseOver
        {
            get
            {
                return (bool) base.GetValue(IsMouseOverProperty);
            }
            internal set
            {
                this.SetValue(IsMouseOverPropertyKey, value);
            }
        }

        [Telerik.Windows.Controls.SRDescription("TabItemIsSelectedDescription"), Telerik.Windows.Controls.SRCategory("Appearance"), DefaultValue(false)]
        public bool IsSelected
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

        [Telerik.Windows.Controls.SRDescription("TabItemLeftTemplateDescription"), Telerik.Windows.Controls.SRCategory("Appearance"), DefaultValue((string) null)]
        public virtual ControlTemplate LeftTemplate
        {
            get
            {
                return (ControlTemplate) base.GetValue(LeftTemplateProperty);
            }
            set
            {
                base.SetValue(LeftTemplateProperty, value);
            }
        }

        internal RadTabControl Owner
        {
            get
            {
                return (Telerik.Windows.Controls.ItemsControl.ItemsControlFromItemContainer(this) as RadTabControl);
            }
        }

        [Telerik.Windows.Controls.SRDescription("TabItemRightTemplateDescription"), DefaultValue((string) null), Telerik.Windows.Controls.SRCategory("Appearance")]
        public virtual ControlTemplate RightTemplate
        {
            get
            {
                return (ControlTemplate) base.GetValue(RightTemplateProperty);
            }
            set
            {
                base.SetValue(RightTemplateProperty, value);
            }
        }

        [Browsable(false)]
        public virtual Orientation TabOrientation
        {
            get
            {
                return (Orientation) base.GetValue(TabOrientationProperty);
            }
            internal set
            {
                this.SetValue(TabOrientationPropertyKey, value);
            }
        }

        [Browsable(false)]
        public virtual Dock TabStripPlacement
        {
            get
            {
                return (Dock) base.GetValue(TabStripPlacementProperty);
            }
            internal set
            {
                this.SetValue(TabStripPlacementPropertyKey, value);
            }
        }

        [Telerik.Windows.Controls.SRDescription("TabItemTopTemplateDescription"), Telerik.Windows.Controls.SRCategory("Appearance"), DefaultValue((string) null)]
        public virtual ControlTemplate TopTemplate
        {
            get
            {
                return (ControlTemplate) base.GetValue(TopTemplateProperty);
            }
            set
            {
                base.SetValue(TopTemplateProperty, value);
            }
        }

        private class StateFlags
        {
            internal bool HasPresenter { get; set; }

            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
            internal bool IsLoaded { get; set; }
        }
    }
}

