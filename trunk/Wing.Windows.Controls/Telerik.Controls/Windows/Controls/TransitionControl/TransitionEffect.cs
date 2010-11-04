namespace Telerik.Windows.Controls.TransitionControl
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Text;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Effects;

    public abstract class TransitionEffect : ShaderEffect
    {
        public static readonly DependencyProperty CurrentSamplerProperty = ShaderEffect.RegisterPixelShaderSamplerProperty("CurrentSampler", typeof(TransitionEffect), 0, SamplingMode.NearestNeighbor);
        public static readonly DependencyProperty OldSamplerProperty = ShaderEffect.RegisterPixelShaderSamplerProperty("OldSampler", typeof(TransitionEffect), 1, SamplingMode.NearestNeighbor);
        public static readonly DependencyProperty ProgressProperty = DependencyProperty.Register("Progress", typeof(double), typeof(TransitionEffect), new PropertyMetadata(0.0, new PropertyChangedCallback(TransitionEffect.OnProgressPropertyChange)));

        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        protected TransitionEffect()
        {
            base.PixelShader = this.LoadShader();
            base.UpdateShaderValue(CurrentSamplerProperty);
            base.UpdateShaderValue(OldSamplerProperty);
            this.OnProgressChanged(double.NegativeInfinity, 1.0);
        }

        private static string GetAssemblyShortName(Type type)
        {
            return type.Assembly.ToString().Split(new char[] { ',' })[0];
        }

        protected abstract PixelShader LoadShader();
        protected abstract void OnProgressChanged(double oldProgress, double newProgress);
        private static void OnProgressPropertyChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as TransitionEffect).OnProgressChanged((double) e.OldValue, (double) e.NewValue);
        }

        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        protected static Uri PackUri<T>(string relativeFile)
        {
            return PackUri(typeof(T), relativeFile);
        }

        private static Uri PackUri(Type type, string relativeFile)
        {
            StringBuilder uriString = new StringBuilder();
            uriString.Append("/");
            uriString.Append(GetAssemblyShortName(type));
            uriString.Append(";component/");
            uriString.Append(relativeFile);
            return new Uri(uriString.ToString(), UriKind.RelativeOrAbsolute);
        }

        [Browsable(false)]
        public Brush CurrentSampler
        {
            get
            {
                return (Brush) base.GetValue(CurrentSamplerProperty);
            }
            set
            {
                base.SetValue(CurrentSamplerProperty, value);
            }
        }

        [Browsable(false)]
        public Brush OldSampler
        {
            get
            {
                return (Brush) base.GetValue(OldSamplerProperty);
            }
            set
            {
                base.SetValue(OldSamplerProperty, value);
            }
        }

        public double Progress
        {
            get
            {
                return (double) base.GetValue(ProgressProperty);
            }
            set
            {
                base.SetValue(ProgressProperty, value);
            }
        }
    }
}

