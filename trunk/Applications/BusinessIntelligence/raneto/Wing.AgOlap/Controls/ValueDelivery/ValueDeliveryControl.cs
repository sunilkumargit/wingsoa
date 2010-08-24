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
using Ranet.AgOlap.Controls.Buttons;
using Ranet.AgOlap.Controls.General;
using Ranet.Olap.Core.Data;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Ranet.AgOlap.Controls.General.ItemControls;
using Ranet.Olap.Core.Providers;
using Ranet.AgOlap.Controls.Data;
using Ranet.Olap.Core;

namespace Ranet.AgOlap.Controls.ValueDelivery
{
    public class QueryEventArgs : EventArgs
    {
        public readonly String Query = String.Empty;
        public QueryEventArgs(String query)
        {
            Query = query;
        }
    }

    public class MemberItem : INotifyPropertyChanged
    {
        public MemberItem(MemberInfo data, double val, CellInfo cell)
        {
            if (data == null)
                throw new ArgumentNullException("data");
            Cell = cell;
            Member = data;
            Caption = data.Caption;
            m_OriginalValue = val;
            m_DeliveredValue = 0;
            m_NewValue = val;
        }

        public readonly CellInfo Cell = null;
        public readonly MemberInfo Member = null;

        public String Caption { get; set; }
        double m_OriginalValue = 0;
        public double OriginalValue
        {
            get { return m_OriginalValue; }
            set
            {
                if (value != m_OriginalValue)
                {
                    m_OriginalValue = value;
                    Raise_PropertyChanged("OriginalValue");
                }
            }
        }

        public bool IsDelivered
        {
            get
            {
                return m_DeliveredValue != 0;
            }
        }

        public double m_DeliveredValue = 0;
        public double DeliveredValue
        {
            get { return m_DeliveredValue; }
            set
            {
                if (m_DeliveredValue != value)
                {
                    m_DeliveredValue = value;
                    NewValue = OriginalValue + value;
                    Raise_PropertyChanged("DeliveredValue");
                }
            }
        }

        double m_NewValue = 0;
        public double NewValue
        {
            get { return m_NewValue; }
            set
            {
                if (m_NewValue != value)
                {
                    m_NewValue = value;
                    Raise_PropertyChanged("NewValue");
                }
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;
        void Raise_PropertyChanged(String propName)
        {
            if (!String.IsNullOrEmpty(propName))
            {
                PropertyChangedEventHandler handler = PropertyChanged;
                if (handler != null)
                {
                    handler(this, new PropertyChangedEventArgs(propName));
                }
            }
        }

        #endregion
    }

    public class ValueDeliveryControl : UserControl
    {
        CellInfo m_Cell = null;
        public CellInfo Cell
        {
            get {
                return m_Cell;
            }
        }

        CellTupleControl m_Tupple = null;
        DataGrid m_MembersGrid = null;
        
        String m_CubeName = String.Empty;
        public String CubeName
        {
            get { return m_CubeName; }
            set { m_CubeName = value;
            m_MembersChoice.ACubeName = m_CubeName;
            }
        }

        ComboBoxEx m_DevileryModeCombo = null;
        TextBlock m_ValueToDelivery = null;
        TextBlock m_Delivered = null;
        TextBlock m_RestToDelivery = null;
        MemberChoicePopUp m_MembersChoice = null;

        public String ConnectionID
        {
            get { return m_MembersChoice.AConnection; }
            set { m_MembersChoice.AConnection = value; }
        }

        public ValueDeliveryControl()
        {
            Grid LayoutRoot = new Grid();
            LayoutRoot.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            LayoutRoot.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            LayoutRoot.RowDefinitions.Add(new RowDefinition());

            // Грид для свойств ячейки
            Grid Row0_LayoutRoot = new Grid();
            Row0_LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition());
            Row0_LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            Row0_LayoutRoot.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            Row0_LayoutRoot.RowDefinitions.Add(new RowDefinition());
            
            Border Row0_Border = new Border() { BorderBrush = new SolidColorBrush(Colors.DarkGray) };
            Row0_Border.BorderThickness = new Thickness(1);
            Row0_Border.Padding = new Thickness(5);
            Row0_Border.Child = Row0_LayoutRoot;

            LayoutRoot.Children.Add(Row0_Border);

            TextBlock cellSettings = new TextBlock();
            cellSettings.Margin = new Thickness(0, 0, 0, 0);
            cellSettings.Text = Localization.ValueDeliveryControl_DeliveredCellSettings;
            Row0_LayoutRoot.Children.Add(cellSettings);

            // Тапл ячейки
            m_Tupple = new CellTupleControl();
            m_Tupple.SelectedItemChanged += new EventHandler<TupleItemArgs>(m_Tupple_SelectedItemChanged);
            m_Tupple.Margin = new Thickness(0, 5, 0, 0);
            m_Tupple.Height = 170;
            Row0_LayoutRoot.Children.Add(m_Tupple);
            Grid.SetRow(m_Tupple, 1);

            Grid DeliveryInfo_LayoutRoot = new Grid();
            DeliveryInfo_LayoutRoot.Margin = new Thickness(10, 0, 20, 0);
            DeliveryInfo_LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition());
            DeliveryInfo_LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto, MinWidth = 50 });
            DeliveryInfo_LayoutRoot.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            DeliveryInfo_LayoutRoot.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            DeliveryInfo_LayoutRoot.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            DeliveryInfo_LayoutRoot.RowDefinitions.Add(new RowDefinition());

