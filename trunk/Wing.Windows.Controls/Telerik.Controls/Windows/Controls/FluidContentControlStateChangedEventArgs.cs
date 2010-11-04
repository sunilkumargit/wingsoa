namespace Telerik.Windows.Controls
{
    using System;
    using System.Runtime.CompilerServices;

    public class FluidContentControlStateChangedEventArgs : EventArgs
    {
        public FluidContentControlStateChangedEventArgs(FluidContentControlState oldState, FluidContentControlState newState)
        {
            this.NewState = newState;
            this.OldState = oldState;
        }

        public FluidContentControlState NewState { get; set; }

        public FluidContentControlState OldState { get; set; }
    }
}

