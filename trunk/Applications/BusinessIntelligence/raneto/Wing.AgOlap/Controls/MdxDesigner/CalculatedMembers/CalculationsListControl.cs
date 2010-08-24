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
using System.Collections.Generic;

namespace Ranet.AgOlap.Controls.MdxDesigner.CalculatedMembers
{
    public class CalculationsListControl: UserControl
    {
        protected CustomTree m_Tree;
        Grid grdIsWaiting;

        FolderTreeNode m_CalculatedMembers_Node;
        FolderTreeNode m_NamedSets_Node;
        
        public CalculationsListControl()
        {
            Grid LayoutRoot = new Grid();
            
            m_Tree = new CustomTree() { BorderBrush = new SolidColorBrush(Colors.DarkGray) };
            m_Tree.SelectedItemChanged += new RoutedPropertyChangedEventHandler<object>(m_Tree_SelectedItemChanged);
            LayoutRoot.Children.Add(m_Tree);

            m_CalculatedMembers_Node = new FolderTreeNode();
            m_CalculatedMembers_Node.Text = Localization.CalculationsListControl_CalculatedMembers_TreeNode;
            m_Tree.Items.Add(m_CalculatedMembers_Node);

            m_NamedSets_Node = new FolderTreeNode();
            m_NamedSets_Node.Text = Localization.CalculationsListControl_NamedSets_TreeNode;
            m_Tree.Items.Add(m_NamedSets_Node);

            m_CalculatedMembers_Node.IsExpanded = true;
            m_NamedSets_Node.IsExpanded = true;

            this.Content = LayoutRoot;

            //this.Loaded += new RoutedEventHandler(ObjectsListControlBase_Loaded);
        }

        void ClearTree()
        {
            m_CalculatedMembers_Node.Items.Clear();
            m_NamedSets_Node.Items.Clear();
        }

        //void ObjectsListControlBase_Loaded(object sender, RoutedEventArgs e)
        //{
        //    if (m_Tree.Items.Count > 0)
        //    {
        //        TreeNode<T> node = m_Tree.Items[0] as TreeNode<T>;
        //        if (node != null)
        //        {
        //            node.IsSelected = true;
        //        }
        //    }
        //}

        public event EventHandler<SelectionChangedEventArgs<CalculationInfoBase>> SelectionChanged;

        void m_Tree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeNode<CalculationInfoBase> node_old = e.OldValue as TreeNode<CalculationInfoBase>;
            CalculationInfoBase descr_old = null;
            if (node_old != null)
            {
                descr_old = node_old.Info;
            }

            TreeNode<CalculationInfoBase> node_new = e.NewValue as TreeNode<CalculationInfoBase>;
            CalculationInfoBase descr_new = null;
            if (node_new != null)
            {
                descr_new = node_new.Info;
            }
            //else
            //{
            //    // Иногда по неясным причинам в e.NewValue приходит {object} хотя при установке IsSelected = true узел был корректный. Поэтому в случае если узел определить не удалось, считаем что выбран нулевой узел
            //    if (m_CalculatedMembers_Node.Items.Count > 0)
            //    {
            //        TreeNode<CalculationInfoBase> node0 = m_CalculatedMembers_Node.Items[0] as TreeNode<CalculationInfoBase>;
            //        if (node0 != null)
            //        {
            //            descr_new = node0.Info;
            //        }
            //    }
            //    if(descr_new == null && m_NamedSets_Node.Items.Count > 0)
            //    {
            //        TreeNode<CalculationInfoBase> node0 = m_NamedSets_Node.Items[0] as TreeNode<CalculationInfoBase>;
            //        if (node0 != null)
            //        {
            //            descr_new = node0.Info;
            //        }
            //    }

            //}

            EventHandler<SelectionChangedEventArgs<CalculationInfoBase>> handler = SelectionChanged;
            if (handler != null)
            {
                handler(this, new SelectionChangedEventArgs<CalculationInfoBase>(descr_old, descr_new));
            }
        }

        public void RemoveItem(CalculationInfoBase item)
        {
            if (item != null)
            {
                // Сами попытаемся найти выбранный элемент.
                foreach (object obj in m_CalculatedMembers_Node.Items)
                {
                    TreeNode<CalculationInfoBase> x = obj as TreeNode<CalculationInfoBase>;
                    if (x != null && x.Info == item)
                    {
                        m_CalculatedMembers_Node.Items.Remove(obj);
                        return;
                    }
                }

                foreach (object obj in m_NamedSets_Node.Items)
                {
                    TreeNode<CalculationInfoBase> x = obj as TreeNode<CalculationInfoBase>;
                    if (x != null && x.Info == item)
                    {
                        m_NamedSets_Node.Items.Remove(obj);
                        return;
                    }
                }            
            }
        }

