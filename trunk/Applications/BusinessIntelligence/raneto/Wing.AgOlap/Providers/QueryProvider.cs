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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ranet.Olap.Core.Data;
using Ranet.Olap.Core.Metadata;
using Ranet.Olap.Core.Providers;
using Ranet.AgOlap.Controls;
using Ranet.Olap.Core;

namespace Ranet.AgOlap.Providers
{
    public class QueryProvider
    {
        public const String CHILDREN_COUNT_MEMBER = "[Measures].[-Special_ChildrenCount-]";

        public QueryProvider(){}

        public QueryProvider(String cube, String subCube, String hierarchy)
        {
            if (String.IsNullOrEmpty(cube) && String.IsNullOrEmpty(subCube))
                throw new ArgumentException("cube or subCube must bet initialized.");
            if (String.IsNullOrEmpty(hierarchy))
                throw new ArgumentNullException("hierarchy");

            Cube = cube;
            SubCube = subCube;
            HierarchyUniqueName = hierarchy;
        }

        bool m_UseCalculatedMembers = true;
        public bool UseCalculatedMembers
        {
            get { return m_UseCalculatedMembers; }
            set { m_UseCalculatedMembers = value; }
        }

        public String Cube = String.Empty;
        public String SubCube = String.Empty;
        String FromSet
        {
            get {
                String from_Set = String.Empty;
                if (String.IsNullOrEmpty(SubCube))
                    from_Set = OlapHelper.ConvertToQueryStyle(Cube);
                else
                    from_Set = OlapHelper.ConvertSubCubeToQueryStyle(SubCube);
                return from_Set;
            }
        }

        public String HierarchyUniqueName = String.Empty;

        private String GetSubset(String set, long begin, long count)
        {
            if (begin < 0 || count < 0)
            {
                if (UseCalculatedMembers)
                    return String.Format("AddCalculatedMembers({0})", set);
                else
                    return set;
            }
            else
            {
                set = set.Trim();
                if (!set.EndsWith("}") && !set.StartsWith("{"))
                    set = "{" + set + "}";

                if (UseCalculatedMembers)
                    return String.Format("Subset(AddCalculatedMembers({0}), {1}, {2})", set, begin.ToString(), count.ToString());
                else
                    return String.Format("Subset({0}, {1}, {2})", set, begin.ToString(), count.ToString());
            }
        }

        private String BuildSet_GetLevelMembers(String levelUniqueName, long begin, long count)
        {
            String Set = String.Empty;

            if (!String.IsNullOrEmpty(levelUniqueName))
            {
                String OriginSet;

                //Set для получения элементов, содержащихся на указанном уровне
                OriginSet = "{" + levelUniqueName + ".Members}";

                Set = GetSubset(OriginSet, begin, count);
            }

            return Set;
        }

        public String GetLevelMembers(String levelUniqueName, long begin, long count)
        {
            String Set = BuildSet_GetLevelMembers(levelUniqueName, begin, count);
            return LoadSet(Set, true);
        }

        private String BuildSet_GetHierarchyMembers(int levelIndex, long begin, long count)
        {
            String Set = String.Empty;

            if (!String.IsNullOrEmpty(HierarchyUniqueName))
            {
                String OriginSet;

                OriginSet = "{" + String.Format("{0}.Levels({1}).Members", HierarchyUniqueName, levelIndex) + "}";

                Set = GetSubset(OriginSet, begin, count);
            }
            return Set;
        }

