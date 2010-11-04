namespace Telerik.Windows.Controls.Primitives
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    [TemplatePart(Name="RootVisual", Type=typeof(Grid)), TemplatePart(Name="ContentPresenter", Type=typeof(ContentPresenter))]
    public class LayoutTransformControl : ContentControl
    {
        private Size _childActualSize = Size.Empty;
        private ContentPresenter _contentPresenter;
        private MatrixTransform _matrixTransform;
        private Matrix _transformation;
        private Panel _transformationPanel;
        private const double AcceptableDelta = 0.0001;
        private const string ContentPresenterTemplatePartName = "ContentPresenter";
        public static readonly DependencyProperty LayoutTransformProperty = DependencyProperty.Register("LayoutTransform", typeof(Transform), typeof(LayoutTransformControl), new PropertyMetadata(new PropertyChangedCallback(LayoutTransformControl.LayoutTransformChanged)));
        private const string RootVisualTemplatePartName = "RootVisual";
        private const int RoundPrecision = 4;

        public LayoutTransformControl()
        {
            base.DefaultStyleKey = typeof(LayoutTransformControl);
            base.IsTabStop = false;
            base.UseLayoutRounding = false;
        }

        public void ApplyLayoutTransform()
        {
            this.ProcessTransform(this.LayoutTransform);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            FrameworkElement child = this.Child;
            if ((this._transformationPanel != null) && (child != null))
            {
                Size finalSizeTransformed = this.ComputeLargestTransformedSize(finalSize);
                if (IsSizeSmaller(finalSizeTransformed, this._transformationPanel.DesiredSize))
                {
                    finalSizeTransformed = this._transformationPanel.DesiredSize;
                }
                Rect transformedRect = RectTransform(new Rect(0.0, 0.0, finalSizeTransformed.Width, finalSizeTransformed.Height), this._transformation);
                Rect finalRect = new Rect(-transformedRect.Left + ((finalSize.Width - transformedRect.Width) / 2.0), -transformedRect.Top + ((finalSize.Height - transformedRect.Height) / 2.0), finalSizeTransformed.Width, finalSizeTransformed.Height);
                this._transformationPanel.Arrange(finalRect);
                if (IsSizeSmaller(finalSizeTransformed, child.RenderSize) && (Size.Empty == this._childActualSize))
                {
                    this._childActualSize = new Size(child.ActualWidth, child.ActualHeight);
                    base.InvalidateMeasure();
                    return finalSize;
                }
                this._childActualSize = Size.Empty;
            }
            return finalSize;
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification="Closely corresponds to WPF's FrameworkElement.FindMaximalAreaLocalSpaceRect.")]
        private Size ComputeLargestTransformedSize(Size arrangeBounds)
        {
            Size computedSize = Size.Empty;
            bool infiniteWidth = double.IsInfinity(arrangeBounds.Width);
            if (infiniteWidth)
            {
                arrangeBounds.Width = arrangeBounds.Height;
            }
            bool infiniteHeight = double.IsInfinity(arrangeBounds.Height);
            if (infiniteHeight)
            {
                arrangeBounds.Height = arrangeBounds.Width;
            }
            double a = this._transformation.M11;
            double b = this._transformation.M12;
            double c = this._transformation.M21;
            double d = this._transformation.M22;
            double maxWidthFromWidth = Math.Abs((double) (arrangeBounds.Width / a));
            double maxHeightFromWidth = Math.Abs((double) (arrangeBounds.Width / c));
            double maxWidthFromHeight = Math.Abs((double) (arrangeBounds.Height / b));
            double maxHeightFromHeight = Math.Abs((double) (arrangeBounds.Height / d));
            double idealWidthFromWidth = maxWidthFromWidth / 2.0;
            double idealHeightFromWidth = maxHeightFromWidth / 2.0;
            double idealWidthFromHeight = maxWidthFromHeight / 2.0;
            double idealHeightFromHeight = maxHeightFromHeight / 2.0;
            double slopeFromWidth = -(maxHeightFromWidth / maxWidthFromWidth);
            double slopeFromHeight = -(maxHeightFromHeight / maxWidthFromHeight);
            if ((0.0 == arrangeBounds.Width) || (0.0 == arrangeBounds.Height))
            {
                return new Size(arrangeBounds.Width, arrangeBounds.Height);
            }
            if (infiniteWidth && infiniteHeight)
            {
                return new Size(double.PositiveInfinity, double.PositiveInfinity);
            }
            if (!MatrixHasInverse(this._transformation))
            {
                return new Size(0.0, 0.0);
            }
            if ((0.0 == b) || (0.0 == c))
            {
                double maxHeight = infiniteHeight ? double.PositiveInfinity : maxHeightFromHeight;
                double maxWidth = infiniteWidth ? double.PositiveInfinity : maxWidthFromWidth;
                if ((0.0 == b) && (0.0 == c))
                {
                    return new Size(maxWidth, maxHeight);
                }
                if (0.0 == b)
                {
                    double computedHeight = Math.Min(idealHeightFromWidth, maxHeight);
                    return new Size(maxWidth - Math.Abs((double) ((c * computedHeight) / a)), computedHeight);
                }
                if (0.0 == c)
                {
                    double computedWidth = Math.Min(idealWidthFromHeight, maxWidth);
                    computedSize = new Size(computedWidth, maxHeight - Math.Abs((double) ((b * computedWidth) / d)));
                }
                return computedSize;
            }
            if ((0.0 == a) || (0.0 == d))
            {
                double maxWidth = infiniteHeight ? double.PositiveInfinity : maxWidthFromHeight;
                double maxHeight = infiniteWidth ? double.PositiveInfinity : maxHeightFromWidth;
                if ((0.0 == a) && (0.0 == d))
                {
                    return new Size(maxWidth, maxHeight);
                }
                if (0.0 == a)
                {
                    double computedHeight = Math.Min(idealHeightFromHeight, maxHeight);
                    return new Size(maxWidth - Math.Abs((double) ((d * computedHeight) / b)), computedHeight);
                }
                if (0.0 == d)
                {
                    double computedWidth = Math.Min(idealWidthFromWidth, maxWidth);
                    computedSize = new Size(computedWidth, maxHeight - Math.Abs((double) ((a * computedWidth) / c)));
                }
                return computedSize;
            }
            if (idealHeightFromWidth <= ((slopeFromHeight * idealWidthFromWidth) + maxHeightFromHeight))
            {
                return new Size(idealWidthFromWidth, idealHeightFromWidth);
            }
            if (idealHeightFromHeight <= ((slopeFromWidth * idealWidthFromHeight) + maxHeightFromWidth))
            {
                return new Size(idealWidthFromHeight, idealHeightFromHeight);
            }
            double _computedWidth = (maxHeightFromHeight - maxHeightFromWidth) / (slopeFromWidth - slopeFromHeight);
            return new Size(_computedWidth, (slopeFromWidth * _computedWidth) + maxHeightFromWidth);
        }

        private Matrix GetTransformMatrix(Transform transform)
        {
            if (transform != null)
            {
                TransformGroup transformGroup = transform as TransformGroup;
                if (transformGroup != null)
                {
                    Matrix groupMatrix = Matrix.Identity;
                    foreach (Transform child in transformGroup.Children)
                    {
                        groupMatrix = MatrixMultiply(groupMatrix, this.GetTransformMatrix(child));
                    }
                    return groupMatrix;
                }
                RotateTransform rotateTransform = transform as RotateTransform;
                if (rotateTransform != null)
                {
                    double angle = rotateTransform.Angle;
                    double angleRadians = (6.2831853071795862 * angle) / 360.0;
                    double sine = Math.Sin(angleRadians);
                    double cosine = Math.Cos(angleRadians);
                    return new Matrix(cosine, sine, -sine, cosine, 0.0, 0.0);
                }
                ScaleTransform scaleTransform = transform as ScaleTransform;
                if (scaleTransform != null)
                {
                    double scaleX = scaleTransform.ScaleX;
                    return new Matrix(scaleX, 0.0, 0.0, scaleTransform.ScaleY, 0.0, 0.0);
                }
                SkewTransform skewTransform = transform as SkewTransform;
                if (skewTransform != null)
                {
                    double angleX = skewTransform.AngleX;
                    double angleY = skewTransform.AngleY;
                    double angleXRadians = (6.2831853071795862 * angleX) / 360.0;
                    return new Matrix(1.0, (6.2831853071795862 * angleY) / 360.0, angleXRadians, 1.0, 0.0, 0.0);
                }
                MatrixTransform matrixTransform = transform as MatrixTransform;
                if (matrixTransform != null)
                {
                    return matrixTransform.Matrix;
                }
            }
            return Matrix.Identity;
        }

        private static bool IsSizeSmaller(Size a, Size b)
        {
            if ((a.Width + 0.0001) >= b.Width)
            {
                return ((a.Height + 0.0001) < b.Height);
            }
            return true;
        }

        private static void LayoutTransformChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutTransformControl) o).ProcessTransform((Transform) e.NewValue);
        }

        private static bool MatrixHasInverse(Matrix matrix)
        {
            return (0.0 != ((matrix.M11 * matrix.M22) - (matrix.M12 * matrix.M21)));
        }

        private static Matrix MatrixMultiply(Matrix matrix1, Matrix matrix2)
        {
            return new Matrix((matrix1.M11 * matrix2.M11) + (matrix1.M12 * matrix2.M21), (matrix1.M11 * matrix2.M12) + (matrix1.M12 * matrix2.M22), (matrix1.M21 * matrix2.M11) + (matrix1.M22 * matrix2.M21), (matrix1.M21 * matrix2.M12) + (matrix1.M22 * matrix2.M22), ((matrix1.OffsetX * matrix2.M11) + (matrix1.OffsetY * matrix2.M21)) + matrix2.OffsetX, ((matrix1.OffsetX * matrix2.M12) + (matrix1.OffsetY * matrix2.M22)) + matrix2.OffsetY);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            Size measureSize;
            FrameworkElement child = this.Child;
            if ((this._transformationPanel == null) || (child == null))
            {
                return Size.Empty;
            }
            if (this._childActualSize == Size.Empty)
            {
                measureSize = this.ComputeLargestTransformedSize(availableSize);
            }
            else
            {
                measureSize = this._childActualSize;
            }
            this._transformationPanel.Measure(measureSize);
            Rect transformedDesiredRect = RectTransform(new Rect(0.0, 0.0, this._transformationPanel.DesiredSize.Width, this._transformationPanel.DesiredSize.Height), this._transformation);
            return new Size(transformedDesiredRect.Width, transformedDesiredRect.Height);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this._transformationPanel = base.GetTemplateChild("RootVisual") as Grid;
            this._contentPresenter = base.GetTemplateChild("ContentPresenter") as ContentPresenter;
            this._matrixTransform = new MatrixTransform();
            if (this._transformationPanel != null)
            {
                this._transformationPanel.RenderTransform = this._matrixTransform;
            }
            this.ApplyLayoutTransform();
        }

        private void ProcessTransform(Transform transform)
        {
            this._transformation = RoundMatrix(this.GetTransformMatrix(transform), 4);
            if (this._matrixTransform != null)
            {
                this._matrixTransform.Matrix = this._transformation;
            }
            base.InvalidateMeasure();
        }

        private static Rect RectTransform(Rect rect, Matrix matrix)
        {
            Point leftTop = matrix.Transform(new Point(rect.Left, rect.Top));
            Point rightTop = matrix.Transform(new Point(rect.Right, rect.Top));
            Point leftBottom = matrix.Transform(new Point(rect.Left, rect.Bottom));
            Point rightBottom = matrix.Transform(new Point(rect.Right, rect.Bottom));
            double left = Math.Min(Math.Min(leftTop.X, rightTop.X), Math.Min(leftBottom.X, rightBottom.X));
            double top = Math.Min(Math.Min(leftTop.Y, rightTop.Y), Math.Min(leftBottom.Y, rightBottom.Y));
            double right = Math.Max(Math.Max(leftTop.X, rightTop.X), Math.Max(leftBottom.X, rightBottom.X));
            double bottom = Math.Max(Math.Max(leftTop.Y, rightTop.Y), Math.Max(leftBottom.Y, rightBottom.Y));
            return new Rect(left, top, right - left, bottom - top);
        }

        private static Matrix RoundMatrix(Matrix matrix, int decimals)
        {
            return new Matrix(Math.Round(matrix.M11, decimals), Math.Round(matrix.M12, decimals), Math.Round(matrix.M21, decimals), Math.Round(matrix.M22, decimals), matrix.OffsetX, matrix.OffsetY);
        }

        private FrameworkElement Child
        {
            get
            {
                if (this._contentPresenter == null)
                {
                    return null;
                }
                return ((this._contentPresenter.Content as FrameworkElement) ?? this._contentPresenter);
            }
        }

        public Transform LayoutTransform
        {
            get
            {
                return (Transform) base.GetValue(LayoutTransformProperty);
            }
            set
            {
                base.SetValue(LayoutTransformProperty, value);
            }
        }
    }
}

