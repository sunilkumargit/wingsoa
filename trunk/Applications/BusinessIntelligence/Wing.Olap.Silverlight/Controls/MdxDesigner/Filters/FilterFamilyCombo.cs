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
using Wing.Olap.Controls.Combo;

namespace Wing.Olap.Controls.MdxDesigner.Filters
{
    public class FilterFamilyCombo : UserControl
    {
        ComboBox comboBox;

        public FilterFamilyCombo()
        {
            Grid LayoutRoot = new Grid();

            comboBox = new RanetComboBox();
            LayoutRoot.Children.Add(comboBox);

            comboBox.Items.Add(new FilterFamilyItemControl(FilterFamilyTypes.LabelFilter));
            comboBox.Items.Add(new FilterFamilyItemControl(FilterFamilyTypes.ValueFilter));
            comboBox.Items.Add(new FilterFamilyItemControl(FilterFamilyTypes.TopFilter));

            comboBox.SelectedIndex = -1;
            comboBox.SelectionChanged += new SelectionChangedEventHandler(comboBox_SelectionChanged);
            comboBox.SelectedIndex = 0;
            this.Content = LayoutRoot;

            Height = 24;
        }

        void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Raise_SelectionChanged();
        }

        public event EventHandler SelectionChanged;
        void Raise_SelectionChanged()
        {
            EventHandler handler = SelectionChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        public FilterFamilyTypes CurrentType
        {
            get {
                FilterFamilyItemControl ctrl = comboBox.SelectedItem as FilterFamilyItemControl;
                if (ctrl != null)
                {
                    return ctrl.Type;
                }
                return FilterFamilyTypes.TopFilter;
            }
        }

        public void SelectItem(FilterFamilyTypes type)
        {
            int i = 0;
            foreach (FilterFamilyItemControl item in comboBox.Items)
            {
                if (item.Type == type)
                {
                    comboBox.SelectedIndex = i;
                    return;
                }
                i++;
            }

            comboBox.SelectedIndex = -1;
        }
    }
}
