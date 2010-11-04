namespace Telerik.Windows.Controls
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Windows;

    public class LayoutSerializationEventArgs : EventArgs
    {
        public LayoutSerializationEventArgs(DependencyObject affectedElement, string affectedElementSerializationTag)
        {
            this.AffectedElement = affectedElement;
            this.AffectedElementSerializationTag = affectedElementSerializationTag;
        }

        public DependencyObject AffectedElement { get; protected set; }

        public string AffectedElementSerializationTag { get; private set; }
    }
}

