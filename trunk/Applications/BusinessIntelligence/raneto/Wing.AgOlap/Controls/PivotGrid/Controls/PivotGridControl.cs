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
using Ranet.Olap.Core.Providers;
using Ranet.AgOlap.Controls.PivotGrid.Layout;
using System.Collections.Generic;
using Ranet.Olap.Core.Data;
using System.Windows.Controls.Primitives;
using Ranet.AgOlap.Controls.ContextMenu;
using Ranet.AgOlap.Controls.General;
using Ranet.AgOlap.Controls.PivotGrid.Conditions;
using Ranet.AgOlap.Controls.PivotGrid.Editors;
using Ranet.AgOlap.Features;
using System.Windows.Browser;
using Ranet.Olap.Core.Providers.ClientServer;
using Ranet.Olap.Core;
using Ranet.AgOlap.Providers;

namespace Ranet.AgOlap.Controls.PivotGrid.Controls
{
    public delegate void PerformMemberActionEventHandler(object sender, PerformMemberActionArgs args);
    public delegate void MemberActionEventHandler(object sender, MemberActionEventArgs args);
    public delegate void CellValueChangedEventHandler(object sender, CellValueChangedEventArgs e);
    public delegate void FocusedCellChangedEventHandler(object sender, FocusedCellChangedEventArgs e);

    public class PivotGridControl : UserControl
    {
        double m_DEFAULT_WIDTH = 100;
        public double DEFAULT_WIDTH
        {
            get { return m_DEFAULT_WIDTH; }
            set { m_DEFAULT_WIDTH = value; }
        }
        
        double m_DEFAULT_HEIGHT = 22;
        public double DEFAULT_HEIGHT
        {
            get { return m_DEFAULT_HEIGHT; }
            set { m_DEFAULT_HEIGHT = value; }
        }
        
        public const int SPLITTER_SIZE = 3;

        double m_MIN_WIDTH = 38;
        public double MIN_WIDTH
        {
            get { return m_MIN_WIDTH; }
            set { m_MIN_WIDTH = value; }
        }

        double m_MIN_HEIGHT = 20;
        public double MIN_HEIGHT
        {
            get { return m_MIN_HEIGHT; }
            set { m_MIN_HEIGHT = value; }
        }

        public double DRILLDOWN_SPACE_HEIGHT
        {
            get { return DEFAULT_HEIGHT; }
        }

        double m_DRILLDOWN_SPACE_WIDTH = 22;
        public double DRILLDOWN_SPACE_WIDTH
        {
            get { return m_DRILLDOWN_SPACE_WIDTH; }
            set { m_DRILLDOWN_SPACE_WIDTH = value; }
        }

        Grid ItemsLayoutRoot;
        ScrollBar m_HorizontalScroll;
        ScrollBar m_VerticalScroll;
        
        Cache2D<CellControl> m_CellControls_Cache = new Cache2D<CellControl>();
        Grid LayoutRoot;
        
        Brush m_MembersBackground = null;
        internal Brush MembersBackground
        {
            get { return m_MembersBackground; }
        }

        PivotQueryManager m_QueryManager = null;
        public PivotQueryManager QueryManager
        {
            get { return m_QueryManager; }
            set { m_QueryManager = value; }
        }

        Brush m_CellsBorder = null;
        internal Brush CellsBorder
        {
            get { return m_CellsBorder; }
        }

        Brush m_CellsBackground = null;
        internal Brush CellsBackground
        {
            get { return m_CellsBackground; }
        }

        Brush m_MembersBorder = null;
        internal Brush MembersBorder
        {
            get { return m_MembersBorder; }
        }

        double m_DefaultFontSize = 11;
        public double DefaultFontSize
        {
            get { return m_DefaultFontSize; }
            set { m_DefaultFontSize = value; }
        }

        public PivotGridControl()
        {
            IsTabStop = true;

            LayoutRoot = new Grid();
            LayoutRoot.RowDefinitions.Add(new RowDefinition());
            LayoutRoot.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition());
            LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

            StyleContainer styleContainer = new StyleContainer();

            m_HorizontalScroll = new ScrollBar();
            m_HorizontalScroll.Orientation = Orientation.Horizontal;
            LayoutRoot.Children.Add(m_HorizontalScroll);
            Grid.SetColumn(m_HorizontalScroll, 0);
            Grid.SetRow(m_HorizontalScroll, 1);
            m_HorizontalScroll.SmallChange = 1;
            m_HorizontalScroll.LargeChange = 1;
            m_HorizontalScroll.Minimum = 0;
            m_HorizontalScroll.Maximum = 0;
            m_HorizontalScroll.Scroll += new ScrollEventHandler(m_HorizontalScroll_Scroll);
            m_HorizontalScroll.ValueChanged += new RoutedPropertyChangedEventHandler<double>(m_HorizontalScroll_ValueChanged);
            m_HorizontalScroll.Visibility = Visibility.Collapsed;

            m_VerticalScroll = new ScrollBar();
            m_VerticalScroll.Orientation = Orientation.Vertical;
            LayoutRoot.Children.Add(m_VerticalScroll);
            Grid.SetColumn(m_VerticalScroll, 1);
            Grid.SetRow(m_VerticalScroll, 0);
            m_VerticalScroll.SmallChange = 1;
            m_VerticalScroll.LargeChange = 1;
            m_VerticalScroll.Minimum = 0;
            m_VerticalScroll.Maximum = 0;
            m_VerticalScroll.Scroll += new ScrollEventHandler(m_VerticalScroll_Scroll);
            m_VerticalScroll.ValueChanged += new RoutedPropertyChangedEventHandler<double>(m_VerticalScroll_ValueChanged);
            m_VerticalScroll.Visibility = Visibility.Collapsed;

            if (styleContainer.Resources != null &&
                styleContainer.Resources.Contains("ScrollBarGlowStyle"))
            {
                m_HorizontalScroll.Style = styleContainer.Resources["ScrollBarGlowStyle"] as Style;
                m_VerticalScroll.Style = styleContainer.Resources["ScrollBarGlowStyle"] as Style;
            }

            ItemsLayoutRoot = new Grid();
            // Чтобы работало колесо мыши льём цветом
            ItemsLayoutRoot.Background = new SolidColorBrush(Colors.White);
            LayoutRoot.Children.Add(ItemsLayoutRoot);
            this.Content = LayoutRoot;

