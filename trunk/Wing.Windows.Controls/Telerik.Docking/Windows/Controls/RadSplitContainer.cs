namespace Telerik.Windows.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using Telerik.Windows;
    using Telerik.Windows.Controls.Docking;
    using Telerik.Windows.Controls.Primitives;

    [TemplatePart(Name="DockResizer", Type=typeof(RadGridResizer))]
    public class RadSplitContainer : Telerik.Windows.Controls.ItemsControl, ISplitItem, IDocumentHostAware, IToolWindowAware, IThemable
    {
        private RadDocking docking;
        public static readonly DependencyProperty InitialPositionProperty = DependencyProperty.Register("InitialPosition", typeof(DockState), typeof(RadSplitContainer), new Telerik.Windows.PropertyMetadata(DockState.DockedLeft, new PropertyChangedCallback(RadSplitContainer.OnInitialPositionChanged)));
        private bool isInDocumentHost;
        internal static readonly DependencyProperty IsInOpenWindowProperty = DependencyProperty.RegisterAttached("IsInOpenWindow", typeof(bool), typeof(RadSplitContainer), new Telerik.Windows.PropertyMetadata(true));
        private bool isInToolWindow;
        private ProportionalStackPanel itemsPanel;
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.RegisterAttached("Orientation", typeof(System.Windows.Controls.Orientation), typeof(RadPane), new Telerik.Windows.PropertyMetadata(System.Windows.Controls.Orientation.Horizontal, new PropertyChangedCallback(RadSplitContainer.OnOrientationChanged)));
        private bool panelIsOriented;
        private bool recicled;
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId="Resizer")]
        public static readonly DependencyProperty SplitterPositionProperty;
        private static readonly DependencyPropertyKey SplitterPositionPropertyKey = DependencyPropertyExtensions.RegisterReadOnly("SplitterPosition", typeof(Dock?), typeof(RadSplitContainer), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadSplitContainer.OnSplitterPositionChanged)));
        internal static readonly DependencyProperty WindowZIndexProperty = DependencyProperty.RegisterAttached("WindowZIndex", typeof(int), typeof(RadSplitContainer), null);

        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static RadSplitContainer()
        {
            RadDockingCommands.EnsureCommandsClassLoaded();
            SplitterPositionProperty = SplitterPositionPropertyKey.DependencyProperty;
            EventManager.RegisterClassHandler(typeof(RadSplitContainer), RadGridResizer.PreviewResizeStartEvent, new EventHandler<ResizeEventArgs>(RadSplitContainer.OnPreviewResize));
        }

        public RadSplitContainer()
        {
            base.DefaultStyleKey = typeof(RadSplitContainer);
            base.Loaded += new RoutedEventHandler(this.OnLoaded);
        }

        public void AddItem(ISplitItem item, DockPosition dockPosition, ISplitItem relativeTo)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            if (relativeTo == null)
            {
                throw new ArgumentNullException("relativeTo");
            }
            if (dockPosition == DockPosition.Center)
            {
                throw new ArgumentException("The value 'Center' is invalid for this method!", "dockPosition");
            }
            if ((item.ParentContainer != null) && item.ParentContainer.Items.Contains(item))
            {
                throw new ArgumentException("The item already has a parent! Remove it from its parent first!", "item");
            }
            if (!base.Items.Contains(relativeTo))
            {
                throw new ArgumentException("The relativeTo item is not child of the split container!", "relativeTo");
            }
            item.Control.ClearValue(RadDocking.DockStateProperty);
            Size relativeSize = ProportionalStackPanel.GetRelativeSize(relativeTo as DependencyObject);
            this.ChangeOrientationIfNeeded(dockPosition);
            if (this.Orientation == System.Windows.Controls.Orientation.Vertical)
            {
                RadSplitContainer newContainer;
                int index = base.Items.IndexOf(relativeTo);
                switch (dockPosition)
                {
                    case DockPosition.Top:
                        base.Items.Insert(index, item);
                        return;

                    case DockPosition.Bottom:
                        base.Items.Insert(index + 1, item);
                        return;

                    case DockPosition.Center:
                        return;

                    case DockPosition.Left:
                        base.Items.Remove(relativeTo);
                        newContainer = this.GetSplitContainer();
                        newContainer.Orientation = System.Windows.Controls.Orientation.Horizontal;
                        ProportionalStackPanel.SetRelativeSize(newContainer, relativeSize);
                        newContainer.Items.Add(item);
                        newContainer.Items.Add(relativeTo);
                        base.Items.Insert(index, newContainer);
                        return;

                    case DockPosition.Right:
                        base.Items.Remove(relativeTo);
                        newContainer = this.GetSplitContainer();
                        newContainer.Orientation = System.Windows.Controls.Orientation.Horizontal;
                        ProportionalStackPanel.SetRelativeSize(newContainer, relativeSize);
                        newContainer.Items.Add(relativeTo);
                        newContainer.Items.Add(item);
                        base.Items.Insert(index, newContainer);
                        return;
                }
            }
            else if (this.Orientation == System.Windows.Controls.Orientation.Horizontal)
            {
                RadSplitContainer newContainer;
                int index = base.Items.IndexOf(relativeTo);
                switch (dockPosition)
                {
                    case DockPosition.Top:
                        base.Items.Remove(relativeTo);
                        newContainer = this.GetSplitContainer();
                        newContainer.Orientation = System.Windows.Controls.Orientation.Vertical;
                        ProportionalStackPanel.SetRelativeSize(newContainer, relativeSize);
                        newContainer.Items.Add(item);
                        newContainer.Items.Add(relativeTo);
                        base.Items.Insert(index, newContainer);
                        return;

                    case DockPosition.Bottom:
                        base.Items.Remove(relativeTo);
                        newContainer = this.GetSplitContainer();
                        newContainer.Orientation = System.Windows.Controls.Orientation.Vertical;
                        ProportionalStackPanel.SetRelativeSize(newContainer, relativeSize);
                        newContainer.Items.Add(relativeTo);
                        newContainer.Items.Add(item);
                        base.Items.Insert(index, newContainer);
                        return;

                    case DockPosition.Center:
                        return;

                    case DockPosition.Left:
                        base.Items.Insert(index, item);
                        return;

                    case DockPosition.Right:
                        base.Items.Insert(index + 1, item);
                        return;
                }
            }
        }

        private static void AddItemSize(System.Windows.Controls.Control control, bool horizontal, ref RadPaneGroup.RelativeSizes size)
        {
            if (control.Visibility != Visibility.Collapsed)
            {
                Size relativeSize = ProportionalStackPanel.GetRelativeSize(control);
                double splitterChange = ProportionalStackPanel.GetSplitterChange(control);
                double relativeLength = horizontal ? relativeSize.Width : relativeSize.Height;
                size.RelativeLengthSum += relativeLength;
                if (splitterChange == 0.0)
                {
                    size.RelativeLengthWithoutChange += relativeLength;
                }
                else
                {
                    size.RelativeChangesSum += splitterChange;
                    size.RelativeLengthWithChangeSet += relativeLength;
                }
            }
        }

        private void ChangeOrientationIfNeeded(DockPosition dockPosition)
        {
            if (((this.Orientation == System.Windows.Controls.Orientation.Vertical) && ((dockPosition == DockPosition.Left) || (dockPosition == DockPosition.Right))) && (base.Items.Count == 1))
            {
                this.Orientation = System.Windows.Controls.Orientation.Horizontal;
            }
            else if (((this.Orientation == System.Windows.Controls.Orientation.Horizontal) && ((dockPosition == DockPosition.Top) || (dockPosition == DockPosition.Bottom))) && (base.Items.Count == 1))
            {
                this.Orientation = System.Windows.Controls.Orientation.Vertical;
            }
        }

        protected internal override void ChangeVisualState(bool useTransitions)
        {
            base.ChangeVisualState(useTransitions);
            if (this.SplitterPosition.HasValue)
            {
                switch (this.SplitterPosition.Value)
                {
                    case Dock.Left:
                        this.GoToState(useTransitions, new string[] { "ResizerLeft" });
                        return;

                    case Dock.Top:
                        this.GoToState(useTransitions, new string[] { "ResizerTop" });
                        return;

                    case Dock.Right:
                        this.GoToState(useTransitions, new string[] { "ResizerRight" });
                        return;

                    case Dock.Bottom:
                        this.GoToState(useTransitions, new string[] { "ResizerBottom" });
                        return;
                }
            }
            else
            {
                this.GoToState(useTransitions, new string[] { "HideResizer" });
            }
        }

        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            base.ClearContainerForItemOverride(element, item);
            this.RefreshVisibility();
        }

        private void CopyTheOnlyChildToThis()
        {
            if (base.Items.Count != 1)
            {
                throw new InvalidOperationException();
            }
            RadSplitContainer childContainer = base.Items[0] as RadSplitContainer;
            if ((childContainer != null) && RadDocking.GetIsAutogenerated(childContainer))
            {
                childContainer.recicled = true;
                while (childContainer.Items.Count > 0)
                {
                    object item = childContainer.Items[0];
                    childContainer.Items.RemoveAt(0);
                    base.Items.Add(item);
                }
                base.Items.Remove(childContainer);
            }
        }

        private int CountSplitContainers()
        {
            return base.Items.OfType<UIElement>().Count<UIElement>(item => (item.Visibility != Visibility.Collapsed));
        }

        public IEnumerable<RadPane> EnumeratePanes()
        {
            return Items.OfType<ISplitItem>().SelectMany(c => c.EnumeratePanes());
/*
            return (from c in (from c in base.Items.Cast<object>()
                where c is ISplitItem
                select c).Cast<ISplitItem>() select c.EnumeratePanes());
 */
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new RadPaneGroup();
        }

        internal static bool GetIsInOpenWindow(DependencyObject d)
        {
            return (bool) d.GetValue(IsInOpenWindowProperty);
        }

        internal FrameworkElement GetNextVisibleElement(ISplitItem group)
        {
            for (int groupIndex = base.ItemContainerGenerator.IndexFromContainer(group.Control) - 1; groupIndex > -1; groupIndex--)
            {
                UIElement nextElement = base.ItemContainerGenerator.ContainerFromIndex(groupIndex) as UIElement;
                if ((nextElement != null) && (nextElement.Visibility == Visibility.Visible))
                {
                    return (nextElement as FrameworkElement);
                }
            }
            return null;
        }

        private RadSplitContainer GetSplitContainer()
        {
            RadSplitContainer newContainer;
            if (this.docking != null)
            {
                newContainer = this.docking.GeneratedItemsFactory.CreateSplitContainer();
            }
            else
            {
                newContainer = new RadSplitContainer();
            }
            RadDocking.SetIsAutogenerated(newContainer, true);
            return newContainer;
        }

        internal RadPaneGroup.RelativeSizes GetSumOfRelativeSizes()
        {
            RadPaneGroup.RelativeSizes size = new RadPaneGroup.RelativeSizes();
            bool horizontal = this.Orientation == System.Windows.Controls.Orientation.Horizontal;
            foreach (object item in base.Items)
            {
                ISplitItem splitItem = item as ISplitItem;
                if (splitItem != null)
                {
                    AddItemSize(splitItem.Control, horizontal, ref size);
                }
            }
            return size;
        }

        internal static int GetWindowZIndex(DependencyObject d)
        {
            return (int) d.GetValue(WindowZIndexProperty);
        }

        protected void GoToState(bool useTransitions, params string[] stateNames)
        {
            if (stateNames != null)
            {
                foreach (string str in stateNames)
                {
                    if (VisualStateManager.GoToState(this, str, useTransitions))
                    {
                        break;
                    }
                }
            }
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return ((item is ISplitItem) && (item is UIElement));
        }

        private void MoveTheOnlyChildToParent()
        {
            if (base.Items.Count != 1)
            {
                throw new InvalidOperationException();
            }
            RadSplitContainer parentContainer = this.ParentContainer;
            Panel parentPanel = base.Parent as Panel;
            ToolWindow parentWindow = base.Parent as ToolWindow;
            RadPaneGroup childGroup = base.Items[0] as RadPaneGroup;
            RadSplitContainer childContainer = base.Items[0] as RadSplitContainer;
            if (parentContainer != null)
            {
                this.recicled = true;
                int index = parentContainer.Items.IndexOf(this);
                if (childGroup != null)
                {
                    base.Items.Remove(childGroup);
                    parentContainer.Items.Insert(index, childGroup);
                }
                else if (childContainer != null)
                {
                    base.Items.Remove(childContainer);
                    parentContainer.Items.Insert(index, childContainer);
                }
                parentContainer.Items.Remove(this);
            }
            else if (childContainer != null)
            {
                if (parentWindow != null)
                {
                    this.recicled = true;
                    base.Items.Remove(childContainer);
                    parentWindow.Content = childContainer;
                }
                else if (parentPanel != null)
                {
                    int index = parentPanel.Children.IndexOf(this);
                    parentPanel.Children.RemoveAt(index);
                    parentPanel.Children.Insert(index, childContainer);
                }
            }
        }

        private static void OnInitialPositionChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender.ReadLocalValue(RadDocking.DockStateProperty) == DependencyProperty.UnsetValue)
            {
                RadDocking.SetDockState(sender, (DockState) e.NewValue);
            }
        }

        protected virtual void OnIsInDocumentHostChanged(bool oldValue, bool newValue)
        {
            foreach (object item in base.Items)
            {
                IDocumentHostAware container = base.ItemContainerGenerator.ContainerFromItem(item) as IDocumentHostAware;
                if (container != null)
                {
                    container.IsInDocumentHost = this.IsInDocumentHost;
                }
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
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            if (!this.recicled)
            {
                base.Dispatcher.DesignerSafeBeginInvoke(delegate {
                    this.RemoveUnused();
                    this.RefreshVisibility();
                });
            }
        }

        [SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers")]
        protected virtual void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.UpdateChildrenSplitterPositions();
            this.docking = this.ParentOfType<RadDocking>();
        }

        private static void OnOrientationChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            RadSplitContainer splitContainer = sender as RadSplitContainer;
            if (splitContainer != null)
            {
                if (splitContainer.itemsPanel != null)
                {
                    splitContainer.itemsPanel.Orientation = splitContainer.Orientation;
                }
                else
                {
                    splitContainer.panelIsOriented = false;
                }
                splitContainer.UpdateGridSplitters();
            }
        }

        private static void OnPreviewResize(object sender, ResizeEventArgs e)
        {
            RadSplitContainer container = sender as RadSplitContainer;
            e.ResizedElement = container;
            RadSplitContainer parentContainer = container.ParentContainer;
            if (parentContainer != null)
            {
                e.AffectedElement = parentContainer.GetNextVisibleElement(container);
            }
        }

        private static void OnSplitterPositionChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as RadSplitContainer).ChangeVisualState(false);
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            this.itemsPanel = VisualTreeHelper.GetParent(element) as ProportionalStackPanel;
            RadPaneGroup group = element as RadPaneGroup;
            if ((element != item) && (group != null))
            {
                group.Items.Add(item);
            }
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
            if (!this.panelIsOriented)
            {
                this.panelIsOriented = true;
                ProportionalStackPanel panel = VisualTreeHelper.GetParent(element) as ProportionalStackPanel;
                if (panel != null)
                {
                    panel.Orientation = this.Orientation;
                }
                else
                {
                    this.panelIsOriented = false;
                }
            }
        }

        internal void RefreshVisibility()
        {
            if (!this.recicled)
            {
                this.UpdateWindowHeader();
                Visibility visibility = (this.CountSplitContainers() == 0) ? Visibility.Collapsed : Visibility.Visible;
                if (base.Visibility != visibility)
                {
                    base.Visibility = visibility;
                    if (this.ParentContainer != null)
                    {
                        this.ParentContainer.RefreshVisibility();
                    }
                    else
                    {
                        ToolWindow parentWindow = this.ParentToolWindow;
                        if ((parentWindow != null) && (visibility == Visibility.Collapsed))
                        {
                            parentWindow.CloseWindow();
                        }
                    }
                }
                this.UpdateGridSplitters();
            }
        }

        internal void RemoveChild(ISplitItem item)
        {
            base.Items.Remove(item);
        }

        internal void RemoveFromParent()
        {
            base.ClearValue(ProportionalStackPanel.SplitterChangeProperty);
            if (this.ParentContainer != null)
            {
                this.ParentContainer.RemoveChild(this);
            }
            else
            {
                Panel panel = base.Parent as Panel;
                Telerik.Windows.Controls.ItemsControl itensControl = base.Parent as Telerik.Windows.Controls.ItemsControl;
                ContentControl control = base.Parent as ContentControl;
                if (panel != null)
                {
                    panel.Children.Remove(this);
                }
                else if (itensControl != null)
                {
                    itensControl.Items.Remove(this);
                }
                else if (control != null)
                {
                    control.ClearValue(ContentControl.ContentProperty);
                }
            }
        }

        internal void RemoveUnused()
        {
            if (base.Items.Count == 0)
            {
                if (RadDocking.GetIsAutogenerated(this))
                {
                    this.recicled = true;
                    this.RemoveFromParent();
                }
                else
                {
                    base.Visibility = Visibility.Collapsed;
                }
                if (this.ParentContainer != null)
                {
                    this.ParentContainer.RemoveUnused();
                }
            }
            else if (base.Items.Count == 1)
            {
                if (RadDocking.GetIsAutogenerated(this))
                {
                    this.MoveTheOnlyChildToParent();
                }
                else
                {
                    this.CopyTheOnlyChildToThis();
                }
            }
        }

        public void ResetTheme()
        {
            foreach (DependencyObject o in base.Items.OfType<DependencyObject>())
            {
                o.CopyValue(this, StyleManager.ThemeProperty);
            }
        }

        internal static void SetIsInOpenWindow(DependencyObject d, bool value)
        {
            d.SetValue(IsInOpenWindowProperty, value);
        }

        internal static void SetWindowZIndex(DependencyObject d, int value)
        {
            d.SetValue(WindowZIndexProperty, value);
        }

        private void UpdateChildrenSplitterPositions()
        {
            if (this.itemsPanel != null)
            {
                this.itemsPanel.Orientation = this.Orientation;
            }
            else
            {
                this.panelIsOriented = false;
            }
            this.UpdateGridSplitters();
        }

        private void UpdateGridSplitters()
        {
            if (base.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
            {
                if (base.ItemContainerGenerator.Status != GeneratorStatus.Error)
                {
                    EventHandler eh = null;
                    eh = delegate (object s, EventArgs e) {
                        if (this.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
                        {
                            this.ItemContainerGenerator.StatusChanged -= eh;
                            this.UpdateGridSplitters();
                        }
                    };
                    base.ItemContainerGenerator.StatusChanged += eh;
                }
            }
            else
            {
                bool isFirst = true;
                if (this.itemsPanel != null)
                {
                    foreach (UIElement element in this.itemsPanel.Children)
                    {
                        isFirst = this.UpdateSplitterPosition(isFirst, element);
                    }
                }
                else if (RadControl.IsInDesignMode)
                {
                    foreach (UIElement element in base.Items.OfType<UIElement>())
                    {
                        isFirst = this.UpdateSplitterPosition(isFirst, element);
                    }
                }
            }
        }

        private bool UpdateSplitterPosition(bool isFirst, UIElement element)
        {
            if (element.Visibility == Visibility.Visible)
            {
                RadPaneGroup group = element as RadPaneGroup;
                RadSplitContainer container = element as RadSplitContainer;
                if (group != null)
                {
                    if (!isFirst)
                    {
                        group.SplitterPosition = new Dock?((this.Orientation == System.Windows.Controls.Orientation.Horizontal) ? Dock.Left : Dock.Top);
                    }
                    else
                    {
                        isFirst = false;
                        group.SplitterPosition = null;
                    }
                }
                else if (container != null)
                {
                    if (!isFirst)
                    {
                        container.SplitterPosition = new Dock?((this.Orientation == System.Windows.Controls.Orientation.Horizontal) ? Dock.Left : Dock.Top);
                    }
                    else
                    {
                        isFirst = false;
                        container.SplitterPosition = null;
                    }
                }
                this.ChangeVisualState();
            }
            return isFirst;
        }

        private static void UpdateSplitterPosition(ISplitItem container, Dock? dock)
        {
            RadPaneGroup group = container as RadPaneGroup;
            RadSplitContainer splitContainer = container as RadSplitContainer;
            if (group != null)
            {
                group.SplitterPosition = dock;
            }
            else if (splitContainer != null)
            {
                splitContainer.SplitterPosition = dock;
            }
        }

        private void UpdateWindowHeader()
        {
            ToolWindow window = base.Parent as ToolWindow;
            if (window != null)
            {
                window.UpdateToolWindowHeader();
            }
            else if (this.ParentContainer != null)
            {
                this.ParentContainer.UpdateWindowHeader();
            }
        }

        public System.Windows.Controls.Control Control
        {
            get
            {
                return this;
            }
        }

        public DockState InitialPosition
        {
            get
            {
                return (DockState) base.GetValue(InitialPositionProperty);
            }
            set
            {
                base.SetValue(InitialPositionProperty, value);
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

        public RadSplitContainer ParentContainer
        {
            get
            {
                return (base.Parent as RadSplitContainer);
            }
        }

        internal ToolWindow ParentToolWindow
        {
            get
            {
                return this.ParentOfType<ToolWindow>();
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

        internal DockState State
        {
            get
            {
                if (this.ParentContainer != null)
                {
                    return this.ParentContainer.State;
                }
                return RadDocking.GetDockState(this);
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
    }
}

