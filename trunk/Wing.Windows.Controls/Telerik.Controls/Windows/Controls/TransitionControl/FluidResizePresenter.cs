namespace Telerik.Windows.Controls.TransitionControl
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Animation;

    public class FluidResizePresenter : ContentPresenter
    {
        private EventHandler animator;
        private double coef;
        private long currentFrame;
        private Size currentMeasureSize;
        public static readonly DependencyProperty DurationProperty = DependencyProperty.Register("Duration", typeof(System.Windows.Duration), typeof(FluidResizePresenter), new PropertyMetadata(new System.Windows.Duration(TimeSpan.FromSeconds(0.5))));
        public static readonly DependencyProperty EasingProperty = DependencyProperty.Register("Easing", typeof(IEasingFunction), typeof(FluidResizePresenter), new PropertyMetadata(new CircleEase { EasingMode=EasingMode.EaseInOut }));
        private long frames;
        private bool isAnimated;
        private Size? lastDesiredSize;
        private long lastTick = DateTime.Now.Ticks;
        private Size newMeasureSize;
        private Size oldMeasureSize;
        private long ticks;
        private long totalTicks;

        public FluidResizePresenter()
        {
            this.animator = new EventHandler(this.CompositionTarget_Rendering);
            this.FramesPerSecond = 30;
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            if (this.NextFrame())
            {
                base.InvalidateMeasure();
            }
        }

        private void Initialize()
        {
            this.ticks = TimeSpan.FromSeconds(1.0).Ticks / ((long) this.FramesPerSecond);
            this.totalTicks = this.Duration.TimeSpan.Ticks;
            this.frames = this.totalTicks / this.ticks;
            this.currentFrame = 0L;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            Size desiredSize = base.MeasureOverride(availableSize);
            if (desiredSize != this.lastDesiredSize)
            {
                if (!this.lastDesiredSize.HasValue)
                {
                    this.oldMeasureSize = desiredSize;
                    this.newMeasureSize = desiredSize;
                    this.currentMeasureSize = desiredSize;
                }
                else
                {
                    this.newMeasureSize = desiredSize;
                    this.oldMeasureSize = this.currentMeasureSize;
                    this.RunAnimation();
                }
                this.lastDesiredSize = new Size?(desiredSize);
            }
            else if (this.isAnimated)
            {
                if (this.currentFrame == this.frames)
                {
                    this.StopAnimation();
                }
                this.currentMeasureSize.Width = this.oldMeasureSize.Width + ((this.newMeasureSize.Width - this.oldMeasureSize.Width) * this.coef);
                this.currentMeasureSize.Height = this.oldMeasureSize.Height + ((this.newMeasureSize.Height - this.oldMeasureSize.Height) * this.coef);
                if (Math.Abs((double) (this.currentMeasureSize.Width - this.newMeasureSize.Width)) > 1.0)
                {
                    this.currentMeasureSize.Width = Math.Round((double) (this.currentMeasureSize.Width / 2.0)) * 2.0;
                }
                if (Math.Abs((double) (this.currentMeasureSize.Height - this.newMeasureSize.Height)) > 1.0)
                {
                    this.currentMeasureSize.Height = Math.Round((double) (this.currentMeasureSize.Height / 2.0)) * 2.0;
                }
            }
            return this.currentMeasureSize;
        }

        private bool NextFrame()
        {
            long now = DateTime.Now.Ticks;
            if ((now - this.lastTick) < this.ticks)
            {
                return false;
            }
            this.lastTick = now;
            this.currentFrame += 1L;
            double time = ((double) this.currentFrame) / ((double) this.frames);
            this.coef = (this.Easing != null) ? this.Easing.Ease(time) : time;
            return true;
        }

        private void RunAnimation()
        {
            this.Initialize();
            if (!this.isAnimated)
            {
                CompositionTarget.Rendering += this.animator;
            }
            this.isAnimated = true;
        }

        private void StopAnimation()
        {
            this.isAnimated = false;
            CompositionTarget.Rendering -= this.animator;
        }

        public System.Windows.Duration Duration
        {
            get
            {
                return (System.Windows.Duration) base.GetValue(DurationProperty);
            }
            set
            {
                base.SetValue(DurationProperty, value);
            }
        }

        public IEasingFunction Easing
        {
            get
            {
                return (IEasingFunction) base.GetValue(EasingProperty);
            }
            set
            {
                base.SetValue(EasingProperty, value);
            }
        }

        private int FramesPerSecond { get; set; }
    }
}

