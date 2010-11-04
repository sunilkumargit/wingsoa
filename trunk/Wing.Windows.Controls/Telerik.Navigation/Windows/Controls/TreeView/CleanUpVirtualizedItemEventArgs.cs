namespace Telerik.Windows.Controls.TreeView
{
    using System;
    using System.Windows;
    using Telerik.Windows;

    public class CleanUpVirtualizedItemEventArgs : RadRoutedEventArgs
    {
        private bool cancel;
        private object elementValue;
        private System.Windows.UIElement targetElement;

        internal CleanUpVirtualizedItemEventArgs(object value, System.Windows.UIElement element) : base(TreeViewPanel.CleanUpVirtualizedItemEvent)
        {
            this.elementValue = value;
            this.targetElement = element;
        }

        public bool Cancel
        {
            get
            {
                return this.cancel;
            }
            set
            {
                this.cancel = value;
            }
        }

        public System.Windows.UIElement UIElement
        {
            get
            {
                return this.targetElement;
            }
        }

        public object Value
        {
            get
            {
                return this.elementValue;
            }
        }
    }
}

