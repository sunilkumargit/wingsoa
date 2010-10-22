/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Windows;
using Wing.Olap.Controls.General;

namespace Wing.Olap.Controls
{
    public class MeasureChoicePopUp : AgPopUpControlBase
    {
        MeasureChoiceCtrl m_ChoiceControl = null;
        protected MeasureChoiceCtrl ChoiceControl
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
                m_ChoiceControl.SelectedItemChanged += new EventHandler<ItemEventArgs>(m_ChoiceControl_SelectedItemChanged);
                m_ChoiceControl.Margin = new Thickness(5);

                ContentControl = m_ChoiceControl;
                PopUpContainer.PopupControl.Caption = Localization.MeasureChoice_Caption + "...";
            }

            UpdateButtonsState();

            BeforePopUp();

            if (NeedReload)
            {
                m_ChoiceControl.URL = URL;
                m_ChoiceControl.Connection = m_AConnection;
                m_ChoiceControl.CubeName = m_ACubeName;
                m_ChoiceControl.MeasureGroupName = m_AMeasureGroupName;

                m_ChoiceControl.Initialize();
                NeedReload = false;
            }
        }

        void m_ChoiceControl_SelectedItemChanged(object sender, ItemEventArgs e)
        {
            UpdateButtonsState();
        }

        protected virtual void BeforePopUp()
        {

        }

        protected virtual MeasureChoiceCtrl GetChoiceControl()
        {
            return new MeasureChoiceCtrl();
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
            if (m_ChoiceControl.SelectedInfo != null)
            {
                PopUpContainer.Text = m_ChoiceControl.SelectedInfo.Caption;
            }
            else
            {
                PopUpContainer.Text = String.Empty;
            }
        }

        protected bool NeedReload = true;

        #region Свойства для настройки на OLAP
        private String m_AConnection = String.Empty;
        /// <summary>
        /// Описание соединения с БД
        /// </summary>
        public String AConnection
        {
            get
            {
                return m_AConnection;
            }
            set
            {
                m_AConnection = value;
                NeedReload = true;
            }
        }

        private string m_ACubeName;
        /// <summary>
        /// Имя OLAP куба
        /// </summary>
        public String ACubeName
        {
            get
            {
                return m_ACubeName;
            }
            set
            {
                m_ACubeName = value;
                NeedReload = true;
            }
        }

        private string m_AMeasureGroupName;
        /// <summary>
        /// Имя OLAP куба
        /// </summary>
        public String AMeasureGroupName
        {
            get
            {
                return m_AMeasureGroupName;
            }
            set
            {
                m_AMeasureGroupName = value;
                NeedReload = true;
            }
        }
        #endregion Свойства для настройки на OLAP
    }
}