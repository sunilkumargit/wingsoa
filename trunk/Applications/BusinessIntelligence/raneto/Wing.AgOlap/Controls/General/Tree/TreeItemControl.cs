/*   
    Copyright (C) 2009 Galaktika Corporation ZAO

    This file is a part of Ranet.UILibrary.Olap
 
    Ranet.UILibrary.Olap is a free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
      
    You should have received a copy of the GNU General Public License
    along with Ranet.UILibrary.Olap.  If not, see
  	<http://www.gnu.org/licenses/> 
  
    If GPL v.3 is not suitable for your products or company,
    Galaktika Corp provides Ranet.UILibrary.Olap under a flexible commercial license
    designed to meet your specific usage and distribution requirements.
    If you have already obtained a commercial license from Galaktika Corp,
    you can use this file under those license terms.
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

namespace Ranet.AgOlap.Controls.General.Tree
{
    public delegate void MouseDoubleClickEventHandler(object sender, EventArgs e);

    public class TreeItemControl : DragDropControl
    {
        Storyboard resDoubleClickTimer;

        public Image ItemIcon;
        public TextBlock ItemText;

        public TreeItemControl(bool useIcon)
        { 
            Grid LayoutRoot = new Grid();
            LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(16) });
            LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition());

            ItemIcon = new Image() { Width = 16, Height = 16 };
            LayoutRoot.Children.Add(ItemIcon);
            Grid.SetColumn(ItemIcon, 0);
            
            ItemText = new TextBlock() {Margin = new Thickness(3,0,0,0) };
            LayoutRoot.Children.Add(ItemText);
            Grid.SetColumn(ItemText, 1);

            if (useIcon)
            {
                ItemIcon.MouseLeftButtonDown += new MouseButtonEventHandler(ItemIcon_MouseLeftButtonDown);
            }
            else
            {
                if (LayoutRoot.ColumnDefinitions.Count > 0)
                {
                    LayoutRoot.ColumnDefinitions[0].Width = new GridLength(0);
                }
            }

            resDoubleClickTimer = new Storyboard();
            resDoubleClickTimer.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 250));
            resDoubleClickTimer.Completed += new EventHandler(Storyboard_Completed);
            LayoutRoot.Resources.Add("resDoubleClickTimer", resDoubleClickTimer);

            this.MouseLeftButtonDown += new MouseButtonEventHandler(TreeItemControl_MouseLeftButtonDown);

            this.Content = LayoutRoot;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();


        }

        public event MouseDoubleClickEventHandler MouseDoubleClick;
        protected void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            if (this.MouseDoubleClick != null)
            {
                this.MouseDoubleClick(this, e);
            }
        }

        int m_ClickCount = 0;
        MouseButtonEventArgs m_LastArgs;

        void TreeItemControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            m_ClickCount++;
            m_LastArgs = e;
            resDoubleClickTimer.Begin();
        }

        private void Storyboard_Completed(object sender, EventArgs e)
        {
            if (m_ClickCount > 1)
            {
                this.OnMouseDoubleClick(m_LastArgs);
            }
            m_ClickCount = 0;
        }

        public TreeItemControl()
            : this(true)
        {
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
            set
            {
                ItemIcon.Source = value;
            }
            get
            {
                return ItemIcon.Source as BitmapImage;
            }
        }
    }
}
