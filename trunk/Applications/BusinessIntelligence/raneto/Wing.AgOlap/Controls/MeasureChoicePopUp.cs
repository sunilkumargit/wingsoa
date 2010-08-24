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
using Ranet.AgOlap.Controls.Buttons;

namespace Ranet.AgOlap.Controls
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