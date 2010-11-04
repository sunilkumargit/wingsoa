namespace Telerik.Windows.Media.Imaging
{
    using System;

    internal class PngCodecInfo : BitmapCodecInfo
    {
        public override string FileExtensions
        {
            get
            {
                return ".png";
            }
        }

        public override string MimeTypes
        {
            get
            {
                return "image/png";
            }
        }
    }
}

