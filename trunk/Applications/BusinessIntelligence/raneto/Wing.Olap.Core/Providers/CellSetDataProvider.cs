/*   
    Copyright (C) 2009 Galaktika Corporation ZAO

    This file is a part of Ranet.UILibrary.Olap
 
    Ranet.UILibrary.Olap is a free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
      
    You should have received a copy of the GNU General Public License
    along with Ranet.UILibrary.Olap.  If not, see <http://www.gnu.org/licenses/>.
  
    If GPL v.3 is not suitable for your products or company,
    Galaktika Corp provides Ranet.UILibrary.Olap under a flexible commercial license
    designed to meet your specific usage and distribution requirements.
    If you have already obtained a commercial license from Galaktika Corp,
    you can use this file under those license terms.
*/

using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ranet.Olap.Core.Data;

namespace Ranet.Olap.Core.Providers
{
    public class CellSetDataProvider : IPivotGridDataProvider
    {

        public CellSetDataProvider(CellSetData cs_descr)
            : this(cs_descr, DataReorganizationTypes.MergeNeighbors)
        {
        }

        public CellSetDataProvider(CellSetData cs_descr, DataReorganizationTypes reorganizationType)
        {
            m_CellSet_Descr = cs_descr;
            DataReorganizationType = reorganizationType;
            m_Columns = CreateFields(0);
            m_Rows = CreateFields(1);
        }

        /// <summary>
        /// Тип реорганизации многомерных данных
        /// </summary>
        public readonly DataReorganizationTypes DataReorganizationType = DataReorganizationTypes.MergeNeighbors;

        //public CellData GetCellDescription(int col, int row)
        //{
        //    if (m_CellSet_Descr != null && col >= 0 && row >= 0)
        //    {
        //        foreach (CellData cell in m_CellSet_Descr.Cells)
        //        {
        //            if (cell.Axis0_Coord == col &&
        //                cell.Axis1_Coord == row)
        //            {
        //                return cell;
        //            }
        //        }
        //    }
        //    return null;
        //}

        Dictionary<String, SortDescriptor> m_RowsSortInfo;
        public Dictionary<String, SortDescriptor> RowsSortInfo
        {
            get
            {
                if (m_RowsSortInfo == null)
                    m_RowsSortInfo = new Dictionary<string, SortDescriptor>();
                 return m_RowsSortInfo;
            }
        }

        Dictionary<String, SortDescriptor> m_ColumnsSortInfo;
        public Dictionary<String, SortDescriptor> ColumnsSortInfo
        {
            get
            {
                if (m_ColumnsSortInfo == null)
                    m_ColumnsSortInfo = new Dictionary<string, SortDescriptor>();
                return m_ColumnsSortInfo;
            }
        }

        public void RotateSortInfo()
        {
            var x = m_RowsSortInfo;
            m_RowsSortInfo = m_ColumnsSortInfo;
            m_ColumnsSortInfo = x;
        }

        public void ClearSort()
        {
            ClearSort(0);
            ClearSort(1);
        }

        public void ClearSort(int axisNum)
        {
            if (axisNum == 0)
            {
                m_Columns_LowestMembers.Clear();
                m_Columns_Sorted_LowestMembers.Clear();
                // Чтобы сохранилась информация по какому свойству сортировали в последний раз
                foreach (var sort in ColumnsSortInfo.Values)
                {
                    sort.Type = SortTypes.None;
                }
                m_Columns = CreateFields(0);
            }

            if (axisNum == 1)
            {
                m_Rows_LowestMembers.Clear();
                m_Rows_Sorted_LowestMembers.Clear();
                // Чтобы сохранилась информация по какому свойству сортировали в последний раз
                foreach (var sort in RowsSortInfo.Values)
                {
                    sort.Type = SortTypes.None;
                }
                m_Rows = CreateFields(1);
            }
        }
        
        public void Sort(int axisNum, String hierarchyUniquqName, SortDescriptor sortDescr)
        {
            if (axisNum == 0 || axisNum == 1)
            {
                MemberInfoCollection members = axisNum == 0 ? Columns : Rows;
                Dictionary<int, MemberInfo> sorted_LowestMembers = axisNum == 0 ? m_Columns_Sorted_LowestMembers : m_Rows_Sorted_LowestMembers;
                Dictionary<String, SortDescriptor> sortInfo = axisNum == 0 ? ColumnsSortInfo : RowsSortInfo;

                // Если описатель для сортировки null, то применяем сортировку None
                if (sortDescr == null)
                    sortDescr = new SortDescriptor();

                // Коллекция подлежит сортировки если:
                //  тип сортировки - None
                //  либо точно задано свойство по которому идет сортировка
                bool need_Sort = sortDescr.Type == SortTypes.None || (sortDescr.Type != SortTypes.None && !String.IsNullOrEmpty(sortDescr.SortBy));

                if (need_Sort)
                {
                    Sort(members, hierarchyUniquqName, sortDescr);
                    // Формируем отсортированные индексы
                    sorted_LowestMembers.Clear();
                    BuildSortedIndexes(axisNum, members);
                }

                    // Если сортировка выполнялась, то информацию о ней сохраняем
                    // Сохраняем даже сортировки None - чтобы потом при клике на элементе было понятно по чем его сортировали в прошлый раз
                if (need_Sort)
                {
                    if (sortInfo.ContainsKey(hierarchyUniquqName))
                        sortInfo[hierarchyUniquqName] = sortDescr;
                    else
                        sortInfo.Add(hierarchyUniquqName, sortDescr);
                }
            }
        }

