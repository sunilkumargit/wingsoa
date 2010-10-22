using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Wing.Olap.Features
{
    static class StringExtensions
    {
        public static Size Measure(this string text, double fontSize, FontFamily fontFamily)
        {
            var block = new TextBlock()
            {
                Text = text,
                FontSize = fontSize,
            };

            if(fontFamily != null)
                block.FontFamily = fontFamily;

            return new Size(block.ActualWidth, block.ActualHeight);
        }
    }
}
