namespace Telerik.Windows.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using Telerik.Windows.Controls.Animation;

    public class CoverFlowPanel : VirtualizingPanel
    {
        private List<UIElement> arrangedItems = new List<UIElement>();
        private int firstVisibleChildIndex;
        private bool isFirstArrange = true;
        private bool isLimitExceeded;
        private bool isRecyclingMode = true;
        private bool isVirtualizing;
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Virtualizing", Justification = "The spelling is correct.")]
        public static readonly DependencyProperty IsVirtualizingProperty = DependencyProperty.RegisterAttached("IsVirtualizing", typeof(bool), typeof(CoverFlowPanel), new PropertyMetadata(false));
        private List<UIElement> itemsToArrange = new List<UIElement>();
        internal static readonly DependencyProperty LastPositionProperty = DependencyProperty.RegisterAttached("LastPosition", typeof(Rect), typeof(CoverFlowPanel), new PropertyMetadata(Rect.Empty));
        private RadCoverFlow parentCoverFlow;
        private IList<UIElement> realizedChildren;
        private ReflectionEffect reflectionEffect = new ReflectionEffect();
        private bool selectionChangedHandled;
        private int visibleCount;
        private int visibleStart = -1;

        public CoverFlowPanel()
        {
            this.MaximumMarginalSlots = 10;
        }

        private bool AddContainerFromGenerator(int childIndex, UIElement child, bool newlyRealized)
        {
            bool flag = false;
            if (!newlyRealized)
            {
                if (!this.isRecyclingMode)
                {
                    return flag;
                }
                IList<UIElement> realizedChildrenInstance = this.RealizedChildren;
                if ((childIndex < realizedChildrenInstance.Count) && (realizedChildrenInstance[childIndex] == child))
                {
                    return flag;
                }
                return this.InsertRecycledContainer(childIndex, child);
            }
            this.InsertNewContainer(childIndex, child);
            return flag;
        }

        private void AdjustCacheWindow(int firstViewport, int itemCount)
        {
            int num = firstViewport;
            int num2 = (firstViewport + this.visibleCount) - 1;
            if (num2 >= itemCount)
            {
                num2 = this.isLimitExceeded ? (itemCount - 1) : itemCount;
            }
            int cacheEnd = this.CacheEnd;
            if (num < this.CacheStart)
            {
                this.CacheStart = num;
            }
            else if (num2 > cacheEnd)
            {
                this.CacheStart += num2 - cacheEnd;
            }
        }

        private void AdjustFirstVisibleChildIndex(int startIndex, int count)
        {
            if (startIndex < this.firstVisibleChildIndex)
            {
                int num = (startIndex + count) - 1;
                if (num < this.firstVisibleChildIndex)
                {
                    this.firstVisibleChildIndex -= count;
                }
                else
                {
                    this.firstVisibleChildIndex = startIndex;
                }
            }
        }

        private double ArrangeCenterItem(int index, double centerX, double centerY)
        {
            Point renderLocation;
            Size renderSize;
            UIElement item = this.RealizedChildren[index];
            this.UpdateArrangementIndices(item);
            Rad3D.GetRenderSizeAndLocation(item, 0.0, 1.0, out renderSize, out renderLocation);
            this.ArrangeItem(item, (centerX - renderLocation.X) - this.GetCompensationX(index, renderSize.Width), centerY, 1.0, 0.0, 0, true);
            this.UpdateReflection(item);
            return renderSize.Width;
        }

        private void ArrangeItem(UIElement item, double x, double centerY, double scale, double rotationY, int zIndex, bool animate)
        {
            Storyboard animation;
            double itemHeight = this.IsReflectionEnabled ? (item.DesiredSize.Height * 2.0) : item.DesiredSize.Height;
            Rect itemRect = new Rect(new Point(Math.Round(x), Math.Round((double)((centerY - (itemHeight / 2.0)) - this.GetCameraViewPointCompensationY(item.DesiredSize)))), item.DesiredSize);
            Canvas.SetZIndex(item, zIndex);
            Rect lastPosition = (Rect)item.GetValue(LastPositionProperty);
            if ((this.isFirstArrange || lastPosition.IsEmpty) || !animate)
            {
                animation = this.CreateFadeAnimation(item, scale, rotationY);
            }
            else
            {
                animation = this.CreateMoveAnimation(item, rotationY, scale, itemRect, lastPosition);
            }
            (item as RadCoverFlowItem).Storyboard = animation;
            animation.Begin();
            item.SetValue(LastPositionProperty, itemRect);
            item.Arrange(itemRect);
        }

        private void ArrangeItemAt(int index, double offsetX, double centerY, bool animate)
        {
            Point renderLocation;
            Size renderSize;
            UIElement item = this.RealizedChildren[index];
            bool isLeftItem = index < this.SelectedIndex;
            double rotationY = this.GetRotationY(index);
            Rad3D.GetRenderSizeAndLocation(item, rotationY, this.ParentCoverFlow.ItemScale, out renderSize, out renderLocation);
            int zIndex = isLeftItem ? (index - this.SelectedIndex) : -index;
            double x = (offsetX - renderLocation.X) - this.GetCompensationX(index, renderSize.Width);
            this.ArrangeItem(item, x, centerY, this.ParentCoverFlow.ItemScale, rotationY, zIndex, animate);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (this.RealizedChildren.Count != 0)
            {
                double centerX = finalSize.Width / 2.0;
                double centerY = finalSize.Height / 2.0;
                centerX += this.ParentCoverFlow.OffsetX;
                centerY += this.ParentCoverFlow.OffsetY;
                this.itemsToArrange.AddRange(this.arrangedItems);
                this.arrangedItems.Clear();
                int selectedIndex = this.SelectedIndex;
                double centerItemWidth = this.ArrangeCenterItem(selectedIndex, centerX, centerY);
                double distanceBetweenItems = this.ParentCoverFlow.DistanceBetweenItems;
                double rightX = (centerX + (centerItemWidth / 2.0)) + this.ParentCoverFlow.DistanceFromSelectedItem;
                for (int index = selectedIndex + 1; index < this.RealizedChildren.Count; index++)
                {
                    this.UpdateArrangementIndices(this.RealizedChildren[index]);
                    this.ArrangeItemAt(index, rightX, centerY, true);
                    rightX += distanceBetweenItems;
                    this.UpdateReflection(this.RealizedChildren[index]);
                }
                double leftX = (centerX - (centerItemWidth / 2.0)) - this.ParentCoverFlow.DistanceFromSelectedItem;
                for (int index = selectedIndex - 1; index >= 0; index--)
                {
                    this.UpdateArrangementIndices(this.RealizedChildren[index]);
                    this.ArrangeItemAt(index, leftX, centerY, true);
                    leftX -= distanceBetweenItems;
                    this.UpdateReflection(this.RealizedChildren[index]);
                }
                this.itemsToArrange.Clear();
                base.Clip = new RectangleGeometry { Rect = new Rect(new Point(), finalSize) };
                this.isFirstArrange = false;
            }
            return finalSize;
        }

        private int ChildIndexFromRealizedIndex(int realizedChildIndex)
        {
            if ((this.IsVirtualizing && this.isRecyclingMode) && (realizedChildIndex < this.realizedChildren.Count))
            {
                UIElement element = this.realizedChildren[realizedChildIndex];
                UIElementCollection children = base.Children;
                for (int i = realizedChildIndex; i < children.Count; i++)
                {
                    if (children[i] == element)
                    {
                        return i;
                    }
                }
            }
            return realizedChildIndex;
        }

        private void CleanupContainers(int firstViewport)
        {
            int startIndex = -1;
            int count = 0;
            int itemIndex = -1;
            IList<UIElement> realizedChildrenInstance = this.RealizedChildren;
            bool flag = false;
            if (realizedChildrenInstance.Count != 0)
            {
                this.AdjustCacheWindow(firstViewport, this.ItemCount);
                for (int j = 0; j < realizedChildrenInstance.Count; j++)
                {
                    UIElement local1 = realizedChildrenInstance[j];
                    int num4 = itemIndex;
                    itemIndex = this.GetGeneratedIndex(j);
                    this.GetGeneratedIndex(j);
                    if ((itemIndex - num4) != 1)
                    {
                        flag = true;
                    }
                    if (flag)
                    {
                        if ((startIndex >= 0) && (count > 0))
                        {
                            this.CleanupRange(startIndex, count);
                            j -= count;
                            count = 0;
                            startIndex = -1;
                        }
                        flag = false;
                    }
                    if (this.IsOutsideCacheWindow(itemIndex))
                    {
                        if (startIndex == -1)
                        {
                            startIndex = j;
                        }
                        count++;
                    }
                    else
                    {
                        flag = true;
                    }
                }
                if ((startIndex >= 0) && (count > 0))
                {
                    this.CleanupRange(startIndex, count);
                }
            }
        }

        private void CleanupRange(int startIndex, int count)
        {
            if (this.isRecyclingMode)
            {
                ((IRecyclingItemContainerGenerator)base.ItemContainerGenerator).Recycle(new GeneratorPosition(startIndex, 0), count);
                int num = startIndex + count;
                while (num > startIndex)
                {
                    num--;
                    RadCoverFlowItem element = this.RealizedChildren[num] as RadCoverFlowItem;
                    element.SetValue(LastPositionProperty, Rect.Empty);
                    if (element.Storyboard != null)
                    {
                        element.Storyboard.Stop();
                    }
                    this.RealizedChildren.RemoveAt(num);
                }
            }
            else
            {
                base.ItemContainerGenerator.Remove(new GeneratorPosition(startIndex, 0), count);
                base.RemoveInternalChildRange(startIndex, count);
            }
            this.AdjustFirstVisibleChildIndex(startIndex, count);
        }

        private Storyboard CreateFadeAnimation(UIElement item, double scale, double rotationY)
        {
            AnimationExtensions.AnimationContext context = AnimationExtensions.Create().Animate(new FrameworkElement[] { (FrameworkElement)item });
            double[] opacityData = new double[4];
            opacityData[2] = this.parentCoverFlow.ItemChangeDelay.TotalSeconds;
            opacityData[3] = 1.0;
            double[] yRotation = new double[2];
            yRotation[1] = rotationY;
            double[] scaleData = new double[2];
            scaleData[1] = scale;
            context.EnsureDefaultTransforms().Origin(0.5, 0.5).Opacity(opacityData)
                .SingleProperty(Rad3D.RotationYProperty, yRotation)
                .SingleProperty(Rad3D.ScaleProperty, scaleData)
                .EaseAll(new System.Windows.Media.Animation.CircleEase());
            return context.Instance;
        }

        private Storyboard CreateMoveAnimation(UIElement item, double rotationY, double scale, Rect newPosition, Rect lastPosition)
        {
            Duration animationDuration = new Duration(this.ParentCoverFlow.ItemChangeDelay);
            IEasingFunction easingFunction = this.ParentCoverFlow.EasingFunction;
            AnimationExtensions.AnimationContext context = AnimationExtensions.Create().Animate(new FrameworkElement[] { (FrameworkElement)item });
            TransformGroup group = item.RenderTransform as TransformGroup;
            (group.Children[3] as TranslateTransform).X = lastPosition.X - newPosition.X;
            double[] xCoord = new double[2];
            xCoord[0] = animationDuration.TimeSpan.TotalSeconds;
            context.MoveX(xCoord)
                .EaseAll(easingFunction)
                .SingleProperty(Rad3D.ScaleProperty, new double[] { animationDuration.TimeSpan.TotalSeconds, scale })
                .EaseAll(easingFunction)
                .Origin(0.5, 0.5)
                .SingleProperty(Rad3D.RotationYProperty, new double[] { animationDuration.TimeSpan.TotalSeconds, rotationY })
                .EaseAll(easingFunction);
            return context.Instance;
        }

        private int DetermineLeftPossibleSlots(Size availableSize, out int leftMarginalSlots, out double minimumStartX)
        {
            int value = 0;
            bool isNewlyRealized = true;
            minimumStartX = 0.0;
            if (this.RealizedChildren.Count > 0)
            {
                value = this.GetNumberOfPossibleSlots(this.RealizedChildren[0], availableSize, out minimumStartX);
            }
            else
            {
                int selectedIndex = 0;
                if (this.ParentCoverFlow.SelectedItem != null)
                {
                    selectedIndex = this.ParentCoverFlow.Items.IndexOf(this.ParentCoverFlow.SelectedItem);
                }
                GeneratorPosition position = this.IndexToGeneratorPositionForStart(selectedIndex, out this.firstVisibleChildIndex);
                if (this.ParentCoverFlow.SelectedIndex != 0)
                {
                    position.Index--;
                }
                using (base.ItemContainerGenerator.StartAt(position, GeneratorDirection.Forward, true))
                {
                    UIElement elementForMeasure = base.ItemContainerGenerator.GenerateNext(out isNewlyRealized) as UIElement;
                    if (isNewlyRealized)
                    {
                        this.AddContainerFromGenerator(0, elementForMeasure, isNewlyRealized);
                    }
                    value = this.GetNumberOfPossibleSlots(elementForMeasure, availableSize, out minimumStartX);
                    if (!this.IsVirtualizing)
                    {
                        this.isRecyclingMode = false;
                    }
                    this.CleanupRange(0, 1);
                    this.isRecyclingMode = true;
                }
            }
            int totalNumberOfSlots = this.ExpandNumberLeftMarginalOfSlots(value);
            leftMarginalSlots = totalNumberOfSlots - value;
            if (leftMarginalSlots > 0)
            {
                minimumStartX -= ((double)leftMarginalSlots) * this.ParentCoverFlow.DistanceBetweenItems;
            }
            return totalNumberOfSlots;
        }

        private void DisconnectRecycledContainers()
        {
            int num = 0;
            UIElement element2 = (this.realizedChildren.Count > 0) ? this.realizedChildren[0] : null;
            UIElementCollection children = base.Children;
            for (int i = 0; i < children.Count; i++)
            {
                UIElement element = children[i];
                if (element == element2)
                {
                    num++;
                    if (num < this.realizedChildren.Count)
                    {
                        element2 = this.realizedChildren[num];
                    }
                    else
                    {
                        element2 = null;
                    }
                }
                else
                {
                    children.Remove(element);
                    i--;
                }
            }
        }

        private void EnsureRealizedChildren()
        {
            if (this.realizedChildren == null)
            {
                UIElementCollection children = base.Children;
                this.realizedChildren = new List<UIElement>(children.Count);
                for (int i = 0; i < children.Count; i++)
                {
                    this.realizedChildren.Add(children[i]);
                }
            }
        }

        private void EnsureSelectionChangedHandler()
        {
            if ((this.ParentCoverFlow != null) && !this.selectionChangedHandled)
            {
                this.ParentCoverFlow.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.OnCoverFlowSelectionChanged);
                this.selectionChangedHandled = true;
            }
        }

        private int ExpandNumberLeftMarginalOfSlots(int currentleftSlots)
        {
            return Math.Min((int)(currentleftSlots + this.MaximumMarginalSlots), (int)(currentleftSlots * 2));
        }

        private double GetCameraViewPointCompensationY(Size desiredSize)
        {
            switch (this.ParentCoverFlow.CameraViewpoint)
            {
                case CameraViewpoint.Bottom:
                    return -Math.Round((double)(desiredSize.Height / 2.0), 0);

                case CameraViewpoint.Top:
                    return Math.Round((double)(desiredSize.Height / 2.0), 0);
            }
            return 0.0;
        }

        private double GetCompensationX(int index, double width)
        {
            int selectedIndex = this.SelectedIndex;
            if (index < selectedIndex)
            {
                return width;
            }
            if (index > selectedIndex)
            {
                return 0.0;
            }
            return (width / 2.0);
        }

        private int GetGeneratedIndex(int childIndex)
        {
            return this.ConcreteItemContainerGenerator.IndexFromGeneratorPosition(new GeneratorPosition(childIndex, 0));
        }

        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Virtualizing", Justification = "The spelling is correct.")]
        public static bool GetIsVirtualizing(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsVirtualizingProperty);
        }

        private int GetNumberOfPossibleSlots(UIElement fakeElement, Size availableSize, out double minimumStartX)
        {
            minimumStartX = 0.0;
            Size leftDesiredSize = new Size();
            Size centerDesiredSize = new Size();
            Size childAvailableSize = new Size(double.PositiveInfinity, double.PositiveInfinity);
            fakeElement.Measure(childAvailableSize);
            centerDesiredSize = fakeElement.DesiredSize;
            int value = 0;
            leftDesiredSize = Rad3D.GetRenderSize(fakeElement as FrameworkElement, -this.ParentCoverFlow.RotationY, this.ParentCoverFlow.ItemScale);
            double leftAvailable = availableSize.Width / 2.0;
            leftAvailable -= centerDesiredSize.Width / 2.0;
            bool hasSpace = true;
            if (!double.IsInfinity(availableSize.Width))
            {
                while (hasSpace)
                {
                    double distanceToMove = 0.0;
                    if (value == 0)
                    {
                        distanceToMove = leftDesiredSize.Width + this.ParentCoverFlow.DistanceFromSelectedItem;
                    }
                    else
                    {
                        distanceToMove = this.ParentCoverFlow.DistanceBetweenItems;
                    }
                    leftAvailable -= distanceToMove;
                    if (leftAvailable >= 0.0)
                    {
                        value++;
                    }
                    else
                    {
                        minimumStartX = leftAvailable += distanceToMove;
                        hasSpace = false;
                    }
                }
                return value;
            }
            return this.ItemCount;
        }

        internal double GetRotationY(int itemIndex)
        {
            int selectedIndex = this.SelectedIndex;
            double rotationY = 0.0;
            if (itemIndex < selectedIndex)
            {
                return this.ParentCoverFlow.RotationY;
            }
            if (itemIndex > selectedIndex)
            {
                rotationY = -this.ParentCoverFlow.RotationY;
            }
            return rotationY;
        }

        private GeneratorPosition IndexToGeneratorPositionForStart(int index, out int childIndex)
        {
            GeneratorPosition position;
            if (this.ConcreteItemContainerGenerator != null)
            {
                System.Windows.Controls.ItemContainerGenerator concreteItemContainerGenerator = this.ConcreteItemContainerGenerator;
                position = (concreteItemContainerGenerator != null) ? concreteItemContainerGenerator.GeneratorPositionFromIndex(index) : new GeneratorPosition(-1, index + 1);
            }
            else
            {
                IItemContainerGenerator itemContainerGenerator = base.ItemContainerGenerator;
                position = (itemContainerGenerator != null) ? itemContainerGenerator.GeneratorPositionFromIndex(index) : new GeneratorPosition(-1, index + 1);
            }
            childIndex = (position.Offset == 0) ? position.Index : (position.Index + 1);
            return position;
        }

        private bool InsertContainer(int childIndex, UIElement container, bool isRecycled)
        {
            bool flag = false;
            UIElementCollection children = base.Children;
            int index = 0;
            if (childIndex > 0)
            {
                index = this.ChildIndexFromRealizedIndex(childIndex - 1) + 1;
            }
            if ((!isRecycled || (index >= children.Count)) || (children[index] != container))
            {
                if (index < children.Count)
                {
                    int num2 = index;
                    if (isRecycled && (VisualTreeHelper.GetParent(container) != null))
                    {
                        int num3 = children.IndexOf(container);
                        children.RemoveAt(num3);
                        if (index > num3)
                        {
                            index--;
                        }
                        children.Insert(index, container);
                        flag = true;
                    }
                    else
                    {
                        base.InsertInternalChild(num2, container);
                    }
                }
                else if (isRecycled && (VisualTreeHelper.GetParent(container) != null))
                {
                    children.Remove(container);
                    children.Add(container);
                    flag = true;
                }
                else
                {
                    base.AddInternalChild(container);
                }
            }
            if (this.IsVirtualizing && this.isRecyclingMode)
            {
                this.realizedChildren.Insert(childIndex, container);
            }
            base.ItemContainerGenerator.PrepareItemContainer(container);
            return flag;
        }

        private void InsertNewContainer(int childIndex, UIElement container)
        {
            this.InsertContainer(childIndex, container, false);
        }

        private bool InsertRecycledContainer(int childIndex, UIElement container)
        {
            return this.InsertContainer(childIndex, container, true);
        }

        internal bool IsOutsideCacheWindow(int itemIndex)
        {
            if (itemIndex >= this.CacheStart)
            {
                return (itemIndex > this.CacheEnd);
            }
            return true;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            int leftMarginalSlots;
            double currentX;
            System.Windows.Controls.ItemContainerGenerator concreteItemContainerGenerator = this.ConcreteItemContainerGenerator;
            if (this.ParentCoverFlow.Items.Count == 0)
            {
                return base.MeasureOverride(availableSize);
            }
            this.EnsureSelectionChangedHandler();
            if (this.ParentCoverFlow.Orientation == Orientation.Horizontal)
            {
                this.IsReflectionEnabled = this.ParentCoverFlow.IsReflectionEnabled;
            }
            else
            {
                this.IsReflectionEnabled = false;
            }
            Size panelDesiredSize = new Size();
            Size childAvailableSize = new Size(double.PositiveInfinity, double.PositiveInfinity);
            this.IsVirtualizing = GetIsVirtualizing(this.ParentCoverFlow) && !double.IsInfinity(availableSize.Width);
            int selectedIndex = this.ParentCoverFlow.SelectedIndex;
            if (this.ParentCoverFlow.SelectedIndex == -1)
            {
                selectedIndex = this.ParentCoverFlow.SelectedIndex = 0;
            }
            this.TotalLeftSlots = this.DetermineLeftPossibleSlots(availableSize, out leftMarginalSlots, out currentX);
            double num1 = availableSize.Width / 2.0;
            int firstVisibleIndex = this.IsVirtualizing ? Math.Max(0, selectedIndex - this.TotalLeftSlots) : 0;
            GeneratorPosition position = this.IndexToGeneratorPositionForStart(firstVisibleIndex, out this.firstVisibleChildIndex);
            int currentVisibleRealizedIndex = this.firstVisibleChildIndex;
            this.visibleCount = 0;
            if ((selectedIndex - this.TotalLeftSlots) < firstVisibleIndex)
            {
                currentX += (Math.Abs((int)(selectedIndex - this.TotalLeftSlots)) * this.ParentCoverFlow.DistanceBetweenItems) + this.ParentCoverFlow.DistanceFromSelectedItem;
            }
            this.LeftItems = 0;
            using (base.ItemContainerGenerator.StartAt(position, GeneratorDirection.Forward, true))
            {
                for (int i = firstVisibleIndex; i < this.ItemCount; i++)
                {
                    bool isNewlyRealized;
                    UIElement element = base.ItemContainerGenerator.GenerateNext(out isNewlyRealized) as UIElement;
                    if (element == null)
                    {
                        break;
                    }
                    this.AddContainerFromGenerator(currentVisibleRealizedIndex, element, isNewlyRealized);
                    this.visibleCount++;
                    currentVisibleRealizedIndex++;
                    if (i < selectedIndex)
                    {
                        this.LeftItems++;
                    }
                    element.Measure(childAvailableSize);
                    this.SetCameraOffset(element);
                    Size itemDesiredSize = Rad3D.GetRenderSize(element as FrameworkElement, this.GetRotationY(currentVisibleRealizedIndex), this.ParentCoverFlow.ItemScale);
                    double distanceToMove = 0.0;
                    if (i == selectedIndex)
                    {
                        distanceToMove = this.ParentCoverFlow.DistanceFromSelectedItem + itemDesiredSize.Width;
                    }
                    else if (i == firstVisibleIndex)
                    {
                        distanceToMove = 0.0;
                    }
                    else
                    {
                        distanceToMove = this.ParentCoverFlow.DistanceBetweenItems;
                    }
                    double checkNewPosition = currentX + distanceToMove;
                    if (checkNewPosition <= availableSize.Width)
                    {
                        currentX += distanceToMove;
                        if ((i - firstVisibleIndex) >= leftMarginalSlots)
                        {
                            panelDesiredSize.Width += distanceToMove;
                            panelDesiredSize.Height = Math.Max(panelDesiredSize.Height, itemDesiredSize.Height);
                        }
                    }
                    else
                    {
                        this.isLimitExceeded = true;
                        if (this.IsVirtualizing)
                        {
                            if (leftMarginalSlots <= 1)
                            {
                                break;
                            }
                            leftMarginalSlots--;
                        }
                    }
                }
            }
            double num2 = availableSize.Width;
            this.visibleStart = firstVisibleIndex;
            if (this.IsVirtualizing)
            {
                this.CleanupContainers(this.visibleStart);
                if (this.isRecyclingMode)
                {
                    this.DisconnectRecycledContainers();
                }
            }
            if (this.IsReflectionEnabled)
            {
                panelDesiredSize.Height *= 2.0;
            }
            return new Size(Math.Min(panelDesiredSize.Width, availableSize.Width), double.IsInfinity(availableSize.Height) ? panelDesiredSize.Height : availableSize.Height);
        }

        private void OnCoverFlowSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            base.InvalidateMeasure();
        }

        protected override void OnItemsChanged(object sender, ItemsChangedEventArgs args)
        {
            base.OnItemsChanged(sender, args);
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Replace:
                    {
                        int start = args.Position.Index;
                        int length = start + args.ItemUICount;
                        for (int i = start; i < length; i++)
                        {
                            this.RealizedChildren.RemoveAt(i);
                        }
                        return;
                    }
                case (NotifyCollectionChangedAction.Replace | NotifyCollectionChangedAction.Remove):
                    break;

                case NotifyCollectionChangedAction.Reset:
                    this.RealizedChildren.Clear();
                    break;

                default:
                    return;
            }
        }

        private static void RemoveReflection(UIElement item)
        {
            item.ClearValue(UIElement.EffectProperty);
        }

        private void SetCameraOffset(UIElement item)
        {
            Rad3D.SetCameraOffset(item, new Point3D(0.0, this.GetCameraViewPointCompensationY(item.DesiredSize), 0.0));
        }

        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Virtualizing", Justification = "The spelling is correct.")]
        public static void SetIsVirtualizing(DependencyObject obj, bool value)
        {
            obj.SetValue(IsVirtualizingProperty, value);
        }

        private void UpdateArrangementIndices(UIElement selectedItem)
        {
            if (this.itemsToArrange.Contains(selectedItem))
            {
                this.itemsToArrange.Remove(selectedItem);
            }
            this.arrangedItems.Add(selectedItem);
        }

        private void UpdateReflection(UIElement item)
        {
            if (this.IsReflectionEnabled)
            {
                this.reflectionEffect.ElementHeight = item.DesiredSize.Height + 1.0;
                this.reflectionEffect.ReflectionHeight = this.parentCoverFlow.ReflectionHeight;
                this.reflectionEffect.ReflectionOpacity = this.parentCoverFlow.ReflectionOpacity;
                item.Effect = this.reflectionEffect;
            }
            else
            {
                RemoveReflection(item);
            }
        }

        internal int CacheEnd
        {
            get
            {
                int num = this.visibleCount;
                if (num > 0)
                {
                    return ((this.CacheStart + num) - 1);
                }
                return 0;
            }
        }

        internal int CacheStart { get; set; }

        private System.Windows.Controls.ItemContainerGenerator ConcreteItemContainerGenerator
        {
            get
            {
                return (base.ItemContainerGenerator as System.Windows.Controls.ItemContainerGenerator);
            }
        }

        internal bool IsReflectionEnabled { get; set; }

        internal bool IsVirtualizing
        {
            get
            {
                return this.isVirtualizing;
            }
            set
            {
                if (!base.IsItemsHost || !value)
                {
                    this.realizedChildren = null;
                }
                this.isVirtualizing = value;
            }
        }

        internal int ItemCount
        {
            get
            {
                System.Windows.Controls.ItemsControl itemsOwner = System.Windows.Controls.ItemsControl.GetItemsOwner(this);
                if (itemsOwner == null)
                {
                    return 0;
                }
                if (this.ConcreteItemContainerGenerator == null)
                {
                    return itemsOwner.Items.Count;
                }
                return itemsOwner.Items.Count;
            }
        }

        internal int LeftItems { get; set; }

        internal int MaximumMarginalSlots { get; set; }

        private RadCoverFlow ParentCoverFlow
        {
            get
            {
                if (this.parentCoverFlow == null)
                {
                    this.parentCoverFlow = System.Windows.Controls.ItemsControl.GetItemsOwner(this) as RadCoverFlow;
                }
                return this.parentCoverFlow;
            }
        }

        internal IList<UIElement> RealizedChildren
        {
            get
            {
                if (this.IsVirtualizing && this.isRecyclingMode)
                {
                    this.EnsureRealizedChildren();
                    return this.realizedChildren;
                }
                return base.Children;
            }
        }

        internal int SelectedIndex
        {
            get
            {
                if (!this.IsVirtualizing)
                {
                    return this.ParentCoverFlow.SelectedIndex;
                }
                return this.LeftItems;
            }
        }

        internal int TotalLeftSlots { get; set; }
    }
}

