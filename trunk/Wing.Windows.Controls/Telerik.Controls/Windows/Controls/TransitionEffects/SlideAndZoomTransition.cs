namespace Telerik.Windows.Controls.TransitionEffects
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Media.Effects;
    using Telerik.Windows.Controls.TransitionControl;

    public class SlideAndZoomTransition : TransitionProvider
    {
        public SlideAndZoomTransition()
        {
            this.MinZoom = 0.9;
            this.StartSlideAt = 0.25;
            this.MinAlpha = 0.1;
        }

        protected override Transition CreateTransition()
        {
            return new SlideAndZoomCLRTransition(this);
        }

        protected override ShaderEffect CreateTransitionEffect()
        {
            return new SlideAndZoomTransitionEffect { MinZoom = this.MinZoom, StartSlideAt = this.StartSlideAt, SlideDirection = this.SlideDirection, MinAlpha = this.MinAlpha };
        }

        public double MinAlpha { get; set; }

        public double MinZoom { get; set; }

        public FlowDirection SlideDirection { get; set; }

        public double StartSlideAt { get; set; }
    }
}

