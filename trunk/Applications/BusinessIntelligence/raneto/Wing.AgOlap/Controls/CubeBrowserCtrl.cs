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
using Ranet.AgOlap.Controls.General.ClientServer;
using Ranet.AgOlap.Commands;
using Ranet.Olap.Core.Metadata;
using Ranet.Olap.Core;
using System.Collections.Generic;
using System.Windows.Controls.Primitives;
using Ranet.AgOlap.Controls.MdxDesigner.CalculatedMembers;

namespace Ranet.AgOlap.Controls
{

    public class CubeBrowserCtrl : OlapBrowserControl
    {
        public CubeBrowserCtrl()
        {
            m_MeasureGroupHeader.Visibility = m_ComboMeasureGroup.Visibility = Visibility.Visible;
            //m_TreeHeader.Visibility = Visibility.Visible;
        }

        CubeTreeNode m_CubeNode = null;

        protected override void RefreshTree()
        {
            base.RefreshTree();

            if (CubeInfo != null)
            {
                //// Будем выводить информацию для всего куба
                //m_CubeNode = new CubeTreeNode(CubeInfo);
                //m_CubeNode.MouseDoubleClick += new EventHandler<CustomEventArgs<CustomTreeNode>>(TreeNode_MouseDoubleClick);
                //AllowDragDrop(m_CubeNode);
                //Tree.Items.Add(m_CubeNode);

                // Добавляем узел KPIs
                KPIsFolderTreeNode kpisNode = new KPIsFolderTreeNode();
                kpisNode.MouseDoubleClick += new EventHandler<CustomEventArgs<CustomTreeNode>>(TreeNode_MouseDoubleClick);
                //m_CubeNode.Items.Add(kpisNode);
                Tree.Items.Add(kpisNode);
                CreateKPIs(kpisNode, CubeInfo, true);

                // Добавляем узел Measures
                m_Measures_Node = new MeasuresFolderTreeNode();
                m_Measures_Node.MouseDoubleClick += new EventHandler<CustomEventArgs<CustomTreeNode>>(TreeNode_MouseDoubleClick);
                m_Measures_Node.Text = "Measures";
                //m_CubeNode.Items.Add(m_Measures_Node);
                Tree.Items.Add(m_Measures_Node);
                m_Measures_Node.Icon = UriResources.Images.Measure16;
                CreateMeasures(m_Measures_Node, CubeInfo); 

                // Добавляем узел NamedSets
                m_Sets_Node = new FolderTreeNode();
                m_Sets_Node.MouseDoubleClick += new EventHandler<CustomEventArgs<CustomTreeNode>>(TreeNode_MouseDoubleClick);
                m_Sets_Node.Text = "Sets";
                if (m_Sets_Node != null)
                {
                    m_NamedSetFolderNodes["Sets"] = m_Sets_Node;
                }
                //m_CubeNode.Items.Add(m_Sets_Node);
                Tree.Items.Add(m_Sets_Node);
                CreateNamedSets(m_Sets_Node, CubeInfo);

                // Добавляем узел Calculations
                m_CustomCalculations_Node = new FolderTreeNode(UriResources.Images.CustomCalculations16, UriResources.Images.CustomCalculations16);
                m_CustomCalculations_Node.MouseDoubleClick += new EventHandler<CustomEventArgs<CustomTreeNode>>(TreeNode_MouseDoubleClick);
                m_CustomCalculations_Node.Text = Localization.MdxDesigner_Calculations;
                if (m_CubeNode != null)
                    m_CubeNode.Items.Add(m_CustomCalculations_Node);
                else
                    Tree.Items.Add(m_CustomCalculations_Node);
                CreateCalculations(m_CustomCalculations_Node);

                // Создаем измерения
                CreateDimensions(m_CubeNode, CubeInfo, true);

                //m_CubeNode.IsExpanded = true;
            }
        }
    }
}
