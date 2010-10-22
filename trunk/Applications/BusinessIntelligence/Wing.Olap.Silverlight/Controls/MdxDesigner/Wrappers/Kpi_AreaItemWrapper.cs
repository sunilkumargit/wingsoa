/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using Wing.Olap.Core.Metadata;

namespace Wing.Olap.Controls.MdxDesigner.Wrappers
{
    public class Kpi_AreaItemWrapper : AreaItemWrapper
    {
        KpiControlType m_Type;
        public KpiControlType Type
        {
            get { return m_Type; }
            set { m_Type = value; }
        }

        String m_Name = String.Empty;
        public String Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        public Kpi_AreaItemWrapper()
        {
        }

        public Kpi_AreaItemWrapper(KpiInfo info, KpiControlType type)
            : base(info)
        {
            Name = info.Name;
            Type = type;
            Custom_KpiGoal = info.Custom_KpiGoal;
            Custom_KpiStatus = info.Custom_KpiStatus;
            Custom_KpiTrend = info.Custom_KpiTrend;
            Custom_KpiValue = info.Custom_KpiValue;

        }

        String m_Custom_KpiTrend = String.Empty;
        public String Custom_KpiTrend
        {
            get { return m_Custom_KpiTrend; }
            set { m_Custom_KpiTrend = value; }
        }

        String m_Custom_KpiStatus = String.Empty;
        public String Custom_KpiStatus
        {
            get { return m_Custom_KpiStatus; }
            set { m_Custom_KpiStatus = value; }
        }

        String m_Custom_KpiValue = String.Empty;
        public String Custom_KpiValue
        {
            get { return m_Custom_KpiValue; }
            set { m_Custom_KpiValue = value; }
        }

        String m_Custom_KpiGoal = String.Empty;
        public String Custom_KpiGoal
        {
            get { return m_Custom_KpiGoal; }
            set { m_Custom_KpiGoal = value; }
        }
    }
}