        void BuildSortedIndexes(int axisNum, MemberInfoCollection list)
        {
            foreach (var item in list)
            {
                if (item.Children.Count == 0)
                {
                    if (axisNum == 0)
                    {
                        int x = m_Columns_Sorted_LowestMembers.Count;
                        m_Columns_Sorted_LowestMembers.Add(x, item);
                        item.Sorted_MemberIndexInAxis = x;
                    }
                    if (axisNum == 1)
                    {
                        int x = m_Rows_Sorted_LowestMembers.Count;
                        m_Rows_Sorted_LowestMembers.Add(x, item);
                        item.Sorted_MemberIndexInAxis = x;
                    }

                    BuildSortedIndexes(axisNum, item.DrilledDownChildren);
                }
                else
                {
                    BuildSortedIndexes(axisNum, item.Children);
                    BuildSortedIndexes(axisNum, item.DrilledDownChildren);
                }
            }
        }

        void Sort(MemberInfoCollection list, String hierarchyUniquqName, SortDescriptor type)
        {
            // Сортируем коллекцию, если она содержит элементы, принадолежащие данной иерархии. В протипном случае идем вглубь в дочерние.
            if (list != null && list.Count > 0)
            {
                if (list[0].HierarchyUniqueName == hierarchyUniquqName)
                {
                    list.Sort(type);
                    // Сортируем так же элементы вложенных DrilledDown - коллекций
                    foreach (var member in list)
                    {
                        Sort(member.DrilledDownChildren, hierarchyUniquqName, type);
                    }
                }
                else
                {
                    // Сортируем вглубь
                    foreach (var member in list)
                    {
                        Sort(member.DrilledDownChildren, hierarchyUniquqName, type);
                    }

                    // Сортируем вглубь
                    foreach (var member in list)
                    {
                        Sort(member.Children, hierarchyUniquqName, type);
                    }
                }
            }
        }

        public int GetAxisCoord(int axisNum, int sorted_AxisCoord)
        {
            if (axisNum == 0 && m_Columns_Sorted_LowestMembers.ContainsKey(sorted_AxisCoord))
            {
                return m_Columns_Sorted_LowestMembers[sorted_AxisCoord].MemberIndexInAxis;
            }
            if (axisNum == 1 && m_Rows_Sorted_LowestMembers.ContainsKey(sorted_AxisCoord))
            {
                return m_Rows_Sorted_LowestMembers[sorted_AxisCoord].MemberIndexInAxis;
            } 
            return -1;
        }

        Dictionary<CellData, CellInfo> m_CellInfos = new Dictionary<CellData, CellInfo>();
        public CellInfo GetCellInfo(int column_index, int row_index)
        {
            CellInfo cell_Info = null;
            CellData cell_data = null;

            if (m_CellSet_Descr != null && 
                Rows != null && // чтобы проинициализировать
                Columns != null && // чтобы проинициализировать
                //column_index >= 0 &&
                column_index < m_Columns_LowestMembers.Count &&
                row_index < m_Rows_LowestMembers.Count) 
            {
                if(row_index >= 0)
                    cell_data = m_CellSet_Descr.GetCellDescription(column_index, row_index);
                else
                    cell_data = m_CellSet_Descr.GetCellDescription(column_index);
                
                if (cell_data != null)
                {
                    if (m_CellInfos.ContainsKey(cell_data))
                    {
                        cell_Info = m_CellInfos[cell_data];
                    }
                    else
                    {
                        if (row_index >= 0)
                        {
                            cell_Info = new CellInfo(cell_data,
                               column_index >= 0 ? m_Columns_LowestMembers[column_index] : MemberInfo.Empty,
                               m_Rows_LowestMembers[row_index], 
                               GetInvisibleCoords(column_index, row_index));
                        }
                        else
                        {
                            cell_Info = new CellInfo(cell_data,
                               column_index >= 0 ? m_Columns_LowestMembers[column_index] : MemberInfo.Empty,
                               MemberInfo.Empty, 
                               GetInvisibleCoords(column_index, row_index));
                        }
                        m_CellInfos.Add(cell_data, cell_Info);
                    }
                }
            }
            return cell_Info;
        }

