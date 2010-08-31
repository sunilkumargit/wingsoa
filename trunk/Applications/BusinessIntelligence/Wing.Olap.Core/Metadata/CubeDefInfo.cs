/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Collections.Generic;

namespace Wing.Olap.Core.Metadata
{
    public enum CubeInfoType
    {
        Unknown,
        Cube,
        Dimension
    }

    /// <summary>
    /// Класс, прделоставляющий информацию о кубе
    /// </summary>
    public class CubeDefInfo : InfoBase
    {
        private DateTime m_LastProcessed = DateTime.MinValue;
        public DateTime LastProcessed
        {
            get { return m_LastProcessed; }
            set { m_LastProcessed = value; }
        }

        private DateTime m_LastUpdated = DateTime.MinValue;
        public DateTime LastUpdated
        {
            get { return m_LastUpdated; }
            set { m_LastUpdated = value; }
        }

        private CubeInfoType m_Type = CubeInfoType.Cube;
        public CubeInfoType Type
        {
            get { return m_Type; }
            set { m_Type = value; }
        }

        List<DimensionInfo> m_Dimensions = null;
        public List<DimensionInfo> Dimensions
        {
            get
            {
                if (m_Dimensions == null)
                    m_Dimensions = new List<DimensionInfo>();
                return m_Dimensions;
            }
            set {
                m_Dimensions = value;
            }
        }

        public DimensionInfo GetDimension(String dimensionUniqueName)
        {
            if (!String.IsNullOrEmpty(dimensionUniqueName))
            {
                foreach (DimensionInfo dimension in Dimensions)
                {
                    if (dimension.UniqueName == dimensionUniqueName)
                        return dimension;
                }
            }
            return null;
        }

        List<KpiInfo> m_Kpis = null;
        public List<KpiInfo> Kpis
        {
            get
            {
                if (m_Kpis == null)
                    m_Kpis = new List<KpiInfo>();
                return m_Kpis;
            }
            set
            {
                m_Kpis = value;
            }
        }

        public KpiInfo GetKpi(String kpiName)
        {
            if (!String.IsNullOrEmpty(kpiName))
            {
                foreach (KpiInfo kpi in Kpis)
                {
                    if (kpi.Name == kpiName)
                        return kpi;
                }
            }
            return null;
        }

        List<MeasureInfo> m_Measures = null;
        public List<MeasureInfo> Measures
        {
            get
            {
                if (m_Measures == null)
                    m_Measures = new List<MeasureInfo>();
                return m_Measures;
            }
            set
            {
                m_Measures = value;
            }
        }

        List<NamedSetInfo> m_NamedSets = null;
        public List<NamedSetInfo> NamedSets
        {
            get
            {
                if (m_NamedSets == null)
                    m_NamedSets = new List<NamedSetInfo>();
                return m_NamedSets;
            }
            set
            {
                m_NamedSets = value;
            }
        }

        List<MeasureGroupInfo> m_MeasureGroups = null;
        public List<MeasureGroupInfo> MeasureGroups
        {
            get {
                if (m_MeasureGroups == null)
                {
                    m_MeasureGroups = new List<MeasureGroupInfo>();
                }
                return m_MeasureGroups; 
            }
            set { m_MeasureGroups = value; }
        }

        public MeasureInfo GetMeasure(String measureUniqueName)
        {
            if (!String.IsNullOrEmpty(measureUniqueName))
            {
                foreach (MeasureInfo measure in Measures)
                {
                    if (measure.UniqueName == measureUniqueName)
                        return measure;
                }
            }
            return null;
        }

        public CubeDefInfo()
        {
        }

        private static CubeDefInfo m_Empty;
        public static CubeDefInfo Empty
        {
            get
            {
                if (m_Empty == null)
                {
                    m_Empty = new CubeDefInfo();
                }
                return m_Empty;
            }
        }
    }
}
