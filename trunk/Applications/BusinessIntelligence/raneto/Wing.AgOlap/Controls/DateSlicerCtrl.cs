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
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Ranet.AgOlap.Commands;
using Ranet.AgOlap.Controls.General;
using Ranet.Olap.Core;
using Ranet.Olap.Core.Data;
using Ranet.Olap.Core.Providers.ClientServer;
using Ranet.AgOlap.Controls.Buttons;

namespace Ranet.AgOlap.Controls
{    

    public class SlicerCtrl : AgControlBase
    {
        private Grid m_DataGrid;
        private BusyControl m_Waiting;
        private RanetHotButton m_Clear;
        private ScrollViewer viewer;
        private Grid grdIsWaiting;
        private StackPanel m_Panel;
        private Dictionary<String, MemberData> m_LoadedMembers = new Dictionary<String, MemberData>();
        public const double CellWidth = 50;
        public const double CellHeight = 20;
        private double CellsCount = 25;
        private List<int> m_slicedButtons = new List<int>();
        
        public SlicerCtrl()
        {
            viewer = new ScrollViewer();
            viewer.BorderThickness = new Thickness(0);
            viewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            viewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;                 
            Grid LayoutRoot = new Grid();
            m_DataGrid = new Grid();
            LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition() {Width = GridLength.Auto});
            LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition() {MaxWidth = 20});
            LayoutRoot.RowDefinitions.Add(new RowDefinition() {MaxHeight = 20});
            LayoutRoot.RowDefinitions.Add(new RowDefinition(){ Height = GridLength.Auto});
            LayoutRoot.RowDefinitions.Add(new RowDefinition() {Height = GridLength.Auto});
            m_Panel = new StackPanel();
            m_Panel.Orientation = Orientation.Horizontal;
            m_Clear = new RanetHotButton();
            m_Clear.Width = 20;
            m_Clear.Height = 20;
            m_Clear.Content = "C";
            m_Clear.Click +=new RoutedEventHandler(m_Clear_Click);
            m_Clear.Visibility = System.Windows.Visibility.Collapsed;
            LayoutRoot.Children.Add(m_Clear);
            Grid.SetRow(m_Clear,0);
            Grid.SetColumn(m_Clear,1);
            LayoutRoot.Children.Add(m_Panel);
            Grid.SetRow(m_Panel, 1);
            Grid.SetColumn(m_Panel, 0);
            LayoutRoot.Children.Add(m_DataGrid);
            Grid.SetColumn(m_DataGrid, 0);
            Grid.SetRow(m_DataGrid, 2);

            grdIsWaiting = new Grid() { Background = new SolidColorBrush(Color.FromArgb(125, 0xFF, 0xFF, 0xFF)) };
            grdIsWaiting.Visibility = Visibility.Collapsed;
            m_Waiting = new BusyControl();
            m_Waiting.Text = Localization.Loading;
            grdIsWaiting.Children.Add(m_Waiting);
            LayoutRoot.Children.Add(grdIsWaiting);
            Grid.SetColumnSpan(grdIsWaiting, LayoutRoot.ColumnDefinitions.Count > 0 ? LayoutRoot.ColumnDefinitions.Count : 1);
            Grid.SetRowSpan(grdIsWaiting, LayoutRoot.RowDefinitions.Count > 0 ? LayoutRoot.RowDefinitions.Count : 1);

            viewer.Content = LayoutRoot;
            this.Content = viewer;
            this.SlicerHeight = 10;
            this.SlicerWidth = 10;           
            //this.m_DataGrid.MouseLeftButtonDown += new MouseButtonEventHandler(m_DataGrid_MouseLeftButtonDown);
            //this.m_DataGrid.MouseLeftButtonUp += new MouseButtonEventHandler(m_DataGrid_MouseLeftButtonUp);
            this.DirectionChanged += new EventHandler<ChangedDirectionEventArgs>(SlicerCtrl_DirectionChanged);
            
