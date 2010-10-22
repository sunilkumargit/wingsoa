/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Windows.Media.Imaging;
using Wing.Olap.Controls.MdxDesigner.Wrappers;

namespace Wing.Olap.Controls.MdxDesigner
{
    public class InfoItemControl : AreaItemControl
    {
        public readonly AreaItemWrapper Wrapper;

        public InfoItemControl(AreaItemWrapper wrapper, BitmapImage icon)
            : this(wrapper)
        {
            Icon = icon;
        }

        public InfoItemControl(AreaItemWrapper wrapper) 
            : base ()
        {
            if (wrapper == null)
                throw new ArgumentNullException("wrapper");

            Wrapper = wrapper;
            Caption = wrapper.Caption;

            UseDragDrop = true;
        }
    }
}
