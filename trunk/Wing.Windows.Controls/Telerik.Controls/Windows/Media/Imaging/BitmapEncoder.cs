namespace Telerik.Windows.Media.Imaging
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Windows.Media.Imaging;

    public abstract class BitmapEncoder
    {
        private List<WriteableBitmap> frames = new List<WriteableBitmap>();

        protected BitmapEncoder()
        {
        }

        public virtual void Dispose()
        {
        }

        public virtual void Save(Stream stream)
        {
            if (this.Frames.Count == 0)
            {
                throw new NotSupportedException("Frames Count is 0.");
            }
            if (this.Frames[0] == null)
            {
                throw new NotSupportedException("The Frame is null.");
            }
            if (!(this.Frames[0] is WriteableBitmap))
            {
                throw new NotSupportedException("The Frame must have the WriteableBitmap type.");
            }
        }

        public virtual BitmapCodecInfo CodecInfo
        {
            get
            {
                return null;
            }
        }

        public IList<WriteableBitmap> Frames
        {
            get
            {
                return this.frames;
            }
        }
    }
}

