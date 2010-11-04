namespace Telerik.Windows.Controls
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media.Animation;
    using Telerik.Windows;
    using Telerik.Windows.Controls.TransitionControl;
    using Telerik.Windows.Controls.Animation;

    [TemplateVisualState(Name="TransitionInitialized", GroupName="TransitionStates"), TemplatePart(Name="PART_RootElement", Type=typeof(FrameworkElement)), TemplatePart(Name="PART_ContentPresenter", Type=typeof(TransitionPresenter)), TemplateVisualState(Name="InitializingTransition", GroupName="TransitionStates")]
    public class RadTransitionControl : ContentControl
    {
        private TransitionPresenter contentPresenter;
        private const string ContentPresenterPartName = "PART_ContentPresenter";
        public static readonly DependencyProperty DurationProperty = DependencyProperty.Register("Duration", typeof(TimeSpan), typeof(RadTransitionControl), new Telerik.Windows.PropertyMetadata(TimeSpan.FromSeconds(RadAnimation.GetDurationSecondsForLength(500.0))));
        public static readonly DependencyProperty EasingProperty = DependencyProperty.Register("Easing", typeof(IEasingFunction), typeof(RadTransitionControl), null);
        private const string InitializingTransitionStateName = "InitializingTransition";
        private static readonly DependencyPropertyKey IsTransitioningPropertyKey = DependencyPropertyExtensions.RegisterReadOnly("IsTransitioning", typeof(bool), typeof(RadTransitionControl), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadTransitionControl.OnIsTransitioningChanged)));
        public static readonly DependencyProperty IsTransitioningProperty = IsTransitioningPropertyKey.DependencyProperty;
        private const string RootElementPartName = "PART_RootElement";
        private const string TransitionInitializedStateName = "TransitionInitialized";
        public static readonly DependencyProperty TransitionProperty = DependencyProperty.Register("Transition", typeof(TransitionProvider), typeof(RadTransitionControl), null);
        private const string TransitionStatesStateGroupName = "TransitionStates";

        public RadTransitionControl()
        {
            base.DefaultStyleKey = typeof(RadTransitionControl);
            
        }

        private void ChangeVisualState(bool useTransitions)
        {
            if (this.IsTransitioning)
            {
                this.GoToVisualState("InitializingTransition", false);
            }
            else
            {
                this.GoToVisualState("TransitionInitialized", useTransitions);
            }
        }

        private void GoToVisualState(string stateName, bool useTransitions)
        {
            VisualStateManager.GoToState(this, stateName, useTransitions);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (this.contentPresenter != null)
            {
                this.contentPresenter.ContentChanging -= new EventHandler(this.OnContentChanging);
            }
            this.contentPresenter = base.GetTemplateChild("PART_ContentPresenter") as TransitionPresenter;
            if (this.contentPresenter != null)
            {
                this.contentPresenter.ContentChanging += new EventHandler(this.OnContentChanging);
            }
            this.ChangeVisualState(false);
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            if (this.contentPresenter != null)
            {
                this.contentPresenter.CurrentContent = newContent;
            }
            base.OnContentChanged(oldContent, newContent);
        }

        private void OnContentChanging(object sender, EventArgs e)
        {
            this.IsTransitioning = true;
            this.IsTransitioning = false;
        }

        private static void OnIsTransitioningChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as RadTransitionControl).ChangeVisualState(true);
        }

        public void PrepareAnimation()
        {
            this.contentPresenter.PrepareAnimation();
        }

        public void StartAnimation()
        {
            this.contentPresenter.StartAnimation();
        }

        public TimeSpan Duration
        {
            get
            {
                return (TimeSpan) base.GetValue(DurationProperty);
            }
            set
            {
                base.SetValue(DurationProperty, value);
            }
        }

        public IEasingFunction Easing
        {
            get
            {
                return (IEasingFunction) base.GetValue(EasingProperty);
            }
            set
            {
                base.SetValue(EasingProperty, value);
            }
        }

        public bool IsTransitioning
        {
            get
            {
                return (bool) base.GetValue(IsTransitioningProperty);
            }
            protected set
            {
                this.SetValue(IsTransitioningPropertyKey, value);
            }
        }

        public TransitionProvider Transition
        {
            get
            {
                return (TransitionProvider) base.GetValue(TransitionProperty);
            }
            set
            {
                base.SetValue(TransitionProperty, value);
            }
        }
    }
}

