namespace Telerik.Windows.Controls.TransitionEffects
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Media.Effects;
    using Telerik.Windows.Controls.TransitionControl;

    public class MotionBlurredZoomTransition : TransitionProvider
    {
        public MotionBlurredZoomTransition()
        {
            this.Center = new Point(0.5, 0.5);
            this.InBlurRatio = 0.3;
            this.OutBlurRatio = -0.3;
            this.Samples = 7.0;
        }

        protected override ShaderEffect CreateTransitionEffect()
        {
            return new MotionBlurredZoomEffect { Center = this.Center, InBlurRatio = this.InBlurRatio, OutBlurRatio = this.OutBlurRatio, Samples = this.Samples };
        }

        public Point Center { get; set; }

        public double InBlurRatio { get; set; }

        public double OutBlurRatio { get; set; }

        public double Samples { get; set; }
    }
}

