namespace Telerik.Windows.Controls.TransitionEffects
{
    using System;
    using System.Windows;
    using System.Windows.Media.Effects;

    internal class WaveTransitionEffect : BaseTransitionEffect
    {
        public static readonly DependencyProperty AmplitudeProperty = DependencyProperty.Register("Amplitude", typeof(double), typeof(WaveTransitionEffect), new PropertyMetadata(0.2, ShaderEffect.PixelShaderConstantCallback(1)));
        public static readonly DependencyProperty AngleProperty = DependencyProperty.Register("Angle", typeof(double), typeof(WaveTransitionEffect), new PropertyMetadata(0.5, ShaderEffect.PixelShaderConstantCallback(2)));
        public static readonly DependencyProperty FadeProperty = DependencyProperty.Register("Fade", typeof(double), typeof(WaveTransitionEffect), new PropertyMetadata(0.7, ShaderEffect.PixelShaderConstantCallback(3)));

        public WaveTransitionEffect()
        {
            base.UpdateShaderValue(AngleProperty);
            base.UpdateShaderValue(AmplitudeProperty);
            base.UpdateShaderValue(FadeProperty);
        }

        public double Amplitude
        {
            get
            {
                return (double) base.GetValue(AmplitudeProperty);
            }
            set
            {
                base.SetValue(AmplitudeProperty, value);
            }
        }

        public double Angle
        {
            get
            {
                return (double) base.GetValue(AngleProperty);
            }
            set
            {
                base.SetValue(AngleProperty, value);
            }
        }

        public double Fade
        {
            get
            {
                return (double) base.GetValue(FadeProperty);
            }
            set
            {
                base.SetValue(FadeProperty, value);
            }
        }
    }
}