        public String GetMembersCount(String levelUniqueName)
        {
            StringBuilder builder = new StringBuilder();

            // В случае, если в выражении FROM используется подкуб то вычисление количества дочерних методом:
            //  WITH MEMBER [Measures].[-Special_ChildrenCount-]
            //  AS 'Count(AddCalculatedMembers([Product].[Product Categories].CurrentMember.Children))' 
            //  срабатывает некорректно.
            // В данном случае чтобы получить число дочерних с учетом подкуба правильно делать так:
            //  WITH SET [-SpecialSet-] as [Product].[Product Categories].Members
            //  MEMBER [Measures].[-Special_ChildrenCount-] 
            //  AS Intersect([-SpecialSet-],[Product].[Product Categories].CurrentMember.Children).Count

            String SPECIAL_SET = "[-SpecialSet-]";
            String set_expression = String.Empty;
            if(UseCalculatedMembers)
                set_expression = String.Format("AddCalculatedMembers({0}.Members)", HierarchyUniqueName);
            else
                set_expression = "{" + String.Format("{0}.Members", HierarchyUniqueName) + "}";
            String children_expression = String.Empty;
            if(UseCalculatedMembers)
                children_expression = String.Format("AddCalculatedMembers({0}.CurrentMember.Children)", HierarchyUniqueName);
            else
                children_expression = "{" + String.Format("{0}.CurrentMember.Children", HierarchyUniqueName) + "}";

            builder.AppendFormat("WITH SET {0} as {1} ", SPECIAL_SET, set_expression);
            String countStr = String.Format("'Intersect({0}, {1}).Count'", SPECIAL_SET, children_expression);
            builder.AppendFormat(" MEMBER {0} AS {1} ", CHILDREN_COUNT_MEMBER, countStr);

            // СТАРЫЙ ВАРИАНТ - НЕ УЧИТЫВАЕТ ПОДКУБ
            //String countStr = String.Format("Count(AddCalculatedMembers({0}.Members))", levelUniqueName);
            //builder.AppendFormat("WITH MEMBER {0} AS '{1}' ", CHILDREN_COUNT_MEMBER, countStr);

            String members_expression = String.Empty;
            if (UseCalculatedMembers)
                members_expression = String.Format("AddCalculatedMembers({0}.Members)", levelUniqueName);
            else
                members_expression = "{" + String.Format("{0}.Members", levelUniqueName) + "}";

            builder.AppendFormat("Select {0} on 0, {1} on 1 from {2}", members_expression, CHILDREN_COUNT_MEMBER, FromSet);

            return builder.ToString();
        }

        public String GetMembersCount(int levelIndex)
        {
            StringBuilder builder = new StringBuilder();

            // В случае, если в выражении FROM используется подкуб то вычисление количества дочерних методом:
            //  WITH MEMBER [Measures].[-Special_ChildrenCount-]
            //  AS 'Count(AddCalculatedMembers([Product].[Product Categories].CurrentMember.Children))' 
            //  срабатывает некорректно.
            // В данном случае чтобы получить число дочерних с учетом подкуба правильно делать так:
            //  WITH SET [-SpecialSet-] as [Product].[Product Categories].Members
            //  MEMBER [Measures].[-Special_ChildrenCount-] 
            //  AS Intersect([-SpecialSet-],[Product].[Product Categories].CurrentMember.Children).Count

            String SPECIAL_SET = "[-SpecialSet-]";
            String set_expression = String.Empty;
            if (UseCalculatedMembers)
                set_expression = String.Format("AddCalculatedMembers({0}.Members)", HierarchyUniqueName);
            else
                set_expression = "{" + String.Format("{0}.Members", HierarchyUniqueName) + "}";
            String children_expression = String.Empty;
            if(UseCalculatedMembers)
                children_expression = String.Format("AddCalculatedMembers({0}.CurrentMember.Children)", HierarchyUniqueName);
            else
                children_expression = "{" + String.Format("{0}.CurrentMember.Children", HierarchyUniqueName) + "}";

            builder.AppendFormat("WITH SET {0} as {1} ", SPECIAL_SET, set_expression);
            String countStr = String.Format("'Intersect({0}, {1}).Count'", SPECIAL_SET, children_expression);
            builder.AppendFormat(" MEMBER {0} AS {1} ", CHILDREN_COUNT_MEMBER, countStr);

            // СТАРЫЙ ВАРИАНТ - НЕ УЧИТЫВАЕТ ПОДКУБ
            //String countStr = String.Format("Count(AddCalculatedMembers({0}.Levels({1}).Members))", hierarchyUniqueName, levelIndex);
            //builder.AppendFormat("WITH MEMBER {0} AS '{1}' ", CHILDREN_COUNT_MEMBER, countStr);

            String members_expression = String.Empty;
            if (UseCalculatedMembers)
                members_expression = String.Format("AddCalculatedMembers({0}.Levels({1}).Members)", HierarchyUniqueName, levelIndex);
            else
                members_expression = "{" + String.Format("{0}.Levels({1}).Members", HierarchyUniqueName, levelIndex) + "}";

            builder.AppendFormat("Select {0} on 0, {1} on 1 from {2}", members_expression, CHILDREN_COUNT_MEMBER, FromSet);

            return builder.ToString();
        }

