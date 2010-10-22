/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
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
using Wing.Olap.Controls.General;
using System.Windows.Controls.Primitives;
using System.Collections.Generic;
using Wing.Olap.Controls.MdxDesigner;
using Wing.Olap.Controls.General.Tree;
using Wing.Olap.Core.Metadata;
using Wing.Olap.Controls.ContextMenu;
using System.Text;
using Wing.Olap.Controls.Buttons;
using Wing.Olap.Controls.ValueCopy;
using Wing.Olap.Controls.MdxDesigner.Wrappers;
using Wing.Olap.Core;
using Wing.Olap.Controls.ToolBar;
using System.IO;
using System.IO.IsolatedStorage;
using Wing.Olap.Controls.MdxDesigner.Filters;
using Wing.Olap.Controls.Forms;
using Wing.Olap.Core.Storage;
using Wing.Olap.Controls.MdxDesigner.CalculatedMembers;
using Wing.Olap.Controls.MemberChoice.Info;
using Wing.Olap.Providers;
using Wing.Olap.Core.Data;
using Wing.Olap.Core.Providers;

namespace Wing.Olap.Controls
{
    public class PivotMdxDesignerControl : AgControlBase
    {
        ServerExplorerCtrl m_ServerExplorer;

        PivotAreaContainer m_RowsAreaContainer;
        PivotAreaContainer m_ColumnsAreaContainer;
        PivotAreaContainer m_FilterAreaContainer;
        PivotAreaContainer m_DataAreaContainer;

        Grid LayoutRoot;
        RanetToolBar m_ToolBar;

        Wing.Olap.Controls.General.RichTextBox m_MdxQuery;
        UpdateablePivotGridControl m_PivotGrid;
        protected UpdateablePivotGridControl PivotGrid
        {
            get { return m_PivotGrid; }
        }

        RanetToggleButton m_ShowMetadataArea;
        RanetToggleButton m_ShowMDXQuery;
        RanetToggleButton m_EditMDXQuery;
        RanetToggleButton m_RunQueryAutomatic;
        RanetToolBarButton m_ExecuteQuery;
        RanetToolBarButton m_CalculatedMemberEditor;
        RanetToolBarButton m_ExportLayout;
        RanetToolBarButton m_ImportLayout;

        RanetToolBarSplitter m_RunAreaSplitter;
        RanetToolBarSplitter m_StorageAreaSplitter;

        GridSplitter LayoutRoot_VertSplitter;
        Border Input_Border;
        GridSplitter Output_HorzSplitter;
        Border Mdx_Border;
        Border Pivot_Border;
        Grid Areas_LayoutRoot;

        ColumnDefinition m_Input_Column;
        RowDefinition m_MDX_Row;

        double m_InputColumnWidth = 350;
        double m_MDXRowHeight = 200;

        #region constructors
        public PivotMdxDesignerControl()
        {
            ScrollViewer Scroll = new ScrollViewer();
            Scroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            Scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;

            LayoutRoot = new Grid();
            LayoutRoot.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            LayoutRoot.RowDefinitions.Add(new RowDefinition());
            m_Input_Column = new ColumnDefinition() { Width = new GridLength(m_InputColumnWidth) };
            LayoutRoot.ColumnDefinitions.Add(m_Input_Column);
            LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition() { /*Width = new GridLength(2, GridUnitType.Star)*/ });

            // ТУЛБАР 
            m_ToolBar = new RanetToolBar();
            m_ToolBar.Margin = new Thickness(0, 0, 0, 4);
            //LayoutRoot.Children.Add(m_ToolBar);
            Grid.SetColumnSpan(m_ToolBar, 2);

            m_ShowMetadataArea = new RanetToggleButton();
            m_ShowMetadataArea.ClickMode = ClickMode.Press;
            m_ShowMetadataArea.Click += new RoutedEventHandler(m_ShowMetadataArea_Click);
            m_ShowMetadataArea.IsChecked = new bool?(true);
            m_ShowMetadataArea.Content = UiHelper.CreateIcon(UriResources.Images.Menu16);
            ToolTipService.SetToolTip(m_ShowMetadataArea, Localization.MdxDesigner_ShowQueryDesigner_ToolTip);
            m_ToolBar.AddItem(m_ShowMetadataArea);

            m_ShowMDXQuery = new RanetToggleButton();
            m_ShowMDXQuery.ClickMode = ClickMode.Press;
            m_ShowMDXQuery.Click += new RoutedEventHandler(m_ShowMDXQuery_Click);
            m_ShowMDXQuery.IsChecked = new bool?(false);
            m_ShowMDXQuery.Content = UiHelper.CreateIcon(UriResources.Images.Mdx16);
            ToolTipService.SetToolTip(m_ShowMDXQuery, Localization.MdxDesigner_ShowQuery_ToolTip);
            //m_ToolBar.AddItem(m_ShowMDXQuery);

            m_EditMDXQuery = new RanetToggleButton();
            m_EditMDXQuery.ClickMode = ClickMode.Press;
            m_EditMDXQuery.Click += new RoutedEventHandler(m_EditMDXQuery_Click);
            m_EditMDXQuery.IsChecked = new bool?(false);
            m_EditMDXQuery.Content = UiHelper.CreateIcon(UriResources.Images.Edit16);
            ToolTipService.SetToolTip(m_EditMDXQuery, Localization.MdxDesigner_EditQuery_ToolTip);
            //m_ToolBar.AddItem(m_EditMDXQuery);

            m_RunAreaSplitter = new RanetToolBarSplitter();
            m_ToolBar.AddItem(m_RunAreaSplitter);

            m_CalculatedMemberEditor = new RanetToolBarButton();
            m_CalculatedMemberEditor.Content = UiHelper.CreateIcon(UriResources.Images.CustomCalculations16);
            m_CalculatedMemberEditor.Click += new RoutedEventHandler(m_CalculatedMemberEditor_Click);
            ToolTipService.SetToolTip(m_CalculatedMemberEditor, Localization.MdxDesigner_CalculatedMemberEditor);
            m_ToolBar.AddItem(m_CalculatedMemberEditor);

            m_RunQueryAutomatic = new RanetToggleButton();
            m_RunQueryAutomatic.ClickMode = ClickMode.Press;
            m_RunQueryAutomatic.IsChecked = new bool?(true);
            m_RunQueryAutomatic.Content = UiHelper.CreateIcon(UriResources.Images.AutoRun16);
            ToolTipService.SetToolTip(m_RunQueryAutomatic, Localization.MdxDesigner_RunQueryAutomatic);
            //m_ToolBar.AddItem(m_RunQueryAutomatic);

            m_ExecuteQuery = new RanetToolBarButton();
            m_ExecuteQuery.Content = UiHelper.CreateIcon(UriResources.Images.Run16);
            m_ExecuteQuery.Click += new RoutedEventHandler(m_ExecuteQuery_Click);
            ToolTipService.SetToolTip(m_ExecuteQuery, Localization.MdxDesigner_ExecuteQuery);
            //m_ToolBar.AddItem(m_ExecuteQuery);

            m_StorageAreaSplitter = new RanetToolBarSplitter();
            m_ToolBar.AddItem(m_StorageAreaSplitter);

            m_ImportLayout = new RanetToolBarButton();
            m_ImportLayout.Content = UiHelper.CreateIcon(UriResources.Images.FileImport16);
            m_ImportLayout.Click += new RoutedEventHandler(m_ImportLayout_Click);
            ToolTipService.SetToolTip(m_ImportLayout, Localization.MdxDesigner_ImportLayout_ToolTip);
            //m_ToolBar.AddItem(m_ImportLayout);

            m_ExportLayout = new RanetToolBarButton();
            m_ExportLayout.Content = UiHelper.CreateIcon(UriResources.Images.FileExport16);
            m_ExportLayout.Click += new RoutedEventHandler(m_ExportLayout_Click);
            ToolTipService.SetToolTip(m_ExportLayout, Localization.MdxDesigner_ExportLayout_ToolTip);
            //m_ToolBar.AddItem(m_ExportLayout);

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
            m_MDX_Row = new RowDefinition() { Height = new GridLength(0.0) };
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
            m_ServerExplorer.CubeBrowser.DragStarted += new EventHandler<DragNodeArgs<DragStartedEventArgs>>(m_CubeBrowser_DragStarted);
            m_ServerExplorer.CubeBrowser.DragDelta += new EventHandler<DragNodeArgs<DragDeltaEventArgs>>(m_CubeBrowser_DragDelta);
            m_ServerExplorer.CubeBrowser.DragCompleted += new EventHandler<DragNodeArgs<DragCompletedEventArgs>>(m_CubeBrowser_DragCompleted);
            m_ServerExplorer.CubeSelected += new EventHandler<CustomEventArgs<string>>(m_ServerExplorer_CubeSelected);

            Input_LayoutRoot.Children.Add(m_ServerExplorer);
            Grid.SetRow(m_ServerExplorer, 0);

