/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Windows.Controls;
using Wing.Olap.Controls.Combo;
using Wing.Olap.Controls.General.ItemControls;
using Wing.Olap.Core.Metadata;

namespace Wing.Olap.Controls.MdxDesigner.Filters
{
    public class LevelPropertyCombo : UserControl
    {
        ComboBox comboBox;

        public LevelPropertyCombo()
        {
            Grid LayoutRoot = new Grid();

            comboBox = new RanetComboBox();
            LayoutRoot.Children.Add(comboBox);

            comboBox.Items.Add(new LevelPropertyItemControl(new LevelPropertyInfo("Caption", "Caption")));
            comboBox.Items.Add(new LevelPropertyItemControl(new LevelPropertyInfo("Name", "Name")));
            comboBox.Items.Add(new LevelPropertyItemControl(new LevelPropertyInfo("UniqueName", "UNIQUE_NAME")));

            comboBox.SelectedIndex = 0;
            this.Content = LayoutRoot;
        }

        public String CurrentProperty
        {
            get {
                LevelPropertyItemControl ctrl = comboBox.SelectedItem as LevelPropertyItemControl;
                if (ctrl != null)
                {
                    return ctrl.Info.Name;
                }
                return String.Empty;
            }
        }

        public void SelectItem(String name)
        {
            if (comboBox.Items.Count > 0)
                comboBox.SelectedIndex = 0;
            else
                comboBox.SelectedIndex = -1;

            int i = 0;
            foreach (LevelPropertyItemControl item in comboBox.Items)
            {
                if (item.Info.Name == name)
                {
                    comboBox.SelectedIndex = i;
                    break;
                }
                i++;
            }
        }
    }
}
