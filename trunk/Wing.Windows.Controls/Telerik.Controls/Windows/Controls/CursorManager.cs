namespace Telerik.Windows.Controls
{
    using System;
    using System.Windows;
    using System.Windows.Input;

    internal static class CursorManager
    {
        internal static void PerformTimeConsumingOperation(FrameworkElement frameworkElement, Action action)
        {
            Cursor cursor = frameworkElement.Cursor;
            frameworkElement.Cursor = Cursors.Wait;
            try
            {
                action();
            }
            finally
            {
                frameworkElement.Cursor = cursor;
            }
        }
    }
}

