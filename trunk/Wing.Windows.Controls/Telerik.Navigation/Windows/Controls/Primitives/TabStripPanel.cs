namespace Telerik.Windows.Controls.Primitives
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using Telerik.Windows.Controls;
    using Telerik.Windows.Controls.TabControl;

    public class TabStripPanel : Panel
    {
        public static readonly DependencyProperty AlignProperty = DependencyProperty.Register("Align", typeof(TabStripAlign), typeof(TabStripPanel), new PropertyMetadata(TabStripAlign.Justify, null));
        public static readonly DependencyProperty AllTabsEqualHeightProperty = DependencyProperty.Register("AllTabsEqualHeight", typeof(bool), typeof(TabStripPanel), new PropertyMetadata(true, new PropertyChangedCallback(TabStripPanel.OnPanelPropertyChnaged)));
        public static readonly DependencyProperty RearrangeTabsProperty = DependencyProperty.Register("RearrangeTabs", typeof(bool), typeof(TabStripPanel), new PropertyMetadata(true, new PropertyChangedCallback(TabStripPanel.OnPanelPropertyChnaged)));
        private IList<List<UIElement>> rows;
        public static readonly DependencyProperty TabStripPlacementProperty = DependencyProperty.Register("TabStripPlacement", typeof(Dock), typeof(TabStripPanel), new PropertyMetadata(Dock.Top, new PropertyChangedCallback(TabStripPanel.OnPanelPropertyChnaged)));

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        protected override Size ArrangeOverride(Size finalSize)
        {
            this.Initialize();
            this.RearrangeForSelectedItem();
            double arrangeX = 0.0;
            double arrangeY = 0.0;
            double arrangeWidth = 0.0;
            double arrangeHeight = 0.0;
            double rowX = 0.0;
            double rowY = 0.0;
            if (base.Children.Count == 0)
            {
                return base.ArrangeOverride(finalSize);
            }
            if ((this.TabStripPlacement == Dock.Top) || (this.TabStripPlacement == Dock.Bottom))
            {
                double actualDesiredHeight = this.rows.Aggregate<List<UIElement>, double>(0.0, (total, next) => total + next.Max<UIElement>(item => item.DesiredSize.Height));
                double heightMultiply = finalSize.Height / actualDesiredHeight;
                foreach (List<UIElement> row in this.rows)
                {
                    double rowActualDesiredWidth = row.Aggregate<UIElement, double>(0.0, (total, next) => total + next.DesiredSize.Width);
                    arrangeX = 0.0;
                    if (rowActualDesiredWidth < finalSize.Width)
                    {
                        if (this.Align == TabStripAlign.Right)
                        {
                            arrangeX = finalSize.Width - rowActualDesiredWidth;
                        }
                        else if (this.Align == TabStripAlign.Center)
                        {
                            arrangeX = (finalSize.Width - rowActualDesiredWidth) / 2.0;
                        }
                    }
                    double widthMultiplier = 1.0;
                    if ((rowActualDesiredWidth > finalSize.Width) || (this.Align == TabStripAlign.Justify))
                    {
                        widthMultiplier = finalSize.Width / rowActualDesiredWidth;
                    }
                    double maxRowHeight = row.Max<UIElement>(o => o.DesiredSize.Height) * heightMultiply;
                    foreach (UIElement child in row)
                    {
                        if (this.AllTabsEqualHeight)
                        {
                            arrangeY = rowY;
                            arrangeHeight = maxRowHeight;
                        }
                        else
                        {
                            arrangeHeight = child.DesiredSize.Height * heightMultiply;
                            arrangeY = (this.TabStripPlacement == Dock.Top) ? ((rowY + maxRowHeight) - arrangeHeight) : rowY;
                        }
                        arrangeWidth = child.DesiredSize.Width * widthMultiplier;
                        arrangeHeight = double.IsNaN(arrangeHeight) ? 0.0 : arrangeHeight;
                        arrangeWidth = double.IsNaN(arrangeWidth) ? 0.0 : arrangeWidth;
                        child.Arrange(new Rect(arrangeX, arrangeY, arrangeWidth, arrangeHeight));
                        arrangeX += arrangeWidth;
                    }
                    rowY += maxRowHeight;
                }
            }
            else
            {
                double actualDesiredWidth = this.rows.Aggregate<List<UIElement>, double>(0.0, (total, next) => total + next.Max<UIElement>(item => item.DesiredSize.Width));
                double widthMultiply = finalSize.Width / actualDesiredWidth;
                foreach (List<UIElement> row in this.rows)
                {
                    double rowActualDesiredHeight = row.Aggregate<UIElement, double>(0.0, (total, next) => total + next.DesiredSize.Height);
                    arrangeY = 0.0;
                    if (rowActualDesiredHeight < finalSize.Height)
                    {
                        if (this.Align == TabStripAlign.Right)
                        {
                            arrangeY = finalSize.Height - rowActualDesiredHeight;
                        }
                        else if (this.Align == TabStripAlign.Center)
                        {
                            arrangeY = (finalSize.Height - rowActualDesiredHeight) / 2.0;
                        }
                    }
                    double heightMultiplier = 1.0;
                    if ((rowActualDesiredHeight > finalSize.Height) || (this.Align == TabStripAlign.Justify))
                    {
                        heightMultiplier = finalSize.Height / rowActualDesiredHeight;
                    }
                    double maxRowWidth = row.Max<UIElement>(o => o.DesiredSize.Width) * widthMultiply;
                    foreach (UIElement child in row)
                    {
                        if (this.AllTabsEqualHeight)
                        {
                            arrangeX = rowX;
                            arrangeWidth = maxRowWidth;
                        }
                        else
                        {
                            arrangeWidth = child.DesiredSize.Width * widthMultiply;
                            arrangeX = (this.TabStripPlacement == Dock.Left) ? ((rowX + maxRowWidth) - arrangeWidth) : rowX;
                        }
                        arrangeHeight = child.DesiredSize.Height * heightMultiplier;
                        child.Arrange(new Rect(arrangeX, arrangeY, arrangeWidth, arrangeHeight));
                        arrangeY += arrangeHeight;
                    }
                    rowX += maxRowWidth;
                }
            }
            this.rows = null;
            return finalSize;
        }

        internal void Initialize()
        {
            this.rows = new List<List<UIElement>>(2);
            this.rows.Add(new List<UIElement>(8));
            int currentRow = 0;
            foreach (UIElement child in base.Children)
            {
                this.rows[currentRow].Add(child);
                if ((child is RadTabItem) && (child as RadTabItem).IsBreak)
                {
                    currentRow++;
                    this.rows.Add(new List<UIElement>(8));
                }
            }
            if (this.rows.Last<List<UIElement>>().Count == 0)
            {
                this.rows.Remove(this.rows.Last<List<UIElement>>());
            }
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification="The method will be refactored.")]
        protected override Size MeasureOverride(Size availableSize)
        {
            double desiredWidth = 0.0;
            double desiredHeight = 0.0;
            if (base.Children.Count == 0)
            {
                return new Size(desiredWidth, desiredHeight);
            }
            this.Initialize();
            if ((this.TabStripPlacement == Dock.Top) || (this.TabStripPlacement == Dock.Bottom))
            {
                desiredHeight = this.rows.Aggregate<List<UIElement>, double>(0.0, (total, next) => total + next.Max<UIElement>(delegate (UIElement item) {
                    item.Measure(new Size(availableSize.Width, availableSize.Height));
                    return item.DesiredSize.Height;
                }));
                if (!double.IsPositiveInfinity(availableSize.Height))
                {
                    desiredHeight = Math.Min(desiredHeight, availableSize.Height);
                }
                List<double> wantedWidths = (from row in this.rows select row.Aggregate<UIElement, double>(0.0, delegate (double total, UIElement next) {
                    next.Measure(new Size(availableSize.Width, availableSize.Height));
                    return total + next.DesiredSize.Width;
                })).ToList<double>();
                desiredWidth = ((IEnumerable<double>) wantedWidths).Max();
                for (int i = 0; i < this.rows.Count; i++)
                {
                    if (wantedWidths[i] > availableSize.Width)
                    {
                        double shrinkCoef = availableSize.Width / wantedWidths[i];
                        this.rows[i].ForEach(delegate (UIElement el) {
                            el.Measure(new Size(el.DesiredSize.Width * shrinkCoef, availableSize.Height));
                        });
                    }
                }
            }
            else
            {
                desiredWidth = this.rows.Aggregate<List<UIElement>, double>(0.0, (total, next) => total + next.Max<UIElement>(delegate (UIElement item) {
                    item.Measure(new Size(double.PositiveInfinity, availableSize.Height));
                    return item.DesiredSize.Width;
                }));
                if (!double.IsPositiveInfinity(availableSize.Width))
                {
                    desiredWidth = Math.Min(desiredWidth, availableSize.Width);
                }
                List<double> wantedheights = (from row in this.rows select row.Aggregate<UIElement, double>(0.0, delegate (double total, UIElement next) {
                    next.Measure(new Size(double.PositiveInfinity, availableSize.Height));
                    return total + next.DesiredSize.Height;
                })).ToList<double>();
                desiredHeight = ((IEnumerable<double>) wantedheights).Max();
                for (int i = 0; i < this.rows.Count; i++)
                {
                    if (wantedheights[i] > availableSize.Height)
                    {
                        double shrinkCoef = availableSize.Height / wantedheights[i];
                        this.rows[i].ForEach(delegate (UIElement el) {
                            el.Measure(new Size(availableSize.Width, el.DesiredSize.Height * shrinkCoef));
                        });
                    }
                }
            }
            this.rows = null;
            return new Size(Math.Min(availableSize.Width, desiredWidth), Math.Min(availableSize.Height, desiredHeight));
        }

        private static void OnPanelPropertyChnaged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            TabStripPanel tabStrip = sender as TabStripPanel;
            tabStrip.InvalidateMeasure();
            tabStrip.InvalidateArrange();
        }

        internal void RearrangeForSelectedItem()
        {
            if (this.RearrangeTabs && this.rows.Any<List<UIElement>>())
            {
                List<UIElement> selectedRow = this.rows.FirstOrDefault<List<UIElement>>(row => row.OfType<RadTabItem>().Any<RadTabItem>(tab => tab.IsSelected));
                this.rows.Remove(selectedRow);
                this.rows.Add(selectedRow);
            }
            this.Rows = (from r in this.Rows
                where r != null
                select r).ToList<List<UIElement>>();
        }

        public TabStripAlign Align
        {
            get
            {
                return (TabStripAlign) base.GetValue(AlignProperty);
            }
            set
            {
                base.SetValue(AlignProperty, value);
            }
        }

        public bool AllTabsEqualHeight
        {
            get
            {
                return (bool) base.GetValue(AllTabsEqualHeightProperty);
            }
            set
            {
                base.SetValue(AllTabsEqualHeightProperty, value);
            }
        }

        public bool RearrangeTabs
        {
            get
            {
                return (bool) base.GetValue(RearrangeTabsProperty);
            }
            set
            {
                base.SetValue(RearrangeTabsProperty, value);
            }
        }

        internal IList<List<UIElement>> Rows
        {
            get
            {
                return this.rows;
            }
            set
            {
                this.rows = value;
            }
        }

        public Dock TabStripPlacement
        {
            get
            {
                return (Dock) base.GetValue(TabStripPlacementProperty);
            }
            set
            {
                base.SetValue(TabStripPlacementProperty, value);
            }
        }
    }
}

