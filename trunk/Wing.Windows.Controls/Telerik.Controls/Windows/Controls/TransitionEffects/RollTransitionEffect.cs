namespace Telerik.Windows.Controls.TransitionEffects
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media.Effects;

    internal class RollTransitionEffect : BaseTransitionEffect
    {
        public static readonly DependencyProperty InterpolationFunctionProperty = DependencyProperty.Register("InterpolationFunction", typeof(double), typeof(RollTransitionEffect), new PropertyMetadata(2.0, ShaderEffect.PixelShaderConstantCallback(4)));
        public static readonly DependencyProperty IsRollOutProperty = DependencyProperty.Register("IsRollOut", typeof(bool), typeof(RollTransitionEffect), new PropertyMetadata(new PropertyChangedCallback(RollTransitionEffect.OnIsRollOutChange)));
        public static readonly DependencyProperty IsTopToBottomProperty = DependencyProperty.Register("IsTopToBottom", typeof(bool), typeof(RollTransitionEffect), new PropertyMetadata(true, new PropertyChangedCallback(RollTransitionEffect.OnIsTopToBottomChange)));
        public static readonly DependencyProperty LightIntensityProperty = DependencyProperty.Register("LightIntensity", typeof(double), typeof(RollTransitionEffect), new PropertyMetadata(0.5, ShaderEffect.PixelShaderConstantCallback(2)));
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(System.Windows.Controls.Orientation), typeof(RollTransitionEffect), new PropertyMetadata(System.Windows.Controls.Orientation.Vertical, new PropertyChangedCallback(RollTransitionEffect.OnOrientationChange)));
        public static readonly DependencyProperty RelativeRollWidthProperty = DependencyProperty.Register("RelativeRollWidth", typeof(double), typeof(RollTransitionEffect), new PropertyMetadata(0.5, ShaderEffect.PixelShaderConstantCallback(3)));
        public static readonly DependencyProperty RollSizeProperty = DependencyProperty.Register("RollSize", typeof(double), typeof(RollTransitionEffect), new PropertyMetadata(0.15, ShaderEffect.PixelShaderConstantCallback(1)));
        private static readonly DependencyProperty TopToBottomProperty = DependencyProperty.Register("TopToBottom", typeof(double), typeof(RollTransitionEffect), new PropertyMetadata(1.0, ShaderEffect.PixelShaderConstantCallback(6)));
        private static readonly DependencyProperty TransitionDirectionProperty = DependencyProperty.Register("TransitionDirection", typeof(double), typeof(RollTransitionEffect), new PropertyMetadata(1.0, ShaderEffect.PixelShaderConstantCallback(5)));
        private static readonly DependencyProperty VerticalProperty = DependencyProperty.Register("Vertical", typeof(double), typeof(RollTransitionEffect), new PropertyMetadata(1.0, ShaderEffect.PixelShaderConstantCallback(7)));

        public RollTransitionEffect()
        {
            base.UpdateShaderValue(RollSizeProperty);
            base.UpdateShaderValue(LightIntensityProperty);
            base.UpdateShaderValue(RelativeRollWidthProperty);
            base.UpdateShaderValue(InterpolationFunctionProperty);
            base.UpdateShaderValue(TransitionDirectionProperty);
            base.UpdateShaderValue(TopToBottomProperty);
            base.UpdateShaderValue(VerticalProperty);
            this.Update();
        }

        private static void OnIsRollOutChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as RollTransitionEffect).Update();
        }

        private static void OnIsTopToBottomChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as RollTransitionEffect).Update();
        }

        private static void OnOrientationChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as RollTransitionEffect).Update();
        }

        private void Update()
        {
            base.SetValue(TransitionDirectionProperty, this.IsRollOut ? -1.0 : 1.0);
            base.SetValue(TopToBottomProperty, this.IsTopToBottom ? 1.0 : -11.0);
            base.SetValue(VerticalProperty, (this.Orientation == System.Windows.Controls.Orientation.Vertical) ? 1.0 : -1.0);
        }

        public double InterpolationFunction
        {
            get
            {
                return (double) base.GetValue(InterpolationFunctionProperty);
            }
            set
            {
                base.SetValue(InterpolationFunctionProperty, value);
            }
        }

        public bool IsRollOut
        {
            get
            {
                return (bool) base.GetValue(IsRollOutProperty);
            }
            set
            {
                base.SetValue(IsRollOutProperty, value);
            }
        }

        public bool IsTopToBottom
        {
            get
            {
                return (bool) base.GetValue(IsTopToBottomProperty);
            }
            set
            {
                base.SetValue(IsTopToBottomProperty, value);
            }
        }

        public double LightIntensity
        {
            get
            {
                return (double) base.GetValue(LightIntensityProperty);
            }
            set
            {
                base.SetValue(LightIntensityProperty, value);
            }
        }

        public System.Windows.Controls.Orientation Orientation
        {
            get
            {
                return (System.Windows.Controls.Orientation) base.GetValue(OrientationProperty);
            }
            set
            {
                base.SetValue(OrientationProperty, value);
            }
        }

        public double RelativeRollWidth
        {
            get
            {
                return (double) base.GetValue(RelativeRollWidthProperty);
            }
            set
            {
                base.SetValue(RelativeRollWidthProperty, value);
            }
        }

        public double RollSize
        {
            get
            {
                return (double) base.GetValue(RollSizeProperty);
            }
            set
            {
                base.SetValue(RollSizeProperty, value);
            }
        }
    }
}

