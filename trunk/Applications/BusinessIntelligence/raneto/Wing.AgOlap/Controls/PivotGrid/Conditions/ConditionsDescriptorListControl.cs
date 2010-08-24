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
using Ranet.AgOlap.Controls.General.Tree;
using Ranet.AgOlap.Controls.General;
using System.Collections.Generic;
using Ranet.AgOlap.Controls.ToolBar;
using System.Windows.Media.Imaging;

namespace Ranet.AgOlap.Controls.PivotGrid.Conditions
{
    public class ConditionsDescriptorListControl : ObjectsListControlBase<CellConditionsDescriptor>
    {
        public ConditionsDescriptorListControl()
            : base()
        {
            ToolBar.Visibility = Visibility.Collapsed;
        }

        public override TreeNode<CellConditionsDescriptor> BuildTreeNode(CellConditionsDescriptor item)
        {
            BitmapImage icon = UriResources.Images.CellStyle16;
            return new TreeNode<CellConditionsDescriptor>(item.MemberUniqueName, icon, item);
        }

        public override bool Contains(string name)
        {
            if (List != null)
            {
                foreach (CellConditionsDescriptor descr in List)
                {
                    if (descr.MemberUniqueName == name)
                        return true;
                }
            }
            return false;
        }

        public override void Refresh()
        {
            foreach (object obj in Tree.Items)
            {
                RefreshItemNode(obj as TreeNode<CellConditionsDescriptor>);
            }
        }

        void RefreshItemNode(TreeNode<CellConditionsDescriptor> node)
        {
            if (node != null && node.Info != null)
            {
                node.Text = node.Info.MemberUniqueName;
            }
        }

        public void AddNew()
        {
            OnAddButtonClick();
        }

        public void DeleteCurrent()
        {
            OnDeleteButtonClick();
        }

        public void DeleteAll()
        {
            OnDeleteAllButtonClick();
        }
    }
}