        public CellInfo GetCellByTuple(IDictionary<String, MemberInfo> tuple)
        {
            if (tuple != null)
            {
                // Придется перебирать все ячейки
                for (int col_index = 0; col_index < Columns_Size; col_index++)
                {
                    for (int row_index = 0; row_index < Rows_Size; row_index++)
                    {
                        CellInfo info = GetCellInfo(col_index, row_index);
                        if (info != null && info.CompareByTuple(tuple))
                            return info;
                    }
                }
            }
            return null;
        }

        #region IPivotGridDataProvider Members

        private CellSetData m_CellSet_Descr = null;
        public CellSetData CellSet_Description
        {
            get {
                return m_CellSet_Descr;
            }
        }

        private Dictionary<string, object> m_Properties;
        public IDictionary<string, object> Properties
        {
            get
            {
                if (m_Properties == null)
                {
                    m_Properties = new Dictionary<string, object>();
                    m_Properties.Add("CONNECTION_STRING", "");
                }

                return m_Properties;
            }
        }

        private MemberInfoCollection m_Columns;
        public MemberInfoCollection Columns
        {
            get
            {
                if (m_Columns == null)
                {
                    m_Columns = new MemberInfoCollection(null);
                }

                return m_Columns;
            }
        }

        private MemberInfoCollection m_Rows;
        public MemberInfoCollection Rows
        {
            get
            {
                if (m_Rows == null)
                {
                    m_Rows = new MemberInfoCollection(null);
                }

                return m_Rows;
            }
        }

        Dictionary<int, MemberInfo> m_Columns_LowestMembers = new Dictionary<int, MemberInfo>();
        public Dictionary<int, MemberInfo> Columns_LowestMembers
        {
            get { return m_Columns_LowestMembers; }
        }

        Dictionary<int, MemberInfo> m_Columns_Sorted_LowestMembers = new Dictionary<int, MemberInfo>();
        public Dictionary<int, MemberInfo> Columns_Sorted_LowestMembers
        {
            get { return m_Columns_Sorted_LowestMembers; }
        }

        /// <summary>
        /// Количество элементов на последней линии в колонках
        /// </summary>
        public int Columns_Size
        {
            get {
                return m_Columns_LowestMembers.Count;
            }
        }

        Dictionary<int, MemberInfo> m_Rows_LowestMembers = new Dictionary<int, MemberInfo>();
        public Dictionary<int, MemberInfo> Rows_LowestMembers
        {
            get { return m_Rows_LowestMembers; }
        }
        
        Dictionary<int, MemberInfo> m_Rows_Sorted_LowestMembers = new Dictionary<int, MemberInfo>();
        public Dictionary<int, MemberInfo> Rows_Sorted_LowestMembers
        {
            get { return m_Rows_Sorted_LowestMembers; }
        }

        /// <summary>
        /// Количество элементов на последней линии в строках
        /// </summary>
        public int Rows_Size
        {
            get
            {
                return m_Rows_LowestMembers.Count;
            }
        }

