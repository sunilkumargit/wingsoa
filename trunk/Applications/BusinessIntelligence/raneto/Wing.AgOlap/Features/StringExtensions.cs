using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Ranet.AgOlap.Features
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
