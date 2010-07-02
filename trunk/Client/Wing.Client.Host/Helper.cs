using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Wing.Client
{
    public static class Helper
    {
        public static Uri GetBaseUrl(this System.Windows.Interop.SilverlightHost host)
        {
            Uri baseUri = host.Source;
            UriBuilder uriBuilder = new UriBuilder(baseUri.Scheme, baseUri.Host, baseUri.Port);
            return uriBuilder.Uri;
        }

        public static void DelayExecution(TimeSpan delay, Func<bool> callback)
        {
            var timer = new DispatcherTimer();
            timer.Interval = delay;
            EventHandler callbackWrapper = null;
            
            callbackWrapper = delegate(Object sender, EventArgs e)
            {
                if (!callback())
                {
                    ((DispatcherTimer)sender).Stop();
                    timer.Tick -= callbackWrapper;
                }
            };

            timer.Tick += callbackWrapper;
            timer.Start();
        }
    }
}
