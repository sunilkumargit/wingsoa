namespace Telerik.Windows.Controls
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Automation.Peers;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Media;
    using Telerik.Windows;

    [TemplatePart(Name="ProgressBarIndicator", Type=typeof(FrameworkElement)), DefaultEvent("ValueChanged"), TemplatePart(Name="ProgressBarTrack", Type=typeof(FrameworkElement)), TemplatePart(Name="SkipValueSpacer", Type=typeof(FrameworkElement)), TemplateVisualState(Name="Indeterminate", GroupName="CommonStates"), TemplateVisualState(Name="Determinate", GroupName="CommonStates"), TemplateVisualState(Name="Horizontal", GroupName="OrientationStates"), TemplateVisualState(Name="Vertical", GroupName="OrientationStates"), DefaultProperty("Value")]
    public class RadProgressBar : RangeBase
    {
        internal const string ElementIndicatorName = "ProgressBarIndicator";
        internal const string ElementSpacerName = "SkipValueSpacer";
        internal const string ElementTrackName = "ProgressBarTrack";
        internal const string GroupCommon = "CommonStates";
        public static readonly DependencyProperty IsIndeterminateProperty = DependencyProperty.Register("IsIndeterminate", typeof(bool), typeof(RadProgressBar), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadProgressBar.OnIsIndeterminateChanged)));
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(System.Windows.Controls.Orientation), typeof(RadProgressBar), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadProgressBar.OnOrientationChanged)));
        public static readonly DependencyProperty SkipValueProperty = DependencyProperty.Register("SkipValue", typeof(double), typeof(RadProgressBar), new Telerik.Windows.PropertyMetadata(0.0, new PropertyChangedCallback(RadProgressBar.OnSkipValueChanged)));
        internal const string StateDeterminate = "Determinate";
        internal const string StateIndeterminate = "Indeterminate";

        public RadProgressBar()
        {
            base.DefaultStyleKey = typeof(RadProgressBar);
            
        }

        internal double GetRange()
        {
            return (base.Maximum - base.Minimum);
        }

        internal double GetSkipRatio()
        {
            return (this.SkipValue / this.GetRange());
        }

        private bool GoToState(bool useTransitions, string stateName)
        {
            return VisualStateManager.GoToState(this, stateName, useTransitions);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (this.ElementTrack != null)
            {
                this.ElementTrack.SizeChanged -= new SizeChangedEventHandler(this.OnTrackSizeChanged);
            }
            this.ElementTrack = base.GetTemplateChild("ProgressBarTrack") as FrameworkElement;
            this.ElementIndicator = base.GetTemplateChild("ProgressBarIndicator") as FrameworkElement;
            this.ElementSpacer = base.GetTemplateChild("SkipValueSpacer") as FrameworkElement;
            if (this.ElementTrack != null)
            {
                this.ElementTrack.SizeChanged += new SizeChangedEventHandler(this.OnTrackSizeChanged);
            }
            this.UpdateVisualState(false);
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new RadProgressBarAutomationPeer(this);
        }

        private static void OnIsIndeterminateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadProgressBar bar = (RadProgressBar) d;
            if (bar != null)
            {
                bar.UpdateVisualState(false);
            }
        }

        protected override void OnMaximumChanged(double oldMaximum, double newMaximum)
        {
            base.OnMaximumChanged(oldMaximum, newMaximum);
            this.SetProgressBarIndicatorLength();
        }

        protected override void OnMinimumChanged(double oldMinimum, double newMinimum)
        {
            base.OnMinimumChanged(oldMinimum, newMinimum);
            this.SetProgressBarIndicatorLength();
        }

        private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            RadProgressBar pb = d as RadProgressBar;
            if (pb != null)
            {
                pb.UpdateVisualState(false);
            }
        }

        private static void OnSkipValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as RadProgressBar).SetProgressBarIndicatorLength();
        }

        private void OnTrackSizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.SetProgressBarIndicatorLength();
        }

        protected override void OnValueChanged(double oldValue, double newValue)
        {
            base.OnValueChanged(oldValue, newValue);
            this.SetProgressBarIndicatorLength();
        }

        private void SetProgressBarIndicatorLength()
        {
            double minimum = base.Minimum;
            double maximum = base.Maximum;
            double progressValue = base.Value;
            if ((this.ElementTrack != null) && (this.ElementIndicator != null))
            {
                FrameworkElement parent = VisualTreeHelper.GetParent(this.ElementIndicator) as FrameworkElement;
                if (parent != null)
                {
                    double indicatorMargins = this.ElementIndicator.Margin.Left + this.ElementIndicator.Margin.Right;
                    Border border = parent as Border;
                    if (border != null)
                    {
                        indicatorMargins += border.Padding.Left + border.Padding.Right;
                    }
                    else
                    {
                        Control control = parent as Control;
                        if (control != null)
                        {
                            indicatorMargins += control.Padding.Left + control.Padding.Right;
                        }
                    }
                    double filledAreaRatio = (this.IsIndeterminate || (maximum == minimum)) ? 1.0 : ((progressValue - minimum) / (maximum - minimum));
                    double totalArea = Math.Max((double) 0.0, (double) (parent.ActualWidth - indicatorMargins));
                    if (this.ElementSpacer != null)
                    {
                        this.ElementSpacer.Width = totalArea * this.GetSkipRatio();
                        totalArea -= this.ElementSpacer.Width;
                    }
                    this.ElementIndicator.Width = filledAreaRatio * totalArea;
                }
            }
        }

        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId="useTransitions"), SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        internal void UpdateVisualState(bool useTransitions)
        {
            if (this.Orientation == System.Windows.Controls.Orientation.Vertical)
            {
                this.GoToState(useTransitions, "Vertical");
            }
            else
            {
                this.GoToState(useTransitions, "Horizontal");
            }
            if (!this.IsIndeterminate)
            {
                this.GoToState(useTransitions, "Determinate");
            }
            else
            {
                this.GoToState(useTransitions, "Indeterminate");
            }
        }

        internal FrameworkElement ElementIndicator { get; set; }

        internal FrameworkElement ElementSpacer { get; set; }

        internal FrameworkElement ElementTrack { get; set; }

        public bool IsIndeterminate
        {
            get
            {
                return (bool) base.GetValue(IsIndeterminateProperty);
            }
            set
            {
                base.SetValue(IsIndeterminateProperty, value);
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

        public double SkipValue
        {
            get
            {
                return (double) base.GetValue(SkipValueProperty);
            }
            set
            {
                base.SetValue(SkipValueProperty, value);
            }
        }
    }
}

