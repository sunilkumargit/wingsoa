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
using System.Xml.Serialization;
using Wing.Olap.Controls.MemberChoice.Info;
using System.Collections.Generic;

namespace Wing.Olap.Controls.MdxDesigner.Wrappers
{
    public class Level_AreaItemWrapper : Filtered_AreaItemWrapper
    {
        String m_UniqueName = String.Empty;
        public String UniqueName
        {
            get { return m_UniqueName; }
            set { m_UniqueName = value; }
        }

        String m_HierarchyUniqueName = String.Empty;
        public String HierarchyUniqueName
        {
            get { return m_HierarchyUniqueName; }
            set { m_HierarchyUniqueName = value; }
        }

        public Level_AreaItemWrapper()
        {
        }

        public Level_AreaItemWrapper(LevelInfo info)
            : base(info)
        {
            UniqueName = info.UniqueName;
            HierarchyUniqueName = info.ParentHirerachyId;
        }
    }
}
