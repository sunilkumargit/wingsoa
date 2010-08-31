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
using Wing.AgOlap.Controls.General;

namespace Wing.AgOlap.Controls.General.ItemControls
{
    public class LevelPropertyItemControl : ItemControlBase
    {
        public LevelPropertyItemControl(LevelPropertyInfo info)
        {
            if (info == null)
                throw new ArgumentNullException("info");
            
            m_Info = info;
            Text = info.Caption;
            
            Icon = UriResources.Images.LevelProperty16;
        }

        LevelPropertyInfo m_Info = null;
        public LevelPropertyInfo Info
        {
            get {
                return m_Info;
            }
        }
    }
}