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
using Wing.Olap.Providers;

namespace Wing.Olap.Controls.MemberChoice.Info
{
    #region ��������� ���������
    //����� ������� OlapMemberInfo � �������� ����� ���������� � ����� �� 3 ���������
    //	�� ������
    //	������ ���
    //	������ � ���������
    //
    //� ��� ����� ������ ����� ��������������� ���� ���������� � ������������ ��������� - ���
    //������ ������, � ������� ���� �����������
    /// <summary>
    /// ��������� ��������� OlapMemberInfo
    /// </summary>
    public enum SelectStates
    {
        /// <summary>
        /// �� ��������������� (��������� ���������)
        /// </summary>
        Not_Initialized,
        /// <summary>
        /// �� ������
        /// </summary>
        Not_Selected,
        /// <summary>
        /// ������ ���
        /// </summary>
        Selected_Self,
        /// <summary>
        /// ������ � ���������
        /// </summary>
        Selected_With_Children,
        /// <summary>
        /// ������� ��� ��������
        /// </summary>
        Labeled_As_Parent,
        /// <summary>
        /// ������ ���������
        /// </summary>
        Selected_By_Parent,
        /// <summary>
        /// ������ ��������� � ���������
        /// </summary>
        Selected_By_Parent_With_Children,
        /// <summary>
        /// ������ � ���������, ���� ����������� (������������ ������ � ������ Filter_Area)
        /// </summary>
        Selected_With_Children_Has_Excluded,
        /// <summary>
        /// ������ ��������� � ���������, ���� ����������� (������������ ������ � ������ Filter_Area)
        /// </summary>
        Selected_By_Parent_With_Children_Has_Excluded,
    }
    #endregion ��������� ���������

    /// <summary>
    /// ���������� �� ���� ������.
    /// </summary>
    public class OlapMemberInfo
    {
        #region �������� OlapMemberInfo

        private MemberData m_Info = null;
        /// <summary>
        /// ������ ��� �������� ���������, ��������������� ������� ���� ������
        /// </summary>
        public MemberData Info
        {
            get
            {
                return m_Info;
            }
        }

        /// <summary>
        /// ��������� �������� OlapMemberInfo
        /// </summary>
        Dictionary<String, OlapMemberInfo> m_Children = null;

        /// <summary>
        /// ��������� �������� OlapMemberInfo. ���� - ����. ���
        /// </summary>
        public Dictionary<String, OlapMemberInfo> Children
        {
            get
            {
                if (m_Children == null)
                    m_Children = new Dictionary<string, OlapMemberInfo>();
                return m_Children;
            }
        }

        /// <summary>
        /// ������� c�������� "�����������" OlapMemberInfo
        /// </summary>
        private SelectStates m_SelectState = SelectStates.Not_Initialized;
        /// <summary>
        /// ������� c�������� "�����������" OlapMemberInfo
        /// </summary>
        public SelectStates SelectState
        {
            get
            {
                return this.m_SelectState;
            }
        }


        private string HierarchyUniqueName
        {
            get
            {
                return Info.HierarchyUniqueName;
            }
        }
        #endregion �������� OlapMemberInfo

        #region �������� Member �� CellSet

        /// <summary>
        /// ���������� ��� (member.UniqueName)
        /// </summary>
        public String UniqueName
        {
            get
            {
                return this.Info.UniqueName;
            }
        }

        /// <summary>
        /// ���������� true ���� ���� �������� ��������, ����� - false
        /// </summary>
        public bool HasChildren
        {
            get
            {
                if (Info != null && Info.ChildCount > 0)
                {
                    return true;
                }
                return false;
            }
        }
        #endregion �������� Member �� CellSet

        #region ������������

        public OlapMemberInfo(MemberData info)
        {
            if (info == null)
                throw new ArgumentNullException("info");

            this.m_Info = info;

            this.cubeChildrenCount = QueryProvider.GetRealChildrenCount(info);
        }


        #endregion ������������

