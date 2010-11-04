namespace Telerik.Windows.Controls
{
    using System;
    using System.Collections.ObjectModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    internal static class ThreeD
    {
        public static double Distance(Point3D pt1, Point3D pt2)
        {
            double dy = pt2.YPosition - pt1.YPosition;
            double dx = pt2.XPosition - pt1.XPosition;
            return Math.Sqrt((dy * dy) + (dx * dx));
        }

        public static double GetAngle(Point3D vector1, Point3D vector2)
        {
            double a1 = vector1.XPosition;
            double b1 = vector1.YPosition;
            double c1 = vector1.ZPosition;
            double a2 = vector2.XPosition;
            double b2 = vector2.YPosition;
            double c2 = vector2.ZPosition;
            double dotP = ((a1 * a2) + (b1 * b2)) + (c1 * c2);
            double cos = dotP / (GetLength(vector1) * GetLength(vector2));
            double sin2 = 1.0 - (cos * cos);
            if (sin2 < 0.0)
            {
                sin2 = 0.0;
            }
            return Math.Atan2(Math.Sqrt(sin2), cos);
        }

        public static double GetLength(Point3D vector1)
        {
            double a1 = vector1.XPosition;
            double b1 = vector1.YPosition;
            double c1 = vector1.ZPosition;
            return Math.Sqrt(((a1 * a1) + (b1 * b1)) + (c1 * c1));
        }

        public static Point3D GetMiddlePoint(Point3D point1, Point3D point2)
        {
            double a1 = point1.XPosition;
            double b1 = point1.YPosition;
            double c1 = point1.ZPosition;
            double a2 = point2.XPosition;
            double b2 = point2.YPosition;
            double c2 = point2.ZPosition;
            return new Point3D((a1 + a2) / 2.0, (b1 + b2) / 2.0, (c1 + c2) / 2.0);
        }

        public static double GetRotateAngle(Point3D axis, Point3D vector1, Point3D vector2)
        {
            double angle = GetAngle(vector1, vector2);
            Point3D test = new Point3D(vector1.XPosition, vector1.YPosition, vector1.ZPosition);
            if (IsZero(GetAngle(Rotate3D(test, axis, angle), vector2)))
            {
                return angle;
            }
            test = new Point3D(vector1.XPosition, vector1.YPosition, vector1.ZPosition);
            if (IsZero(GetAngle(Rotate3D(test, axis, -angle), vector2)))
            {
                return -angle;
            }
            return 0.0;
        }

        public static Point3D GetRotateAxis(Point3D vector1, Point3D vector2)
        {
            double y;
            double z;
            double a1 = vector1.XPosition;
            double b1 = vector1.YPosition;
            double c1 = vector1.ZPosition;
            double a2 = vector2.XPosition;
            double b2 = vector2.YPosition;
            double c2 = vector2.ZPosition;
            if (!IsZero((a1 * b2) - (b1 * a2)))
            {
                z = 1.0;
                y = ((c1 * a2) - (c2 * a1)) / ((a1 * b2) - (a2 * b1));
                double x = ((c1 * b2) - (c2 * b1)) / ((b1 * a2) - (b2 * a1));
                return new Point3D(x, y, z);
            }
            z = 0.0;
            if (!IsZero(a1))
            {
                y = 1.0;
                return new Point3D(-b1 / a1, y, z);
            }
            y = 0.0;
            return new Point3D(1.0, y, z);
        }

        public static bool IsZero(double number)
        {
            return (Math.Abs(number) < 1E-05);
        }

        public static Point Rotate2D(double positionX, double positionY, double positionAngle)
        {
            double sin = Math.Sin(positionAngle);
            double cos = Math.Cos(positionAngle);
            return new Point((positionX * cos) - (positionY * sin), (positionX * sin) + (positionY * cos));
        }

        public static Point3D Rotate3D(Point3D newPoint, Point3D axis, double positionAngle)
        {
            Point pt;
            Point3D verticalAxis = new Point3D(axis.XPosition, axis.YPosition, axis.ZPosition);
            double horizontalRotation = Math.Atan2(verticalAxis.YPosition, verticalAxis.ZPosition);
            if (horizontalRotation != 0.0)
            {
                pt = Rotate2D(verticalAxis.ZPosition, verticalAxis.YPosition, -horizontalRotation);
                verticalAxis.ZPosition = pt.X;
                verticalAxis.YPosition = pt.Y;
                pt = Rotate2D(newPoint.ZPosition, newPoint.YPosition, -horizontalRotation);
                newPoint.ZPosition = pt.X;
                newPoint.YPosition = pt.Y;
            }
            double verticalRotation = Math.Atan2(verticalAxis.ZPosition, verticalAxis.XPosition);
            if (verticalRotation != 0.0)
            {
                pt = Rotate2D(newPoint.XPosition, newPoint.ZPosition, -verticalRotation);
                newPoint.XPosition = pt.X;
                newPoint.ZPosition = pt.Y;
            }
            pt = Rotate2D(newPoint.ZPosition, newPoint.YPosition, positionAngle);
            newPoint.ZPosition = pt.X;
            newPoint.YPosition = pt.Y;
            if (verticalRotation != 0.0)
            {
                pt = Rotate2D(newPoint.XPosition, newPoint.ZPosition, verticalRotation);
                newPoint.XPosition = pt.X;
                newPoint.ZPosition = pt.Y;
            }
            if (horizontalRotation != 0.0)
            {
                pt = Rotate2D(newPoint.ZPosition, newPoint.YPosition, horizontalRotation);
                newPoint.ZPosition = pt.X;
                newPoint.YPosition = pt.Y;
            }
            return newPoint;
        }

        public static Collection<Point3D> Rotate3DArray(Collection<Point3D> points, Point3D axis, double positionAngle)
        {
            Collection<Point3D> newPoints = new Collection<Point3D>();
            Point3D verticalAxis = new Point3D(axis.XPosition, axis.YPosition, axis.ZPosition);
            double horizontalRotation = Math.Atan2(verticalAxis.YPosition, verticalAxis.ZPosition);
            double sinX = Math.Sin(-horizontalRotation);
            double cosX = Math.Cos(-horizontalRotation);
            Point3D pt = new Point3D(verticalAxis.XPosition, (verticalAxis.ZPosition * sinX) + (verticalAxis.YPosition * cosX), (verticalAxis.ZPosition * cosX) - (verticalAxis.YPosition * sinX));
            verticalAxis = pt;
            double verticalRotation = Math.Atan2(verticalAxis.ZPosition, verticalAxis.XPosition);
            double sinY = Math.Sin(-verticalRotation);
            double cosY = Math.Cos(-verticalRotation);
            double sinA = Math.Sin(positionAngle);
            double cosA = Math.Cos(positionAngle);
            for (int i = 0; i < points.Count; i++)
            {
                Point3D point = points[i];
                pt = new Point3D((point.ZPosition * cosX) - (point.YPosition * sinX), (point.ZPosition * sinX) + (point.YPosition * cosX), 0.0);
                point.ZPosition = pt.XPosition;
                point.YPosition = pt.YPosition;
                pt = new Point3D((point.XPosition * cosY) - (point.ZPosition * sinY), (point.XPosition * sinY) + (point.ZPosition * cosY), 0.0);
                point.XPosition = pt.XPosition;
                point.ZPosition = pt.YPosition;
                pt = new Point3D((point.ZPosition * cosA) - (point.YPosition * sinA), (point.ZPosition * sinA) + (point.YPosition * cosA), 0.0);
                point.ZPosition = pt.XPosition;
                point.YPosition = pt.YPosition;
                pt = new Point3D((point.XPosition * cosY) - (point.ZPosition * -sinY), (point.XPosition * -sinY) + (point.ZPosition * cosY), 0.0);
                point.XPosition = pt.XPosition;
                point.ZPosition = pt.YPosition;
                pt = new Point3D((point.ZPosition * cosX) - (point.YPosition * -sinX), (point.ZPosition * -sinX) + (point.YPosition * cosX), 0.0);
                point.ZPosition = pt.XPosition;
                point.YPosition = pt.YPosition;
                newPoints.Add(point);
            }
            return newPoints;
        }

        public static void Skew(FrameworkElement parent, FrameworkElement child, double mcW, double mcH, Point3D pointTL, Point3D pointBL, Point3D pointTR)
        {
            double angleP2 = Math.Atan2(pointTR.YPosition - pointTL.YPosition, pointTR.XPosition - pointTL.XPosition);
            double angleP1 = Math.Atan2(pointBL.YPosition - pointTL.YPosition, pointBL.XPosition - pointTL.XPosition);
            double angle = (angleP1 - angleP2) / 2.0;
            double arm = 1.0 / (Math.Sqrt(2.0) * Math.Cos(angle));
            ScaleTransform parentScale = parent.FindName(parent.Name + "ScaleTransform") as ScaleTransform;
            RotateTransform parentRotate = parent.FindName(parent.Name + "RotateTransform") as RotateTransform;
            ScaleTransform childScale = parent.FindName(child.Name + "ScaleTransform") as ScaleTransform;
            RotateTransform childRotate = parent.FindName(child.Name + "RotateTransform") as RotateTransform;
            parent.SetValue(Canvas.LeftProperty, pointTL.XPosition);
            parent.SetValue(Canvas.TopProperty, pointTL.YPosition);
            parentScale.SetValue(ScaleTransform.ScaleXProperty, 1.0);
            parentRotate.SetValue(RotateTransform.AngleProperty, 57.295779513082323 * (angleP1 - angle));
            childRotate.SetValue(RotateTransform.AngleProperty, -45.0);
            parentScale.SetValue(ScaleTransform.ScaleYProperty, Math.Tan(angle));
            childScale.SetValue(ScaleTransform.ScaleXProperty, (Distance(pointTR, pointTL) / arm) / mcW);
            childScale.SetValue(ScaleTransform.ScaleYProperty, (Distance(pointBL, pointTL) / arm) / mcH);
        }

        public static Point3D Vector(Point3D proxyPoint, Point3D distancePoint)
        {
            double x = distancePoint.XPosition - proxyPoint.XPosition;
            double y = distancePoint.YPosition - proxyPoint.YPosition;
            return new Point3D(x, y, distancePoint.ZPosition - proxyPoint.ZPosition);
        }
    }
}

