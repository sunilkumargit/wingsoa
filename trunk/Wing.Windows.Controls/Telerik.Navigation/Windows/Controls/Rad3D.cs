namespace Telerik.Windows.Controls
{
    using System;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    internal class Rad3D
    {
        internal static readonly DependencyProperty CameraDistanceProperty = DependencyProperty.RegisterAttached("CameraDistance", typeof(double), typeof(Rad3D), new PropertyMetadata(1000.0, new PropertyChangedCallback(Rad3D.OnChanged)));
        internal static readonly DependencyProperty CameraOffsetProperty = DependencyProperty.RegisterAttached("CameraOffset", typeof(Point3D), typeof(Rad3D), new PropertyMetadata(new Point3D(0.0, 0.0, 0.0), new PropertyChangedCallback(Rad3D.OnChanged)));
        internal static readonly DependencyProperty CameraRotationProperty = DependencyProperty.RegisterAttached("CameraRotation", typeof(double), typeof(Rad3D), new PropertyMetadata(0.0, new PropertyChangedCallback(Rad3D.OnChanged)));
        internal static readonly DependencyProperty RotationYProperty = DependencyProperty.RegisterAttached("RotationY", typeof(double), typeof(Rad3D), new PropertyMetadata(0.0, new PropertyChangedCallback(Rad3D.OnChanged)));
        internal static readonly DependencyProperty ScaleProperty = DependencyProperty.RegisterAttached("Scale", typeof(double), typeof(Rad3D), new PropertyMetadata(1.0, new PropertyChangedCallback(Rad3D.OnChanged)));
        internal static readonly DependencyProperty SizeProperty = DependencyProperty.RegisterAttached("Size", typeof(Size), typeof(Rad3D), new PropertyMetadata(null));

        internal static Matrix3D CalculateNewTransform(Size size, Point pul, Point pur, Point pll, Point plr)
        {
            Matrix3D affine = Matrix3D.Identity;
            Matrix3D scale = new Matrix3D
            {
                M11 = 1.0 / size.Width,
                M22 = 1.0 / size.Height
            };
            affine = new Matrix3D
            {
                OffsetX = pul.X,
                OffsetY = pul.Y,
                M11 = (pur.X - affine.OffsetX) / 1.0,
                M12 = (pur.Y - affine.OffsetY) / 1.0,
                M21 = (pll.X - affine.OffsetX) / 1.0,
                M22 = (pll.Y - affine.OffsetY) / 1.0
            };
            Matrix3D nonAffine = new Matrix3D();
            double den = (affine.M11 * affine.M22) - (affine.M12 * affine.M21);
            double a = ((((affine.M22 * plr.X) - (affine.M21 * plr.Y)) + (affine.M21 * affine.OffsetY)) - (affine.M22 * affine.OffsetX)) / den;
            double b = ((((affine.M11 * plr.Y) - (affine.M12 * plr.X)) + (affine.M12 * affine.OffsetX)) - (affine.M11 * affine.OffsetY)) / den;
            nonAffine.M11 = a / ((a + b) - 1.0);
            nonAffine.M22 = b / ((a + b) - 1.0);
            nonAffine.M14 = nonAffine.M11 - 1.0;
            nonAffine.M24 = nonAffine.M22 - 1.0;
            return ((scale * nonAffine) * affine);
        }

        internal static double GetCameraDistance(DependencyObject obj)
        {
            return (double)obj.GetValue(CameraDistanceProperty);
        }

        internal static Point3D GetCameraOffset(DependencyObject obj)
        {
            return (Point3D)obj.GetValue(CameraOffsetProperty);
        }

        internal static double GetCameraRotation(DependencyObject obj)
        {
            return (double)obj.GetValue(CameraRotationProperty);
        }

        internal static Point GetRenderLocation(FrameworkElement element, double rotationY)
        {
            Size size;
            Point loc;
            GetRenderSizeAndLocation(element, rotationY, GetScale(element), out size, out loc);
            return loc;
        }

        internal static Size GetRenderSize(FrameworkElement element, double rotationY)
        {
            Size size;
            Point loc;
            GetRenderSizeAndLocation(element, rotationY, GetScale(element), out size, out loc);
            return size;
        }

        internal static Size GetRenderSize(FrameworkElement element, double rotationY, double itemScale)
        {
            Size size;
            Point loc;
            GetRenderSizeAndLocation(element, rotationY, itemScale, out size, out loc);
            return size;
        }

        internal static void GetRenderSizeAndLocation(UIElement element, double rotationY, double scale, out Size renderSize, out Point renderLoc)
        {
            double cameraY = GetCameraRotation(element);
            double itemScale = scale;
            double cameraDistance = GetCameraDistance(element);
            Size elementSize = element.DesiredSize;
            int halfWidth = ((int)elementSize.Width) / 2;
            int halfHeight = ((int)elementSize.Height) / 2;
            Point3D tl = new Point3D((double)-halfWidth, (double)-halfHeight, 0.0);
            Point3D tr = new Point3D((double)halfWidth, (double)-halfHeight, 0.0);
            Point3D bl = new Point3D((double)-halfWidth, (double)halfHeight, 0.0);
            Point3D br = new Point3D((double)halfWidth, (double)halfHeight, 0.0);
            Matrix3D xRotation = ThreeDTools.CreateXRotationMatrix(-cameraY);
            Matrix3D yRotation = ThreeDTools.CreateYRotationMatrix(rotationY);
            double itemZ = (cameraDistance / itemScale) - cameraDistance;
            Matrix3D translate = ThreeDTools.CreateTranslateMatrix(0.0, 0.0, itemZ);
            Point3D translateFinal = GetCameraOffset(element);
            Matrix3D t1 = ThreeDTools.CreateTranslateMatrix(translateFinal.XPosition, translateFinal.YPosition, translateFinal.ZPosition);
            Matrix3D m = ((xRotation * yRotation) * translate) * t1;
            tl = ThreeDTools.Transform(tl, m);
            tr = ThreeDTools.Transform(tr, m);
            bl = ThreeDTools.Transform(bl, m);
            br = ThreeDTools.Transform(br, m);
            Camera camera = PerspectiveCamera.CreateLookAtCenterPerspectiveCamera(cameraDistance, cameraDistance);
            Point tlp = Translate(camera.Project(tl), halfWidth, halfHeight);
            Point trp = Translate(camera.Project(tr), halfWidth, halfHeight);
            Point blp = Translate(camera.Project(bl), halfWidth, halfHeight);
            Point brp = Translate(camera.Project(br), halfWidth, halfHeight);
            renderLoc = new Point(Min(tlp.X, trp.X, blp.X, brp.X), Min(tlp.Y, trp.Y, blp.Y, brp.Y));
            renderSize = new Size(Max(tlp.X, trp.X, blp.X, brp.X) - renderLoc.X, Max(tlp.Y, trp.Y, blp.Y, brp.Y) - renderLoc.Y);
        }

        internal static double GetRotationY(DependencyObject obj)
        {
            return (double)obj.GetValue(RotationYProperty);
        }

        internal static double GetScale(DependencyObject obj)
        {
            return (double)obj.GetValue(ScaleProperty);
        }

        internal static Size GetSize(DependencyObject obj)
        {
            return (Size)obj.GetValue(SizeProperty);
        }

        private static double Max(double a, double b, double c, double d)
        {
            return Math.Max(Math.Max(a, b), Math.Max(c, d));
        }

        private static double Min(double a, double b, double c, double d)
        {
            return Math.Min(Math.Min(a, b), Math.Min(c, d));
        }

        private static void OnChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            RenderTransformation(sender as FrameworkElement, GetCameraDistance(sender), GetCameraRotation(sender), GetRotationY(sender), GetScale(sender));
        }

        internal static void RenderTransformation(FrameworkElement element, double cameraDistance, double cameraY, double rotationY, double itemScale)
        {
            Size elementSize = element.DesiredSize;
            int halfWidth = ((int)elementSize.Width) / 2;
            int halfHeight = ((int)elementSize.Height) / 2;
            Point3D tl = new Point3D((double)-halfWidth, (double)-halfHeight, 0.0);
            Point3D tr = new Point3D((double)halfWidth, (double)-halfHeight, 0.0);
            Point3D bl = new Point3D((double)-halfWidth, (double)halfHeight, 0.0);
            Point3D br = new Point3D((double)halfWidth, (double)halfHeight, 0.0);
            Matrix3D xRotation = ThreeDTools.CreateXRotationMatrix(-cameraY);
            Matrix3D yRotation = ThreeDTools.CreateYRotationMatrix(rotationY);
            double itemZ = (cameraDistance / itemScale) - cameraDistance;
            Matrix3D translate = ThreeDTools.CreateTranslateMatrix(0.0, 0.0, itemZ);
            Point3D translateFinal = GetCameraOffset(element);
            Matrix3D t1 = ThreeDTools.CreateTranslateMatrix(translateFinal.XPosition, translateFinal.YPosition, translateFinal.ZPosition);
            Matrix3D m = ((xRotation * yRotation) * translate) * t1;
            tl = ThreeDTools.Transform(tl, m);
            tr = ThreeDTools.Transform(tr, m);
            bl = ThreeDTools.Transform(bl, m);
            br = ThreeDTools.Transform(br, m);
            Camera camera = PerspectiveCamera.CreateLookAtCenterPerspectiveCamera(cameraDistance, cameraDistance);
            Point tlp = Translate(camera.Project(tl), halfWidth, halfHeight);
            Point trp = Translate(camera.Project(tr), halfWidth, halfHeight);
            Point blp = Translate(camera.Project(bl), halfWidth, halfHeight);
            Point brp = Translate(camera.Project(br), halfWidth, halfHeight);
            element.Projection = new Matrix3DProjection { ProjectionMatrix = CalculateNewTransform(elementSize, tlp, trp, blp, brp) };
        }

        internal static void SetCameraDistance(DependencyObject obj, double value)
        {
            obj.SetValue(CameraDistanceProperty, value);
        }

        internal static void SetCameraOffset(DependencyObject obj, Point3D value)
        {
            obj.SetValue(CameraOffsetProperty, value);
        }

        internal static void SetCameraRotation(DependencyObject obj, double value)
        {
            obj.SetValue(CameraRotationProperty, value);
        }

        internal static void SetRotationY(DependencyObject obj, double value)
        {
            obj.SetValue(RotationYProperty, value);
        }

        internal static void SetScale(DependencyObject obj, double value)
        {
            obj.SetValue(ScaleProperty, value);
        }

        internal static void SetSize(DependencyObject obj, Size value)
        {
            obj.SetValue(SizeProperty, value);
        }

        private static Point Translate(Point p, int x, int y)
        {
            return new Point(p.X + x, p.Y + y);
        }
    }
}

