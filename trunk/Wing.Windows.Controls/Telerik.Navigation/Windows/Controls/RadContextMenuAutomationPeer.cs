namespace Telerik.Windows.Controls
{
    using System;
    using System.Windows.Automation;
    using System.Windows.Automation.Peers;
    using System.Windows.Automation.Provider;
    using System.Windows;

    public class RadContextMenuAutomationPeer : FrameworkElementAutomationPeer, IExpandCollapseProvider
    {
        public RadContextMenuAutomationPeer(RadContextMenu owner) : base((FrameworkElement) owner)
        {
        }

        public void Collapse()
        {
            this.EnsureEnabled();
            (base.Owner as RadContextMenu).IsOpen = false;
        }

        private void EnsureEnabled()
        {
            if (!base.IsEnabled())
            {
                throw new ElementNotEnabledException();
            }
        }

        public void Expand()
        {
            this.EnsureEnabled();
            (base.Owner as RadContextMenu).IsOpen = true;
        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Menu;
        }

        protected override string GetClassNameCore()
        {
            return "RadContextMenu";
        }

        protected override string GetNameCore()
        {
            string nameCore = base.GetNameCore();
            if (string.IsNullOrEmpty(nameCore))
            {
                nameCore = "Rad Context Menu";
            }
            return nameCore;
        }

        public System.Windows.Automation.ExpandCollapseState ExpandCollapseState
        {
            get
            {
                if (!(base.Owner as RadContextMenu).IsOpen)
                {
                    return System.Windows.Automation.ExpandCollapseState.Collapsed;
                }
                return System.Windows.Automation.ExpandCollapseState.Expanded;
            }
        }
    }
}

