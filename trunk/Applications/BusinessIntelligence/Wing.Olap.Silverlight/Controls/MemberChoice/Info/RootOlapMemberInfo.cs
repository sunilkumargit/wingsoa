/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using Wing.Olap.Core.Data;

namespace Wing.Olap.Controls.MemberChoice.Info
{
    /// <summary>
    /// Summary description for RootOlapMemberInfo.
    /// </summary>
    public class RootOlapMemberInfo : OlapMemberInfo
    {
        Dictionary<String, OlapMemberInfo> m_AllMembers = null;
        public Dictionary<String, OlapMemberInfo> AllMembers
        {
            get
            {
                if (m_AllMembers == null)
                {
                    m_AllMembers = new Dictionary<String, OlapMemberInfo>();
                }
                return m_AllMembers;
            }
        }

        public RootOlapMemberInfo(MemberData member)
            : base(member)
        {
        }

        public OlapMemberInfo FindMember(String uniqueName)
        {
            if (AllMembers.ContainsKey(uniqueName))
                return AllMembers[uniqueName];
            return null;
        }

        //public OlapMemberInfo FindMember(MemberDataWrapper findMember)
        //{
        //    //HybridDictionary dict = (HybridDictionary)(AllMembersByLevels[findMember.LevelName]);
        //    //dict[memberInfo.UniqueName] = memberInfo;

        //    object obj = AllMembers[findMember.Member.UniqueName];
        //    /*if(dict == null)
        //        return null;*/
        //    //object obj = dict[findMember.UniqueName];
        //    if(obj != null && obj is OlapMemberInfo)
        //        return (OlapMemberInfo)obj;
        //    else
        //        return null;
        //}

        public void ClearMembersState()
        {
            SetChildrenState(SelectStates.Not_Initialized, true);
        }

        #region AddMemberToHierarchy - ���������� �������� � ����������� ��������
        /// <summary>
        /// ��������� ������� � �������� MemberNodeInfo
        /// </summary>
        /// <param name="member">Member, ���������� � ������� ������ ���� ��������� � ��������</param>
        public OlapMemberInfo AddMemberToHierarchy(MemberData info)
        {
            //������: �������� OlapMemberInfo � �������� 
            //��������� ��������:
            //	- ���� ������� � �������� ��� ����, �� �����
            //	- ���� �������� � �������� ��� ���, �� ���� ��� ��������.
            //	  ���� �������� �����, �� �������� � �������� ���� ���, �� ��������� �������� � ���� ������ ���������� �������� ����������
            //	  ���� �������� �� �����, �� ������� �������� �������� (����� �� ��������) � ��������	
            //����� ������� � �������� ����������� �� ������ �������, �� � ��������� ����� ��� ��������� (���� �� �� ����)

            //���� ���������� � ������ ����� ��������� � ����������� ��������
            OlapMemberInfo memberInfo = FindMember(info.UniqueName);

            //���� ������� � �������� �� ������, �� ��� ����� ��������
            if (memberInfo == null)
            {
                //������� OlapMemberInfo
                memberInfo = new OlapMemberInfo(info);
                // ������������� � ������� ������������ �������� �� ������� - ��������� ��������� ���� ��������
                // ������ ������� ������� �������, ����������� ����������� ������������ ���������� ��������� � ������
                memberInfo.HierarchyStateChanged += new StateChangedEventHandler(memberInfo_HierarchyStateChanged);

                //OlapMemberInfo, ������� ����� ������������ ��� ������������ ����� ....->���->����->���������� ����������� ������
                //����� ����� ����������� ��� ������ ������������ ���������
                OlapMemberInfo addedHierarchy = memberInfo;

                bool foundParentHierarchy = true;
                while (foundParentHierarchy)
                {
                    String parentUniqueName = String.Empty;
                    if (addedHierarchy.Info != null)
                    {
                        PropertyData prop = addedHierarchy.Info.GetMemberProperty(MemberData.PARENT_UNIQUE_NAME_PROPERTY);
                        if (prop != null && prop.Value != null)
                        {
                            parentUniqueName = prop.Value.ToString();
                        }
                    }

                    if (String.IsNullOrEmpty(parentUniqueName))
                    {
                        // � ������ ���� �������� � �������� ��������� �������� ������ 1 � ����� ����������� ������� All, 
                        // �� ��������� ��� � ��������� �������� ��������� ��������� �������� ������� 0 � 1. ����� ������ �� ���������
                        // ����� ���������� ������������� ��������� �������� ��������� � ��������� �� �������� ������ ����������� ������� ��������� ��� ����-������ �� ��� ���������
                        // ���� �������� ����������, �� ��� ��������� �� �������� � ����������� � �������� � ������� ��������.

                        // ���� ����� �������� �������� �������� ��� �������
                        Dictionary<String, OlapMemberInfo> toDelete = new Dictionary<string, OlapMemberInfo>();
                        foreach (var child in Children.Values)
                        {
                            String pun = String.Empty;
                            if (child.Info != null)
                            {
                                PropertyData prop = child.Info.GetMemberProperty(MemberData.PARENT_UNIQUE_NAME_PROPERTY);
                                if (prop != null && prop.Value != null)
                                {
                                    pun = prop.Value.ToString();
                                }
                            }
                            if (!String.IsNullOrEmpty(pun) && pun == addedHierarchy.UniqueName)
                            {
                                toDelete.Add(child.UniqueName, child);
                            }
                        }

                        // ������� �� ��������� �������� �������� ��������
                        foreach (var del in toDelete)
                        {
                            Children.Remove(del.Key);
                        }

                        // ��������� � ��������� �������� ���������
                        AddChild(addedHierarchy);

                        // ��������� � �������� �� ��������, ������� ���� �� ����� ���������
                        foreach (var del in toDelete)
                        {
                            addedHierarchy.AddChild(del.Value);
                        }

                        foundParentHierarchy = false;
                        break;
                    }
                    else
                    {
                        OlapMemberInfo parentOlapMemberInfo = null;
                        parentOlapMemberInfo = FindMember(parentUniqueName);

                        //���� �������� ������, �� ��������� ��� ���� �������������� ����� � ��������
                        if (parentOlapMemberInfo != null)
                        {
                            parentOlapMemberInfo.AddChild(addedHierarchy);

                            foundParentHierarchy = false;
                            break;
                        }
                        else
                        {
                            //��������� � ��������� �������� ���������
                            AddChild(addedHierarchy);
                            foundParentHierarchy = false;
                            break;
                            //throw new Exception(String.Format("�������� ��� �������� {0} �� ������ � ��������", info.Member.UniqueName));

                            ////������� OlapMemberInfo ��� ��������
                            //parentOlapMemberInfo = new OlapMemberInfo(addedHierarchy.Member.Parent, Mode, addedHierarchy.Member.Parent.ChildCount);
                            ////��������� ������������ �������� � �����, ������� ����� �������� � ��������
                            //parentOlapMemberInfo.AddChildOlapMemberInfo(addedHierarchy);

                            ////� ���� ������ �� �������� ������ ������ � ����� ���� ��������� � ��������� memberInfoHierarchy.AllMembers
                            //memberInfoHierarchy.AllMembers[addedHierarchy.UniqueName] = addedHierarchy;

                            //addedHierarchy = parentOlapMemberInfo;

                        }
                    }
                }
            }

            return memberInfo;
        }

        void memberInfo_HierarchyStateChanged(OlapMemberInfo sender)
        {
            Raise_HierarchyStateChanged(sender);
        }

        #endregion AddMemberToHierarchy - ���������� �������� � ����������� ��������
    }
}
