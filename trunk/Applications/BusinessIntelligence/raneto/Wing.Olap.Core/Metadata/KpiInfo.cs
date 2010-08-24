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
    public class KpiInfo : InfoBase
    {
        private String m_DisplayFolder = String.Empty;
        public String DisplayFolder
        {
            get { return m_DisplayFolder; }
            set { m_DisplayFolder = value; }
        }

        private String m_StatusGraphic = String.Empty;
        public String StatusGraphic
        {
            get { return m_StatusGraphic; }
            set { m_StatusGraphic = value; }
        }

        private String m_TrendGraphic = String.Empty;
        public String TrendGraphic
        {
            get { return m_TrendGraphic; }
            set { m_TrendGraphic = value; }
        }

        private String m_ParentCubeId = String.Empty;
        public String ParentCubeId
        {
            get { return m_ParentCubeId; }
            set { m_ParentCubeId = value; }
        }

        private String m_ParentKpiId = String.Empty;
        public String ParentKpiId
        {
            get { return m_ParentKpiId; }
            set { m_ParentKpiId = value; }
        }

        public const String KPI_VALUE = "KPI_VALUE";
        public const String KPI_GOAL = "KPI_GOAL";
        public const String KPI_STATUS = "KPI_STATUS";
        public const String KPI_TREND = "KPI_TREND";

        public String Custom_KpiValue
        {
            get {
                object val = GetPropertyValue(KPI_VALUE);
                if (val != null)
                    return val.ToString();
                return String.Empty;
            }
        }

        public String Custom_KpiGoal
        {
            get
            {
                object val = GetPropertyValue(KPI_GOAL);
                if (val != null)
                    return val.ToString();
                return String.Empty;
            }
        }

        public String Custom_KpiStatus
        {
            get
            {
                object val = GetPropertyValue(KPI_STATUS);
                if (val != null)
                    return val.ToString();
                return String.Empty;
            }
        }

        public String Custom_KpiTrend
        {
            get
            {
                object val = GetPropertyValue(KPI_TREND);
                if (val != null)
                    return val.ToString();
                return String.Empty;
            }
        }
    }
}