        public CalculationInfoBase CurrentObject
        {
            get
            {
                TreeNode<CalculationInfoBase> node = null;
                // m_Tree.SelectedItem иногда равно null. Несмотря на то что среди узлов есть такой, у которого IsSlected == true
                if (m_Tree.SelectedItem == null)
                {
                    // Сами попытаемся найти выбранный элемент.
                    foreach (object obj in m_CalculatedMembers_Node.Items)
                    {
                        TreeNode<CalculationInfoBase> x = obj as TreeNode<CalculationInfoBase>;
                        if (x != null && x.IsSelected)
                        {
                            node = x;
                            break;
                        }
                    }

                    if (node == null)
                    {
                        foreach (object obj in m_NamedSets_Node.Items)
                        {
                            TreeNode<CalculationInfoBase> x = obj as TreeNode<CalculationInfoBase>;
                            if (x != null && x.IsSelected)
                            {
                                node = x;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    node = m_Tree.SelectedItem as TreeNode<CalculationInfoBase>;
                }

                if (node != null)
                {
                    return node.Info;
                }
                return null;
            }
            set
            {
                if (value != null)
                {
                    TreeViewItem selected = null;
                    foreach (object obj in m_CalculatedMembers_Node.Items)
                    {
                        TreeViewItem item = obj as TreeViewItem;
                        if (item != null)
                        {
                            item.IsSelected = false;
                            TreeNode<CalculationInfoBase> node = item as TreeNode<CalculationInfoBase>;
                            if (node != null && node.Info.Equals(value))
                            {
                                selected = item;
                                break;
                            }
                        }
                    }

                    if (selected == null)
                    {
                        foreach (object obj in m_NamedSets_Node.Items)
                        {
                            TreeViewItem item = obj as TreeViewItem;
                            if (item != null)
                            {
                                item.IsSelected = false;
                                TreeNode<CalculationInfoBase> node = item as TreeNode<CalculationInfoBase>;
                                if (node != null && node.Info.Equals(value))
                                {
                                    selected = item;
                                    break;
                                }
                            }
                        }
                    }

                    if (selected != null)
                    {
                        m_Tree.SelectedItemChanged -= new RoutedPropertyChangedEventHandler<object>(m_Tree_SelectedItemChanged);
                        selected.IsSelected = true;
                        selected.Focus();
                        EventHandler<SelectionChangedEventArgs<CalculationInfoBase>> handler = SelectionChanged;
                        if (handler != null)
                        {
                            handler(this, new SelectionChangedEventArgs<CalculationInfoBase>(null, value));
                        }
                        m_Tree.SelectedItemChanged += new RoutedPropertyChangedEventHandler<object>(m_Tree_SelectedItemChanged);
                    }
                }
            }
        }

        public void Initialize(Dictionary<String, CalculationInfoBase> members, Dictionary<String, CalculationInfoBase> sets)
        {
            Initialize(members, sets, null);
        }

        public void Initialize(Dictionary<String, CalculationInfoBase> members, Dictionary<String, CalculationInfoBase> sets, CalculationInfoBase toSelect)
        {
            ClearTree();

            // Default selected node
            TreeNode<CalculationInfoBase> selected = null;
            if (members != null)
            {
                foreach (CalculationInfoBase info in members.Values)
                {
                    TreeNode<CalculationInfoBase> node = new TreeNode<CalculationInfoBase>(info.Name, UriResources.Images.CustomMeasure16, info);
                    if(selected == null || info == toSelect)
                        selected = node;
                    m_CalculatedMembers_Node.Items.Add(node);
                }
            }

            if (sets != null)
            {
                foreach (CalculationInfoBase info in sets.Values)
                {
                    TreeNode<CalculationInfoBase> node = new TreeNode<CalculationInfoBase>(info.Name, UriResources.Images.CustomNamedSet16, info);
                    if (selected == null || info == toSelect)
                        selected = node;
                    m_NamedSets_Node.Items.Add(node);
                }
            }

            // Делаем выбранным узел (тот который указан, либо первый из имеющихся)
            if (selected != null)
            {
                // Через событие делаем узел выбранным (иначе на нем фокус не ставится)
                selected.Loaded += new RoutedEventHandler(node_Loaded);
            }
        }

        void node_Loaded(object sender, RoutedEventArgs e)
        {
            TreeNode<CalculationInfoBase> node = sender as TreeNode<CalculationInfoBase>;
            if (node != null)
            {
                node.Loaded -= new RoutedEventHandler(node_Loaded);
                node.IsSelected = true;
                node.Focus();
            }
        }

        public virtual void Refresh()
        {
            foreach (object obj in m_CalculatedMembers_Node.Items)
            {
                TreeNode<CalculationInfoBase> node = obj as TreeNode<CalculationInfoBase>;
                if (node != null && node.Info != null)
                {
                    node.Text = node.Info.Name;
                }
            }

            foreach (object obj in m_NamedSets_Node.Items)
            {
                TreeNode<CalculationInfoBase> node = obj as TreeNode<CalculationInfoBase>;
                if (node != null && node.Info != null)
                {
                    node.Text = node.Info.Name;
                }
            }
        }
    }
}

