namespace Telerik.Windows.Media.Imaging
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Windows;
    using System.Windows.Media.Imaging;

    internal class ImageExporter
    {
        [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        internal static void Export(FrameworkElement element, Stream stream, BitmapEncoder encoder)
        {
            BitmapSource image = ExportHelper.GetElementImage(element);
            encoder.Frames.Add(BitmapFrame.Create(image));
            encoder.Save(stream);
        }
    }
}

