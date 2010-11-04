namespace Telerik.Windows.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    public class PanelBarPanel : Panel
    {
        public static readonly DependencyProperty DesiredHeightProperty = DependencyProperty.RegisterAttached("DesiredHeight", typeof(GridLength), typeof(PanelBarPanel), new PropertyMetadata(new PropertyChangedCallback(PanelBarPanel.OnDesiredSizeChanged)));
        public static readonly DependencyProperty DesiredWidthProperty = DependencyProperty.RegisterAttached("DesiredWidth", typeof(GridLength), typeof(PanelBarPanel), new PropertyMetadata(new PropertyChangedCallback(PanelBarPanel.OnDesiredSizeChanged)));
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(PanelBarPanel), new PropertyMetadata(new PropertyChangedCallback(PanelBarPanel.OnOrientationChanged)));
        internal static readonly DependencyProperty SuppressMeasureProperty = DependencyProperty.RegisterAttached("SuppressMeasure", typeof(bool), typeof(PanelBarPanel), new PropertyMetadata(false));

        protected override Size ArrangeOverride(Size finalSize)
        {
            IEnumerable<UIElement> starElements = from el in base.Children.Cast<UIElement>()
                where GetDesiredHeight(el).IsStar
                select el;
            double nonStarDesiredHeight = (from el in base.Children.Cast<UIElement>()
                where !GetDesiredHeight(el).IsStar
                select el).Sum<UIElement>(el => el.DesiredSize.Height);
            double starDesiredHeight = starElements.Sum<UIElement>(el => el.DesiredSize.Height);
            double starRemainingHeight = Math.Max((double) (finalSize.Height - nonStarDesiredHeight), (double) 0.0);
            double currentHeight = 0.0;
            if (starRemainingHeight < starDesiredHeight)
            {
                foreach (UIElement child in base.Children.Cast<UIElement>())
                {
                    if (GetDesiredHeight(child).IsStar)
                    {
                        double arrangeHeight = Math.Min(child.DesiredSize.Height, starRemainingHeight);
                        child.Arrange(new Rect(0.0, currentHeight, finalSize.Width, arrangeHeight));
                        currentHeight += arrangeHeight;
                        starRemainingHeight = Math.Max((double) 0.0, (double) (starRemainingHeight - arrangeHeight));
                    }
                    else
                    {
                        double arrangeHeight = child.DesiredSize.Height;
                        child.Arrange(new Rect(0.0, currentHeight, finalSize.Width, arrangeHeight));
                        currentHeight += arrangeHeight;
                    }
                }
                return finalSize;
            }
            double starAdditionalHeight = 0.0;
            if (starElements.Any<UIElement>())
            {
                starAdditionalHeight = (starRemainingHeight - starDesiredHeight) / ((double) starElements.Count<UIElement>());
            }
            foreach (UIElement child in base.Children.Cast<UIElement>())
            {
                double arrangeHeight = child.DesiredSize.Height;
                if (GetDesiredHeight(child).IsStar)
                {
                    arrangeHeight += starAdditionalHeight;
                }
                child.Arrange(new Rect(0.0, currentHeight, finalSize.Width, arrangeHeight));
                currentHeight += arrangeHeight;
            }
            return finalSize;
        }

        internal static double CountElements(Panel panel, bool horizontal, List<UIElement> starElements, List<UIElement> notStarElements)
        {
            double allStarsCount = 0.0;
            foreach (UIElement element in panel.Children)
            {
                GridLength gridlength;
                if (element.Visibility == Visibility.Collapsed)
                {
                    continue;
                }
                if (horizontal)
                {
                    gridlength = (GridLength) element.GetValue(DesiredWidthProperty);
                }
                else
                {
                    gridlength = (GridLength) element.GetValue(DesiredHeightProperty);
                }
                if (gridlength.IsStar)
                {
                    allStarsCount += gridlength.Value;
                    starElements.Add(element);
                }
                else
                {
                    notStarElements.Add(element);
                }
            }
            return allStarsCount;
        }

        public static GridLength GetDesiredHeight(DependencyObject obj)
        {
            return (GridLength) obj.GetValue(DesiredHeightProperty);
        }

        public static GridLength GetDesiredWidth(DependencyObject obj)
        {
            return (GridLength) obj.GetValue(DesiredWidthProperty);
        }

        internal static bool GetSuppressMeasure(DependencyObject obj)
        {
            return (bool) obj.GetValue(SuppressMeasureProperty);
        }

        private static void MeasureElement(ref Size size, ref Size remainingSize, ref Size sizeToMeasureWith, bool horizontal, UIElement element)
        {
            element.Measure(sizeToMeasureWith);
            Size desiredSize = element.DesiredSize;
            if (horizontal)
            {
                size.Width += desiredSize.Width;
                size.Height = Math.Max(size.Height, desiredSize.Height);
                remainingSize.Width = Math.Max((double) 0.0, (double) (remainingSize.Width - desiredSize.Width));
            }
            else
            {
                size.Width = Math.Max(size.Width, desiredSize.Width);
                size.Height += desiredSize.Height;
                remainingSize.Height = Math.Max((double) 0.0, (double) (remainingSize.Height - desiredSize.Height));
            }
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        protected override Size MeasureOverride(Size availableSize)
        {
            double maxDesiredWidth = availableSize.Width;
            double maxDesiredHeight = availableSize.Height;
            if (double.IsInfinity(availableSize.Height))
            {
                foreach (FrameworkElement child in base.Children.Cast<FrameworkElement>())
                {
                    child.Measure(availableSize);
                }
            }
            else
            {
                IEnumerable<UIElement> starElements = from el in base.Children.Cast<UIElement>()
                    where GetDesiredHeight(el).IsStar
                    select el;
                IEnumerable<UIElement> notStarElements = from el in base.Children.Cast<UIElement>()
                    where !GetDesiredHeight(el).IsStar
                    select el;
                double remainingHeight = availableSize.Height;
                foreach (UIElement child in notStarElements)
                {
                    child.Measure(new Size(availableSize.Width, Math.Max(remainingHeight, 0.0)));
                    remainingHeight -= child.DesiredSize.Height;
                }
                if (starElements.Any<UIElement>())
                {
                    double unfilledHeight = Math.Max((double) (availableSize.Height - notStarElements.Sum<UIElement>(el => el.DesiredSize.Height)), (double) 0.0);
                    foreach (UIElement element in starElements)
                    {
                        element.Measure(new Size(availableSize.Width, double.PositiveInfinity));
                    }
                    double starElementsTotalDesiredHeight = starElements.Sum<UIElement>(el => el.DesiredSize.Height);
                    if (unfilledHeight >= starElementsTotalDesiredHeight)
                    {
                        double additionalHeight = (unfilledHeight - starElementsTotalDesiredHeight) / ((double) starElements.Count<UIElement>());
                        foreach (UIElement starEl in starElements)
                        {
                            starEl.Measure(new Size(availableSize.Width, starEl.DesiredSize.Height + additionalHeight));
                        }
                    }
                    else
                    {
                        foreach (FrameworkElement element in starElements.Cast<FrameworkElement>())
                        {
                            double measureHeight = unfilledHeight;
                            element.Measure(new Size(availableSize.Width, measureHeight));
                            unfilledHeight = Math.Max((double) (unfilledHeight - element.DesiredSize.Height), (double) 0.0);
                        }
                    }
                }
            }
            if (base.Children.Cast<UIElement>().Count<UIElement>() == 0)
            {
                maxDesiredHeight = 0.0;
                maxDesiredWidth = 0.0;
            }
            else
            {
                maxDesiredWidth = Math.Min(maxDesiredWidth, base.Children.Cast<UIElement>().Max<UIElement>(el => el.DesiredSize.Width));
                maxDesiredHeight = Math.Min(maxDesiredHeight, base.Children.Cast<UIElement>().Sum<UIElement>(el => el.DesiredSize.Height));
            }
            return new Size(maxDesiredWidth, maxDesiredHeight);
        }

        private static void OnDesiredSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PanelBarPanel panel = VisualTreeHelper.GetParent(d) as PanelBarPanel;
            if ((panel != null) && !GetSuppressMeasure(panel))
            {
                panel.InvalidateMeasure();
            }
        }

        private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as PanelBarPanel).InvalidateMeasure();
        }

        public static void SetDesiredHeight(DependencyObject obj, GridLength value)
        {
            obj.SetValue(DesiredHeightProperty, value);
        }

        public static void SetDesiredWidth(DependencyObject obj, GridLength value)
        {
            obj.SetValue(DesiredWidthProperty, value);
        }

        internal static void SetSuppressMeasure(DependencyObject obj, bool value)
        {
            obj.SetValue(SuppressMeasureProperty, value);
        }
    }
}

