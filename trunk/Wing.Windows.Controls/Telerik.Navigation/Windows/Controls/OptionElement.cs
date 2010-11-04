namespace Telerik.Windows.Controls
{
    using System;
    using System.Windows;
    using System.Windows.Automation;
    using System.Windows.Controls.Primitives;

    internal abstract class OptionElement
    {
        internal OptionElement()
        {
        }

        public virtual void Hide(ToggleButton toggleElement)
        {
            if (toggleElement != null)
            {
                toggleElement.Visibility = Visibility.Collapsed;
            }
        }

        public abstract void PropagateItemState(RadTreeViewItem item);
        public abstract void Render(ToggleButton toggleElement, ToggleState state);
        protected virtual void SetCheckState(ToggleState state, ToggleButton toggleElement)
        {
            this.Render(toggleElement, state);
        }

        public virtual void Show(ToggleButton toggleElement)
        {
            if (toggleElement != null)
            {
                toggleElement.Visibility = Visibility.Visible;
            }
        }
    }
}

