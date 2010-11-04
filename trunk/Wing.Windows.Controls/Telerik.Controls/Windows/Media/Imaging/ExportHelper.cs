namespace Telerik.Windows.Media.Imaging
{
    using System;
    using System.Windows;
    using System.Windows.Media.Imaging;

    internal static class ExportHelper
    {
        private static void ForceLayoutPassIfNeeded(FrameworkElement element)
        {
            if ((element.ActualWidth == 0.0) || (element.ActualHeight == 0.0))
            {
                element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                element.Arrange(new Rect(new Point(0.0, 0.0), element.DesiredSize));
            }
        }

        private static BitmapSource GetBitmapSource(FrameworkElement element)
        {
            Size imageSize = GetImageSize(element);
            WriteableBitmap image = new WriteableBitmap((int) imageSize.Width, (int) imageSize.Height);
            image.Render(element, null);
            image.Invalidate();
            return image;
        }

        internal static BitmapSource GetElementImage(FrameworkElement element)
        {
            return GetBitmapSource(element);
        }

        internal static Size GetImageSize(FrameworkElement element)
        {
            ForceLayoutPassIfNeeded(element);
            return new Size(element.ActualWidth, element.ActualHeight);
        }
    }
}

