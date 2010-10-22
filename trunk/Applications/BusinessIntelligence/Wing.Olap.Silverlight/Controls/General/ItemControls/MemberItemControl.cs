/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Windows.Media.Imaging;
using Wing.Olap.Core.Data;

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