/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;

namespace Wing.Olap.Core.Data
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
