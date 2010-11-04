namespace Telerik.Windows.Controls.Animation
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Media.Animation;

    public abstract class BaseAnimation : RadAnimation
    {
        protected BaseAnimation()
        {
            this.Easing = Easings.QuarticOut;
        }

        public sealed override Storyboard CreateAnimation(FrameworkElement control)
        {
            FrameworkElement target = string.IsNullOrEmpty(this.TargetElementName) ? control : RadAnimation.FindTarget(control, this.TargetElementName);
            if (target == null)
            {
                return new Storyboard();
            }
            return this.CreateAnimationOverride(control, target);
        }

        protected abstract Storyboard CreateAnimationOverride(FrameworkElement control, FrameworkElement target);
        public sealed override void UpdateAnimation(FrameworkElement control, Storyboard storyboard, params object[] args)
        {
            FrameworkElement target = RadAnimation.FindTarget(control, this.TargetElementName);
            if (target != null)
            {
                this.UpdateAnimationOverride(control, storyboard, target, args);
            }
            base.UpdateAnimation(control, storyboard, new object[0]);
        }

        protected abstract void UpdateAnimationOverride(FrameworkElement control, Storyboard storyboard, FrameworkElement target, params object[] args);

        public IEasingFunction Easing { get; set; }

        public string TargetElementName { get; set; }
    }
}

