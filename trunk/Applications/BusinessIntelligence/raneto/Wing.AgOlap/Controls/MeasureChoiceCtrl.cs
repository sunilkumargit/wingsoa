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
using Ranet.AgOlap.Controls.General.ClientServer;
using Ranet.AgOlap.Commands;
using Ranet.Olap.Core;
using Ranet.Olap.Core.Metadata;
using System.Collections.Generic;
using System.Text;

namespace Ranet.AgOlap.Controls
{
    public class MeasureChoiceCtrl : OlapBrowserControl, IChoiceControl
    {
        public MeasureChoiceCtrl()
        {
        }

        protected override void RefreshTree()
        {
            base.RefreshTree();

            // Добавляем узел Measures
            m_Measures_Node = new MeasuresFolderTreeNode();
            m_Measures_Node.MouseDoubleClick += new EventHandler<CustomEventArgs<CustomTreeNode>>(TreeNode_MouseDoubleClick);
            m_Measures_Node.Text = "Measures";
            Tree.Items.Add(m_Measures_Node);
            m_Measures_Node.Icon = UriResources.Images.Measure16;
            
            CreateMeasures(m_Measures_Node, CubeInfo);
            m_Measures_Node.IsExpanded = true;
        }

        protected override void  Tree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            MeasureTreeNode node = e.NewValue as MeasureTreeNode;
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

        public MeasureInfo SelectedInfo
        {
            get {
                MeasureTreeNode node = null;
                node = SelectedNode as MeasureTreeNode;
                if (node != null)
                {
                    //Запоминаем выбранный элемент
                    return node.Info as MeasureInfo;
                }

                return null;
            }
        }

        protected override void TreeNode_MouseDoubleClick(object sender, CustomEventArgs<CustomTreeNode> e)
        {
            // Показатели будут конечными узлами. Двойной клик на них будет равнозначен выбору
            MeasureTreeNode node = e.Args as MeasureTreeNode;
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
