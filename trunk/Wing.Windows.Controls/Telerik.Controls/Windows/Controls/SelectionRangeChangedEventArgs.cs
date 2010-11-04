namespace Telerik.Windows.Controls
{
    using System;

    public class SelectionRangeChangedEventArgs : EventArgs
    {
        private double selectionEnd;
        private double selectionStart;

        public SelectionRangeChangedEventArgs()
        {
        }

        public SelectionRangeChangedEventArgs(double selectionStart, double selectionEnd)
        {
            this.selectionStart = selectionStart;
            this.selectionEnd = selectionEnd;
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
    }
}

