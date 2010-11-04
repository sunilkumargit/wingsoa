namespace Telerik.Windows.Controls
{
    using System;
    using System.Windows;

    public class RadDragDeltaEventArgs : RoutedEventArgs
    {
        private double horizontalChange;
        private double selectionEnd;
        private double selectionStart;
        private double value;
        private double verticalChange;

        public RadDragDeltaEventArgs()
        {
        }

        public RadDragDeltaEventArgs(double horizontalChange, double verticalChange, double value, double selectionStart, double selectionEnd)
        {
            this.horizontalChange = horizontalChange;
            this.verticalChange = verticalChange;
            this.value = value;
            this.selectionStart = selectionStart;
            this.selectionEnd = selectionEnd;
        }

        public double HorizontalChange
        {
            get
            {
                return this.horizontalChange;
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

        public double VerticalChange
        {
            get
            {
                return this.verticalChange;
            }
        }
    }
}

