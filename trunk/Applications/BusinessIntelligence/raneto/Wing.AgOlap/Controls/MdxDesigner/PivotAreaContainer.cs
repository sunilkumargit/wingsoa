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
using System.Collections.Generic;
using Ranet.AgOlap.Controls.General.Tree;
using System.Windows.Media.Imaging;
using Ranet.AgOlap.Controls.General;
using System.Windows.Controls.Primitives;

namespace Ranet.AgOlap.Controls.MdxDesigner
{
    public class DragAreaItemArgs<T> : EventArgs
    {
        public readonly AreaItemControl Item;
        public readonly T Args;

        public DragAreaItemArgs(AreaItemControl item, T args)
        {
            Item = item;
            Args = args;
        }
    }

    public class PivotAreaContainer : UserControl
    {
        Grid LayoutRoot;
        HeaderControl m_Header;
        
        AreaItemsList m_ItemsList;

        public String Caption
        {
            get { return m_Header.Caption; }
            set { m_Header.Caption = value; }
        }

        public BitmapImage Icon
        {
            
            get { return m_Header.Icon; }
            set { m_Header.Icon = value; }
        }

        public PivotAreaContainer()
        {
            LayoutRoot = new Grid();
            LayoutRoot.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            LayoutRoot.RowDefinitions.Add(new RowDefinition());

            m_Header = new HeaderControl() { Margin = new Thickness(0, 0, 0, 3) };
            LayoutRoot.Children.Add(m_Header);

            m_ItemsList = new AreaItemsList();
            m_ItemsList.ItemRemoved += new EventHandler<AreaItemArgs>(m_ItemsList_ItemRemoved);
            m_ItemsList.BeforeShowContextMenu += new EventHandler<AreaItemArgs>(m_ItemsList_BeforeShowContextMenu);
            m_ItemsList.ItemsListChanged += new EventHandler(m_ItemsList_ItemsListChanged);
            LayoutRoot.Children.Add(m_ItemsList);
            Grid.SetRow(m_ItemsList, 1);

            this.Content = LayoutRoot;
        }

        void m_ItemsList_ItemsListChanged(object sender, EventArgs e)
        {
            Raise_ItemsListChanged();
        }

        void m_ItemsList_BeforeShowContextMenu(object sender, AreaItemArgs e)
        {
            Raise_BeforeShowContextMenu(e);
        }

        #region События
        public event EventHandler<AreaItemArgs> ItemRemoved;
        public event EventHandler ItemsListChanged;
        public event EventHandler<AreaItemArgs> BeforeShowContextMenu;

        void Raise_ItemsListChanged()
        {
            EventHandler handler = ItemsListChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        void Raise_BeforeShowContextMenu(AreaItemArgs args)
        {
            EventHandler<AreaItemArgs> handler = BeforeShowContextMenu;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        #endregion События

        void m_ItemsList_ItemRemoved(object sender, AreaItemArgs e)
        {
            e.ItemControl.DragStarted -= new DragStartedEventHandler(ctrl_DragStarted);
            e.ItemControl.DragDelta -= new DragDeltaEventHandler(ctrl_DragDelta);
            e.ItemControl.DragCompleted -= new DragCompletedEventHandler(ctrl_DragCompleted);

            EventHandler<AreaItemArgs> handler = ItemRemoved;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void AddItem(AreaItemControl ctrl)
        {
            AddItem(ctrl, true);
        }

        public void AddItem(AreaItemControl ctrl, bool raise_ItemsListChanged)
        {
            if (ctrl != null)
            {
                ctrl.Area = this;
                ctrl.DragStarted += new DragStartedEventHandler(ctrl_DragStarted);
                ctrl.DragDelta += new DragDeltaEventHandler(ctrl_DragDelta);
                ctrl.DragCompleted += new DragCompletedEventHandler(ctrl_DragCompleted);
                m_ItemsList.AddItem(ctrl, raise_ItemsListChanged);
            }
        }

        void ctrl_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            EventHandler<DragAreaItemArgs<DragCompletedEventArgs>> handler = DragCompleted;
            if (handler != null)
            {
                handler(this, new DragAreaItemArgs<DragCompletedEventArgs>(sender as AreaItemControl, e));
            }
        }

        void ctrl_DragDelta(object sender, DragDeltaEventArgs e)
        {
            EventHandler<DragAreaItemArgs<DragDeltaEventArgs>> handler = DragDelta;
            if (handler != null)
            {
                handler(this, new DragAreaItemArgs<DragDeltaEventArgs>(sender as AreaItemControl, e));
            }
        }

        void ctrl_DragStarted(object sender, DragStartedEventArgs e)
        {
            EventHandler<DragAreaItemArgs<DragStartedEventArgs>> handler = DragStarted;
            if (handler != null)
            {
                handler(this, new DragAreaItemArgs<DragStartedEventArgs>(sender as AreaItemControl, e));
            }
        }

        public void RemoveItem(AreaItemControl ctrl, bool raise_ListChanged)
        {
            if (ctrl != null)
            {
                ctrl.Area = null;
            }
            m_ItemsList.RemoveItem(ctrl, raise_ListChanged);
        }

        public void RemoveItem(AreaItemControl ctrl)
        {
            if (ctrl != null)
            {
                ctrl.Area = null;
            }
            m_ItemsList.RemoveItem(ctrl);
        }

        public void Clear()
        {
            foreach (AreaItemControl ctrl in m_ItemsList.Items)
                ctrl.Area = null;

            m_ItemsList.Clear();
        }

        public List<AreaItemControl> Items
        {
            get {
                return m_ItemsList.Items;
            }
        }

        bool m_IsReadyToDrop = false;
        public bool IsReadyToDrop
        {
            get { return m_IsReadyToDrop; }
            set
            {
                if (m_IsReadyToDrop != value)
                {
                    m_IsReadyToDrop = value;
                    if (value)
                    {
                        m_ItemsList.Effect = new System.Windows.Media.Effects.DropShadowEffect() { ShadowDepth = 1, Opacity = 0.8, Color = Colors.Blue };
                    }
                    else
                    {
                        m_ItemsList.Effect = null;
                    }
                }
            }
        }

        // Summary:
        //     Occurs when the System.Windows.Controls.Primitives.Thumb control loses mouse
        //     capture.
        public event EventHandler<DragAreaItemArgs<DragCompletedEventArgs>> DragCompleted;
        //
        // Summary:
        //     Occurs one or more times as the mouse pointer is moved when a System.Windows.Controls.Primitives.Thumb
        //     control has logical focus and mouse capture.
        public event EventHandler<DragAreaItemArgs<DragDeltaEventArgs>> DragDelta;
        //
        // Summary:
        //     Occurs when a System.Windows.Controls.Primitives.Thumb control receives logical
        //     focus and mouse capture.
        public event EventHandler<DragAreaItemArgs<DragStartedEventArgs>> DragStarted;
    }
}
