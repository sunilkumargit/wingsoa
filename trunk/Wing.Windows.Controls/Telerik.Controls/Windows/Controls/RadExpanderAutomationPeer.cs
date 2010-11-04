namespace Telerik.Windows.Controls
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Windows.Automation;
    using System.Windows.Automation.Peers;
    using System.Windows.Automation.Provider;
    using System.Windows;

    public class RadExpanderAutomationPeer : FrameworkElementAutomationPeer, IExpandCollapseProvider
    {
        public RadExpanderAutomationPeer(RadExpander element) : base((FrameworkElement) element)
        {
        }

        public void Collapse()
        {
            this.GuarantyEnabled();
            this.OwnerAsExpander().IsExpanded = false;
        }

        public void Expand()
        {
            this.GuarantyEnabled();
            this.OwnerAsExpander().IsExpanded = true;
        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Group;
        }

        protected override string GetClassNameCore()
        {
            return "RadExpander";
        }

        protected override string GetNameCore()
        {
            string name = base.GetNameCore();
            if (string.IsNullOrEmpty(name))
            {
                RadExpander expander = this.OwnerAsExpander();
                if (expander.HeaderButton != null)
                {
                    name = expander.HeaderButton.GetPlainText();
                }
            }
            return name;
        }

        public override object GetPattern(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.ExpandCollapse)
            {
                return this;
            }
            return base.GetPattern(patternInterface);
        }

        private void GuarantyEnabled()
        {
            if (!base.IsEnabled())
            {
                throw new ElementNotEnabledException();
            }
        }

        protected override bool IsEnabledCore()
        {
            return this.OwnerAsExpander().IsEnabled;
        }

        private RadExpander OwnerAsExpander()
        {
            RadExpander expander = base.Owner as RadExpander;
            if (expander == null)
            {
                throw new InvalidOperationException("The Owner have to be a RadExpander");
            }
            return expander;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal void RaiseExpandCollapseAutomationEvent(bool oldValue, bool newValue)
        {
            base.RaisePropertyChangedEvent(ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty, oldValue ? System.Windows.Automation.ExpandCollapseState.Expanded : System.Windows.Automation.ExpandCollapseState.Collapsed, newValue ? System.Windows.Automation.ExpandCollapseState.Expanded : System.Windows.Automation.ExpandCollapseState.Collapsed);
        }

        public System.Windows.Automation.ExpandCollapseState ExpandCollapseState
        {
            get
            {
                if (!this.OwnerAsExpander().IsExpanded)
                {
                    return System.Windows.Automation.ExpandCollapseState.Collapsed;
                }
                return System.Windows.Automation.ExpandCollapseState.Expanded;
            }
        }
    }
}