            Areas_LayoutRoot = new Grid();
            Areas_LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition());
            Areas_LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition());
            Areas_LayoutRoot.RowDefinitions.Add(new RowDefinition());
            Areas_LayoutRoot.RowDefinitions.Add(new RowDefinition());

            m_FilterAreaContainer = new PivotAreaContainer();
            m_FilterAreaContainer.ItemRemoved += new EventHandler<AreaItemArgs>(AreaContainer_ItemRemoved);
            m_FilterAreaContainer.Margin = new Thickness(0, 3, 0, 0);
            m_FilterAreaContainer.Icon = UriResources.Images.FiltersArea16;
            m_FilterAreaContainer.Caption = Localization.MdxDesigner_FilterArea_Caption;
            m_FilterAreaContainer.BeforeShowContextMenu += new EventHandler<AreaItemArgs>(m_FilterAreaContainer_BeforeShowContextMenu);
            m_FilterAreaContainer.ItemsListChanged += new EventHandler(AreaContainer_ItemsListChanged);
            Areas_LayoutRoot.Children.Add(m_FilterAreaContainer);
            Grid.SetRow(m_FilterAreaContainer, 0);
            Grid.SetColumn(m_FilterAreaContainer, 0);

            m_RowsAreaContainer = new PivotAreaContainer();
            m_RowsAreaContainer.ItemRemoved += new EventHandler<AreaItemArgs>(AreaContainer_ItemRemoved);
            m_RowsAreaContainer.Margin = new Thickness(0, 5, 0, 0);
            m_RowsAreaContainer.Icon = UriResources.Images.RowsArea16;
            m_RowsAreaContainer.Caption = Localization.MdxDesigner_RowsArea_Caption;
            m_RowsAreaContainer.BeforeShowContextMenu += new EventHandler<AreaItemArgs>(m_RowsAreaContainer_BeforeShowContextMenu);
            m_RowsAreaContainer.ItemsListChanged += new EventHandler(AreaContainer_ItemsListChanged);
            Areas_LayoutRoot.Children.Add(m_RowsAreaContainer);
            Grid.SetRow(m_RowsAreaContainer, 1);
            Grid.SetColumn(m_RowsAreaContainer, 0);

            m_ColumnsAreaContainer = new PivotAreaContainer();
            m_ColumnsAreaContainer.ItemRemoved += new EventHandler<AreaItemArgs>(AreaContainer_ItemRemoved);
            m_ColumnsAreaContainer.Margin = new Thickness(5, 3, 0, 0);
            m_ColumnsAreaContainer.Icon = UriResources.Images.ColumnsArea16;
            m_ColumnsAreaContainer.Caption = Localization.MdxDesigner_ColumnsArea_Caption;
            m_ColumnsAreaContainer.BeforeShowContextMenu += new EventHandler<AreaItemArgs>(m_ColumnsAreaContainer_BeforeShowContextMenu);
            m_ColumnsAreaContainer.ItemsListChanged += new EventHandler(AreaContainer_ItemsListChanged);
            Areas_LayoutRoot.Children.Add(m_ColumnsAreaContainer);
            Grid.SetRow(m_ColumnsAreaContainer, 0);
            Grid.SetColumn(m_ColumnsAreaContainer, 1);

            m_DataAreaContainer = new PivotAreaContainer();
            m_DataAreaContainer.ItemRemoved += new EventHandler<AreaItemArgs>(AreaContainer_ItemRemoved);
            m_DataAreaContainer.Margin = new Thickness(5, 5, 0, 0);
            m_DataAreaContainer.Icon = UriResources.Images.DataArea16;
            m_DataAreaContainer.Caption = Localization.MdxDesigner_DataArea_Caption;
            m_DataAreaContainer.ItemsListChanged += new EventHandler(AreaContainer_ItemsListChanged);
            Areas_LayoutRoot.Children.Add(m_DataAreaContainer);
            Grid.SetRow(m_DataAreaContainer, 1);
            Grid.SetColumn(m_DataAreaContainer, 1);

            Input_LayoutRoot.Children.Add(Areas_LayoutRoot);
            Grid.SetRow(Areas_LayoutRoot, 1);

            GridSplitter Input_HorzSplitter = new RanetGridSplitter();
            Input_HorzSplitter.VerticalAlignment = VerticalAlignment.Bottom;
            Input_HorzSplitter.HorizontalAlignment = HorizontalAlignment.Stretch;
            Input_HorzSplitter.IsTabStop = false;
            Input_HorzSplitter.Background = new SolidColorBrush(Colors.Transparent);
            Input_LayoutRoot.Children.Add(Input_HorzSplitter);
            Grid.SetRow(Input_HorzSplitter, 0);
            Grid.SetColumn(Input_HorzSplitter, 0);

            // Результат выполнения запроса
            Grid Pivot_LayotRoot = new Grid();
            Pivot_LayotRoot.Margin = new Thickness(0, 3, 0, 0);
            Pivot_LayotRoot.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            Pivot_LayotRoot.RowDefinitions.Add(new RowDefinition());

            // Заголовок
            HeaderControl pivot_Header = new HeaderControl(UriResources.Images.PivotGrid16, Localization.MdxDesigner_QueryResult) { Margin = new Thickness(0, 0, 0, 3) };
            Pivot_LayotRoot.Children.Add(pivot_Header);

            // Сводная таблица
            m_PivotGrid = new UpdateablePivotGridControl();
            m_PivotGrid.Margin = new Thickness(0, 0, 0, 0);
            m_PivotGrid.AutoWidthColumns = true;
            m_PivotGrid.DataReorganizationType = DataReorganizationTypes.LinkToParent;
            m_PivotGrid.DefaultMemberAction = MemberClickBehaviorTypes.DrillDown;
            m_PivotGrid.IsUpdateable = true;
            m_PivotGrid.ColumnsIsInteractive = true;
            m_PivotGrid.RowsIsInteractive = true;
            m_PivotGrid.ShowToolBar = true;
            Pivot_LayotRoot.Children.Add(m_PivotGrid);
            Grid.SetRow(m_PivotGrid, 1);

            Pivot_Border = new Border() { Padding = new Thickness(3), BorderBrush = new SolidColorBrush(Colors.DarkGray), BorderThickness = new Thickness(1) };
            Pivot_Border.Margin = new Thickness(0, 0, 0, 0);
            Pivot_Border.Child = Pivot_LayotRoot;

            Ouput_LayoutRoot.Children.Add(Pivot_Border);
            Grid.SetRow(Pivot_Border, 1);

            Output_HorzSplitter = new RanetGridSplitter();
            Output_HorzSplitter.VerticalAlignment = VerticalAlignment.Bottom;
            Output_HorzSplitter.HorizontalAlignment = HorizontalAlignment.Stretch;
            Output_HorzSplitter.IsTabStop = false;
            Output_HorzSplitter.Background = new SolidColorBrush(Colors.Transparent);
            Ouput_LayoutRoot.Children.Add(Output_HorzSplitter);
            Grid.SetRow(Output_HorzSplitter, 0);
            Grid.SetColumn(Output_HorzSplitter, 0);

            // Информация о MDX запросе
            Grid Mdx_LayotRoot = new Grid();
            Mdx_LayotRoot.Margin = new Thickness(0, 3, 0, 0);
            Mdx_LayotRoot.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            Mdx_LayotRoot.RowDefinitions.Add(new RowDefinition());

            // Заголовок
            HeaderControl mdx_Header = new HeaderControl(UriResources.Images.Mdx16, Localization.MdxDesigner_MdxQuery) { Margin = new Thickness(0, 0, 0, 3) };
            Mdx_LayotRoot.Children.Add(mdx_Header);

            // Текст запроса
            m_MdxQuery = new Wing.Olap.Controls.General.RichTextBox();
            m_MdxQuery.AcceptsReturn = true;
            m_MdxQuery.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            m_MdxQuery.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            m_MdxQuery.IsReadOnly = true;
            Mdx_LayotRoot.Children.Add(m_MdxQuery);
            Grid.SetRow(m_MdxQuery, 1);

            Mdx_Border = new Border() { Padding = new Thickness(3), BorderBrush = new SolidColorBrush(Colors.DarkGray), BorderThickness = new Thickness(1) };
            Mdx_Border.Margin = new Thickness(0, 0, 0, 5);
            Mdx_Border.Child = Mdx_LayotRoot;

            Ouput_LayoutRoot.Children.Add(Mdx_Border);
            Grid.SetRow(Mdx_Border, 0);

            AllowDragDrop(m_FilterAreaContainer);
            AllowDragDrop(m_ColumnsAreaContainer);
            AllowDragDrop(m_RowsAreaContainer);
            AllowDragDrop(m_DataAreaContainer);

            //Scroll.Content = LayoutRoot;

            m_ServerExplorer.OlapDataLoader = GetOlapDataLoader();
            m_PivotGrid.OlapDataLoader = GetOlapDataLoader();
            m_StorageManager = GetStorageManager();
            m_StorageManager.InvokeCompleted += new EventHandler<DataLoaderEventArgs>(StorageManager_ActionCompleted);

            this.Content = LayoutRoot;
        }

        void m_ServerExplorer_CubeSelected(object sender, CustomEventArgs<string> e)
        {
            OnCubeChanged();
            return;
            if (CalculatedMembers.Count > 0 ||
                    CalculatedNamedSets.Count > 0 ||
                    m_FilterAreaContainer.Items.Count > 0 ||
                    m_DataAreaContainer.Items.Count > 0 ||
                    m_RowsAreaContainer.Items.Count > 0 ||
                    m_ColumnsAreaContainer.Items.Count > 0)
            {
                e.Cancel = true;
                PopUpQuestionDialog dlg = new PopUpQuestionDialog();
                dlg.Caption = Localization.Warning;
                dlg.ContentCtrl.Text = Localization.MdxDesigner_SaveCustomCalculations_Message;
                dlg.ContentCtrl.DialogType = DialogButtons.YesNo;

                dlg.DialogClosed += new EventHandler<DialogResultArgs>(SaveSettings_DialogClosed);
                dlg.Show();
            }
        }

        void SaveSettings_DialogClosed(object sender, DialogResultArgs e)
        {
            if (e.Result == DialogResult.Yes)
            {
                ExportSettings("REFRESH_CUBE_METADATA");
            }
            else
            {
                OnCubeChanged();
            }
        }

        private void OnCubeChanged()
        {
            Clear();
            m_ServerExplorer.RefreshCubeMetadata();
        }

        public bool AutoExecuteQuery
        {
            get
            {
                if (m_RunQueryAutomatic.IsChecked.HasValue)
                    return m_RunQueryAutomatic.IsChecked.Value;
                else
                    return true;
            }
            set { m_RunQueryAutomatic.IsChecked = new bool?(value); }
        }

        void m_EditMDXQuery_Click(object sender, RoutedEventArgs e)
        {
            if (m_EditMDXQuery.IsChecked.HasValue)
            {
                m_MdxQuery.IsReadOnly = !m_EditMDXQuery.IsChecked.Value;

                if (m_EditMDXQuery.IsChecked.Value && m_ShowMDXQuery.IsChecked.HasValue && m_ShowMDXQuery.IsChecked.Value == false)
                {
                    // Эмулируем нажатие на кнопку Показать MDX
                    m_ShowMDXQuery.IsChecked = new bool?(true);
                    m_ShowMDXQuery_Click(sender, e);
                }
            }
        }

        List<CalcMemberInfo> m_CalculatedMembers;
        /// <summary>
        /// Список пользовательских вычисляемых элементов
        /// </summary>
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
                m_ServerExplorer.CubeBrowser.CalculatedMembers = value;
            }
        }

        List<CalculatedNamedSetInfo> m_CalculatedNamedSets;
        /// <summary>
        /// Список пользовательских вычисляемых именованных наборов
        /// </summary>
        List<CalculatedNamedSetInfo> CalculatedNamedSets
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
                m_ServerExplorer.CubeBrowser.CalculatedNamedSets = value;
            }
        }

        List<NamedSet_AreaItemControl> GetUsedNamedSets()
        {
            List<NamedSet_AreaItemControl> list = GetUsedNamedSets(m_FilterAreaContainer);
            List<NamedSet_AreaItemControl> ret = GetUsedNamedSets(m_RowsAreaContainer);
            foreach (NamedSet_AreaItemControl item in ret)
            {
                if (!list.Contains(item)) { list.Add(item); }
            }
            ret = GetUsedNamedSets(m_ColumnsAreaContainer);
            foreach (NamedSet_AreaItemControl item in ret)
            {
                if (!list.Contains(item)) { list.Add(item); }
            }
            ret = GetUsedNamedSets(m_DataAreaContainer);
            foreach (NamedSet_AreaItemControl item in ret)
            {
                if (!list.Contains(item)) { list.Add(item); }
            }

            return list;
        }

        List<NamedSet_AreaItemControl> GetUsedNamedSets(PivotAreaContainer area)
        {
            List<NamedSet_AreaItemControl> list = new List<NamedSet_AreaItemControl>();
            if (area != null)
            {
                foreach (AreaItemControl child in area.Items)
                {
                    NamedSet_AreaItemControl set_ctrl = child as NamedSet_AreaItemControl;
                    if (set_ctrl != null)
                    {

                        if (!list.Contains(set_ctrl))
                        {
                            list.Add(set_ctrl);
                        }
                    }
                }
            }
            return list;
        }

        List<CalculationInfoBase> GetUsedCalculatedMembers()
        {
            List<CalculationInfoBase> list = GetUsedCalculatedMembers(m_FilterAreaContainer);
            List<CalculationInfoBase> ret = GetUsedCalculatedMembers(m_RowsAreaContainer);
            foreach (CalculationInfoBase item in ret)
            {
                if (!list.Contains(item)) { list.Add(item); }
            }
            ret = GetUsedCalculatedMembers(m_ColumnsAreaContainer);
            foreach (CalculationInfoBase item in ret)
            {
                if (!list.Contains(item)) { list.Add(item); }
            }
            ret = GetUsedCalculatedMembers(m_DataAreaContainer);
            foreach (CalculationInfoBase item in ret)
            {
                if (!list.Contains(item)) { list.Add(item); }
            }

            return list;
        }

        List<CalculationInfoBase> GetUsedCalculatedMembers(PivotAreaContainer area)
        {
            List<CalculationInfoBase> list = new List<CalculationInfoBase>();
            if (area != null)
            {
                foreach (AreaItemControl child in area.Items)
                {
                    CalculatedMember_AreaItemControl member_ctrl = child as CalculatedMember_AreaItemControl;
                    if (member_ctrl != null)
                    {
                        CalculationInfoBase item = GetCalculatedMember(member_ctrl.CalculatedMember.Name);
                        if (item != null && !list.Contains(item))
                        {
                            list.Add(item);
                        }
                    }

                    CalculateNamedSet_AreaItemControl set_ctrl = child as CalculateNamedSet_AreaItemControl;
                    if (set_ctrl != null)
                    {
                        CalculationInfoBase item = GetCalculatedNamedSet(set_ctrl.CalculatedNamedSet.Name);
                        if (item != null && !list.Contains(item))
                        {
                            list.Add(item);
                        }
                    }
                }
            }
            return list;
        }

        ModalDialog m_CalculatedMemberEditorModalDialog = null;
        CalculationsEditor m_CalculatedItemsEditor = null;

        void m_CalculatedMemberEditor_Click(object sender, RoutedEventArgs e)
        {
            if (m_CalculatedMemberEditorModalDialog == null)
            {
                m_CalculatedMemberEditorModalDialog = new ModalDialog();
                m_CalculatedMemberEditorModalDialog.BeforeClosed += new EventHandler<DialogResultArgs>(dlg_BeforeClosed);
                m_CalculatedMemberEditorModalDialog.DialogClosed += new EventHandler<DialogResultArgs>(dlg_DialogClosed);
                m_CalculatedMemberEditorModalDialog.DialogOk += new EventHandler<DialogResultArgs>(dlg_DialogOk);
                m_CalculatedMemberEditorModalDialog.Caption = Localization.CalcMemberEditor_DialogCaption;
                m_CalculatedMemberEditorModalDialog.MinHeight = 300;
                m_CalculatedMemberEditorModalDialog.MinWidth = 300;
                m_CalculatedMemberEditorModalDialog.Height = 500;
                m_CalculatedMemberEditorModalDialog.Width = 600;
            }

            if (m_CalculatedItemsEditor == null)
            {
                m_CalculatedItemsEditor = new CalculationsEditor();
                m_CalculatedItemsEditor.EditEnd += (s, args) => { /*m_CalculatedMemberEditorModalDialog.ListenKeys(true);*/ };
                m_CalculatedItemsEditor.EditStart += (s, args) => { /*m_CalculatedMemberEditorModalDialog.ListenKeys(false)*/ };
                m_CalculatedMemberEditorModalDialog.Content = m_CalculatedItemsEditor;
                m_CalculatedItemsEditor.CubeBrowser.OlapDataLoader = GetOlapDataLoader();
            }
            if (m_CalculatedItemsEditor.CubeBrowser.Connection != Connection)
                m_CalculatedItemsEditor.CubeBrowser.Connection = Connection;
            if (m_CalculatedItemsEditor.CubeBrowser.CubeName != CubeName)
                m_CalculatedItemsEditor.CubeBrowser.CubeName = CubeName;
            if (m_CalculatedItemsEditor.CubeBrowser.SubCube != SubCube)
                m_CalculatedItemsEditor.CubeBrowser.SubCube = SubCube;

            Dictionary<String, CalculationInfoBase> members = new Dictionary<string, CalculationInfoBase>();
            foreach (CalcMemberInfo info in CalculatedMembers)
            {
                CalcMemberInfo cloned = info.Clone() as CalcMemberInfo;
                if (cloned != null && !members.ContainsKey(cloned.Name))
                    members.Add(cloned.Name, cloned);
            }

            Dictionary<String, CalculationInfoBase> sets = new Dictionary<string, CalculationInfoBase>();
            foreach (CalculatedNamedSetInfo info in CalculatedNamedSets)
            {
                CalculatedNamedSetInfo cloned = info.Clone() as CalculatedNamedSetInfo;
                if (cloned != null && !sets.ContainsKey(cloned.Name))
                    sets.Add(cloned.Name, cloned);
            }

            m_CalculatedItemsEditor.Initialize(members, sets, m_ServerExplorer.CubeBrowser.CubeInfo, m_ServerExplorer.CubeBrowser.CurrentMeasureGroupName);
            m_CalculatedMemberEditorModalDialog.Show();
        }

        void dlg_DialogOk(object sender, DialogResultArgs e)
        {
            if (m_CalculatedItemsEditor != null)
            {
                m_CalculatedItemsEditor.EndEdit();

                List<CalcMemberInfo> members = new List<CalcMemberInfo>();
                foreach (CalculationInfoBase info in m_CalculatedItemsEditor.Members.Values)
                {
                    CalcMemberInfo memberInfo = info as CalcMemberInfo;
                    if (memberInfo != null)
                    {
                        members.Add(memberInfo);
                    }
                }

                List<CalculatedNamedSetInfo> sets = new List<CalculatedNamedSetInfo>();
                foreach (CalculationInfoBase info in m_CalculatedItemsEditor.Sets.Values)
                {
                    CalculatedNamedSetInfo setInfo = info as CalculatedNamedSetInfo;
                    if (setInfo != null)
                    {
                        sets.Add(setInfo);
                    }
                }

                CalculatedMembers = members;
                CalculatedNamedSets = sets;

                RefreshCalculationItems();
                HighLightCustomNodes();
                RefreshMdxQuery();
            }
        }

        void dlg_BeforeClosed(object sender, DialogResultArgs e)
        {
        }

        void dlg_DialogClosed(object sender, DialogResultArgs e)
        {
        }

        /// <summary>
        /// Обновляет информацию о вычислениях. Удаляет лишние, обновляет информацию у существующих
        /// </summary>
        void RefreshCalculationItems()
        {
            RefreshCalculationItems(m_FilterAreaContainer);
            RefreshCalculationItems(m_RowsAreaContainer);
            RefreshCalculationItems(m_ColumnsAreaContainer);
            RefreshCalculationItems(m_DataAreaContainer);
        }

        /// <summary>
        /// Обновляет информацию о вычислениях. Удаляет лишние, обновляет информацию у существующих
        /// </summary>
        /// <param name="area">Область для обновления</param>
        void RefreshCalculationItems(PivotAreaContainer area)
        {
            if (area != null)
            {
                List<AreaItemControl> toDelete = new List<AreaItemControl>();
                foreach (AreaItemControl child in area.Items)
                {
                    InfoItemControl info_ctrl = child as InfoItemControl;
                    if (info_ctrl != null)
                    {
                        Calculated_AreaItemWrapper calculated = info_ctrl.Wrapper as Calculated_AreaItemWrapper;
                        if (calculated != null)
                        {
                            CalculationInfoBase item = null;
                            if (calculated is CalculatedMember_AreaItemWrapper)
                            {
                                item = GetCalculatedMember(calculated.Name);
                            }
                            if (calculated is CalculatedNamedSet_AreaItemWrapper)
                            {
                                item = GetCalculatedNamedSet(calculated.Name);
                            }

                            if (item == null)
                            {
                                toDelete.Add(child);
                            }
                            else
                            {
                                info_ctrl.Caption = calculated.Caption;
                            }
                        }
                    }
                }

                // Удаляем элементы, которые уже удалены в дизайнере
                foreach (AreaItemControl child in toDelete)
                {
                    area.RemoveItem(child);
                }
            }
        }

        void HighLightCustomNodes()
        {
            HighLightCustomNodes(m_FilterAreaContainer);
            HighLightCustomNodes(m_RowsAreaContainer);
            HighLightCustomNodes(m_ColumnsAreaContainer);
            HighLightCustomNodes(m_DataAreaContainer);
        }

        void HighLightCustomNodes(PivotAreaContainer area)
        {
            if (area != null)
            {
                foreach (AreaItemControl child in area.Items)
                {
                    InfoItemControl info_ctrl = child as InfoItemControl;
                    if (info_ctrl != null)
                    {
                        CustomTreeNode node = FindCustomNode(info_ctrl.Wrapper);
                        if (node != null)
                            node.UseBoldText = true;
                    }
                }
            }
        }
        #endregion Конструктор

        void StorageManager_ActionCompleted(object sender, DataLoaderEventArgs e)
        {
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

            StorageActionArgs args = e.UserState as StorageActionArgs;
            if (args != null)
            {
                if (args.ActionType == StorageActionTypes.Load)
                {
                    ImportMdxLayoutInfo(e.Result.Content);
                }
            }
        }

        public override string URL
        {
            get
            {
                return base.URL;
            }
            set
            {
                base.URL = value;

                m_ServerExplorer.URL = value;
                m_PivotGrid.URL = value;

                OlapDataLoader olapDataLoader = m_ServerExplorer.OlapDataLoader as OlapDataLoader;
                if (olapDataLoader != null)
                {
                    olapDataLoader.URL = value;
                }
                olapDataLoader = m_PivotGrid.OlapDataLoader as OlapDataLoader;
                if (olapDataLoader != null)
                {
                    olapDataLoader.URL = value;
                }
            }
        }

        #region Сохранение/загрузка настроек
        void m_ImportLayout_Click(object sender, RoutedEventArgs e)
        {
            ObjectLoadDialog dlg = new ObjectLoadDialog(StorageManager) { ContentType = StorageContentTypes.MdxDesignerLayout };
            dlg.DialogOk += new EventHandler(LoadDialog_DialogOk);
            dlg.LogManager = LogManager;
            dlg.Show();
            //ImportMdxLayoutInfo(ImportFromStorage());
        }

        void m_ExportLayout_Click(object sender, RoutedEventArgs e)
        {
            ExportSettings();
            //ExportToStorage(ExportMdxLayoutInfo());
        }

        void ExportSettings()
        {
            ExportSettings(null);
        }

        void ExportSettings(object tag)
        {
            ObjectSaveAsDialog dlg = new ObjectSaveAsDialog(StorageManager) { ContentType = StorageContentTypes.MdxDesignerLayout };
            dlg.DialogOk += new EventHandler(SaveAsDialog_DialogOk);
            dlg.DialogClosed += new EventHandler(SaveAsDialog_DialogClosed);
            dlg.Show();
            dlg.Tag = tag;
        }

        void SaveAsDialog_DialogClosed(object sender, EventArgs e)
        {
            ObjectSaveAsDialog dlg = sender as ObjectSaveAsDialog;
            if (dlg == null)
                return;
            if (dlg.Tag != null && dlg.Tag.ToString() == "REFRESH_CUBE_METADATA")
            {
                OnCubeChanged();
            }
        }

        void SaveAsDialog_DialogOk(object sender, EventArgs e)
        {
            ObjectSaveAsDialog dlg = sender as ObjectSaveAsDialog;
            if (dlg == null)
                return;

            String str = ExportMdxLayoutInfo();

            StorageActionArgs args = new StorageActionArgs();
            args.ActionType = StorageActionTypes.Save;
            args.Content = str;
            args.ContentType = StorageContentTypes.MdxDesignerLayout;
            ObjectDescription descr = dlg.Object;
            if (descr != null && !String.IsNullOrEmpty(descr.Name))
            {
                if (String.IsNullOrEmpty(descr.Caption))
                    descr.Caption = descr.Name;
                args.FileDescription = new ObjectStorageFileDescription(descr);
                StorageManager.Invoke(XmlSerializationUtility.Obj2XmlStr(args, Constants.XmlNamespace), args);
            }
            else
            {
                MessageBox.Show(Localization.ObjectSaveDialog_NameIsEmpty_Message, Localization.Warning, MessageBoxButton.OK);
            }
        }

        void LoadDialog_DialogOk(object sender, EventArgs e)
        {
            ObjectLoadDialog dlg = sender as ObjectLoadDialog;
            if (dlg == null)
                return;

            if (dlg.CurrentObject != null && !String.IsNullOrEmpty(dlg.CurrentObject.ContentFileName))
            {
                StorageActionArgs args = new StorageActionArgs();
                args.ActionType = StorageActionTypes.Load;
                args.ContentType = StorageContentTypes.MdxDesignerLayout;
                args.FileDescription = dlg.CurrentObject;
                StorageManager.Invoke(XmlSerializationUtility.Obj2XmlStr(args, Constants.XmlNamespace), args);
            }
        }
        #region Для истории
        //public String ImportFromStorage()
        //{
        //    String res = String.Empty;
        //    IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication();
        //    if (isoStore != null)
        //    {
        //        if (isoStore.FileExists(IsoStorageFile))
        //        {
        //            IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream(IsoStorageFile, System.IO.FileMode.Open, isoStore);
        //            StreamReader reader = new StreamReader(isoStream);
        //            res = reader.ReadToEnd();
        //            reader.Close();
        //        }
        //    }
        //    return res;
        //}

        //public void ExportToStorage(String str)
        //{
        //    if (str != null)
        //    {
        //        IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication();
        //        if (isoStore != null)
        //        {
        //            IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream(IsoStorageFile, System.IO.FileMode.Create, isoStore);
        //            StreamWriter writer = new StreamWriter(isoStream);
        //            writer.Write(str);
        //            writer.Close();
        //        }
        //    }
        //}

        //String m_IsoStorageFile = String.Empty;
        //private String IsoStorageFile
        //{
        //    get
        //    {
        //        if (String.IsNullOrEmpty(m_IsoStorageFile))
        //        {
        //            m_IsoStorageFile = this.Name + "_MdxLayout.xml";
        //            if (!String.IsNullOrEmpty(IsolatedStoragePrefix))
        //            {
        //                String[] items = IsolatedStoragePrefix.Split(new String[] { "." }, StringSplitOptions.RemoveEmptyEntries);
        //                if (items != null && items.Length > 0)
        //                    m_IsoStorageFile = items[items.Length - 1] + "_" + m_IsoStorageFile;
        //            }
        //        }
        //        return m_IsoStorageFile;
        //    }
        //}

        //String m_IsolatedStoragePrefix = String.Empty;
        //public String IsolatedStoragePrefix
        //{
        //    get
        //    {
        //        return m_IsolatedStoragePrefix;
        //    }
        //    set
        //    {
        //        m_IsolatedStoragePrefix = value;
        //        m_IsoStorageFile = String.Empty;
        //    }
        //}
        #endregion Для истории

        #endregion Сохранение/загрузка настроек

        #region Команды от тулбара
        void m_ShowMDXQuery_Click(object sender, RoutedEventArgs e)
        {
            if (m_ShowMDXQuery.IsChecked.HasValue && m_ShowMDXQuery.IsChecked.Value == true)
            {
                m_MDX_Row.Height = new GridLength(m_MDXRowHeight);
                Mdx_Border.Visibility = Visibility.Visible;
                Pivot_Border.Margin = new Thickness(0, 1, 0, 0);
                Output_HorzSplitter.Visibility = Visibility.Visible;
            }
            else
            {
                m_MDXRowHeight = m_MDX_Row.ActualHeight;
                m_MDX_Row.Height = new GridLength(0.0);
                Mdx_Border.Visibility = Visibility.Collapsed;
                Pivot_Border.Margin = new Thickness(0, 0, 0, 0);
                Output_HorzSplitter.Visibility = Visibility.Collapsed;
            }
        }

        void m_ExecuteQuery_Click(object sender, RoutedEventArgs e)
        {
            InitializePivotGrid(m_MdxQuery.Text);
        }

        void m_ShowMetadataArea_Click(object sender, RoutedEventArgs e)
        {
            if (m_ShowMetadataArea.IsChecked.HasValue && m_ShowMetadataArea.IsChecked.Value == true)
            {
                m_Input_Column.Width = new GridLength(m_InputColumnWidth);
                Input_Border.Visibility = Visibility.Visible;
                LayoutRoot_VertSplitter.Visibility = Visibility.Visible;
            }
            else
            {
                m_InputColumnWidth = m_Input_Column.ActualWidth;
                m_Input_Column.Width = new GridLength(0.0);
                Input_Border.Visibility = Visibility.Collapsed;
                LayoutRoot_VertSplitter.Visibility = Visibility.Collapsed;
            }
        }
        #endregion Команды от тулбара

        #region Тягание между областями сводной таблицы
        void AllowDragDrop(PivotAreaContainer area)
        {
            if (area != null)
            {
                area.DragStarted += new EventHandler<DragAreaItemArgs<DragStartedEventArgs>>(area_DragStarted);
                area.DragDelta += new EventHandler<DragAreaItemArgs<DragDeltaEventArgs>>(area_DragDelta);
                area.DragCompleted += new EventHandler<DragAreaItemArgs<DragCompletedEventArgs>>(area_DragCompleted);
            }
        }

        void area_DragCompleted(object sender, DragAreaItemArgs<DragCompletedEventArgs> e)
        {
            try
            {
                PivotAreaContainer container = sender as PivotAreaContainer;

                if (container != null)
                {
                    Point point = new Point(m_DragStart.X + e.Args.HorizontalChange, m_DragStart.Y + e.Args.VerticalChange);
                    // Если перетаскивание за пределы контрола, содержащего все 4 области, то удалить элемент списка (102.94164)
                    if (!AgControlBase.GetSLBounds(Areas_LayoutRoot).Contains(point))
                    {
                        container.RemoveItem(e.Item);
                        return;
                    }

                    if (m_RowsAreaContainer.IsReadyToDrop ||
                            m_ColumnsAreaContainer.IsReadyToDrop ||
                            m_FilterAreaContainer.IsReadyToDrop ||
                            m_DataAreaContainer.IsReadyToDrop)
                    {
                        // Отписываемся от события на удаление. т.к. там мы только меняем стиль текста в узле дерева. А при перетаскивании у нас элемент останется задействованным.
                        container.ItemRemoved -= new EventHandler<AreaItemArgs>(AreaContainer_ItemRemoved);
                        container.RemoveItem(e.Item, false);
                        container.ItemRemoved += new EventHandler<AreaItemArgs>(AreaContainer_ItemRemoved);

                        if (m_RowsAreaContainer.IsReadyToDrop)
                        {
                            m_RowsAreaContainer.AddItem(e.Item);
                        }

                        if (m_ColumnsAreaContainer.IsReadyToDrop)
                        {
                            m_ColumnsAreaContainer.AddItem(e.Item);
                        }

                        if (m_FilterAreaContainer.IsReadyToDrop)
                        {
                            m_FilterAreaContainer.AddItem(e.Item);
                        }

                        if (m_DataAreaContainer.IsReadyToDrop)
                        {
                            m_DataAreaContainer.AddItem(e.Item);
                        }
                    }
                }
            }
            finally
            {
                m_RowsAreaContainer.IsReadyToDrop = false;
                m_ColumnsAreaContainer.IsReadyToDrop = false;
                m_FilterAreaContainer.IsReadyToDrop = false;
                m_DataAreaContainer.IsReadyToDrop = false;
            }
        }

        bool CanDragToArea(PivotAreaContainer container, AreaItemControl item)
        {
            if (item != null && container != null)
            {
                if (container == m_DataAreaContainer)
                {
                    // В область данных можно таскать только меры, KPI и вычисляемые элементы
                    if (item is Kpi_AreaItemControl ||
                            item is Measure_AreaItemControl ||
                            item is CalculatedMember_AreaItemControl)
                        return true;
                }

                if (container == m_RowsAreaContainer || container == m_ColumnsAreaContainer)
                {
                    // В область строк и колонок можно таскать: иерархии, уровни, Set(ы) и узел "Values"
                    if (item is Hierarchy_AreaItemControl ||
                            item is Level_AreaItemControl ||
                            item is CalculateNamedSet_AreaItemControl ||
                            item is Values_AreaItemControl)
                        return true;
                }

                if (container == m_FilterAreaContainer)
                {
                    // В область фильтров можно таскать: иерархии, уровни
                    if (item is Hierarchy_AreaItemControl ||
                            item is Level_AreaItemControl)
                        return true;
                }
            }
            return false;
        }

        void area_DragDelta(object sender, DragAreaItemArgs<DragDeltaEventArgs> e)
        {
            m_RowsAreaContainer.IsReadyToDrop = false;
            m_ColumnsAreaContainer.IsReadyToDrop = false;
            m_FilterAreaContainer.IsReadyToDrop = false;
            m_DataAreaContainer.IsReadyToDrop = false;

            Point m_DragDelta = new Point(m_PrevDrag.X + e.Args.HorizontalChange, m_PrevDrag.Y + e.Args.VerticalChange);

            PivotAreaContainer container = sender as PivotAreaContainer;

            // Подсветка областей, куда разрешено таскание
            if (container != m_RowsAreaContainer)
            {
                Rect m_RowsArea_Bounds = AgControlBase.GetSLBounds(m_RowsAreaContainer);
                if (m_RowsArea_Bounds.Contains(m_DragDelta) && CanDragToArea(m_RowsAreaContainer, e.Item))
                {
                    m_RowsAreaContainer.IsReadyToDrop = true;
                }
            }

            if (container != m_ColumnsAreaContainer)
            {
                Rect m_ColumnsArea_Bounds = AgControlBase.GetSLBounds(m_ColumnsAreaContainer);
                if (m_ColumnsArea_Bounds.Contains(m_DragDelta) && CanDragToArea(m_ColumnsAreaContainer, e.Item))
                {
                    m_ColumnsAreaContainer.IsReadyToDrop = true;
                }
            }

            if (container != m_FilterAreaContainer)
            {
                Rect m_FilterArea_Bounds = AgControlBase.GetSLBounds(m_FilterAreaContainer);
                if (m_FilterArea_Bounds.Contains(m_DragDelta) && CanDragToArea(m_FilterAreaContainer, e.Item))
                {
                    m_FilterAreaContainer.IsReadyToDrop = true;
                }
            }

            if (container != m_DataAreaContainer)
            {
                Rect m_DataArea_Bounds = AgControlBase.GetSLBounds(m_DataAreaContainer);
                if (m_DataArea_Bounds.Contains(m_DragDelta) && CanDragToArea(m_DataAreaContainer, e.Item))
                {
                    m_DataAreaContainer.IsReadyToDrop = true;
                }
            }

            m_PrevDrag = m_DragDelta;
        }

        void area_DragStarted(object sender, DragAreaItemArgs<DragStartedEventArgs> e)
        {
            m_DragStart = new Point(e.Args.HorizontalOffset, e.Args.VerticalOffset);
            m_PrevDrag = m_DragStart;

        }
        #endregion Тягание между областями сводной таблицы

        void AreaContainer_ItemsListChanged(object sender, EventArgs e)
        {
            RefreshMdxQuery();
        }

        void RefreshMdxQuery()
        {
            //String select_Blank = "SELECT {0} FROM {1} {2} CELL PROPERTIES VALUE, FORMAT_STRING, LANGUAGE, BACK_COLOR, FORE_COLOR, FONT_FLAGS";
            String area_Blank = "{0} DIMENSION PROPERTIES PARENT_UNIQUE_NAME, HIERARCHY_UNIQUE_NAME, CUSTOM_ROLLUP, UNARY_OPERATOR, KEY0 ON {1}";
            String where_Blank = "WHERE ({0})";

            // Ось 0 - колонки
            String columns_Set = BuildAreaSet(m_ColumnsAreaContainer);

            // Ось 1 -  строки
            String rows_Set = BuildAreaSet(m_RowsAreaContainer);

            String tmp = String.Empty;
            String where_Set = String.Empty;
            List<String> where_filters = new List<string>();
            List<String> from_filters = new List<string>();

            // Элементы из области данных в условие Where попадают только если в области данных 1 элемент
            // Еисли их больше, то они попадут через описание спец. узла VALUES из области строк или столбцов
            // Кроме того этот единственный элемент попадает на ось 0 если на ней ничего нет (102.93591)
            if (m_DataAreaContainer.Items.Count == 1)
            {
                tmp = BuildItemSet(m_DataAreaContainer.Items[0]);
                if (!String.IsNullOrEmpty(tmp))
                {
                    if (String.IsNullOrEmpty(columns_Set))
                    {
                        columns_Set = "{" + tmp + "}";
                    }
                    else
                    {
                        where_filters.Add(tmp);
                    }
                }
            }


            // Building Select expression
            String select_Set = String.Empty;
            if (!String.IsNullOrEmpty(rows_Set))
            {
                if (String.IsNullOrEmpty(columns_Set))
                {
                    columns_Set = "{}";
                }

                // 2 axes
                select_Set = String.Format(area_Blank, columns_Set, 0);
                select_Set += ", \r\n";
                select_Set += String.Format(area_Blank, rows_Set, 1);
            }
            else
            {
                // 1 axis

                // If columns_Set is Empty then query must be: SELECT  FROM [Adventure Works] 
                if (!String.IsNullOrEmpty(columns_Set))
                    select_Set = String.Format(area_Blank, columns_Set, 0);
            }

            // Если для элемента из области фильтров фильтр явно не задан, то он попадет в выражение WHERE
            // Иначе такие элементы попадают в SubCube (т.е. в выражение FROM) И ВЫРАЖЕНИЕ WHERE (ПФ)
            foreach (AreaItemControl child in m_FilterAreaContainer.Items)
            {
                tmp = BuildItemSet(child);
                if (!String.IsNullOrEmpty(tmp))
                {
                    FilteredItemControl filtered_Item = child as FilteredItemControl;
                    if (filtered_Item != null &&
                            filtered_Item.FilteredWrapper != null &&
                            filtered_Item.FilteredWrapper.CompositeFilter.MembersFilter.IsUsed &&
                            !String.IsNullOrEmpty(filtered_Item.FilteredWrapper.CompositeFilter.MembersFilter.FilterSet))
                    {
                        from_filters.Add(tmp);
                        where_filters.Add(tmp);
                    }
                    else
                    {
                        where_filters.Add(tmp);
                    }
                }
            }

            foreach (string str in where_filters)
            {
                if (!String.IsNullOrEmpty(where_Set))
                {
                    where_Set += ", ";
                }
                where_Set += str;
            }
            if (!String.IsNullOrEmpty(where_Set))
            {
                where_Set = String.Format(where_Blank, where_Set);
            }

            // Элементы, для которых установлен Top - фильтр попадают каждый в отдельный подкуб
            List<String> filter_SubCubes = BuildFilterSubCubes(m_ColumnsAreaContainer);
            List<String> subcubes = BuildFilterSubCubes(m_RowsAreaContainer);
            foreach (String sub in subcubes)
            {
                filter_SubCubes.Add(sub);
            }

            // Подкуб формируемый из фильтров: Top и т.д.
            String inner_from = String.Empty;
            int select_count = 0;
            foreach (String sub in filter_SubCubes)
            {
                if (select_count > 0)
                    inner_from += "\r\n";
                inner_from += "(SELECT {";
                inner_from += sub;
                inner_from += "} on COLUMNS From ";
                select_count++;
            }

            if (String.IsNullOrEmpty(inner_from))
            {
                inner_from = FromSet;
            }
            else
            {
                inner_from += FromSet;
                if (String.IsNullOrEmpty(SubCube))
                    inner_from += Environment.NewLine + where_Set; // 102.94731
                for (int i = 0; i < select_count; i++)
                {
                    inner_from += ")";
                }
            }

            // Формируется подкуб из элементов, размещенных в области фильтров.
            String from_Set = String.Empty;
            if (from_filters.Count > 0)
            {
                from_Set = "(SELECT (";
                int i = 0;
                foreach (String filter_Set in from_filters)
                {
                    if (i > 0)
                        from_Set += ", ";
                    from_Set += filter_Set;
                    i++;
                }
                from_Set += ") on COLUMNS From ";
                from_Set += inner_from;
                from_Set += ")";
            }

            if (String.IsNullOrEmpty(from_Set))
                from_Set = inner_from;

            StringBuilder builder = new StringBuilder();

            int count = 0;
            // Именованные сеты из КУБА
            foreach (NamedSet_AreaItemControl set_ctrl in GetUsedNamedSets())
            {
                if (set_ctrl.NamedSet == null)
                    continue;

                if (!String.IsNullOrEmpty(set_ctrl.NamedSet.Name))
                {
                    NamedSetTreeNode node = m_ServerExplorer.CubeBrowser.FindNamedSet(set_ctrl.NamedSet.Name);
                    if (node != null)
                    {
                        NamedSetInfo setInfo = node.Info as NamedSetInfo;
                        if (setInfo != null)
                        {
                            String script = setInfo.Expression;
                            //if (!String.IsNullOrEmpty(script))
                            {
                                String name = setInfo.Name.Trim();
                                if (!name.StartsWith("["))
                                    name = "[" + name;
                                if (!name.EndsWith("]"))
                                    name = name + "]";

                                if (count == 0)
                                    builder.Append("WITH ");
                                if (count != 0)
                                    builder.Append(" ");
                                builder.AppendLine();

                                builder.AppendFormat("SET {0} AS {1}", name, "{" + script + "}");

                                count++;
                            }
                        }
                    }
                }
            }

            // Вычисляемые элементы и сеты
            // В запрос генерим ВСЕ вычисляемые элементы (т.к. они могут использоваться в сетах) 
            foreach (var info in CalculatedMembers)
            {
                if (!String.IsNullOrEmpty(info.Name))
                {
                    String script = info.GetScript();
                    String name = info.Name.Trim();
                    if (!name.StartsWith("["))
                        name = "[" + name;
                    if (!name.EndsWith("]"))
                        name = name + "]";

                    if (count == 0)
                        builder.Append("WITH ");
                    if (count != 0)
                        builder.Append(" ");
                    builder.AppendLine();

                    builder.AppendFormat("MEMBER {0} AS {1}", name, String.IsNullOrEmpty(script) ? "NULL" : script);
                    count++;
                }
            }
            // В запрос генерим только используемые сеты
            foreach (CalculationInfoBase info in GetUsedCalculatedMembers())
            {
                CalculatedNamedSetInfo setInfo = info as CalculatedNamedSetInfo;
                if (setInfo != null)
                {
                    if (!String.IsNullOrEmpty(info.Name))
                    {
                        String script = setInfo.Expression;
                        String name = info.Name.Trim();
                        if (!name.StartsWith("["))
                            name = "[" + name;
                        if (!name.EndsWith("]"))
                            name = name + "]";

                        if (count == 0)
                            builder.Append("WITH ");
                        if (count != 0)
                            builder.Append(" ");
                        builder.AppendLine();

                        builder.AppendFormat("SET {0} AS {1}", name, "{" + script + "}");

                        count++;
                    }
                }
            }

            if (!String.IsNullOrEmpty(builder.ToString()))
                builder.AppendLine(" ");

            builder.AppendLine("Select ");
            if (!String.IsNullOrEmpty(select_Set))
                builder.AppendLine(select_Set + " ");
            builder.AppendLine("FROM ");
            builder.AppendLine(from_Set + " ");
            if (!String.IsNullOrEmpty(where_Set))
            {
                builder.AppendLine(where_Set + " ");
            }
            builder.AppendLine("CELL PROPERTIES BACK_COLOR, CELL_ORDINAL, FORE_COLOR, FONT_NAME, FONT_SIZE, FONT_FLAGS, FORMAT_STRING, VALUE, FORMATTED_VALUE, UPDATEABLE");
            String mdxQuery = builder.ToString();
            //String mdxQuery = String.Format(select_Blank, select_Set, OlapHelper.ConvertToQueryStyle(CubeName), where_Set);
            m_MdxQuery.Text = mdxQuery;

            // Проверяем нужно ли выполнять запрос автоматически
            if (AutoExecuteQuery)
            {
                InitializePivotGrid(mdxQuery);
            }
        }

        private const String LEVEL_MEMBERS_SET_BLANK = "{0}.Members";
        private const String HIERARCHY_MEMBERS_SET_BLANK = "{0}.Levels(0).Members";

        List<String> BuildFilterSubCubes(PivotAreaContainer area)
        {
            List<String> result = new List<string>();

            if (area != null)
            {
                foreach (AreaItemControl child in area.Items)
                {
                    FilteredItemControl filtered_Item = child as FilteredItemControl;

                    if (filtered_Item != null)
                    {
                        if (filtered_Item.FilteredWrapper.CompositeFilter.Filter.IsUsed)
                        {
                            String set = BuildItemSet(child);

                            #region Top-Filter
                            if (filtered_Item.FilteredWrapper.CompositeFilter.Filter.CurrentFilter == FilterFamilyTypes.TopFilter)
                            {
                                String function = String.Empty;
                                switch (filtered_Item.FilteredWrapper.CompositeFilter.Filter.TopFilter.FilterType)
                                {
                                    case TopFilterTypes.Top:
                                        switch (filtered_Item.FilteredWrapper.CompositeFilter.Filter.TopFilter.FilterTarget)
                                        {
                                            case TopFilterTargetTypes.Items:
                                                function = "TopCount";
                                                break;
                                            case TopFilterTargetTypes.Percent:
                                                function = "TopPercent";
                                                break;
                                            case TopFilterTargetTypes.Sum:
                                                function = "TopSum";
                                                break;
                                        }
                                        break;
                                    case TopFilterTypes.Bottom:
                                        switch (filtered_Item.FilteredWrapper.CompositeFilter.Filter.TopFilter.FilterTarget)
                                        {
                                            case TopFilterTargetTypes.Items:
                                                function = "BottomCount";
                                                break;
                                            case TopFilterTargetTypes.Percent:
                                                function = "BottomPercent";
                                                break;
                                            case TopFilterTargetTypes.Sum:
                                                function = "BottomSum";
                                                break;
                                        }
                                        break;
                                }

                                if (!String.IsNullOrEmpty(function) && !String.IsNullOrEmpty(set))
                                {
                                    String Top_Template = " " + function + "({0}, {1}, {2})";
                                    set = String.Format(Top_Template, set, filtered_Item.FilteredWrapper.CompositeFilter.Filter.TopFilter.Count.ToString(), filtered_Item.FilteredWrapper.CompositeFilter.Filter.TopFilter.MeasureUniqueName);
                                    result.Add(set);
                                }
                            }
                            #endregion Top-Filter

                            #region Value-Filter
                            if (filtered_Item.FilteredWrapper.CompositeFilter.Filter.CurrentFilter == FilterFamilyTypes.ValueFilter)
                            {
                                String function = String.Empty;
                                switch (filtered_Item.FilteredWrapper.CompositeFilter.Filter.ValueFilter.FilterType)
                                {
                                    case ValueFilterTypes.Equal:
                                        function = "=";
                                        break;
                                    case ValueFilterTypes.NotEqual:
                                        function = "<>";
                                        break;
                                    case ValueFilterTypes.Less:
                                        function = "<";
                                        break;
                                    case ValueFilterTypes.LessOrEqual:
                                        function = "<=";
                                        break;
                                    case ValueFilterTypes.Greater:
                                        function = ">";
                                        break;
                                    case ValueFilterTypes.GreaterOrEqual:
                                        function = ">=";
                                        break;
                                }

                                if (!String.IsNullOrEmpty(set))
                                {
                                    if (!String.IsNullOrEmpty(function))
                                    {
                                        String Top_Template = " Filter(Hierarchize({0}), ({1} " + function + " {2}))";
                                        set = String.Format(Top_Template, set, filtered_Item.FilteredWrapper.CompositeFilter.Filter.ValueFilter.MeasureUniqueName, filtered_Item.FilteredWrapper.CompositeFilter.Filter.ValueFilter.Num1.ToString());
                                        result.Add(set);
                                    }
                                    else
                                    {
                                        if (filtered_Item.FilteredWrapper.CompositeFilter.Filter.ValueFilter.FilterType == ValueFilterTypes.Between ||
                                                filtered_Item.FilteredWrapper.CompositeFilter.Filter.ValueFilter.FilterType == ValueFilterTypes.NotBetween)
                                        {
                                            String Top_Template = " Filter(Hierarchize({0}), ({1} >= {2} AND {1} <= {3}))";
                                            if (filtered_Item.FilteredWrapper.CompositeFilter.Filter.ValueFilter.FilterType == ValueFilterTypes.NotBetween)
                                                Top_Template = " Filter(Hierarchize({0}), ({1} < {2} OR {1} > {3}))";
                                            set = String.Format(Top_Template, set, filtered_Item.FilteredWrapper.CompositeFilter.Filter.ValueFilter.MeasureUniqueName, filtered_Item.FilteredWrapper.CompositeFilter.Filter.ValueFilter.Num1.ToString(), filtered_Item.FilteredWrapper.CompositeFilter.Filter.ValueFilter.Num2.ToString());
                                            result.Add(set);
                                        }
                                    }
                                }
                            }
                            #endregion Value-Filter

                            #region Label-Filter
                            if (filtered_Item.FilteredWrapper.CompositeFilter.Filter.CurrentFilter == FilterFamilyTypes.LabelFilter)
                            {
                                Hierarchy_AreaItemControl hierarchy_Item = filtered_Item as Hierarchy_AreaItemControl;
                                Level_AreaItemControl level_Item = filtered_Item as Level_AreaItemControl;
                                String hierarchy_UniqueName = String.Empty;

                                if (hierarchy_Item != null)
                                {
                                    hierarchy_UniqueName = hierarchy_Item.Hierarchy.UniqueName;
                                }
                                if (level_Item != null)
                                {
                                    hierarchy_UniqueName = level_Item.Level.HierarchyUniqueName;
                                }

                                String Filter_Template = String.Empty;
                                String BeginEnd_Template = String.Empty;
                                String Between_Template = String.Empty;
                                switch (filtered_Item.FilteredWrapper.CompositeFilter.Filter.LabelFilter.FilterType)
                                {
                                    case LabelFilterTypes.Equal:
                                        Filter_Template = "Filter({0}, ({1}.CurrentMember.Properties(\"{2}\") = \"{3}\"))";
                                        ; break;
                                    case LabelFilterTypes.NotEqual:
                                        Filter_Template = "Filter({0}, ({1}.CurrentMember.Properties(\"{2}\") <> \"{3}\"))";
                                        break;
                                    case LabelFilterTypes.Less:
                                        Filter_Template = "Filter({0}, ({1}.CurrentMember.Properties(\"{2}\") < \"{3}\"))";
                                        break;
                                    case LabelFilterTypes.LessOrEqual:
                                        Filter_Template = "Filter({0}, ({1}.CurrentMember.Properties(\"{2}\") <= \"{3}\"))";
                                        break;
                                    case LabelFilterTypes.Greater:
                                        Filter_Template = "Filter({0}, ({1}.CurrentMember.Properties(\"{2}\") > \"{3}\"))";
                                        break;
                                    case LabelFilterTypes.GreaterOrEqual:
                                        Filter_Template = "Filter({0}, ({1}.CurrentMember.Properties(\"{2}\") >= \"{3}\"))";
                                        break;
                                    case LabelFilterTypes.BeginWith:
                                        BeginEnd_Template = "Filter({0}, (Left({1}.CurrentMember.Properties(\"{2}\"), {3}) = \"{4}\"))";
                                        break;
                                    case LabelFilterTypes.EndWith:
                                        BeginEnd_Template = "Filter({0}, (Right({1}.CurrentMember.Properties(\"{2}\"), {3}) = \"{4}\"))";
                                        break;
                                    case LabelFilterTypes.NotEndWith:
                                        BeginEnd_Template = "Filter({0}, (Right({1}.CurrentMember.Properties(\"{2}\"), {3}) <> \"{4}\"))";
                                        break;
                                    case LabelFilterTypes.Contain:
                                        Filter_Template = "Filter({0}, (InStr(1, {1}.CurrentMember.Properties(\"{2}\"), \"{3}\") > 0))";
                                        break;
                                    case LabelFilterTypes.NotContain:
                                        Filter_Template = "Filter({0}, (InStr(1, {1}.CurrentMember.Properties(\"{2}\"), \"{3}\") == 0))";
                                        break;
                                    case LabelFilterTypes.Between:
                                        Between_Template = "Filter({0}, ({1}.CurrentMember.Properties(\"{2}\") >= \"{3}\") AND ({1}.CurrentMember.Properties(\"{2}\") <= \"{4}\"))";
                                        break;
                                    case LabelFilterTypes.NotBetween:
                                        Between_Template = "Filter({0}, ({1}.CurrentMember.Properties(\"{2}\") < \"{3}\") OR ({1}.CurrentMember.Properties(\"{2}\") > \"{4}\"))";
                                        break;
                                }

                                if (!String.IsNullOrEmpty(set))
                                {
                                    if (!String.IsNullOrEmpty(Filter_Template))
                                    {
                                        set = String.Format(Filter_Template, set, hierarchy_UniqueName, filtered_Item.FilteredWrapper.CompositeFilter.Filter.LabelFilter.LevelPropertyName, filtered_Item.FilteredWrapper.CompositeFilter.Filter.LabelFilter.Text1);
                                        result.Add(set);
                                    }
                                    if (!String.IsNullOrEmpty(BeginEnd_Template))
                                    {
                                        set = String.Format(BeginEnd_Template, set, hierarchy_UniqueName, filtered_Item.FilteredWrapper.CompositeFilter.Filter.LabelFilter.LevelPropertyName, filtered_Item.FilteredWrapper.CompositeFilter.Filter.LabelFilter.Text1.Length.ToString(), filtered_Item.FilteredWrapper.CompositeFilter.Filter.LabelFilter.Text1);
                                        result.Add(set);
                                    }
                                    if (!String.IsNullOrEmpty(Between_Template))
                                    {
                                        set = String.Format(Between_Template, set, hierarchy_UniqueName, filtered_Item.FilteredWrapper.CompositeFilter.Filter.LabelFilter.LevelPropertyName, filtered_Item.FilteredWrapper.CompositeFilter.Filter.LabelFilter.Text1, filtered_Item.FilteredWrapper.CompositeFilter.Filter.LabelFilter.Text2);
                                        result.Add(set);
                                    }
                                }
                            }
                            #endregion Label-Filter
                        }
                    }
                }
            }
            return result;
        }

        String BuildAreaSet(PivotAreaContainer area)
        {
            String tmp = string.Empty;
            String result = String.Empty;

            String hierarchize_Blank = "Hierarchize({0})";

            if (area != null)
            {
                // Список из распаршенных элементов области
                List<String> list = new List<string>();
                foreach (AreaItemControl child in area.Items)
                {
                    tmp = BuildItemSet(child);
                    if (!String.IsNullOrEmpty(tmp))
                        list.Add(tmp);
                }

                if (list.Count > 0)
                {
                    if (list.Count == 1)
                    {
                        // Один элемент
                        result = String.Format(hierarchize_Blank, list[0]);
                    }
                    else
                    {
                        int i = 0;
                        int crossCount = 0;
                        // Пересечение элементов
                        foreach (String set in list)
                        {
                            if (i > 0)
                                result += ", ";
                            if (i < list.Count - 1)
                            {
                                result += "CrossJoin(";
                                crossCount++;
                            }
                            result += set;
                            i++;
                        }

                        for (int x = 0; x < crossCount; x++)
                        {
                            result += ")";
                        }
                        result = String.Format(hierarchize_Blank, result);
                    }
                }
            }
            return result;
        }

        CalculationInfoBase GetCalculatedMember(String name)
        {
            foreach (CalculationInfoBase info in CalculatedMembers)
            {
                if (info.Name == name)
                    return info;
            }
            return null;
        }

        CalculationInfoBase GetCalculatedNamedSet(String name)
        {
            foreach (CalculationInfoBase info in CalculatedNamedSets)
            {
                if (info.Name == name)
                    return info;
            }
            return null;
        }

        String BuildItemSet(AreaItemControl item)
        {
            String result = String.Empty;

            if (item != null)
            {
                InfoItemControl info_Item = item as InfoItemControl;
                if (info_Item != null)
                {
                    FilteredItemControl filtered_Item = item as FilteredItemControl;
                    if (filtered_Item != null && filtered_Item.FilteredWrapper != null)
                    {
                        if (filtered_Item.FilteredWrapper != null && filtered_Item.FilteredWrapper.CompositeFilter.MembersFilter.IsUsed && !String.IsNullOrEmpty(filtered_Item.FilteredWrapper.CompositeFilter.MembersFilter.FilterSet))
                            result = filtered_Item.FilteredWrapper.CompositeFilter.MembersFilter.FilterSet;
                        else
                        {
                            Hierarchy_AreaItemControl hierarchy_Item = item as Hierarchy_AreaItemControl;
                            if (hierarchy_Item != null)
                            {
                                return String.Format(HIERARCHY_MEMBERS_SET_BLANK, hierarchy_Item.Hierarchy.UniqueName);
                            }

                            Level_AreaItemControl level_Item = item as Level_AreaItemControl;
                            if (level_Item != null)
                            {
                                return String.Format(LEVEL_MEMBERS_SET_BLANK, level_Item.Level.UniqueName);
                            }
                        }
                    }

                    CalculatedMember_AreaItemControl calcMember_Item = item as CalculatedMember_AreaItemControl;
                    if (calcMember_Item != null && calcMember_Item.CalculatedMember != null)
                    {
                        CalculationInfoBase info = GetCalculatedMember(calcMember_Item.CalculatedMember.Name);
                        if (info != null)
                        {
                            return info.Name;
                        }
                    }

                    CalculateNamedSet_AreaItemControl calculatedSet_Item = item as CalculateNamedSet_AreaItemControl;
                    if (calculatedSet_Item != null)
                    {
                        CalculationInfoBase info = GetCalculatedNamedSet(calculatedSet_Item.CalculatedNamedSet.Name);
                        if (info != null)
                        {
                            return info.Name;
                        }
                    }

                    NamedSet_AreaItemControl set_Item = item as NamedSet_AreaItemControl;
                    if (set_Item != null)
                    {
                        return OlapHelper.ConvertToQueryStyle(set_Item.NamedSet.Name);
                    }

                    Measure_AreaItemControl measure_Item = item as Measure_AreaItemControl;
                    if (measure_Item != null)
                    {
                        return measure_Item.Measure.UniqueName;
                    }

                    Kpi_AreaItemControl kpi_Item = item as Kpi_AreaItemControl;
                    if (kpi_Item != null)
                    {
                        if (kpi_Item.Type == KpiControlType.Goal)
                        {
                            return kpi_Item.Kpi.Custom_KpiGoal;
                        }

                        if (kpi_Item.Type == KpiControlType.Value)
                        {
                            return kpi_Item.Kpi.Custom_KpiValue;
                        }

                        if (kpi_Item.Type == KpiControlType.Status)
                        {
                            return kpi_Item.Kpi.Custom_KpiStatus;
                        }

                        if (kpi_Item.Type == KpiControlType.Trend)
                        {
                            return kpi_Item.Kpi.Custom_KpiTrend;
                        }
                    }
                }

                // Если специальный элемент VALUES
                if (item is Values_AreaItemControl)
                {
                    result = String.Empty;
                    foreach (AreaItemControl child in m_DataAreaContainer.Items)
                    {
                        String toAdd = String.Empty;
                        Measure_AreaItemControl measure_Item = child as Measure_AreaItemControl;
                        if (measure_Item != null)
                        {
                            toAdd = measure_Item.Measure.UniqueName;
                        }

                        CalculatedMember_AreaItemControl calcMember_Item = child as CalculatedMember_AreaItemControl;
                        if (calcMember_Item != null && calcMember_Item.CalculatedMember != null)
                        {
                            CalculationInfoBase info = GetCalculatedMember(calcMember_Item.CalculatedMember.Name);
                            if (info != null)
                            {
                                toAdd = info.Name;
                            }
                        }

                        Kpi_AreaItemControl kpi_Item = item as Kpi_AreaItemControl;
                        if (kpi_Item != null)
                        {
                            if (kpi_Item.Type == KpiControlType.Goal)
                            {
                                toAdd = kpi_Item.Kpi.Custom_KpiGoal;
                            }

                            if (kpi_Item.Type == KpiControlType.Value)
                            {
                                toAdd = kpi_Item.Kpi.Custom_KpiValue;
                            }

                            if (kpi_Item.Type == KpiControlType.Status)
                            {
                                toAdd = kpi_Item.Kpi.Custom_KpiStatus;
                            }

                            if (kpi_Item.Type == KpiControlType.Trend)
                            {
                                toAdd = kpi_Item.Kpi.Custom_KpiTrend;
                            }
                        }

                        if (!String.IsNullOrEmpty(toAdd))
                        {
                            if (!String.IsNullOrEmpty(result))
                                result += ", ";
                            result += toAdd;
                        }
                    }
                }

                if (!String.IsNullOrEmpty(result))
                {
                    result = "{" + result + "}";
                }
            }

            return result;
        }

        void AreaContainer_ItemRemoved(object sender, AreaItemArgs e)
        {
            InfoItemControl info_ctrl = e.ItemControl as InfoItemControl;
            if (info_ctrl != null)
            {
                CustomTreeNode node = FindCustomNode(info_ctrl.Wrapper);
                if (node != null)
                    node.UseBoldText = false;
            }

            if (sender == m_DataAreaContainer)
            {
                // Спец. элемент удаляем с отпиской/подпиской удаления. Чтобы не удалились элементы из области данных
                if (m_DataAreaContainer.Items.Count < 2)
                {
                    m_RowsAreaContainer.ItemRemoved -= new EventHandler<AreaItemArgs>(AreaContainer_ItemRemoved);
                    m_ColumnsAreaContainer.ItemRemoved -= new EventHandler<AreaItemArgs>(AreaContainer_ItemRemoved);
                    m_RowsAreaContainer.RemoveItem(FindValuesItem(m_RowsAreaContainer), false);
                    m_ColumnsAreaContainer.RemoveItem(FindValuesItem(m_ColumnsAreaContainer), false);
                    m_RowsAreaContainer.ItemRemoved += new EventHandler<AreaItemArgs>(AreaContainer_ItemRemoved);
                    m_ColumnsAreaContainer.ItemRemoved += new EventHandler<AreaItemArgs>(AreaContainer_ItemRemoved);
                }
            }

            // Если удален специальный элемент VALUES, то нужно очистить область данных
            if (e.ItemControl is Values_AreaItemControl)
            {
                m_DataAreaContainer.Clear();
            }
        }

        #region Drag and Drop из метаданных куба
        /// <summary>
        /// Позиция старта таскания
        /// </summary>
        Point m_DragStart = new Point(0, 0);
        /// <summary>
        /// Предыдущая позиция при перетаскивании
        /// </summary>
        Point m_PrevDrag = new Point(0, 0);

        void m_CubeBrowser_DragStarted(object sender, DragNodeArgs<DragStartedEventArgs> e)
        {
            m_DragStart = new Point(e.Args.HorizontalOffset, e.Args.VerticalOffset);
            m_PrevDrag = m_DragStart;
        }

        HierarchyTreeNode FindHierarchy(TreeViewItem node)
        {
            if (node != null)
            {
                HierarchyTreeNode hierarchyNode = node as HierarchyTreeNode;
                if (hierarchyNode != null)
                {
                    return hierarchyNode;
                }

                foreach (TreeViewItem child in node.Items)
                {
                    hierarchyNode = FindHierarchy(child);
                    if (hierarchyNode != null)
                        return hierarchyNode;
                }
            }
            return null;
        }

        void DropToArea(PivotAreaContainer container, CustomTreeNode node)
        {
            DropToArea(container, node, true);
        }

        void DropToArea(PivotAreaContainer container, CustomTreeNode node, bool raise_ItemsListChanged)
        {
            if (container != null && node != null)
            {
                Members_FilterWrapper custom_MemberFilter = null;

                // Убиваем конкурентов :D

                #region Узелы для элементов измерений
                // Если таскается элемент, то в случае если в данной области есть узел для иерархии или уровня, то элемент добавляется в фильтр
                var memberNode = node as MemberLiteTreeNode;
                if (memberNode != null && memberNode.Info != null)
                {
                    // Убиваем конкурентов во всех областях кроме данной :D
                    if (container == m_RowsAreaContainer)
                    {
                        m_ColumnsAreaContainer.RemoveItem(FindConcurent(m_ColumnsAreaContainer, node), false);
                        m_FilterAreaContainer.RemoveItem(FindConcurent(m_FilterAreaContainer, node), false);
                        m_DataAreaContainer.RemoveItem(FindConcurent(m_DataAreaContainer, node), false);
                    }
                    if (container == m_ColumnsAreaContainer)
                    {
                        m_RowsAreaContainer.RemoveItem(FindConcurent(m_RowsAreaContainer, node), false);
                        m_FilterAreaContainer.RemoveItem(FindConcurent(m_FilterAreaContainer, node), false);
                        m_DataAreaContainer.RemoveItem(FindConcurent(m_DataAreaContainer, node), false);
                    }
                    if (container == m_FilterAreaContainer)
                    {
                        m_ColumnsAreaContainer.RemoveItem(FindConcurent(m_ColumnsAreaContainer, node), false);
                        m_RowsAreaContainer.RemoveItem(FindConcurent(m_RowsAreaContainer, node), false);
                        m_DataAreaContainer.RemoveItem(FindConcurent(m_DataAreaContainer, node), false);
                    }
                    if (container == m_DataAreaContainer)
                    {
                        m_ColumnsAreaContainer.RemoveItem(FindConcurent(m_ColumnsAreaContainer, node), false);
                        m_RowsAreaContainer.RemoveItem(FindConcurent(m_RowsAreaContainer, node), false);
                        m_FilterAreaContainer.RemoveItem(FindConcurent(m_FilterAreaContainer, node), false);
                    }

                    // Находим конкурента в данной области
                    var concurent = FindConcurent(container, node);
                    var filtered_concurent = concurent as FilteredItemControl;
                    if (filtered_concurent != null && filtered_concurent.FilteredWrapper != null &&
                            (filtered_concurent is Hierarchy_AreaItemControl || // Если конкурентом является иерархиия то это нормально
                            (filtered_concurent is Level_AreaItemControl && ((Level_AreaItemControl)filtered_concurent).Level != null && ((Level_AreaItemControl)filtered_concurent).Level.UniqueName == memberNode.Info.LevelName)) // Если конкурентом является уровень, то это должен быть тот же уровень что и у элемента
                    )
                    {
                        bool isDublicate = false;
                        // Ищем такой же элемент в фильтре
                        foreach (var item in filtered_concurent.FilteredWrapper.CompositeFilter.MembersFilter.SelectedInfo)
                        {
                            if (item.UniqueName == memberNode.Info.UniqueName && item.SelectState == SelectStates.Selected_Self)
                                isDublicate = true;
                        }
                        if (!isDublicate)
                        {
                            // Добавляем сами руками в FilterSet. Он превильно сгенерируется только при закрытии диалога с фильтром
                            if (String.IsNullOrEmpty(filtered_concurent.FilteredWrapper.CompositeFilter.MembersFilter.FilterSet))
                            {
                                filtered_concurent.FilteredWrapper.CompositeFilter.MembersFilter.FilterSet = "{" + memberNode.Info.UniqueName + "}";
                            }
                            else
                            {
                                String str = filtered_concurent.FilteredWrapper.CompositeFilter.MembersFilter.FilterSet.Trim();
                                if (str.EndsWith("}"))
                                {
                                    str = str.Substring(0, str.Length - 1);
                                }
                                str += ", " + memberNode.Info.UniqueName + "}";
                                filtered_concurent.FilteredWrapper.CompositeFilter.MembersFilter.FilterSet = str;
                            }

                            var member_settings = new MemberChoiceSettings(memberNode.Info, SelectStates.Selected_Self);
                            filtered_concurent.FilteredWrapper.CompositeFilter.MembersFilter.SelectedInfo.Add(member_settings);
                        }
                        filtered_concurent.FilteredWrapper.CompositeFilter.MembersFilter.IsUsed = true;
                        filtered_concurent.Refresh();
                        if (m_FilterDialogs.ContainsKey(concurent))
                        {
                            ModalDialog dialog = m_FilterDialogs[concurent];
                            if (dialog != null)
                            {
                                var filterControl = dialog.Content as FilterBuilderControl;
                                if (filterControl != null)
                                {
                                    // Переинициализировать контрол выбора элементов измерения в фильтре при открытии
                                    filterControl.MemberChoiceIsInitialized = false;
                                }
                            }
                        }

                        RefreshMdxQuery();
                        return;
                    }
                    else
                    {
                        // Удаляем данного конкурента, т.к. он не поддерживает фильтр
                        container.RemoveItem(concurent, false);
                    }

                    // Добавляем новый узел для иерархии
                    // Ищем иерархию для данного элемента
                    var hierarchyNode = m_ServerExplorer.CubeBrowser.FindHierarchyNode(memberNode.Info.HierarchyUniqueName);
                    if (hierarchyNode != null)
                    {
                        custom_MemberFilter = new Members_FilterWrapper();
                        var member_settings = new MemberChoiceSettings(memberNode.Info, SelectStates.Selected_Self);
                        custom_MemberFilter.SelectedInfo.Add(member_settings);
                        custom_MemberFilter.FilterSet = "{" + memberNode.Info.UniqueName + "}";
                        node = hierarchyNode;
                    }
                }
                else
                {
                    AreaItemControl concurent = FindConcurent(m_RowsAreaContainer, node);
                    m_RowsAreaContainer.RemoveItem(concurent, false);

                    concurent = FindConcurent(m_ColumnsAreaContainer, node);
                    m_ColumnsAreaContainer.RemoveItem(concurent, false);

                    concurent = FindConcurent(m_FilterAreaContainer, node);
                    m_FilterAreaContainer.RemoveItem(concurent, false);

                    concurent = FindConcurent(m_DataAreaContainer, node);
                    m_DataAreaContainer.RemoveItem(concurent, false);
                }
                #endregion Узелы для элементов измерений


                #region Узлы для вычисляемых элементов
                CalculatedMemberTreeNode calcMemberNode = node as CalculatedMemberTreeNode;
                if (calcMemberNode != null && calcMemberNode.Info != null)
                {
                    // Вычисляемые элементы могут кидаться только в область данных
                    AreaItemControl ctrl = new CalculatedMember_AreaItemControl(new CalculatedMember_AreaItemWrapper(calcMemberNode.Info), calcMemberNode.Icon);
                    ctrl.ContextMenuCreated += new EventHandler(ctrl_ContextMenuCreated);
                    ctrl.UserData = node;


                    if (container == m_DataAreaContainer)
                    {
                        int count = m_DataAreaContainer.Items.Count;

                        // В случае, если в области данных стало более одного объекта, то добавляем специальный узел Values в область колонок 
                        if (count == 1)
                        {
                            AreaItemControl value_ctrl = new Values_AreaItemControl();
                            value_ctrl.ContextMenuCreated += new EventHandler(ctrl_ContextMenuCreated);
                            m_ColumnsAreaContainer.AddItem(value_ctrl, false);
                        }

                        m_DataAreaContainer.AddItem(ctrl, raise_ItemsListChanged);
                        calcMemberNode.UseBoldText = true;
                    }
                }
                #endregion Узлы для вычисляемых элементов

                #region Узлы для именованных наборов
                CalculatedNamedSetTreeNode calculatedSetNode = node as CalculatedNamedSetTreeNode;
                if (calculatedSetNode != null && calculatedSetNode.Info != null)
                {
                    // Set(ы) могут кидаться только в области строк и столбцов
                    AreaItemControl ctrl = new CalculateNamedSet_AreaItemControl(new CalculatedNamedSet_AreaItemWrapper(calculatedSetNode.Info), calculatedSetNode.Icon);
                    ctrl.ContextMenuCreated += new EventHandler(ctrl_ContextMenuCreated);
                    ctrl.UserData = node;

                    if (container == m_RowsAreaContainer)
                    {
                        m_RowsAreaContainer.AddItem(ctrl, raise_ItemsListChanged);
                        calculatedSetNode.UseBoldText = true;
                    }

                    if (container == m_ColumnsAreaContainer)
                    {
                        m_ColumnsAreaContainer.AddItem(ctrl, raise_ItemsListChanged);
                        calculatedSetNode.UseBoldText = true;
                    }
                }

                NamedSetTreeNode setNode = node as NamedSetTreeNode;
                if (setNode != null)
                {
                    NamedSetInfo setInfo = setNode.Info as NamedSetInfo;
                    if (setInfo != null)
                    {
                        // Set(ы) могут кидаться только в области строк и столбцов
                        AreaItemControl ctrl = new NamedSet_AreaItemControl(new NamedSet_AreaItemWrapper(setInfo), setNode.Icon);
                        ctrl.ContextMenuCreated += new EventHandler(ctrl_ContextMenuCreated);
                        ctrl.UserData = node;

                        if (container == m_RowsAreaContainer)
                        {
                            m_RowsAreaContainer.AddItem(ctrl, raise_ItemsListChanged);
                            setNode.UseBoldText = true;
                        }

                        if (container == m_ColumnsAreaContainer)
                        {
                            m_ColumnsAreaContainer.AddItem(ctrl, raise_ItemsListChanged);
                            setNode.UseBoldText = true;
                        }
                    }
                }
                #endregion Узлы для именованных наборов

                #region Узлы метаданных (InfoBaseTreeNode)
                InfoBaseTreeNode info_node = node as InfoBaseTreeNode;
                if (info_node != null)
                {
                    HierarchyInfo hierarchyInfo = info_node.Info as HierarchyInfo;
                    LevelInfo levelInfo = info_node.Info as LevelInfo;
                    MeasureInfo measureInfo = info_node.Info as MeasureInfo;
                    KpiInfo kpiInfo = info_node.Info as KpiInfo;

                    // Иерархии и уровни можно кидать только в области: строк, столбцов, фильтров
                    if (hierarchyInfo != null || levelInfo != null)
                    {
                        FilteredItemControl ctrl = null;
                        if (hierarchyInfo != null)
                            ctrl = new Hierarchy_AreaItemControl(new Hierarchy_AreaItemWrapper(hierarchyInfo), info_node.Icon);
                        if (levelInfo != null)
                            ctrl = new Level_AreaItemControl(new Level_AreaItemWrapper(levelInfo), info_node.Icon);

                        ctrl.ShowFilter += new EventHandler(FilteredItem_ShowFilter);
                        ctrl.CancelFilter += new EventHandler(FilteredItem_CancelFilter);
                        ctrl.ContextMenuCreated += new EventHandler(ctrl_ContextMenuCreated);
                        ctrl.UserData = node;
                        if (custom_MemberFilter != null)
                        {
                            ctrl.FilteredWrapper.CompositeFilter.MembersFilter = custom_MemberFilter;
                            ctrl.FilteredWrapper.CompositeFilter.MembersFilter.IsUsed = true;
                            ctrl.Refresh();
                        }

                        if (container == m_RowsAreaContainer)
                        {
                            m_RowsAreaContainer.AddItem(ctrl, raise_ItemsListChanged);
                            info_node.UseBoldText = true;
                        }

                        if (container == m_ColumnsAreaContainer)
                        {
                            m_ColumnsAreaContainer.AddItem(ctrl, raise_ItemsListChanged);
                            info_node.UseBoldText = true;
                        }

                        if (container == m_FilterAreaContainer)
                        {
                            m_FilterAreaContainer.AddItem(ctrl, raise_ItemsListChanged);
                            info_node.UseBoldText = true;
                        }
                    }

                    // меры и Kpi могут кидаться только в область данных
                    if (measureInfo != null ||
                            kpiInfo != null)
                    {
                        AreaItemControl ctrl = null;
                        if (measureInfo != null)
                            ctrl = new Measure_AreaItemControl(new Measure_AreaItemWrapper(measureInfo), info_node.Icon);
                        if (kpiInfo != null)
                        {
                            KpiControlType type = KpiControlType.Value;
                            if (node is KpiStatusTreeNode)
                                type = KpiControlType.Status;
                            if (node is KpiTrendTreeNode)
                                type = KpiControlType.Trend;
                            if (node is KpiGoalTreeNode)
                                type = KpiControlType.Goal;

                            ctrl = new Kpi_AreaItemControl(new Kpi_AreaItemWrapper(kpiInfo, type), info_node.Icon);
                        }

                        if (ctrl != null)
                        {
                            ctrl.ContextMenuCreated += new EventHandler(ctrl_ContextMenuCreated);
                            ctrl.UserData = node;

                            if (container == m_DataAreaContainer)
                            {
                                int count = m_DataAreaContainer.Items.Count;

                                // В случае, если в области данных стало более одного объекта, то добавляем специальный узел Values в область колонок 
                                if (count == 1)
                                {
                                    AreaItemControl value_ctrl = new Values_AreaItemControl();
                                    value_ctrl.ContextMenuCreated += new EventHandler(ctrl_ContextMenuCreated);
                                    m_ColumnsAreaContainer.AddItem(value_ctrl, false);
                                }

                                m_DataAreaContainer.AddItem(ctrl, raise_ItemsListChanged);
                                info_node.UseBoldText = true;
                            }
                        }
                    }
                }
                #endregion Узлы метаданных (InfoBaseTreeNode)
            }
        }

        void m_CubeBrowser_DragCompleted(object sender, DragNodeArgs<DragCompletedEventArgs> e)
        {
            try
            {
                if (e.Args.Canceled == false)
                {
                    TreeViewItem node = e.Node;

                    // Если кидаем в область MDX запроса
                    if (MDXQueryIsReadyToDrop)
                    {
                        String str = m_ServerExplorer.CubeBrowser.GetNodeString(node as CustomTreeNode);
                        if (!String.IsNullOrEmpty(str))
                        {
                            m_MdxQuery.Text += " " + str;
                        }
                        MDXQueryIsReadyToDrop = false;
                    }
                    else
                    {
                        // Если тягается измерение, то подменяем этот узел на первый из узлов иерархий
                        DimensionTreeNode dimNode = node as DimensionTreeNode;
                        if (dimNode != null)
                        {
                            HierarchyTreeNode hierarchyNode = null;

                            // Для начала ищем иерархию среди дочерних
                            foreach (TreeViewItem child in dimNode.Items)
                            {
                                hierarchyNode = child as HierarchyTreeNode;
                                if (hierarchyNode != null)
                                {
                                    node = hierarchyNode;
                                    break;
                                }
                            }
                            if (hierarchyNode == null)
                            {
                                // раз иерархии не нашли среди дочерних, то возможно средидочерних есть папки, в которые иерархия может быть вложена
                                // Значит пытаемся найти рекурсивно
                                hierarchyNode = FindHierarchy(dimNode);
                                if (hierarchyNode == null)
                                    return;
                                else
                                    node = hierarchyNode;
                            }
                        }

                        PivotAreaContainer currentContainer = null;
                        if (m_RowsAreaContainer.IsReadyToDrop)
                        {
                            currentContainer = m_RowsAreaContainer;
                        }
                        if (m_ColumnsAreaContainer.IsReadyToDrop)
                        {
                            currentContainer = m_ColumnsAreaContainer;
                        }
                        if (m_FilterAreaContainer.IsReadyToDrop)
                        {
                            currentContainer = m_FilterAreaContainer;
                        }
                        if (m_DataAreaContainer.IsReadyToDrop)
                        {
                            currentContainer = m_DataAreaContainer;
                        }

                        DropToArea(currentContainer, node as CustomTreeNode);
                    }

                }
            }
            finally
            {
                m_RowsAreaContainer.IsReadyToDrop = false;
                m_ColumnsAreaContainer.IsReadyToDrop = false;
                m_FilterAreaContainer.IsReadyToDrop = false;
                m_DataAreaContainer.IsReadyToDrop = false;
            }
        }

        bool m_MDXQueryIsReadyToDrop = false;
        bool MDXQueryIsReadyToDrop
        {
            get
            {
                return Mdx_Border.Visibility == Visibility.Visible && !m_MdxQuery.IsReadOnly && m_MDXQueryIsReadyToDrop;
            }
            set
            {
                if (m_MDXQueryIsReadyToDrop != value)
                {
                    m_MDXQueryIsReadyToDrop = value;
                    if (value)
                    {
                        Mdx_Border.BorderBrush = new SolidColorBrush(Color.FromArgb(50, Colors.Blue.R, Colors.Blue.G, Colors.Blue.B));
                        Mdx_Border.Background = new SolidColorBrush(Color.FromArgb(20, Colors.Blue.R, Colors.Blue.G, Colors.Blue.B));
                    }
                    else
                    {
                        Mdx_Border.BorderBrush = new SolidColorBrush(Colors.DarkGray);
                        Mdx_Border.Background = new SolidColorBrush(Colors.Transparent);
                    }
                }
            }
        }


        void m_CubeBrowser_DragDelta(object sender, DragNodeArgs<DragDeltaEventArgs> e)
        {
            m_RowsAreaContainer.IsReadyToDrop = false;
            m_ColumnsAreaContainer.IsReadyToDrop = false;
            m_FilterAreaContainer.IsReadyToDrop = false;
            m_DataAreaContainer.IsReadyToDrop = false;
            MDXQueryIsReadyToDrop = false;

            Point m_DragDelta = new Point(m_PrevDrag.X + e.Args.HorizontalChange, m_PrevDrag.Y + e.Args.VerticalChange);

            //m_RowsAreContainer.Caption = m_DragDelta.ToString();
            //List<UIElement> hits = (List<UIElement>)VisualTreeHelper.FindElementsInHostCoordinates(m_DragDelta, Application.Current.RootVisual);
            //if (hits != null)
            //{
            //    foreach (UIElement item in hits)
            //    {
            //        PivotAreaContainer container = item as PivotAreaContainer;
            //        if (container != null)
            //            container.IsReadyToDrop = true;
            //    }
            //}

            if (Mdx_Border.Visibility == Visibility.Visible && !m_MdxQuery.IsReadOnly)
            {
                // Получаем границы поля MDX
                Rect m_MdxQuery_Bounds = AgControlBase.GetSLBounds(m_MdxQuery);
                if (m_MdxQuery_Bounds.Contains(m_DragDelta))
                {
                    MDXQueryIsReadyToDrop = true;
                    m_PrevDrag = m_DragDelta;
                    return;
                }
            }

            if (e.Node is MeasureTreeNode ||
                    e.Node is KpiValueTreeNode ||
                    e.Node is KpiStatusTreeNode ||
                    e.Node is KpiTrendTreeNode ||
                    e.Node is KpiGoalTreeNode ||
                    e.Node is CalculatedMemberTreeNode ||
                    e.Node is NamedSetTreeNode ||
                    e.Node is CalculatedNamedSetTreeNode ||
                    e.Node is DimensionTreeNode ||
                    e.Node is LevelTreeNode ||
                    e.Node is HierarchyTreeNode ||
                    e.Node is MemberLiteTreeNode)
            {
                // В область данных можно таскать только меры, KPI и вычисляемые элементы
                if (e.Node is MeasureTreeNode ||
                        e.Node is KpiValueTreeNode ||
                        e.Node is KpiStatusTreeNode ||
                        e.Node is KpiTrendTreeNode ||
                        e.Node is KpiGoalTreeNode ||
                        e.Node is CalculatedMemberTreeNode)
                {
                    Rect m_DataArea_Bounds = AgControlBase.GetSLBounds(m_DataAreaContainer);
                    if (m_DataArea_Bounds.Contains(m_DragDelta))
                    {
                        m_DataAreaContainer.IsReadyToDrop = true;
                    }
                }
                else
                {
                    Rect m_RowsArea_Bounds = AgControlBase.GetSLBounds(m_RowsAreaContainer);
                    if (m_RowsArea_Bounds.Contains(m_DragDelta))
                    {
                        m_RowsAreaContainer.IsReadyToDrop = true;
                    }

                    Rect m_ColumnsArea_Bounds = AgControlBase.GetSLBounds(m_ColumnsAreaContainer);
                    if (m_ColumnsArea_Bounds.Contains(m_DragDelta))
                    {
                        m_ColumnsAreaContainer.IsReadyToDrop = true;
                    }

                    // Set(ы) в фильтры не добавляются
                    if (!(e.Node is NamedSetTreeNode || e.Node is CalculatedNamedSetTreeNode))
                    {
                        Rect m_FilterArea_Bounds = AgControlBase.GetSLBounds(m_FilterAreaContainer);
                        if (m_FilterArea_Bounds.Contains(m_DragDelta))
                        {
                            m_FilterAreaContainer.IsReadyToDrop = true;
                        }
                    }
                }
            }

            m_PrevDrag = m_DragDelta;
        }

        #endregion Drag and Drop из метаданных куба

        #region Фильтры
        /// <summary>
        /// Коллекция диалогов для построения фильтра
        /// </summary>
        Dictionary<AreaItemControl, ModalDialog> m_FilterDialogs = new Dictionary<AreaItemControl, ModalDialog>();

        //public event EventHandler<GetIDataLoaderArgs> GetMetadataLoader;
        //public event EventHandler<GetIDataLoaderArgs> GetMembersLoader;

        //void Raise_GetMetadataLoader(GetIDataLoaderArgs args)
        //{
        //    EventHandler<GetIDataLoaderArgs> handler = this.GetMetadataLoader;
        //    if (handler != null)
        //    {
        //        handler(this, args);
        //    }
        //}

        //void Raise_GetMembersLoader(GetIDataLoaderArgs args)
        //{
        //    EventHandler<GetIDataLoaderArgs> handler = this.GetMembersLoader;
        //    if (handler != null)
        //    {
        //        handler(this, args);
        //    }
        //}

        void FilteredItem_ShowFilter(object sender, EventArgs e)
        {
            ModalDialog dialog = null;

            Composite_FilterWrapper composite_wrapper = null;
            FilteredItemControl item = sender as FilteredItemControl;
            if (item != null)
            {
                if (m_FilterDialogs.ContainsKey(item))
                {
                    dialog = m_FilterDialogs[item];
                }

                Hierarchy_AreaItemControl hierarchy_Item = item as Hierarchy_AreaItemControl;
                Level_AreaItemControl level_Item = item as Level_AreaItemControl;
                if (hierarchy_Item != null)
                {
                    composite_wrapper = hierarchy_Item.FilteredWrapper.CompositeFilter;
                }

                if (level_Item != null)
                {
                    composite_wrapper = level_Item.FilteredWrapper.CompositeFilter;
                }

                if (dialog == null)
                {
                    if (hierarchy_Item != null || level_Item != null)
                    {
                        String hierarchy_UniqueName = String.Empty;
                        String level_UniqueName = String.Empty;

                        if (hierarchy_Item != null)
                        {
                            hierarchy_UniqueName = hierarchy_Item.Hierarchy.UniqueName;
                        }

                        if (level_Item != null)
                        {
                            level_UniqueName = level_Item.Level.UniqueName;
                            hierarchy_UniqueName = level_Item.Level.HierarchyUniqueName;
                        }

                        dialog = new ModalDialog();
                        FilterBuilderControl filterControl = new FilterBuilderControl();
                        filterControl.ChoiceControl.CubeName = m_ServerExplorer.CurrentCubeName;
                        filterControl.ChoiceControl.SubCube = SubCube;
                        filterControl.ChoiceControl.Connection = Connection;
                        filterControl.ChoiceControl.HierarchyUniqueName = hierarchy_UniqueName;
                        filterControl.ChoiceControl.LogManager = LogManager;
                        filterControl.ChoiceControl.StartLevelUniqueName = level_UniqueName;
                        filterControl.ChoiceControl.MultiSelect = true;

                        filterControl.ChoiceControl.OlapDataLoader = GetOlapDataLoader();
                        filterControl.CubeInfo = m_ServerExplorer.CubeBrowser.CubeInfo;

                        dialog.Caption = Localization.FilterBuilder_Caption + "...";
                        dialog.Content = filterControl;
                        dialog.Tag = item;
                        dialog.Width = 650;
                        dialog.Height = 500;
                        dialog.DialogClosed += new EventHandler<DialogResultArgs>(dialog_DialogClosed);

                        m_FilterDialogs[item] = dialog;
                    }
                }
            }

            if (dialog != null)
            {
                FilterBuilderControl filterControl = dialog.Content as FilterBuilderControl;
                if (filterControl != null)
                {
                    filterControl.ChoiceControl.CubeName = m_ServerExplorer.CurrentCubeName;

                    if (item.Area == m_ColumnsAreaContainer ||
                            item.Area == m_RowsAreaContainer)
                        filterControl.UseFilterControl = true;
                    else
                        filterControl.UseFilterControl = false;

                    filterControl.Initialize(composite_wrapper);
                }

                dialog.Show();
            }
        }

        void dialog_DialogClosed(object sender, DialogResultArgs e)
        {
            ModalDialog dialog = sender as ModalDialog;
            if (dialog != null)
            {
                if (e.Result == DialogResult.OK)
                {

                    FilteredItemControl filtered_Item = dialog.Tag as FilteredItemControl;

                    if (filtered_Item != null)
                    {
                        if (filtered_Item.FilteredWrapper != null)
                        {
                            FilterBuilderControl filterControl = dialog.Content as FilterBuilderControl;
                            if (filterControl != null)
                            {
                                filtered_Item.FilteredWrapper.CompositeFilter = filterControl.CompositeFilter;
                            }
                        }
                        filtered_Item.Refresh();
                    }
                    RefreshMdxQuery();
                }
                //else
                //{
                //    var filterControl = dialog.Content as FilterBuilderControl;
                //    if (filterControl != null)
                //    {
                //        // Переинициализировать контрол выбора элементов измерения в фильтре при открытии
                //        filterControl.MemberChoiceIsInitialized = false;
                //    }
                //}
            }
        }

        protected virtual IDataLoader GetOlapDataLoader()
        {
            return new OlapDataLoader(URL);
        }

        IDataLoader m_OlapDataLoader = null;
        public IDataLoader OlapDataLoader
        {
            set
            {
                m_OlapDataLoader = value;
                m_ServerExplorer.OlapDataLoader = m_OlapDataLoader;
                m_PivotGrid.OlapDataLoader = m_OlapDataLoader;
            }
            get
            {
                return m_OlapDataLoader;
            }
        }

        IStorageManager m_StorageManager = null;
        public IStorageManager StorageManager
        {
            get
            {
                return m_StorageManager;
            }
        }

        protected virtual IStorageManager GetStorageManager()
        {
            return new StorageManager(URL);
        }

        #endregion

        #region Контекстное меню
        Dictionary<AreaItemControl, ContextMenuItem> m_MoveToFilters_MenuItems = new Dictionary<AreaItemControl, ContextMenuItem>();
        Dictionary<AreaItemControl, ContextMenuItem> m_MoveToRows_MenuItems = new Dictionary<AreaItemControl, ContextMenuItem>();
        Dictionary<AreaItemControl, ContextMenuItem> m_MoveToColumns_MenuItems = new Dictionary<AreaItemControl, ContextMenuItem>();
        Dictionary<AreaItemControl, ContextMenuSplitter> m_MoveSplitters = new Dictionary<AreaItemControl, ContextMenuSplitter>();

        void ctrl_ContextMenuCreated(object sender, EventArgs e)
        {
            AreaItemControl item = sender as AreaItemControl;
            if (item != null)
            {
                if (item.ContextMenu.Items.Count > 0)
                {
                    ContextMenuSplitter splitter = item.ContextMenu.AddMenuSplitter();
                    splitter.Tag = item;
                    splitter.Visibility = Visibility.Collapsed;
                    m_MoveSplitters[item] = splitter;

                    if (item is Values_AreaItemControl)
                    {
                        // Спец. элемент
                    }
                    else
                    {
                        ContextMenuItem MoveToFilters_MenuItem = new ContextMenuItem(Localization.MdxDesigner_ContextMenu_MoveToFiltersArea);
                        MoveToFilters_MenuItem.Icon = UriResources.Images.FiltersArea16;
                        MoveToFilters_MenuItem.ItemClick += new EventHandler(MoveToFilters_MenuItem_ItemClick);
                        item.ContextMenu.AddMenuItem(MoveToFilters_MenuItem);
                        MoveToFilters_MenuItem.Tag = item;
                        MoveToFilters_MenuItem.Visibility = Visibility.Collapsed;
                        m_MoveToFilters_MenuItems[item] = MoveToFilters_MenuItem;
                    }

                    ContextMenuItem MoveToRows_MenuItem = new ContextMenuItem(Localization.MdxDesigner_ContextMenu_MoveToRowsArea);
                    MoveToRows_MenuItem.Icon = UriResources.Images.RowsArea16;
                    MoveToRows_MenuItem.ItemClick += new EventHandler(MoveToRows_MenuItem_ItemClick);
                    item.ContextMenu.AddMenuItem(MoveToRows_MenuItem);
                    MoveToRows_MenuItem.Tag = item;
                    MoveToRows_MenuItem.Visibility = Visibility.Collapsed;
                    m_MoveToRows_MenuItems[item] = MoveToRows_MenuItem;

                    ContextMenuItem MoveToColumns_MenuItem = new ContextMenuItem(Localization.MdxDesigner_ContextMenu_MoveToColumnsArea);
                    MoveToColumns_MenuItem.Icon = UriResources.Images.ColumnsArea16;
                    MoveToColumns_MenuItem.ItemClick += new EventHandler(MoveToColumns_MenuItem_ItemClick);
                    item.ContextMenu.AddMenuItem(MoveToColumns_MenuItem);
                    MoveToColumns_MenuItem.Tag = item;
                    MoveToColumns_MenuItem.Visibility = Visibility.Collapsed;
                    m_MoveToColumns_MenuItems[item] = MoveToColumns_MenuItem;
                }
            }
        }

        void ChangeMoveItemsVisibility(AreaItemControl item, Visibility type)
        {
            if (item != null)
            {
                ChangeMoveSplitterVisibility(item, type);
                ChangeMoveToColumnsVisibility(item, type);
                ChangeMoveToFiltersVisibility(item, type);
                ChangeMoveToRowsVisibility(item, type);
            }
        }

        void ChangeMoveToColumnsVisibility(AreaItemControl item, Visibility type)
        {
            if (item != null)
            {
                if (m_MoveToColumns_MenuItems.ContainsKey(item))
                {
                    m_MoveToColumns_MenuItems[item].Visibility = type;
                }
            }
        }

        void ChangeMoveToRowsVisibility(AreaItemControl item, Visibility type)
        {
            if (item != null)
            {
                if (m_MoveToRows_MenuItems.ContainsKey(item))
                {
                    m_MoveToRows_MenuItems[item].Visibility = type;
                }
            }
        }

        void ChangeMoveSplitterVisibility(AreaItemControl item, Visibility type)
        {
            if (item != null)
            {
                if (m_MoveSplitters.ContainsKey(item))
                {
                    m_MoveSplitters[item].Visibility = type;
                }
            }
        }

        void ChangeMoveToFiltersVisibility(AreaItemControl item, Visibility type)
        {
            if (item != null)
            {
                if (m_MoveToFilters_MenuItems.ContainsKey(item))
                {
                    m_MoveToFilters_MenuItems[item].Visibility = type;
                }
            }
        }

        void m_ColumnsAreaContainer_BeforeShowContextMenu(object sender, AreaItemArgs e)
        {
            ChangeMoveToColumnsVisibility(e.ItemControl, Visibility.Collapsed);

            ChangeMoveToRowsVisibility(e.ItemControl, Visibility.Visible);
            ChangeMoveToFiltersVisibility(e.ItemControl, Visibility.Visible);
            ChangeMoveSplitterVisibility(e.ItemControl, Visibility.Visible);
        }

        void m_RowsAreaContainer_BeforeShowContextMenu(object sender, AreaItemArgs e)
        {
            ChangeMoveToRowsVisibility(e.ItemControl, Visibility.Collapsed);

            ChangeMoveToColumnsVisibility(e.ItemControl, Visibility.Visible);
            ChangeMoveToFiltersVisibility(e.ItemControl, Visibility.Visible);
            ChangeMoveSplitterVisibility(e.ItemControl, Visibility.Visible);
        }

        void m_FilterAreaContainer_BeforeShowContextMenu(object sender, AreaItemArgs e)
        {
            ChangeMoveToFiltersVisibility(e.ItemControl, Visibility.Collapsed);

            ChangeMoveToColumnsVisibility(e.ItemControl, Visibility.Visible);
            ChangeMoveToRowsVisibility(e.ItemControl, Visibility.Visible);
            ChangeMoveSplitterVisibility(e.ItemControl, Visibility.Visible);
        }
        #endregion Контекстное меню

        #region Перемещение элемента между областями
        void MoveToColumns_MenuItem_ItemClick(object sender, EventArgs e)
        {
            ContextMenuItem menuItem = sender as ContextMenuItem;
            if (menuItem != null)
            {
                AreaItemControl item = menuItem.Tag as AreaItemControl;
                if (item != null)
                {
                    // Спец. элемент удаляем с отпиской/подпиской удаления. Чтобы не удалились элементы из области данных
                    if (item is Values_AreaItemControl)
                    {
                        m_RowsAreaContainer.ItemRemoved -= new EventHandler<AreaItemArgs>(AreaContainer_ItemRemoved);
                        m_RowsAreaContainer.RemoveItem(item, false);
                        m_RowsAreaContainer.ItemRemoved += new EventHandler<AreaItemArgs>(AreaContainer_ItemRemoved);
                    }
                    else
                    {
                        m_RowsAreaContainer.RemoveItem(item, false);
                    }

                    m_FilterAreaContainer.RemoveItem(item, false);

                    m_ColumnsAreaContainer.AddItem(item);
                }
            }
        }

        void MoveToRows_MenuItem_ItemClick(object sender, EventArgs e)
        {
            ContextMenuItem menuItem = sender as ContextMenuItem;
            if (menuItem != null)
            {
                AreaItemControl item = menuItem.Tag as AreaItemControl;
                if (item != null)
                {
                    // Спец. элемент удаляем с отпиской/подпиской удаления. Чтобы не удалились элементы из области данных
                    if (item is Values_AreaItemControl)
                    {
                        m_ColumnsAreaContainer.ItemRemoved -= new EventHandler<AreaItemArgs>(AreaContainer_ItemRemoved);
                        m_ColumnsAreaContainer.RemoveItem(item, false);
                        m_ColumnsAreaContainer.ItemRemoved += new EventHandler<AreaItemArgs>(AreaContainer_ItemRemoved);
                    }
                    else
                    {
                        m_ColumnsAreaContainer.RemoveItem(item, false);
                    }

                    m_FilterAreaContainer.RemoveItem(item, false);

                    m_RowsAreaContainer.AddItem(item);
                }
            }
        }

        void MoveToFilters_MenuItem_ItemClick(object sender, EventArgs e)
        {
            ContextMenuItem menuItem = sender as ContextMenuItem;
            if (menuItem != null)
            {
                AreaItemControl item = menuItem.Tag as AreaItemControl;
                if (item != null)
                {
                    m_ColumnsAreaContainer.RemoveItem(item, false);
                    m_RowsAreaContainer.RemoveItem(item, false);

                    m_FilterAreaContainer.AddItem(item);
                }
            }
        }
        #endregion Перемещение элемента между областями

        AreaItemControl FindValuesItem(PivotAreaContainer container)
        {
            if (container != null)
            {
                foreach (AreaItemControl item in container.Items)
                {
                    if (item is Values_AreaItemControl)
                        return item;
                }
            }
            return null;
        }

        /// <summary>
        /// Возвращает конкурента для элемента в указанной области. Конкурентами (для строк, колонок, фильтров) считаются элементы, принадлежащие одной иерархии
        /// Конкурентами для области данных являются элементы (меры, вычисляемые элементы) с таким же именем
        /// </summary>
        /// <param name="container"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        AreaItemControl FindConcurent(PivotAreaContainer container, TreeViewItem node)
        {
            if (container != null)
            {
                InfoBaseTreeNode infoNode = node as InfoBaseTreeNode;
                if (container == m_DataAreaContainer)
                {
                    CalculatedMemberTreeNode calcMemberNode = node as CalculatedMemberTreeNode;
                    if (infoNode != null || calcMemberNode != null)
                    {
                        KpiInfo kpi = infoNode != null ? infoNode.Info as KpiInfo : null;
                        MeasureInfo measure = infoNode != null ? infoNode.Info as MeasureInfo : null;
                        CalcMemberInfo member = calcMemberNode != null ? calcMemberNode.Info : null;

                        List<AreaItemControl> items = container.Items;
                        foreach (AreaItemControl item in items)
                        {
                            Measure_AreaItemControl measure_Item = item as Measure_AreaItemControl;
                            if (measure_Item != null && measure != null && measure.UniqueName == measure_Item.Measure.UniqueName)
                            {
                                return item;
                            }

                            Kpi_AreaItemControl kpi_Item = item as Kpi_AreaItemControl;
                            if (kpi_Item != null && kpi != null && kpi.Name == kpi_Item.Kpi.Name)
                            {
                                return item;
                            }

                            CalculatedMember_AreaItemControl member_Item = item as CalculatedMember_AreaItemControl;
                            if (member_Item != null && member != null && member.Name == member_Item.CalculatedMember.Name)
                            {
                                return item;
                            }
                        }
                    }
                }

                if (container == m_RowsAreaContainer ||
                        container == m_ColumnsAreaContainer ||
                        container == m_FilterAreaContainer)
                {
                    var setNode = node as CalculatedNamedSetTreeNode;
                    var memberNode = node as MemberLiteTreeNode;

                    if (infoNode != null || setNode != null || memberNode != null)
                    {
                        // если node - узел иерархии, то ищем по уникальному имени еирархии среди элементов
                        // если node - узел уровня, то ищем по уникальному имени иерархии, которой этот узел принадлежит

                        HierarchyInfo hierarchy = infoNode != null ? infoNode.Info as HierarchyInfo : null;
                        LevelInfo level = infoNode != null ? infoNode.Info as LevelInfo : null;
                        CalculatedNamedSetInfo calculatedSet = setNode != null ? setNode.Info : null;
                        NamedSetInfo set = infoNode != null ? infoNode.Info as NamedSetInfo : null;

                        List<AreaItemControl> items = container.Items;
                        foreach (AreaItemControl item in items)
                        {
                            CalculateNamedSet_AreaItemControl calculatedSet_Item = item as CalculateNamedSet_AreaItemControl;
                            if (calculatedSet_Item != null && calculatedSet != null && calculatedSet.Name == calculatedSet_Item.CalculatedNamedSet.Name)
                            {
                                return item;
                            }

                            NamedSet_AreaItemControl set_Item = item as NamedSet_AreaItemControl;
                            if (set_Item != null && set != null && set.Name == set_Item.NamedSet.Name)
                            {
                                return item;
                            }

                            Hierarchy_AreaItemControl hierarchy_Item = item as Hierarchy_AreaItemControl;
                            if (hierarchy_Item != null)
                            {
                                if (hierarchy != null && hierarchy_Item.Hierarchy.UniqueName == hierarchy.UniqueName)
                                {
                                    return item;
                                }
                                if (level != null && hierarchy_Item.Hierarchy.UniqueName == level.ParentHirerachyId)
                                {
                                    return item;
                                }
                                if (memberNode != null && memberNode.Info != null && hierarchy_Item.Hierarchy.UniqueName == memberNode.Info.HierarchyUniqueName)
                                {
                                    return item;
                                }
                            }

                            Level_AreaItemControl level_Item = item as Level_AreaItemControl;
                            if (level_Item != null)
                            {
                                if (hierarchy != null && level_Item.Level.HierarchyUniqueName == hierarchy.UniqueName)
                                {
                                    return item;
                                }
                                if (level != null && level_Item.Level.HierarchyUniqueName == level.ParentHirerachyId)
                                {
                                    return item;
                                }
                                if (memberNode != null && memberNode.Info != null && level_Item.Level.HierarchyUniqueName == memberNode.Info.HierarchyUniqueName)
                                {
                                    return item;
                                }
                            }
                        }
                    }
                }
            }
            return null;
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
                m_ServerExplorer.Connection = value;
                m_PivotGrid.Connection = value;
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

        /// <summary>
        /// Скрипт для обновления значений ячеек сводной таблицы
        /// </summary>
        public String UpdateScript
        {
            get
            {
                return m_PivotGrid.UpdateScript;
            }
            set
            {
                m_PivotGrid.UpdateScript = value;
            }
        }

        private String FromSet
        {
            get
            {
                String from_Set = String.Empty;
                if (String.IsNullOrEmpty(SubCube))
                    from_Set = OlapHelper.ConvertToQueryStyle(m_ServerExplorer.CurrentCubeName);
                else
                    from_Set = OlapHelper.ConvertSubCubeToQueryStyle(SubCube);
                return from_Set;
            }
        }

        #endregion Свойства для настройки на OLAP

        public void Clear()
        {
            m_FilterAreaContainer.ItemsListChanged -= new EventHandler(AreaContainer_ItemsListChanged);
            m_RowsAreaContainer.ItemsListChanged -= new EventHandler(AreaContainer_ItemsListChanged);
            m_ColumnsAreaContainer.ItemsListChanged -= new EventHandler(AreaContainer_ItemsListChanged);
            m_DataAreaContainer.ItemsListChanged -= new EventHandler(AreaContainer_ItemsListChanged);

            CalculatedNamedSets.Clear();
            CalculatedMembers.Clear();

            m_FilterAreaContainer.Clear();
            m_RowsAreaContainer.Clear();
            m_ColumnsAreaContainer.Clear();
            m_DataAreaContainer.Clear();

            m_FilterDialogs.Clear();
            m_MdxQuery.Text = String.Empty;

            InitializePivotGrid(String.Empty);

            m_FilterAreaContainer.ItemsListChanged += new EventHandler(AreaContainer_ItemsListChanged);
            m_RowsAreaContainer.ItemsListChanged += new EventHandler(AreaContainer_ItemsListChanged);
            m_ColumnsAreaContainer.ItemsListChanged += new EventHandler(AreaContainer_ItemsListChanged);
            m_DataAreaContainer.ItemsListChanged += new EventHandler(AreaContainer_ItemsListChanged);
        }

        public void Initialize()
        {
            if (m_ServerExplorer.LogManager != LogManager)
                m_ServerExplorer.LogManager = LogManager;


            m_CalculatedItemsEditor = null;
            Clear();

            m_ServerExplorer.CubeBrowser.Initialized += new EventHandler(CubeBrowser_Initialized);
            m_ServerExplorer.Initialize();
        }

        void CubeBrowser_Initialized(object sender, EventArgs e)
        {
            m_ServerExplorer.CubeBrowser.Initialized -= new EventHandler(CubeBrowser_Initialized);

            if (DefaultTuples != null)
            {
                foreach (var tuple in DefaultTuples)
                {
                    InitializeAreasByTuple(tuple);
                }
                RefreshMdxQuery();
            }
        }

        void InitializeAreasByTuple(List<ShortMemberInfo> tuple)
        {
            if (tuple != null)
            {
                Clear();

                foreach (var item in tuple)
                {
                    if (item.HierarchyUniqueName != "[Measures]")
                    {
                        // Добавляем новый узел для иерархии
                        var hierarchyNode = m_ServerExplorer.CubeBrowser.FindHierarchyNode(item.HierarchyUniqueName);
                        if (hierarchyNode != null)
                        {
                            MemberLiteTreeNode node = new MemberLiteTreeNode(new MemberData() { UniqueName = item.UniqueName, HierarchyUniqueName = item.HierarchyUniqueName });
                            DropToArea(m_FilterAreaContainer, node, false);
                        }
                    }
                    else
                    {
                        var measureNode = m_ServerExplorer.CubeBrowser.FindMeasureNode(item.UniqueName);
                        // меры в область данных
                        if (measureNode != null && measureNode.Info is MeasureInfo)
                        {
                            DropToArea(m_DataAreaContainer, measureNode, false);
                        }
                    }
                }
            }
        }

        protected virtual void InitializePivotGrid(String query)
        {
            m_PivotGrid.Query = query;
            m_PivotGrid.Initialize();
        }

        #region Экспорт-импорт
        public String ExportMdxLayoutInfo()
        {
            String res = String.Empty;

            MdxLayoutWrapper layout = new MdxLayoutWrapper();
            layout.CubeName = m_ServerExplorer.CurrentCubeName;
            layout.SubCube = SubCube;
            layout.Filters = GetItemWrappers(m_FilterAreaContainer);
            layout.Rows = GetItemWrappers(m_RowsAreaContainer);
            layout.Columns = GetItemWrappers(m_ColumnsAreaContainer);
            layout.Data = GetItemWrappers(m_DataAreaContainer);
            layout.CalculatedMembers = CalculatedMembers;
            layout.CalculatedNamedSets = CalculatedNamedSets;

            return XmlSerializationUtility.Obj2XmlStr(layout);
        }

        public void ImportMdxLayoutInfo(String str)
        {
            if (!String.IsNullOrEmpty(str))
            {
                MdxLayoutWrapper layout = XmlSerializationUtility.XmlStr2Obj<MdxLayoutWrapper>(str);
                if (layout != null)
                {
                    Clear();

                    bool cube_found = m_ServerExplorer.SelectCube(layout.CubeName);
                    if (!String.IsNullOrEmpty(layout.CubeName) && !cube_found)
                    {
                        MessageBox.Show(String.Format(Localization.CubeNotFound_Message, layout.CubeName), Localization.Warning, MessageBoxButton.OK);
                    }

                    SubCube = layout.SubCube;

                    BuildFromItemWrappers(m_FilterAreaContainer, layout.Filters);
                    BuildFromItemWrappers(m_RowsAreaContainer, layout.Rows);
                    BuildFromItemWrappers(m_ColumnsAreaContainer, layout.Columns);
                    BuildFromItemWrappers(m_DataAreaContainer, layout.Data);
                    CalculatedMembers = layout.CalculatedMembers;
                    CalculatedNamedSets = layout.CalculatedNamedSets;

                    HighLightCustomNodes();
                    RefreshMdxQuery();
                }
            }
        }

        CustomTreeNode FindCustomNode(AreaItemWrapper wrapper)
        {
            CustomTreeNode nodeInTree = null;
            if (wrapper != null)
            {
                Measure_AreaItemWrapper measures_item = wrapper as Measure_AreaItemWrapper;
                if (measures_item != null)
                {
                    nodeInTree = m_ServerExplorer.CubeBrowser.FindMeasureNode(measures_item.UniqueName);
                }

                CalculatedMember_AreaItemWrapper calcMember_item = wrapper as CalculatedMember_AreaItemWrapper;
                if (calcMember_item != null)
                {
                    nodeInTree = m_ServerExplorer.CubeBrowser.FindCalculatedMember(calcMember_item.Name);
                }

                CalculatedNamedSet_AreaItemWrapper calculatedSet_item = wrapper as CalculatedNamedSet_AreaItemWrapper;
                if (calculatedSet_item != null)
                {
                    nodeInTree = m_ServerExplorer.CubeBrowser.FindCalculatedNamedSet(calculatedSet_item.Name);
                }

                NamedSet_AreaItemWrapper set_item = wrapper as NamedSet_AreaItemWrapper;
                if (set_item != null)
                {
                    nodeInTree = m_ServerExplorer.CubeBrowser.FindNamedSet(set_item.Name);
                }

                Level_AreaItemWrapper level_item = wrapper as Level_AreaItemWrapper;
                if (level_item != null)
                {
                    nodeInTree = m_ServerExplorer.CubeBrowser.FindLevelNode(level_item.UniqueName);
                }

                Hierarchy_AreaItemWrapper hierarchy_item = wrapper as Hierarchy_AreaItemWrapper;
                if (hierarchy_item != null)
                {
                    nodeInTree = m_ServerExplorer.CubeBrowser.FindHierarchyNode(hierarchy_item.UniqueName);
                }

                Kpi_AreaItemWrapper kpi_item = wrapper as Kpi_AreaItemWrapper;
                if (kpi_item != null)
                {
                    switch (kpi_item.Type)
                    {
                        case KpiControlType.Goal:
                            nodeInTree = m_ServerExplorer.CubeBrowser.FindKpiGoalNode(kpi_item.Name);
                            break;
                        case KpiControlType.Status:
                            nodeInTree = m_ServerExplorer.CubeBrowser.FindKpiStatusNode(kpi_item.Name);
                            break;
                        case KpiControlType.Trend:
                            nodeInTree = m_ServerExplorer.CubeBrowser.FindKpiTrendNode(kpi_item.Name);
                            break;
                        case KpiControlType.Value:
                            nodeInTree = m_ServerExplorer.CubeBrowser.FindKpiValueNode(kpi_item.Name);
                            break;
                    }
                }
            }

            return nodeInTree;
        }

        void BuildFromItemWrappers(PivotAreaContainer container, List<AreaItemWrapper> wrappers)
        {
            if (container != null)
            {
                container.Clear();

                if (wrappers != null)
                {
                    container.ItemsListChanged -= new EventHandler(AreaContainer_ItemsListChanged);
                    foreach (AreaItemWrapper wrapper in wrappers)
                    {
                        AreaItemControl ctrl = null;
                        CustomTreeNode nodeInTree = FindCustomNode(wrapper);

                        Values_AreaItemWrapper values_item = wrapper as Values_AreaItemWrapper;
                        if (values_item != null)
                        {
                            ctrl = new Values_AreaItemControl();
                        }

                        Measure_AreaItemWrapper measures_item = wrapper as Measure_AreaItemWrapper;
                        if (measures_item != null)
                        {
                            ctrl = new Measure_AreaItemControl(measures_item);
                        }

                        CalculatedMember_AreaItemWrapper calcMember_item = wrapper as CalculatedMember_AreaItemWrapper;
                        if (calcMember_item != null)
                        {
                            ctrl = new CalculatedMember_AreaItemControl(calcMember_item);
                        }

                        CalculatedNamedSet_AreaItemWrapper calculatedSet_item = wrapper as CalculatedNamedSet_AreaItemWrapper;
                        if (calculatedSet_item != null)
                        {
                            ctrl = new CalculateNamedSet_AreaItemControl(calculatedSet_item);
                        }

                        NamedSet_AreaItemWrapper set_item = wrapper as NamedSet_AreaItemWrapper;
                        if (set_item != null)
                        {
                            ctrl = new NamedSet_AreaItemControl(set_item);
                        }

                        Level_AreaItemWrapper level_item = wrapper as Level_AreaItemWrapper;
                        if (level_item != null)
                        {
                            ctrl = new Level_AreaItemControl(level_item);
                        }

                        Hierarchy_AreaItemWrapper hierarchy_item = wrapper as Hierarchy_AreaItemWrapper;
                        if (hierarchy_item != null)
                        {
                            ctrl = new Hierarchy_AreaItemControl(hierarchy_item);
                        }

                        Kpi_AreaItemWrapper kpi_item = wrapper as Kpi_AreaItemWrapper;
                        if (kpi_item != null)
                        {
                            ctrl = new Kpi_AreaItemControl(kpi_item);
                        }

                        if (ctrl != null)
                        {
                            if (nodeInTree != null)
                            {
                                ctrl.UserData = nodeInTree;
                                ctrl.Icon = nodeInTree.Icon;
                                nodeInTree.UseBoldText = true;
                            }

                            container.AddItem(ctrl);
                            ctrl.ContextMenuCreated += new EventHandler(ctrl_ContextMenuCreated);

                            FilteredItemControl filtered_item = ctrl as FilteredItemControl;
                            if (filtered_item != null)
                            {
                                filtered_item.ShowFilter += new EventHandler(FilteredItem_ShowFilter);
                                filtered_item.CancelFilter += new EventHandler(FilteredItem_CancelFilter);
                            }
                        }
                    }
                    container.ItemsListChanged += new EventHandler(AreaContainer_ItemsListChanged);
                }
            }
        }

        void FilteredItem_CancelFilter(object sender, EventArgs e)
        {
            RefreshMdxQuery();
        }

        List<AreaItemWrapper> GetItemWrappers(PivotAreaContainer container)
        {
            List<AreaItemWrapper> list = new List<AreaItemWrapper>();
            if (container != null)
            {
                foreach (AreaItemControl item in container.Items)
                {
                    Values_AreaItemControl values_item = item as Values_AreaItemControl;
                    if (values_item != null)
                    {
                        list.Add(new Values_AreaItemWrapper());
                        continue;
                    }

                    Measure_AreaItemControl measures_item = item as Measure_AreaItemControl;
                    if (measures_item != null)
                    {
                        list.Add(measures_item.Measure);
                        continue;
                    }

                    CalculateNamedSet_AreaItemControl calculatedSet_item = item as CalculateNamedSet_AreaItemControl;
                    if (calculatedSet_item != null)
                    {
                        list.Add(calculatedSet_item.CalculatedNamedSet);
                        continue;
                    }

                    NamedSet_AreaItemControl set_item = item as NamedSet_AreaItemControl;
                    if (set_item != null)
                    {
                        list.Add(set_item.NamedSet);
                        continue;
                    }

                    CalculatedMember_AreaItemControl calcMember_item = item as CalculatedMember_AreaItemControl;
                    if (calcMember_item != null)
                    {
                        list.Add(calcMember_item.CalculatedMember);
                        continue;
                    }

                    FilteredItemControl level_item = item as FilteredItemControl;
                    if (level_item != null)
                    {
                        list.Add(level_item.Wrapper);
                        continue;
                    }

                    Kpi_AreaItemControl kpi_item = item as Kpi_AreaItemControl;
                    if (kpi_item != null)
                    {
                        list.Add(kpi_item.Kpi);
                        continue;
                    }
                }
            }
            return list;
        }

        #endregion Экспорт-импорт

        public override ILogService LogManager
        {
            get { return base.LogManager; }
            set
            {
                base.LogManager = value;
                m_ServerExplorer.LogManager = value;
                m_PivotGrid.LogManager = value;
            }
        }

        /// <summary>
        /// Возможность выбрать куб из списка
        /// </summary>
        public bool CanSelectCube
        {
            get { return m_ServerExplorer.CanSelectCube; }
            set { m_ServerExplorer.CanSelectCube = value; }
        }

        public string MdxQuery
        {
            get
            {
                return m_MdxQuery.Text;
            }
        }

        #region Управление видимостью кнопок на тулбаре
        public bool ShowMetadataArea_ButtonVisible
        {
            get { return m_ShowMetadataArea.Visibility == Visibility.Visible ? true : false; }
            set
            {
                m_ShowMetadataArea.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
                UpdateToolBar();
            }
        }

        public bool ShowMDXQuery_ButtonVisible
        {
            get { return m_ShowMDXQuery.Visibility == Visibility.Visible ? true : false; }
            set
            {
                m_ShowMDXQuery.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
                UpdateToolBar();
            }
        }

        public bool EditMDXQuery_ButtonVisible
        {
            get { return m_EditMDXQuery.Visibility == Visibility.Visible ? true : false; }
            set
            {
                m_EditMDXQuery.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
                UpdateToolBar();
            }
        }

        public bool RunQueryAutomatic_ButtonVisible
        {
            get { return m_RunQueryAutomatic.Visibility == Visibility.Visible ? true : false; }
            set
            {
                m_RunQueryAutomatic.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
                UpdateToolBar();
            }
        }

        public bool ExecuteQuery_ButtonVisible
        {
            get { return m_ExecuteQuery.Visibility == Visibility.Visible ? true : false; }
            set
            {
                m_ExecuteQuery.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
                UpdateToolBar();
            }
        }

        public bool CalculatedMemberEditor_ButtonVisible
        {
            get { return m_CalculatedMemberEditor.Visibility == Visibility.Visible ? true : false; }
            set
            {
                m_CalculatedMemberEditor.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
                UpdateToolBar();
            }
        }

        public bool ExportLayout_ButtonVisible
        {
            get { return m_ExportLayout.Visibility == Visibility.Visible ? true : false; }
            set
            {
                m_ExportLayout.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
                UpdateToolBar();
            }
        }

        public bool ImportLayout_ButtonVisible
        {
            get { return m_ImportLayout.Visibility == Visibility.Visible ? true : false; }
            set
            {
                m_ImportLayout.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
                UpdateToolBar();
            }
        }

        public bool ToolBar_Visible
        {
            get { return m_ToolBar.Visibility == Visibility.Visible ? true : false; }
            set
            {
                m_ToolBar.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        void UpdateToolBar()
        {
            m_RunAreaSplitter.Visibility = (ShowMetadataArea_ButtonVisible || ShowMDXQuery_ButtonVisible || EditMDXQuery_ButtonVisible) ? Visibility.Visible : Visibility.Collapsed;
            m_StorageAreaSplitter.Visibility = (CalculatedMemberEditor_ButtonVisible || RunQueryAutomatic_ButtonVisible || ExecuteQuery_ButtonVisible) ? Visibility.Visible : Visibility.Collapsed;

            m_ToolBar.Visibility = (m_RunAreaSplitter.Visibility == Visibility.Visible || m_StorageAreaSplitter.Visibility == Visibility.Visible || ImportLayout_ButtonVisible || ExportLayout_ButtonVisible) ? Visibility.Visible : Visibility.Collapsed;
        }
        #endregion Управление видимостью кнопок на тулбаре

        public List<List<ShortMemberInfo>> DefaultTuples;

        public DataReorganizationTypes DataReorganizationType
        {
            get { return PivotGrid.DataReorganizationType; }
            set { PivotGrid.DataReorganizationType = value; }
        }

    }
}