        public String GetHierarchyMembers(int levelIndex, long begin, long count)
        {
            String Set = BuildSet_GetHierarchyMembers(levelIndex, begin, count);
            return LoadSet(Set, true);
        }

        //public List<MemberDataWrapper> GetAscendantsToHierarchyMembers(String cubeName, String hierarchyUniqueName, int levelIndex, String ancestorUniqueName)
        //{
        //    String Set = BuildSet_GetHierarchyMembers(cubeName, hierarchyUniqueName, levelIndex, ancestorUniqueName, -1, -1);
        //    return LoadAscendantsToSet(Set, cubeName, hierarchyUniqueName, ancestorUniqueName);
        //}

        public String GetChildrenMembers(String memberUniqueName, long begin, long count)
        {
            String Set = String.Empty;
            String OriginSet = "{" + String.Format("{0}.Children", memberUniqueName) + "}";

            Set = GetSubset(OriginSet, begin, count);

            return LoadSet(Set, true);
        }

        public String GetChildrenMembers(String memberUniqueName)
        {
            String Set = "{" + String.Format("{0}.Children", memberUniqueName) + "}";

            // Запрос для выполнения
            String query = "Select " + Set + " Dimension Properties KEY0, HIERARCHY_UNIQUE_NAME, PARENT_UNIQUE_NAME on 0, {} on 1 from " + FromSet;

            return query;
        }

        public String GetMembers(String set)
        {
            if (String.IsNullOrEmpty(set))
                return String.Empty;

            // Запрос для выполнения
            String query = "Select " + set + " Dimension Properties KEY0, HIERARCHY_UNIQUE_NAME, PARENT_UNIQUE_NAME on 0, {} on 1 from " + FromSet;

            return query;
        }

        public String GetMember(String memberUniqueName, List<LevelPropertyInfo> memberProperties)
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder members = new StringBuilder();

            members.Append("{");
            if (memberProperties != null && memberProperties.Count > 0)
            {
                // Формируем строку WITH MEMBER
                builder.Append("WITH ");
                int i = 0;
                List<String> used_properties = new List<String>();
                foreach (LevelPropertyInfo prop in memberProperties)
                {
                    if (!used_properties.Contains(prop.Name))
                    {
                        String memberName = String.Format("[Measures].[-{0}-]", prop.Name);
                        if (i > 0)
                            members.Append(", ");
                        members.Append(memberName);

                        builder.AppendFormat("MEMBER {0} AS {1} ", memberName, String.Format("'{0}.Level.Hierarchy.CurrentMember.Properties(\"{1}\", TYPED)'", memberUniqueName, prop.Name));
                        i++;
                        used_properties.Add(prop.Name);
                    }
                }
            }
            members.Append("}");

            // Select
            builder.AppendFormat("Select {0}  Dimension Properties KEY0, HIERARCHY_UNIQUE_NAME, PARENT_UNIQUE_NAME on 0, {1} on 1 from {2}", memberUniqueName, members, FromSet);

            return builder.ToString();
        }

