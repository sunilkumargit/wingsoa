namespace Telerik.Windows.Controls.TransitionEffects
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Media;
    using Telerik.Windows.Controls.Animation;
    using Telerik.Windows.Controls.TransitionControl;

    [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId="CLR")]
    public class SlideAndZoomCLRTransition : Transition
    {
        private double alpha1;
        private double alpha2;
        private SlideAndZoomTransition provider;
        private double xOffset1;
        private double xOffset2;
        private double yOffset1;
        private double yOffset2;
        private double zoom1;
        private double zoom2;

        public SlideAndZoomCLRTransition(SlideAndZoomTransition provider)
        {
            this.provider = provider;
        }

        protected override void OnCleanUp()
        {
            if (base.OldContentPresenter != null)
            {
                base.OldContentPresenter.Visibility = Visibility.Collapsed;
            }
        }

        protected override void OnInitialized()
        {
            if (base.CurrentContentPresenter != null)
            {
                base.CurrentContentPresenter.Visibility = Visibility.Visible;
                base.CurrentContentPresenter.EnsureDefaultTransforms();
            }
            if (base.OldContentPresenter != null)
            {
                base.OldContentPresenter.Visibility = Visibility.Visible;
                base.OldContentPresenter.EnsureDefaultTransforms();
            }
        }

        protected override void OnProgressChanged(double oldProgress, double newProgress)
        {
            double directionMultiplier = (this.provider.SlideDirection == FlowDirection.RightToLeft) ? ((double) 1) : ((double) (-1));
            double minAlpha = this.provider.MinAlpha;
            double minZoom = this.provider.MinZoom;
            double timeBeforeSlide = this.provider.StartSlideAt;
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

        private double Alpha1
        {
            get
            {
                return this.alpha1;
            }
            set
            {
                this.alpha1 = value;
                if (base.CurrentContentPresenter != null)
                {
                    base.CurrentContentPresenter.Opacity = this.alpha1;
                }
            }
        }

        private double Alpha2
        {
            get
            {
                return this.alpha2;
            }
            set
            {
                this.alpha2 = value;
                if (base.OldContentPresenter != null)
                {
                    base.OldContentPresenter.Opacity = this.alpha2;
                }
            }
        }

        private double XOffset1
        {
            get
            {
                return this.xOffset1;
            }
            set
            {
                this.xOffset1 = value;
                if (base.CurrentContentPresenter != null)
                {
                    TransformGroup group = base.CurrentContentPresenter.RenderTransform as TransformGroup;
                    TranslateTransform scale = group.Children[3] as TranslateTransform;
                    scale.X = this.xOffset1 * base.CurrentContentPresenter.ActualWidth;
                }
            }
        }

        private double XOffset2
        {
            get
            {
                return this.xOffset2;
            }
            set
            {
                this.xOffset2 = value;
                if (base.OldContentPresenter != null)
                {
                    TransformGroup group = base.OldContentPresenter.RenderTransform as TransformGroup;
                    TranslateTransform scale = group.Children[3] as TranslateTransform;
                    scale.X = this.xOffset2 * base.CurrentContentPresenter.ActualWidth;
                }
            }
        }

        private double YOffset1
        {
            get
            {
                return this.yOffset1;
            }
            set
            {
                this.yOffset1 = value;
                if (base.CurrentContentPresenter != null)
                {
                    TransformGroup group = base.CurrentContentPresenter.RenderTransform as TransformGroup;
                    TranslateTransform scale = group.Children[3] as TranslateTransform;
                    scale.Y = this.yOffset1 * base.CurrentContentPresenter.ActualHeight;
                }
            }
        }

        private double YOffset2
        {
            get
            {
                return this.yOffset2;
            }
            set
            {
                this.yOffset2 = value;
                if (base.OldContentPresenter != null)
                {
                    TransformGroup group = base.OldContentPresenter.RenderTransform as TransformGroup;
                    TranslateTransform scale = group.Children[3] as TranslateTransform;
                    scale.Y = this.yOffset2 * base.OldContentPresenter.ActualHeight;
                }
            }
        }

        private double Zoom1
        {
            get
            {
                return this.zoom1;
            }
            set
            {
                this.zoom1 = value;
                if (base.CurrentContentPresenter != null)
                {
                    TransformGroup group = base.CurrentContentPresenter.RenderTransform as TransformGroup;
                    ScaleTransform scale = group.Children[0] as ScaleTransform;
                    scale.ScaleX = scale.ScaleY = this.zoom1;
                }
            }
        }

        private double Zoom2
        {
            get
            {
                return this.zoom2;
            }
            set
            {
                this.zoom2 = value;
                if (base.OldContentPresenter != null)
                {
                    TransformGroup group = base.OldContentPresenter.RenderTransform as TransformGroup;
                    ScaleTransform scale = group.Children[0] as ScaleTransform;
                    scale.ScaleX = scale.ScaleY = this.zoom2;
                }
            }
        }
    }
}

