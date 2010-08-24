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
using Ranet.AgOlap.Controls.General;

namespace Ranet.AgOlap.Controls
{
    public class DateChoicePopUp : AgPopUpControlBase
    {
        DateChoiceCtrl m_ChoiceControl = null;
        protected DateChoiceCtrl ChoiceControl
        {
            get
            {
                return m_ChoiceControl;
            }
        }

        public new double Height
        {
            get { return Height; }
            set
            {
                base.Height = value;
                PopUpContainer.Height = value;
            }
        }

        protected override void OnBeforePopUp(object sender, EventArgs e)
        {
            if (m_ChoiceControl == null)
            {
                m_ChoiceControl = GetChoiceControl();
                m_ChoiceControl.ApplySelection += new EventHandler(m_ChoiceControl_ApplySelection);
                m_ChoiceControl.SelectedItemChanged += new EventHandler<DateEventArgs>(m_ChoiceControl_SelectedItemChanged);

                ContentControl = m_ChoiceControl;
                PopUpContainer.PopupControl.Caption = Localization.DateChoice_Caption + "...";
            }

            UpdateButtonsState();

            BeforePopUp();

            if (NeedReload)
            {
                m_ChoiceControl.URL = URL;
                m_ChoiceControl.Connection = m_Connection;
                m_ChoiceControl.CubeName = m_CubeName;
                m_ChoiceControl.DayLevelUniqueName = m_DayLevelUniqueName;
                m_ChoiceControl.DateToUniqueNameTemplate = m_DateToUniqueNameTemplate;

                m_ChoiceControl.Initialize();
                NeedReload = false;
            }
            
            base.OnBeforePopUp(sender, e);
        }

        void m_ChoiceControl_SelectedItemChanged(object sender, DateEventArgs e)
        {
            UpdateButtonsState();
        }

        protected virtual void BeforePopUp()
        {

        }

        protected virtual DateChoiceCtrl GetChoiceControl()
        {
            return new DateChoiceCtrl();
        }

        void m_ChoiceControl_ApplySelection(object sender, EventArgs e)
        {
            ApplySelection();
        }

        protected override void ApplySelection()
        {
            UpdatePopUpText();
            base.ApplySelection();
        }

        void UpdatePopUpText()
        {
            if (m_ChoiceControl.SelectedDate != null && m_ChoiceControl.SelectedDate.HasValue)
            {
                PopUpContainer.Text = m_ChoiceControl.SelectedDate.Value.ToShortDateString();
            }
            else
            {
                PopUpContainer.Text = String.Empty;
            }
        }

        protected bool NeedReload = true;

        #region Свойства для настройки на OLAP
        private String m_Connection = String.Empty;
        /// <summary>
        /// Описание соединения с БД
        /// </summary>
        public String Connection
        {
            get
            {
                return m_Connection;
            }
            set
            {
                m_Connection = value;
                NeedReload = true;
            }
        }

        private string m_CubeName;
        /// <summary>
        /// Имя OLAP куба
        /// </summary>
        public String CubeName
        {
            get
            {
                return m_CubeName;
            }
            set
            {
                m_CubeName = value;
                NeedReload = true;
            }
        }

        private string m_DayLevelUniqueName = String.Empty;
        /// <summary>
        /// Уникальное имя уровня, хранящего дни
        /// </summary>
        public String DayLevelUniqueName
        {
            get
            {
                return m_DayLevelUniqueName;
            }
            set
            {
                m_DayLevelUniqueName = value;
                NeedReload = true;
            }
        }

        private string m_DateToUniqueNameTemplate = String.Empty;
        /// <summary>
        /// Шаблон для преобразования даты в уникальное имя
        /// </summary>
        public String DateToUniqueNameTemplate
        {
            get
            {
                return m_DateToUniqueNameTemplate;
            }
            set
            {
                m_DateToUniqueNameTemplate = value;
                NeedReload = true;
            }
        }
        #endregion Свойства для настройки на OLAP

    }
}
