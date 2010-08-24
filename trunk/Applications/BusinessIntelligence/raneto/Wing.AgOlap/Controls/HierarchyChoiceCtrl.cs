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
using Ranet.AgOlap.Controls.General.Tree;
using Ranet.Olap.Core.Metadata;
using System.Collections.Generic;
using Ranet.AgOlap.Controls.General.ClientServer;
using Ranet.AgOlap.Commands;
using Ranet.Olap.Core;
using Ranet.Olap.Core.Providers;

namespace Ranet.AgOlap.Controls
{
    public class HierarchyChoiceCtrl : OlapBrowserControl, IChoiceControl
    {
        public HierarchyChoiceCtrl()
            : base()
        {
        }

        protected override void RefreshTree()
        {
            base.RefreshTree();

            if (CubeInfo != null)
            {
                // Если задано уникальное имя измерения, то создаем для данного измерения узел
                if (!String.IsNullOrEmpty(DimensionUniqueName))
                {
                    DimensionInfo info = CubeInfo.GetDimension(DimensionUniqueName);
                    if (info != null)
                    {
                        DimensionTreeNode dimNode = AddDimensionNode(null, info);
                        CreateHierarchies(dimNode, info, false);
                        dimNode.IsExpanded = true;
                        return;
                    }
                    else
                    {
                        LogManager.LogError(this, String.Format(Localization.MetadataResponseException_DimensionByUniqueName_InCube_NotFound, DimensionUniqueName, CubeName));
                    }
                }

                // Отображаем все измерения
                CreateDimensions(null, CubeInfo, true);
            }
        }

        protected override void CreateHierarchies(CustomTreeNode parentNode, DimensionInfo dimension, bool createLevels)
        {
            // Не создаем узлы для уровней
            base.CreateHierarchies(parentNode, dimension, false);
        }

        protected override void Tree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            HierarchyTreeNode node = e.NewValue as HierarchyTreeNode;
            m_IsReadyToSelection = node != null;

            base.Tree_SelectedItemChanged(sender, e);
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
        public String m_DimensionUniqueName = String.Empty;
        /// <summary>
        /// Имя OLAP измерения. Если не задано, то будут отображены все измерения
        /// </summary>
        public String DimensionUniqueName
        {
            get
            {
                return m_DimensionUniqueName;
            }
            set
            {
                m_DimensionUniqueName = value;
            }
        }
        #endregion Свойства для настройки на OLAP

        public HierarchyInfo SelectedInfo
        {
            get
            {
                HierarchyTreeNode node = null;
                node = SelectedNode as HierarchyTreeNode;
                if (node != null)
                {
                    //Запоминаем выбранный элемент
                    return node.Info as HierarchyInfo;
                }

                return null;
            }
        }

        protected override void TreeNode_MouseDoubleClick(object sender, CustomEventArgs<CustomTreeNode> e)
        {
            // Иерархии будут конечными узлами. Двойной клик на них будет равнозначен выбору
            HierarchyTreeNode node = e.Args as HierarchyTreeNode;
            if (node != null)
                Raise_ApplySelection();
        }

        /// <summary>
        /// Событие генерируется после окончания выбора
        /// </summary>
        public event EventHandler ApplySelection;

        /// <summary>
        /// Генерирует событие "Выбор окончен"
        /// </summary>
        private void Raise_ApplySelection()
        {
            EventHandler handler = ApplySelection;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }
    }
}
