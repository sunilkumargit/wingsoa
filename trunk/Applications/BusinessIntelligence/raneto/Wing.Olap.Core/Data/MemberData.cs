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
using System.Xml.Serialization;
using System.Xml;
using System.Globalization;

namespace Ranet.Olap.Core.Data
{
    public enum MemberVisualizationTypes
    {
        Caption,
        Key,
        KeyAndCaption,
        UniqueName
    }

    public class MemberData : DataBase, IPropertiesData
    {
        public const String KEY0_PROPERTY = "KEY0";
        public const String PARENT_UNIQUE_NAME_PROPERTY = "PARENT_UNIQUE_NAME";

        private long m_ChildCount = 0;
        public long ChildCount
        {
            get { return m_ChildCount; }
            set { m_ChildCount = value; }
        }

        private bool m_DrilledDown = false;
        public bool DrilledDown
        {
            get { return m_DrilledDown; }
            set { m_DrilledDown = value; }
        }

        private int m_LevelDepth = 0;
        public int LevelDepth
        {
            get { return m_LevelDepth; }
            set { m_LevelDepth = value; }
        }

        private String m_LevelName = String.Empty;
        public String LevelName
        {
            get { return m_LevelName; }
            set { m_LevelName = value; }
        }

        private String m_HierarchyUniqueName = String.Empty;
        public String HierarchyUniqueName
        {
            get { return m_HierarchyUniqueName; }
            set { m_HierarchyUniqueName = value; }
        }

        public String Custom_Rollup
        {
            get { 
                PropertyData prop = GetMemberProperty("CUSTOM_ROLLUP");
                if (prop != null && prop.Value != null)
                {
                    return prop.Value.ToString();
                }
                return string.Empty;
            }
        }

        public String Unary_Operator
        {
            get
            {
                PropertyData prop = GetMemberProperty("UNARY_OPERATOR");
                if (prop != null && prop.Value != null)
                {
                    return prop.Value.ToString();
                }
                return string.Empty;
            }
        }

        private bool m_ParentSameAsPrevious = false;
        public bool ParentSameAsPrevious
        {
            get { return m_ParentSameAsPrevious; }
            set { m_ParentSameAsPrevious = value; }
        }

        public MemberData()
        {
        }

        List<PropertyData> m_Properties = null;
        public List<PropertyData> Properties
        {
            get {
                if (m_Properties == null)
                {
                    m_Properties = new List<PropertyData>();
                    //m_Properties.Add(new PropertyData("Caption", this.Caption));
                    //m_Properties.Add(new PropertyData("Name", this.Name));
                    //m_Properties.Add(new PropertyData("UniqueName", this.UniqueName));
                    //m_Properties.Add(new PropertyData("ChildCount", this.ChildCount));
                    //m_Properties.Add(new PropertyData("DrilledDown", this.DrilledDown));
                    //m_Properties.Add(new PropertyData("Description", this.Description));
                    ////m_Properties.Add(new PropertyData("Custom_Rollup", this.Custom_Rollup));
                    ////m_Properties.Add(new PropertyData("Unary_Operator", this.Unary_Operator));
                    //m_Properties.Add(new PropertyData("HierarchyUniqueName", this.HierarchyUniqueName));
                    //m_Properties.Add(new PropertyData("LevelDepth", this.LevelDepth));
                    //m_Properties.Add(new PropertyData("LevelName", this.LevelName));
                    //m_Properties.Add(new PropertyData("ParentSameAsPrevious", this.ParentSameAsPrevious));
                }
                return m_Properties;
            }
            set {
                m_Properties = value;
            }
        }

        List<PropertyData> m_MemberProperties = null;
        public List<PropertyData> MemberProperties
        {
            get
            {
                if (m_MemberProperties == null)
                {
                    m_MemberProperties = new List<PropertyData>();
                }
                return m_MemberProperties;
            }
            set
            {
                m_MemberProperties = value;
            }
        }


        #region IPropertiesData Members


        public PropertyData GetProperty(string name)
        {
            foreach (PropertyData prop in Properties)
            {
                if (prop.Name == name)
                    return prop;
            }
            return null;
        }

        public PropertyData GetMemberProperty(string name)
        {
            foreach (PropertyData prop in MemberProperties)
            {
                if (prop.Name == name)
                    return prop;
            }
            return null;
        }

        #endregion

        public String GetText(MemberVisualizationTypes type)
        {
            String res = String.Empty;

            String key0 = String.Empty;
            if (type == MemberVisualizationTypes.Key ||
                type == MemberVisualizationTypes.KeyAndCaption)
            {
                PropertyData prop = GetMemberProperty(KEY0_PROPERTY);
                if (prop == null)
                {
                    // Пока что оставили для совместимости с вин-версией
                    prop = GetMemberProperty(MemberData.KEY0_PROPERTY);
                }
                if (prop != null)
                {
                    if (prop.Value != null)
                        key0 = prop.Value.ToString();
                    else
                    {
                        // в режиме отображения кодов вместо null нужно светить Caption,  а то получается когда в таблице несколько вычисляемых элементов у всех их светится null (ПФ)
                        // key0 = "null";
                    }
                }
            }

            // Определяем что именно нужно светить в контроле
            switch (type)
            {
                case MemberVisualizationTypes.Caption:
                    res = Caption;
                    break;
                case MemberVisualizationTypes.Key:
                    // Для элементов уровня ALL вместо ключа 0 (который никак нельзя поменять) отображаем Caption
                    if (LevelDepth == 0 && !String.IsNullOrEmpty(LevelName) && LevelName.ToLower().Contains(".[(all)]"))
                    {
                        res = Caption;
                    }
                    else
                    {
                        //Если ключ в запросе не получался, то выводим просто Caption
                        if (!String.IsNullOrEmpty(key0))
                        {
                            res = key0;
                        }
                        else
                        {
                            res = Caption;
                        }
                    }
                    break;
                case MemberVisualizationTypes.KeyAndCaption:
                    // Для элементов уровня ALL вместо ключа 0 (который никак нельзя поменять) отображаем Caption
                    if (LevelDepth == 0 && !String.IsNullOrEmpty(LevelName) && LevelName.ToLower().Contains(".[(all)]"))
                    {
                        res = Caption;
                    }
                    else
                    {
                        //Если ключ в запросе не получался, то выводим просто Caption
                        if (!String.IsNullOrEmpty(key0))
                        {
                            res = key0 + " " + Caption;
                        }
                        else
                        {
                            res = Caption;
                        }
                    }
                    break;
                case MemberVisualizationTypes.UniqueName:
                    res = UniqueName;
                    break;
                default:
                    res = Caption;
                    break;
            }
            return res;
        }

        public String Key0
        {
            get
            {
                String res = String.Empty;
                PropertyData prop = GetMemberProperty(KEY0_PROPERTY);
                if (prop != null)
                {
                    if (prop.Value != null)
                        res = prop.Value.ToString();
                    else
                        res = "null";
                }
                else
                {
                    res = null;
                }
                return res;
            }
        }

        /// <summary>
        /// Member is calculated
        /// </summary>
        public bool IsCalculated
        {
            get
            {
                PropertyData prop = GetMemberProperty(KEY0_PROPERTY);
                if (prop != null && prop.Value == null)
                {
                    return true;
                }
                return false;
            }
        }

        private static MemberData m_Empty;
        public static MemberData Empty
        {
            get
            {
                if (m_Empty == null)
                {
                    m_Empty = new MemberData();
                }

                return m_Empty;
            }
        }
    }
}
