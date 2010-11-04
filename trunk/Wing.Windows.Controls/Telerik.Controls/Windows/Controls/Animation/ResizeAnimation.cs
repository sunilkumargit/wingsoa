namespace Telerik.Windows.Controls.Animation
{
    using System;
    using System.Windows;
    using System.Windows.Media.Animation;

    public class ResizeAnimation : BaseAnimation
    {
        public static readonly DependencyProperty CurrentSizeProperty = DependencyProperty.RegisterAttached("CurrentSize", typeof(Size), typeof(MoveAnimation), new PropertyMetadata(new Size(0.0, 0.0), new PropertyChangedCallback(ResizeAnimation.OnCurrentSizePropertyChanged)));
        public static readonly DependencyProperty OldSizeProperty = DependencyProperty.RegisterAttached("OldSize", typeof(Size), typeof(MoveAnimation), new PropertyMetadata(new Size(0.0, 0.0)));

        protected override Storyboard CreateAnimationOverride(FrameworkElement control, FrameworkElement target)
        {
            RadAnimation.GetDurationSecondsForLength(200.0);
            Size currentSize = GetCurrentSize(target);
            Size oldSize = GetOldSize(target);
            AnimationExtensions.AnimationContext result = AnimationExtensions.Create().Animate(new FrameworkElement[] { target }).EnsureDefaultTransforms();
            if (target.ReadLocalValue(CurrentSizeProperty) != DependencyProperty.UnsetValue)
            {
                double[] wCoord = new double[4];
                wCoord[1] = oldSize.Width;
                wCoord[3] = currentSize.Width;
                double[] hCoord = new double[4];
                hCoord[1] = oldSize.Height;
                hCoord[3] = currentSize.Height;
                result = result.Width(wCoord).Height(hCoord);
            }
            return result.AdjustSpeed().Instance;
        }

        public static Size GetCurrentSize(DependencyObject obj)
        {
            return (Size) obj.GetValue(CurrentSizeProperty);
        }

        public static Size GetOldSize(DependencyObject obj)
        {
            return (Size) obj.GetValue(OldSizeProperty);
        }

        private static void OnCurrentSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.SetValue(OldSizeProperty, e.OldValue);
        }

        public static void SetCurrentSize(DependencyObject obj, Size value)
        {
            obj.SetValue(CurrentSizeProperty, value);
        }

        public static void SetOldSize(DependencyObject obj, Size value)
        {
            obj.SetValue(OldSizeProperty, value);
        }

        protected override void UpdateAnimationOverride(FrameworkElement control, Storyboard storyboard, FrameworkElement target, params object[] args)
        {
            double duration = RadAnimation.GetDurationSecondsForLength(200.0);
            Size currentSize = GetCurrentSize(target);
            Size oldSize = GetOldSize(target);
            AnimationExtensions.AnimationContext result = storyboard.Update().Animate(new FrameworkElement[] { target }).EnsureDefaultTransforms();
            if (target.ReadLocalValue(CurrentSizeProperty) != DependencyProperty.UnsetValue)
            {
                double[] wCoord = new double[4];
                wCoord[1] = oldSize.Width;
                wCoord[2] = duration;
                wCoord[3] = currentSize.Width;
                double[] hCoord = new double[4];
                hCoord[1] = oldSize.Height;
                hCoord[2] = duration;
                hCoord[3] = currentSize.Height;
                result = result.Width(wCoord).Easings(base.Easing).Height(hCoord).Easings(base.Easing).AdjustSpeed();
            }
        }
    }
}

