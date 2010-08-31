﻿/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Collections.Generic;

namespace Wing.Olap.Core.Metadata
{
    public enum HierarchyInfoOrigin
    {
        AttributeHierarchy = 2,
        ParentChildHierarchy = 3,
        UserHierarchy = 1
    }

    /// <summary>
    /// Класс, прделоставляющий информацию об иерархии
    /// </summary>
    public class HierarchyInfo : InfoBase
    {
        private String m_UniqueName = String.Empty;
        public virtual String UniqueName
        {
            get { return m_UniqueName; }
            set { m_UniqueName = value; }
        }

        private String m_DefaultMember = String.Empty;
        public String DefaultMember
        {
            get { return m_DefaultMember; }
            set { m_DefaultMember = value; }
        }

        private String m_DisplayFolder = String.Empty;
        public String DisplayFolder
        {
            get { return m_DisplayFolder; }
            set { m_DisplayFolder = value; }
        }

        private HierarchyInfoOrigin m_HierarchyOrigin = HierarchyInfoOrigin.AttributeHierarchy;
        public HierarchyInfoOrigin HierarchyOrigin
        {
            get { return m_HierarchyOrigin; }
            set { m_HierarchyOrigin = value; }
        }

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

        //private String m_Custom_UnknownMemberUniqueName = String.Empty;
        //public String Custom_UnknownMemberUniqueName
        //{
        //    get { return m_Custom_UnknownMemberUniqueName; }
        //    set { m_Custom_UnknownMemberUniqueName = value; }
        //}

        private String m_Custom_AllMemberUniqueName = String.Empty;
        public String Custom_AllMemberUniqueName 
        {
            get { return m_Custom_AllMemberUniqueName; }
            set { m_Custom_AllMemberUniqueName = value; }
        }

        List<LevelInfo> m_Levels = null;
        public List<LevelInfo> Levels
        {
            get
            {
                if (m_Levels == null)
                    m_Levels = new List<LevelInfo>();
                return m_Levels;
            }
            set
            {
                m_Levels = value;
            }
        }

        public HierarchyInfo()
        {
        }
    }
}
