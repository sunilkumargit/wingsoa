/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
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
using Wing.Olap.Controls.General;
using Wing.Olap.Controls.General.Tree;
using Wing.Olap.Controls.General.ClientServer;
using Wing.Olap.Commands;
using Wing.Olap.Core.Metadata;
using Wing.Olap.Core;
using System.Collections.Generic;
using System.Windows.Controls.Primitives;
using Wing.Olap.Controls.MdxDesigner.CalculatedMembers;

namespace Wing.Olap.Controls
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
