namespace Telerik.Windows.Controls
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Input;
    using Telerik.Windows;
    using Telerik.Windows.Controls.Docking;

    [TemplateVisualState(Name="PaneHeaderHidden", GroupName="PaneHeaderVisibilityStates"), TemplateVisualState(Name="TwoOrMoreItems", GroupName="AutoCollapseStates"), SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling"), TemplateVisualState(Name="SingleItem", GroupName="AutoCollapseStates"), TemplateVisualState(Name="HideResizer", GroupName="ResizerStates"), TemplateVisualState(Name="ResizerTop", GroupName="ResizerStates"), TemplateVisualState(Name="PaneHeaderVisible", GroupName="PaneHeaderVisibilityStates"), TemplateVisualState(Name="ResizerLeft", GroupName="ResizerStates")]
    public class RadPaneGroup : PaneGroupBase, ISplitItem, IDocumentHostAware, IToolWindowAware
    {
        private RadDocking docking;
        public static readonly DependencyProperty DocumentHostTemplateProperty = DependencyProperty.Register("DocumentHostTemplate", typeof(ControlTemplate), typeof(RadPaneGroup), new Telerik.Windows.PropertyMetadata(null, new PropertyChangedCallback(RadPaneGroup.OnDocumentHostTemplateChanged)));
        private StateFlags flags = new StateFlags();
        private bool isInDocumentHost;
        private bool isInToolWindow;
        public static readonly DependencyProperty IsPaneHeaderVisibleProperty;
        private static readonly DependencyPropertyKey IsPaneHeaderVisiblePropertyKey = DependencyPropertyExtensions.RegisterReadOnly("IsPaneHeaderVisible", typeof(bool), typeof(RadPaneGroup), new Telerik.Windows.PropertyMetadata(true, new PropertyChangedCallback(RadPaneGroup.OnIsPaneHeaderVisibleChanged)));
        public static readonly DependencyProperty IsSingleItemProperty;
        private static readonly DependencyPropertyKey IsSingleItemPropertyKey = DependencyPropertyExtensions.RegisterReadOnly("IsSingleItem", typeof(bool), typeof(RadPaneGroup), null);
        private int previousItemsCount;
        public static readonly DependencyProperty SelectedPaneProperty;
        private static readonly DependencyPropertyKey SelectedPanePropertyKey = DependencyPropertyExtensions.RegisterReadOnly("SelectedPane", typeof(RadPane), typeof(RadPaneGroup), null);
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId="Resizer")]
        public static readonly DependencyProperty SplitterPositionProperty;
        private static readonly DependencyPropertyKey SplitterPositionPropertyKey = DependencyPropertyExtensions.RegisterReadOnly("SplitterPosition", typeof(Dock?), typeof(RadPaneGroup), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadPaneGroup.OnSplitterPositionChanged)));

        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static RadPaneGroup()
        {
            RadDockingCommands.EnsureCommandsClassLoaded();
            SelectedPaneProperty = SelectedPanePropertyKey.DependencyProperty;
            IsSingleItemProperty = IsSingleItemPropertyKey.DependencyProperty;
            IsPaneHeaderVisibleProperty = IsPaneHeaderVisiblePropertyKey.DependencyProperty;
            SplitterPositionProperty = SplitterPositionPropertyKey.DependencyProperty;
            EventManager.RegisterClassHandler(typeof(RadPaneGroup), RadGridResizer.PreviewResizeStartEvent, new EventHandler<ResizeEventArgs>(RadPaneGroup.OnPreviewResize));
        }

        public RadPaneGroup()
        {
            base.DefaultStyleKey = typeof(RadPaneGroup);
            this.Panes = new ObservableCollection<RadPane>();
            this.UnpinnedPanesInternal = new WeakReferenceList<RadPane>();
            base.Loaded += new RoutedEventHandler(this.RadPaneGroup_Loaded);
        }

        public void AddItem(ISplitItem item, DockPosition dockPosition)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            if ((item.ParentContainer != null) && item.ParentContainer.Items.Contains(item))
            {
                throw new ArgumentException("The item already has a parent! Remove it from its parent first!", "item");
            }
            if (dockPosition == DockPosition.Center)
            {
                foreach (RadPane pane in item.EnumeratePanes().ToArray<RadPane>())
                {
                    pane.RemoveFromParent();
                    base.Items.Add(pane);
                }
            }
            else
            {
                this.ParentContainer.AddItem(item, dockPosition, this);
            }
        }

        public void AddItem(RadPane pane, DockPosition dockPosition)
        {
            if (pane == null)
            {
                throw new ArgumentNullException("pane");
            }
            if ((pane.PaneGroup != null) && pane.PaneGroup.Panes.Contains(pane))
            {
                throw new ArgumentException("The pane already has a parent! Remove it from its parent first!", "pane");
            }
            if (dockPosition == DockPosition.Center)
            {
                base.Items.Add(pane);
            }
            else
            {
                RadSplitContainer parentContainer = this.ParentContainer;
                RadPaneGroup group = this.GetPaneGroup();
                group.Items.Add(pane);
                parentContainer.AddItem(group, dockPosition, this);
            }
        }

        private void AddPaneBefore(RadPane other, RadPane pane)
        {
            pane.PaneGroup = this;
            if ((other != null) && this.Panes.Contains(other))
            {
                this.Panes.Insert(this.Panes.IndexOf(other), pane);
            }
            else
            {
                this.Panes.Add(pane);
            }
            base.Dispatcher.DesignerSafeBeginInvoke(delegate {
                bool areSyncronizationsSupressed = this.Flags.SupressSyncronization;
                bool areCloseEventsSupressed = this.Flags.SupressCloseEvents;
                bool arePinEventsSupressed = this.Flags.SupressPinEvents;
                this.Flags.SupressSyncronization = true;
                this.Flags.SupressCloseEvents = true;
                this.Flags.SupressPinEvents = true;
                if (this.Flags.IsLoaded)
                {
                    if (this.IsInDocumentHost)
                    {
                        pane.IsPinned = true;
                    }
                    if ((!pane.IsHidden && !pane.IsPinned) && this.Items.Contains(pane))
                    {
                        pane.AddAsUnpinned();
                    }
                    else if (pane.IsHidden && this.Items.Contains(pane))
                    {
                        this.Items.Remove(pane);
                    }
                }
                this.Flags.SupressSyncronization = areSyncronizationsSupressed;
                this.Flags.SupressCloseEvents = areCloseEventsSupressed;
                this.Flags.SupressPinEvents = arePinEventsSupressed;
            });
        }

        protected internal override void ChangeVisualState(bool useTransitions)
        {
            base.ChangeVisualState(useTransitions);
            if ((base.Items.Count > 1) || !this.Flags.IsContainerInitialized)
            {
                this.Flags.IsInSingleItemState = false;
                base.GoToState(useTransitions, new string[] { "TwoOrMoreItems" });
            }
            else
            {
                this.Flags.IsInSingleItemState = true;
                base.GoToState(useTransitions, new string[] { "SingleItem" });
            }
            if (this.SplitterPosition.HasValue)
            {
                if (((Dock) this.SplitterPosition.Value) == Dock.Left)
                {
                    base.GoToState(useTransitions, new string[] { "ResizerLeft" });
                }
                else
                {
                    base.GoToState(useTransitions, new string[] { "ResizerTop" });
                }
            }
            else
            {
                base.GoToState(useTransitions, new string[] { "HideResizer" });
            }
            if (this.IsPaneHeaderVisible)
            {
                base.GoToState(useTransitions, new string[] { "PaneHeaderVisible" });
            }
            else
            {
                base.GoToState(useTransitions, new string[] { "PaneHeaderHidden" });
            }
        }

        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            base.ClearContainerForItemOverride(element, item);
            IDocumentHostAware hostAwareElement = element as IDocumentHostAware;
            if (hostAwareElement != null)
            {
                hostAwareElement.IsInDocumentHost = false;
            }
            IToolWindowAware windowAwareElement = element as IToolWindowAware;
            if (windowAwareElement != null)
            {
                windowAwareElement.IsInToolWindow = false;
            }
        }

        internal void ClosePane(RadPane pane)
        {
            bool areSyncronizationsSuppressed = this.Flags.SupressSyncronization;
            this.Flags.SupressSyncronization = true;
            this.CloseSinglePane(pane);
            this.Flags.SupressSyncronization = areSyncronizationsSuppressed;
        }

        private void CloseSinglePane(RadPane pane)
        {
            pane.IsHidden = true;
            if (base.Items.Contains(pane))
            {
                base.Items.Remove(pane);
            }
            else if (this.UnpinnedPanesInternal.Contains(pane))
            {
                AutoHideArea parentArea = pane.Parent as AutoHideArea;
                if (parentArea != null)
                {
                    parentArea.Remove(pane);
                }
                this.UnpinnedPanesInternal.Remove(pane);
            }
        }

        public IEnumerable<RadPane> EnumeratePanes()
        {
            return this.Panes;
        }

        private int FindIndexToInsert(int panesCollectionIndex)
        {
            return FindIndexToInsert(panesCollectionIndex, this.Panes, base.Items.Cast<RadPane>().ToList<RadPane>());
        }

        internal static int FindIndexToInsert(int panesCollectionIndex, IList<RadPane> panes, IList<RadPane> items)
        {
            if (panesCollectionIndex == 0)
            {
                return 0;
            }
            if (panesCollectionIndex == (panes.Count - 1))
            {
                return items.Count;
            }
            int indexToInsert = -1;
            while ((panesCollectionIndex >= 0) && (indexToInsert < 0))
            {
                indexToInsert = items.IndexOf(panes[panesCollectionIndex]);
                panesCollectionIndex--;
            }
            return (indexToInsert + 1);
        }

        private RadPane FindSelectedPane()
        {
            RadPane pane = base.SelectedItem as RadPane;
            if (pane == null)
            {
                pane = base.ItemContainerGenerator.ContainerFromItem(base.SelectedItem) as RadPane;
            }
            return pane;
        }

        protected override ControlTemplate FindTemplateFromPosition(Dock position)
        {
            if (this.IsInDocumentHost)
            {
                return this.DocumentHostTemplate;
            }
            return base.FindTemplateFromPosition(position);
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            if (this.IsInDocumentHost)
            {
                return new RadDocumentPane();
            }
            return new RadPane();
        }

        private static double GetLenght(Size size, bool isHorizontal)
        {
            if (!isHorizontal)
            {
                return size.Height;
            }
            return size.Width;
        }

        private RadPaneGroup GetPaneGroup()
        {
            RadPaneGroup group;
            if (this.docking != null)
            {
                group = this.docking.GeneratedItemsFactory.CreatePaneGroup();
            }
            else
            {
                group = new RadPaneGroup();
            }
            RadDocking.SetIsAutogenerated(group, true);
            return group;
        }

        internal Rect GetRectDimenstion(DockPosition dock, RadSplitContainer container)
        {
            Point topLeft = new Point();
            ProportionalStackPanel.GetRelativeSize(this);
            Size elementGroupRelativeSize = ProportionalStackPanel.GetRelativeSize(container);
            Size parentContainerRenderSize = Size.Empty;
            Size size = new Size(0.0, 0.0);
            bool parentContainerHorizontal = false;
            if (this.ParentContainer != null)
            {
                parentContainerRenderSize = this.ParentContainer.RenderSize;
                parentContainerHorizontal = this.ParentContainer.Orientation == Orientation.Horizontal;
                RelativeSizes sumOfRelativeSizes = this.ParentContainer.GetSumOfRelativeSizes();
                if (parentContainerHorizontal)
                {
                    sumOfRelativeSizes.RelativeLengthSum += elementGroupRelativeSize.Width;
                    sumOfRelativeSizes.RelativeLengthWithoutChange += elementGroupRelativeSize.Width;
                    size.Width = sumOfRelativeSizes.RelativeLengthSum;
                }
                else
                {
                    sumOfRelativeSizes.RelativeLengthSum += elementGroupRelativeSize.Height;
                    sumOfRelativeSizes.RelativeLengthWithoutChange += elementGroupRelativeSize.Height;
                    size.Height = sumOfRelativeSizes.RelativeLengthSum;
                }
                size = this.GetSize(dock, sumOfRelativeSizes, parentContainerRenderSize, elementGroupRelativeSize);
                topLeft = this.GetTopLeft(dock, sumOfRelativeSizes, parentContainerRenderSize, size);
            }
            return new Rect(topLeft, size);
        }

        private static RelativeSizes GetRelativeSizesSum(double sum)
        {
            return new RelativeSizes { RelativeChangesSum = 0.0, RelativeLengthSum = sum, RelativeLengthWithChangeSet = 0.0, RelativeLengthWithoutChange = sum };
        }

        private static double GetRenderLenght(double splitterChange, Size relativeSize, Size availableSize, RelativeSizes relativeSizes, bool isHorizontal)
        {
            double relativeLength = GetLenght(relativeSize, isHorizontal);
            double availableLenght = GetLenght(availableSize, isHorizontal);
            if (splitterChange == 0.0)
            {
                return (relativeLength * (availableLenght / (relativeSizes.RelativeLengthWithoutChange + relativeSizes.RelativeLengthWithChangeSet)));
            }
            return (((splitterChange * relativeSizes.RelativeLengthWithChangeSet) / relativeSizes.RelativeChangesSum) * (availableLenght / (relativeSizes.RelativeLengthWithoutChange + relativeSizes.RelativeLengthWithChangeSet)));
        }

        private Size GetSize(DockPosition dock, RelativeSizes relativeSizes, Size parentRenderSize, Size draggedElementRelativeSize)
        {
            Size size = base.RenderSize;
            Size groupRelativeSize = ProportionalStackPanel.GetRelativeSize(this);
            if (dock != DockPosition.Center)
            {
                bool dockOrientationHorizontal = (dock == DockPosition.Left) || (dock == DockPosition.Right);
                bool parentOrientationHorizontal = this.ParentContainer.Orientation == Orientation.Horizontal;
                bool findHorizontal = parentOrientationHorizontal == (dockOrientationHorizontal == parentOrientationHorizontal);
                double length = 0.0;
                if (dockOrientationHorizontal != parentOrientationHorizontal)
                {
                    double relativeSizeSum = GetLenght(groupRelativeSize, dockOrientationHorizontal) + GetLenght(draggedElementRelativeSize, dockOrientationHorizontal);
                    relativeSizes = GetRelativeSizesSum(relativeSizeSum);
                }
                length = GetRenderLenght(0.0, draggedElementRelativeSize, parentRenderSize, relativeSizes, findHorizontal);
                if (findHorizontal)
                {
                    size.Width = length;
                    return size;
                }
                size.Height = length;
            }
            return size;
        }

        private Point GetTopLeft(DockPosition dock, RelativeSizes relativeSizes, Size parentRenderSize, Size draggedElementRenderSize)
        {
            Point topLeft = new Point();
            bool shouldTransform = true;
            if (dock != DockPosition.Center)
            {
                int index = this.ParentContainer.Items.IndexOf(this);
                if ((dock == DockPosition.Right) || (dock == DockPosition.Bottom))
                {
                    index++;
                }
                bool dockOrientationHorizontal = (dock == DockPosition.Left) || (dock == DockPosition.Right);
                bool parentOrientationHorizontal = this.ParentContainer.Orientation == Orientation.Horizontal;
                double length = 0.0;
                if (parentOrientationHorizontal != dockOrientationHorizontal)
                {
                    if ((dock == DockPosition.Right) || (dock == DockPosition.Bottom))
                    {
                        length = GetLenght(parentRenderSize, dockOrientationHorizontal) - GetLenght(draggedElementRenderSize, dockOrientationHorizontal);
                    }
                }
                else
                {
                    for (int i = 0; i < index; i++)
                    {
                        UIElement element = this.ParentContainer.Items[i] as UIElement;
                        if (IsElementVisible(element))
                        {
                            length += GetRenderLenght(ProportionalStackPanel.GetSplitterChange(element), ProportionalStackPanel.GetRelativeSize(element), parentRenderSize, relativeSizes, parentOrientationHorizontal);
                        }
                    }
                    shouldTransform = false;
                }
                if (parentOrientationHorizontal == (dockOrientationHorizontal == parentOrientationHorizontal))
                {
                    topLeft.X = length;
                }
                else
                {
                    topLeft.Y = length;
                }
            }
            if (shouldTransform)
            {
                return base.TransformToVisual(this.ParentContainer).Transform(topLeft);
            }
            UIElement firstChild = this.ParentContainer.Items[0] as UIElement;
            return firstChild.TransformToVisual(this.ParentContainer).Transform(topLeft);
        }

        public void HideAllPanes()
        {
            bool areSyncronizationsSupressed = this.Flags.SupressSyncronization;
            this.Flags.SupressSyncronization = true;
            foreach (object item in this.Panes)
            {
                RadPane p = item as RadPane;
                if ((p != null) && !p.IsHidden)
                {
                    p.IsHidden = true;
                }
            }
            this.Flags.SupressSyncronization = areSyncronizationsSupressed;
        }

        private static bool IsElementVisible(UIElement element)
        {
            return ((element != null) && (element.Visibility != Visibility.Collapsed));
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return (item is RadPane);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.flags.IsLoaded = false;
            if (base.PaneHeader != null)
            {
                base.PaneHeader.PaneHeaderGroup = null;
            }
            base.PaneHeader = base.GetTemplateChild("HeaderElement") as PaneHeader;
            if (base.PaneHeader != null)
            {
                base.PaneHeader.PaneHeaderGroup = this;
            }
            FrameworkElement closeButton = base.GetTemplateChild("CloseButton") as FrameworkElement;
            if (closeButton != null)
            {
                Binding binding = new Binding("SelectedPane.CanUserClose") {
                    RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent),
                    Converter = new BooleanToVisibilityConverter()
                };
                closeButton.SetBinding(UIElement.VisibilityProperty, binding);
            }
        }

        private static void OnDocumentHostTemplateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            RadPaneGroup paneGroup = sender as RadPaneGroup;
            if (paneGroup.IsInDocumentHost)
            {
                paneGroup.SetTemplate(paneGroup.FindTemplateFromPosition(paneGroup.TabStripPlacement));
            }
        }

        protected override void OnIsFocusedChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnIsFocusedChanged(e);
            if (base.PaneHeader != null)
            {
                base.PaneHeader.IsHighlighted = base.IsFocused;
            }
        }

        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId="oldValue")]
        private void OnIsInDocumentHostChanged(bool oldValue, bool newValue)
        {
            if (newValue)
            {
                this.TabStripPlacement = Dock.Top;
            }
            else
            {
                this.TabStripPlacement = Dock.Bottom;
            }
            foreach (object item in base.Items)
            {
                IDocumentHostAware hostAware = item as IDocumentHostAware;
                hostAware.IsInDocumentHost = newValue;
            }
        }

        protected virtual void OnIsInToolWindowChanged(bool oldValue, bool newValue)
        {
            foreach (object item in base.Items)
            {
                IToolWindowAware container = base.ItemContainerGenerator.ContainerFromItem(item) as IToolWindowAware;
                if (container != null)
                {
                    container.IsInToolWindow = this.IsInToolWindow;
                }
            }
            if (!newValue)
            {
                this.IsPaneHeaderVisible = true;
            }
        }

        private static void OnIsPaneHeaderVisibleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as RadPaneGroup).ChangeVisualState(true);
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            if (this.Flags.IsLoaded)
            {
                this.IsSingleItem = base.Items.Count == 1;
            }
            if (!this.flags.SupressSyncronization)
            {
                RadPane beforePane = null;
                if (e.NewStartingIndex >= 0)
                {
                    beforePane = base.Items[e.NewStartingIndex] as RadPane;
                }
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        foreach (RadPane pane in e.NewItems.OfType<RadPane>())
                        {
                            this.AddPaneBefore(beforePane, pane);
                        }
                        break;

                    case NotifyCollectionChangedAction.Remove:
                        foreach (RadPane pane in e.OldItems.OfType<RadPane>())
                        {
                            pane.PaneGroup = null;
                            this.Panes.Remove(pane);
                        }
                        break;

                    case NotifyCollectionChangedAction.Replace:
                        foreach (RadPane pane in e.OldItems.OfType<RadPane>())
                        {
                            this.RemovePane(pane);
                        }
                        foreach (RadPane pane in e.NewItems.OfType<RadPane>())
                        {
                            this.AddPaneBefore(beforePane, pane);
                        }
                        break;

                    case NotifyCollectionChangedAction.Reset:
                        foreach (RadPane pane in this.Panes.OfType<RadPane>().ToList<RadPane>())
                        {
                            this.RemovePane(pane);
                        }
                        foreach (RadPane pane in base.Items.OfType<RadPane>())
                        {
                            this.AddPaneBefore(beforePane, pane);
                        }
                        break;
                }
            }
            bool isRemovingSelected = (e.OldItems != null) && e.OldItems.Contains(base.SelectedItem);
            base.OnItemsChanged(e);
            if (isRemovingSelected && (base.Items.Count > 0))
            {
                base.SelectedIndex = 0;
            }
            if (((this.previousItemsCount < 2) && (base.Items.Count > 1)) || ((this.previousItemsCount > 1) && (base.Items.Count < 2)))
            {
                this.ChangeVisualState();
            }
            this.previousItemsCount = base.Items.Count;
            this.RefreshVisibility();
            if (base.Items.Count == 0)
            {
                this.Flags.IsContainerInitialized = false;
            }
            if ((this.Flags.IsLoaded && (e.NewItems != null)) && (e.NewItems.Count > 0))
            {
                base.SelectedItem = e.NewItems[0];
            }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (!base.IsFocused)
            {
                base.Focus();
            }
        }

        private static void OnPreviewResize(object sender, ResizeEventArgs e)
        {
            RadPaneGroup group = sender as RadPaneGroup;
            e.ResizedElement = group;
            RadSplitContainer container = group.ParentContainer;
            if (container != null)
            {
                e.AffectedElement = container.GetNextVisibleElement(group);
            }
        }

        protected override void OnSelectionChanged(IList removedItems, IList addedItems)
        {
            this.SelectedPane = this.FindSelectedPane();
            CommandManager.InvalidateRequerySuggested();
            base.OnSelectionChanged(removedItems, addedItems);
        }

        private static void OnSplitterPositionChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as RadPaneGroup).ChangeVisualState(false);
        }

        public void PinAllPanes()
        {
            bool areSyncronizationsSupressed = this.Flags.SupressSyncronization;
            this.Flags.SupressSyncronization = true;
            foreach (object item in this.Panes)
            {
                RadPane p = item as RadPane;
                if (((p != null) && !p.IsHidden) && !p.IsPinned)
                {
                    p.IsPinned = true;
                }
            }
            this.Flags.SupressSyncronization = areSyncronizationsSupressed;
        }

        internal void PinPane(RadPane pane)
        {
            bool areSyncronizationsSupressed = this.Flags.SupressSyncronization;
            this.Flags.SupressSyncronization = true;
            pane.IsPinned = true;
            if (!pane.IsHidden && this.UnpinnedPanesInternal.Contains(pane))
            {
                base.Items.Insert(this.FindIndexToInsert(this.Panes.IndexOf(pane)), pane);
                this.UnpinnedPanesInternal.Remove(pane);
            }
            this.Flags.SupressSyncronization = areSyncronizationsSupressed;
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            IDocumentHostAware hostAwareElement = element as IDocumentHostAware;
            if (hostAwareElement != null)
            {
                hostAwareElement.IsInDocumentHost = this.IsInDocumentHost;
            }
            IToolWindowAware windowAwareElement = element as IToolWindowAware;
            if (windowAwareElement != null)
            {
                windowAwareElement.IsInToolWindow = this.IsInToolWindow;
            }
            this.Flags.IsContainerInitialized = true;
            if (base.Items.Count == 1)
            {
                base.Dispatcher.DesignerSafeBeginInvoke(delegate {
                    this.ChangeVisualState();
                });
            }
            this.SelectedPane = this.FindSelectedPane();
        }

        private void RadPaneGroup_Loaded(object sender, RoutedEventArgs e)
        {
            this.Flags.IsLoaded = true;
            this.RefreshPanes();
            this.RefreshVisibility();
            this.ChangeVisualState();
            this.IsSingleItem = base.Items.Count == 1;
            this.docking = this.ParentOfType<RadDocking>();
        }

        private void RefreshPanes()
        {
            bool areSyncronizationsSupressed = this.Flags.SupressSyncronization;
            bool areCloseEventsSupressed = this.Flags.SupressCloseEvents;
            bool arePinEventsSupressed = this.Flags.SupressPinEvents;
            this.Flags.SupressSyncronization = true;
            this.Flags.SupressCloseEvents = true;
            this.Flags.SupressPinEvents = true;
            foreach (RadPane pane in this.Panes)
            {
                if ((!pane.IsHidden && !pane.IsPinned) && base.Items.Contains(pane))
                {
                    pane.AddAsUnpinned();
                }
                else if (pane.IsHidden && base.Items.Contains(pane))
                {
                    base.Items.Remove(pane);
                }
            }
            this.Flags.SupressSyncronization = areSyncronizationsSupressed;
            this.Flags.SupressCloseEvents = areCloseEventsSupressed;
            this.Flags.SupressPinEvents = arePinEventsSupressed;
        }

        private void RefreshVisibility()
        {
            Visibility visibility = (base.Items.Count == 0) ? Visibility.Collapsed : Visibility.Visible;
            if (base.Visibility != visibility)
            {
                base.Visibility = visibility;
                if (this.ParentContainer != null)
                {
                    this.ParentContainer.RefreshVisibility();
                }
            }
            if ((this.Panes.Count == 0) && (this.ParentContainer != null))
            {
                RadSplitContainer parentContainer = this.ParentContainer;
                if (RadDocking.GetIsAutogenerated(this))
                {
                    this.RemoveFromParent();
                }
                parentContainer.RemoveUnused();
            }
        }

        public void RemoveFromParent()
        {
            if ((this.ParentContainer != null) && this.ParentContainer.Items.Contains(this))
            {
                base.ClearValue(ProportionalStackPanel.SplitterChangeProperty);
                this.ParentContainer.RemoveChild(this);
            }
        }

        public void RemovePane(RadPane pane)
        {
            bool areSyncronizationsSupressed = this.Flags.SupressSyncronization;
            this.Flags.SupressSyncronization = true;
            if (this.Panes.Contains(pane))
            {
                this.Panes.Remove(pane);
                pane.ClearValue(Telerik.Windows.RoutedEvent.LogicalParentProperty);
            }
            if (base.Items.Contains(pane))
            {
                base.Items.Remove(pane);
            }
            else if (!pane.IsPinned)
            {
                AutoHideArea parentArea = pane.Parent as AutoHideArea;
                if ((parentArea != null) && parentArea.Items.Contains(pane))
                {
                    parentArea.Remove(pane);
                    this.UnpinnedPanesInternal.Remove(pane);
                }
            }
            this.Flags.SupressSyncronization = areSyncronizationsSupressed;
        }

        public void ShowAllPanes()
        {
            bool areSyncronizationsSupressed = this.Flags.SupressSyncronization;
            this.Flags.SupressSyncronization = true;
            foreach (object item in this.Panes)
            {
                RadPane p = item as RadPane;
                if ((p != null) && p.IsHidden)
                {
                    p.IsHidden = false;
                }
            }
            this.Flags.SupressSyncronization = areSyncronizationsSupressed;
        }

        internal void ShowPane(RadPane pane)
        {
            bool areSyncronizationsSupressed = this.Flags.SupressSyncronization;
            this.Flags.SupressSyncronization = true;
            this.ShowSinglePane(pane);
            this.Flags.SupressSyncronization = areSyncronizationsSupressed;
        }

        private void ShowSinglePane(RadPane pane)
        {
            pane.PaneGroup = this;
            pane.IsHidden = false;
            base.Items.Insert(this.FindIndexToInsert(this.Panes.IndexOf(pane)), pane);
            if (!pane.IsPinned)
            {
                pane.AddAsUnpinned();
            }
        }

        public void UnpinAllPanes()
        {
            bool areSyncronizationsSupressed = this.Flags.SupressSyncronization;
            this.Flags.SupressSyncronization = true;
            foreach (object item in this.Panes)
            {
                RadPane p = item as RadPane;
                if (((p != null) && !p.IsHidden) && p.IsPinned)
                {
                    p.IsPinned = false;
                }
            }
            this.Flags.SupressSyncronization = areSyncronizationsSupressed;
        }

        internal void UnpinPane(RadPane pane)
        {
            bool areSyncronizationsSuppressed = this.Flags.SupressSyncronization;
            this.Flags.SupressSyncronization = true;
            pane.IsPinned = false;
            if (!pane.IsHidden && base.Items.Contains(pane))
            {
                base.Items.Remove(pane);
                this.UnpinnedPanesInternal.Add(pane);
            }
            this.Flags.SupressSyncronization = areSyncronizationsSuppressed;
        }

        public System.Windows.Controls.Control Control
        {
            get
            {
                return this;
            }
        }

        public ControlTemplate DocumentHostTemplate
        {
            get
            {
                return (ControlTemplate) base.GetValue(DocumentHostTemplateProperty);
            }
            set
            {
                base.SetValue(DocumentHostTemplateProperty, value);
            }
        }

        internal StateFlags Flags
        {
            get
            {
                return this.flags;
            }
        }

        public bool IsInDocumentHost
        {
            get
            {
                return ((IDocumentHostAware) this).IsInDocumentHost;
            }
        }

        public bool IsInToolWindow
        {
            get
            {
                return ((IToolWindowAware) this).IsInToolWindow;
            }
        }

        public bool IsPaneHeaderVisible
        {
            get
            {
                return (bool) base.GetValue(IsPaneHeaderVisibleProperty);
            }
            internal set
            {
                this.SetValue(IsPaneHeaderVisiblePropertyKey, value);
            }
        }

        public bool IsSingleItem
        {
            get
            {
                return (bool) base.GetValue(IsSingleItemProperty);
            }
            internal set
            {
                this.SetValue(IsSingleItemPropertyKey, value);
            }
        }

        private IList<RadPane> Panes { get; set; }

        public RadSplitContainer ParentContainer
        {
            get
            {
                return (base.Parent as RadSplitContainer);
            }
        }

        public RadPane SelectedPane
        {
            get
            {
                return (RadPane) base.GetValue(SelectedPaneProperty);
            }
            private set
            {
                this.SetValue(SelectedPanePropertyKey, value);
            }
        }

        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId="Resizer")]
        public Dock? SplitterPosition
        {
            get
            {
                return (Dock?) base.GetValue(SplitterPositionProperty);
            }
            internal set
            {
                this.SetValue(SplitterPositionPropertyKey, value);
            }
        }

        bool IDocumentHostAware.IsInDocumentHost
        {
            get
            {
                return this.isInDocumentHost;
            }
            set
            {
                if (this.isInDocumentHost != value)
                {
                    this.isInDocumentHost = value;
                    this.OnIsInDocumentHostChanged(!value, value);
                }
            }
        }

        bool IToolWindowAware.IsInToolWindow
        {
            get
            {
                return this.isInToolWindow;
            }
            set
            {
                if (this.isInToolWindow != value)
                {
                    this.isInToolWindow = value;
                    this.OnIsInToolWindowChanged(!value, value);
                }
            }
        }

        public IEnumerable<RadPane> UnpinnedPanes
        {
            get
            {
                return this.UnpinnedPanes.ToList<RadPane>();
            }
        }

        internal WeakReferenceList<RadPane> UnpinnedPanesInternal { get; private set; }

        internal class InternalTestHook
        {
            internal InternalTestHook(RadPaneGroup group)
            {
                this.Group = group;
            }

            internal RadPaneGroup Group { get; private set; }

            internal bool IsGroupLoaded
            {
                get
                {
                    return this.Group.flags.IsLoaded;
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct RelativeSizes
        {
            internal double RelativeLengthWithoutChange;
            internal double RelativeLengthWithChangeSet;
            internal double RelativeChangesSum;
            internal double RelativeLengthSum;
        }

        internal class StateFlags
        {
            internal bool IsContainerInitialized { get; set; }

            internal bool IsInSingleItemState { get; set; }

            internal bool IsLoaded { get; set; }

            internal bool SupressCloseEvents { get; set; }

            internal bool SupressPinEvents { get; set; }

            internal bool SupressSyncronization { get; set; }
        }
    }
}

