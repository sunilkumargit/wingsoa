/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Wing.Olap.Controls.General
{
    public class HeaderControl : UserControl
    {
        TextBlock m_Caption;
        Image m_Icon;
        Image m_TransparentIcon;

        public String Caption
        {
            get { return m_Caption.Text; }
            set { m_Caption.Text = value; }
        }

        public BitmapImage Icon
        {
            get { return m_Icon.Source as BitmapImage; }
            set { m_Icon.Source = value; }
        }

        public BitmapImage TransparentIcon
        {
            get { return m_TransparentIcon.Source as BitmapImage; }
            set { m_TransparentIcon.Source = value; }
        }

        public HeaderControl()
        {
            // Заголовок
            Grid HeaderLayoutRoot = new Grid();
            HeaderLayoutRoot.VerticalAlignment = VerticalAlignment.Center;
            HeaderLayoutRoot.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            HeaderLayoutRoot.ColumnDefinitions.Add(new ColumnDefinition());

            m_Icon = new Image() { Width = 16, Height = 16 };
            HeaderLayoutRoot.Children.Add(m_Icon);

            m_TransparentIcon = new Image() { Width = 16, Height = 16 };
            HeaderLayoutRoot.Children.Add(m_TransparentIcon);

            m_Caption = new TextBlock() { Margin = new Thickness(3, 0, 0, 0) };
            HeaderLayoutRoot.Children.Add(m_Caption);
            Grid.SetColumn(m_Caption, 1);

            this.Content = HeaderLayoutRoot;
        }

        public HeaderControl(BitmapImage icon, String caption)
            : this()
        {
            Icon = icon;
            Caption = caption;
        }
    }
}
