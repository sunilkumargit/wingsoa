namespace Telerik.Windows.Media.Imaging
{
    using System;

    public abstract class BitmapCodecInfo
    {
        protected BitmapCodecInfo()
        {
        }

        public virtual string FileExtensions
        {
            get
            {
                return string.Empty;
            }
        }

        public virtual string MimeTypes
        {
            get
            {
                return string.Empty;
            }
        }

        public virtual bool SupportsAnimation
        {
            get
            {
                return false;
            }
        }

        public virtual bool SupportsMultipleFrames
        {
            get
            {
                return false;
            }
        }
    }
}

