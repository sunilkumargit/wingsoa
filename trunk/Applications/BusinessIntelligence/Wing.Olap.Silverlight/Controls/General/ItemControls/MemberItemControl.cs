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
using Wing.Olap.Core.Data;
using Wing.Olap.Controls.General;
using System.Windows.Media.Imaging;

namespace Wing.Olap.Controls.General.ItemControls
{
    public class MemberItemControl : ItemControlBase
    {
        internal MemberItemControl(MemberData info, BitmapImage image)
            : this(info)
        {
            Icon = image;
        }

        public MemberItemControl(MemberData info)
        {
            if (info == null)
                throw new ArgumentNullException("info");
            
            m_Info = info;
            Text = info.Caption;
            
            Icon = UriResources.Images.Member16;
        }

        MemberData m_Info = null;
        public MemberData Info
        {
            get {
                return m_Info;
            }
        }
    }
}