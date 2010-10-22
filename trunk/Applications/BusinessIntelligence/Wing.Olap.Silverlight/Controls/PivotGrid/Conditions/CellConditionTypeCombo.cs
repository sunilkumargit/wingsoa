/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System.Windows.Controls;
using Wing.Olap.Controls.Combo;

namespace Wing.Olap.Controls.PivotGrid.Conditions
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
            get
            {
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