        private MemberInfoCollection CreateFields(int axisNum)
        {
            if (axisNum == 0)
            {
                m_Columns_LowestMembers.Clear();
                m_Columns_Sorted_LowestMembers.Clear();
            }
            if (axisNum == 1)
            {
                m_Rows_LowestMembers.Clear();
                m_Rows_Sorted_LowestMembers.Clear();
            }

            MemberInfoCollection fields = new MemberInfoCollection(null);

            DateTime start = DateTime.Now;
            // Формируем иерархию элементов MemberInfo
            if (m_CellSet_Descr != null && m_CellSet_Descr.Axes.Count > axisNum)
            {
                int position_Index = 0;
                // Проход по позиция оси
                foreach (PositionData pos in m_CellSet_Descr.Axes[axisNum].Positions)
                {
                    // Глубина оси (кол-во элементов  в позиции)
                    int depth = pos.Members.Count;

                    // Предыдущий элемент в данной позиции
                    MemberInfo prev_member_in_position = null;

                    // Проход по элементам в каждой позици
                    for (int i = 0; i < depth; i++)
                    {
                        if (i > 0 && prev_member_in_position == null)
                            throw new Exception(String.Format("Ошибка построения иерархии элементов. Не найден описатель для элемента {0} в позиции {1}", i, position_Index));
                        // Получаем элемент из хранилища, которое является общим для всей оси
                        MemberData member = m_CellSet_Descr.Axes[axisNum].Members[pos.Members[i].Id];
                        if (member == null)
                        {
                            throw new Exception(String.Format("Ошибка построения иерархии элементов. На оси не найден элемент с Id: {0}", pos.Members[i].Id));
                        }

                        if (DataReorganizationType == DataReorganizationTypes.MergeNeighbors)
                        {
                            #region Правила
                            // Правила постоения иерархии:
                            //  1. Получаем для данного элемента PARENT_UNIQUE_NAME. 
                            //  2. Определяем КОЛЛЕКЦИЮ, в которую должен попасть элемент:
                            //      - Если это элемент нулевой линии, то ПО УМОЛЧАНИЮ он является кандидатом 
                            //        на добавление в коллекцию корневых MemberInfo
                            //      - Если это элемент не нулевой линии, то ПО УМОЛЧАНИЮ он явлется кандидатом 
                            //        на добавление в коллекцию Children для MemberInfo предыдущего элемента данной позиции (prev_member_in_position)
                            //  3. Берем последний элемент в КОЛЛЕКЦИИ. Проходим вглубь по его коллекции DrilledDownChildren и строим список из последних элементов каждой из них.
                            //     Проходим по полученному списку от самого глубокого вверх. Если находим элемент, который является родителем для текущего (UNIQUE_NAME совпадает c PARENT_UNIQUE_NAME текущего),
                            //     то КОЛЛЕКЦИЯ меняется на коллецию DrilledDownChildren родителя. 

                            // Правила объединения:
                            //  1. Объединению подлежат только следующие друг за другом одинаковые элементы. Но только в том случае, если коллекция DrilledDownChildren у предыдущего является пустой (в противном случае произойдет перемещение ячеек и может потеряться ORDER запроса)
                            //            - В следующем запросе года не должны объединяться чтобы не потерять очередность ячеек, предусмотренную запросом
                            //                     select [Measures].[Internet Sales Amount] DIMENSION PROPERTIES PARENT_UNIQUE_NAME , HIERARCHY_UNIQUE_NAME , CUSTOM_ROLLUP , UNARY_OPERATOR , KEY0 on 0,
                            //                    {([Date].[Calendar].[Calendar Year].&[2001],  [Product].[Product Categories].[Category].[Bikes]),
                            //                    ([Date].[Calendar].[Calendar Semester].&[2001]&[2], [Product].[Product Categories].[Subcategory].[Mountain Bikes]),
                            //                    ([Date].[Calendar].[Calendar Year].&[2001], [Product].[Product Categories].[Category].[Clothing])}
                            //                    DIMENSION PROPERTIES PARENT_UNIQUE_NAME , HIERARCHY_UNIQUE_NAME , CUSTOM_ROLLUP , UNARY_OPERATOR , KEY0 on 1
                            //                    from [Adventure Works]
                            //  2. Элементы последней линии (ближайшей к ячейкам) объединению не подлежат
                            #endregion Правила

                            #region Определение места элемента в иерархии
                            // Уникальное имя родителя (Правила постоения иерархии - пункт 1)
                            String parentUniqueName = GetMemberPropertyValue(member, "PARENT_UNIQUE_NAME");
                            // КОЛЛЕКЦИЯ (Правила постоения иерархии - пункт 2)
                            MemberInfoCollection container = i == 0 ? fields : prev_member_in_position.Children;
                            // Ищем родителя (Правила постоения иерархии - пункт 3)
                            if (!String.IsNullOrEmpty(parentUniqueName))
                            {
                                if (container.Count > 0)
                                {
                                    MemberInfo last = container[container.Count - 1];
                                    // Теперь строим коллекцию из последних в каждой коллекции DrilledDownChildren вглубь по всей ветке
                                    List<MemberInfo> dd_last_list = new List<MemberInfo>();
                                    dd_last_list.Add(last);
                                    while (last.DrilledDownChildren.Count > 0)
                                    {
                                        dd_last_list.Add(last.DrilledDownChildren[last.DrilledDownChildren.Count - 1]);
                                        last = last.DrilledDownChildren[last.DrilledDownChildren.Count - 1];
                                    }

                                    for (int x = dd_last_list.Count; x > 0; x--)
                                    {
                                        var info = dd_last_list[x - 1];
                                        if (info.UniqueName == parentUniqueName)
                                        {
                                            container = info.DrilledDownChildren;
                                            break;
                                        }
                                    }
                                }
                            }
                            #endregion

                            #region Объединение элементов
                            if (i != (depth - 1) && container.Count > 0 && container[container.Count - 1].UniqueName == member.UniqueName && container[container.Count - 1].DrilledDownChildren.Count == 0)
                            {
                                // Объединение элементов
                                prev_member_in_position = container[container.Count - 1];
                            }
                            else
                            {
                                // Создание нового элемента
                                prev_member_in_position = CreateMemberInfo(member);
                                container.Add(prev_member_in_position);
                            }

                            // Если элементы одной линии повторяются друг за другом, то свойство DrilledDown у них верное только у последнего
                            if (prev_member_in_position != null && !prev_member_in_position.IsCalculated)
                            {
                                prev_member_in_position.DrilledDown = prev_member_in_position.DrilledDown | pos.Members[i].DrilledDown;
                            }

                            #endregion
                        }

                        if (DataReorganizationType == DataReorganizationTypes.None)
                        {
                            #region Правила
                            // В данном режиме никакой реорганизации и объединения не производится
                            #endregion

                            #region Определение места элемента в иерархии
                            // КОЛЛЕКЦИЯ 
                            MemberInfoCollection container = i == 0 ? fields : prev_member_in_position.Children;
                            #endregion

                            #region Объединение элементов
                            // Создание нового элемента
                            prev_member_in_position = CreateMemberInfo(member);
                            container.Add(prev_member_in_position);
                            #endregion
                        }

                        if (DataReorganizationType == DataReorganizationTypes.LinkToParent)
                        {
                            #region Правила
                            // В данном режиме элементы выстраиваются в иерархию используя уникальное имя родителя. При этом в иерархию 
                            // выстраивается вся ось.
                            // Например было:
                            //      2002    a
                            //        Q1    b
                            //       H1     c
                            //      2002    d
                            //       H2     e
     
                            // Перегруппируется в:
                            //      2002    a
                            //      2002    d
                            //       H1     c
                            //        Q1    b
                            //       H2     e
                            
                            //  1. Собираем нервы в кулак.
                            //  2. Определяем КОЛЛЕКЦИЮ, в которую должен попасть элемент:
                            //      - Если это элемент нулевой линии, то ПО УМОЛЧАНИЮ он является кандидатом 
                            //        на добавление в коллекцию корневых MemberInfo
                            //      - Если это элемент не нулевой линии, то ПО УМОЛЧАНИЮ он явлется кандидатом 
                            //        на добавление в коллекцию Children для MemberInfo предыдущего элемента данной позиции (prev_member_in_position)
                            //  3. КОЛЛЕКЦИЮ выстраиваем в плоский список рекурсивно с учетом ТОЛЬКО DrilledDownChildren. 
                            //      Создаем ТЕКУЩИЙ ОПИСАТЕЛЬ (MemberInfo) для данного элемнта
                            //      Проходим с конца списка в начало:
                            //      a) Если находим элемент, который является дочерним для ТЕКУЩЕГО ОПИСАТЕЛЯ, то удаляем его из коллекции, в которой он находился и вставлем его в 0 позицию в коллекцию DriDrilledDownChildren ТЕКУЩЕГО ОПИСАТЕЛЯ
                            //          CONTINUE
                            //      b) Если находим элемент с таким же уник. именем, то: 
                            //          - если это последняя линия, то они объединению не подлежат. Коллекция DriDrilledDownChildren у двойника зануляется и ТЕКУЩИЙ ОПИСАТЕЛЬ добавляется следом за ним.
                            //          - если это не последняя линия то объекты объединяются, т.е. двойнику переходит коллекция DriDrilledDownChildren текущего описателя и он будет считаться далее как ТЕКУЩИЙ ОПИСАТЕЛЬ
                            //          CТОП ЦИКЛА
                            //      c) Если находим элемент, который является родителем для данного, то добавляем ТЕКУЩИЙ ОПИСАТЕЛЬ в коллекцию DrilledDownChildren родителя. 
                            //          СТОП ЦИКЛА
                            //      d) Если ни 3b) ни 3c) не отработало, то ТЕКУЩИЙ ОПИСАТЕЛЬ добавляется в КОЛЛЕКЦИЮ с учетом номера уровня. Чтобы не получилось что перед ним есть элементы с большим по глубине номером уровня.
                            //          НО если элемент вычисляемый, то уровень не учитываем, а добавляем его в конец КОЛЛЕКЦИИ.
                            //  
                            #endregion

                            #region Определение места элемента в иерархии
                            // Выполняем пункт 1 :)

                            // КОЛЛЕКЦИЯ (Правила постоения иерархии - пункт 2)
                            MemberInfoCollection container = i == 0 ? fields : prev_member_in_position.Children;
                            // Выстраиваем в плоский список рекурсивно с учетом ТОЛЬКО DrilledDownChildren
                            List<MemberInfo> line = new List<MemberInfo>();
                            foreach (var item in container)
                            {
                                line.Add(item);
                                line.AddRange(item.CollectDrilledDownChildren());
                            }
                            // Создание нового элемента
                            prev_member_in_position = CreateMemberInfo(member);

                            // Последний из обследуемых элементов КОЛЛЕКЦИИ, чья глубина больше чем у данного
                            MemberInfo reverse_last_leveldepth_member = null;
                            bool isOk = false;
                            for (int indx = line.Count - 1; indx >= 0; indx--)
                            {
                                var mi = line[indx];

                                if(mi.Container == container && mi.LevelDepth > member.LevelDepth)
                                    reverse_last_leveldepth_member = mi;

                                // 3a
                                if(mi.ParentUniqueName == member.UniqueName)
                                {
                                    // Нашли дочерний, цепляем его и тащим за собой
                                    mi.Container.Remove(mi);
                                    prev_member_in_position.DrilledDownChildren.Insert(0, mi);
                                    continue;
                                }

                                // 3b
                                if (mi.UniqueName == member.UniqueName)
                                {
                                    // Найден дубликат
                                    if (i != (depth - 1))
                                    {
                                        // Не последняя линия
                                        mi.DrilledDownChildren.Clear();
                                        foreach(var x in prev_member_in_position.DrilledDownChildren)
                                        {
                                            mi.DrilledDownChildren.Add(x);
                                        }
                                        prev_member_in_position = mi;
                                    }
                                    else
                                    { 
                                        // Последняя линия
                                        mi.DrilledDownChildren.Clear();
                                        mi.IsDublicate = true;
                                        // ДОБАВЛЕНИЕ В ИЕРАРХИЮ
                                        mi.Container.Insert(mi.Container.IndexOf(mi) + 1, prev_member_in_position);
                                    }
                                    isOk = true;
                                    break;
                                }

                                // 3c
                                if (mi.UniqueName == prev_member_in_position.ParentUniqueName)
                                {
                                    // Нашли родителя, цепляемся к нему
                                    // ДОБАВЛЕНИЕ В ИЕРАРХИЮ
                                    mi.DrilledDownChildren.Add(prev_member_in_position);
                                    isOk = true;
                                    break;
                                }
                            }

                            // 3d
                            // Более глубокий элемент уровня не найден, значит добавляем хвост КОЛЛЕКЦИИ
                            if(!isOk)
                            {
                                // Если Key0 == null то элемент вычисляемый
                                if (member.IsCalculated)
                                {
                                    // ДОБАВЛЕНИЕ В ИЕРАРХИЮ
                                    container.Add(prev_member_in_position);
                                }
                                else
                                {
                                    if (reverse_last_leveldepth_member != null && container.Contains(reverse_last_leveldepth_member))
                                    {
                                        // ДОБАВЛЕНИЕ В ИЕРАРХИЮ
                                        container.Insert(container.IndexOf(reverse_last_leveldepth_member), prev_member_in_position);
                                    }
                                    else
                                    {
                                        // ДОБАВЛЕНИЕ В ИЕРАРХИЮ
                                        container.Add(prev_member_in_position);
                                    }
                                }
                            }

                            #endregion
                        }

                        if (i == (depth - 1))
                        {
                            if (prev_member_in_position == null)
                                throw new Exception("Ошибка. Не создан элемент последней линии.");
                            if (axisNum == 0)
                            {
                                m_Columns_LowestMembers.Add(position_Index, prev_member_in_position);
                                m_Columns_Sorted_LowestMembers.Add(position_Index, prev_member_in_position);
                            }
                            if (axisNum == 1)
                            {
                                m_Rows_LowestMembers.Add(position_Index, prev_member_in_position);
                                m_Rows_Sorted_LowestMembers.Add(position_Index, prev_member_in_position);
                            }

                            prev_member_in_position.MemberIndexInAxis = prev_member_in_position.Sorted_MemberIndexInAxis = position_Index;
                        }

                    }
                    position_Index++;
                }


                if (DataReorganizationType == DataReorganizationTypes.LinkToParent)
                {
                    // Операции по вставке объектов поперепутали MemberOrder для элементов. Исправляем это.
                    // И заодно устанавливаем флаг DrilledDown в true только если у элемента не пустая коллекция DrilledDownChildren
                    foreach (var x in fields)
                    {
                        x.RefreshMemberOrder();
                        x.CrackDrilledDown();
                    }
                }
            }
            DateTime stop = DateTime.Now;

            return fields;
        }

