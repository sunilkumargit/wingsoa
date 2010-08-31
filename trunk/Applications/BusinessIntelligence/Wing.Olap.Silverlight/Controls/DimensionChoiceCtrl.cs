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
using Wing.Olap.Core.Metadata;
using Wing.Olap.Controls.General.ClientServer;
using Wing.Olap.Commands;
using Wing.Olap.Core;
using System.Collections.Generic;

namespace Wing.Olap.Controls
{
    public class DimensionChoiceCtrl : OlapBrowserControl, IChoiceControl
    {
        public DimensionChoiceCtrl()
        {
        }

        protected override void RefreshTree()
        {
            base.RefreshTree();

            // Создаем измерения
            CreateDimensions(null, CubeInfo, false);
        }

        protected override void Tree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            DimensionTreeNode node = e.NewValue as DimensionTreeNode;
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

        public DimensionInfo SelectedInfo
        {
            get
            {
                DimensionTreeNode node = null;
                node = SelectedNode as DimensionTreeNode;
                if (node != null)
                {
                    //Запоминаем выбранный элемент
                    return node.Info as DimensionInfo;
                }

                return null;
            }
        }

        protected override void TreeNode_MouseDoubleClick(object sender, CustomEventArgs<CustomTreeNode> e)
        {
            // Измерения будут конечными узлами. Двойной клик на них будет равнозначен выбору
            DimensionTreeNode node = e.Args as DimensionTreeNode;
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
