namespace Telerik.Windows.Controls.TreeView
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Automation;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Threading;
    using Telerik.Windows;
    using Telerik.Windows.Controls;

    public class TreeViewPanel : VirtualizingPanel, IScrollInfo
    {
        private Size availableSize;
        private FrameworkElement bringIntoViewContainer;
        private int bringIntoViewIndex;
        private const double CacheCleanupDelay = 3.0;
        private int cacheStart;
        public static readonly DependencyProperty ChildDefaultLengthProperty = DependencyProperty.Register("ChildDefaultLength", typeof(double), typeof(TreeViewPanel), new Telerik.Windows.PropertyMetadata(24.0));
        private Dictionary<TreeViewPanel, bool> cleanupSet;
        private DispatcherTimer cleanupTimer;
        public static readonly Telerik.Windows.RoutedEvent CleanUpVirtualizedItemEvent = EventManager.RegisterRoutedEvent("CleanUpVirtualizedItemEvent", RoutingStrategy.Direct, typeof(Telerik.Windows.Controls.TreeView.CleanUpVirtualizedItemEventHandler), typeof(TreeViewPanel));
        private const int ContainerCacheSize = 0;
        private int firstVisibleChildIndex;
        private static readonly UncommonField<WeakReference[]> FocusTrailField = new UncommonField<WeakReference[]>(null);
        private static readonly UncommonField<bool> FocusTrailItemField = new UncommonField<bool>(false);
        private bool hasMeasured;
        private Telerik.Windows.Controls.TreeView.IndexTree indexTree;
        internal static readonly DependencyProperty IndexTreeProperty = DependencyProperty.RegisterAttached("IndexTree", typeof(Telerik.Windows.Controls.TreeView.IndexTree), typeof(TreeViewPanel), null);
        private static readonly DependencyProperty IsMeasureValidProperty = DependencyProperty.RegisterAttached("IsMeasureValid", typeof(bool), typeof(TreeViewPanel), new Telerik.Windows.PropertyMetadata(false));
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Virtualizing", Justification = "The spelling is correct.")]
        public static readonly DependencyProperty IsVirtualizingProperty = DependencyProperty.RegisterAttached("IsVirtualizing", typeof(bool), typeof(TreeViewPanel), new Telerik.Windows.PropertyMetadata(false));
        private bool isVisualCacheEnabled;
        private int lastVisibleChildIndex;
        private double maxWidth;
        private static readonly DependencyProperty MeasureDataProperty = DependencyProperty.RegisterAttached("MeasureData", typeof(MeasureData), typeof(TreeViewPanel), null);
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(System.Windows.Controls.Orientation), typeof(TreeViewPanel), new Telerik.Windows.PropertyMetadata(System.Windows.Controls.Orientation.Vertical, new PropertyChangedCallback(TreeViewPanel.OnOrientationChanged)));
        private List<UIElement> realizedChildren;
        internal static readonly Telerik.Windows.RoutedEvent RequestBringIntoViewEvent = EventManager.RegisterRoutedEvent("RequestBringIntoView", RoutingStrategy.Bubble, typeof(RequestBringIntoViewEventHandler), typeof(TreeViewPanel));
        private ScrollData scrollData;
        private const double ScrollLineDelta = 24.0;
        private bool suspendPropertyChange;
        private Telerik.Windows.Controls.TreeView.VirtualizationMode virtualizationMode;
        public static readonly DependencyProperty VirtualizationModeProperty = DependencyProperty.RegisterAttached("VirtualizationMode", typeof(Telerik.Windows.Controls.TreeView.VirtualizationMode), typeof(TreeViewPanel), new Telerik.Windows.PropertyMetadata(Telerik.Windows.Controls.TreeView.VirtualizationMode.Recycling));
        private int visibleCount;
        private int visibleStart;
        private bool vspIsVirtualizing;
        private const int WheelScrollLines = 3;
        private int widestIndex;

        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "This is how you register class event handlers")]
        static TreeViewPanel()
        {
            EventManager.RegisterClassHandler(typeof(TreeViewPanel), RequestBringIntoViewEvent, new RequestBringIntoViewEventHandler(TreeViewPanel.OnRequestBringIntoView));
        }

        public TreeViewPanel()
        {
            base.GotFocus += new RoutedEventHandler(this.OnGridViewVirtualizingPanelGotFocus);
            base.LostFocus += new RoutedEventHandler(this.OnGridViewVirtualizingPanelLostFocus);
            this.IsVisualCacheEnabled = true;
        }

        public static void AddCleanUpVirtualizedItemHandler(DependencyObject element, Telerik.Windows.Controls.TreeView.CleanUpVirtualizedItemEventHandler handler)
        {
            element.AddHandler(CleanUpVirtualizedItemEvent, handler);
        }

        private bool AddContainerFromGenerator(int childIndex, UIElement child, bool newlyRealized)
        {
            bool visualOrderChanged = false;
            if (!newlyRealized)
            {
                if (!this.InRecyclingMode)
                {
                    return visualOrderChanged;
                }
                IList children = this.RealizedChildren;
                if ((childIndex < children.Count) && (children[childIndex] == child))
                {
                    return visualOrderChanged;
                }
                bool isVisualCacheEnabled = this.IsVisualCacheEnabled;
                return this.InsertRecycledContainer(childIndex, child);
            }
            this.InsertNewContainer(childIndex, child);
            return visualOrderChanged;
        }

        private MeasureData AddFocusTrail(MeasureData measureData)
        {
            double page = this.ViewportHeight;
            Rect viewport = measureData.Viewport;
            if (DoubleUtil.AreClose(0.0, page) && measureData.HasViewport)
            {
                page = measureData.Viewport.Height;
            }
            measureData.Viewport = viewport;
            return measureData;
        }

        private void AdjustCacheWindow(int firstViewport, int itemCount)
        {
            int firstContainer = firstViewport;
            int lastContainer = (firstViewport + this.visibleCount) - 1;
            if (lastContainer >= itemCount)
            {
                lastContainer = itemCount - 1;
            }
            int cacheEnd = this.CacheEnd;
            if (firstContainer < this.cacheStart)
            {
                this.cacheStart = firstContainer;
            }
            else if (lastContainer > cacheEnd)
            {
                this.cacheStart += lastContainer - cacheEnd;
            }
        }

        private void AdjustFirstVisibleChildIndex(int startIndex, int count)
        {
            if (startIndex < this.firstVisibleChildIndex)
            {
                int endIndex = (startIndex + count) - 1;
                if (endIndex < this.firstVisibleChildIndex)
                {
                    this.firstVisibleChildIndex -= count;
                }
                else
                {
                    this.firstVisibleChildIndex = startIndex;
                }
            }
        }

        private MeasureData AdjustMeasureDataForAnimation(MeasureData measureData, Size constraint)
        {
            if (measureData == null)
            {
                return null;
            }
            if ((double.IsInfinity(measureData.AvailableSize.Height) && !double.IsInfinity(constraint.Height)) && this.IsVirtualizing)
            {
                this.EnsureIndexTree();
                double total = (this.indexTree.Count > 0) ? this.indexTree.CumulativeValue(this.indexTree.Count - 1) : 0.0;
                return new MeasureData(constraint, new Rect(measureData.Viewport.X, (measureData.Viewport.Y - constraint.Height) + total, measureData.Viewport.Width, measureData.Viewport.Height));
            }
            return measureData;
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "The instance is used for a Debug.Assert, it has to remain non-static.")]
        private MeasureData AdjustViewportOffset(MeasureData givenMeasureData, Telerik.Windows.Controls.ItemsControl itemsControl)
        {
            MeasureData newMeasureData = null;
            double offset = 24.0;
            if (givenMeasureData == null)
            {
                return newMeasureData;
            }
            Rect viewport = givenMeasureData.Viewport;
            IProvideStackingSize stackingSize = itemsControl as IProvideStackingSize;
            if (stackingSize != null)
            {
                offset = stackingSize.HeaderSize();
                if ((offset <= 0.0) || DoubleUtil.IsNaN(offset))
                {
                    offset = this.ContainerStackingSizeEstimate(stackingSize);
                }
            }
            viewport.Y -= offset;
            return new MeasureData(givenMeasureData.AvailableSize, viewport);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            IList children;
            Rect childRectangle = new Rect(new Point(), finalSize);
            this.EnsureScrollData();
            double previousChildSize = 0.0;
            Telerik.Windows.Controls.ItemsControl itemsControl = null;
            bool childrenAreContainers = true;
            bool bringIntoViewCotnainerIsArranged = false;
            FrameworkElement bringIntoView = this.BringIntoViewContainer;
            if (this.IsScrolling)
            {
                double offsetY = this.scrollData.ComputedOffset.Y;
                childRectangle.X = -1.0 * this.scrollData.ComputedOffset.X;
                childRectangle.Y = -offsetY;
            }
            if (this.IsVirtualizing)
            {
                itemsControl = System.Windows.Controls.ItemsControl.GetItemsOwner(this) as Telerik.Windows.Controls.ItemsControl;
                children = itemsControl.Items;
                childrenAreContainers = false;
            }
            else
            {
                children = this.RealizedChildren;
            }
            int childrenCount = (children != null) ? children.Count : 0;
            double arrangeOffset = 0.0;
            System.Windows.Controls.ItemContainerGenerator containerGenerator = base.ItemContainerGenerator as System.Windows.Controls.ItemContainerGenerator;
            if (childrenAreContainers)
            {
                for (int i = 0; i < childrenCount; i++)
                {
                    UIElement container = null;
                    container = (UIElement)children[i];
                    Size childSize = container.DesiredSize;
                    childRectangle.Y += previousChildSize;
                    previousChildSize = childSize.Height;
                    childRectangle.Height = previousChildSize;
                    childRectangle.Width = Math.Max(finalSize.Width, childSize.Width);
                    if (container != null)
                    {
                        container.Arrange(childRectangle);
                        SetIsMeasureValid(container, true);
                    }
                }
            }
            else
            {
                for (int i = 0; i < base.Children.Count; i++)
                {
                    UIElement container = base.Children[i];
                    if (container == null)
                    {
                        break;
                    }
                    Size childSize = container.DesiredSize;
                    int containerIndex = containerGenerator.IndexFromContainer(container);
                    if (container == bringIntoView)
                    {
                        bringIntoViewCotnainerIsArranged = true;
                    }
                    if (containerIndex > 0)
                    {
                        arrangeOffset = this.indexTree.CumulativeValue(containerIndex - 1) - this.scrollData.ComputedOffset.Y;
                    }
                    else
                    {
                        arrangeOffset = -this.scrollData.ComputedOffset.Y;
                    }
                    childRectangle.Width = Math.Max(finalSize.Width, childSize.Width);
                    childRectangle.Y = arrangeOffset;
                    childRectangle.Height = childSize.Height;
                    container.Arrange(childRectangle);
                    SetIsMeasureValid(container, true);
                }
            }
            if (bringIntoViewCotnainerIsArranged)
            {
                RadTreeViewItem treeViewItem = bringIntoView as RadTreeViewItem;
                if ((treeViewItem != null) && treeViewItem.IsInViewport)
                {
                    this.BringIntoViewContainer = null;
                }
            }
            return finalSize;
        }

        protected override void BringIndexIntoView(int index)
        {
            int childIndex;
            if ((index < 0) || (index >= this.ItemCount))
            {
                throw new ArgumentOutOfRangeException("index");
            }
            IItemContainerGenerator generator = base.ItemContainerGenerator;
            GeneratorPosition position = this.IndexToGeneratorPositionForStart(index, out childIndex);
            using (generator.StartAt(position, GeneratorDirection.Forward, true))
            {
                bool newlyRealized;
                UIElement child = generator.GenerateNext(out newlyRealized) as UIElement;
                if (this.IsVisualCacheEnabled)
                {
                    (child as ICachable).IsCaching = false;
                }
                if (child != null)
                {
                    this.AddContainerFromGenerator(childIndex, child, newlyRealized);
                    base.InvalidateMeasure();
                    base.InvalidateArrange();
                    FrameworkElement element = child as FrameworkElement;
                    if (element != null)
                    {
                        element.BringIntoView();
                        this.BringIntoViewContainer = element;
                        this.bringIntoViewIndex = index;
                    }
                }
            }
        }

        internal void BringIndexIntoViewInternal(int index)
        {
            this.BringIndexIntoView(index);
        }

        private int ChildIndexFromRealizedIndex(int realizedChildIndex)
        {
            if ((this.IsVirtualizing && this.InRecyclingMode) && (realizedChildIndex < this.RealizedChildren.Count))
            {
                UIElement child = this.realizedChildren[realizedChildIndex];
                UIElementCollection children = this.InternalChildren;
                for (int i = realizedChildIndex; i < children.Count; i++)
                {
                    if (children[i] == child)
                    {
                        return i;
                    }
                }
                if (this.IsVisualCacheEnabled)
                {
                    return base.Children.IndexOf(child);
                }
            }
            return realizedChildIndex;
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "The method can be refactored after virtualziation has been implemented.")]
        private void CleanupContainers(int firstViewport, Telerik.Windows.Controls.ItemsControl itemsControl)
        {
            if (itemsControl != null)
            {
                int cleanupRangeStart = -1;
                int cleanupCount = 0;
                int itemIndex = -1;
                IList children = this.RealizedChildren;
                int focusedChild = -1;
                int previousFocusable = -1;
                int nextFocusable = -1;
                bool performCleanup = false;
                if (children.Count != 0)
                {
                    int itemsCount = (itemsControl.Items != null) ? itemsControl.Items.Count : ((itemsControl.ItemContainerGenerator != null) ? itemsControl.Items.Count : 0);
                    this.AdjustCacheWindow(firstViewport, itemsCount);
                    RadTreeView treeView = null;
                    object selectedItem = null;
                    FrameworkElement bringIntoViewContainer = this.BringIntoViewContainer;
                    for (int childIndex = 0; childIndex < children.Count; childIndex++)
                    {
                        UIElement child = (UIElement)children[childIndex];
                        int lastItemIndex = itemIndex;
                        itemIndex = this.GetGeneratedIndex(childIndex);
                        if ((itemIndex - lastItemIndex) != 1)
                        {
                            performCleanup = true;
                        }
                        if (performCleanup)
                        {
                            if ((cleanupRangeStart >= 0) && (cleanupCount > 0))
                            {
                                this.CleanupRange(children, base.ItemContainerGenerator, cleanupRangeStart, cleanupCount);
                                childIndex -= cleanupCount;
                                focusedChild -= cleanupCount;
                                previousFocusable -= cleanupCount;
                                nextFocusable -= cleanupCount;
                                cleanupCount = 0;
                                cleanupRangeStart = -1;
                            }
                            performCleanup = false;
                        }
                        RadTreeViewItem treeViewItem = child as RadTreeViewItem;
                        if ((treeView == null) && (treeViewItem != null))
                        {
                            treeView = treeViewItem.ParentTreeView;
                            if (treeView != null)
                            {
                                selectedItem = treeView.SelectedItem;
                            }
                        }
                        if ((this.IsOutsideCacheWindow(itemIndex) && ((treeViewItem == null) || (((!treeViewItem.IsExpanded && !(treeViewItem.Item is RadTreeViewItem)) && (!treeViewItem.IsInEditMode && (treeViewItem.CheckState != ToggleState.Indeterminate))) && ((selectedItem == null) || (treeViewItem.Item != selectedItem))))) && ((((childIndex != focusedChild) && (childIndex != previousFocusable)) && ((childIndex != nextFocusable) && (childIndex != this.widestIndex))) && ((!IsInFocusTrail(child) && (child != bringIntoViewContainer)) && this.NotifyCleanupItem(child, itemsControl))))
                        {
                            if (cleanupRangeStart == -1)
                            {
                                cleanupRangeStart = childIndex;
                            }
                            cleanupCount++;
                            child.ClearValue(IsMeasureValidProperty);
                        }
                        else
                        {
                            performCleanup = true;
                        }
                    }
                    if ((cleanupRangeStart >= 0) && (cleanupCount > 0))
                    {
                        this.CleanupRange(children, base.ItemContainerGenerator, cleanupRangeStart, cleanupCount);
                    }
                }
            }
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "The method can be refactored after virtualziation has been implemented.")]
        private void CleanupContainersHierarchical(Telerik.Windows.Controls.ItemsControl itemsControl)
        {
            if (itemsControl != null)
            {
                int itemIndex = -1;
                IList children = this.RealizedChildren;
                int focusedChild = -1;
                int previousFocusable = -1;
                int nextFocusable = -1;
                if (children.Count != 0)
                {
                    RadTreeView treeView = null;
                    object selectedItem = null;
                    FrameworkElement bringIntoViewContainer = this.BringIntoViewContainer;
                    for (int childIndex = 0; childIndex < children.Count; childIndex++)
                    {
                        UIElement child = (UIElement)children[childIndex];
                        itemIndex = (base.ItemContainerGenerator as System.Windows.Controls.ItemContainerGenerator).IndexFromContainer(child);
                        RadTreeViewItem treeViewItem = child as RadTreeViewItem;
                        if ((treeView == null) && (treeViewItem != null))
                        {
                            treeView = treeViewItem.ParentTreeView;
                            if (treeView != null)
                            {
                                selectedItem = treeView.SelectedItem;
                            }
                        }
                        if ((((itemIndex != -1) && ((itemIndex < this.firstVisibleChildIndex) || (itemIndex > this.lastVisibleChildIndex))) && ((itemIndex < 0) && ((treeViewItem == null) || ((!treeViewItem.IsInEditMode && !(treeViewItem.Item is RadTreeViewItem)) && ((selectedItem == null) || (treeViewItem.Item != selectedItem)))))) && ((((childIndex != focusedChild) && (childIndex != previousFocusable)) && ((childIndex != nextFocusable) && (childIndex != this.widestIndex))) && ((!IsInFocusTrail(child) && (child != bringIntoViewContainer)) && this.NotifyCleanupItem(child, itemsControl))))
                        {
                            this.CleanupRange(children, base.ItemContainerGenerator, childIndex, 1);
                            SaveIndexTree(child, treeView);
                            child.ClearValue(IsMeasureValidProperty);
                        }
                    }
                }
            }
        }

        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "children", Justification = "The children parameter is used for a Debug.Assert, it cannot be removed.")]
        private void CleanupRange(IList children, IItemContainerGenerator generator, int startIndex, int count)
        {
            if (this.InRecyclingMode)
            {
                ((IRecyclingItemContainerGenerator)generator).Recycle(new GeneratorPosition(startIndex, 0), count);
                if (this.IsVisualCacheEnabled)
                {
                    this.RemoveInternalChildRangeOverride(startIndex, count);
                }
                else
                {
                    base.RemoveInternalChildRange(startIndex, count);
                    this.realizedChildren.RemoveRange(startIndex, count);
                }
            }
            else
            {
                base.RemoveInternalChildRange(startIndex, count);
                generator.Remove(new GeneratorPosition(startIndex, 0), count);
            }
            if (!this.InHierarchicalMode)
            {
                this.AdjustFirstVisibleChildIndex(startIndex, count);
            }
        }

        internal void ClearAllContainers(Telerik.Windows.Controls.ItemsControl itemsControl)
        {
            IItemContainerGenerator generator = base.ItemContainerGenerator;
            IList children = this.RealizedChildren;
            for (int i = 0; i < children.Count; i++)
            {
                UIElement child = (UIElement)children[i];
                int childIndex = itemsControl.ItemContainerGenerator.IndexFromContainer(child);
                this.indexTree[childIndex] = child.DesiredSize.Height;
            }
            if (generator != null)
            {
                generator.RemoveAll();
            }
            this.RealizedChildren.Clear();
            this.InternalChildren.Clear();
        }

        private void ClearBringIntoView()
        {
            this.bringIntoViewIndex = -1;
            this.BringIntoViewContainer = null;
            if (this.IsScrolling && (this.scrollData.BringIntoViewContainerReference != null))
            {
                this.scrollData.BringIntoViewContainerReference.Target = null;
            }
        }

        internal static double CoerceOffset(double offset, double extent, double viewport)
        {
            if (offset > (extent - viewport))
            {
                offset = extent - viewport;
            }
            if (offset < 0.0)
            {
                offset = 0.0;
            }
            return offset;
        }

        private int ComputeIndexOfFirstVisibleItem(MeasureData measureData, Telerik.Windows.Controls.ItemsControl itemsControl, out double firstItemOffset)
        {
            firstItemOffset = 0.0;
            if (itemsControl == null)
            {
                return 0;
            }
            if (measureData == null)
            {
                return 0;
            }
            double viewportOffset = measureData.Viewport.Y;
            int result = this.indexTree.CumulativeIndex(viewportOffset);
            if (result > 0)
            {
                firstItemOffset = this.indexTree.CumulativeValue(result - 1);
            }
            if (result < 0)
            {
                result = 0;
            }
            return result;
        }

        internal static double ComputeScrollOffsetWithMinimalScroll(double topView, double bottomView, double topChild, double bottomChild)
        {
            bool flag = DoubleUtil.LessThan(topChild, topView) && DoubleUtil.LessThan(bottomChild, bottomView);
            bool flag2 = DoubleUtil.GreaterThan(bottomChild, bottomView) && DoubleUtil.GreaterThan(topChild, topView);
            bool flag3 = (bottomChild - topChild) > (bottomView - topView);
            if (flag && !flag3)
            {
                return topChild;
            }
            if (flag2 && flag3)
            {
                return topChild;
            }
            if (!flag && !flag2)
            {
                return topView;
            }
            return (bottomChild - (bottomView - topView));
        }

        private static bool ContainerHasKeyboardFocusWithin(UIElement container)
        {
            RadTreeViewItem treeItem = container as RadTreeViewItem;
            return ((treeItem != null) && treeItem.IsKeyboardFocusWithin);
        }

        private double ContainerStackingSizeEstimate(IProvideStackingSize estimate)
        {
            double stackingSize = 0.0;
            if (estimate != null)
            {
                stackingSize = estimate.EstimatedContainerSize();
            }
            if ((stackingSize > 0.0) && !DoubleUtil.IsNaN(stackingSize))
            {
                return stackingSize;
            }
            return this.ChildDefaultLength;
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "The instance is used in a Debug.Assert, it has to remain non-static.")]
        private MeasureData CreateChildMeasureData(MeasureData measureData, Size layoutSlotSize, Size stackDesiredSize)
        {
            Rect viewport = measureData.Viewport;
            viewport.Y -= stackDesiredSize.Height;
            return new MeasureData(layoutSlotSize, viewport);
        }

        [Conditional("DEBUG")]
        private void DebugAssertRealizedChildrenEqualVisualChildren()
        {
            if (this.IsVirtualizing && this.InRecyclingMode)
            {
                UIElementCollection children = this.InternalChildren;
                for (int i = 0; i < children.Count; i++)
                {
                }
            }
        }

        [Conditional("DEBUG")]
        private void DebugVerifyRealizedChildren()
        {
            Telerik.Windows.Controls.ItemContainerGenerator generator = base.ItemContainerGenerator as Telerik.Windows.Controls.ItemContainerGenerator;
            Telerik.Windows.Controls.ItemsControl itemsControl = System.Windows.Controls.ItemsControl.GetItemsOwner(this) as Telerik.Windows.Controls.ItemsControl;
            if ((generator != null) && (itemsControl != null))
            {
                foreach (UIElement child in this.InternalChildren)
                {
                    int dataIndex = generator.IndexFromContainer(child);
                    if (dataIndex != -1)
                    {
                        base.ItemContainerGenerator.GeneratorPositionFromIndex(dataIndex);
                    }
                }
            }
        }

        private static void DelayedCleanUp(TreeViewPanel panel)
        {
            for (int index = 0; index < panel.Children.Count; index++)
            {
                ICachable cachable = panel.Children[index] as ICachable;
                if ((cachable != null) && cachable.IsCaching)
                {
                    panel.RemoveInternalChildRange(index, 1);
                    index--;
                }
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        private void DisconnectRecycledContainers()
        {
            if (!this.IsVisualCacheEnabled)
            {
                int realizedIndex = 0;
                UIElement realizedChild = (this.realizedChildren.Count > 0) ? this.realizedChildren[0] : null;
                UIElementCollection children = this.InternalChildren;
                for (int i = 0; i < children.Count; i++)
                {
                    UIElement visualChild = children[i];
                    if (visualChild == realizedChild)
                    {
                        realizedIndex++;
                        if (realizedIndex < this.realizedChildren.Count)
                        {
                            realizedChild = this.realizedChildren[realizedIndex];
                        }
                        else
                        {
                            realizedChild = null;
                        }
                    }
                    else
                    {
                        children.Remove(visualChild);
                        i--;
                    }
                }
            }
        }

        private void EnsureCleanupSet()
        {
            if (this.cleanupSet == null)
            {
                this.cleanupSet = new Dictionary<TreeViewPanel, bool>(0x20);
                this.cleanupTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3.0) };
                this.cleanupTimer.Tick += new EventHandler(this.OnCleanupTimerTick);
            }
        }

        private WeakReference[] EnsureFocusTrail()
        {
            WeakReference[] focusTrail = FocusTrailField.GetValue(this);
            if (focusTrail == null)
            {
                focusTrail = new WeakReference[2];
                FocusTrailField.SetValue(this, focusTrail);
            }
            return focusTrail;
        }

        private void EnsureIndexTree()
        {
            if (this.indexTree == null)
            {
                RadTreeViewItem itemsControl = System.Windows.Controls.ItemsControl.GetItemsOwner(this) as RadTreeViewItem;
                if ((itemsControl != null) && (itemsControl.ParentTreeView != null))
                {
                    this.indexTree = itemsControl.ParentTreeView.ItemStorage.GetValue<Telerik.Windows.Controls.TreeView.IndexTree>(itemsControl.Item, IndexTreeProperty, null);
                }
                if (this.indexTree == null)
                {
                    this.indexTree = new Telerik.Windows.Controls.TreeView.IndexTree(this.ItemCount, this.ChildDefaultLength);
                }
            }
        }

        private void EnsureRealizedChildren()
        {
            if (this.realizedChildren == null)
            {
                UIElementCollection children = this.InternalChildren;
                this.realizedChildren = new List<UIElement>(children.Count);
                for (int i = 0; i < children.Count; i++)
                {
                    this.realizedChildren.Add(children[i]);
                }
            }
        }

        private void EnsureScrollData()
        {
            if (this.scrollData == null)
            {
                this.scrollData = new ScrollData();
            }
        }

        private Size ExtendDesiredSize(Telerik.Windows.Controls.ItemsControl itemsControl, Size stackDesiredSize, int pivotIndex, bool before)
        {
            IList items = itemsControl.Items;
            int itemsCount = (items != null) ? items.Count : 0;
            if (itemsCount == 0)
            {
                return new Size();
            }
            double cumulativeUpToPivot = 0.0;
            if (pivotIndex >= 1)
            {
                cumulativeUpToPivot = this.indexTree.CumulativeValue(pivotIndex - 1);
            }
            double result = 0.0;
            if (before)
            {
                result = cumulativeUpToPivot;
            }
            else
            {
                result = this.indexTree.CumulativeValue(itemsCount - 1);
            }
            return new Size(stackDesiredSize.Width, result);
        }

        private void FocusChanged()
        {
            if (this.IsVirtualizing && this.IsScrolling)
            {
                WeakReference[] focusTrail = this.EnsureFocusTrail();
                for (int i = 0; i < 2; i++)
                {
                    DependencyObject trailItem = (focusTrail[i] != null) ? ((DependencyObject)focusTrail[i].Target) : null;
                    if (trailItem != null)
                    {
                        FocusTrailItemField.ClearValue(trailItem);
                    }
                }
                if (this.IsKeyboardFocusWithin)
                {
                    RadTreeViewItem focusedItem = (FocusManager.GetFocusedElement() as UIElement).ParentOfType<RadTreeViewItem>();
                    DependencyObject previousFocusElement = null;
                    DependencyObject nextFocusElement = null;
                    if (focusedItem != null)
                    {
                        previousFocusElement = focusedItem.PredictFocusInternal(Telerik.Windows.Controls.TreeView.FocusNavigationDirection.Up);
                        nextFocusElement = focusedItem.PredictFocusInternal(Telerik.Windows.Controls.TreeView.FocusNavigationDirection.Down);
                    }
                    if (previousFocusElement != null)
                    {
                        FocusTrailItemField.SetValue(previousFocusElement, true);
                        focusTrail[0] = new WeakReference(previousFocusElement);
                    }
                    if (nextFocusElement != null)
                    {
                        FocusTrailItemField.SetValue(nextFocusElement, true);
                        focusTrail[1] = new WeakReference(nextFocusElement);
                    }
                }
                else
                {
                    FocusTrailField.SetValue(this, null);
                }
            }
        }

        private int GetGeneratedIndex(int childIndex)
        {
            return base.ItemContainerGenerator.IndexFromGeneratorPosition(new GeneratorPosition(childIndex, 0));
        }

        private static bool GetIsMeasureValid(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsMeasureValidProperty);
        }

        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Virtualizing", Justification = "The spelling is correct.")]
        public static bool GetIsVirtualizing(DependencyObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return (bool)element.GetValue(IsVirtualizingProperty);
        }

        internal static MeasureData GetMeasureData(DependencyObject obj)
        {
            if (obj != null)
            {
                return (MeasureData)obj.GetValue(MeasureDataProperty);
            }
            return null;
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "TransformToVisual could throw an exception.")]
        internal static Rect GetRectangle(DependencyObject element)
        {
            UIElement d = element as UIElement;
            if ((d != null) && GetIsMeasureValid(element))
            {
                UIElement visualRoot = Application.Current.RootVisual;
                if (visualRoot != null)
                {
                    GeneralTransform transform = null;
                    try
                    {
                        transform = d.TransformToVisual(visualRoot);
                    }
                    catch
                    {
                        return Rect.Empty;
                    }
                    double x = 0.0;
                    double y = 0.0;
                    double width = d.RenderSize.Width;
                    double height = d.RenderSize.Height;
                    if (width < 0.0)
                    {
                        x = d.RenderSize.Width * 0.5;
                        width = 0.0;
                    }
                    if (height < 0.0)
                    {
                        y = d.RenderSize.Height * 0.5;
                        height = 0.0;
                    }
                    return transform.TransformBounds(new Rect(x, y, width, height));
                }
            }
            return Rect.Empty;
        }

        public static Telerik.Windows.Controls.TreeView.VirtualizationMode GetVirtualizationMode(DependencyObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return (Telerik.Windows.Controls.TreeView.VirtualizationMode)element.GetValue(VirtualizationModeProperty);
        }

        private GeneratorPosition IndexToGeneratorPositionForStart(int index, out int childIndex)
        {
            IItemContainerGenerator generator = base.ItemContainerGenerator;
            GeneratorPosition position = (generator != null) ? generator.GeneratorPositionFromIndex(index) : new GeneratorPosition(-1, index + 1);
            childIndex = (position.Offset == 0) ? position.Index : (position.Index + 1);
            return position;
        }

        private bool InsertContainer(int childIndex, UIElement container, bool isRecycled)
        {
            this.EnsureRealizedChildren();
            bool visualOrderChanged = false;
            UIElementCollection children = this.InternalChildren;
            int visualTreeIndex = 0;
            if (childIndex > 0)
            {
                visualTreeIndex = this.ChildIndexFromRealizedIndex(childIndex - 1) + 1;
            }
            if ((!isRecycled || (visualTreeIndex >= children.Count)) || (children[visualTreeIndex] != container))
            {
                if (visualTreeIndex < children.Count)
                {
                    int insertIndex = visualTreeIndex;
                    if (isRecycled && (VisualTreeHelper.GetParent(container) != null))
                    {
                        if (!this.IsVisualCacheEnabled)
                        {
                            int num3 = children.IndexOf(container);
                            children.RemoveAt(num3);
                            if (visualTreeIndex > num3)
                            {
                                visualTreeIndex--;
                            }
                            children.Insert(visualTreeIndex, container);
                            visualOrderChanged = true;
                        }
                    }
                    else
                    {
                        base.InsertInternalChild(insertIndex, container);
                    }
                }
                else if (isRecycled && (VisualTreeHelper.GetParent(container) != null))
                {
                    if (!this.IsVisualCacheEnabled)
                    {
                        children.Remove(container);
                        children.Add(container);
                    }
                    visualOrderChanged = true;
                }
                else
                {
                    base.AddInternalChild(container);
                }
            }
            if (this.IsVirtualizing && this.InRecyclingMode)
            {
                this.realizedChildren.Insert(childIndex, container);
            }
            base.ItemContainerGenerator.PrepareItemContainer(container);
            return visualOrderChanged;
        }

        private void InsertNewContainer(int childIndex, UIElement container)
        {
            this.InsertContainer(childIndex, container, false);
        }

        private bool InsertRecycledContainer(int childIndex, UIElement container)
        {
            return this.InsertContainer(childIndex, container, true);
        }

        private static bool IsInFocusTrail(UIElement container)
        {
            bool containerHasKeyboardFocusWithin = ContainerHasKeyboardFocusWithin(container);
            if (!FocusTrailItemField.GetValue(container))
            {
                return containerHasKeyboardFocusWithin;
            }
            return true;
        }

        private bool IsInViewport(DependencyObject element)
        {
            DependencyObject visualParent = this;
            SetIsMeasureValid(visualParent, true);
            Rect rectangle = GetRectangle(visualParent);
            Rect rect = GetRectangle(element);
            return rectangle.IntersectsWith(rect);
        }

        private bool IsOutsideCacheWindow(int itemIndex)
        {
            if (itemIndex >= this.cacheStart)
            {
                return (itemIndex > this.CacheEnd);
            }
            return true;
        }

        internal static bool IsValidOrientation(object orientationValue)
        {
            System.Windows.Controls.Orientation orientation = (System.Windows.Controls.Orientation)orientationValue;
            if (orientation != System.Windows.Controls.Orientation.Horizontal)
            {
                return (orientation == System.Windows.Controls.Orientation.Vertical);
            }
            return true;
        }

        public virtual void LineDown()
        {
            this.SetVerticalOffset(this.VerticalOffset + 24.0);
        }

        public virtual void LineLeft()
        {
            this.SetHorizontalOffset(this.HorizontalOffset - 24.0);
        }

        public virtual void LineRight()
        {
            this.SetHorizontalOffset(this.HorizontalOffset + 24.0);
        }

        public virtual void LineUp()
        {
            this.SetVerticalOffset(this.VerticalOffset - 24.0);
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "TransformToVisual throws a general exception.")]
        public Rect MakeVisible(UIElement visual, Rect rectangle)
        {
            Vector newOffset = new Vector();
            Rect newRect = new Rect();
            Rect originalRect = rectangle;
            if ((rectangle.IsEmpty || (visual == null)) || ((visual == this) || !this.IsAncestorOf(visual)))
            {
                return Rect.Empty;
            }
            Point point = new Point();
            try
            {
                point = visual.TransformToVisual(this).Transform(new Point(rectangle.X, rectangle.Y));
            }
            catch
            {
            }
            rectangle.X = point.X;
            rectangle.Y = point.Y;
            if (!this.IsScrolling)
            {
                return rectangle;
            }
            this.MakeVisiblePhysicalHelper(rectangle, ref newOffset, ref newRect, true);
            this.MakeVisiblePhysicalHelper(rectangle, ref newOffset, ref newRect, false);
            newOffset.X = CoerceOffset(newOffset.X, this.scrollData.Extent.Width, this.scrollData.Viewport.Width);
            newOffset.Y = CoerceOffset(newOffset.Y, this.scrollData.Extent.Height, this.scrollData.Viewport.Height);
            if (!DoubleUtil.AreClose(newOffset, this.scrollData.Offset))
            {
                this.scrollData.Offset = newOffset;
                base.InvalidateMeasure();
                this.OnScrollChange();
                if (this.ScrollOwner != null)
                {
                    FrameworkElement element = visual as FrameworkElement;
                    if (element != null)
                    {
                        element.BringIntoView(originalRect);
                    }
                }
            }
            return newRect;
        }

        private void MakeVisiblePhysicalHelper(Rect rectangle, ref Vector newOffset, ref Rect newRect, bool isHorizontal)
        {
            double viewportOffset;
            double viewportSize;
            double targetRectOffset;
            double targetRectSize;
            if (isHorizontal)
            {
                viewportOffset = this.scrollData.ComputedOffset.X;
                viewportSize = this.ViewportWidth;
                targetRectOffset = rectangle.X;
                targetRectSize = rectangle.Width;
            }
            else
            {
                viewportOffset = this.scrollData.ComputedOffset.Y;
                viewportSize = this.ViewportHeight;
                targetRectOffset = rectangle.Y;
                targetRectSize = rectangle.Height;
            }
            targetRectOffset += viewportOffset;
            double minPhysicalOffset = ComputeScrollOffsetWithMinimalScroll(viewportOffset, viewportOffset + viewportSize, targetRectOffset, targetRectOffset + targetRectSize);
            double left = Math.Max(targetRectOffset, minPhysicalOffset);
            targetRectSize = Math.Max((double)(Math.Min((double)(targetRectSize + targetRectOffset), (double)(minPhysicalOffset + viewportSize)) - left), (double)0.0);
            targetRectOffset = left;
            targetRectOffset -= viewportOffset;
            if (isHorizontal)
            {
                newOffset.X = minPhysicalOffset;
                newRect.X = targetRectOffset;
                newRect.Width = targetRectSize;
            }
            else
            {
                newOffset.Y = minPhysicalOffset;
                newRect.Y = targetRectOffset;
                newRect.Height = targetRectSize;
            }
        }

        [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#", Justification = "Available size exists as field as well."), SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Indeed, it should eb refactored."), SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "The method can be simplified at a later stage.")]
        protected override Size MeasureOverride(Size constraint)
        {
            double firstItemOffset;
            this.availableSize = constraint;
            this.EnsureIndexTree();
            this.SignUpForContentPresenter();
            Size stackDesiredSize = new Size();
            Size layoutSlotSize = constraint;
            double virtualizedItemsSize = 0.0;
            int lastViewport = -1;
            Rect originalViewport = Rect.Empty;
            Telerik.Windows.Controls.ItemsControl itemsControl = System.Windows.Controls.ItemsControl.GetItemsOwner(this) as Telerik.Windows.Controls.ItemsControl;
            MeasureData measureData = GetMeasureData(itemsControl);
            measureData = this.AdjustMeasureDataForAnimation(measureData, constraint);
            if (!this.IsScrolling && (measureData == null))
            {
                this.EnsureIndexTree();
                if (this.indexTree.Count == 0)
                {
                    return new Size();
                }
                return new Size(100.0, this.indexTree.CumulativeValue(this.indexTree.Count - 1));
            }
            int itemCount = this.ItemCount;
            this.SetVirtualizationState(itemsControl, (measureData != null) && measureData.HasViewport);
            IList realizedChildren = this.RealizedChildren;
            IItemContainerGenerator generator = base.ItemContainerGenerator;
            if (this.IsScrolling)
            {
                originalViewport = new Rect(this.scrollData.Offset.X, this.scrollData.Offset.Y, constraint.Width, constraint.Height);
                if (this.scrollData.BringIntoViewContainerReference == null)
                {
                    this.scrollData.BringIntoViewContainerReference = new WeakReference(null);
                }
                measureData = new MeasureData(constraint, originalViewport);
                measureData = this.AddFocusTrail(measureData);
            }
            else
            {
                measureData = this.AdjustViewportOffset(measureData, itemsControl);
            }
            layoutSlotSize.Height = double.PositiveInfinity;
            if (this.IsScrolling && this.CanHorizontallyScroll)
            {
                layoutSlotSize.Width = double.PositiveInfinity;
            }
            int firstViewport = this.ComputeIndexOfFirstVisibleItem(measureData, itemsControl, out firstItemOffset);
            this.firstVisibleChildIndex = firstViewport;
            stackDesiredSize = this.ExtendDesiredSize(itemsControl, stackDesiredSize, firstViewport, true);
            virtualizedItemsSize = stackDesiredSize.Height;
            if (this.IsVirtualizing && this.InRecyclingMode)
            {
                this.CleanupContainers(firstViewport, itemsControl);
            }
            int childIndex = this.firstVisibleChildIndex;
            GeneratorPosition startPos = this.IndexToGeneratorPositionForStart(this.IsVirtualizing ? firstViewport : 0, out childIndex);
            if (this.IsVirtualizing && this.InRecyclingMode)
            {
                foreach (UIElement child in this.RealizedChildren)
                {
                    int index = itemsControl.ItemContainerGenerator.IndexFromContainer(child);
                    if (index < firstViewport)
                    {
                        Size calculatedDesiredSize = new Size(layoutSlotSize.Width, (index == 0) ? 0.0 : this.indexTree.CumulativeValue(index - 1));
                        MeasureData childMeasureData = this.CreateChildMeasureData(measureData, layoutSlotSize, calculatedDesiredSize);
                        SetMeasureData(child, childMeasureData);
                        RadTreeViewItem treeViewItem = child as RadTreeViewItem;
                        if (((treeViewItem != null) && treeViewItem.IsExpanded) && treeViewItem.HasItems)
                        {
                            child.InvalidateMeasure();
                        }
                        Size childDesiredSize = child.DesiredSize;
                        child.Measure(layoutSlotSize);
                        if (childDesiredSize != child.DesiredSize)
                        {
                            childDesiredSize = child.DesiredSize;
                        }
                        double length = childDesiredSize.Height;
                        this.indexTree[index] = length;
                    }
                }
            }
            this.visibleCount = 0;
            if (itemCount > 0)
            {
                using (generator.StartAt(startPos, GeneratorDirection.Forward, true))
                {
                    int i = this.IsVirtualizing ? firstViewport : 0;
                    int count = itemCount;
                    while (i < count)
                    {
                        bool newlyRealized;
                        UIElement child = generator.GenerateNext(out newlyRealized) as UIElement;
                        if (this.IsVisualCacheEnabled)
                        {
                            (child as ICachable).IsCaching = false;
                        }
                        if (child == null)
                        {
                            break;
                        }
                        this.AddContainerFromGenerator(childIndex, child, newlyRealized);
                        childIndex++;
                        this.visibleCount++;
                        MeasureData childMeasureData = this.CreateChildMeasureData(measureData, layoutSlotSize, stackDesiredSize);
                        SetMeasureData(child, childMeasureData);
                        RadTreeViewItem treeViewItem = child as RadTreeViewItem;
                        if (((treeViewItem != null) && treeViewItem.IsExpanded) && treeViewItem.HasItems)
                        {
                            child.InvalidateMeasure();
                        }
                        Size childDesiredSize = child.DesiredSize;
                        child.Measure(layoutSlotSize);
                        if (childDesiredSize != child.DesiredSize)
                        {
                            childDesiredSize = child.DesiredSize;
                        }
                        double length = childDesiredSize.Height;
                        double width = childDesiredSize.Width;
                        if (length != 0.0)
                        {
                            this.indexTree[i] = length;
                        }
                        stackDesiredSize.Width = Math.Max(stackDesiredSize.Width, childDesiredSize.Width);
                        stackDesiredSize.Height += childDesiredSize.Height;
                        if (i == this.widestIndex)
                        {
                            this.maxWidth = stackDesiredSize.Width;
                        }
                        if (width > this.maxWidth)
                        {
                            this.maxWidth = width;
                            this.widestIndex = i;
                        }
                        lastViewport = i;
                        if (this.IsVirtualizing && (i >= firstViewport))
                        {
                            double viewportSize = measureData.Viewport.Height;
                            double totalGenerated = ((stackDesiredSize.Height - virtualizedItemsSize) + firstItemOffset) - measureData.Viewport.Y;
                            if (totalGenerated > (viewportSize + 24.0))
                            {
                                break;
                            }
                        }
                        i++;
                    }
                }
            }
            this.lastVisibleChildIndex = lastViewport;
            if (this.IsVirtualizing && this.InRecyclingMode)
            {
                foreach (UIElement child in this.RealizedChildren)
                {
                    int index = itemsControl.ItemContainerGenerator.IndexFromContainer(child);
                    if (index > lastViewport)
                    {
                        Size calculatedDesiredSize = new Size(layoutSlotSize.Width, (index == 0) ? 0.0 : this.indexTree.CumulativeValue(index - 1));
                        MeasureData childMeasureData = this.CreateChildMeasureData(measureData, layoutSlotSize, calculatedDesiredSize);
                        SetMeasureData(child, childMeasureData);
                        RadTreeViewItem treeViewItem = child as RadTreeViewItem;
                        if (((treeViewItem != null) && treeViewItem.IsExpanded) && treeViewItem.HasItems)
                        {
                            child.InvalidateMeasure();
                        }
                        Size childDesiredSize = child.DesiredSize;
                        child.Measure(layoutSlotSize);
                        if (childDesiredSize != child.DesiredSize)
                        {
                            childDesiredSize = child.DesiredSize;
                        }
                        double length = childDesiredSize.Height;
                        if (length != 0.0)
                        {
                            this.indexTree[index] = length;
                        }
                    }
                }
            }
            FrameworkElement bringIntoViewFramework = this.BringIntoViewContainer;
            if (bringIntoViewFramework != null)
            {
                if (((this.bringIntoViewIndex >= firstViewport) && (this.bringIntoViewIndex <= lastViewport)) && this.IsInViewport(bringIntoViewFramework))
                {
                    this.BringIntoViewContainer = null;
                }
                else
                {
                    bringIntoViewFramework.BringIntoView();
                }
            }
            this.visibleStart = firstViewport;
            int pivotIndex = Math.Min(itemsControl.Items.Count, firstViewport + this.visibleCount);
            stackDesiredSize = this.ExtendDesiredSize(itemsControl, stackDesiredSize, pivotIndex, false);
            stackDesiredSize.Width = Math.Max(this.maxWidth, stackDesiredSize.Width);
            if (this.IsScrolling)
            {
                Vector offset = new Vector(originalViewport.X, originalViewport.Y);
                if (this.SetAndVerifyScrollingData(new Size(originalViewport.Width, originalViewport.Height), stackDesiredSize, offset))
                {
                    this.MeasureOverride(constraint);
                }
            }
            if (this.IsVirtualizing && !this.InRecyclingMode)
            {
                if (this.InHierarchicalMode)
                {
                    this.CleanupContainersHierarchical(itemsControl);
                }
                else
                {
                    this.CleanupContainers(firstViewport, itemsControl);
                }
            }
            if (this.IsVirtualizing && this.InRecyclingMode)
            {
                this.DisconnectRecycledContainers();
            }
            return stackDesiredSize;
        }

        public virtual void MouseWheelDown()
        {
            this.SetVerticalOffset(this.VerticalOffset + 72.0);
        }

        public virtual void MouseWheelLeft()
        {
            this.SetHorizontalOffset(this.HorizontalOffset - 72.0);
        }

        public virtual void MouseWheelRight()
        {
            this.SetHorizontalOffset(this.HorizontalOffset + 72.0);
        }

        public virtual void MouseWheelUp()
        {
            this.SetVerticalOffset(this.VerticalOffset - 72.0);
        }

        private bool NotifyCleanupItem(UIElement child, Telerik.Windows.Controls.ItemsControl itemsControl)
        {
            if (!this.IsScrolling && this.IsVisualCacheEnabled)
            {
                RadTreeViewItem treeViewItem = itemsControl as RadTreeViewItem;
                if (treeViewItem != null)
                {
                    RadTreeView treeView = treeViewItem.ParentTreeView;
                    if ((treeView != null) && (treeView.ItemsHost != null))
                    {
                        treeView.ItemsHost.RegisterForDelayedCleanup(this);
                    }
                }
            }
            return true;
        }

        private void OnCleanupTimerTick(object sender, EventArgs e)
        {
            DelayedCleanUp(this);
            foreach (TreeViewPanel panel in this.cleanupSet.Keys)
            {
                DelayedCleanUp(panel);
            }
            this.cleanupSet.Clear();
            this.cleanupTimer.Stop();
        }

        protected virtual void OnCleanUpVirtualizedItem(Telerik.Windows.Controls.TreeView.CleanUpVirtualizedItemEventArgs e)
        {
            Telerik.Windows.Controls.ItemsControl itemsControl = System.Windows.Controls.ItemsControl.GetItemsOwner(this) as Telerik.Windows.Controls.ItemsControl;
            if (itemsControl != null)
            {
                itemsControl.RaiseEvent(e);
            }
        }

        protected override void OnClearChildren()
        {
            base.OnClearChildren();
            this.realizedChildren = null;
            this.visibleStart = this.firstVisibleChildIndex = this.visibleCount = 0;
        }

        private void OnGridViewVirtualizingPanelGotFocus(object sender, RoutedEventArgs e)
        {
            DependencyObject focusedElement = FocusManager.GetFocusedElement() as DependencyObject;
            this.IsKeyboardFocusWithin = (focusedElement != null) && this.IsAncestorOf(focusedElement);
            this.FocusChanged();
        }

        private void OnGridViewVirtualizingPanelLostFocus(object sender, RoutedEventArgs e)
        {
            DependencyObject focusedElement = FocusManager.GetFocusedElement() as DependencyObject;
            this.IsKeyboardFocusWithin = (focusedElement != null) && this.IsAncestorOf(focusedElement);
            if (!this.IsKeyboardFocusWithin)
            {
                this.FocusChanged();
            }
        }

        protected override void OnItemsChanged(object sender, ItemsChangedEventArgs args)
        {
            int itemIndex;
            base.OnItemsChanged(sender, args);
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    this.EnsureIndexTree();
                    this.indexTree.Insert(base.ItemContainerGenerator.IndexFromGeneratorPosition(args.Position), this.ChildDefaultLength);
                    goto Label_013A;

                case NotifyCollectionChangedAction.Remove:
                    {
                        itemIndex = args.Position.Offset + args.Position.Index;
                        System.Windows.Controls.ItemsControl itemsOwner = System.Windows.Controls.ItemsControl.GetItemsOwner(this);
                        if ((itemsOwner == null) || (this.bringIntoViewIndex < itemsOwner.Items.Count))
                        {
                            if (this.bringIntoViewIndex == itemIndex)
                            {
                                this.ClearBringIntoView();
                            }
                            if ((this.bringIntoViewIndex >= 0) && (this.bringIntoViewIndex > itemIndex))
                            {
                                this.bringIntoViewIndex -= args.ItemCount;
                            }
                            break;
                        }
                        this.ClearBringIntoView();
                        break;
                    }
                case NotifyCollectionChangedAction.Replace:
                    this.EnsureIndexTree();
                    this.OnItemsReplace(args);
                    goto Label_013A;

                case NotifyCollectionChangedAction.Reset:
                    this.indexTree = null;
                    base.Children.Clear();
                    if (this.realizedChildren != null)
                    {
                        this.realizedChildren.Clear();
                    }
                    this.widestIndex = -1;
                    this.maxWidth = 0.0;
                    goto Label_013A;

                default:
                    goto Label_013A;
            }
            this.OnItemsRemove(args);
            if (this.indexTree != null)
            {
                this.indexTree.RemoveAt(itemIndex);
            }
        Label_013A:
            base.InvalidateMeasure();
        }

        private void OnItemsMove(ItemsChangedEventArgs args)
        {
            this.RemoveChildRange(args.OldPosition, args.ItemCount, args.ItemUICount);
        }

        private void OnItemsRemove(ItemsChangedEventArgs args)
        {
            this.RemoveChildRange(args.Position, args.ItemCount, args.ItemUICount);
        }

        private void OnItemsReplace(ItemsChangedEventArgs args)
        {
            this.RemoveChildRange(args.Position, args.ItemCount, args.ItemUICount);
        }

        private static void OnOrientationChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            TreeViewPanel panel = sender as TreeViewPanel;
            if (!panel.suspendPropertyChange)
            {
                panel.suspendPropertyChange = false;
            }
            else
            {
                if (!IsValidOrientation(e.NewValue))
                {
                    panel.suspendPropertyChange = true;
                    panel.SetValue(OrientationProperty, e.OldValue);
                }
                ResetScrolling(panel);
            }
        }

        private static void OnRequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {
            TreeViewPanel ancestor = sender as TreeViewPanel;
            UIElement targetObject = e.TargetObject as UIElement;
            if (((targetObject != null) && (targetObject != ancestor)) && ancestor.IsAncestorOf(targetObject))
            {
                e.Handled = ancestor.IsScrolling;
                Rect targetRect = e.TargetRect;
                if (targetRect.IsEmpty)
                {
                    targetRect = new Rect(new Point(), targetObject.RenderSize);
                }
                ancestor.MakeVisible(targetObject, targetRect);
            }
        }

        private void OnScrollChange()
        {
            if (this.ScrollOwner != null)
            {
                this.ScrollOwner.InvalidateScrollInfo();
            }
        }

        internal void OnTreeViewItemCollapsed(int index)
        {
            if (this.IsVirtualizing)
            {
                this.EnsureIndexTree();
                if ((index >= 0) && (index < this.indexTree.Count))
                {
                    this.indexTree[index] = this.ChildDefaultLength;
                }
                if (this.widestIndex == index)
                {
                    this.widestIndex = -1;
                    this.maxWidth = 0.0;
                }
                base.Dispatcher.BeginInvoke(delegate
                {
                    base.InvalidateMeasure();
                });
            }
        }

        internal void OnViewportOffsetChanged()
        {
            base.InvalidateMeasure();
        }

        protected virtual void OnViewportSizeChanged(Size oldViewportSize, Size newViewportSize)
        {
        }

        public virtual void PageDown()
        {
            this.SetVerticalOffset(this.VerticalOffset + this.ViewportHeight);
        }

        public virtual void PageLeft()
        {
            this.SetHorizontalOffset(this.HorizontalOffset - this.ViewportWidth);
        }

        public virtual void PageRight()
        {
            this.SetHorizontalOffset(this.HorizontalOffset + this.ViewportWidth);
        }

        public virtual void PageUp()
        {
            this.SetVerticalOffset(this.VerticalOffset - this.ViewportHeight);
        }

        private void RegisterForDelayedCleanup(TreeViewPanel treeViewPanel)
        {
            this.EnsureCleanupSet();
            this.cleanupSet[treeViewPanel] = true;
            this.cleanupTimer.Stop();
            this.cleanupTimer.Start();
        }

        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "itemCount", Justification = "ItemCount is used in a Debug.Assert, the parameter cannot be removed.")]
        private void RemoveChildRange(GeneratorPosition position, int itemCount, int itemUICount)
        {
            UIElementCollection children = this.InternalChildren;
            int pos = position.Index;
            if (position.Offset > 0)
            {
                pos++;
            }
            if (pos < children.Count)
            {
                int uiCount = itemUICount;
                if (uiCount > 0)
                {
                    if (this.IsVisualCacheEnabled)
                    {
                        this.RemoveInternalChildRangeOverride(pos, uiCount);
                    }
                    else
                    {
                        base.RemoveInternalChildRange(pos, uiCount);
                        if (this.IsVirtualizing && this.InRecyclingMode)
                        {
                            this.realizedChildren.RemoveRange(pos, uiCount);
                        }
                    }
                }
            }
        }

        public static void RemoveCleanUpVirtualizedItemHandler(DependencyObject element, Telerik.Windows.Controls.TreeView.CleanUpVirtualizedItemEventHandler handler)
        {
            element.RemoveHandler(CleanUpVirtualizedItemEvent, handler);
        }

        private void RemoveInternalChildRangeOverride(int startIndex, int count)
        {
            if (this.realizedChildren != null)
            {
                if (this.IsVisualCacheEnabled)
                {
                    int endIndex = startIndex + count;
                    for (int index = startIndex; index < endIndex; index++)
                    {
                        UIElement child = this.realizedChildren[index];
                        (child as ICachable).IsCaching = true;
                    }
                }
                else
                {
                    base.RemoveInternalChildRange(startIndex, count);
                }
                this.realizedChildren.RemoveRange(startIndex, count);
            }
        }

        private static void ResetScrolling(TreeViewPanel element)
        {
            element.InvalidateMeasure();
            if (element.IsScrolling)
            {
                element.scrollData.ClearLayout();
            }
        }

        private static void SaveIndexTree(UIElement child, RadTreeView treeView)
        {
            RadTreeViewItem treeViewItem = child as RadTreeViewItem;
            if (((treeView != null) && (treeViewItem != null)) && (treeViewItem.ItemsHost != null))
            {
                treeView.ItemStorage.SetValue(treeViewItem.Item, IndexTreeProperty, treeViewItem.ItemsHost.indexTree);
            }
        }

        private void ScrollPresenterSizeChanged(object sender, SizeChangedEventArgs e)
        {
            base.InvalidateMeasure();
        }

        private bool SetAndVerifyScrollingData(Size viewport, Size extent, Vector offset)
        {
            bool result = false;
            offset.X = CoerceOffset(offset.X, extent.Width, viewport.Width);
            offset.Y = CoerceOffset(offset.Y, extent.Height, viewport.Height);
            bool viewportChanged = !DoubleUtil.AreClose(viewport, this.scrollData.Viewport);
            bool extentChanged = !DoubleUtil.AreClose(extent, this.scrollData.Extent);
            bool offsetChanged = !DoubleUtil.AreClose(offset, this.scrollData.ComputedOffset);
            this.scrollData.Offset = offset;
            if ((viewportChanged || extentChanged) || offsetChanged)
            {
                Size oldViewportSize = this.scrollData.Viewport;
                this.scrollData.Viewport = viewport;
                this.scrollData.Extent = extent;
                this.scrollData.ComputedOffset = offset;
                if (viewportChanged)
                {
                    this.OnViewportSizeChanged(oldViewportSize, viewport);
                }
                if (offsetChanged)
                {
                    this.OnViewportOffsetChanged();
                    result = true;
                }
                this.OnScrollChange();
            }
            return result;
        }

        public void SetHorizontalOffset(double offset)
        {
            this.EnsureScrollData();
            double scrollX = ValidateInputOffset(offset, "HorizontalOffset");
            if (!DoubleUtil.AreClose(scrollX, this.scrollData.Offset.X))
            {
                this.scrollData.Offset = new Vector(scrollX, this.scrollData.Offset.Y);
                this.OnViewportOffsetChanged();
                base.InvalidateMeasure();
            }
        }

        private static void SetIsMeasureValid(DependencyObject obj, bool value)
        {
            obj.SetValue(IsMeasureValidProperty, value);
        }

        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Virtualizing", Justification = "The spelling is correct.")]
        public static void SetIsVirtualizing(DependencyObject element, bool value)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            element.SetValue(IsVirtualizingProperty, value);
        }

        private static void SetMeasureData(DependencyObject obj, MeasureData value)
        {
            obj.SetValue(MeasureDataProperty, value);
        }

        public void SetVerticalOffset(double offset)
        {
            this.EnsureScrollData();
            double scrollY = ValidateInputOffset(offset, "VerticalOffset");
            if (!DoubleUtil.AreClose(scrollY, this.scrollData.Offset.Y))
            {
                this.scrollData.Offset = new Vector(this.scrollData.Offset.X, scrollY);
                this.OnViewportOffsetChanged();
            }
        }

        public static void SetVirtualizationMode(DependencyObject element, Telerik.Windows.Controls.TreeView.VirtualizationMode value)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            element.SetValue(VirtualizationModeProperty, value);
        }

        private void SetVirtualizationState(Telerik.Windows.Controls.ItemsControl itemsControl, bool hasMeasureData)
        {
            Telerik.Windows.Controls.TreeView.VirtualizationMode mode = (itemsControl != null) ? GetVirtualizationMode(itemsControl) : Telerik.Windows.Controls.TreeView.VirtualizationMode.Standard;
            this.IsVirtualizing = ((itemsControl != null) && GetIsVirtualizing(itemsControl)) && (this.IsScrolling || hasMeasureData);
            if (this.HasMeasured)
            {
                if (this.VirtualizationMode != mode)
                {
                    throw new InvalidOperationException(Telerik.Windows.Controls.SR.GetString("CantSwitchVirtualizationModePostMeasure"));
                }
            }
            else
            {
                this.HasMeasured = true;
                this.VirtualizationMode = mode;
            }
        }

        private void SignUpForContentPresenter()
        {
            if (this.IsScrolling && this.scrollData.HasSignedUpForContentPresenterSizeChanged)
            {
                this.scrollData.HasSignedUpForContentPresenterSizeChanged = true;
                DependencyObject firstParent = VisualTreeHelper.GetParent(this);
                ScrollContentPresenter scrollPresenter = firstParent as ScrollContentPresenter;
                if ((scrollPresenter == null) && (firstParent != null))
                {
                    scrollPresenter = VisualTreeHelper.GetParent(firstParent) as ScrollContentPresenter;
                }
                if (scrollPresenter != null)
                {
                    scrollPresenter.SizeChanged += new SizeChangedEventHandler(this.ScrollPresenterSizeChanged);
                }
            }
        }

        internal static double ValidateInputOffset(double offset, string parameterName)
        {
            if (DoubleUtil.IsNaN(offset))
            {
                throw new ArgumentOutOfRangeException(parameterName, Telerik.Windows.Controls.SR.GetString("ScrollViewer_CannotBeNaN", new object[] { parameterName }));
            }
            return Math.Max(0.0, offset);
        }

        internal Size AvailableSize
        {
            get
            {
                return this.availableSize;
            }
        }

        private FrameworkElement BringIntoViewContainer
        {
            get
            {
                return this.bringIntoViewContainer;
            }
            set
            {
                this.bringIntoViewContainer = value;
            }
        }

        private int CacheEnd
        {
            get
            {
                int cacheCount = this.visibleCount;
                if (cacheCount > 0)
                {
                    return ((this.cacheStart + cacheCount) - 1);
                }
                return 0;
            }
        }

        [DefaultValue(false)]
        public bool CanHorizontallyScroll
        {
            get
            {
                if (this.scrollData == null)
                {
                    return false;
                }
                return this.scrollData.AllowHorizontal;
            }
            set
            {
                this.EnsureScrollData();
                if (this.scrollData.AllowHorizontal != value)
                {
                    this.scrollData.AllowHorizontal = value;
                    base.InvalidateMeasure();
                }
            }
        }

        [DefaultValue(false)]
        public bool CanVerticallyScroll
        {
            get
            {
                if (this.scrollData == null)
                {
                    return false;
                }
                return this.scrollData.AllowVertical;
            }
            set
            {
                this.EnsureScrollData();
                if (this.scrollData.AllowVertical != value)
                {
                    this.scrollData.AllowVertical = value;
                    base.InvalidateMeasure();
                }
            }
        }

        public double ChildDefaultLength
        {
            get
            {
                return (double)base.GetValue(ChildDefaultLengthProperty);
            }
            set
            {
                base.SetValue(ChildDefaultLengthProperty, value);
            }
        }

        public double ExtentHeight
        {
            get
            {
                if (this.scrollData == null)
                {
                    return 0.0;
                }
                return this.scrollData.Extent.Height;
            }
        }

        public double ExtentWidth
        {
            get
            {
                if (this.scrollData == null)
                {
                    return 0.0;
                }
                return this.scrollData.Extent.Width;
            }
        }

        internal bool HasMeasured
        {
            get
            {
                return this.hasMeasured;
            }
            set
            {
                this.hasMeasured = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public double HorizontalOffset
        {
            get
            {
                if (this.scrollData == null)
                {
                    return 0.0;
                }
                return this.scrollData.ComputedOffset.X;
            }
        }

        internal Telerik.Windows.Controls.TreeView.IndexTree IndexTree
        {
            get
            {
                return this.indexTree;
            }
        }

        private bool InHierarchicalMode
        {
            get
            {
                return (this.virtualizationMode == Telerik.Windows.Controls.TreeView.VirtualizationMode.Hierarchical);
            }
        }

        private bool InRecyclingMode
        {
            get
            {
                return (this.virtualizationMode == Telerik.Windows.Controls.TreeView.VirtualizationMode.Recycling);
            }
        }

        internal UIElementCollection InternalChildren
        {
            get
            {
                return base.Children;
            }
        }

        private bool IsKeyboardFocusWithin { get; set; }

        internal bool IsScrolling
        {
            get
            {
                return ((this.scrollData != null) && (this.scrollData.ScrollOwner != null));
            }
        }

        private bool IsVirtualizing
        {
            get
            {
                return this.vspIsVirtualizing;
            }
            set
            {
                if (!(value && base.IsItemsHost))
                {
                    this.realizedChildren = null;
                }
                this.vspIsVirtualizing = value;
            }
        }

        public bool IsVisualCacheEnabled
        {
            get
            {
                return ((this.isVisualCacheEnabled && this.IsVirtualizing) && (this.VirtualizationMode == Telerik.Windows.Controls.TreeView.VirtualizationMode.Recycling));
            }
            set
            {
                this.isVisualCacheEnabled = value;
            }
        }

        private int ItemCount
        {
            get
            {
                Telerik.Windows.Controls.ItemsControl itemsControl = System.Windows.Controls.ItemsControl.GetItemsOwner(this) as Telerik.Windows.Controls.ItemsControl;
                if (itemsControl == null)
                {
                    return 0;
                }
                return itemsControl.Items.Count;
            }
        }

        [Obsolete("Orientation is not supported by the TreeViewPanel, replace it with a StackPanel.")]
        public System.Windows.Controls.Orientation Orientation { get; set; }

        private IList RealizedChildren
        {
            get
            {
                if (this.IsVirtualizing && this.InRecyclingMode)
                {
                    this.EnsureRealizedChildren();
                    return this.realizedChildren;
                }
                return this.InternalChildren;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ScrollViewer ScrollOwner
        {
            get
            {
                this.EnsureScrollData();
                return this.scrollData.ScrollOwner;
            }
            set
            {
                this.EnsureScrollData();
                if (value != this.scrollData.ScrollOwner)
                {
                    ResetScrolling(this);
                    this.scrollData.ScrollOwner = value;
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public double VerticalOffset
        {
            get
            {
                if (this.scrollData == null)
                {
                    return 0.0;
                }
                int minOffset = ((this.ExtentHeight > 1.0) && this.IsVirtualizing) ? 1 : 0;
                return Math.Max(this.scrollData.ComputedOffset.Y, (double)minOffset);
            }
        }

        public double ViewportHeight
        {
            get
            {
                if (this.scrollData == null)
                {
                    return 0.0;
                }
                return this.scrollData.Viewport.Height;
            }
        }

        public double ViewportWidth
        {
            get
            {
                if (this.scrollData == null)
                {
                    return 0.0;
                }
                return this.scrollData.Viewport.Width;
            }
        }

        private Telerik.Windows.Controls.TreeView.VirtualizationMode VirtualizationMode
        {
            get
            {
                return this.virtualizationMode;
            }
            set
            {
                this.virtualizationMode = value;
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1064:ExceptionsShouldBePublic", Justification = "The interface is not an exception class.")]
        internal interface ICachable
        {
            bool IsCaching { get; set; }
        }

        [SuppressMessage("Microsoft.Design", "CA1064:ExceptionsShouldBePublic", Justification = "The interface is not an exception classs.")]
        internal interface IProvideStackingSize
        {
            double EstimatedContainerSize();
            double HeaderSize();
        }

        private class ScrollData
        {
            internal bool AllowHorizontal;
            internal bool AllowVertical;
            internal WeakReference BringIntoViewContainerReference;
            internal Vector ComputedOffset = new Vector(0.0, 0.0);
            internal Size Extent;
            internal bool HasSignedUpForContentPresenterSizeChanged;
            internal Vector Offset;
            internal ScrollViewer ScrollOwner;
            internal Size Viewport;

            internal void ClearLayout()
            {
                this.Offset = new Vector();
                Size size = new Size();
                this.Extent = size;
                this.Viewport = this.Extent = size;
            }
        }
    }
}

