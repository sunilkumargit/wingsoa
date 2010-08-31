/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Windows.Controls;
using System.Windows.Media;
using Wing.Olap.Controls.Data;
using Wing.Olap.Controls.General.Tree;

namespace Wing.Olap.Controls.General.DataGrid
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
