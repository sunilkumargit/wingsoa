namespace Telerik.Windows.Controls
{
    using System;
    using System.Windows;

    public class RadDragCompletedEventArgs : RoutedEventArgs
    {
        private bool canceled;
        private double horizontalChange;
        private double selectionEnd;
        private double selectionStart;
        private double value;
        private double verticalChange;

        public RadDragCompletedEventArgs()
        {
        }

        public RadDragCompletedEventArgs(double horizontalChange, double verticalChange, double value, double selectionStart, double selectionEnd, bool canceled)
        {
            this.horizontalChange = horizontalChange;
            this.verticalChange = verticalChange;
            this.value = value;
            this.selectionStart = selectionStart;
            this.selectionEnd = selectionEnd;
            this.canceled = canceled;
        }

        public bool Canceled
        {
            get
            {
                return this.canceled;
            }
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

