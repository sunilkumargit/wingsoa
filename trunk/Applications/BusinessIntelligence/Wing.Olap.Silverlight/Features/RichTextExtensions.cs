/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

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
using Wing.Olap.Controls.General;

namespace Wing.Olap.Features
{
    public static class RichTextExtensions
    {
        private const int DEFAULT_SCROLL_AMOUNT = 30;

        /// <summary>
        /// Adds mouse wheel support to a <see cref="TextBox"/>.
        /// As long as the <see cref="TextBox"/> has focus,
        /// the mouse wheel can be used to scroll up and down.
        /// </summary>
        /// <param name="textBox">The text box.</param>
        /// <returns>The <see cref="TextBox"/>.</returns>
        public static TextBox AddMouseWheelSupport(this Wing.Olap.Controls.General.RichTextBox textBox)
        {
            return AddMouseWheelSupport(textBox, DEFAULT_SCROLL_AMOUNT);
        }

        /// <summary>
        /// Adds mouse wheel support to a <see cref="TextBox"/>.
        /// As long as the <see cref="TextBox"/> has focus,
        /// the mouse wheel can be used to scroll up and down.
        /// </summary>
        /// <param name="textBox">The text box.</param>
        /// <param name="scrollAmount">The amount to scroll by when the mouse wheel is moved.</param>
        /// <returns>The <see cref="TextBox"/>.</returns>
        public static TextBox AddMouseWheelSupport(this Wing.Olap.Controls.General.RichTextBox textBox, double scrollAmount)
        {
            ScrollViewer scrollViewer = textBox.GetScroller();
            if (scrollViewer != null)
            {
                Wing.Olap.Features.ScrollViewerMouseWheelSupport.AddMouseWheelSupport(scrollViewer, scrollAmount);
                textBox.IsMouseWheelAttached = true;
            }

            return textBox;
        }

        //public static ScrollViewer GetScroller(this RichTextBox textBox)
        //{
        //    FieldInfo scrollViewerFileldInfo = typeof(TextBox).GetField("_scrollViewer", BindingFlags.NonPublic | BindingFlags.Instance);
        //    if (scrollViewerFileldInfo != null)
        //    {

        //    }

        //    return null;
        //}
    }
}
