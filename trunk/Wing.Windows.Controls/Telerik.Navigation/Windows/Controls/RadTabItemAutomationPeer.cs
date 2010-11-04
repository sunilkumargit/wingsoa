namespace Telerik.Windows.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows.Automation;
    using System.Windows.Automation.Peers;
    using System.Windows.Automation.Provider;
    using System.Windows.Controls;

    public class RadTabItemAutomationPeer : ItemAutomationPeer, ISelectionItemProvider
    {
        public RadTabItemAutomationPeer(RadTabItem owner) : base(owner)
        {
        }

        public void AddToSelection()
        {
            this.EnsureEnabled();
            this.EnsureTabControl().SelectedItem = base.Item;
        }

        private void EnsureEnabled()
        {
            if (!base.IsEnabled())
            {
                throw new ElementNotEnabledException();
            }
        }

        private RadTabControl EnsureTabControl()
        {
            ItemsControlAutomationPeer tabControlPeer = base.ItemsControlAutomationPeer;
            if (tabControlPeer == null)
            {
                throw new InvalidOperationException("The tab item is not associated with TabControl's automation peer");
            }
            RadTabControl owner = tabControlPeer.Owner as RadTabControl;
            if (owner == null)
            {
                throw new InvalidOperationException("The tab item's Owner have to be RadTabControl");
            }
            return owner;
        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.TabItem;
        }

        [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification="This is an override of a built-in class, the return type cannot be changed.")]
        protected override List<AutomationPeer> GetChildrenCore()
        {
            List<AutomationPeer> childrenCore = base.GetChildrenCore();
            RadTabItem item = this.OwnerAsRadTabItem();
            if (item != null)
            {
                RadTabControl tabControl = Telerik.Windows.Controls.ItemsControl.ItemsControlFromItemContainer(item) as RadTabControl;
                if (((item == null) || !item.IsSelected) || (tabControl == null))
                {
                    return childrenCore;
                }
                ContentPresenter contentHost = tabControl.ContentElement;
                if (contentHost == null)
                {
                    return childrenCore;
                }
                List<AutomationPeer> children = new FrameworkElementAutomationPeer(contentHost).GetChildren();
                if (children == null)
                {
                    return childrenCore;
                }
                if (childrenCore == null)
                {
                    return children;
                }
                childrenCore.AddRange(children);
            }
            return childrenCore;
        }

        protected override string GetClassNameCore()
        {
            return "RadTabItem";
        }

        protected override string GetNameCore()
        {
            return TextSearch.GetPrimaryText(base.Owner, string.Empty);
        }

        public override object GetPattern(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.SelectionItem)
            {
                return this;
            }
            return base.GetPattern(patternInterface);
        }

        private RadTabItem OwnerAsRadTabItem()
        {
            RadTabItem tabItem = base.Owner as RadTabItem;
            if (tabItem == null)
            {
                throw new InvalidOperationException("The Owner has to be a RadTabItem");
            }
            return tabItem;
        }

        internal void RaiseAutomationIsSelectedChanged(bool isSelected)
        {
            base.RaisePropertyChangedEvent(SelectionItemPatternIdentifiers.IsSelectedProperty, !isSelected, isSelected);
        }

        public void RemoveFromSelection()
        {
            this.EnsureEnabled();
            if (this.OwnerAsRadTabItem().IsSelected)
            {
                throw new InvalidOperationException("RemoveFromSelection - can not remove the selected tab item");
            }
        }

        public void Select()
        {
            this.EnsureEnabled();
            this.EnsureTabControl().SelectedItem = base.Item;
        }

        public bool IsSelected
        {
            get
            {
                ItemsControlAutomationPeer tabControlPeer = base.ItemsControlAutomationPeer;
                if (tabControlPeer != null)
                {
                    RadTabControl owner = tabControlPeer.Owner as RadTabControl;
                    if (owner != null)
                    {
                        return (owner.SelectedItem == base.Item);
                    }
                }
                return false;
            }
        }

        public IRawElementProviderSimple SelectionContainer
        {
            get
            {
                return base.ProviderFromPeer(base.ItemsControlAutomationPeer);
            }
        }
    }
}

