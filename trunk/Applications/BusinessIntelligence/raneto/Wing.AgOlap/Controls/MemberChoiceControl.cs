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
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Text;
using System.Windows.Media.Imaging;
using Ranet.Olap.Core.Data;
using Ranet.AgOlap.Controls.MemberChoice.Info;
using System.ComponentModel;
using Ranet.AgOlap.Controls.MemberChoice.ClientServer;
using Ranet.AgOlap.Controls.General.Tree;
using System.Threading;
using Ranet.AgOlap.Controls.General;
using Ranet.AgOlap.Commands;
using Ranet.Olap.Core;
using Ranet.AgOlap.Controls.General.ClientServer;
using Ranet.Olap.Core.Metadata;
using Ranet.AgOlap.Controls.MemberChoice.Filter;
using Ranet.AgOlap.Controls.Buttons;
using Ranet.AgOlap.Controls.ToolBar;
using Ranet.AgOlap.Controls.MemberChoice;
using Ranet.AgOlap.Controls.General.ItemControls;
using Ranet.AgOlap.Controls.Tab;
using Ranet.Olap.Core.Providers;
using Ranet.AgOlap.Providers;
using Ranet.Olap.Core.Providers.ClientServer;

namespace Ranet.AgOlap.Controls
{
    // Подумать:
    // Как сделать переход из дерева найденных на дерево выбора, по двойному клику идет раскрытие узла - значит это нам не подходит
    // 
    //
    //
    public class MemberChoiceControl : AgTreeControlBase, IChoiceControl
    {
        CustomTree membersTree = null;

        ComboBoxEx Levels_ComboBox = null;
        RanetToggleButton m_ShowMemberPropertiesButton = null;

        Grid m_PropertiesLayout;
        PropertiesControl m_CurrentMemberControl;
        Border m_PropertiesLayoutBorder;

        TextBlock m_Find_Count;
        TextBlock m_Selected_Count;
    
        RanetTabControl tabControl;
        TabItem membersTab;
        TabItem findTab;
        TabItem mdxSetTab;

        RanetToolBarButton Clear_Choice_Button;

        bool m_Initializing = false;
        public MemberChoiceControl()
        {
            try
            {
                m_Initializing = true;
                Grid LayoutRoot = new Grid();
                LayoutRoot.RowDefinitions.Add(new RowDefinition());
                // Строка для кнопок
                //OK LayoutRoot.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Star), MinWidth = 50 });
                // Колонка для свойств
                LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition() { MinWidth = 50 });

                tabControl = new RanetTabControl();
                tabControl.TabCtrl.SelectionChanged += new SelectionChangedEventHandler(TabControl_SelectionChanged);

                membersTab = new TabItem();
                membersTab.Header = Localization.Members;
                membersTab.Style = tabControl.Resources["TabControlOutputItem"] as Style;

                findTab = new TabItem();
                findTab.Header = Localization.Find;
                findTab.Style = tabControl.Resources["TabControlOutputItem"] as Style;

                mdxSetTab = new TabItem();
                mdxSetTab.Header = Localization.MemberChoice_Selected;
                mdxSetTab.Style = tabControl.Resources["TabControlOutputItem"] as Style;

                tabControl.TabCtrl.Items.Add(membersTab);
                tabControl.TabCtrl.Items.Add(findTab);
                tabControl.TabCtrl.Items.Add(mdxSetTab);

                LayoutRoot.Children.Add(tabControl);

                //OK StackPanel buttonsPanel = new StackPanel() { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right };
                //LayoutRoot.Children.Add(buttonsPanel);
                //Grid.SetRow(buttonsPanel, 1);

                //Border border = new Border() { BorderBrush = new SolidColorBrush(Colors.DarkGray), BorderThickness = new Thickness(1), Padding = new Thickness(5), Background = new SolidColorBrush(Colors.White) };
                //border.Child = LayoutRoot;

                // Растягиваем дерево
                Grid.SetColumnSpan(tabControl, 2);

                // Закладка MEMBERS
                Grid membersTab_LayoutRoot = new Grid();
                membersTab.Content = membersTab_LayoutRoot;
                membersTab_LayoutRoot.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                membersTab_LayoutRoot.RowDefinitions.Add(new RowDefinition());

                membersTree = new CustomTree() { BorderBrush = new SolidColorBrush(Colors.DarkGray) };
                membersTree.Margin = new Thickness(0, 3, 0, 0);
                membersTree.SelectedItemChanged += new RoutedPropertyChangedEventHandler<object>(membersTree_SelectedItemChanged);
                membersTab_LayoutRoot.Children.Add(membersTree);
                Grid.SetRow(membersTree, 1);

                GridSplitter splitter_Vert = new GridSplitter();
                splitter_Vert.IsTabStop = false;
                LayoutRoot.Children.Add(splitter_Vert);
                Grid.SetColumn(splitter_Vert, 0);
                Grid.SetRow(splitter_Vert, 0);
                splitter_Vert.Background = new SolidColorBrush(Colors.Transparent);
                splitter_Vert.HorizontalAlignment = HorizontalAlignment.Right;
                splitter_Vert.VerticalAlignment = VerticalAlignment.Stretch;
                splitter_Vert.Width = 3;

                // Свойства элемента
                m_CurrentMemberControl = new PropertiesControl();
                m_CurrentMemberControl.Background = new SolidColorBrush(Color.FromArgb(0x7F, 128, 128, 128));
                m_CurrentMemberControl.PropertiesList.PropertyColumnWidth = new DataGridLength(100);
                m_PropertiesLayoutBorder = new Border() { BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0x80, 0x80, 0x80)), BorderThickness = new Thickness(1) };
                m_PropertiesLayoutBorder.Child = m_CurrentMemberControl;
                LayoutRoot.Children.Add(m_PropertiesLayoutBorder);
                Grid.SetRow(m_PropertiesLayoutBorder, 0);
                Grid.SetColumn(m_PropertiesLayoutBorder, 1);

                // Прячем свойства
                m_PropertiesLayoutBorder.Visibility = Visibility.Collapsed;

                StackPanel members_toolBar = new StackPanel();
                members_toolBar.Orientation = Orientation.Horizontal;

                membersTab_LayoutRoot.Children.Add(members_toolBar);
                Grid.SetRow(members_toolBar, 0);
                Grid.SetColumnSpan(members_toolBar, 2);

                RanetToolBarButton Refresh_Button = new RanetToolBarButton();
                Refresh_Button.Margin = new Thickness(0, 0, 0, 0);
                Refresh_Button.Click += new RoutedEventHandler(RefreshButton_Click);
                Refresh_Button.Content = UiHelper.CreateIcon(UriResources.Images.Refresh16);
                ToolTipService.SetToolTip(Refresh_Button, Localization.MemberChoice_Refresh_ToolTip);
                members_toolBar.Children.Add(Refresh_Button);

                Clear_Choice_Button = new RanetToolBarButton();
                Clear_Choice_Button.Click += new RoutedEventHandler(Clear_Choice_Button_Click);
                Clear_Choice_Button.Content = UiHelper.CreateIcon(UriResources.Images.ClearChoice16);
                ToolTipService.SetToolTip(Clear_Choice_Button, Localization.MemberChoice_ClearSelection_ToolTip);
                members_toolBar.Children.Add(Clear_Choice_Button);

                Levels_ComboBox = new ComboBoxEx();
                Levels_ComboBox.Margin = new Thickness(2, 0, 0, 0);
                Levels_ComboBox.Width = 150;
                members_toolBar.Children.Add(Levels_ComboBox);

                //OK buttonsPanel.Margin = new Thickness(0, 5, 0, 0);
                //Grid.SetColumnSpan(buttonsPanel, 2);
                //OkButton = new RanetButton() { Width = 75, HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(0, 0, 10, 0) };
                //OkButton.Content = Localization.DialogButton_Ok;
                //OkButton.Height = 22;
                //OkButton.Click += new RoutedEventHandler(OkButton_Click);
                //buttonsPanel.Children.Add(OkButton);
                //CancelButton = new RanetButton() { Width = 75, HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(0, 0, 0, 0) };
                //CancelButton.Content = Localization.DialogButton_Cancel;
                //CancelButton.Height = 22;
                //CancelButton.Click += new RoutedEventHandler(CancelButton_Click);
                //buttonsPanel.Children.Add(CancelButton);

                membersTree.Special_MouseDoubleClick += new EventHandler<CustomEventArgs<CustomTreeNode>>(node_SpecialNodeExpanded);

                // Закладка ПОИСК
                Grid findTab_LayoutRoot = new Grid();
                findTab.Content = findTab_LayoutRoot;

