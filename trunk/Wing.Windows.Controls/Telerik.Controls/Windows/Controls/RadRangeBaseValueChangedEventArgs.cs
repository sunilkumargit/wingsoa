namespace Telerik.Windows.Controls
{
    using System;
    using System.Runtime.CompilerServices;

    public class RadRangeBaseValueChangedEventArgs : EventArgs
    {
        public RadRangeBaseValueChangedEventArgs(double? oldValue, double? newValue)
        {
            this.OldValue = oldValue;
            this.NewValue = newValue;
        }

        public double? NewValue { get; set; }

        public double? OldValue { get; set; }
    }
}

