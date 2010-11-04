namespace Telerik.Windows.Controls.TransitionControl
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using Telerik.Windows;
    using Telerik.Windows.Controls.Animation;
    using Telerik.Windows.Media.Imaging;

    [ContentProperty("CurrentContent")]
    public class TransitionPresenter : ContentPresenter
    {
        private Storyboard animation;
        public static readonly DependencyProperty AnimationDurationProperty = DependencyProperty.RegisterAttached("AnimationDuration", typeof(TimeSpan), typeof(TransitionPresenter), new Telerik.Windows.PropertyMetadata(DefaultAnimationDuration, new PropertyChangedCallback(TransitionPresenter.OnAnimationDurationPropertyChanged)));
        public static readonly DependencyProperty AnimationEasingProperty = DependencyProperty.RegisterAttached("AnimationEasing", typeof(IEasingFunction), typeof(TransitionPresenter), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(TransitionPresenter.OnAnimationEasingPropertyChanged)));
        public static readonly DependencyProperty CurrentContentProperty = DependencyProperty.Register("CurrentContent", typeof(object), typeof(TransitionPresenter), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(TransitionPresenter.OnContentChange)));
        public static readonly DependencyProperty CurrentContentTemplateProperty = DependencyProperty.Register("CurrentContentTemplate", typeof(DataTemplate), typeof(TransitionPresenter), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(TransitionPresenter.OnContentTemplateChange)));
        public static readonly DependencyProperty DurationProperty = DependencyProperty.Register("Duration", typeof(TimeSpan), typeof(TransitionPresenter), new Telerik.Windows.PropertyMetadata(DefaultAnimationDuration));
        public static readonly DependencyProperty EasingProperty = DependencyProperty.Register("Easing", typeof(IEasingFunction), typeof(TransitionPresenter), null);
        public static readonly DependencyProperty OldContentPresenterProperty = DependencyProperty.Register("OldContentPresenter", typeof(FrameworkElement), typeof(TransitionPresenter), null);
        private static readonly DependencyPropertyKey OldVisualBrushPropertyKey = DependencyPropertyExtensions.RegisterReadOnly("OldVisualBrush", typeof(Brush), typeof(TransitionPresenter), null);
        public static readonly DependencyProperty OldVisualBrushProperty = OldVisualBrushPropertyKey.DependencyProperty;
        private static readonly DependencyPropertyKey OldVisualHeightPropertyKey = DependencyPropertyExtensions.RegisterReadOnly("OldVisualHeight", typeof(double), typeof(TransitionPresenter), null);
        public static readonly DependencyProperty OldVisualHeightProperty = OldVisualHeightPropertyKey.DependencyProperty;
        private static readonly DependencyPropertyKey OldVisualWidthPropertyKey = DependencyPropertyExtensions.RegisterReadOnly("OldVisualWidth", typeof(double), typeof(TransitionPresenter), null);
        public static readonly DependencyProperty OldVisualWidthProperty = OldVisualWidthPropertyKey.DependencyProperty;
        internal static readonly DependencyProperty TargetElementProperty = DependencyProperty.Register("TargetElement", typeof(FrameworkElement), typeof(TransitionPresenter), null);
        public static readonly DependencyProperty TransitionEffectProperty = DependencyProperty.RegisterAttached("TransitionEffect", typeof(TransitionProvider), typeof(TransitionPresenter), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(TransitionPresenter.OnTransitionEffectPropertyChanged)));
        public static readonly DependencyProperty TransitionProperty = DependencyProperty.Register("Transition", typeof(TransitionProvider), typeof(TransitionPresenter), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(TransitionPresenter.OnTransitionPropertyChanged)));

        internal event EventHandler ContentChanging;

        public TransitionPresenter()
        {
            base.Loaded += new RoutedEventHandler(this.OnLoaded);
            
        }

        private void ChangeContent()
        {
            this.PrepareAnimation();
            this.OnContentChanging();
        }

        public static TimeSpan GetAnimationDuration(DependencyObject obj)
        {
            return (TimeSpan) obj.GetValue(AnimationDurationProperty);
        }

        public static IEasingFunction GetAnimationEasing(DependencyObject obj)
        {
            return (IEasingFunction) obj.GetValue(AnimationEasingProperty);
        }

        public static TransitionProvider GetTransitionEffect(DependencyObject obj)
        {
            return (TransitionProvider) obj.GetValue(TransitionEffectProperty);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            Size result = base.MeasureOverride(availableSize);
            this.StartAnimation();
            return result;
        }

        private static void OnAnimationDurationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TimeSpan newValue = (TimeSpan) e.NewValue;
            TransitionPresenter control = d as TransitionPresenter;
            if (control != null)
            {
                control.Duration = newValue;
            }
        }

        private static void OnAnimationEasingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            IEasingFunction newValue = e.NewValue as IEasingFunction;
            TransitionPresenter control = d as TransitionPresenter;
            if (control != null)
            {
                control.Easing = newValue;
            }
        }

        private static void OnContentChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TransitionPresenter tp = (TransitionPresenter) d;
            tp.ChangeContent();
            tp.Content = e.NewValue;
        }

        private void OnContentChanging()
        {
            if (this.ContentChanging != null)
            {
                this.ContentChanging(this, EventArgs.Empty);
            }
        }

        private static void OnContentTemplateChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TransitionPresenter tp = (TransitionPresenter) d;
            tp.ChangeContent();
            tp.ContentTemplate = (DataTemplate) e.NewValue;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.IsLoaded = true;
        }

        private static void OnTransitionEffectPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TransitionProvider newValue = e.NewValue as TransitionProvider;
            TransitionPresenter control = d as TransitionPresenter;
            if (control != null)
            {
                control.Transition = newValue;
            }
        }

        private static void OnTransitionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            object newValue = e.NewValue;
            TransitionPresenter presenter1 = (TransitionPresenter) d;
        }

        private void PlayAnimation()
        {
            if (this.animation != null)
            {
                this.animation.Begin();
            }
        }

        public void PrepareAnimation()
        {
            FrameworkElement target = this.ActualTargetElement;
            Size size = target.RenderSize;
            if (this.IsLoaded)
            {
                this.OldVisualBrush = new ImageBrush { ImageSource = ExportHelper.GetElementImage(target) };
                this.OldVisualWidth = size.Width;
                this.OldVisualHeight = size.Height;
                this.SetupAndStartTransitionAnimation(target);
            }
        }

        public static void SetAnimationDuration(DependencyObject obj, TimeSpan value)
        {
            obj.SetValue(AnimationDurationProperty, value);
        }

        public static void SetAnimationEasing(DependencyObject obj, IEasingFunction value)
        {
            obj.SetValue(AnimationEasingProperty, value);
        }

        public static void SetTransitionEffect(DependencyObject obj, TransitionProvider value)
        {
            obj.SetValue(TransitionEffectProperty, value);
        }

        private void SetupAndStartTransitionAnimation(FrameworkElement target)
        {
            this.StopAnimation();
            if (this.Transition != null)
            {
                this.Transition.SetupTransitionAnimation(target, this, this.OldContentPresenter, ref this.animation, this.Easing, this.Duration, this.OldVisualBrush, 0.001, 1.0);
                this.ShouldStartAnimation = true;
            }
        }

        public void StartAnimation()
        {
            if (this.ShouldStartAnimation)
            {
                base.Dispatcher.BeginInvoke(new Action(this.PlayAnimation));
                this.ShouldStartAnimation = false;
            }
        }

        private void StopAnimation()
        {
            if ((this.animation != null) && (this.Transition != null))
            {
                this.Transition.StopAnimation(this.animation, this.ActualTargetElement);
            }
        }

        private FrameworkElement ActualTargetElement
        {
            get
            {
                return (this.TargetElement ?? ((VisualTreeHelper.GetParent(this) as FrameworkElement) ?? this));
            }
        }

        public object CurrentContent
        {
            get
            {
                return base.GetValue(CurrentContentProperty);
            }
            set
            {
                base.SetValue(CurrentContentProperty, value);
            }
        }

        public DataTemplate CurrentContentTemplate
        {
            get
            {
                return (DataTemplate) base.GetValue(CurrentContentTemplateProperty);
            }
            set
            {
                base.SetValue(CurrentContentTemplateProperty, value);
            }
        }

        private static TimeSpan DefaultAnimationDuration
        {
            get
            {
                return TimeSpan.FromSeconds(RadAnimation.GetDurationSecondsForLength(500.0));
            }
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

        private bool IsLoaded { get; set; }

        public FrameworkElement OldContentPresenter
        {
            get
            {
                return (FrameworkElement) base.GetValue(OldContentPresenterProperty);
            }
            set
            {
                base.SetValue(OldContentPresenterProperty, value);
            }
        }

        public Brush OldVisualBrush
        {
            get
            {
                return (Brush) base.GetValue(OldVisualBrushProperty);
            }
            protected set
            {
                this.SetValue(OldVisualBrushPropertyKey, value);
            }
        }

        public double OldVisualHeight
        {
            get
            {
                return (double) base.GetValue(OldVisualHeightProperty);
            }
            protected set
            {
                this.SetValue(OldVisualHeightPropertyKey, value);
            }
        }

        public double OldVisualWidth
        {
            get
            {
                return (double) base.GetValue(OldVisualWidthProperty);
            }
            protected set
            {
                this.SetValue(OldVisualWidthPropertyKey, value);
            }
        }

        private bool ShouldStartAnimation { get; set; }

        internal FrameworkElement TargetElement
        {
            get
            {
                return (FrameworkElement) base.GetValue(TargetElementProperty);
            }
            set
            {
                base.SetValue(TargetElementProperty, value);
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

