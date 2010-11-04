namespace Telerik.Windows.Controls
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;
    using Telerik.Windows;

    [TemplateVisualState(Name="Disabled", GroupName="CommonStates"), StyleTypedProperty(Property="FocusVisualStyle", StyleTargetType=typeof(Control)), TemplateVisualState(Name="Unfocused", GroupName="FocusStates"), TemplateVisualState(Name="Normal", GroupName="CommonStates"), TemplateVisualState(Name="Focused", GroupName="FocusStates")]
    public class HeaderedContentControl : ContentControl
    {
        public static readonly DependencyProperty FocusVisualStyleProperty = DependencyProperty.Register("FocusVisualStyle", typeof(Style), typeof(HeaderedContentControl), null);
        private static readonly DependencyPropertyKey HasHeaderPropertyKey = DependencyPropertyExtensions.RegisterReadOnly("HasHeader", typeof(bool), typeof(HeaderedContentControl), new Telerik.Windows.PropertyMetadata(false));
        public static readonly DependencyProperty HasHeaderProperty = HasHeaderPropertyKey.DependencyProperty;
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(object), typeof(HeaderedContentControl), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(HeaderedContentControl.OnHeaderChanged)));
        public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(HeaderedContentControl), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(HeaderedContentControl.OnHeaderTemplateChanged)));
        public static readonly DependencyProperty HeaderTemplateSelectorProperty = DependencyProperty.Register("HeaderTemplateSelector", typeof(DataTemplateSelector), typeof(HeaderedContentControl), null);
        private static readonly DependencyPropertyKey IsFocusedPropertyKey = DependencyPropertyExtensions.RegisterReadOnly("IsFocused", typeof(bool), typeof(HeaderedContentControl), new Telerik.Windows.PropertyMetadata(false, new PropertyChangedCallback(HeaderedContentControl.OnIsFocusedChanged)));
        public static readonly DependencyProperty IsFocusedProperty = IsFocusedPropertyKey.DependencyProperty;

        public HeaderedContentControl()
        {
            
            base.DefaultStyleKey = typeof(HeaderedContentControl);
            base.IsEnabledChanged += new DependencyPropertyChangedEventHandler(this.OnIsEnabledChanged);
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

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.OnHeaderTemplateChanged(null, this.HeaderTemplate);
            this.OnHeaderChanged(null, this.Header);
            this.ChangeVisualState();
            if (RadControl.IsInDesignMode && (this.Header == null))
            {
                this.Header = base.GetType().Name;
            }
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            if (e.OriginalSource == this)
            {
                this.IsFocused = true;
                base.OnGotFocus(e);
            }
        }

        protected virtual void OnHeaderChanged(object oldHeader, object newHeader)
        {
        }

        private static void OnHeaderChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            HeaderedContentControl control = (HeaderedContentControl) sender;
            control.HasHeader = e.NewValue != null;
            control.OnHeaderChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnHeaderTemplateChanged(DataTemplate oldHeaderTemplate, DataTemplate newHeaderTemplate)
        {
        }

        private static void OnHeaderTemplateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((HeaderedContentControl) sender).OnHeaderTemplateChanged((DataTemplate) e.OldValue, (DataTemplate) e.NewValue);
        }

        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId="e", Justification="This is how EventArgs are named")]
        protected internal virtual void OnIsEnabledChanged(DependencyPropertyChangedEventArgs e)
        {
            this.ChangeVisualState();
        }

        private void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.OnIsEnabledChanged(e);
        }

        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId="e", Justification="This is how EventArgs are named")]
        protected virtual void OnIsFocusedChanged(DependencyPropertyChangedEventArgs e)
        {
            this.ChangeVisualState();
        }

        private static void OnIsFocusedChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as HeaderedContentControl).OnIsFocusedChanged(e);
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            this.IsFocused = false;
            base.OnLostFocus(e);
        }

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
    }
}

