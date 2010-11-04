namespace Telerik.Windows.Controls.Automation.Peers
{
    using System;
    using System.Windows.Automation.Peers;
    using System.Windows.Automation.Provider;
    using Telerik.Windows.Controls;

    public class ListBoxItemAutomationPeer : Telerik.Windows.Controls.Automation.Peers.SelectorItemAutomationPeer, IScrollItemProvider
    {
        public ListBoxItemAutomationPeer(object owner, Telerik.Windows.Controls.Automation.Peers.SelectorAutomationPeer selectorAutomationPeer) : base(owner, selectorAutomationPeer)
        {
        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.ListItem;
        }

        protected override string GetClassNameCore()
        {
            return "ListBoxItem";
        }

        public override object GetPattern(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.ScrollItem)
            {
                return this;
            }
            return base.GetPattern(patternInterface);
        }

        void IScrollItemProvider.ScrollIntoView()
        {
            ListBox owner = base.ItemsControlAutomationPeer.Owner as ListBox;
            if (owner != null)
            {
                owner.ScrollIntoView(base.Item);
            }
        }
    }
}

