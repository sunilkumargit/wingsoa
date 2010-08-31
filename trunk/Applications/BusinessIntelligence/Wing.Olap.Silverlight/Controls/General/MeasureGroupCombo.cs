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
using Wing.AgOlap.Controls.General.ItemControls;
using Wing.Olap.Core.Metadata;
using System.Collections.Generic;
using Wing.AgOlap.Controls.General.Tree;

namespace Wing.AgOlap.Controls.General
{
    public class MeasureGroupCombo : UserControl
    {
        public const String ALL_MEASURES_GROUPS = "<ALL_MEASURES_GROUPS>";
        ComboBoxEx comboBox;

        public MeasureGroupCombo()
        {
            Grid LayoutRoot = new Grid();
            Height = 22;

            comboBox = new ComboBoxEx();
            LayoutRoot.Children.Add(comboBox);

            comboBox.Combo.Items.Add(new MeasureGroupItemControl(new MeasureGroupInfo() { Name = MeasureGroupCombo.ALL_MEASURES_GROUPS, Caption = Localization.MeasureGroup_All }));
            comboBox.Combo.SelectedIndex = 0;
            comboBox.SelectionChanged += new SelectionChangedEventHandler(comboBox_SelectionChanged);

            this.Content = LayoutRoot;
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

        public MeasureGroupInfo CurrentItem
        {
            get {
                MeasureGroupItemControl ctrl = comboBox.Combo.SelectedItem as MeasureGroupItemControl;
                if (ctrl != null)
                {
                    return ctrl.Info;
                }
                return null;
            }
        }

        public void Initialize(IList<MeasureGroupInfo> list)
        {
            comboBox.Clear();

            comboBox.Combo.Items.Add(new MeasureGroupItemControl(new MeasureGroupInfo() { Name = MeasureGroupCombo.ALL_MEASURES_GROUPS, Caption = Localization.MeasureGroup_All }));
            if (list != null)
            {
                foreach (MeasureGroupInfo info in list)
                {
                    comboBox.Combo.Items.Add(new MeasureGroupItemControl(info));
                }
            }
            
            comboBox.Combo.SelectedIndex = 0;
        }


        public bool SelectItem(String name)
        {
            int i = 0;
            foreach (object item in comboBox.Combo.Items)
            {
                var measure_item = item as MeasureGroupItemControl;
                if(measure_item != null && measure_item.Info.Name == name)
                {
                    comboBox.Combo.SelectedIndex = i;
                    return true;
                }
                i++;
            }
            if (comboBox.Combo.Items.Count > 0)
                comboBox.Combo.SelectedIndex = 0;
            else
                comboBox.Combo.SelectedIndex = -1;
            return false;
        }

        public bool IsWaiting
        {
            set
            {
                if (value == true)
                {
                    comboBox.Clear();

                    String NODE_TEXT = Localization.Loading;
                    WaitTreeControl ctrl = new WaitTreeControl() { Text = Localization.Loading };
                    comboBox.Combo.Items.Add(ctrl);
                    comboBox.Combo.SelectedIndex = 0;
                }
                else
                {
                    comboBox.Clear();
                    comboBox.Combo.Items.Add(new MeasureGroupItemControl(new MeasureGroupInfo() { Name = MeasureGroupCombo.ALL_MEASURES_GROUPS, Caption = Localization.MeasureGroup_All }));
                    comboBox.Combo.SelectedIndex = 0;
                }
            }
        }
    }
}
