namespace Telerik.Windows.Controls
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    [DefaultProperty("TickFrequency")]
    public class RadTickBar : Control
    {
        public static readonly DependencyProperty EnableSideTicksProperty = DependencyProperty.Register("EnableSideTicks", typeof(bool), typeof(RadTickBar), new PropertyMetadata(new PropertyChangedCallback(RadTickBar.OnPropertyChanged)));
        public static readonly DependencyProperty IsDirectionReversedProperty = DependencyProperty.Register("IsDirectionReversed", typeof(bool), typeof(RadTickBar), new PropertyMetadata(new PropertyChangedCallback(RadTickBar.OnPropertyChanged)));
        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof(double), typeof(RadTickBar), new PropertyMetadata(new PropertyChangedCallback(RadTickBar.OnPropertyChanged)));
        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum", typeof(double), typeof(RadTickBar), new PropertyMetadata(new PropertyChangedCallback(RadTickBar.OnPropertyChanged)));
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(System.Windows.Controls.Orientation), typeof(RadTickBar), new PropertyMetadata(new PropertyChangedCallback(RadTickBar.OnPropertyChanged)));
        private WeakReference parentReference = new WeakReference(null);
        public static readonly DependencyProperty PlacementProperty = DependencyProperty.Register("Placement", typeof(TickBarPlacement), typeof(RadTickBar), new PropertyMetadata(new PropertyChangedCallback(RadTickBar.OnPropertyChanged)));
        private double range;
        private Size tickBarSize;
        public static readonly DependencyProperty TickFrequencyProperty = DependencyProperty.Register("TickFrequency", typeof(double), typeof(RadTickBar), new PropertyMetadata(new PropertyChangedCallback(RadTickBar.OnPropertyChanged)));
        private Size tickSize = new Size();
        public static readonly DependencyProperty TicksProperty = DependencyProperty.Register("Ticks", typeof(DoubleCollection), typeof(RadTickBar), new PropertyMetadata(new PropertyChangedCallback(RadTickBar.OnPropertyChanged)));
        public static readonly DependencyProperty TickTemplateProperty = DependencyProperty.Register("TickTemplate", typeof(DataTemplate), typeof(RadTickBar), new PropertyMetadata(new PropertyChangedCallback(RadTickBar.OnPropertyChanged)));
        public static readonly DependencyProperty TickTemplateSelectorProperty = DependencyProperty.Register("TickTemplateSelector", typeof(DataTemplateSelector), typeof(RadTickBar), new PropertyMetadata(new PropertyChangedCallback(RadTickBar.OnTickTemplatePropertyChanged)));

        public RadTickBar()
        {
            base.DefaultStyleKey = typeof(RadTickBar);
            this.EnableSideTicks = true;
            base.SizeChanged += new SizeChangedEventHandler(this.RadTickBar_SizeChanged);
        }

        private void CreateTick(double tickValue)
        {
            double containerLength = (this.Orientation == System.Windows.Controls.Orientation.Horizontal) ? this.tickBarSize.Width : this.tickBarSize.Height;
            double physicalPoint = (tickValue * containerLength) / this.range;
            physicalPoint = double.IsNaN(physicalPoint) ? 0.0 : physicalPoint;
            FrameworkElement tick = this.LoadTemplate(tickValue);
            this.LayoutRoot.Children.Add(tick);
            this.tickSize = MeasureTick(tick);
            if (this.Orientation == System.Windows.Controls.Orientation.Horizontal)
            {
                double left = physicalPoint - (this.tickSize.Width / 2.0);
                left = !this.IsDirectionReversed ? left : ((this.tickBarSize.Width - left) - this.tickSize.Width);
                left = Math.Round(left, 0);
                Canvas.SetLeft(tick, left);
                if (double.IsNaN(this.LayoutRoot.Height))
                {
                    this.LayoutRoot.MinHeight = this.tickSize.Height;
                }
                else
                {
                    this.LayoutRoot.MinHeight = Math.Max(this.LayoutRoot.MinHeight, this.tickSize.Height);
                }
            }
            else
            {
                double top = physicalPoint - (this.tickSize.Height / 2.0);
                top = !this.IsDirectionReversed ? ((this.tickBarSize.Height - top) - this.tickSize.Height) : top;
                top = Math.Round(top, 0);
                Canvas.SetTop(tick, top);
                if (double.IsNaN(this.LayoutRoot.Width))
                {
                    this.LayoutRoot.MinWidth = this.tickSize.Width;
                }
                else
                {
                    this.LayoutRoot.MinWidth = Math.Max(this.LayoutRoot.MinWidth, this.tickSize.Width);
                }
            }
        }

        internal void DrawTicks()
        {
            if ((((this.LayoutRoot != null) && !double.IsInfinity(base.ActualWidth)) && !double.IsInfinity(base.ActualHeight)) && (((this.Ticks != null) || (this.TickFrequency != 0.0)) || this.EnableSideTicks))
            {
                this.tickBarSize = new Size(base.ActualWidth, base.ActualHeight);
                this.range = this.Maximum - this.Minimum;
                this.LayoutRoot.Children.Clear();
                if (this.EnableSideTicks)
                {
                    this.CreateTick(0.0);
                }
                double tickFrequency = this.TickFrequency;
                DoubleCollection ticks = this.Ticks;
                if ((ticks != null) && (ticks.Count > 0))
                {
                    for (int index = 0; index < ticks.Count; index++)
                    {
                        this.CreateTick(ticks[index] - this.Minimum);
                    }
                }
                else if (tickFrequency > 0.0)
                {
                    for (double tickValue = tickFrequency; tickValue < this.range; tickValue += tickFrequency)
                    {
                        this.CreateTick(tickValue);
                    }
                }
                if (this.EnableSideTicks)
                {
                    if ((this.Placement == TickBarPlacement.Left) || (this.Placement == TickBarPlacement.Right))
                    {
                        FrameworkElement tick = this.LoadTemplate(this.Maximum - this.Minimum);
                        this.LayoutRoot.Children.Add(tick);
                        this.tickSize = MeasureTick(tick);
                        double top = !this.IsDirectionReversed ? (-1.0 * (this.tickSize.Height / 2.0)) : (this.tickBarSize.Height - (this.tickSize.Height / 2.0));
                        top = Math.Round(top, 0);
                        Canvas.SetTop(tick, top);
                        double minHeight = 0.0;
                        foreach (FrameworkElement element in this.LayoutRoot.Children)
                        {
                            minHeight += element.DesiredSize.Height;
                        }
                        if (double.IsNaN(this.ParentSlider.Height))
                        {
                            this.LayoutRoot.MinHeight = minHeight;
                        }
                        else
                        {
                            this.LayoutRoot.MinHeight = Math.Min(minHeight, this.ParentSlider.Height);
                        }
                    }
                    else
                    {
                        FrameworkElement tick = this.LoadTemplate(this.Maximum - this.Minimum);
                        this.LayoutRoot.Children.Add(tick);
                        this.tickSize = MeasureTick(tick);
                        double left = !this.IsDirectionReversed ? (this.tickBarSize.Width - (this.tickSize.Width / 2.0)) : (-1.0 * (this.tickSize.Width / 2.0));
                        left = Math.Round(left, 0);
                        Canvas.SetLeft(tick, left);
                        double minWidth = 0.0;
                        foreach (FrameworkElement element in this.LayoutRoot.Children)
                        {
                            minWidth += element.DesiredSize.Width;
                        }
                        if (double.IsNaN(this.ParentSlider.Width))
                        {
                            this.LayoutRoot.MinWidth = minWidth;
                        }
                        else
                        {
                            this.LayoutRoot.MinWidth = Math.Min(minWidth, this.ParentSlider.Width);
                        }
                    }
                }
            }
        }

        private FrameworkElement LoadTemplate(double tickValue)
        {
            FrameworkElement tick = null;
            if (this.TickTemplate != null)
            {
                tick = this.TickTemplate.LoadContent() as FrameworkElement;
            }
            else if (this.TickTemplateSelector != null)
            {
                tick = this.TickTemplateSelector.SelectTemplate(tickValue + this.Minimum, null).LoadContent() as FrameworkElement;
            }
            else
            {
                tick = this.DefaultTickTemplate.LoadContent() as FrameworkElement;
            }
            tick.DataContext = tickValue + this.Minimum;
            return tick;
        }

        private static Size MeasureTick(FrameworkElement tick)
        {
            tick.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            tick.UpdateLayout();
            return tick.DesiredSize;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.LayoutRoot = base.GetTemplateChild("LayoutRoot") as Panel;
        }

        private static void OnPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (args.Property == TickTemplateProperty)
            {
                new Canvas { DataContext = args.NewValue };
            }
            RadTickBar tickBar = sender as RadTickBar;
            if (tickBar != null)
            {
                tickBar.DrawTicks();
            }
        }

        private static void OnTickTemplatePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadTickBar tickBar = sender as RadTickBar;
            if (tickBar != null)
            {
                tickBar.DrawTicks();
            }
        }

        private void RadTickBar_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.DrawTicks();
        }

        internal DataTemplate DefaultTickTemplate { get; set; }

        public bool EnableSideTicks
        {
            get
            {
                return (bool) base.GetValue(EnableSideTicksProperty);
            }
            set
            {
                base.SetValue(EnableSideTicksProperty, value);
            }
        }

        public bool IsDirectionReversed
        {
            get
            {
                return (bool) base.GetValue(IsDirectionReversedProperty);
            }
            set
            {
                base.SetValue(IsDirectionReversedProperty, value);
            }
        }

        private Panel LayoutRoot { get; set; }

        public double Maximum
        {
            get
            {
                return (double) base.GetValue(MaximumProperty);
            }
            set
            {
                base.SetValue(MaximumProperty, value);
            }
        }

        public double Minimum
        {
            get
            {
                return (double) base.GetValue(MinimumProperty);
            }
            set
            {
                base.SetValue(MinimumProperty, value);
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

        internal RadSlider ParentSlider
        {
            get
            {
                return (this.parentReference.Target as RadSlider);
            }
            set
            {
                this.parentReference = new WeakReference(value);
            }
        }

        public TickBarPlacement Placement
        {
            get
            {
                return (TickBarPlacement) base.GetValue(PlacementProperty);
            }
            set
            {
                base.SetValue(PlacementProperty, value);
            }
        }

        public double TickFrequency
        {
            get
            {
                return (double) base.GetValue(TickFrequencyProperty);
            }
            set
            {
                base.SetValue(TickFrequencyProperty, value);
            }
        }

        public DoubleCollection Ticks
        {
            get
            {
                return (DoubleCollection) base.GetValue(TicksProperty);
            }
            set
            {
                base.SetValue(TicksProperty, value);
            }
        }

        public DataTemplate TickTemplate
        {
            get
            {
                return (DataTemplate) base.GetValue(TickTemplateProperty);
            }
            set
            {
                base.SetValue(TickTemplateProperty, value);
            }
        }

        public DataTemplateSelector TickTemplateSelector
        {
            get
            {
                return (DataTemplateSelector) base.GetValue(TickTemplateSelectorProperty);
            }
            set
            {
                base.SetValue(TickTemplateSelectorProperty, value);
            }
        }
    }
}

