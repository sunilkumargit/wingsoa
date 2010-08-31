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
using Wing.Olap.Controls.General.Tree;
using System.Collections.Generic;
using Wing.Olap.Controls.General;
using Wing.Olap.Commands;
using Wing.Olap.Controls.General.ClientServer;
using Wing.Olap.Core.Metadata;
using Wing.Olap.Core;
using System.Text;

namespace Wing.Olap.Controls
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
