/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;

namespace Wing.Olap.Controls.ValueCopy.Wrappers
{
    public class CoordinateItem_Wrapper
    {
        public CoordinateItem_Wrapper()
        { }

        public CoordinateItem_Wrapper(CoordinateItem item)
        {
            CoordinateState = item.CoordinateState;
            DimensionCaption = item.DimensionCaption;
            DimensionUniqueName = item.DimensionUniqueName;
            HierarchyCaption = item.HierarchyCaption;
            HierarchyUniqueName = item.HierarchyUniqueName;
            Hierarchy_Custom_AllMemberUniqueName = item.Hierarchy_Custom_AllMemberUniqueName;

            SourceMember = item.SourceMember;
            DestMember = item.DestMember;
        }

        CoordinateStateTypes m_CoordinateState = CoordinateStateTypes.Enabled;
        public CoordinateStateTypes CoordinateState
        {
            get { return m_CoordinateState; }
            set { m_CoordinateState = value; }
        }

        MemberWrap m_SourceMember = null;
        public MemberWrap SourceMember
        {
            get { return m_SourceMember; }
            set { m_SourceMember = value; }
        }

        MemberWrap m_DestMember = null;
        public MemberWrap DestMember
        {
            get { return m_DestMember; }
            set { m_DestMember = value; }
        }

        String m_DimensionUniqueName = String.Empty;
        /// <summary>
        /// Уникальное имя измерения
        /// </summary>
        public String DimensionUniqueName
        {
            get { return m_DimensionUniqueName; }
            set { m_DimensionUniqueName = value; }
        }

        String m_HierarchyUniqueName = String.Empty;
        /// <summary>
        /// Уникальное имя иерархии
        /// </summary>
        public String HierarchyUniqueName
        {
            get { return m_HierarchyUniqueName; }
            set { m_HierarchyUniqueName = value; }
        }

        String m_DimensionCaption = String.Empty;
        /// <summary>
        /// Название измерения - используется ТОЛЬКО для отображения в гриде
        /// </summary>
        public String DimensionCaption
        {
            get { return m_DimensionCaption; }
            set { m_DimensionCaption = value; }
        }

        String m_HierarchyCaption = String.Empty;
        /// <summary>
        /// Название иерархии - используется ТОЛЬКО для отображения в гриде
        /// </summary>
        public String HierarchyCaption
        {
            get { return m_HierarchyCaption; }
            set { m_HierarchyCaption = value; }
        }

        String m_Hierarchy_Custom_AllMemberUniqueName = String.Empty;
        /// <summary>
        /// Уникальное имя элемента All
        /// </summary>
        public String Hierarchy_Custom_AllMemberUniqueName
        {
            get { return m_Hierarchy_Custom_AllMemberUniqueName; }
            set { m_Hierarchy_Custom_AllMemberUniqueName = value; }
        }

    }
}
