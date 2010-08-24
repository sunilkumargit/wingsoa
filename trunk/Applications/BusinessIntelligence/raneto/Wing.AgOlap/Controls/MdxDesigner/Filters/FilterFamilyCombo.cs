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
using Ranet.AgOlap.Controls.Combo;

namespace Ranet.AgOlap.Controls.MdxDesigner.Filters
{
    public class FilterFamilyCombo : UserControl
    {
        ComboBox comboBox;

        public FilterFamilyCombo()
        {
            Grid LayoutRoot = new Grid();

            comboBox = new RanetComboBox();
            LayoutRoot.Children.Add(comboBox);

            comboBox.Items.Add(new FilterFamilyItemControl(FilterFamilyTypes.LabelFilter));
            comboBox.Items.Add(new FilterFamilyItemControl(FilterFamilyTypes.ValueFilter));
            comboBox.Items.Add(new FilterFamilyItemControl(FilterFamilyTypes.TopFilter));

            comboBox.SelectedIndex = -1;
            comboBox.SelectionChanged += new SelectionChangedEventHandler(comboBox_SelectionChanged);
            comboBox.SelectedIndex = 0;
            this.Content = LayoutRoot;

            Height = 24;
        }

        void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Raise_SelectionChanged();
        }

        public event EventHandler SelectionChanged;
        void Raise_SelectionChanged()
        {
            EventHandler handler = SelectionChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        public FilterFamilyTypes CurrentType
        {
            get {
                FilterFamilyItemControl ctrl = comboBox.SelectedItem as FilterFamilyItemControl;
                if (ctrl != null)
                {
                    return ctrl.Type;
                }
                return FilterFamilyTypes.TopFilter;
            }
        }

        public void SelectItem(FilterFamilyTypes type)
        {
            int i = 0;
            foreach (FilterFamilyItemControl item in comboBox.Items)
            {
                if (item.Type == type)
                {
                    comboBox.SelectedIndex = i;
                    return;
                }
                i++;
            }

            comboBox.SelectedIndex = -1;
        }
    }
}
