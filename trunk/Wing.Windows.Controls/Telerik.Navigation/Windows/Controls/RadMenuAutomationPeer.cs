namespace Telerik.Windows.Controls
{
    using System;
    using System.Windows.Automation.Peers;
    using System.Windows;

    public class RadMenuAutomationPeer : FrameworkElementAutomationPeer
    {
        public RadMenuAutomationPeer(RadMenu owner) : base((FrameworkElement) owner)
        {
        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Menu;
        }

        protected override string GetClassNameCore()
        {
            return "RadMenu";
        }

        protected override string GetNameCore()
        {
            string nameCore = base.GetNameCore();
            if (string.IsNullOrEmpty(nameCore))
            {
                nameCore = "Rad Menu";
            }
            return nameCore;
        }
    }
}

