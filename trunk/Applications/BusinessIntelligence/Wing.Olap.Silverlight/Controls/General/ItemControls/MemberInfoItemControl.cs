/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Windows.Media.Imaging;
using Wing.Olap.Core.Providers;

namespace Wing.Olap.Controls.General.ItemControls
{
    public class MemberInfoItemControl : ItemControlBase
    {
        internal MemberInfoItemControl(MemberInfo info, BitmapImage image)
            : this(info)
        {
            Icon = image;
        }

        public MemberInfoItemControl(MemberInfo info)
        {
            if (info == null)
                throw new ArgumentNullException("info");
            
            m_Info = info;
            Text = info.Caption;
            
            Icon = UriResources.Images.Member16;
        }

        MemberInfo m_Info = null;
        public MemberInfo Info
        {
            get {
                return m_Info;
            }
        }
    }
}
