namespace Telerik.Windows.Controls.Animation
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Animation;

    public class SlideAnimation : OrientedAnimation
    {
        private static readonly DependencyProperty ClipStartXProperty = DependencyProperty.RegisterAttached("ClipStartX", typeof(double), typeof(SlideAnimation), new PropertyMetadata(new PropertyChangedCallback(SlideAnimation.OnClipStartXChanged)));
        private static readonly DependencyProperty ClipStartYProperty = DependencyProperty.RegisterAttached("ClipStartY", typeof(double), typeof(SlideAnimation), new PropertyMetadata(new PropertyChangedCallback(SlideAnimation.OnClipStartYChanged)));

        public SlideAnimation()
        {
            this.PixelsToAnimate = double.NaN;
            base.Easing = null;
            this.Duration = TimeSpan.MinValue;
            this.Orientation = System.Windows.Controls.Orientation.Vertical;
        }

        private static void ClipControlWhileAnimationIsRunning(FrameworkElement target, Storyboard storyboard)
        {
            target.Clip = new RectangleGeometry();
            EventHandler completeHandler = null;
            completeHandler = new EventHandler(delegate(object sender, EventArgs e)
            {
                target.ClearValue(UIElement.ClipProperty);
                storyboard.Completed -= completeHandler;
            });
            storyboard.Completed += completeHandler;
        }

        protected override Storyboard CreateAnimationOverride(FrameworkElement control, FrameworkElement target)
        {
            return AnimationExtensions.Create().Animate(new FrameworkElement[] { target }).SingleProperty(ClipStartXProperty, new double[6]).SingleProperty(ClipStartYProperty, new double[6]).EnsureDefaultTransforms().MoveX(new double[6]).MoveY(new double[6]).Instance;
        }

        private void DisableControlWhileAnimationIsRunning(FrameworkElement target, Storyboard storyboard)
        {
            FrameworkElement firstChild = null;
            target.IsHitTestVisible = false;
            if (VisualTreeHelper.GetChildrenCount(target) > 0)
            {
                firstChild = VisualTreeHelper.GetChild(target, 0) as FrameworkElement;
                firstChild.IsHitTestVisible = false;
                if (base.Direction == AnimationDirection.In)
                {
                    EventHandler completeHandler = null;
                    completeHandler = new EventHandler(delegate(object sender, EventArgs e)
                    {
                        firstChild.IsHitTestVisible = true;
                        target.IsHitTestVisible = true;
                        storyboard.Completed -= completeHandler;
                    });
                    storyboard.Completed += completeHandler;
                }
            }
        }

        private double GetAnimationDuration()
        {
            if (!(this.Duration == TimeSpan.MinValue))
            {
                return this.Duration.TotalSeconds;
            }
            return base.GetValueDependingOnDirection<double>(0.265, 0.165);
        }

        private IEasingFunction GetEasing()
        {
            return (base.Easing ?? base.GetValueDependingOnDirection<IEasingFunction>(Easings.CircleOut, Easings.CircleIn));
        }

        private double GetPixelsToAnimate()
        {
            if (!double.IsNaN(this.PixelsToAnimate))
            {
                return this.PixelsToAnimate;
            }
            return base.GetValueDependingOnDirection<double>(75.0, 50.0);
        }

        private static void OnClipStartXChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UIElement element = d as UIElement;
            RectangleGeometry clip = d.GetValue(UIElement.ClipProperty) as RectangleGeometry;
            if (clip != null)
            {
                return;
            }
            var rect = new RectangleGeometry();
            rect.Rect = new Rect((double)e.NewValue, 0.0, Math.Max(element.DesiredSize.Width, element.RenderSize.Width), Math.Max(element.DesiredSize.Height, element.RenderSize.Height));
            element.Clip = rect;
        }

        private static void OnClipStartYChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UIElement element = d as UIElement;
            RectangleGeometry clip = d.GetValue(UIElement.ClipProperty) as RectangleGeometry;
            if (clip != null)
            {
                return;
            }
            var rect = new RectangleGeometry();
            rect.Rect = new Rect(0.0, (double)e.NewValue, Math.Max(element.DesiredSize.Width, element.RenderSize.Width), Math.Max(element.DesiredSize.Height, element.RenderSize.Height));
            element.Clip = rect;
        }

        protected override void UpdateAnimationOverride(FrameworkElement control, Storyboard storyboard, FrameworkElement target, params object[] args)
        {
            System.Windows.Controls.Orientation orientation = this.Orientation;
            Telerik.Windows.Controls.Animation.SlideMode slideMode = this.SlideMode;
            if ((args.Length > 0) && (args[0] is System.Windows.Controls.Orientation))
            {
                orientation = (System.Windows.Controls.Orientation)args[0];
            }
            if ((args.Length > 1) && (args[1] is Telerik.Windows.Controls.Animation.SlideMode))
            {
                slideMode = (Telerik.Windows.Controls.Animation.SlideMode)args[1];
            }
            double value = this.GetPixelsToAnimate();
            int valueCoeficient = (slideMode == Telerik.Windows.Controls.Animation.SlideMode.Top) ? -1 : 1;
            double fromValue = base.GetValueDependingOnDirection<double>(value * valueCoeficient, 0.0);
            double endValue = base.GetValueDependingOnDirection<double>(0.0, value * valueCoeficient);
            double delay = base.GetDelay(target);
            double duration = this.GetAnimationDuration() + delay;
            if (orientation == System.Windows.Controls.Orientation.Vertical)
            {
                double[] clipXStart = new double[6];
                clipXStart[2] = delay;
                clipXStart[4] = duration;
                double[] clipYStart = new double[6];
                clipYStart[1] = -fromValue;
                clipYStart[2] = delay;
                clipYStart[3] = -fromValue;
                clipYStart[4] = duration;
                clipYStart[5] = -endValue;
                double[] xCoord = new double[6];
                xCoord[2] = delay;
                xCoord[4] = duration;
                double[] yCoord = new double[6];
                yCoord[1] = fromValue;
                yCoord[2] = delay;
                yCoord[3] = fromValue;
                yCoord[4] = duration;
                yCoord[5] = endValue;
                storyboard.Update().Animate(new FrameworkElement[] { target })
                    .SingleProperty(ClipStartXProperty, clipXStart)
                    .SingleProperty(ClipStartYProperty, clipYStart).Easings(this.GetEasing())
                    .MoveX(xCoord)
                    .MoveY(yCoord)
                    .Easings(this.GetEasing());
            }
            else
            {
                double[] clipXStart = new double[6];
                clipXStart[1] = -fromValue;
                clipXStart[2] = delay;
                clipXStart[3] = -fromValue;
                clipXStart[4] = duration;
                clipXStart[5] = -endValue;
                double[] clipYStart = new double[6];
                clipYStart[2] = delay;
                clipYStart[4] = duration;
                double[] xCoord = new double[6];
                xCoord[1] = fromValue;
                xCoord[2] = delay;
                xCoord[3] = fromValue;
                xCoord[4] = duration;
                xCoord[5] = endValue;
                double[] yCoord = new double[6];
                yCoord[2] = delay;
                yCoord[4] = duration;
                storyboard.Update().Animate(new FrameworkElement[] { target })
                    .SingleProperty(ClipStartXProperty, clipXStart)
                    .SingleProperty(ClipStartYProperty, clipYStart)
                    .Easings(this.GetEasing())
                    .MoveX(xCoord)
                    .MoveY(yCoord)
                    .Easings(this.GetEasing());
            }
            ClipControlWhileAnimationIsRunning(target, storyboard);
        }

        public TimeSpan Duration { get; set; }

        public System.Windows.Controls.Orientation Orientation { get; set; }

        public double PixelsToAnimate { get; set; }

        public Telerik.Windows.Controls.Animation.SlideMode SlideMode { get; set; }
    }
}

