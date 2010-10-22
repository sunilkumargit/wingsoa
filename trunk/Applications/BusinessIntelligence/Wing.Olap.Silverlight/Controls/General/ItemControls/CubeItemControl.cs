/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using Wing.Olap.Core.Metadata;

namespace Wing.Olap.Controls.General.ItemControls
{
    public class CubeItemControl : ItemControlBase
    {
        public CubeItemControl(CubeDefInfo info, bool useIcon)
            : base(useIcon)
        {
            if (info == null)
                throw new ArgumentNullException("info");
            
            m_Info = info;
            Text = info.Caption;

            if (useIcon)
            {
                Icon = UriResources.Images.Cube16;
            }
        }

        CubeDefInfo m_Info = null;
        public CubeDefInfo Info
        {
            get {
                return m_Info;
            }
        }
    }
}