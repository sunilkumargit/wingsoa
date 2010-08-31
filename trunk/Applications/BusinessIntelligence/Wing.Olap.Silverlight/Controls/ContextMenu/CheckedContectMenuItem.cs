using System;
using System.Windows;

namespace Wing.Olap.Controls.ContextMenu
{
    public class CheckedContectMenuItem : ContextMenuItem
    {
        public CheckedContectMenuItem(String caption)
            : base(caption)
        {
            IsChecked = false;
        }

        public bool m_IsChecked = false;
        public bool IsChecked
        {
            get { return m_IsChecked; }
            set {
                m_IsChecked = value;
                if (value)
                    Icon = UriResources.Images.ContexMenuChecked16;
                else
                    Icon = UriResources.Images.ContexMenuUnChecked16;
            }
        }

        protected override void btnContent_Click(object sender, RoutedEventArgs e)
        {
            IsChecked = !IsChecked;
            base.btnContent_Click(sender, e);
        }
    }
}
