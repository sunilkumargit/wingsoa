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
using System.Xml;
using System.Globalization;

namespace Ranet.Olap.Core.Data
{
    public class CellData
    {
        public const String FORMAT_STRING = "FORMAT_STRING";

        public CellData()
        { 
        }

        public CellData(CellValueData value)
        {
            m_Value = value;
        }

        int m_Axis0_Coord = -1;
        public int Axis0_Coord
        {
            get { return m_Axis0_Coord; }
            set { m_Axis0_Coord = value; }
        }

        int m_Axis1_Coord = -1;
        public int Axis1_Coord
        {
            get { return m_Axis1_Coord; }
            set { m_Axis1_Coord = value; }
        }

        CellValueData m_Value = null;
        /// <summary>
        /// Значение ячейки
        /// </summary>
        public CellValueData Value
        {
            get { return m_Value; }
            set { m_Value = value; }
        }

        int m_BackColor = int.MaxValue;
        public int BackColor
        {
            get {
                object obj = null;
                if (Value != null && !Value.IsError)
                {
                    try
                    {
                        if (m_BackColor == int.MaxValue)
                        {
                            obj = Value.GetPropertyValue("BACK_COLOR");
                            if (obj != null)
                            {
                                m_BackColor = Convert.ToInt32(obj);
                            }
                            else
                            {
                                m_BackColor = int.MinValue;
                            }
                        }
                        return m_BackColor;
                    }
                    catch (Exception ex)
                    { 
                    }
                }
                return int.MinValue;
            }
        }

        int m_ForeColor = int.MaxValue;
        public int ForeColor
        {
            get
            {
                object obj = null;
                if (Value != null && !Value.IsError)
                {
                    try
                    {
                        if (m_ForeColor == int.MaxValue)
                        {
                            obj = Value.GetPropertyValue("FORE_COLOR");
                            if (obj != null)
                            {
                                m_ForeColor = Convert.ToInt32(obj);
                            }
                            else
                            {
                                m_ForeColor = int.MinValue;
                            }
                        }
                        return m_ForeColor;
                    }
                    catch (Exception ex)
                    {
                    }
                }
                return int.MinValue;
            }
        }

        public String FormatString
        {
            get
            {
                String res = null;
                if (Value != null && !Value.IsError)
                {
                    try
                    {
                        object obj = Value.GetPropertyValue(FORMAT_STRING);
                        if (obj != null)
                        {
                            res = obj.ToString();
                        }
                    }
                    catch
                    {
                    }
                }
                return res;
            }
        }
    }
}
