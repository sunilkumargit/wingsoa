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
using Ranet.Olap.Core.Metadata;
using Ranet.AgOlap.Controls.General.ItemControls;
using Ranet.AgOlap.Controls.Combo;

namespace Ranet.AgOlap.Controls.MdxDesigner.Filters
{
    public class MeasuresCombo : UserControl
    {
        ComboBox comboMeasures;

        public MeasuresCombo()
        {
            Grid LayoutRoot = new Grid();

            comboMeasures = new RanetComboBox();
            LayoutRoot.Children.Add(comboMeasures);
            this.Content = LayoutRoot;
        }

        public void Initialize(CubeDefInfo cubeInfo)
        {
            m_CubeInfo = cubeInfo;
            CreateMeasures();
        }

        void CreateMeasures()
        {
            comboMeasures.Items.Clear();
            if (m_CubeInfo != null)
            {
                foreach (MeasureInfo info in m_CubeInfo.Measures)
                {
                    comboMeasures.Items.Add(new MeasureItemControl(info));
                }
            }
            if (comboMeasures.Items.Count > 0)
                comboMeasures.SelectedIndex = 0;
        }

        CubeDefInfo m_CubeInfo = null;

        public MeasureInfo CurrentMeasure
        {
            get {
                MeasureItemControl ctrl = comboMeasures.SelectedItem as MeasureItemControl;
                if (ctrl != null)
                {
                    return ctrl.Info;
                }
                return null;
            }
        }

        public void SelectItem(String uniqueName)
        {
            if(comboMeasures.Items.Count > 0)
                comboMeasures.SelectedIndex = 0;
            else
                comboMeasures.SelectedIndex = -1;
            
            if (!String.IsNullOrEmpty(uniqueName))
            {
                int i = 0;
                foreach (MeasureItemControl item in comboMeasures.Items)
                {
                    if (item.Info.UniqueName == uniqueName)
                    {
                        comboMeasures.SelectedIndex = i;
                        break;
                    }
                    i++;
                }
            }
        }
    }
}
