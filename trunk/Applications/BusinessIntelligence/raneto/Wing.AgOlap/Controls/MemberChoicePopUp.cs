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
using Ranet.AgOlap.Controls.MemberChoice.Info;
using System.Collections.Generic;
using Ranet.Olap.Core.Data;

namespace Ranet.AgOlap.Controls
{
    public class MemberChoicePopUp : AgPopUpControlBase
    {
        MemberChoiceControl m_ChoiceControl = null;
        protected MemberChoiceControl ChoiceControl
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

        Grid LayoutRoot = null;

        public MemberChoicePopUp()
        {
            //this.Height = 22;
            ////this.MinHeight = 22;
            ////this.MaxHeight = 22;
            //this.VerticalAlignment = VerticalAlignment.Top;

            //m_Popup.BeforePopUp += new EventHandler(m_Popup_BeforePopUp);

            //LayoutRoot = new Grid();
            //LayoutRoot.Children.Add(m_Popup);

            //this.Content = LayoutRoot;
        }

        protected override void OnBeforePopUp(object sender, EventArgs e)
        {
            if (m_ChoiceControl == null)
            {
                m_ChoiceControl = GetChoiceControl();
                m_ChoiceControl.ApplySelection += new EventHandler(m_ChoiceControl_ApplySelection);
                //m_ChoiceControl.CancelSelection += new EventHandler(m_ChoiceControl_CancelSelection);
                m_ChoiceControl.SelectedItemChanged += new EventHandler<ItemEventArgs>(m_ChoiceControl_SelectedItemChanged);
                
                ContentControl = m_ChoiceControl;
                PopUpContainer.PopupControl.Caption = Localization.MemberChoice_Caption + "...";
            }

            UpdateButtonsState();

            BeforePopUp();

            m_ChoiceControl.SelectLeafs = m_SelectLeafs;

            if (NeedReload)
            {
                m_ChoiceControl.URL = URL;
                m_ChoiceControl.Connection = m_AConnection;
                m_ChoiceControl.CubeName = m_ACubeName;
                m_ChoiceControl.SubCube = m_ASubCube;
                m_ChoiceControl.HierarchyUniqueName = m_AHierarchyName;

                m_ChoiceControl.MultiSelect = m_AMultiSelect;
                m_ChoiceControl.UseStepLoading = m_AUseStepLoading;
                m_ChoiceControl.Step = m_AStep;
                m_ChoiceControl.StartLevelUniqueName = m_AStartLevelUniqueName;
                m_ChoiceControl.SelectedSet = m_SelectedSet;
                m_ChoiceControl.SelectedInfo = m_SelectedInfo;
                m_ChoiceControl.ShowOnlyFirstLevelMembers = m_AShowOnlyFirstLevelMembers;
                m_ChoiceControl.MemberVisualizationType = m_MemberVisualizationType;

                m_ChoiceControl.Initialize();
                NeedReload = false;
            }

        }

        void m_ChoiceControl_SelectedItemChanged(object sender, ItemEventArgs e)
        {
            UpdateButtonsState();
        }

        String m_SelectedSet = String.Empty;
        public String SelectedSet {
            get {
                return m_SelectedSet;
            }
            set {
                m_SelectedSet = value;
                NeedReload = true;
            }
        }

        private List<MemberChoiceSettings> m_SelectedInfo = null;
        /// <summary>
        /// Информация о выбранных элементах
        /// </summary>
        public List<MemberChoiceSettings> SelectedInfo
        {
            get
            {
                return m_SelectedInfo;
            }
            set
            {
                m_SelectedInfo = value;
                NeedReload = true;
            }
        }

        protected virtual void BeforePopUp()
        {

        }

        protected virtual MemberChoiceControl GetChoiceControl()
        {
            return new MemberChoiceControl();
        }

        //void m_ChoiceControl_CancelSelection(object sender, EventArgs e)
        //{
        //    m_Popup.IsDropDownOpen = false;
        //}

        void m_ChoiceControl_ApplySelection(object sender, EventArgs e)
        {
            m_SelectedSet = m_ChoiceControl.SelectedSet;
            m_SelectedInfo = m_ChoiceControl.SelectedInfo;
            ApplySelection();
        }

        protected override void ApplySelection()
        {
            UpdatePopUpText();
            base.ApplySelection();
        }

