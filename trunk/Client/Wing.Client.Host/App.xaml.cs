using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Wing.Client.Core;
using Wing.Client.Host;

namespace Wing.Client
{
    public partial class App : Application
    {
        private static Dispatcher _dispatcher;

        public App()
        {
            this.Startup += this.Application_Startup;
            this.Exit += this.Application_Exit;
            this.UnhandledException += this.Application_UnhandledException;
            InitializeComponent();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            //application starter
            var rootVisual = new Grid();
            App.Current.RootVisual = rootVisual;
            var splashUi = new EntryPage();
            rootVisual.Children.Add(splashUi);
            if (Application.Current.IsRunningOutOfBrowser)
                Application.Current.MainWindow.WindowState = WindowState.Maximized;
            _dispatcher = rootVisual.Dispatcher;
            var starter = new ApplicationStarter(splashUi, new GridRootVisualManager(rootVisual));
            starter.Run();
        }

        private void Application_Exit(object sender, EventArgs e)
        {
        }

        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            _dispatcher.BeginInvoke(() => MessageBox.Show(e.ExceptionObject.ToString()));
            return;
            // If the app is running outside of the debugger then report the exception using
            // the browser's exception mechanism. On IE this will display it a yellow alert 
            // icon in the status bar and Firefox will display a script error.
            if (!System.Diagnostics.Debugger.IsAttached)
            {

                // NOTE: This will allow the application to continue running after an exception has been thrown
                // but not handled. 
                // For production applications this error handling should be replaced with something that will 
                // report the error to the website and stop the application.
                e.Handled = true;
                Deployment.Current.Dispatcher.BeginInvoke(delegate { ReportErrorToDOM(e); });
            }
        }
        private void ReportErrorToDOM(ApplicationUnhandledExceptionEventArgs e)
        {
            try
            {
                string errorMsg = e.ExceptionObject.Message + e.ExceptionObject.StackTrace;
                errorMsg = errorMsg.Replace('"', '\'').Replace("\r\n", @"\n");

                System.Windows.Browser.HtmlPage.Window.Eval("throw new Error(\"Unhandled Error in Silverlight Application " + errorMsg + "\");");
            }
            catch (Exception)
            {
            }
        }
    }
}
