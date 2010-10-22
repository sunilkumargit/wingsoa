/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System.Windows.Controls;
using Wing.Olap.Controls.Combo;

namespace Wing.Olap.Controls.MdxDesigner.Filters
{
    public class ValueFilterTypeCombo : UserControl
    {
        ComboBox comboBox;

        public ValueFilterTypeCombo()
        {
            Grid LayoutRoot = new Grid();

            comboBox = new RanetComboBox();
            LayoutRoot.Children.Add(comboBox);

            comboBox.Items.Add(new ValueFilterTypeItemControl(ValueFilterTypes.Equal));
            comboBox.Items.Add(new ValueFilterTypeItemControl(ValueFilterTypes.NotEqual));
            comboBox.Items.Add(new ValueFilterTypeItemControl(ValueFilterTypes.Greater));
            comboBox.Items.Add(new ValueFilterTypeItemControl(ValueFilterTypes.GreaterOrEqual));
            comboBox.Items.Add(new ValueFilterTypeItemControl(ValueFilterTypes.Less));
            comboBox.Items.Add(new ValueFilterTypeItemControl(ValueFilterTypes.LessOrEqual));
            comboBox.Items.Add(new ValueFilterTypeItemControl(ValueFilterTypes.Between));
            comboBox.Items.Add(new ValueFilterTypeItemControl(ValueFilterTypes.NotBetween));
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

        public ValueFilterTypes CurrentType
        {
            get {
                ValueFilterTypeItemControl ctrl = comboBox.SelectedItem as ValueFilterTypeItemControl;
                if (ctrl != null)
                {
                    return ctrl.Type;
                }
                return ValueFilterTypes.Equal;
            }
        }

        public void SelectItem(ValueFilterTypes type)
        {
            if (comboBox.Items.Count > 0)
                comboBox.SelectedIndex = 0;
            else
                comboBox.SelectedIndex = -1;

            int i = 0;
            foreach (ValueFilterTypeItemControl item in comboBox.Items)
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
