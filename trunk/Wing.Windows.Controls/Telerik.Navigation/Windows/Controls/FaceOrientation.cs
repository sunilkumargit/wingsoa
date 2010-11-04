namespace Telerik.Windows.Controls
{
    using System;
    using System.Runtime.CompilerServices;

    internal class FaceOrientation
    {
        public FaceOrientation(Face reg, Point3D axis, double faceRotate, double cornerRotate)
        {
            this.Reg = reg;
            this.Axis = axis;
            this.FaceRotate = faceRotate;
            this.CornerRotate = cornerRotate;
        }

        public Point3D Axis { get; set; }

        public double CornerRotate { get; set; }

        public double FaceRotate { get; set; }

        public Face Reg { get; set; }
    }
}

