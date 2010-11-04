namespace Telerik.Windows.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Controls;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;
    using Telerik.Windows.Controls.Animation;
    using System.Windows;

    public class LoadingIndicator : Control
    {
        private List<Storyboard> animations = new List<Storyboard>();
        private bool isRunning;
        private List<Ellipse> points = new List<Ellipse>();

        public LoadingIndicator()
        {
            base.DefaultStyleKey = typeof(LoadingIndicator);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            for (int i = 1; i < 11; i++)
            {
                this.points.Add((Ellipse)base.GetTemplateChild("ellipse" + i));
            }
            List<double> scaleFrames = new List<double> { 0.0, 0.0, 0.05, 1.0, 0.5, 1.0, 1.0, 0.0, 1.0, 0.0 };
            for (int i = 0; i < this.points.Count; i++)
            {
                Storyboard anim = AnimationExtensions.Create()
                    .Animate(new FrameworkElement[] { this.points[i] })
                    .Opacity(scaleFrames.ToArray())
                    .SingleProperty(Rad3D.ScaleProperty, scaleFrames.ToArray())
                    .Instance;
                anim.RepeatBehavior = RepeatBehavior.Forever;
                anim.BeginTime = new TimeSpan?(TimeSpan.FromSeconds(i * 0.1));
                this.animations.Add(anim);
            }
            this.Start();
        }

        public void Start()
        {
            if (!this.isRunning)
            {
                this.animations.ForEach(delegate(Storyboard w)
                {
                    w.Begin();
                });
                this.isRunning = true;
            }
        }

        public void Stop()
        {
            this.animations.ForEach(delegate(Storyboard w)
            {
                w.Stop();
            });
            this.isRunning = false;
        }

        public bool IsRunning
        {
            get
            {
                return this.isRunning;
            }
        }
    }
}

