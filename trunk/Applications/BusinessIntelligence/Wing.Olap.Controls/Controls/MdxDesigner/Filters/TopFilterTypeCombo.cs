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
using Wing.AgOlap.Controls.Combo;

namespace Wing.AgOlap.Controls.MdxDesigner.Filters
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
