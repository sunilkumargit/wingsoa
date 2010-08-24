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
using System.Collections.Generic;
using Ranet.AgOlap.Controls.General;
using Ranet.AgOlap.Commands;
using Ranet.AgOlap.Controls.General.ClientServer;
using Ranet.Olap.Core.Metadata;
using Ranet.Olap.Core;
using System.Text;

namespace Ranet.AgOlap.Controls
{
    public class KpiChoiceCtrl : OlapBrowserControl, IChoiceControl
    {
        public KpiChoiceCtrl() : base()
        {
        }

        protected override void RefreshTree()
        {
            base.RefreshTree();

            // Добавляем узел KPIs
            KPIsFolderTreeNode kpisNode = new KPIsFolderTreeNode();
            kpisNode.MouseDoubleClick += new EventHandler<CustomEventArgs<CustomTreeNode>>(TreeNode_MouseDoubleClick);
            Tree.Items.Add(kpisNode);
            
            // Для варианта выбора KPI (OlapBrowserContentTypes.Kpis) Value, Goal, Status, Trend не отображаются
            CreateKPIs(kpisNode, CubeInfo, false);
            kpisNode.IsExpanded = true;
        }

        protected override void Tree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            KpiTreeNode node = e.NewValue as KpiTreeNode;
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

        public KpiInfo SelectedInfo
        {
            get
            {
                KpiTreeNode node = null;
                node = SelectedNode as KpiTreeNode;
                if (node != null)
                {
                    //Запоминаем выбранный элемент
                    return node.Info as KpiInfo;
                }

                return null;
            }
        }

        protected override void TreeNode_MouseDoubleClick(object sender, CustomEventArgs<CustomTreeNode> e)
        {
            // KPI будут конечными узлами. Двойной клик на них будет равнозначен выбору
            KpiTreeNode node = e.Args as KpiTreeNode;
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
