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
using Wing.AgOlap.Controls.General;
using Wing.Olap.Core.Metadata;

namespace Wing.AgOlap.Controls.General.ItemControls
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