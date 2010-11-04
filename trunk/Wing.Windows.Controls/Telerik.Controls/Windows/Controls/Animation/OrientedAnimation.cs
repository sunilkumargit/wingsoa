namespace Telerik.Windows.Controls.Animation
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Windows;

    public abstract class OrientedAnimation : BaseAnimation
    {
        private static readonly DependencyProperty ShouldSkipDelayProperty = DependencyProperty.RegisterAttached("ShouldSkipDelay", typeof(bool), typeof(OrientedAnimation), null);

        protected OrientedAnimation()
        {
        }

        protected double GetDelay(DependencyObject target)
        {
            if ((this.Direction == AnimationDirection.In) && !GetShouldSkipDelay(target))
            {
                SetShouldSkipDelay(target, true);
                return 0.1;
            }
            return 0.0;
        }

        private static bool GetShouldSkipDelay(DependencyObject d)
        {
            return (bool) d.GetValue(ShouldSkipDelayProperty);
        }

        protected T GetValueDependingOnDirection<T>(T inValue, T outValue)
        {
            if (this.Direction == AnimationDirection.In)
            {
                return inValue;
            }
            return outValue;
        }

        private static void SetShouldSkipDelay(DependencyObject d, bool value)
        {
            d.SetValue(ShouldSkipDelayProperty, value);
        }

        public AnimationDirection Direction { get; set; }
    }
}

