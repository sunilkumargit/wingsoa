/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using Wing.Olap.Core.Metadata;

namespace Wing.Olap.Controls.MdxDesigner.Wrappers
{
    public class NamedSet_AreaItemWrapper : AreaItemWrapper
    {
        String m_Name = String.Empty;
        public String Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        public NamedSet_AreaItemWrapper()
        {
        }

        public NamedSet_AreaItemWrapper(NamedSetInfo info)
            : base(info)
        {
            Name = info.Name;
        }
    }
}

