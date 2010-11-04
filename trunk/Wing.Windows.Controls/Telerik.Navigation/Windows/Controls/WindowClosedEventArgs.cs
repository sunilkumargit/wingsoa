namespace Telerik.Windows.Controls
{
    using System;
    using System.Runtime.CompilerServices;

    public class WindowClosedEventArgs : EventArgs
    {
        public bool? DialogResult { get; set; }

        public string PromptResult { get; set; }
    }
}

