/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Wing.Olap.Controls.General.ItemControls
{
    public class ItemControlBase : UserControl
    {
        StackPanel LayoutRoot = null;
        //Grid LayoutRoot = null;
        Image ItemIcon = null;
        TextBlock ItemText = null;

        public ItemControlBase()
            : this(true)
        {
        }

        public ItemControlBase(bool useIcon)
        {
            LayoutRoot = new StackPanel();
            LayoutRoot.Orientation = Orientation.Horizontal;
            //LayoutRoot = new Grid();
            //LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto } );
            //LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition());

            if(useIcon)
            {
                ItemIcon = new Image() { Width = 16, Height = 16 };
                ItemIcon.MouseLeftButtonDown += new MouseButtonEventHandler(ItemIcon_MouseLeftButtonDown);
                LayoutRoot.Children.Add(ItemIcon);
            }
            
            ItemText = new TextBlock() { Margin = new Thickness(3, 0, 0, 0) };
            ItemText.VerticalAlignment = VerticalAlignment.Center;
            ItemText.MouseLeftButtonDown += new MouseButtonEventHandler(ItemText_MouseLeftButtonDown);
            LayoutRoot.Children.Add(ItemText);
            //Grid.SetColumn(ItemText, 1);

            this.Content = LayoutRoot;
        }

        public String Text
        {
            set
            {
                ItemText.Text = value;
            }
            get
            {
                return ItemText.Text;
            }
        }

        public BitmapImage Icon
        {
            get
            {
                return ItemIcon.Source as BitmapImage;
            }
            set
            {
                if (ItemIcon != null)
                {
                    ItemIcon.Source = value;
                }
            }
        }

        
        public event EventHandler IconClick;
        void ItemIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            EventHandler handler = IconClick;
            if (handler != null)
            {
                e.Handled = true;
                handler(this, EventArgs.Empty);
            }
        }

        public event EventHandler TextClick;
        void ItemText_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            EventHandler handler = TextClick;
            if (handler != null)
            {
                e.Handled = true;
                handler(this, EventArgs.Empty);
            }
        }
    }
}