                findTab_LayoutRoot.RowDefinitions.Add(new RowDefinition());
                findTab_LayoutRoot.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                findTab_LayoutRoot.RowDefinitions.Add(new RowDefinition());
                findTab_LayoutRoot.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(22) });

                filterBuilder = new FilterBuilderControl();
                filterBuilder.ApplyFilter += new EventHandler(filterBuilder_ApplyFilter);
                findTab_LayoutRoot.Children.Add(filterBuilder);

                RanetButton RunSearch_Button = new RanetButton();
                RunSearch_Button.Height = 22;
                RunSearch_Button.Width = 75;
                RunSearch_Button.Margin = new Thickness(2, 5, 0, 0);
                RunSearch_Button.Content = Localization.Find;
                RunSearch_Button.HorizontalAlignment = HorizontalAlignment.Right;
                RunSearch_Button.Click += new RoutedEventHandler(RunSearch_Button_Click);

                findTab_LayoutRoot.Children.Add(RunSearch_Button);
                Grid.SetRow(RunSearch_Button, 1);

                // Дерево с результатом поиска
                findResultTree = new CustomTree() { BorderBrush = new SolidColorBrush(Colors.DarkGray) };
                findResultTree.SelectedItemChanged += new RoutedPropertyChangedEventHandler<object>(tree_SelectedItemChanged);
                findResultTree.Margin = new Thickness(0, 5, 0, 0);
                findTab_LayoutRoot.Children.Add(findResultTree);
                Grid.SetRow(findResultTree, 2);

                // Количество найденных элементов
                Grid find_buttons_panel = new Grid();
                find_buttons_panel.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                find_buttons_panel.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

                TextBlock m_Find_Results = new TextBlock() { Text = Localization.MemberChoice_MembersFound };
                m_Find_Results.VerticalAlignment = VerticalAlignment.Center;
                find_buttons_panel.Children.Add(m_Find_Results);

                m_Find_Count = new TextBlock() { Text = "0" };
                m_Find_Count.VerticalAlignment = VerticalAlignment.Center;
                find_buttons_panel.Children.Add(m_Find_Count);
                Grid.SetColumn(m_Find_Count, 1);

                findTab_LayoutRoot.Children.Add(find_buttons_panel);
                Grid.SetRow(find_buttons_panel, 3);

                // Закладка ВЫБРАННЫЕ
                Grid mdxSetTab_LayoutRoot = new Grid();
                mdxSetTab.Content = mdxSetTab_LayoutRoot;
                mdxSetTab_LayoutRoot.RowDefinitions.Add(new RowDefinition());
                mdxSetTab_LayoutRoot.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(22) });
                mdxSetTab_LayoutRoot.RowDefinitions.Add(new RowDefinition());

                selectedList = new CustomTree() { BorderBrush = new SolidColorBrush(Colors.DarkGray) };
                selectedList.SelectedItemChanged += new RoutedPropertyChangedEventHandler<object>(tree_SelectedItemChanged);
                mdxSetTab_LayoutRoot.Children.Add(selectedList);

                // Количество выбранных элементов
                Grid selected_buttons_panel = new Grid();
                selected_buttons_panel.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                selected_buttons_panel.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

                TextBlock m_Selected_Results = new TextBlock() { Text = Localization.MemberChoice_MembersSelected };
                m_Selected_Results.Margin = new Thickness(0, 5, 0, 5);
                m_Selected_Results.VerticalAlignment = VerticalAlignment.Center;
                selected_buttons_panel.Children.Add(m_Selected_Results);

                m_Selected_Count = new TextBlock() { Text = "0" };
                m_Selected_Count.VerticalAlignment = VerticalAlignment.Center;
                m_Selected_Count.Margin = new Thickness(0, 5, 0, 5);
                selected_buttons_panel.Children.Add(m_Selected_Count);
                Grid.SetColumn(m_Selected_Count, 1);

                mdxSetTab_LayoutRoot.Children.Add(selected_buttons_panel);
                Grid.SetRow(selected_buttons_panel, 1);

                mdxSetTextBox = new SimpleTextBox() { AcceptsReturn = true, TextWrapping = TextWrapping.Wrap, IsReadOnly = true };
                mdxSetTextBox.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                mdxSetTextBox.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                mdxSetTextBox.Margin = new Thickness(0, 0, 0, 0);
                mdxSetTextBox.BorderBrush = new SolidColorBrush(Colors.Black);
                mdxSetTab_LayoutRoot.Children.Add(mdxSetTextBox);
                Grid.SetRow(mdxSetTextBox, 2);

                SelectedItemChanged_Story = new Storyboard();
                SelectedItemChanged_Story.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 500));
                SelectedItemChanged_Story.Completed += new EventHandler(SelectedItemChanged_Story_Completed);
                membersTab_LayoutRoot.Resources.Add("SelectedItemChanged_Story", SelectedItemChanged_Story);

                // Кнопка для отображения свойств элемента
                m_ShowMemberPropertiesButton = new RanetToggleButton();

                TabToolBar toolbar = tabControl.ToolBar;
                if (toolbar != null)
                {
                    ToolTipService.SetToolTip(m_ShowMemberPropertiesButton, Localization.MemberChoice_ShowMemberProperties_ToolTip);
                    m_ShowMemberPropertiesButton.Click += new RoutedEventHandler(SowMemberPropertiesButton_Click);
                    m_ShowMemberPropertiesButton.Content = UiHelper.CreateIcon(UriResources.Images.MemberProperty16);
                    toolbar.Stack.Children.Add(m_ShowMemberPropertiesButton);
                }

                this.Content = LayoutRoot;
            }
            finally {
                m_Initializing = false;
            }
        }

        public readonly QueryProvider DataManager = new QueryProvider();

        void ChangeCurrentMember(MemberTreeNode node)
        {
            if (node != null)
            {
                m_CurrentMember = node.MemberInfo.Info;
            }
            else
            {
                m_CurrentMember = null;
            }
        }

        void tree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ChangeCurrentMember(e.NewValue as MemberTreeNode);

            if (m_ShowMemberPropertiesButton.IsChecked.Value)
            {
                SelectedItemChanged_Story.Stop();
                SelectedItemChanged_Story.Begin();
            }
        }

        void SowMemberPropertiesButton_Click(object sender, RoutedEventArgs e)
        {
            if (m_ShowMemberPropertiesButton.IsChecked.Value)
            {
                Grid.SetColumnSpan(tabControl, 1);
                m_PropertiesLayoutBorder.Visibility = Visibility.Visible;

                MemberTreeNode treeNode = membersTree.SelectedItem as MemberTreeNode;
                if (treeNode != null)
                {
                    m_CurrentMember = treeNode.MemberInfo.Info;
                }
                else
                {
                    m_CurrentMember = null;
                }
                ShowMemberProperties();
            }
            else
            {
                Grid.SetColumnSpan(tabControl, 2);
                m_PropertiesLayoutBorder.Visibility = Visibility.Collapsed;
            }
        }

        Storyboard SelectedItemChanged_Story;

        void SelectedItemChanged_Story_Completed(object sender, EventArgs e)
        {
            ShowMemberProperties();
        }
        void filterBuilder_ApplyFilter(object sender, EventArgs e)
        {
            RunSearch();
        }

        CustomTree findResultTree;
        CustomTree selectedList;

        void selectedList_ItemSelected(object sender, ItemSelectedEventArgs e)
        {
            SelectMember(e.UniqueName);
        }

        TextBox mdxSetTextBox;

        void ClearCustomSelection()
        {
            membersTree.SelectedItemChanged -= new RoutedPropertyChangedEventHandler<object>(membersTree_SelectedItemChanged);

            foreach (TreeViewItem item in m_CustomSelection)
            {
                item.IsSelected = false;
            }

            m_CustomSelection.Clear();

            membersTree.SelectedItemChanged += new RoutedPropertyChangedEventHandler<object>(membersTree_SelectedItemChanged);

        }

        bool m_IsReadyToSelection = false;
        public bool IsReadyToSelection
        {
            get
            {
                return m_IsReadyToSelection;
            }
        }

        void membersTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            MemberTreeNode treeNode = e.NewValue as MemberTreeNode;
            // Если одиночный выбор элементов, и включен режим выбирать только листы, то кнопку ОК делаем доступной только для листьев 
            if (MultiSelect == false && SelectLeafs)
            { 
                if(treeNode == null)
                {
                    m_IsReadyToSelection = false;
                }
                else
                {
                    if (treeNode.MemberInfo != null && treeNode.MemberInfo.CubeChildrenCount == 0)
                    {
                        m_IsReadyToSelection = true;
                    }
                    else
                    {
                        m_IsReadyToSelection = false;
                    }
                }
            }

            ClearCustomSelection();

            if (treeNode != null)
            {
                m_CurrentMember = treeNode.MemberInfo.Info;
            }
            else
            {
                m_CurrentMember = null;
            }

            if (m_ShowMemberPropertiesButton.IsChecked.Value)
            {
                SelectedItemChanged_Story.Stop();
                SelectedItemChanged_Story.Begin();
            }

            Raise_SelectedItemChanged(null);
            
            if (MultiSelect == false)
            {
                GenerateSetBySelectionState();
            }
        }

        MemberData m_CurrentMember = null;

        void ShowMemberProperties()
        {
            if (m_ShowMemberPropertiesButton.IsChecked.Value)
            {
                if (m_CurrentMember != null && m_Properties != null)
                {
                    m_CurrentMemberControl.IsLoading = true;
                    String query = DataManager.GetMember(m_CurrentMember.UniqueName, m_Properties);
                    LogManager.LogInformation(this, this.Name + " - Loading member properties.");
                    MdxQueryArgs query_args = CommandHelper.CreateMdxQueryArgs(Connection, query);
                    OlapDataLoader.LoadData(query_args, new UserSchemaWrapper<MemberChoiceQueryType, MemberTreeNode>(MemberChoiceQueryType.GetMember, null));
                }
                else
                {
                    m_CurrentMemberControl.Clear();
                }
            }
        }

        void membersList_ItemSelected(object sender, ItemSelectedEventArgs e)
        {
            SelectMember(e.UniqueName);
        }

        private void SelectMember(String uniqueName)
        {
            //ClearCustomSelection();

            if (!String.IsNullOrEmpty(uniqueName))
            {
                MemberTreeNode node = null;
                if (m_TreeNodes.ContainsKey(uniqueName))
                {
                    node = m_TreeNodes[uniqueName];
                }

                if (node != null)
                {
                    // Все вышестоящие узлы раскрываем
                    CustomTreeNode parentNode = node.Parent as CustomTreeNode;
                    while (parentNode != null)
                    {
                        parentNode.IsExpanded = true;
                        parentNode = parentNode.Parent as CustomTreeNode;
                    }
                    // Если узел в дереве найден, то нужно просто переключиться на него
                    SelectNode(node);

                    tabControl.TabCtrl.SelectedItem = membersTab;
                }
                else
                {
                    //Необходимо дозагрузить все узлы предки и сам узел и отобразить его в дереве
                    LoadAscendants(uniqueName);
                }
            }
        }

        void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            //m_OlapMemberInfoHierarchy = new RootOlapMemberInfo(MemberData.Empty);

            //ClearTree();

            ////Получаем элементы
            //if (useStepLoading)
            //    LoadRootMembers(0, Step);
            //else
            //    LoadRootMembers(-1, -1);
            RefreshTree(false);
        }

        void RunSearch_Button_Click(object sender, RoutedEventArgs e)
        {
            RunSearch();
        }

        FilterBuilderControl filterBuilder;
        //RanetButton OkButton;
        //RanetButton CancelButton;

        /// <summary>
        /// Очистить выбранные
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Clear_Choice_Button_Click(object sender, RoutedEventArgs e)
        {
            OlapMemberInfoHierarchy.ClearMembersState();
            GenerateSetBySelectionState();
        }

        /// <summary>
        /// Событие генерируется при изменении состояния выбранности любого узла дерева
        /// </summary>
        public event EventHandler SelectedInfoChanged;

        void Raise_SelectedInfoChanged()
        {
            EventHandler handler = SelectedInfoChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        #region Свойства

        #region Свойства для настройки на OLAP
        /// <summary>
        /// 
        /// </summary>
        private String m_Connection = String.Empty;
        /// <summary>
        /// Описание соединения с БД для идентификации соединения на сервере (строка соединения либо ID)
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

        /// <summary>
        /// Имя OLAP куба
        /// </summary>
        String m_CubeName = String.Empty;
        /// <summary>
        /// Имя OLAP куба
        /// </summary>
        public String CubeName
        {
            get
            {
                return this.m_CubeName;
            }
            set
            {
                m_CubeName = value;
                DataManager.Cube = value;
            }
        }

        String m_SubCube = String.Empty;
        /// <summary>
        /// Выражение для под-куба
        /// </summary>
        public String SubCube
        {
            get
            {
                return this.m_SubCube;
            }
            set
            {
                m_SubCube = value;
                DataManager.SubCube = value;
            }
        }

        /// <summary>
        /// Имя иерархии измерения
        /// </summary>
        String m_HierarchyName = String.Empty;
        /// <summary>
        /// Имя иерархии измерения
        /// </summary>
        public String HierarchyUniqueName
        {
            get
            {
                return this.m_HierarchyName;
            }
            set
            {
                m_HierarchyName = value;
                DataManager.HierarchyUniqueName = value;
            }
        }
        #endregion Свойства для настройки на OLAP

        #region Частичная загрузка
        private int m_Step = 100;
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

        /// <summary>
        /// Использование частичной загрузки
        /// </summary>
        private bool useStepLoading = true;
        /// <summary>
        /// Использование частичной загрузки
        /// </summary>
        public bool UseStepLoading
        {
            get
            {
                return useStepLoading;
            }
            set
            {
                useStepLoading = value;
            }
        }
        #endregion Частичная загрузка

        private bool m_SelectLeafs = false;
        /// <summary>
        /// Выбирать только листья
        /// </summary>
        public bool SelectLeafs
        {
            get
            {
                return m_SelectLeafs;
            }
            set
            {
                m_SelectLeafs = value;
            }
        }

        private string startLevelUniqueName = String.Empty;
        /// <summary>
        /// Уникальное имя уровня, начиная с которого элементы будут отображаться в дереве
        /// </summary>
        public string StartLevelUniqueName
        {
            get
            {
                return startLevelUniqueName;
            }
            set
            {
                startLevelUniqueName = value;
                //rootTreeListNodesCount = -1;
            }
        }

        String CurrentLevelUniqueName
        {
            get
            {
                LevelItemControl ctrl = Levels_ComboBox.Combo.SelectedItem as LevelItemControl;
                if (ctrl != null)
                {
                    return ctrl.Info.UniqueName;
                }
                return StartLevelUniqueName;
            }
        }

        /// <summary>
        /// Флажок. Выводить элементы ТОЛЬКО верхнего уровня
        /// </summary>
        private bool m_ShowOnlyFirstLevelMembers = false;
        /// <summary>
        /// Флажок. Выводить элементы ТОЛЬКО верхнего уровня
        /// </summary>
        public bool ShowOnlyFirstLevelMembers
        {
            set
            {
                m_ShowOnlyFirstLevelMembers = value;
                // Лочим контрол выбора уровня если нужно отображать элементы только верхнего уровня
                Levels_ComboBox.IsEnabled = !value;
            }
            get
            {
                return m_ShowOnlyFirstLevelMembers;
            }
        }
        #endregion Свойства

        /// <summary>
        /// Создает на основании схемы информацию об уровнях, инициализирует список уровней TreeBarManager
        /// </summary>
        private void CreateLevelsInfo()
        {
            Levels_ComboBox.IsEnabled = false;
            Levels_ComboBox.Clear();

            Levels_ComboBox.SelectionChanged -= new SelectionChangedEventHandler(Levels_ComboBox_SelectionChanged);

            if (String.IsNullOrEmpty(CubeName))
            {
                LogManager.LogError(this, String.Format(Localization.ControlPropertyNotInitialized_Message, Localization.CubeName_PropertyDesc));
                return;
            }

            if (String.IsNullOrEmpty(HierarchyUniqueName))
            {
                LogManager.LogError(this, String.Format(Localization.ControlPropertyNotInitialized_Message, Localization.HierarchyUniqueName_PropertyDesc));
                return;
            }

            Levels_ComboBox.IsWaiting = true;
            LogManager.LogInformation(this, this.Name + " - Loading levels information.");
            MetadataQuery args = CommandHelper.CreateGetLevelsQueryArgs(Connection, CubeName, String.Empty, HierarchyUniqueName);
            OlapDataLoader.LoadData(args, args);
        }

        void InitLevelsList(List<LevelInfo> levels)
        {
            Levels_ComboBox.Clear();
            Levels_ComboBox.IsEnabled = !ShowOnlyFirstLevelMembers;

            LevelItemControl startLevelItem = null;
            if (levels != null)
            {
                int indx = 0;
                bool useAllLevel = true;
                foreach (LevelInfo info in levels)
                {
                    LevelItemControl ctrl = new LevelItemControl(info);

                    //Если нулевой уровень не All, то иконку ставим как для уровня 1
                    if (indx == 0 && info.LevelType != LevelInfoTypeEnum.All)
                        useAllLevel = false;
                    if (!useAllLevel)
                        ctrl.UseAllLevelIcon = false;

                    Levels_ComboBox.Combo.Items.Add(ctrl);

                    if (!String.IsNullOrEmpty(StartLevelUniqueName) && info.UniqueName == StartLevelUniqueName)
                        startLevelItem = ctrl;

                    indx++;
                }
            }

            if (Levels_ComboBox.Combo.Items.Count > 0)
            {
                if (startLevelItem != null)
                {
                    Levels_ComboBox.Combo.SelectedItem = startLevelItem;
                }
                else
                {
                    Levels_ComboBox.Combo.SelectedIndex = 0;
                }
            }

            Levels_ComboBox.SelectionChanged -= new SelectionChangedEventHandler(Levels_ComboBox_SelectionChanged);
            Levels_ComboBox.SelectionChanged += new SelectionChangedEventHandler(Levels_ComboBox_SelectionChanged);
        }

        void Levels_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LevelItemControl ctrl = Levels_ComboBox.Combo.SelectedItem as LevelItemControl;
            if (ctrl != null)
            {
                RefreshTree(false);
            }
        }

        IDataLoader m_OlapDataLoader = null;
        public IDataLoader OlapDataLoader
        {
            set
            {
                m_OlapDataLoader = value;
                m_OlapDataLoader.DataLoaded += new EventHandler<DataLoaderEventArgs>(OlapDataLoader_DataLoaded);
            }
            get
            {
                if (m_OlapDataLoader == null)
                {
                    m_OlapDataLoader = new OlapDataLoader(URL);
                    m_OlapDataLoader.DataLoaded += new EventHandler<DataLoaderEventArgs>(OlapDataLoader_DataLoaded);
                }
                return m_OlapDataLoader;
            }
        }

        List<LevelPropertyInfo> m_Properties = null;

        void OlapDataLoader_DataLoaded(object sender, DataLoaderEventArgs e)
        {
            MemberTreeNode parentNode = null;

            UserSchemaWrapper<MemberChoiceQueryType, MemberTreeNode> wrapper = e.UserState as UserSchemaWrapper<MemberChoiceQueryType, MemberTreeNode>;
            if (wrapper != null)
            {
                if (wrapper.Schema != MemberChoiceQueryType.GetAscendants &&
                    wrapper.Schema != MemberChoiceQueryType.GetMember &&
                    wrapper.Schema != MemberChoiceQueryType.GetMembers &&
                    wrapper.Schema != MemberChoiceQueryType.LoadSetWithAscendants &&
                    wrapper.Schema != MemberChoiceQueryType.FindMembers)
                {
                    parentNode = wrapper.UserData;
                    if (parentNode != null)
                    {
                        parentNode.IsWaiting = false;
                    }
                    else
                    {
                        membersTree.IsWaiting = false;
                    }
                }
            }


            MetadataQuery args = e.UserState as MetadataQuery;

            if (e.Error != null)
            {
                if (args != null)
                {
                    if (args.QueryType == MetadataQueryType.GetLevels)
                        Levels_ComboBox.IsError = true;
                }

                LogManager.LogError(this, e.Error.ToString());
                return;
            }

            if (e.Result.ContentType == InvokeContentType.Error)
            {
                if (args != null)
                {
                    if (args.QueryType == MetadataQueryType.GetLevels)
                        Levels_ComboBox.IsError = true;
                }

                LogManager.LogError(this, e.Result.Content);
                return;
            }

            if (args != null)
            {
                switch (args.QueryType)
                {
                    case MetadataQueryType.GetLevels:
                        List<LevelInfo> levels = XmlSerializationUtility.XmlStr2Obj<List<LevelInfo>>(e.Result.Content);
                        InitLevelsList(levels);
                        break;
                    case MetadataQueryType.GetLevelProperties:
                        filterBuilder.Tree.IsWaiting = false;
                        List<LevelPropertyInfo> properties = XmlSerializationUtility.XmlStr2Obj<List<LevelPropertyInfo>>(e.Result.Content);
                        m_Properties = properties;

                        MemberTreeNode treeNode = membersTree.SelectedItem as MemberTreeNode;
                        if (treeNode != null)
                        {
                            m_CurrentMember = treeNode.MemberInfo.Info;
                        }
                        else
                        {
                            m_CurrentMember = null;
                        }
                        ShowMemberProperties();
                        filterBuilder.Initialize(properties);
                        m_LevelPropertiesIsLoaded = true;

                        break;
                }
            }

            if (wrapper != null)
            {
                switch (wrapper.Schema)
                {
                    //case MemberChoiceQueryType.GetRootMembersCount:
                    //    Service_GetRootMembersCountCompleted(e, parentNode);
                    //    break;
                    case MemberChoiceQueryType.GetRootMembers:
                        service_GetRootMembersCompleted(e, parentNode);
                        break;
                    case MemberChoiceQueryType.GetChildrenMembers:
                        Service_GetChildrenMembersCompleted(e, parentNode);
                        break;
                    case MemberChoiceQueryType.FindMembers:
                        FindMembers_InvokeCommandCompleted(e, parentNode);
                        break;
                    case MemberChoiceQueryType.GetAscendants:
                        GetAscendants_InvokeCommandCompleted(e);
                        break;
                    case MemberChoiceQueryType.GetMember:
                        GetMember_InvokeCommandCompleted(e);
                        break;
                    case MemberChoiceQueryType.LoadSetWithAscendants:
                        LoadSetWithAscendants_InvokeCommandCompleted(e);
                        break;
                    case MemberChoiceQueryType.GetMembers:
                        GetMembers_InvokeCommandCompleted(e);
                        break;
                }
            }
        }

        void GetMember_InvokeCommandCompleted(DataLoaderEventArgs e)
        {
            List<MemberData> members = new List<MemberData>();
            if (!String.IsNullOrEmpty(e.Result.Content))
            {
                CellSetData cellSet = CellSetData.Deserialize(e.Result.Content);
                members = QueryProvider.GetMembers(cellSet);
            }

            MemberData member = null;
            if (members.Count > 0)
                member = members[0];

            if (member != null)
            {
                m_CurrentMemberControl.Initialize(member);
            }
            else
            {
                m_CurrentMemberControl.Clear();
            }
        }

        bool m_IsInitialized = false;
        public void Initialize()
        {
            // Чистим фильтр для поиска
            filterBuilder.Tree.Items.Clear();
            // Чистим результат поиска
            findResultTree.Items.Clear();

            CreateLevelsInfo();
            RefreshTree(true);

            m_IsInitialized = true;
        }

        //long m_RootMembersCubeCount = 0;

        /// <summary>
        /// Перерисовывает дерево. Информация о выбранных элементах остается
        /// </summary>
        private void RefreshTree(bool reloadSelection)
        {
            if (MultiSelect == false && SelectLeafs)
            {
                m_IsReadyToSelection = false;
            }
            else
            {
                m_IsReadyToSelection = true;
            }

            ////Чистим дерево
            //Tree.Nodes.Clear();
            //m_TreeNodes.Clear();

            ClearTree();

            //m_RootMembersCubeCount = 0;

            m_LevelPropertiesIsLoaded = false;

            if (reloadSelection)
            {
                //Формируем иерархию OlapMemberInfo
                m_OlapMemberInfoHierarchy = BuildHierarсhyBySelection();
                //m_OlapMemberInfoHierarchy = BuildHierarсhyBySelection(SelectedInfo);
            }
            else
            {
                Load();
                //GetRootMembersCount();
            }

            m_IsInitialized = true;
        }

        private void GetRootMembersCount()
        {
            String query = String.Empty;
            if (String.IsNullOrEmpty(CurrentLevelUniqueName))
                query = DataManager.GetMembersCount(0);
            else
                query = DataManager.GetMembersCount(CurrentLevelUniqueName);

            LogManager.LogInformation(this, this.Name + " - Calculating root members count.");
            MdxQueryArgs query_args = CommandHelper.CreateMdxQueryArgs(Connection, query);
            OlapDataLoader.LoadData(query_args, new UserSchemaWrapper<MemberChoiceQueryType, MemberTreeNode>(MemberChoiceQueryType.GetRootMembersCount, null));
        }

        void ClearTree()
        {
            membersTree.Items.Clear();
            m_TreeNodes.Clear();
        }

        void SetFilterBuilderToError()
        {
            // Чистим фильтр для поиска
            filterBuilder.Tree.Items.Clear();
            filterBuilder.Tree.IsWaiting = false;
            filterBuilder.Tree.IsError = true;
        }

        //void Service_GetRootMembersCountCompleted(DataLoaderEventArgs e, MemberTreeNode parentNode)
        void Load()
        {
            membersTree.IsWaiting = false;
            membersTree.IsFullLoaded = true;

            //m_RootMembersCubeCount = 0;

            //if (e.Result.ContentType == InvokeContentType.Error)
            //{
            //    ClearTree();
            //    membersTree.IsError = true;

            //    SetFilterBuilderToError();
            //    return;
            //}

            //if (!String.IsNullOrEmpty(e.Result.Content))
            //{
            //    CellSetData cellSet = CellSetData.Deserialize(e.Result.Content);
            //    m_RootMembersCubeCount = DataManager.GetCount(cellSet);
            //}

            //Получаем элементы
            if (useStepLoading)
                LoadRootMembers(0, Step);
            else
                LoadRootMembers(-1, -1);
        }

        private void LoadRootMembers(long begin, long count)
        {
            ClearTree();
            membersTree.IsWaiting = true;

            String query = String.Empty;
            if (String.IsNullOrEmpty(CurrentLevelUniqueName))
                query = DataManager.GetHierarchyMembers(0, begin, count);
            else
                query = DataManager.GetLevelMembers(CurrentLevelUniqueName, begin, count);

            LogManager.LogInformation(this, this.Name + " - Loading root members.");
            MdxQueryArgs query_args = CommandHelper.CreateMdxQueryArgs(Connection, query);
            OlapDataLoader.LoadData(query_args, new UserSchemaWrapper<MemberChoiceQueryType, MemberTreeNode>(MemberChoiceQueryType.GetRootMembers, null));
        }

        void SelectNode(MemberTreeNode node)
        {
            ClearCustomSelection();

            if (node != null)
            {
                node.IsSelected = true;
                if (!m_CustomSelection.Contains(node))
                    m_CustomSelection.Add(node);
            }
        }

        void service_GetRootMembersCompleted(DataLoaderEventArgs e, MemberTreeNode parentNode)
        {
            membersTree.IsWaiting = false;
            membersTree.IsFullLoaded = true;

            if (e.Result.ContentType == InvokeContentType.Error)
            {
                ClearTree();
                membersTree.IsError = true;

                SetFilterBuilderToError();
                return;
            }

            List<MemberData> members = new List<MemberData>();
            if (!String.IsNullOrEmpty(e.Result.Content))
            {
                CellSetData cellSet = CellSetData.Deserialize(e.Result.Content);
                members = QueryProvider.GetMembers(cellSet);
            }

            CreateChildNodes(null, members);

            //            ClearCustomSelection();

            if (membersTree.Items.Count > 0)
            {
                MemberTreeNode node = membersTree.Items[0] as MemberTreeNode;
                SelectNode(node);
            }

            if (!m_LevelPropertiesIsLoaded)
                LoadLevelProperties();
        }

        #region Создание иерархии по Set и Dictionary
        /// <summary>
        /// Формирует OlapMemberInfoCollection для указанного Set с учетом иерархии
        /// </summary>
        /// <param name="Set">Set</param>
        private RootOlapMemberInfo BuildHierarсhyBySelection()
        {
            //Задача:
            //Получить элементы, входящие в Set и сформировать иерархию с учетом родителей
            //Например: База - FoodMart 2000, Куб - Sales, Измерение - [Customers]
            //	Set: {[Customers].[Mexico].Children,  [Customers].[USA].Children} - все штаты Мексики и США
            //	Сформированная иерархия должна включать в себя: 
            //		All Customers
            //			Mexico
            //				<штаты Мексики>
            //			США
            //				<штаты США>
            //
            //
            //Алгоритм решения:
            //1. Выполняем запрос, который ворзвращает всех предков элементов Set, не попавших в сам Set
            //2. Строим иерархию предков, не попавших в Set (результат из п.1)
            //3. Выполняем запрос, который ворзвращает элементы Set
            //4. Добавляем в иерархию элементы Set и у станавливаем их в нужное состояние
            //	

            RootOlapMemberInfo memberInfoHierarchy = new RootOlapMemberInfo(MemberData.Empty);
            memberInfoHierarchy.HierarchyStateChanged += new OlapMemberInfo.StateChangedEventHandler(OlapMemberInfoHierarchy_HierarchyStateChanged);

            if (SelectedInfo.Count == 0)
            {
                if (!LoadSetWithAscendants(m_SelectedSet))
                {
                    //GetRootMembersCount();
                    Load();
                }
                return memberInfoHierarchy;
            }


            //Формируем Set из элементов
            string Set = String.Empty;

            foreach (MemberChoiceSettings si in SelectedInfo)
            {
                if (!String.IsNullOrEmpty(si.UniqueName))
                {
                    if (!String.IsNullOrEmpty(Set))
                        Set += ",";
                    Set += si.UniqueName;
                }
            }

            if (String.IsNullOrEmpty(Set))
            {
//                GetRootMembersCount();
                Load();
                return memberInfoHierarchy;
            }

            Set = "{" + Set + "}";

            if (!LoadSetWithAscendants(Set))
            {
                Load();
                //GetRootMembersCount();
            }

            return memberInfoHierarchy;
        }

        bool LoadSetWithAscendants(String Set)
        {
            if (!String.IsNullOrEmpty(Set))
            {
                //Читаем данные о предках
                String query = DataManager.LoadSetWithAscendants(Set);
                LogManager.LogInformation(this, this.Name + " - Loading members with ascendants.");
                MdxQueryArgs query_args = CommandHelper.CreateMdxQueryArgs(Connection, query);
                OlapDataLoader.LoadData(query_args, new UserSchemaWrapper<MemberChoiceQueryType, MemberTreeNode>(MemberChoiceQueryType.LoadSetWithAscendants, null));
                return true;
            }
            return false;
        }

        void LoadSetWithAscendants_InvokeCommandCompleted(DataLoaderEventArgs e)
        {
            if (e.Result.ContentType == InvokeContentType.Error)
            {
                ClearTree();
                membersTree.IsError = true;

                SetFilterBuilderToError();
                return;
            }

            List<MemberData> members = new List<MemberData>();
            if (!String.IsNullOrEmpty(e.Result.Content))
            {
                CellSetData cellSet = CellSetData.Deserialize(e.Result.Content);
                members = QueryProvider.GetMembers(cellSet);
            }

            if (members != null)
            {
                #region Строим иерархию c предками
                foreach (MemberData info in members)
                {
                    OlapMemberInfo memberInfo = OlapMemberInfoHierarchy.AddMemberToHierarchy(info);
                }
                #endregion Строим иерархию c предками

                if (SelectedInfo.Count == 0 && !String.IsNullOrEmpty(m_SelectedSet))
                {
                    //Читаем данные о выбранных элементах
                    String query = DataManager.GetMembers(m_SelectedSet);
                    LogManager.LogInformation(this, this.Name + " - Loading selected members.");
                    MdxQueryArgs query_args = CommandHelper.CreateMdxQueryArgs(Connection, query);
                    OlapDataLoader.LoadData(query_args, new UserSchemaWrapper<MemberChoiceQueryType, MemberTreeNode>(MemberChoiceQueryType.GetMembers, null));
                }
                else
                {
                    #region Устанавливаем нужные состояния для элементов
                    SetMembersInfoToNewState(OlapMemberInfoHierarchy, SelectedInfo.ToList<MemberChoiceSettings>(), SelectStates.Selected_With_Children_Has_Excluded, SelectStates.Selected_With_Children);
                    SetMembersInfoToNewState(OlapMemberInfoHierarchy, SelectedInfo.ToList<MemberChoiceSettings>(), SelectStates.Not_Selected, SelectStates.Not_Selected);
                    //SetMembersInfoToNewState(OlapMemberInfoHierarchy, SelectedInfo.ToList<MemberChoiceSettings>(), SelectStates.Selected_Self, SelectStates.Not_Selected);
                    SetMembersInfoToNewState(OlapMemberInfoHierarchy, SelectedInfo.ToList<MemberChoiceSettings>(), SelectStates.Labeled_As_Parent, SelectStates.Not_Selected);
                    SetMembersInfoToNewState(OlapMemberInfoHierarchy, SelectedInfo.ToList<MemberChoiceSettings>(), SelectStates.Selected_Self, SelectStates.Selected_Self);
                    #endregion Устанавливаем нужные состояния для элементов
                }
            }
            //GetRootMembersCount();
            Load();
        }

        void GetMembers_InvokeCommandCompleted(DataLoaderEventArgs e)
        {
            if (e.Result.ContentType == InvokeContentType.Error)
            {
                ClearTree();
                membersTree.IsError = true;
                return;
            }

            List<MemberData> members = new List<MemberData>();
            if (!String.IsNullOrEmpty(e.Result.Content))
            {
                CellSetData cellSet = CellSetData.Deserialize(e.Result.Content);
                members = QueryProvider.GetMembers(cellSet);
            }

            if (members != null)
            {
                foreach (MemberData wrapper in members)
                {
                    OlapMemberInfo memberInfo = OlapMemberInfoHierarchy.FindMember(wrapper.UniqueName);

                    //Если элемент найден, то переводим в новое состояние
                    if (memberInfo != null)
                    {
                        memberInfo.SetNewState(SelectStates.Selected_Self);
                    }
                }
            }
        }

        /// <summary>
        /// Переводит элементы иерархии memberInfoHierarchy, перечисленные в SelectionInfo и находящиеся там в состоянии stateInDictionary, в состояние newState 
        /// </summary>
        /// <param name="memberInfoHierarchy">Иерархия элементов</param>
        /// <param name="SelectionInfo">Коллекция элементов, которые моут быть переведены</param>
        /// <param name="stateInDictionary">Состояние в коллекции, которое подлежит переводу в новое</param>
        /// <param name="newState">Новое состояние элементов</param>
        private void SetMembersInfoToNewState(RootOlapMemberInfo memberInfoHierarchy, List<MemberChoiceSettings> selectionInfo, SelectStates stateInDictionary, SelectStates newState)
        {
            if (selectionInfo != null)
            {
                foreach (MemberChoiceSettings si in selectionInfo)
                {
                    if (!String.IsNullOrEmpty(si.UniqueName) && si.SelectState == stateInDictionary)
                    {
                        OlapMemberInfo memberInfo = memberInfoHierarchy.FindMember(si.UniqueName);

                        //Если элемент найден, то переводим в новое состояние
                        if (memberInfo != null)
                        {
                            memberInfo.SetNewState(newState);
                        }
                    }
                }
            }
        }
        #endregion Создание иерархии по Set и Dictionary


        bool m_LevelPropertiesIsLoaded = false;

        /// <summary>
        /// Иерархия OlapMemberInfo
        /// </summary>
        private RootOlapMemberInfo m_OlapMemberInfoHierarchy = null;
        private RootOlapMemberInfo OlapMemberInfoHierarchy
        {
            get
            {
                if (m_OlapMemberInfoHierarchy == null)
                {
                    m_OlapMemberInfoHierarchy = new RootOlapMemberInfo(MemberData.Empty);
                    m_OlapMemberInfoHierarchy.HierarchyStateChanged += new OlapMemberInfo.StateChangedEventHandler(OlapMemberInfoHierarchy_HierarchyStateChanged);
                }
                return m_OlapMemberInfoHierarchy;
            }
        }

        void OlapMemberInfoHierarchy_HierarchyStateChanged(OlapMemberInfo sender)
        {
            GenerateSetBySelectionState();
        }

        /// <summary>
        /// Режим выбора: true - множественный выбор, false - одиночный выбор
        /// </summary>
        private bool m_MultiSelect = true;
        public bool MultiSelect
        {
            get
            {
                return m_MultiSelect;
            }
            set
            {
                m_MultiSelect = value;
                if (value)
                {
                    Clear_Choice_Button.Visibility = Visibility.Visible;
                }
                else 
                {
                    Clear_Choice_Button.Visibility = Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// Ключ - уникальное имя 
        /// </summary>
        Dictionary<String, MemberTreeNode> m_TreeNodes = new Dictionary<String, MemberTreeNode>();

        void CreateChildNodes(MemberTreeNode parent, List<MemberData> members)
        {
            if (members == null || members.Count == 0)
                return;

            List<OlapMemberInfo> infos = new List<OlapMemberInfo>();
            List<OlapMemberInfo> to_Collapse = new List<OlapMemberInfo>();

            for (int i = 0; i < members.Count; i++)
            {
                MemberData wrapper = members[i];
                // Грузим Step+1 элемент, а отображать будем только Step штук. Если в ответе придет Step+1 элемент, то значит нужно отображать узел LoadNext
                if (UseStepLoading && members.Count > Step && i >= (members.Count - 1))
                    break;

                // Каждый из элементов помещаем в иерархию
                OlapMemberInfo info = OlapMemberInfoHierarchy.AddMemberToHierarchy(wrapper);
                if (info != null)
                {
                    long realCount = QueryProvider.GetRealChildrenCount(wrapper);
                    if (info.CubeChildrenCount != realCount)
                    {
                        to_Collapse.Add(info);
                        info.CubeChildrenCount = realCount;
                    }
                    infos.Add(info);
                }
            }

            //Если известен родительский узел дерева. То получаем его OlapMemberInfo
            if (parent != null)
            {
                parent.IsWaiting = false;
            }

            // Вычисляем индекс служебного узла "Загрузить далее..."
            int loadNextIndex = -1;
            if (parent == null)
            {
                for (int i = membersTree.Items.Count - 1; i >= 0; i--)
                {
                    LoadNextTreeNode loadNext = membersTree.Items[i] as LoadNextTreeNode;
                    if (loadNext != null)
                    {
                        loadNextIndex = i;
                        break;
                    }
                }
            }
            else
            {
                for (int i = parent.Items.Count - 1; i >= 0; i--)
                {
                    LoadNextTreeNode loadNext = parent.Items[i] as LoadNextTreeNode;
                    if (loadNext != null)
                    {
                        loadNextIndex = i;
                        break;
                    }
                }
            }

            int indx = 0;
            foreach (OlapMemberInfo info in infos)
            {
                MemberTreeNode node = null;

                //Если родительский узел не задан, то узел добавится в корень
                if (m_TreeNodes.ContainsKey(info.UniqueName))
                {
                    node = m_TreeNodes[info.UniqueName];
                    if (to_Collapse.Contains(info))
                    {
                        //Сворачиваем узел, чтобы при разворачивании зачиталось новое количество дочерних
                        if (node != null)
                        {
                            node.IsExpanded = false;
                            //Удаляем дочерние узлы дерева
                            node.Items.Clear();
                            // Обновляем информацию о количестве дочерних
                            node.MemberInfo.CubeChildrenCount = info.CubeChildrenCount;
                        }
                    }

                }

                if (node == null)
                {
                    // Создание нового узла для добавления в дерево
                    node = new MemberTreeNode(info, MultiSelect);
                    node.MemberVisualizationType = MemberVisualizationType;
                    node.Special_MouseDoubleClick += new EventHandler<CustomEventArgs<CustomTreeNode>>(node_SpecialNodeExpanded);
                    node.Expanded += new RoutedEventHandler(MemberNode_Expanded);
                    m_TreeNodes[info.UniqueName] = node;

                    // Если узла "Загрузить далее..." не найдено то узел добавляем в конец списка
                    // В противном случае вставляем узел перед узлом "Загрузить далее..."
                    if (loadNextIndex == -1)
                    {
                        if (parent == null)
                            membersTree.Items.Add(node);
                        else
                            parent.Items.Add(node);
                    }
                    else
                    {
                        if (parent == null)
                            membersTree.Items.Insert(loadNextIndex + indx, node);
                        else
                            parent.Items.Insert(loadNextIndex + indx, node);
                    }
                }
                else
                {
                    // т.к. узел уже был создан ранее, то это означает что элемент был загружен в дерево во внеочередном порядке (т.к. находится после узла "Загрузить далее...")
                    // такое возможно при попытке загрузить родителей для элемента при переключении по двойному клику с закладки с результатами поиска
                    // В этом случе узел нужно удалить из списка и вставить перед узлом "Загрузить далее..."
                    if (loadNextIndex != -1)
                    {
                        if (parent == null)
                        {
                            membersTree.Items.Remove(node);
                            membersTree.Items.Insert(loadNextIndex + indx, node);
                            node.IsPreloaded = false;
                        }
                        else
                        {
                            parent.Items.Remove(node);
                            parent.Items.Insert(loadNextIndex + indx, node);
                            node.IsPreloaded = false;
                        }
                    }
                }

                //Если нужно выводить элемнты ТОЛЬКО верхнего уровня
                if (ShowOnlyFirstLevelMembers == false)
                {
                    //Если есть дочерние, то добавляем фиктивный узел для отображения [+] напротив данного узла
                    if (!node.IsExpanded && info.HasChildren)
                    {
                        if (node.Items.Count == 0)
                            node.IsWaiting = true;
                    }
                }
                indx++;
            }

            //Если режим Частичной загрузки, то сверяем количество загруженных элементов и количество элементов в кубе
            if (useStepLoading)
            {
                // Грузим Step+1 элемент, а отображать будем только Step штук. Если в ответе придет Step+1 элемент, то значит нужно отображать узел LoadNext
                if (members.Count > Step)
                {
                    if (loadNextIndex == -1)
                    {
                        if (parent != null)
                            parent.IsFullLoaded = false;
                        else
                        {
                            membersTree.IsFullLoaded = false;
                        }
                    }
                }
                else
                {
                    if (parent != null)
                        parent.IsFullLoaded = true;
                    else
                    {
                        membersTree.IsFullLoaded = true;
                    }
                }
            }
            else 
            {
                if (parent != null)
                    parent.IsFullLoaded = true;
                else
                {
                    membersTree.IsFullLoaded = true;
                }
            }

            membersTree.UpdateLayout();
        }

        void node_SpecialNodeExpanded(object sender, CustomEventArgs<CustomTreeNode> e)
        {
            LoadNextTreeNode loadNext = null;
            TreeViewItem node = e.Args as TreeViewItem;
            if (node == null)
                return;
            //Количество загруженных узлов - это количество узлов - 2 (узел "Загрузить далее", узел "Загрузить все")
            long loadedChildrenCount = 0;
            long step = Step;

            TreeViewItem parent = node.Parent as TreeViewItem;
            if (useStepLoading)
            {
                if (parent == null)
                {
                    // Догружаем в корень дерева
                    loadedChildrenCount = membersTree.Items.Count;

                    int offset = 0;
                    for (int i = membersTree.Items.Count - 1, x = 1; i >= 0; i--, x++)
                    {
                        loadNext = membersTree.Items[i] as LoadNextTreeNode;
                        if (loadNext != null)
                        {
                            offset = x;
                            break;
                        }
                    }
                    loadedChildrenCount -= offset;
                }
                else
                {
                    //Догружаем в элемент
                    loadedChildrenCount = parent.Items.Count;

                    int offset = 0;
                    for (int i = parent.Items.Count - 1, x = 1; i >= 0; i--, x++)
                    {
                        loadNext = parent.Items[i] as LoadNextTreeNode;
                        if (loadNext != null)
                        {
                            offset = x;
                            break;
                        }
                    }
                    loadedChildrenCount -= offset;
                }
            }
            else
            {
                loadedChildrenCount = -1;
                step = 1;
            }

            loadNext = e.Args as LoadNextTreeNode;
            if (loadNext != null)
            {
                if (parent != null)
                {
                    MemberTreeNode parentNode = parent as MemberTreeNode;
                    if (parentNode != null)
                    {
                        //Зачитываем дочерних 
                        LoadChildren(parentNode, loadedChildrenCount, Step);
                    }
                }
                else
                {
                    //Зачитываем дочерних - как для корневого узла
                    LoadRootMembers(loadedChildrenCount, Step);
                }
                return;
            }

            //LoadAllTreeNode loadAll = sender as LoadAllTreeNode;
            //if (loadAll != null)
            //{
            //    long count = 0;
            //    if (parent != null)
            //    {
            //        MemberTreeNode parentNode = parent as MemberTreeNode;
            //        if (parentNode != null)
            //        {
            //            count = parentNode.MemberInfo.CubeChildrenCount - loadedChildrenCount;
            //            //Зачитываем дочерних 
            //            LoadChildren(parentNode, loadedChildrenCount, count);
            //        }
            //    }
            //    else
            //    {
            //        count = m_RootMembersCubeCount - loadedChildrenCount;
            //        //Зачитываем дочерних - как для корневого узла
            //        LoadRootMembers(loadedChildrenCount, count);
            //    }
            //    return;
            //}

            //ReloadAllTreeNode reloadAll = sender as ReloadAllTreeNode;
            //if (reloadAll != null)
            //{
            //    if (parent != null)
            //    {
            //        MemberTreeNode parentNode = parent as MemberTreeNode;
            //        if (parentNode != null)
            //        {
            //            parentNode.IsReloadAll = false;
            //            //Зачитываем дочерних 
            //            LoadChildren(parentNode);
            //        }
            //    }
            //    else
            //    {
            //        membersTree.IsReloadAll = false;
            //        //Зачитываем дочерних - как для корневого узла
            //        LoadRootMembers(-1, -1);
            //    }
            //    return;
            //}
        }

        ///// <summary>
        ///// Проверяет все ли дочерние мемберы для указанного узла добавлены в дерево
        ///// </summary>
        ///// <param name="parent"></param>
        ///// <returns></returns>
        //private bool AllChildrenMembersAddedToTreeNode(MemberTreeNode node)
        //{
        //    //long loadedChildrenCount = 0;
        //    long cubeChildrenCount = 0;
        //    /*if (node == null)	// - корневой узел
        //    {
        //        loadedChildrenCount = membersTree.Items.Count;
        //        cubeChildrenCount = m_RootMembersCubeCount;

        //        if (loadedChildrenCount < cubeChildrenCount)
        //        {
        //            return false;
        //        }
        //        else
        //            return true;
        //    }
        //    else
        //    {
        //        loadedChildrenCount = node.Items.Count;

        //        OlapMemberInfo info = node.MemberInfo;
        //        cubeChildrenCount = info.CubeChildrenCount;

        //        if (loadedChildrenCount < cubeChildrenCount)
        //        {
        //            return false;
        //        }
        //        else
        //            return true;
        //    }*/
        //    //Количество загруженных узлов - это количество узлов - 2 (узел "Загрузить далее", узел "Загрузить все")
        //    long loadedChildrenCount = 0;

        //    if (node == null)
        //    {
        //        cubeChildrenCount = m_RootMembersCubeCount;
        //        // Догружаем в корень дерева
        //        loadedChildrenCount = membersTree.Items.Count;

        //        int offset = 0;
        //        for (int i = membersTree.Items.Count - 1, x = 1; i >= 0; i--, x++)
        //        {
        //            LoadNextTreeNode loadNext = membersTree.Items[i] as LoadNextTreeNode;
        //            if (loadNext != null)
        //            {
        //                offset = x;
        //                break;
        //            }
        //        }
        //        loadedChildrenCount -= offset;
        //    }
        //    else
        //    {
        //        cubeChildrenCount = node.MemberInfo.CubeChildrenCount;
        //        //Догружаем в элемент
        //        loadedChildrenCount = node.Items.Count;

        //        int offset = 0;
        //        for (int i = node.Items.Count - 1, x = 1; i >= 0; i--, x++)
        //        {
        //            LoadNextTreeNode loadNext = node.Items[i] as LoadNextTreeNode;
        //            if (loadNext != null)
        //            {
        //                offset = x;
        //                break;
        //            }
        //        }
        //        loadedChildrenCount -= offset;
        //    }
        //    if (loadedChildrenCount < cubeChildrenCount)
        //    {
        //        return false;
        //    }
        //    else
        //        return true;
        //}

        void MemberNode_Expanded(object sender, RoutedEventArgs e)
        {
            MemberTreeNode node = sender as MemberTreeNode;
            if (node == null)
                return;

            if (node.IsInitialized)
            {
                // В случае если узел уже инициализировался и дочерних в кубе у него нет, то при отключенном множетвенном выборе двойной клик считаем выбором элемента
                if (MultiSelect == false && node.MemberInfo != null && node.MemberInfo.CubeChildrenCount == 0)
                {
                    ApplyMembersSelection();
                }
            }
            else
            {
                BeforeExpandNode(node);
            }

            //Tree.SetFocusedNode(e.Node);
        }

        /// <summary>
        /// Раскрытие узла дерева
        /// </summary>
        /// <param name="node"></param>
        private void BeforeExpandNode(MemberTreeNode node)
        {
            node.Items.Clear();
            node.IsWaiting = true;

            //Загружаем дочерние элементы для данного узла
            if (useStepLoading)
            {
                LoadChildren(node, 0, Step);
            }
            else
            {
                LoadChildren(node);
            }
        }

        /// <summary>
        /// Загружает сам элемент и предков вплоть до верхнего уровня ОТОБРАЖАЕМОГО В ДЕРЕВЕ
        /// </summary>
        /// <param name="uniqueName"></param>
        private void LoadAscendants(String uniqueName)
        {
            String query = DataManager.GetAscendants(CurrentLevelUniqueName, uniqueName);
            LogManager.LogInformation(this, this.Name + " - Loading ascendants for '" + uniqueName + "'");
            MdxQueryArgs query_args = CommandHelper.CreateMdxQueryArgs(Connection, query);
            OlapDataLoader.LoadData(query_args, new UserSchemaWrapper<MemberChoiceQueryType, MemberTreeNode>(MemberChoiceQueryType.GetAscendants, null));

        }

        private void LoadLevelProperties()
        {
            // Чистим фильтр для поиска
            filterBuilder.Tree.Items.Clear();
            filterBuilder.Tree.IsWaiting = true;
            // Чистим результат поиска
            findResultTree.Items.Clear();

            LogManager.LogInformation(this, this.Name + " - Loading levels attributes.");
            MetadataQuery args = CommandHelper.CreateLoadLevelPropertiesArgs(Connection, CubeName, String.Empty, HierarchyUniqueName, String.Empty);
            OlapDataLoader.LoadData(args, args);
        }

        /// <summary>
        /// Загружает все дочерние элементы для указанного
        /// </summary>
        private void LoadChildren(MemberTreeNode item)
        {
            LoadChildren(item, -1, -1);
        }

        /// <summary>
        /// Загружает <paramref name="count"/> дочерних элементов для элемента с указанным <paramref name="uniqueName"/>, начиная c <paramref name="begin"/>.
        /// </summary>
        /// <param name="begin">индекс начала</param>
        /// <param name="count">количество</param>
        private void LoadChildren(MemberTreeNode item, long begin, long count)
        {
            if (item == null)
                return;

            OlapMemberInfo info = item.MemberInfo;

            //MemberChoiceQuery args = CommandHelper.CreateGetChildrenMembersQueryArgs(Connection, CubeName, SubCube, HierarchyUniqueName, info.UniqueName, CurrentLevelUniqueName, begin, count);
            //OlapDataLoader.LoadData(args, new UserSchemaWrapper<MemberChoiceQuery, MemberTreeNode>(args, item));

            // Грузим Step+1 элемент, а отображать будем только Step штук. Если в ответе придет Step+1 элемент, то значит нужно отображать узел LoadNext
            if (count != -1)
                count += 1;
            String query = DataManager.GetChildrenMembers(info.UniqueName, begin, count);
            LogManager.LogInformation(this, this.Name + " - Loading children for '" + info.UniqueName + "'");
            MdxQueryArgs query_args = CommandHelper.CreateMdxQueryArgs(Connection, query);
            OlapDataLoader.LoadData(query_args, new UserSchemaWrapper<MemberChoiceQueryType, MemberTreeNode>(MemberChoiceQueryType.GetChildrenMembers, item));
        }

        void Service_GetChildrenMembersCompleted(DataLoaderEventArgs e, MemberTreeNode parentNode)
        {
            if (e.Result.ContentType == InvokeContentType.Error)
            {
                ClearTree();
                membersTree.IsError = true;
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

        public static BitmapImage GetIconImage(OlapMemberInfo memberInfo)
        {
            if (memberInfo == null)
                return null;

            BitmapImage res = null;

            switch (memberInfo.SelectState)
            {
                case SelectStates.Not_Initialized:
                case SelectStates.Not_Selected:
                    res = UriResources.Images.NotSelected16;
                    break;
                case SelectStates.Selected_Self:
                case SelectStates.Selected_By_Parent:
                    res = UriResources.Images.Selected16;
                    break;
                case SelectStates.Selected_With_Children:
                case SelectStates.Selected_By_Parent_With_Children:
                    res = UriResources.Images.SelectedChildren16;
                    break;
                case SelectStates.Labeled_As_Parent:
                    res = UriResources.Images.HasSelectedChildren16;
                    break;
                case SelectStates.Selected_With_Children_Has_Excluded:
                case SelectStates.Selected_By_Parent_With_Children_Has_Excluded:
                    res = UriResources.Images.HasExcludedChildren16;
                    break;
                default:
                    break;
            }

            return res;
        }

        /// <summary>
        /// Событие генерируется после окончания выбора элементов иземерения
        /// </summary>
        public event EventHandler ApplySelection;

        ///// <summary>
        ///// Событие генерируется при нажатии на кнопку Отмена
        ///// </summary>
        //public event EventHandler CancelSelection;

        /// <summary>
        /// Генерирует событие "Выбор окончен"
        /// </summary>
        private void Raise_ApplySelection()
        {
            EventHandler handler = ApplySelection;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        ///// <summary>
        ///// Генерирует событие "Отмена"
        ///// </summary>
        //private void Raise_CancelSelection()
        //{
        //    EventHandler handler = CancelSelection;
        //    if (handler != null)
        //        handler(this, EventArgs.Empty);
        //}

        //OK private void OkButton_Click(object sender, RoutedEventArgs e)
        //{
        //    ApplyMembersSelection();
        //}

        private void ApplyMembersSelection()
        {
            GenerateSetBySelectionState();
            //Генерируем событие - выбор окончен
            Raise_ApplySelection();
        }

        //OK private void CancelButton_Click(object sender, RoutedEventArgs e)
        //{
        //    // Генерируем событие - Отмена
        //    Raise_CancelSelection();
        //}

        /// <summary>
        /// Set, сгенерированный по выбранным элементам
        /// </summary>
        private string m_SelectedSet = String.Empty;

        /// <summary>
        /// Set, сгенерированный по выбранным элементам
        /// </summary>
        public String SelectedSet
        {
            get
            {
                return m_SelectedSet;
            }
            set
            {
                m_SelectedSet = value;
            }
        }

        /// <summary>
        /// Коллекция для хранения информации о выбранных элементах
        /// </summary>
        private List<MemberChoiceSettings> m_SelectedInfo = null;
        /// <summary>
        /// Информация о выбранных элементах
        /// </summary>
        public List<MemberChoiceSettings> SelectedInfo
        {
            get
            {
                if (m_SelectedInfo == null)
                {
                    m_SelectedInfo = new List<MemberChoiceSettings>();
                }
                return m_SelectedInfo;
            }
            set
            {
                m_SelectedInfo = value;
            }
        }

        Dictionary<String, OlapMemberInfo> m_MembersInSet = null;
        public Dictionary<String, OlapMemberInfo> MembersInSet
        { 
            get{
                if(m_MembersInSet == null)
                {
                    m_MembersInSet = new Dictionary<String, OlapMemberInfo>();
                }
                return m_MembersInSet;
                }
        }

        /// <summary>
        /// Генерирует Set по иерархии элементов
        /// </summary>
        /// <returns></returns>
        private void GenerateSetBySelectionState()
        {
            MembersInSet.Clear();
            if (MultiSelect)
            {
                String Set = this.OlapMemberInfoHierarchy.GenerateSet(MembersInSet);

                if (Set != null && Set.Length > 0)
                {
                    //Убрал HIERARCHIZE Set = "HIERARCHIZE(" + Set + ")";

                    //if (useFilter == true)
                    //{
                    //    if (filterValue != null && filterValue.Length > 0)
                    //    {
                    //        if (filterBeginWith)
                    //            Set = CreateFilterSet(Set, filterValue, 1);
                    //        else
                    //            Set = CreateFilterSet(Set, filterValue, 0);
                    //    }
                    //}
                }

                m_SelectedInfo = new List<MemberChoiceSettings>();
                foreach (OlapMemberInfo memberInfo in MembersInSet.Values)
                {
                    MemberChoiceSettings cs = new MemberChoiceSettings(memberInfo.Info, memberInfo.SelectState);
                    m_SelectedInfo.Add(cs);
                }

                //Запоминаем выбранный Set
                if (String.IsNullOrEmpty(Set))
                    Set = "{}";

                m_SelectedSet = Set;
            }
            else
            {
                m_SelectedSet = String.Empty;
                SelectedInfo.Clear();

                MemberTreeNode node = null;
                //if (tabControl != null && findTab != null && tabControl.SelectedItem == findTab &&
                //    findResultTree.SelectedItem != null)
                //{
                //    node = findResultTree.SelectedItem as MemberTreeNode;
                //}
                //else
                {
                    node = membersTree.SelectedItem as MemberTreeNode;
                }

                if (node != null && node.MemberInfo != null)
                {
                    MembersInSet.Add(node.MemberInfo.UniqueName, node.MemberInfo);

                    MemberChoiceSettings cs = new MemberChoiceSettings(node.MemberInfo.Info, SelectStates.Selected_Self);
                    SelectedInfo.Add(cs);

                    //Запоминаем выбранный элемент
                    m_SelectedSet = node.MemberInfo.UniqueName;
                }
            }
            Raise_SelectedInfoChanged();
        }

        private void FindButton_Click(object sender, RoutedEventArgs e)
        {
            RunSearch();
        }

        /// <summary>
        /// Коллекция элементов, отображаемых в результате поиска
        /// </summary>
        //HybridDictionary foundMembers = new HybridDictionary();

        /// <summary>
        /// Выполняет поиск в кубе
        /// </summary>
        private void RunSearch()
        {
            FilterOperationBase filter = filterBuilder.GetFilter();
            findResultTree.Items.Clear();
            m_Find_Count.Text = "0";
            findResultTree.IsWaiting = true;

            //findResultTree.Items.Clear();
            //findResultTree.IsWaiting = true;
            //membersTree.Items.Clear();
            //membersTree.IsWaiting = true;

            //String strToSearch = findTextBox.Text;
            //if (!String.IsNullOrEmpty(strToSearch))
            //{

            String query = DataManager.SearchMembers(CurrentLevelUniqueName, filter);
            LogManager.LogInformation(this, this.Name + " - Searching for members.");
            MdxQueryArgs query_args = CommandHelper.CreateMdxQueryArgs(Connection, query);
            OlapDataLoader.LoadData(query_args, new UserSchemaWrapper<MemberChoiceQueryType, MemberTreeNode>(MemberChoiceQueryType.FindMembers, null));

            //}
        }

        void FindMembers_InvokeCommandCompleted(DataLoaderEventArgs e, MemberTreeNode node_)
        {
            findResultTree.Items.Clear();
            m_Find_Count.Text = "0";

            if (e.Result.ContentType == InvokeContentType.Error)
            {
                findResultTree.IsError = true;
                return;
            }

            List<MemberData> members = new List<MemberData>();
            if (!String.IsNullOrEmpty(e.Result.Content))
            {
                CellSetData cellSet = CellSetData.Deserialize(e.Result.Content);
                members = QueryProvider.GetMembers(cellSet);
            }

            //ClearTree();
            // CreateChildNodes(null, members);

            List<OlapMemberInfo> infos = new List<OlapMemberInfo>();

            foreach (MemberData wrapper in members)
            {
                // Каждый из элементов помещаем в иерархию
                OlapMemberInfo info = OlapMemberInfoHierarchy.AddMemberToHierarchy(wrapper);
                if (info != null)
                {
                    infos.Add(info);
                }
            }

            foreach (OlapMemberInfo info in infos)
            {
                MemberTreeNode node = null;

                node = new MemberTreeNode(info, MultiSelect);
                node.MemberVisualizationType = MemberVisualizationType;
                node.Expanded += new RoutedEventHandler(findedNode_DblClick);
                node.Collapsed += new RoutedEventHandler(findedNode_DblClick);

                findResultTree.Items.Add(node);
            }

            m_Find_Count.Text = infos.Count.ToString();
        }

        void findedNode_DblClick(object sender, RoutedEventArgs e)
        {
            MemberTreeNode node = sender as MemberTreeNode;
            if (node != null)
            {
                SelectMember(node.MemberInfo.UniqueName);
            }
        }

        /// <summary>
        /// Коллекция элементов, которые мы руками устанавливаем как выбранные. Используем чтобы при изменении выбранного элемнта дерева очистить выбранные ранее
        /// </summary>
        List<TreeViewItem> m_CustomSelection = new List<TreeViewItem>();

        void GetAscendants_InvokeCommandCompleted(DataLoaderEventArgs e)
        {
            if (e.Result.ContentType == InvokeContentType.Error)
            {
                ClearTree();
                membersTree.IsError = true;
                return;
            }

            List<MemberData> members = new List<MemberData>();
            if (!String.IsNullOrEmpty(e.Result.Content))
            {
                CellSetData cellSet = CellSetData.Deserialize(e.Result.Content);
                members = QueryProvider.GetMembers(cellSet);
            }

            List<OlapMemberInfo> infos = new List<OlapMemberInfo>();

            foreach (MemberData member in members)
            {
                // Каждый из элементов помещаем в иерархию
                OlapMemberInfo info = OlapMemberInfoHierarchy.AddMemberToHierarchy(member);
                if (info != null)
                {
                    infos.Add(info);
                }
            }

            foreach (OlapMemberInfo info in infos)
            {
                MemberTreeNode node = null;

                //Если родительский узел не задан, то узел добавится в корень
                if (m_TreeNodes.ContainsKey(info.UniqueName))
                {
                    node = m_TreeNodes[info.UniqueName];
                }

                if (node == null)
                {
                    node = new MemberTreeNode(info, MultiSelect);
                    node.MemberVisualizationType = MemberVisualizationType;
                    node.IsPreloaded = true;
                    node.Special_MouseDoubleClick += new EventHandler<CustomEventArgs<CustomTreeNode>>(node_SpecialNodeExpanded);
                    node.Expanded += new RoutedEventHandler(MemberNode_Expanded);

                    //Если есть дочерние, то добавляем фиктивный узел для отображения [+] напротив данного узла
                    if (!node.IsExpanded /*&& !node.IsReloadAll*/ && info.HasChildren)
                    {
                        if (node.Items.Count == 0)
                            node.IsWaiting = true;
                    }

                    //node.Expanded += new RoutedEventHandler(FindMemberNode_Expanded);
                    m_TreeNodes[info.UniqueName] = node;

                    MemberTreeNode parent = null;
                    String parentUniqueName = String.Empty;
                    if (info.Info != null)
                    {
                        PropertyData prop = info.Info.GetMemberProperty(MemberData.PARENT_UNIQUE_NAME_PROPERTY);
                        if (prop != null && prop.Value != null)
                        {
                            parentUniqueName = prop.Value.ToString();
                        }
                    }

                    if (!String.IsNullOrEmpty(parentUniqueName))
                    {
                        //Если родительский узел не задан, то узел добавится в корень
                        if (m_TreeNodes.ContainsKey(parentUniqueName))
                        {
                            parent = m_TreeNodes[parentUniqueName];
                        }
                    }

                    if (parent == null)
                    {
                        membersTree.IsFullLoaded = false;
                        membersTree.Items.Add(node);
                        membersTree.IsWaiting = false;
                        //membersTree.IsReloadAll = true;
                    }
                    else
                    {
                        if (parent.Items.Count < parent.MemberInfo.CubeChildrenCount)
                            parent.IsFullLoaded = false;

                        parent.Items.Add(node);
                        parent.IsWaiting = false;
                        parent.IsExpanded = true;
                        //parent.IsReloadAll = true;
                    }
                }
            }

            MemberTreeNode memberNode = null;
            if (infos.Count > 0 && m_TreeNodes.ContainsKey(infos[infos.Count -1].UniqueName))
            {
                memberNode = m_TreeNodes[infos[infos.Count - 1].UniqueName];
            }

            if (memberNode != null)
            {
                // Если узел в дереве найден, то нужно просто переключиться на него
                SelectNode(memberNode);

                tabControl.TabCtrl.SelectedItem = membersTab;
            }

            //membersList.Initialize(infos);
            ////Получаем все родительские элементы для указанного
            //List<MemberDataWrapper> dict = GetAscendants(uniqueName);
            //if (dict != null && dict.Count > 0)
            //{
            //    bool allParentIsLoaded = true;
            //    TreeListNode parentNode = null;
            //    //Каждый из предков и сам узел в дерево
            //    foreach (MemberDataWrapper de in dict)
            //    {
            //        TreeListNode node = PreloadChildrenNodes(parentNode, de);
            //        if (node == null)
            //        {
            //            allParentIsLoaded = false;
            //            break;
            //        }

            //        parentNode = node;
            //    }

            //    //Если процесс дозагрузки дочерних для всех родителей прошел успешно
            //    if (allParentIsLoaded && parentNode != null)
            //    {
            //        Tree.SetFocusedNode(parentNode);
            //        TabControl.SelectedTabPageIndex = 0;
            //        //Устанавливаем фокус на дерево
            //        Tree.Focus();
            //    }
            //}
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (m_Initializing)
                return;

            if (tabControl != null && mdxSetTab != null && tabControl.TabCtrl.SelectedItem == mdxSetTab)
            {
                InitSelectionStates();
            }

            if (tabControl.TabCtrl.SelectedItem == membersTab)
                ChangeCurrentMember(membersTree.SelectedItem as MemberTreeNode);
            if (tabControl.TabCtrl.SelectedItem == findTab)
                ChangeCurrentMember(findResultTree.SelectedItem as MemberTreeNode);
            if (tabControl.TabCtrl.SelectedItem == mdxSetTab)
                ChangeCurrentMember(selectedList.SelectedItem as MemberTreeNode);
            ShowMemberProperties();

        }

        void InitSelectionStates()
        {
            if (tabControl != null && mdxSetTab != null && tabControl.TabCtrl.SelectedItem == mdxSetTab)
            {
                selectedList.Items.Clear();
                m_Selected_Count.Text = "0";

                GenerateSetBySelectionState();

                mdxSetTextBox.Text = SelectedSet;

                foreach (OlapMemberInfo info in MembersInSet.Values)
                {
                    MemberTreeNode node = null;

                    node = new MemberTreeNode(info, MultiSelect);
                    node.MemberVisualizationType = MemberVisualizationType;
                    info.StateChanged += new OlapMemberInfo.StateChangedEventHandler(selectedNode_StateChanged);
                    node.Expanded += new RoutedEventHandler(selectedNode_DblClick);
                    node.Collapsed += new RoutedEventHandler(selectedNode_DblClick);

                    selectedList.Items.Add(node);
                }
                m_Selected_Count.Text = MembersInSet.Values.Count.ToString();
            }
        }

        void selectedNode_StateChanged(OlapMemberInfo sender)
        {
            InitSelectionStates();
        }

        void selectedNode_DblClick(object sender, RoutedEventArgs e)
        {
            MemberTreeNode node = sender as MemberTreeNode;
            if (node != null)
            {
                SelectMember(node.MemberInfo.UniqueName);
            }
        }

        MemberVisualizationTypes m_MemberVisualizationType = MemberVisualizationTypes.Caption;
        public MemberVisualizationTypes MemberVisualizationType
        {
            get { return m_MemberVisualizationType; }
            set
            {
                m_MemberVisualizationType = value;
            }
        }

    }
}
