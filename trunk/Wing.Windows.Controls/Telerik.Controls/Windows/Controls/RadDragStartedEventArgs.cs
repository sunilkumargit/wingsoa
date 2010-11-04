namespace Telerik.Windows.Controls
{
    using System;
    using System.Windows;

    public class RadDragStartedEventArgs : RoutedEventArgs
    {
        private double horizontalOffset;
        private double selectionEnd;
        private double selectionStart;
        private double value;
        private double verticalOffset;

        public RadDragStartedEventArgs()
        {
        }

        public RadDragStartedEventArgs(double horizontalOffset, double verticalOffset, double value, double selectionStart, double selectionEnd)
        {
            this.horizontalOffset = horizontalOffset;
            this.verticalOffset = verticalOffset;
            this.value = value;
            this.selectionStart = selectionStart;
            this.selectionEnd = selectionEnd;
        }

        public double HorizontalOffset
        {
            get
            {
                return this.horizontalOffset;
            }
        }

        public double SelectionEnd
        {
            get
            {
                return this.selectionEnd;
            }
        }

        public double SelectionStart
        {
            get
            {
                return this.selectionStart;
            }
        }

        public double Value
        {
            get
            {
                return this.value;
            }
        }

        public double VerticalOffset
        {
            get
            {
                return this.verticalOffset;
            }
        }
    }
}

