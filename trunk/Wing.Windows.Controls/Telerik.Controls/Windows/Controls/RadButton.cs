namespace Telerik.Windows.Controls
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Threading;
    using Telerik.Windows;
    using Telerik.Windows.Controls.Design;

    public class RadButton : Button, ICommandSource
    {
        public static readonly Telerik.Windows.RoutedEvent ActivateEvent = EventManager.RegisterRoutedEvent("Activate", RoutingStrategy.Bubble, typeof(EventHandler<RadRoutedEventArgs>), typeof(RadButton));
        public static readonly Telerik.Windows.RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(RadButton));
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(RadButton), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadButton.OnCommandParameterChanged)));
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(RadButton), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadButton.OnCommandChanged)));
        public static readonly DependencyProperty CommandTargetProperty = DependencyProperty.Register("CommandTarget", typeof(UIElement), typeof(RadButton), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadButton.OnCommandTargetChanged)));
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(System.Windows.CornerRadius), typeof(RadButton), new Telerik.Windows.PropertyMetadata());
        private EventHandler guardCanExecuteChangedHandler;
        public static readonly DependencyProperty HoverDelayProperty = DependencyProperty.Register("HoverDelay", typeof(TimeSpan), typeof(RadButton), new Telerik.Windows.PropertyMetadata(TimeSpan.Zero, new PropertyChangedCallback(RadButton.OnHoverDelayChanged)));
        public static readonly Telerik.Windows.RoutedEvent HoverEvent = EventManager.RegisterRoutedEvent("Hover", RoutingStrategy.Bubble, typeof(EventHandler<RadRoutedEventArgs>), typeof(RadButton));
        private DispatcherTimer hoverTimer;
        public static readonly DependencyProperty IsBackgroundVisibleProperty = DependencyProperty.Register("IsBackgroundVisible", typeof(bool), typeof(RadButton), new Telerik.Windows.PropertyMetadata(true));

        public event EventHandler<RadRoutedEventArgs> Activate
        {
            add
            {
                this.AddHandler(ActivateEvent, value);
            }
            remove
            {
                this.RemoveHandler(ActivateEvent, value);
            }
        }

        [Browsable(false)]
        public event EventHandler<RadRoutedEventArgs> Hover
        {
            add
            {
                this.AddHandler(HoverEvent, value);
            }
            remove
            {
                this.RemoveHandler(HoverEvent, value);
            }
        }

        public RadButton()
        {
            base.DefaultStyleKey = typeof(RadButton);
            base.Loaded += new RoutedEventHandler(this.OnLoaded);
            
        }

        private void CanExecuteApply()
        {
            if (this.Command != null)
            {
                RoutedCommand routedCommand = this.Command as RoutedCommand;
                base.IsEnabled = (routedCommand == null) ? this.Command.CanExecute(this.CommandParameter) : routedCommand.CanExecute(this.CommandParameter, this.CommandTarget ?? this);
            }
        }

        private void CanExecuteChanged(object sender, EventArgs e)
        {
            this.CanExecuteApply();
        }

        private void ChangeCommand(ICommand oldCommand, ICommand newCommand)
        {
            if (oldCommand != null)
            {
                oldCommand.CanExecuteChanged -= new EventHandler(this.CanExecuteChanged);
            }
            if (newCommand != null)
            {
                this.guardCanExecuteChangedHandler = new EventHandler(this.CanExecuteChanged);
                newCommand.CanExecuteChanged += this.guardCanExecuteChangedHandler;
            }
            this.CanExecuteApply();
        }

        internal virtual void ChangeVisualState(bool useTransitions)
        {
            if (!base.IsEnabled)
            {
                this.GoToState("Disabled", useTransitions);
            }
            else if (base.IsPressed)
            {
                this.GoToState("Pressed", useTransitions);
            }
            else if (base.IsMouseOver)
            {
                this.GoToState("MouseOver", useTransitions);
            }
            else
            {
                this.GoToState("Normal", useTransitions);
            }
            if (base.IsFocused && base.IsEnabled)
            {
                this.GoToState("Focused", useTransitions);
            }
            else
            {
                this.GoToState("Unfocused", useTransitions);
            }
        }

        private void ExecuteCommand()
        {
            if (this.Command != null)
            {
                RoutedCommand routedCommand = this.Command as RoutedCommand;
                if (routedCommand == null)
                {
                    this.Command.Execute(this.CommandParameter);
                }
                else
                {
                    routedCommand.Execute(this.CommandParameter, this.CommandTarget ?? this);
                }
            }
        }

        internal bool GoToState(string stateName, bool useTransitions)
        {
            return VisualStateManager.GoToState(this, stateName, useTransitions);
        }

        internal static bool HasChildOf(DependencyObject parent, DependencyObject child, bool skipParent)
        {
            if ((parent == null) || (child == null))
            {
                return false;
            }
            if (!skipParent && (parent == child))
            {
                return true;
            }
            FrameworkElement element = child as FrameworkElement;
            DependencyObject currentParent = VisualTreeHelper.GetParent(child);
            if ((currentParent == null) && (element != null))
            {
                currentParent = element.Parent;
            }
            return HasChildOf(parent, currentParent, false);
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private bool HoverTimerApplyState(bool applyAction)
        {
            double milliseconds = this.HoverDelay.TotalMilliseconds;
            if (milliseconds > 0.0)
            {
                if (this.hoverTimer == null)
                {
                    this.hoverTimer = new DispatcherTimer();
                    this.hoverTimer.Tick += new EventHandler(this.OnHoverTimerTick);
                }
                if (milliseconds != this.hoverTimer.Interval.TotalMilliseconds)
                {
                    this.hoverTimer.Interval = this.HoverDelay;
                }
                if (applyAction)
                {
                    if (base.IsMouseOver)
                    {
                        this.HoverTimerStart();
                    }
                    else
                    {
                        this.HoverTimerStop();
                    }
                }
            }
            else
            {
                this.HoverTimerDestroy();
            }
            return (this.hoverTimer != null);
        }

        private void HoverTimerDestroy()
        {
            if (this.hoverTimer != null)
            {
                this.hoverTimer.Stop();
                this.hoverTimer.Tick -= new EventHandler(this.OnHoverTimerTick);
                this.hoverTimer = null;
            }
        }

        private void HoverTimerRestart()
        {
            if (this.HoverTimerApplyState(false))
            {
                this.hoverTimer.Start();
            }
        }

        private void HoverTimerStart()
        {
            if (this.HoverTimerApplyState(false))
            {
                this.hoverTimer.Start();
            }
        }

        private void HoverTimerStop()
        {
            if (this.HoverTimerApplyState(false))
            {
                this.hoverTimer.Stop();
            }
        }

        protected internal virtual void OnActivate()
        {
            this.RaiseEvent(new RadRoutedEventArgs(ActivateEvent, this));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.ChangeVisualState(false);
        }

        protected override void OnClick()
        {
            this.RaiseClick();
            base.OnClick();
            this.OnActivate();
            this.ExecuteCommand();
        }

        internal void OnClickInternal()
        {
            this.OnClick();
        }

        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadButton button = d as RadButton;
            if (button != null)
            {
                button.ChangeCommand((ICommand) e.OldValue, (ICommand) e.NewValue);
            }
        }

        private static void OnCommandParameterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadButton button = d as RadButton;
            if (button != null)
            {
                button.CanExecuteApply();
            }
        }

        private static void OnCommandTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadButton button = d as RadButton;
            if (button != null)
            {
                button.CanExecuteApply();
            }
        }

        protected internal virtual void OnHover()
        {
            this.RaiseEvent(new RadRoutedEventArgs(HoverEvent, this));
        }

        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId="e")]
        private static void OnHoverDelayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadButton button = d as RadButton;
            if (button != null)
            {
                button.HoverTimerApplyState(true);
            }
        }

        private void OnHoverTimerTick(object sender, EventArgs e)
        {
            this.HoverTimerStop();
            this.OnHover();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
        }

        private void OnLoaded(object sender, EventArgs e)
        {
            this.CanExecuteApply();
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            this.HoverTimerStart();
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            this.HoverTimerStop();
            base.OnMouseLeave(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            this.HoverTimerRestart();
            base.OnMouseMove(e);
        }

        private RadRoutedEventArgs RaiseClick()
        {
            return RaiseRadRoutedEvent(ClickEvent, this);
        }

        internal static RadRoutedEventArgs RaiseRadRoutedEvent(Telerik.Windows.RoutedEvent routedEvent, UIElement source)
        {
            RadRoutedEventArgs e = new RadRoutedEventArgs(routedEvent, source);
            source.RaiseEvent(e);
            return e;
        }

        [TypeConverter(typeof(CommandConverter))]
        public ICommand Command
        {
            get
            {
                return (ICommand) base.GetValue(CommandProperty);
            }
            set
            {
                base.SetValue(CommandProperty, value);
            }
        }

        public object CommandParameter
        {
            get
            {
                return base.GetValue(CommandParameterProperty);
            }
            set
            {
                base.SetValue(CommandParameterProperty, value);
            }
        }

        public UIElement CommandTarget
        {
            get
            {
                return (UIElement) base.GetValue(CommandTargetProperty);
            }
            set
            {
                base.SetValue(CommandTargetProperty, value);
            }
        }

        public System.Windows.CornerRadius CornerRadius
        {
            get
            {
                return (System.Windows.CornerRadius) base.GetValue(CornerRadiusProperty);
            }
            set
            {
                base.SetValue(CornerRadiusProperty, value);
            }
        }

        [Browsable(false)]
        public TimeSpan HoverDelay
        {
            get
            {
                return (TimeSpan) base.GetValue(HoverDelayProperty);
            }
            set
            {
                base.SetValue(HoverDelayProperty, value);
            }
        }

        [Browsable(false)]
        public bool IsBackgroundVisible
        {
            get
            {
                return (bool) base.GetValue(IsBackgroundVisibleProperty);
            }
            set
            {
                base.SetValue(IsBackgroundVisibleProperty, value);
            }
        }
    }
}

