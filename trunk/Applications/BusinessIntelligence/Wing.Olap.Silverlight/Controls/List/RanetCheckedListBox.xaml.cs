/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/
using System;
using System.Windows;
using System.Windows.Controls;

namespace Wing.Olap.Controls.List
{
    public partial class RanetCheckedListBox : UserControl
    {
        public RanetCheckedListBox()
        {            
            InitializeComponent();

            this.ListBox.SelectionMode = System.Windows.Controls.SelectionMode.Multiple;
            this.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            this.ListBox.ItemContainerStyle = Resources["CheckedItemStyle"] as Style;

            m_DataSource = new ObservableObjectCollection();
        }

        //protected override DependencyObject GetContainerForItemOverride()
        //{
        //    RanetCheckedItem item = new RanetCheckedItem();
        //    if (this.ItemContainerStyle != null)
        //    {
        //        item.Style = this.ItemContainerStyle;
        //    }
        //    return item;
        //}

        //protected override bool IsItemItsOwnContainerOverride(object item)
        //{
        //    return (item is RanetCheckedItem);
        //}

        private ObservableObjectCollection m_DataSource;
        public ObservableObjectCollection DataSource
        {
            get { return m_DataSource; }
            set { m_DataSource = value; }
        }

        public void AddItem(RanetCheckedItem item)
        {
            if (this.m_DataSource != null)
            {
                this.m_DataSource.Add(item.Text);
                this.ListBox.Items.Add(item);
            }
        }

    }

    public class RanetCheckedItem : ListBoxItem
    {
        public static readonly DependencyProperty IsCheckedProperty;
        public static readonly DependencyProperty TextProperty;

        static RanetCheckedItem()
        {
            IsCheckedProperty = DependencyProperty.Register("IsChecked", typeof(bool), typeof(RanetCheckedItem), new PropertyMetadata(false, ValueChangedCallback));
            TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(RanetCheckedItem), new PropertyMetadata(string.Empty, TextChangedCallback));
        }

        public RanetCheckedItem()
        {
            DefaultStyleKey = typeof(RanetCheckedItem);
        }

        static void ValueChangedCallback(object obj, DependencyPropertyChangedEventArgs args)
        {
            var r = obj as RanetCheckedItem;
            if (r == null)
                return;
            if (args.Property == RanetCheckedItem.IsCheckedProperty)
                (obj as RanetCheckedItem).IsChecked = Convert.ToBoolean(args.NewValue);
        }

        static void TextChangedCallback(object obj, DependencyPropertyChangedEventArgs args)
        {
            var r = obj as RanetCheckedItem;
            if (r == null)
                return;

            (r as RanetCheckedItem).Text = args.NewValue.ToString();
        }

        public bool IsChecked
        {
            get { return base.IsSelected; }
            set { base.IsSelected = value; }
        }

        public string Text
        {
            get;
            set;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            var checkBox = base.GetTemplateChild("checkBox");
            if ((checkBox is CheckBox))
            {
                (checkBox as CheckBox).IsChecked = this.IsChecked;
                (checkBox as CheckBox).Content = this.Text;
                (checkBox as CheckBox).Checked += new RoutedEventHandler(RanetCheckedItem_Checked);
                (checkBox as CheckBox).Unchecked += new RoutedEventHandler(RanetCheckedItem_Unchecked);
            }
        }

        void RanetCheckedItem_Unchecked(object sender, RoutedEventArgs e)
        {
            this.IsSelected = false;
        }

        void RanetCheckedItem_Checked(object sender, RoutedEventArgs e)
        {
            this.IsSelected = true;
        }
    }
}
