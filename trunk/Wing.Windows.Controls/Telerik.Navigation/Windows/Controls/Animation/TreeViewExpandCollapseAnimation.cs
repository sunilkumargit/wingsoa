namespace Telerik.Windows.Controls.Animation
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Media.Animation;

    public class TreeViewExpandCollapseAnimation : RadAnimation
    {
        private const double MaximumAnimationDistance = 200.0;

        public TreeViewExpandCollapseAnimation()
        {
            this.TargetName = "ItemsHost";
        }

        public override Storyboard CreateAnimation(FrameworkElement control)
        {
            FrameworkElement target = RadAnimation.FindTarget(control, this.TargetName);
            if (target == null)
            {
                return null;
            }
            return AnimationExtensions.Create().Animate(new FrameworkElement[] { target }).Height(new double[4]).Instance;
        }

        public override void UpdateAnimation(FrameworkElement control, Storyboard storyboard, params object[] args)
        {
            FrameworkElement target = RadAnimation.FindTarget(control, this.TargetName);
            if (storyboard.GetCurrentState() != ClockState.Stopped)
            {
                storyboard.Stop();
                target.Height = double.NaN;
            }
            double endHeight = target.ActualHeight;
            double startHeight = 0.0;
            if (this.Direction == AnimationDirection.Out)
            {
                startHeight = endHeight;
                endHeight = 0.0;
            }
            if ((this.Direction == AnimationDirection.In) && (endHeight > 200.0))
            {
                startHeight = endHeight - 200.0;
            }
            if ((startHeight > 400.0) && (this.Direction == AnimationDirection.Out))
            {
                startHeight = 400.0;
            }
            double duration = Math.Min(2.0, RadAnimation.GetDurationSecondsForLength(Math.Max(startHeight, endHeight)));
            IEasingFunction splineToUse = (this.Direction == AnimationDirection.In) ? Easings.QuarticOut : Easings.QuinticOut;
            double[] animData = new double[4];
            animData[1] = startHeight;
            animData[2] = duration;
            animData[3] = endHeight;
            storyboard.Update().Animate(new FrameworkElement[] { target }).Height(animData).Easings(splineToUse);
            if (this.Direction != AnimationDirection.Out)
            {
                target.Height = startHeight;
            }
            AnimationManager.AddCallback(storyboard, delegate {
                target.Height = double.NaN;
            });
            base.UpdateAnimation(control, storyboard, new object[0]);
        }

        public AnimationDirection Direction { get; set; }

        public string TargetName { get; set; }
    }
}

