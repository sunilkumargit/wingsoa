/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System.Windows.Browser;

namespace Wing.Olap.Features
{
    public class BrowserHelper
    {
        public static bool IsMozilla
        {
            get
            {
                try
                {
                    if (HtmlPage.BrowserInformation.Name == "Netscape")
                        return true;
                }
                catch { }
                return false;
            }
        }
    }
}
