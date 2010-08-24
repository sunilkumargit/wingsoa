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
    public enum LevelInfoTypeEnum
    {
        Account = 0x1014,
        All = 1,
        BomResource = 0x1012,
        Calculated = 2,
        Channel = 0x1061,
        Company = 0x1042,
        CurrencyDestination = 0x1052,
        CurrencySource = 0x1051,
        Customer = 0x1021,
        CustomerGroup = 0x1022,
        CustomerHousehold = 0x1023,
        GeoCity = 0x2006,
        GeoContinent = 0x2001,
        GeoCountry = 0x2003,
        GeoCounty = 0x2005,
        GeoPoint = 0x2008,
        GeoPostalCode = 0x2007,
        GeoRegion = 0x2002,
        GeoStateOrProvince = 0x2004,
        OrgUnit = 0x1011,
        Person = 0x1041,
        Product = 0x1031,
        ProductGroup = 0x1032,
        Promotion = 0x1071,
        Quantitative = 0x1013,
        Regular = 0,
        Representative = 0x1062,
        Reserved1 = 8,
        Scenario = 0x1015,
        Time = 4,
        TimeDays = 0x204,
        TimeHalfYears = 0x24,
        TimeHours = 0x304,
        TimeMinutes = 0x404,
        TimeMonths = 0x84,
        TimeQuarters = 0x44,
        TimeSeconds = 0x804,
        TimeUndefined = 0x1004,
        TimeWeeks = 260,
        TimeYears = 20,
        Utility = 0x1016
    }

    public class LevelInfo : InfoBase
    {
        private String m_ParentCubeId = string.Empty;
        public String ParentCubeId
        {
            get { return m_ParentCubeId; }
            set { m_ParentCubeId = value; }
        }

        private String m_ParentDimensionId = String.Empty;
        public String ParentDimensionId
        {
            get { return m_ParentDimensionId; }
            set { m_ParentDimensionId = value; }
        }

        private String m_ParentHirerachyId = String.Empty;
        public String ParentHirerachyId
        {
            get { return m_ParentHirerachyId; }
            set { m_ParentHirerachyId = value; }
        }

        private String m_UniqueName = String.Empty;
        public virtual String UniqueName
        {
            get { return m_UniqueName; }
            set { m_UniqueName = value; }
        }

        private int m_LevelNumber = 0;
        public int LevelNumber
        {
            get { return m_LevelNumber; }
            set { m_LevelNumber = value; }
        }

        private LevelInfoTypeEnum m_LevelType = LevelInfoTypeEnum.Regular;
        public LevelInfoTypeEnum LevelType
        {
            get { return m_LevelType; }
            set { m_LevelType = value; }
        }

        private long m_MemberCount = 0;
        public long MemberCount
        {
            get { return m_MemberCount; }
            set { m_MemberCount = value; }
        }

        public string AttributeHierarchyName
        {
            get {
                if (Properties != null)
                {
                    object val = GetPropertyValue("LEVEL_ATTRIBUTE_HIERARCHY_NAME");
                    if (val != null)
                        return val.ToString();
                }
                return String.Empty;                
            }    
        }

        public int KeyCardinality
        {
            get
            {
                if (Properties != null)
                {
                    object val = GetPropertyValue("LEVEL_KEY_CARDINALITY");
                    if (val != null)
                        return Convert.ToInt32(val);
                }
                return 0;
            }
        }

        public int Origin
        {
            get
            {
                if (Properties != null)
                {
                    object val = GetPropertyValue("LEVEL_ORIGIN");
                    if (val != null)
                        return Convert.ToInt32(val);
                }
                return 0;
            }
        }

        public bool IsLevelFromKeyAttributeHierarchy
        {
            get
            {
                if ((Origin & 0x04) == 0x04)
                    return true;
                return false;
            }
        }

        public String KeySqlColumnName
        {
            get {
                if (Properties != null)
                {
                    object val = GetPropertyValue("LEVEL_KEY_SQL_COLUMN_NAME");
                    if (val != null)
                        return val.ToString();
                }
                return String.Empty;  
            }
        }

        public String NameSqlColumnName
        {
            get
            {
                if (Properties != null)
                {
                    object val = GetPropertyValue("LEVEL_NAME_SQL_COLUMN_NAME");
                    if (val != null)
                        return val.ToString();
                }
                return String.Empty;
            }
        }

        public bool NameColumnSameAsKeyColumn
        {
            get {
                // LEVEL_NAME_SQL_COLUMN_NAME сожержит примерно следующее: <LEVEL_NAME_SQL_COLUMN_NAME>NAME( [$Account].[Parent Account Key5] )</LEVEL_NAME_SQL_COLUMN_NAME>
                // LEVEL_KEY_SQL_COLUMN_NAME сожержит примерно следующее: <LEVEL_KEY_SQL_COLUMN_NAME>KEY( [$Account].[Parent Account Key5] )</LEVEL_KEY_SQL_COLUMN_NAME>

                // Следовательно пытаемся сравнить кустки строк без NAME и KEY соответсвенно чтобы определить настроены ли ключ и наименование на одну и ту же колонку
                String key = KeySqlColumnName;
                String name = NameSqlColumnName;
                if (String.IsNullOrEmpty(key) || String.IsNullOrEmpty(name))
                    return false;

                if (key.Length > 3 && name.Length > 4)
                {
                    String keySubStr = key.Substring(3);
                    String nameSubStr = name.Substring(4);
                    if (keySubStr == nameSubStr)
                        return true;
                }
                return false;
            }
        }

        private List<LevelPropertyInfo> m_LevelProperties = new List<LevelPropertyInfo>();
        public List<LevelPropertyInfo> LevelProperties
        {
            get { return m_LevelProperties; }
        }

        public LevelPropertyInfo GetLevelPropertyInfoByName(String propName)
        {
            if (m_LevelProperties != null)
            {
                foreach (LevelPropertyInfo propInfo in m_LevelProperties)
                {
                    if (propInfo.Name.ToLower() == propName.ToLower())
                        return propInfo;
                }
            }
            return null;
        }

        public LevelInfo()
        {
        }
    }
}
