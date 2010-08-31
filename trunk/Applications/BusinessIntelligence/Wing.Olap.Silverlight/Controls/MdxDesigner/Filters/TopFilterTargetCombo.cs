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
