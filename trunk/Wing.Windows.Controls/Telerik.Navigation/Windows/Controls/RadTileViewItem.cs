namespace Telerik.Windows.Controls
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using System.Windows.Media.Animation;
    using Telerik.Windows;
    using Telerik.Windows.Controls.DragDrop;
    using Wing.Windows.Controls.Telerik.Navigation.Windows.Controls;

    [DefaultProperty("Header"), TemplateVisualState(GroupName="CommonStates", Name="MouseOver"), TemplateVisualState(GroupName="CommonStates", Name="Disabled"), StyleTypedProperty(Property="ItemContainerStyle", StyleTargetType=typeof(RadTreeViewItem)), DefaultEvent("TileStateChanged"), TemplateVisualState(GroupName="CommonStates", Name="Normal")]
    public class RadTileViewItem : HeaderedContentControl
    {
        private static int currentZIndex = 1;
        private static TimeSpan doubleClickDelta = TimeSpan.FromMilliseconds(300.0);
        private const string ElementGripBar = "GripBarElement";
        private const string ElementMaximizeToggleButton = "MaximizeToggleButton";
        private UIElement gripBar;
        private ContentPresenter headerElement;
        private bool ignoreCheckedChanged;
        private bool isAnimating;
        private DateTime lastGripBarClickTime;
        private ToggleButton maximizeToggle;
        public static readonly DependencyProperty MinimizedHeightProperty = DependencyProperty.Register("MinimizedHeight", typeof(double), typeof(RadTileViewItem), new System.Windows.PropertyMetadata(new PropertyChangedCallback(RadTileViewItem.OnMinimizedSizePropertyChanged)));
        public static readonly DependencyProperty MinimizedWidthProperty = DependencyProperty.Register("MinimizedWidth", typeof(double), typeof(RadTileViewItem), new System.Windows.PropertyMetadata(new PropertyChangedCallback(RadTileViewItem.OnMinimizedSizePropertyChanged)));
        private WeakReference parentTileViewReference;
        private Storyboard positionAnimation;
        public static readonly Telerik.Windows.RoutedEvent PositionChangedEvent = EventManager.RegisterRoutedEvent("PositionChanged", RoutingStrategy.Bubble, typeof(EventHandler<RadRoutedEventArgs>), typeof(RadTileViewItem));
        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register("Position", typeof(int), typeof(RadTileViewItem), new Telerik.Windows.PropertyMetadata(-1, new PropertyChangedCallback(RadTileViewItem.OnPositionChanged)));
        public static readonly Telerik.Windows.RoutedEvent PreviewTileStateChangedEvent = EventManager.RegisterRoutedEvent("PreviewTileStateChanged", RoutingStrategy.Tunnel, typeof(EventHandler<PreviewTileStateChangedEventArgs>), typeof(RadTileViewItem));
        private Storyboard sizeAnimation;
        public static readonly Telerik.Windows.RoutedEvent TileStateChangedEvent = EventManager.RegisterRoutedEvent("TileStateChanged", RoutingStrategy.Bubble, typeof(EventHandler<RadRoutedEventArgs>), typeof(RadTileViewItem));
        public static readonly DependencyProperty TileStateProperty = DependencyProperty.Register("TileState", typeof(TileViewItemState), typeof(RadTileViewItem), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadTileViewItem.OnTileStatePropertyChanged)));

        public event EventHandler<RadRoutedEventArgs> PositionChanged
        {
            add
            {
                this.AddHandler(PositionChangedEvent, value, false);
            }
            remove
            {
                this.RemoveHandler(PositionChangedEvent, value);
            }
        }

        public event EventHandler<PreviewTileStateChangedEventArgs> PreviewTileStateChanged
        {
            add
            {
                this.AddHandler(PreviewTileStateChangedEvent, value, false);
            }
            remove
            {
                this.RemoveHandler(PreviewTileStateChangedEvent, value);
            }
        }

        public event EventHandler<RadRoutedEventArgs> TileStateChanged
        {
            add
            {
                this.AddHandler(TileStateChangedEvent, value, false);
            }
            remove
            {
                this.RemoveHandler(TileStateChangedEvent, value);
            }
        }

        public RadTileViewItem()
        {
            base.DefaultStyleKey = typeof(RadTileViewItem);
            this.lastGripBarClickTime = new DateTime();
        }

        internal void CleanContentPresenters()
        {
            if (this.headerElement != null)
            {
                this.headerElement.ContentTemplate = null;
                this.headerElement.Content = null;
            }
        }

        private void GripBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.Focus();
            bool isdoubleClick = IsDoubleClick(this.lastGripBarClickTime);
            if ((this.ParentTileView != null) && ((isdoubleClick && (this.ParentTileView.TileStateChangeTrigger == TileStateChangeTrigger.DoubleClick)) || (!isdoubleClick && (this.ParentTileView.TileStateChangeTrigger == TileStateChangeTrigger.SingleClick))))
            {
                this.ToggleTileState();
            }
            this.lastGripBarClickTime = DateTime.Now;
            RadTileView parentTileView = Telerik.Windows.Controls.ItemsControl.ItemsControlFromItemContainer(this) as RadTileView;
            if (parentTileView != null)
            {
                parentTileView.OnItemDragging(this, e);
            }
            e.Handled = true;
        }

        private void HandleItemMaximized()
        {
            if (((this.ParentTileView != null) && (this.maximizeToggle != null)) && (this.ParentTileView.MaximizeMode == TileViewMaximizeMode.One))
            {
                this.maximizeToggle.IsEnabled = false;
            }
            if (this.maximizeToggle != null)
            {
                this.maximizeToggle.IsChecked = true;
                this.ignoreCheckedChanged = false;
            }
        }

        private void HandleItemMinimized()
        {
            if (((this.ParentTileView != null) && (this.maximizeToggle != null)) && ((this.ParentTileView.MaximizeMode != TileViewMaximizeMode.Zero) && !this.maximizeToggle.IsEnabled))
            {
                this.maximizeToggle.IsEnabled = true;
            }
            if (this.maximizeToggle != null)
            {
                this.ignoreCheckedChanged = this.maximizeToggle.IsChecked.Value;
                this.maximizeToggle.IsChecked = false;
            }
        }

        private void HandleItemRestored()
        {
            this.ignoreCheckedChanged = true;
            if (this.maximizeToggle != null)
            {
                this.maximizeToggle.IsChecked = false;
            }
        }

        private void HandleMaximizedReverted()
        {
            if (this.maximizeToggle != null)
            {
                this.ignoreCheckedChanged = this.maximizeToggle.IsChecked.Value;
                this.maximizeToggle.IsChecked = false;
            }
        }

        private static bool IsDoubleClick(DateTime lastClickTime)
        {
            return ((DateTime.Now - lastClickTime) <= doubleClickDelta);
        }

        private void MaximizeToggle_Checked(object sender, RoutedEventArgs e)
        {
            this.TileState = TileViewItemState.Maximized;
            if (this.ParentTileView != null)
            {
                this.ParentTileView.OnTilesStateChangeEnded(new RadRoutedEventArgs(RadTileView.TilesStateChangedEvent));
            }
        }

        private void MaximizeToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!this.ignoreCheckedChanged)
            {
                this.TileState = TileViewItemState.Restored;
                if (this.ParentTileView != null)
                {
                    this.ParentTileView.OnTilesStateChangeEnded(new RadRoutedEventArgs(RadTileView.TilesStateChangedEvent));
                }
            }
            this.ignoreCheckedChanged = false;
        }

        public override void OnApplyTemplate()
        {
            this.CleanContentPresenters();
            base.OnApplyTemplate();
            this.headerElement = base.GetTemplateChild("HeaderElement") as ContentPresenter;
            this.UpdateHeaderPresenterContent();
            this.gripBar = base.GetTemplateChild("GripBarElement") as UIElement;
            if (this.gripBar != null)
            {
                RadDragAndDropManager.SetAllowDrag(this.gripBar, true);
                this.gripBar.MouseLeftButtonDown += new MouseButtonEventHandler(this.GripBar_MouseLeftButtonDown);
            }
            this.maximizeToggle = (base.GetTemplateChild("MaximizeToggleButton") as ToggleButton) ?? new ToggleButton();
            if (this.maximizeToggle != null)
            {
                this.maximizeToggle.Checked -= new RoutedEventHandler(this.MaximizeToggle_Checked);
                this.maximizeToggle.Unchecked -= new RoutedEventHandler(this.MaximizeToggle_Unchecked);
                this.maximizeToggle.Checked += new RoutedEventHandler(this.MaximizeToggle_Checked);
                this.maximizeToggle.Unchecked += new RoutedEventHandler(this.MaximizeToggle_Unchecked);
                if (this.TileState == TileViewItemState.Restored)
                {
                    this.ignoreCheckedChanged = this.maximizeToggle.IsChecked.Value;
                    this.maximizeToggle.IsChecked = false;
                }
                else if (this.TileState == TileViewItemState.Maximized)
                {
                    this.maximizeToggle.IsChecked = true;
                }
            }
            if (((this.ParentTileView != null) && (this.maximizeToggle != null)) && ((this.ParentTileView.MaximizeMode == TileViewMaximizeMode.Zero) || ((this.ParentTileView.MaximizeMode == TileViewMaximizeMode.One) && (this.TileState == TileViewItemState.Maximized))))
            {
                this.maximizeToggle.IsEnabled = false;
            }
        }

        protected override void OnHeaderChanged(object oldHeader, object newHeader)
        {
            base.OnHeaderChanged(oldHeader, newHeader);
            this.UpdateHeaderPresenterContent();
        }

        protected virtual void OnIsAnimatingChanged()
        {
        }

        private static void OnMinimizedSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadTileViewItem source = d as RadTileViewItem;
            if ((source != null) && (source.ParentTileView != null))
            {
                RadTileView parentTileView = source.ParentTileView;
                parentTileView.Dispatcher.BeginInvoke(delegate {
                    if (parentTileView.MaximizedItem != null)
                    {
                        RadTileViewItem maximizedItemContainer = parentTileView.ItemContainerGenerator.ContainerFromItem(parentTileView.MaximizedItem) as RadTileViewItem;
                        parentTileView.DetermineScrollBarVisibility(maximizedItemContainer);
                    }
                });
            }
        }

        protected internal virtual void OnPositionChanged(RadRoutedEventArgs e)
        {
            this.RaiseEvent(e);
        }

        private static void OnPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadTileViewItem item = d as RadTileViewItem;
            if (item != null)
            {
                RadRoutedEventArgs args = new RadRoutedEventArgs(PositionChangedEvent, item);
                item.OnPositionChanged(args);
            }
        }

        protected internal virtual void OnPreviewTileStateChanged(PreviewTileStateChangedEventArgs e)
        {
            this.RaiseEvent(e);
        }

        protected internal virtual void OnTileStateChanged(RadRoutedEventArgs e)
        {
            this.RaiseEvent(e);
        }

        protected virtual void OnTileStateChanged(TileViewItemState oldValue, TileViewItemState newValue)
        {
            RadTileViewItem item = this;
            item.OldState = oldValue;
            if ((item != null) && !item.TileStateRevertedFlag)
            {
                PreviewTileStateChangedEventArgs args = new PreviewTileStateChangedEventArgs {
                    TileState = item.OldState,
                    RoutedEvent = PreviewTileStateChangedEvent,
                    Source = item
                };
                item.OnPreviewTileStateChanged(args);
                if (args.Handled)
                {
                    try
                    {
                        item.TileStateRevertedFlag = true;
                        item.TileState = oldValue;
                        if (newValue == TileViewItemState.Maximized)
                        {
                            item.HandleMaximizedReverted();
                        }
                    }
                    finally
                    {
                        item.TileStateRevertedFlag = false;
                    }
                }
                else
                {
                    switch (newValue)
                    {
                        case TileViewItemState.Restored:
                            item.HandleItemRestored();
                            break;

                        case TileViewItemState.Maximized:
                            item.HandleItemMaximized();
                            break;

                        case TileViewItemState.Minimized:
                            item.HandleItemMinimized();
                            break;
                    }
                    item.OnTileStateChanged(new RadRoutedEventArgs(TileStateChangedEvent, item));
                }
            }
        }

        private static void OnTileStatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadTileViewItem source = d as RadTileViewItem;
            TileViewItemState newValue = (TileViewItemState) e.NewValue;
            TileViewItemState oldValue = (TileViewItemState) e.OldValue;
            if (source != null)
            {
                source.OnTileStateChanged(oldValue, newValue);
            }
        }

        private void ToggleTileState()
        {
            if (this.maximizeToggle != null)
            {
                this.maximizeToggle.IsChecked = this.maximizeToggle.IsChecked.HasValue 
                    ? new bool?(!this.maximizeToggle.IsChecked.GetValueOrDefault())
                    : null;
            }
        }

        internal void UpdateHeaderPresenterContent()
        {
            if (this.headerElement != null)
            {
                this.headerElement.Content = base.Header;
            }
        }

        private void UpdateIsAnimating()
        {
            this.IsAnimating = (this.PositionAnimation != null) || (this.SizeAnimation != null);
        }

        internal static int CurrentZIndex
        {
            get
            {
                return currentZIndex;
            }
            set
            {
                currentZIndex = value;
            }
        }

        protected bool IsAnimating
        {
            get
            {
                return this.isAnimating;
            }
            private set
            {
                if (this.isAnimating != value)
                {
                    this.isAnimating = value;
                    this.OnIsAnimatingChanged();
                }
            }
        }

        public double MinimizedHeight
        {
            get
            {
                return (double) base.GetValue(MinimizedHeightProperty);
            }
            set
            {
                base.SetValue(MinimizedHeightProperty, value);
            }
        }

        public double MinimizedWidth
        {
            get
            {
                return (double) base.GetValue(MinimizedWidthProperty);
            }
            set
            {
                base.SetValue(MinimizedWidthProperty, value);
            }
        }

        internal TileViewItemState OldState { get; set; }

        [ScriptableMember, Browsable(false), DefaultValue((string) null), Description("Gets the parent tileview that the item is assigned to."), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadTileView ParentTileView
        {
            get
            {
                if (this.parentTileViewReference == null)
                {
                    return null;
                }
                return (RadTileView) this.parentTileViewReference.Target;
            }
            internal set
            {
                this.parentTileViewReference = new WeakReference(value);
            }
        }

        [DefaultValue(-1)]
        public int Position
        {
            get
            {
                return (int) base.GetValue(PositionProperty);
            }
            internal set
            {
                base.SetValue(PositionProperty, value);
            }
        }

        internal Storyboard PositionAnimation
        {
            get
            {
                return this.positionAnimation;
            }
            set
            {
                this.positionAnimation = value;
                this.UpdateIsAnimating();
            }
        }

        internal Storyboard SizeAnimation
        {
            get
            {
                return this.sizeAnimation;
            }
            set
            {
                this.sizeAnimation = value;
                this.UpdateIsAnimating();
            }
        }

        public TileViewItemState TileState
        {
            get
            {
                return (TileViewItemState) base.GetValue(TileStateProperty);
            }
            set
            {
                base.SetValue(TileStateProperty, value);
            }
        }

        internal bool TileStateRevertedFlag { get; set; }
    }
}

