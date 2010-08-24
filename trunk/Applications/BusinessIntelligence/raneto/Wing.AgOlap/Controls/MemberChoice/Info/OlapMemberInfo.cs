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
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using Ranet.Olap.Core.Data;
using Ranet.AgOlap.Providers;

namespace Ranet.AgOlap.Controls.MemberChoice.Info
{
    #region Возможные состояния
    //Любой элемент OlapMemberInfo в иерархии может находиться в одном из 3 состояний
    //	Не выбран
    //	Выбран сам
    //	Выбран с дочерними
    //
    //А уже какие иконки будут соответствовать этим состояниям у родительских элементов - это
    //задача дерева, в котором идет отображение
    /// <summary>
    /// Состояние элементов OlapMemberInfo
    /// </summary>
    public enum SelectStates
    {
        /// <summary>
        /// Не инизиализирован (начальное состояние)
        /// </summary>
        Not_Initialized,
        /// <summary>
        /// Не выбран
        /// </summary>
        Not_Selected,
        /// <summary>
        /// Выбран сам
        /// </summary>
        Selected_Self,
        /// <summary>
        /// Выбран с дочерними
        /// </summary>
        Selected_With_Children,
        /// <summary>
        /// Помечен как родитель
        /// </summary>
        Labeled_As_Parent,
        /// <summary>
        /// Выбран родителем
        /// </summary>
        Selected_By_Parent,
        /// <summary>
        /// Выбран родителем с дочерними
        /// </summary>
        Selected_By_Parent_With_Children,
        /// <summary>
        /// Выбран с дочерними, есть исключенные (Используется только в режиме Filter_Area)
        /// </summary>
        Selected_With_Children_Has_Excluded,
        /// <summary>
        /// Выбран родителем с дочерними, есть исключенные (Используется только в режиме Filter_Area)
        /// </summary>
        Selected_By_Parent_With_Children_Has_Excluded,
    }
    #endregion Возможные состояния

    /// <summary>
    /// Информация об узле дерева.
    /// </summary>
    public class OlapMemberInfo
    {
        #region Свойства OlapMemberInfo

        private MemberData m_Info = null;
        /// <summary>
        /// раппер для элемента измерения, соответствующий данному узлу дерева
        /// </summary>
        public MemberData Info
        {
            get
            {
                return m_Info;
            }
        }

        /// <summary>
        /// Коллекция дочерних OlapMemberInfo
        /// </summary>
        Dictionary<String, OlapMemberInfo> m_Children = null;

        /// <summary>
        /// Коллекция дочерних OlapMemberInfo. Ключ - уник. имя
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
        /// Текущее cостояние "выбранности" OlapMemberInfo
        /// </summary>
        private SelectStates m_SelectState = SelectStates.Not_Initialized;
        /// <summary>
        /// Текущее cостояние "выбранности" OlapMemberInfo
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
        #endregion Свойства OlapMemberInfo

        #region Свойства Member из CellSet

        /// <summary>
        /// Уникальное имя (member.UniqueName)
        /// </summary>
        public String UniqueName
        {
            get
            {
                return this.Info.UniqueName;
            }
        }

        /// <summary>
        /// Возвращает true если есть дочерние элементы, иначе - false
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
        #endregion Свойства Member из CellSet

        #region Конструкторы

        public OlapMemberInfo(MemberData info)
        {
            if (info == null)
                throw new ArgumentNullException("info");

            this.m_Info = info;

            this.cubeChildrenCount = QueryProvider.GetRealChildrenCount(info);
        }


        #endregion Конструкторы

        #region private Свойства
        /// <summary>
        /// Родительский элемент OlapMemberInfo
        /// </summary>
        private OlapMemberInfo m_Parent = null;
        /// <summary>
        /// Родительский элемент OlapMemberInfo
        /// </summary>
        public OlapMemberInfo Parent
        {
            get
            {
                return m_Parent;
            }
        }


        /// <summary>
        /// Родительский элемент OlapMemberInfo
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
        /// Возвращает true - элемент имеет выделенные дочерние  
        /// либо false - у элемента нет выделенных дочерних  
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

