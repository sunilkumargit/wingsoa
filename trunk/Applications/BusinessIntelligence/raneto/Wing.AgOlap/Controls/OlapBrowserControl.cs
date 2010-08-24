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
using Ranet.AgOlap.Controls.General;
using Ranet.AgOlap.Controls.General.Tree;
using System.Collections.Generic;
using Ranet.Olap.Core.Metadata;
using Ranet.AgOlap.Controls.MdxDesigner.CalculatedMembers;
using System.Windows.Controls.Primitives;
using Ranet.Olap.Core;
using Ranet.AgOlap.Commands;
using Ranet.AgOlap.Controls.General.ClientServer;
using Ranet.AgOlap.Controls.MemberChoice.ClientServer;
using Ranet.AgOlap.Controls.MemberChoice;
using Ranet.Olap.Core.Data;
using Ranet.AgOlap.Controls.ValueCopy;
using Ranet.AgOlap.Providers;
using Ranet.Olap.Core.Providers.ClientServer;

namespace Ranet.AgOlap.Controls
{
    public class DragNodeArgs<T> : EventArgs 
    {
        public readonly TreeViewItem Node;
        public readonly T Args;

        public DragNodeArgs(TreeViewItem node, T args)
        {
            Node = node;
            Args = args;
        }
    }

    public abstract class OlapBrowserControl : AgTreeControlBase
    {
        protected readonly CustomTree Tree = null;

        #region Коллекции созданных узлов
        Dictionary<String, DimensionTreeNode> m_Dimension_Nodes = new Dictionary<string, DimensionTreeNode>();
        Dictionary<String, HierarchyTreeNode> m_Hierarchy_Nodes = new Dictionary<string, HierarchyTreeNode>();
        Dictionary<String, NamedSetTreeNode> m_NamedSet_Nodes = new Dictionary<string, NamedSetTreeNode>();
        Dictionary<String, LevelTreeNode> m_Level_Nodes = new Dictionary<string, LevelTreeNode>();
        Dictionary<String, MeasureTreeNode> m_Measure_Nodes = new Dictionary<string, MeasureTreeNode>();
        Dictionary<String, KpiValueTreeNode> m_KpiValue_Nodes = new Dictionary<string, KpiValueTreeNode>();
        Dictionary<String, KpiGoalTreeNode> m_KpiGoal_Nodes = new Dictionary<string, KpiGoalTreeNode>();
        Dictionary<String, KpiTrendTreeNode> m_KpiTrend_Nodes = new Dictionary<string, KpiTrendTreeNode>();
        Dictionary<String, KpiStatusTreeNode> m_KpiStatus_Nodes = new Dictionary<string, KpiStatusTreeNode>();
        protected Dictionary<String, CalculatedMemberTreeNode> m_CalculatedMember_Nodes = new Dictionary<string, CalculatedMemberTreeNode>();
        protected Dictionary<String, CalculatedNamedSetTreeNode> m_CalculatedNamedSet_Nodes = new Dictionary<string, CalculatedNamedSetTreeNode>();

        void ClearDictionaries()
        {
            m_Hierarchy_Nodes.Clear();
            m_Level_Nodes.Clear();
            m_Measure_Nodes.Clear();
            m_KpiValue_Nodes.Clear();
            m_KpiGoal_Nodes.Clear();
            m_KpiTrend_Nodes.Clear();
            m_KpiStatus_Nodes.Clear();
            m_NamedSet_Nodes.Clear();
        }

        public DimensionTreeNode FindDimensionNode(String uniqueName)
        {
            if (m_Dimension_Nodes.ContainsKey(uniqueName))
                return m_Dimension_Nodes[uniqueName];
            return null;
        }

        public HierarchyTreeNode FindHierarchyNode(String uniqueName)
        {
            if (m_Hierarchy_Nodes.ContainsKey(uniqueName))
                return m_Hierarchy_Nodes[uniqueName];
            return null;
        }

        public CalculatedMemberTreeNode FindCalculatedMember(String name)
        {
            if (m_CalculatedMember_Nodes.ContainsKey(name))
                return m_CalculatedMember_Nodes[name];
            return null;
        }

        public CalculatedNamedSetTreeNode FindCalculatedNamedSet(String name)
        {
            if (m_CalculatedNamedSet_Nodes.ContainsKey(name))
                return m_CalculatedNamedSet_Nodes[name];
            return null;
        }

        public NamedSetTreeNode FindNamedSet(String name)
        {
            if (m_NamedSet_Nodes.ContainsKey(name))
                return m_NamedSet_Nodes[name];
            return null;
        }

        public LevelTreeNode FindLevelNode(String uniqueName)
        {
            if (m_Level_Nodes.ContainsKey(uniqueName))
                return m_Level_Nodes[uniqueName];
            return null;
        }

        public MeasureTreeNode FindMeasureNode(String uniqueName)
        {
            if (m_Measure_Nodes.ContainsKey(uniqueName))
                return m_Measure_Nodes[uniqueName];
            return null;
        }

        public KpiValueTreeNode FindKpiValueNode(String name)
        {
            if (m_KpiValue_Nodes.ContainsKey(name))
                return m_KpiValue_Nodes[name];
            return null;
        }

        public KpiGoalTreeNode FindKpiGoalNode(String name)
        {
            if (m_KpiGoal_Nodes.ContainsKey(name))
                return m_KpiGoal_Nodes[name];
            return null;
        }

        public KpiStatusTreeNode FindKpiStatusNode(String name)
        {
            if (m_KpiStatus_Nodes.ContainsKey(name))
                return m_KpiStatus_Nodes[name];
            return null;
        }

        public KpiTrendTreeNode FindKpiTrendNode(String name)
        {
            if (m_KpiTrend_Nodes.ContainsKey(name))
                return m_KpiTrend_Nodes[name];
            return null;
        }
        #endregion Коллекции созданных узлов

        /// <summary>
        /// Выбранный узел дерева
        /// </summary>
        public CustomTreeNode SelectedNode
        {
            get {
                return Tree.SelectedItem as CustomTreeNode;
            }
        }

        protected MeasureGroupCombo m_ComboMeasureGroup;
        //protected HeaderControl m_TreeHeader;
        protected HeaderControl m_MeasureGroupHeader;

