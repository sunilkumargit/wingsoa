using System;
using System.Windows.Threading;

namespace Wing.Client
{
    public static class Helper
    {
        public static Uri GetBaseUrl(this System.Windows.Interop.SilverlightHost host)
        {
            Uri baseUri = host.Source;
            return new Uri(baseUri.ToString().Replace("ClientBin/Wing.Client.xap", ""));
        }

        public static Uri GetRelativeUrl(this System.Windows.Interop.SilverlightHost host, String relativeUri)
        {
            if (relativeUri.StartsWith(@"/"))
                relativeUri = relativeUri.Substring(1);
            return new Uri(host.GetBaseUrl(), relativeUri);
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
