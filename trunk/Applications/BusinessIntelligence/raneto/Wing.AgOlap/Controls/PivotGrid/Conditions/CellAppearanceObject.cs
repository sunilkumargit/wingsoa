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
using System.Windows.Media.Imaging;

namespace Ranet.AgOlap.Controls.PivotGrid.Conditions
{
    public class CellAppearanceObject
    {
        public CellAppearanceObject()
        {
        }

        public CellAppearanceObject(Color backColor, Color bordercolor, Color forecolor)
        {
            m_BackColor = backColor;
            m_BorderColor = bordercolor;
            m_ForeColor = forecolor;
        }

        public CellAppearanceObject(Color backColor, Color bordercolor, Color forecolor, BitmapImage image)
            : this(backColor, bordercolor, forecolor)
        {
            m_CustomImage = image;
        }

        public CellAppearanceObject(Color backColor, Color bordercolor, Color forecolor, CellAppearanceOptions options)
            : this(backColor, bordercolor, forecolor)
        {
            m_Options = options;
        }

        public CellAppearanceObject(Color backColor, Color bordercolor, Color forecolor, BitmapImage image, CellAppearanceOptions options)
            : this(backColor, bordercolor, forecolor, image)
        {
            m_Options = options;
        }

        #region Свойства
        private Color m_BackColor = Colors.White;
        public Color BackColor
        {
            get
            {
                return m_BackColor;
            }
            set
            {
                m_BackColor = value;
                RaisePropertyChanged();
            }
        }

        //private Color m_BackColor2 = Colors.White;
        //public Color BackColor2
        //{
        //    get
        //    {
        //        return m_BackColor2;
        //    }
        //    set
        //    {
        //        m_BackColor2 = value;
        //        RaisePropertyChanged();
        //    }
        //}

        //private LinearGradientMode m_GradientMode;
        //public LinearGradientMode GradientMode
        //{
        //    get
        //    {
        //        return m_GradientMode;
        //    }
        //    set
        //    {
        //        m_GradientMode = value;
        //        RaisePropertyChanged();
        //    }
        //}

        private Color m_BorderColor = Colors.DarkGray;
        public Color BorderColor
        {
            get
            {
                return m_BorderColor;
            }
            set
            {
                m_BorderColor = value;
                RaisePropertyChanged();
            }
        }

        private Color m_ForeColor = Colors.Black;
        public Color ForeColor
        {
            get
            {
                return m_ForeColor;
            }
            set
            {
                m_ForeColor = value;
                RaisePropertyChanged();
            }
        }

        //private Font m_Font;
        //public Font Font
        //{
        //    get
        //    {
        //        return m_Font;
        //    }
        //    set
        //    {
        //        m_Font = value;
        //        RaisePropertyChanged();
        //    }
        //}

        ////[System.Runtime.Serialization.OptionalFieldAttribute]
        //private StyleTextOptions m_TextOptions;
        ////[TypeConverter(typeof(ExpandableObjectConverter))]
        //public StyleTextOptions TextOptions
        //{
        //    get {
        //        if (m_TextOptions == null)
        //            m_TextOptions = new StyleTextOptions();
        //        return m_TextOptions;
        //    }
        //    set {
        //        m_TextOptions = value;
        //        RaisePropertyChanged();
        //    }
        //}

        private CellAppearanceOptions m_Options;
        public CellAppearanceOptions Options
        {
            get
            {
                if (m_Options == null)
                    m_Options = new CellAppearanceOptions();
                return m_Options;
            }
            set 
            {
                m_Options = value;
                RaisePropertyChanged();
            }
        }

        private CellProgressBarOptions m_ProgressBarOptions;
        public CellProgressBarOptions ProgressBarOptions
        {
            get
            {
                if (m_ProgressBarOptions == null)
                    m_ProgressBarOptions = new CellProgressBarOptions();
                return m_ProgressBarOptions;
            }
            set
            {
                m_ProgressBarOptions = value;
                RaisePropertyChanged();
            }
        }

        private BitmapImage m_CustomImage = null;
        /// <summary>
        /// Картинка
        /// </summary>
        public BitmapImage CustomImage
        {
            get
            {
                return m_CustomImage;
            }
            set
            {
                m_CustomImage = value;
            }
        }

        private String m_CustomImageUri = String.Empty;
        /// <summary>
        /// Путь для картинки
        /// </summary>
        public String CustomImageUri
        {
            get
            {
                return m_CustomImageUri;
            }
            set
            {
                m_CustomImageUri = value;
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
