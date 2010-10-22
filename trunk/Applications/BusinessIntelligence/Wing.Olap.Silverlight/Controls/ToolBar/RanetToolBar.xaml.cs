/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Wing.Olap.Controls.ToolBar
{
    public delegate void ItemClickEventHandler(object sender, ItemClickEventArgs e);

    public class ItemClickEventArgs : EventArgs
    {
        private UIElement item;

        public ItemClickEventArgs(UIElement item)
        {
            this.item = item;
        }

        public UIElement Item
        {
            get { return item; }
        }
    }

    public partial class RanetToolBar : UserControl
    {
        public event ItemClickEventHandler ItemClick;

        public RanetToolBar()
        {
            InitializeComponent();
        }

        public UIElementCollection Items
        {
            get
            {
                return stack.Children;
            }
        }

        public void AddItem(FrameworkElement item)
        {
           if(item != null)
           {
               ButtonBase btn = item as ButtonBase;
               if (btn != null)
               {
                   btn.Click -= new RoutedEventHandler(btn_Click);
                   btn.Click += new RoutedEventHandler(btn_Click);
               }
               stack.Children.Add(item);
           }
        }

        void btn_Click(object sender, RoutedEventArgs e)
        {
            Raise_ItemClick(new ItemClickEventArgs(sender as UIElement));
        }

        private void Raise_ItemClick(ItemClickEventArgs e)
        {
            ItemClickEventHandler handler = this.ItemClick;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
