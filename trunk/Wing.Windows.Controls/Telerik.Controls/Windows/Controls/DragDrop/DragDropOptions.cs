namespace Telerik.Windows.Controls.DragDrop
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Controls;
    using Telerik.Windows.Controls;

    public class DragDropOptions
    {
        public ContentControl ArrowCue { get; set; }

        public Point CurrentDragPoint { get; internal set; }

        public FrameworkElement Destination { get; set; }

        public ContentControl DestinationCue { get; set; }

        public FrameworkElement DestinationCueHost { get; set; }

        internal DeferredActionBase DestinationCueHostRollback { get; set; }

        internal DeferredActionBase DestinationCueRollback { get; set; }

        public object DragCue { get; set; }

        internal ContentControl DragCueHost { get; set; }

        internal DeferredActionBase DragCueRollback { get; set; }

        public Point MouseClickPoint { get; internal set; }

        public IList<UIElement> ParticipatingVisualRoots { get; internal set; }

        public object Payload { get; set; }

        public Point RelativeDragPoint { get; internal set; }

        public FrameworkElement Source { get; internal set; }

        public ContentControl SourceCue { get; set; }

        public FrameworkElement SourceCueHost { get; set; }

        internal DeferredActionBase SourceCueHostRollback { get; set; }

        internal DeferredActionBase SourceCueRollback { get; set; }

        public DragStatus Status { get; internal set; }
    }
}

