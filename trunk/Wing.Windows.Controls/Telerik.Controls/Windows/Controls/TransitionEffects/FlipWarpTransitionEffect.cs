namespace Telerik.Windows.Controls.TransitionEffects
{
    using System;
    using System.Windows;
    using System.Windows.Media.Effects;
    using Telerik.Windows.Controls.TransitionControl;

    internal class FlipWarpTransitionEffect : TransitionEffect
    {
        private static readonly DependencyProperty Left0Property = DependencyProperty.Register("Left0", typeof(double), typeof(FlipWarpTransitionEffect), new PropertyMetadata(0.0, ShaderEffect.PixelShaderConstantCallback(0)));
        private static readonly DependencyProperty Left1Property = DependencyProperty.Register("Left1", typeof(double), typeof(FlipWarpTransitionEffect), new PropertyMetadata(0.0, ShaderEffect.PixelShaderConstantCallback(1)));
        private static readonly DependencyProperty Left2Property = DependencyProperty.Register("Left2", typeof(double), typeof(FlipWarpTransitionEffect), new PropertyMetadata(0.0, ShaderEffect.PixelShaderConstantCallback(2)));
        private static readonly DependencyProperty Left3Property = DependencyProperty.Register("Left3", typeof(double), typeof(FlipWarpTransitionEffect), new PropertyMetadata(0.0, ShaderEffect.PixelShaderConstantCallback(3)));
        private static readonly DependencyProperty Right0Property = DependencyProperty.Register("Right0", typeof(double), typeof(FlipWarpTransitionEffect), new PropertyMetadata(1.0, ShaderEffect.PixelShaderConstantCallback(4)));
        private static readonly DependencyProperty Right1Property = DependencyProperty.Register("Right1", typeof(double), typeof(FlipWarpTransitionEffect), new PropertyMetadata(1.0, ShaderEffect.PixelShaderConstantCallback(5)));
        private static readonly DependencyProperty Right2Property = DependencyProperty.Register("Right2", typeof(double), typeof(FlipWarpTransitionEffect), new PropertyMetadata(1.0, ShaderEffect.PixelShaderConstantCallback(6)));
        private static readonly DependencyProperty RightProperty = DependencyProperty.Register("Right3", typeof(double), typeof(FlipWarpTransitionEffect), new PropertyMetadata(1.0, ShaderEffect.PixelShaderConstantCallback(7)));

        public FlipWarpTransitionEffect()
        {
            base.UpdateShaderValue(Left0Property);
            base.UpdateShaderValue(Left1Property);
            base.UpdateShaderValue(Left2Property);
            base.UpdateShaderValue(Left3Property);
            base.UpdateShaderValue(Right0Property);
            base.UpdateShaderValue(Right1Property);
            base.UpdateShaderValue(Right2Property);
            base.UpdateShaderValue(RightProperty);
        }

        protected override PixelShader LoadShader()
        {
            return new PixelShader { UriSource = TransitionEffect.PackUri<BaseTransitionEffect>("TransitionControl/TransitionEffects/Effects/WarpTransitionEffect.ps") };
        }

        protected override void OnProgressChanged(double oldProgress, double newProgress)
        {
            double topProgress = Math.Max((double) 0.0, (double) ((newProgress * 2.0) - 1.0));
            double bottomProgress = Math.Min((double) 1.0, (double) (newProgress * 2.0));
            this.Left0 = this.Left1 = topProgress;
            this.Left2 = this.Left3 = bottomProgress;
            this.Right0 = this.Right1 = 1.0 - topProgress;
            this.Right2 = this.Right3 = 1.0 - bottomProgress;
        }

        protected double Left0
        {
            get
            {
                return (double) base.GetValue(Left0Property);
            }
            set
            {
                base.SetValue(Left0Property, value);
            }
        }

        protected double Left1
        {
            get
            {
                return (double) base.GetValue(Left1Property);
            }
            set
            {
                base.SetValue(Left1Property, value);
            }
        }

        protected double Left2
        {
            get
            {
                return (double) base.GetValue(Left2Property);
            }
            set
            {
                base.SetValue(Left2Property, value);
            }
        }

        protected double Left3
        {
            get
            {
                return (double) base.GetValue(Left3Property);
            }
            set
            {
                base.SetValue(Left3Property, value);
            }
        }

        protected double Right0
        {
            get
            {
                return (double) base.GetValue(Right0Property);
            }
            set
            {
                base.SetValue(Right0Property, value);
            }
        }

        protected double Right1
        {
            get
            {
                return (double) base.GetValue(Right1Property);
            }
            set
            {
                base.SetValue(Right1Property, value);
            }
        }

        protected double Right2
        {
            get
            {
                return (double) base.GetValue(Right2Property);
            }
            set
            {
                base.SetValue(Right2Property, value);
            }
        }

        protected double Right3
        {
            get
            {
                return (double) base.GetValue(RightProperty);
            }
            set
            {
                base.SetValue(RightProperty, value);
            }
        }
    }
}

