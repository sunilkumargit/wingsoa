using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Wing
{
    public static class ImagingHelper
    {
        public static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }

        public static void SaveAsJpeg(Image image, String filePath, int jpegQuality)
        {
            ImageCodecInfo myImageCodecInfo;
            System.Drawing.Imaging.Encoder myEncoder;
            EncoderParameter myEncoderParameter;
            EncoderParameters myEncoderParameters;

            // Get an ImageCodecInfo object that represents the JPEG codec.
            myImageCodecInfo = ImagingHelper.GetEncoderInfo("image/jpeg");
            myEncoder = System.Drawing.Imaging.Encoder.Quality;
            myEncoderParameters = new EncoderParameters();
            myEncoderParameter = new EncoderParameter(myEncoder, 70L);
            myEncoderParameters.Param = new EncoderParameter[] { myEncoderParameter };
            image.Save(filePath, myImageCodecInfo, myEncoderParameters);
        }
    }
}
