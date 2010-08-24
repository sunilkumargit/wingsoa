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
using Ranet.AgOlap.Controls.General.Tree;
using Ranet.AgOlap.Controls.General;
using Ranet.Olap.Core.Metadata;
using Ranet.AgOlap.Controls.General.ClientServer;
using Ranet.AgOlap.Commands;
using Ranet.Olap.Core;
using System.Collections.Generic;
using Ranet.AgOlap.Controls.General.ItemControls;

namespace Ranet.AgOlap.Controls
{
    public class ServerExplorerCtrl : AgTreeControlBase
    {
        /// <summary>
        /// Указывает необходимость отображать информация о всех типах кубов (Cube, Dimension, Unknown)
        /// </summary>
        public bool ShowAllCubes;

        CubeBrowserCtrl m_CubeBrowser;
        public CubeBrowserCtrl CubeBrowser
        {
            get { return m_CubeBrowser; }
        }

        ComboBoxEx Cubes_ComboBox = null;

        public ServerExplorerCtrl()
        {
            m_CubeBrowser = new CubeBrowserCtrl();

            Grid LayoutRoot = new Grid();
            LayoutRoot.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            LayoutRoot.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            LayoutRoot.RowDefinitions.Add(new RowDefinition());

            var cubesComboHeader = new HeaderControl(UriResources.Images.Cube16, Localization.MdxDesigner_Cube) { Margin = new Thickness(0, 0, 0, 3) };

            Cubes_ComboBox = new ComboBoxEx();
            Cubes_ComboBox.SelectionChanged += new SelectionChangedEventHandler(Cubes_ComboBox_SelectionChanged);
            Cubes_ComboBox.Height = 22;
            Cubes_ComboBox.Margin = new Thickness(0, 0, 0, 5);

            LayoutRoot.Children.Add(cubesComboHeader);
            LayoutRoot.Children.Add(Cubes_ComboBox);
            Grid.SetRow(Cubes_ComboBox, 1);
            LayoutRoot.Children.Add(m_CubeBrowser);
            Grid.SetRow(m_CubeBrowser, 2);

            base.Content = LayoutRoot;

            m_OlapDataLoader = GetOlapDataLoader();

            m_CubeBrowser.OlapDataLoader = GetOlapDataLoader();
            
            m_CubeBrowser.SelectedItemChanged += new EventHandler<ItemEventArgs>(m_CubeBrowser_SelectedItemChanged);

            Cubes_ComboBox.IsEnabled = false;
            m_CubeBrowser.IsEnabled = false;
        }

        void m_CubeBrowser_SelectedItemChanged(object sender, ItemEventArgs e)
        {
            var info_Node = m_CubeBrowser.SelectedNode as InfoBaseTreeNode;
            InfoBase info = info_Node != null ? info_Node.Info : null;
            Raise_SelectedItemChanged(info);
        }

        public event EventHandler<CustomEventArgs<String>> CubeSelected;

        Dictionary<String, CubeDefInfo> m_MetadataCache = new Dictionary<string, CubeDefInfo>();

        void Cubes_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool initCubeBrowser = true;
            if (e.RemovedItems != null && e.RemovedItems.Count > 0)
            {
                EventHandler<CustomEventArgs<String>> handler = this.CubeSelected;
                if (handler != null)
                {
                    var args = new CustomEventArgs<String>(CurrentCubeName);
                    handler(this, args);
                    // Если событие обработано и пришла отмена, то значит переключать куб не надо
                    if (args.Cancel)
                    {
                        initCubeBrowser = false;
                        // Вручную возвращаем куб обратно
                        //Cubes_ComboBox.SelectionChanged -= new SelectionChangedEventHandler(Cubes_ComboBox_SelectionChanged);
                        //Cubes_ComboBox.Combo.SelectedItem = e.RemovedItems != null && e.RemovedItems.Count > 0 ? e.RemovedItems[0] : null;
                        //Cubes_ComboBox.SelectionChanged += new SelectionChangedEventHandler(Cubes_ComboBox_SelectionChanged);
                    }
                }
            }

