/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/
using System;

namespace Wing.Olap.Core.Providers
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