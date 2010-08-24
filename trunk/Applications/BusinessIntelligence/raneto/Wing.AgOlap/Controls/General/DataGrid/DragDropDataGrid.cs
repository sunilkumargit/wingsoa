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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Ranet.AgOlap.Controls.Data;
using Ranet.AgOlap.Controls.MdxDesigner;
using Ranet.Olap.Core.Providers;
using Ranet.AgOlap.Controls.General.Tree;

namespace Ranet.AgOlap.Controls.General.DataGrid
{
    public class DragGridItemArgs<T> : EventArgs
    {
        public readonly DragDropDataGrid Item;
        public readonly T Args;

        public DragGridItemArgs(DragDropDataGrid item, T args)
        {
            Item = item;
            Args = args;
        }
    }

    public class DragDropDataGrid : DragDropControl
    {
        private Grid LayoutRoot;
        private RanetDataGrid m_Grid = new RanetDataGrid();
        public DragDropDataGrid()
        {
            LayoutRoot = new Grid();
            LayoutRoot.Children.Add(m_Grid);
            this.Content = LayoutRoot;
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
                        this.Effect = new System.Windows.Media.Effects.DropShadowEffect() { ShadowDepth = 1, Opacity = 0.8, Color = Colors.Blue };
                    }
                    else
                    {
                        this.Effect = null;
                    }
                }
            }
        }     

        public RanetDataGrid Grid
        {
            get { return this.m_Grid; }
        }
               
    }

}
