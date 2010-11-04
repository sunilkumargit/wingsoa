namespace Telerik.Windows.Controls.Animation
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Media.Animation;

    public class ScaleAnimation : OrientedAnimation
    {
        public ScaleAnimation()
        {
            this.MinScale = 0.01;
            this.MaxScale = 1.0;
        }

        protected override Storyboard CreateAnimationOverride(FrameworkElement control, FrameworkElement target)
        {
            double relativeFrom = base.GetValueDependingOnDirection<double>(this.MinScale, this.MaxScale);
            double relatoveTo = base.GetValueDependingOnDirection<double>(this.MaxScale, this.MinScale);
            target.RenderTransformOrigin = new Point(0.5, 0.5);
            double[] xCoord = new double[4];
            xCoord[1] = relativeFrom;
            xCoord[3] = relatoveTo;
            double[] yCoord = new double[4];
            yCoord[1] = relativeFrom;
            yCoord[3] = relatoveTo;
            return AnimationExtensions.Create().Animate(new FrameworkElement[] { target })
                .EnsureDefaultTransforms()
                .ScaleX(xCoord)
                .ScaleY(yCoord)
                .AdjustSpeed()
                .Instance;
        }

        protected override void UpdateAnimationOverride(FrameworkElement control, Storyboard storyboard, FrameworkElement target, params object[] args)
        {
            double relativeFrom = base.GetValueDependingOnDirection<double>(this.MinScale, this.MaxScale);
            double relativeTo = base.GetValueDependingOnDirection<double>(this.MaxScale, this.MinScale);
            double duration = RadAnimation.GetDurationSecondsForLength(100.0);
            double[] xCoord = new double[4];
            xCoord[1] = relativeFrom;
            xCoord[2] = duration;
            xCoord[3] = relativeTo;
            double[] yCoord = new double[4];
            yCoord[1] = relativeFrom;
            yCoord[2] = duration;
            yCoord[3] = relativeTo;
            storyboard.Update()
                .Animate(new FrameworkElement[] { target })
                .EnsureDefaultTransforms()
                .ScaleX(xCoord)
                .Easings(base.Easing)
                .ScaleY(yCoord)
                .Easings(base.Easing)
                .AdjustSpeed();
        }

        public double MaxScale { get; set; }

        public double MinScale { get; set; }
    }
}

