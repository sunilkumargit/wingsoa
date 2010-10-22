/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using Wing.Olap.Core.Metadata;

namespace Wing.Olap.Controls.General.ItemControls
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