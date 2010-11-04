namespace Telerik.Windows.Controls
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Automation.Peers;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using Telerik.Windows;
    using Telerik.Windows.Controls.Animation;

    [TemplateVisualState(Name="Disabled", GroupName="CommonStateGroup"), TemplateVisualState(Name="DirectionLeft", GroupName="GroupExpandDirection"), TemplateVisualState(Name="DirectionRight", GroupName="GroupExpandDirection"), DefaultEvent("Expanded"), DefaultProperty("IsExpanded"), TemplateVisualState(Name="Normal", GroupName="CommonStateGroup"), TemplateVisualState(Name="DirectionDown", GroupName="GroupExpandDirection"), TemplateVisualState(Name="DirectionUp", GroupName="GroupExpandDirection"), TemplateVisualState(Name="Collapsed", GroupName="ExpandStateGroup"), TemplateVisualState(Name="Expanded", GroupName="ExpandStateGroup")]
    public class RadExpander : HeaderedContentControl
    {
        public static readonly DependencyProperty ClickModeProperty;
        public static readonly Telerik.Windows.RoutedEvent CollapsedEvent;
        private FrameworkElement contentElement;
        public static readonly DependencyProperty DecoratorTemplateProperty = DependencyProperty.Register("DecoratorTemplate", typeof(ControlTemplate), typeof(RadExpander), null);
        public static readonly DependencyProperty ExpandDirectionProperty;
        public static readonly Telerik.Windows.RoutedEvent ExpandedEvent;
        private ToggleButton headerButton;
        public static readonly DependencyProperty HeaderControlTemplateProperty = DependencyProperty.Register("HeaderControlTemplate", typeof(ControlTemplate), typeof(RadExpander), null);
        private bool insidePreviewRevert;
        public static readonly DependencyProperty IsExpandedProperty;
        private bool lastIsExpanded;
        private bool lastIsExpanedValue;
        public static readonly Telerik.Windows.RoutedEvent PreviewCollapsedEvent;
        public static readonly Telerik.Windows.RoutedEvent PreviewExpandedEvent;
        [Obsolete("This property is obsolete.", true)]
        public static readonly DependencyProperty TemplateDownProperty = DependencyProperty.Register("TemplateDown", typeof(ControlTemplate), typeof(RadExpander), null);
        [Obsolete("This property is obsolete.", true)]
        public static readonly DependencyProperty TemplateLeftProperty = DependencyProperty.Register("TemplateLeft", typeof(ControlTemplate), typeof(RadExpander), null);
        [Obsolete("This property is obsolete.", true)]
        public static readonly DependencyProperty TemplateRightProperty = DependencyProperty.Register("TemplateRight", typeof(ControlTemplate), typeof(RadExpander), null);
        [Obsolete("This property is obsolete.", true)]
        public static readonly DependencyProperty TemplateUpProperty = DependencyProperty.Register("TemplateUp", typeof(ControlTemplate), typeof(RadExpander), null);

        public event RoutedEventHandler Collapsed
        {
            add
            {
                this.AddHandler(CollapsedEvent, value);
            }
            remove
            {
                this.RemoveHandler(CollapsedEvent, value);
            }
        }

        public event RoutedEventHandler Expanded
        {
            add
            {
                this.AddHandler(ExpandedEvent, value);
            }
            remove
            {
                this.RemoveHandler(ExpandedEvent, value);
            }
        }

        public event RoutedEventHandler PreviewCollapsed
        {
            add
            {
                this.AddHandler(PreviewCollapsedEvent, value);
            }
            remove
            {
                this.RemoveHandler(PreviewCollapsedEvent, value);
            }
        }

        public event RoutedEventHandler PreviewExpanded
        {
            add
            {
                this.AddHandler(PreviewExpandedEvent, value);
            }
            remove
            {
                this.RemoveHandler(PreviewExpandedEvent, value);
            }
        }

        static RadExpander()
        {
            IsExpandedProperty = DependencyProperty.Register("IsExpanded", typeof(bool), typeof(RadExpander), new Telerik.Windows.PropertyMetadata(false, delegate (DependencyObject d, DependencyPropertyChangedEventArgs a) {
                (d as RadExpander).IsExpandedChanged((bool) a.NewValue);
            }, (d, a) => (d as RadExpander).CoerceIsExpanded((bool) a)));
            ClickModeProperty = DependencyProperty.Register("ClickMode", typeof(System.Windows.Controls.ClickMode), typeof(RadExpander), new Telerik.Windows.PropertyMetadata(System.Windows.Controls.ClickMode.Release));
            ExpandDirectionProperty = DependencyProperty.Register("ExpandDirection", typeof(Telerik.Windows.Controls.ExpandDirection), typeof(RadExpander), new Telerik.Windows.PropertyMetadata(Telerik.Windows.Controls.ExpandDirection.Down, delegate (DependencyObject d, DependencyPropertyChangedEventArgs a) {
                (d as RadExpander).ChangeVisualState(true);
            }));
            CollapsedEvent = EventManager.RegisterRoutedEvent("Collapsed", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(RadExpander));
            ExpandedEvent = EventManager.RegisterRoutedEvent("Expanded", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(RadExpander));
            PreviewCollapsedEvent = EventManager.RegisterRoutedEvent("PreviewCollapsed", RoutingStrategy.Bubble, typeof(RadRoutedEventHandler), typeof(RadExpander));
            PreviewExpandedEvent = EventManager.RegisterRoutedEvent("PreviewExpanded", RoutingStrategy.Bubble, typeof(RadRoutedEventHandler), typeof(RadExpander));
        }

        public RadExpander()
        {
            base.DefaultStyleKey = typeof(RadExpander);
            
        }

        protected internal override void ChangeVisualState(bool useTransitions)
        {
            base.ChangeVisualState(useTransitions);
            if (this.ExpandDirection == Telerik.Windows.Controls.ExpandDirection.Left)
            {
                this.GoToState(useTransitions, new string[] { "DirectionLeft" });
            }
            else if (this.ExpandDirection == Telerik.Windows.Controls.ExpandDirection.Right)
            {
                this.GoToState(useTransitions, new string[] { "DirectionRight" });
            }
            else if (this.ExpandDirection == Telerik.Windows.Controls.ExpandDirection.Up)
            {
                this.GoToState(useTransitions, new string[] { "DirectionUp" });
            }
            else
            {
                this.GoToState(useTransitions, new string[] { "DirectionDown" });
            }
            if (this.IsExpanded)
            {
                this.ExpandContent(useTransitions);
                if (this.IsAnimated && !this.lastIsExpanedValue)
                {
                    EventHandler playExpand = null;
                    playExpand = delegate (object sender, EventArgs layourEventArgs) {
                        AnimationManager.Play(this, "Expand");
                        this.LayoutUpdated -= playExpand;
                    };
                    base.LayoutUpdated += playExpand;
                }
                this.lastIsExpanedValue = true;
            }
            else if (this.IsAnimated)
            {
                if (this.lastIsExpanedValue)
                {
                    AnimationManager.Play(this, "Collapse", delegate {
                        this.CollapseContent(useTransitions);
                    }, new object[0]);
                    this.lastIsExpanedValue = false;
                }
                this.GoToState(useTransitions, new string[] { "Collapsed" });
            }
            else
            {
                this.CollapseContent(useTransitions);
                this.lastIsExpanedValue = false;
            }
        }

        private object CoerceIsExpanded(bool newValue)
        {
            if (!this.insidePreviewRevert && (newValue != this.lastIsExpanded))
            {
                RadRoutedEventArgs args = newValue ? this.RaisePreviewExpanded() : this.RaisePreviewCollapsed();
                if (args.Handled)
                {
                    newValue = this.lastIsExpanded;
                    this.insidePreviewRevert = true;
                    if (this.HeaderButton != null)
                    {
                        this.HeaderButton.IsChecked = new bool?(newValue);
                    }
                    this.IsExpanded = newValue;
                    this.insidePreviewRevert = false;
                    this.ChangeVisualState();
                }
                this.lastIsExpanded = newValue;
            }
            return newValue;
        }

        private void CollapseContent(bool useTransitions)
        {
            if (!this.IsExpanded)
            {
                this.GoToState(useTransitions, new string[] { "Collapsed" });
                if (this.contentElement != null)
                {
                    this.contentElement.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void ExpandContent(bool useTransitions)
        {
            if (this.IsExpanded)
            {
                if (this.contentElement != null)
                {
                    this.contentElement.Visibility = Visibility.Visible;
                }
                this.GoToState(useTransitions, new string[] { "Expanded" });
            }
        }

        private void GoToState(bool useTransitions, params string[] stateNames)
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

        private void IsExpandedChanged(bool newValue)
        {
            if (!this.insidePreviewRevert)
            {
                this.ChangeVisualState(true);
                RadExpanderAutomationPeer peer = FrameworkElementAutomationPeer.FromElement(this) as RadExpanderAutomationPeer;
                if (peer != null)
                {
                    peer.RaiseExpandCollapseAutomationEvent(!newValue, newValue);
                }
                this.RaiseEvent(new RadRoutedEventArgs(newValue ? ExpandedEvent : CollapsedEvent, this));
                if (this.HeaderButton != null)
                {
                    bool isChecked = this.HeaderButton.IsChecked == true;
                    if (newValue != isChecked)
                    {
                        this.HeaderButton.IsChecked = new bool?(newValue);
                    }
                }
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (this.HeaderButton != null)
            {
                this.HeaderButton.Click -= new RoutedEventHandler(this.OnHeaderClick);
            }
            this.contentElement = base.GetTemplateChild("Content") as FrameworkElement;
            this.headerButton = base.GetTemplateChild("HeaderButton") as ToggleButton;
            if (this.HeaderButton != null)
            {
                this.HeaderButton.IsChecked = new bool?(this.IsExpanded);
                this.HeaderButton.Click += new RoutedEventHandler(this.OnHeaderClick);
            }
            this.ChangeVisualState(false);
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new RadExpanderAutomationPeer(this);
        }

        private void OnHeaderClick(object sender, RoutedEventArgs e)
        {
            this.IsExpanded = !this.IsExpanded;
        }

        private RadRoutedEventArgs RaisePreviewCollapsed()
        {
            RadRoutedEventArgs args = new RadRoutedEventArgs(PreviewCollapsedEvent, this);
            this.RaiseEvent(args);
            return args;
        }

        private RadRoutedEventArgs RaisePreviewExpanded()
        {
            RadRoutedEventArgs args = new RadRoutedEventArgs(PreviewExpandedEvent, this);
            this.RaiseEvent(args);
            return args;
        }

        [DefaultValue(false), Telerik.Windows.Controls.SRDescription("ExpanderClickMode"), Telerik.Windows.Controls.SRCategory("BehaviourCategory")]
        public System.Windows.Controls.ClickMode ClickMode
        {
            get
            {
                return (System.Windows.Controls.ClickMode) base.GetValue(ClickModeProperty);
            }
            set
            {
                base.SetValue(ClickModeProperty, value);
            }
        }

        [Telerik.Windows.Controls.SRDescription("ExpanderDecoratorTemplate"), Telerik.Windows.Controls.SRCategory("ContentCategory"), DefaultValue(false)]
        public ControlTemplate DecoratorTemplate
        {
            get
            {
                return (ControlTemplate) base.GetValue(DecoratorTemplateProperty);
            }
            set
            {
                base.SetValue(DecoratorTemplateProperty, value);
            }
        }

        [Telerik.Windows.Controls.SRDescription("ExpanderExpandDirection"), Telerik.Windows.Controls.SRCategory("CommonCategory"), DefaultValue(0)]
        public Telerik.Windows.Controls.ExpandDirection ExpandDirection
        {
            get
            {
                return (Telerik.Windows.Controls.ExpandDirection) base.GetValue(ExpandDirectionProperty);
            }
            set
            {
                base.SetValue(ExpandDirectionProperty, value);
            }
        }

        internal ToggleButton HeaderButton
        {
            get
            {
                return this.headerButton;
            }
        }

        [Telerik.Windows.Controls.SRCategory("ContentCategory"), Telerik.Windows.Controls.SRDescription("ExpanderHeaderControlTemplate"), DefaultValue(false)]
        public ControlTemplate HeaderControlTemplate
        {
            get
            {
                return (ControlTemplate) base.GetValue(HeaderControlTemplateProperty);
            }
            set
            {
                base.SetValue(HeaderControlTemplateProperty, value);
            }
        }

        [Telerik.Windows.Controls.SRDescription("ExpanderIsAnimated"), Telerik.Windows.Controls.SRCategory("BehaviourCategory"), Browsable(false), DefaultValue(false)]
        internal bool IsAnimated
        {
            get
            {
                return ((this.IsAnimationInitalized && AnimationManager.IsGlobalAnimationEnabled) && AnimationManager.GetIsAnimationEnabled(this));
            }
            set
            {
                AnimationManager.SetIsAnimationEnabled(this, value);
            }
        }

        internal bool IsAnimationInitalized
        {
            get
            {
                return (base.ReadLocalValue(AnimationManager.IsAnimationEnabledProperty) != DependencyProperty.UnsetValue);
            }
        }

        [Telerik.Windows.Controls.SRDescription("ExpanderIsExpanded"), DefaultValue(false), Telerik.Windows.Controls.SRCategory("CommonCategory")]
        public bool IsExpanded
        {
            get
            {
                return (bool) base.GetValue(IsExpandedProperty);
            }
            set
            {
                base.SetValue(IsExpandedProperty, value);
            }
        }

        [Browsable(false), Obsolete("This property is obsolete.", true), DefaultValue((string) null), Telerik.Windows.Controls.SRCategory("AppearanceCategory"), Telerik.Windows.Controls.SRDescription("ExpanderTemplateDown")]
        public ControlTemplate TemplateDown
        {
            get
            {
                return (ControlTemplate) base.GetValue(TemplateDownProperty);
            }
            set
            {
                base.SetValue(TemplateDownProperty, value);
            }
        }

        [Browsable(false), Obsolete("This property is obsolete.", true), DefaultValue((string) null), Telerik.Windows.Controls.SRCategory("AppearanceCategory"), Telerik.Windows.Controls.SRDescription("ExpanderTemplateLeft")]
        public ControlTemplate TemplateLeft
        {
            get
            {
                return (ControlTemplate) base.GetValue(TemplateLeftProperty);
            }
            set
            {
                base.SetValue(TemplateLeftProperty, value);
            }
        }

        [Browsable(false), Telerik.Windows.Controls.SRCategory("AppearanceCategory"), Telerik.Windows.Controls.SRDescription("ExpanderTemplateRight"), Obsolete("This property is obsolete.", true), DefaultValue((string) null)]
        public ControlTemplate TemplateRight
        {
            get
            {
                return (ControlTemplate) base.GetValue(TemplateRightProperty);
            }
            set
            {
                base.SetValue(TemplateRightProperty, value);
            }
        }

        [Obsolete("This property is obsolete.", true), Browsable(false), DefaultValue((string) null), Telerik.Windows.Controls.SRCategory("AppearanceCategory"), Telerik.Windows.Controls.SRDescription("ExpanderTemplateUp")]
        public ControlTemplate TemplateUp
        {
            get
            {
                return (ControlTemplate) base.GetValue(TemplateUpProperty);
            }
            set
            {
                base.SetValue(TemplateUpProperty, value);
            }
        }
    }
}

