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
