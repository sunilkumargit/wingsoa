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
using Ranet.AgOlap.Controls.Forms;
using Ranet.AgOlap.Controls.MemberChoice.Info;
using Ranet.AgOlap.Controls.General;

namespace Ranet.AgOlap.Controls.ValueCopy
{
    public class GetIDataLoaderArgs : EventArgs
    {
        public IDataLoader Loader = null;
        public bool Handled = false;
    }

    public partial class CoordinatesControl : UserControl
    {
        public String CubeName = String.Empty;
        public String ConnectionID = String.Empty;

        public event EventHandler<GetIDataLoaderArgs> GetOlapDataLoader;
        public ILogService LogManager = null;
        
        void Raise_GetOlapDataLoader(GetIDataLoaderArgs args)
        {
            EventHandler<GetIDataLoaderArgs> handler = this.GetOlapDataLoader;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        public CoordinatesControl()
        {
            InitializeComponent();

            coordinatesGrid.AlternatingRowBackground = new SolidColorBrush(Colors.White);
            coordinatesGrid.RowBackground = new SolidColorBrush(Colors.White);
            coordinatesGrid.AutoGenerateColumns = false;
            coordinatesGrid.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            coordinatesGrid.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            coordinatesGrid.RowHeight = 22;

            coordinatesGrid.Loaded += new RoutedEventHandler(coordinatesGrid_Loaded);
            coordinatesGrid.KeyDown += new KeyEventHandler(coordinatesGrid_KeyDown);

            coordinatesGrid.BeginningEdit += new EventHandler<DataGridBeginningEditEventArgs>(coordinatesGrid_BeginningEdit);

            ShowNotEmptyCheckBox.Content = Localization.ValueCopyControl_ShowNotEmpty_Caption;
            ShowNotEmptyCheckBox.Checked += new RoutedEventHandler(ShowNotEmptyCheckBox_Checked);
            ShowNotEmptyCheckBox.Unchecked += new RoutedEventHandler(ShowNotEmptyCheckBox_Checked);
        }

        private void ChangeSourceButton_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void ChangeDestButton_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void DeleteSourceButton_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void DeleteDestButton_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void myComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            //ComboBox curComboBox = sender as ComboBox;
            //curComboBox.Items.Add("Default");
            //curComboBox.Items.Add("Readonly");
            //curComboBox.Items.Add("Disabled");
            //curComboBox.SelectedIndex = 0;
        }

        public bool ShowNotEmpty
        {
            get {
                if (ShowNotEmptyCheckBox.IsChecked.HasValue)
                    return ShowNotEmptyCheckBox.IsChecked.Value;
                return false;
            }
            set {
                ShowNotEmptyCheckBox.IsChecked = new bool?(value);
            }
        }

        void ShowNotEmptyCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            UpdateItemsSource();
        }

