namespace Telerik.Windows.Controls.Automation.Peers
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Windows.Automation;
    using System.Windows.Automation.Peers;
    using System.Windows.Automation.Provider;
    using Telerik.Windows.Controls;

    public abstract class SelectorItemAutomationPeer : ItemAutomationPeer, ISelectionItemProvider
    {
        protected SelectorItemAutomationPeer(object owner, Telerik.Windows.Controls.Automation.Peers.SelectorAutomationPeer selectorAutomationPeer) : base(owner, selectorAutomationPeer)
        {
        }

        public override object GetPattern(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.SelectionItem)
            {
                return this;
            }
            return null;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal void RaiseAutomationIsSelectedChanged(bool isSelected)
        {
            base.RaisePropertyChangedEvent(SelectionItemPatternIdentifiers.IsSelectedProperty, !isSelected, isSelected);
        }

        void ISelectionItemProvider.AddToSelection()
        {
            if (!base.IsEnabled())
            {
                throw new ElementNotEnabledException();
            }
            Telerik.Windows.Controls.Selector owner = (Telerik.Windows.Controls.Selector) base.ItemsControlAutomationPeer.Owner;
            if ((owner == null) || ((!owner.CanSelectMultiple && (owner.SelectedItem != null)) && (owner.SelectedItem != base.Item)))
            {
                throw new InvalidOperationException();
            }
            owner.SelectionChange.Begin();
            owner.SelectionChange.Add(base.Item);
            owner.SelectionChange.End();
        }

        void ISelectionItemProvider.RemoveFromSelection()
        {
            if (!base.IsEnabled())
            {
                throw new ElementNotEnabledException();
            }
            Telerik.Windows.Controls.Selector owner = (Telerik.Windows.Controls.Selector) base.ItemsControlAutomationPeer.Owner;
            owner.SelectionChange.Begin();
            owner.SelectionChange.Remove(base.Item);
            owner.SelectionChange.End();
        }

        void ISelectionItemProvider.Select()
        {
            if (!base.IsEnabled())
            {
                throw new ElementNotEnabledException();
            }
            Telerik.Windows.Controls.Selector owner = (Telerik.Windows.Controls.Selector) base.ItemsControlAutomationPeer.Owner;
            if (owner == null)
            {
                throw new InvalidOperationException();
            }
            owner.SelectedItem = base.Item;
        }

        private Telerik.Windows.Controls.Selector Selector
        {
            get
            {
                return (Telerik.Windows.Controls.Selector) base.ItemsControlAutomationPeer.Owner;
            }
        }

        bool ISelectionItemProvider.IsSelected
        {
            get
            {
                return this.Selector.SelectionChange.Contains(base.Item);
            }
        }

        IRawElementProviderSimple ISelectionItemProvider.SelectionContainer
        {
            get
            {
                return base.ProviderFromPeer(base.ItemsControlAutomationPeer);
            }
        }
    }
}

