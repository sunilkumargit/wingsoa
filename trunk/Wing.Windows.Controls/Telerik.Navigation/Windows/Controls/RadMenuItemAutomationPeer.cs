namespace Telerik.Windows.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Automation;
    using System.Windows.Automation.Peers;
    using System.Windows.Automation.Provider;
    using System.Windows.Controls;

    public class RadMenuItemAutomationPeer : FrameworkElementAutomationPeer, IExpandCollapseProvider, IToggleProvider, IInvokeProvider
    {
        public RadMenuItemAutomationPeer(RadMenuItem owner) : base((FrameworkElement) owner)
        {
        }

        public void Collapse()
        {
            this.GuarantyEnabled();
            RadMenuItem owner = this.OwnerAsRadMenuItem();
            if (!owner.HasItems || ((owner.Role != MenuItemRole.SubmenuHeader) && (owner.Role != MenuItemRole.TopLevelHeader)))
            {
                throw new InvalidOperationException();
            }
            owner.CloseMenu();
        }

        public void Expand()
        {
            this.GuarantyEnabled();
            RadMenuItem owner = this.OwnerAsRadMenuItem();
            if (!owner.HasItems || ((owner.Role != MenuItemRole.SubmenuHeader) && (owner.Role != MenuItemRole.TopLevelHeader)))
            {
                throw new InvalidOperationException();
            }
            owner.OpenMenu();
        }

        protected override string GetAccessKeyCore()
        {
            return string.Empty;
        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.MenuItem;
        }

        [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
        protected override List<AutomationPeer> GetChildrenCore()
        {
            RadMenuItem owner = this.OwnerAsRadMenuItem();
            ItemCollection items = owner.Items;
            if (items.Count <= 0)
            {
                return null;
            }
            List<AutomationPeer> list = new List<AutomationPeer>(items.Count);
            for (int i = 0; i < items.Count; i++)
            {
                UIElement element = owner.ItemContainerGenerator.ContainerFromIndex(i) as UIElement;
                if (element != null)
                {
                    AutomationPeer peer = FrameworkElementAutomationPeer.FromElement(element);
                    if (peer == null)
                    {
                        peer = FrameworkElementAutomationPeer.CreatePeerForElement(element);
                    }
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
            return "RadMenuItem";
        }

        protected override string GetNameCore()
        {
            string nameCore = base.GetNameCore();
            RadMenuItem owner = this.OwnerAsRadMenuItem();
            if (string.IsNullOrEmpty(nameCore) && (owner.Header is string))
            {
                nameCore = (string) owner.Header;
            }
            return nameCore;
        }

        public override object GetPattern(PatternInterface patternInterface)
        {
            RadMenuItem owner = this.OwnerAsRadMenuItem();
            if (((patternInterface == PatternInterface.Invoke) && !owner.HasItems) && ((owner.Role == MenuItemRole.TopLevelItem) || (owner.Role == MenuItemRole.SubmenuItem)))
            {
                return this;
            }
            if (((patternInterface == PatternInterface.ExpandCollapse) && owner.HasItems) && ((owner.Role == MenuItemRole.TopLevelHeader) || (owner.Role == MenuItemRole.SubmenuHeader)))
            {
                return this;
            }
            if ((patternInterface == PatternInterface.Toggle) && owner.IsCheckable)
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

        public void Invoke()
        {
            this.GuarantyEnabled();
            RadMenuItem owner = this.OwnerAsRadMenuItem();
            switch (owner.Role)
            {
                case MenuItemRole.TopLevelItem:
                case MenuItemRole.SubmenuItem:
                    owner.HandleMouseUp();
                    return;

                case MenuItemRole.TopLevelHeader:
                case MenuItemRole.SubmenuHeader:
                    owner.HandleMouseDown();
                    return;
            }
        }

        protected override bool IsControlElementCore()
        {
            return true;
        }

        private RadMenuItem OwnerAsRadMenuItem()
        {
            RadMenuItem menuItem = base.Owner as RadMenuItem;
            if (menuItem == null)
            {
                throw new InvalidOperationException("The Owner have to be RadMenuItem");
            }
            return menuItem;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal void RaiseExpandCollapseAutomationEvent(bool oldValue, bool newValue)
        {
            base.RaiseAutomationEvent(AutomationEvents.PropertyChanged);
            base.RaisePropertyChangedEvent(ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty, oldValue ? System.Windows.Automation.ExpandCollapseState.Expanded : System.Windows.Automation.ExpandCollapseState.Collapsed, newValue ? System.Windows.Automation.ExpandCollapseState.Expanded : System.Windows.Automation.ExpandCollapseState.Collapsed);
        }

        public void Toggle()
        {
            this.GuarantyEnabled();
            RadMenuItem owner = this.OwnerAsRadMenuItem();
            if (!owner.IsCheckable)
            {
                throw new InvalidOperationException();
            }
            owner.IsChecked = !owner.IsChecked;
            owner.ChangeVisualState(false);
        }

        public System.Windows.Automation.ExpandCollapseState ExpandCollapseState
        {
            get
            {
                RadMenuItem owner = this.OwnerAsRadMenuItem();
                if ((!owner.HasItems || (owner.Role == MenuItemRole.TopLevelItem)) || (owner.Role == MenuItemRole.SubmenuItem))
                {
                    return System.Windows.Automation.ExpandCollapseState.LeafNode;
                }
                if (!owner.IsSubmenuOpen)
                {
                    return System.Windows.Automation.ExpandCollapseState.Collapsed;
                }
                return System.Windows.Automation.ExpandCollapseState.Expanded;
            }
        }

        public System.Windows.Automation.ToggleState ToggleState
        {
            get
            {
                if (!this.OwnerAsRadMenuItem().IsChecked)
                {
                    return System.Windows.Automation.ToggleState.Off;
                }
                return System.Windows.Automation.ToggleState.On;
            }
        }
    }
}

