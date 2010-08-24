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

namespace Ranet.AgOlap.Controls.PivotGrid.Conditions
{
    public class CellConditionTypeCombo : UserControl
    {
        ComboBox comboBox;

        public CellConditionTypeCombo()
        {
            Grid LayoutRoot = new Grid();

            comboBox = new RanetComboBox();
            LayoutRoot.Children.Add(comboBox);

            comboBox.Items.Add(new CellConditionTypeItemControl(CellConditionType.None));
            comboBox.Items.Add(new CellConditionTypeItemControl(CellConditionType.Equal));
            comboBox.Items.Add(new CellConditionTypeItemControl(CellConditionType.NotEqual));
            comboBox.Items.Add(new CellConditionTypeItemControl(CellConditionType.Greater));
            comboBox.Items.Add(new CellConditionTypeItemControl(CellConditionType.GreaterOrEqual));
            comboBox.Items.Add(new CellConditionTypeItemControl(CellConditionType.Less));
            comboBox.Items.Add(new CellConditionTypeItemControl(CellConditionType.LessOrEqual));
            comboBox.Items.Add(new CellConditionTypeItemControl(CellConditionType.Between));
            comboBox.Items.Add(new CellConditionTypeItemControl(CellConditionType.NotBetween));
            comboBox.SelectionChanged += new SelectionChangedEventHandler(comboBox_SelectionChanged);

            comboBox.SelectedIndex = 0;
            this.Content = LayoutRoot;
        }

        public event SelectionChangedEventHandler SelectionChanged;
        void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectionChangedEventHandler handler = SelectionChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public CellConditionType CurrentType
        {
            get {
                CellConditionTypeItemControl ctrl = comboBox.SelectedItem as CellConditionTypeItemControl;
                if (ctrl != null)
                {
                    return ctrl.Type;
                }
                return CellConditionType.None;
            }
        }

        public void SelectItem(CellConditionType type)
        {
            if (comboBox.Items.Count > 0)
                comboBox.SelectedIndex = 0;
            else
                comboBox.SelectedIndex = -1;

            int i = 0;
            foreach (CellConditionTypeItemControl item in comboBox.Items)
            {
                if (item.Type == type)
                {
                    comboBox.SelectedIndex = i;
                    break;
                }
                i++;
            }
        }
    }
}