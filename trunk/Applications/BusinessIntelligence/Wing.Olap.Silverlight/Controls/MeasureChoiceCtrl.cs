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
