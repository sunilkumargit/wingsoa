namespace Telerik.Windows.Controls
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media.Imaging;

    internal class FrameworkElementToImageConverter
    {
        private static Image CloneImageElement(Image originalImage)
        {
            return new Image { Width = originalImage.ActualWidth, Height = originalImage.ActualHeight, VerticalAlignment = VerticalAlignment.Stretch, Source = originalImage.Source };
        }

        public static Image ToImage(FrameworkElement element)
        {
            Image elementAsImage = element as Image;
            if (elementAsImage != null)
            {
                return CloneImageElement(elementAsImage);
            }
            elementAsImage = new Image();
            WriteableBitmap bitmap = new WriteableBitmap(element, null);
            elementAsImage.Source = bitmap;
            return elementAsImage;
        }
    }
}

