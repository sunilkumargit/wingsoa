/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Windows;
using System.Windows.Controls;

namespace Wing.Olap.Controls.PivotGrid.Controls
{
    public partial class ExpanderControl : UserControl
    {
        public ExpanderControl()
        {
            InitializeComponent();

            this.SizeChanged += new SizeChangedEventHandler(ExpanderControl_SizeChanged);
        }

        void ExpanderControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            border.BorderThickness = new Thickness(Math.Round(Math.Max(1, this.ActualHeight / 15)));
            LayoutRoot.Margin = new Thickness(Math.Round(Math.Max(1, (2 * this.ActualHeight / 15))));

            //if (this.ActualHeight % 2 != 0)
            {
                LayoutRoot.ColumnDefinitions[1].Width = new GridLength(Math.Round(Math.Max(1, this.ActualHeight / 15)));
                LayoutRoot.RowDefinitions[1].Height = new GridLength(Math.Round(Math.Max(1, this.ActualHeight / 15)));
                //line1.StrokeThickness = 0.5 * this.ActualHeight / 13;
                //line2.StrokeThickness = 0.5 * this.ActualHeight / 13;
                border1.BorderThickness = new Thickness(Math.Round(Math.Max(1, this.ActualHeight / 15)));
                border2.BorderThickness = new Thickness(Math.Round(Math.Max(1, this.ActualHeight / 15)));
            }
        }

        public bool IsExpanded
        {
            get
            {
                if (border2.Visibility == Visibility.Visible)
                    return false;
                return true;
            }
            set
            {
                if (value)
                {
                    border2.Visibility = Visibility.Collapsed;
                }
                else
                {
                    border2.Visibility = Visibility.Visible;
                }
            }
        }
    }
}
