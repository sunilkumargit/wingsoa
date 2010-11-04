namespace Telerik.Windows.Controls.Animation
{
    using System;
    using System.Windows;

    public abstract class AnimationSelectorBase
    {
        protected AnimationSelectorBase()
        {
        }

        public abstract RadAnimation SelectAnimation(FrameworkElement control, string name);
    }
}

