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
using System.Windows;
using System.Windows.Controls;

namespace Ranet.AgOlap.Controls.List
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
