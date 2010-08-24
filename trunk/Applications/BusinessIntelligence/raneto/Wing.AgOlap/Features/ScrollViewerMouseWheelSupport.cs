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
using System.Windows.Browser;
using System.Windows.Controls;
using System.Windows;
using System.Reflection;
using Ranet.AgOlap.Controls.General.Tree;

namespace Ranet.AgOlap.Features
{
    public enum MouseWheelAssociationMode
    {
        /// <summary>
        /// The mouse wheel will affect a <see cref="ScrollViewer" /> whenever the cursor rolls over it.
        /// </summary>
        OnHover,

        /// <summary>
        /// The mouse wheel will only affect a <see cref="ScrollViewer" /> when it is explicitly assigned focus by a mouse click or a keyboard tab.
        /// </summary>
        OnFocus
    }
    
    /// <summary>
    /// Utility class for adding mouse wheel scrolling to ScrollViewer controls
    /// </summary>
    public static class ScrollViewerMouseWheelSupport
    {
        private const int DEFAULT_SCROLL_AMOUNT = 30;
        private static MouseWheelSupport mouseWheelHelper;

        /// <summary>
        /// This should be called exactly one time in the constructor of the application's main XAML
        /// code behind file, immediately after the call to InitializeComponent().
        /// </summary>
        /// <param name="applicationRootVisual">The application root visual. Just give a name to the root element of your application and pass it in here.</param>
        public static void Initialize(FrameworkElement applicationRootVisual)
        {
            Initialize(applicationRootVisual, MouseWheelAssociationMode.OnHover);
        }
        
        /// <summary>
        /// This should be called exactly one time in the constructor of the application's main XAML
        /// code behind file, immediately after the call to InitializeComponent().
        /// </summary>
        /// <param name="applicationRootVisual">The application root visual. Just give a name to the root element of your application and pass it in here.</param>
        /// <param name="activationMode">Specifies whether the mouse wheel is associated with a <see cref="ScrollViewer" /> by simply hovering over it (OnHover) or if the user must explicitly assign focus to it before the wheel affects it (OnFocus).</param>
        public static void Initialize(FrameworkElement applicationRootVisual, MouseWheelAssociationMode activationMode)
        {
            MouseWheelSupport.RegisterRootVisual(applicationRootVisual);
            MouseWheelSupport.UseFocusBehaviorModel = (activationMode == MouseWheelAssociationMode.OnFocus);
        }

        /// <summary>
        /// Adds mouse wheel support to a <see cref="ScrollViewer"/>.
        /// As long as the <see cref="ScrollViewer"/> has focus,
        /// the mouse wheel can be used to scroll up and down.
        /// </summary>
        /// <param name="scrollViewer">The scroll viewer.</param>
        /// <returns>The <see cref="ScrollViewer"/>.</returns>
        public static ScrollViewer AddMouseWheelSupport(this ScrollViewer scrollViewer, FrameworkElement mouseMoveElement)
        {
            return AddMouseWheelSupport(scrollViewer, mouseMoveElement, DEFAULT_SCROLL_AMOUNT);
        }

        /// <summary>
        /// Adds mouse wheel support to a <see cref="ScrollViewer"/>.
        /// As long as the <see cref="ScrollViewer"/> has focus,
        /// the mouse wheel can be used to scroll up and down.
        /// </summary>
        /// <param name="scrollViewer">The scroll viewer.</param>
        /// <returns>The <see cref="ScrollViewer"/>.</returns>
        public static ScrollViewer AddMouseWheelSupport(this ScrollViewer scrollViewer)
        {
            return AddMouseWheelSupport(null, scrollViewer, DEFAULT_SCROLL_AMOUNT);
        }

        /// <summary>
        /// Adds mouse wheel support to a <see cref="ScrollViewer"/>.
        /// As long as the <see cref="ScrollViewer"/> has focus,
        /// the mouse wheel can be used to scroll up and down.
        /// </summary>
        /// <param name="scrollViewer">The scroll viewer.</param>
        /// <param name="scrollAmount">The amount to scroll by when the mouse wheel is moved.</param>
        /// <returns>The <see cref="ScrollViewer"/>.</returns>
        public static ScrollViewer AddMouseWheelSupport(this ScrollViewer scrollViewer, double scrollAmount)
        {
            return AddMouseWheelSupport(null, scrollViewer, scrollAmount);
        }

