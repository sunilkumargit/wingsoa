namespace Telerik.Windows.Controls
{
    using System;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Automation.Peers;
    using Wing.Windows.Controls.Telerik.Navigation.Windows.Controls;
    using System.Windows.Controls;
    using System.Windows.Data;
    using Telerik.Windows;
    using Telerik.Windows.Controls.Animation;

    [TemplateVisualState(Name="Unfocused", GroupName="FocusStates"), DefaultProperty("Items"), TemplateVisualState(Name="Normal", GroupName="CommonStates"), TemplateVisualState(Name="Disabled", GroupName="CommonStates"), TemplateVisualState(Name="Focused", GroupName="FocusStates"), ScriptableType, StyleTypedProperty(Property="ItemContainerStyle", StyleTargetType=typeof(ContentPresenter))]
    public class ItemsControl : System.Windows.Controls.ItemsControl
    {
        private static readonly DependencyPropertyKey HasItemsPropertyKey = DependencyPropertyExtensions.RegisterReadOnly("HasItems", typeof(bool), typeof(Telerik.Windows.Controls.ItemsControl), new Telerik.Windows.PropertyMetadata(false));
        public static readonly DependencyProperty HasItemsProperty = HasItemsPropertyKey.DependencyProperty;
        private static readonly DependencyPropertyKey IsFocusedPropertyKey = DependencyPropertyExtensions.RegisterReadOnly("IsFocused", typeof(bool), typeof(Telerik.Windows.Controls.ItemsControl), new Telerik.Windows.PropertyMetadata(false, new PropertyChangedCallback(Telerik.Windows.Controls.ItemsControl.OnIsFocusedChanged)));
        public static readonly DependencyProperty IsFocusedProperty = IsFocusedPropertyKey.DependencyProperty;
        public static readonly DependencyProperty IsTextSearchEnabledProperty = DependencyProperty.Register("IsTextSearchEnabled", typeof(bool), typeof(Telerik.Windows.Controls.ItemsControl), new Telerik.Windows.PropertyMetadata(true));
        private Telerik.Windows.Controls.ItemContainerGenerator itemContainerGenerator;
        public static readonly DependencyProperty ItemContainerStyleProperty = DependencyProperty.Register("ItemContainerStyle", typeof(Style), typeof(Telerik.Windows.Controls.ItemsControl), null);
        public static readonly DependencyProperty ItemContainerStyleSelectorProperty = DependencyProperty.Register("ItemContainerStyleSelector", typeof(StyleSelector), typeof(Telerik.Windows.Controls.ItemsControl), null);
        public static readonly DependencyProperty ItemTemplateSelectorProperty = DependencyProperty.Register("ItemTemplateSelector", typeof(DataTemplateSelector), typeof(Telerik.Windows.Controls.ItemsControl), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(Telerik.Windows.Controls.ItemsControl.OnRadItemTemplateSelectorChanged)));

        public ItemsControl()
        {
            
            base.DefaultStyleKey = typeof(Telerik.Windows.Controls.ItemsControl);
            base.IsEnabledChanged += new DependencyPropertyChangedEventHandler(this.OnIsEnabledChanged);
        }

        private static void ApplyContainerBindings(FrameworkElement container, DataTemplate template)
        {
            ContainerBinding.ApplyContainerBindings(container, template);
        }

        protected internal virtual void ChangeVisualState()
        {
            this.ChangeVisualState(true);
        }

        protected internal virtual void ChangeVisualState(bool useTransitions)
        {
            if (base.IsEnabled)
            {
                VisualStateManager.GoToState(this, "Normal", useTransitions);
            }
            else
            {
                VisualStateManager.GoToState(this, "Disabled", useTransitions);
            }
            if (this.IsFocused)
            {
                VisualStateManager.GoToState(this, "Focused", useTransitions);
            }
            else
            {
                VisualStateManager.GoToState(this, "Unfocused", useTransitions);
            }
        }

        private Style GetContainerStyle(object item, DependencyObject element)
        {
            Style elementStyle = this.ItemContainerStyle;
            if ((elementStyle == null) && (this.ItemContainerStyleSelector != null))
            {
                elementStyle = this.ItemContainerStyleSelector.SelectStyle(item, element);
            }
            return elementStyle;
        }

        public static Telerik.Windows.Controls.ItemsControl ItemsControlFromItemContainer(DependencyObject container)
        {
            return (System.Windows.Controls.ItemsControl.ItemsControlFromItemContainer(container) as Telerik.Windows.Controls.ItemsControl);
        }

        internal DependencyObject NavigateTo(int index, int direction)
        {
            int coercedIndex = -1;
            DependencyObject container = null;
            int count = base.Items.Count - 1;
            if ((base.Items.Count > index) && (index >= 0))
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
            for (int i = 0; i < base.Items.Count; i++)
            {
                container = this.ItemContainerGenerator.ContainerFromIndex(coercedIndex);
                bool enabled = false;
                if (container is Control)
                {
                    enabled = (bool) container.GetValue(Control.IsEnabledProperty);
                }
                else
                {
                    enabled = true;
                }
                bool visible = ((Visibility) container.GetValue(UIElement.VisibilityProperty)) == Visibility.Visible;
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

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.ChangeVisualState();
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            if (e.OriginalSource == this)
            {
                this.IsFocused = true;
                AutomationPeer automationPeer = FrameworkElementAutomationPeer.FromElement(this);
                if (automationPeer != null)
                {
                    automationPeer.RaiseAutomationEvent(AutomationEvents.AutomationFocusChanged);
                }
            }
            base.OnGotFocus(e);
        }

        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId="e", Justification="This is how event args are named")]
        protected internal virtual void OnIsEnabledChanged(DependencyPropertyChangedEventArgs e)
        {
            this.ChangeVisualState();
        }

        private void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.OnIsEnabledChanged(e);
        }

        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId="e")]
        protected virtual void OnIsFocusedChanged(DependencyPropertyChangedEventArgs e)
        {
            this.ChangeVisualState();
        }

        private static void OnIsFocusedChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as Telerik.Windows.Controls.ItemsControl).OnIsFocusedChanged(e);
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            this.SetValue(HasItemsPropertyKey, base.Items.Count > 0);
            base.OnItemsChanged(e);
        }

        protected virtual void OnItemTemplateSelectorChanged(DataTemplateSelector oldItemTemplateSelector, DataTemplateSelector newItemTemplateSelector)
        {
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            this.IsFocused = false;
            base.OnLostFocus(e);
            AutomationPeer automationPeer = FrameworkElementAutomationPeer.FromElement(this);
            if (automationPeer != null)
            {
                automationPeer.RaiseAutomationEvent(AutomationEvents.AutomationFocusChanged);
            }
        }

        private static void OnRadItemTemplateSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Telerik.Windows.Controls.ItemsControl) d).OnItemTemplateSelectorChanged((DataTemplateSelector) e.OldValue, (DataTemplateSelector) e.NewValue);
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            ContentControl contentControl = element as ContentControl;
            ContentPresenter contentPresenter = element as ContentPresenter;
            this.ItemContainerGenerator.NotifyBeginPrepareContainer();
            if (((contentControl == null) && (contentPresenter == null)) && (element.GetValue(FrameworkElement.StyleProperty) == null))
            {
                Style elementStyle = this.GetContainerStyle(item, element);
                if (elementStyle != null)
                {
                    element.SetValue(FrameworkElement.StyleProperty, elementStyle);
                }
            }
            Theme theme = StyleManager.GetTheme(this);
            if ((theme != null) && (StyleManager.GetTheme(element) == null))
            {
                StyleManager.SetTheme(element, theme);
            }
            HeaderedContentControl headeredContent = element as HeaderedContentControl;
            if (headeredContent != null)
            {
                this.PrepareHeaderedContentControl(item, headeredContent);
                ApplyContainerBindings(headeredContent, headeredContent.HeaderTemplate);
                ApplyContainerBindings(headeredContent, headeredContent.ContentTemplate);
            }
            else if (contentControl != null)
            {
                this.PrepareContentControl(item, contentControl);
                ApplyContainerBindings(contentControl, contentControl.ContentTemplate);
            }
            else if (contentPresenter != null)
            {
                this.PrepareContentPresenter(item, contentPresenter);
            }
            else
            {
                HeaderedItemsControl control4 = element as HeaderedItemsControl;
                if (control4 != null)
                {
                    control4.PrepareHeaderedItemsControl(item, this);
                    ApplyContainerBindings(control4, control4.HeaderTemplate);
                }
                else
                {
                    Telerik.Windows.Controls.ItemsControl radItemsControl = element as Telerik.Windows.Controls.ItemsControl;
                    if (radItemsControl != null)
                    {
                        radItemsControl.PrepareRadItemsControl(item, this);
                    }
                    else
                    {
                        System.Windows.Controls.ItemsControl itemsControl = element as System.Windows.Controls.ItemsControl;
                        if (itemsControl != null)
                        {
                            PrepareItemsControl(item, itemsControl, base.ItemTemplate, base.DisplayMemberPath);
                        }
                    }
                }
            }
            AnimationSelectorBase parentAnimationSelector = AnimationManager.GetAnimationSelector(this);
            AnimationSelectorBase childAnimationSelector = AnimationManager.GetAnimationSelector(element);
            if ((parentAnimationSelector != null) && (childAnimationSelector == null))
            {
                AnimationManager.SetAnimationSelector(element, parentAnimationSelector);
            }
            AnimationManager.SetIsAnimationEnabled(element, AnimationManager.GetIsAnimationEnabled(this));
            this.ItemContainerGenerator.NotifyEndPrepareContainer(base.Items.Count);
        }

        internal void PrepareContentControl(object item, ContentControl element)
        {
            if (object.ReferenceEquals(item, element))
            {
                base.PrepareContainerForItemOverride(element, item);
            }
            else
            {
                DataTemplate dataTemplateSetbyStyle = null;
                if (element.GetValue(FrameworkElement.StyleProperty) == null)
                {
                    Style elementStyle = this.GetContainerStyle(item, element);
                    if (elementStyle != null)
                    {
                        element.SetValue(FrameworkElement.StyleProperty, elementStyle);
                        dataTemplateSetbyStyle = element.ContentTemplate;
                    }
                }
                base.PrepareContainerForItemOverride(element, item);
                DataTemplate dataTemplate = null;
                if (base.ItemTemplate != null)
                {
                    dataTemplate = base.ItemTemplate;
                }
                else if (this.ItemTemplateSelector != null)
                {
                    dataTemplate = this.ItemTemplateSelector.SelectTemplate(item, element);
                }
                if (dataTemplate != null)
                {
                    element.ContentTemplate = dataTemplate;
                }
                else if (dataTemplateSetbyStyle != null)
                {
                    element.ContentTemplate = dataTemplateSetbyStyle;
                }
            }
        }

        private void PrepareContentPresenter(object item, ContentPresenter element)
        {
            if (object.ReferenceEquals(item, element))
            {
                base.PrepareContainerForItemOverride(element, item);
            }
            else
            {
                DataTemplate dataTemplateSetbyStyle = null;
                if (element.GetValue(FrameworkElement.StyleProperty) == null)
                {
                    Style elementStyle = this.GetContainerStyle(item, element);
                    if (elementStyle != null)
                    {
                        element.SetValue(FrameworkElement.StyleProperty, elementStyle);
                        dataTemplateSetbyStyle = element.ContentTemplate;
                    }
                }
                base.PrepareContainerForItemOverride(element, item);
                DataTemplate dataTemplate = null;
                if (base.ItemTemplate != null)
                {
                    dataTemplate = base.ItemTemplate;
                }
                else if (this.ItemTemplateSelector != null)
                {
                    dataTemplate = this.ItemTemplateSelector.SelectTemplate(item, element);
                }
                if (dataTemplate != null)
                {
                    element.ContentTemplate = dataTemplate;
                }
                else if (dataTemplateSetbyStyle != null)
                {
                    element.ContentTemplate = dataTemplateSetbyStyle;
                }
            }
        }

        private void PrepareHeaderedContentControl(object item, HeaderedContentControl headeredContent)
        {
            if (!object.ReferenceEquals(item, headeredContent))
            {
                if (headeredContent.ReadLocalValue(ContentControl.ContentProperty) == DependencyProperty.UnsetValue)
                {
                    headeredContent.Content = item;
                }
                DataTemplate dataTemplateSetbyStyle = null;
                if (headeredContent.GetValue(FrameworkElement.StyleProperty) == null)
                {
                    Style elementStyle = this.GetContainerStyle(item, headeredContent);
                    if (elementStyle != null)
                    {
                        headeredContent.SetValue(FrameworkElement.StyleProperty, elementStyle);
                        dataTemplateSetbyStyle = headeredContent.ContentTemplate;
                    }
                }
                base.PrepareContainerForItemOverride(headeredContent, item);
                DataTemplate contentTemplate = headeredContent.ContentTemplate;
                headeredContent.ContentTemplate = dataTemplateSetbyStyle;
                bool shouldSetHeader = true;
                if (((base.ItemTemplate != null) || (this.ItemTemplateSelector != null)) && !string.IsNullOrEmpty(base.DisplayMemberPath))
                {
                    throw new InvalidOperationException("Can not set ItemTemplate and DisplayMemberPath simultaneously.");
                }
                if (((headeredContent.HeaderTemplate != null) || (headeredContent.HeaderTemplateSelector != null)) && !string.IsNullOrEmpty(base.DisplayMemberPath))
                {
                    throw new InvalidOperationException("Can not set HeaderTemplate/Selector and DisplayMemberPath simultaneously.");
                }
                if (headeredContent.HeaderTemplate == null)
                {
                    if (headeredContent.HeaderTemplateSelector != null)
                    {
                        headeredContent.HeaderTemplate = headeredContent.HeaderTemplateSelector.SelectTemplate(item, headeredContent);
                    }
                    else if (base.ItemTemplate != null)
                    {
                        headeredContent.HeaderTemplate = base.ItemTemplate;
                    }
                    else if (this.ItemTemplateSelector != null)
                    {
                        DataTemplate template = this.ItemTemplateSelector.SelectTemplate(item, headeredContent);
                        if (template != null)
                        {
                            headeredContent.HeaderTemplate = template;
                        }
                    }
                }
                if ((headeredContent.HeaderTemplate == null) && !string.IsNullOrEmpty(base.DisplayMemberPath))
                {
                    Binding binding = new Binding(base.DisplayMemberPath) {
                        Converter = new Telerik.Windows.Controls.DisplayMemberValueConverter()
                    };
                    headeredContent.SetBinding(HeaderedContentControl.HeaderProperty, binding);
                    shouldSetHeader = false;
                }
                if ((shouldSetHeader && !(item is UIElement)) && (headeredContent.Header == null))
                {
                    headeredContent.Header = item;
                }
            }
        }

        private static void PrepareItemsControl(object item, System.Windows.Controls.ItemsControl itemsControl, DataTemplate itemTemplate, string displayMemberPath)
        {
            if (item != itemsControl)
            {
                if ((itemTemplate != null) && !string.IsNullOrEmpty(displayMemberPath))
                {
                    throw new InvalidOperationException("Can not set ItemTemplate and DisplayMemberPath simultaneously.");
                }
                if (itemTemplate != null)
                {
                    itemsControl.ItemTemplate = itemTemplate;
                }
                else if (!string.IsNullOrEmpty(displayMemberPath))
                {
                    itemsControl.DisplayMemberPath = displayMemberPath;
                }
            }
        }

        internal void PrepareRadItemsControl(object item, Telerik.Windows.Controls.ItemsControl parentItemsControl)
        {
            if (item != this)
            {
                DataTemplate itemTemplate = parentItemsControl.ItemTemplate;
                DataTemplateSelector itemTemplateSelector = parentItemsControl.ItemTemplateSelector;
                Style itemContainerStyle = parentItemsControl.ItemContainerStyle;
                StyleSelector itemContainerStyleSelector = parentItemsControl.ItemContainerStyleSelector;
                string displayMemberPath = parentItemsControl.DisplayMemberPath;
                PrepareItemsControl(item, this, itemTemplate, displayMemberPath);
                if (((itemTemplate != null) || (itemTemplateSelector != null)) && !string.IsNullOrEmpty(displayMemberPath))
                {
                    throw new InvalidOperationException("Can not set ItemTemplate and DisplayMemberPath simultaneously.");
                }
                if (itemTemplateSelector != null)
                {
                    this.ItemTemplateSelector = itemTemplateSelector;
                }
                if ((itemContainerStyle != null) && (this.ItemContainerStyle == null))
                {
                    this.ItemContainerStyle = itemContainerStyle;
                }
                if ((itemContainerStyleSelector != null) && (this.ItemContainerStyleSelector == null))
                {
                    this.ItemContainerStyleSelector = itemContainerStyleSelector;
                }
            }
        }

        [Browsable(false)]
        public bool HasItems
        {
            get
            {
                return (bool) base.GetValue(HasItemsProperty);
            }
            private set
            {
                this.SetValue(HasItemsPropertyKey, value);
            }
        }

        [Browsable(false)]
        public bool IsFocused
        {
            get
            {
                return (bool) base.GetValue(IsFocusedProperty);
            }
            protected set
            {
                this.SetValue(IsFocusedPropertyKey, value);
            }
        }

        public bool IsTextSearchEnabled
        {
            get
            {
                return (bool) base.GetValue(IsTextSearchEnabledProperty);
            }
            set
            {
                base.SetValue(IsTextSearchEnabledProperty, value);
            }
        }

        [Browsable(false), ScriptableMember]
        public Telerik.Windows.Controls.ItemContainerGenerator ItemContainerGenerator
        {
            get
            {
                if (this.itemContainerGenerator == null)
                {
                    this.itemContainerGenerator = new Telerik.Windows.Controls.ItemContainerGenerator(base.ItemContainerGenerator);
                }
                return this.itemContainerGenerator;
            }
        }

        public Style ItemContainerStyle
        {
            get
            {
                return (Style) base.GetValue(ItemContainerStyleProperty);
            }
            set
            {
                base.SetValue(ItemContainerStyleProperty, value);
            }
        }

        public StyleSelector ItemContainerStyleSelector
        {
            get
            {
                return (StyleSelector) base.GetValue(ItemContainerStyleSelectorProperty);
            }
            set
            {
                base.SetValue(ItemContainerStyleSelectorProperty, value);
            }
        }

        public DataTemplateSelector ItemTemplateSelector
        {
            get
            {
                return (DataTemplateSelector) base.GetValue(ItemTemplateSelectorProperty);
            }
            set
            {
                base.SetValue(ItemTemplateSelectorProperty, value);
            }
        }
    }
}

