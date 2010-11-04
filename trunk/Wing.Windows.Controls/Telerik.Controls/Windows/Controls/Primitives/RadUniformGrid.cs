namespace Telerik.Windows.Controls.Primitives
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using Telerik.Windows;

    [DefaultProperty("ChildrenFlow")]
    public class RadUniformGrid : Panel
    {
        public static readonly DependencyProperty ChildrenFlowProperty = DependencyPropertyExtensions.Register("ChildrenFlow", typeof(Orientation), typeof(RadUniformGrid), new Telerik.Windows.PropertyMetadata(Orientation.Horizontal, new PropertyChangedCallback(RadUniformGrid.OnOrientationChanged), null));
        private int columns;
        public static readonly DependencyProperty ColumnsProperty = DependencyPropertyExtensions.Register("Columns", typeof(int), typeof(RadUniformGrid), new Telerik.Windows.PropertyMetadata(0, new PropertyChangedCallback(RadUniformGrid.OnPropertyChanged), null));
        public static readonly DependencyProperty FirstColumnProperty = DependencyPropertyExtensions.Register("FirstColumn", typeof(int), typeof(RadUniformGrid), new Telerik.Windows.PropertyMetadata(0, new PropertyChangedCallback(RadUniformGrid.OnPropertyChanged), null));
        public static readonly DependencyProperty HideFirstColumnProperty = DependencyProperty.Register("HideFirstColumn", typeof(bool), typeof(RadUniformGrid), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadUniformGrid.OnHideChanged)));
        public static readonly DependencyProperty HideFirstRowProperty = DependencyProperty.Register("HideFirstRow", typeof(bool), typeof(RadUniformGrid), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadUniformGrid.OnHideChanged)));
        private static readonly DependencyProperty PositionInfoProperty = DependencyProperty.RegisterAttached("PositionInfo", typeof(PositionInfo), typeof(RadUniformGrid), null);
        public static readonly DependencyProperty PreserveSpaceForCollapsedChildrenProperty = DependencyPropertyExtensions.Register("PreserveSpaceForCollapsedChildren", typeof(bool), typeof(RadUniformGrid), new Telerik.Windows.PropertyMetadata(false, null, null));
        private int rows;
        public static readonly DependencyProperty RowsProperty = DependencyProperty.Register("Rows", typeof(int), typeof(RadUniformGrid), new Telerik.Windows.PropertyMetadata(0, new PropertyChangedCallback(RadUniformGrid.OnPropertyChanged), null));

        public RadUniformGrid()
        {
            
            this.ChildrenFlow = Orientation.Horizontal;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            bool useRounding = base.UseLayoutRounding;
            UIElementCollection collection = base.Children;
            int firstColumn = this.HideFirstColumn ? 1 : 0;
            int firstRow = this.HideFirstRow ? 1 : 0;
            int columnCount = (this.CalculatedColumns != 0) ? this.CalculatedColumns : 1;
            int rowCount = (this.CalculatedRows != 0) ? this.CalculatedRows : 1;
            double width = finalSize.Width / ((double) columnCount);
            double height = finalSize.Height / ((double) rowCount);
            int horizontalExtra = 0;
            int verticalExtra = 0;
            Rect currentRect = new Rect();
            if (useRounding)
            {
                width = Math.Floor(width);
                height = Math.Floor(height);
                horizontalExtra = ((int) finalSize.Width) - (((int) width) * columnCount);
                verticalExtra = ((int) finalSize.Height) - (((int) height) * rowCount);
                horizontalExtra = CountMissingExtra(horizontalExtra, columnCount);
                verticalExtra = CountMissingExtra(verticalExtra, rowCount);
                int[] columnX = new int[columnCount];
                int[] columnWidth = new int[columnCount];
                int[] rowY = new int[rowCount];
                int[] rowHeight = new int[rowCount];
                int horizontalSignificientBits = (int) Math.Ceiling(Math.Log((double) columnCount, 2.0));
                int verticalSignificientBits = (int) Math.Ceiling(Math.Log((double) rowCount, 2.0));
                int currentColumnX = 0;
                int currentRowY = 0;
                int currentColumnWidth = 0;
                int currentRowHeight = 0;
                for (int i = 0; i < columnCount; i++)
                {
                    columnX[i] = currentColumnX;
                    currentColumnWidth = ((int) width) + ((horizontalExtra > FlipInt(i, horizontalSignificientBits)) ? 1 : 0);
                    columnWidth[i] = currentColumnWidth;
                    currentColumnX += currentColumnWidth;
                }
                for (int i = 0; i < rowCount; i++)
                {
                    rowY[i] = currentRowY;
                    currentRowHeight = ((int) height) + ((verticalExtra > FlipInt(i, verticalSignificientBits)) ? 1 : 0);
                    rowHeight[i] = currentRowHeight;
                    currentRowY += currentRowHeight;
                }
                foreach (UIElement element in collection)
                {
                    PositionInfo pi = GetPositionInfo(element);
                    if (((pi.Column < firstColumn) || (pi.Row < firstRow)) || (((pi.Row - firstRow) >= rowCount) || ((pi.Column - firstColumn) >= columnCount)))
                    {
                        currentRect.Width = 0.0;
                        currentRect.Height = 0.0;
                        currentRect.X = double.MinValue;
                        currentRect.Y = double.MinValue;
                        element.Arrange(currentRect);
                        continue;
                    }
                    if ((element.Visibility != Visibility.Collapsed) || this.PreserveSpaceForCollapsedChildren)
                    {
                        currentRect.Width = columnWidth[pi.Column - firstColumn];
                        currentRect.Height = rowHeight[pi.Row - firstRow];
                        currentRect.X = columnX[pi.Column - firstColumn];
                        currentRect.Y = rowY[pi.Row - firstRow];
                        element.Arrange(currentRect);
                    }
                }
                return finalSize;
            }
            foreach (UIElement element in collection)
            {
                PositionInfo pi = GetPositionInfo(element);
                currentRect.Width = width;
                currentRect.Height = height;
                if ((pi.Column < firstColumn) || (pi.Row < firstRow))
                {
                    currentRect.X = double.MinValue;
                    currentRect.Y = double.MinValue;
                    element.Arrange(currentRect);
                    continue;
                }
                if ((element.Visibility != Visibility.Collapsed) || this.PreserveSpaceForCollapsedChildren)
                {
                    currentRect.X = ((pi.Column - firstColumn) * width) + Math.Min(horizontalExtra, pi.Column - firstColumn);
                    currentRect.Y = ((pi.Row - firstRow) * height) + Math.Min(verticalExtra, pi.Row - firstRow);
                    element.Arrange(currentRect);
                }
            }
            return finalSize;
        }

        private static int CountMissingExtra(int extra, int cells)
        {
            if (cells <= 0)
            {
                return extra;
            }
            int significientBits = (int) Math.Ceiling(Math.Log((double) cells, 2.0));
            int protoExtraPixels = extra;
            int nextPow = ((int) Math.Pow((double) significientBits, 2.0)) - 1;
            for (int extraToPow = cells; extraToPow < nextPow; extraToPow++)
            {
                if (FlipInt(extraToPow, significientBits) < extra)
                {
                    protoExtraPixels++;
                }
            }
            return protoExtraPixels;
        }

        private static int FlipInt(int number, int bitCount)
        {
            int r = 0;
            for (int c = 0; c < bitCount; c++)
            {
                r = r << 1;
                r |= number & 1;
                number = number >> 1;
            }
            return r;
        }

        private static PositionInfo GetPositionInfo(DependencyObject obj)
        {
            return (PositionInfo) obj.GetValue(PositionInfoProperty);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            this.UpdateComputedValues();
            Size calculatedSize = new Size(availableSize.Width / ((double) this.CalculatedColumns), availableSize.Height / ((double) this.CalculatedRows));
            if (base.UseLayoutRounding)
            {
                calculatedSize.Width = Math.Floor(calculatedSize.Width);
                calculatedSize.Height = Math.Floor(calculatedSize.Height);
            }
            double width = 0.0;
            double height = 0.0;
            int start = 0;
            int row = 0;
            int column = 0;
            bool horizontal = this.ChildrenFlow == Orientation.Horizontal;
            if (horizontal)
            {
                column = this.CalculatedFirstColumn;
            }
            else
            {
                row = this.CalculatedFirstColumn;
            }
            int count = base.Children.Count;
            while (start < count)
            {
                UIElement element = base.Children[start];
                PositionInfo pi = GetPositionInfo(element);
                if (pi == null)
                {
                    pi = new PositionInfo();
                    SetPositionInfo(element, pi);
                }
                pi.Column = column;
                pi.Row = row;
                if (horizontal)
                {
                    column++;
                    if (column >= this.columns)
                    {
                        column = 0;
                        row++;
                    }
                }
                else
                {
                    row++;
                    if (row >= this.rows)
                    {
                        row = 0;
                        column++;
                    }
                }
                element.Measure(calculatedSize);
                Size desiredSize = element.DesiredSize;
                if (width < desiredSize.Width)
                {
                    width = desiredSize.Width;
                }
                if (height < desiredSize.Height)
                {
                    height = desiredSize.Height;
                }
                if (base.UseLayoutRounding)
                {
                    width = Math.Ceiling(width);
                    height = Math.Ceiling(height);
                }
                start++;
            }
            return new Size(width * this.CalculatedColumns, height * this.CalculatedRows);
        }

        private static void OnHideChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadUniformGrid ug = d as RadUniformGrid;
            if (ug != null)
            {
                ug.InvalidateMeasure();
                ug.InvalidateArrange();
            }
        }

        private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Orientation orientation = (Orientation) e.NewValue;
            if ((orientation != Orientation.Horizontal) && (orientation != Orientation.Vertical))
            {
                throw new ArgumentOutOfRangeException("e");
            }
            RadUniformGrid ug = d as RadUniformGrid;
            if (ug != null)
            {
                ug.InvalidateMeasure();
                ug.InvalidateArrange();
            }
        }

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!ValidateValue(e.NewValue))
            {
                throw new ArgumentOutOfRangeException(e.Property.ToString());
            }
            RadUniformGrid ug = d as RadUniformGrid;
            if (ug != null)
            {
                ug.InvalidateMeasure();
                ug.InvalidateArrange();
            }
        }

        private static void SetPositionInfo(DependencyObject obj, PositionInfo value)
        {
            obj.SetValue(PositionInfoProperty, value);
        }

        private void UpdateComputedValues()
        {
            this.columns = this.Columns;
            this.rows = this.Rows;
            if ((this.rows <= 0) || (this.columns <= 0))
            {
                int num = 0;
                int num2 = 0;
                int count = base.Children.Count;
                while (num2 < count)
                {
                    UIElement element = base.Children[num2];
                    if ((element.Visibility != Visibility.Collapsed) || this.PreserveSpaceForCollapsedChildren)
                    {
                        num++;
                    }
                    num2++;
                }
                if (num == 0)
                {
                    num = 1;
                }
                if (this.rows <= 0)
                {
                    if (this.columns > 0)
                    {
                        this.rows = ((num + this.CalculatedFirstColumn) + (this.columns - 1)) / this.columns;
                    }
                    else
                    {
                        this.rows = (int) Math.Sqrt((double) num);
                        if ((this.rows * this.rows) < num)
                        {
                            this.rows++;
                        }
                        this.columns = this.rows;
                    }
                }
                else if (this.columns <= 0)
                {
                    this.columns = (num + (this.rows - 1)) / this.rows;
                }
            }
        }

        private static bool ValidateValue(object o)
        {
            return (((int) o) >= 0);
        }

        private int CalculatedColumns
        {
            get
            {
                if (!this.HideFirstColumn)
                {
                    return this.columns;
                }
                int localColumns = this.columns - 1;
                if (localColumns < 0)
                {
                    return 0;
                }
                return localColumns;
            }
        }

        private int CalculatedFirstColumn
        {
            get
            {
                int current = (this.ChildrenFlow == Orientation.Horizontal) ? this.columns : this.rows;
                if (this.FirstColumn >= current)
                {
                    return 0;
                }
                return this.FirstColumn;
            }
        }

        private int CalculatedRows
        {
            get
            {
                if (!this.HideFirstRow)
                {
                    return this.rows;
                }
                int localRows = this.rows - 1;
                if (localRows < 0)
                {
                    return 0;
                }
                return localRows;
            }
        }

        public Orientation ChildrenFlow
        {
            get
            {
                return (Orientation) base.GetValue(ChildrenFlowProperty);
            }
            set
            {
                base.SetValue(ChildrenFlowProperty, value);
            }
        }

        public int Columns
        {
            get
            {
                return (int) base.GetValue(ColumnsProperty);
            }
            set
            {
                base.SetValue(ColumnsProperty, value);
            }
        }

        public int FirstColumn
        {
            get
            {
                return (int) base.GetValue(FirstColumnProperty);
            }
            set
            {
                base.SetValue(FirstColumnProperty, value);
            }
        }

        public bool HideFirstColumn
        {
            get
            {
                return (bool) base.GetValue(HideFirstColumnProperty);
            }
            set
            {
                base.SetValue(HideFirstColumnProperty, value);
            }
        }

        public bool HideFirstRow
        {
            get
            {
                return (bool) base.GetValue(HideFirstRowProperty);
            }
            set
            {
                base.SetValue(HideFirstRowProperty, value);
            }
        }

        public bool PreserveSpaceForCollapsedChildren
        {
            get
            {
                return (bool) base.GetValue(PreserveSpaceForCollapsedChildrenProperty);
            }
            set
            {
                base.SetValue(PreserveSpaceForCollapsedChildrenProperty, value);
            }
        }

        public int Rows
        {
            get
            {
                return (int) base.GetValue(RowsProperty);
            }
            set
            {
                base.SetValue(RowsProperty, value);
            }
        }
    }
}

