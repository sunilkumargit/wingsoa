/*   
    Copyright (C) 2009 Galaktika Corporation ZAO

    This file is a part of Ranet.UILibrary.Olap
 
    Ranet.UILibrary.Olap is a free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
      
    You should have received a copy of the GNU General Public License
    along with Ranet.UILibrary.Olap.  If not, see
  	<http://www.gnu.org/licenses/> 
  
    If GPL v.3 is not suitable for your products or company,
    Galaktika Corp provides Ranet.UILibrary.Olap under a flexible commercial license
    designed to meet your specific usage and distribution requirements.
    If you have already obtained a commercial license from Galaktika Corp,
    you can use this file under those license terms.
*/

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
using System.Windows.Browser;

namespace Ranet.AgOlap.Features
{
    /// <summary>
    ///     This class is used to access the user's clipboard
    /// </summary>
    public static class Clipboard
    {
        /// <summary>
        ///     Set the text to the user's clipboard
        /// </summary>
        /// <param name="text">Text to set in the clipboard</param>
        public static void SetClipboardText(string text)
        {
            // get the user's clipboard
            var clipboard = (ScriptObject)HtmlPage.Window.GetProperty("clipboardData");

            if (clipboard != null)
            {
                // if clipboard is available, try to set the text to the clipboard
                bool success = (bool)clipboard.Invoke("setData", "text", text);
                if (!success)
                {
                    HtmlPage.Window.Alert(Localization.Clipboard_UnableCopy);
                }
            }
            else
            {
                HtmlPage.Window.Alert(Localization.Clipboard_ClipboardUnavailable);
            }
        }

        public static string GetClipboardText()
        {
            return GetClipboardText(true);
        }

        /// <summary>
        ///     Get the text from the user's clipboard
        /// </summary>
        /// <returns>Text present in user's clipboard</returns>
        public static string GetClipboardText(bool useAlert)
        {
            string text = string.Empty;
            try
            {
                // get the user's clipboard
                var clipboard = (ScriptObject)HtmlPage.Window.GetProperty("clipboardData");

                if (clipboard != null)
                {
                    // if clipboard is available, try to get the text from the clipboard
                    text = (string)clipboard.Invoke("getData", "text");
                }
                else
                {
                    if (useAlert)
                    {
                        HtmlPage.Window.Alert(Localization.Clipboard_ClipboardUnavailable);
                    }
                }
            }
            catch
            {
            }
            return text;
        }

         //const string HostNoClipboard = "The clipboard isn't available in the current host.";  
         //const string ClipboardFailure = "The text couldn't be copied into the clipboard.";  
         //const string BeforeFlashCopy = "The text will now attempt to be copied...";  
         //const string FlashMimeType = "application/x-shockwave-flash";  
   
         //// HARD-CODED!  
         //const string ClipboardFlashMovie = "clipboard.swf";  
   
         ///// <summary>  
         ///// Write to the clipboard (IE and/or Flash)  
         ///// </summary>  
         //public static void SetText(string text)
         //{
         //    // document.window.clipboardData.setData(format, data);  
         //    var clipboardData = (ScriptObject)HtmlPage.Window.GetProperty("clipboardData");
         //    if (clipboardData != null)
         //    {
         //        bool success = (bool)clipboardData.Invoke("setData", "text", text);
         //        if (!success)
         //        {
         //            HtmlPage.Window.Alert(ClipboardFailure);
         //        }
         //    }
         //    else
         //    {
         //        HtmlPage.Window.Alert(BeforeFlashCopy);

         //        // Append a Flash embed element with the data encoded  
         //        string safeText = HttpUtility.UrlEncode(text);
         //        var elem = HtmlPage.Document.CreateElement("div");
         //        HtmlPage.Document.Body.AppendChild(elem);
         //        elem.SetProperty("innerHTML", "<embed src=\"" +
         //            ClipboardFlashMovie + "\" " +
         //            "FlashVars=\"clipboard=" + safeText + "\" width=\"0\" " +
         //            "height=\"0\" type=\"" + FlashMimeType + "\"></embed>");
         //    }

         //}  
    }
}
