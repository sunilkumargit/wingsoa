namespace Telerik.Windows.Controls.TransitionEffects
{
    using System.Windows.Media.Effects;
    using Telerik.Windows.Controls.TransitionControl;

    public class FlipWarpTransition : TransitionProvider
    {
        protected override ShaderEffect CreateTransitionEffect()
        {
            return new FlipWarpTransitionEffect();
        }
    }
}

