namespace Telerik.Windows.Controls
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media.Animation;

    public class FadeTransition : ITransition
    {
        private Storyboard currentAnimation;
        private Control currentPage;
        private Control newPage;
        private TimeSpan timeDuration;

        public FadeTransition()
        {
            this.timeDuration = new TimeSpan(0, 0, 1);
        }

        public FadeTransition(TimeSpan duration)
        {
            this.timeDuration = new TimeSpan(0, 0, 1);
            this.timeDuration = duration;
            this.IsRunning = false;
        }

        public void Begin(IFrame page)
        {
            Panel target = null;
            IFrame previousFrame = null;
            if (target.Children.Count != 0)
            {
                previousFrame = target.Children[0] as IFrame;
            }
            this.IsRunning = true;
            this.newPage = page as Control;
            this.currentPage = previousFrame as Control;
            if (this.currentPage != this.newPage)
            {
                if (this.currentPage != null)
                {
                    this.currentPage.RenderTransform = null;
                    Duration duration = new Duration(this.timeDuration);
                    DoubleAnimation animation = new DoubleAnimation
                    {
                        Duration = duration,
                        To = 0.0
                    };
                    Storyboard fadeIn = new Storyboard();
                    this.CurrentStoryboard = fadeIn;
                    fadeIn.Duration = duration;
                    fadeIn.Children.Add(animation);
                    fadeIn.Completed += new EventHandler(this.FadeIn_Completed);
                    Storyboard.SetTarget(animation, this.currentPage);
                    Storyboard.SetTargetProperty(animation, new PropertyPath("Opacity", new object[0]));
                    fadeIn.Begin();
                }
                else
                {
                    target.Children.Clear();
                    target.Children.Add(this.newPage);
                    target.UpdateLayout();
                }
            }
        }

        private void FadeIn_Completed(object sender, EventArgs e)
        {
            (sender as Storyboard).Stop();
            Panel target = null; // NavigationService.GetNavigationService().Target;
            target.Children.Clear();
            this.currentPage = null;
            if (!target.Children.Contains(this.newPage))
            {
                target.Children.Add(this.newPage);
                this.newPage.RenderTransform = null;
            }
            else
            {
                RadFrameContainer nextPanel = this.newPage.Parent as RadFrameContainer;
                if (nextPanel != null)
                {
                    nextPanel.Children.Clear();
                    target.Children.Add(this.newPage);
                    target.UpdateLayout();
                }
            }
            this.newPage.RenderTransform = null;
            TimeSpan fadeOutDuration = TimeSpan.FromSeconds(this.timeDuration.TotalSeconds / 2.0);
            Duration duration = new Duration(fadeOutDuration);
            DoubleAnimation animation = new DoubleAnimation
            {
                Duration = duration,
                From = 0.0,
                To = 1.0
            };
            Storyboard fadeOut = new Storyboard();
            this.CurrentStoryboard = fadeOut;
            fadeOut.Duration = duration;
            fadeOut.Children.Add(animation);
            Storyboard.SetTarget(animation, this.newPage);
            Storyboard.SetTargetProperty(animation, new PropertyPath("Opacity", new object[0]));
            fadeOut.Completed += new EventHandler(this.FadeOut_Completed);
            fadeOut.Begin();
        }

        private void FadeOut_Completed(object sender, EventArgs e)
        {
            (sender as Storyboard).Stop();
            this.IsRunning = false;
        }

        public void StopTransition()
        {
            if (this.CurrentStoryboard != null)
            {
                this.CurrentStoryboard.Stop();
            }
        }

        public Storyboard CurrentStoryboard
        {
            get
            {
                return this.currentAnimation;
            }
            set
            {
                this.currentAnimation = value;
            }
        }

        public bool IsRunning { get; set; }
    }
}

