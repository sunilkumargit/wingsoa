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
using System.Windows.Media.Imaging;
using System.Windows.Controls.Primitives;

namespace Ranet.AgOlap.Controls.General.Tree
{
    public class CustomTreeNode : TreeViewItem
    {
        public String Text 
        {
            set {
                m_ItemCtrl.Text = value;
            }
            get {
                return m_ItemCtrl.Text;
            }
        }

        public BitmapImage Icon
        {
            set {
                m_ItemCtrl.Icon = value;
            }
            get {
                return m_ItemCtrl.Icon;
            }
        }

        public bool UseBoldText
        {
            get { 
                return m_ItemCtrl.ItemText.FontWeight == FontWeights.Bold;
            }
            set {
                if (value)
                {
                    m_ItemCtrl.ItemText.FontWeight = FontWeights.Bold;
                }
                else
                {
                    m_ItemCtrl.ItemText.FontWeight = FontWeights.Normal;
                }
            }
        }

        public bool UseDragDrop
        {
            get {
                return m_ItemCtrl.UseDragDrop;
            }
            set {
                m_ItemCtrl.UseDragDrop = value;
            }
        }

        public event DragCompletedEventHandler DragCompleted;
        public event DragDeltaEventHandler DragDelta;
        public event DragStartedEventHandler DragStarted;

        protected TreeItemControl m_ItemCtrl = null;

        public CustomTreeNode(bool useIcon)
        {
            m_ItemCtrl = new TreeItemControl(useIcon);
            m_ItemCtrl.DragStarted += new DragStartedEventHandler(m_ItemCtrl_DragStarted);
            m_ItemCtrl.DragDelta += new DragDeltaEventHandler(m_ItemCtrl_DragDelta);
            m_ItemCtrl.DragCompleted += new DragCompletedEventHandler(m_ItemCtrl_DragCompleted);
            m_ItemCtrl.MouseDoubleClick += new MouseDoubleClickEventHandler(m_ItemCtrl_MouseDoubleClick);
            Header = m_ItemCtrl;
        }

        public CustomTreeNode()
            : this(true)
        {
        }

        void m_ItemCtrl_MouseDoubleClick(object sender, EventArgs e)
        {
            Raise_MouseDoubleClick(e);
        }

        public event EventHandler<CustomEventArgs<CustomTreeNode>> MouseDoubleClick;
        void Raise_MouseDoubleClick(EventArgs e)
        {
            EventHandler<CustomEventArgs<CustomTreeNode>> handler = MouseDoubleClick;
            if (handler != null)
            {
                handler(this, new CustomEventArgs<CustomTreeNode>(this));
            }
        }

        void m_ItemCtrl_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            DragCompletedEventHandler handler = DragCompleted;
            if(handler != null)
            {
                handler(this, e);
            }
        }

        void m_ItemCtrl_DragDelta(object sender, DragDeltaEventArgs e)
        {
            DragDeltaEventHandler handler = DragDelta;
            if (handler != null)
            {
                handler(this, e);
            } 
        }

        void m_ItemCtrl_DragStarted(object sender, DragStartedEventArgs e)
        {
            DragStartedEventHandler handler = DragStarted;
            if (handler != null)
            {
                handler(this, e);
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
                if (value)
                {
                    if (nextItem != null)
                    {
                        LoadNextTreeNode next = nextItem as LoadNextTreeNode;
                        if (next != null)
                        {
                            next.MouseDoubleClick -= new EventHandler<CustomEventArgs<CustomTreeNode>>(SpecialNode_MouseDoubleClick);
                        }
                        Items.Remove(nextItem);
                    }
                }
                else
                {
                    if (nextItem == null)
                        AddSpecialNode(SpecialNodes.LoadNext);
                }
            }
            get
            {
                if (GetSpecialNode(SpecialNodes.LoadNext) == null)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// Проверяет загружались ли дочерние узлы. 
        /// </summary>
        public virtual bool IsInitialized
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

        protected TreeViewItem AddSpecialNode(SpecialNodes nodeType)
        {
            TreeViewItem node = null;
            switch (nodeType)
            { 
                case SpecialNodes.Wait:
                    node = new WaitTreeNode();
                    break;
                case SpecialNodes.Error:
                    node = new ErrorTreeNode();
                    break;
                case SpecialNodes.LoadNext:
                    LoadNextTreeNode new_node = new LoadNextTreeNode();
                    new_node.MouseDoubleClick += new EventHandler<CustomEventArgs<CustomTreeNode>>(SpecialNode_MouseDoubleClick);
                    node = new_node;
                    break;
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
            foreach (object obj in Items)
            {
                switch (nodeType)
                {
                    case SpecialNodes.Wait:
                        WaitTreeNode wait = obj as WaitTreeNode;
                        if (wait != null)
                            return wait;
                        break;
                    case SpecialNodes.Error:
                        ErrorTreeNode error = obj as ErrorTreeNode;
                        if (error != null)
                            return error;
                        break;
                    case SpecialNodes.LoadNext:
                        LoadNextTreeNode next = obj as LoadNextTreeNode;
                        if (next != null)
                            return next;
                        break;
                }
            }
            return null;
        }


    }
}