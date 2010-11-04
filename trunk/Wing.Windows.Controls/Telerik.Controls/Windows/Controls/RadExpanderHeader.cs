namespace Telerik.Windows.Controls
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using Telerik.Windows;

    [TemplateVisualState(Name="Collapsed", GroupName="ExpandStateGroup"), DefaultProperty("IsExpanded"), TemplateVisualState(Name="Unfocused", GroupName="FocusStateGroup"), TemplateVisualState(Name="DirectionRight", GroupName="GroupExpandDirection"), TemplateVisualState(Name="MouseOver", GroupName="CommonStateGroup"), TemplateVisualState(Name="DirectionLeft", GroupName="GroupExpandDirection"), TemplateVisualState(Name="Focused", GroupName="FocusStateGroup"), TemplateVisualState(Name="Expanded", GroupName="ExpandStateGroup"), TemplateVisualState(Name="Pressed", GroupName="CommonStateGroup"), TemplateVisualState(Name="Disabled", GroupName="CommonStateGroup"), TemplateVisualState(Name="Normal", GroupName="CommonStateGroup"), TemplateVisualState(Name="DirectionDown", GroupName="GroupExpandDirection"), TemplateVisualState(Name="DirectionUp", GroupName="GroupExpandDirection")]
    public class RadExpanderHeader : ToggleButton
    {
        public static readonly DependencyProperty CornerRadiusProperty;
        public static readonly DependencyProperty DecoratorTemplateProperty;
        public static readonly DependencyProperty ExpandDirectionProperty;
        public static readonly DependencyProperty IsExpandedProperty;

        static RadExpanderHeader()
        {
            IsExpandedProperty = DependencyProperty.Register("IsExpanded", typeof(bool), typeof(RadExpanderHeader), new Telerik.Windows.PropertyMetadata(false, delegate (DependencyObject d, DependencyPropertyChangedEventArgs a) {
                (d as RadExpanderHeader).OnIsExpandedChanged();
            }));
            DecoratorTemplateProperty = DependencyProperty.Register("DecoratorTemplate", typeof(ControlTemplate), typeof(RadExpanderHeader), null);
            ExpandDirectionProperty = DependencyProperty.Register("ExpandDirection", typeof(Telerik.Windows.Controls.ExpandDirection), typeof(RadExpanderHeader), new Telerik.Windows.PropertyMetadata(Telerik.Windows.Controls.ExpandDirection.Down, delegate (DependencyObject d, DependencyPropertyChangedEventArgs a) {
                (d as RadExpanderHeader).OnStateChanged();
            }));
            CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(System.Windows.CornerRadius), typeof(RadExpanderHeader), null);
        }

        public RadExpanderHeader()
        {
            base.DefaultStyleKey = typeof(RadExpanderHeader);
            base.IsEnabledChanged += delegate (object o, DependencyPropertyChangedEventArgs e) {
                this.ChangeVisualState(true);
            };
            
            base.Checked += delegate (object s, RoutedEventArgs e) {
                if (!this.IsExpanded)
                {
                    this.IsExpanded = true;
                }
            };
            base.Unchecked += delegate (object s, RoutedEventArgs e) {
                if (this.IsExpanded)
                {
                    this.IsExpanded = false;
                }
            };
            base.Indeterminate += delegate (object s, RoutedEventArgs e) {
                if (this.IsExpanded)
                {
                    this.IsExpanded = false;
                }
            };
        }

        protected internal void ChangeVisualState(bool useTransitions)
        {
            if (!base.IsEnabled)
            {
                this.GoToState(useTransitions, new string[] { "Disabled" });
            }
            else if (base.IsPressed)
            {
                this.GoToState(useTransitions, new string[] { "Pressed" });
            }
            else if (base.IsMouseOver)
            {
                this.GoToState(useTransitions, new string[] { "MouseOver" });
            }
            else
            {
                this.GoToState(useTransitions, new string[] { "Normal" });
            }
            if (this.IsExpanded)
            {
                this.GoToState(useTransitions, new string[] { "Expanded" });
            }
            else
            {
                this.GoToState(useTransitions, new string[] { "Collapsed" });
            }
            if (base.IsFocused)
            {
                this.GoToState(useTransitions, new string[] { "Focused" });
            }
            else
            {
                this.GoToState(useTransitions, new string[] { "Unfocused" });
            }
            if (this.ExpandDirection == Telerik.Windows.Controls.ExpandDirection.Left)
            {
                this.GoToState(useTransitions, new string[] { "DirectionLeft" });
            }
            else if (this.ExpandDirection == Telerik.Windows.Controls.ExpandDirection.Right)
            {
                this.GoToState(useTransitions, new string[] { "DirectionRight" });
            }
            else if (this.ExpandDirection == Telerik.Windows.Controls.ExpandDirection.Up)
            {
                this.GoToState(useTransitions, new string[] { "DirectionUp" });
            }
            else
            {
                this.GoToState(useTransitions, new string[] { "DirectionDown" });
            }
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

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.ChangeVisualState(false);
        }

        private void OnIsExpandedChanged()
        {
            bool? isChecked = base.IsChecked;
            bool isExpanded = this.IsExpanded;
            if ((isChecked.GetValueOrDefault() != isExpanded) || !isChecked.HasValue)
            {
                base.IsChecked = new bool?(this.IsExpanded);
            }
            this.ChangeVisualState(true);
        }

        private void OnStateChanged()
        {
            this.ChangeVisualState(true);
        }

        [Telerik.Windows.Controls.SRDescription("ExpanderExpandDirection"), Telerik.Windows.Controls.SRCategory("AppearanceCategory")]
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

        [Telerik.Windows.Controls.SRCategory("ContentCategory"), Telerik.Windows.Controls.SRDescription("ExpanderDecoratorTemplate"), DefaultValue(false)]
        public ControlTemplate DecoratorTemplate
        {
            get
            {
                return (ControlTemplate) base.GetValue(DecoratorTemplateProperty);
            }
            set
            {
                base.SetValue(DecoratorTemplateProperty, value);
            }
        }

        [DefaultValue(0), Telerik.Windows.Controls.SRCategory("BehaviourCategory"), Telerik.Windows.Controls.SRDescription("ExpanderExpandDirection")]
        public Telerik.Windows.Controls.ExpandDirection ExpandDirection
        {
            get
            {
                return (Telerik.Windows.Controls.ExpandDirection) base.GetValue(ExpandDirectionProperty);
            }
            set
            {
                base.SetValue(ExpandDirectionProperty, value);
            }
        }

        [Telerik.Windows.Controls.SRDescription("ExpanderIsExpanded"), Telerik.Windows.Controls.SRCategory("BehaviourCategory"), DefaultValue(false)]
        public bool IsExpanded
        {
            get
            {
                return (bool) base.GetValue(IsExpandedProperty);
            }
            set
            {
                base.SetValue(IsExpandedProperty, value);
            }
        }
    }
}

