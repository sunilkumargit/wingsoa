namespace Telerik.Windows.Controls.TransitionEffects
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Windows.Media.Effects;
    using Telerik.Windows.Controls.TransitionControl;

    public class WaveTransition : TransitionProvider
    {
        public WaveTransition()
        {
            this.Angle = 0.5;
            this.Amplitude = 0.2;
            this.Fade = 0.7;
        }

        protected override ShaderEffect CreateTransitionEffect()
        {
            return new WaveTransitionEffect { Amplitude = this.Amplitude, Angle = this.Angle, Fade = this.Fade };
        }

        public double Amplitude { get; set; }

        public double Angle { get; set; }

        public double Fade { get; set; }
    }
}

