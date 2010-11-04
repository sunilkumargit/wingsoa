namespace Telerik.Windows.Controls.Animation
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Media.Animation;

    internal class Easings
    {
        private static IEasingFunction circleIn;
        private static IEasingFunction circleOut;
        private static IEasingFunction quarticOut;
        private static IEasingFunction quiticOut;
        private static IEasingFunction slideDown1;
        private static IEasingFunction slideUp1;

        internal Easings(double x1, double y1, double x2, double y2)
        {
            this.Point1 = new Point(x1, y1);
            this.Point2 = new Point(x2, y2);
        }

        internal static IEasingFunction CircleIn
        {
            get
            {
                if (Easings.circleIn == null)
                {
                    IEasingFunction circleIn = Easings.circleIn;
                }
                return (Easings.circleIn = new CircleEase { EasingMode = EasingMode.EaseIn });
            }
        }

        internal static IEasingFunction CircleOut
        {
            get
            {
                if (Easings.circleOut == null)
                {
                    IEasingFunction circleOut = Easings.circleOut;
                }
                return (Easings.circleOut = new CircleEase { EasingMode = EasingMode.EaseOut });
            }
        }

        internal Point Point1 { get; set; }

        internal Point Point2 { get; set; }

        internal static IEasingFunction QuarticOut
        {
            get
            {
                if (Easings.quarticOut == null)
                {
                    IEasingFunction quarticOut = Easings.quarticOut;
                }
                return (Easings.quarticOut = new QuarticEase { EasingMode = EasingMode.EaseOut });
            }
        }

        internal static IEasingFunction QuinticOut
        {
            get
            {
                if (Easings.quiticOut == null)
                {
                    IEasingFunction quiticOut = Easings.quiticOut;
                }
                return (Easings.quiticOut = new QuarticEase { EasingMode = EasingMode.EaseOut });
            }
        }

        internal static IEasingFunction SlideDown1
        {
            get
            {
                if (slideDown1 == null)
                {
                    IEasingFunction function1 = slideDown1;
                }
                return (slideDown1 = new QuadraticEase());
            }
        }

        internal static Easings SlideDown2
        {
            get
            {
                return new Easings(0.264, 0.0, 0.228, 1.0);
            }
        }

        internal static Easings SlideDown3
        {
            get
            {
                return new Easings(0.02, 0.196, 0.362, 1.0);
            }
        }

        internal static IEasingFunction SlideUp1
        {
            get
            {
                if (slideUp1 == null)
                {
                    IEasingFunction function1 = slideUp1;
                }
                return (slideUp1 = new QuadraticEase());
            }
        }

        internal static Easings SlideUp2
        {
            get
            {
                return new Easings(0.224, 0.0, 0.0, 1.0);
            }
        }

        internal static Easings SlideUp3
        {
            get
            {
                return new Easings(0.0, 0.116, 0.431, 1.0);
            }
        }

        private abstract class BaseEase : IEasingFunction
        {
            protected BaseEase()
            {
            }

            public double Ease(double normalizedTime)
            {
                if (this.EasingMode == System.Windows.Media.Animation.EasingMode.EaseIn)
                {
                    return this.F(normalizedTime);
                }
                if (this.EasingMode == System.Windows.Media.Animation.EasingMode.EaseOut)
                {
                    return (1.0 - this.F(1.0 - normalizedTime));
                }
                return normalizedTime;
            }

            protected abstract double F(double t);

            public System.Windows.Media.Animation.EasingMode EasingMode { get; set; }
        }

        private class CircleEase : Easings.BaseEase
        {
            protected override double F(double t)
            {
                return (1.0 - Math.Sqrt(1.0 - (t * t)));
            }
        }
    }
}