            // Начальное значение
            TextBlock Label_ValueToDelivery = new TextBlock() { Text = Localization.ValueDeliveryDialog_ValueToDelivery, Margin = new Thickness(5) };
            DeliveryInfo_LayoutRoot.Children.Add(Label_ValueToDelivery);
            m_ValueToDelivery = new TextBlock() { Text = m_OriginalValue.ToString(), Margin = new Thickness(5) };
            DeliveryInfo_LayoutRoot.Children.Add(m_ValueToDelivery);
            Grid.SetColumn(m_ValueToDelivery, 1);
            
            // Распределено
            TextBlock Label_Delivered = new TextBlock() { Text = Localization.ValueDeliveryDialog_Delivered, Margin = new Thickness(5) };
            DeliveryInfo_LayoutRoot.Children.Add(Label_Delivered);
            Grid.SetRow(Label_Delivered, 1);
            m_Delivered = new TextBlock() { Margin = new Thickness(5) };
            DeliveryInfo_LayoutRoot.Children.Add(m_Delivered);
            Grid.SetColumn(m_Delivered, 1);
            Grid.SetRow(m_Delivered, 1);
            
            // Остаток к распределению
            TextBlock Label_RestToDelivery = new TextBlock() { Text = Localization.ValueDeliveryDialog_RestToDelivery, Margin = new Thickness(5) };
            DeliveryInfo_LayoutRoot.Children.Add(Label_RestToDelivery);
            Grid.SetRow(Label_RestToDelivery, 2);
            m_RestToDelivery = new TextBlock() { Margin = new Thickness(5) };
            DeliveryInfo_LayoutRoot.Children.Add(m_RestToDelivery);
            Grid.SetColumn(m_RestToDelivery, 1);
            Grid.SetRow(m_RestToDelivery, 2);

            RefreshDeliveredInfo();

            Row0_LayoutRoot.Children.Add(DeliveryInfo_LayoutRoot);
            Grid.SetColumn(DeliveryInfo_LayoutRoot, 1);
            Grid.SetRow(DeliveryInfo_LayoutRoot, 1);

