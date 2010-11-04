namespace Telerik.Windows.Controls.TransitionEffects
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Windows.Controls;
    using System.Windows.Media.Effects;
    using Telerik.Windows.Controls.TransitionControl;

    public class RollTransition : TransitionProvider
    {
        public RollTransition()
        {
            this.RollSize = 0.15;
            this.LightIntensity = 0.5;
            this.RelativeRollWidth = 0.3;
            this.InterpolationFunction = 2.0;
            this.IsTopToBottom = true;
            this.Orientation = System.Windows.Controls.Orientation.Vertical;
        }

        protected override ShaderEffect CreateTransitionEffect()
        {
            return new RollTransitionEffect { InterpolationFunction = this.InterpolationFunction, IsRollOut = this.IsRollOut, IsTopToBottom = this.IsTopToBottom, LightIntensity = this.LightIntensity, Orientation = this.Orientation, RelativeRollWidth = this.RelativeRollWidth, RollSize = this.RollSize };
        }

        public double InterpolationFunction { get; set; }

        public bool IsRollOut { get; set; }

        public bool IsTopToBottom { get; set; }

        public double LightIntensity { get; set; }

        public System.Windows.Controls.Orientation Orientation { get; set; }

        public double RelativeRollWidth { get; set; }

        public double RollSize { get; set; }
    }
}