            if (initCubeBrowser)
            {
                // Кэшируем если данные уже были загружены
                if (!String.IsNullOrEmpty(m_CubeBrowser.CubeName) &&
                    m_CubeBrowser.CubeInfo != null)
                {
                    if (m_MetadataCache.ContainsKey(m_CubeBrowser.CubeName))
                    {
                        m_MetadataCache[m_CubeBrowser.CubeName] = m_CubeBrowser.CubeInfo;
                    }
                    else
                    {
                        m_MetadataCache.Add(m_CubeBrowser.CubeName, m_CubeBrowser.CubeInfo);
                    }
                }
                
                m_CubeBrowser.CubeName = CurrentCubeName;
                // Если контрол просмотра куба настроен на тот куб, которым изначально был инициализирован данный контрол
                // то учитываем подкуб
                if (m_CubeBrowser.CubeName == CubeName)
                {
                    m_CubeBrowser.SubCube = SubCube;
                }
                else
                {
                    m_CubeBrowser.SubCube = String.Empty;
                }

                // Используем кэш если возможно
                if (m_MetadataCache.ContainsKey(m_CubeBrowser.CubeName) && m_MetadataCache[m_CubeBrowser.CubeName] != null)
                    m_CubeBrowser.Initialize(m_MetadataCache[m_CubeBrowser.CubeName]);
                else
                    m_CubeBrowser.Initialize();
            }
        }

