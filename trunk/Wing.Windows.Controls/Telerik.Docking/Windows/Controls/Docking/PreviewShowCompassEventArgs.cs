namespace Telerik.Windows.Controls.Docking
{
    using System;
    using System.Runtime.CompilerServices;
    using Telerik.Windows.Controls;

    public class PreviewShowCompassEventArgs : EventArgs
    {
        public PreviewShowCompassEventArgs(RadPaneGroup targetGroup, RadSplitContainer draggedSplitContainer, Telerik.Windows.Controls.Docking.Compass compass)
        {
            this.TargetGroup = targetGroup;
            this.DraggedSplitContainer = draggedSplitContainer;
            this.Compass = compass;
        }

        public bool Canceled { get; set; }

        public Telerik.Windows.Controls.Docking.Compass Compass { get; private set; }

        public RadSplitContainer DraggedSplitContainer { get; private set; }

        public RadPaneGroup TargetGroup { get; private set; }
    }
}

