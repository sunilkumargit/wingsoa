namespace Telerik.Windows.Controls.Docking
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Threading;
    using Telerik.Windows;
    using Telerik.Windows.Controls;
    using Telerik.Windows.Controls.Animation;
    using Telerik.Windows.Controls.Primitives;

    [TemplatePart(Name="PopupChild", Type=typeof(FrameworkElement)), TemplatePart(Name="ContentPopup", Type=typeof(System.Windows.Controls.Primitives.Popup)), TemplatePart(Name="HeaderElement", Type=typeof(PaneHeader))]
    public class AutoHideArea : PaneGroupBase, INotifyLayoutChange
    {
        private InternalState areaState;
        private PaneState currentState;
        private const double FlyoutMinSize = 50.0;
        private Dictionary<RadPaneGroup, List<RadPane>> groupChildrenCache = new Dictionary<RadPaneGroup, List<RadPane>>();
        private System.Windows.Controls.Primitives.Popup popup;
        private FrameworkElement popupChild;
        public static readonly DependencyProperty SelectedPaneProperty = DependencyProperty.Register("SelectedPane", typeof(RadPane), typeof(AutoHideArea), null);
        private DispatcherTimer timer;

        public event EventHandler LayoutChangeEnded;

        public event EventHandler LayoutChangeStarted;

        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static AutoHideArea()
        {
            RadDockingCommands.EnsureCommandsClassLoaded();
            EventManager.RegisterClassHandler(typeof(AutoHideArea), RadMenuItem.SubmenuClosedEvent, new RoutedEventHandler(AutoHideArea.OnMenuClosed));
        }

        public AutoHideArea()
        {
            base.DefaultStyleKey = typeof(AutoHideArea);
            this.Initialize();
        }

        internal void Add(RadPane pane)
        {
            pane.IsLastInGroup = true;
            if (!this.groupChildrenCache.ContainsKey(pane.PaneGroup))
            {
                this.groupChildrenCache.Add(pane.PaneGroup, new List<RadPane> { pane });
                base.Items.Add(pane);
            }
            else
            {
                List<RadPane> panes = this.groupChildrenCache[pane.PaneGroup];
                panes[panes.Count - 1].IsLastInGroup = false;
                base.Items.Insert(base.Items.IndexOf(panes[panes.Count - 1]) + 1, pane);
                panes.Add(pane);
            }
            if (this.areaState.IsLoaded && this.areaState.IsTemplateApplied)
            {
                this.OpenPane(pane);
                this.ChangeState(PaneState.AnimationHiding);
                this.StartHideAnimation();
            }
            else
            {
                base.SelectedIndex = -1;
            }
        }

        private void AddHandlers(System.Windows.Controls.Primitives.Popup element)
        {
            if (element != null)
            {
                element.LostFocus += new RoutedEventHandler(this.OnAutoHideAreaLostFocus);
            }
        }

        private void AddHandlers(FrameworkElement child)
        {
            if (child != null)
            {
                child.MouseEnter += new MouseEventHandler(this.PopupMouseEnter);
                child.MouseLeave += new MouseEventHandler(this.PopupMouseLeave);
                child.SizeChanged += new SizeChangedEventHandler(this.PopupResize);
                child.LostFocus += new RoutedEventHandler(this.OnAutoHideAreaLostFocus);
            }
        }

        private void ChangeState(PaneState newState)
        {
            if (this.IsLayoutChanging)
            {
                this.OnLayoutChangeEnded();
            }
            if ((this.currentState == PaneState.TimerHiding) || (this.currentState == PaneState.TimerShowing))
            {
                this.timer.Stop();
            }
            if (newState == PaneState.Closed)
            {
                base.SelectedIndex = -1;
            }
            switch (newState)
            {
                case PaneState.TimerShowing:
                case PaneState.TimerHiding:
                    this.timer.Start();
                    break;

                case PaneState.AnimationShowing:
                    this.OnLayoutChangeStarted();
                    this.StartShowAnimation();
                    break;

                case PaneState.AnimationHiding:
                    this.OnLayoutChangeStarted();
                    this.StartHideAnimation();
                    break;
            }
            this.currentState = newState;
        }

        private bool CheckIsFocused()
        {
            FrameworkElement element = GetFocusedElement();
            if ((element == null) || !this.IsAncestorOf(element))
            {
                if (!HasFocus(this))
                {
                    return false;
                }
                RadContextMenu cm = GetFocusedElement().ParentOfType<RadContextMenu>();
                if (cm != null)
                {
                    RoutedEventHandler closeHandler = null;
                    closeHandler = delegate {
                        cm.Closed -= closeHandler;
                        this.TakeAction(PaneAction.Deactivate);
                    };
                    cm.Closed += closeHandler;
                }
            }
            return true;
        }

        internal void EnsurePopupCloseCorrectly()
        {
            if (this.currentState == PaneState.StaticVisible)
            {
                this.TakeAction(PaneAction.CloseImmediately);
            }
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new RadPane();
        }

        private double GetCurrentPopupSize()
        {
            if (this.popupChild != null)
            {
                double res = IsVertical(this.TabStripPlacement) ? this.popupChild.Width : this.popupChild.Height;
                if (!double.IsNaN(res))
                {
                    return Math.Max(50.0, res);
                }
            }
            return 50.0;
        }

        private static FrameworkElement GetFocusedElement()
        {
            return (FocusManager.GetFocusedElement() as FrameworkElement);
        }

        private static bool HasFocus(UIElement target)
        {
            FrameworkElement focusedSubtree = GetFocusedElement();
            while (focusedSubtree != null)
            {
                if (object.ReferenceEquals(focusedSubtree, target))
                {
                    return true;
                }
                if (focusedSubtree.ReadLocalValue(Telerik.Windows.RoutedEvent.LogicalParentProperty) != DependencyProperty.UnsetValue)
                {
                    focusedSubtree = Telerik.Windows.RoutedEvent.GetLogicalParent(focusedSubtree) as FrameworkElement;
                }
                else
                {
                    focusedSubtree = (VisualTreeHelper.GetParent(focusedSubtree) ?? focusedSubtree.Parent) as FrameworkElement;
                }
            }
            return false;
        }

        private void HidePopup()
        {
            PopupManager.Close(this.popup, PopupType.DockWindow, false);
        }

        private void Initialize()
        {
            this.areaState = new InternalState { IsLoaded = false, IsTemplateApplied = false };
            this.AnimationsEnabled = true;
            this.timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(300.0) };
            this.timer.Tick += delegate (object s, EventArgs e) {
                this.TakeAction(PaneAction.TimeOut);
            };
            base.LostFocus += new RoutedEventHandler(this.OnAutoHideAreaLostFocus);
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return (item is RadPane);
        }

        private static bool IsVertical(Dock dock)
        {
            if ((dock != Dock.Left) && (dock != Dock.Right))
            {
                return false;
            }
            return true;
        }

        private void LoadOldTemplateParts()
        {
            this.popup = base.GetTemplateChild("LeftContentPopup") as System.Windows.Controls.Primitives.Popup;
            if (this.popup == null)
            {
                this.popup = base.GetTemplateChild("TopContentPopup") as System.Windows.Controls.Primitives.Popup;
            }
            if (this.popup == null)
            {
                this.popup = base.GetTemplateChild("RightContentPopup") as System.Windows.Controls.Primitives.Popup;
            }
            if (this.popup == null)
            {
                this.popup = base.GetTemplateChild("BottomContentPopup") as System.Windows.Controls.Primitives.Popup;
            }
            if (this.popup != null)
            {
                this.popupChild = this.popup.Child as FrameworkElement;
            }
        }

        internal void NotifyMouseEnter(RadPane radPane)
        {
            bool flag = radPane != base.SelectedItem;
            base.SelectedItem = radPane;
            if ((flag && (this.currentState != PaneState.Closed)) && (this.currentState != PaneState.TimerShowing))
            {
                this.TakeAction(PaneAction.ChangeTab);
            }
            else
            {
                this.TakeAction(PaneAction.Activate);
            }
        }

        internal void NotifyMouseLeave()
        {
            if (!this.IsMouseInside)
            {
                this.TakeAction(PaneAction.Deactivate);
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.areaState.IsTemplateApplied = true;
            this.RemoveHandlers(this.popupChild);
            this.RemoveHandlers(this.popup);
            this.popup = base.GetTemplateChild("ContentPopup") as System.Windows.Controls.Primitives.Popup;
            if (this.popup == null)
            {
                this.LoadOldTemplateParts();
            }
            if (this.popup == null)
            {
                Telerik.Windows.Controls.Primitives.Popup radPopup = base.GetTemplateChild("ContentPopup") as Telerik.Windows.Controls.Primitives.Popup;
                if (radPopup != null)
                {
                    this.popup = radPopup.RealPopup;
                }
            }
            this.popupChild = base.GetTemplateChild("PopupChild") as FrameworkElement;
            base.PaneHeader = base.GetTemplateChild("HeaderElement") as PaneHeader;
            this.AddHandlers(this.popupChild);
            this.AddHandlers(this.popup);
        }

        private void OnAutoHideAreaLostFocus(object sender, RoutedEventArgs e)
        {
            if (!this.IsMouseInside)
            {
                this.TakeAction(PaneAction.Deactivate);
            }
        }

        private void OnLayoutChangeEnded()
        {
            if (this.LayoutChangeEnded != null)
            {
                this.LayoutChangeEnded(this, EventArgs.Empty);
            }
            this.IsLayoutChanging = false;
        }

        private void OnLayoutChangeStarted()
        {
            this.IsLayoutChanging = true;
            if (this.LayoutChangeStarted != null)
            {
                this.LayoutChangeStarted(this, EventArgs.Empty);
            }
        }

        protected override void OnLoaded(object sender, RoutedEventArgs e)
        {
            base.OnLoaded(sender, e);
            this.areaState.IsLoaded = true;
            if (base.SelectedIndex != -1)
            {
                base.SelectedIndex = -1;
            }
        }

        private static void OnMenuClosed(object sender, RoutedEventArgs e)
        {
            AutoHideArea autoHide = sender as AutoHideArea;
            if ((autoHide != null) && !autoHide.IsMouseInside)
            {
                autoHide.TakeAction(PaneAction.Deactivate);
            }
        }

        protected override void OnSelectionChanged(IList removedItems, IList addedItems)
        {
            base.OnSelectionChanged(removedItems, addedItems);
            RadPane pane = base.ItemContainerGenerator.ContainerFromItem(base.SelectedItem) as RadPane;
            if ((pane == null) && (this.popup != null))
            {
                this.HidePopup();
                this.ChangeState(PaneState.Closed);
            }
            else if (this.popup != null)
            {
                this.SelectedPane = pane;
                switch (this.TabStripPlacement)
                {
                    case Dock.Left:
                    case Dock.Right:
                        if (double.IsNaN(pane.AutoHideWidth) || (pane.AutoHideWidth < 50.0))
                        {
                            pane.AutoHideWidth = 50.0;
                        }
                        if (this.popupChild != null)
                        {
                            this.popupChild.Height = base.ActualHeight;
                            this.popupChild.Width = pane.AutoHideWidth;
                        }
                        break;

                    case Dock.Top:
                    case Dock.Bottom:
                        if (double.IsNaN(pane.AutoHideHeight) || (pane.AutoHideHeight < 50.0))
                        {
                            pane.AutoHideHeight = 50.0;
                        }
                        if (this.popupChild != null)
                        {
                            this.popupChild.Width = base.ActualWidth;
                            this.popupChild.Height = pane.AutoHideHeight;
                        }
                        break;
                }
                this.UpdatePopup();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private void OpenPane(RadPane pane)
        {
            base.SelectedItem = pane;
            this.ShowPopup();
            this.ChangeState(PaneState.StaticVisible);
        }

        internal void PinGroup(RadPaneGroup group)
        {
            base.SelectedIndex = -1;
            while (group.UnpinnedPanesInternal.Count > 0)
            {
                RadPane pane = group.UnpinnedPanesInternal[0];
                this.Remove(pane);
                group.PinPane(pane);
            }
            base.SelectedIndex = -1;
        }

        internal void PinPane(RadPane pane)
        {
            pane.ClearValue(RadPane.AutoHideWidthProperty);
            pane.ClearValue(RadPane.AutoHideHeightProperty);
            base.SelectedIndex = -1;
            this.Remove(pane);
            pane.PaneGroup.PinPane(pane);
            base.SelectedIndex = -1;
        }

        private void PopupMouseEnter(object sender, MouseEventArgs e)
        {
            this.IsMouseInsidePopup = true;
            this.NotifyMouseEnter(base.SelectedItem as RadPane);
        }

        private void PopupMouseLeave(object sender, MouseEventArgs e)
        {
            this.IsMouseInsidePopup = false;
            this.NotifyMouseLeave();
        }

        private void PopupResize(object sender, SizeChangedEventArgs args)
        {
            if (this.currentState == PaneState.StaticVisible)
            {
                RadPane pane = base.SelectedItem as RadPane;
                if ((pane != null) && (this.popupChild != null))
                {
                    switch (this.TabStripPlacement)
                    {
                        case Dock.Left:
                        case Dock.Right:
                            pane.AutoHideWidth = args.NewSize.Width;
                            break;

                        case Dock.Top:
                        case Dock.Bottom:
                            pane.AutoHideHeight = args.NewSize.Height;
                            break;
                    }
                    this.UpdatePopup();
                }
            }
        }

        internal void Remove(RadPane pane)
        {
            List<RadPane> panes = this.groupChildrenCache[pane.PaneGroup];
            panes.Remove(pane);
            if (panes.Count == 0)
            {
                this.groupChildrenCache.Remove(pane.PaneGroup);
            }
            else if (pane.IsLastInGroup)
            {
                pane.IsLastInGroup = false;
                panes[panes.Count - 1].IsLastInGroup = true;
            }
            base.Items.Remove(pane);
        }

        private void RemoveHandlers(System.Windows.Controls.Primitives.Popup element)
        {
            if (element != null)
            {
                element.LostFocus -= new RoutedEventHandler(this.OnAutoHideAreaLostFocus);
            }
        }

        private void RemoveHandlers(FrameworkElement child)
        {
            if (child != null)
            {
                child.MouseEnter -= new MouseEventHandler(this.PopupMouseEnter);
                child.MouseLeave -= new MouseEventHandler(this.PopupMouseLeave);
                child.SizeChanged -= new SizeChangedEventHandler(this.PopupResize);
                child.LostFocus -= new RoutedEventHandler(this.OnAutoHideAreaLostFocus);
            }
        }

        private void ShowPopup()
        {
            PopupManager.Open(this.popup, PopupType.DockWindow, false);
        }

        private void StartHideAnimation()
        {
            if (this.AnimationsEnabled)
            {
                this.AnimationsEnabled = false;
                Action animationEndedAction = delegate {
                    this.TakeAction(PaneAction.AnimationEnd);
                };
                AnimationManager.Stop(this, this.TabStripPlacement + "In");
                AnimationManager.Stop(this, this.TabStripPlacement + "Out");
                AnimationManager.Play(this, this.TabStripPlacement + "Out", animationEndedAction, new object[0]);
            }
        }

        private void StartShowAnimation()
        {
            if (this.AnimationsEnabled)
            {
                this.AnimationsEnabled = false;
                this.ShowPopup();
                AnimationManager.Stop(this, this.TabStripPlacement + "In");
                AnimationManager.Stop(this, this.TabStripPlacement + "Out");
                AnimationManager.Play(this, this.TabStripPlacement + "In", delegate {
                    this.TakeAction(PaneAction.AnimationEnd);
                }, new object[0]);
            }
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        private void TakeAction(PaneAction action)
        {
            if (action == PaneAction.AnimationEnd)
            {
                this.AnimationsEnabled = true;
            }
            if ((action == PaneAction.CloseImmediately) && (this.currentState != PaneState.Closed))
            {
                this.ChangeState(PaneState.Closed);
            }
            else if (((action == PaneAction.ChangeTab) && (this.currentState != PaneState.Closed)) && (this.currentState != PaneState.TimerShowing))
            {
                this.ChangeState(PaneState.AnimationShowing);
            }
            else
            {
                switch (this.currentState)
                {
                    case PaneState.Closed:
                        if (action != PaneAction.Activate)
                        {
                            break;
                        }
                        this.ChangeState(PaneState.TimerShowing);
                        return;

                    case PaneState.TimerShowing:
                        if ((action != PaneAction.Deactivate) || this.CheckIsFocused())
                        {
                            if (action != PaneAction.TimeOut)
                            {
                                break;
                            }
                            this.ChangeState(PaneState.AnimationShowing);
                            return;
                        }
                        this.ChangeState(PaneState.Closed);
                        return;

                    case PaneState.AnimationShowing:
                        if ((action != PaneAction.Deactivate) || this.CheckIsFocused())
                        {
                            if (action != PaneAction.AnimationEnd)
                            {
                                break;
                            }
                            this.ChangeState(PaneState.StaticVisible);
                            return;
                        }
                        this.ChangeState(PaneState.TimerHiding);
                        return;

                    case PaneState.TimerHiding:
                        if (action != PaneAction.Activate)
                        {
                            if (action != PaneAction.TimeOut)
                            {
                                break;
                            }
                            bool isCloseAllow = false;
                            if (this.SelectedPane != null)
                            {
                                RadContextMenu cm = RadContextMenu.GetContextMenu(this.SelectedPane);
                                if (cm != null)
                                {
                                    isCloseAllow = cm.IsOpen;
                                }
                            }
                            if (isCloseAllow)
                            {
                                break;
                            }
                            this.ChangeState(PaneState.AnimationHiding);
                            return;
                        }
                        this.ChangeState(PaneState.StaticVisible);
                        return;

                    case PaneState.AnimationHiding:
                        if (action != PaneAction.Activate)
                        {
                            if (action != PaneAction.AnimationEnd)
                            {
                                break;
                            }
                            this.ChangeState(PaneState.Closed);
                            return;
                        }
                        return;

                    case PaneState.StaticVisible:
                        if ((action == PaneAction.Deactivate) && !this.CheckIsFocused())
                        {
                            this.ChangeState(PaneState.TimerHiding);
                        }
                        break;

                    default:
                        return;
                }
            }
        }

        internal void UnpinGroup(RadPaneGroup group)
        {
            double actualWidth = group.ActualWidth;
            double actualHeight = group.ActualHeight;
            while (group.Items.Count > 0)
            {
                RadPane pane = group.Items[0] as RadPane;
                if (pane != null)
                {
                    pane.SetValue(RadPane.AutoHideWidthProperty, actualWidth);
                    pane.SetValue(RadPane.AutoHideHeightProperty, actualHeight);
                    group.UnpinPane(pane);
                    this.Add(pane);
                }
            }
            base.SelectedIndex = -1;
        }

        internal void UnpinPane(RadPane pane)
        {
            if (pane.ReadLocalValue(RadPane.AutoHideWidthProperty) == DependencyProperty.UnsetValue)
            {
                pane.SetValue(RadPane.AutoHideWidthProperty, pane.PaneGroup.ActualWidth);
            }
            if (pane.ReadLocalValue(RadPane.AutoHideHeightProperty) == DependencyProperty.UnsetValue)
            {
                pane.SetValue(RadPane.AutoHideHeightProperty, pane.PaneGroup.ActualHeight);
            }
            pane.PaneGroup.UnpinPane(pane);
            this.Add(pane);
        }

        private void UpdatePopup()
        {
            switch (this.TabStripPlacement)
            {
                case Dock.Right:
                    this.popup.RenderTransform = new TranslateTransform { X = -this.popupChild.Width };
                    break;

                case Dock.Bottom:
                    this.popup.RenderTransform = new TranslateTransform { Y = -this.popupChild.Height };
                    break;
            }
            FrameworkElement element = this.popupChild.Parent as FrameworkElement;
            if (element != null)
            {
                element.Clip = new RectangleGeometry { Rect = new Rect(0.0, 0.0, this.popupChild.Width, this.popupChild.Height) };
            }
        }

        private bool AnimationsEnabled { get; set; }

        public bool IsLayoutChanging { get; private set; }

        private bool IsMouseInside
        {
            get
            {
                if (!this.IsMouseInsidePane)
                {
                    return this.IsMouseInsidePopup;
                }
                return true;
            }
        }

        internal bool IsMouseInsidePane { get; set; }

        private bool IsMouseInsidePopup { get; set; }

        public RadPane SelectedPane
        {
            get
            {
                return (RadPane) base.GetValue(SelectedPaneProperty);
            }
            internal set
            {
                base.SetValue(SelectedPaneProperty, value);
            }
        }

        private class InternalState
        {
            public bool IsLoaded { get; set; }

            public bool IsTemplateApplied { get; set; }
        }

        private enum PaneAction
        {
            Activate,
            Deactivate,
            TimeOut,
            AnimationEnd,
            ChangeTab,
            CloseImmediately
        }

        private enum PaneState
        {
            Closed,
            TimerShowing,
            AnimationShowing,
            TimerHiding,
            AnimationHiding,
            StaticVisible
        }
    }
}

