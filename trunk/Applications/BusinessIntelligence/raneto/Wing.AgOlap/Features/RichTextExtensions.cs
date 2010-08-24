/*   
    Copyright (C) 2009 Galaktika Corporation ZAO

    This file is a part of Ranet.UILibrary.Olap
 
    Ranet.UILibrary.Olap is a free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
      
    You should have received a copy of the GNU General Public License
    along with Ranet.UILibrary.Olap.  If not, see
  	<http://www.gnu.org/licenses/> 
  
    If GPL v.3 is not suitable for your products or company,
    Galaktika Corp provides Ranet.UILibrary.Olap under a flexible commercial license
    designed to meet your specific usage and distribution requirements.
    If you have already obtained a commercial license from Galaktika Corp,
    you can use this file under those license terms.
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
using Ranet.AgOlap.Controls.General;

namespace Ranet.AgOlap.Features
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
        public static TextBox AddMouseWheelSupport(this Ranet.AgOlap.Controls.General.RichTextBox textBox)
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
        public static TextBox AddMouseWheelSupport(this Ranet.AgOlap.Controls.General.RichTextBox textBox, double scrollAmount)
        {
            ScrollViewer scrollViewer = textBox.GetScroller();
            if (scrollViewer != null)
            {
                Ranet.AgOlap.Features.ScrollViewerMouseWheelSupport.AddMouseWheelSupport(scrollViewer, scrollAmount);
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
