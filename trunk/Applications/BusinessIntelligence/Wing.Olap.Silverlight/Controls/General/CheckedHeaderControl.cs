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

namespace Wing.AgOlap.Controls.General
{
    public class CheckedHeaderControl : UserControl
    {
        TextBlock m_Caption;
        CheckBox m_CheckBox;

        public String Caption
        {
            get { return m_Caption.Text; }
            set { m_Caption.Text = value; }
        }

        public bool IsChecked
        {
            get { 
                if(m_CheckBox.IsChecked.HasValue)
                    return m_CheckBox.IsChecked.Value;
                return false; 
            }
            set {
                m_CheckBox.IsChecked = new bool?(value);
            }
        }

        public CheckedHeaderControl()
        {
            // Заголовок
            Grid HeaderLayoutRoot = new Grid();
            HeaderLayoutRoot.VerticalAlignment = VerticalAlignment.Center;
            HeaderLayoutRoot.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            HeaderLayoutRoot.ColumnDefinitions.Add(new ColumnDefinition());

            m_CheckBox = new CheckBox() { Width = 16, Height = 16 };
            m_CheckBox.Checked += new RoutedEventHandler(m_CheckBox_Checked);
            m_CheckBox.Unchecked += new RoutedEventHandler(m_CheckBox_Unchecked);
            HeaderLayoutRoot.Children.Add(m_CheckBox);

            m_Caption = new TextBlock() { Margin = new Thickness(3, 0, 0, 0) };
            HeaderLayoutRoot.Children.Add(m_Caption);
            Grid.SetColumn(m_Caption, 1);

            this.Content = HeaderLayoutRoot;
        }

        public event EventHandler CheckedChanged;
        void Raise_CheckedChanged()
        {
            EventHandler handler = CheckedChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }


        void m_CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Raise_CheckedChanged();
        }

        void m_CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Raise_CheckedChanged();
        }

        public CheckedHeaderControl(bool isChecked, String caption)
            : this()
        {
            IsChecked = isChecked;
            Caption = caption;
        }
    }
}
