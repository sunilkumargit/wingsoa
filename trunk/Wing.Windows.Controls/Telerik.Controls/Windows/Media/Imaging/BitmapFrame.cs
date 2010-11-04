namespace Telerik.Windows.Media.Imaging
{
    using System;
    using System.Windows.Media.Imaging;

    public static class BitmapFrame
    {
        public static WriteableBitmap Create(BitmapSource source)
        {
            return (WriteableBitmap) source;
        }
    }
}

