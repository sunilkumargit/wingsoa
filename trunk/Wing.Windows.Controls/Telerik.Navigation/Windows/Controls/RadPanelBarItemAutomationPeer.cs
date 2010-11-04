namespace Telerik.Windows.Controls
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Windows.Automation;
    using System.Windows.Automation.Peers;
    using System.Windows.Automation.Provider;

    public class RadPanelBarItemAutomationPeer : RadTreeViewItemAutomationPeer, IExpandCollapseProvider, ISelectionItemProvider
    {
        public RadPanelBarItemAutomationPeer(RadPanelBarItem owner) : base(owner)
        {
        }

        public void AddToSelection()
        {
            this.GuarantyEnabled();
            this.OwnerAsPanelBarItem().IsSelected = true;
        }

        public void Collapse()
        {
            if (!base.IsEnabled())
            {
                throw new ElementNotEnabledException();
            }
            RadPanelBarItem owner = this.OwnerAsPanelBarItem();
            if (!owner.HasItems)
            {
                throw new InvalidOperationException("Leaf Node");
            }
            owner.IsExpanded = false;
        }

        public void Expand()
        {
            this.GuarantyEnabled();
            RadPanelBarItem owner = this.OwnerAsPanelBarItem();
            if (!owner.HasItems)
            {
                throw new InvalidOperationException("Leaf Node");
            }
            owner.IsExpanded = true;
        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.TreeItem;
        }

        protected override string GetClassNameCore()
        {
            return "RadPanelBarItem";
        }

        protected override string GetNameCore()
        {
            string nameCore = base.GetNameCore();
            if (string.IsNullOrEmpty(nameCore))
            {
                nameCore = "Rad Panel Bar Item";
            }
            return nameCore;
        }

        public override object GetPattern(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.ExpandCollapse)
            {
                return this;
            }
            if (patternInterface == PatternInterface.SelectionItem)
            {
                return this;
            }
            return null;
        }

        private void GuarantyEnabled()
        {
            if (!base.IsEnabled())
            {
                throw new ElementNotEnabledException();
            }
        }

        private RadPanelBarItem OwnerAsPanelBarItem()
        {
            RadPanelBarItem panelBarItem = base.Owner as RadPanelBarItem;
            if (panelBarItem == null)
            {
                throw new InvalidOperationException("The Owner have to be RadPanelBarItem");
            }
            return panelBarItem;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal void RaiseAutomationExpandCollapseChanged(bool oldValue, bool newValue)
        {
            base.RaisePropertyChangedEvent(ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty, oldValue ? System.Windows.Automation.ExpandCollapseState.Expanded : System.Windows.Automation.ExpandCollapseState.Collapsed, newValue ? System.Windows.Automation.ExpandCollapseState.Expanded : System.Windows.Automation.ExpandCollapseState.Collapsed);
        }

        public void RemoveFromSelection()
        {
            this.OwnerAsPanelBarItem().IsSelected = false;
        }

        public void Select()
        {
            this.OwnerAsPanelBarItem().IsSelected = true;
        }

        public System.Windows.Automation.ExpandCollapseState ExpandCollapseState
        {
            get
            {
                RadPanelBarItem owner = this.OwnerAsPanelBarItem();
                if (!owner.HasItems)
                {
                    return System.Windows.Automation.ExpandCollapseState.LeafNode;
                }
                if (!owner.IsExpanded)
                {
                    return System.Windows.Automation.ExpandCollapseState.Collapsed;
                }
                return System.Windows.Automation.ExpandCollapseState.Expanded;
            }
        }

        public bool IsSelected
        {
            get
            {
                return ((RadPanelBarItem) base.Owner).IsSelected;
            }
        }

        public IRawElementProviderSimple SelectionContainer
        {
            get
            {
                return null;
            }
        }
    }
}

