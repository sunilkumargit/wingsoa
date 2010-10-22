/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using Wing.Olap.Core.Metadata;

namespace Wing.Olap.Controls.General.ItemControls
{
    public class MemberPropertyItemControl : ItemControlBase
    {
        public MemberPropertyItemControl(MemberPropertyInfo info)
        {
            if (info == null)
                throw new ArgumentNullException("info");
            
            m_Info = info;
            Text = info.Name;
            
            Icon = UriResources.Images.MemberProperty16;
        }

        MemberPropertyInfo m_Info = null;
        public MemberPropertyInfo Info
        {
            get {
                return m_Info;
            }
        }
    }
}