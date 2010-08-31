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

namespace Ranet.AgOlap.Controls.General.ItemControls
{
    public class FolderItemControl : ItemControlBase
    {
        public FolderItemControl(FolderInfo info)
        {
            if (info == null)
                throw new ArgumentNullException("info");
            
            m_Info = info;
            Text = info.Name;
            
            Icon = info.IsCustom ? UriResources.Images.FolderOpen16 : UriResources.Images.Folder16;
        }

        FolderInfo m_Info = null;
        public FolderInfo Info
        {
            get {
                return m_Info;
            }
        }
    }
}
