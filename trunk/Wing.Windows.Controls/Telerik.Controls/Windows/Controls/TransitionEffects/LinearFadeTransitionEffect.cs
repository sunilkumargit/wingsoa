namespace Telerik.Windows.Controls.TransitionEffects
{
    using System;
    using System.Windows;
    using System.Windows.Media.Effects;

    internal class LinearFadeTransitionEffect : BaseTransitionEffect
    {
        public static readonly DependencyProperty AngleProperty = DependencyProperty.Register("Angle", typeof(double), typeof(LinearFadeTransitionEffect), new PropertyMetadata(0.5, ShaderEffect.PixelShaderConstantCallback(1)));
        public static readonly DependencyProperty DarkerProperty = DependencyProperty.Register("Darker", typeof(double), typeof(LinearFadeTransitionEffect), new PropertyMetadata(0.5, ShaderEffect.PixelShaderConstantCallback(3)));
        public static readonly DependencyProperty ThicknessProperty = DependencyProperty.Register("Thickness", typeof(double), typeof(LinearFadeTransitionEffect), new PropertyMetadata(0.5, ShaderEffect.PixelShaderConstantCallback(2)));

        public LinearFadeTransitionEffect()
        {
            base.UpdateShaderValue(AngleProperty);
            base.UpdateShaderValue(ThicknessProperty);
            base.UpdateShaderValue(DarkerProperty);
        }

        public double Angle
        {
            get
            {
                return (double)base.GetValue(AngleProperty);
            }
            set
            {
                base.SetValue(AngleProperty, value);
            }
        }

        public double Darker
        {
            get
            {
                return (double)base.GetValue(DarkerProperty);
            }
            set
            {
                base.SetValue(DarkerProperty, value);
            }
        }

        public double Thickness
        {
            get
            {
                return (double)base.GetValue(ThicknessProperty);
            }
            set
            {
                base.SetValue(ThicknessProperty, value);
            }
        }
    }
}

