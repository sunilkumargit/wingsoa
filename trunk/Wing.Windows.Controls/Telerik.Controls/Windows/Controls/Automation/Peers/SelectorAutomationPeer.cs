namespace Telerik.Windows.Controls.Automation.Peers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows.Automation.Peers;
    using System.Windows.Automation.Provider;
    using Telerik.Windows.Controls;

    public abstract class SelectorAutomationPeer : ItemsControlAutomationPeer, ISelectionProvider
    {
        protected SelectorAutomationPeer(Selector owner) : base(owner)
        {
        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.List;
        }

        [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
        protected override List<AutomationPeer> GetChildrenCore()
        {
            return base.GetChildrenCore();
        }

        public override object GetPattern(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.Selection)
            {
                return this;
            }
            return base.GetPattern(patternInterface);
        }

        IRawElementProviderSimple[] ISelectionProvider.GetSelection()
        {
            Selector selector = (Selector) base.Owner;
            int selectedItemsCount = selector.SelectionChange.Count;
            List<AutomationPeer> itemPeers = base.GetChildren();
            if ((selectedItemsCount <= 0) || (itemPeers.Count <= 0))
            {
                return null;
            }
            List<IRawElementProviderSimple> list = new List<IRawElementProviderSimple>(selectedItemsCount);
            for (int i = 0; i < selectedItemsCount; i++)
            {
                Telerik.Windows.Controls.Automation.Peers.SelectorItemAutomationPeer peer = (Telerik.Windows.Controls.Automation.Peers.SelectorItemAutomationPeer) itemPeers[selector.Items.IndexOf(selector.SelectionChange[i])];
                if (peer != null)
                {
                    list.Add(base.ProviderFromPeer(peer));
                }
            }
            return list.ToArray();
        }

        bool ISelectionProvider.CanSelectMultiple
        {
            get
            {
                Selector owner = (Selector) base.Owner;
                return owner.CanSelectMultiple;
            }
        }

        bool ISelectionProvider.IsSelectionRequired
        {
            get
            {
                return false;
            }
        }
    }
}

