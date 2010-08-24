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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ranet.Olap.Core.Metadata
{
    public class MeasureGroupInfo
    {
        public MeasureGroupInfo()
        {
        }

        private String m_CatalogName = String.Empty;
        public String CatalogName
        {
            get { return m_CatalogName; }
            set { m_CatalogName = value; }
        }

        private String m_CubeName = String.Empty;
        public String CubeName
        {
            get { return m_CubeName; }
            set { m_CubeName = value; }
        }

        private String m_Caption = String.Empty;
        public virtual String Caption
        {
            get { return m_Caption; }
            set { m_Caption = value; }
        }

        private String m_Description = String.Empty;
        public virtual String Description
        {
            get { return m_Description; }
            set { m_Description = value; }
        }

        private String m_Name = String.Empty;
        public virtual String Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        private bool m_IsWriteEnabled = false;
        public bool IsWriteEnabled
        {
            get { return m_IsWriteEnabled; }
            set { m_IsWriteEnabled = value; }
        }

        List<String> m_Measures = null;
        public List<String> Measures
        {
            get {
                if (m_Measures == null)
                {
                    m_Measures = new List<String>();
                }
                return m_Measures; 
            }
            set { m_Measures = value; }
        }

        List<String> m_Dimensions = null;
        public List<String> Dimensions
        {
            get
            {
                if (m_Dimensions == null)
                {
                    m_Dimensions = new List<String>();
                }
                return m_Dimensions;
            }
            set { m_Dimensions = value; }
        }

        List<String> m_Kpis = null;
        public List<String> Kpis
        {
            get
            {
                if (m_Kpis == null)
                {
                    m_Kpis = new List<String>();
                }
                return m_Kpis;
            }
            set { m_Kpis = value; }
        }
    }
}
