/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Windows;
using System.Windows.Controls;

namespace Wing.Olap.Controls.ToolBar
{
    public class ZoomingToolBarControl : UserControl
    {
        NumericUpDown m_UpDown;
        public ZoomingToolBarControl()
        {
            Grid LayoutRoot = new Grid();
            m_UpDown = new NumericUpDown();
            m_UpDown.Minimum = 10;
            m_UpDown.Maximum = 1000000;
            m_UpDown.Increment = 10;
            m_UpDown.Value = 100;
            Height = 22;
            Width = 50;
            this.Margin = new Thickness(2, 0, 0, 0);
            LayoutRoot.Children.Add(m_UpDown);
            m_UpDown.ValueChanged += new RoutedPropertyChangedEventHandler<double>(m_UpDown_ValueChanged);

            this.Content = LayoutRoot;
        }

        public event EventHandler ValueChanged;
        void m_UpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            EventHandler handle = ValueChanged;
            if (handle != null)
            {
                handle(this, EventArgs.Empty);
            }
        }

        public double Value
        { 
            get{
                return m_UpDown.Value;
            }
            set {
                m_UpDown.Value = value;
            }
        }
    }
}
