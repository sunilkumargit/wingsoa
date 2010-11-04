namespace Telerik.Windows.Controls.Animation
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Animation;

    public abstract class RadAnimation
    {
        internal static readonly double PixelsPerSecond = 500.0;

        protected RadAnimation()
        {
        }

        public abstract Storyboard CreateAnimation(FrameworkElement control);
        internal static FrameworkElement FindTarget(FrameworkElement control, string targetName)
        {
            if (string.IsNullOrEmpty(targetName))
            {
                return control;
            }
            if (VisualTreeHelper.GetChildrenCount(control) > 0)
            {
                FrameworkElement layoutRoot = VisualTreeHelper.GetChild(control, 0) as FrameworkElement;
                return (layoutRoot.FindName(targetName) as FrameworkElement);
            }
            return null;
        }

        internal static double GetDurationSecondsForLength(double pixelsLength)
        {
            double duration = pixelsLength / PixelsPerSecond;
            return Math.Max(duration, 0.2);
        }

        public virtual void UpdateAnimation(FrameworkElement control, Storyboard storyboard, params object[] args)
        {
            if (this.SpeedRatio != 0.0)
            {
                storyboard.SpeedRatio = this.SpeedRatio;
            }
            else
            {
                storyboard.SpeedRatio = AnimationManager.AnimationSpeedRatio;
            }
        }

        public string AnimationName { get; set; }

        public double SpeedRatio { get; set; }
    }
}

