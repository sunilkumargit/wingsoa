namespace Telerik.Windows.Controls
{
    using System;
    using System.Windows;
    using System.Windows.Automation.Peers;
    using System.Windows.Automation.Provider;

    public class RadProgressBarAutomationPeer : Telerik.Windows.Controls.RangeBaseAutomationPeer, IRangeValueProvider
    {
        public RadProgressBarAutomationPeer(RadProgressBar owner) : base(owner)
        {
        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.ProgressBar;
        }

        protected override string GetClassNameCore()
        {
            return "RadProgressBar";
        }

        protected override Point GetClickablePointCore()
        {
            return new Point(double.NaN, double.NaN);
        }

        public override object GetPattern(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.RangeValue)
            {
                return this;
            }
            return null;
        }

        internal override void SetOwnerValue(double value)
        {
            throw new InvalidOperationException(string.Empty);
        }

        bool IRangeValueProvider.IsReadOnly
        {
            get
            {
                return true;
            }
        }

        double IRangeValueProvider.LargeChange
        {
            get
            {
                return double.NaN;
            }
        }

        double IRangeValueProvider.SmallChange
        {
            get
            {
                return double.NaN;
            }
        }
    }
}