            m_VericalScroll_Timer = new Storyboard();
            m_VericalScroll_Timer.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 250));
            m_VericalScroll_Timer.Completed += new EventHandler(m_VericalScroll_Timer_Completed);
            LayoutRoot.Resources.Add("m_VericalScroll_Timer", m_VericalScroll_Timer);

            m_HorizontalScroll_Timer = new Storyboard();
            m_HorizontalScroll_Timer.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 250));
            m_HorizontalScroll_Timer.Completed += new EventHandler(m_HorizontalScroll_Timer_Completed);
            LayoutRoot.Resources.Add("m_HorizontalScroll_Timer", m_HorizontalScroll_Timer);

            GradientStopCollection stops = new GradientStopCollection();
            GradientStop stop0 = new GradientStop();
            stop0.Color = Colors.White;
            GradientStop stop1 = new GradientStop();
            stop1.Color = Colors.LightGray;
            stop1.Offset = 1;
            stops.Add(stop0);
            stops.Add(stop1);
            m_MembersBackground = new LinearGradientBrush(stops, 90);

            m_CellsBackground = new SolidColorBrush(Colors.White);
            m_CellsBorder = new SolidColorBrush(Colors.DarkGray);

            m_MembersBorder = new SolidColorBrush(Colors.DarkGray);

            //ItemsLayoutRoot.AttachContextMenu(p => GetCurrentContextMenu(p));
            UseContextMenu = true;

            this.KeyDown += new KeyEventHandler(SpanCellsAreaControl_KeyDown);
            ItemsLayoutRoot.KeyDown += new KeyEventHandler(SpanCellsAreaControl_KeyDown);
            this.GotFocus += new RoutedEventHandler(PivotGridControl_GotFocus);

            ItemsLayoutRoot.MouseEnter += new MouseEventHandler(CellsAreaControl_MouseEnter);
            ItemsLayoutRoot.MouseLeave += new MouseEventHandler(CellsAreaControl_MouseLeave);
            SelectionManager.SelectionChanged += new EventHandler(SelectionManager_SelectionChanged);

            m_Refresh_Timer = new Storyboard();
            m_Refresh_Timer.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 50));
            m_Refresh_Timer.Completed += new EventHandler(m_Refresh_Timer_Completed);
            LayoutRoot.Resources.Add("m_Refresh_Timer", m_Refresh_Timer);

            this.SizeChanged += new SizeChangedEventHandler(PivotGridControl_SizeChanged);

            m_VericalMouseWhellSupport = new ScrollBarMouseWheelSupport();
            m_VericalMouseWhellSupport.AddMouseWheelSupport(m_VerticalScroll);

            m_HorizontalMouseWhellSupport = new ScrollBarMouseWheelSupport() { IsHorizontal = true };
            m_HorizontalMouseWhellSupport.AddMouseWheelSupport(m_HorizontalScroll);

            TooltipManager = new TooltipController(this);
            TooltipManager.BeforeOpen += new EventHandler<CustomEventArgs<Point>>(TooltipManager_BeforeOpen);
            TooltipManager.ToolTipContent = new ToolTipControl();
        }

        void TooltipManager_BeforeOpen(object sender, CustomEventArgs<Point> e)
        {
            var tooltip = TooltipManager.ToolTipContent as ToolTipControl;
            if (tooltip != null)
            {
                // Определяем объект, который находится по текущим координатам
                if (AgControlBase.GetSLBounds(this).Contains(e.Args))
                {
                    PivotGridItem item_Control = PivotGridItem.GetPivotGridItem(e.Args);
                    if (item_Control != null)
                    {
                        if ((item_Control is RowMemberControl && Rows_UseHint) ||
                            (item_Control is ColumnMemberControl && Columns_UseHint) ||
                            (item_Control is CellControl && Cells_UseHint))
                        {
                            tooltip.Initialize(item_Control);
                            return;
                        }
                    }
                }
            }
            e.Cancel = true;
        }

        bool m_UseContextMenu = true;
        public bool UseContextMenu
        {
            get { return m_UseContextMenu; }
            set 
            {
                m_UseContextMenu = value;
                if (value)
                {
                    ItemsLayoutRoot.AttachContextMenu(p => GetCurrentContextMenu(p));
                }
                else
                {
                    ItemsLayoutRoot.DetachContextMenu();
                }
            }
        }

        bool m_DrillThroughCells = true;
        /// <summary>
        /// Использовать DrillThrough для ячеек
        /// </summary>
        public bool DrillThroughCells
        {
            get { return m_DrillThroughCells; }
            set { m_DrillThroughCells = value; }
        }

        internal readonly TooltipController TooltipManager = null;
        ScrollBarMouseWheelSupport m_VericalMouseWhellSupport;
        ScrollBarMouseWheelSupport m_HorizontalMouseWhellSupport;

        void m_HorizontalScroll_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            OnHorizontalScroll_Scroll();
        }

        void m_VerticalScroll_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            OnVerticalScroll_Scroll();
        }

        void m_Refresh_Timer_Completed(object sender, EventArgs e)
        {
            Refresh(RefreshType.BuildEndRefresh);
        }

        /// <summary>
        /// Таймер для обновления
        /// </summary>
        Storyboard m_Refresh_Timer;

        void PivotGridControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            m_Refresh_Timer.Stop();
            m_Refresh_Timer.Begin();
        }

        void SelectionManager_SelectionChanged(object sender, EventArgs e)
        {
            RefreshSelectedCells();
        }

        public void ChangeCell(CellInfo cell, UpdateEntry entry)
        {
            if (cell != null)
            {
                if (m_CellControls_Dict.ContainsKey(cell))
                {
                    CellControl cell_control = m_CellControls_Dict[cell];
                    if (cell_control != null)
                    {
                        cell_control.NotRecalculatedChange = entry;
                    }
                }
            }
        }

        void PivotGridControl_GotFocus(object sender, RoutedEventArgs e)
        {
            
        }

        #region Свойства

        double m_Scale = 1;
        public double Scale
        {
            get { return m_Scale; }
            set {
                double delta_scale = value - m_Scale;
                if (delta_scale != 0)
                {
                    double OldScale = m_Scale;
                    double NewScale = value;
                    // Это изменение происходит относительное 1, т.е. относительно 100 процентов
                    //delta_scale = 1 + delta_scale;

                    // Размеры старого масштаба принимаем за 100 %
                    // И пересчитываем размеры к новому масштабу
                    // OldSize  ->  OldScale
                    // X        ->  NewScale
                    // X = OldSize * NewScale/OldScale;
                    
                    double scale_coeff = NewScale / OldScale;

                    Dictionary<int, double> tmp = new Dictionary<int, double>();
                    foreach (KeyValuePair<int, double> pair in m_Rows_ColumnWidthes)
                    {
                        tmp.Add(pair.Key, pair.Value);
                    }

                    double min = 0.1;
                    m_Scale = value;
                    foreach (int key in tmp.Keys)
                    {
                        double v = tmp[key] * scale_coeff;
                        if (v == 0)
                            v = min;
                        m_Rows_ColumnWidthes[key] = v;
                    }

                    tmp.Clear();
                    foreach (KeyValuePair<int, double> pair in m_Columns_RowsHeightes)
                    {
                        tmp.Add(pair.Key, pair.Value);
                    }
                    foreach (int key in tmp.Keys)
                    {
                        double v = tmp[key] * scale_coeff;
                        if (v == 0)
                            v = min;
                        m_Columns_RowsHeightes[key] = v;
                    }

                    Dictionary<String, double> tmp1 = new Dictionary<String, double>();
                    foreach (KeyValuePair<String, double> pair in m_MembersWidthes)
                    {
                        tmp1.Add(pair.Key, pair.Value);
                    }
                    foreach (String key in tmp1.Keys)
                    {
                        double v = tmp1[key] * scale_coeff;
                        if (v == 0)
                            v = min;
                        m_MembersWidthes[key] = v;
                    }
                    tmp1.Clear();
                    foreach (KeyValuePair<String, double> pair in m_MembersHeightes)
                    {
                        tmp1.Add(pair.Key, pair.Value);
                    }
                    foreach (String key in tmp1.Keys)
                    {
                        double v = tmp1[key] * scale_coeff;
                        if (v == 0)
                            v = min;
                        m_MembersHeightes[key] = v;
                    }


                    //Refresh(RefreshType.BuildEndRefresh);
                    //ScaleTransform transform = new ScaleTransform();
                    //transform.ScaleX = transform.ScaleY = value;
                    //ItemsLayoutRoot.Height = (LayoutRoot.ActualHeight - m_HorizontalScroll.ActualHeight) / value;
                    //ItemsLayoutRoot.UpdateLayout();
                    //ItemsLayoutRoot.RenderTransform = transform;
                    ////ItemsLayoutRoot.Height = ItemsLayoutRoot.ActualHeight / value;
                    //ItemsLayoutRoot.UpdateLayout();

                    // Если коллекцию m_MemberControls_Dict не чистить, то элементы не будут пересоздаваться и при масштабировании возникнет проблема 
                    // с вычеслениями при определении необходимости отображения "..."
                    ColumnsArea_LovestMemberControls.Clear();
                    RowsArea_LovestMemberControls.Clear();
                    m_MemberControls_Dict.Clear();
                    Refresh(RefreshType.BuildEndRefresh);
                }
            }
        }

        MemberVisualizationTypes m_MemberVisualizationType = MemberVisualizationTypes.Caption;
        public MemberVisualizationTypes MemberVisualizationType
        {
            get { return m_MemberVisualizationType; }
            set {
                m_MemberVisualizationType = value;
                Refresh(RefreshType.Refresh);
            }
        }

        bool m_IsUpdateable = false;
        /// <summary>
        /// Определяет является ли сводная таблица редактируемой
        /// </summary>
        public bool IsUpdateable
        {
            get { 
                return m_IsUpdateable; 
            }
            set { m_IsUpdateable = value; }
        }

        /// <summary>
        /// Определяет может ли таблица редактироваться. Редактируемой она будет только если редактирование разрешено. И задан скрипт обновления!
        /// </summary>
        public bool CanEdit
        {
            get { return IsUpdateable & !String.IsNullOrEmpty(UpdateScript); }
        }

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

        String m_UpdateScript = String.Empty;
        public String UpdateScript 
        {
            get { return m_UpdateScript; }
            set { m_UpdateScript = value; } 
        }

        bool m_Axis0_IsInteractive = true;
        /// <summary>
        /// Определяет возможность использования Expand, Collapse, DrillDown для оси 0
        /// </summary>
        public bool Axis0_IsInteractive
        {
            get { return m_Axis0_IsInteractive; }
            set { m_Axis0_IsInteractive = value; }
        }

        bool m_Axis1_IsInteractive = true;
        /// <summary>
        /// Определяет возможность использования Expand, Collapse, DrillDown для оси 1
        /// </summary>
        public bool Axis1_IsInteractive
        {
            get { return m_Axis1_IsInteractive; }
            set { m_Axis1_IsInteractive = value; }
        }

        /// <summary>
        /// Определяет возможность использования Expand, Collapse, DrillDown для колонок с учетом возможности поворота осей.
        /// </summary>
        public bool Columns_IsInteractive
        {
            get
            {
                if (AxisIsRotated)
                {
                    return Axis1_IsInteractive;
                }
                else
                {
                    return Axis0_IsInteractive;
                }
            }
        }

        /// <summary>
        /// Определяет возможность использования Expand, Collapse, DrillDown для строк с учетом возможности поворота осей.
        /// </summary>
        public bool Rows_IsInteractive
        {
            get
            {
                if (AxisIsRotated)
                {
                    return Axis0_IsInteractive;
                }
                else
                {
                    return Axis1_IsInteractive;
                }
            }
        }

        bool m_Cells_UseHint = true;
        /// <summary>
        /// Определяет необходимость отображения всплывающей подсказки
        /// </summary>
        public bool Cells_UseHint
        {
            get { return m_Cells_UseHint; }
            set { m_Cells_UseHint = value; }
        }

        bool m_Columns_UseHint = true;
        /// <summary>
        /// Определяет необходимость отображения всплывающей подсказки
        /// </summary>
        public bool Columns_UseHint
        {
            get { return m_Columns_UseHint; }
            set { m_Columns_UseHint = value; }
        }

        bool m_Rows_UseHint = true;
        /// <summary>
        /// Определяет необходимость отображения всплывающей подсказки
        /// </summary>
        public bool Rows_UseHint
        {
            get { return m_Rows_UseHint; }
            set { m_Rows_UseHint = value; }
        }

        IList<CellConditionsDescriptor> m_CustomCellsConditions = null;
        public IList<CellConditionsDescriptor> CustomCellsConditions
        {
            get { return m_CustomCellsConditions; }
            set { m_CustomCellsConditions = value; }
        }
        #endregion Свойства

        #region События         
        public event MemberActionEventHandler ExecuteMemberAction;
        protected void Raise_ExecuteMemberAction(MemberActionEventArgs args)
        {
            MemberActionEventHandler handler = this.ExecuteMemberAction;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        public event EventHandler<ControlActionEventArgs<CellInfo>> Cells_PerformControlAction;
        void Raise_PerformControlAction(ControlActionType action, CellInfo cell)
        {
            EventHandler<ControlActionEventArgs<CellInfo>> handler = Cells_PerformControlAction;
            if (handler != null)
            {
                handler(this, new ControlActionEventArgs<CellInfo>(action, cell));
            }
        }

        public event EventHandler<ControlActionEventArgs<MemberControl>> Members_PerformControlAction;
        void Raise_PerformControlAction(ControlActionType action, MemberControl info)
        {
            EventHandler<ControlActionEventArgs<MemberControl>> handler = Members_PerformControlAction;
            if (handler != null)
            {
                handler(this, new ControlActionEventArgs<MemberControl>(action, info));
            }
        }

        public event FocusedCellChangedEventHandler FocusedCellChanged;
        private void Raise_FocusedCellChanged(CellControl oldFocusedCell, CellControl newFocusedCell)
        {
            FocusedCellChangedEventHandler handler = this.FocusedCellChanged;
            if (handler != null)
            {
                handler(this, new FocusedCellChangedEventArgs(this, oldFocusedCell, newFocusedCell));
            }
        }

        public event CellValueChangedEventHandler CellValueChanged;
        private void Raise_CellValueChanged(List<UpdateEntry> changes)
        {
            CellValueChangedEventHandler handler = this.CellValueChanged;
            if (handler != null)
            {
                handler(this, new CellValueChangedEventArgs(changes));
            }
        }

        public event EventHandler UndoCellChanges;
        private void Raise_UndoCellChanges()
        {
            EventHandler handler = this.UndoCellChanges;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        public event EventHandler Rows_ContextMenuCreated;
        protected void Raise_Rows_ContextMenuCreated()
        {
            EventHandler handler = this.Rows_ContextMenuCreated;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        public event EventHandler Columns_ContextMenuCreated;
        protected void Raise_Columns_ContextMenuCreated()
        {
            EventHandler handler = this.Columns_ContextMenuCreated;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        public event EventHandler Cells_ContextMenuCreated;
        protected void Raise_Cells_ContextMenuCreated()
        {
            EventHandler handler = this.Cells_ContextMenuCreated;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
        #endregion События

        #region Скроллинг
        Storyboard m_VericalScroll_Timer;
        Storyboard m_HorizontalScroll_Timer;
        private void m_VericalScroll_Timer_Completed(object sender, EventArgs e)
        {
            //CellControl focusedCell = FocusedCell;
            Refresh(RefreshType.RefreshByRows);
            //InitializeFocusedCell(focusedCell);
        }

        private void m_HorizontalScroll_Timer_Completed(object sender, EventArgs e)
        {
            //CellControl focusedCell = FocusedCell;
            Refresh(RefreshType.RefreshByColumns);
            //InitializeFocusedCell(focusedCell);
        }

        void m_VerticalScroll_Scroll(object sender, ScrollEventArgs e)
        {
            OnVerticalScroll_Scroll();
        }

        void OnVerticalScroll_Scroll()
        {
            int val = RowsArea_FirstVisible_Coordinate.Row;
            try
            {
                val = Convert.ToInt32(m_VerticalScroll.Value);
            }
            catch
            {
                return;
            }
            if (val != RowsArea_FirstVisible_Coordinate.Row)
            {
                m_VericalScroll_Timer.Stop();
                RowsArea_FirstVisible_Coordinate.Row = val;
                m_VericalScroll_Timer.Begin();
            }     
        }

        void m_HorizontalScroll_Scroll(object sender, ScrollEventArgs e)
        {
            OnHorizontalScroll_Scroll();
        }

        void OnHorizontalScroll_Scroll()
        {
            int val = ColumnsArea_FirstVisible_Coordinate.Column;
            try
            {
                val = Convert.ToInt32(m_HorizontalScroll.Value);
            }
            catch
            {
                return;
            }
            if (val != ColumnsArea_FirstVisible_Coordinate.Column)
            {
                m_HorizontalScroll_Timer.Stop();
                ColumnsArea_FirstVisible_Coordinate.Column = val;
                m_HorizontalScroll_Timer.Begin();
            }
        }
        #endregion Скроллинг

        #region Редактирование ячейки

        #region Редактор ячейки
        // Собственно редактор ячейки
        ICustomCellEditor m_CellEditor = null;
        ICustomCellEditor CellEditor
        {
            get
            {
                if (m_CellEditor == null)
                {
                    m_CellEditor = new TextBoxCellEditor();
                    m_CellEditor.PivotGridEditorCancelEdit += new EventHandler(CellEditor_PivotGridEditorCancelEdit);
                    m_CellEditor.PivotGridEditorEndEdit += new EventHandler(CellEditor_PivotGridEditorEndEdit);
                }
                m_CellEditor.Editor.BorderThickness = new Thickness(1 * Scale);
                m_CellEditor.Editor.Padding = new Thickness(2 * Scale, Scale, 2 * Scale, Scale);
                m_CellEditor.Editor.FontSize = DefaultFontSize * Scale;
                return m_CellEditor;
            }
        }

        void CellEditor_PivotGridEditorEndEdit(object sender, EventArgs e)
        {
            EndEdit();

            KeyEventArgs arg = e as KeyEventArgs;
            if (arg != null && arg.Key == Key.Enter)
            {
                this.SpanCellsAreaControl_KeyDown(this, arg);
            }
        }

        void CellEditor_PivotGridEditorCancelEdit(object sender, EventArgs e)
        {
            CancelEdit();
        }
        #endregion Редактор ячейки

        bool m_EditMode = false;
        /// <summary>
        /// Включен ли режим редактирования в данный момент
        /// </summary>
        public bool EditMode
        {
            get
            {
                return m_EditMode;
            }
            set
            {
                m_EditMode = value;
            }
        }

        private void BeginEdit()
        {
            BeginEdit(null);
        }

        private void BeginEdit(String cellEditorText)
        {
            // TODO: Редактирование наверное не надо начинать пока идет запись предыдущей
            //if (PivotGrid.SpanPivotGrid.IsWaiting)
            //    return;

            if (EditMode && FocusedCell != null && FocusedCell.Cell != null && FocusedCell.Cell.IsUpdateable)
            {
                if (cellEditorText == null)
                {
                    if (FocusedCell.NotRecalculatedChange != null)
                    {
                         CellEditor.Value = FocusedCell.NotRecalculatedChange.NewValue;
                    }
                    else
                    {
                        CellEditor.Value = FocusedCell.Cell.ValueToString;
                    }
                }
                else
                {
                    CellEditor.Value = cellEditorText == null ? String.Empty : cellEditorText;
                }

                int layout_row_index = -1;
                int layout_column_index = -1;
                m_CellControls_Cache.GetCoordinates(FocusedCell, out layout_column_index, out layout_row_index);

                if (layout_column_index > -1 && layout_row_index > -1)
                {
                    // Прячем подсказку чтобы не мешала при редактировании
                    TooltipManager.Hide();

                    if (!ItemsLayoutRoot.Children.Contains(CellEditor.Editor))
                    {
                        ItemsLayoutRoot.Children.Add(CellEditor.Editor);
                    }
                    Grid.SetColumn(CellEditor.Editor, CellsArea_BeginColumnIndex + layout_column_index);
                    Grid.SetRow(CellEditor.Editor, CellsArea_BeginRowIndex + layout_row_index);
                }

                CellEditor.Editor.Focus();

                FocusedCell.IsEditing = true;
            }
        }

        /// <summary>
        /// Применяет изменения, внесенные в контрол редактирования
        /// </summary>
        internal void EndEdit()
        {
            // Проверяем что редактирование в принципе начато на данный момент
            if (FocusedCell != null && FocusedCell.IsEditing)
            {
                FocusedCell.IsEditing = false;

                if (m_CellEditor != null && FocusedCell.Cell != null)
                {
                    ItemsLayoutRoot.Children.Remove(m_CellEditor.Editor);

                    if (CellEditor.Value == null || String.IsNullOrEmpty(CellEditor.Value.ToString()))
                        return;

                    //// Проверяем, что произошло изменение значения
                    //if (FocusedCell.Cell.CellDescr.Value != null && FocusedCell.Cell.CellDescr.Value.Value != null)
                    //{
                    //    if (FocusedCell.Cell.CellDescr.Value.Value.ToString() == value)
                    //        return;
                    //}

                    String value = CellEditor.Value.ToString();
                    // Проверяем чтобы значение изменилось
                    if (FocusedCell.Cell.ValueToString != value)
                    {
                        var entry = new UpdateEntry(FocusedCell.Cell, value);
                        var changes = new List<UpdateEntry>();
                        changes.Add(entry);

                        FocusedCell.NotRecalculatedChange = entry;

                        // В качестве разделителя для числа обязательно должна использоватьеся точка (т.к. эта строка будет помещена в МDX)
                        value = value.Replace(System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencyDecimalSeparator, ".");

                        Raise_CellValueChanged(changes);
                    }
                }
            }

            this.Focus();
        }

        internal void CancelEdit()
        {
            // Проверяем что редактирование в принципе начато на данный момент
            if (FocusedCell != null)
            {
                FocusedCell.IsEditing = false;

                if (m_CellEditor != null)
                {
                    ItemsLayoutRoot.Children.Remove(m_CellEditor.Editor);
                }
            }

            this.Focus();
        }

        #endregion Редактирование ячейки

        CellSetDataProvider m_CellSetProvider;
        internal PivotLayoutProvider m_LayoutProvider;
        private enum RefreshType
        { 
            BuildEndRefresh,
            Refresh,
            RefreshByColumns,
            RefreshByRows
        }

        public void Refresh()
        {
            Refresh(RefreshType.BuildEndRefresh);
        }

        void Refresh(RefreshType type)
        {
            try
            {
                TooltipManager.IsPaused = true;

                System.Diagnostics.Debug.WriteLine("\r\nPivotGrid refresh started");
                if (FocusedCell != null && FocusedCell.IsEditing)
                    EndEdit();

                DateTime start = DateTime.Now;

                if (type == RefreshType.BuildEndRefresh)
                {
                    BuildAreasLayout();
                }

                if (type != RefreshType.RefreshByRows)
                    BuildColumnsArea();
                if (type != RefreshType.RefreshByColumns)
                    BuildRowsArea();

                BuildCellsArea();
                BuildSplitters();

                DateTime stop1 = DateTime.Now;
                System.Diagnostics.Debug.WriteLine(" Building time: " + (stop1 - start).ToString());

                if (m_LayoutProvider != null)
                {
                    if (type != RefreshType.RefreshByRows)
                    {
                        DateTime stop2_1 = DateTime.Now;
                        InitializeColumnsArea(m_LayoutProvider);
                        DateTime stop2_2 = DateTime.Now;
                        System.Diagnostics.Debug.WriteLine(" InitializeColumnsArea time: " + (stop2_2 - stop2_1).ToString());
                    }

                    if (type != RefreshType.RefreshByColumns)
                    {
                        DateTime stop3_1 = DateTime.Now;
                        InitializeRowsArea(m_LayoutProvider);
                        DateTime stop3_2 = DateTime.Now;
                        System.Diagnostics.Debug.WriteLine(" InitializeRowsArea time: " + (stop3_2 - stop3_1).ToString());
                    }

                    DateTime stop4_1 = DateTime.Now;
                    InitializeCellsArea(m_LayoutProvider);
                    DateTime stop4_2 = DateTime.Now;
                    System.Diagnostics.Debug.WriteLine(" InitializeCellsArea time: " + (stop4_2 - stop4_1).ToString());

                    InitializeSplitters();
                }

                DateTime stop = DateTime.Now;
                System.Diagnostics.Debug.WriteLine("PivotGrid refreshing time: " + (stop - start).ToString());
            }
            finally {
                TooltipManager.IsPaused = false;
            }
        }

        //void RefreshByColumnsArea()
        //{
        //    if(FocusedCell != null && FocusedCell.IsEditing)
        //        EndEdit();

        //    DateTime start = DateTime.Now;

        //    BuildColumnsArea();
        //    BuildCellsArea();
        //    BuildSplitters();

        //    System.Diagnostics.Debug.WriteLine("");
        //    DateTime stop1 = DateTime.Now;
        //    System.Diagnostics.Debug.WriteLine("Building time: " + (stop1 - start).ToString());

        //    if (m_LayoutProvider != null)
        //    {
        //        InitializeColumnsArea(m_LayoutProvider);
        //        DateTime stop2 = DateTime.Now;
        //        System.Diagnostics.Debug.WriteLine("InitializeColumnsArea time: " + (stop2 - stop1).ToString());

        //        InitializeCellsArea(m_LayoutProvider);
        //        DateTime stop4 = DateTime.Now;
        //        System.Diagnostics.Debug.WriteLine("InitializeCellsArea time: " + (stop4 - stop2).ToString());

        //        InitializeSplitters();
        //    }

        //    DateTime stop = DateTime.Now;
        //    System.Diagnostics.Debug.WriteLine("PivotGrid initializing time: " + (stop - start).ToString());
        //}

        //void RefreshByRowsArea()
        //{
        //    if (FocusedCell != null && FocusedCell.IsEditing)
        //        EndEdit();

        //    DateTime start = DateTime.Now;

        //    BuildRowsArea();
        //    BuildCellsArea();
        //    BuildSplitters();

        //    System.Diagnostics.Debug.WriteLine("");
        //    DateTime stop1 = DateTime.Now;
        //    System.Diagnostics.Debug.WriteLine("Building time: " + (stop1 - start).ToString());

        //    if (m_LayoutProvider != null)
        //    {
        //        InitializeRowsArea(m_LayoutProvider);
        //        DateTime stop2 = DateTime.Now;
        //        System.Diagnostics.Debug.WriteLine("InitializeRowsArea time: " + (stop2 - stop1).ToString());

        //        InitializeCellsArea(m_LayoutProvider);
        //        DateTime stop4 = DateTime.Now;
        //        System.Diagnostics.Debug.WriteLine("InitializeCellsArea time: " + (stop4 - stop2).ToString());

        //        InitializeSplitters();
        //    }

        //    DateTime stop = DateTime.Now;
        //    System.Diagnostics.Debug.WriteLine("PivotGrid initializing time: " + (stop - start).ToString());
        //}

        PivotDataAnalizer m_AnalyticInfo = null;

        public void Initialize(CellSetDataProvider provider)
        {
            // Ячейка с фокусом содержит элемент из предыдущего CellSetDataProvider. По таплу ныжно найти ей соответствие в новом CellSetDataProvider
            if (FocusedCellView != null)
            {
                if (provider != null)
                {
                    CellInfo info = provider.GetCellInfo(FocusedCellView.CellDescr.Axis0_Coord, FocusedCellView.CellDescr.Axis1_Coord);
                    IDictionary<String, MemberInfo> tuple = FocusedCellView.GetTuple();
                    if(info != null && info.CompareByTuple(tuple))
                    {
                        // Соответствие сразу найдено (в идеале)
                    }
                    else
                    {
                        info = provider.GetCellByTuple(tuple);
                    }
                    FocusedCellView = info;
                }
                else
                    FocusedCellView = null;
            }

            bool stick = false;
            // Пытаемся получить ячейку, которая в текущий момент верхняя левая
            // и сравнить ее с новой ячейкой с теми же координатами. Если ячейки совпадают, то позицию не обнуляем чтобы на экране осталась текущая область
            if (m_CellSetProvider != null &&
                m_CellSetProvider.CellSet_Description != null)
            {
                CellInfo cell_old = m_CellSetProvider.GetCellInfo(ColumnsArea_FirstVisible_Coordinate.Column, RowsArea_FirstVisible_Coordinate.Row);
                if (cell_old != null)
                {
                    if (provider != null &&
                        provider.CellSet_Description != null)
                    {
                        CellInfo cell_new = provider.GetCellInfo(ColumnsArea_FirstVisible_Coordinate.Column, RowsArea_FirstVisible_Coordinate.Row);
                        if (cell_new != null)
                        { 
                            // Сверяем по координатам
                            stick = cell_new.CompareByTuple(cell_old.GetTuple());
                        }
                    }
                }
            }

            //// Если в старой позиции уже находится другая ячейка, то отображаем ячейку с фокусом
            //if (!stick)
            //{
            //    GoToFocusedCell();
            //    //ColumnsArea_FirstVisible_Coordinate.Column = 0;
            //    //RowsArea_FirstVisible_Coordinate.Row = 0;
            //    //m_VerticalScroll.Value = 0;
            //    //m_HorizontalScroll.Value = 0;
            //}

            m_SelectionManager.ClearSelection();

            //m_MembersWidthes.Clear();
            //m_MembersHeightes.Clear();

            ColumnsArea_LovestMemberControls.Clear();
            RowsArea_LovestMemberControls.Clear();
            m_MemberControls_Dict.Clear();

            DateTime start = DateTime.Now;
            //System.Diagnostics.Debug.WriteLine("PivotGrid initializing start: " + start.TimeOfDay.ToString());

            bool new_OlapData = m_CellSetProvider != provider;
            m_CellSetProvider = provider;
            m_LayoutProvider = null;

            if (provider != null)
            {
                PivotDataProvider pivotProvider = new PivotDataProvider(provider);
                m_LayoutProvider = new PivotLayoutProvider(pivotProvider);
            }

            if (m_AnalyticInfo == null)
                m_AnalyticInfo = new PivotDataAnalizer(this);
            else
            {
                m_AnalyticInfo.ClearMembersAnalytic();
                if (new_OlapData)
                    m_AnalyticInfo.BuildCellsAnalytic();
            }

            Refresh(RefreshType.BuildEndRefresh);

            // Если в старой позиции уже находится другая ячейка, то отображаем ячейку с фокусом
            if (!stick)
            {
                GoToFocusedCell();
            }
            DateTime stop = DateTime.Now;
            //System.Diagnostics.Debug.WriteLine("PivotGrid initializing stop: " + stop.TimeOfDay.ToString());
            //System.Diagnostics.Debug.WriteLine("PivotGrid initializing time: " + (stop - start).ToString());
        }

        int CellsArea_BeginColumnIndex
        {
            get { return PivotArea_BeginColumnIndex + m_RowsArea_ColumnsCount; }
        }

        int CellsArea_BeginRowIndex
        {
            get { return PivotArea_BeginRowIndex + m_ColumnsArea_RowsCount; }
        }

        int m_PivotArea_BeginRowIndex = 0;
        int PivotArea_BeginRowIndex
        {
            get { return m_PivotArea_BeginRowIndex; }
        }
        
        int m_PivotArea_BeginColumnIndex = 0;
        int PivotArea_BeginColumnIndex
        {
            get { return m_PivotArea_BeginColumnIndex; }
        }

        int RowsArea_BeginRowIndex
        {
            get { return CellsArea_BeginRowIndex; }
        }

        int RowsArea_EndRowIndex
        {
            get { 
                if(RowsArea_RowsCount > 0)
                    return CellsArea_BeginRowIndex + RowsArea_RowsCount - 1; 
                else
                    return CellsArea_BeginRowIndex; 
            }
        }

        int RowsArea_EndColumnIndex
        {
            get { return m_RowsArea_ColumnsCount > 0 ? PivotArea_BeginColumnIndex + m_RowsArea_ColumnsCount - 1 : PivotArea_BeginColumnIndex; }
        }

        int ColumnsArea_BeginColumnIndex
        {
            get { return CellsArea_BeginColumnIndex; }
        }

        int ColumnsArea_EndColumnIndex
        {
            get { 
                if(ColumnsArea_ColumnsCount > 0)
                    return ColumnsArea_BeginColumnIndex + ColumnsArea_ColumnsCount - 1;
                else
                    return ColumnsArea_BeginColumnIndex; 
            }
        }

        int ColumnsArea_EndRowIndex
        {
            get
            {
                if (ColumnsArea_RowsCount > 0)
                    return PivotArea_BeginRowIndex + ColumnsArea_RowsCount - 1;
                else
                    return PivotArea_BeginRowIndex;
            }
        }

        int m_ColumnsArea_ColumnsCount = 0;
        int ColumnsArea_ColumnsCount
        {
            get { return m_ColumnsArea_ColumnsCount; }
        }
        
        int m_ColumnsArea_RowsCount = 0;
        int ColumnsArea_RowsCount
        {
            get { return m_ColumnsArea_RowsCount; }
        }

        int m_RowsArea_ColumnsCount = 0;
        int RowsArea_ColumnsCount
        {
            get { return m_RowsArea_ColumnsCount; }
        }

        int m_RowsArea_RowsCount = 0;
        int RowsArea_RowsCount
        {
            get { return m_RowsArea_RowsCount; }
        }



        double GetAreaWidth(int beginColumnIndex, int endColumnIndex)
        {
            double res = 0;
            if (beginColumnIndex <= endColumnIndex && endColumnIndex < ItemsLayoutRoot.ColumnDefinitions.Count)
            {
                for (int i = beginColumnIndex; i <= endColumnIndex; i++)
                {
                    ColumnDefinition current_column = ItemsLayoutRoot.ColumnDefinitions[i];
                    res += current_column.Width.Value;
                }
            }
            return res;
        }

        double GetAreaHeight(int beginRowIndex, int endRowIndex)
        {
            double res = 0;
            if (beginRowIndex <= endRowIndex && endRowIndex < ItemsLayoutRoot.RowDefinitions.Count)
            {
                for (int i = beginRowIndex; i <= endRowIndex; i++)
                {
                    RowDefinition current_row = ItemsLayoutRoot.RowDefinitions[i];
                    res += current_row.Height.Value;
                }
            }
            return res;
        }


        #region Коллекции для хранения информации о размерах элементов
        Dictionary<int, double> m_Rows_ColumnWidthes = new Dictionary<int, double>();
        Dictionary<int, double> m_Columns_RowsHeightes = new Dictionary<int, double>();
        
        Dictionary<String, double> m_MembersWidthes = new Dictionary<string, double>();
        double GetMaxWidth()
        {
            double res = Math.Round(DEFAULT_WIDTH * Scale);
            foreach (double val in m_MembersWidthes.Values)
            {
                res = Math.Max(val, res);
            }
            return res;
        }

        Dictionary<String, double> m_MembersHeightes = new Dictionary<string, double>();
        double GetMaxHeight()
        {
            double res = Math.Round(DEFAULT_HEIGHT * Scale);
            foreach (double val in m_MembersHeightes.Values)
            {
                res = Math.Max(val, res);
            }
            return res;
        }
        #endregion Коллекции для хранения информации о размерах элементов

        /// <summary>
        /// Видимая ширина области колонок
        /// </summary>
        double Columns_VisibleWidth
        {
            get {
                // Ширина области строк
                double rows_area_width = 0;
                if (RowsArea_ColumnsCount > 0)
                {
                    rows_area_width = GetAreaWidth(PivotArea_BeginColumnIndex, RowsArea_EndColumnIndex);
                }
                // Ширина видимой области для колонок:
                // От ширины контрола нужно отнять ширину области строк и скроллера
                double scrollWidth = m_VerticalScroll.Visibility == Visibility.Visible ? m_VerticalScroll.ActualWidth : 0;
                return LayoutRoot.ActualWidth - rows_area_width - scrollWidth;
            }
        }

        /// <summary>
        /// Видимая высота области строк
        /// </summary>
        double Rows_VisibleHeight
        {
            get
            {
                // Высота области колонок
                double columns_area_height = 0;
                if (ColumnsArea_RowsCount > 0)
                {
                    columns_area_height = GetAreaHeight(PivotArea_BeginRowIndex, ColumnsArea_EndRowIndex);
                }
                // Ширина видимой области для строк:
                // От высоты контрола нужно отнять высоту области колонок и скроллера
                double scrollHeight = m_HorizontalScroll.Visibility == Visibility.Visible ? m_HorizontalScroll.ActualHeight : 0;
                return LayoutRoot.ActualHeight - columns_area_height - scrollHeight;
            }
        }

        #region Построение областей
        
        void BuildAreasLayout()
        {
            m_ColumnsArea_ColumnsCount = 0;
            m_ColumnsArea_RowsCount = 0;
            m_RowsArea_ColumnsCount = 0;
            m_RowsArea_RowsCount = 0;

            // Чистим кэш ячеек
            m_CellControls_Cache = new Cache2D<CellControl>();

            ItemsLayoutRoot.Children.Clear();
            ItemsLayoutRoot.ColumnDefinitions.Clear();
            ItemsLayoutRoot.RowDefinitions.Clear();
            m_Vertiacal_Splitters.Clear();
            m_Horizontal_Splitters.Clear();

            if (m_LayoutProvider != null)
            {
                PivotLayoutProvider layout = m_LayoutProvider;

                #region Область строк
                // Создаем колонки, которые будут отведены под область строк
                for (int i = 0; i < layout.RowsLayout.Columns_Size; i++)
                {
                    ColumnDefinition current_column = new ColumnDefinition() { Width = GridLength.Auto };
                    if (m_Rows_ColumnWidthes.ContainsKey(i))
                    {
                        current_column.Width = new GridLength(m_Rows_ColumnWidthes[i]);
                    }
                    ItemsLayoutRoot.ColumnDefinitions.Add(current_column);
                }

                // Настраиваем ширины колонок для области строк
                for (int i = 0; i < layout.RowsLayout.Columns_Size; i++)
                {
                    ColumnDefinition current_column = ItemsLayoutRoot.ColumnDefinitions[i + PivotArea_BeginColumnIndex];
                    int column_index_in_area = i;
                    Dictionary<int, int> drillDepth = layout.PivotProvider.RowsArea.DrillDepth;

                    if (AutoWidthColumns && m_AnalyticInfo != null)
                    {
                        // Расширяем колонку с учетом заглубления в ней
                        double width = (DRILLDOWN_SPACE_WIDTH * drillDepth[column_index_in_area] + m_AnalyticInfo.GetEstimatedColumnSizeForRowsArea(column_index_in_area) + 10 + 10 * Scale) * Scale;    // 10-для красоты, 10* - на плюсики
                        if (drillDepth.ContainsKey(column_index_in_area))
                            current_column.Width = new GridLength(Math.Round(Math.Max(DEFAULT_WIDTH * Scale, width)));
                        else
                            current_column.Width = new GridLength(Math.Round(DEFAULT_WIDTH * Scale));

                        if (drillDepth.ContainsKey(column_index_in_area))
                            current_column.MinWidth = Math.Round((DRILLDOWN_SPACE_WIDTH * drillDepth[column_index_in_area] + MIN_WIDTH) * Scale);
                        else
                            current_column.MinWidth = Math.Round(MIN_WIDTH * Scale);
                    }
                    else
                    {
                        // Вычисляем максимальное заглубление по данной колонке
                        if (drillDepth.ContainsKey(column_index_in_area))
                        {
                            // Расширяем колонку с учетом заглубления в ней
                            double width = (DEFAULT_WIDTH + DRILLDOWN_SPACE_WIDTH * drillDepth[column_index_in_area]) * Scale;
                            current_column.Width = new GridLength(Math.Max(current_column.Width.Value, width));
                            current_column.MinWidth = Math.Round((DRILLDOWN_SPACE_WIDTH * drillDepth[column_index_in_area] + MIN_WIDTH) * Scale);
                        }
                        else
                        {
                            current_column.Width = new GridLength(Math.Round(DEFAULT_WIDTH * Scale));
                            current_column.MinWidth = Math.Round(MIN_WIDTH * Scale);
                        }
                    }
                }
                #endregion Область строк

                #region Область колонок
                // Создаем строки, которые будут отверены под область колонок
                for (int i = 0; i < layout.ColumnsLayout.Rows_Size; i++)
                {
                    RowDefinition current_row = new RowDefinition() { Height = GridLength.Auto };
                    if (m_Columns_RowsHeightes.ContainsKey(i))
                    {
                        current_row.Height = new GridLength(m_Columns_RowsHeightes[i]);
                    }

                    ItemsLayoutRoot.RowDefinitions.Add(current_row);
                }

                // Настраиваем высоты строк для области колонок
                for (int i = 0; i < layout.ColumnsLayout.Rows_Size; i++)
                {
                    RowDefinition current_row = ItemsLayoutRoot.RowDefinitions[i + PivotArea_BeginRowIndex];
                    int row_index_in_area = i;
                    Dictionary<int, int> drillDepth = layout.PivotProvider.ColumnsArea.DrillDepth;
                    // Вычисляем максимальное заглубление по данной строке
                    if (drillDepth.ContainsKey(row_index_in_area))
                    {
                        // Делаем строку выше с учетом заглубления в ней
                        double height = (DEFAULT_HEIGHT + DRILLDOWN_SPACE_HEIGHT * drillDepth[row_index_in_area]) * Scale;
                        current_row.Height = new GridLength(Math.Round(Math.Max(current_row.Height.Value, height)));
                        current_row.MinHeight = Math.Round((DRILLDOWN_SPACE_HEIGHT * drillDepth[row_index_in_area] + MIN_HEIGHT) * Scale);
                    }
                    else
                    {
                        current_row.Height = new GridLength(Math.Round(DEFAULT_HEIGHT * Scale));
                        current_row.MinHeight = Math.Round(MIN_HEIGHT * Scale);
                    }
                }
                #endregion Область колонок

                // Число колонок в области строк и число строк в области колонок останутся неизменными при любых манипуляциях с данным представлением
                m_RowsArea_ColumnsCount = ItemsLayoutRoot.ColumnDefinitions.Count;
                m_ColumnsArea_RowsCount = ItemsLayoutRoot.RowDefinitions.Count;

                UpdateLayout();
            }
        }

        RowDefinition m_Fictive_Row = null;
        ColumnDefinition m_Fictive_Column = null;

        /// <summary>
        /// Настраивает область колонок. Создаются колонки и устанавливается их ширина
        /// </summary>
        void BuildColumnsArea()
        {
            DateTime start = DateTime.Now;

            // Сохраняем страе число колонок, чтобы после инициализации области удалить лишние, в случае если они будут.
            int old_ColumnsArea_ColumnsCount = ColumnsArea_ColumnsCount;

            m_ColumnsArea_ColumnsCount = 0;

            #region Настройка области колонок
            if (m_LayoutProvider != null)
            {
                // Удаляем фиктивную колонку
                if (m_Fictive_Column != null)
                    ItemsLayoutRoot.ColumnDefinitions.Remove(m_Fictive_Column);

                PivotLayoutProvider layout = m_LayoutProvider;

                // Создаем колонки для области колонок
                // После добавления каждой колонки проверяем суммарную ширину имеющихся колонок.
                // Если она превышает видимую часть то колонок больше не добавляем

                // Ширина самой широкой колонки
                double max_width = GetMaxWidth();

                int layout_column_index = ColumnsArea_BeginColumnIndex;
                int columnsCount = layout.ColumnsLayout.Columns_Size;
                // если в результате нет осей, а ячейки есть (select from [Adventure Works]), то они должны отображаться без области колонок
                if (columnsCount == 0 && layout.PivotProvider.Provider.CellSet_Description != null &&
                    layout.PivotProvider.Provider.CellSet_Description.Cells.Count > 0)
                {
                    columnsCount = layout.PivotProvider.Provider.CellSet_Description.Cells.Count;
                }

                for (int i = ColumnsArea_FirstVisible_Coordinate.Column, indx = 0; i < columnsCount; i++, indx++)
                {
                    // Пытаемся ПОЛУЧИТЬ данную колонку в гриде
                    ColumnDefinition current_column = null;
                    if (layout_column_index < ItemsLayoutRoot.ColumnDefinitions.Count)
                    {
                        current_column = ItemsLayoutRoot.ColumnDefinitions[layout_column_index];
                    }
                    else
                    {
                        // ДОБАВЛЯЕМ колонку
                        current_column = new ColumnDefinition() { MinWidth = Math.Round(MIN_WIDTH * Scale) };
                        ItemsLayoutRoot.ColumnDefinitions.Add(current_column);
                    }
                    m_ColumnsArea_ColumnsCount++;
                    layout_column_index++;

                    // Настраиваем ШИРИНУ колонки.
                    double size = GetEstimatedColumnSize(i);
                    if (size == 0)
                        size = DEFAULT_WIDTH * Scale;
                    size = Math.Round(size);
                    current_column.Width = new GridLength(size);

                    // Получаем СУММАРНУЮ ширину области с учетом текущей колонки
                    double width = GetAreaWidth(ColumnsArea_BeginColumnIndex, ColumnsArea_BeginColumnIndex + indx);
                    // Если вычисленная суммарная ширина превышает видимый размер + размер самой шировкой колонки, то больше добавлять колонки не нужно
                    // В данном случае учитываем размер самой широкой колонки чтобы в случае ее сужения не образовывалось пустых областей справа
                    if (width > Columns_VisibleWidth + max_width)
                    {
                        break;
                    }
                }

                // Колонки добавлены, ширины настроены
                // Теперь УДАЛЯЕМ лишние колонки если они вдруг появились
                if (old_ColumnsArea_ColumnsCount > ColumnsArea_ColumnsCount)
                {
                    List<ColumnDefinition> toDelete = new List<ColumnDefinition>();
                    for (int i = 0; i < old_ColumnsArea_ColumnsCount - ColumnsArea_ColumnsCount; i++)
                    {
                        if (layout_column_index + i < ItemsLayoutRoot.ColumnDefinitions.Count)
                        {
                            toDelete.Add(ItemsLayoutRoot.ColumnDefinitions[layout_column_index + i]);
                        }
                    }
                    foreach (ColumnDefinition current_column in toDelete)
                    {
                        ItemsLayoutRoot.ColumnDefinitions.Remove(current_column);
                    }
                }

                // Добавляем фиктивную колонку для изменения ширины самой последней колонки 
                if (m_Fictive_Column == null)
                {
                    m_Fictive_Column = new ColumnDefinition() { Width = GridLength.Auto };
                }
                ItemsLayoutRoot.ColumnDefinitions.Add(m_Fictive_Column);

                // Настраиваем скроллеры
                m_HorizontalScroll.LargeChange = Columns_VisibleColumnsCount;
                m_HorizontalScroll.Maximum = layout.ColumnsLayout.Columns_Size - Columns_LastPage_ColumnsCount;
                m_HorizontalScroll.ViewportSize = m_HorizontalScroll.SmallChange;
                // Прячем/отображаем
                if (Columns_VisibleColumnsCount < m_LayoutProvider.ColumnsLayout.Columns_Size)
                {
                    m_HorizontalScroll.Visibility = Visibility.Visible;
                }
                else
                {
                    m_HorizontalScroll.Visibility = Visibility.Collapsed;
                }
            }
            else 
            {
                m_HorizontalScroll.Visibility = Visibility.Collapsed;
            }
            #endregion Настройка области колонок

            DateTime stop = DateTime.Now;
            System.Diagnostics.Debug.WriteLine(" BuildColumnsArea time: " + (stop - start).ToString());
        }

        void BuildRowsArea()
        {
            DateTime start = DateTime.Now;

            // Сохраняем страе число строк, чтобы после инициализации области удалить лишние, в случае если они будут.
            int old_RowsArea_RowsCount = RowsArea_RowsCount;

            m_RowsArea_RowsCount = 0;

            #region Настройка области строк
            if (m_LayoutProvider != null)
            {
                // Удаляем фиктивную строку
                if (m_Fictive_Row != null)
                    ItemsLayoutRoot.RowDefinitions.Remove(m_Fictive_Row);

                PivotLayoutProvider layout = m_LayoutProvider;

                // Создаем строки для области строк
                // После добавления каждой строки проверяем суммарную высоту имеющихся строк.
                // Если она превышает видимую часть то строк больше не добавляем

                // Высота самой высокой строки
                double max_height = GetMaxHeight();

                int layout_row_index = RowsArea_BeginRowIndex;
                int rowsCount = layout.RowsLayout.Rows_Size;
                // если в результате только одна ось, ячейки есть. То они должны отображаться без области строк
                if (rowsCount == 0 && layout.PivotProvider.Provider.CellSet_Description != null &&
                    layout.PivotProvider.Provider.CellSet_Description.Cells.Count > 0)
                {
                    rowsCount = 1;
                }

                for (int i = RowsArea_FirstVisible_Coordinate.Row, indx = 0; i < rowsCount; i++, indx++)
                {
                    // Пытаемся ПОЛУЧИТЬ данную строку в гриде
                    RowDefinition current_row = null;
                    if (layout_row_index < ItemsLayoutRoot.RowDefinitions.Count)
                    {
                        current_row = ItemsLayoutRoot.RowDefinitions[layout_row_index];
                    }
                    else
                    {
                        // ДОБАВЛЯЕМ строку
                        current_row = new RowDefinition() { MinHeight = Math.Round(MIN_HEIGHT * Scale) };
                        ItemsLayoutRoot.RowDefinitions.Add(current_row);
                    }
                    m_RowsArea_RowsCount++;
                    layout_row_index++;

                    // Настраиваем ВЫСОТУ строки.
                    double size = GetEstimatedRowSize(i);
                    if (size == 0)
                        size = DEFAULT_HEIGHT * Scale;
                    size = Math.Round(size);
                    current_row.Height = new GridLength(size);

                    // Получаем СУММАРНУЮ высоту области с учетом текущей строки
                    double height = GetAreaHeight(RowsArea_BeginRowIndex, RowsArea_BeginRowIndex + indx);
                    // Если вычисленная суммарная высоту превышает видимый размер + размер самой высокой строки, то больше добавлять строк не нужно
                    // В данном случае учитываем размер самой высокой строки чтобы в случае ее сужения не образовывалось пустых областей снизу
                    if (height > Rows_VisibleHeight + max_height)
                    {
                        break;
                    }
                }

                // Строки добавлены, высоты настроены
                // Теперь УДАЛЯЕМ лишние строки если они вдруг появились
                if (old_RowsArea_RowsCount > RowsArea_RowsCount)
                {
                    List<RowDefinition> toDelete = new List<RowDefinition>();
                    for (int i = 0; i < old_RowsArea_RowsCount - RowsArea_RowsCount; i++)
                    {
                        if (layout_row_index + i < ItemsLayoutRoot.RowDefinitions.Count)
                        {
                            toDelete.Add(ItemsLayoutRoot.RowDefinitions[layout_row_index + i]);
                        }
                    }
                    foreach (RowDefinition current_row in toDelete)
                    {
                        ItemsLayoutRoot.RowDefinitions.Remove(current_row);
                    }
                }

                // Добавляем фиктивную строку для изменения высоты самой последней строки 
                if (m_Fictive_Row == null)
                {
                    m_Fictive_Row = new RowDefinition() { Height = GridLength.Auto };
                }
                ItemsLayoutRoot.RowDefinitions.Add(m_Fictive_Row);

                // Настраиваем скроллеры
                m_VerticalScroll.LargeChange = Rows_VisibleRowsCount;
                m_VerticalScroll.Maximum = layout.RowsLayout.Rows_Size - Rows_LastPage_ColumnsCount;
                m_VerticalScroll.ViewportSize = m_VerticalScroll.SmallChange;
                // Прячем/отображаем
                if (Rows_VisibleRowsCount < m_LayoutProvider.RowsLayout.Rows_Size)
                {
                    m_VerticalScroll.Visibility = Visibility.Visible;
                }
                else
                {
                    m_VerticalScroll.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                m_VerticalScroll.Visibility = Visibility.Collapsed;
            }
            #endregion Настройка области строк

            DateTime stop = DateTime.Now;
            System.Diagnostics.Debug.WriteLine(" BuildRowsArea time: " + (stop - start).ToString());
        }

        void BuildCellsArea()
        {
            DateTime start = DateTime.Now;

            #region Настройка области ячеек
            int added = 0;
            for (int layout_column_indx = ColumnsArea_BeginColumnIndex, column_indx = 0; layout_column_indx <= ColumnsArea_EndColumnIndex; layout_column_indx++, column_indx++)
            {
                for (int layout_row_indx = RowsArea_BeginRowIndex, row_indx = 0; layout_row_indx <= RowsArea_EndRowIndex; layout_row_indx++, row_indx++)
                {
                    CellControl cell_control = null;
                    cell_control = m_CellControls_Cache[column_indx, row_indx];
                    if (cell_control == null)
                    {
                        cell_control = new CellControl(this);
                        cell_control.MouseEnter += new MouseEventHandler(cell_control_MouseEnter);
                        cell_control.Click += new RoutedEventHandler(cell_control_Click);
                        cell_control.MouseDoubleClick += new EventHandler(CellControl_MouseDoubleClick);
                        cell_control.Cell = null;
                        m_CellControls_Cache.Add(cell_control, column_indx, row_indx);
                        ItemsLayoutRoot.Children.Add(cell_control);
                        Grid.SetColumn(cell_control, layout_column_indx);
                        Grid.SetRow(cell_control, layout_row_indx);
                    }
                    else
                    {
                        // Сбросим при инициализации cell_control.Cell = null;
                    }
                    added++;
                }
            }

            // удаляем лишние по строкам и столбцам
            for (int i = 0; i < m_CellControls_Cache.Columns_Size; i++)
            {
                for (int j = 0; j < m_CellControls_Cache.Rows_Size; j++)
                {
                    if (i >= ColumnsArea_ColumnsCount ||
                        j >= RowsArea_RowsCount)
                    {
                        CellControl cell_Control = m_CellControls_Cache.RemoveAt(i, j);
                        if (cell_Control != null)
                            ItemsLayoutRoot.Children.Remove(cell_Control);
                    }
                }
            }


            DateTime stop = DateTime.Now;
            System.Diagnostics.Debug.WriteLine(" BuildCellsArea time: " + (stop - start).ToString());

            #endregion Настройка области ячеек
        }

        void cell_control_Click(object sender, RoutedEventArgs e)
        {
            this.Focus();
            OnCellControlMouseDown(sender as CellControl);
        }

        void BuildSplitters()
        {
            DateTime start = DateTime.Now;

            #region Сплиттеры
            //// Добавляем сплиттеры для изменения ширины
            //for (int i = 0; i < ItemsLayoutRoot.ColumnDefinitions.Count; i++)
            //{
            //    if (!m_Vertiacal_Splitters.ContainsKey(i))
            //    {
            //        m_Vertiacal_Splitters.Add(i, Add_VertSplitter(ItemsLayoutRoot, i, 0, ItemsLayoutRoot.RowDefinitions.Count));
            //    }
            //    else
            //    {
            //        GridSplitter splitter = m_Vertiacal_Splitters[i];
            //        if (Grid.GetRowSpan(splitter) != ItemsLayoutRoot.RowDefinitions.Count)
            //        {
            //            Grid.SetRowSpan(splitter, ItemsLayoutRoot.RowDefinitions.Count);
            //        }
            //    }
            //}

            //// Добавляем сплиттеры для изменения высоты
            //for (int i = 0; i < ItemsLayoutRoot.RowDefinitions.Count; i++)
            //{
            //    if (!m_Horizontal_Splitters.ContainsKey(i))
            //    {
            //        GridSplitter splitter = Add_HorzSplitter(ItemsLayoutRoot, 0, i, ItemsLayoutRoot.ColumnDefinitions.Count);
            //        m_Horizontal_Splitters.Add(i, splitter);
            //    }
            //    else
            //    {
            //        GridSplitter splitter = m_Horizontal_Splitters[i];
            //        if (Grid.GetColumnSpan(splitter) != ItemsLayoutRoot.ColumnDefinitions.Count)
            //        {
            //            Grid.SetColumnSpan(splitter, ItemsLayoutRoot.ColumnDefinitions.Count);
            //        }
            //    }
            //}

            
            // Сплиттеры ширины на область строк
            for (int i = PivotArea_BeginColumnIndex; i < RowsArea_ColumnsCount; i++)
            {
                if (!m_Vertiacal_Splitters.ContainsKey(i))
                {
                    m_Vertiacal_Splitters.Add(i, Add_VertSplitter(ItemsLayoutRoot, i, 0, ItemsLayoutRoot.RowDefinitions.Count));
                }
                else
                {
                    GridSplitter splitter = m_Vertiacal_Splitters[i];
                    if (Grid.GetRowSpan(splitter) != ItemsLayoutRoot.RowDefinitions.Count)
                    {
                        Grid.SetRowSpan(splitter, ItemsLayoutRoot.RowDefinitions.Count);
                    }
                }
            }
            // Сплиттеры ширины на область ячеек
            int rowSpan = ItemsLayoutRoot.RowDefinitions.Count - CellsArea_BeginRowIndex;
            if (rowSpan > 0)
            {
                // Добавляем сплиттеры для изменения ширины
                for (int i = CellsArea_BeginColumnIndex; i < ItemsLayoutRoot.ColumnDefinitions.Count; i++)
                {
                    if (!m_Vertiacal_Splitters.ContainsKey(i))
                    {
                        m_Vertiacal_Splitters.Add(i, Add_VertSplitter(ItemsLayoutRoot, i, CellsArea_BeginRowIndex, rowSpan));
                    }
                    else
                    {
                        GridSplitter splitter = m_Vertiacal_Splitters[i];
                        if (Grid.GetRowSpan(splitter) != rowSpan)
                        {
                            Grid.SetRowSpan(splitter, rowSpan);
                        }
                    }
                }
            }

            // Сплиттеры высоты на область колонок
            for (int i = PivotArea_BeginRowIndex; i < ColumnsArea_RowsCount; i++)
            {
                if (!m_Horizontal_Splitters.ContainsKey(i))
                {
                    GridSplitter splitter = Add_HorzSplitter(ItemsLayoutRoot, 0, i, ItemsLayoutRoot.ColumnDefinitions.Count);
                    m_Horizontal_Splitters.Add(i, splitter);
                }
                else
                {
                    GridSplitter splitter = m_Horizontal_Splitters[i];
                    if (Grid.GetColumnSpan(splitter) != ItemsLayoutRoot.ColumnDefinitions.Count)
                    {
                        Grid.SetColumnSpan(splitter, ItemsLayoutRoot.ColumnDefinitions.Count);
                    }
                }
            }
            // Сплиттеры высоты на область ячеек
            int columnSpan = ItemsLayoutRoot.ColumnDefinitions.Count - CellsArea_BeginColumnIndex;
            if (columnSpan > 0)
            {
                // Добавляем сплиттеры для изменения высоты
                for (int i = CellsArea_BeginRowIndex; i < ItemsLayoutRoot.RowDefinitions.Count; i++)
                {
                    if (!m_Horizontal_Splitters.ContainsKey(i))
                    {
                        GridSplitter splitter = Add_HorzSplitter(ItemsLayoutRoot, CellsArea_BeginColumnIndex, i, columnSpan);
                        m_Horizontal_Splitters.Add(i, splitter);
                    }
                    else
                    {
                        GridSplitter splitter = m_Horizontal_Splitters[i];
                        if (Grid.GetColumnSpan(splitter) != columnSpan)
                        {
                            Grid.SetColumnSpan(splitter, columnSpan);
                        }
                    }
                }
            }
            #endregion Сплиттеры

            DateTime stop = DateTime.Now;
            System.Diagnostics.Debug.WriteLine(" BuildSplitters time: " + (stop - start).ToString());
        }

        #endregion Построение областей

        Dictionary<int, GridSplitter> m_Vertiacal_Splitters = new Dictionary<int, GridSplitter>();
        Dictionary<int, GridSplitter> m_Horizontal_Splitters = new Dictionary<int, GridSplitter>();

        IList<MemberControl> m_RowsArea_LovestMemberControls = null;
        public IList<MemberControl> RowsArea_LovestMemberControls
        {
            get
            {
                if (m_RowsArea_LovestMemberControls == null)
                {
                    m_RowsArea_LovestMemberControls = new List<MemberControl>();
                }
                return m_RowsArea_LovestMemberControls;
            }
        }

        IList<MemberControl> m_ColumnsArea_LovestMemberControls = null;
        public IList<MemberControl> ColumnsArea_LovestMemberControls
        {
            get
            {
                if (m_ColumnsArea_LovestMemberControls == null)
               {
                    m_ColumnsArea_LovestMemberControls = new List<MemberControl>();
                }
                return m_ColumnsArea_LovestMemberControls;
            }
        }

        public readonly CellCoordinate ColumnsArea_FirstVisible_Coordinate = new CellCoordinate();
        public readonly CellCoordinate RowsArea_FirstVisible_Coordinate = new CellCoordinate();
        public CellCoordinate CellsArea_FirstVisible_Coordinate
        {
            get {
                return new CellCoordinate(ColumnsArea_FirstVisible_Coordinate.Column, RowsArea_FirstVisible_Coordinate.Row);
            }
        }

        #region Инициализация областей
        List<MemberControl> m_ColumnsMembers = new List<MemberControl>();
        Cache2D<GridSplitter> m_ColumnsArea_Splitters = new Cache2D<GridSplitter>();

        public void InitializeColumnsArea(PivotLayoutProvider layout)
        {
            foreach (MemberControl ctrl in m_ColumnsMembers)
            {
                ItemsLayoutRoot.Children.Remove(ctrl);
            }
            m_ColumnsMembers.Clear();

            for (int c = 0; c < m_ColumnsArea_Splitters.Columns_Size; c++)
                for (int r = 0; r < m_ColumnsArea_Splitters.Rows_Size; r++)
                {
                    var splitter = m_ColumnsArea_Splitters[c, r];
                    if (splitter != null)
                    {
                        ItemsLayoutRoot.Children.Remove(splitter);
                    }
                }
            m_ColumnsArea_Splitters = new Cache2D<GridSplitter>();

            ColumnsArea_LovestMemberControls.Clear();
            
            // Получаем сортировку по значению для противоположной оси
            SortByValueDescriptor value_sort = null;
            if (QueryManager != null)
            {
                value_sort = (AxisIsRotated == false ? QueryManager.Axis1_MeasuresSort : QueryManager.Axis0_MeasuresSort) as SortByValueDescriptor;
            }

            int start_ColumnIndex = CellsArea_BeginColumnIndex;
            for (int column_indx = 0, layout_column_indx = start_ColumnIndex; layout_column_indx < ItemsLayoutRoot.ColumnDefinitions.Count; column_indx++, layout_column_indx++)
            {
                ColumnDefinition col = ItemsLayoutRoot.ColumnDefinitions[layout_column_indx];
                if (col == m_Fictive_Column)
                    continue;

                for (int row_indx = 0; row_indx < layout.ColumnsLayout.Rows_Size; row_indx++)
                {
                    // !!!
                    LayoutCellWrapper cell_wrapper = layout.ColumnsLayout[ColumnsArea_FirstVisible_Coordinate.Column + column_indx, ColumnsArea_FirstVisible_Coordinate.Row + row_indx];
                    if (cell_wrapper != null)
                    {
                        foreach (LayoutItem item in cell_wrapper.Items)
                        {
                            MemberControl member_Control = null;
                            MemberLayoutItem member_item = item as MemberLayoutItem;
                            if (member_item != null &&
                                (member_item.IsExtension == false || (member_item.IsExtension == true && column_indx == 0)))
                            {
                                if (m_MemberControls_Dict.ContainsKey(member_item.PivotMember.Member))
                                {
                                    member_Control = m_MemberControls_Dict[member_item.PivotMember.Member];
                                }
                                if (member_Control == null)
                                {
                                    member_Control = new ColumnMemberControl(this, member_item.PivotMember.Member, member_item.PivotMember.PivotDrillDepth);
                                    m_MemberControls_Dict.Add(member_item.PivotMember.Member, member_Control);

                                    // Подписываемся на Drill-операцию
                                    member_Control.ExecuteMemberAction += new MemberActionEventHandler(OnExecuteMemberAction);
                                    // Подписываемся на операцию по умолчанию
                                }

                                if (CellsArea_BeginRowIndex > 0 && row_indx == CellsArea_BeginRowIndex - 1)
                                {
                                    if (member_item.IsExtension == false)
                                    {
                                        ColumnsArea_LovestMemberControls.Add(member_Control);

                                        // Определяем сортирована ли данная колонка
                                        if (value_sort != null && value_sort.Type != SortTypes.None)
                                        {
                                            // Сортирована ли по данному элементу
                                            if (value_sort.CompareByTuple(member_item.PivotMember.Member.GetAxisTuple()))
                                            {
                                                member_Control.SortByValueType = value_sort.Type;
                                            }
                                        }
                                        //if (m_Vertiacal_Splitters.ContainsKey(layout_column_indx))
                                        //{
                                        //    m_Vertiacal_Splitters[layout_column_indx].Tag = member_Control;
                                        //    //Canvas.SetZIndex(m_Vertiacal_Splitters[layout_column_indx], 10);
                                        //}
                                    }
                                }

                                m_ColumnsMembers.Add(member_Control);

                                ItemsLayoutRoot.Children.Add(member_Control);
                                Grid.SetColumn(member_Control, layout_column_indx);
                                Grid.SetColumnSpan(member_Control, member_item.ColumnSpan);
                                Grid.SetRow(member_Control, row_indx);
                                Grid.SetRowSpan(member_Control, member_item.RowSpan);

                                if (member_item.PivotMember.IsFirstDrillDownChild || (column_indx == 0/* && member_item.IsExtension == false*/))
                                {
                                    member_Control.ShowLeftBorder = true;
                                }
                                else
                                {
                                    member_Control.ShowLeftBorder = false;
                                }

                                if (row_indx == 0 || member_item.PivotMember.PivotDrillDepth > 0)
                                {
                                    member_Control.ShowUpBorder = true;
                                }
                                else
                                {
                                    member_Control.ShowUpBorder = false;
                                }

                                // Если элемент размером на несколько строк, то сплиттер добавляем на последнюю строку
                                int cells_area_index = column_indx + (member_item.ColumnSpan > 1 ? (member_item.ColumnSpan - 1) : 0);
                                var splitter = Add_VertSplitter(ItemsLayoutRoot, ColumnsArea_BeginColumnIndex + cells_area_index, row_indx, 1);
                                splitter.Margin = new Thickness(0, member_Control.Margin.Top, 0, 0);
                                m_ColumnsArea_Splitters.Add(splitter, cells_area_index, row_indx);

                                // Чтобы сплиттер для элемента не накладывался на DrillDown родителя
                                if (column_indx > 0)
                                {
                                    // Верхнее смещение сплиттера должно быть как смещение следующего за сплиттером контрола
                                    var prev_member_splitter = m_ColumnsArea_Splitters[column_indx - 1, row_indx];
                                    if (prev_member_splitter != null)
                                    {
                                        prev_member_splitter.Margin = new Thickness(0, member_Control.Margin.Top, 0, 0);
                                    }
                                    else
                                    {
                                        // Если сплиттер на предыдущей строке разметки не найден, то добавим его
                                        prev_member_splitter = Add_VertSplitter(ItemsLayoutRoot, ColumnsArea_BeginColumnIndex + column_indx - 1, row_indx, 1);
                                        prev_member_splitter.Margin = new Thickness(0, member_Control.Margin.Top, 0, 0);
                                        m_ColumnsArea_Splitters.Add(prev_member_splitter, column_indx - 1, row_indx);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        List<MemberControl> m_RowsMembers = new List<MemberControl>();
        Dictionary<MemberInfo, MemberControl> m_MemberControls_Dict = new Dictionary<MemberInfo, MemberControl>();
        Cache2D<GridSplitter> m_RowsArea_Splitters = new Cache2D<GridSplitter>();

        public void InitializeRowsArea(PivotLayoutProvider layout)
        {
            foreach (MemberControl ctrl in m_RowsMembers)
            {
                ItemsLayoutRoot.Children.Remove(ctrl);
            }
            m_RowsMembers.Clear();
            
            for(int c = 0; c < m_RowsArea_Splitters.Columns_Size; c++)
                for(int r = 0; r < m_RowsArea_Splitters.Rows_Size; r++)
                {
                    var splitter = m_RowsArea_Splitters[c, r];
                    if(splitter != null)
                    {
                        ItemsLayoutRoot.Children.Remove(splitter);
                    }
                }
            m_RowsArea_Splitters = new Cache2D<GridSplitter>();

            RowsArea_LovestMemberControls.Clear();

            // Получаем сортировку по значению для противоположной оси
            SortByValueDescriptor value_sort = null;
            if (QueryManager != null)
            {
                value_sort = (AxisIsRotated == false ? QueryManager.Axis0_MeasuresSort : QueryManager.Axis1_MeasuresSort) as SortByValueDescriptor;
            }

            int start_RowIndex = CellsArea_BeginRowIndex;
            for (int column_indx = 0; column_indx < layout.RowsLayout.Columns_Size; column_indx++)
            {
                for (int row_indx = 0, layout_row_indx = start_RowIndex; layout_row_indx < ItemsLayoutRoot.RowDefinitions.Count; row_indx++, layout_row_indx++)
                {
                    RowDefinition row = ItemsLayoutRoot.RowDefinitions[layout_row_indx];
                    if (row == m_Fictive_Row)
                        continue;
                    // !!!
                    LayoutCellWrapper cell_wrapper = layout.RowsLayout[RowsArea_FirstVisible_Coordinate.Column + column_indx, RowsArea_FirstVisible_Coordinate.Row + row_indx];
                    if (cell_wrapper != null)
                    {
                        foreach (LayoutItem item in cell_wrapper.Items)
                        {
                            MemberControl member_Control = null;

                            MemberLayoutItem member_item = item as MemberLayoutItem;
                            if (member_item != null &&
                                (member_item.IsExtension == false || (member_item.IsExtension == true && row_indx == 0)))
                            {
                                if (m_MemberControls_Dict.ContainsKey(member_item.PivotMember.Member))
                                {
                                    member_Control = m_MemberControls_Dict[member_item.PivotMember.Member];
                                }
                                if (member_Control == null)
                                {
                                    member_Control = new RowMemberControl(this, member_item.PivotMember.Member, member_item.PivotMember.PivotDrillDepth);
                                    m_MemberControls_Dict.Add(member_item.PivotMember.Member, member_Control);

                                    // Подписываемся на Drill-операцию
                                    member_Control.ExecuteMemberAction += new MemberActionEventHandler(OnExecuteMemberAction);
                                }

                                if (CellsArea_BeginColumnIndex > 0 && column_indx == CellsArea_BeginColumnIndex - 1)
                                {
                                    if (member_item.IsExtension == false)
                                    {
                                        RowsArea_LovestMemberControls.Add(member_Control);

                                        // Определяем сортирована ли данная колонка
                                        if (value_sort != null && value_sort.Type != SortTypes.None)
                                        {
                                            // Сортирована ли по данному элементу
                                            if (value_sort.CompareByTuple(member_item.PivotMember.Member.GetAxisTuple()))
                                            {
                                                member_Control.SortByValueType = value_sort.Type;
                                            }
                                        }
                                        //if (m_Horizontal_Splitters.ContainsKey(layout_row_indx))
                                        //{
                                        //    m_Horizontal_Splitters[layout_row_indx].Tag = member_Control;
                                        //    //Canvas.SetZIndex(m_Horizontal_Splitters[layout_row_indx], 10);
                                        //}
                                    }
                                }

                                m_RowsMembers.Add(member_Control);

                                //if(member_item.IsExtension)
                                //    member_Control.RenderTransform = new RotateTransform() { Angle = -90  /*CenterY = -20*/ };

                                ItemsLayoutRoot.Children.Add(member_Control);
                                Grid.SetColumn(member_Control, column_indx);
                                Grid.SetColumnSpan(member_Control, member_item.ColumnSpan);
                                Grid.SetRow(member_Control, layout_row_indx);
                                Grid.SetRowSpan(member_Control, member_item.RowSpan);

                                if ((member_item.PivotMember.IsFirstDrillDownChild && member_item.IsExtension == false) || (row_indx == 0/* && member_item.IsExtension == false*/))
                                {
                                    member_Control.ShowUpBorder= true;
                                    //if (member_item.IsExtension)
                                    //{
                                    //    member_Control.Opacity = 0.3;
                                    //}
                                }
                                else
                                {
                                    member_Control.ShowUpBorder = false;
                                }

                                member_Control.RotateCaption(member_item.IsExtension);

                                if (column_indx == 0 || member_item.PivotMember.PivotDrillDepth > 0)
                                {
                                    member_Control.ShowLeftBorder = true;
                                }
                                else
                                {
                                    member_Control.ShowLeftBorder = false;
                                }

                                // Если элемент размером на несколько строк, то сплиттер добавляем на последнюю строку
                                int cells_area_index = row_indx + (member_item.RowSpan > 1 ? (member_item.RowSpan - 1) : 0);
                                var splitter = Add_HorzSplitter(ItemsLayoutRoot, column_indx, RowsArea_BeginRowIndex + cells_area_index, 1);
                                splitter.Margin = new Thickness(member_Control.Margin.Left, 0, 0, 0);
                                m_RowsArea_Splitters.Add(splitter, column_indx, cells_area_index);

                                // Чтобы сплиттер для элемента не накладывался на DrillDown родителя
                                if (row_indx > 0)
                                {
                                    // Левое смещение сплиттера должно быть как смещение следующего за сплиттером контрола
                                    var prev_member_splitter = m_RowsArea_Splitters[column_indx, row_indx - 1];
                                    if (prev_member_splitter != null)
                                    {
                                        prev_member_splitter.Margin = new Thickness(member_Control.Margin.Left, 0, 0, 0);
                                    }
                                    else
                                    {
                                        // Если сплиттер на предыдущей строке разметки не найден, то добавим его
                                        prev_member_splitter = Add_HorzSplitter(ItemsLayoutRoot, column_indx, RowsArea_BeginRowIndex + row_indx - 1, 1);
                                        prev_member_splitter.Margin = new Thickness(member_Control.Margin.Left, 0, 0, 0);
                                        m_RowsArea_Splitters.Add(prev_member_splitter, column_indx, row_indx - 1);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Кэш измененных ячеек
        /// </summary>
        public readonly CellChangesCache LocalChanges = new CellChangesCache();

        public void InitializeCellsArea(PivotLayoutProvider layout)
        {
            CellInfo old_FocusedCellView = FocusedCellView;
            IDictionary<String, MemberInfo> old_tuple = new Dictionary<String, MemberInfo>();
            if (old_FocusedCellView != null)
            {
                old_tuple = old_FocusedCellView.GetTuple();
            }
            CellControl new_FocusedCell = null;

            if (FocusedCell != null)
                FocusedCell.IsFocused = false;

            m_CellControls_Dict.Clear();

            CellChangesCache recalculatedCache = OlapTransactionManager.GetPendingChanges(Connection);
            //int columnIndex = CellsArea_FirstVisible_Coordinate.Column;
            int columnIndex = CellsArea_FirstVisible_Coordinate.Column;

            int layout_column_indx = 0;
            int layout_row_indx = 0;

            bool hasColumnsArea = true;
            int columnsCount = ColumnsArea_LovestMemberControls.Count;
            // если в результате нет осей, а ячейки есть (select from [Adventure Works]), то они должны отображаться без области колонок
            if (columnsCount == 0 && layout.PivotProvider.Provider.CellSet_Description != null &&
                layout.PivotProvider.Provider.CellSet_Description.Cells.Count > 0)
            {
                // Если колонок в CellSet нет, а ячейки есть, то в сводной таблице будет 1 колонка (без шапки)
                columnsCount = 1;
                columnIndex = -1;
                hasColumnsArea = false;
            }

            int Axis0Coord = -1;
            for(int column = 0; column < columnsCount; column++)
            {
                if(hasColumnsArea && ColumnsArea_LovestMemberControls.Count > column)
                {
                    Axis0Coord = ColumnsArea_LovestMemberControls[column].Member.MemberIndexInAxis;
                }

                int rowIndex = CellsArea_FirstVisible_Coordinate.Row;
                layout_row_indx = 0;
                int rowsCount = RowsArea_LovestMemberControls.Count;

                bool hasRowsArea = true;
                // если в результате только одна ось, ячейки есть. То они должны отображаться без области строк
                if (rowsCount == 0 && layout.PivotProvider.Provider.CellSet_Description != null &&
                    layout.PivotProvider.Provider.CellSet_Description.Cells.Count > 0)
                {
                    // Если строк в CellSet нет, а ячейки есть, то в сводной таблице будет 1 строка (без шапки)
                    rowsCount = 1;
                    rowIndex = -1;
                    hasRowsArea = false;
                }

                int Axis1Coord = -1;
                for (int row = 0; row < rowsCount; row++)
                {
                    if (hasRowsArea && RowsArea_LovestMemberControls.Count > row)
                    {
                        Axis1Coord = RowsArea_LovestMemberControls[row].Member.MemberIndexInAxis;
                    }

                    CellInfo cell_info = layout.PivotProvider.Provider.GetCellInfo(Axis0Coord, Axis1Coord);
                    //CellInfo cell_info = layout.PivotProvider.Provider.GetCellInfo(columnIndex, rowIndex);

                    if (cell_info != null)
                    {
                        cell_info.CellsArea_Axis0_Coord = CellsArea_FirstVisible_Coordinate.Column + column;
                        cell_info.CellsArea_Axis1_Coord = CellsArea_FirstVisible_Coordinate.Row + row;

                        CellControl cell_Control = m_CellControls_Cache[layout_column_indx, layout_row_indx];

                        if (cell_Control != null)
                        {
                            m_CellControls_Dict.Add(cell_info, cell_Control);
                            // Left border
                            cell_Control.ShowLeftBorder = !hasRowsArea && layout_column_indx == 0;
                            // Up border
                            cell_Control.ShowUpBorder = !hasColumnsArea && layout_row_indx == 0;

                            cell_Control.Cell = cell_info;
                            if (cell_Control.IsFocused)
                            {
                                cell_Control.IsFocused = false;
                            }

                            if (old_FocusedCellView != null && new_FocusedCell == null)
                            {
                                // Определяем ячейку, на которую должен быть установлен фокус.
                                // Ячейки сверяются по совпадению таплов, т.к. в случае скроллинга для FocusedCellView может быть создан другой контрол
                                // В случае нажатия на кнопку Обновить, либо редактирования чеек FocusedCellView содержит элемент из старого источника. Поэтому нужно обновить его при совпадении таплов
                                if (cell_info.CompareByTuple(old_tuple))
                                {
                                    // Обновляем информацию о ячейке с фокусом
                                    if (cell_info != FocusedCellView)
                                    {
                                        FocusedCellView = cell_info;
                                    }
                                    new_FocusedCell = cell_Control;
                                    new_FocusedCell.IsFocused = true;
                                }
                            }

                            // Cell is changed and not Recalculated
                            cell_Control.NotRecalculatedChange = LocalChanges.FindChange(cell_info);
                            // Cell is changed and already Recalculated
                            cell_Control.RecalculatedChange = recalculatedCache.FindChange(cell_info);
                        }
                    }

                    rowIndex++;
                    layout_row_indx++;
                }
                columnIndex++;
                layout_column_indx++;
            }

            // В КЭШе ячеек для остальных ячеек сбрасываем содержимое
            for (int col = layout_column_indx; col < m_CellControls_Cache.Columns_Size; col++)
            {
                for (int row = 0; row < m_CellControls_Cache.Rows_Size; row++)
                {
                    CellControl cell_Control = m_CellControls_Cache[col, row];
                    if (cell_Control != null)
                    {
                        cell_Control.Cell = null;
                    }
                }
            }
            for (int row = layout_row_indx; row < m_CellControls_Cache.Rows_Size; row++)
            {
                for (int col = 0; col < m_CellControls_Cache.Columns_Size; col++)
                {
                    CellControl cell_Control = m_CellControls_Cache[col, row];
                    if (cell_Control != null)
                    {
                        cell_Control.Cell = null;
                    }
                }
            }

            RefreshSelectedCells();
        }

        void InitializeSplitters()
        {
            DateTime start = DateTime.Now;

            #region Сплиттеры
            // Настраиваем спилиттеры для изменения ширины
            foreach (GridSplitter splitter in m_Vertiacal_Splitters.Values)
            {
                Canvas.SetZIndex(splitter, 10);
            }

            // Настраиваем спилиттеры для изменения высоты
            foreach (GridSplitter splitter in m_Horizontal_Splitters.Values)
            {
                Canvas.SetZIndex(splitter, 10);
            }
            #endregion Сплиттеры

            DateTime stop = DateTime.Now;
            System.Diagnostics.Debug.WriteLine(" InitializeSplitters time: " + (stop - start).ToString());
        }

        #endregion Инициализация областей

        #region Контекстное меню
        CustomContextMenu m_ContextMenu = null;
        public CustomContextMenu ContextMenu
        {
            get { return m_ContextMenu; }
        }

        CustomContextMenu GetCurrentContextMenu(Point p)
        {
            m_ContextMenu = null; 
            PivotGridItem grid_item = null;

            if (AgControlBase.GetSLBounds(ItemsLayoutRoot).Contains(p))
            {
                grid_item = PivotGridItem.GetPivotGridItem(p);
                MemberControl member_control = grid_item as MemberControl;
                if (member_control != null)
                {
                    if (grid_item is RowMemberControl)
                    {
                        m_ContextMenu = Rows_ContextMenu;
                    }
                    if (grid_item is ColumnMemberControl)
                    {
                        m_ContextMenu = Columns_ContextMenu;
                    }

                    if (m_ContextMenu != null)
                    {
                        // Делаем доступными пункты меню только если это необходимо
                        foreach (UIElement element in m_ContextMenu.Items)
                        {
                            ContextMenuItem menu_item = element as ContextMenuItem;
                            if (menu_item != null)
                            {
                                if (menu_item.Tag is MemberActionType)
                                {
                                    // Если пункт меню является в принципе доступным, то устанавливаем его доступность с учетом текущего состояния
                                    if (((MemberActionType)menu_item.Tag == MemberActionType.Collapse) || ((MemberActionType)menu_item.Tag == MemberActionType.Expand))
                                    {
                                        if ((MemberActionType)menu_item.Tag == MemberActionType.Collapse)
                                        {
                                            // Учитываем доступность впринципе и текущее состояние элемента
                                            menu_item.IsEnabled = member_control.UseExpandingCommands & member_control.Member.DrilledDown;
                                        }
                                        if ((MemberActionType)menu_item.Tag == MemberActionType.Expand)
                                        {
                                            // Учитываем доступность впринципе и текущее состояние элемента
                                            menu_item.IsEnabled = member_control.UseExpandingCommands & !member_control.Member.DrilledDown;
                                        }
                                    }
                                }

                                if (menu_item.Tag is ControlActionType)
                                {
                                    ControlActionType action = (ControlActionType)menu_item.Tag;
                                    switch (action)
                                    {
                                        case ControlActionType.SortingByValue:
                                            // Доступность пункта меню - Сортировка по значению
                                            // Делаем доступным только для элементов последних линий
                                            menu_item.IsEnabled = member_control.Member != null && member_control.Member.Children.Count == 0;
                                            break;
                                    }
                                }
                            }
                        }
                    }
                }

                if (grid_item is CellControl || grid_item == null)
                {
                    m_ContextMenu = Cells_ContextMenu;

                    CellControl cell_Control = grid_item as CellControl;
                    if (cell_Control != null)
                    {
                        // Устанавливаем фокус на данную ячеку если она не находится в списке выбранных
                        if (!Selection.Contains(cell_Control.Cell))
                            FocusedCellView = cell_Control.Cell;
                    }

                    // Если меню вызвано за пределами области ячеек, то будет относиться к ячейке с фокусом
                    CellInfo info = cell_Control != null ? cell_Control.Cell : FocusedCellView;

                    if (m_DeliveryValueMenuItem != null)
                        m_DeliveryValueMenuItem.IsEnabled = info != null && info.IsUpdateable;
                    if (m_CopyValueMenuItem != null)
                        m_CopyValueMenuItem.IsEnabled = info != null && info.IsUpdateable;

                    if (m_PasteSelectedCellsMenuItem != null)
                        m_PasteSelectedCellsMenuItem.IsEnabled = EditMode & CanEdit;
                    IList<CellInfo> cells = Selection;
                    if (m_CopySelectedCellsMenuItem != null)
                        m_CopySelectedCellsMenuItem.IsEnabled = cells != null && cells.Count > 0;
                    if (m_DrillThroughMenuItem != null)
                        m_DrillThroughMenuItem.IsEnabled = info != null && !info.IsCalculated && cells != null && cells.Count == 1;
                }

                if (m_ContextMenu != null)
                {
                    m_ContextMenu.Tag = grid_item;
                }

                if (m_ContextMenu != null)
                {
                    foreach (UIElement element in m_ContextMenu.Items)
                    {
                        CheckedContectMenuItem menu_item = element as CheckedContectMenuItem;
                        if (menu_item != null && menu_item.Tag is ControlActionType)
                        {
                            if ((ControlActionType)(menu_item.Tag) == ControlActionType.AutoWidth)
                            {
                                menu_item.IsChecked = AutoWidthColumns;
                            }
                        }
                    }
                }
            }

            var handler = InitializeContextMenu;
            CustomContextMenuEventArgs args = new CustomContextMenuEventArgs(m_ContextMenu, grid_item);
            if (handler != null)
            {
                handler(this, args);
            }

            return args.Menu;
        }

        public event EventHandler<CustomContextMenuEventArgs> InitializeContextMenu;

        CustomContextMenu m_Rows_ContextMenu = null;
        public CustomContextMenu Rows_ContextMenu
        {
            get
            {
                if (m_Rows_ContextMenu == null)
                {
                    m_Rows_ContextMenu = CreateContextMenu(AreaType.RowsArea);
                    Raise_Rows_ContextMenuCreated();
                }
                return m_Rows_ContextMenu;
            }
        }

        CustomContextMenu m_Columns_ContextMenu = null;
        public CustomContextMenu Columns_ContextMenu
        {
            get
            {
                if (m_Columns_ContextMenu == null)
                {
                    m_Columns_ContextMenu = CreateContextMenu(AreaType.ColumnsArea);
                    Raise_Columns_ContextMenuCreated();
                }
                return m_Columns_ContextMenu;
            }
        }

        CustomContextMenu m_Cells_ContextMenu = null;
        public CustomContextMenu Cells_ContextMenu
        {
            get
            {
                if (m_Cells_ContextMenu == null)
                {
                    m_Cells_ContextMenu = CreateContextMenu(AreaType.CellsArea);
                    Raise_Cells_ContextMenuCreated();
                }
                return m_Cells_ContextMenu;
            }
        }

        ContextMenuItem m_DeliveryValueMenuItem = null;
        ContextMenuItem m_CopyValueMenuItem = null;
        ContextMenuSplitter m_DeliveryValueSplitter = null;
        ContextMenuItem m_CopySelectedCellsMenuItem = null;
        ContextMenuItem m_PasteSelectedCellsMenuItem = null;
        ContextMenuSplitter m_CopyCellsSplitter = null;
        ContextMenuItem m_DrillThroughMenuItem = null;

        private CustomContextMenu CreateContextMenu(AreaType areaType)
        {
            CustomContextMenu contextMenu = new CustomContextMenu();
            contextMenu.Opened += new EventHandler(contextMenu_Opened);
            contextMenu.Closed += new EventHandler(contextMenu_Closed);
            ContextMenuItem item;

            if ((areaType == AreaType.RowsArea && Rows_IsInteractive) ||
                (areaType == AreaType.ColumnsArea && Columns_IsInteractive))
            {
                item = new ContextMenuItem(Localization.PivotGrid_Expand);
                item.Tag = MemberActionType.Expand;
                if (areaType == AreaType.RowsArea)
                {
                    item.Shortcut = Localization.PivotGrid_Rows_Expand_ShortCut;
                }
                else
                {
                    item.Shortcut = Localization.PivotGrid_Columns_Expand_ShortCut;
                }
                contextMenu.AddMenuItem(item);
                item.ItemClick += new EventHandler(ContextMenu_ItemClick);

                item = new ContextMenuItem(Localization.PivotGrid_Collapse);
                item.Tag = MemberActionType.Collapse;
                if (areaType == AreaType.RowsArea)
                {
                    item.Shortcut = Localization.PivotGrid_Rows_Collapse_ShortCut;
                }
                else
                {
                    item.Shortcut = Localization.PivotGrid_Columns_Collapse_ShortCut;
                }
                contextMenu.AddMenuItem(item);
                item.ItemClick += new EventHandler(ContextMenu_ItemClick);

                item = new ContextMenuItem(Localization.PivotGrid_DrillDown);
                item.Tag = MemberActionType.DrillDown;
                if (areaType == AreaType.RowsArea)
                {
                    item.Shortcut = Localization.PivotGrid_Rows_Drilldown_ShortCut;
                }
                else
                {
                    item.Shortcut = Localization.PivotGrid_Columns_Drilldown_ShortCut;
                }
                contextMenu.AddMenuItem(item);
                item.ItemClick += new EventHandler(ContextMenu_ItemClick);

                contextMenu.AddMenuSplitter();
            }

            // Пуекты для сортировки элементов
            if ((areaType == AreaType.RowsArea) ||
                (areaType == AreaType.ColumnsArea))
            {
                item = new ContextMenuItem(Localization.ContextMenu_SortingByProperty);
                item.Tag = ControlActionType.SortingByProperty;
                contextMenu.AddMenuItem(item);
                item.ItemClick += new EventHandler(ContextMenu_ItemClick);

                item = new ContextMenuItem(Localization.ContextMenu_SortingByMeasure);
                item.Tag = ControlActionType.SortingAxisByMeasure;
                contextMenu.AddMenuItem(item);
                item.ItemClick += new EventHandler(ContextMenu_ItemClick);

                item = new ContextMenuItem(Localization.ContextMenu_SortingByValue);
                item.Tag = ControlActionType.SortingByValue;
                contextMenu.AddMenuItem(item);
                item.ItemClick += new EventHandler(ContextMenu_ItemClick);

                //item = new ContextMenuItem(Localization.ContextMenu_ClearAxisSorting);
                //item.Tag = ControlActionType.ClearAxisSorting;
                //contextMenu.AddMenuItem(item);
                //item.ItemClick += new EventHandler(ContextMenu_ItemClick);

                contextMenu.AddMenuSplitter();
            }

            if (areaType == AreaType.CellsArea)
            {

                m_CopyValueMenuItem = new ContextMenuItem(Localization.ContextMenu_CopyValue + "      ");
                m_CopyValueMenuItem.Tag = ControlActionType.ValueCopy;
                //item.Icon = UriResources.Images.ActionNode16;
                contextMenu.AddMenuItem(m_CopyValueMenuItem);
                m_CopyValueMenuItem.ItemClick += new EventHandler(ContextMenu_ItemClick);

                m_DeliveryValueMenuItem = new ContextMenuItem(Localization.ContextMenu_DeliveryValue + "      ");
                m_DeliveryValueMenuItem.Tag = ControlActionType.ValueDelivery;
                //item.Icon = UriResources.Images.ActionNode16;
                contextMenu.AddMenuItem(m_DeliveryValueMenuItem);
                m_DeliveryValueMenuItem.ItemClick += new EventHandler(ContextMenu_ItemClick);

                m_DeliveryValueSplitter = contextMenu.AddMenuSplitter();

                m_DrillThroughMenuItem = new ContextMenuItem(Localization.ContextMenu_DrillThrough);
                m_DrillThroughMenuItem.Tag = ControlActionType.DrillThrough;
                //item.Icon = UriResources.Images.ActionNode16;
                contextMenu.AddMenuItem(m_DrillThroughMenuItem);
                m_DrillThroughMenuItem.ItemClick += new EventHandler(ContextMenu_ItemClick);

                m_CopySelectedCellsMenuItem = new ContextMenuItem(Localization.ContextMenu_Copy);
                m_CopySelectedCellsMenuItem.Tag = ControlActionType.Copy;
                m_CopySelectedCellsMenuItem.Icon = UriResources.Images.Copy16;
                contextMenu.AddMenuItem(m_CopySelectedCellsMenuItem);
                m_CopySelectedCellsMenuItem.ItemClick += new EventHandler(ContextMenu_ItemClick);

                m_PasteSelectedCellsMenuItem = new ContextMenuItem(Localization.ContextMenu_Paste);
                m_PasteSelectedCellsMenuItem.Tag = ControlActionType.Paste;
                m_PasteSelectedCellsMenuItem.Icon = UriResources.Images.Paste16;
                contextMenu.AddMenuItem(m_PasteSelectedCellsMenuItem);
                m_PasteSelectedCellsMenuItem.ItemClick += new EventHandler(ContextMenu_ItemClick);

                m_CopyCellsSplitter = contextMenu.AddMenuSplitter();

                if (!CanEdit)
                {
                    m_PasteSelectedCellsMenuItem.Visibility = Visibility.Collapsed;
                    m_CopyValueMenuItem.Visibility = Visibility.Collapsed;
                    m_DeliveryValueMenuItem.Visibility = Visibility.Collapsed;
                    m_DeliveryValueSplitter.Visibility = Visibility.Collapsed;
                }
                else
                {
                    m_PasteSelectedCellsMenuItem.IsEnabled = EditMode & CanEdit;
                }

                if (!DrillThroughCells)
                    m_DrillThroughMenuItem.Visibility = Visibility.Collapsed;
            }

            item = new CheckedContectMenuItem(Localization.ContextMenu_AutoWidthColumns);
            item.Tag = ControlActionType.AutoWidth;
            (item as CheckedContectMenuItem).IsChecked = AutoWidthColumns;
            contextMenu.AddMenuItem(item);
            item.ItemClick += new EventHandler(ContextMenu_ItemClick);

            #region Подменю для управления режимом реорганизации данных
            CustomContextMenu subMenu1 = new CustomContextMenu();
            item = new CheckedContectMenuItem(Localization.ContextMenu_DataReorganizationType_None);
            item.Tag = ControlActionType.DataReorganizationType_None;
            subMenu1.AddMenuItem(item);
            item.ItemClick += new EventHandler(ContextMenu_ItemClick);

            item = new CheckedContectMenuItem(Localization.ContextMenu_DataReorganizationType_MergeNeighbors);
            item.Tag = ControlActionType.DataReorganizationType_MergeNeighbors;
            subMenu1.AddMenuItem(item);
            item.ItemClick += new EventHandler(ContextMenu_ItemClick);

            item = new CheckedContectMenuItem(Localization.ContextMenu_DataReorganizationType_HitchToParent);
            item.Tag = ControlActionType.DataReorganizationType_HitchToParent;
            subMenu1.AddMenuItem(item);
            item.ItemClick += new EventHandler(ContextMenu_ItemClick);

            item = new ContextMenuItem(Localization.ContextMenu_DataReorganizationType);
            item.Tag = ControlActionType.DataReorganizationType;
            item.SubMenu = subMenu1;
            contextMenu.AddMenuItem(item);
            #endregion

            item = new ContextMenuItem(Localization.ContextMenu_ShowMDX);
            item.Tag = ControlActionType.ShowMDX;
            item.Icon = UriResources.Images.Mdx16;
            contextMenu.AddMenuItem(item);
            item.ItemClick += new EventHandler(ContextMenu_ItemClick);

            if (areaType == AreaType.RowsArea || areaType == AreaType.ColumnsArea)
            {
                item = new ContextMenuItem(Localization.ContextMenu_CustomProperties);
                item.Tag = ControlActionType.ShowAttributes;
                item.Icon = UriResources.Images.MemberProperty16;
                contextMenu.AddMenuItem(item);
                item.ItemClick += new EventHandler(ContextMenu_ItemClick);
            }

            item = new ContextMenuItem(Localization.ContextMenu_Properties);
            item.Tag = ControlActionType.ShowProperties;
            item.Icon = UriResources.Images.LevelProperty16;
            contextMenu.AddMenuItem(item);
            item.ItemClick += new EventHandler(ContextMenu_ItemClick);

            return contextMenu;
        }

        void contextMenu_Closed(object sender, EventArgs e)
        {
            TooltipManager.IsPaused = false;
        }

        void contextMenu_Opened(object sender, EventArgs e)
        {
            TooltipManager.IsPaused = true;
        }

        void ContextMenu_ItemClick(object sender, EventArgs e)
        {
            ContextMenuItem item = sender as ContextMenuItem;
            if (item != null && item.Tag != null)
            {
                if (item.Tag is ControlActionType)
                {
                    if ((ControlActionType)(item.Tag) == ControlActionType.AutoWidth)
                    {
                        AutoWidthColumns = !AutoWidthColumns;
                    }
                }

                // Если меню для элемента
                MemberControl member_Control = ContextMenu.Tag as MemberControl;
                if (member_Control != null)
                {
                    if (item.Tag is MemberActionType)
                    {
                        member_Control.Raise_ExecuteMemberAction((MemberActionType)(item.Tag));
                        return;
                    }
                    if (item.Tag is ControlActionType)
                    {
                        Raise_PerformControlAction((ControlActionType)(item.Tag), member_Control);
                        return;
                    }
                }

                // Если меню для ячейки
                if (ContextMenu == m_Cells_ContextMenu)
                {
                    CellControl cell_Control = ContextMenu.Tag as CellControl;
                    if (item.Tag is ControlActionType)
                    {
                        // Если меню вызвано за пределами области ячеек, то будет относиться к ячейке с фокусом
                        CellInfo info = cell_Control != null ? cell_Control.Cell : FocusedCellView;
                        Raise_PerformControlAction((ControlActionType)(item.Tag), info);
                    }
                }
            }
        }
        #endregion

        void InitializeFocusedCell(CellControl old_FocusedCell)
        {
            if (old_FocusedCell != null)
            {
                old_FocusedCell.IsFocused = false;

                IDictionary<String, MemberInfo> old_tuple = new Dictionary<String, MemberInfo>();
                if (old_FocusedCell != null && old_FocusedCell.Cell != null)
                {
                    old_tuple = old_FocusedCell.Cell.GetTuple();
                }

                CellControl new_FocusedCell = null;

                // Пытаемся получить ячейку на которую хотим установить фокус
                for (int i = 0; i < m_CellControls_Cache.Columns_Size; i++)
                {
                    if (new_FocusedCell != null)
                        break;
                    for (int j = 0; j < m_CellControls_Cache.Rows_Size; j++)
                    {
                        CellControl cell_Control = m_CellControls_Cache.RemoveAt(i, j);
                        if (cell_Control != null && cell_Control.Cell != null)
                        {
                            if (cell_Control.Cell.CompareByTuple(old_tuple))
                            {
                                new_FocusedCell = cell_Control;
                                break;
                            }
                        }
                    }
                }

                if (new_FocusedCell != null)
                {
                    //FocusedCell = new_FocusedCell;
                }
            }
        }

        Dictionary<CellInfo, CellControl> m_CellControls_Dict = new Dictionary<CellInfo, CellControl>();

        public CellControl FocusedCell
        {
            get { return GetCellControl(FocusedCellView); }
        }

        CellControl GetCellControl(CellInfo view)
        {
            if (view != null)
            {
                if (m_CellControls_Dict.ContainsKey(view))
                    return m_CellControls_Dict[view];
            }
            return null;
        }

        #region Навигация по ячейкам 

        private SelectionManager<CellInfo> m_SelectionManager = new SelectionManager<CellInfo>();

        /// <summary>
        /// Менеджер для работы с выделением ячеек
        /// </summary>
        private SelectionManager<CellInfo> SelectionManager
        {
            get
            {
                return m_SelectionManager;
            }
        }

        /// <summary>
        /// Список выбранных ячеек
        /// </summary>
        public IList<CellInfo> Selection
        {
            get
            {
                return SelectionManager.GetSelection();
            }
        }

        /// <summary>
        /// Обновляет состояние элементов для ячеек, чтобы выбранные ячейки были выделены
        /// </summary>
        void RefreshSelectedCells()
        {
            if (Selection != null)
            {
                foreach (CellControl cell_Control in m_CellControls_Dict.Values)
                {
                    if (cell_Control.Cell != null)
                    {
                        if (Selection.Contains(cell_Control.Cell))
                        {
                            cell_Control.IsSelected = true;
                        }
                        else
                        {
                            cell_Control.IsSelected = false;
                        }
                    }
                    else
                    {
                        // Если в ячейке информации не содержится, то она однозначно не может быть выбранной
                        cell_Control.IsSelected = false;
                    }
                }
            }
        }

        private CellInfo m_FocusedCellView = null;
        CellInfo FocusedCellView
        {
            get { return m_FocusedCellView; }
            set {
                // Предыдущую ячейку с фокусом отменяем
                if (FocusedCell != null)
                {
                    EndEdit();
                    FocusedCell.IsFocused = false;
                }


                //if ((Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.Shift)
                //{
                //    SelectionManager.ClearSelection();
                //}
                SelectionManager.ClearSelection();

                m_FocusedCellView = value;
                m_PrevAreaBorderCell = m_FocusedCellView;

                if (FocusedCell != null)
                    FocusedCell.IsFocused = value != null;

                if (m_FocusedCellView != null)
                {
                    SelectionManager.AddSelectionArea(new List<CellInfo>() { m_FocusedCellView });  
                }
            }
        }


        //private CellControl m_FocusedCell = null;
        //CellControl FocusedCell
        //{
        //    get { return m_FocusedCell; }
        //    set
        //    {
        //        if (m_FocusedCell != value)
        //        {
        //            //FAST if (value != null)
        //            //{
        //            //    // Если не нажат Ctrl то необходимо сбросить список выделенных ячеек
        //            //    if ((Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.Control)
        //            //        SelectionManager.ClearSelection();
        //            //}
        //            //else
        //            //{
        //            //    SelectionManager.ClearSelection();
        //            //}

        //            // Предыдущую ячейку с фокусом отменяем
        //            CellControl old_focusedCell = m_FocusedCell;
        //            if (m_FocusedCell != null)
        //            {
        //                EndEdit();
        //                m_FocusedCell.IsFocused = false;
        //            }

        //            // Фокус на новую ячейку
        //            m_FocusedCell = value;
        //            if (m_FocusedCell != null)
        //            {
        //                m_FocusedCell.IsFocused = true;
        //                //FAST SelectionManager.AddSelectionArea(new List<CellControl>() { m_FocusedCell });
        //            }

        //            Raise_FocusedCellChanged(old_focusedCell, m_FocusedCell);
        //        }
        //    }
        //}

        void CellControl_MouseDoubleClick(object sender, EventArgs e)
        {
            BeginEdit();
        }

        void OnCellControlMouseDown(CellControl cell)
        {
            // Если нажат Shift и ячейка ч фокусом уже есть, то нужно изменить выбранную область,
            // Ячейка с фокусом при этои не меняется
            if (((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift) && FocusedCellView != null)
            {
                SelectionManager.ChangeSelectionArea(ProcessCellsArea(FocusedCellView, cell.Cell));
                m_PrevAreaBorderCell = cell.Cell;
            }
            else
            {
                FocusedCellView = cell.Cell;
            }
        }

        private bool IsNumericKey(KeyEventArgs args)
        {
            if (args != null)
            {
                switch (args.Key)
                {
                    case Key.NumPad0:
                    case Key.NumPad1:
                    case Key.NumPad2:
                    case Key.NumPad3:
                    case Key.NumPad4:
                    case Key.NumPad5:
                    case Key.NumPad6:
                    case Key.NumPad7:
                    case Key.NumPad8:
                    case Key.NumPad9:
                    case Key.Decimal:
                    case Key.D0:
                    case Key.D1:
                    case Key.D2:
                    case Key.D3:
                    case Key.D4:
                    case Key.D5:
                    case Key.D6:
                    case Key.D7:
                    case Key.D8:
                    case Key.D9:
                         return true;
                    case Key.Subtract:
                    case Key.Add:
                        if ((Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.Control &&
                            (Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.Shift)
                            return true;
                        break;
                }

                switch (args.PlatformKeyCode)
                {
                    case 189:   // "Минус" на основной клаве
                        return true;
                }
            }
            return false;
        }

        String GetString(Key key)
        {
            String res = null;
            switch (key)
            {
                case Key.NumPad0:
                case Key.D0:
                    res = "0";
                    break;
                case Key.NumPad1:
                case Key.D1:
                    res = "1";
                    break;
                case Key.NumPad2:
                case Key.D2:
                    res = "2";
                    break;
                case Key.NumPad3:
                case Key.D3:
                    res = "3";
                    break;
                case Key.NumPad4:
                case Key.D4:
                    res = "4";
                    break;
                case Key.NumPad5:
                case Key.D5:
                    res = "5";
                    break;
                case Key.NumPad6:
                case Key.D6:
                    res = "6";
                    break;
                case Key.NumPad7:
                case Key.D7:
                    res = "7";
                    break;
                case Key.NumPad8:
                case Key.D8:
                    res = "8";
                    break;
                case Key.NumPad9:
                case Key.D9:
                    res = "9";
                    break;
                case Key.Decimal:
                    res = ".";
                    break;
                case Key.Add:
                    break;
                case Key.Subtract:
                    res = "-";
                    break;
            }
            return res;
        }

        /// <summary>
        /// Максимальный индекс контрола для элемента в области строк. 
        /// </summary>
        int Rows_Max_LovestMember_Index
        {
            get
            {
                return RowsArea_LovestMemberControls.Count > 0 ? RowsArea_LovestMemberControls.Count - 1 : 0;
            }
        }

        /// <summary>
        /// Максимальный индекс контрола для элемента в области колонок. 
        /// </summary>
        int Columns_Max_LovestMember_Index
        {
            get
            {
                return ColumnsArea_LovestMemberControls.Count > 0 ? ColumnsArea_LovestMemberControls.Count - 1 : 0;
            }
        }

        bool m_AxisIsRotated = false;
        /// <summary>
        /// Признак поворота осей
        /// </summary>
        public bool AxisIsRotated
        {
            get { return m_AxisIsRotated; }
            set { m_AxisIsRotated = value; }
        }

        /// <summary>
        /// Проверяет возможность выполнения для указанной ячейки дрилл-операций по указанной оси
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="axis"></param>
        /// <returns></returns>
        bool TestCellToInteractive(CellInfo cell, int axis, MemberActionType action)
        {
            MemberInfo member = null;
            if(cell != null && (axis ==0 || axis == 1))
            {
                if(axis == 0)
                {
                    if (Columns_IsInteractive)
                    {
                        member = cell.ColumnMember;
                    }
                }

                if (axis == 1)
                {
                    if (Rows_IsInteractive)
                    {
                        member = cell.RowMember;
                    }
                }

                if (member != null)
                {
                    if ((action == MemberActionType.Expand && member.DrilledDown == false) ||
                        (action == MemberActionType.Collapse && member.DrilledDown == true))
                    {
                        // "+/-"-операции имеют смысл только для элемента, у которого есть дочерние
                        if (member.ChildCount > 0)
                            return true;
                    }
                    if (action == MemberActionType.DrillDown)
                        return true;
                }
            }
            return false;
        }

        void SpanCellsAreaControl_KeyDown(object sender, KeyEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.Shift)
                m_PrevAreaBorderCell = null;

            // Навигацию по ячейкам реализуем сами. Для того чтобы событие не шло дальше к скроллерам e.Handled = true;
            // Кнопки Left и Right в сочетании с Ctrl и Ctrl+Shift используются для навигации по записям истории
            // Ctrl+PLUS - раскрытие элемента в области строк
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                switch (e.Key)
                {
                    case Key.Left:
                    case Key.Right:
                        return;
                }   
            }
            
            switch (e.Key)
            {
                case Key.Up:
                case Key.Down:
                //case Key.Left:
                //case Key.Right:
                case Key.PageUp:
                case Key.PageDown:
                case Key.Home:
                case Key.End:
                    e.Handled = true;
                    break;
            }

            if (FocusedCellView != null)
            {
                if (FocusedCell != null)
                {
                    if (!FocusedCell.IsEditing && IsNumericKey(e))
                    {
                        // Начинаем редактирование, чистим значение если есть
                        if (BrowserHelper.IsMozilla)
                        {
                            // firefox проглатывает первый символ. Чтобы этот символ не потерялся, передаем его
                            BeginEdit(GetString(e.Key));
                        }
                        else
                        {
                            BeginEdit(String.Empty);
                        }
                        return;
                    }
                }

                // Определяем ячейку которая на данный момент является текущей, т.е. относительно ее будет проводиться смещение
                CellInfo currentCell = FocusedCellView;

                // При удержании шифта текущая ячейка не совпадает с фокусной если выбранная область содержит более одного элемента
                if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift && m_PrevAreaBorderCell != null)
                {
                    currentCell = m_PrevAreaBorderCell;
                }

                if (FocusedCellView != null)
                {
                    MemberActionType action = MemberActionType.Expand;
                    int axisNum = -1;
                    switch (e.Key)
                    {
                        case Key.Add:
                            action = MemberActionType.Expand;
                            // Ctrl+PLUS - раскрытие элемента в области строк
                            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control &&
                                (Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.Shift)
                            {
                                axisNum = 1;
                            }
                            // Ctrl+Shift+PLUS - раскрытие элемента в области колонок
                            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control &&
                                (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                            {
                                axisNum = 0;
                            }
                            break;
                        case Key.Subtract:
                            action = MemberActionType.Collapse;
                            // Ctrl+MINUS - свернуть элемент в области строк
                            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control &&
                                (Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.Shift)
                            {
                                axisNum = 1;
                            }
                            // Ctrl+Shift+MINUS - свернуть элемент в области колонок
                            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control &&
                                (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                            {
                                axisNum = 0;
                            }
                            break;
                        case Key.Enter:
                            action = MemberActionType.DrillDown;
                            // Ctrl+ENTER - детализация элемента в области строк
                            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control &&
                                (Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.Shift)
                            {
                                axisNum = 1;
                            }
                            // Ctrl+Shift+ENTER - детализация элемента в области колонок
                            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control &&
                                (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                            {
                                axisNum = 0;
                            }
                            break;
                    }

                    if (axisNum == 0 || axisNum == 1)
                    {
                        // Проверяем на возможность выполнения действия для данной ячейки
                        if (TestCellToInteractive(FocusedCellView, axisNum, action))
                        {
                            MemberInfo member = FocusedCellView.RowMember;
                            if (axisNum == 0)
                                member = FocusedCellView.ColumnMember;
                            Raise_ExecuteMemberAction(new MemberActionEventArgs(axisNum, member, action));
                            e.Handled = true;
                            return;
                        }
                    }
                }

                // Заканчиваем редактирование 
                switch (e.Key)
                {
                    case Key.Up:
                    case Key.Down:
                    case Key.Enter:
                    case Key.Left:
                    case Key.Right:
                    case Key.PageUp:
                    case Key.PageDown:
                    case Key.Home:
                    case Key.End:
                        EndEdit();
                        break;
                    case Key.C:
                        if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                            Raise_PerformControlAction(ControlActionType.Copy, FocusedCellView);
                        break;
                    case Key.V:
                        if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                            Raise_PerformControlAction(ControlActionType.Paste, FocusedCellView);
                        break;
                    case Key.Insert:
                        if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                            Raise_PerformControlAction(ControlActionType.Copy, FocusedCellView);
                        if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                            Raise_PerformControlAction(ControlActionType.Paste, FocusedCellView);
                        break;
                    case Key.Escape:
                        CancelEdit();
                        break;
                    case Key.F2:
                        BeginEdit();
                        return;
                    case Key.Z:
                        // Ctrl+Z - откат значения
                        if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                        {
                            CancelEdit();
                            // Сообщаем контролу, что нужно отменить изменения для выделенной группы ячеек
                            Raise_UndoCellChanges();

                            foreach (CellControl cell_Control in m_CellControls_Dict.Values)
                            {
                                if (cell_Control.Cell != null)
                                {
                                    if (Selection.Contains(cell_Control.Cell))
                                    {
                                        LocalChanges.RemoveChange(cell_Control.Cell);
                                        cell_Control.UndoChanges();
                                    }
                                }
                            }
                            
                            e.Handled = true;
                            return;
                        }
                        break;
                }

                // Координаты ячейки в сетке
                int layout_row_index = -1;
                int layout_column_index = -1;

                if (currentCell != null && currentCell.CellDescr != null)
                {
                    //Axis0_Coord = currentCell.CellDescr.Axis0_Coord;
                    //Axis1_Coord = currentCell.CellDescr.Axis1_Coord;
                    layout_column_index = currentCell.CellsArea_Axis0_Coord;
                    layout_row_index = currentCell.CellsArea_Axis1_Coord;

                    //if (m_CellControls_Dict.ContainsKey(currentCell))
                    //{
                    //    m_CellControls_Cache.GetCoordinates(m_CellControls_Dict[currentCell], out layout_column_index, out layout_row_index);
                    //}
                    bool isNavigation = false;
                    bool isVisibleCell = m_CellControls_Dict.ContainsKey(currentCell);

                    CellControl cell = null;

                    // Навигация
                    int min_row_index = 0;
                    int min_column_index = 0;

                    int rows_scroll_value = 0;
                    int columns_scroll_value = 0;

                    // Ctrl+Shift+ENTER - детализация элемента в области колонок
                    if(e.Key == Key.Enter && 
                        (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control &&
                        (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                        return;

                    switch (e.Key)
                    {
                        case Key.Up:
                        case Key.PageUp:
                            isNavigation = true;
                            // Учитываем запрос с одной осью
                            if (m_LayoutProvider.PivotProvider.RowsArea.RowsCount == 0)
                            {
                                layout_row_index = -1;
                                break;
                            }
                            if (e.Key == Key.PageUp)
                            {
                                // Если индекст строки больше чем индекс первой полностью видимой, то скачем не на предыдущую страницу, а на первую видимую строку
                                if (layout_row_index - RowsArea_FirstVisible_Coordinate.Row > 0)
                                {
                                    layout_row_index = RowsArea_FirstVisible_Coordinate.Row;
                                }
                                else
                                {
                                    rows_scroll_value = Rows_VisibleRowsCount > 0 ? -1 * Rows_VisibleRowsCount : -1;
                                }
                            }
                            else
                            {
                                rows_scroll_value = -1;
                            }
                            layout_row_index += rows_scroll_value;

                            // Сдвиг вверх
                            if (isVisibleCell && layout_row_index < RowsArea_FirstVisible_Coordinate.Row)
                            {
                                if (ScrollVertical(rows_scroll_value) == true)
                                {
                                    //// Т.к. отскроллировали на одну позицию вверх, то нужно оставить неизменным координаты ячейки с фокусом. т.е. сместить ее вниз на 1
                                    //layout_row_index += rows_scroll_value;
                                }

                                // Если нажата  Key.PageUp то проскроллировав на количество видимых на экране ячеек мы корректно получим предыдущую страницу
                                // Фокус нужно установить на первую полностью видимую
                                if (e.Key == Key.PageDown)
                                {
                                    layout_row_index = RowsArea_FirstVisible_Coordinate.Row;
                                }
                            }

                            // Проверяем на минимум.
                            if (layout_row_index < min_row_index)
                                layout_row_index = min_row_index;

                            break;
                        case Key.Down:
                        case Key.Enter:
                        case Key.PageDown:
                            isNavigation = true;
                            // Учитываем запрос с одной осью
                            if (m_LayoutProvider.PivotProvider.RowsArea.RowsCount == 0)
                            {
                                layout_row_index = -1;
                                break;
                            }
                            if (e.Key == Key.PageDown)
                            {
                                // Если индекст строки меньше чем индекс последней полностью видимой, то скачем не на следующую страницу, а на последнюю видимую строку
                                if (layout_row_index < RowsArea_FirstVisible_Coordinate.Row + Rows_LastVisibleIndex)
                                {
                                    layout_row_index = RowsArea_FirstVisible_Coordinate.Row + Rows_LastVisibleIndex;
                                }
                                else
                                {
                                    rows_scroll_value = Rows_VisibleRowsCount > 0 ? Rows_VisibleRowsCount : 1;
                                }
                            }
                            else
                            {
                                rows_scroll_value = 1;
                            }
                            layout_row_index += rows_scroll_value;

                            // Сдвиг вниз
                            if (isVisibleCell && 
                                ( layout_row_index >= RowsArea_FirstVisible_Coordinate.Row + Rows_VisibleRowsCount/* || 
                                layout_row_index > Rows_Max_LovestMember_Index*/))
                            {
                                if (ScrollVertical(rows_scroll_value) == true)
                                {
                                    //// Т.к. отскроллировали на одну позицию вниз, то нужно оставить неизменным координаты ячейки с фокусом. т.е. сместить ее вверх на 1
                                    //layout_row_index -= rows_scroll_value;
                                }

                                // Если нажата  Key.PageDown то проскроллировав на количество видимых на экране ячеек мы корректно получим следующую страницу
                                // Но количество строк на ней может быть не таким как на предыдущей (т.к. высоты строк разные)
                                // Поэтому фокус нужно установить на последнюю полностью видимую
                                if (e.Key == Key.PageDown)
                                {
                                    layout_row_index = RowsArea_FirstVisible_Coordinate.Row + Rows_LastVisibleIndex;
                                }
                            }

                            //// Проверяем на максимум. Т.к. в случае с ПОСЛЕДНЕЙ нижней ячейкой под вычисленной позицией фокуса ячейки может не оказаться.
                            if (layout_row_index >= m_LayoutProvider.PivotProvider.RowsArea.RowsCount)
                                layout_row_index = m_LayoutProvider.PivotProvider.RowsArea.RowsCount > 0 ? m_LayoutProvider.PivotProvider.RowsArea.RowsCount - 1 : 0;

                            break;
                        case Key.Left:
                        case Key.Home:
                            isNavigation = true;
                            // Учитываем запрос когда на оси 0 элементов нет, а ячейка есть
                            if (m_LayoutProvider.PivotProvider.ColumnsArea.ColumnsCount == 0)
                            {
                                layout_column_index = -1;
                                break;
                            }
                            if (e.Key == Key.Home)
                            {
                                // Переход на самую первую ячейку в строке
                                columns_scroll_value = -1 * (layout_column_index + ColumnsArea_FirstVisible_Coordinate.Column);
                            }
                            else
                            {
                                columns_scroll_value = -1;
                            }
                            layout_column_index += columns_scroll_value;

                            // Сдвиг влево
                            if (isVisibleCell && layout_column_index < ColumnsArea_FirstVisible_Coordinate.Column)
                            {
                                if (ScrollHorizontal(columns_scroll_value) == true)
                                {
                                    //// Т.к. отскроллировали на одну позицию влево, то нужно оставить неизменным координаты ячейки с фокусом. т.е. сместить ее вправо на 1
                                    //layout_column_index += columns_scroll_value;
                                }
                            }

                            // Если нажата  Key.Home то фокус нужно установить на первую ячейку
                            if (e.Key == Key.Home)
                            {
                                layout_column_index = min_column_index;
                            }

                            // Проверяем на минимум.
                            if (layout_column_index < min_column_index)
                                layout_column_index = min_column_index;

                            break;
                        case Key.Right:
                        case Key.End:
                            isNavigation = true;
                            // Учитываем запрос когда на оси 0 элементов нет, а ячейка есть
                            if (m_LayoutProvider.PivotProvider.ColumnsArea.ColumnsCount == 0)
                            {
                                layout_column_index = -1;
                                break;
                            }
                            if (e.Key == Key.End)
                            {
                                // Переход на самую последнюю ячейку в строке
                                columns_scroll_value = Convert.ToInt32(m_LayoutProvider.ColumnsLayout.Columns_Size);
                            }
                            else
                            {
                                columns_scroll_value = 1;
                            }
                            layout_column_index += columns_scroll_value;

                            // Сдвиг вправо
                            if (isVisibleCell &&
                                (layout_column_index >= ColumnsArea_FirstVisible_Coordinate.Column + Columns_VisibleColumnsCount/* || 
                                layout_column_index > Columns_Max_LovestMember_Index*/))
                            {
                                if (ScrollHorizontal(columns_scroll_value) == true)
                                {
                                    //// Т.к. отскроллировали на одну позицию вправо, то нужно оставить неизменным координаты ячейки с фокусом. т.е. сместить ее влево на 1
                                    //layout_column_index -= columns_scroll_value;
                                }

                                if (e.Key == Key.End)
                                    layout_column_index = ColumnsArea_FirstVisible_Coordinate.Column + Columns_Max_LovestMember_Index;
                            }

                            // Проверяем на максимум. Т.к. в случае с ПОСЛЕДНЕЙ правой ячейкой под вычисленной позицией фокуса ячейки может не оказаться.
                            if (layout_column_index >= m_LayoutProvider.PivotProvider.ColumnsArea.ColumnsCount)
                                layout_column_index = m_LayoutProvider.PivotProvider.ColumnsArea.ColumnsCount > 0 ? m_LayoutProvider.PivotProvider.ColumnsArea.ColumnsCount - 1 : 0;

                            break;
                    }

                    // Если навигация, то прячем подсказку
                    if (isNavigation)
                    {
                        TooltipManager.Hide();
                    }

                    if ((layout_column_index >= 0 || (layout_column_index == -1 && m_LayoutProvider.PivotProvider.ColumnsArea.ColumnsCount == 0)) &&    // Учитываем запросы когда ось 0 пустая, а ячейка есть
                        (layout_row_index >= 0 || (layout_row_index == -1 && m_LayoutProvider.PivotProvider.RowsArea.RowsCount == 0)))                  // Учитываем запросы с одной осью
                    {
                        if (isNavigation)
                        {
                            // Координаты ячейки в сетке
                            int Axis0_Coord = -1;
                            int lovest_column_index = layout_column_index - CellsArea_FirstVisible_Coordinate.Column;
                            if (lovest_column_index >= 0 && m_ColumnsArea_LovestMemberControls.Count > lovest_column_index)
                            {
                                Axis0_Coord = m_ColumnsArea_LovestMemberControls[lovest_column_index].Member.MemberIndexInAxis;
                            }
                            int Axis1_Coord = -1;
                            int lovest_row_index = layout_row_index - CellsArea_FirstVisible_Coordinate.Row;
                            if (lovest_row_index >= 0 && m_RowsArea_LovestMemberControls.Count > lovest_row_index)
                            {
                                Axis1_Coord = m_RowsArea_LovestMemberControls[lovest_row_index].Member.MemberIndexInAxis;
                            }

                            CellInfo cell_Info = m_LayoutProvider.PivotProvider.Provider.GetCellInfo(Axis0_Coord, Axis1_Coord);
                            //cell = m_CellControls_Cache[layout_column_index, layout_row_index];

                            // устанавливаем фокус на новую ячейку либо если нажат Shift меняем выбранную область 
                            if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift &&
                                (Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.Control)    // важно при попытке сделать Drilldown в области колонок (Ctrl_Shift+Enter)
                            {
                                m_PrevAreaBorderCell = cell_Info;
                                // При удержании Shift выбирается область
                                SelectionManager.ChangeSelectionArea(ProcessCellsArea(FocusedCellView, cell_Info));
                            }
                            else
                            {
                                FocusedCellView = cell_Info;
                            }
                        }
                    }
                }
            }
        }

        public void GoToFocusedCell()
        {
            if (FocusedCellView != null)
            {
                int column_Index = FocusedCellView.CellDescr.Axis0_Coord;
                int row_Index = FocusedCellView.CellDescr.Axis1_Coord;

                m_HorizontalScroll.Value = column_Index;
                m_VerticalScroll.Value = row_Index;

                ColumnsArea_FirstVisible_Coordinate.Column = Convert.ToInt32(m_HorizontalScroll.Value);
                RowsArea_FirstVisible_Coordinate.Row = Convert.ToInt32(m_VerticalScroll.Value);
                Refresh(RefreshType.Refresh);
            }
        }

        double GetEstimatedRowSize(int memberIndex)
        {
            double size = 0;
            // Пытаемся получить высоту данной строки по уникальному имени элемента, который будет в области строк на последней линии
            if (memberIndex < m_LayoutProvider.RowsLayout.Rows_Size && m_LayoutProvider.RowsLayout.Columns_Size > 0)
            {
                LayoutCellWrapper cell_wrapper = m_LayoutProvider.RowsLayout[m_LayoutProvider.RowsLayout.Columns_Size - 1, memberIndex];
                if (cell_wrapper != null)
                {
                    MemberLayoutItem member_item = cell_wrapper.Items[cell_wrapper.Items.Count - 1] as MemberLayoutItem;
                    if (member_item != null && m_MembersHeightes.ContainsKey(member_item.PivotMember.Member.UniqueName))
                    {
                        return m_MembersHeightes[member_item.PivotMember.Member.UniqueName];
                    }
                    else
                    {
                        return Math.Round(DEFAULT_HEIGHT * Scale);
                    }
                }
            }
            return size;
        }

        double GetEstimatedColumnSize(int memberIndex)
        {
            double size = 0;
            // Пытаемся получить ширину данной колонки по уникальному имени элемента, который будет в области колонок на последней линии
            if (memberIndex < m_LayoutProvider.ColumnsLayout.Columns_Size && m_LayoutProvider.ColumnsLayout.Rows_Size > 0)
            {
                LayoutCellWrapper cell_wrapper = m_LayoutProvider.ColumnsLayout[memberIndex, m_LayoutProvider.ColumnsLayout.Rows_Size - 1];
                if (cell_wrapper != null)
                {
                    MemberLayoutItem member_item = cell_wrapper.Items[cell_wrapper.Items.Count - 1] as MemberLayoutItem;
                    if (member_item != null && m_MembersWidthes.ContainsKey(member_item.PivotMember.Member.UniqueName))
                    {
                        return m_MembersWidthes[member_item.PivotMember.Member.UniqueName];
                    }
                    else
                    {
                        if (AutoWidthColumns && m_CellSetProvider != null)
                        {
                            if (m_AnalyticInfo != null &&
                               member_item != null && member_item.PivotMember != null && member_item.PivotMember.Member != null)
                            {
                                // Желаемая ширина элемента
                                double member_width = Math.Round(DEFAULT_WIDTH * Scale);
                                if (m_AnalyticInfo != null)
                                    member_width = m_AnalyticInfo.GetEstimatedColumnSizeForColumnsArea(memberIndex) + 10 + 10 * Scale;    // 10-для красоты, 10* - на плюсики
                                // Желаемая ширина ячейки
                                MinMaxDescriptor<CellInfo> minmax = null;
                                if(m_AnalyticInfo.Cells_DisplayValueLength_MinMax.ContainsKey(member_item.PivotMember.Member))
                                    minmax = m_AnalyticInfo.Cells_DisplayValueLength_MinMax[member_item.PivotMember.Member];
                                double cell_width = DEFAULT_WIDTH;
                                if (minmax != null && minmax.Max != null)
                                {
                                    cell_width = StringExtensions.Measure(minmax.Max.DisplayValue, DefaultFontSize * Scale, null).Width + 10 + 5 * Scale; // 10-для красоты, 5* - отступ текста слева
                                }
                                return Math.Round(Math.Max(cell_width, member_width));
                            }
                        }
                        return Math.Round(DEFAULT_WIDTH * Scale);
                    }
                }
            }
            return size;
        }

        int Columns_LastPage_ColumnsCount
        {
            get
            {
                int count = 0;
                double width = 0;
                for (int i = m_LayoutProvider.ColumnsLayout.Columns_Size - 1; i >= 0; i--)
                {
                    // Получаем СУММАРНУЮ ширину области с учетом текущей колонки
                    width += GetEstimatedColumnSize(i);
                    
                    // Если вычисленная суммарная ширина превышает видимый размер, топредел достигнут
                    if (width > Columns_VisibleWidth)
                    {
                        break;
                    }

                    count++;
                }
                return count;
            }
        }

        int Rows_LastPage_ColumnsCount
        {
            get
            {
                int count = 0;
                double height = 0;
                for (int i = m_LayoutProvider.RowsLayout.Rows_Size - 1; i >= 0; i--)
                {
                    // Получаем СУММАРНУЮ высоту области с учетом текущей строки
                    height += GetEstimatedRowSize(i);

                    // Если вычисленная суммарная высота превышает видимый размер, топредел достигнут
                    if (height > Rows_VisibleHeight)
                    {
                        break;
                    }

                    count++;
                }
                return count;
            }
        }

        bool ScrollHorizontal(double val)
        {
            double old_val = m_HorizontalScroll.Value;
            m_HorizontalScroll.Value += val;
            if (old_val != m_HorizontalScroll.Value)
            {
                //m_HorizontalScroll_Timer.Stop();
                ColumnsArea_FirstVisible_Coordinate.Column = Convert.ToInt32(m_HorizontalScroll.Value);
                //m_HorizontalScroll_Timer.Begin();
                Refresh(RefreshType.RefreshByColumns);
                return true;
            }
            return false;
        }

        bool ScrollVertical(double val)
        {
            double old_val = m_VerticalScroll.Value;
            m_VerticalScroll.Value += val;
            if (old_val != m_VerticalScroll.Value)
            {
                //m_VericalScroll_Timer.Stop();
                RowsArea_FirstVisible_Coordinate.Row = Convert.ToInt32(m_VerticalScroll.Value);
                //m_VericalScroll_Timer.Begin();
                Refresh(RefreshType.RefreshByRows);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Индекс последней полностью видимой колонки
        /// </summary>
        int Columns_LastVisibleIndex
        {
            get { return Columns_VisibleColumnsCount > 0 ? Columns_VisibleColumnsCount - 1 : 0; }
        }

        /// <summary>
        /// Индекс последней полностью видимой строки
        /// </summary>
        int Rows_LastVisibleIndex
        {
            get { return Rows_VisibleRowsCount > 0 ? Rows_VisibleRowsCount - 1 : 0; }
        }


        int Columns_VisibleColumnsCount
        {
            get {
                for (int i = 0; i <= ColumnsArea_ColumnsCount; i++)
                {
                    int layout_column_index = ColumnsArea_BeginColumnIndex + i;
                    // Пытаемся ПОЛУЧИТЬ данную колонку в гриде
                    ColumnDefinition current_column = null;
                    if (layout_column_index  < ItemsLayoutRoot.ColumnDefinitions.Count)
                    {
                        current_column = ItemsLayoutRoot.ColumnDefinitions[layout_column_index];
                    }

                    // Получаем СУММАРНУЮ ширину области с учетом текущей колонки
                    double width = GetAreaWidth(ColumnsArea_BeginColumnIndex, layout_column_index);
                    // Если вычисленная суммарная ширина превышает видимый размер, то значит количество видимых известно
                    if (width > Columns_VisibleWidth)
                    {
                        return i;
                    }
                }

                return ColumnsArea_ColumnsCount;
            }
        }

        int Rows_VisibleRowsCount
        {
            get
            {
                for (int i = 0; i <= RowsArea_RowsCount; i++)
                {
                    int layout_rows_index = RowsArea_BeginRowIndex + i;
                    // Пытаемся ПОЛУЧИТЬ данную строку в гриде
                    RowDefinition current_row = null;
                    if (layout_rows_index < ItemsLayoutRoot.RowDefinitions.Count)
                    {
                        current_row = ItemsLayoutRoot.RowDefinitions[layout_rows_index];
                    }

                    // Получаем СУММАРНУЮ высоту области с учетом текущей строки
                    double height = GetAreaHeight(RowsArea_BeginRowIndex, layout_rows_index);
                    // Если вычисленная суммарная высота превышает видимый размер, то значит количество видимых известно
                    if (height > Rows_VisibleHeight)
                    {
                        return i;
                    }
                }

                return RowsArea_RowsCount;
            }
        }

        #endregion Навигация по ячейкам

        #region Работа с областями ячеек
        bool m_LeftButtonPressed = false;
        //public bool LeftButtonPressed
        //{
        //    get
        //    {
        //        return m_LeftButtonPressed;
        //    }
        //}

        void CellsAreaControl_MouseLeave(object sender, MouseEventArgs e)
        {
            HtmlPage.Document.DetachEvent("onmousedown", new EventHandler<HtmlEventArgs>(Document_OnMouseDown));
            HtmlPage.Document.DetachEvent("onmouseup", new EventHandler<HtmlEventArgs>(Document_OnMouseUp));
            HtmlPage.Document.DetachEvent("onmouseout", new EventHandler<HtmlEventArgs>(Document_OnMouseLeave));
            if (m_VericalMouseWhellSupport != null)
                m_VericalMouseWhellSupport.ScrollAlways = false;
            if (m_HorizontalMouseWhellSupport != null)
                m_HorizontalMouseWhellSupport.ScrollAlways = false;
        }

        void CellsAreaControl_MouseEnter(object sender, MouseEventArgs e)
        {
            HtmlPage.Document.AttachEvent("onmousedown", new EventHandler<HtmlEventArgs>(Document_OnMouseDown));
            HtmlPage.Document.AttachEvent("onmouseup", new EventHandler<HtmlEventArgs>(Document_OnMouseUp));
            HtmlPage.Document.AttachEvent("onmouseout", new EventHandler<HtmlEventArgs>(Document_OnMouseLeave));
            if (m_VericalMouseWhellSupport != null)
                m_VericalMouseWhellSupport.ScrollAlways = true;
            if (m_HorizontalMouseWhellSupport != null)
                m_HorizontalMouseWhellSupport.ScrollAlways = true;
        }

        void Document_OnMouseLeave(object sender, HtmlEventArgs e)
        {
            m_LeftButtonPressed = false;
        }

        void Document_OnMouseDown(object sender, HtmlEventArgs e)
        {
            // Проверяем чтобы клик был в рамках области ячеек
            Point p = new Point(e.ClientX, e.ClientY);
            if (AgControlBase.GetSLBounds(this).Contains(p))
            {
                m_LeftButtonPressed = true;
                //// Клик правойкнопкой мыши приведет к установке фокуса на ячейку только если нет выделенной области
                //if (e.MouseButton == MouseButtons.Right)
                //{
                //    if (Selection.Count < 2)
                //    {
                //        CellControl cell = GetCellByPoint(p);
                //        if (cell != null)
                //        {
                //            OnCellControlMouseDown(cell);
                //        }
                //    }
                //}
            }
        }

        void Document_OnMouseUp(object sender, HtmlEventArgs e)
        {
            m_LeftButtonPressed = false;
        }

        void cell_control_MouseEnter(object sender, MouseEventArgs e)
        {
            if (m_LeftButtonPressed)
            {
                ChangeSelectionAreaTo(sender as CellControl);
            }
        }

        /// <summary>
        /// Ячейка, которая была текущей пока удерживался Shift и нажимались кнопки навигации
        /// Она поможет сохранить предыдущую границу области для множественного выбора ячеек
        /// </summary>
        CellInfo m_PrevAreaBorderCell = null;

        public void ChangeSelectionAreaTo(CellControl cell)
        {
            if (cell != null)
            {
                IList<CellInfo> area = ProcessCellsArea(FocusedCellView, cell.Cell);
                SelectionManager.ChangeSelectionArea(area);
            }
        }

        /// <summary>
        /// Формирует список ячеек, попдающих в область, на углах которой указанные ячейки
        /// </summary>
        /// <param name="cell1"></param>
        /// <param name="cell2"></param>
        /// <returns></returns>
        private IList<CellInfo> ProcessCellsArea(CellInfo cell1, CellInfo cell2)
        {
            IList<CellInfo> selectionArea = new List<CellInfo>();

            if (cell1 != null && cell2 != null)
            {
                int beginColumnIndex = Math.Min(cell1.ColumnMember.Sorted_MemberIndexInAxis, cell2.ColumnMember.Sorted_MemberIndexInAxis);
                int endColumnIndex = Math.Max(cell1.ColumnMember.Sorted_MemberIndexInAxis, cell2.ColumnMember.Sorted_MemberIndexInAxis);
                int beginRowIndex = Math.Min(cell1.RowMember.Sorted_MemberIndexInAxis, cell2.RowMember.Sorted_MemberIndexInAxis);
                int endRowIndex = Math.Max(cell1.RowMember.Sorted_MemberIndexInAxis, cell2.RowMember.Sorted_MemberIndexInAxis);

                //beginRowIndex и endRowIndex могут быть равны -1 (когда только одна ось в запросе)
                if (beginColumnIndex >= 0 &&
                    endColumnIndex >= 0 &&
                    ((beginRowIndex >= 0 && endRowIndex >= 0) || (beginRowIndex == -1 && endRowIndex >= -1)))
                {
                    for (int columnIndex = beginColumnIndex; columnIndex <= endColumnIndex; columnIndex++)
                    {
                        for (int rowIndex = beginRowIndex; rowIndex <= endRowIndex; rowIndex++)
                        {

                            CellInfo cell_Info = m_LayoutProvider.PivotProvider.Provider.GetCellInfo(m_LayoutProvider.PivotProvider.Provider.GetAxisCoord(0, columnIndex), 
                                m_LayoutProvider.PivotProvider.Provider.GetAxisCoord(1, rowIndex));
                            if (cell_Info != null)
                            {
                                if (m_CellControls_Dict.ContainsKey(cell_Info))
                                {
                                    CellControl cell_Control = m_CellControls_Dict[cell_Info];
                                    if (cell_Control != null)
                                    {
                                        cell_Control.IsSelected = true;
                                    }
                                }
                                selectionArea.Add(cell_Info);
                            }
                        }
                    }
                }
            }

            if (selectionArea.Count == 0)
            {
                if (cell1 != null)
                    selectionArea.Add(cell1);
                if (cell2 != null)
                    selectionArea.Add(cell2);
            }

            return selectionArea;
        }
        #endregion Работа с областями ячеек

        /// <summary>
        /// Добавляет в грид сплиттер, который меняет ширину
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="columnIndex"></param>
        /// <param name="rowIndex"></param>
        /// <returns></returns>
        protected GridSplitter Add_VertSplitter(Grid grid, int columnIndex, int rowIndex, int Span)
        {
            GridSplitter splitter_Vert = new GridSplitter();
            splitter_Vert.IsTabStop = false;
            grid.Children.Add(splitter_Vert);
            Grid.SetColumn(splitter_Vert, columnIndex);
            Grid.SetRow(splitter_Vert, rowIndex);
            Grid.SetRowSpan(splitter_Vert, Span);

            splitter_Vert.HorizontalAlignment = HorizontalAlignment.Right;
            splitter_Vert.VerticalAlignment = VerticalAlignment.Stretch;
            splitter_Vert.Width = PivotGridControl.SPLITTER_SIZE;

            /*GradientStopCollection stops = new GradientStopCollection();
            GradientStop stop0 = new GradientStop();
            stop0.Color = Colors.White;
            GradientStop stop1 = new GradientStop();
            stop1.Color = Colors.DarkGray;
            stop1.Offset = 1;
            stops.Add(stop0);
            stops.Add(stop1);
            splitter_Vert.Background = new LinearGradientBrush(stops, 0);*/
            splitter_Vert.Background = new SolidColorBrush(Colors.Transparent);

            //splitter_Vert.ShowsPreview = true;
            splitter_Vert.MouseLeftButtonUp += new MouseButtonEventHandler(splitter_Vert_MouseLeftButtonUp);
            //splitter_Vert.MouseMoved += new EventHandler(splitter_Vert_MouseMoved);
            return splitter_Vert;
        }

        void splitter_Vert_MouseMoved(object sender, EventArgs e)
        {
            Refresh(RefreshType.BuildEndRefresh);
        }


        void splitter_Vert_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            GridSplitter splitter = sender as GridSplitter;
            if (splitter != null)
            {
                int column = Grid.GetColumn(splitter);
                if (column > -1)
                {
                    if (column >= CellsArea_BeginColumnIndex)
                    {
                        int lovest_index = column - CellsArea_BeginColumnIndex;
                        if (lovest_index >= 0 && m_ColumnsArea_LovestMemberControls.Count > lovest_index)
                            m_MembersWidthes[m_ColumnsArea_LovestMemberControls[lovest_index].Member.UniqueName] = ItemsLayoutRoot.ColumnDefinitions[column].ActualWidth;
                    }
                    else
                    {
                        m_Rows_ColumnWidthes[column] = ItemsLayoutRoot.ColumnDefinitions[column].ActualWidth;
                    }
                }

                //MemberControl member = splitter.Tag as MemberControl;
                //if (member != null)
                //{
                //    int column = Grid.GetColumn(splitter);
                //    m_MembersWidthes[member.Member.UniqueName] = ItemsLayoutRoot.ColumnDefinitions[column].ActualWidth;
                //}
                //else
                //{
                //    int column = Grid.GetColumn(splitter);
                //    if (column >= 0)
                //    {
                //        m_Rows_ColumnWidthes[column] = ItemsLayoutRoot.ColumnDefinitions[column].ActualWidth;
                //    }
                //}
                Refresh(RefreshType.RefreshByColumns);
            }
        }

        /// <summary>
        /// Добавляет в грид сплиттер, который меняет высоту
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="columnIndex"></param>
        /// <param name="rowIndex"></param>
        /// <returns></returns>
        protected GridSplitter Add_HorzSplitter(Grid grid, int columnIndex, int rowIndex, int Span)
        {
            GridSplitter splitter_Horz = new GridSplitter();
            splitter_Horz.IsTabStop = false;
            grid.Children.Add(splitter_Horz);
            Grid.SetColumn(splitter_Horz, columnIndex);
            Grid.SetRow(splitter_Horz, rowIndex);
            Grid.SetColumnSpan(splitter_Horz, Span);

            splitter_Horz.HorizontalAlignment = HorizontalAlignment.Stretch;
            splitter_Horz.VerticalAlignment = VerticalAlignment.Bottom;
            splitter_Horz.Height = PivotGridControl.SPLITTER_SIZE;

            splitter_Horz.Background = new SolidColorBrush(Colors.Transparent);

            splitter_Horz.MouseLeftButtonUp += new MouseButtonEventHandler(splitter_Horz_MouseLeftButtonUp);
            return splitter_Horz;
        }

        void splitter_Horz_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            GridSplitter splitter = sender as GridSplitter;
            if (splitter != null)
            {
                int row = Grid.GetRow(splitter);
                if (row > -1)
                {
                    if (row >= CellsArea_BeginRowIndex)
                    {
                        int lovest_index = row - CellsArea_BeginRowIndex;
                        if (lovest_index >= 0 && m_RowsArea_LovestMemberControls.Count > lovest_index)
                            m_MembersHeightes[m_RowsArea_LovestMemberControls[lovest_index].Member.UniqueName] = ItemsLayoutRoot.RowDefinitions[row].ActualHeight;
                    }
                    else
                    {
                        m_Columns_RowsHeightes[row] = ItemsLayoutRoot.RowDefinitions[row].ActualHeight;
                    }
                }

                //MemberControl member = splitter.Tag as MemberControl;
                //if (member != null)
                //{
                //    int row = Grid.GetRow(splitter);
                //    m_MembersHeightes[member.Member.UniqueName] = ItemsLayoutRoot.RowDefinitions[row].ActualHeight;
                //}
                //else 
                //{
                //    int row = Grid.GetRow(splitter);
                //    if (row >= 0)
                //    {
                //        m_Columns_RowsHeightes[row] = ItemsLayoutRoot.RowDefinitions[row].ActualHeight;
                //    }
                //}
                Refresh(RefreshType.RefreshByRows);
            } 
        }

        protected void OnExecuteMemberAction(object sender, MemberActionEventArgs args)
        {
            // Если ячейка с фокусом находилось не в строке или не в колонке, для которой производится данная операция,
            // то фокус устанавливаем на первую ВИДИМУЮ (чтобы сохранить скроллы) ячейку в данной колонке или строке
            if (FocusedCellView != null)
            {
                int cell_column_indx = -1;
                int cell_row_indx = -1;
                if (FocusedCell != null)
                {
                    // Получаем позицию ячейки с фокусом в гриде
                    cell_column_indx = Grid.GetColumn(FocusedCell);
                    cell_row_indx = Grid.GetRow(FocusedCell);
                }
                else
                {
                    cell_column_indx = CellsArea_BeginColumnIndex;
                    cell_row_indx = CellsArea_BeginRowIndex;
                }

                MemberControl member_Control = sender as MemberControl;
                if (member_Control != null)
                {
                    // Получаем позицию данного контрола в гриде
                    int member_column_indx = Grid.GetColumn(member_Control);
                    int member_row_indx = Grid.GetRow(member_Control);

                    if (cell_column_indx > -1 &&
                        cell_row_indx > -1 &&
                        member_column_indx > -1 &&
                        member_row_indx > -1 &&
                        cell_column_indx != member_column_indx &&
                        cell_row_indx != member_row_indx)
                    {
                        if (member_Control is ColumnMemberControl)
                        {
                            CellControl new_focused = m_CellControls_Cache[member_column_indx - CellsArea_BeginColumnIndex, 0];
                            if (new_focused != null)
                                FocusedCellView = new_focused.Cell;
                            else
                                FocusedCellView = null;
                        }
                        if (member_Control is RowMemberControl)
                        {
                            CellControl new_focused = m_CellControls_Cache[0, member_row_indx - CellsArea_BeginRowIndex];
                            if (new_focused != null)
                                FocusedCellView = new_focused.Cell;
                            else
                                FocusedCellView = null;
                        }
                    }
                }
            }

            Raise_ExecuteMemberAction(args);
        }

        #region Экспорт-импорт размеров
        public PivotGridSizeInfo GetSizeInfo()
        {
            PivotGridSizeInfo sizeInfo = new PivotGridSizeInfo();

            sizeInfo.Scale = Scale;

            // Область колонок
            sizeInfo.ColumnsAreaSize = new MembersAreaSizeInfo();
            // Ширины элементов
            foreach (String uniqueName in m_MembersWidthes.Keys)
            {
                MemberSizeInfo member_size = new MemberSizeInfo(uniqueName, m_MembersWidthes[uniqueName]);
                sizeInfo.ColumnsAreaSize.MembersSize.Add(member_size);
            }
            // Высоты строк
            foreach (int indx in m_Columns_RowsHeightes.Keys)
            {
                LineSizeInfo line_size = new LineSizeInfo(indx, m_Columns_RowsHeightes[indx]);
                sizeInfo.ColumnsAreaSize.LinesSize.Add(line_size);
            }
            // Старая реализация - размеры всех строк
            //for (int i = PivotArea_BeginRowIndex, indx = 0; i < ColumnsArea_RowsCount; i++, indx++)
            //{
            //    if (i < ItemsLayoutRoot.RowDefinitions.Count)
            //    {
            //        RowDefinition current_row = ItemsLayoutRoot.RowDefinitions[i];
            //        LineSizeInfo line_size = new LineSizeInfo(indx, current_row.ActualHeight);
            //        sizeInfo.ColumnsAreaSize.LinesSize.Add(line_size);
            //    }
            //}

            // Область строк
            sizeInfo.RowsAreaSize = new MembersAreaSizeInfo();
            // Высоты элементов
            foreach (String uniqueName in m_MembersHeightes.Keys)
            {
                MemberSizeInfo member_size = new MemberSizeInfo(uniqueName, m_MembersHeightes[uniqueName]);
                sizeInfo.RowsAreaSize.MembersSize.Add(member_size);
            }
            // Ширины колонок
            foreach (int indx in m_Rows_ColumnWidthes.Keys)
            {
                LineSizeInfo line_size = new LineSizeInfo(indx, m_Rows_ColumnWidthes[indx]);
                sizeInfo.RowsAreaSize.LinesSize.Add(line_size);
            }
            // Старая реализация - размеры всех колонок
            //for (int i = PivotArea_BeginColumnIndex, indx = 0; i < RowsArea_ColumnsCount; i++, indx++)
            //{
            //    if (i < ItemsLayoutRoot.ColumnDefinitions.Count)
            //    {
            //        ColumnDefinition current_column = ItemsLayoutRoot.ColumnDefinitions[i];
            //        LineSizeInfo line_size = new LineSizeInfo(indx, current_column.ActualWidth);
            //        sizeInfo.RowsAreaSize.LinesSize.Add(line_size);
            //    }
            //}

            return sizeInfo;
        }

        public void SetSizeInfo(PivotGridSizeInfo sizeInfo)
        {

            // Область колонок
            m_MembersWidthes.Clear();
            // Ширины элементов
            foreach (MemberSizeInfo member_size in sizeInfo.ColumnsAreaSize.MembersSize)
            {
                if (!String.IsNullOrEmpty(member_size.MemberUniqueName) && member_size.Size >= 0)
                {
                    m_MembersWidthes.Add(member_size.MemberUniqueName, member_size.Size);
                }
            }

            // Высоты строк
            foreach (LineSizeInfo line_size in sizeInfo.ColumnsAreaSize.LinesSize)
            {
                m_Columns_RowsHeightes[line_size.LineIndex] = line_size.Size;
            }

            // Область строк
            m_MembersHeightes.Clear();
            // Высоты элементов
            foreach (MemberSizeInfo member_size in sizeInfo.RowsAreaSize.MembersSize)
            {
                if (!String.IsNullOrEmpty(member_size.MemberUniqueName) && member_size.Size >= 0)
                {
                    m_MembersHeightes.Add(member_size.MemberUniqueName, member_size.Size);
                }
            }
            // Ширины колонок
            foreach (LineSizeInfo line_size in sizeInfo.RowsAreaSize.LinesSize)
            {
                m_Rows_ColumnWidthes[line_size.LineIndex] = line_size.Size;
            }

            // Это приведет к пересчету масштаба
            double OldScale = Scale;
            m_Scale = sizeInfo.Scale;
            Scale = OldScale;
        }

        public void ClearCustomSize()
        {
            m_Columns_RowsHeightes.Clear();
            m_Rows_ColumnWidthes.Clear();
            m_MembersHeightes.Clear();
            m_MembersWidthes.Clear();

            Refresh(RefreshType.BuildEndRefresh);
        }

        public void RestoreDefaultSize()
        {
            m_Scale = 1;
            ClearCustomSize();
        }
        #endregion Экспорт-импорт размеров

        internal SortDescriptor GetAxisPropertySort(int axis, MemberInfo info)
        {
            if (info != null && m_CellSetProvider != null)
            {
                Dictionary<String, SortDescriptor> sortInfo = null;
                if (axis == 0)
                {
                    sortInfo = m_CellSetProvider.ColumnsSortInfo;
                }
                if (axis == 1)
                {
                    sortInfo = m_CellSetProvider.RowsSortInfo;
                }
                if (sortInfo != null && sortInfo.ContainsKey(info.HierarchyUniqueName))
                    return sortInfo[info.HierarchyUniqueName];
            }
            return null;
        }

        internal SortDescriptor GetAxisPropertySort(MemberControl member)
        {
            if (member != null)
            {
                int axis = -1;
                if (member is ColumnMemberControl)
                {
                    axis = 0;
                }
                if (member is RowMemberControl)
                {
                    axis = 1;
                }
                return GetAxisPropertySort(axis, member.Member);
            }
            return null;
        }

        bool m_AutoWidthColumns = false;
        public bool AutoWidthColumns
        {
            get { return m_AutoWidthColumns; }
            set {
                m_AutoWidthColumns = value;
                if (value)
                {
                    ClearCustomSize();
                }
            }
        }

        ViewModeTypes m_ColumnsViewMode = ViewModeTypes.Tree;
        public ViewModeTypes ColumnsViewMode
        {
            get { return m_ColumnsViewMode; }
            set { 
                m_ColumnsViewMode = value;
                if (m_AnalyticInfo != null)
                    m_AnalyticInfo.ClearMembersAnalytic();
            }
        }

        ViewModeTypes m_RowsViewMode = ViewModeTypes.Tree;
        public ViewModeTypes RowsViewMode
        {
            get { return m_RowsViewMode; }
            set { 
                m_RowsViewMode = value;
                if (m_AnalyticInfo != null)
                    m_AnalyticInfo.ClearMembersAnalytic();
            }
        }
    }

    public class CellCoordinate
    {
        public int Column = 0;
        public int Row = 0;

        public CellCoordinate()
        {
        }

        public CellCoordinate(int column, int row)
        {
            Column = column;
            Row = row;
        }
    }

    public class CustomContextMenuEventArgs : EventArgs
    {
        public CustomContextMenu Menu = null;
        public PivotGridItem GridItem = null;

        public CustomContextMenuEventArgs(CustomContextMenu menu, PivotGridItem item)
        {
            Menu = menu;
            GridItem = item;
        }
    }
}
