namespace Telerik.Windows.Controls.TransitionEffects
{
    using System;
    using System.Windows;
    using System.Windows.Media.Effects;

    internal class PixelateTransitionEffect : BaseTransitionEffect
    {
        private static readonly DependencyProperty IsPixelLEDDoubleProperty = DependencyProperty.Register("IsPixelLEDDouble", typeof(double), typeof(PixelateTransitionEffect), new PropertyMetadata(1.0, ShaderEffect.PixelShaderConstantCallback(4)));
        public static readonly DependencyProperty IsPixelLEDProperty = DependencyProperty.Register("IsPixelLED", typeof(bool), typeof(PixelateTransitionEffect), new PropertyMetadata(true, new PropertyChangedCallback(PixelateTransitionEffect.OnIsPixelLEDChanged)));
        private static readonly DependencyProperty MinimumPixelsDoubleProperty = DependencyProperty.Register("MinimumPixelsDouble", typeof(double), typeof(PixelateTransitionEffect), new PropertyMetadata(15.0, ShaderEffect.PixelShaderConstantCallback(3)));
        public static readonly DependencyProperty MinimumPixelsProperty = DependencyProperty.Register("MinimumPixels", typeof(int), typeof(PixelateTransitionEffect), new PropertyMetadata(15, new PropertyChangedCallback(PixelateTransitionEffect.OnMinimumPixelsChanged)));

        public PixelateTransitionEffect()
        {
            base.UpdateShaderValue(MinimumPixelsDoubleProperty);
            base.UpdateShaderValue(IsPixelLEDDoubleProperty);
        }

        private void OnIsPixelLEDChanged(DependencyPropertyChangedEventArgs e)
        {
            this.IsPixelLEDDouble = ((bool) e.NewValue) ? 1.0 : 0.0;
        }

        private static void OnIsPixelLEDChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as PixelateTransitionEffect).OnIsPixelLEDChanged(e);
        }

        private void OnMinimumPixelsChanged(DependencyPropertyChangedEventArgs e)
        {
            this.MinimumPixelsDouble = (int) e.NewValue;
        }

        private static void OnMinimumPixelsChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as PixelateTransitionEffect).OnMinimumPixelsChanged(e);
        }

        public bool IsPixelLED
        {
            get
            {
                return (bool) base.GetValue(IsPixelLEDProperty);
            }
            set
            {
                base.SetValue(IsPixelLEDProperty, value);
            }
        }

        private double IsPixelLEDDouble
        {
            get
            {
                return (double) base.GetValue(IsPixelLEDDoubleProperty);
            }
            set
            {
                base.SetValue(IsPixelLEDDoubleProperty, value);
            }
        }

        public int MinimumPixels
        {
            get
            {
                return (int) base.GetValue(MinimumPixelsProperty);
            }
            set
            {
                base.SetValue(MinimumPixelsProperty, value);
            }
        }

        private double MinimumPixelsDouble
        {
            get
            {
                return (double) base.GetValue(MinimumPixelsDoubleProperty);
            }
            set
            {
                base.SetValue(MinimumPixelsDoubleProperty, value);
            }
        }
    }
}

