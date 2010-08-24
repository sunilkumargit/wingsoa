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

namespace Ranet.AgOlap.Controls.PivotGrid.Conditions
{
    public class CellProgressBarOptions
    {
        public CellProgressBarOptions()
        {
        }

        #region Свойства
        private bool m_IsIndeterminate = false;
        public bool IsIndeterminate
        { 
            get{
                return m_IsIndeterminate;
            }
            set {
                m_IsIndeterminate = value;
                RaisePropertyChanged();
            }
        }

        private Color m_StartColor = Colors.Red;
        public Color StartColor
        {
            get
            {
                return m_StartColor;
            }
            set
            {
                m_StartColor = value;
                RaisePropertyChanged();
            }
        }

        private Color m_EndColor = Colors.Green;
        public Color EndColor
        {
            get
            {
                return m_EndColor;
            }
            set
            {
                m_EndColor = value;
                RaisePropertyChanged();
            }
        }

        private double m_MinValue = 0;
        public double MinValue
        {
            get
            {
                return m_MinValue;
            }
            set
            {
                m_MinValue = value;
                RaisePropertyChanged();
            }
        }

        private double m_MaxValue = 1;
        public double MaxValue
        {
            get
            {
                return m_MaxValue;
            }
            set
            {
                m_MaxValue = value;
                RaisePropertyChanged();
            }
        }
        #endregion Свойства

        /// <summary>
        /// Событие - произошло изменение свойств
        /// </summary>
        public event EventHandler PropertyChanged;

        protected void RaisePropertyChanged()
        {

            EventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        void m_Appearance_Changed(object sender, EventArgs e)
        {
            RaisePropertyChanged();
        }
    }
}
