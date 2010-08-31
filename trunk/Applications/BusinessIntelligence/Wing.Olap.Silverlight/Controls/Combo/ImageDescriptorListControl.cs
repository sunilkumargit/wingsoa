/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Windows;
using System.Windows.Media.Imaging;
using Wing.Olap.Controls.General;
using Wing.Olap.Controls.General.Tree;

namespace Wing.Olap.Controls.Combo
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
