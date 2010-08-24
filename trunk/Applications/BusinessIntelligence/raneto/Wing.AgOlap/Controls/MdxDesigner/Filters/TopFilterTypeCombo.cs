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
    public class TopFilterTypeCombo : UserControl
    {
        ComboBox comboBox;

        public TopFilterTypeCombo()
        {
            Grid LayoutRoot = new Grid();

            comboBox = new RanetComboBox();
            LayoutRoot.Children.Add(comboBox);

            comboBox.Items.Add(new TopFilterTypeItemControl(TopFilterTypes.Top));
            comboBox.Items.Add(new TopFilterTypeItemControl(TopFilterTypes.Bottom));

            comboBox.SelectedIndex = 0;
            this.Content = LayoutRoot;
        }

        public TopFilterTypes CurrentType
        {
            get {
                TopFilterTypeItemControl ctrl = comboBox.SelectedItem as TopFilterTypeItemControl;
                if (ctrl != null)
                {
                    return ctrl.Type;
                }
                return TopFilterTypes.Top;
            }
        }

        public void SelectItem(TopFilterTypes type)
        {
            if (comboBox.Items.Count > 0)
                comboBox.SelectedIndex = 0;
            else
                comboBox.SelectedIndex = -1;

            int i = 0;
            foreach (TopFilterTypeItemControl item in comboBox.Items)
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
