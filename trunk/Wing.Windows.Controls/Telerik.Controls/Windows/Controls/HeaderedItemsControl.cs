namespace Telerik.Windows.Controls
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using Telerik.Windows;

    [StyleTypedProperty(Property="FocusVisualStyle", StyleTargetType=typeof(Control))]
    public class HeaderedItemsControl : Telerik.Windows.Controls.ItemsControl
    {
        private BoolField boolFieldStore;
        public static readonly DependencyProperty FocusVisualStyleProperty = DependencyProperty.Register("FocusVisualStyle", typeof(Style), typeof(HeaderedItemsControl), null);
        private static readonly DependencyPropertyKey HasHeaderPropertyKey = DependencyPropertyExtensions.RegisterReadOnly("HasHeader", typeof(bool), typeof(HeaderedItemsControl), new Telerik.Windows.PropertyMetadata(false));
        public static readonly DependencyProperty HasHeaderProperty = HasHeaderPropertyKey.DependencyProperty;
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(object), typeof(HeaderedItemsControl), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(HeaderedItemsControl.OnHeaderChanged)));
        public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(HeaderedItemsControl), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(HeaderedItemsControl.OnHeaderTemplateChanged)));
        public static readonly DependencyProperty HeaderTemplateSelectorProperty = DependencyProperty.Register("HeaderTemplateSelector", typeof(DataTemplateSelector), typeof(HeaderedItemsControl), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(HeaderedItemsControl.OnHeaderTemplateSelectorChanged)));

        public HeaderedItemsControl()
        {
            
            base.DefaultStyleKey = typeof(HeaderedItemsControl);
        }

        private bool GetBoolField(BoolField field)
        {
            return ((this.boolFieldStore & field) != ((BoolField) 0));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (RadControl.IsInDesignMode && (this.Header == null))
            {
                this.Header = base.GetType().Name;
            }
        }

        protected virtual void OnHeaderChanged(object oldHeader, object newHeader)
        {
        }

        private static void OnHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HeaderedItemsControl control = (HeaderedItemsControl) d;
            control.HasHeader = e.NewValue != null;
            control.OnHeaderChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnHeaderTemplateChanged(DataTemplate oldHeaderTemplate, DataTemplate newHeaderTemplate)
        {
        }

        private static void OnHeaderTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((HeaderedItemsControl) d).OnHeaderTemplateChanged((DataTemplate) e.OldValue, (DataTemplate) e.NewValue);
        }

        private static void OnHeaderTemplateSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((HeaderedItemsControl) d).OnHeaderTemplateSelectorChanged((DataTemplateSelector) e.OldValue, (DataTemplateSelector) e.NewValue);
        }

        protected virtual void OnHeaderTemplateSelectorChanged(DataTemplateSelector oldHeaderTemplateSelector, DataTemplateSelector newHeaderTemplateSelector)
        {
        }

        internal void PrepareHeaderedItemsControl(object item, Telerik.Windows.Controls.ItemsControl parentItemsControl)
        {
            bool flag = item != this;
            this.SetBoolField(BoolField.HeaderIsNotLogical, flag);
            base.PrepareRadItemsControl(item, parentItemsControl);
            if (flag)
            {
                bool shouldSetHeader = true;
                DataTemplate itemTemplate = parentItemsControl.ItemTemplate;
                DataTemplateSelector itemTemplateSelector = parentItemsControl.ItemTemplateSelector;
                string displayMemberPath = parentItemsControl.DisplayMemberPath;
                if (itemTemplate != null)
                {
                    base.SetValue(HeaderTemplateProperty, itemTemplate);
                }
                if (itemTemplateSelector != null)
                {
                    base.SetValue(HeaderTemplateSelectorProperty, itemTemplateSelector);
                }
                if (!string.IsNullOrEmpty(displayMemberPath))
                {
                    Binding binding = new Binding(displayMemberPath) {
                        Converter = new Telerik.Windows.Controls.DisplayMemberValueConverter()
                    };
                    base.SetBinding(HeaderProperty, binding);
                    shouldSetHeader = false;
                }
                if (shouldSetHeader && (this.HeaderIsItem || (this.Header == null)))
                {
                    this.Header = item;
                    this.HeaderIsItem = true;
                }
                this.PrepareHierarchy(item, parentItemsControl);
            }
        }

        private void PrepareHierarchy(object item, Telerik.Windows.Controls.ItemsControl parentItemsControl)
        {
            DataTemplate headerTemplate = this.HeaderTemplate;
            if (headerTemplate == null)
            {
                DataTemplateSelector headerTemplateSelector = this.HeaderTemplateSelector;
                if (headerTemplateSelector != null)
                {
                    headerTemplate = headerTemplateSelector.SelectTemplate(item, this);
                }
            }
            HierarchicalDataTemplate template2 = headerTemplate as HierarchicalDataTemplate;
            if (template2 != null)
            {
                bool itemTemplateIsSame = base.ItemTemplate == parentItemsControl.ItemTemplate;
                bool itemContainerStyleIsSame = base.ItemContainerStyle == parentItemsControl.ItemContainerStyle;
                if ((template2.ItemsSource != null) && (base.ItemsSource == null))
                {
                    base.SetBinding(System.Windows.Controls.ItemsControl.ItemsSourceProperty, template2.ItemsSource);
                }
                if (template2.IsItemTemplateSelectorSet && (base.ItemTemplateSelector == parentItemsControl.ItemTemplateSelector))
                {
                    base.ClearValue(System.Windows.Controls.ItemsControl.ItemTemplateProperty);
                    base.ClearValue(Telerik.Windows.Controls.ItemsControl.ItemTemplateSelectorProperty);
                    if (template2.ItemTemplateSelector != null)
                    {
                        base.ItemTemplateSelector = template2.ItemTemplateSelector;
                    }
                }
                if (template2.IsItemTemplateSet && itemTemplateIsSame)
                {
                    base.ClearValue(System.Windows.Controls.ItemsControl.ItemTemplateProperty);
                    if (template2.ItemTemplate != null)
                    {
                        base.ItemTemplate = template2.ItemTemplate;
                    }
                }
                if (template2.IsItemContainerStyleSelectorSet && (base.ItemContainerStyleSelector == parentItemsControl.ItemContainerStyleSelector))
                {
                    base.ClearValue(Telerik.Windows.Controls.ItemsControl.ItemContainerStyleProperty);
                    base.ClearValue(Telerik.Windows.Controls.ItemsControl.ItemContainerStyleSelectorProperty);
                    if (template2.ItemContainerStyleSelector != null)
                    {
                        base.ItemContainerStyleSelector = template2.ItemContainerStyleSelector;
                    }
                }
                if (template2.IsItemContainerStyleSet && itemContainerStyleIsSame)
                {
                    base.ClearValue(Telerik.Windows.Controls.ItemsControl.ItemContainerStyleProperty);
                    if (template2.ItemContainerStyle != null)
                    {
                        base.ItemContainerStyle = template2.ItemContainerStyle;
                    }
                }
            }
            if (this.HeaderTemplate != headerTemplate)
            {
                base.SetValue(HeaderTemplateProperty, headerTemplate);
            }
        }

        private void SetBoolField(BoolField field, bool value)
        {
            if (value)
            {
                this.boolFieldStore |= field;
            }
            else
            {
                this.boolFieldStore &= ~field;
            }
        }

        [Telerik.Windows.Controls.SRCategory("AppearanceCategory"), Description("Gets or sets the style used by the focus visual of the cotnrol. This is a dependency property.")]
        public Style FocusVisualStyle
        {
            get
            {
                return (Style) base.GetValue(FocusVisualStyleProperty);
            }
            set
            {
                base.SetValue(FocusVisualStyleProperty, value);
            }
        }

        [Browsable(false)]
        public bool HasHeader
        {
            get
            {
                return (bool) base.GetValue(HasHeaderProperty);
            }
            private set
            {
                this.SetValue(HasHeaderPropertyKey, value);
            }
        }

        [Telerik.Windows.Controls.SRDescription("HeaderedItemsControlHeaderDescription"), DefaultValue((string) null), Telerik.Windows.Controls.SRCategory("ContentCategory")]
        public object Header
        {
            get
            {
                return base.GetValue(HeaderProperty);
            }
            set
            {
                base.SetValue(HeaderProperty, value);
            }
        }

        private bool HeaderIsItem
        {
            get
            {
                return this.GetBoolField(BoolField.HeaderIsItem);
            }
            set
            {
                this.SetBoolField(BoolField.HeaderIsItem, value);
            }
        }

        [DefaultValue((string) null), Telerik.Windows.Controls.SRCategory("ContentCategory"), Telerik.Windows.Controls.SRDescription("HeaderedItemsControlHeaderDescription")]
        public DataTemplate HeaderTemplate
        {
            get
            {
                return (DataTemplate) base.GetValue(HeaderTemplateProperty);
            }
            set
            {
                base.SetValue(HeaderTemplateProperty, value);
            }
        }

        [Browsable(false)]
        public DataTemplateSelector HeaderTemplateSelector
        {
            get
            {
                return (DataTemplateSelector) base.GetValue(HeaderTemplateSelectorProperty);
            }
            set
            {
                base.SetValue(HeaderTemplateSelectorProperty, value);
            }
        }

        [Flags]
        private enum BoolField : uint
        {
            HeaderIsItem = 4,
            HeaderIsNotLogical = 8,
            ItemTemplateSelectorSet = 0x40,
            ItemTemplateSet = 0x20
        }
    }
}

