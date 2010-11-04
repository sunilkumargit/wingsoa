namespace Telerik.Windows.Controls.Animation
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Media.Animation;

    public class FadeAnimation : OrientedAnimation
    {
        public FadeAnimation()
        {
            this.MinOpacity = 0.0;
            this.MaxOpacity = 1.0;
        }

        protected override Storyboard CreateAnimationOverride(FrameworkElement control, FrameworkElement target)
        {
            double relativeFrom = base.GetValueDependingOnDirection<double>(this.MinOpacity, this.MaxOpacity);
            double relativeTo = base.GetValueDependingOnDirection<double>(this.MaxOpacity, this.MinOpacity);
            double[] opacityData = new double[4];
            opacityData[1] = relativeFrom;
            opacityData[3] = relativeTo;
            return AnimationExtensions.Create()
                .Animate(new FrameworkElement[] { target })
                .Opacity(opacityData)
                .AdjustSpeed()
                .Instance;
        }

        protected override void UpdateAnimationOverride(FrameworkElement control, Storyboard storyboard, FrameworkElement target, params object[] args)
        {
            double relativeFrom = base.GetValueDependingOnDirection<double>(this.MinOpacity, this.MaxOpacity);
            double relativeTo = base.GetValueDependingOnDirection<double>(this.MaxOpacity, this.MinOpacity);
            double duration = RadAnimation.GetDurationSecondsForLength(200.0);
            double[] opacityData = new double[4];
            opacityData[1] = relativeFrom;
            opacityData[2] = duration;
            opacityData[3] = relativeTo;
            storyboard
                .Update()
                .Animate(new FrameworkElement[] { target })
                .Opacity(opacityData)
                .Easings(base.Easing)
                .AdjustSpeed();
        }

        public double MaxOpacity { get; set; }

        public double MinOpacity { get; set; }
    }
}

