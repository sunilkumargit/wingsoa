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
    public class TopFilterTargetCombo : UserControl
    {
        ComboBox comboBox;

        public TopFilterTargetCombo()
        {
            Grid LayoutRoot = new Grid();

            comboBox = new RanetComboBox();
            LayoutRoot.Children.Add(comboBox);

            comboBox.Items.Add(new TopFilterTargetItemControl(TopFilterTargetTypes.Items));
            comboBox.Items.Add(new TopFilterTargetItemControl(TopFilterTargetTypes.Percent));
            comboBox.Items.Add(new TopFilterTargetItemControl(TopFilterTargetTypes.Sum));

            comboBox.SelectedIndex = 0;
            this.Content = LayoutRoot;
        }

        public TopFilterTargetTypes CurrentType
        {
            get {
                TopFilterTargetItemControl ctrl = comboBox.SelectedItem as TopFilterTargetItemControl;
                if (ctrl != null)
                {
                    return ctrl.Type;
                }
                return TopFilterTargetTypes.Items;
            }
        }

        public void SelectItem(TopFilterTargetTypes type)
        {
            if (comboBox.Items.Count > 0)
                comboBox.SelectedIndex = 0;
            else
                comboBox.SelectedIndex = -1;

            int i = 0;
            foreach (TopFilterTargetItemControl item in comboBox.Items)
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