        #region private ��������
        /// <summary>
        /// ������������ ������� OlapMemberInfo
        /// </summary>
        private OlapMemberInfo m_Parent = null;
        /// <summary>
        /// ������������ ������� OlapMemberInfo
        /// </summary>
        public OlapMemberInfo Parent
        {
            get
            {
                return m_Parent;
            }
        }


        /// <summary>
        /// ������������ ������� OlapMemberInfo
        /// </summary>
        private void SetParentOlapMemberInfo(OlapMemberInfo parent)
        {
            this.m_Parent = parent;

            if (Parent != null)
            {
                SetStateByParent();
            }
        }


        /// <summary>
        /// ���������� true - ������� ����� ���������� ��������  
        /// ���� false - � �������� ��� ���������� ��������  
        /// </summary>
        private bool HasSelectedChildren
        {
            get
            {
                if (Children != null)
                {
                    foreach (OlapMemberInfo info in Children.Values)
                    {
                        if (info.SelectState != SelectStates.Not_Initialized && info.SelectState != SelectStates.Not_Selected)
                        {
                            return true;
                        }
                    }
                }

                //���� ������� "������ � ���������", "������ � ���������, ���� �����������",
                //"������ ��������� � ���������", "������ ��������� � ���������, ���� �����������"
                //�� ��� ��� ����������� �������� ��� ������ ���� ��������� � ��������. 
                //� ��� �������� ������� �������� � Set ��� �������� (����� ����������� �������) ����� ���� ��������� �� ��� � ������ ������������� ��������� ��������
                //������� ���� � ������������� ���� ���������� ��������� �� ���� �� ��� ����������, �� ������� ��� � �������� ���� ��������� ��������
                if ((this.SelectState == SelectStates.Selected_With_Children ||
                    this.SelectState == SelectStates.Selected_With_Children_Has_Excluded ||
                    this.SelectState == SelectStates.Selected_By_Parent_With_Children ||
                    this.SelectState == SelectStates.Selected_By_Parent_With_Children_Has_Excluded)
                    && AllChildrenIsLoaded == false)
                    return true;

                return false;
            }
        }

        /// <summary>
        /// ���������� true - ���� ��� �������� �������� ������� (� ���-�� ��������� ����� ��������� ���-�� ��������): ������ - "������ ���", �� ������ - "������ � ���������"  
        /// ����� - false;  
        /// </summary>
        private bool AllChildrenIsSelected
        {
            get
            {
                if (Children != null)
                {
                    if (this.Info == null)
                        return false;

                    //������ ����� ������� ����������
                    if (CubeChildrenCount == 0 || LoadedChildrenCount != CubeChildrenCount)
                        return false;

                    foreach (OlapMemberInfo memberInfo in Children.Values)
                    {
                        //���� ������� - ����, �� ��������� ������ ���� "������ ���"
                        //���� ������� - �� ����, �� ��������� ������ ���� "������ � ���������"
                        if (memberInfo == null ||
                            ((memberInfo.HasChildren && memberInfo.SelectState != SelectStates.Selected_With_Children) ||
                            (!memberInfo.HasChildren && memberInfo.SelectState != SelectStates.Selected_Self)))
                        {
                            return false;
                        }
                    }

                    return true;
                }
                return false;
            }
        }

        long cubeChildrenCount = 0;
        /// <summary>
        /// ���������� �������� ��������� � ����
        /// </summary>
        public long CubeChildrenCount
        {
            get
            {
                return cubeChildrenCount;
            }
            set
            {
                cubeChildrenCount = value;
            }
        }

        /// <summary>
        /// ���������� �������� ���������, ����������� � ��������
        /// </summary>
        public long LoadedChildrenCount
        {
            get
            {
                if (this.Children == null)
                    return 0;
                return Convert.ToInt64(Children.Count);
            }
        }

