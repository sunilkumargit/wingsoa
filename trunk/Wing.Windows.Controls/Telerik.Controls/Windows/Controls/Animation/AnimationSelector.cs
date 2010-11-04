namespace Telerik.Windows.Controls.Animation
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Windows;
    using System.Windows.Markup;

    [ContentProperty("Animations")]
    public class AnimationSelector : AnimationSelectorBase
    {
        private ObservableCollection<RadAnimation> animations = new ObservableCollection<RadAnimation>();
        private Dictionary<string, RadAnimation> availableAnimations = new Dictionary<string, RadAnimation>();

        public AnimationSelector()
        {
            this.animations.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnAnimationsCollectionChanged);
        }

        private void OnAnimationAdded(RadAnimation radAnimation)
        {
            if (radAnimation.AnimationName != null)
            {
                this.availableAnimations.Add(radAnimation.AnimationName, radAnimation);
            }
        }

        private void OnAnimationRemoved(RadAnimation radAnimation)
        {
            if (this.availableAnimations.ContainsKey(radAnimation.AnimationName))
            {
                this.availableAnimations.Remove(radAnimation.AnimationName);
            }
        }

        private void OnAnimationsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                this.OnAnimationAdded(e.NewItems[0] as RadAnimation);
            }
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                this.OnAnimationRemoved(e.OldItems[0] as RadAnimation);
            }
        }

        public override RadAnimation SelectAnimation(FrameworkElement control, string name)
        {
            if (this.availableAnimations.ContainsKey(name))
            {
                return this.availableAnimations[name];
            }
            foreach (RadAnimation animation in this.animations)
            {
                if (animation.AnimationName == name)
                {
                    this.availableAnimations.Add(name, animation);
                    return animation;
                }
            }
            return null;
        }

        public IList Animations
        {
            get
            {
                return this.animations;
            }
        }
    }
}

