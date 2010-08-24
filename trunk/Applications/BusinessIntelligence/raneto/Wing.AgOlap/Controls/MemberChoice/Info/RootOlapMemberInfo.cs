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

namespace Ranet.AgOlap.Controls.MemberChoice.Info
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
            : base (member)
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
			SetChildrenState (SelectStates.Not_Initialized, true);
		}

        #region AddMemberToHierarchy - Добавление элемента в загруженную иерархию
        /// <summary>
        /// Добавляет элемент в иерархию MemberNodeInfo
        /// </summary>
        /// <param name="member">Member, информация о котором должна быть добавлена в иерархию</param>
        public OlapMemberInfo AddMemberToHierarchy(MemberData info)
        {
            //Задача: Добавить OlapMemberInfo в иерархию 
            //Возможные варианты:
            //	- если элемент в иерархии уже есть, то выход
            //	- если элемента в иерархии еще нет, то ищем его родителя.
            //	  Если Родитель задан, Но родителя в иерархии тоже нет, то добавляем родителя и ищем дальше следующего родителя рекурсивно
            //	  Если родитель не задан, то элемент является корневым (одним из корневых) в иерархии	
            //Таким образом в иерархию добавляется не только элемент, но и полностью ветка его родителей (если ее не было)

            //Ищем информацию о данном члене измерения в загруженной иерархии
            OlapMemberInfo memberInfo = FindMember(info.UniqueName);

            //Если элемент в иерархии не найден, то его нужно добавить
            if (memberInfo == null)
            {
                //Создаем OlapMemberInfo
                memberInfo = new OlapMemberInfo(info);
                // Подписываемся у каждого добавленного элемента на событие - изменение состояния всей иерархии
                // данное событие генерит элемент, послуживший инициатором рекурсивного обновления состояний в дереве
                memberInfo.HierarchyStateChanged += new StateChangedEventHandler(memberInfo_HierarchyStateChanged);

                //OlapMemberInfo, который будем использовать для формирования ветки ....->Дед->Отец->Изначально добавляемый мембер
                //Ветку будем формировать при поиске родительских элементов
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
                        // В случае если например в мерархию загружены элементы уровня 1 и потом догружается элемент All, 
                        // то получится что в коллекции корневых элементов находятся элементы уровней 0 и 1. Чтобы такого не произошло
                        // после добавления просматриваем коллекцию корневых элементов и проверяем не является данный добавленный элемент родителем для кого-нибудь из уже имеющихся
                        // Если дочерние обнаружены, то они удаляются из корневых и добавляются в дочерние к данному элементу.

                        // Ищем среди корневых элементы дочерние для данного
                        Dictionary<String, OlapMemberInfo> toDelete = new Dictionary<string, OlapMemberInfo>();
                        foreach(var child in Children.Values)
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

                        // Удаляем из коллекции корневых дочерние элементы
                        foreach (var del in toDelete)
                        {
                            Children.Remove(del.Key);
                        }

                        // Добавляем в коллекцию корневых элементов
                        AddChild(addedHierarchy);

                        // Добавляем в дочерние те элементы, которые были до этого корневыми
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

                        //если родитель найден, то добавляем ему нашу сформированную ветку в дочерние
                        if (parentOlapMemberInfo != null)
                        {
                            parentOlapMemberInfo.AddChild(addedHierarchy);

                            foundParentHierarchy = false;
                            break;
                        }
                        else
                        {
                            //Добавляем в коллекцию корневых элементов
                            AddChild(addedHierarchy);
                            foundParentHierarchy = false;
                            break;
                            //throw new Exception(String.Format("Родитель для элемента {0} не найден в иерархии", info.Member.UniqueName));

                            ////Создаем OlapMemberInfo для родителя
                            //parentOlapMemberInfo = new OlapMemberInfo(addedHierarchy.Member.Parent, Mode, addedHierarchy.Member.Parent.ChildCount);
                            ////Добавляем ненайденного родителя в ветку, которую нужно добавить в иерархию
                            //parentOlapMemberInfo.AddChildOlapMemberInfo(addedHierarchy);

                            ////В этом случае мы иерархию строим руками и нужно явно добавлять в коллекцию memberInfoHierarchy.AllMembers
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

        #endregion AddMemberToHierarchy - Добавление элемента в загруженную иерархию
	}
}