        /// <summary>
        /// ���������� true ���� ��� �������� �������� ��������� � ��������, ����� - false
        /// </summary>
        private bool AllChildrenIsLoaded
        {
            get
            {
                //������� ����������
                if (this.Info == null)
                    return true;

                if (LoadedChildrenCount != CubeChildrenCount)
                {
                    return false;
                }

                return true;
            }
        }
        /// <summary>
        /// ���������� true - ������� ����� ��������, ������� �� �������;
        /// ���� false - � �������� ��� ����������� ��������
        /// </summary>
        public bool HasExcludedChildren
        {
            get
            {
                if (Children != null)
                {
                    if (this.Info == null)
                        return false;

                    //������ ����� ������� ����������. ���� �� ��� �������� ���������, �� �������,��� ����������� �������� ���� (��� �� ����������������)

                    //��!!!!! ���� ������� "������ � ���������", "������ � ���������, ���� �����������",
                    //"������ ��������� � ���������", "������ ��������� � ���������, ���� �����������"
                    //�� ��� ��� ����������� �������� ��� ������ ���� ��������� � ��������. 
                    //� ��� �������� ������� �������� � Set ��� �������� (����� ����������� �������) ����� ���� ��������� �� ��� � ������ ������������� ��������� ��������
                    //������� ���� � ������������� ���� ���������� ��������� �� ���� �� ��� ����������, �� ��� ������������� �������� �������
                    if (this.SelectState != SelectStates.Selected_With_Children &&
                        this.SelectState != SelectStates.Selected_With_Children_Has_Excluded &&
                        this.SelectState != SelectStates.Selected_By_Parent_With_Children &&
                        this.SelectState != SelectStates.Selected_By_Parent_With_Children_Has_Excluded)
                    {
                        if (LoadedChildrenCount != CubeChildrenCount)
                            return true;
                    }

                    foreach (OlapMemberInfo memberInfo in Children.Values)
                    {
                        if (memberInfo != null &&
                            (memberInfo.SelectState == SelectStates.Not_Initialized ||
                            memberInfo.SelectState == SelectStates.Not_Selected ||
                            memberInfo.SelectState == SelectStates.Labeled_As_Parent ||
                            (memberInfo.SelectState == SelectStates.Selected_Self && memberInfo.HasChildren == true) ||
                            (memberInfo.SelectState == SelectStates.Selected_By_Parent && memberInfo.HasChildren == true)))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        private OlapMemberInfo Root
        {
            get
            {
                OlapMemberInfo parent = this;
                while (parent.Parent != null)
                {
                    parent = parent.Parent;
                }
                return parent;
            }
        }
        #endregion private ��������

        #region ������� � ��������

        /// <summary>
        /// ���������� ������� ��������� ���������.
        /// </summary>
        public delegate void StateChangedEventHandler(OlapMemberInfo sender);
        /// <summary>
        /// ������� ���������� ��������� 
        /// </summary>
        public event StateChangedEventHandler StateChanged;

        /// <summary>
        /// ������� ���������� ��������� ���� �������� ���������
        /// </summary>
        public event StateChangedEventHandler HierarchyStateChanged;

        #endregion ������� � ��������

        #region ��������� �������

        /// <summary>
        /// ���������� ������� - ���������� ��������� ���� �������� ���������
        /// </summary>
        protected void Raise_HierarchyStateChanged(OlapMemberInfo sender)
        {
            StateChangedEventHandler handler = this.HierarchyStateChanged;
            if (handler != null)
            {
                handler(sender);
            }
        }

        /// <summary>
        /// ���������� ������� - ���������� ���������
        /// </summary>
        private void Raise_StateChanged()
        {
            StateChangedEventHandler handler = this.StateChanged;
            if (handler != null)
            {
                handler(this);
            }
        }
        #endregion ��������� �������

        #region ������������ �� ��������� ��������� ��� �������� � ������������ ���������
        /// <summary>
        /// ������������� �������� �������� � ��������� ���������
        /// </summary>
        /// <param name="state">����� ���������</param>
        /// <param name="recursive">true - ������� ���� ����������; false - ������� ���� ���� �������� ���������</param>
        private void Raise_SetChildrenState(SelectStates state, bool recursive)
        {
            if (Children != null)
            {
                foreach (OlapMemberInfo memberInfo in Children.Values)
                {
                    memberInfo.SetChildrenState(state, recursive);
                }
            }
        }

        /// <summary>
        /// �������� �������� ��������� �� ��������� ��������� �������� ��������
        /// </summary>
        /// <param name="changedMember">�������, ������� ����� ������ ������� ���� ���������</param>
        private void Raise_ParentSelectStateChanged(OlapMemberInfo changedMember)
        {
            //���������� ��� ������� ������ ��������, �������� ������ �������, ������� ���������� ����������� �������
            if (Children != null)
            {
                foreach (OlapMemberInfo memberInfo in Children.Values)
                {
                    memberInfo.ParentSelectStateChanged(this, changedMember);
                }
            }
        }

        /// <summary>
        /// �������� ������������� �������� �� ��������� ��������� �������� ��������
        /// </summary>
        /// <param name="changedMember">�������, ������� ����� ������ ������� ���� ���������</param>
        private void Raise_ChildSelectStateChanged(OlapMemberInfo changedMember)
        {
            //���������� ��� ������� �������� �������� ������ �������, ������� ���������� ����������� �������
            if (Parent != null)
            {
                Parent.ChildSelectStateChanged(this, changedMember);
            }
        }
        #endregion ������������ �� ��������� ��������� ��� �������� � ������������ ���������

        #region ��������� ��������� �� ���������� ������������ ���� �������� ���������
        /// <summary>
        /// ������������� �������� �������� � ��������� ���������
        /// </summary>
        /// <param name="state">����� ���������</param>
        /// <param name="recursive">true - ������� ���� ����������; false - ������� ���� ���� �������� ���������</param>
        protected void SetChildrenState(SelectStates state, bool recursive)
        {
            bool changed = SetState(state);

            if (recursive)
                Raise_SetChildrenState(state, recursive);
            else
            {
                if (changed)
                    Raise_SetChildrenState(state, recursive);
            }
        }

        /// <summary>
        /// ������� ���������� ��������� ��� �������� ��� ������������ � ��� ��� ����-���� ������������ OlapMemberInfo ��������� � ����� ���������
        /// </summary>
        /// <param name="changedMember">�������, ������� ������ ������� ���������</param>
        private void ParentSelectStateChanged(OlapMemberInfo sender, OlapMemberInfo changedMember)
        {
            bool changed = false;
            switch (changedMember.SelectState)
            {
                case SelectStates.Selected_Self:
                    break;
                case SelectStates.Not_Selected:
                    changed = SetState(SelectStates.Not_Initialized);
                    if (changed)
                    {
                        Raise_ParentSelectStateChanged(changedMember);
                    }
                    break;
                case SelectStates.Selected_With_Children:
                    if (HasChildren)
                        SetState(SelectStates.Selected_By_Parent_With_Children);
                    else
                        SetState(SelectStates.Selected_By_Parent);
                    Raise_ParentSelectStateChanged(changedMember);
                    break;
                case SelectStates.Not_Initialized:
                    changed = SetState(SelectStates.Not_Initialized);
                    if (changed)
                        Raise_ChildSelectStateChanged(changedMember);
                    break;
            }
        }

        /// <summary>
        /// ������� ���������� �������� ��� �������� ��� ������������ � ��� ��� ����-���� �������� OlapMemberInfo ��������� � ����� ���������
        /// </summary>
        /// <param name="changedMember">�������, ������� ������ ������� ���������</param>
        private void ChildSelectStateChanged(OlapMemberInfo sender, OlapMemberInfo changedMember)
        {
            bool changed = false;
            switch (changedMember.SelectState)
            {
                case SelectStates.Selected_Self:
                    if (SelectState == SelectStates.Selected_Self && AllChildrenIsSelected)
                    {
                        SetNewState(SelectStates.Selected_With_Children);
                    }
                    else
                    {
                        if (SelectState == SelectStates.Not_Initialized ||
                            SelectState == SelectStates.Not_Selected)
                        {
                            changed = SetState(SelectStates.Labeled_As_Parent);
                        }
                        if (changed)
                            Raise_ChildSelectStateChanged(changedMember);
                    }
                    break;
                case SelectStates.Not_Selected:
                case SelectStates.Not_Initialized:
                    if (this.HasSelectedChildren == false)
                    {
                        if (SelectState == SelectStates.Labeled_As_Parent)
                        {
                            changed = SetState(SelectStates.Not_Initialized);
                            Raise_SetChildrenState(SelectStates.Not_Initialized, false);

                            if (changed)
                                Raise_ChildSelectStateChanged(changedMember);
                            break;
                        }
                    }

                    if (HasSelectedChildren == false && AllChildrenIsLoaded == true)
                    {
                        switch (SelectState)
                        {
                            case SelectStates.Selected_With_Children:
                            case SelectStates.Selected_With_Children_Has_Excluded:
                                changed = SetState(SelectStates.Selected_Self);
                                Raise_SetChildrenState(SelectStates.Not_Initialized, false);
                                break;
                            case SelectStates.Selected_By_Parent_With_Children:
                            case SelectStates.Selected_By_Parent_With_Children_Has_Excluded:
                                changed = SetState(SelectStates.Selected_By_Parent);
                                break;
                        }
                    }
                    else
                    {
                        switch (SelectState)
                        {
                            case SelectStates.Selected_With_Children:
                                changed = SetState(SelectStates.Selected_With_Children_Has_Excluded);
                                break;
                            case SelectStates.Selected_By_Parent_With_Children:
                                changed = SetState(SelectStates.Selected_By_Parent_With_Children_Has_Excluded);
                                break;
                        }
                    }
                    if (changed)
                        Raise_ChildSelectStateChanged(changedMember);
                    break;
                case SelectStates.Selected_With_Children:
                    if (SelectState == SelectStates.Selected_Self && AllChildrenIsSelected)
                    {
                        SetNewState(SelectStates.Selected_With_Children);
                    }
                    else
                    {
                        if (SelectState == SelectStates.Not_Initialized ||
                            SelectState == SelectStates.Not_Selected)
                        {
                            changed = SetState(SelectStates.Labeled_As_Parent);
                        }
                        else
                        {
                            if (HasExcludedChildren == false)
                            {
                                if (SelectState == SelectStates.Selected_With_Children_Has_Excluded)
                                {
                                    changed = SetState(SelectStates.Selected_With_Children);
                                    Raise_ParentSelectStateChanged(this);
                                }
                                if (SelectState == SelectStates.Selected_By_Parent_With_Children_Has_Excluded)
                                    changed = SetState(SelectStates.Selected_By_Parent_With_Children);
                            }
                        }
                        if (changed)
                            Raise_ChildSelectStateChanged(changedMember);
                    }
                    break;
            }
        }
        #endregion ��������� ��������� �� ���������� ������������ ���� �������� ���������

        /*		/// <summary>
				/// ���������� ��������� ������������ ���������
				/// </summary>
				/// <param name="member"></param>
				/// <returns></returns>
				private HybridDictionary GetAscendants(Member member)
				{
					if(member == null)
						return null;
			
					if(member.Parent == null)
						return null;

					HybridDictionary ascendants = new HybridDictionary();
			
					do{
						member = member.Parent;
						ascendants.Add(member.UniqueName, member);
					}while(member.Parent != null);

					return ascendants;
				}
		*/

        /// <summary>
        /// ���������� ArrayList � Set-��� ��� �������� ���������
        /// </summary>
        /// <returns></returns>
        private List<String> GetChildrenSet(Dictionary<String, OlapMemberInfo> membersInSet)
        {
            List<String> childSet = new List<String>();

            try
            {
                if (Children != null)
                {
                    foreach (OlapMemberInfo memberInfo in Children.Values)
                    {
                        String str = memberInfo.GenerateSet(membersInSet);
                        if (!String.IsNullOrEmpty(str))
                        {
                            childSet.Add(str);
                        }
                    }
                }
            }
            catch (Exception)
            {

            }

            return childSet;
        }

        /// <summary>
        /// ���������� Set �� �������� ��������, ������� ������ ���� ���������
        /// </summary>
        /// <returns></returns>
        private String GetExcludedChildrenSet(Dictionary<String, OlapMemberInfo> membersInSet)
        {
            String childSet = null;

            try
            {
                if (Children != null)
                {
                    foreach (OlapMemberInfo memberInfo in Children.Values)
                    {
                        if (memberInfo.SelectState == SelectStates.Not_Initialized ||
                            memberInfo.SelectState == SelectStates.Not_Selected ||
                            memberInfo.SelectState == SelectStates.Selected_Self ||
                            memberInfo.SelectState == SelectStates.Labeled_As_Parent ||
                            memberInfo.SelectState == SelectStates.Selected_With_Children_Has_Excluded ||
                            memberInfo.SelectState == SelectStates.Selected_By_Parent_With_Children_Has_Excluded ||
                           (memberInfo.SelectState == SelectStates.Selected_By_Parent && memberInfo.HasChildren == true))
                        {
                            if (childSet != null)
                            {
                                childSet = childSet + ", ";
                            }

                            childSet = childSet + memberInfo.UniqueName;

                            //������� ������� � ������ ���������� ������������ �������
                            if (membersInSet != null)
                                membersInSet[memberInfo.UniqueName] = memberInfo;
                        }
                    }
                }

                if (childSet != null)
                    childSet = "{" + childSet + "}";
            }
            catch (Exception)
            {

            }

            return childSet;
        }

        /// <summary>
        /// ���������� Set ��� �������� � ������ ��������
        /// </summary>
        /// <returns>null - ������� � Set �� ��������</returns>
        public String GenerateSet(Dictionary<String, OlapMemberInfo> membersInSet)
        {
            String ResultSet;
            String selfSet = null;
            List<String> childSet = new List<String>();

            switch (SelectState)
            {
                case SelectStates.Selected_With_Children:
                    //AddCalculatedMembers(
                    //GENERATE
                    //(
                    //		{ 
                    //		 	 <Member_Unique_Name>
                    //		}, 
                    //		DESCENDANTS
                    //      (
                    //			<Dimension_Unique_Name>.CURRENTMEMBER
                    //		)
                    //	)
                    //)

                    selfSet = "AddCalculatedMembers(GENERATE({" +
                        this.UniqueName +
                        "}, DESCENDANTS(" + HierarchyUniqueName + ".CURRENTMEMBER)))";

                    //������� ������� � ������ ���������� ������������ �������
                    if (membersInSet != null)
                        membersInSet[this.UniqueName] = this;
                    break;
                case SelectStates.Selected_By_Parent:
                case SelectStates.Selected_With_Children_Has_Excluded:
                case SelectStates.Selected_By_Parent_With_Children_Has_Excluded:

                    if (SelectState == SelectStates.Selected_By_Parent && HasChildren == false)
                        //childSet = GetChildrenSet(membersInSet);
                        break;

                    //{
                    //GENERATE
                    //(
                    //		{ 
                    //		  EXCEPT
                    //			(
                    //				<Member_Unique_Name>.Children, 
                    //				{
                    //					<Excluded_Member_Unique_Name>,
                    //					<Excluded_Member_Unique_Name>,
                    //					<Excluded_Member_Unique_Name>	
                    //				}
                    //			)
                    //		}, 
                    //		DESCENDANTS
                    //		(
                    //			<Dimension_Unique_Name>.CURRENTMEMBER
                    //		)
                    //	)
                    //,<Member_Unique_Name>
                    //}

                    //�������� Set �� ��������, ������� ����� ���������
                    String toExclude = GetExcludedChildrenSet(membersInSet);

                    //������ ���� �� ������. �� ���� ����� ���������, �� ��������� ��� � ������ case SelectStates.Selected_With_Children:
                    if (toExclude == null || toExclude.Length <= 0)
                    {
                        selfSet = "AddCalculatedMembers(GENERATE({" +
                            this.UniqueName +
                            "}, DESCENDANTS(" + HierarchyUniqueName + ".CURRENTMEMBER)))";

                        //������� ������� � ������ ���������� ������������ �������
                        if (membersInSet != null)
                            membersInSet[this.UniqueName] = this;
                        break;
                    }

                    selfSet = "{GENERATE({EXCEPT(AddCalculatedMembers(" +
                        this.UniqueName + ".Children)," +
                        toExclude +
                        ")}, DESCENDANTS(" + HierarchyUniqueName + ".CURRENTMEMBER))," + this.UniqueName + "}";

                    childSet = GetChildrenSet(membersInSet);

                    //������� ������� � ������ ���������� ������������ �������
                    if (membersInSet != null)
                        membersInSet[this.UniqueName] = this;
                    break;
                case SelectStates.Selected_Self:
                    selfSet = this.UniqueName;
                    childSet = GetChildrenSet(membersInSet);

                    //������� ������� � ������ ���������� ������������ �������
                    if (membersInSet != null)
                        membersInSet[this.UniqueName] = this;
                    break;
                case SelectStates.Labeled_As_Parent:
                    childSet = GetChildrenSet(membersInSet);
                    break;
                case SelectStates.Not_Initialized:
                    break;
                case SelectStates.Not_Selected:
                    childSet = GetChildrenSet(membersInSet);

                    //������� ������� � ������ ���������� ������������ �������
                    if (membersInSet != null)
                        membersInSet[this.UniqueName] = this;

                    break;
            }


            if (String.IsNullOrEmpty(selfSet) && childSet.Count == 0)
                return null;

            ResultSet = selfSet;
            foreach (String obj in childSet)
            {
                if (!String.IsNullOrEmpty(ResultSet))
                {
                    ResultSet = ResultSet + ", ";
                }

                ResultSet = ResultSet + obj;
            }

            //��������� � �������� ������ ������ ����
            //���� selfSet �� null ��� childSet.Count > 1 ��� �������� �������� ����������� ������ ������
            if (!String.IsNullOrEmpty(selfSet) || childSet.Count > 1)
                ResultSet = "{" + ResultSet + "}";

            return ResultSet;
        }

        #region ������ � ��������� OlapMemberInfo

        /// <summary>
        /// ������������� ��������� ��� ��������.
        /// </summary>
        /// <param name="newState">����� ���������</param>
        /// <returns>true - ��������� ����������, false - ��������� �� ����������</returns>
        private bool SetState(SelectStates newState)
        {
            if (SelectState != newState)
            {
                m_SelectState = newState;
                Raise_StateChanged();
                return true;
            }
            return false;
        }

        /// <summary>
        /// ��������� ������ OlapMemberInfo � ����� ���������
        /// </summary>
        public void SetNewState(SelectStates newState)
        {

            if (newState == SelectStates.Selected_Self && AllChildrenIsSelected)
            {
                //������������� � ����� ���������
                SetState(SelectStates.Selected_With_Children);
            }
            else
            {
                //������������� � ����� ���������
                SetState(newState);
            }

            //�������� �������� ����� � ���, ��� ������������ ������� ������� ���� ���������
            Raise_ParentSelectStateChanged(this);

            //�������� ������������� ���� � ���, ��� �������� ������� ������� ���� ���������
            Raise_ChildSelectStateChanged(this);

            Raise_HierarchyStateChanged(this);
        }

        /// <summary>
        /// ������������� ����� ��������� � ����� - �� ������->������ ���->������ � ���������
        /// </summary>
        public void SetNextState()
        {
            switch (SelectState)
            {
                case SelectStates.Not_Selected:
                case SelectStates.Not_Initialized:
                case SelectStates.Labeled_As_Parent:
                    SetNewState(SelectStates.Selected_Self);
                    break;
                case SelectStates.Selected_Self:
                case SelectStates.Selected_By_Parent:
                    if (this.HasChildren)
                        SetNewState(SelectStates.Selected_With_Children);
                    else
                        SetNewState(SelectStates.Not_Selected);
                    break;
                case SelectStates.Selected_With_Children:
                case SelectStates.Selected_By_Parent_With_Children:
                case SelectStates.Selected_With_Children_Has_Excluded:
                case SelectStates.Selected_By_Parent_With_Children_Has_Excluded:
                    SetNewState(SelectStates.Not_Selected);
                    break;
            }
        }

        /// <summary>
        /// ������������� ��������� �������� � ������������ � ��������� �������������
        /// </summary>
        private bool SetStateByParent()
        {
            if (Parent == null)
                return false;

            bool changed = false;

            switch (Parent.SelectState)
            {
                case SelectStates.Not_Initialized:
                    changed = SetState(SelectStates.Not_Initialized);
                    break;
                case SelectStates.Not_Selected:
                    changed = SetState(SelectStates.Not_Initialized);
                    break;
                case SelectStates.Selected_Self:
                    changed = SetState(SelectStates.Not_Initialized);
                    break;
                case SelectStates.Selected_With_Children:
                    if (HasChildren)
                        changed = SetState(SelectStates.Selected_By_Parent_With_Children);
                    else
                        changed = SetState(SelectStates.Selected_By_Parent);
                    break;
                case SelectStates.Selected_By_Parent:
                    changed = SetState(SelectStates.Not_Initialized);
                    break;
                case SelectStates.Selected_By_Parent_With_Children:
                    if (HasChildren)
                        changed = SetState(SelectStates.Selected_By_Parent_With_Children);
                    else
                        changed = SetState(SelectStates.Selected_By_Parent);
                    break;
                case SelectStates.Labeled_As_Parent:
                    changed = SetState(SelectStates.Not_Initialized);
                    break;
                case SelectStates.Selected_With_Children_Has_Excluded:
                    if (HasChildren)
                        changed = SetState(SelectStates.Selected_By_Parent_With_Children);
                    else
                        changed = SetState(SelectStates.Selected_By_Parent);
                    break;
                case SelectStates.Selected_By_Parent_With_Children_Has_Excluded:
                    if (HasChildren)
                        changed = SetState(SelectStates.Selected_By_Parent_With_Children);
                    else
                        changed = SetState(SelectStates.Selected_By_Parent);
                    break;
            }

            return changed;
        }

        #endregion ������ � ��������� OlapMemberInfo

        #region ������ � ���������� �������� ���������
        /// <summary>
        /// ��������� ������� � ��������� �������� OlapMemberInfo
        /// </summary>
        /// <param name="memberInfo"></param>
        public void AddChild(OlapMemberInfo memberInfo)
        {
            if (Root != null && Root is RootOlapMemberInfo)
            {
                ((RootOlapMemberInfo)Root).AllMembers[memberInfo.UniqueName] = memberInfo;
            }

            Children[memberInfo.UniqueName] = memberInfo;
            memberInfo.SetParentOlapMemberInfo(this);
        }

        /// <summary>
        /// ���������� ������������ �������� ��������
        /// </summary>
        /// <param name="useRoot">true - � ������ RootOlapMemberInfo, false - ��� RootOlapMemberInfo</param>
        /// <returns></returns>
        public IList<OlapMemberInfo> GetAscendants(bool useRoot)
        {
            IList<OlapMemberInfo> ascendants = new List<OlapMemberInfo>();

            OlapMemberInfo parent = this;
            while (parent.Parent != null)
            {
                parent = parent.Parent;

                if (parent is RootOlapMemberInfo && useRoot == false)
                    continue;

                ascendants.Add(parent);
            }
            return ascendants;
        }

        public OlapMemberInfo GetChild(String uniqueName)
        {
            foreach (OlapMemberInfo mi in Children.Values)
            {
                if (uniqueName == mi.UniqueName)
                {
                    return mi;
                }
            }
            return null;
        }

        #endregion ������ � ���������� �������� ���������
    }
}
