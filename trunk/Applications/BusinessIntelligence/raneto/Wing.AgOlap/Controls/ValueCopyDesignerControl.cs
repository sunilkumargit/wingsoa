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
using Ranet.AgOlap.Controls.ToolBar;
using Ranet.Olap.Core.Providers.ClientServer;
using Ranet.AgOlap.Commands;
using Ranet.Olap.Core;
using Ranet.Olap.Core.Data;
using Ranet.Olap.Core.Providers;
using System.Collections.Generic;
using Ranet.Olap.Core.Metadata;
using Ranet.AgOlap.Controls.General.ClientServer;
using Ranet.AgOlap.Controls.PivotGrid;
using Ranet.AgOlap.Controls.ValueCopy;
using Ranet.AgOlap.Controls.ValueCopy.Wrappers;
using System.IO.IsolatedStorage;
using System.IO;
using Ranet.AgOlap.Controls.Forms;
using Ranet.Olap.Core.Storage;

namespace Ranet.AgOlap.Controls
{
    public class ValueCopyDesignerControl : AgControlBase
    {
        #region Свойства
        
        #region Свойства для настройки на OLAP
        private String m_Connection = String.Empty;
        /// <summary>
        /// Описание соединения с БД
        /// </summary>
        public String Connection
        {
            get { return m_Connection; }
            set { m_Connection = value; }
        }

        private string m_CubeName;
        /// <summary>
        /// Имя OLAP куба
        /// </summary>
        public String CubeName
        {
            get { return m_CubeName; }
            set { m_CubeName = value; }
        }

        #endregion Свойства для настройки на OLAP

        /// <summary>
        /// MDX запрос, первая ячейка которого будет служить источником координат для копирования
        /// </summary>
        public String Query { get; set; }

