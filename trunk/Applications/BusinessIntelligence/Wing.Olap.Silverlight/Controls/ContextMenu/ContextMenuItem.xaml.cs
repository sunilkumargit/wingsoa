/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Wing.Olap.Controls.ContextMenu
{
    public partial class ContextMenuItem : UserControl
    {
        public event EventHandler ItemClick;
        Image m_SubMenuIcon;

        public ContextMenuItem(String caption)
        {
            InitializeComponent();

            Grid grdLayout = new Grid();
            grdLayout.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            grdLayout.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            grdLayout.ColumnDefinitions.Add(new ColumnDefinition());
            grdLayout.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

            m_Icon = new Image();
            m_Icon.Margin = new Thickness(4, 4, 4, 4);
            m_Icon.Width = 16;
            m_Icon.Height = 16;

            btnContent.Padding = new Thickness(0, 0, 0, 0);

            Border border = new Border();
            border.Width = 1;
            border.Background = new SolidColorBrush(Colors.DarkGray);
            border.Margin = new Thickness(2, 0, 0, 0);

            m_Text = new TextBlock();
            m_Text.Margin = new Thickness(4, 4, 4, 4);
            m_Text.Padding = new Thickness(5, 0, 0, 0);
            m_Text.Text = caption;
            m_Text.Foreground = new SolidColorBrush(Colors.Black);

            m_ShortcutText = new TextBlock();
            m_ShortcutText.Margin = new Thickness(0, 4, 4, 4);
            m_ShortcutText.Foreground = new SolidColorBrush(Colors.Black);

            m_SubMenuIcon = new Image();
            m_SubMenuIcon.Margin = new Thickness(4, 4, 2, 4);
            m_SubMenuIcon.Width = 16;
            m_SubMenuIcon.Height = 16;
            m_SubMenuIcon.Visibility = Visibility.Collapsed;
            m_SubMenuIcon.Source = UriResources.Images.SubMenu16;

            grdLayout.Children.Add(m_Icon);
            grdLayout.Children.Add(border);
            Grid.SetColumn(border, 1);
            grdLayout.Children.Add(m_Text);
            Grid.SetColumn(m_Text, 2);
            grdLayout.Children.Add(m_ShortcutText);
            Grid.SetColumn(m_ShortcutText, 3);
            grdLayout.Children.Add(m_SubMenuIcon);
            Grid.SetColumn(m_SubMenuIcon, 3);

            btnContent.Content = grdLayout;

            btnContent.HorizontalContentAlignment = HorizontalAlignment.Stretch;

            btnContent.Click += new RoutedEventHandler(btnContent_Click);

            this.Loaded += new RoutedEventHandler(ContextMenuItem_Loaded);
        }

        void ContextMenuItem_Loaded(object sender, RoutedEventArgs e)
        {
            if (HasSubMenu)
            {
                m_ShortcutText.Visibility = Visibility.Collapsed;
                m_SubMenuIcon.Visibility = Visibility.Visible;
            }
            else
            {
                m_ShortcutText.Visibility = Visibility.Visible;
                m_SubMenuIcon.Visibility = Visibility.Collapsed;
            }
        }

        public bool HasSubMenu
        {
            get { return SubMenu != null && SubMenu.Items.Count > 0; }
        }

        public CustomContextMenu SubMenu;

        Image m_Icon = null;
        TextBlock m_Text = null;
        TextBlock m_ShortcutText = null;

        public string Caption
        {
            get
            {
                return m_Text.Text;
            }
            set
            {
                m_Text.Text = value;
            }
        }

        public string Shortcut
        {
            get
            {
                return m_ShortcutText.Text;
            }
            set
            {
                m_ShortcutText.Text = value;
            }
        }

        public BitmapImage Icon
        {
            set
            {
                m_Icon.Source = value;
            }
            get
            {
                return m_Icon.Source as BitmapImage;
            }
        }

        public bool m_IsEnabled = true;
        public new bool IsEnabled
        {
            get {
                return m_IsEnabled;
            }
            set {
                if (value)
                {
                    btnContent.Click -= new RoutedEventHandler(btnContent_Click);
                    btnContent.Click += new RoutedEventHandler(btnContent_Click);
                    m_Text.Foreground = m_ShortcutText.Foreground = new SolidColorBrush(Colors.Black);
                }
                else
                {
                    btnContent.Click -= new RoutedEventHandler(btnContent_Click);
                    m_Text.Foreground = m_ShortcutText.Foreground = new SolidColorBrush(Colors.DarkGray);
                }
                m_IsEnabled = value;
            }
        }

        protected virtual void btnContent_Click(object sender, RoutedEventArgs e)
        {
            EventHandler handler = ItemClick;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}
