namespace Telerik.Windows.Controls.TransitionEffects
{
    using System.Windows.Media.Effects;
    using Telerik.Windows.Controls.TransitionControl;

    public class FadeTransition : TransitionProvider
    {
        protected override ShaderEffect CreateTransitionEffect()
        {
            return new FadeTransitionEffect();
        }
    }
}

