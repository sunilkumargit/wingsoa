/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System.Windows.Media;

namespace Wing.Olap.Controls.General
{
    public static class ColorHelper
    {
        public static Color FromRgb(int rgb)
        {
            return Color.FromArgb(255, GetR(rgb), GetG(rgb), GetB(rgb));
        }

        private static byte GetR(int rgb)
        {
            return (byte)(rgb & 0x0000FF);
        }

        private static byte GetG(int rgb)
        {
            return (byte)((rgb & 0x00FF00) >> 8);
        }

        private static byte GetB(int rgb)
        {
            return (byte)((rgb & 0xFF0000) >> 16);
        }
    }
}
