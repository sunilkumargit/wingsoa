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
using System.Windows.Controls;

namespace Ranet.AgOlap.Controls.General.Tree
{
    public enum SpecialNodes
    {
        Wait,
        Error,
        LoadNext
    }

    public class CustomTree : TreeView
    {
        public CustomTree()
        {
            DefaultStyleKey = typeof(CustomTree);
            this.MouseEnter += new MouseEventHandler(CustomTree_MouseEnter);
        }

        ~CustomTree()
        {
            Ranet.AgOlap.Features.ScrollViewerMouseWheelSupport.RemoveMouseWheelSupport(this);
        }

        void CustomTree_MouseEnter(object sender, MouseEventArgs e)
        {
            
        }

        ScrollViewer Scroller = null;
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Scroller = base.GetTemplateChild("ScrollViewer") as ScrollViewer;
            if (Scroller != null)
            {
                Ranet.AgOlap.Features.ScrollViewerMouseWheelSupport.AddMouseWheelSupport(Scroller, this);
            }
        }

        public bool IsWaiting
        {
            set
            {
                IsError = false;
                TreeViewItem waitItem = GetSpecialNode(SpecialNodes.Wait);
                if (!value)
                {
                    if (waitItem != null)
                    {

                        Items.Remove(waitItem);
                    }
                }
                else
                {
                    if (waitItem == null)
                        AddSpecialNode(SpecialNodes.Wait);
                }
            }
            get
            {
                if (GetSpecialNode(SpecialNodes.Wait) != null)
                    return true;
                else
                    return false;
            }
        }

        public bool IsError
        {
            set
            {
                TreeViewItem errorItem = GetSpecialNode(SpecialNodes.Error);
                if (!value)
                {
                    if (errorItem != null)
                    {

                        Items.Remove(errorItem);
                    }
                }
                else
                {
                    if (errorItem == null)
                        AddSpecialNode(SpecialNodes.Error);
                }
            }
            get
            {
                if (GetSpecialNode(SpecialNodes.Error) != null)
                    return true;
                else
                    return false;
            }
        }

        public bool IsFullLoaded
        {
            set
            {
                TreeViewItem nextItem = GetSpecialNode(SpecialNodes.LoadNext);
                //TreeViewItem allItem = GetSpecialNode(SpecialNodes.LoadAll);
                if (value)
                {
                    LoadNextTreeNode next = nextItem as LoadNextTreeNode;
                    if (next != null)
                    {
                        next.MouseDoubleClick -= new EventHandler<CustomEventArgs<CustomTreeNode>>(SpecialNode_MouseDoubleClick);
                        Items.Remove(nextItem);
                    }

                    //if (allItem != null)
                    //{
                    //    allItem.Expanded -= new RoutedEventHandler(node_Expanded);
                    //    Items.Remove(allItem);
                    //}
                }
                else
                {
                    if (nextItem == null)
                        AddSpecialNode(SpecialNodes.LoadNext);
                    //if (allItem == null)
                    //    AddSpecialNode(SpecialNodes.LoadAll);
                }
            }
            get
            {
                if (GetSpecialNode(SpecialNodes.LoadNext) == null/* &&
                    GetSpecialNode(SpecialNodes.LoadAll) == null*/)
                    return true;
                else
                    return false;
            }
        }

        //public bool IsReloadAll
        //{
        //    set
        //    {
        //        TreeViewItem nextItem = GetSpecialNode(SpecialNodes.ReloadAll);
        //        if (!value)
        //        {
        //            if (nextItem != null)
        //            {
        //                nextItem.Expanded -= new RoutedEventHandler(node_Expanded);
        //                Items.Remove(nextItem);
        //            }
        //        }
        //        else
        //        {
        //            if (nextItem == null)
        //                AddSpecialNode(SpecialNodes.ReloadAll);
        //        }
        //    }
        //    get
        //    {
        //        if (GetSpecialNode(SpecialNodes.ReloadAll) == null)
        //            return false;
        //        else
        //            return true;
        //    }
        //}

        /// <summary>
        /// Проверяет загружались ли дочерние узлы. 
        /// </summary>
        public bool IsInialized
        {
            get
            {
                //Если у элемента один дочерний и он "WaitNode", то значит данные не грузились
                if (Items.Count == 1 && IsWaiting)
                {
                    return false;
                }
                return true;
            }
        }

        private TreeViewItem AddSpecialNode(SpecialNodes nodeType)
        {
            TreeViewItem node = null;
            switch (nodeType)
            {
                case SpecialNodes.Wait:
                    node = new WaitTreeNode();
                    break;
                case SpecialNodes.LoadNext:
                    LoadNextTreeNode new_node = new LoadNextTreeNode();
                    new_node.MouseDoubleClick += new EventHandler<CustomEventArgs<CustomTreeNode>>(SpecialNode_MouseDoubleClick);
                    node = new_node;
                    break;
                case SpecialNodes.Error:
                    node = new ErrorTreeNode();
                    break;

                //case SpecialNodes.LoadAll:
                //    node = new LoadAllTreeNode();
                //    node.Expanded += new RoutedEventHandler(node_Expanded);
                //    break;
                //case SpecialNodes.ReloadAll:
                //    node = new ReloadAllTreeNode();
                //    node.Expanded += new RoutedEventHandler(node_Expanded);
                //    break;
            }
            if (node != null)
            {
                Items.Add(node);
            }
            return node;
        }

        public event EventHandler<CustomEventArgs<CustomTreeNode>> Special_MouseDoubleClick;
        protected void SpecialNode_MouseDoubleClick(object sender, EventArgs e)
        {
            EventHandler<CustomEventArgs<CustomTreeNode>> handler = this.Special_MouseDoubleClick;
            if (handler != null)
            {
                handler(this, new CustomEventArgs<CustomTreeNode>(sender as CustomTreeNode));
            }
        }

        public TreeViewItem GetSpecialNode(SpecialNodes nodeType)
        {
            try
            {
                foreach (object obj in Items)
                {
                    switch (nodeType)
                    {
                        case SpecialNodes.Wait:
                            WaitTreeNode wait = obj as WaitTreeNode;
                            if (wait != null)
                                return wait;
                            break;
                        case SpecialNodes.LoadNext:
                            LoadNextTreeNode next = obj as LoadNextTreeNode;
                            if (next != null)
                                return next;
                            break;
                        case SpecialNodes.Error:
                            ErrorTreeNode error = obj as ErrorTreeNode;
                            if (error != null)
                                return error;
                            break;

                        //case SpecialNodes.LoadAll:
                        //    LoadAllTreeNode all = obj as LoadAllTreeNode;
                        //    if (all != null)
                        //        return all;
                        //    break;
                        //case SpecialNodes.ReloadAll:
                        //    ReloadAllTreeNode reload = obj as ReloadAllTreeNode;
                        //    if (reload != null)
                        //        return reload;
                        //    break;
                    }
                }
            }catch
            {
            }
            return null;
        }
    }
}
