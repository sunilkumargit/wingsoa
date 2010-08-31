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

namespace Wing.AgOlap.Controls.PivotGrid.Conditions
{
    public class CellAppearanceOptions
    {
        public CellAppearanceOptions()
        {
        }

        #region Свойства
        private bool m_ShowValue = true;
        public bool ShowValue
        {
            get
            {
                return m_ShowValue;
            }
            set
            {
                if (m_ShowValue != value)
                {
                    m_ShowValue = value;
                }
                RaisePropertyChanged();
            }
        }

        private bool m_UseBackColor = false;
        public bool UseBackColor
        {
            get
            {
                return m_UseBackColor;
            }
            set
            {
                m_UseBackColor = value;
                RaisePropertyChanged();
            }
        }

        private bool m_UseBorderColor = false;
        public bool UseBorderColor
        {
            get
            {
                return m_UseBorderColor;
            }
            set
            {
                m_UseBorderColor = value;
                RaisePropertyChanged();
            }
        }

        private bool m_UseForeColor = false;
        public bool UseForeColor
        {
            get
            {
                return m_UseForeColor;
            }
            set
            {
                m_UseForeColor = value;
                RaisePropertyChanged();
            }
        }

        ////[System.Runtime.Serialization.OptionalFieldAttribute]
        //private bool m_UseFont;
        //[DefaultValue(false)]
        //public bool UseFont
        //{
        //    get
        //    {
        //        return m_UseFont;
        //    }
        //    set
        //    {
        //        m_UseFont = value;
        //        RaisePropertyChanged();
        //    }
        //}

        private bool m_UseImage = false;
        public bool UseImage
        {
            get
            {
                return m_UseImage;
            }
            set
            {
                m_UseImage = value;
                RaisePropertyChanged();
            }
        }

        //[System.Runtime.Serialization.OptionalFieldAttribute]
        //private bool m_UseTextOptions;
        //[DefaultValue(false)]
        //public bool UseTextOptions
        //{
        //    get
        //    {
        //        return m_UseTextOptions;
        //    }
        //    set
        //    {
        //        m_UseTextOptions = value;
        //        RaisePropertyChanged();
        //    }
        //}

        private bool m_UseProgressBar = false;
        public bool UseProgressBar
        {
            get
            {
                return m_UseProgressBar;
            }
            set
            {
                m_UseProgressBar = value;
                RaisePropertyChanged();
            }
        }

        //[System.Runtime.Serialization.OptionalFieldAttribute]
        private bool m_UseAllOptions = false;
        public bool UseAllOptions
        {
            get
            {
                return m_UseAllOptions;
            }
            set
            {
                m_UseAllOptions = value;
                RaisePropertyChanged();
                if (value)
                {
                    IgnoreAllOptions = !value;
                }
            }
        }

        private bool m_IgnoreAllOptions = false;
        public bool IgnoreAllOptions
        {
            get
            {
                return m_IgnoreAllOptions;
            }
            set
            {
                m_IgnoreAllOptions = value;
                RaisePropertyChanged();
                if (value)
                {
                    UseAllOptions = !value;
                }
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
