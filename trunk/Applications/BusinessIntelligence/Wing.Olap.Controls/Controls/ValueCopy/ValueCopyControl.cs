/*   
    Copyright (C) 2009 Galaktika Corporation ZAO

    This file is a part of Wing.UILibrary.Olap
 
    Wing.UILibrary.Olap is a free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
      
    You should have received a copy of the GNU General Public License
    along with Wing.UILibrary.Olap.  If not, see
  	<http://www.gnu.org/licenses/> 
  
    If GPL v.3 is not suitable for your products or company,
    Galaktika Corp provides Wing.UILibrary.Olap under a flexible commercial license
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
using Wing.AgOlap.Controls.General;
using Wing.AgOlap.Controls.General.ClientServer;
using Wing.Olap.Core.Metadata;
using System.Collections.Generic;
using System.ComponentModel;
using Wing.AgOlap.Controls.Tab;
using Wing.AgOlap.Controls.General.ItemControls;
using Wing.AgOlap.Controls.Buttons;
using Wing.AgOlap.Controls.ValueDelivery;
using Wing.AgOlap.Controls.General.Tree;
using Wing.Olap.Core.Providers;
using Wing.AgOlap.Controls.ValueCopy.Wrappers;
using Wing.AgOlap.Controls.ToolBar;
using System.IO.IsolatedStorage;
using System.IO;
using Wing.Olap.Core;

namespace Wing.AgOlap.Controls.ValueCopy
{
    public class ValueCopyControl : UserControl
    {
        CellInfo m_Cell = null;
        public CellInfo Cell
        {
            get
            {
                return m_Cell;
            }
        }

        String m_CubeName = String.Empty;
        public String CubeName
        {
            get { return m_CubeName; }
            set
            {
                m_CubeName = value;
                m_Coordinates.CubeName = m_CubeName;
            }
        }

        String m_ConnectionID = String.Empty;
        public String ConnectionID
        {
            get { return m_ConnectionID; }
            set
            {
                m_ConnectionID = value;
                m_Coordinates.ConnectionID = m_ConnectionID;
            }
        }

        public event EventHandler<GetIDataLoaderArgs> GetOlapDataLoader;

        void Raise_GetOlapDataLoader(GetIDataLoaderArgs args)
        {
            EventHandler<GetIDataLoaderArgs> handler = this.GetOlapDataLoader;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        public ILogService LogManager
        {
            get
            {
                return m_Coordinates.LogManager;
            }
            set
            {
                m_Coordinates.LogManager = value;
            }
        }
        
        CoordinatesControl m_Coordinates;
        ComboBoxEx m_CopyType;
        TextBox m_Coefficient;
        TextBox m_Value;
        RanetToolBar m_ToolBar;

        public bool IsAdminMode
        {
            get { return m_Coordinates.IsAdminMode; }
            set { m_Coordinates.IsAdminMode = value; }
        }

        public ValueCopyControl()
        {
            Grid LayoutRoot = new Grid();
            LayoutRoot.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            LayoutRoot.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            LayoutRoot.RowDefinitions.Add(new RowDefinition());

            Grid Row0_LayoutRoot = new Grid();
            Row0_LayoutRoot.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            Row0_LayoutRoot.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            Row0_LayoutRoot.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            Row0_LayoutRoot.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            Row0_LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition());
            Row0_LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition());
            //Row0_LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

            // Тип копирования
            TextBlock Label_CopyType = new TextBlock() { Text = Localization.ValueCopyControl_TypeOfCopying, Margin = new Thickness(0) };
            Row0_LayoutRoot.Children.Add(Label_CopyType);
            Grid.SetRow(Label_CopyType, 0);
            Grid.SetColumnSpan(Label_CopyType, 2);
            m_CopyType = new ComboBoxEx();
            m_CopyType.Margin = new Thickness(0, 5, 0, 0);
            
            ItemControlBase ctrl;
            ctrl = new ItemControlBase(false);
            ctrl.Text = Localization.ValueCopyControl_CopyValueFromSource;
            ctrl.Tag = ValueCopyTypes.CopyValueFromSource;
            m_CopyType.Combo.Items.Add(ctrl);
            ctrl = new ItemControlBase(false);
            ctrl.Text = Localization.ValueCopyControl_DeliveryValuePropotrionalSource;
            ctrl.Tag = ValueCopyTypes.DeliveryValuePropotrionalSource;
            m_CopyType.Combo.Items.Add(ctrl);
            Row0_LayoutRoot.Children.Add(m_CopyType);
            Grid.SetRow(m_CopyType, 1);
            Grid.SetColumnSpan(m_CopyType, 2);

            // Коэффициент
            TextBlock Label_Coefficient = new TextBlock() { Text = Localization.ValueCopyControl_Сoefficient, Margin = new Thickness(0, 5, 0, 0) };
            Row0_LayoutRoot.Children.Add(Label_Coefficient);
            Grid.SetRow(Label_Coefficient, 2);
            m_Coefficient = new Wing.AgOlap.Controls.General.RichTextBox() { Text = "1", Margin = new Thickness(0, 5, 0, 0) };
            Row0_LayoutRoot.Children.Add(m_Coefficient);
            Grid.SetRow(m_Coefficient, 3);

            // Значение
            TextBlock Label_Value = new TextBlock() { Text = Localization.ValueCopyControl_Value, Margin = new Thickness(5, 5, 0, 0) };
            Row0_LayoutRoot.Children.Add(Label_Value);
            Grid.SetRow(Label_Value, 2);
            Grid.SetColumn(Label_Value, 1);
            m_Value = new Wing.AgOlap.Controls.General.RichTextBox() { Text = "0", Margin = new Thickness(5, 5, 0, 0) };
            Row0_LayoutRoot.Children.Add(m_Value);
            Grid.SetRow(m_Value, 3);
            Grid.SetColumn(m_Value, 1);

            m_CopyType.SelectionChanged += new SelectionChangedEventHandler(m_CopyType_SelectionChanged);
            m_CopyType.Combo.SelectedIndex = 0;
            LayoutRoot.Children.Add(Row0_LayoutRoot);
            Grid.SetRow(Row0_LayoutRoot, 1);

            Grid Row1_LayoutRoot = new Grid();
            Row1_LayoutRoot.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            Row1_LayoutRoot.RowDefinitions.Add(new RowDefinition());

            // Координаты
            TextBlock Label_Coordinates = new TextBlock() { Text = Localization.ValueCopyControl_Coordinates, Margin = new Thickness(0, 5, 0, 0) };
            Row1_LayoutRoot.Children.Add(Label_Coordinates);
            Grid.SetRow(Label_Coordinates, 0);
            m_Coordinates = new CoordinatesControl();
            m_Coordinates.GetOlapDataLoader += new EventHandler<GetIDataLoaderArgs>(m_Coordinates_GetOlapDataLoader);
            m_Coordinates.Margin = new Thickness(5);

            Border coordinates_Border = new Border() { Margin = new Thickness(0,5,0,0), BorderBrush = new SolidColorBrush(Colors.DarkGray), BorderThickness = new Thickness(1) };
            coordinates_Border.Child = m_Coordinates;
            Row1_LayoutRoot.Children.Add(coordinates_Border);
            Grid.SetRow(coordinates_Border, 1);

            LayoutRoot.Children.Add(Row1_LayoutRoot);
            Grid.SetRow(Row1_LayoutRoot, 2);

            // Ожидание загрузки
            m_Waiting = new BusyControl();
            m_Waiting.Text = Localization.Loading;
            Row1_LayoutRoot.Children.Add(m_Waiting);
            Grid.SetRow(m_Waiting, 2);

            // ТУЛБАР 
            m_ToolBar = new RanetToolBar();
            m_ToolBar.Margin = new Thickness(0, 0, 0, 4);
            LayoutRoot.Children.Add(m_ToolBar);
            Grid.SetRow(m_ToolBar, 0);
            UseToolbar = false;

            RanetToolBarButton m_ImportLayout = new RanetToolBarButton();
            m_ImportLayout.Content = UiHelper.CreateIcon(UriResources.Images.FileImport16);
            m_ImportLayout.Click += new RoutedEventHandler(m_ImportLayout_Click);
            ToolTipService.SetToolTip(m_ImportLayout, Localization.ValueCopyControl_ImportSettings_ToolTip);
            m_ToolBar.AddItem(m_ImportLayout);

            RanetToolBarButton m_ExportLayout = new RanetToolBarButton();
            m_ExportLayout.Content = UiHelper.CreateIcon(UriResources.Images.FileExport16);
            m_ExportLayout.Click += new RoutedEventHandler(m_ExportLayout_Click);
            ToolTipService.SetToolTip(m_ExportLayout, Localization.ValueCopyControl_ExportSettings_ToolTip);
            m_ToolBar.AddItem(m_ExportLayout);

            IsBusy = false;
            this.Content = LayoutRoot;
        }

        #region Экспорт-импорт настроек

        void m_ExportLayout_Click(object sender, RoutedEventArgs e)
        {
            ValueCopySettingsWrapper wrapper = GetCopySettings();
            if (wrapper != null)
            {
                String str = XmlSerializationUtility.Obj2XmlStr(wrapper, Constants.XmlNamespace);
                ExportToStorage(str);
            }
        }

        void m_ImportLayout_Click(object sender, RoutedEventArgs e)
        {
            String str = ImportFromStorage();
            if (!String.IsNullOrEmpty(str))
            {
                ValueCopySettingsWrapper wrapper = XmlSerializationUtility.XmlStr2Obj<ValueCopySettingsWrapper>(str);
                Initialize(wrapper);
            }
        }

        public void Initialize(ValueCopySettingsWrapper wrapper)
        {
            if (wrapper != null)
            {
                m_Value.Text = wrapper.Value;
                m_Coefficient.Text = wrapper.Coefficient.ToString();
                if (wrapper.CopyType == ValueCopyTypes.CopyValueFromSource)
                {
                    m_CopyType.Combo.SelectedIndex = 0;
                }
                else
                {
                    m_CopyType.Combo.SelectedIndex = 1;
                }

                m_Coordinates.ShowNotEmpty = wrapper.ShowNotEmptyCoordinates;

                List<CoordinateItem> coordinates = new List<CoordinateItem>();
                foreach (CoordinateItem_Wrapper coord_wrap in wrapper.CoordinatesList)
                {
                    CoordinateItem item = new CoordinateItem(coord_wrap);
                    coordinates.Add(item);
                }

                m_Coordinates.Initialize(coordinates);
            }
        }

        void ExportToStorage(String str)
        {
            if (str != null)
            {
                IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication();
                if (isoStore != null)
                {
                    IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream(IsoStorageFile, System.IO.FileMode.Create, isoStore);
                    StreamWriter writer = new StreamWriter(isoStream);
                    writer.Write(str);
                    writer.Close();
                }
            }
        }

        public String ImportFromStorage()
        {
            String res = String.Empty;
            IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication();
            if (isoStore != null)
            {
                if (isoStore.FileExists(IsoStorageFile))
                {
                    IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream(IsoStorageFile, System.IO.FileMode.Open, isoStore);
                    StreamReader reader = new StreamReader(isoStream);
                    res = reader.ReadToEnd();
                    reader.Close();
                }
            }
            return res;
        }

        String m_IsoStorageFile = String.Empty;
        private String IsoStorageFile
        {
            get
            {
                if (String.IsNullOrEmpty(m_IsoStorageFile))
                {
                    m_IsoStorageFile = this.Name + "_VC.xml";
                    if (!String.IsNullOrEmpty(IsolatedStoragePrefix))
                    {
                        String[] items = IsolatedStoragePrefix.Split(new String[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                        if (items != null && items.Length > 0)
                            m_IsoStorageFile = items[items.Length - 1] + "_" + m_IsoStorageFile;
                    }
                }
                return m_IsoStorageFile;
            }
        }

        String m_IsolatedStoragePrefix = String.Empty;
        public String IsolatedStoragePrefix
        {
            get
            {
                return m_IsolatedStoragePrefix;
            }
            set
            {
                m_IsolatedStoragePrefix = value;
                m_IsoStorageFile = String.Empty;
            }
        }
        #endregion Экспорт-импорт настроек

        BusyControl m_Waiting;

        public bool m_IsBusy = false;
        public bool IsBusy 
        {
            get {
                return m_IsBusy;
            }
            set {
                if (value)
                {
                    m_Coordinates.IsEnabled = false;
                    m_Waiting.Visibility = Visibility.Visible;
                }
                else
                {
                    m_Coordinates.IsEnabled = true;
                    m_Waiting.Visibility = Visibility.Collapsed;
                }
                m_IsBusy = value;
            }
        }

        void m_Coordinates_GetOlapDataLoader(object sender, GetIDataLoaderArgs e)
        {
            Raise_GetOlapDataLoader(e);
        }

        void m_CopyType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            m_Coefficient.IsEnabled = m_CopyType.Combo.SelectedIndex == 0;
            m_Value.IsEnabled = !(m_CopyType.Combo.SelectedIndex == 0);
        }

        public event EventHandler LoadMetaData;
        void Raise_LoadMetaData()
        {
            EventHandler handler = LoadMetaData;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        IDictionary<String, MemberWrap> m_Slice = null;
        public void Initialize(IDictionary<String, MemberWrap> slice, object val)
        {
            m_Value.Text = "0";

            Initialize(slice);
            if(val != null)
                m_Value.Text = val.ToString();
        }

        public void Initialize(IDictionary<String, MemberWrap> slice)
        {
            m_Slice = slice;
            Raise_LoadMetaData();
        }

        public void InitializeMetadata(CubeDefInfo cubeInfo)
        {
            List<CoordinateItem> list = new List<CoordinateItem>();
            
            if(cubeInfo != null)
            {
                foreach (DimensionInfo dim_Info in cubeInfo.Dimensions)
                {
                    foreach (HierarchyInfo hier_info in dim_Info.Hierarchies)
                    {
                        CoordinateItem item = new CoordinateItem(dim_Info.UniqueName, hier_info.UniqueName);
                        item.Hierarchy_Custom_AllMemberUniqueName = hier_info.Custom_AllMemberUniqueName;
                        item.DimensionCaption = dim_Info.Caption;
                        item.HierarchyCaption = hier_info.Caption;
                        if (m_Slice != null && m_Slice.ContainsKey(hier_info.UniqueName))
                        {
                            MemberWrap wrap = m_Slice[hier_info.UniqueName];
                            item.SourceMember = wrap;

                            item.DestMember = wrap;
                            // Если в приемник попадает элемент типа ALL, то
                            // пытаемся заменить его на DefaultMember. Если же DefaultMember совпадет в итоге с AllMember
                            // то в приемник устанавливаем Unknown элемент
                            if (!String.IsNullOrEmpty(hier_info.Custom_AllMemberUniqueName) &&
                                hier_info.Custom_AllMemberUniqueName == wrap.UniqueName)
                            {
                                if (!String.IsNullOrEmpty(hier_info.DefaultMember) &&
                                    hier_info.DefaultMember == hier_info.Custom_AllMemberUniqueName)
                                {
                                    MemberWrap unk_wrap = new MemberWrap();
                                    unk_wrap.IsUnknownMember = true;
                                    item.DestMember = unk_wrap;
                                }
                                else
                                {
                                    if (!String.IsNullOrEmpty(hier_info.DefaultMember))
                                    {
                                        MemberWrap def_wrap = new MemberWrap();
                                        def_wrap.IsDefaultMember = true;
                                        item.DestMember = def_wrap;
                                    }
                                }
                            }
                        }
                        list.Add(item);
                    }
                }
            }
            m_Coordinates.Initialize(list);
        }

        private bool CheckSourceDestDiffs()
        {
            bool isSrcAndDestDiffs = false;

            IList<CoordinateItem> list = m_Coordinates.CoordinatesList;
            if (list != null)
            {
                foreach (CoordinateItem item in list)
                {
                    if (item.SourceMemberUniqueName != item.DestMemberUniqueName)
                    {
                        isSrcAndDestDiffs = true;
                        break;
                    }
                }
            }
            if (!isSrcAndDestDiffs)
                MessageBox.Show(Localization.ValueCopyControl_SourceAndDestIsEqual_Warning, Localization.MessageBox_Error, MessageBoxButton.OK);
            return isSrcAndDestDiffs;
        }

        private bool CheckSourceDestFieldsCompletion()
        {
            bool isSrcAndDestFldsFilledUp = true;

            IList<CoordinateItem> list = m_Coordinates.CoordinatesList;
            if (list != null)
            {
                foreach (CoordinateItem item in list)
                {
                    //Если хоть одно поле не заполнено
                    if (String.IsNullOrEmpty(item.SourceMemberUniqueName) != String.IsNullOrEmpty(item.DestMemberUniqueName))
                    {
                        isSrcAndDestFldsFilledUp = false;//Если хоть одно поле не заполнено
                        break;
                    }
                }
            }
            if (!isSrcAndDestFldsFilledUp)
                MessageBox.Show(Localization.ValueCopyControl_SourceAndDestFieldNotFilled_Warning, Localization.MessageBox_Error, MessageBoxButton.OK);
            return isSrcAndDestFldsFilledUp;
        }

        bool CheckDestFieldsToAllMembers()
        {
            bool hasAllMembers = false;
            IList<CoordinateItem> list = m_Coordinates.CoordinatesList;
            if (list != null)
            {
                foreach (CoordinateItem item in list)
                {
                    if (!String.IsNullOrEmpty(item.Hierarchy_Custom_AllMemberUniqueName) &&
                        item.Hierarchy_Custom_AllMemberUniqueName == item.DestMemberUniqueName)
                    {
                        hasAllMembers = true;
                        break;
                    }
                }
            }
            if (hasAllMembers)
                MessageBox.Show(Localization.ValueCopyControl_DestFieldIsAllMember, Localization.MessageBox_Error, MessageBoxButton.OK);
            return hasAllMembers;
        }

        public String BuildCopyScript()
        {
            String formattedQuery = String.Empty;

            if (!CheckSourceDestDiffs()) return formattedQuery;
            if (!CheckSourceDestFieldsCompletion()) return formattedQuery;
            if (CheckDestFieldsToAllMembers()) return formattedQuery;

            IList<CoordinateItem> list = m_Coordinates.CoordinatesList;
            if (list == null)
                return formattedQuery;

            #region destinationQuery
            string destinationQuery = null;
            bool first = true;
            foreach (CoordinateItem item in list)
            {
                if (!String.IsNullOrEmpty(item.DestMemberUniqueName))
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        destinationQuery += ", ";
                    }

                    destinationQuery += item.DestMemberUniqueName;
                }
            }
            destinationQuery = String.Format("({0})", destinationQuery);
            #endregion destinationQuery

            #region valueQuery
            string valueQuery = null;

            if (m_CopyType.Combo.SelectedIndex == 0)
            {
                first = true;
                foreach (CoordinateItem item in list)
                {
                    if (!String.IsNullOrEmpty(item.SourceMemberUniqueName))
                    {
                        if (first)
                        {
                            first = false;
                        }
                        else
                        {
                            valueQuery += ", ";
                        }

                        valueQuery += item.SourceMemberUniqueName;
                    }
                }
                valueQuery = String.Format("({0}) * {1}", valueQuery, m_Coefficient.Text);
            }
            else
            {
                valueQuery = m_Value.Text;
            }
            #endregion valueQuery

            #region currentTuple
            string currentTuple = null;

            first = true;
            foreach (CoordinateItem item in list)
            {
                if (!String.IsNullOrEmpty(item.DestMemberUniqueName))
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        currentTuple += ", ";
                    }

                    string currentMember = String.Format("{0}.CurrentMember", item.HierarchyUniqueName);

                    if (!String.IsNullOrEmpty(item.SourceMemberUniqueName))
                    {
                        currentMember = String.Format("COUSIN({0}, {1})", currentMember, item.SourceMemberUniqueName);
                    }

                    currentTuple += currentMember;
                }
            }
            currentTuple = String.Format("({0})", currentTuple);
            #endregion currentTuple

            #region goalTuple
            string goalTuple = null;

            first = true;
            foreach (CoordinateItem item in list)
            {
                if (!String.IsNullOrEmpty(item.SourceMemberUniqueName))
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        goalTuple += ", ";
                    }

                    goalTuple += item.SourceMemberUniqueName;
                }
            }
            //Получить измерения которые не использовались
            List<string> notUsedDimensions = new List<string>();
            foreach (CoordinateItem item in list)
            {
                if (!notUsedDimensions.Contains(item.DimensionUniqueName))
                {
                    notUsedDimensions.Add(item.DimensionUniqueName);
                }
            }

            foreach (CoordinateItem item in list)
            {
                if (!String.IsNullOrEmpty(item.SourceMemberUniqueName))
                {
                    if (notUsedDimensions.Contains(item.DimensionUniqueName))
                    {
                        notUsedDimensions.Remove(item.DimensionUniqueName);
                    }
                }
            }

            foreach (string rootDimension in notUsedDimensions)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    goalTuple += ", ";
                }

                goalTuple += String.Format("ROOT({0})", rootDimension);
            }

            goalTuple = String.Format("({0})", goalTuple);
            #endregion goalTuple

            string updateQuery = String.Format("UPDATE CUBE [{0}] SET {1} = {2} USE_WEIGHTED_ALLOCATION BY {3}/{4}", CubeName, destinationQuery, valueQuery, currentTuple, goalTuple);

            try
            {
                formattedQuery = OlapCmdUpdateStringFormatter.Format(updateQuery);
            }
            catch (Exception ex)
            {
                formattedQuery = updateQuery;
            }

            return formattedQuery;
        }

        public ValueCopySettingsWrapper GetCopySettings()
        {
            ValueCopySettingsWrapper wrapper = new ValueCopySettingsWrapper();
            if (m_CopyType.Combo.SelectedIndex == 0)
                wrapper.CopyType = ValueCopyTypes.CopyValueFromSource;
            else
                wrapper.CopyType = ValueCopyTypes.DeliveryValuePropotrionalSource;
            try
            {
                wrapper.Coefficient = Convert.ToDouble(m_Coefficient.Text);
            }
            catch {
                wrapper.Coefficient = 1;
            }
            wrapper.Value = m_Value.Text;
            wrapper.ShowNotEmptyCoordinates = m_Coordinates.ShowNotEmpty;

            foreach (CoordinateItem item in m_Coordinates.CoordinatesList)
            {
                CoordinateItem_Wrapper item_wrapper = new CoordinateItem_Wrapper(item);
                wrapper.CoordinatesList.Add(item_wrapper);
            }
            return wrapper;
        }

        public bool UseToolbar
        {
            get
            {
                if (m_ToolBar.Visibility == Visibility.Visible)
                    return true;
                return false;
            }
            set {
                if (value)
                {
                    m_ToolBar.Visibility = Visibility.Visible;
                }
                else
                {
                    m_ToolBar.Visibility = Visibility.Collapsed;
                }
            }
        }
    }
}
