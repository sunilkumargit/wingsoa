namespace Telerik.Windows.Media.Imaging
{
    using System;

    internal class BmpCodecInfo : BitmapCodecInfo
    {
        public override string FileExtensions
        {
            get
            {
                return ".bmp,.dib,.rle";
            }
        }

        public override string MimeTypes
        {
            get
            {
                return "image/bmp";
            }
        }
    }
}

