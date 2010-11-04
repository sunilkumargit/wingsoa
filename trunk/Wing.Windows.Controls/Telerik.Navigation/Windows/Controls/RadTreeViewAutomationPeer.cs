namespace Telerik.Windows.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows.Automation.Peers;
    using System.Windows.Automation.Provider;
    using System.Windows.Controls;

    public class RadTreeViewAutomationPeer : ItemsControlAutomationPeer, ISelectionProvider
    {
        public RadTreeViewAutomationPeer(RadTreeView owner) : base(owner)
        {
        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Tree;
        }

        [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
        protected override List<AutomationPeer> GetChildrenCore()
        {
            Telerik.Windows.Controls.ItemsControl owner = (Telerik.Windows.Controls.ItemsControl) base.Owner;
            ItemCollection items = owner.Items;
            if (items.Count <= 0)
            {
                return null;
            }
            List<AutomationPeer> list = new List<AutomationPeer>(items.Count);
            for (int i = 0; i < items.Count; i++)
            {
                RadTreeViewItem element = owner.ItemContainerGenerator.ContainerFromIndex(i) as RadTreeViewItem;
                if (element != null)
                {
                    AutomationPeer item = FrameworkElementAutomationPeer.FromElement(element);
                    if (item == null)
                    {
                        item = FrameworkElementAutomationPeer.CreatePeerForElement(element);
                    }
                    list.Add(item);
                }
            }
            return list;
        }

        protected override string GetClassNameCore()
        {
            return "RadTreeView";
        }

        public override object GetPattern(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.Selection)
            {
                return this;
            }
            return null;
        }

        IRawElementProviderSimple[] ISelectionProvider.GetSelection()
        {
            IRawElementProviderSimple[] simpleArray = null;
            RadTreeViewItem selectedContainer = ((RadTreeView) base.Owner).SelectedContainer;
            if (selectedContainer != null)
            {
                AutomationPeer peer = FrameworkElementAutomationPeer.FromElement(selectedContainer);
                if (peer != null)
                {
                    simpleArray = new IRawElementProviderSimple[] { base.ProviderFromPeer(peer) };
                }
            }
            if (simpleArray == null)
            {
                simpleArray = new IRawElementProviderSimple[0];
            }
            return simpleArray;
        }

        bool ISelectionProvider.CanSelectMultiple
        {
            get
            {
                RadTreeView tree = base.Owner as RadTreeView;
                return ((tree != null) && (tree.SelectionMode != Telerik.Windows.Controls.SelectionMode.Single));
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

