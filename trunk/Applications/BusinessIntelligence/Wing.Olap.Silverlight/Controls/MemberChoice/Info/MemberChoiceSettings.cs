/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Collections.Generic;
using System.Text;
using Wing.Olap.Core.Data;

namespace Wing.Olap.Controls.MemberChoice.Info
{
    public class MemberChoiceSettings
    {
        public MemberChoiceSettings()
        { 
        }


        /// <summary>
        /// ����������� �������, ������������ ��������� ��������
        /// </summary>
        /// <param name="uniqueName">���������� ��� �������</param>
        /// <param name="caption">��������� �������</param>
        /// <param name="state">���������</param>
        public MemberChoiceSettings(MemberData info, SelectStates state)
        {
            m_Info = info;
            m_SelectState = state;
        }

        private MemberData m_Info = null;
        /// <summary>
        /// ���������� �� ��������
        /// </summary>
        public MemberData Info
        {
            get { return m_Info; }
            set { m_Info = value; }
        }

        /// <summary>
        /// ���������� ��� �������
        /// </summary>
        public String UniqueName
        {
            get {
                if (Info != null)
                {
                    return Info.UniqueName;
                }
                return String.Empty;
            }
        }
        
        /// <summary>
        /// ��������� �������
        /// </summary>
        public String Caption
        {
            get
            {
                if (Info != null)
                {
                    return Info.Caption;
                }
                return String.Empty;
            }
            
        }

        /// <summary>
        /// ��������� ����������� ��������
        /// </summary>
        private SelectStates m_SelectState = SelectStates.Not_Initialized;
        public SelectStates SelectState
        {
            get { return m_SelectState; }
            set { m_SelectState = value; }
        }
    }
}
