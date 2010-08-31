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
using Wing.Olap.Controls.General.Tree;
using Wing.Olap.Controls.General;
using Wing.Olap.Core;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace Wing.Olap.Controls.Storage
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
