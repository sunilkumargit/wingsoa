namespace Telerik.Windows.Controls
{
    using System;
    using System.Runtime.CompilerServices;
    using Wing.Windows.Controls.Telerik.Navigation.Windows.Controls;

    [ScriptableType]
    public class RadTreeViewItemPreparedEventArgs : EventArgs
    {
        public RadTreeViewItemPreparedEventArgs(RadTreeViewItem preparedItem)
        {
            this.PreparedItem = preparedItem;
        }

        [ScriptableMember]
        public RadTreeViewItem PreparedItem { get; internal set; }
    }
}

