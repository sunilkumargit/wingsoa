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
using System.Windows.Media.Imaging;
using Ranet.AgOlap.Controls.General.Tree;

namespace Ranet.AgOlap.Controls.Combo
{
    public class ImageDescriptor
    {
        public BitmapImage Image = null;
        public String Name = String.Empty;
        public String Uri = String.Empty;

        public ImageDescriptor()
        { 
        }

        public ImageDescriptor(BitmapImage image, String name, String uri)
        {
            Image = image;
            Name = name;
            Uri = uri;
        }

        private static ImageDescriptor m_Empty;
        public static ImageDescriptor Empty
        {
            get
            {
                if (m_Empty == null)
                {
                    m_Empty = new ImageDescriptor(null, Localization.ComboBoxItem_None, String.Empty);
                }
                return m_Empty;
            }
        }

    }

    public class ImageDescriptorListControl : ObjectsListControlBase<ImageDescriptor>
    {
        public ImageDescriptorListControl()
            : base()
        {
            ToolBar.Visibility = Visibility.Collapsed;
        }

        public override TreeNode<ImageDescriptor> BuildTreeNode(ImageDescriptor item)
        {
            BitmapImage icon = item.Image;
            return new TreeNode<ImageDescriptor>(item.Name, icon, item);
        }

        public override bool Contains(string name)
        {
            if (List != null)
            {
                foreach (ImageDescriptor descr in List)
                {
                    if (descr.Name == name)
                        return true;
                }
            }
            return false;
        }
    }
}
