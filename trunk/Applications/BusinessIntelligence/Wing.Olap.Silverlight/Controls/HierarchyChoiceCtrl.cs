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
