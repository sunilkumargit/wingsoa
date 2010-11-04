using Wing.Windows.Controls.Telerik.Navigation.Windows.Controls;
namespace Telerik.Windows.Controls
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Media.Imaging;
    using Telerik.Windows;
    using Telerik.Windows.Controls.Animation;
    using Telerik.Windows.Controls.Primitives;
    using Telerik.Windows.Controls.TileView;

    [DefaultProperty("Items"), TemplateVisualState(GroupName = "CommonStates", Name = "MouseOver"), TemplateVisualState(GroupName = "CommonStates", Name = "Normal"), ScriptableType, StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(RadTileViewItem)), TemplateVisualState(GroupName = "CommonStates", Name = "Disabled"), DefaultEvent("TileStateChanged")]
    public class RadTileView : Telerik.Windows.Controls.ItemsControl
    {
        private int columns = 1;
        public static readonly DependencyProperty ContentTemplateProperty = DependencyProperty.Register("ContentTemplate", typeof(DataTemplate), typeof(RadTileView), null);
        public static readonly DependencyProperty ContentTemplateSelectorProperty = DependencyProperty.Register("ContentTemplateSelector", typeof(DataTemplateSelector), typeof(RadTileView), null);
        private Point dragStartPosition;
        private Point dragStartPositionRelativeOffset;
        private Point dragStartPositionRelativeOffsetComplement;
        public static readonly DependencyProperty IsAnimationOptimizedProperty = DependencyProperty.Register("IsAnimationOptimized", typeof(bool), typeof(RadTileView), new System.Windows.PropertyMetadata(new PropertyChangedCallback(RadTileView.OnIsAnimationOptimizedPropertyChanged)));
        public static readonly DependencyProperty IsItemDraggingEnabledProperty = DependencyProperty.Register("IsItemDraggingEnabled", typeof(bool), typeof(RadTileView), new Telerik.Windows.PropertyMetadata(true));
        public static readonly DependencyProperty IsItemsAnimationEnabledProperty = DependencyProperty.Register("IsItemsAnimationEnabled", typeof(bool), typeof(RadTileView), new Telerik.Windows.PropertyMetadata(true, new PropertyChangedCallback(RadTileView.OnIsItemsAnimationEnabledChanged)));
        public static readonly DependencyProperty MaxColumnsProperty = DependencyProperty.Register("MaxColumns", typeof(int), typeof(RadTileView), new Telerik.Windows.PropertyMetadata(0x7fffffff, new PropertyChangedCallback(RadTileView.OnMaxColumnsChanged)));
        public static readonly DependencyProperty MaximizedItemProperty = DependencyProperty.Register("MaximizedItem", typeof(object), typeof(RadTileView), new Telerik.Windows.PropertyMetadata(null, new PropertyChangedCallback(RadTileView.OnMaximizedItemChanged)));
        public static readonly DependencyProperty MaximizeModeProperty = DependencyProperty.Register("MaximizeMode", typeof(TileViewMaximizeMode), typeof(RadTileView), new Telerik.Windows.PropertyMetadata(TileViewMaximizeMode.ZeroOrOne));
        public static readonly DependencyProperty MaxRowsProperty = DependencyProperty.Register("MaxRows", typeof(int), typeof(RadTileView), new Telerik.Windows.PropertyMetadata(0x7fffffff, new PropertyChangedCallback(RadTileView.OnMaxRowsChanged)));
        public static readonly DependencyProperty MinimizedColumnWidthProperty = DependencyProperty.Register("MinimizedColumnWidth", typeof(double), typeof(RadTileView), new Telerik.Windows.PropertyMetadata(250.0, new PropertyChangedCallback(RadTileView.OnMinimizedColumnWidthChanged)));
        public static readonly DependencyProperty MinimizedItemsPositionProperty = DependencyProperty.Register("MinimizedItemsPosition", typeof(Dock), typeof(RadTileView), new Telerik.Windows.PropertyMetadata(Dock.Right, new PropertyChangedCallback(RadTileView.OnMinimizedItemsPositionChanged)));
        public static readonly DependencyProperty MinimizedRowHeightProperty = DependencyProperty.Register("MinimizedRowHeight", typeof(double), typeof(RadTileView), new Telerik.Windows.PropertyMetadata(75.0, new PropertyChangedCallback(RadTileView.OnMinimizedRowHeightChanged)));
        public static readonly Telerik.Windows.RoutedEvent PreviewTileDragStartedEvent = EventManager.RegisterRoutedEvent("PreviewTileDragStarted", RoutingStrategy.Tunnel, typeof(EventHandler<TileViewDragEventArgs>), typeof(RadTileView));
        public static readonly DependencyProperty ReorderingDurationProperty = DependencyProperty.Register("ReorderingDuration", typeof(Duration), typeof(RadTileView), new Telerik.Windows.PropertyMetadata(new Duration(TimeSpan.FromMilliseconds(330.0))));
        public static readonly DependencyProperty ReorderingEasingProperty = DependencyProperty.Register("ReorderingEasing", typeof(IEasingFunction), typeof(RadTileView), null);
        public static readonly DependencyProperty ResizingDurationProperty = DependencyProperty.Register("ResizingDuration", typeof(Duration), typeof(RadTileView), new Telerik.Windows.PropertyMetadata(new Duration(TimeSpan.FromMilliseconds(330.0))));
        public static readonly DependencyProperty ResizingEasingProperty = DependencyProperty.Register("ResizingEasing", typeof(IEasingFunction), typeof(RadTileView), null);
        private int rows = 1;
        private System.Windows.Controls.Primitives.ScrollBar scrollBar;
        public static readonly DependencyProperty ScrollBarVisibilityProperty = DependencyProperty.Register("ScrollBarVisibility", typeof(Telerik.Windows.Controls.TileView.ScrollBarVisibility), typeof(RadTileView), new System.Windows.PropertyMetadata(new PropertyChangedCallback(RadTileView.OnScrollBarVisibilityPropertyChanged)));
        public static readonly Telerik.Windows.RoutedEvent TileDragEndedEvent = EventManager.RegisterRoutedEvent("TileDragEnded", RoutingStrategy.Bubble, typeof(EventHandler<TileViewDragEventArgs>), typeof(RadTileView));
        public static readonly Telerik.Windows.RoutedEvent TileDragStartedEvent = EventManager.RegisterRoutedEvent("TileDragStarted", RoutingStrategy.Bubble, typeof(EventHandler<TileViewDragEventArgs>), typeof(RadTileView));
        public static readonly Telerik.Windows.RoutedEvent TilesStateChangedEvent = EventManager.RegisterRoutedEvent("TilesStateChanged", RoutingStrategy.Bubble, typeof(EventHandler<RadRoutedEventArgs>), typeof(RadTileView));
        public static readonly DependencyProperty TileStateChangeTriggerProperty = DependencyProperty.Register("TileStateChangeTrigger", typeof(Telerik.Windows.Controls.TileStateChangeTrigger), typeof(RadTileView), new Telerik.Windows.PropertyMetadata(Telerik.Windows.Controls.TileStateChangeTrigger.DoubleClick));

        public event EventHandler LayoutChangeEnded;

        public event EventHandler LayoutChangeStarted;

        public event EventHandler<TileViewDragEventArgs> PreviewTileDragStarted
        {
            add
            {
                this.AddHandler(PreviewTileDragStartedEvent, value, false);
            }
            remove
            {
                this.RemoveHandler(PreviewTileDragStartedEvent, value);
            }
        }

        public event EventHandler<PreviewTileStateChangedEventArgs> PreviewTileStateChanged
        {
            add
            {
                this.AddHandler(RadTileViewItem.PreviewTileStateChangedEvent, value, false);
            }
            remove
            {
                this.RemoveHandler(RadTileViewItem.PreviewTileStateChangedEvent, value);
            }
        }

        public event EventHandler<TileViewDragEventArgs> TileDragEnded
        {
            add
            {
                this.AddHandler(TileDragEndedEvent, value, false);
            }
            remove
            {
                this.RemoveHandler(TileDragEndedEvent, value);
            }
        }

        public event EventHandler<TileViewDragEventArgs> TileDragStarted
        {
            add
            {
                this.AddHandler(TileDragStartedEvent, value, false);
            }
            remove
            {
                this.RemoveHandler(TileDragStartedEvent, value);
            }
        }

        public event EventHandler<RadRoutedEventArgs> TilePositionChanged
        {
            add
            {
                this.AddHandler(RadTileViewItem.PositionChangedEvent, value, false);
            }
            remove
            {
                this.RemoveHandler(RadTileViewItem.PositionChangedEvent, value);
            }
        }

        public event EventHandler<RadRoutedEventArgs> TilesStateChanged
        {
            add
            {
                this.AddHandler(TilesStateChangedEvent, value);
            }
            remove
            {
                this.RemoveHandler(TilesStateChangedEvent, value);
            }
        }

        public event EventHandler<RadRoutedEventArgs> TileStateChanged
        {
            add
            {
                this.AddHandler(RadTileViewItem.TileStateChangedEvent, value, false);
            }
            remove
            {
                this.RemoveHandler(RadTileViewItem.TileStateChangedEvent, value);
            }
        }

        public RadTileView()
        {
            base.DefaultStyleKey = typeof(RadTileView);
            base.SizeChanged += new SizeChangedEventHandler(this.RadTileView_SizeChanged);
            this.PreviewTileStateChanged += new EventHandler<PreviewTileStateChangedEventArgs>(this.RadTileView_PreviewTileStateChanged);
            
            base.ItemContainerGenerator.StatusChanged -= new EventHandler(this.ItemContainerGenerator_StatusChanged);
            base.ItemContainerGenerator.StatusChanged += new EventHandler(this.ItemContainerGenerator_StatusChanged);
        }

        private void AnimateItemPosition(RadTileViewItem item, Point targetPosition)
        {
            Point currentPosition = GetCurrentItemPosition(item);
            double changeX = targetPosition.X - currentPosition.X;
            double changeY = targetPosition.Y - currentPosition.Y;
            if (item.PositionAnimation != null)
            {
                item.PositionAnimation.Stop();
                item.PositionAnimation = null;
            }
            if (this.MaximizedItem == item)
            {
                int currentZIndex = RadTileViewItem.CurrentZIndex;
                RadTileViewItem.CurrentZIndex = currentZIndex + 1;
                Canvas.SetZIndex(item, currentZIndex);
            }
            double endKeyTime = this.ReorderingDuration.TimeSpan.TotalSeconds;
            double[] xCoord = new double[4];
            xCoord[1] = -changeX;
            xCoord[2] = endKeyTime;
            double[] yCoord = new double[4];
            yCoord[1] = -changeY;
            yCoord[2] = endKeyTime;
            Storyboard sb = AnimationExtensions.Create().Animate(new FrameworkElement[] { item })
                .EnsureDefaultTransforms()
                .MoveX(xCoord)
                .MoveY(yCoord)
                .Origin(0.0, 0.0)
                .EaseAll((this.ReorderingEasing ?? Easings.SlideDown1))
                .AdjustSpeed()
                .Instance;
            sb.Completed += delegate(object sender, EventArgs e)
            {
                sb.Stop();
                item.PositionAnimation = null;
                this.OnLayoutChangeEnded();
            };
            SetItemPosition(item, targetPosition);
            item.PositionAnimation = sb;
            this.OnLayoutChangeStarted();
            sb.Begin();
        }

        private void AnimateItemSize(RadTileViewItem item, Size targetSize)
        {
            if (item.SizeAnimation != null)
            {
                item.SizeAnimation.Stop();
                item.SizeAnimation = null;
            }
            Size currentSize = new Size(item.ActualWidth, item.ActualHeight);
            item.Height = targetSize.Height;
            item.Width = targetSize.Width;
            bool shouldOptimize = false;
            if (this.IsAnimationOptimized)
            {
                shouldOptimize = (item.OldState == TileViewItemState.Restored) && (item.TileState == TileViewItemState.Maximized);
                if (!shouldOptimize)
                {
                    shouldOptimize = (item.OldState == TileViewItemState.Minimized) && (item.TileState == TileViewItemState.Maximized);
                }
            }
            if (shouldOptimize)
            {
                Panel root = VisualTreeHelper.GetChild(item, 0) as Panel;
                if (root != null)
                {
                    ContentPresenter contentElement = root.FindName("ContentElement") as ContentPresenter;
                    if (contentElement != null)
                    {
                        if (item.TileState == TileViewItemState.Maximized)
                        {
                            contentElement.Width = targetSize.Width;
                            contentElement.Height = targetSize.Height;
                            contentElement.UpdateLayout();
                        }
                        WriteableBitmap bitmap = new WriteableBitmap(contentElement, null);
                        Image img = new Image
                        {
                            Stretch = Stretch.None,
                            Source = bitmap
                        };
                        contentElement.Visibility = Visibility.Collapsed;
                        Panel cache = root.FindName("ContentCacheHost") as Panel;
                        if (cache != null)
                        {
                            cache.Children.Clear();
                            cache.Children.Add(img);
                        }
                    }
                }
            }
            double endKeyTime = this.ResizingDuration.TimeSpan.TotalSeconds;
            double[] hCoord = new double[4];
            hCoord[1] = currentSize.Height;
            hCoord[2] = endKeyTime;
            hCoord[3] = targetSize.Height;
            double[] wCoord = new double[4];
            wCoord[1] = currentSize.Width;
            wCoord[2] = endKeyTime;
            wCoord[3] = targetSize.Width;
            Storyboard sb = AnimationExtensions.Create().Animate(new FrameworkElement[] { item })
                .EnsureDefaultTransforms()
                .Height(hCoord)
                .Width(wCoord)
                .EaseAll((this.ResizingEasing ?? Easings.SlideDown1))
                .AdjustSpeed()
                .Instance;

            sb.Completed += delegate(object sender, EventArgs e)
            {
                sb.Stop();
                item.SizeAnimation = null;
                if (shouldOptimize)
                {
                    Panel root = VisualTreeHelper.GetChild(item, 0) as Panel;
                    if (root != null)
                    {
                        ContentPresenter contentElement = root.FindName("ContentElement") as ContentPresenter;
                        if (contentElement != null)
                        {
                            contentElement.Width = double.NaN;
                            contentElement.Height = double.NaN;
                            contentElement.Visibility = Visibility.Visible;
                        }
                        Panel cache = root.FindName("ContentCacheHost") as Panel;
                        if (cache != null)
                        {
                            cache.Children.Clear();
                        }
                    }
                }
                this.OnLayoutChangeEnded();
            };
            item.SizeAnimation = sb;
            this.OnLayoutChangeStarted();
            sb.Begin();
        }

        private void AnimateItemSizes()
        {
            if (this.ShouldAnimateItemSizes())
            {
                foreach (RadTileViewItem container in this.GetGeneratedItemContainers())
                {
                    this.AnimateItemSize(container, this.GetItemSize(container));
                }
            }
        }

        private void AnimateItemsLayout()
        {
            if (this.IsVisualUpdateNeccessary())
            {
                IEnumerable<RadTileViewItem> containers = this.GetGeneratedItemContainers();
                if (this.MaximizedItem == null)
                {
                    this.AnimateRestoredItemsPositions(containers);
                }
                else
                {
                    this.AnimateItemsWhenThereIsMaximized();
                }
            }
        }

        private void AnimateItemsWhenThereIsMaximized()
        {
            Dictionary<int, RadTileViewItem> orderedContainers = this.GetOrderedContainers();
            double currentOffset = 0.0;
            if (this.scrollBar != null)
            {
                currentOffset = -1.0 * this.scrollBar.Value;
            }
            for (int i = 0; i < orderedContainers.Count; i++)
            {
                RadTileViewItem maxedItem = base.ItemContainerGenerator.ContainerFromItem(this.MaximizedItem) as RadTileViewItem;
                if (orderedContainers[i] != maxedItem)
                {
                    this.AnimateItemPosition(orderedContainers[i], this.GetNewDockingPosition(currentOffset));
                    currentOffset = this.GetCurrentOffset(currentOffset, new Size(orderedContainers[i].Width, orderedContainers[i].Height));
                }
                else
                {
                    this.AnimateItemPosition(orderedContainers[i], this.GetNewMaximizedDockingPosition());
                }
            }
        }

        private void AnimateRestoredItemsPositions(IEnumerable<RadTileViewItem> containers)
        {
            foreach (RadTileViewItem container in containers)
            {
                if (this.ShouldAnimateItemPosition(container))
                {
                    this.AnimateItemPosition(container, this.GetTargetItemPosition(container));
                }
            }
        }

        private void AssignItemContainersToCells(Dictionary<int, RadTileViewItem> orderedContainers)
        {
            if (orderedContainers.Count != 0)
            {
                this.ConstructLayoutGrid();
                int containersCount = 0;
                int itemsWithGeneratedContainers = this.CountItemsWithGeneratedContainers();
                for (int r = 0; r < this.rows; r++)
                {
                    for (int c = 0; c < this.columns; c++)
                    {
                        if (orderedContainers.ContainsKey(containersCount))
                        {
                            Grid.SetRow(orderedContainers[containersCount], r);
                            Grid.SetColumn(orderedContainers[containersCount], c);
                            int index = this.CalculateItemIndex(orderedContainers[containersCount]);
                            if (orderedContainers[containersCount].Position != index)
                            {
                                orderedContainers[containersCount].Position = index;
                            }
                        }
                        containersCount++;
                        if (containersCount == itemsWithGeneratedContainers)
                        {
                            break;
                        }
                    }
                    if (containersCount == itemsWithGeneratedContainers)
                    {
                        return;
                    }
                }
            }
        }

        protected virtual void AttachItemContainerEventHandlers(RadTileViewItem item)
        {
            if (item != null)
            {
                item.TileStateChanged -= new EventHandler<RadRoutedEventArgs>(this.TileViewItem_TileStateChanged);
                item.TileStateChanged += new EventHandler<RadRoutedEventArgs>(this.TileViewItem_TileStateChanged);
            }
        }

        private Point CalculateDraggedItemPosition(Point currentPoint)
        {
            Rect absoluteTileViewCoordinates = GetAbsoluteCoordinates(this);
            double xDelta = currentPoint.X - this.dragStartPosition.X;
            double yDelta = currentPoint.Y - this.dragStartPosition.Y;
            bool shouldChangeX = (currentPoint.X > (absoluteTileViewCoordinates.Left + this.dragStartPositionRelativeOffset.X)) && (currentPoint.X < (absoluteTileViewCoordinates.Right - this.dragStartPositionRelativeOffsetComplement.X));
            bool shouldChangeY = (currentPoint.Y > (absoluteTileViewCoordinates.Top + this.dragStartPositionRelativeOffset.Y)) && (currentPoint.Y < (absoluteTileViewCoordinates.Bottom - this.dragStartPositionRelativeOffsetComplement.Y));
            Point currentPosition = this.GetDraggedItemCurrentPosition();
            double newX = ((this.dragStartPosition.X - this.dragStartPositionRelativeOffset.X) + xDelta) - absoluteTileViewCoordinates.Left;
            double newY = ((this.dragStartPosition.Y - this.dragStartPositionRelativeOffset.Y) + yDelta) - absoluteTileViewCoordinates.Top;
            double adjustedX = shouldChangeX ? newX : currentPosition.X;
            double adjustedY = shouldChangeY ? newY : currentPosition.Y;
            return new Point(Math.Round(adjustedX), Math.Round(adjustedY));
        }

        private int CalculateItemIndex(RadTileViewItem item)
        {
            return ((Grid.GetRow(item) * this.columns) + Grid.GetColumn(item));
        }

        internal bool CanDragItems()
        {
            return ((this.MaximizedItem == null) && this.IsItemDraggingEnabled);
        }

        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            base.ClearContainerForItemOverride(element, item);
            RadTileViewItem panel = element as RadTileViewItem;
            this.DetachItemContainerEventHandlers(panel);
            this.AssignItemContainersToCells(this.GetOrderedContainers());
            if ((item == this.MaximizedItem) || (base.Items.Count == 1))
            {
                this.MaximizedItem = null;
                if ((this.MaximizeMode == TileViewMaximizeMode.One) && (base.Items.Count > 0))
                {
                    RadTileViewItem maximizeCandidate = base.ItemContainerGenerator.ContainerFromItem(base.Items[0]) as RadTileViewItem;
                    if (maximizeCandidate != null)
                    {
                        maximizeCandidate.TileState = TileViewItemState.Maximized;
                    }
                }
                if (this.IsItemsAnimationEnabled)
                {
                    this.AnimateItemSizes();
                    this.AnimateItemsLayout();
                }
                else
                {
                    this.UpdateItemSizes();
                    this.UpdateItemsLayout();
                }
            }
            else
            {
                this.UpdateItemsSizeAndPosition();
            }
        }

        private void ConstructLayoutGrid()
        {
            double itemsWithGeneratedContainers = this.CountItemsWithGeneratedContainers();
            this.rows = (int)Math.Floor(Math.Sqrt(itemsWithGeneratedContainers));
            if (this.MaxRows > 0)
            {
                if (this.rows > this.MaxRows)
                {
                    this.rows = this.MaxRows;
                }
                this.columns = (int)Math.Ceiling(itemsWithGeneratedContainers / ((double)this.rows));
            }
            if (this.MaxColumns > 0)
            {
                this.columns = (int)Math.Ceiling(itemsWithGeneratedContainers / ((double)this.rows));
                if (this.columns > this.MaxColumns)
                {
                    this.columns = this.MaxColumns;
                    this.rows = (int)Math.Ceiling(itemsWithGeneratedContainers / ((double)this.columns));
                }
            }
            if ((this.MaxColumns == 0) && (this.MaxRows == 0))
            {
                this.columns = (int)Math.Ceiling(itemsWithGeneratedContainers / ((double)this.rows));
            }
        }

        private int CountItemsWithGeneratedContainers()
        {
            return base.Items.Cast<object>().Count<object>(item => ((base.ItemContainerGenerator.ContainerFromItem(item) != null) && ((base.ItemContainerGenerator.ContainerFromItem(item) as RadTileViewItem).Visibility == Visibility.Visible)));
        }

        protected virtual void DetachItemContainerEventHandlers(RadTileViewItem item)
        {
            if (item != null)
            {
                item.TileStateChanged -= new EventHandler<RadRoutedEventArgs>(this.TileViewItem_TileStateChanged);
                if (this.MaximizedItem == base.ItemContainerGenerator.ItemFromContainer(item))
                {
                    this.MaximizedItem = null;
                }
            }
        }

        internal void DetermineScrollBarVisibility(RadTileViewItem maximizedItem)
        {
            if (this.scrollBar != null)
            {
                if ((this.MinimizedItemsPosition == Dock.Left) || (this.MinimizedItemsPosition == Dock.Right))
                {
                    double maximizedItemMinimizedHeight = (maximizedItem != null) ? maximizedItem.MinimizedHeight : 0.0;
                    double totalMinimizedHeight = this.GetGeneratedItemContainers().Sum<RadTileViewItem>(tileViewItem => tileViewItem.MinimizedHeight) - maximizedItemMinimizedHeight;
                    bool scrollBarNeeded = totalMinimizedHeight > this.MainCanvasSize.Height;
                    if (this.ScrollBarVisibility == Telerik.Windows.Controls.TileView.ScrollBarVisibility.Auto)
                    {
                        this.scrollBar.Visibility = scrollBarNeeded ? Visibility.Visible : Visibility.Collapsed;
                    }
                    if (this.ScrollBarVisibility == Telerik.Windows.Controls.TileView.ScrollBarVisibility.Visible)
                    {
                        this.scrollBar.Visibility = Visibility.Visible;
                    }
                    if (this.ScrollBarVisibility == Telerik.Windows.Controls.TileView.ScrollBarVisibility.Collapsed)
                    {
                        this.scrollBar.Visibility = Visibility.Collapsed;
                    }
                    if (this.scrollBar.Visibility == Visibility.Visible)
                    {
                        this.scrollBar.Maximum = totalMinimizedHeight - this.MainCanvasSize.Height;
                        this.scrollBar.ViewportSize = this.MainCanvasSize.Height;
                    }
                }
                else
                {
                    double maximizedItemMinimizedWidth = (maximizedItem != null) ? maximizedItem.MinimizedWidth : 0.0;
                    double totalMinimizedWidth = this.GetGeneratedItemContainers().Sum<RadTileViewItem>(tileViewItem => tileViewItem.MinimizedWidth) - maximizedItemMinimizedWidth;
                    bool scrollBarNeeded = totalMinimizedWidth > this.MainCanvasSize.Width;
                    if (this.ScrollBarVisibility == Telerik.Windows.Controls.TileView.ScrollBarVisibility.Auto)
                    {
                        this.scrollBar.Visibility = scrollBarNeeded ? Visibility.Visible : Visibility.Collapsed;
                    }
                    if (this.ScrollBarVisibility == Telerik.Windows.Controls.TileView.ScrollBarVisibility.Visible)
                    {
                        this.scrollBar.Visibility = Visibility.Visible;
                    }
                    if (this.ScrollBarVisibility == Telerik.Windows.Controls.TileView.ScrollBarVisibility.Collapsed)
                    {
                        this.scrollBar.Visibility = Visibility.Collapsed;
                    }
                    if (this.scrollBar.Visibility == Visibility.Visible)
                    {
                        this.scrollBar.Maximum = totalMinimizedWidth - this.MainCanvasSize.Width;
                        this.scrollBar.ViewportSize = this.MainCanvasSize.Width;
                    }
                }
                this.scrollBar.UpdateLayout();
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Tansform to visual should be wrapped with a try-catch.")]
        private static Rect GetAbsoluteCoordinates(UIElement element)
        {
            GeneralTransform gt = null;
            try
            {
                gt = element.TransformToVisual(null);
            }
            catch
            {
            }
            if (gt == null)
            {
                return new Rect();
            }
            return new Rect(gt.Transform(new Point(0.0, 0.0)), element.RenderSize);
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new RadTileViewItem();
        }

        private int GetCurrentColumn(Point mousePosition)
        {
            return (int)Math.Floor(mousePosition.X / (this.MainCanvasSize.Width / ((double)this.columns)));
        }

        private static Point GetCurrentItemPosition(RadTileViewItem item)
        {
            double currentX = Canvas.GetLeft(item);
            return new Point(currentX, Canvas.GetTop(item));
        }

        private double GetCurrentOffset(double currentOffset, Size lastItemSize)
        {
            if (this.MinimizedItemsPosition.Equals(Dock.Left) || this.MinimizedItemsPosition.Equals(Dock.Right))
            {
                currentOffset += lastItemSize.Height;
                return currentOffset;
            }
            currentOffset += lastItemSize.Width;
            return currentOffset;
        }

        private int GetCurrentRow(Point mousePosition)
        {
            return (int)Math.Floor(mousePosition.Y / (this.MainCanvasSize.Height / ((double)this.rows)));
        }

        private Point GetDraggedItemCurrentPosition()
        {
            return new Point(Canvas.GetLeft(this.DraggedContainer), Canvas.GetTop(this.DraggedContainer));
        }

        private IEnumerable<RadTileViewItem> GetGeneratedItemContainers()
        {
            return (from item in base.Items.Cast<object>()
                    where (base.ItemContainerGenerator.ContainerFromItem(item) is RadTileViewItem) && ((base.ItemContainerGenerator.ContainerFromItem(item) as RadTileViewItem).Visibility == Visibility.Visible)
                    select base.ItemContainerGenerator.ContainerFromItem(item) as RadTileViewItem);
        }

        private Size GetItemSize(RadTileViewItem item)
        {
            if (this.MaximizedItem == null)
            {
                return this.GetRestoredItemSize(item);
            }
            RadTileViewItem maxedItem = base.ItemContainerGenerator.ContainerFromItem(this.MaximizedItem) as RadTileViewItem;
            if (item == maxedItem)
            {
                return this.GetMaximizedItemSize(item);
            }
            return this.GetMinimizedItemSize(item);
        }

        private RadTileViewItem GetItemToSwapWith(Point currentPoint)
        {
            foreach (RadTileViewItem container in this.GetGeneratedItemContainers())
            {
                if (this.ShouldSwapWithItem(container, currentPoint))
                {
                    return container;
                }
            }
            return null;
        }

        private Size GetMaximizedItemSize(RadTileViewItem item)
        {
            double newWidth = ((this.MainCanvasSize.Width - this.MinimizedColumnWidth) - item.Margin.Left) - item.Margin.Right;
            double newHeight = (this.MainCanvasSize.Height - item.Margin.Top) - item.Margin.Bottom;
            if (this.MinimizedItemsPosition.Equals(Dock.Bottom) || this.MinimizedItemsPosition.Equals(Dock.Top))
            {
                newWidth = (this.MainCanvasSize.Width - item.Margin.Left) - item.Margin.Right;
                newHeight = ((this.MainCanvasSize.Height - this.MinimizedRowHeight) - item.Margin.Top) - item.Margin.Bottom;
            }
            newWidth = Math.Max(0.0, newWidth);
            return new Size(newWidth, Math.Max(0.0, newHeight));
        }

        private Size GetMinimizedItemSize(RadTileViewItem item)
        {
            double newWidth;
            double newHeight;
            IEnumerable<RadTileViewItem> generatedItems = this.GetGeneratedItemContainers();
            int generatedContainersCount = this.CountItemsWithGeneratedContainers();
            double totalPresetHeight = ((IEnumerable<double>)(from i in generatedItems
                                                              where i.MinimizedHeight > double.Epsilon
                                                              select i.MinimizedHeight)).Sum();
            double totalPresetWidth = ((IEnumerable<double>)(from i in generatedItems
                                                             where i.MinimizedWidth > double.Epsilon
                                                             select i.MinimizedWidth)).Sum();
            int itemsNeedingHeight = generatedContainersCount - (from i in generatedItems
                                                                 where i.MinimizedHeight > 0.0
                                                                 select i).Count<RadTileViewItem>();
            int itemsNeedingWidth = generatedContainersCount - (from i in generatedItems
                                                                where i.MinimizedWidth > 0.0
                                                                select i).Count<RadTileViewItem>();
            if (this.MaximizedItem != null)
            {
                RadTileViewItem maxedItem = base.ItemContainerGenerator.ContainerFromItem(this.MaximizedItem) as RadTileViewItem;
                if ((maxedItem != null) && (maxedItem.MinimizedWidth > double.Epsilon))
                {
                    totalPresetWidth -= maxedItem.MinimizedWidth;
                    itemsNeedingWidth++;
                }
                if ((maxedItem != null) && (maxedItem.MinimizedHeight > double.Epsilon))
                {
                    totalPresetHeight -= maxedItem.MinimizedHeight;
                    itemsNeedingHeight++;
                }
            }
            double widthToDistribute = this.MainCanvasSize.Width - totalPresetWidth;
            double heightToDistribute = this.MainCanvasSize.Height - totalPresetHeight;
            if (this.MinimizedItemsPosition.Equals(Dock.Bottom) || this.MinimizedItemsPosition.Equals(Dock.Top))
            {
                newWidth = ((widthToDistribute / ((double)(itemsNeedingWidth - 1))) - item.Margin.Left) - item.Margin.Right;
                newHeight = (this.MinimizedRowHeight - item.Margin.Top) - item.Margin.Bottom;
            }
            else
            {
                newWidth = (this.MinimizedColumnWidth - item.Margin.Left) - item.Margin.Right;
                newHeight = ((heightToDistribute / ((double)Math.Max(1, itemsNeedingHeight - 1))) - item.Margin.Top) - item.Margin.Bottom;
            }
            bool shouldConsiderMinimizedWidth = (this.MinimizedItemsPosition == Dock.Bottom) || (this.MinimizedItemsPosition == Dock.Top);
            newWidth = ((item.MinimizedWidth > 0.0) && shouldConsiderMinimizedWidth) ? item.MinimizedWidth : Math.Max(0.0, newWidth);
            return new Size(newWidth, ((item.MinimizedHeight > 0.0) && !shouldConsiderMinimizedWidth) ? item.MinimizedHeight : Math.Max(0.0, newHeight));
        }

        private Point GetNewDockingPosition(double currentOffset)
        {
            double newX = 0.0;
            double newY = currentOffset;
            switch (this.MinimizedItemsPosition)
            {
                case Dock.Left:
                    newX = 0.0;
                    newY = currentOffset;
                    break;

                case Dock.Top:
                    newX = currentOffset;
                    newY = 0.0;
                    break;

                case Dock.Right:
                    newX = this.MainCanvasSize.Width - this.MinimizedColumnWidth;
                    newY = currentOffset;
                    break;

                case Dock.Bottom:
                    newX = currentOffset;
                    newY = this.MainCanvasSize.Height - this.MinimizedRowHeight;
                    break;
            }
            return new Point(newX, newY);
        }

        private Point GetNewMaximizedDockingPosition()
        {
            double newX = 0.0;
            double newY = 0.0;
            switch (this.MinimizedItemsPosition)
            {
                case Dock.Left:
                    newX = this.MinimizedColumnWidth;
                    newY = 0.0;
                    break;

                case Dock.Top:
                    newX = 0.0;
                    newY = this.MinimizedRowHeight;
                    break;

                case Dock.Right:
                    newX = 0.0;
                    newY = 0.0;
                    break;

                case Dock.Bottom:
                    newX = 0.0;
                    newY = 0.0;
                    break;
            }
            return new Point(newX, newY);
        }

        private Dictionary<int, RadTileViewItem> GetOrderedContainers()
        {
            Dictionary<int, RadTileViewItem> orderedContainers = new Dictionary<int, RadTileViewItem>();
            Dictionary<int, RadTileViewItem> unorderedContainers = new Dictionary<int, RadTileViewItem>();
            foreach (RadTileViewItem container in this.GetGeneratedItemContainers())
            {
                int index = this.CalculateItemIndex(container);
                if (!unorderedContainers.ContainsKey(index))
                {
                    unorderedContainers.Add(index, container);
                }
            }
            int tempIndex = 0;
            (from value in unorderedContainers
             orderby value.Key
             select value).ToList<KeyValuePair<int, RadTileViewItem>>().ForEach(delegate(KeyValuePair<int, RadTileViewItem> pair)
            {
                orderedContainers.Add(tempIndex, pair.Value);
                tempIndex++;
            });
            return orderedContainers;
        }

        private Point GetRelativeMousePosition(Point absolutePosition)
        {
            Point result = new Point();
            if (IsMouseOverVisual(absolutePosition, this))
            {
                Rect ownAbsoluteCoordinates = GetAbsoluteCoordinates(this);
                double relativeX = absolutePosition.X - ownAbsoluteCoordinates.Left;
                double relativeY = absolutePosition.Y - ownAbsoluteCoordinates.Top;
                result = new Point(relativeX, relativeY);
            }
            return result;
        }

        private Point GetRestoredItemPosition(RadTileViewItem item)
        {
            double x = Grid.GetColumn(item) * (this.MainCanvasSize.Width / ((double)this.columns));
            return new Point(x, Grid.GetRow(item) * (this.MainCanvasSize.Height / ((double)this.rows)));
        }

        private Size GetRestoredItemSize(RadTileViewItem item)
        {
            double width = ((this.MainCanvasSize.Width / ((double)this.columns)) - item.Margin.Left) - item.Margin.Right;
            double height = ((this.MainCanvasSize.Height / ((double)this.rows)) - item.Margin.Top) - item.Margin.Bottom;
            width = Math.Max(0.0, width);
            return new Size(width, Math.Max(0.0, height));
        }

        private Point GetSanitizedDraggedItemPosition(Point currentPoint)
        {
            Point currentPosition = this.GetDraggedItemCurrentPosition();
            Point newPosition = this.CalculateDraggedItemPosition(currentPoint);
            if (!this.IsValidDraggedItemPosition(newPosition))
            {
                return currentPosition;
            }
            return newPosition;
        }

        private void GetScrollBar(RadTileView tileView)
        {
            switch (tileView.MinimizedItemsPosition)
            {
                case Dock.Left:
                    tileView.scrollBar = base.GetTemplateChild("LeftScrollBar") as System.Windows.Controls.Primitives.ScrollBar;
                    break;

                case Dock.Top:
                    tileView.scrollBar = base.GetTemplateChild("TopScrollBar") as System.Windows.Controls.Primitives.ScrollBar;
                    break;

                case Dock.Right:
                    tileView.scrollBar = base.GetTemplateChild("RightScrollBar") as System.Windows.Controls.Primitives.ScrollBar;
                    break;

                case Dock.Bottom:
                    tileView.scrollBar = base.GetTemplateChild("BottomScrollBar") as System.Windows.Controls.Primitives.ScrollBar;
                    break;

                default:
                    tileView.scrollBar = null;
                    break;
            }
            if (tileView.scrollBar != null)
            {
                tileView.scrollBar.ValueChanged -= new RoutedPropertyChangedEventHandler<double>(tileView.ScrollBar_ValueChanged);
                tileView.scrollBar.ValueChanged += new RoutedPropertyChangedEventHandler<double>(tileView.ScrollBar_ValueChanged);
            }
        }

        private Point GetTargetItemPosition(RadTileViewItem item)
        {
            double targetX = Grid.GetColumn(item) * (this.MainCanvasSize.Width / ((double)this.columns));
            return new Point(targetX, Grid.GetRow(item) * (this.MainCanvasSize.Height / ((double)this.rows)));
        }

        private void HandleTileViewItemMaximized(RadTileViewItem item)
        {
            int currentZIndex = RadTileViewItem.CurrentZIndex;
            RadTileViewItem.CurrentZIndex = currentZIndex + 1;
            Canvas.SetZIndex(item, currentZIndex);
            this.MaximizedItem = base.ItemContainerGenerator.ItemFromContainer(item);
            foreach (RadTileViewItem container in this.GetGeneratedItemContainers())
            {
                RadTileViewItem item2 = base.ItemContainerGenerator.ContainerFromItem(this.MaximizedItem) as RadTileViewItem;
                if (container != item2)
                {
                    container.TileState = TileViewItemState.Minimized;
                }
            }
            this.UpdateItemsSizeAndPosition();
            SetItemSize(item, this.GetMaximizedItemSize(item));
        }

        private void HandleTileViewItemRestored(RadTileViewItem item)
        {
            if (this.MaximizeMode != TileViewMaximizeMode.One)
            {
                this.MaximizedItem = null;
                foreach (RadTileViewItem container in this.GetGeneratedItemContainers())
                {
                    container.TileState = TileViewItemState.Restored;
                }
                if (this.IsItemsAnimationEnabled)
                {
                    this.AnimateItemSizes();
                    this.AnimateItemsLayout();
                }
                else
                {
                    this.UpdateItemSizes();
                    this.UpdateItemsLayout();
                }
            }
            else
            {
                item.TileState = TileViewItemState.Maximized;
            }
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return (item is RadTileViewItem);
        }

        private bool IsItemMaximizedByDefault(RadTileViewItem item)
        {
            if (((this.MaximizeMode != TileViewMaximizeMode.One) || (this.MaximizedItem != null)) && (item.TileState != TileViewItemState.Maximized))
            {
                return (this.MaximizedItem == base.ItemContainerGenerator.ItemFromContainer(item));
            }
            return true;
        }

        private static bool IsMouseOverVisual(Point mousePosition, UIElement visual)
        {
            Rect ownAbsoluteCoordinates = GetAbsoluteCoordinates(visual);
            return ((((mousePosition.X >= ownAbsoluteCoordinates.Left) && (mousePosition.X <= ownAbsoluteCoordinates.Right)) && (mousePosition.Y >= ownAbsoluteCoordinates.Top)) && (mousePosition.Y <= ownAbsoluteCoordinates.Bottom));
        }

        private bool IsValidDraggedItemPosition(Point newPosition)
        {
            Size itemSize = new Size(this.DraggedContainer.ActualWidth, this.DraggedContainer.ActualHeight);
            Rect itemBounds = new Rect(newPosition, itemSize);
            return (((((itemBounds.Bottom <= this.MainCanvasSize.Height) && (itemBounds.Bottom >= itemSize.Height)) && ((itemBounds.Left >= 0.0) && (itemBounds.Left <= (this.MainCanvasSize.Width - itemSize.Width)))) && (((itemBounds.Right >= itemSize.Width) && (itemBounds.Right <= this.MainCanvasSize.Width)) && (itemBounds.Top >= 0.0))) && (itemBounds.Top <= (this.MainCanvasSize.Height - itemSize.Height)));
        }

        private bool IsVisualUpdateNeccessary()
        {
            return ((!double.IsInfinity(this.MainCanvasSize.Width) && !double.IsNaN(this.MainCanvasSize.Width)) && (this.MainCanvasSize.Width != 0.0));
        }

        private void ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
        {
            if (base.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
            {
                this.UpdateScrollBar();
            }
        }

        private void ListenForVisibilityChange(RadTileViewItem container)
        {
            VisibilityHelper visibilityHelper = new VisibilityHelper
            {
                ContainerVisibility = container.Visibility,
                ContainerVisibilityChangeCallback = new Action(this.UpdateTilesLayout)
            };
            Binding binding = new Binding("Visibility")
            {
                Mode = BindingMode.TwoWay,
                Source = container
            };
            visibilityHelper.SetBinding(VisibilityHelper.ContainerVisibilityProperty, binding);
        }

        private void MaximizedContainerSizeChanged(object sender, SizeChangedEventArgs e)
        {
            RadTileViewItem item = sender as RadTileViewItem;
            if (item != null)
            {
                if (this.ShouldRestrainMaximizedItemWidth(e.NewSize.Width))
                {
                    item.Width = e.PreviousSize.Width;
                    this.UpdateItemSizes();
                }
                else if (this.ShouldRestrainMaximizedItemHeight(e.NewSize.Height))
                {
                    item.Height = e.PreviousSize.Height;
                    this.UpdateItemSizes();
                }
            }
        }

        public override void OnApplyTemplate()
        {
            this.ForEachContainerItem<RadTileViewItem>(delegate(RadTileViewItem tileItem)
            {
                tileItem.CleanContentPresenters();
            });
            base.OnApplyTemplate();
            this.GetScrollBar(this);
            this.UpdateScrollBar();
        }

        protected virtual void OnIsAnimationOptimizedChanged(bool oldValue, bool newValue)
        {
        }

        private static void OnIsAnimationOptimizedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadTileView source = d as RadTileView;
            bool newValue = (bool)e.NewValue;
            bool oldValue = (bool)e.OldValue;
            if (source != null)
            {
                source.OnIsAnimationOptimizedChanged(oldValue, newValue);
            }
        }

        private static void OnIsItemsAnimationEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadTileView tileView = d as RadTileView;
            if (tileView != null)
            {
                AnimationManager.SetIsAnimationEnabled(tileView, (bool)e.NewValue);
            }
        }

        internal bool OnItemDragging(RadTileViewItem draggedItem, MouseEventArgs e)
        {
            if (!this.CanDragItems())
            {
                return false;
            }
            TileViewDragEventArgs args = new TileViewDragEventArgs(draggedItem, PreviewTileDragStartedEvent, this);
            this.OnPreviewTileDragStarted(args);
            if (!args.Handled)
            {
                this.OnLayoutChangeStarted();
                base.CaptureMouse();
                this.DraggedContainer = draggedItem;
                this.dragStartPosition = e.GetPosition(null);
                Rect draggedTileAbsoluteCoordinates = GetAbsoluteCoordinates(this.DraggedContainer);
                Point draggedTileAbsolutePosition = new Point(draggedTileAbsoluteCoordinates.X, draggedTileAbsoluteCoordinates.Y);

                this.dragStartPositionRelativeOffset = new Point
                {
                    X = this.dragStartPosition.X - draggedTileAbsolutePosition.X,
                    Y = this.dragStartPosition.Y - draggedTileAbsolutePosition.Y
                };

                this.dragStartPositionRelativeOffsetComplement = new Point
                {
                    X = draggedTileAbsoluteCoordinates.Right - this.dragStartPosition.X,
                    Y = draggedTileAbsoluteCoordinates.Bottom - this.dragStartPosition.Y
                };

                int currentZIndex = RadTileViewItem.CurrentZIndex;
                RadTileViewItem.CurrentZIndex = currentZIndex + 1;
                Canvas.SetZIndex(this.DraggedContainer, currentZIndex);
                base.MouseMove += new MouseEventHandler(this.RadTileView_MouseMove);
                base.AddHandler(UIElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(this.RadTileView_MouseLeftButtonUp), true);
                this.OnTileDragStarted(new TileViewDragEventArgs(draggedItem, TileDragStartedEvent, this));
            }
            return true;
        }

        private void OnLayoutChangeEnded()
        {
            this.OnLayoutChangeEnded(EventArgs.Empty);
        }

        protected virtual void OnLayoutChangeEnded(EventArgs args)
        {
            if (this.LayoutChangeEnded != null)
            {
                this.LayoutChangeEnded(this, args);
            }
        }

        private void OnLayoutChangeStarted()
        {
            this.OnLayoutChangeStarted(EventArgs.Empty);
        }

        protected virtual void OnLayoutChangeStarted(EventArgs args)
        {
            if (this.LayoutChangeStarted != null)
            {
                this.LayoutChangeStarted(this, args);
            }
        }

        private static void OnMaxColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadTileView tileView = d as RadTileView;
            if (tileView != null)
            {
                tileView.AssignItemContainersToCells(tileView.GetOrderedContainers());
                tileView.AnimateItemSizes();
                tileView.AnimateItemsLayout();
            }
        }

        private static void OnMaximizedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadTileView tileView = d as RadTileView;
            if (tileView != null)
            {
                RadTileViewItem item = tileView.ItemContainerGenerator.ContainerFromItem(e.NewValue) as RadTileViewItem;
                if ((item != null) && (tileView.MaximizeMode == TileViewMaximizeMode.Zero))
                {
                    tileView.MaximizedItem = null;
                }
                else if ((item != null) && (item.TileState != TileViewItemState.Maximized))
                {
                    if (tileView.MaximizedItem != null)
                    {
                        RadTileViewItem oldItem = tileView.ItemContainerGenerator.ContainerFromItem(tileView.MaximizedItem) as RadTileViewItem;
                        if (oldItem != null)
                        {
                            oldItem.TileState = TileViewItemState.Minimized;
                        }
                    }
                    item.TileState = TileViewItemState.Maximized;
                }
                else if ((e.NewValue == null) && (e.OldValue != null))
                {
                    RadTileViewItem oldItem = tileView.ItemContainerGenerator.ContainerFromItem(e.OldValue) as RadTileViewItem;
                    if ((oldItem != null) && (oldItem.TileState == TileViewItemState.Maximized))
                    {
                        oldItem.TileState = TileViewItemState.Restored;
                    }
                }
            }
        }

        private static void OnMaxRowsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadTileView tileView = d as RadTileView;
            if (tileView != null)
            {
                tileView.AssignItemContainersToCells(tileView.GetOrderedContainers());
                tileView.AnimateItemSizes();
                tileView.AnimateItemsLayout();
            }
        }

        private static void OnMinimizedColumnWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadTileView tileView = d as RadTileView;
            if (tileView != null)
            {
                tileView.UpdateItemsLayout();
            }
        }

        private static void OnMinimizedItemsPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadTileView tileView = d as RadTileView;
            if (tileView != null)
            {
                if (tileView.scrollBar != null)
                {
                    tileView.scrollBar.Visibility = Visibility.Collapsed;
                }
                tileView.GetScrollBar(tileView);
                if (tileView.MaximizedItem != null)
                {
                    tileView.DetermineScrollBarVisibility(tileView.ItemContainerGenerator.ContainerFromItem(tileView.MaximizedItem) as RadTileViewItem);
                }
                tileView.SetClip();
                tileView.AnimateItemSizes();
                tileView.AnimateItemsLayout();
            }
        }

        private static void OnMinimizedRowHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadTileView tileView = d as RadTileView;
            if (tileView != null)
            {
                tileView.UpdateItemsLayout();
            }
        }

        protected internal virtual void OnPreviewTileDragStarted(TileViewDragEventArgs e)
        {
            this.RaiseEvent(e);
        }

        protected virtual void OnScrollBarVisibilityChanged(Telerik.Windows.Controls.TileView.ScrollBarVisibility oldValue, Telerik.Windows.Controls.TileView.ScrollBarVisibility newValue)
        {
            if (this.MaximizedItem != null)
            {
                this.DetermineScrollBarVisibility(base.ItemContainerGenerator.ContainerFromItem(this.MaximizedItem) as RadTileViewItem);
                base.Dispatcher.BeginInvoke(delegate
                {
                    this.UpdateItemSizes();
                    this.UpdateItemsLayout();
                });
            }
        }

        private static void OnScrollBarVisibilityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadTileView source = d as RadTileView;
            Telerik.Windows.Controls.TileView.ScrollBarVisibility newValue = (Telerik.Windows.Controls.TileView.ScrollBarVisibility)e.NewValue;
            Telerik.Windows.Controls.TileView.ScrollBarVisibility oldValue = (Telerik.Windows.Controls.TileView.ScrollBarVisibility)e.OldValue;
            if (source != null)
            {
                source.OnScrollBarVisibilityChanged(oldValue, newValue);
            }
        }

        protected internal virtual void OnTileDragEnded(TileViewDragEventArgs e)
        {
            this.RaiseEvent(e);
        }

        protected internal virtual void OnTileDragStarted(TileViewDragEventArgs e)
        {
            this.RaiseEvent(e);
        }

        protected internal virtual void OnTilePositionChanged(RadRoutedEventArgs e)
        {
            this.RaiseEvent(e);
        }

        protected internal virtual void OnTilesStateChangeEnded(RadRoutedEventArgs e)
        {
            this.RaiseEvent(e);
        }

        private void PrepareContainer(RadTileViewItem container, object item)
        {
            container.ParentTileView = this;
            Dictionary<int, RadTileViewItem> orderedContainers = this.GetOrderedContainers();
            if (!orderedContainers.ContainsKey(orderedContainers.Count))
            {
                orderedContainers.Add(orderedContainers.Count, container);
            }
            if ((this.MaximizeMode == TileViewMaximizeMode.Zero) && (container.TileState == TileViewItemState.Maximized))
            {
                container.TileState = TileViewItemState.Restored;
            }
            this.AttachItemContainerEventHandlers(container);
            this.AssignItemContainersToCells(orderedContainers);
            this.UpdateItemsSizeAndPosition();
            if (this.IsItemMaximizedByDefault(container))
            {
                container.SizeChanged -= new SizeChangedEventHandler(this.MaximizedContainerSizeChanged);
                container.SizeChanged += new SizeChangedEventHandler(this.MaximizedContainerSizeChanged);
                if (container.TileState == TileViewItemState.Maximized)
                {
                    this.HandleTileViewItemMaximized(container);
                }
                else
                {
                    container.TileState = TileViewItemState.Maximized;
                }
            }
            if ((this.MaximizedItem != null) && (this.MaximizedItem != item))
            {
                container.TileState = TileViewItemState.Minimized;
            }
            if (container.ContentTemplate == null)
            {
                if (this.ContentTemplate != null)
                {
                    container.ContentTemplate = this.ContentTemplate;
                }
                else if (this.ContentTemplateSelector != null)
                {
                    container.ContentTemplate = this.ContentTemplateSelector.SelectTemplate(item, container);
                }
            }
            this.ListenForVisibilityChange(container);
            this.UpdateItemsLayout();
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            RadTileViewItem container = element as RadTileViewItem;
            this.PrepareContainer(container, item);
            StyleManager.SetThemeFromParent(container, this);
            container.UpdateHeaderPresenterContent();
        }

        private void RadTileView_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.OnTileDragEnded(new TileViewDragEventArgs(this.DraggedContainer, TileDragEndedEvent, this));
            this.OnLayoutChangeEnded();
            this.DraggedContainer = null;
            base.MouseMove -= new MouseEventHandler(this.RadTileView_MouseMove);
            base.MouseLeftButtonUp -= new MouseButtonEventHandler(this.RadTileView_MouseLeftButtonUp);
            base.ReleaseMouseCapture();
            this.UpdateItemsLayout();
        }

        private void RadTileView_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.DraggedContainer != null)
            {
                Point absolutePoistion = e.GetPosition(null);
                Point newPosition = this.GetSanitizedDraggedItemPosition(absolutePoistion);
                if (newPosition != this.GetDraggedItemCurrentPosition())
                {
                    SetItemPosition(this.DraggedContainer, newPosition);
                }
                RadTileViewItem itemToSwapWith = this.GetItemToSwapWith(absolutePoistion);
                if (itemToSwapWith != null)
                {
                    this.SwapItems(itemToSwapWith);
                }
            }
        }

        private void RadTileView_PreviewTileStateChanged(object sender, RadRoutedEventArgs e)
        {
            RadTileViewItem caller = e.Source as RadTileViewItem;
            Telerik.Windows.Controls.ItemsControl sourceItemsControl = Telerik.Windows.Controls.ItemsControl.ItemsControlFromItemContainer(caller);
            if (((caller != null) && (sourceItemsControl != null)) && ((this == sourceItemsControl) && (caller.TileState == TileViewItemState.Maximized)))
            {
                e.Handled = this.MaximizeMode == TileViewMaximizeMode.Zero;
            }
        }

        private void RadTileView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.UpdateScrollBar();
        }

        private void ScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            base.Dispatcher.BeginInvoke(delegate
            {
                foreach (RadTileViewItem item in from item in this.GetGeneratedItemContainers()
                                                 where item.TileState == TileViewItemState.Minimized
                                                 select item)
                {
                    if ((this.MinimizedItemsPosition == Dock.Left) || (this.MinimizedItemsPosition == Dock.Right))
                    {
                        double top = (double)item.GetValue(Canvas.TopProperty);
                        item.SetValue(Canvas.TopProperty, top - (e.NewValue - e.OldValue));
                        continue;
                    }
                    double left = (double)item.GetValue(Canvas.LeftProperty);
                    item.SetValue(Canvas.LeftProperty, left - (e.NewValue - e.OldValue));
                }
            });
        }

        private void SetClip()
        {
            if ((this.MinimizedItemsPosition == Dock.Left) || (this.MinimizedItemsPosition == Dock.Right))
            {
                if ((this.scrollBar != null) && (this.scrollBar.Visibility == Visibility.Visible))
                {
                    base.Clip = new RectangleGeometry { Rect = new Rect(0.0, base.Padding.Top, base.ActualWidth, Math.Max((double)0.0, (double)(base.ActualHeight - (2.0 * base.Padding.Bottom)))) };
                }
                else
                {
                    base.Clip = new RectangleGeometry { Rect = new Rect(0.0, 0.0, base.ActualWidth, base.ActualHeight) };
                }
            }
            else if ((this.scrollBar != null) && (this.scrollBar.Visibility == Visibility.Visible))
            {
                base.Clip = new RectangleGeometry { Rect = new Rect(base.Padding.Left, 0.0, Math.Max((double)0.0, (double)(base.ActualWidth - (2.0 * base.Padding.Right))), base.ActualHeight) };
            }
            else
            {
                base.Clip = new RectangleGeometry { Rect = new Rect(0.0, 0.0, base.ActualWidth, base.ActualHeight) };
            }
        }

        private static void SetItemPosition(RadTileViewItem item, Point position)
        {
            Canvas.SetLeft(item, position.X);
            Canvas.SetTop(item, position.Y);
        }

        private static void SetItemSize(RadTileViewItem item, Size itemSize)
        {
            item.Width = itemSize.Width;
            item.Height = itemSize.Height;
        }

        private bool ShouldAnimateItemPosition(RadTileViewItem item)
        {
            if (item == this.DraggedContainer)
            {
                return false;
            }
            Point currentPosition = GetCurrentItemPosition(item);
            Point targetPosition = this.GetTargetItemPosition(item);
            if (targetPosition.X == currentPosition.X)
            {
                return (targetPosition.Y != currentPosition.Y);
            }
            return true;
        }

        private bool ShouldAnimateItemSizes()
        {
            return (((!double.IsInfinity(this.MainCanvasSize.Width) && !double.IsNaN(this.MainCanvasSize.Width)) && (this.MainCanvasSize.Width != 0.0)) && (this.MainCanvasSize.Height != 0.0));
        }

        private bool ShouldRestrainMaximizedItemHeight(double newHeight)
        {
            if ((this.MinimizedItemsPosition != Dock.Top) && (this.MinimizedItemsPosition != Dock.Bottom))
            {
                return false;
            }
            return (newHeight > (this.MainCanvasSize.Height - this.MinimizedRowHeight));
        }

        private bool ShouldRestrainMaximizedItemWidth(double newWidth)
        {
            if ((this.MinimizedItemsPosition != Dock.Left) && (this.MinimizedItemsPosition != Dock.Right))
            {
                return false;
            }
            return (newWidth > (this.MainCanvasSize.Width - this.MinimizedColumnWidth));
        }

        private bool ShouldSwapWithItem(RadTileViewItem item, Point currentPoint)
        {
            Point currentMousePosition = currentPoint;
            Point relativeMousePosition = this.GetRelativeMousePosition(currentMousePosition);
            int currentRow = this.GetCurrentRow(relativeMousePosition);
            int currentColumn = this.GetCurrentColumn(relativeMousePosition);
            int itemRow = Grid.GetRow(item);
            int itemColumn = Grid.GetColumn(item);
            bool isMouseOverItem = IsMouseOverVisual(currentMousePosition, item);
            if (((item == this.DraggedContainer) || !isMouseOverItem) || (item.PositionAnimation != null))
            {
                return false;
            }
            if (itemColumn != currentColumn)
            {
                return (itemRow == currentRow);
            }
            return true;
        }

        private void SwapItems(RadTileViewItem itemToSwapWith)
        {
            int draggingPanelNewColumn = Grid.GetColumn(itemToSwapWith);
            int draggingPanelNewRow = Grid.GetRow(itemToSwapWith);
            Grid.SetColumn(itemToSwapWith, Grid.GetColumn(this.DraggedContainer));
            Grid.SetRow(itemToSwapWith, Grid.GetRow(this.DraggedContainer));
            Grid.SetColumn(this.DraggedContainer, draggingPanelNewColumn);
            Grid.SetRow(this.DraggedContainer, draggingPanelNewRow);
            this.DraggedContainer.Position = this.CalculateItemIndex(this.DraggedContainer);
            itemToSwapWith.Position = this.CalculateItemIndex(itemToSwapWith);
            if (this.IsItemsAnimationEnabled)
            {
                this.AnimateItemsLayout();
            }
            else
            {
                this.UpdateItemsLayout();
            }
        }

        private void TileViewItem_TileStateChanged(object sender, RadRoutedEventArgs e)
        {
            RadTileViewItem item = sender as RadTileViewItem;
            if ((item != null) && (Telerik.Windows.Controls.ItemsControl.ItemsControlFromItemContainer(item) == this))
            {
                if (item.TileState == TileViewItemState.Maximized)
                {
                    this.DetermineScrollBarVisibility(item);
                    this.HandleTileViewItemMaximized(item);
                }
                else if (item.TileState == TileViewItemState.Restored)
                {
                    if (this.scrollBar != null)
                    {
                        this.scrollBar.Visibility = Visibility.Collapsed;
                        this.scrollBar.UpdateLayout();
                    }
                    this.HandleTileViewItemRestored(item);
                }
            }
            this.SetClip();
        }

        private static RadTileViewItem TryGetTileViewItem(FrameworkElement element)
        {
            RadTileViewItem result = element as RadTileViewItem;
            DependencyObject obj = element;
            while ((result == null) && (obj != null))
            {
                obj = VisualTreeHelper.GetParent(obj);
                result = obj as RadTileViewItem;
            }
            return result;
        }

        private void UpdateItemSizes()
        {
            foreach (RadTileViewItem container in this.GetGeneratedItemContainers())
            {
                SetItemSize(container, this.GetItemSize(container));
            }
        }

        private void UpdateItemsLayout()
        {
            if (this.IsVisualUpdateNeccessary())
            {
                IEnumerable<RadTileViewItem> containers = this.GetGeneratedItemContainers();
                if (this.MaximizedItem == null)
                {
                    this.UpdateRestoredItemsPositions(containers);
                }
                else
                {
                    this.UpdateItemsPositionsWhenThereIsMaximized();
                }
            }
        }

        private void UpdateItemsPositionsWhenThereIsMaximized()
        {
            Dictionary<int, RadTileViewItem> orderedContainers = this.GetOrderedContainers();
            double currentOffset = 0.0;
            if (this.scrollBar != null)
            {
                currentOffset = -1.0 * this.scrollBar.Value;
            }
            for (int i = 0; i < orderedContainers.Count; i++)
            {
                Size newSize;
                Point newPosition;
                RadTileViewItem maxedItem = base.ItemContainerGenerator.ContainerFromItem(this.MaximizedItem) as RadTileViewItem;
                if (orderedContainers[i] != maxedItem)
                {
                    newSize = this.GetMinimizedItemSize(orderedContainers[i]);
                    newPosition = this.GetNewDockingPosition(currentOffset);
                    currentOffset = this.GetCurrentOffset(currentOffset, newSize);
                }
                else
                {
                    newSize = this.GetMaximizedItemSize(orderedContainers[i]);
                    newPosition = this.GetNewMaximizedDockingPosition();
                }
                SetItemSize(orderedContainers[i], newSize);
                SetItemPosition(orderedContainers[i], newPosition);
            }
        }

        public void UpdateItemsSizeAndPosition()
        {
            if (this.IsItemsAnimationEnabled)
            {
                this.AnimateItemSizes();
                this.AnimateItemsLayout();
            }
            else
            {
                this.UpdateItemSizes();
                this.UpdateItemsLayout();
            }
            this.UpdateScrollBar();
        }

        private void UpdateRestoredItem(RadTileViewItem item)
        {
            SetItemPosition(item, this.GetRestoredItemPosition(item));
            SetItemSize(item, this.GetRestoredItemSize(item));
        }

        private void UpdateRestoredItemsPositions(IEnumerable<RadTileViewItem> containers)
        {
            foreach (RadTileViewItem container in containers)
            {
                if ((container.PositionAnimation == null) && (container != this.DraggedContainer))
                {
                    this.UpdateRestoredItem(container);
                }
            }
        }

        private void UpdateScrollBar()
        {
            base.Dispatcher.BeginInvoke(delegate
            {
                if (this.MaximizedItem != null)
                {
                    this.DetermineScrollBarVisibility(base.ItemContainerGenerator.ContainerFromItem(this.MaximizedItem) as RadTileViewItem);
                }
                this.UpdateItemSizes();
                this.UpdateItemsLayout();
                this.SetClip();
            });
        }

        private void UpdateTilesLayout()
        {
            foreach (RadTileViewItem container in this.GetGeneratedItemContainers())
            {
                this.PrepareContainer(container, container.DataContext);
            }
        }

        [Telerik.Windows.Controls.SRCategory("AppearanceCategory")]
        public DataTemplate ContentTemplate
        {
            get
            {
                return (DataTemplate)base.GetValue(ContentTemplateProperty);
            }
            set
            {
                base.SetValue(ContentTemplateProperty, value);
            }
        }

        [Telerik.Windows.Controls.SRCategory("AppearanceCategory")]
        public DataTemplateSelector ContentTemplateSelector
        {
            get
            {
                return (DataTemplateSelector)base.GetValue(ContentTemplateSelectorProperty);
            }
            set
            {
                base.SetValue(ContentTemplateSelectorProperty, value);
            }
        }

        internal RadTileViewItem DraggedContainer { get; set; }

        public bool IsAnimationOptimized
        {
            get
            {
                return (bool)base.GetValue(IsAnimationOptimizedProperty);
            }
            set
            {
                base.SetValue(IsAnimationOptimizedProperty, value);
            }
        }

        public bool IsDragging
        {
            get
            {
                return (this.DraggedContainer != null);
            }
        }

        public bool IsItemDraggingEnabled
        {
            get
            {
                return (bool)base.GetValue(IsItemDraggingEnabledProperty);
            }
            set
            {
                base.SetValue(IsItemDraggingEnabledProperty, value);
            }
        }

        public bool IsItemsAnimationEnabled
        {
            get
            {
                return (((bool)base.GetValue(IsItemsAnimationEnabledProperty)) && AnimationManager.GetIsAnimationEnabled(this));
            }
            set
            {
                base.SetValue(IsItemsAnimationEnabledProperty, value);
            }
        }

        public bool IsLayoutChanging
        {
            get
            {
                bool isAnimationInProgress = this.GetGeneratedItemContainers().Any<RadTileViewItem>(delegate(RadTileViewItem item)
                {
                    if (item.PositionAnimation == null)
                    {
                        return item.SizeAnimation != null;
                    }
                    return true;
                });
                if (!this.IsDragging)
                {
                    return isAnimationInProgress;
                }
                return true;
            }
        }

        private Size MainCanvasSize
        {
            get
            {
                double width = base.ActualWidth;
                double height = base.ActualHeight;
                if ((this.scrollBar != null) && (this.scrollBar.Visibility == Visibility.Visible))
                {
                    if (this.scrollBar.Orientation == Orientation.Vertical)
                    {
                        width -= this.scrollBar.DesiredSize.Width;
                    }
                    else
                    {
                        height -= this.scrollBar.DesiredSize.Height;
                    }
                }
                return new Size(width, height);
            }
        }

        public int MaxColumns
        {
            get
            {
                return (int)base.GetValue(MaxColumnsProperty);
            }
            set
            {
                base.SetValue(MaxColumnsProperty, value);
            }
        }

        public object MaximizedItem
        {
            get
            {
                return base.GetValue(MaximizedItemProperty);
            }
            set
            {
                base.SetValue(MaximizedItemProperty, value);
            }
        }

        public TileViewMaximizeMode MaximizeMode
        {
            get
            {
                return (TileViewMaximizeMode)base.GetValue(MaximizeModeProperty);
            }
            set
            {
                base.SetValue(MaximizeModeProperty, value);
            }
        }

        public int MaxRows
        {
            get
            {
                return (int)base.GetValue(MaxRowsProperty);
            }
            set
            {
                base.SetValue(MaxRowsProperty, value);
            }
        }

        public double MinimizedColumnWidth
        {
            get
            {
                return (double)base.GetValue(MinimizedColumnWidthProperty);
            }
            set
            {
                base.SetValue(MinimizedColumnWidthProperty, value);
            }
        }

        public Dock MinimizedItemsPosition
        {
            get
            {
                return (Dock)base.GetValue(MinimizedItemsPositionProperty);
            }
            set
            {
                base.SetValue(MinimizedItemsPositionProperty, value);
            }
        }

        public double MinimizedRowHeight
        {
            get
            {
                return (double)base.GetValue(MinimizedRowHeightProperty);
            }
            set
            {
                base.SetValue(MinimizedRowHeightProperty, value);
            }
        }

        public Duration ReorderingDuration
        {
            get
            {
                return (Duration)base.GetValue(ReorderingDurationProperty);
            }
            set
            {
                base.SetValue(ReorderingDurationProperty, value);
            }
        }

        public IEasingFunction ReorderingEasing
        {
            get
            {
                return (IEasingFunction)base.GetValue(ReorderingEasingProperty);
            }
            set
            {
                base.SetValue(ReorderingEasingProperty, value);
            }
        }

        public Duration ResizingDuration
        {
            get
            {
                return (Duration)base.GetValue(ResizingDurationProperty);
            }
            set
            {
                base.SetValue(ResizingDurationProperty, value);
            }
        }

        public IEasingFunction ResizingEasing
        {
            get
            {
                return (IEasingFunction)base.GetValue(ResizingEasingProperty);
            }
            set
            {
                base.SetValue(ResizingEasingProperty, value);
            }
        }

        internal System.Windows.Controls.Primitives.ScrollBar ScrollBar
        {
            get
            {
                return this.scrollBar;
            }
        }

        public Telerik.Windows.Controls.TileView.ScrollBarVisibility ScrollBarVisibility
        {
            get
            {
                return (Telerik.Windows.Controls.TileView.ScrollBarVisibility)base.GetValue(ScrollBarVisibilityProperty);
            }
            set
            {
                base.SetValue(ScrollBarVisibilityProperty, value);
            }
        }

        public Telerik.Windows.Controls.TileStateChangeTrigger TileStateChangeTrigger
        {
            get
            {
                return (Telerik.Windows.Controls.TileStateChangeTrigger)base.GetValue(TileStateChangeTriggerProperty);
            }
            set
            {
                base.SetValue(TileStateChangeTriggerProperty, value);
            }
        }
    }
}

