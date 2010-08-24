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
using System.Windows.Media.Imaging;

namespace Ranet.AgOlap.Controls.General.Tree
{
    public class FolderTreeNode : CustomTreeNode
    {
        BitmapImage m_ExpandedIcon = null;
        public BitmapImage ExpandedIcon
        {
            get { return m_ExpandedIcon; }
            set { m_ExpandedIcon = value; }
        }

        BitmapImage m_CollapsedIcon = null;
        public BitmapImage CollapsedIcon
        {
            get { return m_CollapsedIcon; }
            set { 
                m_CollapsedIcon = value;
            }
        }

        protected override void OnExpanded(RoutedEventArgs e)
        {
            base.OnExpanded(e);

            Icon = ExpandedIcon;
        }

        protected override void OnCollapsed(RoutedEventArgs e)
        {
            base.OnCollapsed(e);

            Icon = CollapsedIcon;
        }

        public FolderTreeNode() 
            : this(UriResources.Images.FolderOpen16, UriResources.Images.Folder16)
        {
        }

        public FolderTreeNode(BitmapImage expandedIcon, BitmapImage collapsedIcon)
        {
            ExpandedIcon = expandedIcon;
            CollapsedIcon = collapsedIcon;

            Icon = CollapsedIcon;
        }
    }
}
