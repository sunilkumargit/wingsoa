/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.ComponentModel;

namespace Wing.Olap.Controls.Combo
{
    public class ComboBoxItemData : INotifyPropertyChanged
    {
        private string m_Text = String.Empty;
        public string Text
        {
            get { return m_Text; }
            set
            {
                m_Text = value;
                NotifyPropertyChanged("Text");
            }
        }

        private bool m_IsChecked = false;
        public bool IsChecked
        {
            get { return m_IsChecked; }
            set
            {
                m_IsChecked = value;
                NotifyPropertyChanged("IsChecked");
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
