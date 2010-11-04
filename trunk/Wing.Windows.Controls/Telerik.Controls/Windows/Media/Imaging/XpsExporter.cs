namespace Telerik.Windows.Media.Imaging
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Windows;
    using System.Windows.Media.Imaging;
    using Telerik.IO.Packaging;

    internal static class XpsExporter
    {
        private static Package CreateXPS(Stream stream, string imageExtension, object[] imageParams)
        {
            Dictionary<string, string[]> partList = CreateXPSPartList(imageExtension);
            Package zip = Package.Open(stream, FileMode.Create);
            foreach (string key in partList.Keys)
            {
                Stream xmlStream = zip.CreatePart(new Uri(key, UriKind.Relative), partList[key][0]).GetStream();
                xmlStream.Write(Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n"), 0, Encoding.UTF8.GetByteCount("<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n"));
                for (int i = 1; i < partList[key].Length; i++)
                {
                    string xml = partList[key][i];
                    xml = string.Format(xml, imageParams);
                    xmlStream.Write(Encoding.UTF8.GetBytes(xml), 0, Encoding.UTF8.GetByteCount(xml));
                }
            }
            return zip;
        }

        private static Dictionary<string, string[]> CreateXPSPartList(string imageExtension)
        {
            Dictionary<string, string[]> partList = new Dictionary<string, string[]>();
            string[] xpsXml = new string[] { "application/vnd.ms-package.xps-fixeddocument+xml", "<FixedDocument xmlns=\"http://schemas.microsoft.com/xps/2005/06\">", "<PageContent Source=\"Pages/1.fpage\" />", "</FixedDocument>" };
            partList.Add("/Documents/1/FixedDocument.fdoc", xpsXml);
            xpsXml = new string[] { "application/vnd.ms-package.xps-fixedpage+xml", "<FixedPage Width=\"{0}\" Height=\"{1}\" xmlns=\"http://schemas.microsoft.com/xps/2005/06\" xml:lang=\"en-US\">", "<Path>", "<Path.Fill>", "<ImageBrush ImageSource=\"/Resources/6caacbf0-55a1-4c58-aed6-e7b3021a56e6" + imageExtension + "\" TileMode=\"None\" Viewbox=\"0,0,{0},{1}\" ViewboxUnits=\"Absolute\" Viewport=\"0,0,{0},{1}\" ViewportUnits=\"Absolute\" />", "</Path.Fill>", "<Path.Data>", "<PathGeometry>", "<PathFigure StartPoint=\"0,0\" IsClosed=\"true\">", "<PolyLineSegment Points=\"{0},0 {0},{1} 0,{1}\" />", "</PathFigure>", "</PathGeometry>", "</Path.Data>", "</Path>", "</FixedPage>" };
            partList.Add("/Documents/1/Pages/1.fpage", xpsXml);
            xpsXml = new string[] { "application/vnd.openxmlformats-package.relationships+xml", "<Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\">", "<Relationship Type=\"http://schemas.microsoft.com/xps/2005/06/required-resource\" Target=\"../../../Resources/6caacbf0-55a1-4c58-aed6-e7b3021a56e6" + imageExtension + "\" Id=\"R2d0ccf49e19c4d64\" />", "</Relationships>" };
            partList.Add("/Documents/1/Pages/_rels/1.fpage.rels", xpsXml);
            xpsXml = new string[] { "application/vnd.ms-package.xps-fixeddocumentsequence+xml", "<FixedDocumentSequence xmlns=\"http://schemas.microsoft.com/xps/2005/06\">", "<DocumentReference Source=\"Documents/1/FixedDocument.fdoc\" />", "</FixedDocumentSequence>" };
            partList.Add("/FixedDocumentSequence.fdseq", xpsXml);
            xpsXml = new string[] { "application/vnd.openxmlformats-package.relationships+xml", "<Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\">", "<Relationship Type=\"http://schemas.microsoft.com/xps/2005/06/fixedrepresentation\" Target=\"/FixedDocumentSequence.fdseq\" Id=\"Ra4fcd452fb0a4fa6\" />", "</Relationships>" };
            partList.Add("/_rels/.rels", xpsXml);
            return partList;
        }

        internal static void Export(FrameworkElement element, Stream stream)
        {
            BitmapEncoder encoder = new PngBitmapEncoder();
            BitmapSource image = ExportHelper.GetElementImage(element);
            encoder.Frames.Add(BitmapFrame.Create(image));
            string[] mimeTypes = encoder.CodecInfo.MimeTypes.Split(new char[] { ',' });
            string imageExtension = encoder.CodecInfo.FileExtensions.Split(new char[] { ',' })[0];
            Package zip = CreateXPS(stream, imageExtension, new object[] { image.PixelWidth, image.PixelHeight });
            Uri zipImageUri = new Uri("/Resources/6caacbf0-55a1-4c58-aed6-e7b3021a56e6" + imageExtension, UriKind.Relative);
            encoder.Save(zip.CreatePart(zipImageUri, mimeTypes[0]).GetStream());
            zip.Flush();
            zip.Close();
        }
    }
}

