namespace Telerik.Windows.Controls
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Windows.Automation;
    using System.Windows.Automation.Peers;
    using System.Windows.Automation.Provider;
    using System.Windows.Controls.Primitives;
    using System.Windows;

    public class RangeBaseAutomationPeer : FrameworkElementAutomationPeer, IRangeValueProvider
    {
        public RangeBaseAutomationPeer(RangeBase owner) : base((FrameworkElement) owner)
        {
        }

        internal virtual double GetValue()
        {
            return this.OwnerAsRangeBase().Value;
        }

        internal void GuarantyEnabled()
        {
            if (!base.IsEnabled())
            {
                throw new ElementNotEnabledException();
            }
        }

        private RangeBase OwnerAsRangeBase()
        {
            RangeBase owner = base.Owner as RangeBase;
            if (owner == null)
            {
                throw new InvalidOperationException("The Owner have to be a RangeBase or a Descendant");
            }
            return owner;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal void RaiseIsReadOnlyPropertyChangedEvent(bool oldValue, bool newValue)
        {
            base.RaisePropertyChangedEvent(RangeValuePatternIdentifiers.IsReadOnlyProperty, oldValue, newValue);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal void RaiseLargeChangePropertyChangedEvent(double oldValue, double newValue)
        {
            base.RaisePropertyChangedEvent(RangeValuePatternIdentifiers.LargeChangeProperty, oldValue, newValue);
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
        internal void RaiseSmallChangePropertyChangedEvent(double oldValue, double newValue)
        {
            base.RaisePropertyChangedEvent(RangeValuePatternIdentifiers.SmallChangeProperty, oldValue, newValue);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal void RaiseValuePropertyChangedEvent(double oldValue, double newValue)
        {
            base.RaisePropertyChangedEvent(RangeValuePatternIdentifiers.ValueProperty, oldValue, newValue);
        }

        internal virtual void SetOwnerValue(double value)
        {
            this.GuarantyEnabled();
            RangeBase owner = this.OwnerAsRangeBase();
            if ((value < owner.Minimum) || (value > owner.Maximum))
            {
                throw new ArgumentOutOfRangeException("value");
            }
            owner.Value = value;
        }

        void IRangeValueProvider.SetValue(double value)
        {
            this.SetOwnerValue(value);
        }

        public bool IsReadOnly
        {
            get
            {
                return !base.IsEnabled();
            }
        }

        public double LargeChange
        {
            get
            {
                return this.OwnerAsRangeBase().LargeChange;
            }
        }

        public double Maximum
        {
            get
            {
                return this.OwnerAsRangeBase().Maximum;
            }
        }

        public double Minimum
        {
            get
            {
                return this.OwnerAsRangeBase().Minimum;
            }
        }

        public double SmallChange
        {
            get
            {
                return this.OwnerAsRangeBase().SmallChange;
            }
        }

        public double Value
        {
            get
            {
                return this.GetValue();
            }
        }
    }
}

