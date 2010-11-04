namespace Telerik.Windows.Controls
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using Telerik.Windows;
    using Telerik.Windows.Controls.Design;

    public class RadToggleButton : ToggleButton, ICommandSource
    {
        public static readonly Telerik.Windows.RoutedEvent ActivateEvent = EventManager.RegisterRoutedEvent("Activate", RoutingStrategy.Bubble, typeof(RadRoutedEventArgs), typeof(RadToggleButton));
        public static readonly Telerik.Windows.RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(RadToggleButton));
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(RadToggleButton), new Telerik.Windows.PropertyMetadata(null, new PropertyChangedCallback(RadToggleButton.OnCommandChanged)));
        public static readonly DependencyProperty CommandTargetProperty = DependencyProperty.Register("CommandTarget", typeof(UIElement), typeof(RadToggleButton), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadToggleButton.OnCommandTargetChanged)));
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(System.Windows.CornerRadius), typeof(RadToggleButton), new Telerik.Windows.PropertyMetadata());
        private EventHandler guardCanExecuteChangedHandler;
        public static readonly DependencyProperty IsBackgroundVisibleProperty = DependencyProperty.Register("IsBackgroundVisible", typeof(bool), typeof(RadToggleButton), new Telerik.Windows.PropertyMetadata(true));
        public static readonly Telerik.Windows.RoutedEvent PreviewClickEvent = EventManager.RegisterRoutedEvent("PreviewClick", RoutingStrategy.Bubble, typeof(RadRoutedEventHandler), typeof(RadToggleButton));

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

        public event RoutedEventHandler PreviewClick
        {
            add
            {
                this.AddHandler(PreviewClickEvent, value);
            }
            remove
            {
                this.RemoveHandler(PreviewClickEvent, value);
            }
        }

        public RadToggleButton()
        {
            base.DefaultStyleKey = typeof(RadToggleButton);
            base.Loaded += new RoutedEventHandler(this.OnLoaded);
            
        }

        private void CanExecuteApply()
        {
            if (this.Command != null)
            {
                RoutedCommand routedCommand = this.Command as RoutedCommand;
                base.IsEnabled = (routedCommand == null) ? this.Command.CanExecute(base.CommandParameter) : routedCommand.CanExecute(base.CommandParameter, this.CommandTarget ?? this);
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
            if (base.IsChecked == true)
            {
                this.GoToState("Checked", useTransitions);
            }
            else if (base.IsChecked == false)
            {
                this.GoToState("Unchecked", useTransitions);
            }
            else if (!this.GoToState("Indeterminate", useTransitions))
            {
                this.GoToState("Unchecked", useTransitions);
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
                    this.Command.Execute(base.CommandParameter);
                }
                else
                {
                    routedCommand.Execute(base.CommandParameter, this.CommandTarget ?? this);
                }
            }
        }

        internal bool GoToState(string stateName, bool useTransitions)
        {
            return VisualStateManager.GoToState(this, stateName, useTransitions);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.ChangeVisualState(false);
        }

        protected override void OnClick()
        {
            RadRoutedEventArgs args = new RadRoutedEventArgs(PreviewClickEvent, this);
            this.RaiseEvent(args);
            if (!args.Handled)
            {
                base.OnClick();
                this.RaiseEvent(new RadRoutedEventArgs(ClickEvent, this));
                this.RaiseEvent(new RadRoutedEventArgs(ActivateEvent, this));
                this.ChangeVisualState(false);
                this.ExecuteCommand();
            }
        }

        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadToggleButton button = d as RadToggleButton;
            if (button != null)
            {
                button.ChangeCommand((ICommand) e.OldValue, (ICommand) e.NewValue);
            }
        }

        private static void OnCommandTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadToggleButton button = d as RadToggleButton;
            if (button != null)
            {
                button.CanExecuteApply();
            }
        }

        private void OnLoaded(object sender, EventArgs e)
        {
            this.CanExecuteApply();
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

        object ICommandSource.CommandParameter
        {
            get
            {
                return base.CommandParameter;
            }
        }
    }
}

