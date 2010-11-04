namespace Telerik.Windows.Controls
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Telerik.Windows;
    using Telerik.Windows.Controls.Design;

    public class RadRadioButton : RadioButton, ICommandSource
    {
        public static readonly Telerik.Windows.RoutedEvent ActivateEvent = EventManager.RegisterRoutedEvent("Activate", RoutingStrategy.Bubble, typeof(EventHandler<RadRoutedEventArgs>), typeof(RadRadioButton));
        public static readonly Telerik.Windows.RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(RadRadioButton));
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(RadRadioButton), new Telerik.Windows.PropertyMetadata(null, new PropertyChangedCallback(RadRadioButton.OnCommandChanged)));
        public static readonly DependencyProperty CommandTargetProperty = DependencyProperty.Register("CommandTarget", typeof(UIElement), typeof(RadRadioButton), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadRadioButton.OnCommandTargetChanged)));
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(System.Windows.CornerRadius), typeof(RadRadioButton), new Telerik.Windows.PropertyMetadata());
        private EventHandler guardCanExecuteChangedHandler;
        public static readonly DependencyProperty IsBackgroundVisibleProperty = DependencyProperty.Register("IsBackgroundVisible", typeof(bool), typeof(RadRadioButton), new Telerik.Windows.PropertyMetadata(true));

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

        public RadRadioButton()
        {
            base.DefaultStyleKey = typeof(RadRadioButton);
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

        protected internal virtual void OnActivate()
        {
            this.RaiseEvent(new RadRoutedEventArgs(ActivateEvent, this));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.ChangeVisualState(false);
        }

        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadRadioButton button = d as RadRadioButton;
            if (button != null)
            {
                button.ChangeCommand((ICommand) e.OldValue, (ICommand) e.NewValue);
            }
        }

        private static void OnCommandTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadRadioButton button = d as RadRadioButton;
            if (button != null)
            {
                button.CanExecuteApply();
            }
        }

        private void OnLoaded(object sender, EventArgs e)
        {
            this.CanExecuteApply();
        }

        protected override void OnToggle()
        {
            this.RaiseClick();
            base.OnToggle();
            this.OnActivate();
            this.ExecuteCommand();
        }

        private RadRoutedEventArgs RaiseClick()
        {
            return RadButton.RaiseRadRoutedEvent(ClickEvent, this);
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

