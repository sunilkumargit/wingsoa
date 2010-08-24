/*   
    Copyright (C) 2009 Galaktika Corporation ZAO

    This file is a part of Galaktika.BusinessMonitor
 
    Galaktika.BusinessMonitor is a free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
      
    You should have received a copy of the GNU General Public License
    along with Galaktika.BusinessMonitor.  If not, see <http://www.gnu.org/licenses/>.
  
    If GPL v.3 is not suitable for your products or company,
    Galaktika Corp provides Galaktika.BusinessMonitor under a flexible commercial license
    designed to meet your specific usage and distribution requirements.
    If you have already obtained a commercial license from Galaktika Corp,
    you can use this file under those license terms.
*/

using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Ranet.AgOlap;

namespace Ranet.Olap.Core.Data
{   

    [XmlRoot("tbl", Namespace = Common.Namespace)]
    public class DataTableWrapper
    {
        public DataTableWrapper()
        {
        }

        private List<DataTableColumnDefinition> m_Columns = new List<DataTableColumnDefinition>();
        [XmlArray("cols")]
        [XmlArrayItem("col", typeof(DataTableColumnDefinition))]
        public List<DataTableColumnDefinition> Columns
        {
            get
            {
                return m_Columns;
            }
        }

        private List<DataTableRowDefinition> m_Rows = new List<DataTableRowDefinition>();
        [XmlArray("rows")]
        [XmlArrayItem("row", typeof(DataTableRowDefinition))]
        public List<DataTableRowDefinition> Rows
        {
            get
            {
                return m_Rows;
            }
        }

        private List<string> m_Items = new List<string>();
        [XmlArray("items")]
        //[XmlArrayItem(typeof(Boolean))]
        //[XmlArrayItem(typeof(Char))]
        //[XmlArrayItem(typeof(SByte))]
        //[XmlArrayItem(typeof(Byte))]
        //[XmlArrayItem(typeof(Int16))]
        //[XmlArrayItem(typeof(UInt16))]
        //[XmlArrayItem(typeof(Int32))]
        //[XmlArrayItem(typeof(UInt32))]
        //[XmlArrayItem(typeof(Int64))]
        //[XmlArrayItem(typeof(UInt64))]
        //[XmlArrayItem(typeof(Single))]
        //[XmlArrayItem(typeof(Double))]
        //[XmlArrayItem(typeof(Decimal))]
        //[XmlArrayItem(typeof(DateTime))]
        [XmlArrayItem("s", typeof(string), IsNullable = true)]
        //[XmlArrayItem(typeof(DBNull))]
        public List<string> Items
        {
            get
            {
                return m_Items;
            }
        }

        public int RowCount
        {
            get
            {
                return m_Items.Count / m_Columns.Count;
            }
        }

        public static DataTableWrapper Parse(string xml)
        {
            return XmlSerializationUtility.XmlStr2Obj<DataTableWrapper>(xml);
        }
    }

    [XmlRoot("col", Namespace = Common.Namespace)]
    public class DataTableColumnDefinition
    {
        public DataTableColumnDefinition()
        {
        }

        public DataTableColumnDefinition(string name, string caption)
            : this(name, caption, TypeCode.Object)
        {
        }

        public DataTableColumnDefinition(string name, string caption, Type type)
            : this(name, caption, System.Type.GetTypeCode(type))
        {
        }

        public DataTableColumnDefinition(string name, string caption, TypeCode type)
        {
            this.Name = name;
            this.Caption = caption;
            this.Type = type;
        }

        [XmlElement("n")]
        public string Name { get; set; }
        [XmlElement("c")]
        public string Caption { get; set; }
        [XmlElement("t")]
        public TypeCode Type { get; set; }
    }

    [XmlRoot("cell", Namespace = Common.Namespace)]
    public class DataTableCellDefinition
    {
        public DataTableCellDefinition()
        {
        }

        public DataTableCellDefinition(string name, object value)
            : this(name, value, TypeCode.Object)
        {
        }

        public DataTableCellDefinition(string name, object value, Type type)
            : this(name, value, System.Type.GetTypeCode(type))
        {
        }

        public DataTableCellDefinition(string name, object value, TypeCode type)
        {
            this.ColumnName = name;
            this.Value = value;
            this.Type = type;
        }

        [XmlElement("column")]
        public string ColumnName { get; set; }
        [XmlElement("val")]
        public object Value { get; set; }
        [XmlElement("t")]
        public TypeCode Type { get; set; }


    }


    [XmlRoot("row", Namespace = Common.Namespace)]
    public class DataTableRowDefinition
    {
        public DataTableRowDefinition()
        {
        }

        private List<DataTableCellDefinition> m_Cells = new List<DataTableCellDefinition>();
        [XmlArray("cells")]
        [XmlArrayItem("cell")]
        public List<DataTableCellDefinition> Cells
        {
            get
            {
                return m_Cells;
            }
        }

    }

    //[XmlRoot("dbnull", Namespace = Common.Namespace)]
    //public class DbNull
    //{
    //    public DbNull()
    //    {
    //    }
    //}
}


