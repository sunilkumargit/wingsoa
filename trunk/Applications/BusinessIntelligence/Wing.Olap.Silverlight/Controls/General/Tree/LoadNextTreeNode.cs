/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System.Windows;

namespace Wing.Olap.Controls.General.Tree
{
    public class LoadNextTreeNode : CustomTreeNode
    {
        public LoadNextTreeNode() 
            : base()
        {
            Text = Localization.MemberChoice_LoadNext;
            m_ItemCtrl.ItemText.FontStyle = FontStyles.Italic;
            Icon = UriResources.Images.LoadNext16;
        }
    }
}
