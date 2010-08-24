/*   
    Copyright (C) 2009 Galaktika Corporation ZAO

    This file is a part of Ranet.UILibrary.Olap
 
    Ranet.UILibrary.Olap is a free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
      
    You should have received a copy of the GNU General Public License
    along with Ranet.UILibrary.Olap.  If not, see
  	<http://www.gnu.org/licenses/> 
  
    If GPL v.3 is not suitable for your products or company,
    Galaktika Corp provides Ranet.UILibrary.Olap under a flexible commercial license
    designed to meet your specific usage and distribution requirements.
    If you have already obtained a commercial license from Galaktika Corp,
    you can use this file under those license terms.
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
using Ranet.AgOlap.Controls.General.Tree;
using Ranet.AgOlap.Controls.General.ItemControls;
using Ranet.AgOlap.Controls.Combo;

namespace Ranet.AgOlap.Controls.General
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
