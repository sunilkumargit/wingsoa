namespace Telerik.Windows.Controls.Animation
{
    using System;
    using System.Windows;
    using System.Windows.Media.Animation;

    public class MoveAnimation : BaseAnimation
    {
        public static readonly DependencyProperty CurrentPositionProperty = DependencyProperty.RegisterAttached("CurrentPosition", typeof(Point), typeof(MoveAnimation), new PropertyMetadata(new Point(), new PropertyChangedCallback(MoveAnimation.OnCurrentPositionPropertyChanged)));
        public static readonly DependencyProperty OldPositionProperty = DependencyProperty.RegisterAttached("OldPosition", typeof(Point), typeof(MoveAnimation), new PropertyMetadata(new Point()));

        protected override Storyboard CreateAnimationOverride(FrameworkElement control, FrameworkElement target)
        {
            RadAnimation.GetDurationSecondsForLength(200.0);
            Point currentPosition = GetCurrentPosition(target);
            Point oldPosition = GetOldPosition(target);
            AnimationExtensions.AnimationContext result = AnimationExtensions.Create().Animate(new FrameworkElement[] { target }).EnsureDefaultTransforms();
            if (target.ReadLocalValue(CurrentPositionProperty) != DependencyProperty.UnsetValue)
            {
                double[] xCoord = new double[4];
                xCoord[1] = oldPosition.X;
                xCoord[3] = currentPosition.X;
                double[] yCoord = new double[4];
                yCoord[1] = oldPosition.Y;
                yCoord[3] = currentPosition.Y;
                result = result.MoveX(xCoord).MoveY(yCoord);
            }
            return result.AdjustSpeed().Instance;
        }

        public static Point GetCurrentPosition(DependencyObject obj)
        {
            return (Point)obj.GetValue(CurrentPositionProperty);
        }

        public static Point GetOldPosition(DependencyObject obj)
        {
            return (Point)obj.GetValue(OldPositionProperty);
        }

        private static void OnCurrentPositionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.SetValue(OldPositionProperty, e.OldValue);
        }

        public static void SetCurrentPosition(DependencyObject obj, Point value)
        {
            obj.SetValue(CurrentPositionProperty, value);
        }

        public static void SetOldPosition(DependencyObject obj, Point value)
        {
            obj.SetValue(OldPositionProperty, value);
        }

        protected override void UpdateAnimationOverride(FrameworkElement control, Storyboard storyboard, FrameworkElement target, params object[] args)
        {
            double duration = RadAnimation.GetDurationSecondsForLength(200.0);
            Point currentPosition = GetCurrentPosition(target);
            Point oldPosition = GetOldPosition(target);
            AnimationExtensions.AnimationContext result = storyboard.Update().Animate(new FrameworkElement[] { target }).EnsureDefaultTransforms();
            if (target.ReadLocalValue(CurrentPositionProperty) != DependencyProperty.UnsetValue)
            {
                double[] xCoord = new double[4];
                xCoord[1] = oldPosition.X;
                xCoord[2] = duration;
                xCoord[3] = currentPosition.X;
                double[] yCoord = new double[4];
                yCoord[1] = oldPosition.Y;
                yCoord[2] = duration;
                yCoord[3] = currentPosition.Y;
                result = result.MoveX(xCoord)
                    .Easings(base.Easing)
                    .MoveY(yCoord)
                    .Easings(base.Easing);
            }
        }
    }
}

