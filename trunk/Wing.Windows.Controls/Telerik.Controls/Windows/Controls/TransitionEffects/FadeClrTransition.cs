namespace Telerik.Windows.Controls.TransitionEffects
{
    using System;
    using Telerik.Windows.Controls.TransitionControl;

    public class FadeClrTransition : Transition
    {
        protected override void OnProgressChanged(double oldProgress, double newProgress)
        {
            if (base.OldContentPresenter != null)
            {
                base.OldContentPresenter.Opacity = 1.0 - newProgress;
            }
            if (base.CurrentContentPresenter != null)
            {
                base.CurrentContentPresenter.Opacity = newProgress;
            }
        }
    }
}

