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
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Ranet.AgOlap.Controls.Buttons;
using Ranet.AgOlap.Controls.General.Tree;
using Ranet.AgOlap.Controls.ToolBar;
using Ranet.AgOlap.Providers;
using Ranet.Olap.Core;
using Ranet.Olap.Core.Data;
using Ranet.Olap.Core.Metadata;
using System.Collections.Generic;
using Ranet.AgOlap.Controls.General;
using System.Windows.Data;
using Ranet.AgOlap.Controls.General.DataGrid;
using Ranet.Olap.Core.Providers;
using Ranet.Olap.Core.Providers.ClientServer;
using Ranet.AgOlap.Commands;
using Ranet.AgOlap.Controls.List;

namespace Ranet.AgOlap.Controls
{
    public class KpiViewer : AgControlBase
    {
        private RanetToggleButton showColumnChoice;
        private Grid LayoutRoot;
        private RanetToolBar m_ToolBar;
        private RanetToggleButton m_ShowMetadataArea;
        private GridSplitter LayoutRoot_VertSplitter;
        private Border Input_Border;
        private Border Pivot_Border;
        private Grid Table_LayoutRoot;
        private ColumnDefinition m_InputColumn;
        private BusyControl m_Waiting;

        private ServerExplorerCtrl m_ServerExplorer;
        private DragDropDataGrid m_DataGrid;
        private PagedCollectionView Tree;
        private Dictionary<string, KpiView> m_KpiStorage;
        private List<KpiInfo> m_SourceKpis;
        private List<KpiView> m_sourceCollection;
        private RanetCheckedListBox m_columnsList;
        private Dictionary<string,bool> m_ColumnNames;
        
        double m_InputColumnWidth = 350;
        double m_MDXRowHeight = 200;

        private bool m_Initialized = false;

        public KpiViewer()
        {            
            ScrollViewer Scroll = new ScrollViewer();
            Scroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            Scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            m_DataGrid = new DragDropDataGrid();
            m_DataGrid.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            m_DataGrid.VerticalAlignment = VerticalAlignment.Top;
            LayoutRoot = new Grid();
            LayoutRoot.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            LayoutRoot.RowDefinitions.Add(new RowDefinition());
            m_InputColumn = new ColumnDefinition() { Width = new GridLength(m_InputColumnWidth) };
            LayoutRoot.ColumnDefinitions.Add(m_InputColumn);
            LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition() { /*Width = new GridLength(2, GridUnitType.Star)*/ });

            // ТУЛБАР 
            m_ToolBar = new RanetToolBar();
            m_ToolBar.Margin = new Thickness(0, 0, 0, 4);
            LayoutRoot.Children.Add(m_ToolBar);
            Grid.SetColumnSpan(m_ToolBar, 2);

            m_ShowMetadataArea = new RanetToggleButton();
            m_ShowMetadataArea.ClickMode = ClickMode.Press;            
            m_ShowMetadataArea.IsChecked = true;
            m_ShowMetadataArea.Checked += new RoutedEventHandler(m_ShowMetadataArea_Checked);
            m_ShowMetadataArea.Unchecked += new RoutedEventHandler(m_ShowMetadataArea_Unchecked);
            m_ShowMetadataArea.Content = UiHelper.CreateIcon(UriResources.Images.Menu16);
            ToolTipService.SetToolTip(m_ShowMetadataArea, Localization.MdxDesigner_ShowQueryDesigner_ToolTip);
            m_ToolBar.AddItem(m_ShowMetadataArea);

            RanetToggleButton showAllButton = new RanetToggleButton();
            showAllButton.ClickMode = ClickMode.Press;
            showAllButton.Content = UiHelper.CreateIcon(UriResources.GetImage("/Ranet.AgOlap;component/Controls/Images/OLAP/KPI/ShowAll.png"));
            showAllButton.Checked += new RoutedEventHandler(showAllButton_Checked);
            showAllButton.Unchecked += new RoutedEventHandler(showAllButton_Unchecked);
            ToolTipService.SetToolTip(showAllButton,Localization.ShowAll_Check);
            showAllButton.Visibility = System.Windows.Visibility.Collapsed;
            m_ToolBar.AddItem(showAllButton);

