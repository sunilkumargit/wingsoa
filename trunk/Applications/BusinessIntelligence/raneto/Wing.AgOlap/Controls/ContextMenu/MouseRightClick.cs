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
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Media;

namespace Ranet.AgOlap.Controls.ContextMenu
{
    [ScriptableType]
    public class MouseRightClick
    {
        private static readonly string OverflowScript = @"
var overflow = window.document.getElementById(""overflow"");
overflow.onmouseup = function(e) {
    if (!e) {
        e = window.event;
    }
    if (e.button == 2) {
        e.cancelBubble = true;
        if (e.stopPropagation) {
            e.stopPropagation()
        }
        this.style.zIndex = -1;
        var plugin = window.document.getElementById(""<%=Xaml.ClientID%>"");
        plugin.Content.ContexMenuProvider.OnContextMenu(e.pageX, e.pageY);
    }
}";

        private MouseRightClick()
        {
            this.Initialize();
        }

        public event EventHandler<MouseRightClickEventArgs> RightClick;

        [ScriptableMember]
        public void OnContextMenu(string xStr, string yStr)
        {
            if (m_IsInjected)
            {
                if (HtmlPage.IsEnabled)
                {
                    HtmlPage.Document.Body.RemoveChild(HtmlPage.Document.GetElementById("overflow"));
                    HtmlPage.Document.DocumentElement.RemoveChild(HtmlPage.Document.GetElementById("overflow_js"));
                }
                m_IsInjected = false;
            }
            double num = Convert.ToDouble(xStr, CultureInfo.InvariantCulture);
            double num2 = Convert.ToDouble(yStr, CultureInfo.InvariantCulture);
            Point pluginPosition = this.GetPluginPosition();
            this.OnRightClick(new Point(num - pluginPosition.X, num2 - pluginPosition.Y));
        }

        private void OnRightClick(Point cp)
        {
            var rightClick = this.RightClick;
            if (rightClick != null)
            {
                var mce = new MouseRightClickEventArgs(cp);
                rightClick(this, mce);
            }
        }

        private Point GetPluginPosition()
        {
            Point point = new Point(0.0, 0.0);
            try
            {
                for (HtmlElement element = HtmlPage.Plugin; element != null; element = (HtmlElement)element.GetProperty("offsetParent"))
                {
                    point.X += (double)element.GetProperty("offsetLeft");
                    point.Y += (double)element.GetProperty("offsetTop");
                }
            }
            catch
            {
            }
            return point;
        }

        private bool m_IsInjected = false;

        private void MouseDownHandler(object sender, HtmlEventArgs e)
        {
            if (e.MouseButton == MouseButtons.Right)
            {
                if (!IsIE)
                {
                    if (!m_IsInjected)
                    {
                        string id = HtmlPage.Plugin.Id;
                        string script = OverflowScript.Replace("<%=Xaml.ClientID%>", id);
                        HtmlElement element = HtmlPage.Document.CreateElement("div");
                        element.Id = "overflow";
                        element.SetStyleAttribute("position", "absolute");
                        element.SetStyleAttribute("backgroundColor", "transparent");
                        element.SetStyleAttribute("width", "100%");
                        element.SetStyleAttribute("height", "100%");
                        element.SetStyleAttribute("left", "0");
                        element.SetStyleAttribute("top", "0");
                        element.SetStyleAttribute("zIndex", "1");
                        HtmlPage.Document.Body.AppendChild(element);
                        HtmlElement element2 = HtmlPage.Document.CreateElement("script");
                        element2.Id = "overflow_js";
                        element2.SetAttribute("type", "text/javascript");
                        element2.SetProperty("text", script);
                        HtmlPage.Document.DocumentElement.AppendChild(element2);
                        m_IsInjected = true;
                    }
                }
                else
                {
                    HtmlPage.Document.DetachEvent("oncontextmenu", new EventHandler<HtmlEventArgs>(this.OnIEContextMenu));
                    HtmlPage.Document.AttachEvent("oncontextmenu", new EventHandler<HtmlEventArgs>(this.OnIEContextMenu));
                }
            }
        }

        private void OnIEContextMenu(object sender, HtmlEventArgs e)
        {
            e.PreventDefault();
            e.StopPropagation();
            HtmlPage.Document.DetachEvent("oncontextmenu", new EventHandler<HtmlEventArgs>(this.OnIEContextMenu));
            this.OnRightClick(new Point((double)e.OffsetX, (double)e.OffsetY));
        }

        private static bool IsIE
        {
            get
            {
                if (HtmlPage.IsEnabled)
                {
                    return HtmlPage.BrowserInformation.UserAgent.Contains("MSIE");
                }

                return false;
            }
        }

        private static MouseRightClick m_Instance;
        public static MouseRightClick Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = new MouseRightClick();
                }

                return m_Instance;
            }
        }

        private void Initialize()
        {
            if (HtmlPage.IsEnabled)
            {
                HtmlPage.RegisterScriptableObject("ContexMenuProvider", this);
                HtmlPage.Document.DetachEvent("onmousedown", new EventHandler<HtmlEventArgs>(MouseDownHandler));
                HtmlPage.Document.AttachEvent("onmousedown", new EventHandler<HtmlEventArgs>(MouseDownHandler));
            }
        }

        private void Dispose()
        {
            if (HtmlPage.IsEnabled)
            {
                HtmlPage.Document.DetachEvent("onmousedown", new EventHandler<HtmlEventArgs>(MouseDownHandler));
            }
        }
    }
}
