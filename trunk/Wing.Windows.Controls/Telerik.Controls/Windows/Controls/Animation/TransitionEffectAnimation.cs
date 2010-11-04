namespace Telerik.Windows.Controls.Animation
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using Telerik.Windows.Controls.TransitionControl;
    using System.Windows.Media.Imaging;

    [ContentProperty("Transition")]
    public class TransitionEffectAnimation : OrientedAnimation
    {
        private static Brush EmptyBrush = new ImageBrush { ImageSource = new WriteableBitmap(1, 1) };

        protected override Storyboard CreateAnimationOverride(FrameworkElement control, FrameworkElement target)
        {
            if (this.Transition == null)
            {
                return new Storyboard();
            }
            double relativeFrom = base.GetValueDependingOnDirection<double>(0.0, 1.0);
            double relativeTo = base.GetValueDependingOnDirection<double>(1.0, 0.0);
            Storyboard storyboard = null;
            this.Transition.SetupTransitionAnimation(target, target, null, ref storyboard, base.Easing, this.Duration, EmptyBrush, relativeFrom, relativeTo);
            return storyboard;
        }

        protected override void UpdateAnimationOverride(FrameworkElement control, Storyboard storyboard, FrameworkElement target, params object[] args)
        {
            if (this.Transition != null)
            {
                double relativeFrom = base.GetValueDependingOnDirection<double>(0.0, 1.0);
                double relativeTo = base.GetValueDependingOnDirection<double>(1.0, 0.0);
                this.Transition.SetupTransitionAnimation(target, target, null, ref storyboard, base.Easing, this.Duration, EmptyBrush, relativeFrom, relativeTo);
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        private TimeSpan Duration
        {
            get
            {
                return TimeSpan.FromSeconds(RadAnimation.GetDurationSecondsForLength(500.0));
            }
        }

        public TransitionProvider Transition { get; set; }
    }
}

