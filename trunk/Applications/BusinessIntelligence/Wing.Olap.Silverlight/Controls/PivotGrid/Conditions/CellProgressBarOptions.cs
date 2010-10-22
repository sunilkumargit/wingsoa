/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Windows.Media;

namespace Wing.Olap.Controls.PivotGrid.Conditions
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
            get
            {
                return m_IsIndeterminate;
            }
            set
            {
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
