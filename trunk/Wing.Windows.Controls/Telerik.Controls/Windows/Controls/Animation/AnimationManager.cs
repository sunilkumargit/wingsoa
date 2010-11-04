namespace Telerik.Windows.Controls.Animation
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using Telerik.Windows.Controls;

    public static class AnimationManager
    {
        internal static readonly DependencyProperty AnimationProperty = DependencyProperty.RegisterAttached("Animation", typeof(WeakReference), typeof(AnimationManager), null);
        public static readonly DependencyProperty AnimationSelectorProperty = DependencyProperty.RegisterAttached("AnimationSelector", typeof(AnimationSelectorBase), typeof(AnimationManager), new PropertyMetadata(new PropertyChangedCallback(AnimationManager.OnAnimationSelectorChanged)));
        internal static readonly DependencyProperty CallbacksProperty = DependencyProperty.RegisterAttached("Callbacks", typeof(ICollection<Action>), typeof(AnimationManager), null);
        private static double globalSpeedRatio = 1.0;
        private static bool isAnimationEnabled = true;
        public static readonly DependencyProperty IsAnimationEnabledProperty = DependencyProperty.RegisterAttached("IsAnimationEnabled", typeof(bool), typeof(AnimationManager), new PropertyMetadata(true));

        internal static void AddCallback(Storyboard storyboard, Action callback)
        {
            ICollection<Action> callbacks = GetCallbacks(storyboard);
            if (callbacks == null)
            {
                SetCallbacks(storyboard, new List<Action> { callback });
            }
            else
            {
                callbacks.Add(callback);
            }
        }

        private static void BeginInvokeCallback(Action callback, DependencyObject target)
        {
            if (callback != null)
            {
                target.Dispatcher.BeginInvoke(callback);
            }
        }

        internal static RadAnimation GetAnimation(DependencyObject obj)
        {
            WeakReference result = obj.GetValue(AnimationProperty) as WeakReference;
            if (result == null)
            {
                return null;
            }
            return (result.Target as RadAnimation);
        }

        public static AnimationSelectorBase GetAnimationSelector(DependencyObject obj)
        {
            return (AnimationSelectorBase) obj.GetValue(AnimationSelectorProperty);
        }

        private static ICollection<Action> GetCallbacks(DependencyObject obj)
        {
            return (obj.GetValue(CallbacksProperty) as ICollection<Action>);
        }

        public static bool GetIsAnimationEnabled(DependencyObject obj)
        {
            return (bool) obj.GetValue(IsAnimationEnabledProperty);
        }

        internal static void InvkokeCallbacks(Storyboard storyboard)
        {
            if (storyboard != null)
            {
                ICollection<Action> callbacks = GetCallbacks(storyboard);
                foreach (Timeline child in storyboard.Children)
                {
                    Storyboard childStoryboard = child as Storyboard;
                    if (childStoryboard != null)
                    {
                        InvkokeCallbacks(childStoryboard);
                    }
                }
                if (callbacks != null)
                {
                    foreach (Action callback in callbacks)
                    {
                        callback();
                    }
                    callbacks.Clear();
                }
            }
        }

        private static void OnAnimationSelectorChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ItemsControl itemsControl = sender as ItemsControl;
            if (itemsControl != null)
            {
                foreach (object item in itemsControl.Items)
                {
                    FrameworkElement container = itemsControl.ItemContainerGenerator.ContainerFromItem(item) as FrameworkElement;
                    if (container != null)
                    {
                        SetAnimationSelector(container, e.NewValue as AnimationSelectorBase);
                    }
                }
            }
        }

        private static void OnStoryboardCompleted(object sender, EventArgs e)
        {
            InvkokeCallbacks(sender as Storyboard);
        }

        public static bool Play(FrameworkElement target, string animationName)
        {
            return Play(target, animationName, null, new object[0]);
        }

        public static bool Play(FrameworkElement target, string animationName, Action completeCallback, params object[] args)
        {
            if ((!GetIsAnimationEnabled(target) || !IsGlobalAnimationEnabled) || (VisualTreeHelper.GetChildrenCount(target) <= 0))
            {
                BeginInvokeCallback(completeCallback, target);
                return false;
            }
            AnimationSelectorBase selector = null;
            RadAnimation animation = null;
            Storyboard storyboard = target.Resources[animationName] as Storyboard;
            if (storyboard == null)
            {
                selector = GetAnimationSelector(target);
                if (selector == null)
                {
                    BeginInvokeCallback(completeCallback, target);
                    return false;
                }
                animation = selector.SelectAnimation(target, animationName);
                if (animation == null)
                {
                    BeginInvokeCallback(completeCallback, target);
                    return false;
                }
                storyboard = animation.CreateAnimation(target);
                if (storyboard == null)
                {
                    BeginInvokeCallback(completeCallback, target);
                    return false;
                }
                storyboard.Completed += new EventHandler(AnimationManager.OnStoryboardCompleted);
                SetAnimation(storyboard, animation);
                target.Resources.Add(animationName, storyboard);
            }
            RadAnimation sourceAnimation = GetAnimation(storyboard);
            AddCallback(storyboard, delegate {
                storyboard.Stop();
            });
            if (completeCallback != null)
            {
                AddCallback(storyboard, completeCallback);
            }
            if (storyboard.Children.Count > 0)
            {
                sourceAnimation.UpdateAnimation(target, storyboard, args);
            }
            storyboard.Begin();
            return true;
        }

        internal static void SetAnimation(DependencyObject obj, RadAnimation value)
        {
            if (value == null)
            {
                obj.SetValue(AnimationProperty, null);
            }
            else
            {
                obj.SetValue(AnimationProperty, new WeakReference(value));
            }
        }

        public static void SetAnimationSelector(DependencyObject obj, AnimationSelectorBase value)
        {
            obj.SetValue(AnimationSelectorProperty, value);
        }

        private static void SetCallbacks(DependencyObject obj, ICollection<Action> value)
        {
            obj.SetValue(CallbacksProperty, value);
        }

        public static void SetIsAnimationEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(IsAnimationEnabledProperty, value);
        }

        public static void Stop(FrameworkElement target, string animationName)
        {
            Storyboard storyboard = target.Resources[animationName] as Storyboard;
            if ((storyboard != null) && (storyboard.GetCurrentState() != ClockState.Stopped))
            {
                storyboard.Stop();
                InvkokeCallbacks(storyboard);
            }
        }

        public static void StopIfRunning(FrameworkElement target, string animationName)
        {
            Storyboard storyboard = target.Resources[animationName] as Storyboard;
            if ((storyboard != null) && (storyboard.GetCurrentState() == ClockState.Active))
            {
                storyboard.Stop();
                InvkokeCallbacks(storyboard);
            }
        }

        public static double AnimationSpeedRatio
        {
            get
            {
                return globalSpeedRatio;
            }
            set
            {
                globalSpeedRatio = value;
            }
        }

        public static bool IsGlobalAnimationEnabled
        {
            get
            {
                return isAnimationEnabled;
            }
            set
            {
                isAnimationEnabled = value;
            }
        }
    }
}

