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
using Ranet.AgOlap.Controls.General.Tree;

namespace Ranet.AgOlap.Controls.Combo
{
    public class CheckedComboBox : ComboBox
    {
        public CheckedComboBox()
        {
            base.DefaultStyleKey = typeof(CheckedComboBox);
            Height = 22;
        }

        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            RefreshSelectedItem();
        }

        void RefreshSelectedItem()
        {
            String hint = String.Empty;
            if (m_SelectedItemText != null)
            {
                m_SelectedItemText.Text = GetSelectedItemText();
                hint = m_SelectedItemText.Text;
            }
            ToolTipService.SetToolTip(this, !String.IsNullOrEmpty(hint) ? hint : null);
        }

        public List<ComboBoxItemData> SelectedItems
        {
            get
            {
                List<ComboBoxItemData> ret = new List<ComboBoxItemData>();

                List<ComboBoxItemData> list = ItemsSource as List<ComboBoxItemData>;
                if (list != null)
                {
                    foreach (ComboBoxItemData item in list)
                    {
                        if (item.IsChecked)
                        {
                            ret.Add(item);
                        }
                    }
                }
                return ret;
            }
        }

        TextBlock m_SelectedItemText = null;
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            m_SelectedItemText = base.GetTemplateChild("txtComboboxText") as TextBlock;
            RefreshSelectedItem();
        }

        protected override void OnDropDownOpened(EventArgs e)
        {
            base.OnDropDownOpened(e);
        }

        protected override void OnDropDownClosed(EventArgs e)
        {
            base.OnDropDownClosed(e);
            RefreshSelectedItem();
        }

        String GetSelectedItemText()
        {
            String str = String.Empty;
            List<ComboBoxItemData> list = ItemsSource as List<ComboBoxItemData>;
            if (list != null)
            {
                foreach (ComboBoxItemData item in list)
                {
                    item.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(item_PropertyChanged);
                    item.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(item_PropertyChanged);
                    if (item.IsChecked && !String.IsNullOrEmpty(item.Text))
                    {
                        if (!String.IsNullOrEmpty(str))
                            str += ", ";
                        str += item.Text;
                    }
                }
            }
            return str;
        }

        public bool IsWaiting
        {
            set
            {
                Items.Clear();
                ItemsSource = null;
                if (value)
                {
                    Items.Add(new WaitTreeControl() { Text = Localization.Loading });
                    SelectedIndex = 0;
                }
                IsEnabled = !value;
            }
        }

        void item_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            RefreshSelectedItem();
        }
    }
}
