namespace Telerik.Windows.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    public class ProportionalStackPanel : Panel
    {
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(System.Windows.Controls.Orientation), typeof(ProportionalStackPanel), new PropertyMetadata(new PropertyChangedCallback(ProportionalStackPanel.OnOrientationChanged)));
        public static readonly DependencyProperty RelativeSizeProperty = DependencyProperty.RegisterAttached("RelativeSize", typeof(Size), typeof(ProportionalStackPanel), new PropertyMetadata(new Size(100.0, 100.0), new PropertyChangedCallback(ProportionalStackPanel.OnRelativeSizeChanged)));
        internal static readonly DependencyProperty SplitterChangeProperty = DependencyProperty.RegisterAttached("SplitterChange", typeof(double), typeof(ProportionalStackPanel), null);
        public static readonly DependencyProperty SuppressMeasureProperty = DependencyProperty.RegisterAttached("SuppressMeasure", typeof(bool), typeof(ProportionalStackPanel), new PropertyMetadata(false));
        private List<ChildSize> visibleChildren;

        protected override Size ArrangeOverride(Size finalSize)
        {
            double changesSum = 0.0;
            double relativeLengthSum = 0.0;
            double relativeLengthWithChangeSet = 0.0;
            double finalLength = this.IsHorizontal ? finalSize.Width : finalSize.Height;
            Rect finalRect = new Rect(new Point(), finalSize);
            foreach (ChildSize child in this.visibleChildren)
            {
                double change = GetSplitterChange(child.Element);
                relativeLengthSum += child.RelativeLength;
                if (change != 0.0)
                {
                    changesSum += change;
                    relativeLengthWithChangeSet += child.RelativeLength;
                }
            }
            double coef = finalLength / relativeLengthSum;
            double coef2 = relativeLengthWithChangeSet / changesSum;
            foreach (ChildSize child in this.visibleChildren)
            {
                double length;
                FrameworkElement element = child.Element;
                double change = GetSplitterChange(child.Element);
                if (change == 0.0)
                {
                    length = child.RelativeLength * coef;
                }
                else
                {
                    length = (change * coef2) * coef;
                }
                if (this.IsHorizontal)
                {
                    finalRect.Width = length;
                }
                else
                {
                    finalRect.Height = length;
                }
                element.Arrange(finalRect);
                if (this.IsHorizontal)
                {
                    finalRect.X += element.RenderSize.Width;
                }
                else
                {
                    finalRect.Y += element.RenderSize.Height;
                }
            }
            return finalSize;
        }

        public static Size GetRelativeSize(DependencyObject obj)
        {
            return (Size) obj.GetValue(RelativeSizeProperty);
        }

        internal static double GetSplitterChange(DependencyObject obj)
        {
            return (double) obj.GetValue(SplitterChangeProperty);
        }

        public static bool GetSuppressMeasure(DependencyObject obj)
        {
            return (bool) obj.GetValue(SuppressMeasureProperty);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            this.visibleChildren = new List<ChildSize>();
            int end = base.Children.Count;
            for (int i = 0; i < end; i++)
            {
                FrameworkElement element = base.Children[i] as FrameworkElement;
                if (element != null)
                {
                    if (element.Visibility != Visibility.Collapsed)
                    {
                        ChildSize child = new ChildSize(element, this.IsHorizontal);
                        this.visibleChildren.Add(child);
                    }
                    else
                    {
                        element.Measure(availableSize);
                    }
                }
            }
            Size desiredSize = new Size();
            Size measureSize = availableSize;
            double relativeLengthWithoutChange = 0.0;
            double relativeLengthWithChangeSet = 0.0;
            double relativeChangesSum = 0.0;
            double relativeLengthSum = 0.0;
            foreach (ChildSize child in this.visibleChildren)
            {
                double change = GetSplitterChange(child.Element);
                relativeLengthSum += child.RelativeLength;
                if (change == 0.0)
                {
                    relativeLengthWithoutChange += child.RelativeLength;
                }
                else
                {
                    relativeChangesSum += change;
                    relativeLengthWithChangeSet += child.RelativeLength;
                }
            }
            double availableLength = this.IsHorizontal ? availableSize.Width : availableSize.Height;
            bool isAvailableLengthInfinity = double.IsInfinity(availableLength);
            foreach (ChildSize child in this.visibleChildren)
            {
                double length;
                FrameworkElement element = child.Element;
                double change = GetSplitterChange(child.Element);
                if (change == 0.0)
                {
                    if (isAvailableLengthInfinity)
                    {
                        length = child.RelativeLength;
                    }
                    else
                    {
                        length = child.RelativeLength * (availableLength / (relativeLengthWithoutChange + relativeLengthWithChangeSet));
                    }
                }
                else if (isAvailableLengthInfinity)
                {
                    length = child.RelativeLength;
                }
                else
                {
                    length = ((change * relativeLengthWithChangeSet) / relativeChangesSum) * (availableLength / (relativeLengthWithoutChange + relativeLengthWithChangeSet));
                }
                if (this.IsHorizontal)
                {
                    measureSize.Width = length;
                }
                else
                {
                    measureSize.Height = length;
                }
                element.Measure(measureSize);
                child.DesiredSize = element.DesiredSize;
                if (this.IsHorizontal)
                {
                    desiredSize.Width += length;
                    desiredSize.Height = Math.Max(desiredSize.Height, child.DesiredSize.Height);
                }
                else
                {
                    desiredSize.Height += length;
                    desiredSize.Width = Math.Max(desiredSize.Width, child.DesiredSize.Width);
                }
            }
            availableLength = this.IsHorizontal ? availableSize.Width : availableSize.Height;
            if (!isAvailableLengthInfinity)
            {
                relativeLengthWithoutChange = this.IsHorizontal ? desiredSize.Width : desiredSize.Height;
                double diff = availableLength - relativeLengthWithoutChange;
                if (diff <= 0.0)
                {
                    return desiredSize;
                }
                if (this.IsHorizontal)
                {
                    desiredSize.Width = availableSize.Width;
                    return desiredSize;
                }
                desiredSize.Height = availableSize.Height;
            }
            return desiredSize;
        }

        private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ProportionalStackPanel).InvalidateMeasure();
        }

        private static void OnRelativeSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ProportionalStackPanel panel = VisualTreeHelper.GetParent(d) as ProportionalStackPanel;
            if ((panel != null) && !GetSuppressMeasure(panel))
            {
                panel.InvalidateMeasure();
            }
        }

        public static void SetRelativeSize(DependencyObject obj, Size value)
        {
            obj.SetValue(RelativeSizeProperty, value);
        }

        internal static void SetSplitterChange(DependencyObject obj, double value)
        {
            obj.SetValue(SplitterChangeProperty, value);
        }

        public static void SetSuppressMeasure(DependencyObject obj, bool value)
        {
            obj.SetValue(SuppressMeasureProperty, value);
        }

        private bool IsHorizontal
        {
            get
            {
                return (this.Orientation == System.Windows.Controls.Orientation.Horizontal);
            }
        }

        public System.Windows.Controls.Orientation Orientation
        {
            get
            {
                return (System.Windows.Controls.Orientation) base.GetValue(OrientationProperty);
            }
            set
            {
                base.SetValue(OrientationProperty, value);
            }
        }

        private class ChildSize
        {
            public ChildSize(FrameworkElement element, bool horizontal)
            {
                this.Element = element;
                this.RelativeSize = ProportionalStackPanel.GetRelativeSize(element);
                if (horizontal)
                {
                    this.RelativeLength = this.RelativeSize.Width;
                }
                else
                {
                    this.RelativeLength = this.RelativeSize.Height;
                }
            }

            public Size DesiredSize { get; set; }

            public FrameworkElement Element { get; private set; }

            public double RelativeLength { get; private set; }

            public Size RelativeSize { get; private set; }
        }
    }
}

