using System;
using System.Windows;

namespace UILibrary.Olap.UITestApplication
{
    public partial class App : Application
    {

        public App()
        {
            this.Startup += this.Application_Startup;
            this.Exit += this.Application_Exit;
            this.UnhandledException += this.Application_UnhandledException;

            InitializeComponent();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            this.RootVisual = new Page();
            if (Application.Current.RootVisual is FrameworkElement)
            {
                Wing.Olap.Features.ScrollViewerMouseWheelSupport.Initialize(Application.Current.RootVisual as FrameworkElement);
            }
        }

        private void Application_Exit(object sender, EventArgs e)
        {

        }

        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            // If the app is running outside of the debugger then report the exception using
            // the browser's exception mechanism. On IE this will display it a yellow alert 
            // icon in the status bar and Firefox will display a script error.
            //if (!System.Diagnostics.Debugger.IsAttached)
            //{

            // NOTE: This will allow the application to continue running after an exception has been thrown
            // but not handled. 
            // For production applications this error handling should be replaced with something that will 
            // report the error to the website and stop the application.
            e.Handled = true;
            Deployment.Current.Dispatcher.BeginInvoke(delegate { ReportErrorToDOM(e); });
            //}
        }
        private void ReportErrorToDOM(ApplicationUnhandledExceptionEventArgs e)
        {
            try
            {
                string errorMsg = e.ExceptionObject.ToString(); // + e.ExceptionObject.StackTrace;
                errorMsg = errorMsg.Replace('"', '\'').Replace("\r\n", @"\n");
                MessageBox.Show(errorMsg, "Error", MessageBoxButton.OK);
            }
            catch (Exception)
            {
            }
        }
    }
}
