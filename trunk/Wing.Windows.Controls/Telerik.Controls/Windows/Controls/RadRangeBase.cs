namespace Telerik.Windows.Controls
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Threading;
    using System.Windows;
    using System.Windows.Automation.Peers;
    using System.Windows.Controls;
    using Telerik.Windows.Controls.Automation.Peers;

    public abstract class RadRangeBase : Control
    {
        public static readonly DependencyProperty AutoReverseProperty = DependencyProperty.Register("AutoReverse", typeof(bool), typeof(RadRangeBase), null);
        private double initialMax;
        private double? initialVal;
        public static readonly DependencyProperty LargeChangeProperty = DependencyProperty.Register("LargeChange", typeof(double), typeof(RadRangeBase), new PropertyMetadata(10.0, new PropertyChangedCallback(RadRangeBase.OnLargeChangePropertyChanged)));
        private int levelsFromRootCall;
        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof(double), typeof(RadRangeBase), new PropertyMetadata(1.7976931348623157E+308, new PropertyChangedCallback(RadRangeBase.OnMaximumPropertyChanged)));
        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum", typeof(double), typeof(RadRangeBase), new PropertyMetadata(-1.7976931348623157E+308, new PropertyChangedCallback(RadRangeBase.OnMinimumPropertyChanged)));
        private double requestedMax = double.MaxValue;
        private double? requestedVal;
        public static readonly DependencyProperty SmallChangeProperty = DependencyProperty.Register("SmallChange", typeof(double), typeof(RadRangeBase), new PropertyMetadata(1.0, new PropertyChangedCallback(RadRangeBase.OnSmallChangePropertyChanged)));
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(double?), typeof(RadRangeBase), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(RadRangeBase.OnValuePropertyChanged)));

        public event EventHandler<RadRangeBaseValueChangedEventArgs> ValueChanged;

        protected RadRangeBase()
        {
        }

        protected internal virtual void ChangeValue(double delta)
        {
            if (!this.Value.HasValue)
            {
                this.Value = 0.0;
            }
            else
            {
                if (this.AutoReverse)
                {
                    if (((this.Value.GetValueOrDefault() == this.Maximum) && this.Value.HasValue) && (delta > 0.0))
                    {
                        this.Value = new double?(this.Minimum);
                        return;
                    }
                }
                if (this.AutoReverse)
                {
                    if (((this.Value.GetValueOrDefault() == this.Minimum) && this.Value.HasValue) && (delta < 0.0))
                    {
                        this.Value = new double?(this.Maximum);
                        return;
                    }
                }
                this.Value = this.Value.HasValue ? new double?(this.Value.GetValueOrDefault() + delta) : null;
            }
        }

        private void CoerceMaximum()
        {
            double minimum = this.Minimum;
            double maximum = this.Maximum;
            if (!DoubleUtilAreClose(new double?(this.requestedMax), new double?(maximum)) && (this.requestedMax >= minimum))
            {
                this.SetCurrentValue(MaximumProperty, this.requestedMax);
            }
            else if (maximum < minimum)
            {
                this.SetCurrentValue(MaximumProperty, minimum);
            }
        }

        private void CoerceValue()
        {
            double minimum = this.Minimum;
            double maximum = this.Maximum;
            double? value = this.Value;
            if (!DoubleUtilAreClose(this.requestedVal, value))
            {
                double? _reqVal = this.requestedVal;
                if ((_reqVal.GetValueOrDefault() >= minimum) && _reqVal.HasValue)
                {
                    if ((_reqVal.GetValueOrDefault() <= maximum) && _reqVal.HasValue)
                    {
                        this.SetCurrentValue(ValueProperty, this.requestedVal);
                        return;
                    }
                }
            }
            if ((value.GetValueOrDefault() < minimum) && value.HasValue)
            {
                this.SetCurrentValue(ValueProperty, minimum);
            }
            if ((value.GetValueOrDefault() > maximum) && value.HasValue)
            {
                this.SetCurrentValue(ValueProperty, maximum);
            }
        }

        private static bool DoubleUtilAreClose(double? one, double? two)
        {
            return ((one.HasValue && two.HasValue) && DoubleUtil.AreClose(one.Value, two.Value));
        }

        private static bool IsValidChange(object value)
        {
            double val = (double) value;
            return (IsValidDoubleValue(val) && (val >= 0.0));
        }

        private static bool IsValidDoubleValue(double? value)
        {
            return (!value.HasValue || (!double.IsNaN(value.Value) && !double.IsInfinity(value.Value)));
        }

        private static bool IsValidDoubleValue(double value)
        {
            return (!double.IsNaN(value) && !double.IsInfinity(value));
        }

        private static void OnLargeChangePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!IsValidChange(e.NewValue))
            {
                throw new ArgumentException("Invalid LargeChange");
            }
        }

        protected virtual void OnMaximumChanged(double oldMaximum, double newMaximum)
        {
        }

        private static void OnMaximumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadRangeBase nullableRangeBase = d as RadRangeBase;
            if (!IsValidDoubleValue((double) e.NewValue))
            {
                throw new ArgumentException("Invalid Maximum");
            }
            if (nullableRangeBase.levelsFromRootCall == 0)
            {
                nullableRangeBase.requestedMax = (double) e.NewValue;
                nullableRangeBase.initialMax = (double) e.OldValue;
                nullableRangeBase.initialVal = nullableRangeBase.Value;
            }
            nullableRangeBase.levelsFromRootCall++;
            nullableRangeBase.CoerceMaximum();
            nullableRangeBase.CoerceValue();
            nullableRangeBase.levelsFromRootCall--;
            if (nullableRangeBase.levelsFromRootCall == 0)
            {
                RadRangeBaseAutomationPeer automationPeer = FrameworkElementAutomationPeer.FromElement(nullableRangeBase) as RadRangeBaseAutomationPeer;
                double maximum = nullableRangeBase.Maximum;
                if (!DoubleUtilAreClose(new double?(nullableRangeBase.initialMax), new double?(maximum)))
                {
                    if (automationPeer != null)
                    {
                        automationPeer.RaiseMaximumPropertyChangedEvent(nullableRangeBase.initialMax, maximum);
                    }
                    nullableRangeBase.OnMaximumChanged(nullableRangeBase.initialMax, maximum);
                }
                double? value = nullableRangeBase.Value;
                if (!DoubleUtilAreClose(nullableRangeBase.initialVal, value))
                {
                    if (automationPeer != null)
                    {
                        automationPeer.RaiseValuePropertyChangedEvent(nullableRangeBase.initialVal, value);
                    }
                    nullableRangeBase.OnValueChanged(nullableRangeBase.initialVal, value);
                }
            }
        }

        protected virtual void OnMinimumChanged(double oldMinimum, double newMinimum)
        {
        }

        private static void OnMinimumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadRangeBase nullableRangeBase = d as RadRangeBase;
            if (!IsValidDoubleValue((double) e.NewValue))
            {
                throw new ArgumentException("Invalid Minimum");
            }
            if (nullableRangeBase.levelsFromRootCall == 0)
            {
                nullableRangeBase.initialMax = nullableRangeBase.Maximum;
                nullableRangeBase.initialVal = nullableRangeBase.Value;
            }
            nullableRangeBase.levelsFromRootCall++;
            nullableRangeBase.CoerceMaximum();
            nullableRangeBase.CoerceValue();
            nullableRangeBase.levelsFromRootCall--;
            if (nullableRangeBase.levelsFromRootCall == 0)
            {
                RadRangeBaseAutomationPeer automationPeer = FrameworkElementAutomationPeer.FromElement(nullableRangeBase) as RadRangeBaseAutomationPeer;
                if (automationPeer != null)
                {
                    automationPeer.RaiseMinimumPropertyChangedEvent((double) e.OldValue, (double) e.NewValue);
                }
                nullableRangeBase.OnMinimumChanged((double) e.OldValue, (double) e.NewValue);
                double maximum = nullableRangeBase.Maximum;
                if (!DoubleUtilAreClose(new double?(nullableRangeBase.initialMax), new double?(maximum)))
                {
                    if (automationPeer != null)
                    {
                        automationPeer.RaiseMaximumPropertyChangedEvent(nullableRangeBase.initialMax, maximum);
                    }
                    nullableRangeBase.OnMaximumChanged(nullableRangeBase.initialMax, maximum);
                }
                double? value = nullableRangeBase.Value;
                if (!DoubleUtilAreClose(nullableRangeBase.initialVal, value))
                {
                    if (automationPeer != null)
                    {
                        automationPeer.RaiseValuePropertyChangedEvent(nullableRangeBase.initialVal, value);
                    }
                    nullableRangeBase.OnValueChanged(nullableRangeBase.initialVal, value);
                }
            }
        }

        private static void OnSmallChangePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!IsValidChange(e.NewValue))
            {
                throw new ArgumentException("Invalid SmallChange");
            }
        }

        protected virtual void OnValueChanged(RadRangeBaseValueChangedEventArgs e)
        {
            EventHandler<RadRangeBaseValueChangedEventArgs> valueChanged = this.ValueChanged;
            if (valueChanged != null)
            {
                valueChanged(this, e);
            }
        }

        protected virtual void OnValueChanged(double? oldValue, double? newValue)
        {
            this.OnValueChanged(new RadRangeBaseValueChangedEventArgs(oldValue, newValue));
        }

        private static void OnValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadRangeBase nullableRangeBase = d as RadRangeBase;
            if (!IsValidDoubleValue((double?) e.NewValue))
            {
                throw new ArgumentException("Invalid Value");
            }
            if (nullableRangeBase.levelsFromRootCall == 0)
            {
                nullableRangeBase.requestedVal = (double?) e.NewValue;
                nullableRangeBase.initialVal = (double?) e.OldValue;
            }
            nullableRangeBase.levelsFromRootCall++;
            nullableRangeBase.CoerceValue();
            nullableRangeBase.levelsFromRootCall--;
            if (nullableRangeBase.levelsFromRootCall == 0)
            {
                double? value = nullableRangeBase.Value;
                if (!DoubleUtilAreClose(nullableRangeBase.initialVal, value))
                {
                    RadRangeBaseAutomationPeer automationPeer = FrameworkElementAutomationPeer.FromElement(nullableRangeBase) as RadRangeBaseAutomationPeer;
                    if (automationPeer != null)
                    {
                        automationPeer.RaiseValuePropertyChangedEvent(nullableRangeBase.initialVal, value);
                    }
                    nullableRangeBase.OnValueChanged(nullableRangeBase.initialVal, value);
                }
            }
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0} Minimum:{1} Maximum:{2} Value:{3}", new object[] { base.ToString(), this.Minimum, this.Maximum, this.Value });
        }

        public bool AutoReverse
        {
            get
            {
                return (bool) base.GetValue(AutoReverseProperty);
            }
            set
            {
                base.SetValue(AutoReverseProperty, value);
            }
        }

        public double LargeChange
        {
            get
            {
                return (double) base.GetValue(LargeChangeProperty);
            }
            set
            {
                base.SetValue(LargeChangeProperty, value);
            }
        }

        [TypeConverter(typeof(NullableDoubleConverter))]
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

        [TypeConverter(typeof(NullableDoubleConverter))]
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

        public double SmallChange
        {
            get
            {
                return (double) base.GetValue(SmallChangeProperty);
            }
            set
            {
                base.SetValue(SmallChangeProperty, value);
            }
        }

        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods"), TypeConverter(typeof(NullableDoubleConverter))]
        public double? Value
        {
            get
            {
                return (double?) base.GetValue(ValueProperty);
            }
            set
            {
                base.SetValue(ValueProperty, value);
            }
        }
    }
}

