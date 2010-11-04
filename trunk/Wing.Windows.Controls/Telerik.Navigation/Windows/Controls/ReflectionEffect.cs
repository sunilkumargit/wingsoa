namespace Telerik.Windows.Controls
{
    using System;
    using System.Windows;
    using System.Windows.Media.Effects;

    public class ReflectionEffect : ShaderEffect
    {
        public static readonly DependencyProperty ElementHeightProperty = DependencyProperty.Register("ElementHeight", typeof(double), typeof(ReflectionEffect), new PropertyMetadata(100.0, new PropertyChangedCallback(ReflectionEffect.OnElementHeightChanged)));
        public static readonly DependencyProperty ReflectionHeightProperty = DependencyProperty.Register("ReflectionHeight", typeof(double), typeof(ReflectionEffect), new PropertyMetadata(1.0, ShaderEffect.PixelShaderConstantCallback(1)));
        public static readonly DependencyProperty ReflectionOpacityProperty = DependencyProperty.Register("ReflectionOpacity", typeof(double), typeof(ReflectionEffect), new PropertyMetadata(1.0, ShaderEffect.PixelShaderConstantCallback(0)));

        public ReflectionEffect()
        {
            base.PixelShader = new PixelShader { UriSource = new Uri("/Telerik.Windows.Controls.Navigation;component/coverflow/Reflection.ps", UriKind.Relative) };
            base.UpdateShaderValue(ReflectionOpacityProperty);
            base.UpdateShaderValue(ReflectionHeightProperty);
        }

        private static void OnElementHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ReflectionEffect).PaddingBottom = (double) e.NewValue;
        }

        public double ElementHeight
        {
            get
            {
                return (double) base.GetValue(ElementHeightProperty);
            }
            set
            {
                base.SetValue(ElementHeightProperty, value);
            }
        }

        public double ReflectionHeight
        {
            get
            {
                return (double) base.GetValue(ReflectionHeightProperty);
            }
            set
            {
                base.SetValue(ReflectionHeightProperty, value);
            }
        }

        public double ReflectionOpacity
        {
            get
            {
                return (double) base.GetValue(ReflectionOpacityProperty);
            }
            set
            {
                base.SetValue(ReflectionOpacityProperty, value);
            }
        }
    }
}