            // Выбор по каким элементам разноска
            Grid DeliveryMode_LayoutRoot = new Grid();
            DeliveryMode_LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition());
            DeliveryMode_LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto});
            DeliveryMode_LayoutRoot.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            DeliveryMode_LayoutRoot.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            DeliveryMode_LayoutRoot.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            Border Row1_Border = new Border() { BorderBrush = new SolidColorBrush(Colors.DarkGray) };
            Row1_Border.BorderThickness = new Thickness(1);
            Row1_Border.Margin = new Thickness(0, 5, 0, 5);
            Row1_Border.Padding = new Thickness(5);
            Row1_Border.Child = DeliveryMode_LayoutRoot;

            LayoutRoot.Children.Add(Row1_Border);
            Grid.SetRow(Row1_Border, 1);

            // Заголовок
            TextBlock delivTo = new TextBlock();
            delivTo.Text = Localization.ValueDeliveryControl_DeliveryTo;
            DeliveryMode_LayoutRoot.Children.Add(delivTo);

            // Выбор элементов для разноски (доступен только для режима "выбранные из списка")
            m_MembersChoice = new MemberChoicePopUp();
            m_MembersChoice.IsEnabled = false;
            DeliveryMode_LayoutRoot.Children.Add(m_MembersChoice);
            Grid.SetRow(m_MembersChoice, 2);

            // Тип разноски
            m_DevileryModeCombo = new ComboBoxEx();
            m_DevileryModeCombo.Margin = new Thickness(0, 5, 0, 5);
            ItemControlBase ctrl;
            ctrl = new ItemControlBase(false);
            ctrl.Text = Localization.ValueDeliveryControl_ChildrenMembers;
            m_DevileryModeCombo.Combo.Items.Add(ctrl);
            ctrl = new ItemControlBase(false);
            ctrl.Text = Localization.ValueDeliveryControl_SiblingsMembers;
            m_DevileryModeCombo.Combo.Items.Add(ctrl);
            ctrl = new ItemControlBase(false);
            ctrl.Text = Localization.ValueDeliveryControl_SelectedMembers;
            m_DevileryModeCombo.Combo.Items.Add(ctrl);
            m_DevileryModeCombo.SelectionChanged += new SelectionChangedEventHandler(m_DevileryModeCombo_SelectionChanged);
            m_DevileryModeCombo.Combo.SelectedIndex = 0;
            DeliveryMode_LayoutRoot.Children.Add(m_DevileryModeCombo);
            Grid.SetRow(m_DevileryModeCombo, 1);
            
            // Кнопка Применить
            RanetButton ApplyButton = new RanetButton();
            ApplyButton.Content = Localization.Apply;
            ApplyButton.Margin = new Thickness(5, 5, 0, 5);
            ApplyButton.Click += new RoutedEventHandler(ApplyButton_Click);
            DeliveryMode_LayoutRoot.Children.Add(ApplyButton);
            Grid.SetColumn(ApplyButton, 1);
            Grid.SetRow(ApplyButton, 1);

            // Элементы по которым возможна разноска
            Grid Row2_LayoutRoot = new Grid();
            Row2_LayoutRoot.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            Row2_LayoutRoot.RowDefinitions.Add(new RowDefinition());

            TextBlock membersToDelivery = new TextBlock();
            membersToDelivery.Margin = new Thickness(0, 0, 0, 0);
            membersToDelivery.Text = Localization.ValueDeliveryControl_MembersToDelivery;
            Row2_LayoutRoot.Children.Add(membersToDelivery);

            m_MembersGrid = new RanetDataGrid();
            m_MembersGrid.Margin = new Thickness(0, 5, 0, 0);
            m_MembersGrid.AlternatingRowBackground = new SolidColorBrush(Colors.White);
            m_MembersGrid.RowBackground = new SolidColorBrush(Colors.White);
            m_MembersGrid.AutoGenerateColumns = false;
            m_MembersGrid.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            m_MembersGrid.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            m_MembersGrid.RowHeight = 22;

            DataGridTextColumn memberColumn = new DataGridTextColumn();
            memberColumn.Header = Localization.ValueDeliveryControl_Member;
            memberColumn.Binding = new System.Windows.Data.Binding("Caption");
            memberColumn.IsReadOnly = true;
            m_MembersGrid.Columns.Add(memberColumn);

            DataGridTextColumn originalValueColumn = new DataGridTextColumn();
            originalValueColumn.Header = Localization.ValueDeliveryControl_OriginalValue;
            originalValueColumn.Binding = new System.Windows.Data.Binding("OriginalValue");
            originalValueColumn.IsReadOnly = true;
            m_MembersGrid.Columns.Add(originalValueColumn);

            DataGridTextColumn deliveredValueColumn = new DataGridTextColumn();
            deliveredValueColumn.Header = Localization.ValueDeliveryControl_Delivered;
            deliveredValueColumn.Binding = new System.Windows.Data.Binding("DeliveredValue");
            m_MembersGrid.Columns.Add(deliveredValueColumn);

            DataGridTextColumn newValueColumn = new DataGridTextColumn();
            newValueColumn.Header = Localization.ValueDeliveryControl_NewValue;
            newValueColumn.Binding = new System.Windows.Data.Binding("NewValue");
            newValueColumn.IsReadOnly = true;
            m_MembersGrid.Columns.Add(newValueColumn);

            Row2_LayoutRoot.Children.Add(m_MembersGrid);
            Grid.SetRow(m_MembersGrid, 1);

            Border Row2_Border = new Border() { BorderBrush = new SolidColorBrush(Colors.DarkGray) };
            Row2_Border.BorderThickness = new Thickness(1);
            Row2_Border.Padding = new Thickness(5);
            Row2_Border.Child = Row2_LayoutRoot;

            LayoutRoot.Children.Add(Row2_Border);
            Grid.SetRow(Row2_Border, 2);

            this.Content = LayoutRoot;
            m_MembersGrid.CellEditEnded += new EventHandler<DataGridCellEditEndedEventArgs>(m_MembersGrid_CellEditEnded);
        }

        void m_MembersGrid_CellEditEnded(object sender, DataGridCellEditEndedEventArgs e)
        {
            if (e != null && e.Row != null)
            {
                MemberItem item = e.Row.DataContext as MemberItem;
                if (item != null)
                {
                    m_DeliveredValue = 0;
                    IList<MemberItem> delivered = GetDeliveredMembers();
                    foreach (MemberItem dev_item in delivered)
                    {
                        m_DeliveredValue += dev_item.DeliveredValue;
                    }
                    RefreshDeliveredInfo();
                }
            }
        }

        void RefreshDeliveredInfo()
        {
            m_ValueToDelivery.Text = m_OriginalValue.ToString();
            m_Delivered.Text = m_DeliveredValue.ToString();
            m_RestToDelivery.Text = (m_OriginalValue - m_DeliveredValue).ToString();
        }

        void m_DevileryModeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            m_MembersChoice.IsEnabled = m_DevileryModeCombo.Combo.SelectedIndex == 2;
        }

        MemberInfo m_CurrentTupleItem = null;
        void m_Tupple_SelectedItemChanged(object sender, TupleItemArgs e)
        {
            if (e != null && e.Item != null)
            {
                m_CurrentTupleItem = e.Item.Info;
                m_MembersChoice.AHierarchyName = m_CurrentTupleItem.HierarchyUniqueName;
            }
            else
            {
                m_CurrentTupleItem = null;
                m_MembersChoice.AHierarchyName = String.Empty;
            }
        }

        public IDataLoader Loader;

        public event EventHandler<QueryEventArgs> LoadMembers;
        void Raise_LoadMembers(QueryEventArgs args)
        {
            EventHandler<QueryEventArgs> handler = LoadMembers;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            String query = BuildGetMembers();
            if (!String.IsNullOrEmpty(query))
            {
                Raise_LoadMembers(new QueryEventArgs(query));
            }
        }

        String BuildGetMembers()
        {
            StringBuilder builder = new StringBuilder();
            if (m_CurrentTupleItem != null && m_DevileryModeCombo.Combo.SelectedIndex >= 0)
            {
                builder.Append("SELECT TOPCOUNT({");

                if (m_DevileryModeCombo.Combo.SelectedIndex == 0)
                {
                    // Запросить дочерние
                    builder.AppendFormat("{0}.Children - {0}.DataMember", m_CurrentTupleItem.UniqueName);
                }
                if (m_DevileryModeCombo.Combo.SelectedIndex == 1)
                {
                    // Запросить соседние
                    builder.AppendFormat("{0}.Siblings - {0}", m_CurrentTupleItem.UniqueName);
                }
                if (m_DevileryModeCombo.Combo.SelectedIndex == 2)
                {
                    // Запросить выбранные
                    builder.AppendFormat("{0}", m_MembersChoice.SelectedSet);
                }

                builder.Append("}, 1000)");
                builder.AppendFormat(" on 0 FROM {0} ", OlapHelper.ConvertToQueryStyle(CubeName));

                // Условие WHERE
                String where = String.Empty;
                IDictionary<String, MemberInfo> tuple = m_Cell.GetTuple();
                foreach (MemberInfo member in tuple.Values)
                {
                    if (member.UniqueName != m_CurrentTupleItem.UniqueName)
                    {
                        if (!String.IsNullOrEmpty(where))
                        {
                            where = where + ", ";
                        }
                        where = where + member.UniqueName;
                    }
                }
                if (!String.IsNullOrEmpty(where))
                {
                    builder.AppendFormat("WHERE ({0})", where);
                }
                // дочерние select {[Персона].[Персонал].&[0x806600000000024F].Children - [Персона].[Персонал].&[0x806600000000024F].DataMember} on 0 from [Бюджет] where ([Сценарий].[Сценарии].&[План], [Период].[ГКМ].[Год].&[2008], [Measures].[Сумма], [Статья].[Статьи].&[и_Ресурсы_Загрузка], [ЦФО].[Менеджмент].&[У-5], [Контрагент].[Контрагенты].[Все контрагенты], [Проект].[Проекты].[Все проекты], [Договор].[Договоры].[Все договоры], [Подразделение].[Подразделения].[Все подразделения], [Номенклатура].[Вид-Группа-Номенклатура].[Вся номенклатура], [Бизнес-процесс].[Бизнес-процессы].[Все бизнес-процессы], [Вид Деятельности].[Вид-Группа-Деятельность].[Вид].&[Технологические работы])
                // соседние select {[Персона].[Персонал].&[0x806600000000019E].Siblings - [Персона].[Персонал].&[0x806600000000019E]} on 0  from [Бюджет] where ([Сценарий].[Сценарии].&[План], [Период].[ГКМ].[Год].&[2008], [Measures].[Сумма], [Статья].[Статьи].&[и_Ресурсы_Загрузка], [ЦФО].[Менеджмент].&[У-5], [Контрагент].[Контрагенты].[Все контрагенты], [Проект].[Проекты].[Все проекты], [Договор].[Договоры].[Все договоры], [Подразделение].[Подразделения].[Все подразделения], [Номенклатура].[Вид-Группа-Номенклатура].[Вся номенклатура], [Бизнес-процесс].[Бизнес-процессы].[Все бизнес-процессы], [Вид Деятельности].[Вид-Группа-Деятельность].[Вид].&[Технологические работы])
            }
            return builder.ToString();
        }

        double m_OriginalValue = 0;
        public double OriginalValue
        {
            get{
                return m_OriginalValue;
            }
        }
        double m_DeliveredValue = 0;
        public double DeliveredValue
        {
            get { return m_DeliveredValue; }
        }

        public bool IsDelivered
        {
            get {
                return m_DeliveredValue != null;
            }
        }

        void InitDeliveredValues()
        {
            m_OriginalValue = 0;
            m_DeliveredValue = 0;

            if (m_Cell != null &&
                    m_Cell.CellDescr != null &&
                    m_Cell.CellDescr.Value != null &&
                    m_Cell.CellDescr.Value.Value != null)
            {
                try
                {
                    m_OriginalValue = Convert.ToDouble(m_Cell.CellDescr.Value.Value);
                }
                catch (System.InvalidCastException)
                {
                }
            }
            RefreshDeliveredInfo();
        }

        public void Initialize(CellInfo cell)
        {
            m_Cell = cell;

            InitDeliveredValues();
            m_Tupple.Initialize(m_Cell);
        }

        public IList<MemberItem> GetDeliveredMembers()
        {
            IList<MemberItem> list = new List<MemberItem>();
            if (m_MembersGrid.ItemsSource != null)
            {
                foreach (object obj in m_MembersGrid.ItemsSource)
                {
                    MemberItem item = obj as MemberItem;
                    if (item != null)
                    {
                        if (item.IsDelivered)
                            list.Add(item);
                    }
                }
            }
            
            return list;
        }

        public void InitializeMembersList(CellSetDataProvider provider)
        {
            IList<MemberItem> list = new List<MemberItem>();
            if (provider != null && provider.CellSet_Description.Axes.Count > 0)
            {
                int col = 0;
                foreach (MemberInfo info in provider.Columns)
                {
                    //CellData cell = provider.CellSet_Description.GetCellDescription(col);
                    //CellView view = new CellView(cell, info, MemberInfo.Empty, provider.GetInvisibleCoords(col));
                    CellInfo view = provider.GetCellInfo(col, -1);
                    double value = 0;
                    if (view != null && 
                        view.Value != null)
                    {
                        try
                        {
                            value = Convert.ToDouble(view.Value);
                        }
                        catch (System.InvalidCastException)
                        {
                        }
                    }

                    list.Add(new MemberItem(info, value, view));
                    col++;
                }
            }

            m_MembersGrid.ItemsSource = list;
        }  

        //public void InitializeMembersList(CellSetData cs_descr)
        //{
        //    IList<MemberItem> list = new List<MemberItem>();
        //    if (cs_descr != null && cs_descr.Axes.Count > 0)
        //    {
        //        int col = 0;
        //        foreach (PositionData pos in cs_descr.Axes[0].Positions)
        //        {
        //            if (pos.Members.Count > 0)
        //            {
        //                CellData cell = cs_descr.GetCellDescription(col);
        //                double value = 0;
        //                if (cell != null && cell.Value != null &&
        //                    cell.Value.Value != null)
        //                {
        //                    try
        //                    {
        //                        value = Convert.ToDouble(cell.Value.Value);
        //                    }
        //                    catch (System.InvalidCastException)
        //                    {
        //                    }
        //                }

        //                list.Add(new MemberItem(pos.Members[0], value));
        //            }
        //            col++;
        //        }
        //    }

        //    m_MembersGrid.ItemsSource = list;
        //}
    }
}
