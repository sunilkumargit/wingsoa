namespace Telerik.Windows.Controls.Primitives
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using Telerik.Windows.Controls;
    using Telerik.Windows.Controls.TabControl;

    public class TabWrapPanel : TabStripPanel
    {
        protected override Size ArrangeOverride(Size finalSize)
        {
            double arrangeX = 0.0;
            double arrangeY = 0.0;
            double arrangeWidth = 0.0;
            double arrangeHeight = 0.0;
            double rowY = 0.0;
            if (base.Children.Count == 0)
            {
                return base.ArrangeOverride(finalSize);
            }
            base.RearrangeForSelectedItem();
            double actualDesiredHeight = base.Rows.Aggregate<List<UIElement>, double>(0.0, (total, next) => total + next.Max<UIElement>(item => item.DesiredSize.Height));
            double heightMultiply = finalSize.Height / actualDesiredHeight;
            TabStripAlign alignOverride = this.CalculateAlignForRotation();
            foreach (List<UIElement> row in base.Rows)
            {
                double rowActualDesiredWidth = row.Aggregate<UIElement, double>(0.0, (total, next) => total + next.DesiredSize.Width);
                arrangeX = 0.0;
                if (rowActualDesiredWidth < finalSize.Width)
                {
                    switch (alignOverride)
                    {
                        case TabStripAlign.Right:
                            arrangeX = finalSize.Width - rowActualDesiredWidth;
                            break;

                        case TabStripAlign.Center:
                            arrangeX = (finalSize.Width - rowActualDesiredWidth) / 2.0;
                            break;
                    }
                }
                double widthMultiplier = 1.0;
                if ((rowActualDesiredWidth > finalSize.Width) || (alignOverride == TabStripAlign.Justify))
                {
                    widthMultiplier = finalSize.Width / rowActualDesiredWidth;
                }
                double maxRowHeight = row.Max<UIElement>(o => o.DesiredSize.Height) * heightMultiply;
                foreach (UIElement child in row)
                {
                    if (base.AllTabsEqualHeight)
                    {
                        arrangeY = rowY;
                        arrangeHeight = maxRowHeight;
                    }
                    else
                    {
                        arrangeHeight = child.DesiredSize.Height * heightMultiply;
                        arrangeY = (base.TabStripPlacement == Dock.Top) ? ((rowY + maxRowHeight) - arrangeHeight) : rowY;
                    }
                    arrangeWidth = child.DesiredSize.Width * widthMultiplier;
                    arrangeHeight = double.IsNaN(arrangeHeight) ? 0.0 : arrangeHeight;
                    arrangeWidth = double.IsNaN(arrangeWidth) ? 0.0 : arrangeWidth;
                    child.Arrange(new Rect(arrangeX, arrangeY, arrangeWidth, arrangeHeight));
                    arrangeX += arrangeWidth;
                }
                rowY += maxRowHeight;
            }
            return finalSize;
        }

        private TabStripAlign CalculateAlignForRotation()
        {
            TabStripAlign alignOverride = base.Align;
            if ((base.TabStripPlacement == Dock.Bottom) || (base.TabStripPlacement == Dock.Right))
            {
                switch (alignOverride)
                {
                    case TabStripAlign.Left:
                        return TabStripAlign.Right;

                    case TabStripAlign.Right:
                        return TabStripAlign.Left;
                }
            }
            return alignOverride;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            double desiredWidth = 0.0;
            double desiredHeight = 0.0;
            double availableWidth = availableSize.Width;
            if (base.Children.Count == 0)
            {
                return new Size(desiredWidth, desiredHeight);
            }
            base.Initialize();
            this.ReverseItemOrderForRotaion();
            foreach (UIElement child in base.Children)
            {
                child.Measure(availableSize);
            }
            base.Rows = base.Rows.SelectMany<List<UIElement>, List<UIElement>>(delegate (List<UIElement> row) {
                if (double.IsInfinity(availableWidth))
                {
                    return new List<UIElement>[] { row };
                }
                double remainingWidth = availableWidth;
                IEnumerable<UIElement> remainingTabs = row;
                List<List<UIElement>> result = new List<List<UIElement>>();
                while (remainingTabs.Any<UIElement>())
                {
                    List<UIElement> split = remainingTabs.TakeWhile<UIElement>(tab => ((remainingWidth -= tab.DesiredSize.Width) > 0.0)).ToList<UIElement>();
                    if (!split.Any<UIElement>())
                    {
                        split = remainingTabs.Take<UIElement>(1).ToList<UIElement>();
                    }
                    remainingTabs = remainingTabs.Skip<UIElement>(split.Count);
                    remainingWidth = availableWidth;
                    result.Add(split);
                }
                return result;
            }).ToList<List<UIElement>>();
            desiredHeight = base.Rows.Aggregate<List<UIElement>, double>(0.0, (total, next) => total + next.Max<UIElement>(delegate (UIElement item) {
                item.Measure(new Size(availableSize.Width, availableSize.Height));
                return item.DesiredSize.Height;
            }));
            if (!double.IsPositiveInfinity(availableSize.Height))
            {
                desiredHeight = Math.Min(desiredHeight, availableSize.Height);
            }
            return new Size(((IEnumerable<double>) (from row in base.Rows select row.Aggregate<UIElement, double>(0.0, (total, next) => total + next.DesiredSize.Width))).Max(), desiredHeight);
        }

        private void ReverseItemOrderForRotaion()
        {
            if ((base.TabStripPlacement == Dock.Bottom) || (base.TabStripPlacement == Dock.Left))
            {
                foreach (List<UIElement> row in base.Rows)
                {
                    row.Reverse();
                }
            }
        }
    }
}

