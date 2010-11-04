namespace Telerik.Windows.Controls.TransitionEffects
{
    using System;
    using System.Windows;
    using Telerik.Windows.Controls.TransitionControl;
    using System.Windows.Media.Effects;

    internal class MotionBlurredZoomEffect : BaseTransitionEffect
    {
        public static readonly DependencyProperty CenterProperty = DependencyProperty.Register("Center", typeof(Point), typeof(MotionBlurredZoomEffect), new PropertyMetadata(new Point(0.5, 0.5), ShaderEffect.PixelShaderConstantCallback(1)));
        public static readonly DependencyProperty InBlurRatioProperty = DependencyProperty.Register("InBlurRatio", typeof(double), typeof(MotionBlurredZoomEffect), new PropertyMetadata(0.3, ShaderEffect.PixelShaderConstantCallback(2)));
        public static readonly DependencyProperty OutBlurRatioProperty = DependencyProperty.Register("OutBlurRatio", typeof(double), typeof(MotionBlurredZoomEffect), new PropertyMetadata(-0.3, ShaderEffect.PixelShaderConstantCallback(3)));
        public static readonly DependencyProperty SamplesProperty = DependencyProperty.Register("Samples", typeof(double), typeof(MotionBlurredZoomEffect), new PropertyMetadata(7.0, ShaderEffect.PixelShaderConstantCallback(4)));

        public MotionBlurredZoomEffect()
        {
            base.UpdateShaderValue(TransitionEffect.ProgressProperty);
            base.UpdateShaderValue(CenterProperty);
            base.UpdateShaderValue(InBlurRatioProperty);
            base.UpdateShaderValue(OutBlurRatioProperty);
            base.UpdateShaderValue(SamplesProperty);
        }

        public Point Center
        {
            get
            {
                return (Point) base.GetValue(CenterProperty);
            }
            set
            {
                base.SetValue(CenterProperty, value);
            }
        }

        public double InBlurRatio
        {
            get
            {
                return (double) base.GetValue(InBlurRatioProperty);
            }
            set
            {
                base.SetValue(InBlurRatioProperty, value);
            }
        }

        public double OutBlurRatio
        {
            get
            {
                return (double) base.GetValue(OutBlurRatioProperty);
            }
            set
            {
                base.SetValue(OutBlurRatioProperty, value);
            }
        }

        public double Samples
        {
            get
            {
                return (double) base.GetValue(SamplesProperty);
            }
            set
            {
                base.SetValue(SamplesProperty, value);
            }
        }
    }
}

