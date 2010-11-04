namespace Telerik.Windows.Controls
{
    using System;
    using System.Windows;
    using System.Windows.Automation.Peers;

    public class RadSliderAutomationPeer : Telerik.Windows.Controls.RangeBaseAutomationPeer
    {
        public RadSliderAutomationPeer(RadSlider owner) : base(owner)
        {
        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Slider;
        }

        protected override string GetClassNameCore()
        {
            return "RadSlider";
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
    }
}

