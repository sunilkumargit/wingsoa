﻿/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/
using System.Windows.Controls;

namespace Wing.AgOlap.Controls.List
{
    public class RanetListBox : ListBox
    {
        public RanetListBox()
        {
            DefaultStyleKey = typeof(RanetListBox);
        }

        ScrollViewer Scroller = null;
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Scroller = base.GetTemplateChild("ScrollViewer") as ScrollViewer;
            if (Scroller != null)
            {
                Wing.AgOlap.Features.ScrollViewerMouseWheelSupport.AddMouseWheelSupport(Scroller, this);
            }
        }


        ~RanetListBox()
        {
            Wing.AgOlap.Features.ScrollViewerMouseWheelSupport.RemoveMouseWheelSupport(this);
        }
    }
}
