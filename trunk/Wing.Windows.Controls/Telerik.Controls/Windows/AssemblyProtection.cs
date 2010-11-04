namespace Telerik.Windows
{
    using System;
    using System.Globalization;
    using System.Windows;
    using Telerik.Windows.Controls;

    internal static class AssemblyProtection
    {
        internal const string ApplicationName = "MyApp";
        private const string Key = "Telerik.Windows.Controls.Key";

        public static void Validate()
        {
        }

        public static void ValidatePassPhrase()
        {
            Application app = Application.Current;
            if (!RadControl.IsInDesignMode)
            {
                if (app.Resources.Contains("Telerik.Windows.Controls.Key"))
                {
                    string appName = app.Resources["Telerik.Windows.Controls.Key"] as string;
                    if ((appName != null) && (appName == "MyApp"))
                    {
                        return;
                    }
                }
                throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "This version of Telerik RadControls for Silverlight is licensed only for use by {0}", new object[] { "MyApp" }));
            }
        }
    }
}

