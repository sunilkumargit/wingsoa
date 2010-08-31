/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
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

namespace Wing.Olap.Controls.PivotGrid
{
    public class CustomGridSplitter : GridSplitter
    {

        public CustomGridSplitter()
            : base()
        { 
        
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            HtmlPage.Document.AttachEvent("onmousemove", new EventHandler<HtmlEventArgs>(Document_OnMouseMove));
        }

        void Document_OnMouseMove(object sender, HtmlEventArgs e)
        {
            EventHandler handler = MouseMoved;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        public event EventHandler MouseMoved;

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            HtmlPage.Document.DetachEvent("onmousemove", new EventHandler<HtmlEventArgs>(Document_OnMouseMove));
        }
    }
}