        #region Загрузчики
        IDataLoader m_OlapDataLoader = null;
        public IDataLoader OlapDataLoader
        {
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

        protected virtual IDataLoader GetOlapDataLoader()
        {
            return new OlapDataLoader(URL);
        }

        protected virtual IStorageManager GetStorageManager()
        {
            return new StorageManager(URL);
        }
        #endregion Загрузчики

        #endregion Свойства

        RanetToolBar m_ToolBar;
        Grid grdIsWaiting;
        ValueCopyControl m_CopyControl;

        public ValueCopyDesignerControl()
        {
            Grid LayoutRoot = new Grid();
            LayoutRoot.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            LayoutRoot.RowDefinitions.Add(new RowDefinition());

            // ТУЛБАР 
            m_ToolBar = new RanetToolBar();
            m_ToolBar.Margin = new Thickness(0, 0, 0, 4);
            LayoutRoot.Children.Add(m_ToolBar);
            Grid.SetRow(m_ToolBar, 0);

            // Контрол копирования данных
            m_CopyControl = new ValueCopyControl();
            m_CopyControl.IsAdminMode = true;
            LayoutRoot.Children.Add(m_CopyControl);
            Grid.SetRow(m_CopyControl, 1);

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

            m_ToolBar.AddItem(new RanetToolBarSplitter());

            RanetToolBarButton m_PreviewButton = new RanetToolBarButton();
            m_PreviewButton.Content = UiHelper.CreateIcon(UriResources.Images.Run16);
            m_PreviewButton.Click += new RoutedEventHandler(m_PreviewButton_Click);
            ToolTipService.SetToolTip(m_PreviewButton, Localization.ValueCopyControl_RunCopyForm_Tooltip);
            m_ToolBar.AddItem(m_PreviewButton);

            m_OlapDataLoader = GetOlapDataLoader();
            m_StorageManager = GetStorageManager();

            m_StorageManager.InvokeCompleted += new EventHandler<DataLoaderEventArgs>(StorageManager_ActionCompleted);
            m_OlapDataLoader.DataLoaded += new EventHandler<DataLoaderEventArgs>(OlapDataLoader_DataLoaded);

            grdIsWaiting = new Grid() { Background = new SolidColorBrush(Color.FromArgb(125, 0xFF, 0xFF, 0xFF)) };
            grdIsWaiting.Visibility = Visibility.Collapsed;
            BusyControl m_Waiting = new BusyControl();
            m_Waiting.Text = Localization.Loading;
            grdIsWaiting.Children.Add(m_Waiting);

            LayoutRoot.Children.Add(grdIsWaiting);
            Grid.SetColumnSpan(grdIsWaiting, LayoutRoot.ColumnDefinitions.Count > 0 ? LayoutRoot.ColumnDefinitions.Count : 1);
            Grid.SetRowSpan(grdIsWaiting, LayoutRoot.RowDefinitions.Count > 0 ? LayoutRoot.RowDefinitions.Count : 1);

            this.Content = LayoutRoot;
        }

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
                    ValueCopySettingsWrapper wrapper = XmlSerializationUtility.XmlStr2Obj<ValueCopySettingsWrapper>(e.Result.Content);
                    m_CopyControl.Initialize(wrapper);
                }
            }
        }

        void m_PreviewButton_Click(object sender, RoutedEventArgs e)
        {
            ModalDialog dlg = new ModalDialog();
            dlg.MinHeight = 300;
            dlg.MinWidth = 400;
            dlg.Height = 500;
            dlg.Width = 600;
            dlg.Caption = Localization.ValueCopyDialog_Caption;
            dlg.DialogOk += new EventHandler<DialogResultArgs>(ValueCopyDialog_OkButtonClick);

            ValueCopyControl CopyControl = new ValueCopyControl();
            CopyControl.CubeName = CubeName;
            CopyControl.ConnectionID = Connection;
            CopyControl.GetOlapDataLoader += new EventHandler<GetIDataLoaderArgs>(CopyControl_GetOlapDataLoader);
            CopyControl.LogManager = this.LogManager;
            CopyControl.Initialize(m_CopyControl.GetCopySettings());
            dlg.Content = CopyControl;

            dlg.Show();
        }

        void CopyControl_GetOlapDataLoader(object sender, GetIDataLoaderArgs e)
        {
            e.Loader = GetOlapDataLoader();
            e.Handled = true;
        }

        void ValueCopyDialog_OkButtonClick(object sender, DialogResultArgs e)
        {
            ModalDialog dlg = sender as ModalDialog;
            if (dlg != null)
            {
                ValueCopyControl copyControl = dlg.Content as ValueCopyControl;
                if (copyControl != null)
                {
                    String query = copyControl.BuildCopyScript();
                    if (!String.IsNullOrEmpty(query))
                    {
                        MdxQueryArgs args = CommandHelper.CreateMdxQueryArgs(Connection, query);
                        args.Type = QueryTypes.Update;
                        OlapDataLoader.LoadData(args, sender);
                    }
                    else
                    {
                        e.Cancel = true;
                    }
                }
            }
        }

        #region Экспорт-импорт настроек

        void m_ExportLayout_Click(object sender, RoutedEventArgs e)
        {
            ObjectSaveAsDialog dlg = new ObjectSaveAsDialog(GetStorageManager()) { ContentType = StorageContentTypes.ValueCopySettings };
            dlg.DialogOk += new EventHandler(SaveAsDialog_DialogOk);
            dlg.Show();
        }

        void SaveAsDialog_DialogOk(object sender, EventArgs e)
        {
            ObjectSaveAsDialog dlg = sender as ObjectSaveAsDialog;
            if (dlg == null)
                return;

            String str = String.Empty;
            ValueCopySettingsWrapper wrapper = m_CopyControl.GetCopySettings();
            if (wrapper != null)
            {
                str = XmlSerializationUtility.Obj2XmlStr(wrapper, Common.Namespace);
            }

            StorageActionArgs args = new StorageActionArgs();
            args.ActionType = StorageActionTypes.Save;
            args.Content = str;
            args.ContentType = StorageContentTypes.ValueCopySettings;
            ObjectDescription descr = dlg.Object;
            if (descr != null && !String.IsNullOrEmpty(descr.Name))
            {
                if (String.IsNullOrEmpty(descr.Caption))
                    descr.Caption = descr.Name;
                args.FileDescription = new ObjectStorageFileDescription(descr);
                StorageManager.Invoke(XmlSerializationUtility.Obj2XmlStr(args, Common.Namespace), args);
            }
            else
            {
                MessageBox.Show(Localization.ObjectSaveDialog_NameIsEmpty_Message, Localization.Warning, MessageBoxButton.OK);
            }
        }

        void m_ImportLayout_Click(object sender, RoutedEventArgs e)
        {
            ObjectLoadDialog dlg = new ObjectLoadDialog(GetStorageManager()) { ContentType = StorageContentTypes.ValueCopySettings };
            dlg.DialogOk += new EventHandler(LoadDialog_DialogOk);
            dlg.LogManager = LogManager;
            dlg.Show();
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
                args.ContentType = StorageContentTypes.ValueCopySettings;
                args.FileDescription = dlg.CurrentObject;
                StorageManager.Invoke(XmlSerializationUtility.Obj2XmlStr(args, Common.Namespace), args);
            }
        }
        #endregion Экспорт-импорт настроек

        void OlapDataLoader_DataLoaded(object sender, DataLoaderEventArgs e)
        {
            bool stopWaiting = true;
            try
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

                MdxQueryArgs mdx_args = e.UserState as MdxQueryArgs;
                if(mdx_args != null)
                {
                    CellSetData cs_descr = CellSetData.Deserialize(e.Result.Content);
                    InitializeTuple(cs_descr);

                    // Зачитываем метаданные куба в целом
                    stopWaiting = false;
                    LogManager.LogInformation(this, this.Name + " - Loading cube metadata.");
                    MetadataQuery args = CommandHelper.CreateGetCubeMetadataArgs(Connection, CubeName, MetadataQueryType.GetCubeMetadata_AllMembers);
                    OlapDataLoader.LoadData(args, args);
                }

                MetadataQuery metadata_args = e.UserState as MetadataQuery;
                if (metadata_args != null)
                {
                    CubeDefInfo cs_descr = XmlSerializationUtility.XmlStr2Obj<CubeDefInfo>(e.Result.Content);
                    m_CopyControl.InitializeMetadata(cs_descr);
                }
            }
            finally
            {
                if (stopWaiting)
                    IsWaiting = false;
            }
        }

        bool m_IsWaiting = false;
        protected bool IsWaiting
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
                    }
                    else
                    {
                        this.Cursor = Cursors.Arrow;
                        grdIsWaiting.Visibility = Visibility.Collapsed;
                    }
                    this.IsEnabled = !value;
                    m_IsWaiting = value;
                }
            }
        }

        public void Initialize()
        {
            m_CopyControl.CubeName = CubeName;
            m_CopyControl.ConnectionID = Connection;
            m_CopyControl.GetOlapDataLoader += new EventHandler<GetIDataLoaderArgs>(m_CopyControl_GetOlapDataLoader);

            IsWaiting = true;
            LogManager.LogInformation(this, this.Name + " - Calculating Tuple form cell.");
            // MDX запрос может служить источником Tuple. Если запрос задан, то пытаемся его выполнить и в качестве Tuple будем использовать координаты первой ячейки.
            if (!String.IsNullOrEmpty(Query))
            {
                MdxQueryArgs args = CommandHelper.CreateMdxQueryArgs(Connection, Query);
                OlapDataLoader.LoadData(args, args);
            }
            else
            {
                // Зачитываем метаданне куба в целом
                MetadataQuery args = CommandHelper.CreateGetCubeMetadataArgs(Connection, CubeName, MetadataQueryType.GetCubeMetadata_AllMembers);
                OlapDataLoader.LoadData(args, args);
            }
        }

        void m_CopyControl_GetOlapDataLoader(object sender, GetIDataLoaderArgs e)
        {
            e.Loader = GetOlapDataLoader();
            e.Handled = true;
        }

        void InitializeTuple(CellSetData cs_descr)
        {
            if (cs_descr != null)
            {
                CellSetDataProvider  cellSetProvider = new CellSetDataProvider(cs_descr);
                CellInfo cell = cellSetProvider.GetCellInfo(0, 0);
                if (cell != null)
                {
                    IDictionary<String, MemberWrap> slice = new Dictionary<String, MemberWrap>();
                    IDictionary<String, MemberInfo> tuple = cell.GetTuple();
                    foreach (String hierarchyUniqueName in tuple.Keys)
                    {
                        slice.Add(hierarchyUniqueName, new MemberWrap(tuple[hierarchyUniqueName]));
                    }
                    m_CopyControl.Initialize(slice);
                }
            }
        }
    }
}