        /// <summary>
        /// ворзвращает всех предков элемента и сам элемент
        /// </summary>
        public String GetAscendants(String startLevelUniqueName, String uniqueName)
        {
            //Запрос - который ворзвращает всех предков
            string NewSet;

            if (!String.IsNullOrEmpty(startLevelUniqueName))
            {
                //Hierarchize(
                //INTERSECT(
                //Ascendants(<uniqueName>)
                //, GENERATE(<уровень>.Members, Descendants(<иерархия>.CURRENTMEMBER))
                //)
                //)

                //Пересекаем предков этого элемента, с дочерними элементов верхнего уровня ДЕРЕВА
                NewSet = "Hierarchize(INTERSECT(Ascendants(" + uniqueName + "), GENERATE(" +
                    startLevelUniqueName + ".Members, Descendants(" + HierarchyUniqueName + ".CURRENTMEMBER))))";
            }
            else
            {
                NewSet = "Hierarchize(Ascendants(" + uniqueName + "))";
            }

            return LoadSet(NewSet, false);
        }

        public virtual String SearchMembers(String startLevelUniqueName, FilterOperationBase filter)
        {
            //Если Уровень, с которого нужно тображать не задан, то поиск по всей иерархии измерения
            //Запрос для поиска будет иметь следующий вид: 
            //HIERARCHIZE(
            //FILTER([Customers].ALLMEMBERS, InStr([Customers].CURRENTMEMBER.NAME, "Bikes") > 0))

            //Если Уровень, с которого нужно тображать задан, то поиск только начиная с этого уровня
            //HIERARCHIZE(
            //FILTER(
            //GENERATE(StartLevelUniqueName.ALLMEMBERS, DESCENDANTS([Customers].CURRENTMEMBER)), InStr([Customers].CURRENTMEMBER.NAME, "Bikes") > 0))


            //Set для поиска
            String FindSet = String.Empty;

            String filterSet = BuildFilterCondition(filter);
            if (!String.IsNullOrEmpty(filterSet))
            {
                if (!String.IsNullOrEmpty(startLevelUniqueName))
                {
                    //Поиск начиная с уровня					
                    FindSet = "HIERARCHIZE(FILTER(GENERATE(" +
                        startLevelUniqueName + ".ALLMEMBERS, DESCENDANTS( " +
                        HierarchyUniqueName + ".CURRENTMEMBER)), " + filterSet + "))";
                }
                else
                {
                    //Поиск по всей иерархии измерения
                    FindSet = "HIERARCHIZE(FILTER(" +
                        HierarchyUniqueName +
                        ".ALLMEMBERS, " + filterSet + "))";
                }
            }
            else
            {
                if (!String.IsNullOrEmpty(startLevelUniqueName))
                {
                    FindSet = "HIERARCHIZE(GENERATE(" +
                        startLevelUniqueName + ".ALLMEMBERS, DESCENDANTS( " +
                        HierarchyUniqueName + ".CURRENTMEMBER)))";
                }
                else
                {
                    //Поиск по всей иерархии измерения
                    FindSet = "HIERARCHIZE(" +
                        HierarchyUniqueName +
                        ".ALLMEMBERS)";
                }
            }

            return LoadSet(FindSet, true);
        }

