namespace Telerik.Windows.Controls.TransitionEffects
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Media.Effects;
    using Telerik.Windows.Controls.TransitionControl;

    public abstract class BaseTransitionEffect : TransitionEffect
    {
        private static readonly DependencyProperty ShaderProgressProperty = DependencyProperty.Register("ShaderProgress", typeof(double), typeof(TransitionEffect), new PropertyMetadata(1.0, ShaderEffect.PixelShaderConstantCallback(0)));

        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        protected BaseTransitionEffect()
        {
            base.UpdateShaderValue(ShaderProgressProperty);
        }

        protected override PixelShader LoadShader()
        {
            return new PixelShader { UriSource = TransitionEffect.PackUri<BaseTransitionEffect>("TransitionControl/TransitionEffects/Effects/" + base.GetType().Name + ".ps") };
        }

        protected override void OnProgressChanged(double oldProgress, double newProgress)
        {
            this.ShaderProgress = newProgress;
        }

        protected double ShaderProgress
        {
            get
            {
                return (double) base.GetValue(ShaderProgressProperty);
            }
            set
            {
                base.SetValue(ShaderProgressProperty, value);
            }
        }
    }
}

