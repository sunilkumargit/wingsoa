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
using System.Collections.Generic;
using Ranet.AgOlap.Controls.MemberChoice.Info;
using Ranet.AgOlap.Controls.General.ItemControls;

namespace Ranet.AgOlap.Controls.MemberChoice
{
    public class ItemSelectedEventArgs : EventArgs
    {
        public String UniqueName {get; internal set;}
        public ItemSelectedEventArgs(String uniqueName)
        {
            UniqueName = uniqueName;
        }
    }

    public class MembersListControl : UserControl
    {
        Grid LayoutRoot;
        public MembersListControl()
        {
            LayoutRoot = new Grid();
            LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition());
            LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

            ScrollViewer scroll = new ScrollViewer();
            scroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            scroll.Content = LayoutRoot;
            scroll.BorderBrush = new SolidColorBrush(Colors.Transparent);

            Border border = new Border() { BorderThickness = new Thickness(1), BorderBrush = new SolidColorBrush(Colors.Black) };
            border.Child = scroll;
            border.CornerRadius = new CornerRadius(2);

            this.Content = border;
        }

        public void Initialize(List<OlapMemberInfo> infos, bool useStateIcons)
        {
            LayoutRoot.RowDefinitions.Clear();
            LayoutRoot.Children.Clear();
            if (infos != null)
            {
                int row = 0;
                foreach (OlapMemberInfo info in infos)
                {
                    LayoutRoot.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(24) });

                    MemberItemControl ctrl = null;
                    if (useStateIcons)
                    {
                        ctrl = new MemberItemControl(info.Info, MemberChoiceControl.GetIconImage(info));
                    }
                    else
                        ctrl = new MemberItemControl(info.Info);
                    ctrl.Tag = info;
                    LayoutRoot.Children.Add(ctrl);
                    Grid.SetRow(ctrl, row);
                    row++;

                    ctrl.IconClick += new EventHandler(MemberItem_IconClick);
                    ctrl.TextClick += new EventHandler(MemberItem_TextClick);
                }
            }
        }

        void MemberItem_IconClick(object sender, EventArgs e)
        {
        }

        void MemberItem_TextClick(object sender, EventArgs e)
        {
            MemberItemControl ctrl = sender as MemberItemControl;
            if (ctrl != null)
            {
                Raise_ItemSelected(ctrl.Info.UniqueName);
            }
        }

        public event EventHandler<ItemSelectedEventArgs> ItemSelected;

        void Raise_ItemSelected(String uniqueName)
        {
            EventHandler<ItemSelectedEventArgs> handler = ItemSelected;
            if (handler != null)
            { 
                handler(this, new ItemSelectedEventArgs(uniqueName));
            }
        }
        
    }
}
