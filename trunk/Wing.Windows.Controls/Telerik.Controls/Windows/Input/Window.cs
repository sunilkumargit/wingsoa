namespace Telerik.Windows.Input
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using Wing.Windows.Controls.Telerik.Navigation.Windows.Controls;
    using Telerik.Windows;
    using System.Windows.Browser;

    internal static class Window
    {
        public static readonly Telerik.Windows.RoutedEvent WindowUnloadEvent = EventManager.RegisterRoutedEvent("WindowUnload", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(FrameworkElement));

        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification="We need to initialize the HTML events in the static constructor.")]
        static Window()
        {
            if (HtmlPage.IsEnabled)
            {
                HtmlPage.Window.AttachEvent("onunload", new EventHandler(Telerik.Windows.Input.Window.OnWindowUnload));
            }
        }

        private static void OnWindowUnload(object sender, EventArgs args)
        {
            if (Application.Current.RootVisual != null)
            {
                Application.Current.RootVisual.RaiseEvent(new RadRoutedEventArgs(WindowUnloadEvent));
            }
        }
    }
}

