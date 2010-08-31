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
using Wing.Olap.Core.Metadata;
using System.Windows.Media.Imaging;
using Wing.AgOlap.Controls.MdxDesigner.Wrappers;

namespace Wing.AgOlap.Controls.MdxDesigner
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
