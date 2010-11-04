namespace Telerik.Windows.Controls.TransitionEffects
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.CompilerServices;
    using System.Windows.Media.Effects;
    using Telerik.Windows.Controls.TransitionControl;

    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId="Pixelate", Justification="Pixelate is an effect name.")]
    public class PixelateTransition : TransitionProvider
    {
        public PixelateTransition()
        {
            this.MinimumPixels = 15;
            this.IsPixelLED = true;
        }

        protected override ShaderEffect CreateTransitionEffect()
        {
            return new PixelateTransitionEffect { IsPixelLED = this.IsPixelLED, MinimumPixels = this.MinimumPixels };
        }

        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId="LED", Justification="LED (LightEmittingDiod) is commonly used abbreviature.")]
        public bool IsPixelLED { get; set; }

        public int MinimumPixels { get; set; }
    }
}

