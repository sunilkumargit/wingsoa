namespace Telerik.Windows.Controls
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using Telerik.Windows;

    [TemplateVisualState(Name="DirectionUp", GroupName="GroupExpandDirection"), TemplateVisualState(Name="Highlighted", GroupName="CommonStateGroup"), TemplateVisualState(Name="Disabled", GroupName="CommonStateGroup"), TemplateVisualState(Name="Normal", GroupName="CommonStateGroup"), TemplateVisualState(Name="DirectionDown", GroupName="GroupExpandDirection"), DefaultProperty("IsExpanded"), TemplateVisualState(Name="Pressed", GroupName="CommonStateGroup"), TemplateVisualState(Name="DirectionRight", GroupName="GroupExpandDirection"), TemplateVisualState(Name="DirectionLeft", GroupName="GroupExpandDirection"), TemplateVisualState(Name="Expanded", GroupName="ExpandStateGroup"), TemplateVisualState(Name="Collapsed", GroupName="ExpandStateGroup")]
    public class RadExpanderDecorator : ContentControl
    {
        public static readonly DependencyProperty ExpandDirectionProperty;
        public static readonly DependencyProperty IsExpandedProperty;
        public static readonly DependencyProperty IsHighlightedProperty;
        public static readonly DependencyProperty IsPressedProperty;

        static RadExpanderDecorator()
        {
            IsExpandedProperty = DependencyProperty.Register("IsExpanded", typeof(bool), typeof(RadExpanderDecorator), new Telerik.Windows.PropertyMetadata(false, delegate (DependencyObject d, DependencyPropertyChangedEventArgs a) {
                (d as RadExpanderDecorator).OnStateChanged();
            }));
            IsPressedProperty = DependencyProperty.Register("IsPressed", typeof(bool), typeof(RadExpanderDecorator), new Telerik.Windows.PropertyMetadata(false, delegate (DependencyObject d, DependencyPropertyChangedEventArgs a) {
                (d as RadExpanderDecorator).OnStateChanged();
            }));
            IsHighlightedProperty = DependencyProperty.Register("IsHighlighted", typeof(bool), typeof(RadExpanderDecorator), new Telerik.Windows.PropertyMetadata(false, delegate (DependencyObject d, DependencyPropertyChangedEventArgs a) {
                (d as RadExpanderDecorator).OnStateChanged();
            }));
            ExpandDirectionProperty = DependencyProperty.Register("ExpandDirection", typeof(Telerik.Windows.Controls.ExpandDirection), typeof(RadExpanderDecorator), new Telerik.Windows.PropertyMetadata(Telerik.Windows.Controls.ExpandDirection.Down, delegate (DependencyObject d, DependencyPropertyChangedEventArgs a) {
                (d as RadExpanderDecorator).OnStateChanged();
            }));
        }

        public RadExpanderDecorator()
        {
            base.DefaultStyleKey = typeof(RadExpanderDecorator);
            base.IsEnabledChanged += delegate (object o, DependencyPropertyChangedEventArgs e) {
                this.ChangeVisualState(true);
            };
            
        }

        protected internal void ChangeVisualState(bool useTransitions)
        {
            if (!base.IsEnabled)
            {
                this.GoToState(useTransitions, new string[] { "Disabled" });
            }
            else if (this.IsPressed)
            {
                this.GoToState(useTransitions, new string[] { "Pressed" });
            }
            else if (this.IsHighlighted)
            {
                this.GoToState(useTransitions, new string[] { "Highlighted" });
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

        private void OnStateChanged()
        {
            this.ChangeVisualState(true);
        }

        [Telerik.Windows.Controls.SRDescription("ExpanderExpandDirection"), Telerik.Windows.Controls.SRCategory("BehaviourCategory"), DefaultValue(0)]
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

        [DefaultValue(false), Telerik.Windows.Controls.SRCategory("BehaviourCategory"), Telerik.Windows.Controls.SRDescription("ExpanderIsExpanded")]
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

        [Telerik.Windows.Controls.SRDescription("ControlIsHighlighted"), DefaultValue(false), Telerik.Windows.Controls.SRCategory("BehaviourCategory")]
        public bool IsHighlighted
        {
            get
            {
                return (bool) base.GetValue(IsHighlightedProperty);
            }
            set
            {
                base.SetValue(IsHighlightedProperty, value);
            }
        }

        [DefaultValue(false), Telerik.Windows.Controls.SRDescription("ControlIsPressed"), Telerik.Windows.Controls.SRCategory("BehaviourCategory")]
        public bool IsPressed
        {
            get
            {
                return (bool) base.GetValue(IsPressedProperty);
            }
            set
            {
                base.SetValue(IsPressedProperty, value);
            }
        }
    }
}