        String BuildFilterCondition(FilterOperationBase filter)
        {
            StringBuilder builder = new StringBuilder();
            if (filter != null)
            {
                // Опрерация
                FilterOperation operation = filter as FilterOperation;
                if (operation != null)
                {
                    List<String> operands = new List<string>();
                    foreach (FilterOperationBase child in operation.Children)
                    {
                        String x = BuildFilterCondition(child);
                        if (!String.IsNullOrEmpty(x))
                            operands.Add(String.Format("({0})", x));
                    }

                    if (operands.Count > 1)
                        builder.Append("(");
                    for (int i = 0; i < operands.Count; i++)
                    {
                        if (i > 0)
                            builder.AppendFormat(" {0} ", operation.Operation.ToString());
                        builder.Append(operands[i]);
                    }
                    if (operands.Count > 1)
                        builder.Append(")");
                }

                FilterOperand operand = filter as FilterOperand;
                if (operand != null)
                {
                    // Если свойство не является общим для всех уровней - (общие: Caption, UniqueName, Name)
                    // То при формировании нужно обязательно проверять чтобы текущий уровень был именно тем, на котором это свойство есть
                    // Исключение составляют случаи когда измерение Parent-Child
                    bool isParentChild = false;
                    // XXX 
                    //try
                    //{
                    //    OlapMetadataProvider provider = new OlapMetadataProvider(QueryExecuter.Connection);
                    //    HierarchyInfo hierarchy = provider.GetHierarchy(cubeName, String.Empty, hierarchyUniqueName);
                    //    if(hierarchy != null && hierarchy.HierarchyOrigin == HierarchyInfoOrigin.ParentChildHierarchy)
                    //    {
                    //        isParentChild = true;
                    //    }
                    //}
                    //catch { }

                    // Свойство с одним и тем же именем может присутствовать на нескольких уровнях. В данном случае operand.PropertyLevels содержит коллекцию уникальных имен уровней. Их все нужно учесть
                    if (operand.PropertyLevels.Count != 0 && !isParentChild)
                    {
                        int i = 0;
                        builder.Append("(");
                        foreach (String levelUniqueName in operand.PropertyLevels)
                        {
                            if (i > 0)
                                builder.Append("or ");
                            builder.AppendFormat("({0}.Level is {1}) ", HierarchyUniqueName, levelUniqueName);
                            i++;
                        }
                        builder.Append(")");
                        builder.Append(" and ");
                    }
                    switch (operand.Condition)
                    {
                        case ConditionTypes.Equal:
                            builder.AppendFormat("{0}.Properties(\"{1}\")='{2}'", HierarchyUniqueName, operand.Property, operand.Value);
                            break;
                        case ConditionTypes.Contains:
                            builder.AppendFormat("InStr({0}.Properties(\"{1}\"),'{2}') <> 0", HierarchyUniqueName, operand.Property, operand.Value);
                            break;
                        case ConditionTypes.BeginWith:
                            //Позиция при использовании InStr начинается с 1
                            builder.AppendFormat("InStr({0}.Properties(\"{1}\"),'{2}') = 1", HierarchyUniqueName, operand.Property, operand.Value);
                            break;
                    }
                }
            }
            return builder.ToString();
        }

        /// <summary>
        /// ворзвращает всех предков элементов Set, и сам Set
        /// </summary>
        /// <param name="set"></param>
        /// <param name="cubeName"></param>
        /// <param name="hierarchyUniqueName"></param>
        /// <param name="ancestorUniqueName"></param>
        /// <returns></returns>
        public String LoadSetWithAscendants(String set)
        {
            if (String.IsNullOrEmpty(set))
                throw new ArgumentNullException("set");

            String tmp = set.Trim();
            if (tmp[0] != '{' && tmp[tmp.Length - 1] != '}')
            {
                tmp = "{" + tmp + "}";
            }

            //Запрос - который ворзвращает всех предков элементов Set, и сам Set
            string NewSet =
                //                "Hierarchize(" +
                "Generate(" +
                tmp + "," +
                "Ascendants(" + HierarchyUniqueName + ".CurrentMember" + ")" +
                //                ")" +
                ")";

            return LoadSet(NewSet, true);
        }

        ///// <summary>
        ///// ворзвращает всех предков элементов Set
        ///// </summary>
        ///// <param name="set"></param>
        ///// <param name="cubeName"></param>
        ///// <param name="hierarchyUniqueName"></param>
        ///// <param name="ancestorUniqueName"></param>
        ///// <param name="UseHierarchize"></param>
        ///// <returns></returns>
        //public List<MemberDataWrapper> LoadAscendantsToSet(String set, String cubeName, String hierarchyUniqueName, String ancestorUniqueName)
        //{
        //    if (String.IsNullOrEmpty(set))
        //        throw new ArgumentNullException("set");

        //    String tmp = "{" + set + "}";

