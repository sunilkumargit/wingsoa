namespace Telerik.Windows.Controls.TransitionEffects
{
    using System;
    using System.Windows;
    using System.Windows.Media.Effects;
    using Telerik.Windows.Controls.TransitionControl;

    internal class SlideAndZoomTransitionEffect : TransitionEffect
    {
        private static readonly DependencyProperty Alpha1Property = DependencyProperty.Register("Alpha1", typeof(double), typeof(SlideAndZoomTransitionEffect), new PropertyMetadata(0.0, ShaderEffect.PixelShaderConstantCallback(6)));
        private static readonly DependencyProperty Alpha2Property = DependencyProperty.Register("Alpha2", typeof(double), typeof(SlideAndZoomTransitionEffect), new PropertyMetadata(0.0, ShaderEffect.PixelShaderConstantCallback(7)));
        public static readonly DependencyProperty MinAlphaProperty = DependencyProperty.Register("MinAlpha", typeof(double), typeof(SlideAndZoomTransitionEffect), new PropertyMetadata(0.1));
        public static readonly DependencyProperty MinZoomProperty = DependencyProperty.Register("MinZoom", typeof(double), typeof(SlideAndZoomTransitionEffect), new PropertyMetadata(0.8));
        public static readonly DependencyProperty SlideDirectionProperty = DependencyProperty.Register("SlideDirection", typeof(FlowDirection), typeof(SlideAndZoomTransitionEffect), new PropertyMetadata(FlowDirection.RightToLeft));
        public static readonly DependencyProperty StartSlideAtProperty = DependencyProperty.Register("StartSlideAt", typeof(double), typeof(SlideAndZoomTransitionEffect), new PropertyMetadata(0.07));
        private static readonly DependencyProperty XOffset1Property = DependencyProperty.Register("XOffset1", typeof(double), typeof(SlideAndZoomTransitionEffect), new PropertyMetadata(0.0, ShaderEffect.PixelShaderConstantCallback(0)));
        private static readonly DependencyProperty XOffset2Property = DependencyProperty.Register("XOffset2", typeof(double), typeof(SlideAndZoomTransitionEffect), new PropertyMetadata(0.0, ShaderEffect.PixelShaderConstantCallback(2)));
        private static readonly DependencyProperty YOffset1Property = DependencyProperty.Register("YOffset1", typeof(double), typeof(SlideAndZoomTransitionEffect), new PropertyMetadata(0.0, ShaderEffect.PixelShaderConstantCallback(1)));
        private static readonly DependencyProperty YOffset2Property = DependencyProperty.Register("YOffset2", typeof(double), typeof(SlideAndZoomTransitionEffect), new PropertyMetadata(0.0, ShaderEffect.PixelShaderConstantCallback(3)));
        private static readonly DependencyProperty Zoom1Property = DependencyProperty.Register("Zoom1", typeof(double), typeof(SlideAndZoomTransitionEffect), new PropertyMetadata(0.0, ShaderEffect.PixelShaderConstantCallback(4)));
        private static readonly DependencyProperty Zoom2Property = DependencyProperty.Register("Zoom2", typeof(double), typeof(SlideAndZoomTransitionEffect), new PropertyMetadata(0.0, ShaderEffect.PixelShaderConstantCallback(5)));

        public SlideAndZoomTransitionEffect()
        {
            base.UpdateShaderValue(XOffset1Property);
            base.UpdateShaderValue(YOffset1Property);
            base.UpdateShaderValue(Zoom1Property);
            base.UpdateShaderValue(Alpha1Property);
            base.UpdateShaderValue(XOffset2Property);
            base.UpdateShaderValue(YOffset2Property);
            base.UpdateShaderValue(Alpha2Property);
        }

        protected override PixelShader LoadShader()
        {
            return new PixelShader { UriSource = TransitionEffect.PackUri<BaseTransitionEffect>("TransitionControl/TransitionEffects/Effects/SlideAndZoomTransitionEffect.ps") };
        }

        protected override void OnProgressChanged(double oldProgress, double newProgress)
        {
            double directionMultiplier = (this.SlideDirection == FlowDirection.RightToLeft) ? ((double) 1) : ((double) (-1));
            double minAlpha = this.MinAlpha;
            double minZoom = this.MinZoom;
            double timeBeforeSlide = this.StartSlideAt;
            double timeForSlide = 1.0 - (timeBeforeSlide * 2.0);
            double zoomProgress = 0.0;
            double slideProgress = 0.0;
            if (newProgress < timeBeforeSlide)
            {
                zoomProgress = newProgress / timeBeforeSlide;
            }
            else if (newProgress <= (timeBeforeSlide + timeForSlide))
            {
                zoomProgress = 1.0;
                slideProgress = (newProgress - timeBeforeSlide) / timeForSlide;
            }
            else
            {
                zoomProgress = (1.0 - newProgress) / timeBeforeSlide;
                slideProgress = 1.0;
            }
            this.Alpha1 = (slideProgress == 0.0) ? (1.0 - ((1.0 - minAlpha) * (1.0 - zoomProgress))) : 1.0;
            this.Zoom1 = 1.0 - ((1.0 - minZoom) * zoomProgress);
            double slide = slideProgress * this.Zoom1;
            this.YOffset1 = (1.0 - this.Zoom1) / 2.0;
            this.XOffset1 = ((1.0 - this.Zoom1) / 2.0) + ((1.0 - slide) * directionMultiplier);
            this.Alpha2 = 1.0 - ((1.0 - minAlpha) * slideProgress);
            this.Zoom2 = 1.0 - ((1.0 - minZoom) * zoomProgress);
            this.YOffset2 = (1.0 - this.Zoom2) / 2.0;
            this.XOffset2 = ((1.0 - this.Zoom2) / 2.0) - (slide * directionMultiplier);
            this.XOffset1 -= ((1.0 - this.Zoom1) / 2.0) + ((1.0 - this.Zoom2) / 2.0);
        }

        protected double Alpha1
        {
            get
            {
                return (double) base.GetValue(Alpha1Property);
            }
            set
            {
                base.SetValue(Alpha1Property, value);
            }
        }

        protected double Alpha2
        {
            get
            {
                return (double) base.GetValue(Alpha2Property);
            }
            set
            {
                base.SetValue(Alpha2Property, value);
            }
        }

        public double MinAlpha
        {
            get
            {
                return (double) base.GetValue(MinAlphaProperty);
            }
            set
            {
                base.SetValue(MinAlphaProperty, value);
            }
        }

        public double MinZoom
        {
            get
            {
                return (double) base.GetValue(MinZoomProperty);
            }
            set
            {
                base.SetValue(MinZoomProperty, value);
            }
        }

        public FlowDirection SlideDirection
        {
            get
            {
                return (FlowDirection) base.GetValue(SlideDirectionProperty);
            }
            set
            {
                base.SetValue(SlideDirectionProperty, value);
            }
        }

        public double StartSlideAt
        {
            get
            {
                return (double) base.GetValue(StartSlideAtProperty);
            }
            set
            {
                base.SetValue(StartSlideAtProperty, value);
            }
        }

        protected double XOffset1
        {
            get
            {
                return (double) base.GetValue(XOffset1Property);
            }
            set
            {
                base.SetValue(XOffset1Property, value);
            }
        }

        protected double XOffset2
        {
            get
            {
                return (double) base.GetValue(XOffset2Property);
            }
            set
            {
                base.SetValue(XOffset2Property, value);
            }
        }

        protected double YOffset1
        {
            get
            {
                return (double) base.GetValue(YOffset1Property);
            }
            set
            {
                base.SetValue(YOffset1Property, value);
            }
        }

        protected double YOffset2
        {
            get
            {
                return (double) base.GetValue(YOffset2Property);
            }
            set
            {
                base.SetValue(YOffset2Property, value);
            }
        }

        protected double Zoom1
        {
            get
            {
                return (double) base.GetValue(Zoom1Property);
            }
            set
            {
                base.SetValue(Zoom1Property, value);
            }
        }

        protected double Zoom2
        {
            get
            {
                return (double) base.GetValue(Zoom2Property);
            }
            set
            {
                base.SetValue(Zoom2Property, value);
            }
        }
    }
}