        public void RefreshCubeMetadata()
        {
            m_CubeBrowser.CubeName = CurrentCubeName;
            m_CubeBrowser.Initialize();
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
                m_CubeBrowser.Connection = value;
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
                m_CubeName = value;
            }
        }

        private String m_SubCube = String.Empty;
        /// <summary>
        /// Подкуб
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
        #endregion Свойства для настройки на OLAP

        /// <summary>
        /// Имя текущего куба
        /// </summary>
        public String CurrentCubeName
        {
            get
            {
                CubeDefInfo info = CurrentCube;
                if (info != null)
                    return info.Name;
                return String.Empty;
            }
        }

        CubeDefInfo CurrentCube
        {
            get
            {
                CubeItemControl ctrl = Cubes_ComboBox.Combo.SelectedItem as CubeItemControl;
                if (ctrl != null)
                {
                    return ctrl.Info;
                }
                return null;
            }
        }

        public void Initialize()
        {
            m_MetadataCache.Clear();
            Cubes_ComboBox.Clear();
            m_CubeBrowser.Clear();

            Cubes_ComboBox.IsEnabled = CanSelectCube;
            if (String.IsNullOrEmpty(CubeName) && !CanSelectCube)
            {
                MessageBox.Show(Localization.ServerExplorer_CubePropertyError, Localization.ServerExplorerControl_Name, MessageBoxButton.OK);
                return;
            }

            if (CanSelectCube)
            {
                GetCubes();
            }
            else
            {
                // Если выбрать куб нельзя, то нет смысла запрашивать список кубов
                InitCubesList(new List<CubeDefInfo>() { new CubeDefInfo() { Name = CubeName, Caption = CubeName } });
            }
        }

        public bool SelectCube(String cubeName)
        {
            int i = 0;
            foreach (CubeItemControl item in Cubes_ComboBox.Combo.Items)
            {
                if (item.Info.Name == cubeName)
                {
                    Cubes_ComboBox.Combo.SelectedIndex = i;
                    return true;
                }
                i++;
            }
            if (Cubes_ComboBox.Combo.Items.Count > 0)
                Cubes_ComboBox.Combo.SelectedIndex = 0;
            else
                Cubes_ComboBox.Combo.SelectedIndex = -1;
            return false;
        }

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
                if(value == null)
                {
                    throw new ArgumentException("OlapDataLoader not must be null.");
                }
                if (m_OlapDataLoader != null)
                {
                    m_OlapDataLoader.DataLoaded -= new EventHandler<DataLoaderEventArgs>(OlapDataLoader_DataLoaded);
                }

                m_OlapDataLoader = value;
                m_CubeBrowser.OlapDataLoader = value;

                if (value != null)
                {
                    m_OlapDataLoader.DataLoaded += new EventHandler<DataLoaderEventArgs>(OlapDataLoader_DataLoaded);
                }
            }
        }

        void OlapDataLoader_DataLoaded(object sender, DataLoaderEventArgs e)
        {
            CustomTreeNode parentNode = null;

            // Метаданные
            UserSchemaWrapper<MetadataQuery, CustomTreeNode> metadata_wrapper = e.UserState as UserSchemaWrapper<MetadataQuery, CustomTreeNode>;
            if (metadata_wrapper != null && metadata_wrapper.Schema.QueryType == MetadataQueryType.GetCubes)
            {
                Cubes_ComboBox.Clear();
            }

            if (e.Error != null)
            {
                m_CubeBrowser.ShowErrorInTree(parentNode);
                if (metadata_wrapper != null && metadata_wrapper.Schema.QueryType == MetadataQueryType.GetCubes)
                {
                    Cubes_ComboBox.IsEnabled = false;
                    Cubes_ComboBox.Clear();
                }
                LogManager.LogError(this, e.Error.ToString());
                return;
            }

            if (e.Result.ContentType == InvokeContentType.Error)
            {
                if (metadata_wrapper != null && metadata_wrapper.Schema.QueryType == MetadataQueryType.GetCubes)
                {
                    Cubes_ComboBox.IsEnabled = false;
                    Cubes_ComboBox.Clear();
                }
                LogManager.LogError(this, e.Result.Content);
                return;
            }

            if (metadata_wrapper != null)
            {
                switch (metadata_wrapper.Schema.QueryType)
                {
                    case MetadataQueryType.GetCubes:
                        GetCubes_InvokeCommandCompleted(e, parentNode);
                        break;
                }
            }
        }

        #region Кубы
        void GetCubes()
        {
            m_CubeBrowser.IsEnabled = false;
            Cubes_ComboBox.SelectionChanged -= new SelectionChangedEventHandler(Cubes_ComboBox_SelectionChanged);
            Cubes_ComboBox.Clear();

            String NODE_TEXT = Localization.Loading;
            ComboBoxItem item = new ComboBoxItem();
            WaitTreeControl ctrl = new WaitTreeControl();
            ctrl.Text = NODE_TEXT;
            item.Content = ctrl;
            Cubes_ComboBox.Combo.Items.Add(item);
            Cubes_ComboBox.Combo.SelectedIndex = 0;
            Cubes_ComboBox.SelectionChanged += new SelectionChangedEventHandler(Cubes_ComboBox_SelectionChanged);

            MetadataQuery args = CommandHelper.CreateGetCubesQueryArgs(Connection);
            if (OlapDataLoader != null)
            {
                OlapDataLoader.DataLoaded -= new EventHandler<DataLoaderEventArgs>(OlapDataLoader_DataLoaded);
                OlapDataLoader.DataLoaded += new EventHandler<DataLoaderEventArgs>(OlapDataLoader_DataLoaded);
                LogManager.LogInformation(this, this.Name + " - Loading cubes.");
                OlapDataLoader.LoadData(args, new UserSchemaWrapper<MetadataQuery, CustomTreeNode>(args, null));
            }
            else
            { 
                throw new Exception("OlapDataLoader NotFiniteNumberException initialized.");
            }
        }

        void GetCubes_InvokeCommandCompleted(DataLoaderEventArgs e, CustomTreeNode parentNode)
        {
            OlapDataLoader.DataLoaded -= new EventHandler<DataLoaderEventArgs>(OlapDataLoader_DataLoaded);
            List<CubeDefInfo> cubes = XmlSerializationUtility.XmlStr2Obj<List<CubeDefInfo>>(e.Result.Content);
            InitCubesList(cubes);
        }

        void InitCubesList(List<CubeDefInfo> cubes)
        {
            Cubes_ComboBox.Clear();
            Cubes_ComboBox.IsEnabled = cubes != null && CanSelectCube;
            m_CubeBrowser.IsEnabled = cubes != null;

            object toSelect = null;
            if (cubes != null)
            {
                foreach (CubeDefInfo info in cubes)
                {
                    if (ShowAllCubes || info.Type == CubeInfoType.Cube)
                    {
                        var item = new CubeItemControl(info, false);
                        if (info.Name == CubeName)
                            toSelect = item;
                        Cubes_ComboBox.Combo.Items.Add(item);
                    }
                }
            }

            if (toSelect != null)
            {
                Cubes_ComboBox.Combo.SelectedItem = toSelect;
            }
            else
            {
                if (Cubes_ComboBox.Combo.Items.Count > 0)
                {
                    Cubes_ComboBox.Combo.SelectedIndex = 0;
                }
            }
        }
        #endregion Кубы

        public override string URL
        {
            get
            {
                return base.URL;
            }
            set
            {
                base.URL = value;
                OlapDataLoader olapDataLoader = OlapDataLoader as OlapDataLoader;
                if (olapDataLoader != null)
                {
                    olapDataLoader.URL = value;
                }

                m_CubeBrowser.URL = value;
                OlapDataLoader cube_olapDataLoader = m_CubeBrowser.OlapDataLoader as OlapDataLoader;
                if (cube_olapDataLoader != null)
                {
                    cube_olapDataLoader.URL = value;
                }
            }
        }

        public override ILogService LogManager
        {
            get { return base.LogManager; }
            set
            {
                base.LogManager = value;
                m_CubeBrowser.LogManager = value;
            }
        }

        bool m_CanSelectCube = false;
        /// <summary>
        /// Возможность выбрать куб из списка
        /// </summary>
        public bool CanSelectCube
        {
            get { return m_CanSelectCube; }
            set { m_CanSelectCube = value; }
        }
    }
}