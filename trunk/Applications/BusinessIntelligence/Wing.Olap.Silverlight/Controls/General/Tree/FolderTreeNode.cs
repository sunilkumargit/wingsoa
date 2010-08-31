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
using System.Windows.Media.Imaging;

namespace Wing.Olap.Controls.General.Tree
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
