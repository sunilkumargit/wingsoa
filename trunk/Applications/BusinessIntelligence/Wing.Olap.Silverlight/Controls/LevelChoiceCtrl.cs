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
using Wing.AgOlap.Controls.General.Tree;
using Wing.Olap.Core.Metadata;
using Wing.AgOlap.Controls.General;
using Wing.AgOlap.Controls.General.ClientServer;
using Wing.AgOlap.Commands;
using Wing.Olap.Core;
using System.Collections.Generic;
using System.Text;
using Wing.Olap.Core.Providers;

namespace Wing.AgOlap.Controls
{
    public class LevelChoiceCtrl : OlapBrowserControl, IChoiceControl
    {
        public LevelChoiceCtrl()
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
                        if (!String.IsNullOrEmpty(HierarchyUniqueName))
                        { 
                            HierarchyInfo hierarchyInfo = info.GetHierarchy(HierarchyUniqueName);
                            if(hierarchyInfo != null)
                            {
                                HierarchyTreeNode hierarchyNode = AddHierarchyNode(null, hierarchyInfo);
                                CreateLevels(hierarchyNode, hierarchyInfo, false);
                                hierarchyNode.IsExpanded = true;
                                return;
                            }
                            else
                            {
                                LogManager.LogError(this, String.Format(Localization.MetadataResponseException_HierarchyByUniqueName_InDimension_NotFound, HierarchyUniqueName, DimensionUniqueName));
                            }
                        }
                        else
                        {
                            DimensionTreeNode dimNode = AddDimensionNode(null, info);
                            CreateHierarchies(dimNode, info, true);
                            dimNode.IsExpanded = true;
                            return;
                        }
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

        protected override void Tree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            LevelTreeNode node = e.NewValue as LevelTreeNode;
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

        public String m_HierarchyUniqueName = String.Empty;
        /// <summary>
        /// Имя иерархии OLAP измерения. Если не задано, то будут отображены все иерархии
        /// </summary>
        public String HierarchyUniqueName
        {
            get
            {
                return m_HierarchyUniqueName;
            }
            set
            {
                m_HierarchyUniqueName = value;
            }
        }
        #endregion Свойства для настройки на OLAP

        public LevelInfo SelectedInfo
        {
            get
            {
                LevelTreeNode node = null;
                node = SelectedNode as LevelTreeNode;
                if (node != null)
                {
                    //Запоминаем выбранный элемент
                    return node.Info as LevelInfo;
                }

                return null;
            }
        }

        protected override void TreeNode_MouseDoubleClick(object sender, CustomEventArgs<CustomTreeNode> e)
        {
            // Уровни будут конечными узлами. Двойной клик на них будет равнозначен выбору
            LevelTreeNode node = e.Args as LevelTreeNode;
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