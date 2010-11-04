namespace Telerik.Windows.Controls
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using Telerik.Windows;

    [DefaultProperty("Orientation")]
    public class RadWrapPanel : Panel
    {
        public static readonly DependencyProperty AnimationDurationProperty = DependencyProperty.Register("AnimationDuration", typeof(int), typeof(RadWrapPanel), new Telerik.Windows.PropertyMetadata(400));
        public static readonly DependencyProperty IsAnimatedProperty = DependencyProperty.Register("IsAnimated", typeof(bool), typeof(RadWrapPanel), null);
        public static readonly DependencyProperty ItemHeightProperty = DependencyProperty.Register("ItemHeight", typeof(double), typeof(RadWrapPanel), new Telerik.Windows.PropertyMetadata((double) 1.0 / (double) 0.0, new PropertyChangedCallback(RadWrapPanel.OnItemHeightChanged)));
        public static readonly DependencyProperty ItemWidthProperty = DependencyProperty.Register("ItemWidth", typeof(double), typeof(RadWrapPanel), new Telerik.Windows.PropertyMetadata((double) 1.0 / (double) 0.0, new PropertyChangedCallback(RadWrapPanel.OnItemHeightChanged)));
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(System.Windows.Controls.Orientation), typeof(RadWrapPanel), new Telerik.Windows.PropertyMetadata(System.Windows.Controls.Orientation.Horizontal, new PropertyChangedCallback(RadWrapPanel.OnOrientationChanged)));
        private Dictionary<UIElement, Point> previousTopLeftArrangePoint;

        public RadWrapPanel()
        {
            
            this.previousTopLeftArrangePoint = new Dictionary<UIElement, Point>();
        }

        private void AnimateLocation(UIElement element, Point previousTopLeft, Point newTopLeft)
        {
            int index = -1;
            TranslateTransform translate = GetTranslateTransform(element, ref index);
            PropertyPath path = null;
            PropertyPath path2 = null;
            if (translate == null)
            {
                translate = CreateTranslateTransfort(element);
                index = 0;
            }
            if (index == -1)
            {
                path = new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.X)", new object[0]);
                path2 = new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)", new object[0]);
            }
            else
            {
                path = new PropertyPath(string.Format(CultureInfo.InvariantCulture, "(UIElement.RenderTransform).(TransformGroup.Children)[{0}].(TranslateTransform.X)", new object[] { index }), new object[0]);
                path2 = new PropertyPath(string.Format(CultureInfo.InvariantCulture, "(UIElement.RenderTransform).(TransformGroup.Children)[{0}].(TranslateTransform.Y)", new object[] { index }), new object[0]);
            }
            Storyboard storyboard = new Storyboard();
            DoubleAnimation doubleAnimationX = this.SetupDoubleAnimation(previousTopLeft.X, newTopLeft.X);
            DoubleAnimation doubleAnimationY = this.SetupDoubleAnimation(previousTopLeft.Y, newTopLeft.Y);
            storyboard.Children.Add(doubleAnimationX);
            storyboard.Children.Add(doubleAnimationY);
            Storyboard.SetTarget(doubleAnimationX, element);
            Storyboard.SetTargetProperty(doubleAnimationX, path);
            Storyboard.SetTarget(doubleAnimationY, element);
            Storyboard.SetTargetProperty(doubleAnimationY, path2);
            storyboard.Begin();
        }

        private void ArrangeLine(double v, double lineV, int start, int end, bool useItemU, double itemU)
        {
            double num = 0.0;
            bool flag = this.Orientation == System.Windows.Controls.Orientation.Horizontal;
            UIElementCollection internalChildren = base.Children;
            for (int i = start; i < end; i++)
            {
                UIElement element = internalChildren[i];
                if (element != null)
                {
                    UVSize size = new UVSize(this.Orientation, element.DesiredSize.Width, element.DesiredSize.Height);
                    double num3 = useItemU ? itemU : size.U;
                    Rect finalRect = new Rect(flag ? num : v, flag ? v : num, flag ? num3 : lineV, flag ? lineV : num3);
                    Point previousTopLeft = this.previousTopLeftArrangePoint[element];
                    Point newTopLeft = new Point(finalRect.Left, finalRect.Top);
                    int index = -1;
                    TranslateTransform tt = GetTranslateTransform(element, ref index);
                    if ((element.Visibility == Visibility.Visible) && this.IsAnimated)
                    {
                        if (((tt != null) && (tt.X != previousTopLeft.X)) && (tt.Y != previousTopLeft.Y))
                        {
                            previousTopLeft.X = -tt.X;
                            previousTopLeft.Y = -tt.Y;
                        }
                        if (newTopLeft != previousTopLeft)
                        {
                            this.AnimateLocation(element, previousTopLeft, newTopLeft);
                        }
                    }
                    else if (tt != null)
                    {
                        tt.X = tt.Y = 0.0;
                    }
                    this.previousTopLeftArrangePoint[element] = newTopLeft;
                    element.Arrange(finalRect);
                    if (element.Visibility == Visibility.Visible)
                    {
                        num += num3;
                    }
                }
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            int start = 0;
            double itemWidth = this.ItemWidth;
            double itemHeight = this.ItemHeight;
            double v = 0.0;
            double itemU = (this.Orientation == System.Windows.Controls.Orientation.Horizontal) ? itemWidth : itemHeight;
            UVSize size = new UVSize(this.Orientation);
            UVSize size2 = new UVSize(this.Orientation, finalSize.Width, finalSize.Height);
            bool flag = !double.IsNaN(itemWidth);
            bool flag2 = !double.IsNaN(itemHeight);
            bool useItemU = (this.Orientation == System.Windows.Controls.Orientation.Horizontal) ? flag : flag2;
            UIElementCollection internalChildren = base.Children;
            int end = 0;
            int count = internalChildren.Count;
            List<UIElement> readyForDeletion = new List<UIElement>();
            foreach (KeyValuePair<UIElement, Point> item in this.previousTopLeftArrangePoint)
            {
                if (!base.Children.Contains(item.Key))
                {
                    readyForDeletion.Add(item.Key);
                }
            }
            foreach (UIElement item in readyForDeletion)
            {
                this.previousTopLeftArrangePoint.Remove(item);
            }
            while (end < count)
            {
                UIElement element = internalChildren[end];
                if ((element != null) && (element.Visibility == Visibility.Visible))
                {
                    UVSize size3 = new UVSize(this.Orientation, flag ? itemWidth : element.DesiredSize.Width, flag2 ? itemHeight : element.DesiredSize.Height);
                    if ((size.U + size3.U) > size2.U)
                    {
                        this.ArrangeLine(v, size.V, start, end, useItemU, itemU);
                        v += size.V;
                        size = size3;
                        if (size3.U > size2.U)
                        {
                            this.ArrangeLine(v, size3.V, end, ++end, useItemU, itemU);
                            v += size3.V;
                            size = new UVSize(this.Orientation);
                        }
                        start = end;
                    }
                    else
                    {
                        size.U += size3.U;
                        size.V = Math.Max(size3.V, size.V);
                    }
                }
                end++;
            }
            if (start < internalChildren.Count)
            {
                this.ArrangeLine(v, size.V, start, internalChildren.Count, useItemU, itemU);
            }
            return finalSize;
        }

        private static TranslateTransform CreateTranslateTransfort(UIElement element)
        {
            TransformGroup group = new TransformGroup();
            TranslateTransform translation = new TranslateTransform();
            group.Children.Add(translation);
            element.RenderTransform = group;
            return translation;
        }

        private static TranslateTransform GetTranslateTransform(UIElement element, ref int index)
        {
            TranslateTransform translate = null;
            TransformGroup tg = element.RenderTransform as TransformGroup;
            if (tg != null)
            {
                foreach (Transform transform in tg.Children)
                {
                    translate = transform as TranslateTransform;
                    if (translate != null)
                    {
                        index = tg.Children.IndexOf(translate);
                        return translate;
                    }
                }
                return translate;
            }
            return (element.RenderTransform as TranslateTransform);
        }

        private static bool IsWidthHeightValid(object value)
        {
            double num = (double) value;
            return (double.IsNaN(num) || ((num >= 0.0) && !double.IsPositiveInfinity(num)));
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            UVSize size = new UVSize(this.Orientation);
            UVSize size2 = new UVSize(this.Orientation);
            UVSize size3 = new UVSize(this.Orientation, availableSize.Width, availableSize.Height);
            double itemWidth = this.ItemWidth;
            double itemHeight = this.ItemHeight;
            bool flag = !double.IsNaN(itemWidth);
            bool flag2 = !double.IsNaN(itemHeight);
            Size remainingSize = new Size(flag ? itemWidth : availableSize.Width, flag2 ? itemHeight : availableSize.Height);
            UIElementCollection internalChildren = base.Children;
            int num3 = 0;
            int count = internalChildren.Count;
            while (num3 < count)
            {
                UIElement element = internalChildren[num3];
                if (!this.previousTopLeftArrangePoint.ContainsKey(element))
                {
                    this.previousTopLeftArrangePoint.Add(element, new Point());
                }
                if (element != null)
                {
                    element.Measure(remainingSize);
                    UVSize size5 = new UVSize(this.Orientation, flag ? itemWidth : element.DesiredSize.Width, flag2 ? itemHeight : element.DesiredSize.Height);
                    if ((size.U + size5.U) > size3.U)
                    {
                        size2.U = Math.Max(size.U, size2.U);
                        size2.V += size.V;
                        size = size5;
                        if (size5.U > size3.U)
                        {
                            size2.U = Math.Max(size5.U, size2.U);
                            size2.V += size5.V;
                            size = new UVSize(this.Orientation);
                        }
                    }
                    else
                    {
                        size.U += size5.U;
                        size.V = Math.Max(size5.V, size.V);
                    }
                }
                num3++;
            }
            size2.U = Math.Max(size.U, size2.U);
            size2.V += size.V;
            return new Size(size2.Width, size2.Height);
        }

        private static void OnItemHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!IsWidthHeightValid(e.NewValue))
            {
                throw new ArgumentException("Invalid value");
            }
            RadWrapPanel panel = d as RadWrapPanel;
            if (panel != null)
            {
                panel.InvalidateMeasure();
            }
        }

        private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadWrapPanel panel = (RadWrapPanel) d;
            if (panel != null)
            {
                panel.InvalidateMeasure();
                panel.InvalidateArrange();
            }
        }

        private DoubleAnimation SetupDoubleAnimation(double from, double to)
        {
            return new DoubleAnimation { From = new double?(from - to), To = 0.0, Duration = new Duration(TimeSpan.FromMilliseconds((double) this.AnimationDuration)), AutoReverse = false };
        }

        public int AnimationDuration
        {
            get
            {
                return (int) base.GetValue(AnimationDurationProperty);
            }
            set
            {
                base.SetValue(AnimationDurationProperty, value);
            }
        }

        public bool IsAnimated
        {
            get
            {
                return (bool) base.GetValue(IsAnimatedProperty);
            }
            set
            {
                base.SetValue(IsAnimatedProperty, value);
            }
        }

        public double ItemHeight
        {
            get
            {
                return (double) base.GetValue(ItemHeightProperty);
            }
            set
            {
                base.SetValue(ItemHeightProperty, value);
            }
        }

        public double ItemWidth
        {
            get
            {
                return (double) base.GetValue(ItemWidthProperty);
            }
            set
            {
                base.SetValue(ItemWidthProperty, value);
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

        [StructLayout(LayoutKind.Sequential)]
        private struct UVSize
        {
            internal double U;
            internal double V;
            private Orientation orientation;
            internal UVSize(Orientation orientation, double width, double height)
            {
                this.U = this.V = 0.0;
                this.orientation = orientation;
                this.Width = width;
                this.Height = height;
            }

            internal UVSize(Orientation orientation)
            {
                this.U = this.V = 0.0;
                this.orientation = orientation;
            }

            internal double Width
            {
                get
                {
                    if (this.orientation != Orientation.Horizontal)
                    {
                        return this.V;
                    }
                    return this.U;
                }
                set
                {
                    if (this.orientation == Orientation.Horizontal)
                    {
                        this.U = value;
                    }
                    else
                    {
                        this.V = value;
                    }
                }
            }
            internal double Height
            {
                get
                {
                    if (this.orientation != Orientation.Horizontal)
                    {
                        return this.U;
                    }
                    return this.V;
                }
                set
                {
                    if (this.orientation == Orientation.Horizontal)
                    {
                        this.V = value;
                    }
                    else
                    {
                        this.U = value;
                    }
                }
            }
        }
    }
}