        /// <summary>
        /// Adds mouse wheel support to a <see cref="ScrollViewer"/>.
        /// As long as the <see cref="ScrollViewer"/> has focus,
        /// the mouse wheel can be used to scroll up and down.
        /// </summary>
        /// <param name="parentScrollViewer">The parent <see cref="ScrollViewer"/> which contains another <see cref="ScrollViewer"/> which should have mouse wheel scrolling support.</param>
        /// <param name="childScrollViewer">A child <see cref="ScrollViewer"/> to add mouse wheel scrolling support to.</param>
        /// <returns>The child <see cref="ScrollViewer"/>.</returns>
        public static ScrollViewer AddMouseWheelSupport(this ScrollViewer parentScrollViewer, ScrollViewer childScrollViewer)
        {
            return parentScrollViewer.AddMouseWheelSupport(childScrollViewer, DEFAULT_SCROLL_AMOUNT);
        }

        /// <summary>
        /// Adds mouse wheel support to a <see cref="ScrollViewer"/>.
        /// As long as the <see cref="ScrollViewer"/> has focus,
        /// the mouse wheel can be used to scroll up and down.
        /// </summary>
        /// <param name="parentScrollViewer">The parent <see cref="ScrollViewer"/> which contains another <see cref="ScrollViewer" /> which should have mouse wheel scrolling support.</param>
        /// <param name="childScrollViewer">A child <see cref="ScrollViewer"/> to add mouse wheel scrolling support to.</param>
        /// <param name="scrollAmount">The amount to scroll by when the mouse wheel is moved.</param>
        /// <returns>The child <see cref="ScrollViewer"/>.</returns>
        public static ScrollViewer AddMouseWheelSupport(this ScrollViewer parentScrollViewer, ScrollViewer childScrollViewer, double scrollAmount)
        {
            mouseWheelHelper = new MouseWheelSupport(childScrollViewer, parentScrollViewer);
            return AddScrollViewerMouseWheelSupport(childScrollViewer, scrollAmount);
        }

        static ScrollViewer AddScrollViewerMouseWheelSupport(ScrollViewer scrollViewer, double scrollAmount)
        {
            if(mouseWheelHelper != null)
            {
                mouseWheelHelper.MouseWheelMoved += (source, eventArgs) =>
                {
                    var delta = eventArgs.WheelDelta;

                    delta *= scrollAmount;

                    if (eventArgs.IsHorizontal)
                    {
                        var newOffset = scrollViewer.HorizontalOffset - delta;

                        if (newOffset > scrollViewer.ScrollableWidth)
                            newOffset = scrollViewer.ScrollableWidth;
                        else if (newOffset < 0)
                            newOffset = 0;

                        scrollViewer.ScrollToHorizontalOffset(newOffset);
                    }
                    else
                    {
                        var newOffset = scrollViewer.VerticalOffset - delta;

                        if (newOffset > scrollViewer.ScrollableHeight)
                            newOffset = scrollViewer.ScrollableHeight;
                        else if (newOffset < 0)
                            newOffset = 0;

                        scrollViewer.ScrollToVerticalOffset(newOffset);
                    }
                    eventArgs.BrowserEventHandled = true;
                };
            }
            return scrollViewer;
        }

        public static ScrollViewer AddMouseWheelSupport(this ScrollViewer childScrollViewer, FrameworkElement mouseMoveElement, double scrollAmount)
        {
            mouseWheelHelper = new MouseWheelSupport(mouseMoveElement, null);
            return AddScrollViewerMouseWheelSupport(childScrollViewer, scrollAmount);
        }

        //public static bool RemoveMouseWheelSupport(ScrollViewer childScrollViewer)
        //{
        //   if (mouseWheelHelper!= null)
        //   {
        //       try
        //       {
        //           mouseWheelHelper.RemoveWheelSupport(childScrollViewer);
        //           mouseWheelHelper = null;
        //           return true;
        //       }
        //       catch 
        //       {
        //           return false;
        //       }               
        //   }
        //   return false;
        //}

        public static bool RemoveMouseWheelSupport(FrameworkElement mouseMoveElement)
        {
            if (mouseWheelHelper != null)
            {
                try
                {
                    mouseWheelHelper.RemoveWheelSupport(mouseMoveElement);
                    mouseWheelHelper = null;
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }
    }
}
