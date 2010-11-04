namespace Telerik.Windows.Controls
{
    using System;
    using System.Runtime.CompilerServices;

    public class FrameInfoEventArgs : EventArgs
    {
        public FrameInfoEventArgs(string frameType)
        {
            this.FrameType = frameType;
        }

        public IFrame Frame { get; set; }

        public string FrameType { get; set; }
    }
}

