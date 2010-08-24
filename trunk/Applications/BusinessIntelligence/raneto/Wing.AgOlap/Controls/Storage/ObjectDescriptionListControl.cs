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
using Ranet.Olap.Core;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace Ranet.AgOlap.Controls.Storage
{
    public class ObjectDescriptionListControl : ObjectsListControlBase<ObjectStorageFileDescription>
    {
        public ObjectDescriptionListControl()
            : base()
        {
            AddButton.Visibility = Visibility.Collapsed;
        }

        public override TreeNode<ObjectStorageFileDescription> BuildTreeNode(ObjectStorageFileDescription item)
        {
            BitmapImage icon = UriResources.Images.FileExtension16;
            if (String.IsNullOrEmpty(item.ContentFileName))
                icon = UriResources.Images.FileError16;
            return new TreeNode<ObjectStorageFileDescription>(item.Description.Caption, icon, item);
        }

        public override bool Contains(string name)
        {
            if (List != null)
            {
                foreach (ObjectStorageFileDescription descr in List)
                {
                    if (descr.Description.Name == name)
                        return true;
                }
            }
            return false;
        }

        public event EventHandler<CustomEventArgs<ObjectStorageFileDescription>> CreateNewButtonClick;
        public event EventHandler<CustomEventArgs<ObjectStorageFileDescription>> DeleteButtonClick;
        public event EventHandler<CustomEventArgs<ObjectStorageFileDescription>> DeleteAllButtonClick;

        protected override void OnAddButtonClick()
        {
            EventHandler<CustomEventArgs<ObjectStorageFileDescription>> handler = CreateNewButtonClick;
            if (handler != null)
            {
                handler(this, new CustomEventArgs<ObjectStorageFileDescription>(CurrentObject));
            }
        }

        protected override void OnDeleteButtonClick()
        {
            EventHandler<CustomEventArgs<ObjectStorageFileDescription>> handler = DeleteButtonClick;
            if (handler != null)
            {
                handler(this, new CustomEventArgs<ObjectStorageFileDescription>(CurrentObject));
            }
        }

        protected override void OnDeleteAllButtonClick()
        {
            EventHandler<CustomEventArgs<ObjectStorageFileDescription>> handler = DeleteAllButtonClick;
            if (handler != null)
            {
                handler(this, new CustomEventArgs<ObjectStorageFileDescription>(CurrentObject));
            }
        }
    }
}
