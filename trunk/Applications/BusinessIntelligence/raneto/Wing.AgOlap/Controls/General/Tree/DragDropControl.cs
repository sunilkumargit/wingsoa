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
using System.Windows.Controls.Primitives;

namespace Ranet.AgOlap.Controls.General.Tree
{
    public class DragDropControl : UserControl
    {
        // Fields
        internal Point _origin;
        internal Point _previousPosition;

        // Events
        public event DragCompletedEventHandler DragCompleted;
        public event DragDeltaEventHandler DragDelta;
        public event DragStartedEventHandler DragStarted;

        public DragDropControl() 
            : this(false)
        {
        }

        // Methods
        public DragDropControl(bool useDragDrop)
        {
            UseDragDrop = useDragDrop;
        }

        bool m_UseDragDrop = false;
        public bool UseDragDrop
        {
            get { return m_UseDragDrop; }
            set { 
                m_UseDragDrop = value;
                if (m_UseDragDrop)
                {
                    base.LostMouseCapture += new MouseEventHandler(this.OnLostMouseCapture);
                    base.MouseEnter += new MouseEventHandler(OnMouseEnter);
                    base.MouseLeave += new MouseEventHandler(OnMouseLeave);
                    base.MouseLeftButtonDown += new MouseButtonEventHandler(OnMouseLeftButtonDown);
                    //base.MouseLeftButtonUp += new MouseButtonEventHandler(OnMouseLeftButtonUp);
                    base.MouseMove += new MouseEventHandler(OnMouseMove);
                }
                else
                {
                    base.LostMouseCapture -= new MouseEventHandler(this.OnLostMouseCapture);
                    base.MouseEnter -= new MouseEventHandler(OnMouseEnter);
                    base.MouseLeave -= new MouseEventHandler(OnMouseLeave);
                    base.MouseLeftButtonDown -= new MouseButtonEventHandler(OnMouseLeftButtonDown);
                    //base.MouseLeftButtonUp -= new MouseButtonEventHandler(OnMouseLeftButtonUp);
                    base.MouseMove -= new MouseEventHandler(OnMouseMove);
                }
            }
        }

        public void CancelDrag()
        {
            if (this.IsDragging)
            {
                this.IsDragging = false;
                this.RaiseDragCompleted(true);
            }
        }

        private void OnLostMouseCapture(object sender, MouseEventArgs e)
        {
            if (((sender == this) && this.IsDragging) && base.IsEnabled)
            {
                this.IsDragging = false;
                base.ReleaseMouseCapture();
                this.RaiseDragCompleted(false);
            }
        }

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            if (base.IsEnabled)
            {
                this.IsMouseOver = true;
            }
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            if (base.IsEnabled)
            {
                this.IsMouseOver = false;
            }
        }

        //private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        //{
        //    base.OnMouseLeftButtonUp(e);
        //    CancelDrag();
        //}

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (!e.Handled && (!this.IsDragging && base.IsEnabled))
            {
                //e.Handled = true;
                base.CaptureMouse();
                this.IsDragging = true;
                this._origin = this._previousPosition = e.GetPosition(null); //(UIElement)base.Parent
                bool flag = false;
                try
                {
                    DragStartedEventHandler dragStarted = this.DragStarted;
                    if (dragStarted != null)
                    {
                        dragStarted(this, new DragStartedEventArgs(this._origin.X, this._origin.Y));
                    }
                    flag = true;
                }
                finally
                {
                    if (!flag)
                    {
                        this.CancelDrag();
                    }
                }
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (this.IsDragging)
            {
                Cursor = Cursors.Hand;
                Point position = e.GetPosition(null); //(UIElement)base.Parent
                if (position != this._previousPosition)
                {
                    DragDeltaEventHandler dragDelta = this.DragDelta;
                    if (dragDelta != null)
                    {
                        dragDelta(this, new DragDeltaEventArgs(position.X - this._previousPosition.X, position.Y - this._previousPosition.Y));
                    }
                    this._previousPosition = position;
                }
            }
        }

        private void RaiseDragCompleted(bool canceled)
        {
            DragCompletedEventHandler dragCompleted = this.DragCompleted;
            if (dragCompleted != null)
            {
                DragCompletedEventArgs e = new DragCompletedEventArgs(this._previousPosition.X - this._origin.X, this._previousPosition.Y - this._origin.Y, canceled);
                dragCompleted(this, e);
            }
        }

        // Properties
        bool m_IsDragging = false;
        public bool IsDragging
        {
            get { return m_IsDragging; }
            set {
                if (!value)
                {
                    Cursor = Cursors.Arrow;
                }
                m_IsDragging = value;
            }
        }
        internal bool IsMouseOver = false;
    }
}
