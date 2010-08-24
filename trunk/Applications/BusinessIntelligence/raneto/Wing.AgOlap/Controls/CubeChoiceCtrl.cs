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

namespace Ranet.AgOlap.Controls
{
    public class CubeChoiceCtrl : AgTreeControlBase
    {
        /// <summary>
        /// Указывает необходимость отображать информация о всех типах кубов (Cube, Dimension, Unknown)
        /// </summary>
        public bool ShowAllCubes;

        CustomTree Tree = null;

        public CubeChoiceCtrl()
        {
            Tree = new CustomTree();

            Grid LayoutRoot = new Grid();
            base.Content = LayoutRoot;
            LayoutRoot.Children.Add(Tree);
            Tree.SelectedItemChanged += new RoutedPropertyChangedEventHandler<object>(Tree_SelectedItemChanged);
        }

        void Tree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ApplyItemsSelection();
            m_IsReadyToSelection = SelectedInfo != null;
            Raise_SelectedItemChanged(SelectedInfo);
        }

        bool m_IsReadyToSelection = false;
        public bool IsReadyToSelection
        {
            get
            {
                return m_IsReadyToSelection;
            }
        }

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

        #endregion Свойства для настройки на OLAP

        private CubeDefInfo m_SelectedInfo = null;
        public CubeDefInfo SelectedInfo
        {
            get {
                return m_SelectedInfo;
            }
        }

        public void Initialize()
        {
            RefreshTree();
        }

        private void RefreshTree()
        {
            Tree.Items.Clear();
            m_GroupNodes.Clear();

            // Добавляем узел Measures
            FolderTreeNode cubesNode = new FolderTreeNode();
            cubesNode.Text = "Cubes";
            Tree.Items.Add(cubesNode);

            cubesNode.IsWaiting = true;
            cubesNode.IsExpanded = true;
            
            MetadataQuery args = CommandHelper.CreateGetCubesQueryArgs(Connection);
            Loader.LoadData(args, cubesNode);
        }

        IDataLoader m_Loader = null;
        public IDataLoader Loader
        {
            set
            {
                if (m_Loader != null)
                {
                    m_Loader.DataLoaded -= new EventHandler<DataLoaderEventArgs>(Loader_DataLoaded);
                }
                m_Loader = value;
                m_Loader.DataLoaded += new EventHandler<DataLoaderEventArgs>(Loader_DataLoaded);
            }
            get
            {
                if (m_Loader == null)
                {
                    m_Loader = new OlapDataLoader(URL);
                    m_Loader.DataLoaded += new EventHandler<DataLoaderEventArgs>(Loader_DataLoaded);
                }
                return m_Loader;
            }
        }

        void ShowErrorInTree(CustomTreeNode parentNode)
        {
            if (parentNode != null)
            {
                parentNode.IsError = true;
            }
            else
            {
                Tree.IsError = true;
            }
        }

        void Loader_DataLoaded(object sender, DataLoaderEventArgs e)
        {
            CustomTreeNode parentNode = e.UserState as CustomTreeNode;
            if (parentNode != null)
            {
                parentNode.IsWaiting = false;
            }
            else
            {
                Tree.IsWaiting = false;
            }

            if (e.Error != null)
            {
                ShowErrorInTree(parentNode);
                LogManager.LogError(this, e.Error.ToString());
                return;
            }

            if (e.Result.ContentType == InvokeContentType.Error)
            {
                ShowErrorInTree(parentNode);
                LogManager.LogError(this, e.Result.Content);
                return;
            }

            List<CubeDefInfo> cubes = XmlSerializationUtility.XmlStr2Obj<List<CubeDefInfo>>(e.Result.Content);
            if (cubes != null)
            {
                foreach (CubeDefInfo info in cubes)
                {
                    if (ShowAllCubes || info.Type == CubeInfoType.Cube)
                    {
                        CubeTreeNode node = new CubeTreeNode(info);
                        // Кубы будут конечными узлами. Двойной клик на них будет равнозначен выбору
                        node.Expanded += new RoutedEventHandler(node_Expanded);
                        node.Collapsed += new RoutedEventHandler(node_Collapsed);

                        if (parentNode == null)
                            Tree.Items.Add(node);
                        else
                            parentNode.Items.Add(node);
                    }
                }
            }
        }

        Dictionary<String, CustomTreeNode> m_GroupNodes = new Dictionary<String, CustomTreeNode>();

        void node_Collapsed(object sender, RoutedEventArgs e)
        {
            ApplyItemsSelection();
            Raise_ApplySelection();
        }

        void node_Expanded(object sender, RoutedEventArgs e)
        {
            ApplyItemsSelection();
            Raise_ApplySelection();
        }

        /// <summary>
        /// Событие генерируется после окончания выбора
        /// </summary>
        public event EventHandler ApplySelection;

        /// <summary>
        /// Событие генерируется при нажатии на кнопку Отмена
        /// </summary>
        public event EventHandler CancelSelection;

        /// <summary>
        /// Генерирует событие "Выбор окончен"
        /// </summary>
        private void Raise_ApplySelection()
        {
            EventHandler handler = ApplySelection;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        /// <summary>
        /// Генерирует событие "Отмена"
        /// </summary>
        private void Raise_CancelSelection()
        {
            EventHandler handler = CancelSelection;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            ApplyItemsSelection();
            Raise_ApplySelection();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Генерируем событие - Отмена
            Raise_CancelSelection();
        }

        private void ApplyItemsSelection()
        {
            CubeTreeNode node = null;
            node = Tree.SelectedItem as CubeTreeNode;

            //Запоминаем выбранный элемент
            m_SelectedInfo = node != null ? node.Info as CubeDefInfo : null;
        }
    }
}