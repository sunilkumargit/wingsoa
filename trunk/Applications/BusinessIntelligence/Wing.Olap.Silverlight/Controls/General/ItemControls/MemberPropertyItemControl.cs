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
using Wing.Olap.Controls.General;
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