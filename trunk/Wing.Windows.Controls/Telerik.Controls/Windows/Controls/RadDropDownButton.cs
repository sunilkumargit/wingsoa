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
    using Telerik.Windows;
    using Telerik.Windows.Controls.Primitives;

    public class RadDropDownButton : RadButton
    {
        public static readonly DependencyProperty AutoOpenDelayProperty = DependencyProperty.Register("AutoOpenDelay", typeof(TimeSpan), typeof(RadDropDownButton), new Telerik.Windows.PropertyMetadata(TimeSpan.Zero, new PropertyChangedCallback(RadDropDownButton.OnAutoOpenDelayChanged)));
        public static readonly DependencyProperty CloseOnEnterProperty = DependencyProperty.Register("CloseOnEnter", typeof(bool), typeof(RadDropDownButton), new Telerik.Windows.PropertyMetadata(false));
        public static readonly DependencyProperty CloseOnEscapeProperty = DependencyProperty.Register("CloseOnEscape", typeof(bool), typeof(RadDropDownButton), new Telerik.Windows.PropertyMetadata(true));
        public static readonly DependencyProperty DropDownButtonPositionProperty;
        public static readonly Telerik.Windows.RoutedEvent DropDownClosedEvent;
        public static readonly DependencyProperty DropDownContentProperty = DependencyProperty.Register("DropDownContent", typeof(object), typeof(RadDropDownButton), new Telerik.Windows.PropertyMetadata(null, new PropertyChangedCallback(RadDropDownButton.OnDropDownContentChanged), null));
        public static readonly DependencyProperty DropDownContentTemplateProperty;
        public static readonly DependencyProperty DropDownContentTemplateSelectorProperty;
        public static readonly DependencyProperty DropDownHeightProperty = DependencyProperty.Register("DropDownHeight", typeof(double), typeof(RadDropDownButton), new Telerik.Windows.PropertyMetadata((double) 1.0 / (double) 0.0));
        public static readonly DependencyProperty DropDownIndicatorVisibilityProperty = DependencyProperty.Register("DropDownIndicatorVisibility", typeof(Visibility), typeof(RadDropDownButton), new Telerik.Windows.PropertyMetadata(Visibility.Visible));
        public static readonly DependencyProperty DropDownMaxHeightProperty = DependencyProperty.Register("DropDownMaxHeight", typeof(double), typeof(RadDropDownButton), new Telerik.Windows.PropertyMetadata((double) 1.0 / (double) 0.0));
        public static readonly DependencyProperty DropDownMaxWidthProperty = DependencyProperty.Register("DropDownMaxWidth", typeof(double), typeof(RadDropDownButton), new Telerik.Windows.PropertyMetadata((double) 1.0 / (double) 0.0));
        public static readonly Telerik.Windows.RoutedEvent DropDownOpenedEvent;
        public static readonly Telerik.Windows.RoutedEvent DropDownOpeningEvent;
        public static readonly DependencyProperty DropDownPlacementProperty;
        public static readonly DependencyProperty DropDownWidthProperty = DependencyProperty.Register("DropDownWidth", typeof(double), typeof(RadDropDownButton), new Telerik.Windows.PropertyMetadata((double) 1.0 / (double) 0.0));
        private bool isLoaded;
        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register("IsOpen", typeof(bool), typeof(RadDropDownButton), new Telerik.Windows.PropertyMetadata(false, new PropertyChangedCallback(RadDropDownButton.OnIsOpenChanged), null));
        private bool isTemplateApplied;
        public static readonly DependencyProperty PopupPlacementTargetProperty;

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

        static RadDropDownButton()
        {
            DropDownButtonPositionProperty = DependencyProperty.Register("DropDownButtonPosition", typeof(Telerik.Windows.Controls.DropDownButtonPosition), typeof(RadDropDownButton), new Telerik.Windows.PropertyMetadata(Telerik.Windows.Controls.DropDownButtonPosition.Right, delegate (DependencyObject d, DependencyPropertyChangedEventArgs a) {
                (d as RadDropDownButton).OnDropDownButtonPositionChanged((Telerik.Windows.Controls.DropDownButtonPosition) a.NewValue);
            }, null));
            DropDownPlacementProperty = DependencyProperty.Register("DropDownPlacement", typeof(Telerik.Windows.Controls.PlacementMode), typeof(RadDropDownButton), new Telerik.Windows.PropertyMetadata(Telerik.Windows.Controls.PlacementMode.Bottom, new PropertyChangedCallback(RadDropDownButton.OnDropDownPlacementChanged), null));
            PopupPlacementTargetProperty = DependencyProperty.Register("PopupPlacementTarget", typeof(FrameworkElement), typeof(RadDropDownButton), new Telerik.Windows.PropertyMetadata(null, new PropertyChangedCallback(RadDropDownButton.OnPopupPlacementTargetChanged), null));
            DropDownContentTemplateProperty = DependencyProperty.Register("DropDownContentTemplate", typeof(DataTemplate), typeof(RadDropDownButton), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadDropDownButton.OnDropDownContentTemplateChanged)));
            DropDownContentTemplateSelectorProperty = DependencyProperty.Register("DropDownContentTemplateSelector", typeof(DataTemplateSelector), typeof(RadDropDownButton), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadDropDownButton.OnDropDownContentTemplateSelectorChanged)));
            DropDownClosedEvent = EventManager.RegisterRoutedEvent("DropDownClosed", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(RadDropDownButton));
            DropDownOpenedEvent = EventManager.RegisterRoutedEvent("DropDownOpened", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(RadDropDownButton));
            DropDownOpeningEvent = EventManager.RegisterRoutedEvent("DropDownOpening", RoutingStrategy.Bubble, typeof(RadRoutedEventHandler), typeof(RadDropDownButton));
        }

        public RadDropDownButton()
        {
            base.DefaultStyleKey = typeof(RadDropDownButton);
            
            base.Loaded += new RoutedEventHandler(this.OnLoaded);
            base.Unloaded += new RoutedEventHandler(this.OnUnloaded);
        }

        protected virtual void ApplyDropDownButtonPosition()
        {
            if (this.DropDownButtonPosition == Telerik.Windows.Controls.DropDownButtonPosition.Left)
            {
                base.GoToState("DropDownButtonAtLeft", false);
            }
            else if (this.DropDownButtonPosition == Telerik.Windows.Controls.DropDownButtonPosition.Right)
            {
                base.GoToState("DropDownButtonAtRight", false);
            }
            else if (this.DropDownButtonPosition == Telerik.Windows.Controls.DropDownButtonPosition.Top)
            {
                base.GoToState("DropDownButtonAtTop", false);
            }
            else
            {
                base.GoToState("DropDownButtonAtBottom", false);
            }
        }

        protected virtual void ApplyPopupPlacement()
        {
            if (this.DropDownPlacement == Telerik.Windows.Controls.PlacementMode.Left)
            {
                base.GoToState("PlacementLeft", false);
                this.SetPopupPlacement(Telerik.Windows.Controls.PlacementMode.Left);
            }
            else if (this.DropDownPlacement == Telerik.Windows.Controls.PlacementMode.Right)
            {
                base.GoToState("PlacementRight", false);
                this.SetPopupPlacement(Telerik.Windows.Controls.PlacementMode.Right);
            }
            else if (this.DropDownPlacement == Telerik.Windows.Controls.PlacementMode.Top)
            {
                base.GoToState("PlacementTop", false);
                this.SetPopupPlacement(Telerik.Windows.Controls.PlacementMode.Top);
            }
            else
            {
                base.GoToState("PlacementBottom", false);
                this.SetPopupPlacement(Telerik.Windows.Controls.PlacementMode.Bottom);
            }
        }

        internal override void ChangeVisualState(bool useTransitions)
        {
            base.ChangeVisualState(useTransitions);
            if (this.IsOpen)
            {
                base.GoToState("Opened", useTransitions);
            }
            else
            {
                base.GoToState("Closed", useTransitions);
            }
        }

        private void DropDownPopup_KeyDown(object sender, KeyEventArgs e)
        {
            if (!e.Handled)
            {
                if ((this.IsOpen && this.CloseOnEscape) && (e.Key == Key.Escape))
                {
                    e.Handled = true;
                    this.IsOpen = false;
                    base.Focus();
                }
                if ((this.IsOpen && this.CloseOnEnter) && (e.Key == Key.Enter))
                {
                    e.Handled = true;
                    this.IsOpen = false;
                    base.Focus();
                }
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
        }

        [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
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

        protected internal override void OnActivate()
        {
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
            this.InitializeDropdownPopup();
            this.ChangeVisualState(false);
            this.ApplyDropDownButtonPosition();
            this.ApplyPopupPlacement();
        }

        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId="e")]
        private static void OnAutoOpenDelayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadDropDownButton button = d as RadDropDownButton;
            if (button != null)
            {
                button.HoverDelay = button.AutoOpenDelay;
            }
        }

        protected override void OnClick()
        {
            if (this.IsOpen || (this.DropDownContent != null))
            {
                this.IsOpen = !this.IsOpen;
            }
            base.OnClick();
        }

        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId="newValue"), SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        private void OnDropDownButtonPositionChanged(Telerik.Windows.Controls.DropDownButtonPosition newValue)
        {
            this.ApplyDropDownButtonPosition();
        }

        protected virtual void OnDropDownContentChanged(object oldValue, object newValue)
        {
        }

        private static void OnDropDownContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadDropDownButton button = d as RadDropDownButton;
            if (button != null)
            {
                button.ChangeVisualState(true);
                button.OnDropDownContentChanged(e.OldValue, e.NewValue);
            }
        }

        protected virtual void OnDropDownContentTemplateChanged(DataTemplate oldValue, DataTemplate newValue)
        {
            this.SetDropDownContentTemplate();
        }

        private static void OnDropDownContentTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RadDropDownButton) d).OnDropDownContentTemplateChanged(e.OldValue as DataTemplate, e.NewValue as DataTemplate);
        }

        private static void OnDropDownContentTemplateSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RadDropDownButton) d).OnDropDownContentTemplateSelectorChanged(e.OldValue as DataTemplateSelector, e.NewValue as DataTemplateSelector);
        }

        protected virtual void OnDropDownContentTemplateSelectorChanged(DataTemplateSelector oldValue, DataTemplateSelector newValue)
        {
            this.SetDropDownContentTemplate();
        }

        protected virtual void OnDropDownPlacementChanged(DataTemplate oldValue, DataTemplate newValue)
        {
            this.ApplyPopupPlacement();
        }

        private static void OnDropDownPlacementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RadDropDownButton) d).OnDropDownPlacementChanged(e.OldValue as DataTemplate, e.NewValue as DataTemplate);
        }

        protected internal override void OnHover()
        {
            if (!this.IsOpen)
            {
                base.Focus();
                this.IsOpen = true;
            }
        }

        private static void OnIsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadDropDownButton button = d as RadDropDownButton;
            if (button != null)
            {
                bool newValue = (bool) e.NewValue;
                if (newValue && button.RaisePopupOpening().Handled)
                {
                    button.IsOpen = false;
                }
                else
                {
                    if (newValue)
                    {
                        button.OpenPopup();
                    }
                    else if (((button.DropDownPopupWrapper != null) && (button.DropDownPopup != null)) && button.DropDownPopup.IsOpen)
                    {
                        button.DropDownPopupWrapper.HidePopup();
                    }
                    button.ChangeVisualState(true);
                }
            }
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
                else if (e.Key == Key.Enter)
                {
                    if (this.IsOpen && this.CloseOnEnter)
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
            this.isLoaded = true;
            if (this.IsOpen)
            {
                this.OpenPopup();
            }
        }

        private static void OnPopupPlacementTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RadDropDownButton) d).SetPopupPlacementTarget(e.NewValue as FrameworkElement);
        }

        private void OnUnloaded(object sender, EventArgs e)
        {
            this.isLoaded = false;
        }

        private void OpenPopup()
        {
            if ((this.isLoaded && (this.DropDownPopupWrapper != null)) && ((this.DropDownPopup != null) && !this.DropDownPopup.IsOpen))
            {
                this.DropDownPopupWrapper.ShowPopup();
            }
        }

        private void RaisePopupClosed()
        {
            UIElement content = this.DropDownContent as UIElement;
            if (content != null)
            {
                content.KeyDown -= new KeyEventHandler(this.DropDownPopup_KeyDown);
            }
            this.RaiseEvent(new RadRoutedEventArgs(DropDownClosedEvent, this));
        }

        private void RaisePopupOpened()
        {
            if (this.DropDownPopup == null)
            {
                UIElement content = this.DropDownContent as UIElement;
                if (content != null)
                {
                    content.KeyDown += new KeyEventHandler(this.DropDownPopup_KeyDown);
                }
            }
            this.RaiseEvent(new RadRoutedEventArgs(DropDownOpenedEvent, this));
        }

        private RadRoutedEventArgs RaisePopupOpening()
        {
            RadRoutedEventArgs args = new RadRoutedEventArgs(DropDownOpeningEvent, this);
            this.RaiseEvent(args);
            return args;
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
            if ((this.DropDownPopupWrapper != null) && (this.DropDownPopupWrapper.Placement != placement))
            {
                this.DropDownPopupWrapper.Placement = placement;
            }
        }

        internal void SetPopupPlacementTarget(FrameworkElement placementTarget)
        {
            this.PopupPlacementTarget = placementTarget;
            if (this.DropDownPopupWrapper != null)
            {
                this.DropDownPopupWrapper.PlacementTarget = this.PopupPlacementTarget;
            }
        }

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

        public bool CloseOnEnter
        {
            get
            {
                return (bool) base.GetValue(CloseOnEnterProperty);
            }
            set
            {
                base.SetValue(CloseOnEnterProperty, value);
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

        public FrameworkElement PopupPlacementTarget
        {
            get
            {
                return (FrameworkElement) base.GetValue(PopupPlacementTargetProperty);
            }
            set
            {
                base.SetValue(PopupPlacementTargetProperty, value);
            }
        }
    }
}

