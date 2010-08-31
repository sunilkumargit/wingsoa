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
using Ranet.Olap.Core.Metadata;
using Ranet.AgOlap.Controls.MdxDesigner.CalculatedMembers;

namespace Ranet.AgOlap.Controls.General
{
    public class MeasureGroupManager
    {
        public const String ROOT_GROUP = "<ROOT_GROUP>";
        /// <summary>
        /// Фиктивная группа. Предназначена для хранения папок, у которых группа не задана (корневые папки)
        /// </summary>
        public readonly GroupInfo RootGroup = new GroupInfo(ROOT_GROUP, "<All>");
        
        public MeasureGroupManager()
        {
            Groups.Add(RootGroup.Name, RootGroup); 
        }

        public void Initialize(CubeDefInfo cubeInfo)
        {
            Groups.Clear();
            Groups.Add(RootGroup.Name, RootGroup); 

            if (cubeInfo != null)
            {
                // Создаем описатели групп мер
                foreach (MeasureGroupInfo groupInfo in cubeInfo.MeasureGroups)
                {
                    if (!Groups.ContainsKey(groupInfo.Name))
                    {
                        Groups.Add(groupInfo.Name, new GroupInfo(groupInfo.Name, groupInfo.Caption));
                    }
                }

                foreach (MeasureInfo measureInfo in cubeInfo.Measures)
                {
                    String groupName = measureInfo.MeasureGroup;

                    // Если папка указана, то добавлем в иерархию (если она не привязана к группе, то она попадает в корневую коллекцию папкок)
                    // DisplayFolder может содержать иерархию папок, разделенную "\\"
                    if (!String.IsNullOrEmpty(measureInfo.DisplayFolder))
                    {
                        // Определяем группу, в которую входит данная папка (группы может и не быть)
                        GroupInfo groupInfo = null;
                        if (!String.IsNullOrEmpty(groupName) && Groups.ContainsKey(groupName))
                        {
                            groupInfo = Groups[groupName];
                        }

                        // Разбиваем полный путь на составляющие и создаем папку для каждой из них
                        String[] folders_names = measureInfo.DisplayFolder.Split(new String[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
                        if (folders_names != null)
                        {
                            String folderName = String.Empty;
                            FolderInfo prev_folderInfo = null;
                            foreach (String folderCaption in folders_names)
                            {
                                // Создаем узел для группы
                                if (!String.IsNullOrEmpty(folderCaption))
                                {
                                    // Получаем имя папки для данного уровня
                                    if (!String.IsNullOrEmpty(folderName))
                                    {
                                        folderName += "\\";
                                    }
                                    folderName += folderCaption;

                                    // Пытаемся найти папку с данным именем. Если она не найдена, то создаем ее
                                    FolderInfo folderInfo = GetFolder(groupInfo != null ? groupInfo.Name : String.Empty, folderName);
                                    if (folderInfo == null)
                                    {
                                        // Создаем данную папку
                                        folderInfo = new FolderInfo(folderName, folderCaption);

                                        // Если предыдущая папка в иерархии известна, то добавляем в нее
                                        if (prev_folderInfo != null)
                                        {
                                            prev_folderInfo.Folders.Add(folderName, folderInfo);
                                        }
                                        else
                                        {
                                            // Если группа известна то добавляем в группу в коллекцию корневых, иначе просто в коллекцию корневых папок
                                            if (groupInfo != null)
                                                groupInfo.Folders.Add(folderName, folderInfo);
                                            else
                                                RootGroup.Folders.Add(folderName, folderInfo);
                                        }
                                    }

                                    prev_folderInfo = folderInfo;
                                }
                            }
                        }
                    }
                }
            }
        }

        public void RefreshCustomInfo(List<CalculationInfoBase> calculatedMembers)
        {
            DeleteCustomItems();

            if(calculatedMembers != null)
            {
                foreach (var info in calculatedMembers)
                {
                    CalcMemberInfo memberInfo = info as CalcMemberInfo;
                    if (memberInfo != null)
                    {
                        // Если для элемента указана папка, то нужно проверить ее наличие
                        // Для этого, если задана группа мер, то нужно ее попытаться найти. Если группа мер по каким-то причинам уже не существует, то папку будем добавлять в корень
                        if (!String.IsNullOrEmpty(memberInfo.DisplayFolder))
                        {
                            Dictionary<String, FolderInfo> folders = RootGroup.Folders;
                            if (!String.IsNullOrEmpty(memberInfo.MeasureGroup))
                            {
                                GroupInfo groupInfo = GetGroup(memberInfo.MeasureGroup);
                                if (groupInfo != null)
                                {
                                    folders = groupInfo.Folders;
                                }
                            }
                            if (!folders.ContainsKey(memberInfo.DisplayFolder))
                            {
                                folders.Add(memberInfo.DisplayFolder, new FolderInfo(memberInfo.DisplayFolder, memberInfo.DisplayFolder, true));
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Возвращает группу по имени
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        internal GroupInfo GetGroup(String name)
        {
            if (!String.IsNullOrEmpty(name))
            {
                if (Groups.ContainsKey(name))
                {
                    return Groups[name];
                }
            }
            else
            {
                return RootGroup;
            }
            return null;
        }

        /// <summary>
        /// Возвращает папку по имени
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="folderName"></param>
        /// <returns></returns>
        internal FolderInfo GetFolder(String groupName, String folderName)
        {
            if (!String.IsNullOrEmpty(folderName))
            {
                // Определяем коллекцию папок, в которой надо искать
                Dictionary<String, FolderInfo> folders = null;
                if (!String.IsNullOrEmpty(groupName))
                {
                    GroupInfo group = GetGroup(groupName);
                    if (group != null)
                    {
                        folders = group.Folders;
                    }
                }
                else
                {
                    folders = RootGroup.Folders;
                }

                if (folders != null)
                {
                    foreach (FolderInfo folderInfo in folders.Values)
                    {
                        if (folderInfo.Name == folderName)
                            return folderInfo;

                        FolderInfo ret = folderInfo.GetFolder(folderName);
                        if (ret != null)
                            return ret;
                    }
                }
            }
            return null;
        }
        


        Dictionary<String, GroupInfo> m_Groups;
        /// <summary>
        /// Список групп мер
        /// </summary>
        internal Dictionary<String, GroupInfo> Groups
        {
            get {
                if (m_Groups == null)
                {
                    m_Groups = new Dictionary<String, GroupInfo>();
                }
                return m_Groups;
            }
        }

        public Object UserData = null;

        public void ClearUserData(bool recursive)
        {
            UserData = null;
            if (recursive)
            {
                foreach (GroupInfo info in Groups.Values)
                {
                    info.ClearUserData(recursive);
                }
            }
        }

        public void DeleteCustomItems()
        {
            // Отбираем что нужно удалять
            List<GroupInfo> toDelete = new List<GroupInfo>();
            foreach (GroupInfo group in Groups.Values)
            {
                group.DeleteCustomItems();
            }
        }
    }

    public class FolderInfo
    {
        string m_Name = String.Empty;
        /// <summary>
        /// Имя папки
        /// </summary>
        public string Name
        {
            get { return m_Name; }
        }

        string m_Caption = String.Empty;
        /// <summary>
        /// Название папки
        /// </summary>
        public string Caption
        {
            get { return m_Caption; }
        }

        bool m_IsCustom = false;
        /// <summary>
        /// Признак определяющий, является ли папка пользовательской
        /// </summary>
        public bool IsCustom
        {
            get { return m_IsCustom; }
        }

        public FolderInfo(String name, String caption)
        {
            m_Name = name;
            m_Caption = caption;
        }

        public FolderInfo(String name, String caption, bool isCustom)
            : this(name, caption)
        {
            m_IsCustom = isCustom;
        }

        /// <summary>
        /// Для хранения пользовательских данных
        /// </summary>
        public object UserData = null;

        /// <summary>
        /// Список вложенных папок
        /// </summary>
        Dictionary<String, FolderInfo> m_Folders;
        public Dictionary<String, FolderInfo> Folders
        {
            get
            {
                if (m_Folders == null)
                {
                    m_Folders = new Dictionary<String, FolderInfo>();
                }
                return m_Folders;
            }
        }

        /// <summary>
        /// Ищет папку по имени
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public FolderInfo GetFolder(String name)
        {
            foreach (FolderInfo info in Folders.Values)
            {
                if (info.Name == name)
                    return info;
                // Ищем во вложенных папках
                FolderInfo ret = info.GetFolder(name);
                if (ret != null)
                    return ret;
            }
            return null;
        }

        public void ClearUserData(bool recursive)
        {
            UserData = null;
            if (recursive)
            {
                foreach (FolderInfo info in Folders.Values)
                {
                    info.ClearUserData(recursive);
                }
            }
        }

        public void RefreshCustomInfo(List<CalculationInfoBase> calculatedMembers)
        { 
            
        }

        public void DeleteCustomItems()
        {
            // Отбираем что нужно удалять
            List<FolderInfo> toDelete = new List<FolderInfo>();
            foreach (FolderInfo info in Folders.Values)
            {
                if (info.IsCustom)
                    toDelete.Add(info);
            }
            // Удаляем
            foreach (FolderInfo info in toDelete)
            {
                Folders.Remove(info.Name);
            }

            // Идем вглубь
            foreach (FolderInfo info in toDelete)
            {
                info.DeleteCustomItems();
            }
        }
    }

    public class GroupInfo
    {
        /// <summary>
        /// Имя группы мер
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Заголовок группы мер
        /// </summary>
        public string Caption { get; private set; }

        public GroupInfo(String name, String caption)
        {
            Name = name;
            Caption = caption;
        }

        /// <summary>
        /// Список папок
        /// </summary>
        Dictionary<String, FolderInfo> m_Folders;
        public Dictionary<String, FolderInfo> Folders
        {
            get
            {
                if (m_Folders == null)
                {
                    m_Folders = new Dictionary<String, FolderInfo>();
                }
                return m_Folders;
            }
        }

        /// <summary>
        /// Для хранения пользовательских данных
        /// </summary>
        public object UserData = null;

        public void ClearUserData(bool recursive)
        {
            UserData = null;
            if (recursive)
            {
                foreach (FolderInfo info in Folders.Values)
                {
                    info.ClearUserData(recursive);
                }
            }
        }

        public void DeleteCustomItems()
        {
            // Отбираем что нужно удалять
            List<FolderInfo> toDelete = new List<FolderInfo>();
            foreach (FolderInfo info in Folders.Values)
            {
                if (info.IsCustom)
                    toDelete.Add(info);
            }
            // Удаляем
            foreach (FolderInfo info in toDelete)
            {
                Folders.Remove(info.Name);
            }
            
            // Идем вглубь
            foreach (FolderInfo info in toDelete)
            {
                info.DeleteCustomItems();
            }
        }
    }
}
