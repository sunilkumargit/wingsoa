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

namespace Wing.Client.Core
{
    public static class MvcHelper
    {
        public static String GetActionUrl(String controller, String action, string getParams = null)
        {
            var baseUrl = Application.Current.Host.GetBaseUrl().ToString();
            if (baseUrl.EndsWith("/"))
                baseUrl = baseUrl.Remove(baseUrl.Length - 1, 1);
            return String.Format("{0}/{1}/{2}.actn?{3}",
                baseUrl,
                controller,
                action,
                getParams);
        }

        public static Uri GetActionUri(String controller, String action, string getParams = null)
        {
            return new Uri(GetActionUrl(controller, action, getParams));
        }
    }
}
