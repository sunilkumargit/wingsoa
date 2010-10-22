/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Collections.Generic;

namespace Wing.Olap.Controls.ValueCopy.Wrappers
{
    public class ValueCopySettingsWrapper
    {
        public ValueCopySettingsWrapper()
        { 
        }

        ValueCopyTypes m_CopyType = ValueCopyTypes.CopyValueFromSource;
        /// <summary>
        /// Тип копирования
        /// </summary>
        public ValueCopyTypes CopyType
        {
            get { return m_CopyType; }
            set { m_CopyType = value; }
        }

        String m_Value = "0";
        /// <summary>
        /// Значение
        /// </summary>
        public String Value
        {
            get { return m_Value; }
            set { m_Value = value; }
        }

        double m_Coefficient = 1;
        /// <summary>
        /// Коэффициент
        /// </summary>
        public double Coefficient
        {
            get { return m_Coefficient; }
            set { m_Coefficient = value; }
        }

        List<CoordinateItem_Wrapper> m_CoordinatesList = null;
        /// <summary>
        /// Координаты
        /// </summary>
        public List<CoordinateItem_Wrapper> CoordinatesList
        {
            get
            {
                if (m_CoordinatesList == null)
                {
                    m_CoordinatesList = new List<CoordinateItem_Wrapper>();
                }
                return m_CoordinatesList;
            }
            set { m_CoordinatesList = value; }
        }

        bool m_ShowNotEmptyCoordinates = false;
        public bool ShowNotEmptyCoordinates
        {
            get { return m_ShowNotEmptyCoordinates; }
            set { m_ShowNotEmptyCoordinates = value; }
        }

    }
}