        void coordinatesGrid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                OnDelete();
            }
        }

        bool m_IsAdminMode = false;
        public bool IsAdminMode
        {
            get { return m_IsAdminMode; }
            set {
                if (value)
                {
                    coordinatesGrid.Columns[0].Visibility = Visibility.Visible;
                }
                else
                {
                    coordinatesGrid.Columns[0].Visibility = Visibility.Collapsed;
                }
                m_IsAdminMode = value; 
            }
        }

        void coordinatesGrid_Loaded(object sender, RoutedEventArgs e)
        {
            if (!IsAdminMode)
            {
                coordinatesGrid.Columns[0].Visibility = Visibility.Collapsed;
            }
            coordinatesGrid.Columns[0].Header = Localization.ValueCopyControl_Accessibility;
            coordinatesGrid.Columns[1].Header = Localization.ValueCopyControl_Dimension;
            coordinatesGrid.Columns[2].Header = Localization.ValueCopyControl_Hierarchy;
            coordinatesGrid.Columns[3].Header = Localization.ValueCopyControl_Source;
            coordinatesGrid.Columns[4].Header = Localization.ValueCopyControl_Destination;
        }

        /// <summary>
        /// Кэш диалогов для источников
        /// </summary>
        Dictionary<CoordinateItem, IChoiceDialog> m_SourceDialogs = new Dictionary<CoordinateItem, IChoiceDialog>();

        /// <summary>
        /// Кэш диалогов для приемников
        /// </summary>
        Dictionary<CoordinateItem, IChoiceDialog> m_DestDialogs = new Dictionary<CoordinateItem, IChoiceDialog>();

        private enum MemberChoiceType
        { 
            Source,
            Destination
        }

        void coordinatesGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            if (e != null && e.Row != null)
            {
                e.Cancel = !BeginEdit(e.Row.DataContext as CoordinateItem);
            }
        }

        bool BeginEdit(CoordinateItem item)
        {
            if (item != null)
            {
                // Если элемент только для чтения, то при отключенном режиме администрирования редактировать его не даем
                if (!IsAdminMode && item.CoordinateState == CoordinateStateTypes.Readonly)
                    return false;

                if (coordinatesGrid.CurrentColumn == coordinatesGrid.Columns[0])
                {
                    return true;
                }
                if (coordinatesGrid.CurrentColumn == coordinatesGrid.Columns[3] ||
                    (coordinatesGrid.CurrentColumn == coordinatesGrid.Columns[4]))
                {
                    IChoiceDialog dlg = null;

                    if (coordinatesGrid.CurrentColumn == coordinatesGrid.Columns[3])
                    {
                        if (m_SourceDialogs.ContainsKey(item))
                        {
                            dlg = m_SourceDialogs[item];
                        }
                    }

                    if (coordinatesGrid.CurrentColumn == coordinatesGrid.Columns[4])
                    {
                        if (m_DestDialogs.ContainsKey(item))
                        {
                            dlg = m_DestDialogs[item];
                        }
                    }

                    if (dlg == null)
                    {
                        if (item.DimensionUniqueName == "[Measures]")
                        {
                            // Выбор меры
                            MeasureChoiceDialog dialog = new MeasureChoiceDialog();
                            dialog.ChoiceControl.CubeName = CubeName;
                            dialog.ChoiceControl.Connection = ConnectionID;
                            dialog.ChoiceControl.LogManager = LogManager;
                            GetIDataLoaderArgs args = new GetIDataLoaderArgs();
                            Raise_GetOlapDataLoader(args);
                            if (args.Handled)
                                dialog.ChoiceControl.OlapDataLoader = args.Loader;
                            dialog.Tag = item;
                            dialog.DialogOk += new EventHandler(Dialog_DialogOk);
                            dialog.DialogCancel += new EventHandler(dlg_DialogCancel);
                            dialog.Show();
                            if (coordinatesGrid.CurrentColumn == coordinatesGrid.Columns[3])
                            {
                                dialog.UserData = MemberChoiceType.Source;
                            }
                            if (coordinatesGrid.CurrentColumn == coordinatesGrid.Columns[4])
                            {
                                dialog.UserData = MemberChoiceType.Destination;
                            }
                            dlg = dialog;
                        }
                        else
                        {
                            // Выбор элемента измерения
                            MemberChoiceDialog dialog = new MemberChoiceDialog();
                            dialog.ChoiceControl.CubeName = CubeName;
                            dialog.ChoiceControl.Connection = ConnectionID;
                            dialog.ChoiceControl.HierarchyUniqueName = item.HierarchyUniqueName;
                            dialog.ChoiceControl.LogManager = LogManager;

                            GetIDataLoaderArgs args = new GetIDataLoaderArgs();
                            Raise_GetOlapDataLoader(args);
                            if (args.Handled)
                                dialog.ChoiceControl.OlapDataLoader = args.Loader;

                            dialog.Tag = item;
                            dialog.DialogOk += new EventHandler(Dialog_DialogOk);
                            dialog.DialogCancel += new EventHandler(dlg_DialogCancel);
                            dialog.Show();
                            if (coordinatesGrid.CurrentColumn == coordinatesGrid.Columns[3])
                            {
                                dialog.UserData = MemberChoiceType.Source;
                            }
                            if (coordinatesGrid.CurrentColumn == coordinatesGrid.Columns[4])
                            {
                                dialog.UserData = MemberChoiceType.Destination;
                            }
                            dlg = dialog;
                        }
                        if (coordinatesGrid.CurrentColumn == coordinatesGrid.Columns[3])
                        {
                            m_SourceDialogs[item] = dlg;
                        }
                        if (coordinatesGrid.CurrentColumn == coordinatesGrid.Columns[4])
                        {
                            m_DestDialogs[item] = dlg;
                        }

                    }

                    if (dlg != null)
                    {
                        dlg.Show();
                        return true;
                    }
                }
            }
            return false;
        }

        void dlg_DialogCancel(object sender, EventArgs e)
        {
            coordinatesGrid.CancelEdit();
        }

        void Dialog_DialogOk(object sender, EventArgs e)
        {
            MemberChoiceDialog dlg = sender as MemberChoiceDialog;
            if (dlg != null && dlg.UserData != null && dlg.UserData is MemberChoiceType)
            {
                CoordinateItem item = dlg.Tag as CoordinateItem;
                if (item != null)
                {
                    List<MemberChoiceSettings> selectedInfo = dlg.ChoiceControl.SelectedInfo;
                    if (selectedInfo != null && selectedInfo.Count == 1)
                    {
                        MemberWrap member = new MemberWrap();
                        member.Caption = dlg.ChoiceControl.SelectedInfo[0].Caption;
                        member.UniqueName = dlg.ChoiceControl.SelectedInfo[0].UniqueName;
                        member.HierarchyUniqueName = dlg.ChoiceControl.HierarchyUniqueName;
                        if(((MemberChoiceType)dlg.UserData) == MemberChoiceType.Source)
                            item.SourceMember = member;                    
                        if(((MemberChoiceType)dlg.UserData) == MemberChoiceType.Destination)
                            item.DestMember = member;                    

                    }
                }
                return;
            }

            MeasureChoiceDialog measure_dlg = sender as MeasureChoiceDialog;
            if (measure_dlg != null && measure_dlg.UserData != null && measure_dlg.UserData is MemberChoiceType)
            {
                CoordinateItem item = measure_dlg.Tag as CoordinateItem;
                if (item != null)
                {
                    if (measure_dlg.ChoiceControl.SelectedInfo != null)
                    {
                        MemberWrap member = new MemberWrap();
                        member.Caption = measure_dlg.ChoiceControl.SelectedInfo.Caption;
                        member.UniqueName = measure_dlg.ChoiceControl.SelectedInfo.UniqueName;
                        member.HierarchyUniqueName = "[Measures]";
                        if (((MemberChoiceType)measure_dlg.UserData) == MemberChoiceType.Source)
                            item.SourceMember = member;
                        if (((MemberChoiceType)measure_dlg.UserData) == MemberChoiceType.Destination)
                            item.DestMember = member;

                    }
                }
            }
        }

        IList<CoordinateItem> m_CoordinatesList = null;
        public IList<CoordinateItem> CoordinatesList
        {
            get {
                if (m_CoordinatesList == null)
                {
                    m_CoordinatesList = new List<CoordinateItem>();
                }
                return m_CoordinatesList;
            }
        }


        public void Initialize(List<CoordinateItem> list)
        {
            m_CoordinatesList = list;

            m_SourceDialogs.Clear();
            m_DestDialogs.Clear();

            UpdateItemsSource();
        }

        void UpdateItemsSource()
        {
            IList<CoordinateItem> notEmptyCoordinatesList = new List<CoordinateItem>();
            IList<CoordinateItem> coordinatesList = new List<CoordinateItem>();
            if (CoordinatesList != null)
            {
                foreach (CoordinateItem item in CoordinatesList)
                {
                    // Проверяем доступен ли элемент для пользователя
                    if (item.CoordinateState != CoordinateStateTypes.Disabled)
                    {
                        // Отбираем координаты, для которых задан приемник либо источник
                        if (ShowNotEmptyCheckBox.IsChecked.Value)
                        {
                            if (!String.IsNullOrEmpty(item.SourceMemberUniqueName) ||
                                !String.IsNullOrEmpty(item.DestMemberUniqueName))
                            {
                                notEmptyCoordinatesList.Add(item);
                            }
                        }
                        coordinatesList.Add(item);
                    }
                }
            }

            if (!ShowNotEmptyCheckBox.IsChecked.Value)
                coordinatesGrid.ItemsSource = coordinatesList;
            else
                coordinatesGrid.ItemsSource = notEmptyCoordinatesList;
        }

        void OnDelete()
        {
            CoordinateItem item = coordinatesGrid.SelectedItem as CoordinateItem;
            if (item != null)
            {
                // Если элемент только для чтения, то при отключенном режиме администрирования редактировать его не даем
                if (!IsAdminMode && item.CoordinateState == CoordinateStateTypes.Readonly)
                    return;

                if (coordinatesGrid.CurrentColumn == coordinatesGrid.Columns[3])
                {
                    item.SourceMember = null;
                }
                if (coordinatesGrid.CurrentColumn == coordinatesGrid.Columns[4])
                {
                    item.DestMember = null;
                }
            }
        }

        //private void Delete_Button_Click(object sender, RoutedEventArgs e)
        //{
        //    OnDelete();
        //}

        private void DeleteSource_Button_Click(object sender, RoutedEventArgs e)
        {
            CoordinateItem item = coordinatesGrid.SelectedItem as CoordinateItem;
            if (item != null)
            {
                // Если элемент только для чтения, то при отключенном режиме администрирования редактировать его не даем
                if (!IsAdminMode && item.CoordinateState == CoordinateStateTypes.Readonly)
                    return;

                if (coordinatesGrid.CurrentColumn == coordinatesGrid.Columns[3])
                {
                    item.SourceMember = null;
                }
            }
        }

        private void DeleteDestination_Button_Click(object sender, RoutedEventArgs e)
        {
            CoordinateItem item = coordinatesGrid.SelectedItem as CoordinateItem;
            if (item != null)
            {
                // Если элемент только для чтения, то при отключенном режиме администрирования редактировать его не даем
                if (!IsAdminMode && item.CoordinateState == CoordinateStateTypes.Readonly)
                    return;

                if (coordinatesGrid.CurrentColumn == coordinatesGrid.Columns[4])
                {
                    item.DestMember = null;
                }
            }
        }

        private void ChangeDestination_Button_Click(object sender, RoutedEventArgs e)
        {
            BeginEdit(coordinatesGrid.SelectedItem as CoordinateItem);
        }

        private void ChangeSource_Button_Click(object sender, RoutedEventArgs e)
        {
            BeginEdit(coordinatesGrid.SelectedItem as CoordinateItem);
        }
    }
}
