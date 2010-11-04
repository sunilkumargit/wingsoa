namespace Telerik.Windows.Controls.Animation
{
    using System;
    using System.Collections;
    using System.Collections.ObjectModel;
    using System.Windows;
    using System.Windows.Markup;
    using System.Windows.Media.Animation;

    [ContentProperty("Children")]
    public class AnimationGroup : RadAnimation
    {
        private readonly ObservableCollection<RadAnimation> children = new ObservableCollection<RadAnimation>();

        public override Storyboard CreateAnimation(FrameworkElement control)
        {
            Storyboard result = new Storyboard();
            foreach (RadAnimation animation in this.children)
            {
                Storyboard child = animation.CreateAnimation(control);
                if ((base.SpeedRatio != 0.0) && (animation.SpeedRatio == 0.0))
                {
                    animation.SpeedRatio = base.SpeedRatio;
                }
                if (child == null)
                {
                    throw new InvalidOperationException("Calls to CreateAnimation should always return a non-null storyboard");
                }
                result.Children.Add(child);
            }
            return result;
        }

        public override void UpdateAnimation(FrameworkElement control, Storyboard storyboard, params object[] args)
        {
            int currentIndex = 0;
            foreach (RadAnimation animation in this.children)
            {
                if ((base.SpeedRatio != 0.0) && (animation.SpeedRatio == 0.0))
                {
                    animation.SpeedRatio = base.SpeedRatio;
                }
                Storyboard childBoard = storyboard.Children[currentIndex] as Storyboard;
                if (childBoard.Children.Count > 0)
                {
                    animation.UpdateAnimation(control, childBoard, args);
                }
                currentIndex++;
            }
        }

        public IList Children
        {
            get
            {
                return this.children;
            }
        }
    }
}

