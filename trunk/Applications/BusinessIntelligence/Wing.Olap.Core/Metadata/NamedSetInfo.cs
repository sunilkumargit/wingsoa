/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wing.Olap.Core.Metadata
{
    public class NamedSetInfo : InfoBase
    {
        private String m_DisplayFolder = String.Empty;
        public virtual String DisplayFolder
        {
            get { return m_DisplayFolder; }
            set { m_DisplayFolder = value; }
        }

        private String m_Expression = String.Empty;
        public virtual String Expression
        {
            get { return m_Expression; }
            set { m_Expression = value; }
        }

        private String m_ParentCubeId = string.Empty;
        public String ParentCubeId
        {
            get { return m_ParentCubeId; }
            set { m_ParentCubeId = value; }
        }

        public NamedSetInfo()
        {
        }
    }
}