            RanetToolBarButton m_ApplyChanges = new RanetToolBarButton();
            m_ApplyChanges.ClickMode = ClickMode.Press;
            m_ApplyChanges.Click += new RoutedEventHandler(m_ApplyChanges_Click);
            m_ApplyChanges.Content = UiHelper.CreateIcon(UriResources.Images.Run16);
            ToolTipService.SetToolTip(m_ApplyChanges, Localization.Apply);
            m_ToolBar.AddItem(m_ApplyChanges);

            //showColumnChoice = new RanetToggleButton();
            //showColumnChoice.ClickMode = ClickMode.Press;
            //showColumnChoice.IsChecked = true;
            //showColumnChoice.Checked += new RoutedEventHandler(showColumnChoice_Checked);
            //showColumnChoice.Unchecked += new RoutedEventHandler(showColumnChoice_Unchecked);
            //showColumnChoice.Content = UiHelper.CreateIcon(UriResources.Images.ColumnsArea16);
            //ToolTipService.SetToolTip(showColumnChoice, Localization.MdxDesigner_RunQueryAutomatic);
            //this.m_ToolBar.AddItem(showColumnChoice);  
                     
            Grid Input_LayoutRoot = new Grid();
            Input_LayoutRoot.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(2, GridUnitType.Star) });
            Input_LayoutRoot.Margin = new Thickness(0, 0, 0, 0);
            Input_LayoutRoot.RowDefinitions.Add(new RowDefinition());

            
            Input_Border = new Border() { Padding = new Thickness(3), BorderBrush = new SolidColorBrush(Colors.DarkGray), BorderThickness = new Thickness(1) };
            Input_Border.Margin = new Thickness(0, 0, 5, 0);
            Input_Border.Child = Input_LayoutRoot;

            Grid Ouput_LayoutRoot = new Grid();
            Ouput_LayoutRoot.Margin = new Thickness(0, 0, 0, 0);
            // По умолчанию высота 0
            var m_MDX_Row = new RowDefinition() { Height = new GridLength(0.0) };
            Ouput_LayoutRoot.RowDefinitions.Add(m_MDX_Row);
            Ouput_LayoutRoot.RowDefinitions.Add(new RowDefinition() /*{ Height = new GridLength(2, GridUnitType.Star) }*/);

            LayoutRoot.Children.Add(Input_Border);
            Grid.SetRow(Input_Border, 1);
            LayoutRoot.Children.Add(Ouput_LayoutRoot);
            Grid.SetRow(Ouput_LayoutRoot, 1);
            Grid.SetColumn(Ouput_LayoutRoot, 1);

            LayoutRoot_VertSplitter = new RanetGridSplitter();
            LayoutRoot_VertSplitter.VerticalAlignment = VerticalAlignment.Stretch;
            LayoutRoot_VertSplitter.HorizontalAlignment = HorizontalAlignment.Right;
            LayoutRoot_VertSplitter.IsTabStop = false;
            LayoutRoot_VertSplitter.Background = new SolidColorBrush(Colors.Transparent);
            LayoutRoot.Children.Add(LayoutRoot_VertSplitter);
            Grid.SetRow(LayoutRoot_VertSplitter, 1);
            Grid.SetColumn(LayoutRoot_VertSplitter, 0);

            // Информация о кубе
            // Просмотрщик куба
            m_ServerExplorer = new ServerExplorerCtrl();
            m_ServerExplorer.Margin = new Thickness(0, 0, 0, 5);   // Для RanetGridSplitter
            m_ServerExplorer.CubeBrowser.DragNodes = true;
            m_ServerExplorer.CubeBrowser.DragStarted += new EventHandler<DragNodeArgs<System.Windows.Controls.Primitives.DragStartedEventArgs>>(CubeBrowser_DragStarted);
            m_ServerExplorer.CubeBrowser.DragDelta += new EventHandler<DragNodeArgs<System.Windows.Controls.Primitives.DragDeltaEventArgs>>(CubeBrowser_DragDelta);
            m_ServerExplorer.CubeBrowser.DragCompleted += new EventHandler<DragNodeArgs<System.Windows.Controls.Primitives.DragCompletedEventArgs>>(CubeBrowser_DragCompleted);
            m_ServerExplorer.CubeSelected += new EventHandler<CustomEventArgs<string>>(m_ServerExplorer_CubeSelected);

            StackPanel rowsPanel = new StackPanel() {Orientation = Orientation.Vertical};
            var cubesComboHeader = new HeaderControl(UriResources.Images.HideEmptyRows16, Localization.ColumnsHeader) { Margin = new Thickness(0, 0, 0, 3) };     
            m_columnsList = new RanetCheckedListBox();
            m_ColumnNames = new Dictionary<string,bool>();
            //m_ColumnNames.Add("Display Folder",false);
            m_ColumnNames.Add("Kpi Name",true);
            m_columnsList.AddItem(new RanetCheckedItem() { Text = "Kpi Name" , IsChecked = true});
            m_ColumnNames.Add("Kpi Value",true);
            m_columnsList.AddItem(new RanetCheckedItem() { Text = "Kpi Value", IsChecked = true });
            m_ColumnNames.Add("Kpi Goal", true);
            m_columnsList.AddItem(new RanetCheckedItem() { Text = "Kpi Goal", IsChecked = true });
            m_ColumnNames.Add("Kpi Variance", true);
            m_columnsList.AddItem(new RanetCheckedItem() { Text = "Kpi Variance", IsChecked = true });
            m_ColumnNames.Add("Trend", true);
            m_columnsList.AddItem(new RanetCheckedItem() { Text = "Trend", IsChecked = true });
            m_ColumnNames.Add("Status", true);
            m_columnsList.AddItem(new RanetCheckedItem() { Text = "Status", IsChecked = true });
            m_ColumnNames.Add("Kpi Weight", false);
            m_columnsList.AddItem(new RanetCheckedItem() { Text = "Kpi Weight", IsChecked = false });
            m_columnsList.ListBox.SelectionChanged += new SelectionChangedEventHandler(m_columnsList_SelectionChanged);
            
            Input_LayoutRoot.Children.Add(m_ServerExplorer);
            Grid.SetRow(m_ServerExplorer, 0);

            rowsPanel.Children.Add(cubesComboHeader);
            rowsPanel.Children.Add(m_columnsList);
            Input_LayoutRoot.Children.Add(rowsPanel);
            Grid.SetRow(rowsPanel, 1);

            // Заголовок
            Table_LayoutRoot = new Grid();
            Table_LayoutRoot.Margin = new Thickness(0, 3, 0, 0);
            Table_LayoutRoot.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            Table_LayoutRoot.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            Table_LayoutRoot.RowDefinitions.Add(new RowDefinition() {Height = GridLength.Auto});
            HeaderControl pivot_Header = new HeaderControl(UriResources.Images.PivotGrid16, Localization.MdxDesigner_QueryResult) { Margin = new Thickness(0, 0, 0, 3) };
            Table_LayoutRoot.Children.Add(pivot_Header);
            Grid.SetRow(Table_LayoutRoot,0);
            m_Waiting = new BusyControl();
            m_Waiting.Text = Localization.Loading;
            Table_LayoutRoot.Children.Add(m_Waiting);
            Grid.SetRow(m_Waiting, 1);

            IsBusy = false;
            //m_OlapDataLoader = GetDataLoader();

            Table_LayoutRoot.Children.Add(m_DataGrid);
            Grid.SetRow(m_DataGrid, 2);
            // Сводная таблица            

            Pivot_Border = new Border() { Padding = new Thickness(3), BorderBrush = new SolidColorBrush(Colors.DarkGray), BorderThickness = new Thickness(1) };
            Pivot_Border.Margin = new Thickness(0, 1, 0, 0);
            Pivot_Border.Child = Table_LayoutRoot;

            Ouput_LayoutRoot.Children.Add(Pivot_Border);
            Grid.SetRow(Pivot_Border, 1);

            //var Output_HorzSplitter = new RanetGridSplitter();
            //Output_HorzSplitter.VerticalAlignment = VerticalAlignment.Bottom;
            //Output_HorzSplitter.HorizontalAlignment = HorizontalAlignment.Stretch;
            //Output_HorzSplitter.IsTabStop = false;
            //Output_HorzSplitter.Background = new SolidColorBrush(Colors.Transparent);
            //Ouput_LayoutRoot.Children.Add(Output_HorzSplitter);
            //Grid.SetRow(Output_HorzSplitter, 0);
            //Grid.SetColumn(Output_HorzSplitter, 0);            
            //

            this.Content = LayoutRoot;            
        }

        void showAllButton_Unchecked(object sender, RoutedEventArgs e)
        {
            this.ShowAllKPIs = false;
            this.m_sourceCollection = new List<KpiView>();
            if (this.CustomizeSettingsImmediately)
                this.UpdateDataGrid(this.ShowAllKPIs);
        }

        void showAllButton_Checked(object sender, RoutedEventArgs e)
        {
            this.ShowAllKPIs = true;
            if (this.CustomizeSettingsImmediately)
                this.UpdateDataGrid(this.ShowAllKPIs);
        }

        void m_ApplyChanges_Click(object sender, RoutedEventArgs e)
        {
            UpdateDataGrid(this.ShowAllKPIs);
        }

        void m_columnsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var item in e.AddedItems)
            {
                if (item is RanetCheckedItem)
                {
                    if (m_ColumnNames.ContainsKey((item as RanetCheckedItem).Text))
                    {
                        m_ColumnNames[(item as RanetCheckedItem).Text] = true;
                    }
                }
            }
            foreach (var item in e.RemovedItems)
            {
                if (item is RanetCheckedItem)
                {
                    if (m_ColumnNames.ContainsKey((item as RanetCheckedItem).Text))
                    {
                        m_ColumnNames[(item as RanetCheckedItem).Text] = false;
                    }
                }
            }
            if (this.CustomizeSettingsImmediately)
                this.UpdateDataGrid(this.ShowAllKPIs);
        }
        
        public void ShowMetadataArea(bool show)
        {
            if (show)
            {
                m_InputColumn.Width = new GridLength(m_InputColumnWidth);
                Input_Border.Visibility = Visibility.Visible;
                LayoutRoot_VertSplitter.Visibility = Visibility.Visible;
            }
            else
            {
                m_InputColumnWidth = m_InputColumn.ActualWidth;
                m_InputColumn.Width = new GridLength(0.0);
                Input_Border.Visibility = Visibility.Collapsed;
                LayoutRoot_VertSplitter.Visibility = Visibility.Collapsed;
            }
        }


        void m_ShowMetadataArea_Unchecked(object sender, RoutedEventArgs e)
        {
            ShowMetadataArea(false);
        }

        void m_ShowMetadataArea_Checked(object sender, RoutedEventArgs e)
        {
            ShowMetadataArea(true);
        }

        #region Drag and Drop

        void m_ServerExplorer_CubeSelected(object sender, CustomEventArgs<string> e)
        {
            if (!string.IsNullOrEmpty(e.Args))
            {
                this.CubeName = e.Args;
            }
        }

        /// <summary>
        /// Позиция старта таскания
        /// </summary>
        Point m_DragStart = new Point(0, 0);
        /// <summary>
        /// Предыдущая позиция при перетаскивании
        /// </summary>
        Point m_PrevDrag = new Point(0, 0);
        void CubeBrowser_DragCompleted(object sender, DragNodeArgs<System.Windows.Controls.Primitives.DragCompletedEventArgs> e)
        {
            try
            {
                if (e.Args.Canceled == false)
                {
                    TreeViewItem node = e.Node;

                    if (e.Node is KpiTreeNode)
                    {
                        DropToArea(node as KpiTreeNode);
                        Raise_KpisLoadCompleted();
                    }
                }
            }
            finally
            {
                this.KpiDataContainer.IsReadyToDrop = false;
            }
        }

        private void DropToArea(KpiTreeNode kpiTreeNode)
        {
            if (kpiTreeNode.Info is KpiInfo)
            {
                foreach (var storage in m_KpiStorage)
                {
                    if (storage.Key.Equals((kpiTreeNode.Info as KpiInfo).Caption))
                    {
                        var x = from kpiView in m_sourceCollection
                                where kpiView.Caption == (kpiTreeNode.Info as KpiInfo).Caption
                                select kpiView;
                        if (!(x.Count()>0))
                        {
                            m_sourceCollection.Add(this.GetKpiViewItem(storage.Value,(kpiTreeNode.Info as KpiInfo)));
                        }
                    }
                }
            }
        }

        private KpiView GetKpiViewItem(KpiView kpiView, KpiInfo info)
        {
            var row = new KpiView();
            if (kpiView != null)
            {
                row.Caption = kpiView.Caption;
                row.KpiGoal = kpiView.KpiGoal;
                row.KpiValue = kpiView.KpiValue;
                //row.KpiValue = view.Value.KpiValue;
                row.KpiTrend = kpiView.KpiTrend;
                row.KpiStatus = kpiView.KpiStatus;
                //row.DisplayFolder = view.Value.DisplayFolder;
                row.KpiVariance = kpiView.KpiVariance;
                //row.KpiVariance = view.Value.KpiVariance;                                
                row.DisplayFolder = info.DisplayFolder;
                row.StatusGraphic = info.StatusGraphic + "_" + row.KpiStatus + ".png";
                row.TrendGraphic = info.TrendGraphic + "_" + row.KpiTrend + ".png";
            }
            return row;
        }

        void CubeBrowser_DragDelta(object sender, DragNodeArgs<System.Windows.Controls.Primitives.DragDeltaEventArgs> e)
        {
            this.KpiDataContainer.IsReadyToDrop = false;

            Point m_DragDelta = new Point(m_PrevDrag.X + e.Args.HorizontalChange, m_PrevDrag.Y + e.Args.VerticalChange);          


            if (e.Node is KpiTreeNode)
            {
                Rect m_DataArea_Bounds = AgControlBase.GetSLBounds(this.KpiDataContainer);
                if (m_DataArea_Bounds.Contains(m_DragDelta))
                {
                    this.KpiDataContainer.IsReadyToDrop = true;
                }
            }

            m_PrevDrag = m_DragDelta;
        }

        void CubeBrowser_DragStarted(object sender, DragNodeArgs<System.Windows.Controls.Primitives.DragStartedEventArgs> e)
        {
            m_DragStart = new Point(e.Args.HorizontalOffset, e.Args.VerticalOffset);
            m_PrevDrag = m_DragStart;
        }

        #endregion

        public void Initialize()
        {
            if (m_ServerExplorer.LogManager != LogManager)
                m_ServerExplorer.LogManager = LogManager;

            this.CanSelectCubes = true;
            m_ServerExplorer.CubeBrowser.Initialized += new EventHandler(CubeBrowser_Initialized);
            m_ServerExplorer.Initialize();
        }

        void CubeBrowser_Initialized(object sender, EventArgs e)
        {
            m_ServerExplorer.CubeBrowser.Initialized -= new EventHandler(CubeBrowser_Initialized);
            InitializeKpiDataGrid();            
        }

        protected virtual void InitializeKpiDataGrid()
        {
            if (!this.m_Initialized)
            {
                if (this.ServerExplorer.CubeBrowser != null && this.ServerExplorer.CubeBrowser.CubeInfo != null)
                {
                    m_sourceCollection = new List<KpiView>();                
                    // Добавляем узел KPIs
                    KPIsFolderTreeNode kpisNode = new KPIsFolderTreeNode();
                    this.CreateKPIs(kpisNode, this.ServerExplorer.CubeBrowser.CubeInfo,true);
                }
            }
        }

        protected void CreateKPIs(CustomTreeNode parentNode, CubeDefInfo cube, bool kpiParameters)
        {
            if (cube != null)
            {
                this.IsBusy = true;
                m_KpiStorage = new Dictionary<string, KpiView>();
                this.m_SourceKpis = cube.Kpis;
                this.KpiDataContainer.Grid.Columns.Clear();
                this.RowIndex = this.ExecuteQueryForRow(0);
                this.KpisLoadCompleted += new EventHandler(KpiViewer_KpisLoadCompleted);
            }
        }

        protected void UpdateDataGrid(bool showAll)
        {
            if (this.m_Initialized)
            {
                if (showAll)
                {
                    m_sourceCollection = GetKpisList();
                    Tree = new PagedCollectionView(m_sourceCollection);
                    //Tree.GroupDescriptions.Add(new PropertyGroupDescription("KpiStatus"));
                    this.KpiDataContainer.Grid.ItemsSource = Tree;
                    this.CustomizeDataGridProperties();
                }
                else
                {
                    Tree = new PagedCollectionView(m_sourceCollection);
                    this.KpiDataContainer.Grid.ItemsSource = Tree;
                    this.CustomizeDataGridProperties();
                }
            }

        }

        void KpiViewer_KpisLoadCompleted(object sender, EventArgs e)
        {
            this.IsBusy = false;
            try
            {
                this.m_Initialized = true;
                UpdateDataGrid(this.ShowAllKPIs);
                //this.m_KpiStorage = new Dictionary<string, KpiView>();                            
            }
            catch (Exception ex)
            {
                LogManager.LogError(sender,ex.Message);                
            }
            
        }       

        void CustomizeDataGridProperties()
        {
            this.KpiDataContainer.Grid.AutoGenerateColumns = false;
            this.KpiDataContainer.Grid.IsReadOnly = true;
            this.KpiDataContainer.Grid.AreRowGroupHeadersFrozen = true;
            this.KpiDataContainer.Grid.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            this.KpiDataContainer.Grid.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;

            this.KpiDataContainer.Grid.Columns.Clear();
            //Column "Name"
            if (this.IsColumnVisible("Kpi Name"))
                this.KpiDataContainer.Grid.Columns.Add(new DataGridTextColumn() { Header = Localization.KPIViewer_ColumnName, Binding = new Binding("Caption") });
            //Column "Value"
            if (this.IsColumnVisible("Kpi Value"))
                this.KpiDataContainer.Grid.Columns.Add(new DataGridTextColumn() { Header = Localization.KPIViewer_ColumnValue, Binding = new Binding("KpiValue") });
            //Column "Goal"
            if (this.IsColumnVisible("Kpi Goal"))
                this.KpiDataContainer.Grid.Columns.Add(new DataGridTextColumn() { Header = Localization.KPIViewer_ColumnGoal, Binding = new Binding("KpiGoal") });
            //Column "Variance"
            if (this.IsColumnVisible("Kpi Variance"))
                this.KpiDataContainer.Grid.Columns.Add(new DataGridTextColumn() { Header = Localization.KPIViewer_ColumnVariance, Binding = new Binding("KpiVariance") });
            //Column "Status"
            //this.KpiDataGrid.Columns.Add(new DataGridTextColumn() { Header = "Status", Binding = new Binding("StatusGraphic") });
            if (this.IsColumnVisible("Status"))
                this.KpiDataContainer.Grid.Columns.Add(new IconDataGridColumn() { Header = Localization.KPIViewer_ColumnStatus, Resource = "Status" });
            //Column "Trend"
            //this.KpiDataGrid.Columns.Add(new DataGridTextColumn() { Header = "Trend", Binding = new Binding("TrendGraphic") });
            if (this.IsColumnVisible("Trend"))
                this.KpiDataContainer.Grid.Columns.Add(new IconDataGridColumn() { Header = Localization.KPIViewer_ColumnTrend, Resource = "Trend" });
            // Column "Weight"
            if (this.IsColumnVisible("Kpi Weight"))
                this.KpiDataContainer.Grid.Columns.Add(new DataGridTextColumn() { Header = "Weight", Binding = new Binding("KpiWeight") });
            // Column "DisplayFolder"
            if (this.IsColumnVisible("Display Folder"))
                this.KpiDataContainer.Grid.Columns.Add(new DataGridTextColumn() { Header = "Display folder", Binding = new Binding("DisplayFolder") });
            if (!string.IsNullOrEmpty(this.GroupProperty))
            {
                this.Tree.GroupDescriptions.Add(new PropertyGroupDescription(this.GroupProperty));
            }
            //this.Tree.GroupDescriptions.Clear();
            //this.Tree.GroupDescriptions.Add(new PropertyGroupDescription("DisplayFolder"));
                      
            
        }      

        private bool IsColumnVisible(string id)
        {
            if (this.m_ColumnNames.ContainsKey(id))
            {
                return m_ColumnNames[id];
            }
            return false;
        }

        private List<KpiView> GetKpisList()
        {
            var result = new List<KpiView>();
            foreach (var view in m_KpiStorage)
            {
                var row = new KpiView();
                if (view.Value != null)
                {
                    row.Caption = view.Value.Caption;
                    row.KpiGoal = view.Value.KpiGoal;
                    row.KpiValue = view.Value.KpiValue;
                    //row.KpiValue = view.Value.KpiValue;
                    row.KpiTrend = view.Value.KpiTrend;
                    row.KpiStatus = view.Value.KpiStatus;
                    //row.DisplayFolder = view.Value.DisplayFolder;
                    row.KpiVariance = view.Value.KpiVariance;
                    //row.KpiVariance = view.Value.KpiVariance;                    
                }
                var x = from kpi in m_SourceKpis
                        where kpi.Caption.Equals(view.Key)
                        select kpi;
                if (x.First() != null)
                {
                    row.DisplayFolder = x.First().DisplayFolder;
                    row.StatusGraphic = x.First().StatusGraphic+"_"+row.KpiStatus+".png";
                    row.TrendGraphic = x.First().TrendGraphic+"_"+row.KpiTrend+".png";
                }
                result.Add(row);
            }
            return result;
        }

        private int ExecuteQueryForRow(int index)
        {
            if (index < m_SourceKpis.Count)
            {
                string query = this.GenerateQueryForRow(m_SourceKpis[index]);
                MdxQueryArgs args = CommandHelper.CreateMdxQueryArgs(m_Connection, query);
                OlapDataLoader.LoadData(args, null);
                return index;
            }
            return -1;
        }

        void Loader_DataLoaded(object sender, DataLoaderEventArgs e)
        {
            //IsBusy = false;

            if (e.Error != null)
            {
                LogManager.LogError(this, e.Error.ToString());
                return;
            }

            if (e.Result.ContentType == InvokeContentType.Error)
            {
                LogManager.LogError(this, e.Result.Content);
                return;
            }

            //CellSetData cs_descr = XmlSerializationUtility.XmlStr2Obj<CellSetData>(e.Result);
            CellSetData cs_descr = CellSetData.Deserialize(e.Result.Content);
            this.AddKpiRowToStorage(cs_descr);
            if (this.RowIndex < 0)
            {
                Raise_KpisLoadCompleted();
            }
        }

        private void AddKpiRowToStorage(CellSetData cs_descr)
        {
            if (cs_descr.Axes.Count > 0)
            {
                if (cs_descr.Cells.Count > 1 && cs_descr.Cells[0].Value != null &&
                    !m_KpiStorage.ContainsKey(cs_descr.Cells[0].Value.Value.ToString()))
                {
                    var row = new KpiView();
                    string key = cs_descr.Cells[0].Value.Value.ToString();
                    row.Caption = key;
                    double num;
                    row.KpiValue = cs_descr.Cells.Count > 1 && double.TryParse(cs_descr.Cells[1].Value.Value.ToString(), out num) ? num : double.NaN;
                    double num1;
                    row.KpiGoal = cs_descr.Cells.Count > 2 && double.TryParse(cs_descr.Cells[2].Value.Value.ToString(), out num1) ? num1 : double.NaN;
                    double num2;
                    row.KpiVariance = cs_descr.Cells.Count > 3 && double.TryParse(cs_descr.Cells[3].Value.Value.ToString(), out num2) ? num2 : double.NaN;                    
                    row.KpiStatus = cs_descr.Cells.Count > 4 ? cs_descr.Cells[4].Value.Value.ToString() : String.Empty;
                    row.KpiTrend = cs_descr.Cells.Count > 5 ? cs_descr.Cells[5].Value.Value.ToString() : String.Empty;
                    row.KpiWeight = cs_descr.Cells.Count > 6 ? cs_descr.Cells[6].Value.Value.ToString() : String.Empty;
                    m_KpiStorage.Add(key, row);
                }
            }
            if (this.RowIndex > -1)
            {
                this.RowIndex = this.ExecuteQueryForRow(++this.RowIndex);
            }            

        }

        public event EventHandler KpisLoadCompleted;
        void Raise_KpisLoadCompleted()
        {
            EventHandler handler = KpisLoadCompleted;
            if (handler != null)
            {
                this.RowIndex = -1;
                handler(this, new EventArgs());
            }
        }

        internal const string KPIValue = "KPIVALUE(\"{0}\")";
        internal const string KPIGoal = "KPIGOAL(\"{0}\")";
        internal const string KPIStatus = "KPISTATUS(\"{0}\")";
        internal const string KPITrend = "KPITREND(\"{0}\")";
        internal const string KPIWeight = "KPIWEIGHT(\"{0}\")";

        private string GenerateQueryForRow(KpiInfo info)
        {
            string variance = String.Format("with member [Variance] as " + KPIValue, info.Caption) + String.Format("-"+KPIGoal,info.Caption);
            string id = String.Format(" member [Id] as \"{0}\"", info.Name);
            string result = variance + id + " SELECT { [Id], ";
            result += String.Format(KPIValue + " , ", info.Caption);
            result += String.Format(KPIGoal + " , ", info.Caption);
            result += " [Variance], ";
            result += String.Format(KPIStatus + " , ", info.Caption);
            result += String.Format(KPITrend + " , ", info.Caption);
            result += String.Format(KPIWeight, info.Caption) + " } ";
            string prop1 =
                "DIMENSION PROPERTIES PARENT_UNIQUE_NAME , HIERARCHY_UNIQUE_NAME , CUSTOM_ROLLUP , UNARY_OPERATOR , KEY0 ON 0 ";
            String cube = String.Format("FROM [{0}] ",m_CubeName);
            string prop2 =
                "CELL PROPERTIES BACK_COLOR , CELL_ORDINAL , FORE_COLOR , FONT_NAME , FONT_SIZE , FONT_FLAGS , FORMAT_STRING , VALUE , FORMATTED_VALUE , UPDATEABLE";
            return result + prop1 + cube + prop2;
        }

        private int RowIndex
        { get; set; }

        #region OLAP Properties
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
                m_ServerExplorer.Connection = value;                
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
                m_ServerExplorer.CubeName = value;
                m_CubeName = value;
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
                m_ServerExplorer.SubCube = value;
            }
        }

        #endregion

        public override ILogService LogManager
        {
            get { return base.LogManager; }
            set
            {
                base.LogManager = value;
                m_ServerExplorer.LogManager = value;                
            }
        }

        IDataLoader m_OlapDataLoader = null;
        public IDataLoader OlapDataLoader
        {
            set
            {
                if (m_OlapDataLoader != null)
                {
                    m_OlapDataLoader.DataLoaded -= new EventHandler<DataLoaderEventArgs>(Loader_DataLoaded);
                }
                m_OlapDataLoader = value;
                m_OlapDataLoader.DataLoaded += new EventHandler<DataLoaderEventArgs>(Loader_DataLoaded);
            }
            get
            {
                //if (m_OlapDataLoader == null)
                //{
                    m_OlapDataLoader = GetDataLoader();
                    m_OlapDataLoader.DataLoaded += new EventHandler<DataLoaderEventArgs>(Loader_DataLoaded);
                //}
                return m_OlapDataLoader;
            }
        }

        public bool m_IsBusy = false;
        public bool IsBusy
        {
            get
            {
                return m_IsBusy;
            }
            set
            {
                if (value)
                {
                    this.KpiDataContainer.Visibility = System.Windows.Visibility.Collapsed;
                    m_Waiting.Visibility = Visibility.Visible;
                }
                else
                {
                    this.KpiDataContainer.Visibility = System.Windows.Visibility.Visible;
                    m_Waiting.Visibility = Visibility.Collapsed;
                }
                m_IsBusy = value;
            }
        }

        public bool CustomizeSettingsImmediately
        {
            get; set;
        }

        public string GroupProperty
        {
            get; set;
        }

        protected virtual IDataLoader GetDataLoader()
        {
            return new OlapDataLoader(URL);
        }
      
        public bool ShowAllKPIs
        { 
            get; 
            set; 
        }

        public bool CanSelectCubes
        {
            get { return this.m_ServerExplorer.CanSelectCube; }
            set { this.m_ServerExplorer.CanSelectCube = value; }
        }

        #region Silverlight controls

        protected ServerExplorerCtrl ServerExplorer
        {
            get { return this.m_ServerExplorer; }
        }
        
        protected RanetToolBar ToolBar
        {
            get { return this.m_ToolBar; }
        }
        protected DragDropDataGrid KpiDataContainer
        {
            get { return m_DataGrid; }
        }

        protected Border MetaDataArea
        {
            get { return this.Input_Border; }
        }

        protected RanetCheckedListBox ColumnsList
        {
            get { return this.m_columnsList; }
        }
        #endregion
    }
}