        void UpdatePopUpText()
        {
            if (m_ChoiceControl.SelectedInfo != null && m_ChoiceControl.SelectedInfo.Count > 0)
            {
                //Если выбрано более одного элемента, то это однозначно множественный выбор
                if (m_ChoiceControl.SelectedInfo.Count > 1)
                    PopUpContainer.Text = Localization.MemberChoice_MultiSelect; //"<Множественный выбор>";
                else
                {
                    MemberChoiceSettings cs = m_ChoiceControl.SelectedInfo[0];
                    if (cs != null)
                    {
                        if (cs.SelectState != SelectStates.Selected_Self)
                            PopUpContainer.Text = Localization.MemberChoice_MultiSelect; //"<Множественный выбор>";
                        else
                        {
                            if (cs.Info != null && cs.Info != null)
                            {
                                PopUpContainer.Text = cs.Info.GetText(MemberVisualizationType);
                            }
                            else
                            {
                                PopUpContainer.Text = String.Empty;
                            }
                            //m_Popup.ToolTip = cs.Caption + " : " + cs.UniqueName;
                        }
                    }
                }
            }
            else
            {
                PopUpContainer.Text = "";
            }

            //String set = String.Empty;
            //if (m_ChoiceControl.SelectedInfo != null)
            //{
            //    set = m_ChoiceControl.SelectedSet;
            //}

            //try
            //{
            //    //Сохраняем в глобальную переменную
            //    IExecutionContext context = (IExecutionContext)this.Context.Services.GetService(typeof(IExecutionContext));
            //    if (context != null && !String.IsNullOrEmpty(AVariableToMDXSet))
            //    {
            //        ThreadPool.QueueUserWorkItem(new WaitCallback(
            //            state => context.ModifyVariable(this.Context, AVariableToMDXSet, set)));
            //    }
            //}
            //catch (Exception ex)
            //{
            //    if (!LogException(ex))
            //        throw ex;
            //}
        }
        
        //protected readonly PopUpContainerControl m_Popup = new PopUpContainerControl();

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

        String m_ASubCube = String.Empty;
        /// <summary>
        /// Выражение для под-куба
        /// </summary>
        public String ASubCube
        {
            get
            {
                return this.m_ASubCube;
            }
            set
            {
                m_ASubCube = value;
            }
        }

        private string m_AHierarchyName;
        /// <summary>
        /// Имя иерархии измерения
        /// </summary>
        public String AHierarchyName
        {
            get
            {
                return m_AHierarchyName;
            }
            set
            {
                m_AHierarchyName = value;
                NeedReload = true;
            }
        }
        #endregion Свойства для настройки на OLAP

        #region Частичная загрузка
        private int m_AStep = 100;
        /// <summary>
        /// Шаг при частичной загрузке ("Задает шаг, используемый при частичной загрузке элементов")
        /// </summary>
        public int AStep
        {
            get
            {
                return m_AStep;
            }
            set
            {
                m_AStep = value;
                NeedReload = true;
            }
        }

        private bool m_AUseStepLoading = true;
        /// <summary>
        /// Использование частичной загрузки ("Флаг указывает необходимость использования частичной загрузки элементов")
        /// </summary>
        public bool AUseStepLoading
        {
            get
            {
                return m_AUseStepLoading;
            }
            set
            {
                m_AUseStepLoading = value;
                NeedReload = true;
            }
        }
        #endregion Частичная загрузка

        private bool m_SelectLeafs = false;
        /// <summary>
        /// Выбирать только листья
        /// </summary>
        public bool ASelectLeafs
        {
            get
            {
                return m_SelectLeafs;
            }
            set
            {
                m_SelectLeafs = value;
                NeedReload = true;
            }
        }

        MemberVisualizationTypes m_MemberVisualizationType = MemberVisualizationTypes.Caption;
        public MemberVisualizationTypes MemberVisualizationType
        {
            get { return m_MemberVisualizationType; }
            set
            {
                m_MemberVisualizationType = value;
                NeedReload = true;
            }
        }

        /// <summary>
        /// Режим выбора: true - множественный выбор, false - одиночный выбор
        /// </summary>
        private bool m_AMultiSelect = true;
        public bool AMultiSelect
        {
            set
            {
                m_AMultiSelect = value;
                NeedReload = true;
            }
            get
            {
                return m_AMultiSelect;
            }
        }

        private string m_AStartLevelUniqueName;
        /// <summary>
        /// Уникальное имя уровня, начиная с которого элементы будут отображаться в дереве
        /// </summary>
        public string AStartLevelUniqueName
        {
            get
            {
                return m_AStartLevelUniqueName;
            }
            set
            {
                m_AStartLevelUniqueName = value;
                NeedReload = true;
            }
        }

        private bool m_AShowOnlyFirstLevelMembers = false;
        /// <summary>
        /// Флажок. Выводить элементы ТОЛЬКО верхнего уровня
        /// </summary>
        public bool AShowOnlyFirstLevelMembers
        {
            set
            {
                m_AShowOnlyFirstLevelMembers = value;
                NeedReload = true;
            }
            get
            {
                return ChoiceControl.ShowOnlyFirstLevelMembers;
            }
        }
    }
}
