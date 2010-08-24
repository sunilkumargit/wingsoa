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
using System.Collections.Generic;
using Ranet.AgOlap.Controls.General;

namespace Ranet.AgOlap.Features
{
    internal class CustomMouseWheelSupport
    {
        public bool ScrollAlways { get; set; }
        private static Point currentMousePosition;
        private static BrowserMouseWheelEventListener browserListener;

        public event EventHandler<MouseWheelEventArgs> MouseWheelMoved;

        public FrameworkElement ElementToAddMouseWheelSupportTo { get; private set; }

        public CustomMouseWheelSupport(FrameworkElement elementToAddMouseWheelSupportTo, FrameworkElement parentElementWithMouseWheelSupport)
        {
            ElementToAddMouseWheelSupportTo = elementToAddMouseWheelSupportTo;

            //Make sure the browser listener is setup
            if (browserListener == null)
                browserListener = new BrowserMouseWheelEventListener();

            //Add an event handler to the browser listener for this particular Silverlight element
            browserListener.Moved += this.HandleBrowserMouseWheelMoved;
        }

        public void RemoveWheelSupport(FrameworkElement elementToAddMouseWheelSupportTo)
        {
            if (browserListener == null)
                browserListener = new BrowserMouseWheelEventListener();
            browserListener.RemoveMouseWheelListener();
            browserListener.Moved -= this.HandleBrowserMouseWheelMoved;
        }

        private void HandleBrowserMouseWheelMoved(object sender, MouseWheelEventArgs e)
        {
            //Only fire the mouse wheel moved event if this is the top-most
            //scrolling element in the UI tree.
            if (IsActive())
                this.MouseWheelMoved(this, e);
        }

        bool IsActive()
        {
            if(ElementToAddMouseWheelSupportTo != null)
            {
                if (ScrollAlways || AgControlBase.GetSLBounds(ElementToAddMouseWheelSupportTo).Contains(browserListener.MousePosition))
                    return true;
            }

            return false;
        }

        private class BrowserMouseWheelEventListener
        {
            public event EventHandler<MouseWheelEventArgs> Moved;
            public Point MousePosition { get; private set; }

            public BrowserMouseWheelEventListener()
            {
                if (HtmlPage.IsEnabled)
                {
                    HtmlPage.Window.AttachEvent("DOMMouseScroll", this.HandleMouseWheel);
                    HtmlPage.Window.AttachEvent("onmousewheel", this.HandleMouseWheel);
                    HtmlPage.Document.AttachEvent("onmousewheel", this.HandleMouseWheel);
                    HtmlPage.Document.AttachEvent("onmousemove", this.HandleMouseMove);
                }
            }

            public void RemoveMouseWheelListener()
            {
                if (HtmlPage.IsEnabled)
                {
                    HtmlPage.Window.DetachEvent("DOMMouseScroll", this.HandleMouseWheel);
                    HtmlPage.Window.DetachEvent("onmousewheel", this.HandleMouseWheel);
                    HtmlPage.Document.DetachEvent("onmousewheel", this.HandleMouseWheel);
                    HtmlPage.Document.DetachEvent("onmousemove", this.HandleMouseMove);
                }
            }

            private void HandleMouseMove(object sender, HtmlEventArgs args)
            {
                MousePosition = new Point(args.ClientX, args.ClientY);
            }

            private void HandleMouseWheel(object sender, HtmlEventArgs args)
            {
                double delta = 0;

                ScriptObject eventObj = args.EventObject;

                if (eventObj.GetProperty("wheelDelta") != null)
                {
                    delta = ((double)eventObj.GetProperty("wheelDelta")) / 120;


                    if (HtmlPage.Window.GetProperty("opera") != null)
                        delta = -delta;
                }
                else if (eventObj.GetProperty("detail") != null)
                {
                    delta = -((double)eventObj.GetProperty("detail")) / 3;

                    if (HtmlPage.BrowserInformation.UserAgent.IndexOf("Macintosh") != -1)
                        delta = delta * 3;
                }

                if (delta != 0 && this.Moved != null)
                {
                    MouseWheelEventArgs wheelArgs = new MouseWheelEventArgs(delta);
                    this.Moved(this, wheelArgs);

                    if (wheelArgs.BrowserEventHandled)
                        args.PreventDefault();
                }
            }
        }
    }
}
