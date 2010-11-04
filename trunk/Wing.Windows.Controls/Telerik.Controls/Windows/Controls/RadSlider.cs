namespace Telerik.Windows.Controls
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Windows;
    using System.Windows.Automation.Peers;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Threading;
    using Telerik.Windows;
    using Telerik.Windows.Controls.Animation;
    using Telerik.Windows.Input;

    [TemplateVisualState(GroupName="ValidationStates", Name="InvalidFocused"), DefaultProperty("Value"), TemplateVisualState(GroupName="CommonStates", Name="Disabled"), TemplateVisualState(GroupName="FocusStates", Name="Focused"), TemplateVisualState(GroupName="FocusStates", Name="Unfocused"), DefaultEvent("ValueChanged"), TemplateVisualState(GroupName="ValidationStates", Name="InvalidUnfocused"), TemplateVisualState(GroupName="CommonStates", Name="Normal"), TemplatePart(Name="LayoutRoot", Type=typeof(FrameworkElement)), TemplateVisualState(GroupName="ValidationStates", Name="Valid")]
    public class RadSlider : DoubleRangeBase
    {
        private RadTickBar bottomTickBar;
        public static readonly DependencyProperty DelayProperty = DependencyProperty.Register("Delay", typeof(int), typeof(RadSlider), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadSlider.OnDelayPropertyChanged)));
        private double destinationValue;
        private ThumbMoveDirection directionOnKeyDown;
        private ThumbMoveDirection directionOnLargeClick;
        private static Point emptyPoint = new Point();
        public static readonly DependencyProperty EnableSideTicksProperty = DependencyProperty.Register("EnableSideTicks", typeof(bool), typeof(RadSlider), new Telerik.Windows.PropertyMetadata(true, new PropertyChangedCallback(RadSlider.OnEnableSideTicksPropertyChanged)));
        public static readonly DependencyProperty HandlesVisibilityProperty = DependencyProperty.Register("HandlesVisibility", typeof(Visibility), typeof(RadSlider), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadSlider.OnHandlesVisibilityPropertyChanged)));
        private RepeatButton horDecreaseHandle;
        private RepeatButton horIncreaseHandle;
        private Thumb horRangeEndThumb;
        private FrameworkElement horRangeLargeDecrease;
        private FrameworkElement horRangeLargeIncrease;
        private Thumb horRangeMiddleThumb;
        private Thumb horRangeStartThumb;
        private FrameworkElement horRangeTemplate;
        private FrameworkElement horSingleLargeDecrease;
        private FrameworkElement horSingleLargeIncrease;
        private Thumb horSingleThumb;
        private FrameworkElement horSingleThumbTemplate;
        private FrameworkElement horTemplate;
        private double initialRange;
        public static readonly DependencyProperty IsDeferredDraggingEnabledProperty = DependencyProperty.Register("IsDeferredDraggingEnabled", typeof(bool), typeof(RadSlider), null);
        public static readonly DependencyProperty IsDirectionReversedProperty = DependencyProperty.Register("IsDirectionReversed", typeof(bool), typeof(RadSlider), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadSlider.OnIsDirectionReversedPropertyChanged)));
        private static readonly DependencyPropertyKey IsFocusedPropertyKey = DependencyPropertyExtensions.RegisterReadOnly("IsFocused", typeof(bool), typeof(RadSlider), new Telerik.Windows.PropertyMetadata(false, new PropertyChangedCallback(RadSlider.OnIsFocusedChanged)));
        public static readonly DependencyProperty IsFocusedProperty = IsFocusedPropertyKey.DependencyProperty;
        private bool isMouseDown;
        public static readonly DependencyProperty IsMouseWheelEnabledProperty = DependencyProperty.Register("IsMouseWheelEnabled", typeof(bool), typeof(RadSlider), null);
        public static readonly DependencyProperty IsMoveToPointEnabledProperty = DependencyProperty.Register("IsMoveToPointEnabled", typeof(bool), typeof(RadSlider), null);
        public static readonly DependencyProperty IsSnapToTickEnabledProperty = DependencyProperty.Register("IsSnapToTickEnabled", typeof(bool), typeof(RadSlider), null);
        private FrameworkElement largeClickSender;
        private DispatcherTimer largeStepTimer = new DispatcherTimer();
        private RadTickBar leftTickBar;
        private double oldSelectionEndThumbDragValue;
        private double oldSelectionStartThumbDragValue;
        private double oldSingleThumbDragValue;
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(System.Windows.Controls.Orientation), typeof(RadSlider), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadSlider.OnOrientationPropertyChanged)));
        public static readonly DependencyProperty RepeatIntervalProperty = DependencyProperty.Register("RepeatInterval", typeof(int), typeof(RadSlider), null);
        private RadTickBar rightTickBar;
        private double selectionEndThumbDragValue;
        private double selectionStartThumbDragValue;
        private double senderMousePosition;
        private double singleThumbDragValue;
        public static readonly DependencyProperty StepActionProperty = DependencyProperty.Register("StepAction", typeof(Telerik.Windows.Controls.StepAction), typeof(RadSlider), null);
        public static readonly DependencyProperty ThumbVisibilityProperty = DependencyProperty.Register("ThumbVisibility", typeof(Visibility), typeof(RadSlider), null);
        public static readonly DependencyProperty TickFrequencyProperty = DependencyProperty.Register("TickFrequency", typeof(double), typeof(RadSlider), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadSlider.OnTickPropertyChanged)));
        public static readonly DependencyProperty TickPlacementProperty = DependencyProperty.Register("TickPlacement", typeof(Telerik.Windows.Controls.TickPlacement), typeof(RadSlider), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadSlider.OnTickPlacementPropertyChanged)));
        public static readonly DependencyProperty TicksProperty = DependencyProperty.Register("Ticks", typeof(DoubleCollection), typeof(RadSlider), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadSlider.OnTickPropertyChanged)));
        public static readonly DependencyProperty TickTemplateProperty = DependencyProperty.Register("TickTemplate", typeof(DataTemplate), typeof(RadSlider), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadSlider.OnTickPropertyChanged)));
        public static readonly DependencyProperty TickTemplateSelectorProperty = DependencyProperty.Register("TickTemplateSelector", typeof(DataTemplateSelector), typeof(RadSlider), null);
        private RadTickBar topTickBar;
        private RepeatButton verDecreaseHandle;
        private RepeatButton verIncreaseHandle;
        private Thumb verRangeEndThumb;
        private FrameworkElement verRangeLargeDecrease;
        private FrameworkElement verRangeLargeIncrease;
        private Thumb verRangeMiddleThumb;
        private Thumb verRangeStartThumb;
        private FrameworkElement verRangeTemplate;
        private FrameworkElement verSingleLargeDecrease;
        private FrameworkElement verSingleLargeIncrease;
        private Thumb verSingleThumb;
        private FrameworkElement verSingleThumbTemplate;
        private FrameworkElement verTemplate;

        public event EventHandler<RadDragCompletedEventArgs> DragCompleted;

        public event EventHandler<RadDragDeltaEventArgs> DragDelta;

        public event EventHandler<RadDragStartedEventArgs> DragStarted;

        public RadSlider()
        {
            
            base.DefaultStyleKey = typeof(RadSlider);
            base.SizeChanged += new SizeChangedEventHandler(this.RadSlider_SizeChanged);
            this.largeStepTimer.Interval = TimeSpan.FromMilliseconds((double) this.Delay);
            if (!Mouse.IsMouseWheelSupported)
            {
                Mouse.AddMouseWheelHandler(this, new EventHandler<Telerik.Windows.Input.MouseWheelEventArgs>(this.OnMouseWheel));
            }
        }

        private static void AttachButtonEvents(RepeatButton button, RoutedEventHandler clickEventHandler)
        {
            button.Click += clickEventHandler;
        }

        private void AttachEvents()
        {
            if (this.horSingleThumb != null)
            {
                AttachThumbEvents(this.horSingleThumb, new DragStartedEventHandler(this.OnSingleThumbDragStarted), new DragDeltaEventHandler(this.OnSingleThumbDragDelta), new DragCompletedEventHandler(this.OnThumbDragCompleted));
            }
            if (this.horRangeStartThumb != null)
            {
                AttachThumbEvents(this.horRangeStartThumb, new DragStartedEventHandler(this.OnSingleThumbDragStarted), new DragDeltaEventHandler(this.OnSingleThumbDragDelta), new DragCompletedEventHandler(this.OnThumbDragCompleted));
            }
            if (this.horRangeMiddleThumb != null)
            {
                AttachThumbEvents(this.horRangeMiddleThumb, new DragStartedEventHandler(this.OnSelectionMiddleThumbDragStarted), new DragDeltaEventHandler(this.OnSelectionMiddleThumbDragDelta), new DragCompletedEventHandler(this.OnThumbDragCompleted));
            }
            if (this.horRangeEndThumb != null)
            {
                AttachThumbEvents(this.horRangeEndThumb, new DragStartedEventHandler(this.OnSingleThumbDragStarted), new DragDeltaEventHandler(this.OnSingleThumbDragDelta), new DragCompletedEventHandler(this.OnThumbDragCompleted));
            }
            if (this.horRangeLargeDecrease != null)
            {
                this.AttachFrameworElementMouseEvents(this.horRangeLargeDecrease, new MouseButtonEventHandler(this.HorizontalLargeDecrease_MouseLeftButtonDown));
            }
            if (this.horRangeLargeIncrease != null)
            {
                this.AttachFrameworElementMouseEvents(this.horRangeLargeIncrease, new MouseButtonEventHandler(this.HorizontalLargeIncrease_MouseLeftButtonDown));
            }
            if (this.horSingleLargeDecrease != null)
            {
                this.AttachFrameworElementMouseEvents(this.horSingleLargeDecrease, new MouseButtonEventHandler(this.HorizontalLargeDecrease_MouseLeftButtonDown));
            }
            if (this.horSingleLargeIncrease != null)
            {
                this.AttachFrameworElementMouseEvents(this.horSingleLargeIncrease, new MouseButtonEventHandler(this.HorizontalLargeIncrease_MouseLeftButtonDown));
            }
            if (this.horSingleThumbTemplate != null)
            {
                AttachSizeChangedEven(this.horSingleThumbTemplate, new SizeChangedEventHandler(this.RadSlider_SizeChanged));
            }
            if (this.horRangeTemplate != null)
            {
                AttachSizeChangedEven(this.horRangeTemplate, new SizeChangedEventHandler(this.RadSlider_SizeChanged));
            }
            if (this.verSingleThumb != null)
            {
                AttachThumbEvents(this.verSingleThumb, new DragStartedEventHandler(this.OnSingleThumbDragStarted), new DragDeltaEventHandler(this.OnSingleThumbDragDelta), new DragCompletedEventHandler(this.OnThumbDragCompleted));
            }
            if (this.verRangeStartThumb != null)
            {
                AttachThumbEvents(this.verRangeStartThumb, new DragStartedEventHandler(this.OnSingleThumbDragStarted), new DragDeltaEventHandler(this.OnSingleThumbDragDelta), new DragCompletedEventHandler(this.OnThumbDragCompleted));
            }
            if (this.verRangeMiddleThumb != null)
            {
                AttachThumbEvents(this.verRangeMiddleThumb, new DragStartedEventHandler(this.OnSelectionMiddleThumbDragStarted), new DragDeltaEventHandler(this.OnSelectionMiddleThumbDragDelta), new DragCompletedEventHandler(this.OnThumbDragCompleted));
            }
            if (this.verRangeEndThumb != null)
            {
                AttachThumbEvents(this.verRangeEndThumb, new DragStartedEventHandler(this.OnSingleThumbDragStarted), new DragDeltaEventHandler(this.OnSingleThumbDragDelta), new DragCompletedEventHandler(this.OnThumbDragCompleted));
            }
            if (this.verSingleLargeDecrease != null)
            {
                this.AttachFrameworElementMouseEvents(this.verSingleLargeDecrease, new MouseButtonEventHandler(this.VerticalLargeDecrease_MouseLeftButtonDown));
            }
            if (this.verSingleLargeIncrease != null)
            {
                this.AttachFrameworElementMouseEvents(this.verSingleLargeIncrease, new MouseButtonEventHandler(this.VerticalLargeIncrease_MouseLeftButtonDown));
            }
            if (this.verRangeLargeDecrease != null)
            {
                this.AttachFrameworElementMouseEvents(this.verRangeLargeDecrease, new MouseButtonEventHandler(this.VerticalLargeDecrease_MouseLeftButtonDown));
            }
            if (this.verRangeLargeIncrease != null)
            {
                this.AttachFrameworElementMouseEvents(this.verRangeLargeIncrease, new MouseButtonEventHandler(this.VerticalLargeIncrease_MouseLeftButtonDown));
            }
            if (this.verSingleThumbTemplate != null)
            {
                AttachSizeChangedEven(this.verSingleThumbTemplate, new SizeChangedEventHandler(this.RadSlider_SizeChanged));
            }
            if (this.verRangeTemplate != null)
            {
                AttachSizeChangedEven(this.verRangeTemplate, new SizeChangedEventHandler(this.RadSlider_SizeChanged));
            }
            if (this.horIncreaseHandle != null)
            {
                AttachButtonEvents(this.horIncreaseHandle, new RoutedEventHandler(this.IncreaseHandle_Click));
            }
            if (this.horDecreaseHandle != null)
            {
                AttachButtonEvents(this.horDecreaseHandle, new RoutedEventHandler(this.DecreaseHandle_Click));
            }
            if (this.verIncreaseHandle != null)
            {
                AttachButtonEvents(this.verIncreaseHandle, new RoutedEventHandler(this.IncreaseHandle_Click));
            }
            if (this.verDecreaseHandle != null)
            {
                AttachButtonEvents(this.verDecreaseHandle, new RoutedEventHandler(this.DecreaseHandle_Click));
            }
        }

        private void AttachFrameworElementMouseEvents(FrameworkElement element, MouseButtonEventHandler mouseLeftButtonDownEventHandler)
        {
            element.MouseLeftButtonDown += mouseLeftButtonDownEventHandler;
            element.MouseLeftButtonUp += new MouseButtonEventHandler(this.Large_MouseLeftButtonUp);
            element.MouseLeave += new MouseEventHandler(this.Large_MouseLeave);
            element.MouseEnter += new MouseEventHandler(this.Large_MouseEnter);
        }

        private static void AttachSizeChangedEven(FrameworkElement element, SizeChangedEventHandler sizeChangedEventHandler)
        {
            element.SizeChanged += sizeChangedEventHandler;
        }

        private static void AttachThumbEvents(Thumb thumb, DragStartedEventHandler dragStartedEventHandler, DragDeltaEventHandler dragDeltaEventHandler, DragCompletedEventHandler dragCompletedEventHandler)
        {
            thumb.DragStarted += dragStartedEventHandler;
            thumb.DragDelta += dragDeltaEventHandler;
            thumb.DragCompleted += dragCompletedEventHandler;
        }

        protected virtual void ChangeVisualState()
        {
            this.ChangeVisualState(AnimationManager.IsGlobalAnimationEnabled);
        }

        protected virtual void ChangeVisualState(bool useTransitions)
        {
            if (base.IsEnabled)
            {
                VisualStateManager.GoToState(this, "Normal", useTransitions);
            }
            else
            {
                VisualStateManager.GoToState(this, "Disabled", useTransitions);
            }
            if (this.IsFocused)
            {
                VisualStateManager.GoToState(this, "Focused", useTransitions);
            }
            else
            {
                VisualStateManager.GoToState(this, "Unfocused", useTransitions);
            }
            if (Validation.GetHasError(this))
            {
                if (this.IsFocused)
                {
                    VisualStateManager.GoToState(this, "InvalidFocused", useTransitions);
                }
                else
                {
                    VisualStateManager.GoToState(this, "InvalidUnfocused", useTransitions);
                }
            }
            else
            {
                VisualStateManager.GoToState(this, "Valid", useTransitions);
            }
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        private bool CheckSelectionRangeConstraints(double change)
        {
            if (this.Orientation == System.Windows.Controls.Orientation.Horizontal)
            {
                if (((change < 0.0) && !this.IsDirectionReversed) && base.SelectionStart.IsCloseTo(base.Minimum))
                {
                    return false;
                }
                if (((change > 0.0) && !this.IsDirectionReversed) && base.SelectionEnd.IsCloseTo(base.Maximum))
                {
                    return false;
                }
                if (((change < 0.0) && this.IsDirectionReversed) && base.SelectionEnd.IsCloseTo(base.Maximum))
                {
                    return false;
                }
                if (((change > 0.0) && this.IsDirectionReversed) && base.SelectionStart.IsCloseTo(base.Minimum))
                {
                    return false;
                }
            }
            else
            {
                if (((change < 0.0) && !this.IsDirectionReversed) && base.SelectionEnd.IsCloseTo(base.Maximum))
                {
                    return false;
                }
                if (((change > 0.0) && !this.IsDirectionReversed) && base.SelectionStart.IsCloseTo(base.Minimum))
                {
                    return false;
                }
                if (((change < 0.0) && this.IsDirectionReversed) && base.SelectionStart.IsCloseTo(base.Minimum))
                {
                    return false;
                }
                if (((change > 0.0) && this.IsDirectionReversed) && base.SelectionEnd.IsCloseTo(base.Maximum))
                {
                    return false;
                }
            }
            return true;
        }

        private void DecreaseHandle_Click(object sender, RoutedEventArgs e)
        {
            base.Focus();
            this.OnDecreaseHandleClick();
        }

        private static void DetachButtonEvents(RepeatButton button, RoutedEventHandler clickEventHandler)
        {
            button.Click -= clickEventHandler;
        }

        private void DetachEvents()
        {
            if (this.horSingleThumb != null)
            {
                DetachThumbEvents(this.horSingleThumb, new DragStartedEventHandler(this.OnSingleThumbDragStarted), new DragDeltaEventHandler(this.OnSingleThumbDragDelta), new DragCompletedEventHandler(this.OnThumbDragCompleted));
            }
            if (this.horRangeStartThumb != null)
            {
                DetachThumbEvents(this.horRangeStartThumb, new DragStartedEventHandler(this.OnSingleThumbDragStarted), new DragDeltaEventHandler(this.OnSingleThumbDragDelta), new DragCompletedEventHandler(this.OnThumbDragCompleted));
            }
            if (this.horRangeMiddleThumb != null)
            {
                DetachThumbEvents(this.horRangeMiddleThumb, new DragStartedEventHandler(this.OnSelectionMiddleThumbDragStarted), new DragDeltaEventHandler(this.OnSelectionMiddleThumbDragDelta), new DragCompletedEventHandler(this.OnThumbDragCompleted));
            }
            if (this.horRangeEndThumb != null)
            {
                DetachThumbEvents(this.horRangeEndThumb, new DragStartedEventHandler(this.OnSingleThumbDragStarted), new DragDeltaEventHandler(this.OnSingleThumbDragDelta), new DragCompletedEventHandler(this.OnThumbDragCompleted));
            }
            if (this.horRangeLargeDecrease != null)
            {
                this.DetachFrameworElementkMouseEvents(this.horRangeLargeDecrease, new MouseButtonEventHandler(this.HorizontalLargeDecrease_MouseLeftButtonDown));
            }
            if (this.horRangeLargeIncrease != null)
            {
                this.DetachFrameworElementkMouseEvents(this.horRangeLargeIncrease, new MouseButtonEventHandler(this.HorizontalLargeIncrease_MouseLeftButtonDown));
            }
            if (this.horSingleLargeDecrease != null)
            {
                this.DetachFrameworElementkMouseEvents(this.horSingleLargeDecrease, new MouseButtonEventHandler(this.HorizontalLargeDecrease_MouseLeftButtonDown));
            }
            if (this.horSingleLargeIncrease != null)
            {
                this.DetachFrameworElementkMouseEvents(this.horSingleLargeIncrease, new MouseButtonEventHandler(this.HorizontalLargeIncrease_MouseLeftButtonDown));
            }
            if (this.verSingleThumb != null)
            {
                DetachThumbEvents(this.verSingleThumb, new DragStartedEventHandler(this.OnSingleThumbDragStarted), new DragDeltaEventHandler(this.OnSingleThumbDragDelta), new DragCompletedEventHandler(this.OnThumbDragCompleted));
            }
            if (this.verRangeStartThumb != null)
            {
                DetachThumbEvents(this.verRangeStartThumb, new DragStartedEventHandler(this.OnSingleThumbDragStarted), new DragDeltaEventHandler(this.OnSingleThumbDragDelta), new DragCompletedEventHandler(this.OnThumbDragCompleted));
            }
            if (this.verRangeMiddleThumb != null)
            {
                DetachThumbEvents(this.verRangeMiddleThumb, new DragStartedEventHandler(this.OnSelectionMiddleThumbDragStarted), new DragDeltaEventHandler(this.OnSelectionMiddleThumbDragDelta), new DragCompletedEventHandler(this.OnThumbDragCompleted));
            }
            if (this.verRangeEndThumb != null)
            {
                DetachThumbEvents(this.verRangeEndThumb, new DragStartedEventHandler(this.OnSingleThumbDragStarted), new DragDeltaEventHandler(this.OnSingleThumbDragDelta), new DragCompletedEventHandler(this.OnThumbDragCompleted));
            }
            if (this.verSingleLargeDecrease != null)
            {
                this.DetachFrameworElementkMouseEvents(this.verSingleLargeDecrease, new MouseButtonEventHandler(this.VerticalLargeDecrease_MouseLeftButtonDown));
            }
            if (this.verSingleLargeIncrease != null)
            {
                this.DetachFrameworElementkMouseEvents(this.verSingleLargeIncrease, new MouseButtonEventHandler(this.VerticalLargeIncrease_MouseLeftButtonDown));
            }
            if (this.verRangeLargeDecrease != null)
            {
                this.DetachFrameworElementkMouseEvents(this.verRangeLargeDecrease, new MouseButtonEventHandler(this.VerticalLargeDecrease_MouseLeftButtonDown));
            }
            if (this.verRangeLargeIncrease != null)
            {
                this.DetachFrameworElementkMouseEvents(this.verRangeLargeIncrease, new MouseButtonEventHandler(this.VerticalLargeIncrease_MouseLeftButtonDown));
            }
            if (this.horIncreaseHandle != null)
            {
                DetachButtonEvents(this.horIncreaseHandle, new RoutedEventHandler(this.IncreaseHandle_Click));
            }
            if (this.horDecreaseHandle != null)
            {
                DetachButtonEvents(this.horDecreaseHandle, new RoutedEventHandler(this.IncreaseHandle_Click));
            }
            if (this.verIncreaseHandle != null)
            {
                DetachButtonEvents(this.verIncreaseHandle, new RoutedEventHandler(this.IncreaseHandle_Click));
            }
            if (this.verDecreaseHandle != null)
            {
                DetachButtonEvents(this.verDecreaseHandle, new RoutedEventHandler(this.IncreaseHandle_Click));
            }
        }

        private void DetachFrameworElementkMouseEvents(FrameworkElement element, MouseButtonEventHandler mouseLeftButtonDownEventHandler)
        {
            element.MouseLeftButtonDown -= mouseLeftButtonDownEventHandler;
            element.MouseLeftButtonUp -= new MouseButtonEventHandler(this.Large_MouseLeftButtonUp);
            element.MouseLeave -= new MouseEventHandler(this.Large_MouseLeave);
            element.MouseEnter -= new MouseEventHandler(this.Large_MouseEnter);
        }

        private static void DetachSizeChangedEven(FrameworkElement element, SizeChangedEventHandler sizeChangedEventHandler)
        {
            element.SizeChanged -= sizeChangedEventHandler;
        }

        private static void DetachThumbEvents(Thumb thumb, DragStartedEventHandler dragStartedEventHandler, DragDeltaEventHandler dragDeltaEventHandler, DragCompletedEventHandler dragCompletedEventHandler)
        {
            thumb.DragStarted -= dragStartedEventHandler;
            thumb.DragDelta -= dragDeltaEventHandler;
            thumb.DragCompleted -= dragCompletedEventHandler;
        }

        private double GetMouseOffsetOnTrackClick()
        {
            if (this.Orientation == System.Windows.Controls.Orientation.Horizontal)
            {
                if (this.directionOnLargeClick == ThumbMoveDirection.Increase)
                {
                    return (this.IsDirectionReversed ? (this.largeClickSender.ActualWidth - this.senderMousePosition) : this.senderMousePosition);
                }
                return (this.IsDirectionReversed ? this.senderMousePosition : (this.largeClickSender.ActualWidth - this.senderMousePosition));
            }
            if (this.directionOnLargeClick == ThumbMoveDirection.Increase)
            {
                return (this.IsDirectionReversed ? this.senderMousePosition : (this.largeClickSender.ActualHeight - this.senderMousePosition));
            }
            return (this.IsDirectionReversed ? (this.largeClickSender.ActualHeight - this.senderMousePosition) : this.senderMousePosition);
        }

        private double GetOffsetFromThumb()
        {
            double mouseOffset = this.GetMouseOffsetOnTrackClick();
            double thumbLength = this.GetThumbLengthOnTrackClick();
            double thumbContainerLength = this.GetThumbContainerLengthOnTrackClick();
            return this.ValueFromDistance(mouseOffset, thumbLength, thumbContainerLength);
        }

        private double GetRelativeMousePosition(FrameworkElement obj, System.Windows.Input.MouseButtonEventArgs e)
        {
            FrameworkElement template = null;
            if (this.Orientation == System.Windows.Controls.Orientation.Horizontal)
            {
                template = !base.IsSelectionRangeEnabled ? this.horSingleThumbTemplate : this.horRangeTemplate;
            }
            else
            {
                template = !base.IsSelectionRangeEnabled ? this.verSingleThumbTemplate : this.verRangeTemplate;
            }
            double absoluteDistanceToObject = (this.Orientation == System.Windows.Controls.Orientation.Horizontal) ? obj.TransformToVisual(template).Transform(emptyPoint).X : obj.TransformToVisual(template).Transform(emptyPoint).Y;
            double absoluteDistanceToMouse = (this.Orientation == System.Windows.Controls.Orientation.Horizontal) ? e.GetPosition(template).X : e.GetPosition(template).Y;
            return Math.Abs((double) (absoluteDistanceToMouse - absoluteDistanceToObject));
        }

        private void GetTemplatedChildren()
        {
            this.LayoutRoot = base.GetTemplateChild("LayoutRoot") as FrameworkElement;
            this.horTemplate = base.GetTemplateChild("HorizontalTemplate") as FrameworkElement;
            this.horSingleThumbTemplate = base.GetTemplateChild("HorizontalSingleThumbTemplate") as FrameworkElement;
            this.horSingleLargeDecrease = base.GetTemplateChild("HorizontalSingleLargeDecrease") as FrameworkElement;
            this.horSingleLargeIncrease = base.GetTemplateChild("HorizontalSingleLargeIncrease") as FrameworkElement;
            this.horSingleThumb = base.GetTemplateChild("HorizontalSingleThumb") as Thumb;
            this.horDecreaseHandle = base.GetTemplateChild("HorizontalDecreaseHandle") as RepeatButton;
            this.horIncreaseHandle = base.GetTemplateChild("HorizontalIncreaseHandle") as RepeatButton;
            this.horRangeTemplate = base.GetTemplateChild("HorizontalRangeTemplate") as FrameworkElement;
            this.horRangeLargeDecrease = base.GetTemplateChild("HorizontalRangeLargeDecrease") as FrameworkElement;
            this.horRangeLargeIncrease = base.GetTemplateChild("HorizontalRangeLargeIncrease") as FrameworkElement;
            this.horRangeStartThumb = base.GetTemplateChild("HorizontalRangeStartThumb") as Thumb;
            this.horRangeMiddleThumb = base.GetTemplateChild("HorizontalRangeMiddleThumb") as Thumb;
            this.horRangeEndThumb = base.GetTemplateChild("HorizontalRangeEndThumb") as Thumb;
            this.topTickBar = base.GetTemplateChild("TopTickBar") as RadTickBar;
            if (this.topTickBar != null)
            {
                this.topTickBar.ParentSlider = this;
            }
            this.bottomTickBar = base.GetTemplateChild("BottomTickBar") as RadTickBar;
            if (this.bottomTickBar != null)
            {
                this.bottomTickBar.ParentSlider = this;
            }
            this.verTemplate = base.GetTemplateChild("VerticalTemplate") as FrameworkElement;
            this.verSingleThumbTemplate = base.GetTemplateChild("VerticalSingleThumbTemplate") as FrameworkElement;
            this.verSingleLargeDecrease = base.GetTemplateChild("VerticalSingleLargeDecrease") as FrameworkElement;
            this.verSingleLargeIncrease = base.GetTemplateChild("VerticalSingleLargeIncrease") as FrameworkElement;
            this.verSingleThumb = base.GetTemplateChild("VerticalSingleThumb") as Thumb;
            this.verDecreaseHandle = base.GetTemplateChild("VerticalDecreaseHandle") as RepeatButton;
            this.verIncreaseHandle = base.GetTemplateChild("VerticalIncreaseHandle") as RepeatButton;
            this.verRangeTemplate = base.GetTemplateChild("VerticalRangeTemplate") as FrameworkElement;
            this.verRangeLargeDecrease = base.GetTemplateChild("VerticalRangeLargeDecrease") as FrameworkElement;
            this.verRangeLargeIncrease = base.GetTemplateChild("VerticalRangeLargeIncrease") as FrameworkElement;
            this.verRangeStartThumb = base.GetTemplateChild("VerticalRangeStartThumb") as Thumb;
            this.verRangeMiddleThumb = base.GetTemplateChild("VerticalRangeMiddleThumb") as Thumb;
            this.verRangeEndThumb = base.GetTemplateChild("VerticalRangeEndThumb") as Thumb;
            this.leftTickBar = base.GetTemplateChild("LeftTickBar") as RadTickBar;
            if (this.leftTickBar != null)
            {
                this.leftTickBar.ParentSlider = this;
            }
            this.rightTickBar = base.GetTemplateChild("RightTickBar") as RadTickBar;
            if (this.rightTickBar != null)
            {
                this.rightTickBar.ParentSlider = this;
            }
        }

        private double GetThumbContainerLengthOnTrackClick()
        {
            if (this.Orientation == System.Windows.Controls.Orientation.Horizontal)
            {
                return (base.IsSelectionRangeEnabled ? this.horRangeTemplate.ActualWidth : this.horSingleThumbTemplate.ActualWidth);
            }
            return (base.IsSelectionRangeEnabled ? this.verRangeTemplate.ActualHeight : this.verSingleThumbTemplate.ActualHeight);
        }

        private double GetThumbLengthOnTrackClick()
        {
            if (this.Orientation == System.Windows.Controls.Orientation.Horizontal)
            {
                if (this.directionOnLargeClick == ThumbMoveDirection.Increase)
                {
                    return (base.IsSelectionRangeEnabled ? this.horRangeEndThumb.ActualWidth : this.horSingleThumb.ActualWidth);
                }
                return (base.IsSelectionRangeEnabled ? this.horRangeStartThumb.ActualWidth : this.horSingleThumb.ActualWidth);
            }
            if (this.directionOnLargeClick == ThumbMoveDirection.Increase)
            {
                return (base.IsSelectionRangeEnabled ? this.verRangeEndThumb.ActualHeight : this.verSingleThumb.ActualHeight);
            }
            return (base.IsSelectionRangeEnabled ? this.verRangeStartThumb.ActualHeight : this.verSingleThumb.ActualHeight);
        }

        private void HorizontalLargeDecrease_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!e.Handled && base.Focus())
            {
                e.Handled = true;
            }
            this.largeClickSender = sender as FrameworkElement;
            this.senderMousePosition = this.GetRelativeMousePosition(this.largeClickSender, e);
            this.directionOnLargeClick = ThumbMoveDirection.Decrease;
            this.destinationValue = base.IsSelectionRangeEnabled ? base.SelectionStart : base.Value;
            this.destinationValue -= this.GetOffsetFromThumb();
            this.LargeClick();
            if (!this.IsMoveToPointEnabled)
            {
                this.isMouseDown = true;
                this.StartLargeStepTimer();
            }
        }

        private void HorizontalLargeIncrease_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!e.Handled && base.Focus())
            {
                e.Handled = true;
            }
            this.largeClickSender = sender as FrameworkElement;
            this.senderMousePosition = this.GetRelativeMousePosition(this.largeClickSender, e);
            this.directionOnLargeClick = ThumbMoveDirection.Increase;
            this.destinationValue = base.IsSelectionRangeEnabled ? base.SelectionEnd : base.Value;
            this.destinationValue += this.GetOffsetFromThumb();
            this.LargeClick();
            if (!this.IsMoveToPointEnabled)
            {
                this.isMouseDown = true;
                this.StartLargeStepTimer();
            }
        }

        private void IncreaseHandle_Click(object sender, RoutedEventArgs e)
        {
            base.Focus();
            this.OnIncreaseHandleClick();
        }

        private void Large_MouseEnter(object sender, MouseEventArgs e)
        {
            this.isMouseDown = false;
            this.StopLargeStepTimer();
        }

        private void Large_MouseLeave(object sender, MouseEventArgs e)
        {
            this.isMouseDown = false;
            this.StopLargeStepTimer();
        }

        private void Large_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.isMouseDown = false;
            this.StopLargeStepTimer();
        }

        private void LargeClick()
        {
            if (base.IsEnabled)
            {
                double newValue;
                base.RaiseSelectionRangeChangedEvent = this.StepAction == Telerik.Windows.Controls.StepAction.ChangeRange;
                if (this.IsMoveToPointEnabled)
                {
                    newValue = this.GetOffsetFromThumb();
                }
                else
                {
                    newValue = base.LargeChange;
                }
                if (this.IsSnapToTickEnabled)
                {
                    double change = (this.directionOnLargeClick == ThumbMoveDirection.Increase) ? newValue : (-1.0 * newValue);
                    double value = base.IsSelectionRangeEnabled ? ((this.directionOnLargeClick == ThumbMoveDirection.Increase) ? base.SelectionEnd : base.SelectionStart) : base.Value;
                    double nearestTickValue = this.MoveToNextTick(change, value);
                    if (!base.IsSelectionRangeEnabled)
                    {
                        if (base.Value != nearestTickValue)
                        {
                            base.RaiseValueChangedEvent = true;
                            base.Value = nearestTickValue;
                        }
                    }
                    else if (this.directionOnLargeClick != ThumbMoveDirection.Increase)
                    {
                        if (nearestTickValue != base.SelectionStart)
                        {
                            double oldRange = base.SelectionRange;
                            base.SelectionStart = nearestTickValue;
                            if (this.StepAction == Telerik.Windows.Controls.StepAction.MoveRange)
                            {
                                double difference = base.SelectionRange - oldRange;
                                base.RaiseSelectionRangeChangedEvent = true;
                                base.SelectionEnd -= difference;
                            }
                        }
                    }
                    else if (nearestTickValue != base.SelectionEnd)
                    {
                        double oldRange = base.SelectionRange;
                        base.SelectionEnd = nearestTickValue;
                        if (this.StepAction == Telerik.Windows.Controls.StepAction.MoveRange)
                        {
                            double difference = base.SelectionRange - oldRange;
                            base.RaiseSelectionRangeChangedEvent = true;
                            base.SelectionStart += difference;
                        }
                    }
                }
                else if (base.IsSelectionRangeEnabled)
                {
                    if (this.directionOnLargeClick != ThumbMoveDirection.Increase)
                    {
                        double step = ((base.SelectionStart - newValue) >= this.destinationValue) ? newValue : (base.SelectionStart - this.destinationValue);
                        base.SelectionStart -= step;
                        if (this.StepAction == Telerik.Windows.Controls.StepAction.MoveRange)
                        {
                            base.RaiseSelectionRangeChangedEvent = true;
                            base.SelectionEnd -= step;
                        }
                    }
                    else
                    {
                        double step = ((base.SelectionEnd + newValue) <= this.destinationValue) ? newValue : (this.destinationValue - base.SelectionEnd);
                        base.SelectionEnd += step;
                        if (this.StepAction == Telerik.Windows.Controls.StepAction.MoveRange)
                        {
                            base.RaiseSelectionRangeChangedEvent = true;
                            base.SelectionStart += step;
                        }
                    }
                }
                else
                {
                    base.RaiseValueChangedEvent = true;
                    if (this.directionOnLargeClick == ThumbMoveDirection.Increase)
                    {
                        base.Value = ((base.Value + newValue) <= this.destinationValue) ? (base.Value + newValue) : this.destinationValue;
                    }
                    else
                    {
                        base.Value = ((base.Value - newValue) >= this.destinationValue) ? (base.Value - newValue) : this.destinationValue;
                    }
                }
            }
        }

        private void LargeStepTimer_Tick(object sender, EventArgs e)
        {
            if (this.isMouseDown)
            {
                this.LargeClick();
            }
        }

        private bool MouseWheelMoved(double delta)
        {
            base.RaiseValueChangedEvent = true;
            bool eventHandled = true;
            double change = (delta > 0.0) ? Math.Min(base.Maximum - base.SelectionEnd, base.SmallChange) : -Math.Min(base.SelectionStart - base.Minimum, base.SmallChange);
            if (base.IsSelectionRangeEnabled)
            {
                if (((delta > 0.0) && (base.SelectionEnd == base.Maximum)) || ((delta < 0.0) && (base.SelectionStart == base.Minimum)))
                {
                    return false;
                }
                if (this.IsSnapToTickEnabled)
                {
                    double newSelectionStart = this.MoveToNextTick(change, base.SelectionStart);
                    double newSelectionEnd = this.MoveToNextTick(change, base.SelectionEnd);
                    if (newSelectionStart.IsGreaterThanOrClose(base.Minimum) && newSelectionEnd.IsLessThanOrClose(base.Maximum))
                    {
                        base.RaiseSelectionRangeChangedEvent = false;
                        base.SelectionStart = newSelectionStart;
                        base.RaiseSelectionRangeChangedEvent = true;
                        base.SelectionEnd = newSelectionEnd;
                    }
                    return eventHandled;
                }
                if ((base.SelectionEnd + change).IsLessThanOrClose(base.Maximum) && (base.SelectionStart + change).IsGreaterThanOrClose(base.Minimum))
                {
                    if (delta < 0.0)
                    {
                        base.RaiseSelectionRangeChangedEvent = false;
                        base.SelectionStart += change;
                        base.RaiseSelectionRangeChangedEvent = true;
                        base.SelectionEnd += change;
                        return eventHandled;
                    }
                    base.RaiseSelectionRangeChangedEvent = false;
                    base.SelectionEnd += change;
                    base.RaiseSelectionRangeChangedEvent = true;
                    base.SelectionStart += change;
                }
                return eventHandled;
            }
            if (((delta > 0.0) && (base.Value == base.Maximum)) || ((delta < 0.0) && (base.Value == base.Minimum)))
            {
                return false;
            }
            base.Value = this.IsSnapToTickEnabled ? this.MoveToNextTick(change, base.Value) : (base.Value + change);
            return eventHandled;
        }

        private bool MoveThumbAfterKeyPress(double change)
        {
            bool handleKeyDown = false;
            if (base.IsSelectionRangeEnabled)
            {
                if (((change < 0.0) || !base.SelectionEnd.IsCloseTo(base.Maximum)) && ((change > 0.0) || !base.SelectionStart.IsCloseTo(base.Minimum)))
                {
                    if (this.directionOnKeyDown == ThumbMoveDirection.Increase)
                    {
                        base.RaiseSelectionRangeChangedEvent = false;
                        double requestedSelectionEnd = this.IsSnapToTickEnabled ? this.MoveToNextTick(change, base.SelectionEnd) : (base.SelectionEnd + change);
                        handleKeyDown = base.SelectionEnd != requestedSelectionEnd;
                        base.SelectionEnd = requestedSelectionEnd;
                        base.RaiseSelectionRangeChangedEvent = true;
                        double requestedSelectionStart = this.IsSnapToTickEnabled ? this.MoveToNextTick(change, base.SelectionStart) : (base.SelectionStart + change);
                        handleKeyDown = base.SelectionStart != requestedSelectionStart;
                        base.SelectionStart = requestedSelectionStart;
                        return handleKeyDown;
                    }
                    base.RaiseSelectionRangeChangedEvent = false;
                    double _requestedSelectionStart = this.IsSnapToTickEnabled ? this.MoveToNextTick(change, base.SelectionStart) : (base.SelectionStart + change);
                    handleKeyDown = base.SelectionStart != _requestedSelectionStart;
                    base.SelectionStart = _requestedSelectionStart;
                    base.RaiseSelectionRangeChangedEvent = true;
                    double _requestedSelectionEnd = this.IsSnapToTickEnabled ? this.MoveToNextTick(change, base.SelectionEnd) : (base.SelectionEnd + change);
                    handleKeyDown = base.SelectionEnd != _requestedSelectionEnd;
                    base.SelectionEnd = _requestedSelectionEnd;
                }
                return handleKeyDown;
            }
            double requestedValue = this.IsSnapToTickEnabled ? this.MoveToNextTick(change, base.Value) : (base.Value + change);
            handleKeyDown = requestedValue != base.Value;
            base.Value = requestedValue;
            return handleKeyDown;
        }

        private double MoveToNextTick(double change, double value)
        {
            if (change == 0.0)
            {
                return 0.0;
            }
            double nextValue = this.SnapToTick(Math.Max(base.Minimum, Math.Min(base.Maximum, value + change)));
            bool flag = change > 0.0;
            if ((nextValue != value) || (flag && (value == base.Maximum)))
            {
                return nextValue;
            }
            if (!flag && (value == base.Minimum))
            {
                return nextValue;
            }
            DoubleCollection ticks = null;
            ticks = this.Ticks;
            if ((ticks != null) && (ticks.Count > 0))
            {
                for (int i = 0; i < ticks.Count; i++)
                {
                    double currentTick = ticks[i];
                    if (((flag && DoubleUtil.GreaterThan(currentTick, value)) && (DoubleUtil.LessThan(currentTick, nextValue) || (nextValue == value))) || ((!flag && DoubleUtil.LessThan(currentTick, value)) && (DoubleUtil.GreaterThan(currentTick, nextValue) || (nextValue == value))))
                    {
                        nextValue = currentTick;
                    }
                }
                return nextValue;
            }
            if (!DoubleUtil.GreaterThan(this.TickFrequency, 0.0))
            {
                return nextValue;
            }
            double num5 = Math.Round((double) ((value - base.Minimum) / this.TickFrequency));
            if (flag)
            {
                num5++;
            }
            else
            {
                num5--;
            }
            return (base.Minimum + (num5 * this.TickFrequency));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.DetachEvents();
            this.GetTemplatedChildren();
            this.AttachEvents();
            this.XamlInitialized = true;
            this.OnOrientationChanged();
            this.OnIsSelectionRangeEnabledChanged();
            this.OnTickPlacementChanged();
            if ((this.TickTemplate == null) && (this.LayoutRoot != null))
            {
                DataTemplate horizontalTickTemplate = this.LayoutRoot.Resources["HorizontalTickTemplate"] as DataTemplate;
                DataTemplate verticalTickTemplate = this.LayoutRoot.Resources["VerticalTickTemplate"] as DataTemplate;
                if ((this.topTickBar != null) && (horizontalTickTemplate != null))
                {
                    this.topTickBar.DefaultTickTemplate = horizontalTickTemplate;
                }
                if ((this.bottomTickBar != null) && (horizontalTickTemplate != null))
                {
                    this.bottomTickBar.DefaultTickTemplate = horizontalTickTemplate;
                }
                if ((this.leftTickBar != null) && (verticalTickTemplate != null))
                {
                    this.leftTickBar.DefaultTickTemplate = verticalTickTemplate;
                }
                if ((this.rightTickBar != null) && (verticalTickTemplate != null))
                {
                    this.rightTickBar.DefaultTickTemplate = verticalTickTemplate;
                }
            }
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new RadSliderAutomationPeer(this);
        }

        internal void OnDecreaseHandleClick()
        {
            base.RaiseSelectionRangeChangedEvent = this.StepAction == Telerik.Windows.Controls.StepAction.ChangeRange;
            base.RaiseValueChangedEvent = true;
            if (this.IsDirectionReversed)
            {
                if (!this.IsSnapToTickEnabled)
                {
                    double step = base.IsSelectionRangeEnabled ? Math.Min(base.SmallChange, base.Maximum - base.SelectionEnd) : base.SmallChange;
                    if (!base.IsSelectionRangeEnabled)
                    {
                        base.Value += step;
                    }
                    else
                    {
                        base.SelectionEnd += step;
                        if (this.StepAction == Telerik.Windows.Controls.StepAction.MoveRange)
                        {
                            base.RaiseSelectionRangeChangedEvent = true;
                            base.SelectionStart += step;
                        }
                    }
                }
                else
                {
                    double newValue = this.MoveToNextTick(base.SmallChange, base.IsSelectionRangeEnabled ? base.SelectionEnd : base.Value);
                    if (!base.IsSelectionRangeEnabled)
                    {
                        if (newValue != base.Value)
                        {
                            base.Value = newValue;
                        }
                    }
                    else if (newValue != base.SelectionEnd)
                    {
                        double oldRange = base.SelectionRange;
                        base.SelectionEnd = newValue;
                        if (this.StepAction == Telerik.Windows.Controls.StepAction.MoveRange)
                        {
                            double difference = base.SelectionRange - oldRange;
                            base.RaiseSelectionRangeChangedEvent = true;
                            base.SelectionStart += difference;
                        }
                    }
                }
            }
            else if (this.IsSnapToTickEnabled)
            {
                double newValue = this.MoveToNextTick(-1.0 * base.SmallChange, base.IsSelectionRangeEnabled ? base.SelectionStart : base.Value);
                if (!base.IsSelectionRangeEnabled)
                {
                    if (newValue != base.Value)
                    {
                        base.Value = newValue;
                    }
                }
                else if (newValue != base.SelectionStart)
                {
                    double oldRange = base.SelectionRange;
                    base.SelectionStart = newValue;
                    if (this.StepAction == Telerik.Windows.Controls.StepAction.MoveRange)
                    {
                        double difference = base.SelectionRange - oldRange;
                        base.RaiseSelectionRangeChangedEvent = true;
                        base.SelectionEnd -= difference;
                    }
                }
            }
            else
            {
                double step = base.IsSelectionRangeEnabled ? Math.Min(base.SmallChange, base.SelectionStart - base.Minimum) : base.SmallChange;
                if (base.IsSelectionRangeEnabled)
                {
                    base.SelectionStart -= step;
                    if (this.StepAction == Telerik.Windows.Controls.StepAction.MoveRange)
                    {
                        base.RaiseSelectionRangeChangedEvent = true;
                        base.SelectionEnd -= step;
                    }
                }
                else
                {
                    base.Value -= step;
                }
            }
        }

        private static void OnDelayPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadSlider slider = d as RadSlider;
            if ((slider != null) && (slider.largeStepTimer != null))
            {
                slider.largeStepTimer.Interval = TimeSpan.FromMilliseconds((double) ((int) e.NewValue));
            }
        }

        protected virtual void OnEnableSideTicksChanged()
        {
            switch (this.TickPlacement)
            {
                case Telerik.Windows.Controls.TickPlacement.None:
                    break;

                case Telerik.Windows.Controls.TickPlacement.TopLeft:
                    if ((this.Orientation == System.Windows.Controls.Orientation.Horizontal) && (this.topTickBar != null))
                    {
                        this.topTickBar.DrawTicks();
                    }
                    if ((this.Orientation != System.Windows.Controls.Orientation.Vertical) || (this.leftTickBar == null))
                    {
                        break;
                    }
                    this.leftTickBar.DrawTicks();
                    return;

                case Telerik.Windows.Controls.TickPlacement.BottomRight:
                    if ((this.Orientation == System.Windows.Controls.Orientation.Horizontal) && (this.bottomTickBar != null))
                    {
                        this.bottomTickBar.DrawTicks();
                    }
                    if ((this.Orientation != System.Windows.Controls.Orientation.Vertical) || (this.rightTickBar == null))
                    {
                        break;
                    }
                    this.rightTickBar.DrawTicks();
                    return;

                case Telerik.Windows.Controls.TickPlacement.Both:
                    if (this.Orientation != System.Windows.Controls.Orientation.Horizontal)
                    {
                        if (this.leftTickBar != null)
                        {
                            this.leftTickBar.DrawTicks();
                        }
                        if (this.rightTickBar != null)
                        {
                            this.rightTickBar.DrawTicks();
                        }
                        break;
                    }
                    if (this.topTickBar != null)
                    {
                        this.topTickBar.DrawTicks();
                    }
                    if (this.bottomTickBar == null)
                    {
                        break;
                    }
                    this.bottomTickBar.DrawTicks();
                    return;

                default:
                    return;
            }
        }

        private static void OnEnableSideTicksPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadSlider slider = d as RadSlider;
            if (slider != null)
            {
                slider.OnEnableSideTicksChanged();
            }
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            this.IsFocused = true;
        }

        private static void OnHandlesVisibilityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadSlider slider = d as RadSlider;
            if (slider.LayoutRoot != null)
            {
                slider.UpdateTrackLayout();
            }
        }

        internal void OnIncreaseHandleClick()
        {
            base.RaiseSelectionRangeChangedEvent = this.StepAction == Telerik.Windows.Controls.StepAction.ChangeRange;
            base.RaiseValueChangedEvent = true;
            if (this.IsDirectionReversed)
            {
                if (!this.IsSnapToTickEnabled)
                {
                    double step = base.IsSelectionRangeEnabled ? Math.Min(base.SmallChange, base.SelectionStart - base.Minimum) : base.SmallChange;
                    if (!base.IsSelectionRangeEnabled)
                    {
                        base.Value -= step;
                    }
                    else
                    {
                        base.SelectionStart -= step;
                        if (this.StepAction == Telerik.Windows.Controls.StepAction.MoveRange)
                        {
                            base.RaiseSelectionRangeChangedEvent = true;
                            base.SelectionEnd -= step;
                        }
                    }
                }
                else
                {
                    double newValue = this.MoveToNextTick(-1.0 * base.SmallChange, base.IsSelectionRangeEnabled ? base.SelectionStart : base.Value);
                    if (!base.IsSelectionRangeEnabled)
                    {
                        if (newValue != base.Value)
                        {
                            base.Value = newValue;
                        }
                    }
                    else if (newValue != base.SelectionStart)
                    {
                        double oldRange = base.SelectionRange;
                        base.SelectionStart = newValue;
                        if (this.StepAction == Telerik.Windows.Controls.StepAction.MoveRange)
                        {
                            double difference = base.SelectionRange - oldRange;
                            base.RaiseSelectionRangeChangedEvent = true;
                            base.SelectionEnd -= difference;
                        }
                    }
                }
            }
            else if (this.IsSnapToTickEnabled)
            {
                double newValue = this.MoveToNextTick(base.SmallChange, base.IsSelectionRangeEnabled ? base.SelectionEnd : base.Value);
                if (!base.IsSelectionRangeEnabled)
                {
                    if (newValue != base.Value)
                    {
                        base.Value = newValue;
                    }
                }
                else if (newValue != base.SelectionEnd)
                {
                    double oldRange = base.SelectionRange;
                    base.SelectionEnd = newValue;
                    if (this.StepAction == Telerik.Windows.Controls.StepAction.MoveRange)
                    {
                        double difference = base.SelectionRange - oldRange;
                        base.RaiseSelectionRangeChangedEvent = true;
                        base.SelectionStart += difference;
                    }
                }
            }
            else
            {
                double step = base.IsSelectionRangeEnabled ? Math.Min(base.SmallChange, base.Maximum - base.SelectionEnd) : base.SmallChange;
                if (base.IsSelectionRangeEnabled)
                {
                    base.SelectionEnd += step;
                    if (this.StepAction == Telerik.Windows.Controls.StepAction.MoveRange)
                    {
                        base.RaiseSelectionRangeChangedEvent = true;
                        base.SelectionStart += step;
                    }
                }
                else
                {
                    base.Value += step;
                }
            }
        }

        private static void OnIsDirectionReversedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadSlider slider = d as RadSlider;
            if (slider.LayoutRoot != null)
            {
                slider.UpdateTrackLayout();
            }
        }

        protected virtual void OnIsFocusedChanged()
        {
            this.ChangeVisualState();
        }

        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId="e")]
        private static void OnIsFocusedChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as RadSlider).OnIsFocusedChanged();
        }

        protected override void OnIsSelectionRangeEnabledChanged()
        {
            base.OnIsSelectionRangeEnabledChanged();
            if ((this.horTemplate != null) && (this.Orientation == System.Windows.Controls.Orientation.Horizontal))
            {
                if (this.horSingleThumbTemplate != null)
                {
                    this.horSingleThumbTemplate.Visibility = base.IsSelectionRangeEnabled ? Visibility.Collapsed : Visibility.Visible;
                }
                if (this.horRangeTemplate != null)
                {
                    this.horRangeTemplate.Visibility = base.IsSelectionRangeEnabled ? Visibility.Visible : Visibility.Collapsed;
                }
            }
            if ((this.verTemplate != null) && (this.Orientation == System.Windows.Controls.Orientation.Vertical))
            {
                if (this.verSingleThumbTemplate != null)
                {
                    this.verSingleThumbTemplate.Visibility = base.IsSelectionRangeEnabled ? Visibility.Collapsed : Visibility.Visible;
                }
                if (this.verRangeTemplate != null)
                {
                    this.verRangeTemplate.Visibility = base.IsSelectionRangeEnabled ? Visibility.Visible : Visibility.Collapsed;
                }
            }
            if (this.XamlInitialized)
            {
                this.UpdateTrackLayout();
                this.UpdateTickBarMargins();
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            e.Handled = this.OnKeyPressed(e.Key);
        }

        internal bool OnKeyPressed(Key key)
        {
            bool handleKeyDown = false;
            if (!base.IsEnabled)
            {
                return handleKeyDown;
            }
            if ((key == Key.Left) || (key == Key.Down))
            {
                double step = base.IsSelectionRangeEnabled ? Math.Min(base.SmallChange, base.SelectionStart - base.Minimum) : base.SmallChange;
                if (this.IsDirectionReversed)
                {
                    this.directionOnKeyDown = ThumbMoveDirection.Increase;
                    return this.MoveThumbAfterKeyPress(step);
                }
                this.directionOnKeyDown = ThumbMoveDirection.Decrease;
                return this.MoveThumbAfterKeyPress(-step);
            }
            if ((key == Key.Right) || (key == Key.Up))
            {
                double step = base.IsSelectionRangeEnabled ? Math.Min(base.SmallChange, base.Maximum - base.SelectionEnd) : base.SmallChange;
                if (this.IsDirectionReversed)
                {
                    this.directionOnKeyDown = ThumbMoveDirection.Decrease;
                    return this.MoveThumbAfterKeyPress(-step);
                }
                this.directionOnKeyDown = ThumbMoveDirection.Increase;
                return this.MoveThumbAfterKeyPress(step);
            }
            if (key == Key.Home)
            {
                if (base.IsSelectionRangeEnabled)
                {
                    double step = base.SelectionStart - base.Minimum;
                    handleKeyDown = base.SelectionStart != base.Minimum;
                    base.SelectionStart = base.Minimum;
                    double requestedSelectionEnd = this.IsSnapToTickEnabled ? this.MoveToNextTick(-step, base.SelectionEnd) : (base.SelectionEnd - step);
                    handleKeyDown = base.SelectionEnd != requestedSelectionEnd;
                    base.SelectionEnd = requestedSelectionEnd;
                    return handleKeyDown;
                }
                handleKeyDown = base.Value != base.Minimum;
                base.Value = base.Minimum;
                return handleKeyDown;
            }
            if (key == Key.End)
            {
                if (base.IsSelectionRangeEnabled)
                {
                    double step = base.Maximum - base.SelectionEnd;
                    handleKeyDown = base.SelectionEnd != base.Maximum;
                    base.SelectionEnd = base.Maximum;
                    double requestedSelectionStart = this.IsSnapToTickEnabled ? this.MoveToNextTick(step, base.SelectionStart) : (base.SelectionStart + step);
                    handleKeyDown = base.SelectionStart != requestedSelectionStart;
                    base.SelectionStart = requestedSelectionStart;
                    return handleKeyDown;
                }
                handleKeyDown = base.Value != base.Maximum;
                base.Value = base.Maximum;
                return handleKeyDown;
            }
            if (key == Key.PageUp)
            {
                double step = base.IsSelectionRangeEnabled ? Math.Min(base.LargeChange, base.Maximum - base.SelectionEnd) : base.LargeChange;
                if (this.IsDirectionReversed)
                {
                    this.directionOnKeyDown = ThumbMoveDirection.Decrease;
                    return this.MoveThumbAfterKeyPress(-step);
                }
                this.directionOnKeyDown = ThumbMoveDirection.Increase;
                return this.MoveThumbAfterKeyPress(step);
            }
            if (key != Key.PageDown)
            {
                return handleKeyDown;
            }
            double _step = base.IsSelectionRangeEnabled ? Math.Min(base.LargeChange, base.SelectionStart - base.Minimum) : base.LargeChange;
            if (this.IsDirectionReversed)
            {
                this.directionOnKeyDown = ThumbMoveDirection.Increase;
                return this.MoveThumbAfterKeyPress(_step);
            }
            this.directionOnKeyDown = ThumbMoveDirection.Decrease;
            return this.MoveThumbAfterKeyPress(-_step);
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            this.IsFocused = false;
            this.largeStepTimer.Stop();
        }

        protected override void OnMaximumChanged(double oldMaximum, double newMaximum)
        {
            base.OnMaximumChanged(oldMaximum, newMaximum);
            if (this.LayoutRoot != null)
            {
                this.UpdateTrackLayout();
            }
            if (this.TickFrequency > 0.0)
            {
                this.RedrawTicks();
            }
        }

        protected override void OnMaximumRangeSpanChanged(double oldValue, double newValue)
        {
            base.OnMaximumRangeSpanChanged(oldValue, newValue);
            if (this.XamlInitialized)
            {
                this.UpdateTrackLayout();
            }
        }

        protected override void OnMinimumChanged(double oldMinimum, double newMinimum)
        {
            base.OnMinimumChanged(oldMinimum, newMinimum);
            if (this.LayoutRoot != null)
            {
                this.UpdateTrackLayout();
            }
            if (this.TickFrequency > 0.0)
            {
                this.RedrawTicks();
            }
        }

        protected override void OnMinimumRangeSpanChanged(double oldValue, double newValue)
        {
            base.OnMinimumRangeSpanChanged(oldValue, newValue);
            if (this.XamlInitialized)
            {
                this.UpdateTrackLayout();
            }
        }

        protected override void OnMouseWheel(System.Windows.Input.MouseWheelEventArgs e)
        {
            if ((base.IsEnabled && this.IsMouseWheelEnabled) && this.IsFocused)
            {
                e.Handled = this.MouseWheelMoved((double) e.Delta);
            }
            base.OnMouseWheel(e);
        }

        private void OnMouseWheel(object sender, Telerik.Windows.Input.MouseWheelEventArgs args)
        {
            if ((base.IsEnabled && this.IsMouseWheelEnabled) && this.IsFocused)
            {
                args.Handled = this.MouseWheelMoved((double) args.Delta);
            }
        }

        protected virtual void OnOrientationChanged()
        {
            if (this.horTemplate != null)
            {
                this.horTemplate.Visibility = (this.Orientation == System.Windows.Controls.Orientation.Horizontal) ? Visibility.Visible : Visibility.Collapsed;
            }
            if (this.verTemplate != null)
            {
                this.verTemplate.Visibility = (this.Orientation == System.Windows.Controls.Orientation.Horizontal) ? Visibility.Collapsed : Visibility.Visible;
            }
            this.UpdateTrackLayout();
        }

        private static void OnOrientationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadSlider slider = d as RadSlider;
            if (slider.LayoutRoot != null)
            {
                slider.OnOrientationChanged();
                slider.OnIsSelectionRangeEnabledChanged();
                slider.OnTickPlacementChanged();
            }
        }

        protected override void OnSelectionEndChanged(double oldValue, double newValue)
        {
            base.OnSelectionEndChanged(oldValue, newValue);
            if (this.XamlInitialized)
            {
                this.UpdateTrackLayout();
            }
        }

        private void OnSelectionMiddleThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            double change = (this.Orientation == System.Windows.Controls.Orientation.Horizontal) ? e.HorizontalChange : e.VerticalChange;
            if (this.CheckSelectionRangeConstraints(change))
            {
                if (change > 0.0)
                {
                    this.singleThumbDragValue = this.selectionEndThumbDragValue;
                    Thumb selectionEndThumb = (this.Orientation == System.Windows.Controls.Orientation.Horizontal) ? this.horRangeEndThumb : this.verRangeEndThumb;
                    if (selectionEndThumb != null)
                    {
                        this.OnSingleThumbDragDelta(selectionEndThumb, e);
                    }
                    this.selectionEndThumbDragValue = this.singleThumbDragValue;
                    if (this.selectionEndThumbDragValue.IsCloseTo(base.Maximum))
                    {
                        base.SelectionStart = this.selectionStartThumbDragValue = this.selectionEndThumbDragValue - this.initialRange;
                    }
                    else
                    {
                        this.singleThumbDragValue = this.selectionStartThumbDragValue;
                        Thumb selectionStartThumb = (this.Orientation == System.Windows.Controls.Orientation.Horizontal) ? this.horRangeStartThumb : this.verRangeStartThumb;
                        if (selectionStartThumb != null)
                        {
                            this.OnSingleThumbDragDelta(selectionStartThumb, e);
                        }
                        this.selectionStartThumbDragValue = this.singleThumbDragValue;
                    }
                }
                else
                {
                    this.singleThumbDragValue = this.selectionStartThumbDragValue;
                    Thumb selectionStartThumb = (this.Orientation == System.Windows.Controls.Orientation.Horizontal) ? this.horRangeStartThumb : this.verRangeStartThumb;
                    if (selectionStartThumb != null)
                    {
                        this.OnSingleThumbDragDelta(selectionStartThumb, e);
                    }
                    this.selectionStartThumbDragValue = this.singleThumbDragValue;
                    if (this.selectionStartThumbDragValue.IsCloseTo(base.Minimum))
                    {
                        base.SelectionEnd = this.selectionEndThumbDragValue = this.selectionStartThumbDragValue + this.initialRange;
                    }
                    else
                    {
                        this.singleThumbDragValue = this.selectionEndThumbDragValue;
                        Thumb selectionEndThumb = (this.Orientation == System.Windows.Controls.Orientation.Horizontal) ? this.horRangeEndThumb : this.verRangeEndThumb;
                        if (selectionEndThumb != null)
                        {
                            this.OnSingleThumbDragDelta(selectionEndThumb, e);
                        }
                        this.selectionEndThumbDragValue = this.singleThumbDragValue;
                    }
                }
                if (!DoubleUtil.AreClose(Math.Round((double) (base.SelectionEnd - base.SelectionStart), 5), this.initialRange))
                {
                    if (base.SelectionEnd == base.Maximum)
                    {
                        base.SelectionStart = base.Maximum - this.initialRange;
                    }
                    if (base.SelectionStart == base.Minimum)
                    {
                        base.SelectionEnd = base.Minimum + this.initialRange;
                    }
                }
                if (this.DragDelta != null)
                {
                    RadDragDeltaEventArgs dragDeltaEventArgs = new RadDragDeltaEventArgs(e.HorizontalChange, e.VerticalChange, base.Value, base.SelectionStart, base.SelectionEnd);
                    this.DragDelta(this, dragDeltaEventArgs);
                }
            }
        }

        private void OnSelectionMiddleThumbDragStarted(object sender, DragStartedEventArgs e)
        {
            base.Focus();
            this.selectionStartThumbDragValue = base.SelectionStart;
            this.selectionEndThumbDragValue = base.SelectionEnd;
            this.oldSelectionStartThumbDragValue = base.SelectionStart;
            this.oldSelectionEndThumbDragValue = base.SelectionEnd;
            this.initialRange = base.SelectionEnd - base.SelectionStart;
            if (this.DragStarted != null)
            {
                RadDragStartedEventArgs dragStartedEventArgs = new RadDragStartedEventArgs(e.HorizontalOffset, e.VerticalOffset, base.Value, base.SelectionStart, base.SelectionEnd);
                this.DragStarted(this, dragStartedEventArgs);
            }
        }

        protected override void OnSelectionStartChanged(double oldValue, double newValue)
        {
            base.OnSelectionStartChanged(oldValue, newValue);
            if (this.XamlInitialized)
            {
                this.UpdateTrackLayout();
            }
        }

        private void OnSingleThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            base.RaiseSelectionRangeChangedEvent = !this.IsDeferredDraggingEnabled;
            base.RaiseValueChangedEvent = !this.IsDeferredDraggingEnabled;
            Thumb senderThumb = sender as Thumb;
            FrameworkElement containerTemplate = null;
            base.ChangeMadeByUi = true;
            if (this.Orientation == System.Windows.Controls.Orientation.Horizontal)
            {
                containerTemplate = base.IsSelectionRangeEnabled ? this.horRangeTemplate : this.horSingleThumbTemplate;
            }
            else
            {
                containerTemplate = base.IsSelectionRangeEnabled ? this.verRangeTemplate : this.verSingleThumbTemplate;
            }
            double newValue = this.SingleThumbDragDelta(senderThumb, e, containerTemplate);
            if ((senderThumb == this.horSingleThumb) || (sender == this.verSingleThumb))
            {
                base.Value = newValue;
            }
            else if ((sender == this.horRangeEndThumb) || (sender == this.verRangeEndThumb))
            {
                base.SelectionEnd = newValue;
            }
            else if ((sender == this.horRangeStartThumb) || (sender == this.verRangeStartThumb))
            {
                base.SelectionStart = newValue;
            }
            if (this.DragDelta != null)
            {
                RadDragDeltaEventArgs dragDeltaEventArgs = new RadDragDeltaEventArgs(e.HorizontalChange, e.VerticalChange, base.Value, base.SelectionStart, base.SelectionEnd);
                this.DragDelta(this, dragDeltaEventArgs);
            }
        }

        private void OnSingleThumbDragStarted(object sender, DragStartedEventArgs e)
        {
            base.Focus();
            Thumb senderThumb = sender as Thumb;
            if ((senderThumb == this.horSingleThumb) || (sender == this.verSingleThumb))
            {
                this.singleThumbDragValue = base.Value;
                this.oldSingleThumbDragValue = base.Value;
            }
            else if ((sender == this.horRangeStartThumb) || (sender == this.verRangeStartThumb))
            {
                this.singleThumbDragValue = base.SelectionStart;
            }
            else if ((sender == this.horRangeEndThumb) || (sender == this.verRangeEndThumb))
            {
                this.singleThumbDragValue = base.SelectionEnd;
            }
            if (this.DragStarted != null)
            {
                RadDragStartedEventArgs dragStartedEventArgs = new RadDragStartedEventArgs(e.HorizontalOffset, e.VerticalOffset, base.Value, base.SelectionStart, base.SelectionEnd);
                this.DragStarted(this, dragStartedEventArgs);
            }
        }

        private void OnThumbDragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (this.DragCompleted != null)
            {
                RadDragCompletedEventArgs dragCompletedEventArgs = new RadDragCompletedEventArgs(e.HorizontalChange, e.VerticalChange, base.Value, base.SelectionStart, base.SelectionEnd, e.Canceled);
                this.DragCompleted(this, dragCompletedEventArgs);
            }
            if (this.IsDeferredDraggingEnabled)
            {
                if (base.IsSelectionRangeEnabled)
                {
                    base.RaiseSelectionRangeChangedEvent = true;
                    SelectionRangeChangedEventArgs oldValuesArgs = new SelectionRangeChangedEventArgs(this.oldSelectionStartThumbDragValue, this.oldSelectionEndThumbDragValue);
                    SelectionRangeChangedEventArgs newValuesArgs = new SelectionRangeChangedEventArgs(base.SelectionStart, base.SelectionEnd);
                    this.OnSelectionRangeChanged(oldValuesArgs, newValuesArgs);
                }
                else
                {
                    base.OnValueChanged(this.oldSingleThumbDragValue, base.Value);
                }
            }
        }

        protected virtual void OnTickPlacementChanged()
        {
            if (this.XamlInitialized)
            {
                if (this.Orientation == System.Windows.Controls.Orientation.Horizontal)
                {
                    if (this.topTickBar != null)
                    {
                        this.topTickBar.Visibility = ((this.TickPlacement == Telerik.Windows.Controls.TickPlacement.TopLeft) || (this.TickPlacement == Telerik.Windows.Controls.TickPlacement.Both)) ? Visibility.Visible : Visibility.Collapsed;
                    }
                    if (this.bottomTickBar != null)
                    {
                        this.bottomTickBar.Visibility = ((this.TickPlacement == Telerik.Windows.Controls.TickPlacement.BottomRight) || (this.TickPlacement == Telerik.Windows.Controls.TickPlacement.Both)) ? Visibility.Visible : Visibility.Collapsed;
                    }
                }
                else
                {
                    if (this.leftTickBar != null)
                    {
                        this.leftTickBar.Visibility = ((this.TickPlacement == Telerik.Windows.Controls.TickPlacement.TopLeft) || (this.TickPlacement == Telerik.Windows.Controls.TickPlacement.Both)) ? Visibility.Visible : Visibility.Collapsed;
                    }
                    if (this.rightTickBar != null)
                    {
                        this.rightTickBar.Visibility = ((this.TickPlacement == Telerik.Windows.Controls.TickPlacement.BottomRight) || (this.TickPlacement == Telerik.Windows.Controls.TickPlacement.Both)) ? Visibility.Visible : Visibility.Collapsed;
                    }
                }
            }
        }

        private static void OnTickPlacementPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadSlider radSlider = d as RadSlider;
            if (radSlider != null)
            {
                radSlider.OnTickPlacementChanged();
            }
        }

        private static void OnTickPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadSlider radSlider = d as RadSlider;
            if (radSlider != null)
            {
                radSlider.RedrawTicks();
            }
        }

        protected override void OnValueChanged(double oldValue, double newValue)
        {
            RadSliderAutomationPeer peer = FrameworkElementAutomationPeer.FromElement(this) as RadSliderAutomationPeer;
            if (peer != null)
            {
                peer.RaiseValuePropertyChangedEvent(oldValue, newValue);
            }
            if ((this.IsDeferredDraggingEnabled && base.RaiseValueChangedEvent) || !this.IsDeferredDraggingEnabled)
            {
                base.OnValueChanged(oldValue, newValue);
            }
            if (this.LayoutRoot != null)
            {
                this.UpdateTrackLayout();
            }
            this.ChangeVisualState();
        }

        private void RadSlider_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.XamlInitialized)
            {
                this.UpdateTrackLayout();
                this.UpdateTickBarMargins();
            }
        }

        private void RedrawTicks()
        {
            if (this.Orientation == System.Windows.Controls.Orientation.Horizontal)
            {
                if (this.topTickBar != null)
                {
                    this.topTickBar.DrawTicks();
                }
                if (this.bottomTickBar != null)
                {
                    this.bottomTickBar.DrawTicks();
                }
            }
            else
            {
                if (this.leftTickBar != null)
                {
                    this.leftTickBar.DrawTicks();
                }
                if (this.rightTickBar != null)
                {
                    this.rightTickBar.DrawTicks();
                }
            }
        }

        private void SetLeftRightTickBarMargin(RadTickBar leftOrRightTickBar)
        {
            if (base.IsSelectionRangeEnabled)
            {
                if ((this.verRangeStartThumb != null) && (this.verRangeEndThumb != null))
                {
                    leftOrRightTickBar.Margin = new Thickness(0.0, this.verRangeEndThumb.ActualHeight, 0.0, this.verRangeStartThumb.ActualHeight);
                }
            }
            else if (this.verSingleThumb != null)
            {
                leftOrRightTickBar.Margin = new Thickness(0.0, this.verSingleThumb.ActualHeight / 2.0, 0.0, this.verSingleThumb.ActualHeight / 2.0);
            }
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        private void SetSingleThumbValue(double valueMultiplier)
        {
            Grid singleThumbTemplateGrid = (this.Orientation == System.Windows.Controls.Orientation.Horizontal) ? (this.horSingleThumbTemplate as Grid) : (this.verSingleThumbTemplate as Grid);
            if (singleThumbTemplateGrid != null)
            {
                if (this.Orientation == System.Windows.Controls.Orientation.Horizontal)
                {
                    singleThumbTemplateGrid.ColumnDefinitions[0].Width = new GridLength(1.0, this.IsDirectionReversed ? GridUnitType.Star : GridUnitType.Auto);
                    singleThumbTemplateGrid.ColumnDefinitions[2].Width = new GridLength(1.0, this.IsDirectionReversed ? GridUnitType.Auto : GridUnitType.Star);
                }
                else
                {
                    singleThumbTemplateGrid.RowDefinitions[0].Height = new GridLength(1.0, this.IsDirectionReversed ? GridUnitType.Auto : GridUnitType.Star);
                    singleThumbTemplateGrid.RowDefinitions[2].Height = new GridLength(1.0, this.IsDirectionReversed ? GridUnitType.Star : GridUnitType.Auto);
                }
                FrameworkElement largeDecreaseThumb = (this.Orientation == System.Windows.Controls.Orientation.Horizontal) ? this.horSingleLargeDecrease : this.verSingleLargeDecrease;
                FrameworkElement largeIncreaseThumb = (this.Orientation == System.Windows.Controls.Orientation.Horizontal) ? this.horSingleLargeIncrease : this.verSingleLargeIncrease;
                DependencyProperty rowOrColumnProperty = (this.Orientation == System.Windows.Controls.Orientation.Horizontal) ? Grid.ColumnProperty : Grid.RowProperty;
                int largeDecreaseThumbRowColumn = (this.Orientation == System.Windows.Controls.Orientation.Horizontal) ? 0 : 2;
                int largeIncreaseThumbRowColumn = (this.Orientation == System.Windows.Controls.Orientation.Horizontal) ? 2 : 0;
                if (largeDecreaseThumb != null)
                {
                    largeDecreaseThumb.SetValue(rowOrColumnProperty, this.IsDirectionReversed ? largeIncreaseThumbRowColumn : largeDecreaseThumbRowColumn);
                }
                if (largeIncreaseThumb != null)
                {
                    largeIncreaseThumb.SetValue(rowOrColumnProperty, this.IsDirectionReversed ? largeDecreaseThumbRowColumn : largeIncreaseThumbRowColumn);
                }
                if (this.Orientation == System.Windows.Controls.Orientation.Horizontal)
                {
                    double increaseHandleWidth = (this.horIncreaseHandle != null) ? this.horIncreaseHandle.DesiredSize.Width : 0.0;
                    double decreaseHandleWidth = (this.horDecreaseHandle != null) ? this.horDecreaseHandle.DesiredSize.Width : 0.0;
                    double trackWidth = (this.horTemplate.ActualWidth - increaseHandleWidth) - decreaseHandleWidth;
                    double expression = valueMultiplier * (trackWidth - this.horSingleThumb.ActualWidth);
                    largeDecreaseThumb.Width = (expression < 0.0) ? 0.0 : expression;
                }
                else
                {
                    double increaseHandleHeight = (this.verIncreaseHandle != null) ? this.verIncreaseHandle.DesiredSize.Height : 0.0;
                    double decreaseHandleHeight = (this.verDecreaseHandle != null) ? this.verDecreaseHandle.DesiredSize.Height : 0.0;
                    double trackHeight = (this.verTemplate.ActualHeight - increaseHandleHeight) - decreaseHandleHeight;
                    double expression = valueMultiplier * (trackHeight - this.verSingleThumb.ActualHeight);
                    largeDecreaseThumb.Height = (expression < 0.0) ? 0.0 : expression;
                }
            }
        }

        private void SetTopBottomTickBarMargin(RadTickBar topOrBottomTickBar)
        {
            if (base.IsSelectionRangeEnabled)
            {
                if ((this.horRangeStartThumb != null) && (this.horRangeEndThumb != null))
                {
                    topOrBottomTickBar.Margin = new Thickness(this.horRangeStartThumb.ActualWidth, 0.0, this.horRangeEndThumb.ActualWidth, 0.0);
                }
            }
            else if (this.horSingleThumb != null)
            {
                topOrBottomTickBar.Margin = new Thickness(this.horSingleThumb.ActualWidth / 2.0, 0.0, this.horSingleThumb.ActualWidth / 2.0, 0.0);
            }
        }

        private double SingleThumbDragDelta(Thumb sender, DragDeltaEventArgs args, FrameworkElement containerTemplate)
        {
            double offset = 0.0;
            if (((this.Orientation == System.Windows.Controls.Orientation.Horizontal) && (sender != null)) && (containerTemplate != null))
            {
                offset = (args.HorizontalChange / (containerTemplate.ActualWidth - sender.ActualWidth)) * (base.Maximum - base.Minimum);
            }
            else if (((this.Orientation == System.Windows.Controls.Orientation.Vertical) && (sender != null)) && (containerTemplate != null))
            {
                offset = (-args.VerticalChange / (containerTemplate.ActualHeight - sender.ActualHeight)) * (base.Maximum - base.Minimum);
            }
            if (!double.IsNaN(offset) && !double.IsInfinity(offset))
            {
                this.singleThumbDragValue += this.IsDirectionReversed ? -offset : offset;
                double newValue = Math.Min(base.Maximum, Math.Max(base.Minimum, this.singleThumbDragValue));
                return (this.IsSnapToTickEnabled ? this.SnapToTick(newValue) : newValue);
            }
            return 0.0;
        }

        private double SnapToTick(double value)
        {
            double minimum = base.Minimum;
            double maximum = base.Maximum;
            DoubleCollection ticks = null;
            ticks = this.Ticks;
            if ((ticks != null) && (ticks.Count > 0))
            {
                for (int i = 0; i < ticks.Count; i++)
                {
                    double tickValue = ticks[i];
                    if (DoubleUtil.AreClose(tickValue, value))
                    {
                        return value;
                    }
                    if (DoubleUtil.LessThan(tickValue, value) && DoubleUtil.GreaterThan(tickValue, minimum))
                    {
                        minimum = tickValue;
                    }
                    else if (DoubleUtil.GreaterThan(tickValue, value) && DoubleUtil.LessThan(tickValue, maximum))
                    {
                        maximum = tickValue;
                    }
                }
            }
            else if (DoubleUtil.GreaterThan(this.TickFrequency, 0.0))
            {
                minimum = base.Minimum + (Math.Round((double) ((value - base.Minimum) / this.TickFrequency)) * this.TickFrequency);
                maximum = Math.Min(base.Maximum, minimum + this.TickFrequency);
            }
            value = DoubleUtil.GreaterThan(value, (minimum + maximum) * 0.5) ? maximum : minimum;
            return value;
        }

        private void StartLargeStepTimer()
        {
            this.largeStepTimer.Tick += new EventHandler(this.LargeStepTimer_Tick);
            this.largeStepTimer.Start();
        }

        private void StopLargeStepTimer()
        {
            this.largeStepTimer.Stop();
            this.largeStepTimer.Tick -= new EventHandler(this.LargeStepTimer_Tick);
        }

        internal void TestLargeClick(double destination, ThumbMoveDirection direction)
        {
            this.destinationValue = destination;
            this.directionOnLargeClick = direction;
            this.LargeClick();
        }

        private void UpdateTickBarMargins()
        {
            if (this.topTickBar != null)
            {
                this.SetTopBottomTickBarMargin(this.topTickBar);
            }
            if (this.bottomTickBar != null)
            {
                this.SetTopBottomTickBarMargin(this.bottomTickBar);
            }
            if (this.leftTickBar != null)
            {
                this.SetLeftRightTickBarMargin(this.leftTickBar);
            }
            if (this.rightTickBar != null)
            {
                this.SetLeftRightTickBarMargin(this.rightTickBar);
            }
        }

        protected virtual void UpdateTrackLayout()
        {
            double maximum = base.Maximum;
            double minimum = base.Minimum;
            double range = maximum - minimum;
            if (range != 0.0)
            {
                double value = base.Value;
                double selectionStart = base.SelectionStart;
                double selectionEnd = base.SelectionEnd;
                double valueMultiplier = 1.0 - ((maximum - value) / (maximum - minimum));
                if (((this.Orientation == System.Windows.Controls.Orientation.Horizontal) ? (this.horTemplate as Grid) : (this.verTemplate as Grid)) != null)
                {
                    if (this.Orientation == System.Windows.Controls.Orientation.Horizontal)
                    {
                        if (base.IsSelectionRangeEnabled && (this.horRangeTemplate != null))
                        {
                            this.horRangeLargeDecrease.SetValue(Grid.ColumnProperty, this.IsDirectionReversed ? 4 : 0);
                            this.horRangeStartThumb.SetValue(Grid.ColumnProperty, this.IsDirectionReversed ? 3 : 1);
                            this.horRangeEndThumb.SetValue(Grid.ColumnProperty, this.IsDirectionReversed ? 1 : 3);
                            this.horRangeLargeIncrease.SetValue(Grid.ColumnProperty, this.IsDirectionReversed ? 0 : 4);
                            double availableSpace = this.horRangeTemplate.ActualWidth - (this.horRangeStartThumb.ActualWidth + this.horRangeEndThumb.ActualWidth);
                            double largeDecreaseSize = (availableSpace * (selectionStart - minimum)) / range;
                            double largeIncreaseSize = (availableSpace * (maximum - selectionEnd)) / range;
                            this.horRangeLargeDecrease.Width = (largeDecreaseSize < 0.0) ? 0.0 : largeDecreaseSize;
                            this.horRangeLargeIncrease.Width = (largeIncreaseSize < 0.0) ? 0.0 : largeIncreaseSize;
                        }
                        else
                        {
                            this.SetSingleThumbValue(valueMultiplier);
                        }
                    }
                    else if (base.IsSelectionRangeEnabled && (this.verRangeTemplate != null))
                    {
                        this.verRangeLargeDecrease.SetValue(Grid.RowProperty, this.IsDirectionReversed ? 0 : 4);
                        this.verRangeStartThumb.SetValue(Grid.RowProperty, this.IsDirectionReversed ? 1 : 3);
                        this.verRangeEndThumb.SetValue(Grid.RowProperty, this.IsDirectionReversed ? 3 : 1);
                        this.verRangeLargeIncrease.SetValue(Grid.RowProperty, this.IsDirectionReversed ? 4 : 0);
                        double availableSpace = this.verRangeTemplate.ActualHeight - (this.verRangeStartThumb.ActualHeight + this.verRangeEndThumb.ActualHeight);
                        double largeDecreaseSize = (availableSpace * (selectionStart - minimum)) / range;
                        double largeIncreaseSize = (availableSpace * (maximum - selectionEnd)) / range;
                        this.verRangeLargeDecrease.Height = (largeDecreaseSize < 0.0) ? 0.0 : largeDecreaseSize;
                        this.verRangeLargeIncrease.Height = (largeIncreaseSize < 0.0) ? 0.0 : largeIncreaseSize;
                    }
                    else
                    {
                        this.SetSingleThumbValue(valueMultiplier);
                    }
                }
            }
        }

        private double ValueFromDistance(double mouseOffset, double thumbLength, double thumbContainerLength)
        {
            mouseOffset += thumbLength / 2.0;
            return ((mouseOffset * (base.Maximum - base.Minimum)) / (thumbContainerLength - thumbLength));
        }

        private void VerticalLargeDecrease_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!e.Handled && base.Focus())
            {
                e.Handled = true;
            }
            this.largeClickSender = sender as FrameworkElement;
            this.senderMousePosition = this.GetRelativeMousePosition(this.largeClickSender, e);
            this.directionOnLargeClick = ThumbMoveDirection.Decrease;
            this.destinationValue = base.IsSelectionRangeEnabled ? base.SelectionStart : base.Value;
            this.destinationValue -= this.GetOffsetFromThumb();
            this.LargeClick();
            if (!this.IsMoveToPointEnabled)
            {
                this.isMouseDown = true;
                this.StartLargeStepTimer();
            }
        }

        private void VerticalLargeIncrease_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!e.Handled && base.Focus())
            {
                e.Handled = true;
            }
            this.largeClickSender = sender as FrameworkElement;
            this.senderMousePosition = this.GetRelativeMousePosition(this.largeClickSender, e);
            this.directionOnLargeClick = ThumbMoveDirection.Increase;
            this.destinationValue = base.IsSelectionRangeEnabled ? base.SelectionEnd : base.Value;
            this.destinationValue += this.GetOffsetFromThumb();
            this.LargeClick();
            if (!this.IsMoveToPointEnabled)
            {
                this.isMouseDown = true;
                this.StartLargeStepTimer();
            }
        }

        internal RadTickBar BottomTickBar
        {
            get
            {
                return this.bottomTickBar;
            }
            set
            {
                this.bottomTickBar = value;
            }
        }

        public int Delay
        {
            get
            {
                return (int) base.GetValue(DelayProperty);
            }
            set
            {
                base.SetValue(DelayProperty, value);
            }
        }

        public bool EnableSideTicks
        {
            get
            {
                return (bool) base.GetValue(EnableSideTicksProperty);
            }
            set
            {
                base.SetValue(EnableSideTicksProperty, value);
            }
        }

        public Visibility HandlesVisibility
        {
            get
            {
                return (Visibility) base.GetValue(HandlesVisibilityProperty);
            }
            set
            {
                base.SetValue(HandlesVisibilityProperty, value);
            }
        }

        internal RepeatButton HorizontalDecreaseHandle
        {
            get
            {
                return this.horDecreaseHandle;
            }
            set
            {
                this.horDecreaseHandle = value;
            }
        }

        internal RepeatButton HorizontalIncreaseHandle
        {
            get
            {
                return this.horIncreaseHandle;
            }
            set
            {
                this.horIncreaseHandle = value;
            }
        }

        internal Thumb HorizontalRangeEndThumb
        {
            get
            {
                return this.horRangeEndThumb;
            }
            set
            {
                this.horRangeEndThumb = value;
            }
        }

        internal FrameworkElement HorizontalRangeLargeDecrease
        {
            get
            {
                return this.horRangeLargeDecrease;
            }
            set
            {
                this.horRangeLargeDecrease = value;
            }
        }

        internal FrameworkElement HorizontalRangeLargeIncrease
        {
            get
            {
                return this.horRangeLargeIncrease;
            }
            set
            {
                this.horRangeLargeIncrease = value;
            }
        }

        internal Thumb HorizontalRangeMiddleThumb
        {
            get
            {
                return this.horRangeMiddleThumb;
            }
            set
            {
                this.horRangeMiddleThumb = value;
            }
        }

        internal Thumb HorizontalRangeStartThumb
        {
            get
            {
                return this.horRangeStartThumb;
            }
            set
            {
                this.horRangeStartThumb = value;
            }
        }

        internal FrameworkElement HorizontalRangeTemplate
        {
            get
            {
                return this.horRangeTemplate;
            }
            set
            {
                this.horRangeTemplate = value;
            }
        }

        internal FrameworkElement HorizontalSingleLargeDecrease
        {
            get
            {
                return this.horSingleLargeDecrease;
            }
            set
            {
                this.horSingleLargeDecrease = value;
            }
        }

        internal FrameworkElement HorizontalSingleLargeIncrease
        {
            get
            {
                return this.horSingleLargeIncrease;
            }
            set
            {
                this.horSingleLargeIncrease = value;
            }
        }

        internal Thumb HorizontalSingleThumb
        {
            get
            {
                return this.horSingleThumb;
            }
            set
            {
                this.horSingleThumb = value;
            }
        }

        internal FrameworkElement HorizontalSingleThumbTemplate
        {
            get
            {
                return this.horSingleThumbTemplate;
            }
            set
            {
                this.horSingleThumbTemplate = value;
            }
        }

        internal FrameworkElement HorizontalTemplate
        {
            get
            {
                return this.horTemplate;
            }
            set
            {
                this.horTemplate = value;
            }
        }

        public bool IsDeferredDraggingEnabled
        {
            get
            {
                return (bool) base.GetValue(IsDeferredDraggingEnabledProperty);
            }
            set
            {
                base.SetValue(IsDeferredDraggingEnabledProperty, value);
            }
        }

        public bool IsDirectionReversed
        {
            get
            {
                return (bool) base.GetValue(IsDirectionReversedProperty);
            }
            set
            {
                base.SetValue(IsDirectionReversedProperty, value);
            }
        }

        [Browsable(false)]
        public bool IsFocused
        {
            get
            {
                return (bool) base.GetValue(IsFocusedProperty);
            }
            internal set
            {
                this.SetValue(IsFocusedPropertyKey, value);
            }
        }

        public bool IsMouseWheelEnabled
        {
            get
            {
                return (bool) base.GetValue(IsMouseWheelEnabledProperty);
            }
            set
            {
                base.SetValue(IsMouseWheelEnabledProperty, value);
            }
        }

        public bool IsMoveToPointEnabled
        {
            get
            {
                return (bool) base.GetValue(IsMoveToPointEnabledProperty);
            }
            set
            {
                base.SetValue(IsMoveToPointEnabledProperty, value);
            }
        }

        public bool IsSnapToTickEnabled
        {
            get
            {
                return (bool) base.GetValue(IsSnapToTickEnabledProperty);
            }
            set
            {
                base.SetValue(IsSnapToTickEnabledProperty, value);
            }
        }

        internal FrameworkElement LayoutRoot { get; set; }

        internal RadTickBar LeftTickBar
        {
            get
            {
                return this.leftTickBar;
            }
            set
            {
                this.leftTickBar = value;
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

        public int RepeatInterval
        {
            get
            {
                return (int) base.GetValue(RepeatIntervalProperty);
            }
            set
            {
                base.SetValue(RepeatIntervalProperty, value);
            }
        }

        internal RadTickBar RightTickBar
        {
            get
            {
                return this.rightTickBar;
            }
            set
            {
                this.rightTickBar = value;
            }
        }

        public Telerik.Windows.Controls.StepAction StepAction
        {
            get
            {
                return (Telerik.Windows.Controls.StepAction) base.GetValue(StepActionProperty);
            }
            set
            {
                base.SetValue(StepActionProperty, value);
            }
        }

        public Visibility ThumbVisibility
        {
            get
            {
                return (Visibility) base.GetValue(ThumbVisibilityProperty);
            }
            set
            {
                base.SetValue(ThumbVisibilityProperty, value);
            }
        }

        public double TickFrequency
        {
            get
            {
                return (double) base.GetValue(TickFrequencyProperty);
            }
            set
            {
                base.SetValue(TickFrequencyProperty, value);
            }
        }

        public Telerik.Windows.Controls.TickPlacement TickPlacement
        {
            get
            {
                return (Telerik.Windows.Controls.TickPlacement) base.GetValue(TickPlacementProperty);
            }
            set
            {
                base.SetValue(TickPlacementProperty, value);
            }
        }

        public DoubleCollection Ticks
        {
            get
            {
                return (DoubleCollection) base.GetValue(TicksProperty);
            }
            set
            {
                base.SetValue(TicksProperty, value);
            }
        }

        public DataTemplate TickTemplate
        {
            get
            {
                return (DataTemplate) base.GetValue(TickTemplateProperty);
            }
            set
            {
                base.SetValue(TickTemplateProperty, value);
            }
        }

        public DataTemplateSelector TickTemplateSelector
        {
            get
            {
                return (DataTemplateSelector) base.GetValue(TickTemplateSelectorProperty);
            }
            set
            {
                base.SetValue(TickTemplateSelectorProperty, value);
            }
        }

        internal RadTickBar TopTickBar
        {
            get
            {
                return this.topTickBar;
            }
            set
            {
                this.topTickBar = value;
            }
        }

        internal RepeatButton VerticalDecreaseHandle
        {
            get
            {
                return this.verDecreaseHandle;
            }
            set
            {
                this.verDecreaseHandle = value;
            }
        }

        internal RepeatButton VerticalIncreaseHandle
        {
            get
            {
                return this.verIncreaseHandle;
            }
            set
            {
                this.verIncreaseHandle = value;
            }
        }

        internal Thumb VerticalRangeEndThumb
        {
            get
            {
                return this.verRangeEndThumb;
            }
            set
            {
                this.verRangeEndThumb = value;
            }
        }

        internal FrameworkElement VerticalRangeLargeDecrease
        {
            get
            {
                return this.verRangeLargeDecrease;
            }
            set
            {
                this.verRangeLargeDecrease = value;
            }
        }

        internal FrameworkElement VerticalRangeLargeIncrease
        {
            get
            {
                return this.verRangeLargeIncrease;
            }
            set
            {
                this.verRangeLargeIncrease = value;
            }
        }

        internal Thumb VerticalRangeMiddleThumb
        {
            get
            {
                return this.verRangeMiddleThumb;
            }
            set
            {
                this.verRangeMiddleThumb = value;
            }
        }

        internal Thumb VerticalRangeStartThumb
        {
            get
            {
                return this.verRangeStartThumb;
            }
            set
            {
                this.verRangeStartThumb = value;
            }
        }

        internal FrameworkElement VerticalRangeTemplate
        {
            get
            {
                return this.verRangeTemplate;
            }
            set
            {
                this.verRangeTemplate = value;
            }
        }

        internal FrameworkElement VerticalSingleLargeDecrease
        {
            get
            {
                return this.verSingleLargeDecrease;
            }
            set
            {
                this.verSingleLargeDecrease = value;
            }
        }

        internal FrameworkElement VerticalSingleLargeIncrease
        {
            get
            {
                return this.verSingleLargeIncrease;
            }
            set
            {
                this.verSingleLargeIncrease = value;
            }
        }

        internal Thumb VerticalSingleThumb
        {
            get
            {
                return this.verSingleThumb;
            }
            set
            {
                this.verSingleThumb = value;
            }
        }

        internal FrameworkElement VerticalSingleThumbTemplate
        {
            get
            {
                return this.verSingleThumbTemplate;
            }
            set
            {
                this.verSingleThumbTemplate = value;
            }
        }

        internal FrameworkElement VerticalTemplate
        {
            get
            {
                return this.verTemplate;
            }
            set
            {
                this.verTemplate = value;
            }
        }

        internal bool XamlInitialized { get; set; }
    }
}

