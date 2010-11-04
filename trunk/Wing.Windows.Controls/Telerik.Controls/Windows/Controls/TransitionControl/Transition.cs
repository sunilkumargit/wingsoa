namespace Telerik.Windows.Controls.TransitionControl
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Windows;

    public abstract class Transition : DependencyObject
    {
        private bool isAttached;
        public static readonly DependencyProperty ProgressProperty = DependencyProperty.Register("Progress", typeof(double), typeof(Transition), new PropertyMetadata((double) 1.0 / (double) 0.0, new PropertyChangedCallback(Transition.OnProgressPropertyChange)));
        internal static readonly DependencyProperty TransitionProperty = DependencyProperty.RegisterAttached("Transition", typeof(Transition), typeof(Transition), new PropertyMetadata(new PropertyChangedCallback(Transition.OnTransitionPropertyChanged)));

        protected Transition()
        {
        }

        internal static Transition GetTransition(DependencyObject obj)
        {
            return (Transition) obj.GetValue(TransitionProperty);
        }

        internal void Initialize(FrameworkElement currentContentPresenter, FrameworkElement oldContentPresenter)
        {
            this.CurrentContentPresenter = currentContentPresenter;
            this.OldContentPresenter = oldContentPresenter;
        }

        protected virtual void OnCleanUp()
        {
        }

        protected virtual void OnInitialized()
        {
        }

        protected abstract void OnProgressChanged(double oldProgress, double newProgress);
        private static void OnProgressPropertyChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Transition transition = d as Transition;
            if ((transition != null) && transition.isAttached)
            {
                transition.OnProgressChanged((double) e.OldValue, (double) e.NewValue);
            }
        }

        private static void OnTransitionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Transition oldValue = e.OldValue as Transition;
            Transition newValue = e.NewValue as Transition;
            if (oldValue != null)
            {
                oldValue.isAttached = false;
                oldValue.OnCleanUp();
            }
            if (newValue != null)
            {
                newValue.isAttached = true;
                newValue.OnInitialized();
            }
        }

        internal static void SetTransition(DependencyObject obj, Transition value)
        {
            obj.SetValue(TransitionProperty, value);
        }

        protected FrameworkElement CurrentContentPresenter { get; set; }

        protected FrameworkElement OldContentPresenter { get; set; }

        public double Progress
        {
            get
            {
                return (double) base.GetValue(ProgressProperty);
            }
            set
            {
                base.SetValue(ProgressProperty, value);
            }
        }
    }
}