                //Если элемент "Выбран с дочерними", "Выбран с дочерними, есть исключенные",
                //"Выбран родителем с дочерними", "Выбран родителем с дочерними, есть исключенные"
                //то все его исключенные элементы уже должны быть загружены в иерархию. 
                //А вот элементы которые попадают в Set как дочерние (через генеренныую формулу) могут быть загружены не все в случае использования частичной загрузки
                //Поэтому если в перечисленных выше состояниях загружена из куба не вся информация, то считаем что у элемента есть выбранные дочерние
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
        /// Возвращает true - если все дочерние элементы выбраны (и кол-во выбранных равно реальному кол-ву мемберов): Листья - "Выбран сам", Не листья - "Выбран с дочерними"  
        /// иначе - false;  
        /// </summary>
        private bool AllChildrenIsSelected
        {
            get
            {
                if (Children != null)
                {
                    if (this.Info == null)
                        return false;

                    //Первым делом сверяем количество
                    if (CubeChildrenCount == 0 || LoadedChildrenCount != CubeChildrenCount)
                        return false;

                    foreach (OlapMemberInfo memberInfo in Children.Values)
                    {
                        //Если элемент - ЛИСТ, то состояние должно быть "Выбран сам"
                        //Если элемент - НЕ ЛИСТ, то состояние должно быть "Выбран с дочерними"
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
        /// Количество дочерних элементов в кубе
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
        /// Количество дочерних элементов, загруженных в иерархию
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
        /// Возвращает true если все дочерние элементы загружены в иерархию, инача - false
        /// </summary>
        private bool AllChildrenIsLoaded
        {
            get
            {
                //сверяем количество
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
        /// Возвращает true - элемент имеет дочерние, которые не выбраны;
        /// либо false - у элемента нет невыбранных дочерних
        /// </summary>
        public bool HasExcludedChildren
        {
            get
            {
                if (Children != null)
                {
                    if (this.Info == null)
                        return false;

                    //Первым делом сверяем количество. Если не все дочерние загружены, то считаем,что исключенные элементы есть (они не инициализированы)

                    //НО!!!!! Если элемент "Выбран с дочерними", "Выбран с дочерними, есть исключенные",
                    //"Выбран родителем с дочерними", "Выбран родителем с дочерними, есть исключенные"
                    //то все его исключенные элементы уже должны быть загружены в иерархию. 
                    //А вот элементы которые попадают в Set как дочерние (через генеренныую формулу) могут быть загружены не все в случае использования частичной загрузки
                    //Поэтому если в перечисленных выше состояниях загружена из куба не вся информация, то что незагруженные элементы выбраны
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
        #endregion private Свойства

        #region События и делегаты

        /// <summary>
        /// Обработчик события изменения состояния.
        /// </summary>
        public delegate void StateChangedEventHandler(OlapMemberInfo sender);
        /// <summary>
        /// Событие изменилось состояние 
        /// </summary>
        public event StateChangedEventHandler StateChanged;

        /// <summary>
        /// Событие изменилось состояние всей иерархии элементов
        /// </summary>
        public event StateChangedEventHandler HierarchyStateChanged;

        #endregion События и делегаты

        #region Генерация событий

        /// <summary>
        /// Генерирует событие - изменилось состояние всей иерархии элементов
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
        /// Генерирует событие - изменилось состояние
        /// </summary>
        private void Raise_StateChanged()
        {
            StateChangedEventHandler handler = this.StateChanged;
            if (handler != null)
            {
                handler(this);
            }
        }
        #endregion Генерация событий

        #region Сигнализация об изменении состояния для дочерних и родительских элементов
        /// <summary>
        /// Устанавливает дочерние элументы в указанное состояние
        /// </summary>
        /// <param name="state">Новое состояние</param>
        /// <param name="recursive">true - процесс идет рекурсивно; false - процесс идет пока меняется состояние</param>
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
        /// Сообщает дочерним элементам об изменении состояния текущего элемента
        /// </summary>
        /// <param name="changedMember">элемент, который самым первым изменил свое состояние</param>
        private void Raise_ParentSelectStateChanged(OlapMemberInfo changedMember)
        {
            //Отправляем это событие дальше дочерним, передаем дальше элемент, который изначально инициировал событие
            if (Children != null)
            {
                foreach (OlapMemberInfo memberInfo in Children.Values)
                {
                    memberInfo.ParentSelectStateChanged(this, changedMember);
                }
            }
        }

        /// <summary>
        /// Сообщает родительскому элементу об изменении состояния текущего элемента
        /// </summary>
        /// <param name="changedMember">элемент, который самым первым изменил свое состояние</param>
        private void Raise_ChildSelectStateChanged(OlapMemberInfo changedMember)
        {
            //Отправляем это событие родителю передаем дальше элемент, который изначально инициировал событие
            if (Parent != null)
            {
                Parent.ChildSelectStateChanged(this, changedMember);
            }
        }
        #endregion Сигнализация об изменении состояния для дочерних и родительских элементов

        #region Изменение состояния по инициативе родительских либо дочерних элементов
        /// <summary>
        /// Устанавливает дочерние элументы в указанное состояние
        /// </summary>
        /// <param name="state">Новое состояние</param>
        /// <param name="recursive">true - процесс идет рекурсивно; false - процесс идет пока меняется состояние</param>
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
        /// Функция вызывается родителем для дочерних для сигнализации о том что како-либо родительский OlapMemberInfo переведен в новое состояние
        /// </summary>
        /// <param name="changedMember">Элемент, который первым изменил состояние</param>
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
        /// Функция вызывается дочерним для родителя для сигнализации о том что како-либо дочерний OlapMemberInfo переведен в новое состояние
        /// </summary>
        /// <param name="changedMember">Элемент, который первым изменил состояние</param>
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
        #endregion Изменение состояния по инициативе родительских либо дочерних элементов

        /*		/// <summary>
				/// Возвращает коллекцию родительских элементов
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
        /// Возвращает ArrayList с Set-ами для дочерних элементов
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
        /// Возвращает Set из дочерних объектов, которые должны быть исключены
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

                            //Элемент заносим в список участников формирования запроса
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
        /// Генерирует Set для элемента с учетом дочерних
        /// </summary>
        /// <returns>null - элемент в Set не попадает</returns>
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

                    //Элемент заносим в список участников формирования запроса
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

                    //Получаем Set из дочерних, который нужно исключить
                    String toExclude = GetExcludedChildrenSet(membersInSet);

                    //Такого быть не должно. Но если вдруг произошло, то поступаем как в случае case SelectStates.Selected_With_Children:
                    if (toExclude == null || toExclude.Length <= 0)
                    {
                        selfSet = "AddCalculatedMembers(GENERATE({" +
                            this.UniqueName +
                            "}, DESCENDANTS(" + HierarchyUniqueName + ".CURRENTMEMBER)))";

                        //Элемент заносим в список участников формирования запроса
                        if (membersInSet != null)
                            membersInSet[this.UniqueName] = this;
                        break;
                    }

                    selfSet = "{GENERATE({EXCEPT(AddCalculatedMembers(" +
                        this.UniqueName + ".Children)," +
                        toExclude +
                        ")}, DESCENDANTS(" + HierarchyUniqueName + ".CURRENTMEMBER))," + this.UniqueName + "}";

                    childSet = GetChildrenSet(membersInSet);

                    //Элемент заносим в список участников формирования запроса
                    if (membersInSet != null)
                        membersInSet[this.UniqueName] = this;
                    break;
                case SelectStates.Selected_Self:
                    selfSet = this.UniqueName;
                    childSet = GetChildrenSet(membersInSet);

                    //Элемент заносим в список участников формирования запроса
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

                    //Элемент заносим в список участников формирования запроса
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

            //Заключаем в фигурные скобки только если
            //Если selfSet не null или childSet.Count > 1 Это позволит избежать расстановки лишних скобок
            if (!String.IsNullOrEmpty(selfSet) || childSet.Count > 1)
                ResultSet = "{" + ResultSet + "}";

            return ResultSet;
        }

        #region Работа с состояним OlapMemberInfo

        /// <summary>
        /// Устанавливает состояние для элемента.
        /// </summary>
        /// <param name="newState">Новое состояние</param>
        /// <returns>true - состояние изменилось, false - состояние не изменилось</returns>
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
        /// Переводит дынный OlapMemberInfo в новое состояние
        /// </summary>
        public void SetNewState(SelectStates newState)
        {

            if (newState == SelectStates.Selected_Self && AllChildrenIsSelected)
            {
                //Устанавливаем в новое состояние
                SetState(SelectStates.Selected_With_Children);
            }
            else
            {
                //Устанавливаем в новое состояние
                SetState(newState);
            }

            //Сообщаем дочерним узлам о том, что родительский элемент изменил свое состояние
            Raise_ParentSelectStateChanged(this);

            //Сообщаем родительскому узлу о том, что дочерний элемент изменил свое состояние
            Raise_ChildSelectStateChanged(this);

            Raise_HierarchyStateChanged(this);
        }

        /// <summary>
        /// Устанавливает новое состояние в цикле - Не выбран->Выбран сам->Выбран с дочерними
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
        /// Устанавливает состояние элемента в соответствии с сотоянием родительского
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

        #endregion Работа с состояним OlapMemberInfo

        #region Работа с коллекцией дочерних элементов
        /// <summary>
        /// Добавляет элемент в коллекцию дочерних OlapMemberInfo
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
        /// Возвращает родительские элементы иерархии
        /// </summary>
        /// <param name="useRoot">true - с учетом RootOlapMemberInfo, false - без RootOlapMemberInfo</param>
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

        #endregion Работа с коллекцией дочерних элементов
    }
}
