namespace Telerik.Windows.Controls
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using Telerik.Windows;

    public class EditorPrepareEventArgs : RadRoutedEventArgs
    {
        public EditorPrepareEventArgs(Telerik.Windows.RoutedEvent routedEvent, object source, FrameworkElement editor) : base(routedEvent, source)
        {
            this.Editor = editor;
        }

        protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget)
        {
            (genericHandler as EditorPrepareEventHandler)(genericTarget, this);
        }

        public FrameworkElement Editor { get; private set; }
    }
}

