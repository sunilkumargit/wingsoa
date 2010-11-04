namespace Telerik.Windows.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Automation;
    using System.Windows.Automation.Peers;
    using System.Windows.Automation.Provider;

    public class RadTabControlAutomationPeer : ItemsControlAutomationPeer, ISelectionProvider
    {
        public RadTabControlAutomationPeer(RadTabControl owner) : base(owner)
        {
        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Tab;
        }

        protected override string GetClassNameCore()
        {
            return "RadTabControl";
        }

        protected override Point GetClickablePointCore()
        {
            return new Point(double.NaN, double.NaN);
        }

        internal IRawElementProviderSimple GetItemProvider(object item)
        {
            IRawElementProviderSimple provider = null;
            RadTabItem tabItem = item as RadTabItem;
            if (tabItem != null)
            {
                AutomationPeer peer = FrameworkElementAutomationPeer.FromElement(tabItem);
                if (peer != null)
                {
                    provider = base.ProviderFromPeer(peer);
                }
            }
            return provider;
        }

        protected override string GetNameCore()
        {
            string nameCore = base.GetNameCore();
            if (string.IsNullOrEmpty(nameCore))
            {
                nameCore = "Rad Tab Control";
            }
            return nameCore;
        }

        public override object GetPattern(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.Selection)
            {
                return this;
            }
            return base.GetPattern(patternInterface);
        }

        internal RadTabItem GetSelectedTabItem()
        {
            RadTabControl owner = this.OwnerAsRadTabControl();
            object item = owner.SelectedItem;
            RadTabItem tabItem = owner.ItemContainerGenerator.ContainerFromItem(item) as RadTabItem;
            if ((tabItem != null) && tabItem.IsSelected)
            {
                return tabItem;
            }
            return null;
        }

        public IRawElementProviderSimple[] GetSelection()
        {
            RadTabItem tabItem = this.GetSelectedTabItem();
            if (tabItem != null)
            {
                IRawElementProviderSimple provider = this.GetItemProvider(tabItem);
                if (provider != null)
                {
                    return new List<IRawElementProviderSimple>(1) { provider }.ToArray();
                }
            }
            return new IRawElementProviderSimple[0];
        }

        private RadTabControl OwnerAsRadTabControl()
        {
            RadTabControl tabControl = base.Owner as RadTabControl;
            if (tabControl == null)
            {
                throw new InvalidOperationException("The Owner have to be RadTabControl");
            }
            return tabControl;
        }

        internal void RaiseSelectionPropertyChanged(int oldValue, int newValue)
        {
            base.RaisePropertyChangedEvent(SelectionPatternIdentifiers.SelectionProperty, oldValue, newValue);
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
                return true;
            }
        }
    }
}

