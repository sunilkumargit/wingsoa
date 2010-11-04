namespace Telerik.Windows.Controls.Automation.Peers
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Windows.Automation;
    using System.Windows.Automation.Peers;
    using System.Windows.Automation.Provider;
    using Telerik.Windows.Controls;
    using System.Windows;

    public class RadRangeBaseAutomationPeer : FrameworkElementAutomationPeer, IRangeValueProvider
    {
        public RadRangeBaseAutomationPeer(RadRangeBase owner) : base((FrameworkElement) owner)
        {
        }

        public override object GetPattern(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.RangeValue)
            {
                return this;
            }
            return null;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal void RaiseMaximumPropertyChangedEvent(double oldValue, double newValue)
        {
            base.RaisePropertyChangedEvent(RangeValuePatternIdentifiers.MaximumProperty, oldValue, newValue);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal void RaiseMinimumPropertyChangedEvent(double oldValue, double newValue)
        {
            base.RaisePropertyChangedEvent(RangeValuePatternIdentifiers.MinimumProperty, oldValue, newValue);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal void RaiseValuePropertyChangedEvent(double? oldValue, double? newValue)
        {
            base.RaisePropertyChangedEvent(RangeValuePatternIdentifiers.ValueProperty, oldValue, newValue);
        }

        void IRangeValueProvider.SetValue(double val)
        {
            if (!base.IsEnabled())
            {
                throw new ElementNotEnabledException();
            }
            RadRangeBase owner = (RadRangeBase) base.Owner;
            if ((val < owner.Minimum) || (val > owner.Maximum))
            {
                throw new ArgumentOutOfRangeException("val");
            }
            owner.Value = new double?(val);
        }

        bool IRangeValueProvider.IsReadOnly
        {
            get
            {
                return !base.IsEnabled();
            }
        }

        double IRangeValueProvider.LargeChange
        {
            get
            {
                return ((RadRangeBase) base.Owner).LargeChange;
            }
        }

        double IRangeValueProvider.Maximum
        {
            get
            {
                return ((RadRangeBase) base.Owner).Maximum;
            }
        }

        double IRangeValueProvider.Minimum
        {
            get
            {
                return ((RadRangeBase) base.Owner).Minimum;
            }
        }

        double IRangeValueProvider.SmallChange
        {
            get
            {
                return ((RadRangeBase) base.Owner).SmallChange;
            }
        }

        double IRangeValueProvider.Value
        {
            get
            {
                double? value = ((RadRangeBase) base.Owner).Value;
                if (!value.HasValue)
                {
                    return 0.0;
                }
                return value.Value;
            }
        }
    }
}