        public OlapBrowserControl()
        {
            Grid LayoutRoot = new Grid() { Margin = new Thickness(0) };
            //LayoutRoot.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            LayoutRoot.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            LayoutRoot.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            LayoutRoot.RowDefinitions.Add(new RowDefinition());

            m_MeasureGroupHeader = new HeaderControl(UriResources.Images.MeasureGroup16, Localization.MeasureGroup) { Margin = new Thickness(0, 0, 0, 0) };
            LayoutRoot.Children.Add(m_MeasureGroupHeader);
            Grid.SetRow(m_MeasureGroupHeader, 0);

            m_ComboMeasureGroup = new MeasureGroupCombo() { Margin = new Thickness(0, 3, 0, 3) }; ;
            m_ComboMeasureGroup.SelectionChanged += new EventHandler(comboMeasureGroup_SelectionChanged);
            LayoutRoot.Children.Add(m_ComboMeasureGroup);
            Grid.SetRow(m_ComboMeasureGroup, 1);

            //m_TreeHeader = new HeaderControl(UriResources.Images.Cube16, Localization.MdxDesigner_CubeMetadata) { Margin = new Thickness(0, 5, 0, 3) };
            //LayoutRoot.Children.Add(m_TreeHeader);
            //Grid.SetRow(m_TreeHeader, 2);

            Tree = new CustomTree();
            LayoutRoot.Children.Add(Tree);
            Grid.SetRow(Tree, 2);

            m_MeasureGroupHeader.Visibility = m_ComboMeasureGroup.Visibility = Visibility.Collapsed;
            //m_TreeHeader.Visibility = Visibility.Collapsed;

            base.Content = LayoutRoot;
            this.OlapDataLoader = GetOlapDataLoader();

            Tree.SelectedItemChanged += new RoutedPropertyChangedEventHandler<object>(Tree_SelectedItemChanged);
        }

        void comboMeasureGroup_SelectionChanged(object sender, EventArgs e)
        {
            RefreshTree();
        }

        protected virtual void Tree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            InfoBase info = null;
            InfoBaseTreeNode node = e.NewValue as InfoBaseTreeNode;
            if (node != null)
            {
                info = node.Info;
            }
            Raise_SelectedItemChanged(info);
        }

        #region Свойства для настройки на OLAP

        private String m_Connection = String.Empty;
        /// <summary>
        /// ID соединения
        /// </summary>
        public String Connection
        {
            get
            {
                return m_Connection;
            }
            set
            {
                m_Connection = value;
            }
        }

        private String m_CubeName = String.Empty;
        /// <summary>
        /// Имя куба
        /// </summary>
        public String CubeName
        {
            get
            {
                return m_CubeName;
            }
            set
            {
                m_CubeInfo = null;
                m_CubeName = value;
            }
        }

        private String m_SubCube = String.Empty;
        /// <summary>
        /// Выражение, определяющее подкуб
        /// </summary>
        public String SubCube
        {
            get
            {
                return m_SubCube;
            }
            set
            {
                m_SubCube = value;
            }
        }

        String  m_MeasureGroupName = String.Empty;
        /// <summary>
        /// Имя группы мер
        /// </summary>
        public String MeasureGroupName
        {
            get
            {
                return m_MeasureGroupName;
            }
            set
            {
                m_MeasureGroupName = value;
                m_ComboMeasureGroup.SelectItem(MeasureGroupName);
            }
        }
        #endregion Свойства для настройки на OLAP

        public String CurrentMeasureGroupName
        {
            get {
                if (m_ComboMeasureGroup.CurrentItem != null &&
                    m_ComboMeasureGroup.CurrentItem.Name != MeasureGroupCombo.ALL_MEASURES_GROUPS)
                    return m_ComboMeasureGroup.CurrentItem.Name;
                return String.Empty;
            }
        }

        CubeDefInfo m_CubeInfo = null;
        /// <summary>
        /// Метаинформация о кубе
        /// </summary>
        public CubeDefInfo CubeInfo
        {
            get { return m_CubeInfo; }
        }

        public void Initialize()
        {
            CalculatedMembers.Clear();
            CalculatedNamedSets.Clear();
            m_CubeInfo = null;
            ClearTree();

            GetCube();
        }

        public void Initialize(CubeDefInfo cubeInfo)
        {
            ClearTree();

            m_CubeInfo = cubeInfo;

            m_ComboMeasureGroup.SelectionChanged -= new EventHandler(comboMeasureGroup_SelectionChanged);
            m_ComboMeasureGroup.SelectionChanged += new EventHandler(comboMeasureGroup_SelectionChanged);
            
            if(m_CubeInfo != null)
            {
                m_ComboMeasureGroup.Initialize(m_CubeInfo.MeasureGroups);
                m_ComboMeasureGroup.SelectItem(MeasureGroupName);
            }

            EventHandler handler = Initialized;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        public event EventHandler Initialized;

        public bool DragNodes = false;

        public void Clear()
        {
            m_ComboMeasureGroup.Initialize(null);
            ClearTree();
        }

        void ClearTree()
        {
            m_Sets_Node = null;
            m_Measures_Node = null;

            ClearDictionaries();

            Tree.Items.Clear();
            m_KPIGroupNodes.Clear();
            m_HierarchyGroupNodes.Clear();
            m_NamedSetFolderNodes.Clear();
            m_MeasuresGroupNodes.Clear();
            m_CalculatedMember_Nodes.Clear();
            m_CalculatedNamedSet_Nodes.Clear();
        }

        // Summary:
        //     Occurs when the System.Windows.Controls.Primitives.Thumb control loses mouse
        //     capture.
        public event EventHandler<DragNodeArgs<DragCompletedEventArgs>> DragCompleted;
        //
        // Summary:
        //     Occurs one or more times as the mouse pointer is moved when a System.Windows.Controls.Primitives.Thumb
        //     control has logical focus and mouse capture.
        public event EventHandler<DragNodeArgs<DragDeltaEventArgs>> DragDelta;
        //
        // Summary:
        //     Occurs when a System.Windows.Controls.Primitives.Thumb control receives logical
        //     focus and mouse capture.
        public event EventHandler<DragNodeArgs<DragStartedEventArgs>> DragStarted;

        protected CustomTreeNode m_Sets_Node = null;
        protected CustomTreeNode m_Measures_Node = null;
        protected CustomTreeNode m_CustomCalculations_Node = null;

        List<CalculatedNamedSetInfo> m_CalculatedNamedSets;
        public List<CalculatedNamedSetInfo> CalculatedNamedSets
        {
            get
            {
                if (m_CalculatedNamedSets == null)
                {
                    m_CalculatedNamedSets = new List<CalculatedNamedSetInfo>();
                }
                return m_CalculatedNamedSets;
            }
            set
            {
                m_CalculatedNamedSets = value;
                //CreateCalulatedNamedSets(m_Sets_Node, CubeInfo);
                CreateCalculations(m_CustomCalculations_Node);
            }
        }

        List<CalcMemberInfo> m_CalculatedMembers;
        public List<CalcMemberInfo> CalculatedMembers
        {
            get
            {
                if (m_CalculatedMembers == null)
                {
                    m_CalculatedMembers = new List<CalcMemberInfo>();
                }
                return m_CalculatedMembers;
            }
            set
            {
                m_CalculatedMembers = value;
                //CreateCalulatedMembers(m_Measures_Node, CubeInfo);
                CreateCalculations(m_CustomCalculations_Node);
            }
        }

        protected void CreateCalculations(CustomTreeNode parentNode)
        {
            m_CalculatedMember_Nodes.Clear();
            m_CalculatedNamedSet_Nodes.Clear();
            if (parentNode != null)
            {
                parentNode.Items.Clear();

                if (CalculatedMembers.Count + CalculatedNamedSets.Count == 0)
                {
                    parentNode.Visibility = Visibility.Collapsed;
                }
                else
                {
                    parentNode.Visibility = Visibility.Visible;
                }

                if (CalculatedMembers.Count + CalculatedNamedSets.Count > 0)
                {
                    // СОздаем узлы для элементов
                    foreach (CalcMemberInfo info in CalculatedMembers)
                    {
                        CalculatedMemberTreeNode node = new CalculatedMemberTreeNode(info);
                        node.MouseDoubleClick += new EventHandler<CustomEventArgs<CustomTreeNode>>(TreeNode_MouseDoubleClick);
                        // Тягание
                        AllowDragDrop(node);

                        m_CalculatedMember_Nodes[info.Name] = node;
                        parentNode.Items.Add(node);
                    }

                    // СОздаем узлы для сетов
                    foreach (CalculatedNamedSetInfo setInfo in CalculatedNamedSets)
                    {
                        CalculatedNamedSetTreeNode namedSetNode = new CalculatedNamedSetTreeNode(setInfo);
                        namedSetNode.MouseDoubleClick += new EventHandler<CustomEventArgs<CustomTreeNode>>(TreeNode_MouseDoubleClick);
                        // Тягание
                        AllowDragDrop(namedSetNode);

                        m_CalculatedNamedSet_Nodes[setInfo.Name] = namedSetNode;
                        parentNode.Items.Add(namedSetNode);
                    }
                }
            }
        }

        public event EventHandler<CustomEventArgs<CustomTreeNode>> MouseDoubleClick;
        void Raise_MouseDoubleClick(CustomEventArgs<CustomTreeNode> e)
        {
            EventHandler<CustomEventArgs<CustomTreeNode>> handler = MouseDoubleClick;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void TreeNode_MouseDoubleClick(object sender, CustomEventArgs<CustomTreeNode> e)
        {
            Raise_MouseDoubleClick(e);
        }

        #region Дерево: Кубы
        protected virtual void RefreshTree()
        {
            ClearTree();
        }

        bool ValidateSettings()
        {
    //        case OlapBrowserContentTypes.Measures:
    //        if (String.IsNullOrEmpty(Connection) ||
    //String.IsNullOrEmpty(CubeName))
    //        {
    //            // Сообщение в лог
    //            StringBuilder builder = new StringBuilder();
    //            if (String.IsNullOrEmpty(Connection))
    //                builder.Append(Localization.Connection_PropertyDesc + ", ");
    //            if (String.IsNullOrEmpty(CubeName))
    //                builder.Append(Localization.CubeName_PropertyDesc);
    //            LogManager.LogMessage(Localization.MeasureChoiceControl_Name, Localization.Error + "! " + String.Format(Localization.ControlSettingsNotInitialized_Message, builder.ToString()));

    //            measuresNode.IsError = true;
    //            return;
    //        }

            return true;
        }

        void DragThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            EventHandler<DragNodeArgs<DragCompletedEventArgs>> handler = DragCompleted;
            if (handler != null)
            {
                handler(sender, new DragNodeArgs<DragCompletedEventArgs>(sender as TreeViewItem, e));
            }
        }

        void DragThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            EventHandler<DragNodeArgs<DragDeltaEventArgs>> handler = DragDelta;
            if (handler != null)
            {
                handler(sender, new DragNodeArgs<DragDeltaEventArgs>(sender as TreeViewItem, e));
            }
        }

