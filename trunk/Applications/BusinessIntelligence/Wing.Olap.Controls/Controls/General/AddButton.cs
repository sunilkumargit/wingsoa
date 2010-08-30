﻿/*   
    Copyright (C) 2009 Galaktika Corporation ZAO

    This file is a part of Wing.UILibrary.Olap
 
    Wing.UILibrary.Olap is a free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
      
    You should have received a copy of the GNU General Public License
    along with Wing.UILibrary.Olap.  If not, see
  	<http://www.gnu.org/licenses/> 
  
    If GPL v.3 is not suitable for your products or company,
    Galaktika Corp provides Wing.UILibrary.Olap under a flexible commercial license
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

namespace Wing.AgOlap.Controls.General
{
    public class AddButton : HotButton
    {
        public AddButton() 
        {
            this.Height = 22;
            this.Width = 22;

            LeaveImage = CreateImage();
            LeaveImage.Source = UriResources.Images.Add16;

            EnterImage = CreateImage();
            EnterImage.Source = UriResources.Images.AddHot16;

            this.Content = LeaveImage;
        }

        Image CreateImage()
        {
            Image image = new Image();
            image.Height = 16;
            image.Width = 16;
            return image;
        }

    }
}
