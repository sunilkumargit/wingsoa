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
using System.Collections.Generic;
using Wing.Olap.Controls.MemberChoice.Info;
using Wing.Olap.Controls.General.ItemControls;

namespace Wing.Olap.Controls.MemberChoice
{
    public class ItemSelectedEventArgs : EventArgs
    {
        public String UniqueName { get; internal set; }
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
