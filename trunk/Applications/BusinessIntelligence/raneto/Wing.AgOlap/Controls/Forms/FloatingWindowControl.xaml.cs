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
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Ranet.AgOlap.Controls.Forms
{
    public partial class FloatingWindowControl : UserControl
    {
        bool IsDrag;
        Point StartingDragPoint;
        Storyboard resDoubleClickTimer;

        public FloatingWindowControl()
        {
            InitializeComponent();
            MaximizeButton.btnMinMaxButton.Checked += new RoutedEventHandler(btnMinMaxButton_Checked);
            MaximizeButton.btnMinMaxButton.Unchecked += new RoutedEventHandler(btnMinMaxButton_Unchecked);
            this.Loaded += new RoutedEventHandler(FloatingWindowControl_Loaded);

            resDoubleClickTimer = new Storyboard();
            resDoubleClickTimer.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 250));
            resDoubleClickTimer.Completed += new EventHandler(Storyboard_Completed);
            LayoutRoot.Resources.Add("resDoubleClickTimer", resDoubleClickTimer);
        }

        void btnMinMaxButton_Unchecked(object sender, RoutedEventArgs e)
        {
            OnMaximize(); 
        }

        void btnMinMaxButton_Checked(object sender, RoutedEventArgs e)
        {
            OnMaximize(); 
        }

        int m_ClickCount = 0;
        MouseButtonEventArgs m_LastArgs;

        private void Storyboard_Completed(object sender, EventArgs e)
        {
            if (m_ClickCount > 1)
            {
                IsMaximized = !IsMaximized;
            }
            m_ClickCount = 0;
        }

        void FloatingWindowControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.SetValue(WidthProperty, this.Width);
            this.SetValue(HeightProperty, this.Height);
        }

        #region LeftSide
        private void LeftSide_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            //Start Drag
            FrameworkElement LeftSide = (FrameworkElement)sender;
            LeftSide.CaptureMouse();

            // Set the starting point for the drag
            StartingDragPoint = e.GetPosition(LeftSide);
            IsDrag = true;

            LeftSide.MouseMove += new MouseEventHandler(LeftSide_MouseMove);
            LeftSide.MouseLeftButtonUp += new MouseButtonEventHandler(LeftSide_MouseLeftButtonUp);
        }

        void LeftSide_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            IsDrag = false;
            //Stop Drag
            FrameworkElement LeftSide = (FrameworkElement)sender;
            LeftSide.ReleaseMouseCapture();

            LeftSide.MouseMove -= new MouseEventHandler(LeftSide_MouseMove);
            LeftSide.MouseLeftButtonUp -= new MouseButtonEventHandler(LeftSide_MouseLeftButtonUp);
        }

        void LeftSide_MouseMove(object sender, MouseEventArgs e)
        {
            if (!IsDrag)
                return;
            Point LeftSidePoint = e.GetPosition(LeftSide);
            double PointDifference = LeftSidePoint.X - StartingDragPoint.X;

            if (PointDifference < 0)
            {
                PointDifference = PointDifference * -1;

                this.SetValue(WidthProperty, (double)(this.ActualWidth + PointDifference));
                Canvas.SetLeft(this, Canvas.GetLeft(this) - PointDifference);
            }
            else
            {
                if (LeftSidePoint.X < (this.ActualWidth - (PointDifference) - this.MinWidth))
                {
                    this.SetValue(WidthProperty, (double)(this.ActualWidth - PointDifference));
                    Canvas.SetLeft(this, Canvas.GetLeft(this) + PointDifference);
                }
            }
            this.UpdateLayout();
        }
        #endregion

        #region RightSide
        private void RightSide_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            //Start Drag
            FrameworkElement RightSide = (FrameworkElement)sender;
            RightSide.CaptureMouse();

            // Set the starting point for the drag
            StartingDragPoint = e.GetPosition(RightSide);
            IsDrag = true;

            RightSide.MouseMove += new MouseEventHandler(RightSide_MouseMove);
            RightSide.MouseLeftButtonUp += new MouseButtonEventHandler(RightSide_MouseLeftButtonUp);
        }

        void RightSide_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            IsDrag = false;
            //Stop Drag
            FrameworkElement RightSide = (FrameworkElement)sender;
            RightSide.ReleaseMouseCapture();

            RightSide.MouseMove -= new MouseEventHandler(RightSide_MouseMove);
            RightSide.MouseLeftButtonUp -= new MouseButtonEventHandler(RightSide_MouseLeftButtonUp);
        }

        void RightSide_MouseMove(object sender, MouseEventArgs e)
        {
            if (!IsDrag)
                return;

            Point Point = e.GetPosition(this);

            if (Point.X > (StartingDragPoint.X + this.MinWidth))
            {
                this.SetValue(WidthProperty, (double)(Point.X - (StartingDragPoint.X)));
            }
            this.UpdateLayout();
        }
        #endregion

        #region BottomSide
        private void BottomSide_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            //Start Drag
            FrameworkElement BottomSide = (FrameworkElement)sender;
            BottomSide.CaptureMouse();

            // Set the starting point for the drag
            StartingDragPoint = e.GetPosition(BottomSide);
            IsDrag = true;

            BottomSide.MouseMove += new MouseEventHandler(BottomSide_MouseMove);
            BottomSide.MouseLeftButtonUp += new MouseButtonEventHandler(BottomSide_MouseLeftButtonUp);
        }

        void BottomSide_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //Stop Drag
            FrameworkElement BottomSide = (FrameworkElement)sender;
            BottomSide.ReleaseMouseCapture();

            BottomSide.MouseMove -= new MouseEventHandler(BottomSide_MouseMove);
            BottomSide.MouseLeftButtonUp -= new MouseButtonEventHandler(BottomSide_MouseLeftButtonUp);
        }

        void BottomSide_MouseMove(object sender, MouseEventArgs e)
        {
            if (!IsDrag)
                return;

            Point Point = e.GetPosition(this);

            if (Point.Y > (StartingDragPoint.Y + this.MinHeight))
            {
                this.SetValue(HeightProperty, (double)(Point.Y - (StartingDragPoint.Y)));
            }
            this.UpdateLayout();
        }
        #endregion

        #region TopSide
        private void TopSide_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            //Start Drag
            FrameworkElement TopSide = (FrameworkElement)sender;
            TopSide.CaptureMouse();

            // Set the starting point for the drag
            StartingDragPoint = e.GetPosition(TopSide);
            IsDrag = true;

            TopSide.MouseMove += new MouseEventHandler(TopSide_MouseMove);
            TopSide.MouseLeftButtonUp += new MouseButtonEventHandler(TopSide_MouseLeftButtonUp);
        }

        void TopSide_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //Stop Drag
            FrameworkElement TopSide = (FrameworkElement)sender;
            TopSide.ReleaseMouseCapture();

            TopSide.MouseMove -= new MouseEventHandler(TopSide_MouseMove);
            TopSide.MouseLeftButtonUp -= new MouseButtonEventHandler(TopSide_MouseLeftButtonUp);
        }

        void TopSide_MouseMove(object sender, MouseEventArgs e)
        {
            if (!IsDrag)
                return;

            Point TopSidePoint = e.GetPosition(TopSide);
            double PointDifference = TopSidePoint.Y - StartingDragPoint.Y;

            if (PointDifference < 0)
            {
                PointDifference = PointDifference * -1;

                this.SetValue(HeightProperty, (double)(this.ActualHeight + PointDifference));
                Canvas.SetTop(this, Canvas.GetTop(this) - PointDifference);
            }
            else
            {
                if (TopSidePoint.Y < (this.ActualHeight - PointDifference - this.MinHeight))
                {
                    this.SetValue(HeightProperty, (double)(this.ActualHeight - PointDifference));
                    Canvas.SetTop(this, Canvas.GetTop(this) + PointDifference);
                }
            }
            this.UpdateLayout();

        }
        #endregion

        #region DragBar
        private void DragBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            m_ClickCount++;
            m_LastArgs = e;
            resDoubleClickTimer.Begin();

            MoveToTop();

            //Start Drag
            FrameworkElement DragBar = (FrameworkElement)sender;
            DragBar.CaptureMouse();

            // Set the starting point for the drag
            try
            {
                StartingDragPoint = e.GetPosition(DragBar);
                IsDrag = true;

                DragBar.MouseMove += new MouseEventHandler(DragBar_MouseMove);
                DragBar.MouseLeftButtonUp += new MouseButtonEventHandler(DragBar_MouseLeftButtonUp);
            }catch
            {
                IsDrag = false;
            }
        }

        private void MoveToTop()
        {
            Canvas objCanvas = (Canvas)this.Parent;

            //Find the highest elemnt
            int intHighestElement = 0;
            foreach (UIElement UIElement in objCanvas.Children)
            {
                int intTmpHighestElement = Canvas.GetZIndex(UIElement);
                intHighestElement = (intTmpHighestElement > intHighestElement) ? intTmpHighestElement : intHighestElement;
            }

            //Canvas.SetZIndex(this, intHighestElement + 1);
            Canvas.SetZIndex(this, intHighestElement + 1);
        }

        private void DragBar_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            IsDrag = false;

            //Stop Drag
            FrameworkElement DragBar = (FrameworkElement)sender;
            DragBar.ReleaseMouseCapture();

            DragBar.MouseMove -= new MouseEventHandler(DragBar_MouseMove);
            DragBar.MouseLeftButtonUp -= new MouseButtonEventHandler(DragBar_MouseLeftButtonUp);

        }

        private void DragBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (!IsDrag)
                return;

            Canvas Canvas = (Canvas)this.Parent;
            Point Point = e.GetPosition(Canvas);

            // Do not allow drag past the top or the left of the screen
            if (Point.Y < 0)
            {
                Point.Y = 0;
                return;
            }

            if (Point.X < 0)
            {
                Point.X = 0;
                return;
            }

            Canvas.SetLeft(this, Point.X - StartingDragPoint.X);
            Canvas.SetTop(this, Point.Y - StartingDragPoint.Y);
        }
        #endregion

        public event EventHandler Close;
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            EventHandler handler = Close;
            if (Close != null)
            {
                Close(this, EventArgs.Empty);
            }
        }

        private void DiagonalSide_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            //Start Drag
            FrameworkElement DiagonalSide = (FrameworkElement)sender;
            DiagonalSide.CaptureMouse();

            // Set the starting point for the drag
            StartingDragPoint = e.GetPosition(DiagonalSide);
            IsDrag = true;

            DiagonalSide.MouseMove += new MouseEventHandler(DiagonalSide_MouseMove);
            DiagonalSide.MouseLeftButtonUp += new MouseButtonEventHandler(DiagonalSide_MouseLeftButtonUp);
        }

        void DiagonalSide_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            IsDrag = false;
            //Stop Drag
            FrameworkElement DiagonalSide = (FrameworkElement)sender;
            DiagonalSide.ReleaseMouseCapture();

            DiagonalSide.MouseMove -= new MouseEventHandler(DiagonalSide_MouseMove);
            DiagonalSide.MouseLeftButtonUp -= new MouseButtonEventHandler(DiagonalSide_MouseLeftButtonUp);
        }

        void DiagonalSide_MouseMove(object sender, MouseEventArgs e)
        {
            if (!IsDrag)
                return;

            Point Point = e.GetPosition(this);

            if (Point.X > (StartingDragPoint.X + this.MinWidth))
            {
                this.SetValue(WidthProperty, (double)(Point.X - (StartingDragPoint.X)));
            }

            if (Point.Y > (StartingDragPoint.Y + this.MinHeight))
            {
                this.SetValue(HeightProperty, (double)(Point.Y - (StartingDragPoint.Y)));
            }
            this.UpdateLayout();
        }

        void OnMaximize()
        {
            if (IsMaximized == false)
            {
                // Разворачивание на весь экран
                if (Application.Current != null && Application.Current.Host != null &&
                    Application.Current.Host.Content != null)
                {
                    // Запоминаем старое положение и размеры    
                    m_Minimized_Size = new Size(this.ActualWidth, this.ActualHeight);
                    m_Minimized_Location = new Point(Canvas.GetLeft(this), Canvas.GetTop(this));

                    this.SetValue(WidthProperty, Application.Current.Host.Content.ActualWidth);
                    this.SetValue(HeightProperty, Application.Current.Host.Content.ActualHeight);

                    Canvas.SetTop(this, 0);
                    Canvas.SetLeft(this, 0);
                }
            }
            else
            { 
                // Свернуть до старых размеров
                this.SetValue(WidthProperty, m_Minimized_Size.Width);
                this.SetValue(HeightProperty, m_Minimized_Size.Height);

                Canvas.SetTop(this, m_Minimized_Location.Y);
                Canvas.SetLeft(this, m_Minimized_Location.X);
            }

            // Change Visual State MaximizeButton using event
            this.SizeChanged += new SizeChangedEventHandler(FloatingWindowControl_SizeChanged);
        }

        void FloatingWindowControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.SizeChanged -= new SizeChangedEventHandler(FloatingWindowControl_SizeChanged);
            VisualStateManager.GoToState(MaximizeButton.btnMinMaxButton, "Normal", true);
        }

        Point m_Minimized_Location = new Point(0, 0);
        Size m_Minimized_Size = new Size(400, 300);

        public bool IsMaximized
        {
            get { return MaximizeButton.IsMaximized; }
            set {
                if (MaximizeButton.IsMaximized != value)
                {
                    MaximizeButton.IsMaximized = value;
                }
            }
        }

    }
}
