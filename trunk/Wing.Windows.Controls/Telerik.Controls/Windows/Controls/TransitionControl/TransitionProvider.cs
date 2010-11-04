namespace Telerik.Windows.Controls.TransitionControl
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Media.Effects;
    using Telerik.Windows;
    using Telerik.Windows.Controls.TransitionEffects;

    public abstract class TransitionProvider
    {
        protected TransitionProvider()
        {
        }

        protected virtual Transition CreateTransition()
        {
            return new FadeClrTransition();
        }

        protected abstract ShaderEffect CreateTransitionEffect();
        private Transition GetTransition(FrameworkElement currentContentPresenter, FrameworkElement oldContentPresenter)
        {
            Transition transition = this.CreateTransition();
            if (transition != null)
            {
                transition.Initialize(currentContentPresenter, oldContentPresenter);
                Transition.SetTransition(currentContentPresenter, transition);
                return transition;
            }
            currentContentPresenter.ClearValue(Transition.TransitionProperty);
            return transition;
        }

        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        internal ShaderEffect GetTransitionEffect()
        {
            return this.CreateTransitionEffect();
        }

        private static void OnTransitionAnimationCompleted(FrameworkElement targetElement)
        {
            targetElement.ClearValue(UIElement.EffectProperty);
            targetElement.ClearValue(Transition.TransitionProperty);
        }

        internal void SetupTransitionAnimation(FrameworkElement targetElement, FrameworkElement currentContentPresenter, FrameworkElement oldContentPresenter, ref Storyboard animation, IEasingFunction easing, TimeSpan animationDuration, Brush oldVisualBrush, double progressFrom, double progressTo)
        {
            currentContentPresenter.TestNotNull("currentContentPresenter");
            ShaderEffect effect = this.GetTransitionEffect();
            Transition transition = null;
            if (effect == null)
            {
                transition = this.GetTransition(currentContentPresenter, oldContentPresenter);
            }
            if (animation == null)
            {
                animation = new Storyboard();
                animation.Completed += delegate (object s, EventArgs e) {
                    OnTransitionAnimationCompleted(targetElement);
                };
            }
            DoubleAnimationUsingKeyFrames doubleAnimation = animation.Children.OfType<DoubleAnimationUsingKeyFrames>().FirstOrDefault<DoubleAnimationUsingKeyFrames>();
            if (doubleAnimation == null)
            {
                doubleAnimation = new DoubleAnimationUsingKeyFrames();
                animation.Children.Add(doubleAnimation);
            }
            if (effect != null)
            {
                SetupTransitionEffect(effect, animation, targetElement, oldVisualBrush, progressFrom);
            }
            else if (transition != null)
            {
                Storyboard.SetTarget(doubleAnimation, transition);
                Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath(Transition.ProgressProperty));
                transition.Progress = progressFrom;
            }
            IEnumerable<EasingDoubleKeyFrame> easingFrames = doubleAnimation.KeyFrames.OfType<EasingDoubleKeyFrame>();
            EasingDoubleKeyFrame start = easingFrames.FirstOrDefault<EasingDoubleKeyFrame>();
            EasingDoubleKeyFrame end = easingFrames.Skip<EasingDoubleKeyFrame>(1).FirstOrDefault<EasingDoubleKeyFrame>();
            if (start == null)
            {
                doubleAnimation.KeyFrames.Add(start = new EasingDoubleKeyFrame());
            }
            start.KeyTime = TimeSpan.FromMilliseconds(1.0);
            start.EasingFunction = easing;
            start.Value = progressFrom;
            if (end == null)
            {
                doubleAnimation.KeyFrames.Add(end = new EasingDoubleKeyFrame());
            }
            end.KeyTime = animationDuration;
            end.EasingFunction = easing;
            end.Value = progressTo;
        }

        private static void SetupTransitionEffect(ShaderEffect effect, Timeline animation, FrameworkElement targetElement, Brush oldVisualBrush, double progressFrom)
        {
            TransitionEffect transitionEffect = effect as TransitionEffect;
            transitionEffect.OldSampler = oldVisualBrush;
            transitionEffect.Progress = progressFrom;
            targetElement.Effect = effect;
            Storyboard.SetTarget(animation, targetElement.Effect);
            Storyboard.SetTargetProperty(animation, new PropertyPath(TransitionEffect.ProgressProperty));
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        internal void StopAnimation(Storyboard animation, FrameworkElement targetElement)
        {
            bool wasAnimationRunning = false;
            try
            {
                wasAnimationRunning = animation.GetCurrentState() == ClockState.Active;
            }
            catch (InvalidOperationException)
            {
            }
            if (wasAnimationRunning)
            {
                OnTransitionAnimationCompleted(targetElement);
            }
            animation.Stop();
        }
    }
}

