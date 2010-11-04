namespace Telerik.Windows.Media.Imaging
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Windows;
    using System.Windows.Media.Imaging;
    using Telerik.IO.Packaging;

    internal static class ExcelMLExporter
    {
        private static Package CreateExcelML(Stream stream, string imageExtension, object[] excelParams)
        {
            Dictionary<string, string[]> partList = CreateExcelMLPartList(imageExtension);
            Package zip = Package.Open(stream, FileMode.Create);
            foreach (string key in partList.Keys)
            {
                Stream xmlStream = zip.CreatePart(new Uri(key, UriKind.Relative), partList[key][0]).GetStream();
                xmlStream.Write(Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>\r\n"), 0, Encoding.UTF8.GetByteCount("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>\r\n"));
                for (int i = 1; i < partList[key].Length; i++)
                {
                    string xml = partList[key][i];
                    xml = string.Format(CultureInfo.InvariantCulture, xml, excelParams);
                    xmlStream.Write(Encoding.UTF8.GetBytes(xml), 0, Encoding.UTF8.GetByteCount(xml));
                }
            }
            return zip;
        }

        private static Dictionary<string, string[]> CreateExcelMLPartList(string imageExtension)
        {
            Dictionary<string, string[]> partList = new Dictionary<string, string[]>();
            string[] excelXml = new string[] { 
                "application/vnd.openxmlformats-officedocument.extended-properties+xml", "<Properties xmlns=\"http://schemas.openxmlformats.org/officeDocument/2006/extended-properties\" xmlns:vt=\"http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes\">", "<Application>Microsoft Excel</Application>", "<DocSecurity>0</DocSecurity>", "<ScaleCrop>false</ScaleCrop>", "<HeadingPairs>", "<vt:vector size=\"2\" baseType=\"variant\">", "<vt:variant><vt:lpstr>Sheets</vt:lpstr></vt:variant>", "<vt:variant><vt:i4>1</vt:i4></vt:variant>", "</vt:vector>", "</HeadingPairs>", "<TitlesOfParts><vt:vector size=\"1\" baseType=\"lpstr\"><vt:lpstr>1</vt:lpstr></vt:vector></TitlesOfParts>", "<Company></Company>", "<LinksUpToDate>false</LinksUpToDate>", "<SharedDoc>false</SharedDoc>", "<HyperlinksChanged>false</HyperlinksChanged>", 
                "<AppVersion>12.0000</AppVersion>", "</Properties>"
             };
            partList.Add("/docProps/app.xml", excelXml);
            excelXml = new string[] { "application/vnd.openxmlformats-package.core-properties+xml", "<cp:coreProperties xmlns:cp=\"http://schemas.openxmlformats.org/package/2006/metadata/core-properties\" xmlns:dc=\"http://purl.org/dc/elements/1.1/\" xmlns:dcterms=\"http://purl.org/dc/terms/\" xmlns:dcmitype=\"http://purl.org/dc/dcmitype/\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">", "<dc:creator></dc:creator>", "<cp:lastModifiedBy></cp:lastModifiedBy>", "<dcterms:created xsi:type=\"dcterms:W3CDTF\">2006-09-28T05:33:49Z</dcterms:created>", "<dcterms:modified xsi:type=\"dcterms:W3CDTF\">2009-07-03T09:20:46Z</dcterms:modified>", "</cp:coreProperties>" };
            partList.Add("/docProps/core.xml", excelXml);
            excelXml = new string[] { 
                "application/vnd.openxmlformats-officedocument.drawing+xml", "<xdr:wsDr xmlns:xdr=\"http://schemas.openxmlformats.org/drawingml/2006/spreadsheetDrawing\" xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\">", "<xdr:oneCellAnchor>", "<xdr:from>", "<xdr:col>{0}</xdr:col><xdr:colOff>{1}</xdr:colOff>", "<xdr:row>{2}</xdr:row><xdr:rowOff>{3}</xdr:rowOff>", "</xdr:from>", "<xdr:ext cx=\"{4}\" cy=\"{5}\" />", "<xdr:pic>", "<xdr:nvPicPr>", "<xdr:cNvPr id=\"2\" name=\"Image 1\" descr=\"Chart Image\"/>", "<xdr:cNvPicPr><a:picLocks noChangeAspect=\"1\"/></xdr:cNvPicPr>", "</xdr:nvPicPr>", "<xdr:blipFill>", "<a:blip xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\" r:embed=\"rId1\" cstate=\"print\"/>", "<a:stretch>", 
                "<a:fillRect/>", "</a:stretch>", "</xdr:blipFill>", "<xdr:spPr><a:prstGeom prst=\"rect\"><a:avLst/></a:prstGeom></xdr:spPr>", "</xdr:pic>", "<xdr:clientData/>", "</xdr:oneCellAnchor>", "</xdr:wsDr>"
             };
            partList.Add("/xl/drawings/drawing1.xml", excelXml);
            excelXml = new string[] { "application/vnd.openxmlformats-package.relationships+xml", "<Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\">", "<Relationship Id=\"rId1\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/image\" Target=\"../media/image1" + imageExtension + "\"/>", "</Relationships>" };
            partList.Add("/xl/drawings/_rels/drawing1.xml.rels", excelXml);
            excelXml = new string[] { 
                "application/vnd.openxmlformats-officedocument.spreadsheetml.styles+xml", "<styleSheet xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\">", "<fonts count=\"1\">", "<font><sz val=\"11\"/><color theme=\"1\"/><name val=\"Calibri\"/><family val=\"2\"/><charset val=\"204\"/><scheme val=\"minor\"/></font>", "</fonts>", "<fills count=\"2\"><fill><patternFill patternType=\"none\"/></fill>", "<fill><patternFill patternType=\"gray125\"/></fill>", "</fills>", "<borders count=\"1\"><border><left/><right/><top/><bottom/><diagonal/></border></borders>", "<cellStyleXfs count=\"1\">", "<xf numFmtId=\"0\" fontId=\"0\" fillId=\"0\" borderId=\"0\"/>", "</cellStyleXfs>", "<cellXfs count=\"1\">", "<xf numFmtId=\"0\" fontId=\"0\" fillId=\"0\" borderId=\"0\" xfId=\"0\"/>", "</cellXfs>", "<cellStyles count=\"1\">", 
                "<cellStyle name=\"Default\" xfId=\"0\" builtinId=\"0\"/>", "</cellStyles>", "<dxfs count=\"0\"/>", "<tableStyles count=\"0\" defaultTableStyle=\"TableStyleMedium9\" defaultPivotStyle=\"PivotStyleLight16\"/>", "</styleSheet>"
             };
            partList.Add("/xl/styles.xml", excelXml);
            excelXml = new string[] { "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml", "<workbook xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\" xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\">", "<fileVersion appName=\"xl\" lastEdited=\"4\" lowestEdited=\"4\" rupBuild=\"4506\"/>", "<workbookPr filterPrivacy=\"1\" defaultThemeVersion=\"124226\"/>", "<bookViews><workbookView xWindow=\"120\" yWindow=\"105\" windowWidth=\"15120\" windowHeight=\"8010\"/></bookViews>", "<sheets><sheet name=\"1\" sheetId=\"1\" r:id=\"rId1\"/></sheets>", "<calcPr calcId=\"125725\"/>", "</workbook>" };
            partList.Add("/xl/workbook.xml", excelXml);
            excelXml = new string[] { "application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml", "<worksheet xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\" xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\">", "<dimension ref=\"A1\"/>", "<sheetViews>", "<sheetView tabSelected=\"1\" workbookViewId=\"0\">", "<selection activeCell=\"A1\" sqref=\"A1\"/>", "</sheetView>", "</sheetViews>", "<sheetFormatPr defaultRowHeight=\"15\"/>", "<sheetData/>", "<pageMargins left=\"0.7\" right=\"0.7\" top=\"0.75\" bottom=\"0.75\" header=\"0.3\" footer=\"0.3\"/>", "<pageSetup paperSize=\"9\" orientation=\"portrait\" horizontalDpi=\"180\" verticalDpi=\"180\" r:id=\"rId1\"/>", "<drawing r:id=\"rId2\"/>", "</worksheet>" };
            partList.Add("/xl/worksheets/sheet1.xml", excelXml);
            excelXml = new string[] { "application/vnd.openxmlformats-package.relationships+xml", "<Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\">", "<Relationship Id=\"rId2\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/drawing\" Target=\"../drawings/drawing1.xml\"/>", "</Relationships>" };
            partList.Add("/xl/worksheets/_rels/sheet1.xml.rels", excelXml);
            excelXml = new string[] { "application/vnd.openxmlformats-package.relationships+xml", "<Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\">", "<Relationship Id=\"rId1\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet\" Target=\"worksheets/sheet1.xml\"/>", "<Relationship Id=\"rId5\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/styles\" Target=\"styles.xml\"/>", "</Relationships>" };
            partList.Add("/xl/_rels/workbook.xml.rels", excelXml);
            excelXml = new string[] { "application/vnd.openxmlformats-package.relationships+xml", "<Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\">", "<Relationship Id=\"rId3\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/extended-properties\" Target=\"docProps/app.xml\"/>", "<Relationship Id=\"rId2\" Type=\"http://schemas.openxmlformats.org/package/2006/relationships/metadata/core-properties\" Target=\"docProps/core.xml\"/>", "<Relationship Id=\"rId1\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument\" Target=\"xl/workbook.xml\"/>", "</Relationships>" };
            partList.Add("/_rels/.rels", excelXml);
            return partList;
        }

        [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        internal static void Export(FrameworkElement element, Stream stream)
        {
            BitmapEncoder encoder = new PngBitmapEncoder();
            BitmapSource image = ExportHelper.GetElementImage(element);
            encoder.Frames.Add(BitmapFrame.Create(image));
            string[] mimeTypes = encoder.CodecInfo.MimeTypes.Split(new char[] { ',' });
            string imageExtension = encoder.CodecInfo.FileExtensions.Split(new char[] { ',' })[0];
            int cols = 0;
            int cx = (int) (image.PixelWidth * 9525.0);
            int rows = 0;
            int cy = (int) (image.PixelHeight * 9525.0);
            Package zip = CreateExcelML(stream, imageExtension, new object[] { cols, 0, rows, 0, cx, cy });
            Uri zipImageUri = new Uri("/xl/media/image1" + imageExtension, UriKind.Relative);
            encoder.Save(zip.CreatePart(zipImageUri, mimeTypes[0]).GetStream());
            zip.Flush();
            zip.Close();
        }
    }
}

