namespace Telerik.Windows.Controls
{
    using System;
    using System.Windows.Automation;
    using System.Windows.Automation.Peers;
    using System.Windows.Automation.Provider;
    using System.Windows;

    public class RadTreeViewItemAutomationPeer : FrameworkElementAutomationPeer, IExpandCollapseProvider, ISelectionItemProvider, IValueProvider, IScrollItemProvider
    {
        public RadTreeViewItemAutomationPeer(RadTreeViewItem owner) : base((FrameworkElement) owner)
        {
        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.TreeItem;
        }

        protected override string GetClassNameCore()
        {
            return "RadTreeViewItem";
        }

        protected override string GetHelpTextCore()
        {
            return base.GetHelpTextCore();
        }

        protected override string GetItemTypeCore()
        {
            return "TreeView Item";
        }

        protected override string GetNameCore()
        {
            return TextSearch.GetPrimaryText(this.Owner, "Header");
        }

        public override object GetPattern(PatternInterface patternInterface)
        {
            if (((patternInterface != PatternInterface.ExpandCollapse) && (patternInterface != PatternInterface.SelectionItem)) && ((patternInterface != PatternInterface.Value) && (patternInterface != PatternInterface.ScrollItem)))
            {
                return null;
            }
            return this;
        }

        protected override bool IsContentElementCore()
        {
            return base.IsContentElementCore();
        }

        internal void RaiseAutomationIsSelectedChanged(bool isSelected)
        {
            base.RaisePropertyChangedEvent(SelectionItemPatternIdentifiers.IsSelectedProperty, !isSelected, isSelected);
        }

        internal void RaiseExpandCollapseAutomationEvent(bool oldValue, bool newValue)
        {
            base.RaisePropertyChangedEvent(ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty, oldValue ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed, newValue ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed);
        }

        public void SetValue(string value)
        {
            if (this.Owner != null)
            {
                this.Owner.Header = value;
            }
        }

        void IExpandCollapseProvider.Collapse()
        {
            if (!base.IsEnabled())
            {
                throw new ElementNotEnabledException();
            }
            RadTreeViewItem owner = this.Owner;
            bool hasItems = owner.HasItems;
            owner.IsExpanded = false;
        }

        void IExpandCollapseProvider.Expand()
        {
            if (!base.IsEnabled())
            {
                throw new ElementNotEnabledException();
            }
            RadTreeViewItem owner = this.Owner;
            bool hasItems = owner.HasItems;
            owner.IsExpanded = true;
        }

        void IScrollItemProvider.ScrollIntoView()
        {
            this.Owner.BringIntoView();
        }

        void ISelectionItemProvider.AddToSelection()
        {
            this.Owner.IsSelected = true;
        }

        void ISelectionItemProvider.RemoveFromSelection()
        {
            this.Owner.IsSelected = false;
        }

        void ISelectionItemProvider.Select()
        {
            this.Owner.IsSelected = true;
        }

        void IValueProvider.SetValue(string value)
        {
            this.SetValue(value);
        }

        public bool IsReadOnly
        {
            get
            {
                if (this.Owner == null)
                {
                    return false;
                }
                return (!this.Owner.IsEditable || ((this.Owner.ParentTreeView != null) && !this.Owner.ParentTreeView.IsEditable));
            }
        }

        protected RadTreeViewItem Owner
        {
            get
            {
                return (base.Owner as RadTreeViewItem);
            }
        }

        ExpandCollapseState IExpandCollapseProvider.ExpandCollapseState
        {
            get
            {
                RadTreeViewItem owner = this.Owner;
                if (!owner.HasItems)
                {
                    return ExpandCollapseState.LeafNode;
                }
                if (!owner.IsExpanded)
                {
                    return ExpandCollapseState.Collapsed;
                }
                return ExpandCollapseState.Expanded;
            }
        }

        bool ISelectionItemProvider.IsSelected
        {
            get
            {
                return this.Owner.IsSelected;
            }
        }

        IRawElementProviderSimple ISelectionItemProvider.SelectionContainer
        {
            get
            {
                ItemsControl parentItemsControl = this.Owner.Owner;
                if (parentItemsControl != null)
                {
                    AutomationPeer peer = FrameworkElementAutomationPeer.FromElement(parentItemsControl);
                    if (peer != null)
                    {
                        return base.ProviderFromPeer(peer);
                    }
                }
                return null;
            }
        }

        bool IValueProvider.IsReadOnly
        {
            get
            {
                return this.IsReadOnly;
            }
        }

        string IValueProvider.Value
        {
            get
            {
                return this.Value;
            }
        }

        public string Value
        {
            get
            {
                if ((this.Owner != null) && (this.Owner.Header != null))
                {
                    return this.Owner.Header.ToString();
                }
                return string.Empty;
            }
        }
    }
}

