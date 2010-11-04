namespace Telerik.Windows.Controls.Docking
{
    using System;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;

    internal class DragValidator
    {
        private bool draggingActive;
        private Point start;
        private UIElement targetElement;

        public event EventHandler<DragCompletedEventArgs> DragCompletedEvent;

        public event EventHandler<DragDeltaEventArgs> DragDeltaEvent;

        public event EventHandler<DragStartedEventArgs> DragStartedEvent;

        public DragValidator(UIElement targetElement)
        {
            this.targetElement = targetElement;
            this.targetElement.MouseLeftButtonDown += new MouseButtonEventHandler(this.TargetElement_MouseLeftButtonDown);
            this.targetElement.MouseLeftButtonUp += new MouseButtonEventHandler(this.TargetElement_MouseLeftButtonUp);
            this.targetElement.LostMouseCapture += new MouseEventHandler(this.TargetElement_LostMouseCapture);
            this.targetElement.MouseMove += new MouseEventHandler(this.TargetElement_MouseMove);
        }

        private void OnDragCompleted(MouseEventArgs e, bool canceled)
        {
            EventHandler<DragCompletedEventArgs> dragCompletedEvent = this.DragCompletedEvent;
            if (dragCompletedEvent != null)
            {
                Point point;
                if (canceled)
                {
                    point = new Point();
                }
                else
                {
                    Point position = e.GetPosition(null);
                    point = new Point(position.X - this.start.X, position.Y - this.start.Y);
                }
                dragCompletedEvent(this.targetElement, new DragCompletedEventArgs(point.X, point.Y, canceled));
            }
        }

        private void OnDragDelta(MouseEventArgs e)
        {
            EventHandler<DragDeltaEventArgs> dragDeltaEvent = this.DragDeltaEvent;
            if (dragDeltaEvent != null)
            {
                Point position = e.GetPosition(null);
                dragDeltaEvent(this.targetElement, new DragDeltaEventArgs(position.X - this.start.X, position.Y - this.start.Y));
            }
        }

        private void OnDragStarted()
        {
            EventHandler<DragStartedEventArgs> dragStartedEvent = this.DragStartedEvent;
            if (dragStartedEvent != null)
            {
                dragStartedEvent(this.targetElement, new DragStartedEventArgs(0.0, 0.0));
            }
        }

        private void TargetElement_LostMouseCapture(object sender, MouseEventArgs e)
        {
            if (this.draggingActive)
            {
                this.draggingActive = false;
                this.OnDragCompleted(e, true);
            }
        }

        private void TargetElement_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.start = e.GetPosition(null);
            this.draggingActive = this.targetElement.CaptureMouse();
            if (this.draggingActive)
            {
                this.OnDragStarted();
            }
        }

        private void TargetElement_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.draggingActive)
            {
                this.draggingActive = false;
                this.targetElement.ReleaseMouseCapture();
                this.OnDragCompleted(e, false);
            }
        }

        private void TargetElement_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.draggingActive)
            {
                this.OnDragDelta(e);
            }
        }
    }
}

