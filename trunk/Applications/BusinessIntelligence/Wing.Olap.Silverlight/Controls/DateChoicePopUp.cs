/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using Wing.Olap.Controls.General;

namespace Wing.Olap.Controls
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
