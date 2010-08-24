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

namespace Ranet.Olap.Core.Providers
{
    public class KpiView
    {

        public const string NotAvailable = "N\\A";

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

        public string Caption
        {
            get; set;
        }

        private double? m_KpiValue;
        public double? KpiValue
        {
            get {
                return m_KpiValue;
            }
            set
            {
                m_KpiValue = value;
            }
        }

        public double? KpiVariance
        {
            get;
            set;
        }

        public String KpiWeight
        {
            get;
            set;
        } 

        public double? KpiGoal
        {
            get; set;
        }

        public String KpiStatus
        {
            get; set;
        }

        public String KpiTrend
        {
            get; set;
        }
    }
}