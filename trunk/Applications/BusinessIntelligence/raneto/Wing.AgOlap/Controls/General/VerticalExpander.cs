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
using Ranet.AgOlap.Controls.Buttons;

namespace Ranet.AgOlap.Controls.General
{
    public class VerticalExpander : Grid
    {
        private TextBlock m_Caption;
        private RanetButton m_ExpandButton;
        //private ContentControl m_Content;
        ColumnDefinition headerColumnDefinition;

        public VerticalExpander()
        {
            headerColumnDefinition = new ColumnDefinition();
            headerColumnDefinition.Width = GridLength.Auto;
            base.ColumnDefinitions.Add(headerColumnDefinition);
            base.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(0.0, GridUnitType.Pixel) });
            base.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1.0, GridUnitType.Star) });

            StackPanel header = new StackPanel();
            header.Orientation = Orientation.Vertical;
            base.Children.Add(header);

            m_ExpandButton = new RanetButton();
            Image icon = new Image();
            icon.Source = UriResources.Images.ForwardDouble16;
            m_ExpandButton.Content = icon;
            m_ExpandButton.Width = 20;
            m_ExpandButton.Height = 20;
            m_ExpandButton.Click += new RoutedEventHandler(ExpandButton_Click);
            header.Children.Add(m_ExpandButton);

            m_Caption = new TextBlock();
            m_Caption.RenderTransform = new RotateTransform() { Angle = 90, CenterX = 5, CenterY = 20 };
            header.Children.Add(m_Caption);

            //m_Content = new ContentControl();
            //m_Content.VerticalAlignment = VerticalAlignment.Stretch;
            //m_Content.HorizontalAlignment = HorizontalAlignment.Stretch;
        }

        public GridLength HeaderWidth
        {
            set
            {
                headerColumnDefinition.Width = value;
            }
            get
            {
                return headerColumnDefinition.Width;
            }
        }


        void ExpandButton_Click(object sender, RoutedEventArgs e)
        {
            this.IsExpanded = !this.IsExpanded;
        }

        private void UpdateView()
        {
            if (m_IsExpanded)
            {
                Image icon = new Image();
                icon.Source = UriResources.Images.BackDouble16;
                m_ExpandButton.Content = icon;
                base.ColumnDefinitions[1].Width = new GridLength(1.0, GridUnitType.Star);
                m_Content.Visibility = Visibility.Visible;
                this.RestoreWidth();
            }
            else
            {
                Image icon = new Image();
                icon.Source = UriResources.Images.ForwardDouble16;
                m_ExpandButton.Content = icon;
                base.ColumnDefinitions[1].Width = new GridLength(0.0);
                m_Content.Visibility = Visibility.Collapsed;
                this.SaveWidth();
            }
        }

        class GridLengthHolder
        {
            public GridLengthHolder()
            {
            }

            public GridLengthHolder(GridLength length)
            {
                this.Length = length;
            }

            public GridLength Length;
        }

        private GridLengthHolder m_Width;
        private void SaveWidth()
        {
            if (this.Parent == null) return;
            Grid parent = this.Parent as Grid;
            if (parent == null)
            {
                FrameworkElement border = this.Parent as FrameworkElement;
                if (border != null)
                {
                    parent = border.Parent as Grid;
                }
            }
            if (parent != null)
            {
                int columnIndex = Grid.GetColumn(this);
                if (columnIndex < parent.ColumnDefinitions.Count)
                {
                    m_Width = new GridLengthHolder(parent.ColumnDefinitions[columnIndex].Width);
                    parent.ColumnDefinitions[columnIndex].Width = GridLength.Auto;
                }
            }
        }

        private void RestoreWidth()
        {
            if (this.Parent == null) return;
            Grid parent = this.Parent as Grid;
            if (parent == null)
            {
                FrameworkElement border = this.Parent as FrameworkElement;
                if (border != null)
                {
                    parent = border.Parent as Grid;
                }
            }
            if (parent != null)
            {
                int columnIndex = Grid.GetColumn(this);
                if (columnIndex < parent.ColumnDefinitions.Count)
                {
                    if (m_Width != null)
                    {
                        parent.ColumnDefinitions[columnIndex].Width = m_Width.Length;
                    }
                    else
                    {
                        parent.ColumnDefinitions[columnIndex].Width = new GridLength(200.0);
                    }
                }
            }
        }

        private bool m_IsExpanded = false;
        public bool IsExpanded
        {
            get
            {
                return m_IsExpanded;
            }
            set
            {
                m_IsExpanded = value;
                this.UpdateView();
            }
        }


        public string Caption
        {
            get
            {
                return m_Caption.Text;
            }
            set
            {
                m_Caption.Text = value;
            }
        }

        private FrameworkElement m_Content;
        public FrameworkElement Content
        {
            get
            {
                return m_Content;
            }
            set
            {
                if (m_Content != null)
                {
                    base.Children.Remove(m_Content);
                }
                m_Content = value;
                m_Content.HorizontalAlignment = HorizontalAlignment.Stretch;
                m_Content.VerticalAlignment = VerticalAlignment.Stretch;
                m_Content.Visibility = Visibility.Collapsed;
                Grid.SetColumn(m_Content, 1);
                base.Children.Add(m_Content);
            }
        }

        //public object Content
        //{
        //    get
        //    {
        //        return m_Content.Content;
        //    }
        //    set
        //    {
        //        m_Content.Content = value;
        //    }
        //}
    }
}
