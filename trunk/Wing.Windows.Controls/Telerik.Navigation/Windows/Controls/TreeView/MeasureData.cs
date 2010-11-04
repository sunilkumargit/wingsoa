namespace Telerik.Windows.Controls.TreeView
{
    using System;
    using System.Windows;
    using Telerik.Windows.Controls;

    internal class MeasureData
    {
        private Size givenAvailableSize;
        private Rect viewport;

        public MeasureData(MeasureData data) : this(data.AvailableSize, data.Viewport)
        {
        }

        public MeasureData(Size availableSize, Rect viewport)
        {
            this.givenAvailableSize = availableSize;
            this.viewport = viewport;
        }

        public bool IsCloseTo(MeasureData other)
        {
            if (other == null)
            {
                return false;
            }
            return (DoubleUtil.AreClose(this.AvailableSize, other.AvailableSize) & DoubleUtil.AreClose(this.Viewport, other.Viewport));
        }

        public Size AvailableSize
        {
            get
            {
                return this.givenAvailableSize;
            }
            set
            {
                this.givenAvailableSize = value;
            }
        }

        public bool HasViewport
        {
            get
            {
                return (this.Viewport != Rect.Empty);
            }
        }

        public Rect Viewport
        {
            get
            {
                return this.viewport;
            }
            set
            {
                this.viewport = value;
            }
        }
    }
}

