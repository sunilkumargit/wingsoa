namespace Telerik.Windows.Media.Imaging
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Windows;

    public static class ExportExtensions
    {
        public static void ExportToExcelMLImage(FrameworkElement element, Stream stream)
        {
            ExcelMLExporter.Export(element, stream);
        }

        public static void ExportToImage(FrameworkElement element, Stream stream, BitmapEncoder encoder)
        {
            ImageExporter.Export(element, stream, encoder);
        }

        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId="Xps")]
        public static void ExportToXpsImage(FrameworkElement element, Stream stream)
        {
            XpsExporter.Export(element, stream);
        }
    }
}

