namespace Telerik.Windows.Controls.TransitionEffects
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Windows.Media.Effects;
    using Telerik.Windows.Controls.TransitionControl;

    public class LinearFadeTransition : TransitionProvider
    {
        public LinearFadeTransition()
        {
            double delta;
            this.Thickness = delta = 0.5;
            this.Angle = this.Darker = delta;
        }

        protected override ShaderEffect CreateTransitionEffect()
        {
            return new LinearFadeTransitionEffect { Angle = this.Angle, Darker = this.Darker, Thickness = this.Thickness };
        }

        public double Angle { get; set; }

        public double Darker { get; set; }

        public double Thickness { get; set; }
    }
}

