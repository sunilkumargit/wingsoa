namespace Telerik.Windows.Controls
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Media.Media3D;

    internal abstract class Camera
    {
        public Camera(Matrix3D transformation, double fov)
        {
            this.Transformation = transformation;
            this.Fov = fov;
            this.Scale = 1.0;
        }

        public Point Project(Point3D point)
        {
            return ThreeDTools.Project(ThreeDTools.Transform(point, this.Transformation), this.Scale, this.Fov);
        }

        public Point[] Project(Point3D[] points)
        {
            return ThreeDTools.Project(ThreeDTools.Transform(points, this.Transformation), this.Scale, this.Fov);
        }

        public double Fov { get; set; }

        public double Scale { get; set; }

        public Matrix3D Transformation { get; set; }
    }
}