        void DragThumb_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            EventHandler<DragNodeArgs<DragStartedEventArgs>> handler = DragStarted;
            if (handler != null)
            {
                handler(sender, new DragNodeArgs<DragStartedEventArgs>(sender as TreeViewItem, e));
            }
        }

        //void cubeNode_Expanded(object sender, RoutedEventArgs e)
        //{
        //    CubeTreeNode cubeNode = sender as CubeTreeNode;
        //    if (cubeNode != null && !cubeNode.IsInitialized)
        //    {
        //        cubeNode.IsWaiting = false;
        //        CreateDimensions(cubeNode, cubeNode.Info as CubeDefInfo);
        //    }
        //}
        #endregion Дерево: Кубы

        #region Дерево: Измерения
        protected void CreateDimensions(CustomTreeNode parentNode, CubeDefInfo cube, bool createHierarchies)
        {
            if (cube != null)
            {
                foreach (DimensionInfo info in cube.Dimensions)
                {
                    if (info.DimensionType != DimensionInfoTypeEnum.Measure)
                    {
                        // Учитываем текущую группу мер
                        if (m_ComboMeasureGroup.CurrentItem != null &&
                            (m_ComboMeasureGroup.CurrentItem.Name == MeasureGroupCombo.ALL_MEASURES_GROUPS ||
                            m_ComboMeasureGroup.CurrentItem.Dimensions.Contains(info.UniqueName)))
                        {
                            DimensionTreeNode dimNode = AddDimensionNode(parentNode, info);                      
                            if(createHierarchies)
                            {
                                CreateHierarchies(dimNode, info, true);
                            }
                        }
                    }
                }
            }
        }

        protected DimensionTreeNode AddDimensionNode(CustomTreeNode parentNode, DimensionInfo info)
        {
            DimensionTreeNode dimNode = new DimensionTreeNode(info);
            dimNode.MouseDoubleClick += new EventHandler<CustomEventArgs<CustomTreeNode>>(TreeNode_MouseDoubleClick);
            // Тягание
            AllowDragDrop(dimNode);

            m_Dimension_Nodes[info.UniqueName] = dimNode;

            if (parentNode == null)
                Tree.Items.Add(dimNode);
            else
                parentNode.Items.Add(dimNode);
            return dimNode;
        }
 
        //void dimNode_Expanded(object sender, RoutedEventArgs e)
        //{
        //    DimensionTreeNode dimNode = sender as DimensionTreeNode;
        //    if (dimNode != null && !dimNode.IsInitialized)
        //    {
        //        dimNode.IsWaiting = false;
        //        CreateHierarchies(dimNode, dimNode.Info as DimensionInfo);
        //    }
        //}

        #endregion Дерево: Измерения

        #region Дерево: Иераррхии
        protected virtual void CreateHierarchies(CustomTreeNode parentNode, DimensionInfo dimension, bool createLevels)
        {
            if (dimension != null)
            {
                foreach (HierarchyInfo info in dimension.Hierarchies)
                {
                    CustomTreeNode groupNode = null;

                    //Иерархии могут быть сгруппированы в папки. Причем папки могут быть вложенными. Например: "Динамика\\Оборачиваемость"
                    if (!String.IsNullOrEmpty(info.DisplayFolder))
                    {
                        // Если папка по такому же полному пути уже создана то все Ок
                        if (m_HierarchyGroupNodes.ContainsKey(info.DisplayFolder))
                        {
                            groupNode = m_HierarchyGroupNodes[info.DisplayFolder];
                        }
                        else
                        {
                            CustomTreeNode prevNode = parentNode;
                            // Разбиваем полный путь на составляющие и создаем папку для каждой из них
                            String[] groups = info.DisplayFolder.Split(new String[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
                            if (groups != null)
                            {
                                foreach (String groupName in groups)
                                {
                                    // Создаем узел для группы
                                    if (!String.IsNullOrEmpty(groupName))
                                    {
                                        if (m_HierarchyGroupNodes.ContainsKey(groupName))
                                        {
                                            prevNode = m_HierarchyGroupNodes[groupName];
                                        }
                                        else
                                        {
                                            groupNode = new FolderTreeNode();
                                            groupNode.MouseDoubleClick += new EventHandler<CustomEventArgs<CustomTreeNode>>(TreeNode_MouseDoubleClick);
                                            groupNode.Text = groupName;
                                            m_HierarchyGroupNodes[groupName] = groupNode;

                                            if (prevNode == null)
                                                Tree.Items.Add(groupNode);
                                            else
                                                prevNode.Items.Add(groupNode);

                                            prevNode = groupNode;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (groupNode == null)
                        groupNode = parentNode;

                    HierarchyTreeNode hierarchyNode = AddHierarchyNode(groupNode, info);

                    if(createLevels)
                    {
                        CreateLevels(hierarchyNode, info, true);
                    }
                }
            }
        }

        protected HierarchyTreeNode AddHierarchyNode(CustomTreeNode parentNode, HierarchyInfo info)
        {
            HierarchyTreeNode hierarchyNode = new HierarchyTreeNode(info);
            hierarchyNode.MouseDoubleClick += new EventHandler<CustomEventArgs<CustomTreeNode>>(TreeNode_MouseDoubleClick);
            // Тягание
            AllowDragDrop(hierarchyNode);

            m_Hierarchy_Nodes[info.UniqueName] = hierarchyNode;

            if (parentNode == null)
                Tree.Items.Add(hierarchyNode);
            else
                parentNode.Items.Add(hierarchyNode);
            return hierarchyNode;
        }

        #endregion Дерево: Иерархии

        #region Дерево: Уровни
        protected virtual void CreateLevels(CustomTreeNode parentNode, HierarchyInfo hierarchy, bool createMembers)
        {
            if (hierarchy != null)
            {
                int indx = 0;
                bool useAllLevel = true;
                foreach (LevelInfo info in hierarchy.Levels)
                {
                    LevelTreeNode levelNode = new LevelTreeNode(info);
                    levelNode.MouseDoubleClick += new EventHandler<CustomEventArgs<CustomTreeNode>>(TreeNode_MouseDoubleClick);
                    levelNode.Expanded += new RoutedEventHandler(levelNode_Expanded);
                    if (createMembers)
                    {
                        // добавляем фиктивный узел для отображения [+] напротив данного узла
                        levelNode.IsWaiting = true;
                    }
                    // Тягание
                    AllowDragDrop(levelNode);

                    m_Level_Nodes[info.UniqueName] = levelNode;

                    //Если нулевой уровень не All, то иконку ставим как для уровня 1
                    if (indx == 0 && info.LevelType != LevelInfoTypeEnum.All)
                        useAllLevel = false;
                    if (!useAllLevel)
                        levelNode.UseAllLevelIcon = false;

                    if (parentNode == null)
                        Tree.Items.Add(levelNode);
                    else
                        parentNode.Items.Add(levelNode);
                    indx++;
                }
            }
        }

        private int m_Step = 20;
        /// <summary>
        /// Шаг при частичной загрузке
        /// </summary>
        public int Step
        {
            get
            {
                if (m_Step <= 0)
                    return 1;
                else
                    return m_Step;
            }
            set
            {
                m_Step = value;
            }
        }

        void levelNode_Expanded(object sender, RoutedEventArgs e)
        {
            LevelTreeNode levelNode = sender as LevelTreeNode;
            if (levelNode != null)
            {
                // Проверяем, грузились ли дочерние элементы
                if (!levelNode.IsInitialized)
                {
                    // Загружаем элементы для уровня
                    // Грузим Step+1 элемент, а отображать будем только Step штук. Если в ответе придет Step+1 элемент, то значит нужно отображать узел LoadNext
                    LoadChildrenMembers(levelNode, 0, Step + 1);
                }
            }
        }

        #endregion Дерево: Уровни

        private void LoadChildrenMembers(CustomTreeNode node, long begin, long count)
        {
            LevelTreeNode levelNode = node as LevelTreeNode;
            if (levelNode != null)
            {
                LevelInfo info = levelNode.Info as LevelInfo;
                if (info != null)
                {
                    node.IsWaiting = true;
                    QueryProvider DataManager = new QueryProvider(CubeName, SubCube, info.ParentHirerachyId);
                    String query = String.Empty;
                    if (String.IsNullOrEmpty(info.UniqueName))
                        query = DataManager.GetHierarchyMembers(0, begin, count);
                    else
                        query = DataManager.GetLevelMembers(info.UniqueName, begin, count);
                    LogManager.LogInformation(this, this.Name + " - Loading members from '" + info.UniqueName + "'");
                    MdxQueryArgs query_args = CommandHelper.CreateMdxQueryArgs(Connection, query);
                    OlapDataLoader.LoadData(query_args, new UserSchemaWrapper<MemberChoiceQueryType, CustomTreeNode>(MemberChoiceQueryType.GetRootMembers, node));
                }
            }

            MemberLiteTreeNode memberNode = node as MemberLiteTreeNode;
            if (memberNode != null && memberNode.Info != null && memberNode.Info != null)
            {
                node.IsWaiting = true;
                QueryProvider DataManager = new QueryProvider(CubeName, SubCube, memberNode.Info.HierarchyUniqueName);
                String query = DataManager.GetChildrenMembers(memberNode.Info.UniqueName, begin, count);
                LogManager.LogInformation(this, this.Name + " - Loading children for '" + memberNode.Info.UniqueName + "'");
                MdxQueryArgs query_args = CommandHelper.CreateMdxQueryArgs(Connection, query);
                OlapDataLoader.LoadData(query_args, new UserSchemaWrapper<MemberChoiceQueryType, CustomTreeNode>(MemberChoiceQueryType.GetChildrenMembers, node));
            }
        }


        #region Дерево: KPI
        //void kpisNode_Expanded(object sender, RoutedEventArgs e)
        //{
        //    KPIsFolderTreeNode kpisNode = sender as KPIsFolderTreeNode;
        //    if (kpisNode != null && !kpisNode.IsInitialized)
        //    {
        //        kpisNode.IsWaiting = false;
        //        CreateKPIs(kpisNode, m_CubeInfo);
        //    }
        //}

        protected void CreateKPIs(CustomTreeNode parentNode, CubeDefInfo cube, bool kpiParameters)
        {
            if (cube != null)
            {
                foreach (KpiInfo info in cube.Kpis)
                {
                    // Учитываем текущую группу мер
                    if (m_ComboMeasureGroup.CurrentItem != null &&
                        (m_ComboMeasureGroup.CurrentItem.Name == MeasureGroupCombo.ALL_MEASURES_GROUPS ||
                        m_ComboMeasureGroup.CurrentItem.Kpis.Contains(info.Name)))
                    {
                        CustomTreeNode groupNode = null;

                        //String groupName = String.Empty;
                        //Показатели могут быть сгруппированы в группы. Причем папки могут быть вложенными. Например: "Динамика\\Оборачиваемость"
                        if (!String.IsNullOrEmpty(info.DisplayFolder))
                        {
                            // Если папка по такому же полному пути уже создана то все Ок
                            if (m_KPIGroupNodes.ContainsKey(info.DisplayFolder))
                            {
                                groupNode = m_KPIGroupNodes[info.DisplayFolder];
                            }
                            else
                            {
                                CustomTreeNode prevNode = parentNode;
                                // Разбиваем полный путь на составляющие и создаем папку для каждой из них
                                String[] groups = info.DisplayFolder.Split(new String[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
                                if (groups != null)
                                {
                                    foreach (String groupName in groups)
                                    {
                                        // Создаем узел для группы
                                        if (!String.IsNullOrEmpty(groupName))
                                        {
                                            if (m_KPIGroupNodes.ContainsKey(groupName))
                                            {
                                                prevNode = m_KPIGroupNodes[groupName];
                                            }
                                            else
                                            {
                                                groupNode = new FolderTreeNode();
                                                groupNode.MouseDoubleClick += new EventHandler<CustomEventArgs<CustomTreeNode>>(TreeNode_MouseDoubleClick);
                                                groupNode.Text = groupName;
                                                m_KPIGroupNodes[groupName] = groupNode;

                                                if (prevNode == null)
                                                    Tree.Items.Add(groupNode);
                                                else
                                                    prevNode.Items.Add(groupNode);

                                                prevNode = groupNode;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (groupNode == null)
                            groupNode = parentNode;

                        KpiTreeNode node = new KpiTreeNode(info);
                        node.MouseDoubleClick += new EventHandler<CustomEventArgs<CustomTreeNode>>(TreeNode_MouseDoubleClick);
                        AllowDragDrop(node);
                        if (groupNode == null)
                            Tree.Items.Add(node);
                        else
                            groupNode.Items.Add(node);

                        if (kpiParameters)
                        {
                            // "KPI_VALUE"
                            KpiValueTreeNode valueNode = new KpiValueTreeNode(info);
                            valueNode.MouseDoubleClick += new EventHandler<CustomEventArgs<CustomTreeNode>>(TreeNode_MouseDoubleClick);
                            AllowDragDrop(valueNode);
                            node.Items.Add(valueNode);
                            m_KpiValue_Nodes[info.Name] = valueNode;

                            // "KPI_GOAL"
                            KpiGoalTreeNode goalNode = new KpiGoalTreeNode(info);
                            goalNode.MouseDoubleClick += new EventHandler<CustomEventArgs<CustomTreeNode>>(TreeNode_MouseDoubleClick);
                            AllowDragDrop(goalNode);
                            node.Items.Add(goalNode);
                            m_KpiGoal_Nodes[info.Name] = goalNode;

                            // "KPI_STATUS"
                            KpiStatusTreeNode statusNode = new KpiStatusTreeNode(info);
                            statusNode.MouseDoubleClick += new EventHandler<CustomEventArgs<CustomTreeNode>>(TreeNode_MouseDoubleClick);
                            AllowDragDrop(statusNode);
                            node.Items.Add(statusNode);
                            m_KpiStatus_Nodes[info.Name] = statusNode;

                            // "KPI_TREND"
                            KpiTrendTreeNode trendNode = new KpiTrendTreeNode(info);
                            trendNode.MouseDoubleClick += new EventHandler<CustomEventArgs<CustomTreeNode>>(TreeNode_MouseDoubleClick);
                            AllowDragDrop(trendNode);
                            node.Items.Add(trendNode);
                            m_KpiTrend_Nodes[info.Name] = trendNode;
                        }
                    }
                }
            }
        }
        #endregion Дерево: KPI

        protected void AllowDragDrop(CustomTreeNode node)
        {
            if (node != null)
            {
                node.UseDragDrop = DragNodes;
                node.DragStarted += new System.Windows.Controls.Primitives.DragStartedEventHandler(DragThumb_DragStarted);
                node.DragDelta += new DragDeltaEventHandler(DragThumb_DragDelta);
                node.DragCompleted += new DragCompletedEventHandler(DragThumb_DragCompleted);
            }
        }

        #region Дерево: Именованные наборы
        protected void CreateNamedSets(CustomTreeNode parentNode, CubeDefInfo cube)
        {
            if (cube != null)
            {
                m_NamedSet_Nodes.Clear();
                m_NamedSetFolderNodes.Clear();
                if (m_Sets_Node != null)
                {
                    m_NamedSetFolderNodes.Add("Sets", m_Sets_Node);
                }

                foreach (NamedSetInfo info in cube.NamedSets)
                {
                    CustomTreeNode groupNode = null;

                    //Элементы могут быть сгруппированы в папки. Причем папки могут быть вложенными. Например: "Динамика\\Оборачиваемость"
                    if (!String.IsNullOrEmpty(info.DisplayFolder))
                    {
                        // Если папка по такому же полному пути уже создана то все Ок
                        if (m_NamedSetFolderNodes.ContainsKey(info.DisplayFolder))
                        {
                            groupNode = m_NamedSetFolderNodes[info.DisplayFolder];
                        }
                        else
                        {
                            CustomTreeNode prevNode = parentNode;
                            // Разбиваем полный путь на составляющие и создаем папку для каждой из них
                            String[] groups = info.DisplayFolder.Split(new String[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
                            if (groups != null)
                            {
                                foreach (String groupName in groups)
                                {
                                    // Создаем узел для группы
                                    if (!String.IsNullOrEmpty(groupName))
                                    {
                                        if (m_NamedSetFolderNodes.ContainsKey(groupName))
                                        {
                                            prevNode = m_NamedSetFolderNodes[groupName];
                                        }
                                        else
                                        {
                                            groupNode = new FolderTreeNode();
                                            groupNode.MouseDoubleClick += new EventHandler<CustomEventArgs<CustomTreeNode>>(TreeNode_MouseDoubleClick);
                                            groupNode.Text = groupName;
                                            m_NamedSetFolderNodes[groupName] = groupNode;

                                            if (prevNode == null)
                                                Tree.Items.Add(groupNode);
                                            else
                                                prevNode.Items.Add(groupNode);

                                            prevNode = groupNode;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (groupNode == null)
                        groupNode = parentNode;

                    NamedSetTreeNode namedSetNode = new NamedSetTreeNode(info);
                    namedSetNode.MouseDoubleClick += new EventHandler<CustomEventArgs<CustomTreeNode>>(TreeNode_MouseDoubleClick);
                    // Тягание
                    AllowDragDrop(namedSetNode);

                    m_NamedSet_Nodes[info.Name] = namedSetNode;

                    if (groupNode == null)
                        Tree.Items.Add(namedSetNode);
                    else
                        groupNode.Items.Add(namedSetNode);

                }
            }
        }
        #endregion Дерево: Именованные наборы

        #region Дерево: Меры
        //void measuresNode_Expanded(object sender, RoutedEventArgs e)
        //{
        //    MeasuresFolderTreeNode measuresNode = sender as MeasuresFolderTreeNode;
        //    if (measuresNode != null && !measuresNode.IsInitialized)
        //    {
        //        measuresNode.IsWaiting = false;
        //        CreateMeasures(measuresNode, m_CubeInfo);
        //    }
        //}
        
        CustomTreeNode CrateMeasureGroupNode(CustomTreeNode parentNode, String measureGroupName, Dictionary<String, CustomTreeNode> nodesDict)
        {
            CustomTreeNode node = null;
            if (!String.IsNullOrEmpty(measureGroupName))
            {
                // Проверяем на наличие в коллекции узлов
                if (nodesDict != null && nodesDict.ContainsKey(measureGroupName) && nodesDict[measureGroupName] != null)
                    return nodesDict[measureGroupName];

                node = new MeasureGroupTreeNode();
                node.MouseDoubleClick += new EventHandler<CustomEventArgs<CustomTreeNode>>(TreeNode_MouseDoubleClick);
                node.Text = measureGroupName;
                if(nodesDict != null)
                    nodesDict[measureGroupName] = node;

                if (parentNode == null)
                    Tree.Items.Add(node);
                else
                    parentNode.Items.Add(node);
            }

            return node;
        }

        CustomTreeNode CrateFolderNode(CustomTreeNode parentNode, String folderName, Dictionary<String, CustomTreeNode> nodesDict)
        {
            CustomTreeNode folderNode = null;
            if (!String.IsNullOrEmpty(folderName))
            {
                // Проверяем на наличие в коллекции узлов
                if (nodesDict != null && nodesDict.ContainsKey(folderName) && nodesDict[folderName] != null)
                    return nodesDict[folderName];

                CustomTreeNode prevNode = parentNode;
                // Разбиваем полный путь на составляющие и создаем папку для каждой из них
                String[] folders = folderName.Split(new String[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
                if (folders != null)
                {
                    String current_folder_name = String.Empty;
                    foreach (String folder_tail in folders)
                    {
                        if (String.IsNullOrEmpty(folder_tail))
                            continue;

                        if (!String.IsNullOrEmpty(current_folder_name))
                            current_folder_name += "\\";
                        current_folder_name += folder_tail;

                        // Создаем узел для группы
                        if (nodesDict != null && nodesDict.ContainsKey(current_folder_name))
                        {
                            prevNode = nodesDict[current_folder_name];
                        }
                        else
                        {
                            FolderTreeNode node = new FolderTreeNode();
                            node.MouseDoubleClick += new EventHandler<CustomEventArgs<CustomTreeNode>>(TreeNode_MouseDoubleClick);
                            node.Text = folder_tail;
                            if (nodesDict != null)
                            {
                                nodesDict[current_folder_name] = node;
                            }

                            if (prevNode == null)
                                Tree.Items.Add(node);
                            else
                                prevNode.Items.Add(node);

                            prevNode = node;
                        }
                    }
                }
            }

            // Проверяем на наличие в коллекции узлов
            if (nodesDict != null && nodesDict.ContainsKey(folderName) && nodesDict[folderName] != null)
                return nodesDict[folderName];

            return folderNode;
        }

        protected void CreateMeasures(CustomTreeNode parentNode, CubeDefInfo cube)
        {
            if (cube != null)
            {
                m_MeasuresGroupNodes.Clear();
                m_Measure_Nodes.Clear();

                foreach (MeasureInfo info in cube.Measures)
                {
                    // Узел, в который дочерним добавится данная мера
                    CustomTreeNode parent = null;

                    // Учитываем текущую группу мер
                    if (m_ComboMeasureGroup.CurrentItem != null &&
                        (m_ComboMeasureGroup.CurrentItem.Name == MeasureGroupCombo.ALL_MEASURES_GROUPS ||
                        m_ComboMeasureGroup.CurrentItem.Measures.Contains(info.UniqueName)))
                    {
                        // Показатели могут быть сгруппированы в группы. Для каждой группы ранее был создан узел
                        // Показатели могут быть сгруппированы в папки. Папка может быть как в группе так и без группы
                        if (!String.IsNullOrEmpty(info.MeasureGroup) || !String.IsNullOrEmpty(info.DisplayFolder))
                        {
                            CustomTreeNode group_node = parentNode;
                            String nodeName = info.MeasureGroup;
                            if (!String.IsNullOrEmpty(info.MeasureGroup))
                            {
                                CustomTreeNode x = CrateMeasureGroupNode(parentNode, info.MeasureGroup, m_MeasuresGroupNodes);
                                if (x != null)
                                    group_node = x;
                            }

                            if (!String.IsNullOrEmpty(info.DisplayFolder))
                            {
                                if (!String.IsNullOrEmpty(nodeName))
                                    nodeName += "\\";
                                nodeName += info.DisplayFolder;
                            }

                            parent = CrateFolderNode(group_node, nodeName, m_MeasuresGroupNodes);
                        }

                        if (parent == null)
                            parent = parentNode;

                        MeasureTreeNode node = new MeasureTreeNode(info);
                        node.MouseDoubleClick += new EventHandler<CustomEventArgs<CustomTreeNode>>(TreeNode_MouseDoubleClick);
                        // Тягание
                        AllowDragDrop(node);

                        m_Measure_Nodes[info.UniqueName] = node;

                        if (parent == null)
                            Tree.Items.Add(node);
                        else
                            parent.Items.Add(node);
                    }
                }
            }
        }
        #endregion Дерево: Меры

        protected virtual IDataLoader GetOlapDataLoader()
        {
            return new OlapDataLoader(URL);
        }

        IDataLoader m_OlapDataLoader = null;
        public IDataLoader OlapDataLoader
        {
            get
            {
                return m_OlapDataLoader;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentException("OlapDataLoader not must be null.");
                }
                if (m_OlapDataLoader != null)
                {
                    m_OlapDataLoader.DataLoaded -= new EventHandler<DataLoaderEventArgs>(OlapDataLoader_DataLoaded);
                }
                m_OlapDataLoader = value;
                if (value != null)
                {
                    m_OlapDataLoader.DataLoaded += new EventHandler<DataLoaderEventArgs>(OlapDataLoader_DataLoaded);
                }
            }
        }

        public void ShowErrorInTree(CustomTreeNode parentNode)
        {
            if (parentNode != null)
            {
                parentNode.IsError = true;
            }
            else
            {
                Tree.IsError = true;
            }
        }


        void OlapDataLoader_DataLoaded(object sender, DataLoaderEventArgs e)
        {
            // Элементы измерения
            CustomTreeNode parentNode = null;
            UserSchemaWrapper<MemberChoiceQueryType, CustomTreeNode> data_wrapper = e.UserState as UserSchemaWrapper<MemberChoiceQueryType, CustomTreeNode>;
            if (data_wrapper != null)
            {
                parentNode = data_wrapper.UserData;
                if (parentNode != null)
                {
                    parentNode.IsWaiting = false;
                }
                else
                {
                    Tree.IsWaiting = false;
                }
            }

            // Метаданные
            UserSchemaWrapper<MetadataQuery, CustomTreeNode> metadata_wrapper = e.UserState as UserSchemaWrapper<MetadataQuery, CustomTreeNode>;
            if (metadata_wrapper != null)
            {
                m_ComboMeasureGroup.IsWaiting = false;

                parentNode = metadata_wrapper.UserData;
                if (parentNode != null)
                {
                    parentNode.IsWaiting = false;
                }
                else
                {
                    Tree.IsWaiting = false;
                }
            }

            if (e.Error != null)
            {
                ShowErrorInTree(parentNode);
                LogManager.LogError(this, e.Error.ToString());
                return;
            }

            if (e.Result.ContentType == InvokeContentType.Error)
            {
                ShowErrorInTree(parentNode);
                LogManager.LogError(this, e.Result.Content);
                return;
            }

            if (data_wrapper != null)
            {
                switch (data_wrapper.Schema)
                {
                    case MemberChoiceQueryType.GetRootMembers:
                    case MemberChoiceQueryType.GetChildrenMembers:
                        LoadMembers_Completed(e, parentNode);
                        break;
                }
            }

            if (metadata_wrapper != null)
            {
                switch (metadata_wrapper.Schema.QueryType)
                {
                    case MetadataQueryType.GetCubeMetadata:
                    case MetadataQueryType.GetCubeMetadata_AllMembers:
                        GetCubeMetadata_InvokeCommandCompleted(e, parentNode);
                        break;
                }
            }
        }

        void LoadMembers_Completed(DataLoaderEventArgs e, CustomTreeNode parentNode)
        {
            if (e.Result.ContentType == InvokeContentType.Error)
            {
                if (parentNode != null)
                    parentNode.IsError = true;
                return;
            }

            List<MemberData> members = new List<MemberData>();
            if (!String.IsNullOrEmpty(e.Result.Content))
            {
                CellSetData cellSet = CellSetData.Deserialize(e.Result.Content);
                members = QueryProvider.GetMembers(cellSet);
            }

            CreateChildNodes(parentNode, members);
        }

        MemberVisualizationTypes m_MemberVisualizationType = MemberVisualizationTypes.Caption;
        public MemberVisualizationTypes MemberVisualizationType
        {
            get { return m_MemberVisualizationType; }
            set
            {
                m_MemberVisualizationType = value;
                RefreshTree();
            }
        }

        void CreateChildNodes(CustomTreeNode parentNode, List<MemberData> members)
        {
            int nodes_count = 0;
            nodes_count = parentNode != null ? parentNode.Items.Count : Tree.Items.Count;

            if (members == null || members.Count == 0)
                return;

            if (parentNode != null)
                parentNode.IsFullLoaded = true;
            else
                Tree.IsFullLoaded = true;

            int indx = 0;
            MemberLiteTreeNode node = null;
            foreach (MemberData wrapper in members)
            {
                // Грузим Step+1 элемент, а отображать будем только Step штук. Если в ответе придет Step+1 элемент, то значит нужно отображать узел LoadNext
                if (indx < Step)
                {
                    // Создание нового узла для добавления в дерево
                    node = new MemberLiteTreeNode(wrapper);
                    node.MemberVisualizationType = MemberVisualizationType;
                    AllowDragDrop(node);
                    node.Special_MouseDoubleClick += new EventHandler<CustomEventArgs<CustomTreeNode>>(Node_Special_MouseDoubleClick);

                    if (parentNode == null)
                        Tree.Items.Add(node);
                    else
                        parentNode.Items.Add(node);

                    //Если есть дочерние, то добавляем фиктивный узел для отображения [+] напротив данного узла
                    if (!node.IsExpanded && QueryProvider.GetRealChildrenCount(wrapper) > 0)
                    {
                        node.IsWaiting = true;
                        node.Expanded += new RoutedEventHandler(MemberNode_Expanded);
                    }
                }
                indx++;
            }

            // Грузим Step+1 элемент, а отображать будем только Step штук. Если в ответе придет Step+1 элемент, то значит нужно отображать узел LoadNext
            if (members.Count > Step)
            {
                if (parentNode != null)
                {
                    parentNode.IsFullLoaded = false;
                    // У родительского узла подписываемся на спец. событие
                    parentNode.Special_MouseDoubleClick -= new EventHandler<CustomEventArgs<CustomTreeNode>>(Node_Special_MouseDoubleClick);
                    parentNode.Special_MouseDoubleClick += new EventHandler<CustomEventArgs<CustomTreeNode>>(Node_Special_MouseDoubleClick);
                }
                else
                {
                    Tree.IsFullLoaded = false;
                    // У дерева узла подписываемся на спец. событие
                    Tree.Special_MouseDoubleClick += new EventHandler<CustomEventArgs<CustomTreeNode>>(Node_Special_MouseDoubleClick);
                    Tree.Special_MouseDoubleClick += new EventHandler<CustomEventArgs<CustomTreeNode>>(Node_Special_MouseDoubleClick);
                }

                // если до добавления узлов у parentNode уже были дочерние, то значит произведена дозагрузка элементов. В этом случае выбранным делаем узел LoadNext
                if (nodes_count > 0)
                {
                    foreach (object obj in parentNode != null ? parentNode.Items : Tree.Items)
                    {
                        LoadNextTreeNode loadNextNode = obj as LoadNextTreeNode;
                        if (loadNextNode != null)
                        {
                            // Выбор делаем через событие, иначе не скроллируется к выбранному элементу
                            loadNextNode.Loaded += new RoutedEventHandler(loadNextNode_Loaded);
                            break;
                        }
                    }
                }
            }
            else
            { 
                // если до добавления узлов у parentNode уже были дочерние, то значит произведена дозагрузка элементов. В этом случае выбранным делаем последний из добавленных
                if (nodes_count > 0 && node != null)
                {
                    // Выбор делаем через событие, иначе не скроллируется к выбранному элементу
                    node.Loaded += new RoutedEventHandler(node_Loaded);
                }
            }


            Tree.UpdateLayout();
        }

        void node_Loaded(object sender, RoutedEventArgs e)
        {
            MemberLiteTreeNode memberNode = sender as MemberLiteTreeNode;
            if (memberNode != null)
            {
                memberNode.IsSelected = true;
                memberNode.Loaded -= new RoutedEventHandler(node_Loaded);
            }         
        }

        void loadNextNode_Loaded(object sender, RoutedEventArgs e)
        {
            LoadNextTreeNode loadNextNode = sender as LoadNextTreeNode;
            if (loadNextNode != null)
            {
                loadNextNode.IsSelected = true;
                loadNextNode.Loaded -= new RoutedEventHandler(loadNextNode_Loaded);
            }
        }

        void MemberNode_Expanded(object sender, RoutedEventArgs e)
        {
            MemberLiteTreeNode memberNode = sender as MemberLiteTreeNode;
            if (memberNode != null)
            {
                // Проверяем, грузились ли дочерние элементы
                if (!memberNode.IsInitialized && memberNode.Info != null)
                {
                    // Загружаем дочерние элементы
                    // Грузим Step+1 элемент, а отображать будем только Step штук. Если в ответе придет Step+1 элемент, то значит нужно отображать узел LoadNext
                    LoadChildrenMembers(memberNode, 0, Step + 1);
                }
            }
        }

        /// <summary>
        /// Клик на специальном узле
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Node_Special_MouseDoubleClick(object sender, CustomEventArgs<CustomTreeNode> e)
        {
            CustomTree tree = sender as CustomTree;
            CustomTreeNode node = sender as CustomTreeNode;
            if (node != null || tree != null)
            {
                LoadNextTreeNode loadNext = e.Args as LoadNextTreeNode;
                if (loadNext != null)
                {
                    //Количество загруженных узлов - это количество узлов
                    long loadedChildrenCount = 0;

                    loadNext.IsSelected = false;
                    if (node != null)
                    {
                        node.IsFullLoaded = true;
                        loadedChildrenCount = node.Items.Count;
                    }
                    if (tree != null)
                    {
                        Tree.IsFullLoaded = true;
                        loadedChildrenCount = Tree.Items.Count;
                    }

                    if (tree != null)
                    {
                        // Грузим Step+1 элемент, а отображать будем только Step штук. Если в ответе придет Step+1 элемент, то значит нужно отображать узел LoadNext
                        LoadChildrenMembers(null, loadedChildrenCount, Step + 1);
                    }
                    else
                    {
                        LevelTreeNode levelNode = node as LevelTreeNode;
                        if (levelNode != null)
                        {
                            // Грузим Step+1 элемент, а отображать будем только Step штук. Если в ответе придет Step+1 элемент, то значит нужно отображать узел LoadNext
                            LoadChildrenMembers(levelNode, loadedChildrenCount, Step + 1);
                        }

                        MemberLiteTreeNode memberNode = node as MemberLiteTreeNode;
                        if (memberNode != null)
                        {
                            // Грузим Step+1 элемент, а отображать будем только Step штук. Если в ответе придет Step+1 элемент, то значит нужно отображать узел LoadNext
                            LoadChildrenMembers(memberNode, loadedChildrenCount, Step + 1);
                        }
                    }
                }
            }
        }

        #region Кубы
        void GetCube()
        {
            Tree.Items.Clear();
            Tree.IsWaiting = true;
            m_ComboMeasureGroup.SelectionChanged -= new EventHandler(comboMeasureGroup_SelectionChanged);
            m_ComboMeasureGroup.IsWaiting = true;
            LogManager.LogInformation(this, this.Name + String.Format(" - Loading cube '{0}' metadata", CubeName));
            MetadataQuery args = CommandHelper.CreateGetCubeMetadataArgs(Connection, CubeName, MetadataQueryType.GetCubeMetadata);
            OlapDataLoader.LoadData(args, new UserSchemaWrapper<MetadataQuery, CustomTreeNode>(args, null));
        }

        void GetCubeMetadata_InvokeCommandCompleted(DataLoaderEventArgs e, CustomTreeNode parentNode)
        {
            CubeDefInfo cube = XmlSerializationUtility.XmlStr2Obj<CubeDefInfo>(e.Result.Content);
            Initialize(cube);
        }
        #endregion Кубы

        Dictionary<String, CustomTreeNode> m_KPIGroupNodes = new Dictionary<String, CustomTreeNode>();
        Dictionary<String, CustomTreeNode> m_HierarchyGroupNodes = new Dictionary<String, CustomTreeNode>();
        Dictionary<String, CustomTreeNode> m_MeasuresGroupNodes = new Dictionary<String, CustomTreeNode>();
        protected Dictionary<String, CustomTreeNode> m_NamedSetFolderNodes = new Dictionary<String, CustomTreeNode>();

        public String GetNodeString(CustomTreeNode node)
        {
            if (node != null)
            {
                InfoBaseTreeNode infoNode = node as InfoBaseTreeNode;
                if (infoNode != null && infoNode.Info != null)
                { 
                    if(infoNode.Info is CubeDefInfo)
                    {
                        if (((CubeDefInfo)infoNode.Info).Name.IndexOf(" ") > 0)
                            return OlapHelper.ConvertToQueryStyle(((CubeDefInfo)infoNode.Info).Name);
                        return ((CubeDefInfo)infoNode.Info).Name;
                    }
                    if (infoNode.Info is DimensionInfo)
                    {
                        return ((DimensionInfo)infoNode.Info).UniqueName;
                    }
                    if (infoNode.Info is HierarchyInfo)
                    {
                        return ((HierarchyInfo)infoNode.Info).UniqueName;
                    }
                    if (infoNode.Info is LevelInfo)
                    {
                        return ((LevelInfo)infoNode.Info).UniqueName;
                    }
                    if (infoNode.Info is MeasureInfo)
                    {
                        return ((MeasureInfo)infoNode.Info).UniqueName;
                    }
                    if (infoNode.Info is NamedSetInfo)
                    {
                        return OlapHelper.ConvertToQueryStyle(((NamedSetInfo)infoNode.Info).Name);
                    }

                    KpiInfo kpiInfo = infoNode.Info as KpiInfo;
                    if (kpiInfo != null)
                    {
                        KpiTreeNode kpiNode = infoNode as KpiTreeNode;
                        if (kpiNode != null)
                        {
                            return String.Format("KPIValue(\"{0}\"), KPIGoal(\"{0}\"), KPIStatus(\"{0}\"), KPITrend(\"{0}\")", kpiInfo.Name);
                        }
                        KpiTrendTreeNode kpiTrendNode = infoNode as KpiTrendTreeNode;
                        if (kpiTrendNode != null)
                        {
                            return String.Format("KPITrend(\"{0}\")", kpiInfo.Name);
                        }
                        KpiValueTreeNode kpiValueNode = infoNode as KpiValueTreeNode;
                        if (kpiValueNode != null)
                        {
                            return String.Format("KPIValue(\"{0}\")", kpiInfo.Name);
                        }
                        KpiStatusTreeNode kpiStatusNode = infoNode as KpiStatusTreeNode;
                        if (kpiStatusNode != null)
                        {
                            return String.Format("KPIStatus(\"{0}\")", kpiInfo.Name);
                        }
                        KpiGoalTreeNode kpiGoalNode = infoNode as KpiGoalTreeNode;
                        if (kpiGoalNode != null)
                        {
                            return String.Format("KPIGoal(\"{0}\")", kpiInfo.Name);
                        }
                    }
                }

                MemberLiteTreeNode memberNode = node as MemberLiteTreeNode;
                if (memberNode != null && memberNode.Info != null && memberNode.Info != null)
                {
                    return memberNode.Info.UniqueName;
                }

                CalculatedMemberTreeNode calcMemberNode = node as CalculatedMemberTreeNode;
                if (calcMemberNode != null && calcMemberNode.Info != null)
                {
                    return calcMemberNode.Info.Name;
                }

                CalculatedNamedSetTreeNode calcSetNode = node as CalculatedNamedSetTreeNode;
                if (calcSetNode != null && calcSetNode.Info != null)
                {
                    return calcSetNode.Info.Name;
                }
            }
            return String.Empty;
        }

        public event EventHandler<GetIDataLoaderArgs> GetMembersLoader;
        void Raise_GetMembersLoader(GetIDataLoaderArgs args)
        {
            EventHandler<GetIDataLoaderArgs> handler = this.GetMembersLoader;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        public bool SelectMeasureGroup(String measureGroupName)
        {
            return m_ComboMeasureGroup.SelectItem(measureGroupName);
        }
    }
}
