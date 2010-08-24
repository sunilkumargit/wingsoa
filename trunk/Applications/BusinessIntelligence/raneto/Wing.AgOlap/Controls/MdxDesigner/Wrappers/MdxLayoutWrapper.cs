/*   
    Copyright (C) 2009 Galaktika Corporation ZAO

    This file is a part of Ranet.UILibrary.Olap
 
    Ranet.UILibrary.Olap is a free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
      
    You should have received a copy of the GNU General Public License
    along with Ranet.UILibrary.Olap.  If not, see
  	<http://www.gnu.org/licenses/> 
  
    If GPL v.3 is not suitable for your products or company,
    Galaktika Corp provides Ranet.UILibrary.Olap under a flexible commercial license
    designed to meet your specific usage and distribution requirements.
    If you have already obtained a commercial license from Galaktika Corp,
    you can use this file under those license terms.
*/

using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using Ranet.AgOlap.Controls.MdxDesigner.CalculatedMembers;

namespace Ranet.AgOlap.Controls.MdxDesigner.Wrappers
{
    public class MdxLayoutWrapper
    {
        String m_CubeName = String.Empty;
        /// <summary>
        /// Имя куба
        /// </summary>
        public String CubeName
        {
            get { return m_CubeName; }
            set { m_CubeName = value; }
        }

        String m_SubCube = String.Empty;
        /// <summary>
        /// Выражение для под-куба
        /// </summary>
        public String SubCube
        {
            get { return this.m_SubCube; }
            set {m_SubCube = value; }
        }

        List<AreaItemWrapper> m_Filters;
        public List<AreaItemWrapper> Filters
        {
            get
            {
                if (m_Filters == null)
                {
                    m_Filters = new List<AreaItemWrapper>();
                }
                return m_Filters;
            }
            set { m_Filters = value; }
        }

        List<AreaItemWrapper> m_Rows;
        public List<AreaItemWrapper> Rows
        {
            get
            {
                if (m_Rows == null)
                {
                    m_Rows = new List<AreaItemWrapper>();
                }
                return m_Rows;
            }
            set { m_Rows = value; }
        }

        List<AreaItemWrapper> m_Columns;
        public List<AreaItemWrapper> Columns
        {
            get
            {
                if (m_Columns == null)
                {
                    m_Columns = new List<AreaItemWrapper>();
                }
                return m_Columns;
            }
            set { m_Columns = value; }
        }

        List<AreaItemWrapper> m_Data;
        public List<AreaItemWrapper> Data
        {
            get
            {
                if (m_Data == null)
                {
                    m_Data = new List<AreaItemWrapper>();
                }
                return m_Data;
            }
            set { m_Data = value; }
        }

        List<CalcMemberInfo> m_CalculatedMembers;
        public List<CalcMemberInfo> CalculatedMembers
        {
            get
            {
                if (m_CalculatedMembers == null)
                {
                    m_CalculatedMembers = new List<CalcMemberInfo>();
                }
                return m_CalculatedMembers;
            }
            set { m_CalculatedMembers = value; }
        }

        List<CalculatedNamedSetInfo> m_CalculatedNamedSets;
        public List<CalculatedNamedSetInfo> CalculatedNamedSets
        {
            get
            {
                if (m_CalculatedNamedSets == null)
                {
                    m_CalculatedNamedSets = new List<CalculatedNamedSetInfo>();
                }
                return m_CalculatedNamedSets;
            }
            set { m_CalculatedNamedSets = value; }
        }

        public MdxLayoutWrapper()
        { 
        
        }
    }
}
