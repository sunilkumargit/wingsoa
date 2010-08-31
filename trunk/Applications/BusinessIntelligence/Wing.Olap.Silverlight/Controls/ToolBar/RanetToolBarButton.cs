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

namespace Wing.AgOlap.Controls.ToolBar
{
    public class RanetToolBarButton :Button
    {
        public RanetToolBarButton()
        {
            DefaultStyleKey = typeof(RanetToolBarButton);
            this.Height = 22;
            this.Width = 22;
            this.Margin = new Thickness(2, 0, 0, 0);
        }

        public RanetToolBarButton(BitmapImage icon, String text)
        {
            Image ItemIcon = null;
            TextBlock ItemText = null;
            StackPanel LayoutRoot = null;

            DefaultStyleKey = typeof(RanetToolBarButton);
            this.Height = 22;
            this.MinWidth = 22;
            this.Margin = new Thickness(2, 0, 0, 0);

            LayoutRoot = new StackPanel();
            LayoutRoot.Orientation = Orientation.Horizontal;

            ItemIcon = new Image() { Width = 16, Height = 16 };
            ItemIcon.Source = icon;
            LayoutRoot.Children.Add(ItemIcon);

            ItemText = new TextBlock() { Margin = new Thickness(3, 0, 3, 0) };
            ItemText.VerticalAlignment = VerticalAlignment.Center;
            ItemText.Text = text;
            LayoutRoot.Children.Add(ItemText);

            this.Content = LayoutRoot;
        }
    }
}