        //    //Запрос - который ворзвращает всех предков элементов Set, и сам Set
        //    string NewSet =
        //        "Except(" +
        //        "Generate(" +
        //        tmp + "," +
        //        "Ascendants(" + hierarchyUniqueName + ".CurrentMember" + ")" +
        //        ")" +
        //        ", " + tmp + ")";

        //    return LoadSet(NewSet, cubeName, hierarchyUniqueName, ancestorUniqueName, true);
        //}

        /// <summary>
        /// Выполняет запрос к кубу для получения данных указанного Set
        /// </summary>
        /// <param name="set">Set который нужно получить</param>
        /// <param name="useHierarchize">Использовать Hierarchize</param>
        /// <returns>CellSetData с результатом запроса</returns>
        internal String LoadSet(String set, bool useHierarchize)
        {
            if (String.IsNullOrEmpty(set))
                throw new ArgumentNullException("set");

            // Запрос для выполнения
            StringBuilder builder = new StringBuilder();

            if (String.IsNullOrEmpty(HierarchyUniqueName))
                throw new Exception("HierarchyUniqueName not initilized");
            if (String.IsNullOrEmpty(FromSet))
                throw new Exception("Cube and SubCube not initilized");

            String members = "{" + CHILDREN_COUNT_MEMBER;

            // В случае, если в выражении FROM используется подкуб то вычисление количества дочерних методом:
            //  WITH MEMBER [Measures].[-Special_ChildrenCount-]
            //  AS 'Count(AddCalculatedMembers([Product].[Product Categories].CurrentMember.Children))' 
            //  срабатывает некорректно.
            // В данном случае чтобы получить число дочерних с учетом подкуба правильно делать так:
            //  WITH SET [-SpecialSet-] as [Product].[Product Categories].Members
            //  MEMBER [Measures].[-Special_ChildrenCount-] 
            //  AS Intersect([-SpecialSet-],[Product].[Product Categories].CurrentMember.Children).Count

            String SPECIAL_SET = "[-SpecialSet-]";
            String set_expression = String.Empty;
            if(UseCalculatedMembers)
                set_expression = String.Format("AddCalculatedMembers({0}.Members)", HierarchyUniqueName);
            else
                set_expression = "{" + String.Format("{0}.Members", HierarchyUniqueName) + "}";
            String children_expression = String.Empty;
            if(UseCalculatedMembers)
                children_expression = String.Format("AddCalculatedMembers({0}.CurrentMember.Children)", HierarchyUniqueName);
            else
                children_expression = "{" + String.Format("{0}.CurrentMember.Children", HierarchyUniqueName) + "}";
            
            builder.AppendFormat("WITH SET {0} as {1} ", SPECIAL_SET, set_expression);
            String countStr = String.Format("'Intersect({0}, {1}).Count'", SPECIAL_SET, children_expression);
            builder.AppendFormat(" MEMBER {0} AS {1} ", CHILDREN_COUNT_MEMBER, countStr);

            // СТАРЫЙ ВАРИАНТ - НЕ УЧИТЫВАЕТ ПОДКУБ
            //String countStr = String.Format("'Count(AddCalculatedMembers({0}.CurrentMember.Children))' ", hierarchyUniqueName);
            //builder.AppendFormat("WITH MEMBER {0} AS {1} ", CHILDREN_COUNT_MEMBER, countStr);

            // Список свойств, которые будем получать дополнительно
            IList<String> memberProperties = new List<String>();
            memberProperties.Add("KEY0");
            memberProperties.Add("HIERARCHY_UNIQUE_NAME");
            memberProperties.Add("PARENT_UNIQUE_NAME");

            foreach (String propName in memberProperties)
            {
                String memberName = String.Format("[Measures].[-{0}-]", propName);
                members = members + "," + memberName;
                builder.AppendFormat("MEMBER {0} AS {1} ", memberName,
                                     String.Format("'{0}.CurrentMember.Properties(\"{1}\", TYPED)'", HierarchyUniqueName,
                                                   propName));
            }
            members = members + "}";

            // Select
            if (useHierarchize)
                builder.AppendFormat("Select Hierarchize({0})  Dimension Properties KEY0, HIERARCHY_UNIQUE_NAME, PARENT_UNIQUE_NAME on 0, {1} on 1 from {2}", set, members, FromSet);
            else
                builder.AppendFormat("Select {0}  Dimension Properties KEY0, HIERARCHY_UNIQUE_NAME, PARENT_UNIQUE_NAME on 0, {1} on 1 from {2}", set, members, FromSet);

            return builder.ToString();
        }

