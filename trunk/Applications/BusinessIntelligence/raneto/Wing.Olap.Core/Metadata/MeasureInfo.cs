/*   
    Copyright (C) 2009 Galaktika Corporation ZAO

    This file is a part of Ranet.UILibrary.Olap
 
    Ranet.UILibrary.Olap is a free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
      
    You should have received a copy of the GNU General Public License
    along with Ranet.UILibrary.Olap.  If not, see <http://www.gnu.org/licenses/>.
  
    If GPL v.3 is not suitable for your products or company,
    Galaktika Corp provides Ranet.UILibrary.Olap under a flexible commercial license
    designed to meet your specific usage and distribution requirements.
    If you have already obtained a commercial license from Galaktika Corp,
    you can use this file under those license terms.
*/

using System;
using System.Net;

namespace Ranet.Olap.Core.Metadata
{
    public class MeasureInfo : InfoBase
    {
        private String m_ParentCubeId = string.Empty;
        public String ParentCubeId
        {
            get { return m_ParentCubeId; }
            set { m_ParentCubeId = value; }
        }

        private String m_UniqueName = String.Empty;
        public virtual String UniqueName
        {
            get { return m_UniqueName; }
            set { m_UniqueName = value; }
        }

        private String m_DisplayFolder = String.Empty;
        public String DisplayFolder
        {
            get { return m_DisplayFolder; }
            set { m_DisplayFolder = value; }
        }

        private String m_Expression = String.Empty;
        public String Expression
        {
            get { return m_Expression; }
            set { m_Expression = value; }
        }

        private int m_NumericPrecision = 0;
        public int NumericPrecision
        {
            get { return m_NumericPrecision; }
            set { m_NumericPrecision = value; }
        }

        private short m_NumericScale = 0;
        public short NumericScale
        {
            get { return m_NumericScale; }
            set { m_NumericScale = value; }
        }
        
        private String m_Units = String.Empty;
        public String Units
        {
            get { return m_Units; }
            set { m_Units = value; }
        }

        public String MeasureGroup
        {
            get {
                PropertyInfo pi = GetProperty("MEASUREGROUP_NAME");
                if (pi != null && pi.Value != null)
                {
                    return pi.Value.ToString();
                }
                return null;
            }
        }
    }
}
