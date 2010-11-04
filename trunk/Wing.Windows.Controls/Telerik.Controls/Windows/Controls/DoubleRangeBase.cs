namespace Telerik.Windows.Controls
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Windows;
    using System.Windows.Automation.Peers;
    using System.Windows.Controls.Primitives;

    public class DoubleRangeBase : RangeBase
    {
        private bool hasLoaded;
        public static readonly DependencyProperty IsSelectionRangeEnabledProperty = DependencyProperty.Register("IsSelectionRangeEnabled", typeof(bool), typeof(RadSlider), new PropertyMetadata(new PropertyChangedCallback(DoubleRangeBase.OnIsSelectionRangeEnabledPropertyChanged)));
        public static readonly DependencyProperty MaximumRangeSpanProperty = DependencyProperty.Register("MaximumRangeSpan", typeof(double), typeof(DoubleRangeBase), new PropertyMetadata((double)1.0 / (double)0.0, new PropertyChangedCallback(DoubleRangeBase.OnMaximumRangeSpanPropertyChanged)));
        public static readonly DependencyProperty MinimumRangeSpanProperty = DependencyProperty.Register("MinimumRangeSpan", typeof(double), typeof(DoubleRangeBase), new PropertyMetadata(new PropertyChangedCallback(DoubleRangeBase.OnMinimumRangeSpanPropertyChanged)));
        private double requestedSelectionEnd;
        private double requestedSelectionStart;
        public static readonly DependencyProperty SelectionEndProperty = DependencyProperty.Register("SelectionEnd", typeof(double), typeof(DoubleRangeBase), new PropertyMetadata(new PropertyChangedCallback(DoubleRangeBase.OnSelectionEndPropertyChanged)));
        public static readonly DependencyProperty SelectionStartProperty = DependencyProperty.Register("SelectionStart", typeof(double), typeof(DoubleRangeBase), new PropertyMetadata(new PropertyChangedCallback(DoubleRangeBase.OnSelectionStartPropertyChanged)));

        public event RoutedPropertyChangedEventHandler<double> MaximumRangeSpanChanged;

        public event RoutedPropertyChangedEventHandler<double> MinimumRangeSpanChanged;

        public event RoutedPropertyChangedEventHandler<double> SelectionEndChanged;

        public event RoutedPropertyChangedEventHandler<SelectionRangeChangedEventArgs> SelectionRangeChanged;

        public event RoutedPropertyChangedEventHandler<double> SelectionStartChanged;

        public DoubleRangeBase()
        {
            this.RaiseSelectionRangeChangedEvent = true;
            this.RaiseValueChangedEvent = true;
            base.Loaded += new RoutedEventHandler(this.DoubleRangeBase_Loaded);
        }

        private void CoerceSelectionEnd()
        {
            if (!IsValidDoubleValue(this.requestedSelectionEnd))
            {
                throw new ArgumentException("Invalid double value", SelectionEndProperty.ToString());
            }
            if (this.requestedSelectionEnd > base.Maximum)
            {
                base.SetValue(SelectionEndProperty, base.Maximum);
            }
            else if (this.requestedSelectionEnd < base.Minimum)
            {
                base.SetValue(SelectionEndProperty, base.Minimum);
            }
            else if (this.requestedSelectionEnd < this.SelectionStart)
            {
                base.SetValue(SelectionEndProperty, this.SelectionStart);
            }
            else if ((this.requestedSelectionEnd - this.MinimumRangeSpan).IsLessThan(this.SelectionStart))
            {
                if (this.ChangeMadeByUi)
                {
                    base.SetValue(SelectionEndProperty, Math.Min(base.Maximum, this.SelectionStart + this.MinimumRangeSpan));
                }
                else
                {
                    base.SetValue(SelectionStartProperty, Math.Max(base.Minimum, this.SelectionEnd - this.MinimumRangeSpan));
                }
            }
            else if (!double.IsPositiveInfinity(this.MaximumRangeSpan) && this.requestedSelectionEnd.IsGreaterThan((this.SelectionStart + this.MaximumRangeSpan)))
            {
                base.SetValue(SelectionEndProperty, Math.Max((double)(this.SelectionStart + this.MinimumRangeSpan), (double)(this.SelectionStart + this.MaximumRangeSpan)));
            }
            else
            {
                base.SetValue(SelectionEndProperty, this.requestedSelectionEnd);
            }
        }

        private void CoerceSelectionStart()
        {
            if (!IsValidDoubleValue(this.requestedSelectionStart))
            {
                throw new ArgumentException("Invalid double value", SelectionStartProperty.ToString());
            }
            if (this.requestedSelectionStart < base.Minimum)
            {
                base.SetValue(SelectionStartProperty, base.Minimum);
            }
            else if (this.requestedSelectionStart > base.Maximum)
            {
                base.SetValue(SelectionStartProperty, base.Maximum);
            }
            else if (this.requestedSelectionStart > this.SelectionEnd)
            {
                base.SetValue(SelectionStartProperty, this.SelectionEnd);
            }
            else if ((this.requestedSelectionStart + this.MinimumRangeSpan).IsGreaterThan(this.SelectionEnd))
            {
                if (this.ChangeMadeByUi)
                {
                    base.SetValue(SelectionStartProperty, Math.Max(base.Minimum, this.SelectionEnd - this.MinimumRangeSpan));
                }
                else
                {
                    base.SetValue(SelectionEndProperty, Math.Min(base.Maximum, this.SelectionStart + this.MinimumRangeSpan));
                }
            }
            else if (!double.IsPositiveInfinity(this.MaximumRangeSpan) && this.requestedSelectionStart.IsLessThan((this.SelectionEnd - this.MaximumRangeSpan)))
            {
                base.SetValue(SelectionStartProperty, Math.Min((double)(this.SelectionEnd - this.MinimumRangeSpan), (double)(this.SelectionEnd - this.MaximumRangeSpan)));
            }
            else
            {
                base.SetValue(SelectionStartProperty, this.requestedSelectionStart);
            }
        }

        private void DoubleRangeBase_Loaded(object sender, RoutedEventArgs e)
        {
            this.hasLoaded = true;
            if (this.IsSelectionRangeEnabled)
            {
                this.CoerceSelectionEnd();
                this.CoerceSelectionStart();
            }
        }

        private static bool IsValidDoubleValue(object value)
        {
            double number = (double)value;
            return (!double.IsNaN(number) && !double.IsInfinity(number));
        }

        protected virtual void OnIsSelectionRangeEnabledChanged()
        {
        }

        private static void OnIsSelectionRangeEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DoubleRangeBase doubleRangeBase = d as DoubleRangeBase;
            if (doubleRangeBase != null)
            {
                doubleRangeBase.OnIsSelectionRangeEnabledChanged();
            }
        }

        protected override void OnMaximumChanged(double oldMaximum, double newMaximum)
        {
            Telerik.Windows.Controls.RangeBaseAutomationPeer peer = FrameworkElementAutomationPeer.FromElement(this) as Telerik.Windows.Controls.RangeBaseAutomationPeer;
            if (peer != null)
            {
                peer.RaiseMaximumPropertyChangedEvent(oldMaximum, newMaximum);
            }
            base.OnMaximumChanged(oldMaximum, newMaximum);
            this.CoerceSelectionStart();
            this.CoerceSelectionEnd();
            if (this.hasLoaded && this.IsSelectionRangeEnabled)
            {
                this.ChangeMadeByUi = false;
                if ((base.Maximum - base.Minimum).IsLessThan(this.MinimumRangeSpan))
                {
                    base.SetValue(RangeBase.MaximumProperty, this.MinimumRangeSpan);
                }
            }
        }

        protected virtual void OnMaximumRangeSpanChanged(double oldValue, double newValue)
        {
            RoutedPropertyChangedEventHandler<double> handler = this.MaximumRangeSpanChanged;
            if (handler != null)
            {
                handler(this, new RoutedPropertyChangedEventArgs<double>(oldValue, newValue));
            }
        }

        private static void OnMaximumRangeSpanPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DoubleRangeBase doubleRangeBase = d as DoubleRangeBase;
            if ((doubleRangeBase != null) && doubleRangeBase.IsSelectionRangeEnabled)
            {
                doubleRangeBase.OnMaximumRangeSpanChanged((double)e.OldValue, (double)e.NewValue);
            }
        }

        protected override void OnMinimumChanged(double oldMinimum, double newMinimum)
        {
            Telerik.Windows.Controls.RangeBaseAutomationPeer peer = FrameworkElementAutomationPeer.FromElement(this) as Telerik.Windows.Controls.RangeBaseAutomationPeer;
            if (peer != null)
            {
                peer.RaiseMinimumPropertyChangedEvent(oldMinimum, newMinimum);
            }
            base.OnMinimumChanged(oldMinimum, newMinimum);
            this.CoerceSelectionEnd();
            this.CoerceSelectionStart();
            if (this.hasLoaded && this.IsSelectionRangeEnabled)
            {
                this.ChangeMadeByUi = false;
                if ((base.Maximum - base.Minimum).IsLessThan(this.MinimumRangeSpan))
                {
                    base.SetValue(RangeBase.MinimumProperty, base.Maximum - this.MinimumRangeSpan);
                }
            }
        }

        protected virtual void OnMinimumRangeSpanChanged(double oldValue, double newValue)
        {
            RoutedPropertyChangedEventHandler<double> handler = this.MinimumRangeSpanChanged;
            if (handler != null)
            {
                handler(this, new RoutedPropertyChangedEventArgs<double>(oldValue, newValue));
            }
        }

        private static void OnMinimumRangeSpanPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DoubleRangeBase doubleRangeBase = d as DoubleRangeBase;
            if ((doubleRangeBase != null) && doubleRangeBase.IsSelectionRangeEnabled)
            {
                doubleRangeBase.OnMinimumRangeSpanChanged((double)e.OldValue, (double)e.NewValue);
            }
        }

        protected virtual void OnSelectionEndChanged(double oldValue, double newValue)
        {
            RoutedPropertyChangedEventHandler<double> handler = this.SelectionEndChanged;
            if (handler != null)
            {
                handler(this, new RoutedPropertyChangedEventArgs<double>(oldValue, newValue));
            }
        }

        private static void OnSelectionEndPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DoubleRangeBase doubleRangeBase = d as DoubleRangeBase;
            if (doubleRangeBase != null)
            {
                doubleRangeBase.requestedSelectionEnd = (double)e.NewValue;
                if (doubleRangeBase.IsSelectionRangeEnabled && doubleRangeBase.hasLoaded)
                {
                    doubleRangeBase.OnSelectionEndChanged((double)e.OldValue, (double)e.NewValue);
                    doubleRangeBase.CoerceSelectionEnd();
                    if (doubleRangeBase.RaiseSelectionRangeChangedEvent)
                    {
                        doubleRangeBase.OnSelectionRangeChanged(new SelectionRangeChangedEventArgs(doubleRangeBase.SelectionStart, (double)e.OldValue), new SelectionRangeChangedEventArgs(doubleRangeBase.SelectionStart, (double)e.NewValue));
                    }
                }
            }
        }

        protected virtual void OnSelectionRangeChanged(SelectionRangeChangedEventArgs oldValue, SelectionRangeChangedEventArgs newValue)
        {
            RoutedPropertyChangedEventHandler<SelectionRangeChangedEventArgs> handler = this.SelectionRangeChanged;
            if (handler != null)
            {
                handler(this, new RoutedPropertyChangedEventArgs<SelectionRangeChangedEventArgs>(oldValue, newValue));
            }
        }

        protected virtual void OnSelectionStartChanged(double oldValue, double newValue)
        {
            RoutedPropertyChangedEventHandler<double> handler = this.SelectionStartChanged;
            if (handler != null)
            {
                handler(this, new RoutedPropertyChangedEventArgs<double>(oldValue, newValue));
            }
        }

        private static void OnSelectionStartPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DoubleRangeBase doubleRangeBase = d as DoubleRangeBase;
            if (doubleRangeBase != null)
            {
                doubleRangeBase.requestedSelectionStart = (double)e.NewValue;
                if (doubleRangeBase.IsSelectionRangeEnabled && doubleRangeBase.hasLoaded)
                {
                    doubleRangeBase.OnSelectionStartChanged((double)e.OldValue, (double)e.NewValue);
                    doubleRangeBase.CoerceSelectionStart();
                    if (doubleRangeBase.RaiseSelectionRangeChangedEvent)
                    {
                        doubleRangeBase.OnSelectionRangeChanged(new SelectionRangeChangedEventArgs((double)e.OldValue, doubleRangeBase.SelectionEnd), new SelectionRangeChangedEventArgs((double)e.NewValue, doubleRangeBase.SelectionEnd));
                    }
                }
            }
        }

        internal bool ChangeMadeByUi { get; set; }

        public bool IsSelectionRangeEnabled
        {
            get
            {
                return (bool)base.GetValue(IsSelectionRangeEnabledProperty);
            }
            set
            {
                base.SetValue(IsSelectionRangeEnabledProperty, value);
            }
        }

        public double MaximumRangeSpan
        {
            get
            {
                return (double)base.GetValue(MaximumRangeSpanProperty);
            }
            set
            {
                base.SetValue(MaximumRangeSpanProperty, value);
            }
        }

        public double MinimumRangeSpan
        {
            get
            {
                return (double)base.GetValue(MinimumRangeSpanProperty);
            }
            set
            {
                base.SetValue(MinimumRangeSpanProperty, value);
            }
        }

        internal bool RaiseSelectionRangeChangedEvent { get; set; }

        internal bool RaiseValueChangedEvent { get; set; }

        public double SelectionEnd
        {
            get
            {
                return (double)base.GetValue(SelectionEndProperty);
            }
            set
            {
                base.SetValue(SelectionEndProperty, value);
            }
        }

        public double SelectionRange
        {
            get
            {
                return (this.SelectionEnd - this.SelectionStart);
            }
        }

        public double SelectionStart
        {
            get
            {
                return (double)base.GetValue(SelectionStartProperty);
            }
            set
            {
                base.SetValue(SelectionStartProperty, value);
            }
        }
    }
}