        public static List<MemberData> GetMembers(CellSetData cs_descr)
        {
            List<MemberData> result = new List<MemberData>();
            if (cs_descr != null)
            {
                // Сами элементы - по оси 0
                // По оси 1 - св-ва элементов
                if (cs_descr.Axes.Count > 0)
                {
                    int colIndex = 0;
                    foreach (PositionData posColumn in cs_descr.Axes[0].Positions)
                    {
                        if (posColumn.Members.Count > 0)
                        {
                            MemberData wrapper = cs_descr.Axes[0].Members[posColumn.Members[0].Id];

                            if (cs_descr.Axes.Count > 1)
                            {
                                int rowIndex = 0;
                                foreach (PositionData posRow in cs_descr.Axes[1].Positions)
                                {
                                    if (posRow.Members.Count > 0)
                                    {
                                        PropertyData prop = new PropertyData();
                                        prop.Name = cs_descr.Axes[1].Members[posRow.Members[0].Id].Caption;

                                        // Название свойства имеет специальный вид -IsDataMember-
                                        // В качестве названия будем использовать подстроку между символами "-"
                                        //String caption = posRow.Members[0].Caption;
                                        //if (caption.StartsWith("-") && caption.EndsWith("-"))
                                        //    caption = caption.Trim('-');

                                        // Если такого атрибута нет, то будет исключение
                                        CellData cell_descr = null;
                                        try
                                        {
                                            cell_descr = cs_descr.GetCellDescription(colIndex, rowIndex);
                                        }
                                        catch
                                        {
                                        }

                                        if (cell_descr != null && cell_descr.Value != CellValueData.Empty && !cell_descr.Value.IsError)
                                        {
                                            prop.Value = cell_descr.Value.Value;
                                            wrapper.MemberProperties.Add(prop);
                                        }
                                    }
                                    rowIndex++;
                                }
                            }

                            result.Add(wrapper);
                        }
                        colIndex++;
                    }
                }
            }
            return result;
        }

        public long GetCount(CellSetData cs_descr)
        {
            long result = 0;
            if (cs_descr != null)
            {
                // Сами элементы - по оси 0
                // По оси 1 - св-ва элементов
                if (cs_descr.Axes.Count > 0)
                {
                    result = cs_descr.Axes[0].Positions.Count;
                }
                //// Сами элементы - по оси 0
                //// По оси 1 - св-ва элементов
                //if (cs_descr.Axes.Count == 1 && cs_descr.Axes[0].Positions.Count == 1 && cs_descr.Cells.Count == 1)
                //{
                //    if (cs_descr.Cells[0] != null && cs_descr.Cells[0].Value != null && cs_descr.Cells[0].Value.Value != null)
                //    {
                //        try
                //        {
                //            result = Convert.ToInt64(cs_descr.Cells[0].Value.Value);
                //        }
                //        catch
                //        {
                //        }
                //    }
                //}
            }
            return result;
        }

        public const String CHILDREN_COUNT_PROPERTY = "-Special_ChildrenCount-";

        public static long GetRealChildrenCount(MemberData member)
        {
            //Количество дочерних узлов в кубе
            long childrenCount = 0;
            if (member != null)
            {
                PropertyData prop = member.GetMemberProperty(CHILDREN_COUNT_PROPERTY);
                if (prop != null)
                {
                    try
                    {
                        childrenCount = Convert.ToInt64(prop.Value);
                    }
                    catch (Exception)
                    {
                        childrenCount = 0;
                    }
                }
            }
            return childrenCount;
        }
    }
}
