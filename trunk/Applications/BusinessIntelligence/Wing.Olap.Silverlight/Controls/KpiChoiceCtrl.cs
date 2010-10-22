/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Windows;
using Wing.Olap.Controls.General;
using Wing.Olap.Controls.General.Tree;
using Wing.Olap.Core.Metadata;

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
