/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Collections.Generic;
using Wing.Olap.Controls.MdxDesigner.CalculatedMembers;

namespace Wing.Olap.Controls.MdxDesigner.Wrappers
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
