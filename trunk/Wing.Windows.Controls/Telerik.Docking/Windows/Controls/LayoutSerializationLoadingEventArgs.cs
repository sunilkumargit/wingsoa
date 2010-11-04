namespace Telerik.Windows.Controls
{
    using System;
    using System.Windows;

    public class LayoutSerializationLoadingEventArgs : LayoutSerializationEventArgs
    {
        public LayoutSerializationLoadingEventArgs(string affectedElementSerializationTag) : base(null, affectedElementSerializationTag)
        {
        }

        public void SetAffectedElement(DependencyObject newElement)
        {
            base.AffectedElement = newElement;
        }
    }
}

