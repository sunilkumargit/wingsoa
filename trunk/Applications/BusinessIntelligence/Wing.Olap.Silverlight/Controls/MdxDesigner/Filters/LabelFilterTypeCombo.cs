/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System.Windows.Controls;
using Wing.Olap.Controls.Combo;

namespace Wing.Olap.Controls.MdxDesigner.Filters
{
    public class LabelFilterTypeCombo : UserControl
    {
        ComboBox comboBox;

        public LabelFilterTypeCombo()
        {
            Grid LayoutRoot = new Grid();

            comboBox = new RanetComboBox();
            LayoutRoot.Children.Add(comboBox);

            comboBox.Items.Add(new LabelFilterTypeItemControl(LabelFilterTypes.Equal));
            comboBox.Items.Add(new LabelFilterTypeItemControl(LabelFilterTypes.NotEqual));
            comboBox.Items.Add(new LabelFilterTypeItemControl(LabelFilterTypes.Greater));
            comboBox.Items.Add(new LabelFilterTypeItemControl(LabelFilterTypes.GreaterOrEqual));
            comboBox.Items.Add(new LabelFilterTypeItemControl(LabelFilterTypes.Less));
            comboBox.Items.Add(new LabelFilterTypeItemControl(LabelFilterTypes.LessOrEqual));
            comboBox.Items.Add(new LabelFilterTypeItemControl(LabelFilterTypes.Between));
            comboBox.Items.Add(new LabelFilterTypeItemControl(LabelFilterTypes.NotBetween));
            comboBox.Items.Add(new LabelFilterTypeItemControl(LabelFilterTypes.BeginWith));
            comboBox.Items.Add(new LabelFilterTypeItemControl(LabelFilterTypes.NotBeginWith));
            comboBox.Items.Add(new LabelFilterTypeItemControl(LabelFilterTypes.EndWith));
            comboBox.Items.Add(new LabelFilterTypeItemControl(LabelFilterTypes.NotEndWith));
            comboBox.Items.Add(new LabelFilterTypeItemControl(LabelFilterTypes.Contain));
            comboBox.Items.Add(new LabelFilterTypeItemControl(LabelFilterTypes.NotContain));
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

        public LabelFilterTypes CurrentType
        {
            get {
                LabelFilterTypeItemControl ctrl = comboBox.SelectedItem as LabelFilterTypeItemControl;
                if (ctrl != null)
                {
                    return ctrl.Type;
                }
                return LabelFilterTypes.Equal;
            }
        }

        public void SelectItem(LabelFilterTypes type)
        {
            if (comboBox.Items.Count > 0)
                comboBox.SelectedIndex = 0;
            else
                comboBox.SelectedIndex = -1;

            int i = 0;
            foreach (LabelFilterTypeItemControl item in comboBox.Items)
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
