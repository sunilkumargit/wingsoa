namespace Telerik.Windows.Controls
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Shapes;
    using System.Windows.Threading;
    using Telerik.Windows;
    using Telerik.Windows.Controls.Animation;

    [TemplateVisualState(Name="Hidden", GroupName="VisibilityStates"), TemplateVisualState(Name="Visible", GroupName="VisibilityStates"), TemplateVisualState(Name="Busy", GroupName="BusyStatusStates"), TemplateVisualState(Name="Idle", GroupName="BusyStatusStates"), StyleTypedProperty(Property="OverlayStyle", StyleTargetType=typeof(Rectangle)), StyleTypedProperty(Property="ProgressBarStyle", StyleTargetType=typeof(RadProgressBar))]
    public class RadBusyIndicator : ContentControl
    {
        private DispatcherTimer _displayAfterTimer;
        public static readonly DependencyProperty BusyContentProperty = DependencyProperty.Register("BusyContent", typeof(object), typeof(RadBusyIndicator), new Telerik.Windows.PropertyMetadata(null));
        public static readonly DependencyProperty BusyContentTemplateProperty = DependencyProperty.Register("BusyContentTemplate", typeof(DataTemplate), typeof(RadBusyIndicator), new Telerik.Windows.PropertyMetadata(null));
        public static readonly DependencyProperty DisplayAfterProperty = DependencyProperty.Register("DisplayAfter", typeof(TimeSpan), typeof(RadBusyIndicator), new Telerik.Windows.PropertyMetadata(TimeSpan.FromSeconds(0.1)));
        private static readonly DependencyPropertyKey IsBusyIndicationVisiblePropertyKey = DependencyPropertyExtensions.RegisterReadOnly("IsBusyIndicationVisible", typeof(bool), typeof(RadBusyIndicator), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadBusyIndicator.OnIsBusyIndicationVisiblePropertyChanged)));
        public static readonly DependencyProperty IsBusyIndicationVisibleProperty = IsBusyIndicationVisiblePropertyKey.DependencyProperty;
        public static readonly DependencyProperty IsBusyProperty = DependencyProperty.Register("IsBusy", typeof(bool), typeof(RadBusyIndicator), new Telerik.Windows.PropertyMetadata(false, new PropertyChangedCallback(RadBusyIndicator.OnIsBusyChanged)));
        private bool isContentVisible;
        public static readonly DependencyProperty IsIndeterminateProperty = DependencyProperty.Register("IsIndeterminate", typeof(bool), typeof(RadBusyIndicator), new Telerik.Windows.PropertyMetadata(true));
        public static readonly DependencyProperty OverlayStyleProperty = DependencyProperty.Register("OverlayStyle", typeof(Style), typeof(RadBusyIndicator), new Telerik.Windows.PropertyMetadata(null));
        public static readonly DependencyProperty ProgressBarStyleProperty = DependencyProperty.Register("ProgressBarStyle", typeof(Style), typeof(RadBusyIndicator), new Telerik.Windows.PropertyMetadata(null));
        public static readonly DependencyProperty ProgressValueProperty = DependencyProperty.Register("ProgressValue", typeof(double), typeof(RadBusyIndicator), new Telerik.Windows.PropertyMetadata(0.0));

        public RadBusyIndicator()
        {
            base.DefaultStyleKey = typeof(RadBusyIndicator);
            this._displayAfterTimer = new DispatcherTimer();
            base.Loaded += delegate {
                this._displayAfterTimer.Tick += new EventHandler(this.DisplayAfterTimerElapsed);
            };
            base.Unloaded += delegate {
                this._displayAfterTimer.Tick -= new EventHandler(this.DisplayAfterTimerElapsed);
                this._displayAfterTimer.Stop();
            };
            
        }

        protected virtual void ChangeVisualState(bool useTransitions)
        {
            if (this.IsBusy)
            {
                VisualStateManager.GoToState(this, "Busy", useTransitions);
                VisualStateManager.GoToState(this, this.IsContentVisible ? "Visible" : "Hidden", useTransitions);
            }
            else
            {
                VisualStateManager.GoToState(this, "Idle", useTransitions);
                VisualStateManager.GoToState(this, this.IsContentVisible ? "Visible" : "Hidden", useTransitions);
            }
        }

        private void DisplayAfterTimerElapsed(object sender, EventArgs e)
        {
            this._displayAfterTimer.Stop();
            this.IsContentVisible = true;
            this.IsBusyIndicationVisible = true;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.ChangeVisualState(false);
        }

        protected virtual void OnIsBusyChanged(DependencyPropertyChangedEventArgs e)
        {
            this.SetPropertiesAccordingIsBusy();
        }

        private static void OnIsBusyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RadBusyIndicator) d).OnIsBusyChanged(e);
        }

        private void OnIsBusyIndicationVisibleChanged(DependencyPropertyChangedEventArgs e)
        {
            this.ChangeVisualState(true);
            if ((bool) e.NewValue)
            {
                AnimationManager.Play(this, "Show");
            }
        }

        private static void OnIsBusyIndicationVisiblePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RadBusyIndicator) d).OnIsBusyIndicationVisibleChanged(e);
        }

        private void SetPropertiesAccordingIsBusy()
        {
            if (this.IsBusy)
            {
                if (this.DisplayAfter.Equals(TimeSpan.Zero))
                {
                    this.IsContentVisible = true;
                    this.IsBusyIndicationVisible = true;
                }
                else
                {
                    this._displayAfterTimer.Interval = this.DisplayAfter;
                    this._displayAfterTimer.Start();
                }
            }
            else
            {
                this._displayAfterTimer.Stop();
                this.IsContentVisible = false;
                AnimationManager.Play(this, "Hide", delegate {
                    if (!this.IsBusyIndicationVisible)
                    {
                        this.ChangeVisualState(true);
                    }
                    this.IsBusyIndicationVisible = false;
                }, new object[0]);
            }
        }

        public object BusyContent
        {
            get
            {
                return base.GetValue(BusyContentProperty);
            }
            set
            {
                base.SetValue(BusyContentProperty, value);
            }
        }

        public DataTemplate BusyContentTemplate
        {
            get
            {
                return (DataTemplate) base.GetValue(BusyContentTemplateProperty);
            }
            set
            {
                base.SetValue(BusyContentTemplateProperty, value);
            }
        }

        public TimeSpan DisplayAfter
        {
            get
            {
                return (TimeSpan) base.GetValue(DisplayAfterProperty);
            }
            set
            {
                base.SetValue(DisplayAfterProperty, value);
            }
        }

        public bool IsBusy
        {
            get
            {
                return (bool) base.GetValue(IsBusyProperty);
            }
            set
            {
                base.SetValue(IsBusyProperty, value);
            }
        }

        public bool IsBusyIndicationVisible
        {
            get
            {
                return (bool) base.GetValue(IsBusyIndicationVisibleProperty);
            }
            private set
            {
                this.SetValue(IsBusyIndicationVisiblePropertyKey, value);
            }
        }

        protected bool IsContentVisible
        {
            get
            {
                return this.isContentVisible;
            }
            set
            {
                this.isContentVisible = value;
            }
        }

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

        public Style OverlayStyle
        {
            get
            {
                return (Style) base.GetValue(OverlayStyleProperty);
            }
            set
            {
                base.SetValue(OverlayStyleProperty, value);
            }
        }

        public Style ProgressBarStyle
        {
            get
            {
                return (Style) base.GetValue(ProgressBarStyleProperty);
            }
            set
            {
                base.SetValue(ProgressBarStyleProperty, value);
            }
        }

        public double ProgressValue
        {
            get
            {
                return (double) base.GetValue(ProgressValueProperty);
            }
            set
            {
                base.SetValue(ProgressValueProperty, value);
            }
        }
    }
}

