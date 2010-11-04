namespace Telerik.Windows.Controls.Docking
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using Telerik.Windows.Controls;

    [TemplatePart(Name="HorizontalTemplate", Type=typeof(FrameworkElement)), TemplatePart(Name="VerticalTemplate", Type=typeof(FrameworkElement))]
    internal class PreviewControl : Control
    {
        private Point gridSplitterOrigin = new Point();
        private FrameworkElement horizontalElement;
        private Dock resizeDirection;
        private FrameworkElement verticalElement;

        public void Bind(RadGridResizer gridResizer)
        {
            UIElement parent = gridResizer.Parent as UIElement;
            if (parent != null)
            {
                base.Style = gridResizer.PreviewStyle;
                Matrix matrix = ((MatrixTransform) gridResizer.TransformToVisual(parent)).Matrix;
                this.gridSplitterOrigin.X = matrix.OffsetX;
                this.gridSplitterOrigin.Y = matrix.OffsetY;
                if (gridResizer.Data != null)
                {
                    this.resizeDirection = gridResizer.Data.ResizePlacement;
                }
                if ((this.resizeDirection == Dock.Left) || (this.resizeDirection == Dock.Right))
                {
                    base.Height = parent.RenderSize.Height;
                    base.Width = gridResizer.Width;
                    base.SetValue(Canvas.LeftProperty, this.gridSplitterOrigin.X);
                }
                else
                {
                    base.Width = parent.RenderSize.Width;
                    base.Height = gridResizer.Height;
                    base.SetValue(Canvas.TopProperty, this.gridSplitterOrigin.Y);
                }
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.horizontalElement = base.GetTemplateChild("HorizontalTemplate") as FrameworkElement;
            this.verticalElement = base.GetTemplateChild("VerticalTemplate") as FrameworkElement;
            if ((this.resizeDirection == Dock.Left) || (this.resizeDirection == Dock.Right))
            {
                if (this.horizontalElement != null)
                {
                    this.horizontalElement.Visibility = Visibility.Collapsed;
                }
                if (this.verticalElement != null)
                {
                    this.verticalElement.Visibility = Visibility.Visible;
                }
            }
            else
            {
                if (this.horizontalElement != null)
                {
                    this.horizontalElement.Visibility = Visibility.Visible;
                }
                if (this.verticalElement != null)
                {
                    this.verticalElement.Visibility = Visibility.Collapsed;
                }
            }
        }

        public double OffsetX
        {
            get
            {
                return (((double) base.GetValue(Canvas.LeftProperty)) - this.gridSplitterOrigin.X);
            }
            set
            {
                base.SetValue(Canvas.LeftProperty, this.gridSplitterOrigin.X + value);
            }
        }

        public double OffsetY
        {
            get
            {
                return (((double) base.GetValue(Canvas.TopProperty)) - this.gridSplitterOrigin.Y);
            }
            set
            {
                base.SetValue(Canvas.TopProperty, this.gridSplitterOrigin.Y + value);
            }
        }
    }
}

