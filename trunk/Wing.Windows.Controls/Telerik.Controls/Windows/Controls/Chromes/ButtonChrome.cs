namespace Telerik.Windows.Controls.Chromes
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;
    using Telerik.Windows;
    using Telerik.Windows.Controls;

    [TemplateVisualState(GroupName="FocusStates", Name="Unfocused"), TemplateVisualState(GroupName="CommonStates", Name="Pressed"), TemplateVisualState(GroupName="CommonStates", Name="None"), TemplateVisualState(GroupName="CommonStates", Name="MouseOver"), TemplateVisualState(GroupName="FocusStates", Name="Focused"), TemplateVisualState(GroupName="CommonStates", Name="Normal"), TemplateVisualState(GroupName="CommonStates", Name="Disabled")]
    public class ButtonChrome : Control
    {
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(System.Windows.CornerRadius), typeof(ButtonChrome), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(ButtonChrome.OnCornerRadiusChanged)));
        internal const string GroupCommon = "CommonStates";
        internal const string GroupFocus = "FocusStates";
        private static readonly DependencyPropertyKey InnerCornerRadiusPropertyKey = DependencyPropertyExtensions.RegisterReadOnly("InnerCornerRadius", typeof(System.Windows.CornerRadius), typeof(ButtonChrome), null);
        public static readonly DependencyProperty InnerCornerRadiusProperty = InnerCornerRadiusPropertyKey.DependencyProperty;
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(System.Windows.Controls.Orientation), typeof(ButtonChrome), new Telerik.Windows.PropertyMetadata(System.Windows.Controls.Orientation.Horizontal, new PropertyChangedCallback(ButtonChrome.OnOrientationChanged)));
        public static readonly DependencyProperty RenderActiveProperty = DependencyProperty.Register("RenderActive", typeof(bool), typeof(ButtonChrome), new Telerik.Windows.PropertyMetadata(false, new PropertyChangedCallback(ButtonChrome.OnRenderActiveChanged)));
        public static readonly DependencyProperty RenderCheckedProperty = DependencyProperty.Register("RenderChecked", typeof(bool), typeof(ButtonChrome), new Telerik.Windows.PropertyMetadata(false, new PropertyChangedCallback(ButtonChrome.OnRenderCheckedChanged)));
        public static readonly DependencyProperty RenderEnabledProperty = DependencyProperty.Register("RenderEnabled", typeof(bool), typeof(ButtonChrome), new Telerik.Windows.PropertyMetadata(true, new PropertyChangedCallback(ButtonChrome.OnRenderEnabledChanged)));
        public static readonly DependencyProperty RenderFocusedProperty = DependencyProperty.Register("RenderFocused", typeof(bool), typeof(ButtonChrome), new Telerik.Windows.PropertyMetadata(false, new PropertyChangedCallback(ButtonChrome.OnRenderFocusedChanged)));
        public static readonly DependencyProperty RenderHighlightedProperty = DependencyProperty.Register("RenderHighlighted", typeof(bool), typeof(ButtonChrome), new Telerik.Windows.PropertyMetadata(false, new PropertyChangedCallback(ButtonChrome.OnRenderHighlightedChanged)));
        public static readonly DependencyProperty RenderMouseOverProperty = DependencyProperty.Register("RenderMouseOver", typeof(bool), typeof(ButtonChrome), new Telerik.Windows.PropertyMetadata(false, new PropertyChangedCallback(ButtonChrome.OnRenderMouseOverChanged)));
        public static readonly DependencyProperty RenderNormalProperty = DependencyProperty.Register("RenderNormal", typeof(bool), typeof(ButtonChrome), new Telerik.Windows.PropertyMetadata(true, new PropertyChangedCallback(ButtonChrome.OnRenderNormalChanged)));
        public static readonly DependencyProperty RenderPressedProperty = DependencyProperty.Register("RenderPressed", typeof(bool), typeof(ButtonChrome), new Telerik.Windows.PropertyMetadata(false, new PropertyChangedCallback(ButtonChrome.OnRenderPressedChanged)));
        public static readonly DependencyProperty RenderSelectedProperty = DependencyProperty.Register("RenderSelected", typeof(bool), typeof(ButtonChrome), new Telerik.Windows.PropertyMetadata(false, new PropertyChangedCallback(ButtonChrome.OnRenderSelectedChanged)));
        internal const string StateActive = "Active";
        internal const string StateActiveVertical = "ActiveVertical";
        internal const string StateChecked = "Checked";
        internal const string StateCheckedFocused = "CheckedFocused";
        internal const string StateDisabled = "Disabled";
        internal const string StateDisabledChecked = "DisabledChecked";
        internal const string StateDisabledVertical = "DisabledVertical";
        internal const string StateFocused = "Focused";
        internal const string StateHighlighted = "Highlighted";
        internal const string StateHighlightedVertical = "HighlightedVertical";
        internal const string StateMouseOver = "MouseOver";
        internal const string StateMouseOverChecked = "MouseOverChecked";
        internal const string StateMouseOverVertical = "MouseOverVertical";
        internal const string StateNone = "None";
        internal const string StateNoneVertical = "NoneVertical";
        internal const string StateNormal = "Normal";
        internal const string StateNormalFocused = "NormalFocused";
        internal const string StateNormalVertical = "NormalVertical";
        internal const string StatePressed = "Pressed";
        internal const string StatePressedVertical = "PressedVertical";
        internal const string StateSelected = "Selected";
        internal const string StateSelectedHighlighted = "SelectedHighlighted";
        internal const string StateSelectedHighlightedVertical = "SelectedHighlightedVertical";
        internal const string StateSelectedVertical = "SelectedVertical";
        internal const string StateUnfocused = "Unfocused";

        public ButtonChrome()
        {
            base.DefaultStyleKey = typeof(ButtonChrome);
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        private void ChangeVisualState(bool useTransitions)
        {
            if (!this.RenderEnabled)
            {
                if (this.RenderChecked)
                {
                    this.GoToState(useTransitions, new string[] { "DisabledChecked" });
                }
                else if (this.RenderNormal)
                {
                    if (this.Orientation == System.Windows.Controls.Orientation.Horizontal)
                    {
                        this.GoToState(useTransitions, new string[] { "Disabled" });
                    }
                    else
                    {
                        this.GoToState(useTransitions, new string[] { "DisabledVertical" });
                    }
                }
                else if (this.Orientation == System.Windows.Controls.Orientation.Horizontal)
                {
                    this.GoToState(useTransitions, new string[] { "None" });
                }
                else
                {
                    this.GoToState(useTransitions, new string[] { "NoneVertical" });
                }
            }
            else if (this.RenderPressed)
            {
                if (this.Orientation == System.Windows.Controls.Orientation.Horizontal)
                {
                    this.GoToState(useTransitions, new string[] { "Pressed" });
                }
                else
                {
                    this.GoToState(useTransitions, new string[] { "PressedVertical" });
                }
            }
            else if (this.RenderMouseOver)
            {
                if (this.RenderChecked)
                {
                    this.GoToState(useTransitions, new string[] { "MouseOverChecked" });
                }
                else if (this.Orientation == System.Windows.Controls.Orientation.Horizontal)
                {
                    this.GoToState(useTransitions, new string[] { "MouseOver" });
                }
                else
                {
                    this.GoToState(useTransitions, new string[] { "MouseOverVertical" });
                }
            }
            else if (this.RenderChecked)
            {
                if (this.RenderFocused)
                {
                    this.GoToState(useTransitions, new string[] { "CheckedFocused", "Checked" });
                }
                else
                {
                    this.GoToState(useTransitions, new string[] { "Checked" });
                }
            }
            else if (this.RenderHighlighted)
            {
                if (this.Orientation == System.Windows.Controls.Orientation.Horizontal)
                {
                    if (this.RenderSelected)
                    {
                        this.GoToState(useTransitions, new string[] { "SelectedHighlighted", "Highlighted" });
                    }
                    else
                    {
                        this.GoToState(useTransitions, new string[] { "Highlighted" });
                    }
                }
                else if (this.RenderSelected)
                {
                    this.GoToState(useTransitions, new string[] { "SelectedHighlightedVertical", "HighlightedVertical" });
                }
                else
                {
                    this.GoToState(useTransitions, new string[] { "HighlightedVertical" });
                }
            }
            else if (this.RenderSelected)
            {
                if (this.Orientation == System.Windows.Controls.Orientation.Horizontal)
                {
                    this.GoToState(useTransitions, new string[] { "Selected" });
                }
                else
                {
                    this.GoToState(useTransitions, new string[] { "SelectedVertical" });
                }
            }
            else if (this.RenderActive)
            {
                if (this.Orientation == System.Windows.Controls.Orientation.Horizontal)
                {
                    this.GoToState(useTransitions, new string[] { "Active" });
                }
                else
                {
                    this.GoToState(useTransitions, new string[] { "ActiveVertical" });
                }
            }
            else if (this.RenderNormal)
            {
                if (this.Orientation == System.Windows.Controls.Orientation.Horizontal)
                {
                    if (this.RenderFocused)
                    {
                        this.GoToState(useTransitions, new string[] { "NormalFocused", "Normal" });
                    }
                    else
                    {
                        this.GoToState(useTransitions, new string[] { "Normal" });
                    }
                }
                else
                {
                    this.GoToState(useTransitions, new string[] { "NormalVertical" });
                }
            }
            else if (this.Orientation == System.Windows.Controls.Orientation.Horizontal)
            {
                this.GoToState(useTransitions, new string[] { "None" });
            }
            else
            {
                this.GoToState(useTransitions, new string[] { "NoneVertical" });
            }
            if (this.RenderFocused && this.RenderEnabled)
            {
                this.GoToState(useTransitions, new string[] { "Focused" });
            }
            else
            {
                this.GoToState(useTransitions, new string[] { "Unfocused" });
            }
        }

        public override void OnApplyTemplate()
        {
            this.ChangeVisualState(false);
            base.OnApplyTemplate();
        }

        private static void OnCornerRadiusChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ButtonChrome buttonChromeSender = (ButtonChrome) sender;
            System.Windows.CornerRadius newCornerRadius = (System.Windows.CornerRadius) e.NewValue;
            System.Windows.CornerRadius newInnerCornerRadius = new System.Windows.CornerRadius(Math.Max((double) 0.0, (double) (newCornerRadius.TopLeft - 1.0)), Math.Max((double) 0.0, (double) (newCornerRadius.TopRight - 1.0)), Math.Max((double) 0.0, (double) (newCornerRadius.BottomRight - 1.0)), Math.Max((double) 0.0, (double) (newCornerRadius.BottomLeft - 1.0)));
            buttonChromeSender.InnerCornerRadius = newInnerCornerRadius;
        }

        private void OnOrientationChanged()
        {
            this.ChangeVisualState(true);
        }

        private static void OnOrientationChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as ButtonChrome).OnOrientationChanged();
        }

        private void OnRenderActiveChanged()
        {
            this.ChangeVisualState(true);
        }

        private static void OnRenderActiveChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as ButtonChrome).OnRenderActiveChanged();
        }

        private void OnRenderCheckedChanged()
        {
            this.ChangeVisualState(true);
        }

        private static void OnRenderCheckedChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as ButtonChrome).OnRenderCheckedChanged();
        }

        private void OnRenderEnabledChanged()
        {
            this.ChangeVisualState(true);
        }

        private static void OnRenderEnabledChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as ButtonChrome).OnRenderEnabledChanged();
        }

        private void OnRenderFocusedChanged()
        {
            this.ChangeVisualState(true);
        }

        private static void OnRenderFocusedChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as ButtonChrome).OnRenderFocusedChanged();
        }

        private void OnRenderHighlightedChanged()
        {
            this.ChangeVisualState(true);
        }

        private static void OnRenderHighlightedChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as ButtonChrome).OnRenderHighlightedChanged();
        }

        private void OnRenderMouseOverChanged()
        {
            this.ChangeVisualState(true);
        }

        private static void OnRenderMouseOverChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as ButtonChrome).OnRenderMouseOverChanged();
        }

        private void OnRenderNormalChanged()
        {
            this.ChangeVisualState(true);
        }

        private static void OnRenderNormalChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as ButtonChrome).OnRenderNormalChanged();
        }

        private void OnRenderPressedChanged()
        {
            this.ChangeVisualState(true);
        }

        private static void OnRenderPressedChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as ButtonChrome).OnRenderPressedChanged();
        }

        private void OnRenderSelectedChanged()
        {
            this.ChangeVisualState(true);
        }

        private static void OnRenderSelectedChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as ButtonChrome).OnRenderSelectedChanged();
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public System.Windows.CornerRadius CornerRadius
        {
            get
            {
                return (System.Windows.CornerRadius) base.GetValue(CornerRadiusProperty);
            }
            set
            {
                base.SetValue(CornerRadiusProperty, value);
            }
        }

        public System.Windows.CornerRadius InnerCornerRadius
        {
            get
            {
                return (System.Windows.CornerRadius) base.GetValue(InnerCornerRadiusProperty);
            }
            private set
            {
                this.SetValue(InnerCornerRadiusPropertyKey, value);
            }
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

        public bool RenderActive
        {
            get
            {
                return (bool) base.GetValue(RenderActiveProperty);
            }
            set
            {
                base.SetValue(RenderActiveProperty, value);
            }
        }

        public bool RenderChecked
        {
            get
            {
                return (bool) base.GetValue(RenderCheckedProperty);
            }
            set
            {
                base.SetValue(RenderCheckedProperty, value);
            }
        }

        public bool RenderEnabled
        {
            get
            {
                return (bool) base.GetValue(RenderEnabledProperty);
            }
            set
            {
                base.SetValue(RenderEnabledProperty, value);
            }
        }

        public bool RenderFocused
        {
            get
            {
                return (bool) base.GetValue(RenderFocusedProperty);
            }
            set
            {
                base.SetValue(RenderFocusedProperty, value);
            }
        }

        public bool RenderHighlighted
        {
            get
            {
                return (bool) base.GetValue(RenderHighlightedProperty);
            }
            set
            {
                base.SetValue(RenderHighlightedProperty, value);
            }
        }

        public bool RenderMouseOver
        {
            get
            {
                return (bool) base.GetValue(RenderMouseOverProperty);
            }
            set
            {
                base.SetValue(RenderMouseOverProperty, value);
            }
        }

        public bool RenderNormal
        {
            get
            {
                return (bool) base.GetValue(RenderNormalProperty);
            }
            set
            {
                base.SetValue(RenderNormalProperty, value);
            }
        }

        public bool RenderPressed
        {
            get
            {
                return (bool) base.GetValue(RenderPressedProperty);
            }
            set
            {
                base.SetValue(RenderPressedProperty, value);
            }
        }

        public bool RenderSelected
        {
            get
            {
                return (bool) base.GetValue(RenderSelectedProperty);
            }
            set
            {
                base.SetValue(RenderSelectedProperty, value);
            }
        }
    }
}

