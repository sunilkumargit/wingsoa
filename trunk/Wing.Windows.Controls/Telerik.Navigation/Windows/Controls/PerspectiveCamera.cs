namespace Telerik.Windows.Controls
{
    using System;
    using System.Windows.Media.Media3D;

    internal class PerspectiveCamera : Camera
    {
        public PerspectiveCamera(Point3D pointOfView, Point3D lookAtPoint, Point3D worldUpVector, double fov) : base(CreateMatrix(pointOfView, lookAtPoint, worldUpVector), fov)
        {
        }

        public static PerspectiveCamera CreateLookAtCenterPerspectiveCamera(double distance, double fov)
        {
            return new PerspectiveCamera(new Point3D(0.0, 0.0, -distance), new Point3D(0.0, 0.0, 0.0), new Point3D(0.0, 1.0, 0.0), fov);
        }

        private static Matrix3D CreateMatrix(Point3D pointOfView, Point3D lookAtPoint, Point3D worldUpVector)
        {
            Matrix3D result = Matrix3D.Identity;
            result.OffsetX = -pointOfView.XPosition;
            result.OffsetY = -pointOfView.YPosition;
            result.OffsetZ = -pointOfView.ZPosition;
            double a = (lookAtPoint.XPosition + worldUpVector.ZPosition) * 0.0;
            result.OffsetX += a;
            return result;
        }
    }
}

