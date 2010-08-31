/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Wing.Olap.Core.Metadata
{
    public enum DimensionInfoTypeEnum
    {
        Accounts = 6,
        BillOfMaterials = 0x10,
        Channel = 13,
        Currency = 11,
        Customers = 7,
        Geography = 0x11,
        Measure = 2,
        Organization = 15,
        Other = 3,
        Products = 8,
        Promotion = 14,
        Quantitative = 5,
        Rates = 12,
        Scenario = 9,
        Time = 1,
        Unknown = 0,
        Utility = 10
    }

    public class DimensionInfo : InfoBase
    {
        private String m_UniqueName = String.Empty;
        public virtual String UniqueName
        {
            get { return m_UniqueName; }
            set { m_UniqueName = value; }
        }

        private DimensionInfoTypeEnum m_DimensionType = DimensionInfoTypeEnum.Unknown;
        public DimensionInfoTypeEnum DimensionType
        {
            get { return m_DimensionType; }
            set { m_DimensionType = value; }
        }

        private bool m_WriteEnabled = false;
        public bool WriteEnabled
        {
            get { return m_WriteEnabled; }
            set { m_WriteEnabled = value; }
        }

        private String m_ParentCubeId = string.Empty;
        public String ParentCubeId
        {
            get { return m_ParentCubeId; }
            set { m_ParentCubeId = value; }
        }

        List<HierarchyInfo> m_Hierarchies = null;
        public List<HierarchyInfo> Hierarchies
        {
            get
            {
                if (m_Hierarchies == null)
                    m_Hierarchies = new List<HierarchyInfo>();
                return m_Hierarchies;
            }
            set
            {
                m_Hierarchies = value;
            }
        }

        public HierarchyInfo GetHierarchy(String hierarchyUniqueName)
        {
            if (!String.IsNullOrEmpty(hierarchyUniqueName))
            {
                foreach (HierarchyInfo hierarchy in Hierarchies)
                {
                    if (hierarchy.UniqueName == hierarchyUniqueName)
                        return hierarchy;
                }
            }
            return null;
        }

        public DimensionInfo()
        {
        }
    }
}
