namespace Telerik.Windows.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows.Automation;
    using System.Windows.Automation.Peers;
    using System.Windows.Automation.Provider;

    public class RadPanelBarAutomationPeer : RadTreeViewAutomationPeer, ISelectionProvider
    {
        public RadPanelBarAutomationPeer(RadPanelBar owner) : base(owner)
        {
        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Tree;
        }

        [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
        protected override List<AutomationPeer> GetChildrenCore()
        {
            RadPanelBar owner = this.OwnerAsRadPanelBar();
            int count = owner.Items.Count;
            if (count <= 0)
            {
                return null;
            }
            List<AutomationPeer> list = new List<AutomationPeer>(count);
            for (int i = 0; i < count; i++)
            {
                RadPanelBarItem element = owner.ItemContainerGenerator.ContainerFromIndex(i) as RadPanelBarItem;
                if (element != null)
                {
                    AutomationPeer peer = FrameworkElementAutomationPeer.FromElement(element) ?? FrameworkElementAutomationPeer.CreatePeerForElement(element);
                    if (peer != null)
                    {
                        list.Add(peer);
                    }
                }
            }
            return list;
        }

        protected override string GetClassNameCore()
        {
            return "RadPanelBar";
        }

        protected override string GetNameCore()
        {
            string nameCore = base.GetNameCore();
            if (string.IsNullOrEmpty(nameCore))
            {
                nameCore = "Rad Panel Bar";
            }
            return nameCore;
        }

        public override object GetPattern(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.Selection)
            {
                return this;
            }
            return null;
        }

        public IRawElementProviderSimple[] GetSelection()
        {
            RadPanelBarItem selected = this.OwnerAsRadPanelBar().SelectedItem as RadPanelBarItem;
            if (selected != null)
            {
                AutomationPeer peer = FrameworkElementAutomationPeer.FromElement(selected);
                if (peer != null)
                {
                    return new IRawElementProviderSimple[] { base.ProviderFromPeer(peer) };
                }
            }
            return new IRawElementProviderSimple[0];
        }

        private RadPanelBar OwnerAsRadPanelBar()
        {
            RadPanelBar owner = base.Owner as RadPanelBar;
            if (owner == null)
            {
                throw new InvalidOperationException("The Owner have to be RadPanelBar");
            }
            return owner;
        }

        internal void RaiseSelectionPropertyChanged(int oldIndex, int newIndex)
        {
            base.RaisePropertyChangedEvent(SelectionPatternIdentifiers.SelectionProperty, oldIndex, newIndex);
        }

        internal void RaiseSelectionPropertyChanged(RadPanelBarItem oldItem, RadPanelBarItem newItem)
        {
            RadPanelBar owner = this.OwnerAsRadPanelBar();
            int oldIndex = -1;
            int newIndex = -1;
            if (oldItem != null)
            {
                oldIndex = owner.ItemContainerGenerator.IndexFromContainer(oldItem);
            }
            if (newItem != null)
            {
                newIndex = owner.ItemContainerGenerator.IndexFromContainer(newItem);
            }
            this.RaiseSelectionPropertyChanged(oldIndex, newIndex);
        }

        public bool CanSelectMultiple
        {
            get
            {
                return false;
            }
        }

        public bool IsSelectionRequired
        {
            get
            {
                return false;
            }
        }
    }
}

