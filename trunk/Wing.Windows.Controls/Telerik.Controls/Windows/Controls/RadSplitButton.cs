namespace Telerik.Windows.Controls
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using System.Windows.Threading;
    using Telerik.Windows;
    using Telerik.Windows.Controls.Design;
    using Telerik.Windows.Controls.Primitives;

    public class RadSplitButton : ContentControl, ICommandSource
    {
        public static readonly Telerik.Windows.RoutedEvent ActivateEvent;
        public static readonly DependencyProperty AutoOpenDelayProperty = DependencyProperty.Register("AutoOpenDelay", typeof(TimeSpan), typeof(RadSplitButton), new Telerik.Windows.PropertyMetadata(TimeSpan.Zero, new PropertyChangedCallback(RadSplitButton.OnAutoOpenDelayChanged)));
        private DispatcherTimer autoOpenTimer;
        public static readonly Telerik.Windows.RoutedEvent CheckedEvent;
        public static readonly Telerik.Windows.RoutedEvent ClickEvent;
        public static readonly DependencyProperty CloseOnEscapeProperty = DependencyProperty.Register("CloseOnEscape", typeof(bool), typeof(RadSplitButton), new Telerik.Windows.PropertyMetadata(true));
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(RadSplitButton), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadSplitButton.OnCommandParameterChanged)));
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(RadSplitButton), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadSplitButton.OnCommandChanged)));
        public static readonly DependencyProperty CommandTargetProperty = DependencyProperty.Register("CommandTarget", typeof(UIElement), typeof(RadSplitButton), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadSplitButton.OnCommandTargetChanged)));
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(System.Windows.CornerRadius), typeof(RadSplitButton), new Telerik.Windows.PropertyMetadata());
        public static readonly DependencyProperty DropDownButtonPositionProperty;
        public static readonly Telerik.Windows.RoutedEvent DropDownClosedEvent;
        public static readonly DependencyProperty DropDownContentProperty;
        public static readonly DependencyProperty DropDownContentTemplateProperty;
        public static readonly DependencyProperty DropDownContentTemplateSelectorProperty;
        public static readonly DependencyProperty DropDownHeightProperty;
        public static readonly DependencyProperty DropDownIndicatorVisibilityProperty = DependencyProperty.Register("DropDownIndicatorVisibility", typeof(Visibility), typeof(RadSplitButton), new Telerik.Windows.PropertyMetadata(Visibility.Visible));
        public static readonly DependencyProperty DropDownMaxHeightProperty;
        public static readonly DependencyProperty DropDownMaxWidthProperty;
        public static readonly Telerik.Windows.RoutedEvent DropDownOpenedEvent;
        public static readonly Telerik.Windows.RoutedEvent DropDownOpeningEvent;
        public static readonly DependencyProperty DropDownPlacementProperty;
        public static readonly DependencyProperty DropDownWidthProperty;
        private EventHandler guardCanExecuteChangedHandler;
        public static readonly DependencyProperty IsBackgroundVisibleProperty = DependencyProperty.Register("IsBackgroundVisible", typeof(bool), typeof(RadSplitButton), new Telerik.Windows.PropertyMetadata(true));
        public static readonly DependencyProperty IsCheckedProperty;
        public static readonly DependencyProperty IsMouseOverProperty;
        public static readonly DependencyProperty IsOpenProperty;
        private bool isTemplateApplied;
        public static readonly DependencyProperty IsToggleProperty;
        public static readonly Telerik.Windows.RoutedEvent UncheckedEvent;

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

        public event RoutedEventHandler Checked
        {
            add
            {
                this.AddHandler(CheckedEvent, value);
            }
            remove
            {
                this.RemoveHandler(CheckedEvent, value);
            }
        }

        public event RoutedEventHandler Click
        {
            add
            {
                this.AddHandler(ClickEvent, value);
            }
            remove
            {
                this.RemoveHandler(ClickEvent, value);
            }
        }

        public event RoutedEventHandler DropDownClosed
        {
            add
            {
                this.AddHandler(DropDownClosedEvent, value);
            }
            remove
            {
                this.RemoveHandler(DropDownClosedEvent, value);
            }
        }

        public event RoutedEventHandler DropDownOpened
        {
            add
            {
                this.AddHandler(DropDownOpenedEvent, value);
            }
            remove
            {
                this.RemoveHandler(DropDownOpenedEvent, value);
            }
        }

        public event RoutedEventHandler DropDownOpening
        {
            add
            {
                this.AddHandler(DropDownOpeningEvent, value);
            }
            remove
            {
                this.RemoveHandler(DropDownOpeningEvent, value);
            }
        }

        public event RoutedEventHandler Unchecked
        {
            add
            {
                this.AddHandler(UncheckedEvent, value);
            }
            remove
            {
                this.RemoveHandler(UncheckedEvent, value);
            }
        }

        static RadSplitButton()
        {
            IsCheckedProperty = DependencyProperty.Register("IsChecked", typeof(bool), typeof(RadSplitButton), new Telerik.Windows.PropertyMetadata(false, delegate (DependencyObject d, DependencyPropertyChangedEventArgs a) {
                (d as RadSplitButton).OnIsCheckedChanged();
            }));
            IsToggleProperty = DependencyProperty.Register("IsToggle", typeof(bool), typeof(RadSplitButton), new Telerik.Windows.PropertyMetadata(false, delegate (DependencyObject d, DependencyPropertyChangedEventArgs a) {
                (d as RadSplitButton).OnIsToggleChanged();
            }));
            DropDownContentProperty = DependencyProperty.Register("DropDownContent", typeof(object), typeof(RadSplitButton), new Telerik.Windows.PropertyMetadata(null, new PropertyChangedCallback(RadSplitButton.OnDropDownContentChanged), null));
            DropDownWidthProperty = DependencyProperty.Register("DropDownWidth", typeof(double), typeof(RadSplitButton), new Telerik.Windows.PropertyMetadata((double) 1.0 / (double) 0.0));
            DropDownHeightProperty = DependencyProperty.Register("DropDownHeight", typeof(double), typeof(RadSplitButton), new Telerik.Windows.PropertyMetadata((double) 1.0 / (double) 0.0));
            DropDownMaxWidthProperty = DependencyProperty.Register("DropDownMaxWidth", typeof(double), typeof(RadSplitButton), new Telerik.Windows.PropertyMetadata((double) 1.0 / (double) 0.0));
            DropDownMaxHeightProperty = DependencyProperty.Register("DropDownMaxHeight", typeof(double), typeof(RadSplitButton), new Telerik.Windows.PropertyMetadata((double) 1.0 / (double) 0.0));
            IsOpenProperty = DependencyProperty.Register("IsOpen", typeof(bool), typeof(RadSplitButton), new Telerik.Windows.PropertyMetadata(false, new PropertyChangedCallback(RadSplitButton.OnIsOpenChanged)));
            IsMouseOverProperty = DependencyProperty.Register("IsMouseOver", typeof(bool), typeof(RadSplitButton), null);
            DropDownButtonPositionProperty = DependencyProperty.Register("DropDownButtonPosition", typeof(Telerik.Windows.Controls.DropDownButtonPosition), typeof(RadSplitButton), new Telerik.Windows.PropertyMetadata(Telerik.Windows.Controls.DropDownButtonPosition.Right, delegate (DependencyObject d, DependencyPropertyChangedEventArgs a) {
                (d as RadSplitButton).OnDropDownButtonPositionChanged((Telerik.Windows.Controls.DropDownButtonPosition) a.NewValue);
            }, null));
            DropDownPlacementProperty = DependencyProperty.Register("DropDownPlacement", typeof(Telerik.Windows.Controls.PlacementMode), typeof(RadSplitButton), new Telerik.Windows.PropertyMetadata(Telerik.Windows.Controls.PlacementMode.Bottom, delegate (DependencyObject d, DependencyPropertyChangedEventArgs a) {
                (d as RadSplitButton).OnDropDownPlacementChanged((Telerik.Windows.Controls.PlacementMode) a.NewValue);
            }, null));
            DropDownContentTemplateProperty = DependencyProperty.Register("DropDownContentTemplate", typeof(DataTemplate), typeof(RadSplitButton), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadSplitButton.OnDropDownContentTemplateChanged)));
            DropDownContentTemplateSelectorProperty = DependencyProperty.Register("DropDownContentTemplateSelector", typeof(DataTemplateSelector), typeof(RadSplitButton), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadSplitButton.OnDropDownContentTemplateSelectorChanged)));
            ClickEvent = EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(RadSplitButton));
            CheckedEvent = EventManager.RegisterRoutedEvent("Checked", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(RadSplitButton));
            UncheckedEvent = EventManager.RegisterRoutedEvent("Unchecked", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(RadSplitButton));
            ActivateEvent = EventManager.RegisterRoutedEvent("Activate", RoutingStrategy.Bubble, typeof(EventHandler<RadRoutedEventArgs>), typeof(RadSplitButton));
            DropDownClosedEvent = EventManager.RegisterRoutedEvent("DropDownClosed", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(RadSplitButton));
            DropDownOpenedEvent = EventManager.RegisterRoutedEvent("DropDownOpened", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(RadSplitButton));
            DropDownOpeningEvent = EventManager.RegisterRoutedEvent("DropDownOpening", RoutingStrategy.Bubble, typeof(RadRoutedEventHandler), typeof(RadSplitButton));
        }

        public RadSplitButton()
        {
            base.DefaultStyleKey = typeof(RadSplitButton);
            base.IsEnabledChanged += new DependencyPropertyChangedEventHandler(this.OnIsEnabledChanged);
            base.Loaded += new RoutedEventHandler(this.OnLoaded);
            
        }

        private void ActionButton_Click(object sender, RoutedEventArgs e)
        {
            this.OnClick();
        }

        internal virtual void ApplyDropDownButtonPositionState()
        {
            if (this.DropDownButtonPosition == Telerik.Windows.Controls.DropDownButtonPosition.Left)
            {
                this.GoToState("DropDownButtonAtLeft", false);
            }
            else if (this.DropDownButtonPosition == Telerik.Windows.Controls.DropDownButtonPosition.Right)
            {
                this.GoToState("DropDownButtonAtRight", false);
            }
            else if (this.DropDownButtonPosition == Telerik.Windows.Controls.DropDownButtonPosition.Top)
            {
                this.GoToState("DropDownButtonAtTop", false);
            }
            else
            {
                this.GoToState("DropDownButtonAtBottom", false);
            }
        }

        internal virtual void ApplyDropDownPlacementState()
        {
            if (this.DropDownPlacement == Telerik.Windows.Controls.PlacementMode.Left)
            {
                this.GoToState("PlacementLeft", false);
                this.ApplyPopupPlacement();
            }
            else if (this.DropDownPlacement == Telerik.Windows.Controls.PlacementMode.Right)
            {
                this.GoToState("PlacementRight", false);
                this.ApplyPopupPlacement();
            }
            else if (this.DropDownPlacement == Telerik.Windows.Controls.PlacementMode.Top)
            {
                this.GoToState("PlacementTop", false);
                this.ApplyPopupPlacement();
            }
            else
            {
                this.GoToState("PlacementBottom", false);
                this.ApplyPopupPlacement();
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        internal void ApplyPopupPlacement()
        {
            if (this.DropDownPopupWrapper != null)
            {
                this.DropDownPopupWrapper.Placement = this.DropDownPlacement;
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private bool AutoOpenTimerApplyState(bool applyAction)
        {
            double milliseconds = this.AutoOpenDelay.TotalMilliseconds;
            if (milliseconds > 0.0)
            {
                if (this.autoOpenTimer == null)
                {
                    this.autoOpenTimer = new DispatcherTimer();
                    this.autoOpenTimer.Tick += new EventHandler(this.OnAutoOpenTimerTick);
                }
                if (milliseconds != this.autoOpenTimer.Interval.TotalMilliseconds)
                {
                    this.autoOpenTimer.Interval = this.AutoOpenDelay;
                }
                if (applyAction)
                {
                    if (this.IsMouseOver)
                    {
                        this.AutoOpenTimerStart();
                    }
                    else
                    {
                        this.AutoOpenTimerStop();
                    }
                }
            }
            else
            {
                this.AutoOpenTimerDestroy();
            }
            return (this.autoOpenTimer != null);
        }

        private void AutoOpenTimerDestroy()
        {
            if (this.autoOpenTimer != null)
            {
                this.autoOpenTimer.Stop();
                this.autoOpenTimer.Tick -= new EventHandler(this.OnAutoOpenTimerTick);
                this.autoOpenTimer = null;
            }
        }

        private void AutoOpenTimerRestart()
        {
            if (this.AutoOpenTimerApplyState(false))
            {
                this.autoOpenTimer.Start();
            }
        }

        private void AutoOpenTimerStart()
        {
            if (this.AutoOpenTimerApplyState(false))
            {
                this.autoOpenTimer.Start();
            }
        }

        private void AutoOpenTimerStop()
        {
            if (this.AutoOpenTimerApplyState(false))
            {
                this.autoOpenTimer.Stop();
            }
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
            else if (this.IsMouseOver)
            {
                this.GoToState("MouseOver", useTransitions);
            }
            else
            {
                this.GoToState("Normal", useTransitions);
            }
            if (this.IsOpen)
            {
                this.GoToState("Opened", useTransitions);
            }
            else
            {
                this.GoToState("Closed", useTransitions);
            }
            if (this.IsToggle && this.IsChecked)
            {
                this.GoToState("Checked", useTransitions);
            }
            else
            {
                this.GoToState("UnChecked", useTransitions);
            }
        }

        private void DropDownButton_Click(object sender, RoutedEventArgs e)
        {
            this.IsOpen = !this.IsOpen;
        }

        private void DropDownPopup_KeyDown(object sender, KeyEventArgs e)
        {
            if ((!e.Handled && this.IsOpen) && (this.CloseOnEscape && (e.Key == Key.Escape)))
            {
                e.Handled = true;
                this.IsOpen = false;
                base.Focus();
            }
        }

        private void DropDownPopupClosed(object sender, EventArgs e)
        {
            this.RaisePopupClosed();
            if (this.IsOpen)
            {
                this.IsOpen = false;
            }
        }

        private void DropDownPopupOpened(object sender, EventArgs e)
        {
            this.RaisePopupOpened();
            if (!this.IsOpen)
            {
                this.IsOpen = true;
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

        private void InitializeDropdownPopup()
        {
            if (this.DropDownPopup != null)
            {
                this.DropDownPopup.Opened -= new EventHandler(this.DropDownPopupOpened);
                this.DropDownPopup.Closed -= new EventHandler(this.DropDownPopupClosed);
                if (this.DropDownPopup.Child != null)
                {
                    this.DropDownPopup.Child.KeyDown -= new KeyEventHandler(this.DropDownPopup_KeyDown);
                }
                this.DropDownPopupWrapper.CatchClickOutsidePopup = false;
                this.DropDownPopupWrapper.PopupOwner = null;
                this.DropDownPopupWrapper = null;
            }
            DependencyObject dependencyObject = base.GetTemplateChild("DropDownPopup");
            this.DropDownPopup = dependencyObject as System.Windows.Controls.Primitives.Popup;
            if (this.DropDownPopup == null)
            {
                Telerik.Windows.Controls.Primitives.Popup radPopUp = dependencyObject as Telerik.Windows.Controls.Primitives.Popup;
                if (radPopUp != null)
                {
                    this.DropDownPopup = radPopUp.RealPopup;
                }
            }
            if (this.DropDownPopup != null)
            {
                this.DropDownPopup.Opened += new EventHandler(this.DropDownPopupOpened);
                this.DropDownPopup.Closed += new EventHandler(this.DropDownPopupClosed);
                if (this.DropDownPopup.Child != null)
                {
                    this.DropDownPopup.Child.KeyDown += new KeyEventHandler(this.DropDownPopup_KeyDown);
                }
                this.DropDownPopupWrapper = new PopupWrapper(this.DropDownPopup, this);
                this.DropDownPopupWrapper.CloseOnOutsideClick = true;
                this.DropDownPopupWrapper.CatchClickOutsidePopup = true;
                this.DropDownPopupWrapper.Placement = this.DropDownPlacement;
                this.DropDownPopupWrapper.PlacementTarget = this.PopupPlacementTarget;
            }
        }

        private void InitializeElements()
        {
            if (this.DropDownButton != null)
            {
                this.DropDownButton.Click -= new RoutedEventHandler(this.DropDownButton_Click);
            }
            if (this.ActionButton != null)
            {
                this.ActionButton.Click -= new RoutedEventHandler(this.ActionButton_Click);
            }
            this.ActionButton = base.GetTemplateChild("ButtonPart") as RadButton;
            this.DropDownButton = base.GetTemplateChild("DropDownPart") as RadToggleButton;
            if (this.ActionButton != null)
            {
                this.ActionButton.Click += new RoutedEventHandler(this.ActionButton_Click);
            }
            if (this.DropDownButton != null)
            {
                this.DropDownButton.IsChecked = new bool?(this.IsOpen);
                this.DropDownButton.Click += new RoutedEventHandler(this.DropDownButton_Click);
            }
        }

        protected internal virtual void OnActivate()
        {
            this.RaiseEvent(new RadRoutedEventArgs(ActivateEvent, this));
        }

        public override void OnApplyTemplate()
        {
            if (this.IsOpen)
            {
                this.IsOpen = false;
            }
            base.OnApplyTemplate();
            this.isTemplateApplied = true;
            this.SetDropDownContentTemplate();
            this.InitializeElements();
            this.InitializeDropdownPopup();
            this.UpdateVisualState(false);
        }

        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId="e")]
        private static void OnAutoOpenDelayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadSplitButton button = d as RadSplitButton;
            if (button != null)
            {
                button.AutoOpenTimerApplyState(true);
            }
        }

        private void OnAutoOpenTimerTick(object sender, EventArgs e)
        {
            if (!this.IsOpen)
            {
                base.Focus();
                this.IsOpen = true;
            }
            this.AutoOpenTimerStop();
        }

        protected virtual void OnChecked()
        {
            this.RaiseChecked();
        }

        protected virtual void OnClick()
        {
            if (this.IsOpen)
            {
                this.IsOpen = false;
            }
            this.OnToggle();
            this.RaiseClick();
            this.OnActivate();
            this.ExecuteCommand();
        }

        internal void OnClickInternal()
        {
            this.OnClick();
        }

        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadSplitButton button = d as RadSplitButton;
            if (button != null)
            {
                button.ChangeCommand((ICommand) e.OldValue, (ICommand) e.NewValue);
            }
        }

        private static void OnCommandParameterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadSplitButton button = d as RadSplitButton;
            if (button != null)
            {
                button.CanExecuteApply();
            }
        }

        private static void OnCommandTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadSplitButton button = d as RadSplitButton;
            if (button != null)
            {
                button.CanExecuteApply();
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic"), SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId="newValue")]
        private void OnDropDownButtonPositionChanged(Telerik.Windows.Controls.DropDownButtonPosition newValue)
        {
            this.UpdateVisualState(false);
        }

        protected virtual void OnDropDownContentChanged(object oldValue, object newValue)
        {
        }

        private static void OnDropDownContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadSplitButton button = d as RadSplitButton;
            if (button != null)
            {
                button.UpdateVisualState(false);
                button.OnDropDownContentChanged(e.OldValue, e.NewValue);
            }
        }

        protected virtual void OnDropDownContentTemplateChanged(DataTemplate oldValue, DataTemplate newValue)
        {
            this.SetDropDownContentTemplate();
        }

        private static void OnDropDownContentTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RadSplitButton) d).OnDropDownContentTemplateChanged(e.OldValue as DataTemplate, e.NewValue as DataTemplate);
        }

        private static void OnDropDownContentTemplateSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RadSplitButton) d).OnDropDownContentTemplateSelectorChanged(e.OldValue as DataTemplateSelector, e.NewValue as DataTemplateSelector);
        }

        protected virtual void OnDropDownContentTemplateSelectorChanged(DataTemplateSelector oldValue, DataTemplateSelector newValue)
        {
            this.SetDropDownContentTemplate();
        }

        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId="newValue")]
        private void OnDropDownPlacementChanged(Telerik.Windows.Controls.PlacementMode newValue)
        {
            this.UpdateVisualState(false);
        }

        private void OnIsCheckedChanged()
        {
            this.UpdateVisualState(false);
            if (this.IsToggle)
            {
                if (this.IsChecked)
                {
                    this.OnChecked();
                }
                else
                {
                    this.OnUnchecked();
                }
            }
        }

        private void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            RadSplitButton button = sender as RadSplitButton;
            if (button != null)
            {
                button.UpdateVisualState(true);
            }
        }

        private static void OnIsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadSplitButton button = d as RadSplitButton;
            if (button != null)
            {
                bool newValue = (bool) e.NewValue;
                if (newValue)
                {
                    if (button.RaisePopupOpening().Handled)
                    {
                        button.IsOpen = false;
                        return;
                    }
                    if (((button.DropDownPopupWrapper != null) && (button.DropDownPopup != null)) && !button.DropDownPopup.IsOpen)
                    {
                        button.DropDownPopupWrapper.ShowPopup();
                    }
                }
                else if (((button.DropDownPopupWrapper != null) && (button.DropDownPopup != null)) && button.DropDownPopup.IsOpen)
                {
                    button.DropDownPopupWrapper.HidePopup();
                }
                button.UpdateVisualState(true);
                if (button.DropDownButton != null)
                {
                    bool isChecked = button.DropDownButton.IsChecked == true;
                    if (newValue != isChecked)
                    {
                        button.DropDownButton.IsChecked = new bool?(newValue);
                    }
                }
            }
        }

        private void OnIsToggleChanged()
        {
            this.UpdateVisualState(false);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (!e.Handled)
            {
                if (e.Key == Key.Escape)
                {
                    if (this.IsOpen && this.CloseOnEscape)
                    {
                        this.IsOpen = false;
                        e.Handled = true;
                        base.Focus();
                    }
                }
                else if ((e.Key == Key.Down) && !this.IsOpen)
                {
                    this.IsOpen = true;
                    e.Handled = true;
                }
            }
        }

        private void OnLoaded(object sender, EventArgs e)
        {
            this.CanExecuteApply();
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            this.IsMouseOver = true;
            this.UpdateVisualState(true);
            this.AutoOpenTimerStart();
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            this.AutoOpenTimerStop();
            this.IsMouseOver = false;
            this.UpdateVisualState(true);
            base.OnMouseLeave(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            this.AutoOpenTimerRestart();
            base.OnMouseMove(e);
        }

        protected virtual void OnToggle()
        {
            if (this.IsToggle)
            {
                this.IsChecked = !this.IsChecked;
            }
        }

        protected virtual void OnUnchecked()
        {
            this.RaiseUnchecked();
        }

        private RadRoutedEventArgs RaiseChecked()
        {
            return RadButton.RaiseRadRoutedEvent(CheckedEvent, this);
        }

        private RadRoutedEventArgs RaiseClick()
        {
            return RadButton.RaiseRadRoutedEvent(ClickEvent, this);
        }

        private void RaisePopupClosed()
        {
            this.RaiseEvent(new RadRoutedEventArgs(DropDownClosedEvent, this));
        }

        private void RaisePopupOpened()
        {
            this.RaiseEvent(new RadRoutedEventArgs(DropDownOpenedEvent, this));
        }

        private RadRoutedEventArgs RaisePopupOpening()
        {
            RadRoutedEventArgs args = new RadRoutedEventArgs(DropDownOpeningEvent, this);
            this.RaiseEvent(args);
            return args;
        }

        private RadRoutedEventArgs RaiseUnchecked()
        {
            return RadButton.RaiseRadRoutedEvent(UncheckedEvent, this);
        }

        private void SetDropDownContentTemplate()
        {
            if (this.isTemplateApplied)
            {
                DataTemplate template = null;
                if (this.DropDownContentTemplateSelector != null)
                {
                    template = this.DropDownContentTemplateSelector.SelectTemplate(this.DropDownContent, this);
                }
                if (template == null)
                {
                    template = this.DropDownContentTemplate;
                }
                if (template != null)
                {
                    ContentPresenter dropDownContentElement = base.GetTemplateChild("DropDownPopupContent") as ContentPresenter;
                    if ((dropDownContentElement != null) && !template.Equals(dropDownContentElement.ContentTemplate))
                    {
                        dropDownContentElement.ContentTemplate = template;
                    }
                }
            }
        }

        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId="placement"), SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        internal void SetPopupPlacement(Telerik.Windows.Controls.PlacementMode placement)
        {
            this.DropDownPlacement = placement;
            this.ApplyPopupPlacement();
        }

        internal void SetPopupPlacementTarget(FrameworkElement placementTarget)
        {
            this.PopupPlacementTarget = placementTarget;
            if (this.DropDownPopupWrapper != null)
            {
                this.DropDownPopupWrapper.PlacementTarget = this.PopupPlacementTarget;
            }
        }

        internal virtual void UpdateVisualState(bool useTransitions)
        {
            this.ChangeVisualState(useTransitions);
            this.ApplyDropDownButtonPositionState();
            this.ApplyDropDownPlacementState();
        }

        internal RadButton ActionButton { get; set; }

        [Browsable(false)]
        public TimeSpan AutoOpenDelay
        {
            get
            {
                return (TimeSpan) base.GetValue(AutoOpenDelayProperty);
            }
            set
            {
                base.SetValue(AutoOpenDelayProperty, value);
            }
        }

        public bool CloseOnEscape
        {
            get
            {
                return (bool) base.GetValue(CloseOnEscapeProperty);
            }
            set
            {
                base.SetValue(CloseOnEscapeProperty, value);
            }
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

        internal RadToggleButton DropDownButton { get; set; }

        public Telerik.Windows.Controls.DropDownButtonPosition DropDownButtonPosition
        {
            get
            {
                return (Telerik.Windows.Controls.DropDownButtonPosition) base.GetValue(DropDownButtonPositionProperty);
            }
            set
            {
                base.SetValue(DropDownButtonPositionProperty, value);
            }
        }

        public object DropDownContent
        {
            get
            {
                return base.GetValue(DropDownContentProperty);
            }
            set
            {
                base.SetValue(DropDownContentProperty, value);
            }
        }

        public DataTemplate DropDownContentTemplate
        {
            get
            {
                return (DataTemplate) base.GetValue(DropDownContentTemplateProperty);
            }
            set
            {
                base.SetValue(DropDownContentTemplateProperty, value);
            }
        }

        public DataTemplateSelector DropDownContentTemplateSelector
        {
            get
            {
                return (DataTemplateSelector) base.GetValue(DropDownContentTemplateSelectorProperty);
            }
            set
            {
                base.SetValue(DropDownContentTemplateSelectorProperty, value);
            }
        }

        public double DropDownHeight
        {
            get
            {
                return (double) base.GetValue(DropDownHeightProperty);
            }
            set
            {
                base.SetValue(DropDownHeightProperty, value);
            }
        }

        public Visibility DropDownIndicatorVisibility
        {
            get
            {
                return (Visibility) base.GetValue(DropDownIndicatorVisibilityProperty);
            }
            set
            {
                base.SetValue(DropDownIndicatorVisibilityProperty, value);
            }
        }

        public double DropDownMaxHeight
        {
            get
            {
                return (double) base.GetValue(DropDownMaxHeightProperty);
            }
            set
            {
                base.SetValue(DropDownMaxHeightProperty, value);
            }
        }

        public double DropDownMaxWidth
        {
            get
            {
                return (double) base.GetValue(DropDownMaxWidthProperty);
            }
            set
            {
                base.SetValue(DropDownMaxWidthProperty, value);
            }
        }

        public Telerik.Windows.Controls.PlacementMode DropDownPlacement
        {
            get
            {
                return (Telerik.Windows.Controls.PlacementMode) base.GetValue(DropDownPlacementProperty);
            }
            set
            {
                base.SetValue(DropDownPlacementProperty, value);
            }
        }

        internal System.Windows.Controls.Primitives.Popup DropDownPopup { get; set; }

        internal PopupWrapper DropDownPopupWrapper { get; set; }

        public double DropDownWidth
        {
            get
            {
                return (double) base.GetValue(DropDownWidthProperty);
            }
            set
            {
                base.SetValue(DropDownWidthProperty, value);
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

        public bool IsChecked
        {
            get
            {
                return (this.IsToggle && ((bool) base.GetValue(IsCheckedProperty)));
            }
            set
            {
                base.SetValue(IsCheckedProperty, value);
            }
        }

        public bool IsMouseOver
        {
            get
            {
                return (bool) base.GetValue(IsMouseOverProperty);
            }
            internal set
            {
                base.SetValue(IsMouseOverProperty, value);
            }
        }

        public bool IsOpen
        {
            get
            {
                return (bool) base.GetValue(IsOpenProperty);
            }
            set
            {
                base.SetValue(IsOpenProperty, value);
            }
        }

        public bool IsToggle
        {
            get
            {
                return (bool) base.GetValue(IsToggleProperty);
            }
            set
            {
                base.SetValue(IsToggleProperty, value);
            }
        }

        internal FrameworkElement PopupPlacementTarget { get; set; }
    }
}