            //this.m_Panel.MouseMove += new MouseEventHandler(m_Panel_MouseMove);
            //this.Content = m_DataGrid;
        }
        

        void SlicerCtrl_DirectionChanged(object sender, ChangedDirectionEventArgs e)
        {
            if (e.Direction == Controls.Direction.Vertical)
            {
                this.SlicerWidth = 1;
                this.SlicerHeight = CellsCount;
            }
            if (e.Direction ==Controls.Direction.Horizontal)
            {
                this.SlicerHeight = 1;
                this.SlicerWidth = CellsCount;
            }
            if (e.Direction ==Controls.Direction.Both)
            {
                this.SlicerWidth = this.SlicerHeight = Math.Floor(Math.Sqrt(CellsCount));                 
            }
        }

        bool m_IsWaiting = false;
        public bool IsWaiting
        {
            get { return m_IsWaiting; }
            set
            {
                if (m_IsWaiting != value)
                {
                    if (value)
                    {
                        this.Cursor = Cursors.Wait;
                        grdIsWaiting.Visibility = Visibility.Visible;
                        this.m_Clear.Visibility = System.Windows.Visibility.Collapsed;
                    }
                    else
                    {
                        this.Cursor = Cursors.Arrow;
                        grdIsWaiting.Visibility = Visibility.Collapsed;
                        this.m_Clear.Visibility = System.Windows.Visibility.Visible;
                    }                    
                    m_IsWaiting = value;
                }
            }
        }

        protected bool NeedReload = true;        

        public double SlicerHeight
        {
            get; set;
        }

        public double SlicerWidth
        {
            get; set;
        }

        private Direction m_Direction = Controls.Direction.Horizontal; 
        public Direction Direction
        {
            get { return m_Direction; }
            set
            {
                if (m_Direction != value)
                {
                    this.CellsCount = SlicerWidth*SlicerHeight;
                    Raise_DirectionChanged(value);
                }
                m_Direction = value;
            }
        }

        #region Свойства для настройки на OLAP

        public String Query { get; set; }        
        
        private String m_Connection = String.Empty;
        /// <summary>
        /// Описание соединения с БД
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
                NeedReload = true;
            }
        }

        private string m_CubeName;
        /// <summary>
        /// Имя OLAP куба
        /// </summary>
        public String CubeName
        {
            get
            {
                return m_CubeName;
            }
            set
            {
                m_CubeName = value;
                NeedReload = true;
            }
        }

        private string m_LevelUniqueName = String.Empty;
        /// <summary>
        /// Уникальное имя уровня иерархии
        /// </summary>
        public String LevelUniqueName
        {
            get
            {
                return m_LevelUniqueName;
            }
            set
            {
                m_LevelUniqueName = value;
                NeedReload = true;
            }
        }        

        private string m_FormatValue = String.Empty;
        /// <summary>
        /// Строка форматирования для значения
        /// </summary>
        public String FormatValue
        {
            get
            {
                return m_FormatValue;
            }
            set
            {
                m_FormatValue = value;
                NeedReload = true;
            }
        }
        #endregion Свойства для настройки на OLAP

        bool m_IsBusy = false;
        public bool IsBusy
        {
            get { return m_IsBusy; }
            set
            {
                m_IsBusy = value;
                if (value)
                        this.IsEnabled = false;
                    else
                        this.IsEnabled = true;                
            }
        }


        public event EventHandler<ChangedDirectionEventArgs> DirectionChanged;
        void Raise_DirectionChanged(Direction direction)
        {
            EventHandler<ChangedDirectionEventArgs> handler = DirectionChanged;
            if (handler != null)
            {
                handler(this, new ChangedDirectionEventArgs(direction));
            }
        }

        public void Initialize()
        {
            m_LoadedMembers.Clear();
            m_slicedButtons.Clear();
            LoadMembers();
        }

        void LoadMembers()
        {
            this.IsWaiting = true;
            if (String.IsNullOrEmpty(Connection))
            {
                // Сообщение в лог
                StringBuilder builder = new StringBuilder();
                if (String.IsNullOrEmpty(Connection))
                    builder.Append(Localization.Connection_PropertyDesc);
                LogManager.LogError(this, String.Format(Localization.ControlSettingsNotInitialized_Message, builder.ToString()));
                this.m_Clear.Visibility = System.Windows.Visibility.Collapsed;
                this.viewer.Content = String.Format(Localization.ControlSettingsNotInitialized_Message,
                                                    builder.ToString());
                this.IsWaiting = false;
                return;
            }

            if (String.IsNullOrEmpty(this.Query))
            {
                if (!String.IsNullOrEmpty(LevelUniqueName))
                {
                    String query = "Select {" + LevelUniqueName +
                                   ".Members} on 0, {} on 1 from " + OlapHelper.ConvertToQueryStyle(CubeName);                   
                    MdxQueryArgs args = CommandHelper.CreateMdxQueryArgs(Connection, query);
                    OlapDataLoader.LoadData(args, null);
                }
            }
            else
            {                     
                MdxQueryArgs args = CommandHelper.CreateMdxQueryArgs(Connection, Query);
                OlapDataLoader.LoadData(args, null);
            }
        }

        #region Загрузчики
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
                if (m_OlapDataLoader == null)
                {
                    m_OlapDataLoader = GetDataLoader();
                    m_OlapDataLoader.DataLoaded += new EventHandler<DataLoaderEventArgs>(Loader_DataLoaded);
                }
                return m_OlapDataLoader;
            }
        }

        protected virtual IDataLoader GetDataLoader()
        {
            return new OlapDataLoader(URL);
        }
        #endregion Загрузчики

        void Loader_DataLoaded(object sender, DataLoaderEventArgs e)
        {            

            if (e.Error != null)
            {
                LogManager.LogError(this, e.Error.ToString());
                this.m_Clear.Visibility = System.Windows.Visibility.Collapsed;
                this.viewer.Content = e.Error.ToString();
                this.IsWaiting = false;
                return;
            }

            if (e.Result.ContentType == InvokeContentType.Error)
            {
                LogManager.LogError(this, e.Result.Content);
                this.m_Clear.Visibility = System.Windows.Visibility.Collapsed;
                this.viewer.Content = e.Result.Content;
                this.IsWaiting = false;
                return;
            }

            try
            {
                CellSetData cs_descr = CellSetData.Deserialize(e.Result.Content);
                OnDatesLoaded(cs_descr);
                UpdateGridCells();
                this.IsWaiting = false;
            }
            catch (Exception exc)
            {
                this.IsWaiting = false;
                this.m_Clear.Visibility = System.Windows.Visibility.Collapsed;
                LogManager.LogError(this, exc.Message);
                throw new Exception(exc.Message);
                //this.viewer.Content = exc.Message;
                //throw exc;
            }
            //CellSetData cs_descr = XmlSerializationUtility.XmlStr2Obj<CellSetData>(e.Result);
                        
        }

        
        void OnDatesLoaded(CellSetData cs_descr)
        {
            m_LoadedMembers.Clear();
            if (cs_descr.Axes.Count > 0)
            {
                foreach (PositionData position in cs_descr.Axes[0].Positions)
                {
                    if (position.Members.Count > 0)
                    {
                        if (!m_LoadedMembers.ContainsKey(cs_descr.Axes[0].Members[position.Members[0].Id].UniqueName))
                            m_LoadedMembers.Add(cs_descr.Axes[0].Members[position.Members[0].Id].UniqueName, cs_descr.Axes[0].Members[position.Members[0].Id]);
                    }
                }
            }
        }

        /// <summary>
        /// Кэш сгенеренных кнопок для slicer
        /// </summary>       
        Dictionary<int, RanetToggleButton> slicerChildren = new Dictionary<int, RanetToggleButton>();

        void UpdateGridCells()
        {
            slicerChildren.Clear();
            this.m_Panel.Children.Clear();            
            this.m_DataGrid.Children.Clear();
            
            if (this.Direction == Controls.Direction.Vertical)
            {
                m_Panel.Orientation = Orientation.Vertical;
                List<string> values = new List<string>();
                var enumerator = m_LoadedMembers.GetEnumerator();
                enumerator.MoveNext();
                for (int i = 0; i < this.SlicerHeight; i++)
                {
                    if (enumerator.Current.Value != null && !String.IsNullOrEmpty(enumerator.Current.Value.Caption))
                    {
                        RanetToggleButton button = new RanetToggleButton();
                        button.ButtonId = i;
                        button.Height = double.NaN;
                        button.Width = double.NaN;
                        button.Checked += new RoutedEventHandler(button_Checked);
                        button.Unchecked += new RoutedEventHandler(button_Unchecked);
                        ToolTipService.SetToolTip(button, enumerator.Current.Value.Caption);
                        button.Content = enumerator.Current.Value.Caption;
                        slicerChildren.Add(i, button);
                    }
                    enumerator.MoveNext();                    
                }            
                foreach (var child in slicerChildren)
                {
                    this.m_Panel.Children.Add(child.Value);
                }
            }
            if (this.Direction == Controls.Direction.Horizontal)
            {
                m_Panel.Orientation = Orientation.Horizontal;   
                List<string> values = new List<string>();
                var enumerator = m_LoadedMembers.GetEnumerator();
                enumerator.MoveNext();
                for (int i = 0; i < this.SlicerWidth; i++)
                {                  
                    if (enumerator.Current.Value != null && !String.IsNullOrEmpty(enumerator.Current.Value.Caption))
                    {
                        RanetToggleButton button = new RanetToggleButton();
                        button.ButtonId = i;
                        button.Height = double.NaN;
                        button.Width = double.NaN;                        
                        button.Checked += new RoutedEventHandler(button_Checked);
                        button.Unchecked += new RoutedEventHandler(button_Unchecked);
                        ToolTipService.SetToolTip(button, enumerator.Current.Value.Caption);
                        button.Content = enumerator.Current.Value.Caption;
                        slicerChildren.Add(i, button);
                    }                                            
                    enumerator.MoveNext();
                }
                foreach (var child in slicerChildren)
                {
                    this.m_Panel.Children.Add(child.Value);
                }
            }

            if (this.Direction == Controls.Direction.Both)
            {
                //m_DataGrid = new Grid();
                var enumerator = m_LoadedMembers.GetEnumerator();
                enumerator.MoveNext();
                for (int i = 0; i < this.SlicerHeight; i++)
                {
                    m_DataGrid.RowDefinitions.Add(new RowDefinition() {Height = GridLength.Auto } );                       
                }
                for (int j = 0; j < this.SlicerWidth; j++)
                {
                    m_DataGrid.ColumnDefinitions.Add(new ColumnDefinition() {Width = GridLength.Auto});                 
                }
                int count = 0;
                for (int i = 0; i < this.SlicerHeight; i++)
                {
                    for (int j = 0; j < this.SlicerWidth; j++)
                    {
                        if (enumerator.Current.Value != null && !String.IsNullOrEmpty(enumerator.Current.Value.Caption))
                        {
                            RanetToggleButton button = new RanetToggleButton();
                            button.ButtonId = count;
                            button.Height = double.NaN;
                            button.Width = double.NaN;                        
                            button.Checked += new RoutedEventHandler(button_Checked);
                            button.Unchecked += new RoutedEventHandler(button_Unchecked);
                            ToolTipService.SetToolTip(button, enumerator.Current.Value.Caption);
                            button.Content = enumerator.Current.Value.Caption;
                            slicerChildren.Add(count,button);
                            count++;
                            m_DataGrid.Children.Add(button);
                            Grid.SetRow(button, i);
                            Grid.SetColumn(button, j);
                        }
                        enumerator.MoveNext();
                    }
                }                                
            }           
        }

        void button_Unchecked(object sender, RoutedEventArgs e)
        {
            if (m_slicedButtons.Contains((sender as RanetToggleButton).ButtonId))
            {
                this.m_slicedButtons.Remove((sender as RanetToggleButton).ButtonId);
                this.ApplySelection();
            }
        }

        void button_Checked(object sender, RoutedEventArgs e)
        {
            if (!m_slicedButtons.Contains((sender as RanetToggleButton).ButtonId))
            {
                this.m_slicedButtons.Add((sender as RanetToggleButton).ButtonId);
                this.ApplySelection();
            }
        }

        
        void m_Clear_Click(object sender, RoutedEventArgs e)
        {
            ClearSelection();
        }
        
        public void ClearSelection()
        {
            foreach (var child in slicerChildren)
            {
                child.Value.IsChecked = false;
            }
        }


        public Dictionary<string,MemberData> GetSelectedElements()
        {
            Dictionary<string, MemberData> result = null;
            if (m_LoadedMembers!= null)
            {
                var enumerator = m_LoadedMembers.GetEnumerator();
                result = new Dictionary<string, MemberData>();              
                int i = 0;
                enumerator.MoveNext();
                do
                {
                    if (m_slicedButtons.Contains(i))
                    {
                        result.Add(enumerator.Current.Key, enumerator.Current.Value);                        
                    }
                    i++;
                    enumerator.MoveNext();
                } while (enumerator.Current.Value != null);
            }
            return result;
        }

        protected virtual void ApplySelection()
        {
            var variable = this.GetSelectedElements();
        } 
      
        protected virtual string GetFormattedValue()
        {
            return string.Empty;
        }

        bool m_IsReadyToSelection = false;
        public bool IsReadyToSelection
        {
            get
            {
                return m_IsReadyToSelection;
            }
        }       
    }

    public enum Direction
    {
        Horizontal,
        Vertical,
        Both
    }

    public class ChangedDirectionEventArgs : EventArgs
    {
        public readonly Direction Direction = Direction.Horizontal;

        public ChangedDirectionEventArgs(Direction direction)
        {
            Direction = direction;
        }
    }
}
