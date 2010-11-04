namespace Telerik.Windows.Controls
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Controls.Primitives;

    internal class DragInfo
    {
        public DragInfo(Point startPoint, Thumb draggedElement, RadWindow window)
        {
            this.MousePosition = this.StartPoint = startPoint;
            this.DraggedElement = draggedElement;
            this.DraggedThumbType = RadWindow.GetThumbType(draggedElement);
            if (this.DraggedThumbType != ThumbType.NONE)
            {
                this.InitialRectangle = new Rect(window.Left, window.Top, double.IsNaN(window.Width) ? window.ActualWidth : window.Width, double.IsNaN(window.Height) ? window.ActualHeight : window.Height);
            }
        }

        public void DoDrag(double horizontalChange, double verticalChange, RadWindow window)
        {
            horizontalChange *= ApplicationHelper.ZoomFactor;
            verticalChange *= ApplicationHelper.ZoomFactor;
            this.UpdateMousePosition(horizontalChange, verticalChange);
            Rect newLocation = Rect.Empty;
            Point mouseOffset = new Point(Math.Round((double) (this.MousePosition.X - this.StartPoint.X)), Math.Round((double) (this.MousePosition.Y - this.StartPoint.Y)));
            switch (this.DraggedThumbType)
            {
                case ThumbType.ElementWestThumb:
                    newLocation = window.CoerceLocation(this.InitialRectangle.X + mouseOffset.X, this.InitialRectangle.Y, this.InitialRectangle.Width - mouseOffset.X, this.InitialRectangle.Height, this.InitialRectangle);
                    break;

                case ThumbType.ElementNorthWestThumb:
                    newLocation = window.CoerceLocation(this.InitialRectangle.X + mouseOffset.X, this.InitialRectangle.Y + mouseOffset.Y, this.InitialRectangle.Width - mouseOffset.X, this.InitialRectangle.Height - mouseOffset.Y, this.InitialRectangle);
                    break;

                case ThumbType.ElementNorthThumb:
                    newLocation = window.CoerceLocation(this.InitialRectangle.X, this.InitialRectangle.Y + mouseOffset.Y, this.InitialRectangle.Width, this.InitialRectangle.Height - mouseOffset.Y, this.InitialRectangle);
                    break;

                case ThumbType.ElementNorthEastThumb:
                    newLocation = window.CoerceLocation(this.InitialRectangle.X, this.InitialRectangle.Y + mouseOffset.Y, this.InitialRectangle.Width + mouseOffset.X, this.InitialRectangle.Height - mouseOffset.Y, this.InitialRectangle);
                    break;

                case ThumbType.ElementEastThumb:
                    newLocation = window.CoerceLocation(this.InitialRectangle.X, this.InitialRectangle.Y, this.InitialRectangle.Width + mouseOffset.X, this.InitialRectangle.Height, this.InitialRectangle);
                    break;

                case ThumbType.ElementSouthEastThumb:
                    newLocation = window.CoerceLocation(this.InitialRectangle.X, this.InitialRectangle.Y, this.InitialRectangle.Width + mouseOffset.X, this.InitialRectangle.Height + mouseOffset.Y, this.InitialRectangle);
                    break;

                case ThumbType.ElementSouthThumb:
                    newLocation = window.CoerceLocation(this.InitialRectangle.X, this.InitialRectangle.Y, this.InitialRectangle.Width, this.InitialRectangle.Height + mouseOffset.Y, this.InitialRectangle);
                    break;

                case ThumbType.ElementSouthWestThumb:
                    newLocation = window.CoerceLocation(this.InitialRectangle.X + mouseOffset.X, this.InitialRectangle.Y, this.InitialRectangle.Width - mouseOffset.X, this.InitialRectangle.Height + mouseOffset.Y, this.InitialRectangle);
                    break;

                case ThumbType.TitleThumb:
                    newLocation = window.CoerceLocation(this.InitialRectangle.X + mouseOffset.X, this.InitialRectangle.Y + mouseOffset.Y, this.InitialRectangle.Width, this.InitialRectangle.Height, this.InitialRectangle);
                    break;
            }
            if (newLocation != Rect.Empty)
            {
                SetWindowSizeAndLocation(window, newLocation);
            }
        }

        public void RestoreWindowSizeAndLocation(RadWindow window)
        {
            SetWindowSizeAndLocation(window, this.InitialRectangle);
        }

        public static void SetWindowSizeAndLocation(RadWindow window, Rect rectangle)
        {
            window.Left = rectangle.Left;
            window.Top = rectangle.Top;
            if (window.IsResizing)
            {
                window.Width = rectangle.Width;
                window.Height = rectangle.Height;
            }
            window.UpdateLayout();
        }

        public void UpdateMousePosition(double horizontalChange, double verticalChange)
        {
            this.MousePosition = new Point(Math.Round((double) (this.MousePosition.X + horizontalChange)), Math.Round((double) (this.MousePosition.Y + verticalChange)));
        }

        public Thumb DraggedElement { get; private set; }

        public ThumbType DraggedThumbType { get; private set; }

        public Rect InitialRectangle { get; private set; }

        public Point MousePosition { get; private set; }

        public Point StartPoint { get; private set; }
    }
}

