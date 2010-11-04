namespace Telerik.Windows.Controls.Chromes
{
    using System;
    using System.Windows;
    using System.Windows.Controls;

    [TemplatePart(Name="PART_ToolTip", Type=typeof(ToolTip))]
    public class ValidationTooltip : Control
    {
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(System.Windows.CornerRadius), typeof(ValidationTooltip), new PropertyMetadata(new System.Windows.CornerRadius()));
        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register("IsOpen", typeof(bool), typeof(ValidationTooltip), new PropertyMetadata(false, new PropertyChangedCallback(ValidationTooltip.OnIsOpenChanged)));
        private ToolTip toolTip;
        public static readonly DependencyProperty TooltipContentProperty = DependencyProperty.Register("TooltipContent", typeof(object), typeof(ValidationTooltip), new PropertyMetadata(null));
        public static readonly DependencyProperty TooltipContentTemplateProperty = DependencyProperty.Register("TooltipContentTemplate", typeof(DataTemplate), typeof(ValidationTooltip), new PropertyMetadata(null));
        public static readonly DependencyProperty TooltipPlacementTargetProperty = DependencyProperty.Register("TooltipPlacementTarget", typeof(UIElement), typeof(ValidationTooltip), new PropertyMetadata(null));

        public ValidationTooltip()
        {
            base.DefaultStyleKey = typeof(ValidationTooltip);
        }

        public override void OnApplyTemplate()
        {
            this.toolTip = base.GetTemplateChild("PART_ToolTip") as ToolTip;
        }

        private void OnIsOpenChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.toolTip != null)
            {
                this.toolTip.IsOpen = (bool) e.NewValue;
            }
        }

        private static void OnIsOpenChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as ValidationTooltip).OnIsOpenChanged(e);
        }

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

        public bool IsOpen
        {
            get
            {
                return (bool) base.GetValue(IsOpenProperty);
            }
            set
            {
                base.SetValue(IsOpenProperty, value);
            }
        }

        public object TooltipContent
        {
            get
            {
                return base.GetValue(TooltipContentProperty);
            }
            set
            {
                base.SetValue(TooltipContentProperty, value);
            }
        }

        public object TooltipContentTemplate
        {
            get
            {
                return base.GetValue(TooltipContentTemplateProperty);
            }
            set
            {
                base.SetValue(TooltipContentTemplateProperty, value);
            }
        }

        public UIElement TooltipPlacementTarget
        {
            get
            {
                return (UIElement) base.GetValue(TooltipPlacementTargetProperty);
            }
            set
            {
                base.SetValue(TooltipPlacementTargetProperty, value);
            }
        }
    }
}

