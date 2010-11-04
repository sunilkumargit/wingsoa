namespace Telerik.Windows.Controls.OutlookBar
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using Telerik.Windows.Controls;

    public class OutlookBarPanel : Panel
    {
        public static readonly DependencyProperty ItemsMaxCountProperty = DependencyProperty.Register("ItemsMaxCount", typeof(int), typeof(OutlookBarPanel), new PropertyMetadata(0x7fffffff, new PropertyChangedCallback(OutlookBarPanel.OnPropertyChanged)));
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(System.Windows.Controls.Orientation), typeof(OutlookBarPanel), new PropertyMetadata(System.Windows.Controls.Orientation.Vertical, new PropertyChangedCallback(OutlookBarPanel.OnPropertyChanged)));

        protected override Size ArrangeOverride(Size finalSize)
        {
            IEnumerable<UIElement> children = base.Children.Cast<UIElement>().ToList<UIElement>();
            if (this.Orientation == System.Windows.Controls.Orientation.Vertical)
            {
                double remainingHeight = finalSize.Height;
                double totalHeight = 0.0;
                int counter = 0;
                foreach (UIElement child in children)
                {
                    IOutlookBarItem outlookBarItem = child as IOutlookBarItem;
                    double arrangeHeight = child.DesiredSize.Height;
                    if ((Math.Round(arrangeHeight).IsLessThanOrClose(Math.Round(remainingHeight)) && (finalSize.Height != 0.0)) && (counter < this.ItemsMaxCount))
                    {
                        child.Arrange(new Rect(0.0, totalHeight, finalSize.Width, arrangeHeight));
                        totalHeight += arrangeHeight;
                        remainingHeight -= arrangeHeight;
                        if (outlookBarItem != null)
                        {
                            outlookBarItem.Location = OutlookBarItemPosition.ActiveArea;
                            counter++;
                        }
                    }
                    else
                    {
                        child.Arrange(new Rect(0.0, 0.0, 0.0, 0.0));
                        remainingHeight = 0.0;
                        if ((outlookBarItem != null) && (outlookBarItem.Location != OutlookBarItemPosition.OverflowArea))
                        {
                            outlookBarItem.Location = OutlookBarItemPosition.MinimizedArea;
                        }
                    }
                }
                return new Size((children.Count<UIElement>() > 0) ? finalSize.Width : 0.0, totalHeight);
            }
            double remainingWidth = finalSize.Width;
            double totalWidth = 0.0;
            foreach (UIElement child in children)
            {
                IOutlookBarItem outlookBarItem = child as IOutlookBarItem;
                double arrangeWidth = child.DesiredSize.Width;
                if (Math.Round(arrangeWidth).IsLessThanOrClose(Math.Round(remainingWidth)) && (finalSize.Width != 0.0))
                {
                    child.Arrange(new Rect(totalWidth, 0.0, arrangeWidth, finalSize.Height));
                    totalWidth += arrangeWidth;
                    remainingWidth -= arrangeWidth;
                    if (outlookBarItem != null)
                    {
                        outlookBarItem.Location = OutlookBarItemPosition.MinimizedArea;
                    }
                }
                else
                {
                    child.Arrange(new Rect(0.0, 0.0, 0.0, 0.0));
                    remainingWidth = 0.0;
                    if (outlookBarItem != null)
                    {
                        outlookBarItem.Location = OutlookBarItemPosition.OverflowArea;
                    }
                }
            }
            return new Size(totalWidth, (children.Count<UIElement>() > 0) ? finalSize.Height : 0.0);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            IEnumerable<UIElement> children = base.Children.Cast<UIElement>().ToList<UIElement>();
            if (!children.Any<UIElement>())
            {
                return new Size(0.0, 0.0);
            }
            foreach (UIElement child in children)
            {
                child.Measure(availableSize);
            }
            if (this.Orientation == System.Windows.Controls.Orientation.Vertical)
            {
                int counter = 0;
                double desiredWidth = Math.Min(children.Max<UIElement>(child => child.DesiredSize.Width), availableSize.Width);
                double availableHeight = availableSize.Height;
                return new Size(desiredWidth, children.TakeWhile<UIElement>(delegate (UIElement item) {
                    availableHeight -= item.DesiredSize.Height;
                    counter++;
                    return ((availableHeight > 0.0) && (counter <= this.ItemsMaxCount));
                }).Sum<UIElement>(item => item.DesiredSize.Height));
            }
            double desiredHeight = Math.Min(children.Max<UIElement>(child => child.DesiredSize.Height), availableSize.Height);
            double availableWidth = availableSize.Width;
            return new Size(children.TakeWhile<UIElement>(delegate (UIElement item) {
                availableWidth -= item.DesiredSize.Width;
                return (availableWidth > 0.0);
            }).Sum<UIElement>(item => item.DesiredSize.Width), desiredHeight);
        }

        private static void OnPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            OutlookBarPanel outlookBarPanel = sender as OutlookBarPanel;
            if (outlookBarPanel != null)
            {
                outlookBarPanel.InvalidateMeasure();
            }
        }

        public int ItemsMaxCount
        {
            get
            {
                return (int) base.GetValue(ItemsMaxCountProperty);
            }
            set
            {
                base.SetValue(ItemsMaxCountProperty, value);
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
    }
}

