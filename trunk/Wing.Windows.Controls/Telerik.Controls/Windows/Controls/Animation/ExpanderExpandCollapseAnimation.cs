namespace Telerik.Windows.Controls.Animation
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Media.Animation;
    using Telerik.Windows.Controls;

    public class ExpanderExpandCollapseAnimation : RadAnimation
    {
        public override Storyboard CreateAnimation(FrameworkElement control)
        {
            FrameworkElement target = RadAnimation.FindTarget(control, this.TargetElementName);
            if (target == null)
            {
                return null;
            }
            if (IsVertical(control))
            {
                return AnimationExtensions.Create().Animate(new FrameworkElement[] { target }).Height(new double[4]).Instance;
            }
            return AnimationExtensions.Create().Animate(new FrameworkElement[] { target }).Width(new double[4]).Instance;
        }

        internal static bool IsVertical(FrameworkElement control)
        {
            RadExpander expander = control as RadExpander;
            return ((expander == null) || ((expander.ExpandDirection != ExpandDirection.Left) && (expander.ExpandDirection != ExpandDirection.Right)));
        }

        public override void UpdateAnimation(FrameworkElement control, Storyboard storyboard, params object[] args)
        {
            bool isAnimationVertical = IsVertical(control);
            FrameworkElement target = RadAnimation.FindTarget(control, this.TargetElementName);
            if (storyboard.GetCurrentState() != ClockState.Stopped)
            {
                storyboard.Stop();
                target.Height = double.NaN;
            }
            double endLength = isAnimationVertical ? target.ActualHeight : target.ActualWidth;
            double startLength = 0.0;
            if (this.Direction == AnimationDirection.Out)
            {
                startLength = endLength;
                endLength = 0.0;
            }
            double duration = RadAnimation.GetDurationSecondsForLength(Math.Max(startLength, endLength));
            IEasingFunction splineToUse = (this.Direction == AnimationDirection.In) ? Easings.SlideDown1 : Easings.SlideUp1;
            if (isAnimationVertical)
            {
                double[] hCoord = new double[4];
                hCoord[1] = startLength;
                hCoord[2] = duration;
                hCoord[3] = endLength;
                storyboard.Update().Animate(new FrameworkElement[] { target }).Height(hCoord).Easings(splineToUse);
                if (this.Direction != AnimationDirection.Out)
                {
                    target.Height = startLength;
                }
                AnimationManager.AddCallback(storyboard, delegate {
                    target.Height = double.NaN;
                });
            }
            else
            {
                double[] wCoord = new double[4];
                wCoord[1] = startLength;
                wCoord[2] = duration;
                wCoord[3] = endLength;
                storyboard.Update().Animate(new FrameworkElement[] { target }).Width(wCoord).Easings(splineToUse);
                if (this.Direction != AnimationDirection.Out)
                {
                    target.Width = startLength;
                }
                AnimationManager.AddCallback(storyboard, delegate {
                    target.Width = double.NaN;
                });
            }
            base.UpdateAnimation(control, storyboard, new object[0]);
        }

        public AnimationDirection Direction { get; set; }

        public string TargetElementName { get; set; }
    }
}

