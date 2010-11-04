namespace Telerik.Windows.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media.Media3D;

    internal static class ThreeDTools
    {
        private static Dictionary<double, double> cosHash = CreateCossDictionary();
        private static Dictionary<double, double> sinHash = CreateSinsDictionary();

        private static double Cos(double alph)
        {
            return Math.Cos(Normalize(alph));
        }

        private static Dictionary<double, double> CreateCossDictionary()
        {
            Dictionary<double, double> result = new Dictionary<double, double>(360);
            double angle = 0.0;
            double step = 0.017453292519943295;
            for (int i = 0; i < 360; i++)
            {
                result.Add((double) i, Math.Cos(angle));
                angle += step;
            }
            return result;
        }

        public static Matrix3D CreateRotationMatrix(double alph, double bet, double gam)
        {
            double a = Floor(alph);
            double b = Floor(bet);
            double c = Floor(gam);
            return new Matrix3D { M11 = cosHash[b] * cosHash[c], M12 = cosHash[b] * sinHash[c], M13 = -sinHash[b], OffsetX = 0.0, M21 = ((cosHash[c] * sinHash[a]) * sinHash[b]) - (cosHash[a] * sinHash[c]), M22 = (cosHash[a] * cosHash[c]) + ((sinHash[c] * sinHash[a]) * sinHash[b]), M23 = cosHash[b] * sinHash[a], OffsetY = 0.0, M31 = ((cosHash[c] * cosHash[a]) * sinHash[b]) + (sinHash[a] * sinHash[c]), M32 = ((sinHash[c] * cosHash[a]) * sinHash[b]) - (cosHash[c] * sinHash[a]), M33 = cosHash[b] * cosHash[a], OffsetZ = 0.0 };
        }

        private static Dictionary<double, double> CreateSinsDictionary()
        {
            Dictionary<double, double> result = new Dictionary<double, double>(360);
            double angle = 0.0;
            double step = 0.017453292519943295;
            for (int i = 0; i < 360; i++)
            {
                result.Add((double) i, Math.Sin(angle));
                angle += step;
            }
            return result;
        }

        public static Matrix3D CreateTranslateMatrix(double translateX, double translateY, double translateZ)
        {
            Matrix3D result = Matrix3D.Identity;
            result.OffsetX = translateX;
            result.OffsetY = translateY;
            result.OffsetZ = translateZ;
            return result;
        }

        public static Matrix3D CreateXRotationMatrix(double alph)
        {
            return new Matrix3D { M11 = 1.0, M12 = 0.0, M13 = 0.0, OffsetX = 0.0, M21 = 0.0, M22 = Cos(alph), M23 = Sin(alph), OffsetY = 0.0, M31 = 0.0, M32 = -Sin(alph), M33 = Cos(alph), OffsetZ = 0.0 };
        }

        public static Matrix3D CreateYRotationMatrix(double alph)
        {
            return new Matrix3D { M11 = Cos(alph), M12 = 0.0, M13 = -Sin(alph), OffsetX = 0.0, M21 = 0.0, M22 = 1.0, M23 = 0.0, OffsetY = 0.0, M31 = Sin(alph), M32 = 0.0, M33 = Cos(alph), OffsetZ = 0.0 };
        }

        public static Matrix3D CreateZRotationMatrix(double alph)
        {
            return new Matrix3D { M11 = Cos(alph), M12 = Sin(alph), M13 = 0.0, OffsetX = 0.0, M21 = -Sin(alph), M22 = Cos(alph), M23 = 0.0, OffsetY = 0.0, M31 = 0.0, M32 = 0.0, M33 = 1.0, OffsetZ = 0.0 };
        }

        private static int Floor(double a)
        {
            return (int) Math.Floor(a);
        }

        private static double Normalize(double val)
        {
            int v = ((int) Math.Round(val, 0)) % 360;
            return ((((v < 0) ? ((double) (360 + v)) : ((double) v)) * 3.1415926535897931) / 180.0);
        }

        public static Point Project(Point3D point, double scale, double fov)
        {
            double pointZ = (point.ZPosition == 0.0) ? 1.0 : point.ZPosition;
            return new Point(((scale * fov) * point.XPosition) / pointZ, ((scale * fov) * point.YPosition) / pointZ);
        }

        public static Point[] Project(Point3D[] points, double scale, double fov)
        {
            Point[] result = new Point[points.Length];
            int i = 0;
            foreach (Point3D point in points)
            {
                result[i++] = Project(point, scale, fov);
            }
            return result;
        }

        private static double Sin(double alph)
        {
            return Math.Sin(Normalize(alph));
        }

        public static Point3D[] Transform(Point3D[] points, Matrix3D matrix)
        {
            Point3D[] newPoints = new Point3D[points.Length];
            int i = 0;
            foreach (Point3D point in points)
            {
                newPoints[i++] = Transform(point, matrix);
            }
            return newPoints;
        }

        public static Point3D Transform(Point3D point, Matrix3D matrix)
        {
            return new Point3D((((point.XPosition * matrix.M11) + (point.YPosition * matrix.M12)) + (point.ZPosition * matrix.M13)) + matrix.OffsetX, (((point.XPosition * matrix.M21) + (point.YPosition * matrix.M22)) + (point.ZPosition * matrix.M23)) + matrix.OffsetY, (((point.XPosition * matrix.M31) + (point.YPosition * matrix.M32)) + (point.ZPosition * matrix.M33)) + matrix.OffsetZ);
        }
    }
}