        public int ReversePos(List<MemberInfo> members, String uniqueName)
        {
            if (members != null)
            {
                for (int i = members.Count - 1; i >= 0; i--)
                {
                    MemberInfo mi = members[i];
                    if (mi.UniqueName == uniqueName)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        private String GetMemberPropertyValue(MemberData member, String propName)
        {
            if (member == null || member.MemberProperties == null || String.IsNullOrEmpty(propName))
                return null;
            foreach(PropertyData prop in member.MemberProperties)
            if (prop.Name == propName && prop.Value != null)
            {
                return prop.Value.ToString();
            }
            return null;
        }

        private MemberInfo CreateInvisibleMemberInfo(MemberData member)
        {
            if (member == null)
                return null;

            MemberInfo info = new InvisibleMemberInfo();
            InitMemberInfo(info, member);

            return info;
        }    

        private MemberInfo CreateMemberInfo(MemberData member)
        {
            if (member == null)
                return null;

            MemberInfo info = new MemberInfo();
            InitMemberInfo(info, member);

            return info;
        }    
    
        private void InitMemberInfo(MemberInfo info, MemberData member)
        {
            if (info == null || member == null)
                return;

            info.Caption = member.Caption;
            info.ChildCount = member.ChildCount;
            info.Description = member.Description;
            info.DrilledDown = member.DrilledDown;
            info.LevelDepth = member.LevelDepth;
            info.LevelName = member.LevelName;
            info.Name = member.Name;
            info.ParentSameAsPrevious = member.ParentSameAsPrevious;

            info.HierarchyUniqueName = member.HierarchyUniqueName;
            info.Custom_Rollup = member.Custom_Rollup;
            info.Unary_Operator = member.Unary_Operator;

            info.UniqueName = member.UniqueName;
            info.ParentUniqueName = GetMemberPropertyValue(member, "PARENT_UNIQUE_NAME");

            // В коллекцию свойств добавляем Properties
            foreach (PropertyData pair in member.Properties)
            {
                String caption = pair.Name;
                if (!info.PropertiesDictionary.ContainsKey(caption))
                    info.PropertiesDictionary.Add(caption, pair.Value);
            }

            // В коллекцию свойств добавляем MemberProperties
            foreach (PropertyData pair in member.MemberProperties)
            {
                String caption = pair.Name;
                if (caption.StartsWith("-") && caption.EndsWith("-"))
                    caption = caption.Trim('-');

                if (!info.PropertiesDictionary.ContainsKey(caption))
                    info.PropertiesDictionary.Add(caption, pair.Value);
            }

            // Для вычисляемых элементов свойство DrilledDown работает неправильно. 
            // Признаком того, что это вычисляемый элемент считаем KEY0 == null. В этом случае сбрасываем DrilledDown
            if (info.PropertiesDictionary.ContainsKey(MemberInfo.KEY0_PROPERTY) &&
                info.PropertiesDictionary[MemberInfo.KEY0_PROPERTY] == null)
            {
                info.DrilledDown = false;                
            }
        }

        //private void CreateHeaders(IList<PivotAreaData> list, int axisIndex)
        //{
        //    if (m_CellSet.Axes.Count > axisIndex)
        //    {
        //        foreach (Hierarchy h in m_CellSet.Axes[axisIndex].Set.Hierarchies)
        //        {
        //            list.Add(new PivotAreaData(h.UniqueName, h.Caption));
        //        }
        //    }
        //}

        public CellValueInfo GetValue(params int[] index)
        {
            if (m_CellSet_Descr == null || index == null || index.Length < 2)
            {
                return CellValueInfo.Empty;
            }

            foreach (CellData cell_descr in m_CellSet_Descr.Cells)
            {
                if (cell_descr.Axis0_Coord == index[0] &&
                    cell_descr.Axis1_Coord == index[1])
                {
                    object value = null;
                    string displayName = string.Empty;

                    if (cell_descr.Value != null)
                    {
                        try
                        {
                            displayName = cell_descr.Value.DisplayValue;
                            value = cell_descr.Value.Value;
                        }
                        catch (Exception exc)
                        {
                            value = exc;
                        }
                    }

                    if (string.IsNullOrEmpty(displayName))
                    {
                        if (value == null)
                        {
                            displayName = String.Empty;
                        }
                        else
                        {
                            displayName = value.ToString();
                        }
                    }

                    CellValueInfo res = new CellValueInfo(value, displayName);
                    foreach (PropertyData prop in cell_descr.Value.Properties)
                    {
                        res.Properties.Add(prop.Name, prop.Value);
                    }
                    return res;
                }

            }

            return CellValueInfo.Empty;
        }

        public IList<MemberInfo> GetInvisibleCoords(params int[] index)
        {
            IList<MemberInfo> res = new List<MemberInfo>();
            if (m_CellSet_Descr == null)
            {
                return res;
            }

            for (int i = index.Length; i < m_CellSet_Descr.Axes.Count; i++)
            {
                if (m_CellSet_Descr.Axes[i].Positions.Count > 0 &&
                    m_CellSet_Descr.Axes[i].Positions[0].Members.Count > 0)
                {
                    MemberInfo member = this.CreateInvisibleMemberInfo(m_CellSet_Descr.Axes[i].Members[m_CellSet_Descr.Axes[i].Positions[0].Members[0].Id]);
                    res.Add(member);
                }
            }

            return res;
        }


        /*public IEnumerable<MemberInfo> GetFilterCoords()
        {
            if (m_CellSet == null)
            {
                return new MemberInfo[] { };
            }

            List<MemberInfo> res = new List<MemberInfo>();
            foreach(Tuple tuple in m_CellSet.FilterAxis.Set.Tuples)
            {
                if(tuple.Members.Count > 0)
                {
                    MemberInfo member = this.CreateMemberInfo(tuple.Members[0]);
                    res.Add(member);
                }
            }
            return res;
        }*/

        /*
        private PivotFieldCollection CreateFields()
        {
            List<PivotField> fields = new List<PivotField>();
            foreach (Axis axis in m_CellSet.Axes)
            {
                foreach (Hierarchy h in axis.Set.Hierarchies)
                {
                        PivotField field = 
                            new PivotField(h.UniqueName, h.Caption);
                        foreach (Property prop in h.Properties)
                        {
                            field.Properties.Add(prop.Name, prop.Value);
                        }

                        fields.Add(field);
                }
            }

            return new PivotFieldCollection(fields);
        }
        */

        #endregion
    }

    /// <summary>
    /// Класс описывает сортировку
    /// </summary>
    public class SortDescriptor
    {
        /// <summary>
        /// Тип сортировки
        /// </summary>
        public SortTypes Type = SortTypes.None;

        String m_SortBy = string.Empty;
        /// <summary>
        /// По чем производится сортировка
        /// </summary>
        public virtual String SortBy
        {
            get { return m_SortBy; }
            set { m_SortBy = value; }
        }

        public virtual SortDescriptor Clone()
        {
            var result = new SortDescriptor();
            result.Type = Type;
            result.SortBy = SortBy;
            return result;
        }
    }

    public enum DataReorganizationTypes
    {
        /// <summary>
        /// Данные никак не группируются
        /// </summary>
        None,
        /// <summary>
        /// Объединять соседние одинаковые элементы
        /// </summary>
        MergeNeighbors,
        /// <summary>
        /// Выстраивать в иерархию
        /// </summary>
        LinkToParent
    }

    public class MinMaxDescriptor<T>
    {
        public T Min = default(T);
        public T Max = default(T);

        public MinMaxDescriptor()
        { }

        public MinMaxDescriptor(T min, T max)
        {
            Min = min;
            Max = max;
        }
    }
}

/*
private class FieldAccessorPropertyDescriptor : PropertyDescriptor
{
    private string m_Caption;
    public FieldAccessorPropertyDescriptor(string uniqueName, string caption)
        : base(uniqueName, new Attribute[] { })
    {
        m_Caption = caption;
    }

    public override string DisplayName
    {
        get
        {
            return m_Caption;
        }
    }

    public override bool CanResetValue(object component)
    {
        throw new NotImplementedException();
    }

    public override object GetValue(object component)
    {
        return null;
    }

    public override void SetValue(object component, object value)
    {
        throw new NotImplementedException();
    }

    public override bool ShouldSerializeValue(object component)
    {
        throw new NotImplementedException();
    }

    public override void ResetValue(object component)
    {
        throw new NotImplementedException();
    }

    public override Type ComponentType
    {
        get 
        {
            return typeof(CellSetRow);
        }
    }

    public override Type PropertyType
    {
        get 
        {
            return typeof(object);
        }
    }

    public override bool IsReadOnly
    {
        get 
        {
            return true;
        }
    }
}

private class CellSetRow : ICustomTypeDescriptor
{
    private CellSetDataProvider m_Provider;
    private int[] m_IndexVector;
    public CellSetRow(CellSetDataProvider provider, int[] indexVector)
    {
        m_Provider = provider;
        m_IndexVector = indexVector;
    }

    public PivotFieldValue GetValue()
    {
        Cell cell = m_Provider.m_CellSet[m_IndexVector];
        object value = null;
        try
        {
            value = cell.Value;
        }
        catch (Exception exc)
        {
            value = exc;
        }

        string formattedValue = string.Empty;
        try
        {
            formattedValue = cell.FormattedValue;
        }
        catch
        {
        }

        return new PivotFieldValue(value, formattedValue);
    }

    #region ICustomTypeDescriptor Members

    public AttributeCollection GetAttributes()
    {
        return new AttributeCollection(new Attribute[] { });
    }

    public string GetClassName()
    {
        return string.Empty;
    }

    public string GetComponentName()
    {
        return string.Empty;
    }

    public TypeConverter GetConverter()
    {
        return TypeDescriptor.GetConverter(typeof(object));
    }

    public EventDescriptor GetDefaultEvent()
    {
        return null;
    }

    public PropertyDescriptor GetDefaultProperty()
    {
        return null;
    }

    public object GetEditor(Type editorBaseType)
    {
        return null;
    }

    public EventDescriptorCollection GetEvents(Attribute[] attributes)
    {
        return new EventDescriptorCollection(new EventDescriptor[] { });
    }

    public EventDescriptorCollection GetEvents()
    {
        return new EventDescriptorCollection(new EventDescriptor[] { });
    }

    public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
    {
        return this.GetProperties();
    }

    public PropertyDescriptorCollection GetProperties()
    {
        PropertyDescriptor[] props = new PropertyDescriptor[m_Provider.Fields.Count];
        //m_Provider.Fields.CopyTo(props, 0);
        return new PropertyDescriptorCollection(props, true);
    }

    public object GetPropertyOwner(PropertyDescriptor pd)
    {
        return this;
    }

    #endregion
}

private class CellSetEnumerator : IEnumerator
{
    private CellSet m_CellSet;
    public CellSetEnumerator(CellSet cs)
    {
        m_CellSet = cs;
    }

    #region IEnumerator Members

    public object Current
    {
        get 
        {
            throw new NotImplementedException();
        }
    }

    public bool MoveNext()
    {
        throw new NotImplementedException();
    }

    public void Reset()
    {
        throw new NotImplementedException();
    }

    #endregion
}
*/
