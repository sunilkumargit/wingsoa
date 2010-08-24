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
using System.Collections.Generic;
using System.Text;
using Ranet.Olap.Core.Data;

namespace Ranet.AgOlap.Controls.MemberChoice.Info
{
    public class MemberChoiceSettings
    {
        public MemberChoiceSettings()
        { 
        }


        /// <summary>
        /// Конструктор объекта, описавающего состояние элемента
        /// </summary>
        /// <param name="uniqueName">Уникальное имя мембера</param>
        /// <param name="caption">Заголовок мембера</param>
        /// <param name="state">состояние</param>
        public MemberChoiceSettings(MemberData info, SelectStates state)
        {
            m_Info = info;
            m_SelectState = state;
        }

        private MemberData m_Info = null;
        /// <summary>
        /// Информация об элементе
        /// </summary>
        public MemberData Info
        {
            get { return m_Info; }
            set { m_Info = value; }
        }

        /// <summary>
        /// Уникальное имя мембера
        /// </summary>
        public String UniqueName
        {
            get {
                if (Info != null)
                {
                    return Info.UniqueName;
                }
                return String.Empty;
            }
        }
        
        /// <summary>
        /// Заголовок мембера
        /// </summary>
        public String Caption
        {
            get
            {
                if (Info != null)
                {
                    return Info.Caption;
                }
                return String.Empty;
            }
            
        }

        /// <summary>
        /// Состояние выбранности элемента
        /// </summary>
        private SelectStates m_SelectState = SelectStates.Not_Initialized;
        public SelectStates SelectState
        {
            get { return m_SelectState; }
            set { m_SelectState = value; }
        }
    }
}
