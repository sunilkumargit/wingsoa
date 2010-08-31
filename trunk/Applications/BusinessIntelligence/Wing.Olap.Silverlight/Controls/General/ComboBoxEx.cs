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
using Wing.Olap.Controls.General.Tree;
using Wing.Olap.Controls.General.ItemControls;
using Wing.Olap.Controls.Combo;

namespace Wing.Olap.Controls.General
{
    public class ComboBoxEx : UserControl
    {
        ContentControl m_ComboContent;
        ComboBox m_Combo;
        public ComboBox Combo
        {
            get {
                return m_Combo;
            }
        }

        public ComboBoxEx()
        {
            m_ComboContent = new ContentControl();
            m_ComboContent.HorizontalAlignment = HorizontalAlignment.Stretch;
            m_ComboContent.HorizontalContentAlignment = HorizontalAlignment.Stretch;

            m_Combo = new RanetComboBox();
            m_Combo.Height = 22;
            m_ComboContent.Content = m_Combo;
            m_Combo.SelectionChanged += new SelectionChangedEventHandler(m_Combo_SelectionChanged);

            this.Content = m_ComboContent;
        }

        public void Clear()
        {
            m_Combo.SelectionChanged -= new SelectionChangedEventHandler(m_Combo_SelectionChanged);
            m_Combo.Items.Clear();
            //m_Combo.SelectionChanged -= new SelectionChangedEventHandler(m_Combo_SelectionChanged);
            //m_Combo = new RanetComboBox();
            //m_Combo.Height = 22;
            //m_ComboContent.Content = m_Combo;
            //m_Combo.SelectionChanged += new SelectionChangedEventHandler(m_Combo_SelectionChanged);
            m_Combo.SelectionChanged += new SelectionChangedEventHandler(m_Combo_SelectionChanged);
        }

        public event SelectionChangedEventHandler SelectionChanged;

        void m_Combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectionChangedEventHandler handler = SelectionChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public bool IsWaiting
        {
            set {
                if (value == true)
                {
                    Clear();

                    String NODE_TEXT = Localization.Loading;
                    WaitTreeControl ctrl = new WaitTreeControl() { Text = Localization.Loading };
                    Combo.Items.Add(ctrl);
                    Combo.SelectedIndex = 0;
                }
                else
                {
                    Clear();
                }
            }
        }

        public bool IsError
        {
            set
            {
                if (value == true)
                {
                    Clear();

                    ErrorItemControl ctrl = new ErrorItemControl();
                    Combo.Items.Add(ctrl);
                    Combo.SelectedIndex = 0;
                }
                else
                {
                    Clear();
                }
            }
        }
    }
}
