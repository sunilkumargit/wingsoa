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
using System.ComponentModel;
using Ranet.Olap.Core.Metadata;
using Ranet.AgOlap.Controls.ValueCopy.Wrappers;
using System.Collections.Generic;

namespace Ranet.AgOlap.Controls.ValueCopy
{
    public class CoordinateItem : INotifyPropertyChanged
    {
        public readonly String DimensionUniqueName = String.Empty;
        public readonly String HierarchyUniqueName = String.Empty;
        public String Hierarchy_Custom_AllMemberUniqueName = String.Empty;
        MemberWrap m_SourceMember = null;
        MemberWrap m_DestMember = null;

        public CoordinateItem(String dimension, String hierarchy)
        {
            if (String.IsNullOrEmpty(dimension))
            {
                throw new ArgumentNullException("dimension");
            }
            if (String.IsNullOrEmpty(hierarchy))
            {
                throw new ArgumentNullException("hierarchy");
            }

            DimensionUniqueName = dimension;
            HierarchyUniqueName = hierarchy;
        }

        public CoordinateItem(CoordinateItem_Wrapper wrapper)
        {
            if (wrapper == null)
            {
                throw new ArgumentNullException("wrapper");
            }

            CoordinateState = wrapper.CoordinateState;
            DimensionCaption = wrapper.DimensionCaption;
            DimensionUniqueName = wrapper.DimensionUniqueName;
            Hierarchy_Custom_AllMemberUniqueName = wrapper.Hierarchy_Custom_AllMemberUniqueName;
            HierarchyCaption = wrapper.HierarchyCaption;
            HierarchyUniqueName = wrapper.HierarchyUniqueName;
            DestMember = wrapper.DestMember;
            SourceMember = wrapper.SourceMember;
        }

        /// <summary>
        /// Состояние координаты. ТОЛЬКО для отображения в гриде
        /// </summary>
        public String Accessibility
        {
            get
            {
                switch (CoordinateState)
                {
                    case CoordinateStateTypes.Disabled:
                        return Localization.ValueCopyControl_CoordinateState_Disabled;
                    case CoordinateStateTypes.Enabled:
                        return Localization.ValueCopyControl_CoordinateState_Enabled;
                    case CoordinateStateTypes.Readonly:
                        return Localization.ValueCopyControl_CoordinateState_Readonly;
                    default:
                        return Localization.ValueCopyControl_CoordinateState_Unknown;
                }
            }
            set
            {
                if (value == Localization.ValueCopyControl_CoordinateState_Disabled)
                {
                    CoordinateState = CoordinateStateTypes.Disabled;
                    return;
                }
                if (value == Localization.ValueCopyControl_CoordinateState_Enabled)
                {
                    CoordinateState = CoordinateStateTypes.Enabled;
                    return;
                }
                if (value == Localization.ValueCopyControl_CoordinateState_Readonly)
                {
                    CoordinateState = CoordinateStateTypes.Readonly;
                    return;
                }
                CoordinateState = CoordinateStateTypes.Unknown;
            }
        }

        /// <summary>
        /// Список возможных состояний координат. ТОЛЬКО для отображения в гриде
        /// </summary>
        public List<string> StatesList
        {
            get
            {
                List<String> list = new List<String>();
                list.Add(Localization.ValueCopyControl_CoordinateState_Enabled);
                list.Add(Localization.ValueCopyControl_CoordinateState_Disabled);
                list.Add(Localization.ValueCopyControl_CoordinateState_Readonly);
                return list;
            }
        }

        /// <summary>
        /// Видимость контролов для редактирования координаты (Изменение источника, приемника, конки удаления). Используется ТОЛЬКО для управления отображением в гриде
        /// </summary>
        public Visibility ModifyControlsVisibility
        {
            get
            {
                if (CoordinateState != CoordinateStateTypes.Readonly)
                    return Visibility.Visible;
                return Visibility.Collapsed;
            }
        }

        String m_DimensionCaption = String.Empty;
        /// <summary>
        /// Название измерения - используется ТОЛЬКО для отображения в гриде
        /// </summary>
        public String DimensionCaption
        {
            get { return m_DimensionCaption; }
            set { m_DimensionCaption = value; }
        }

        CoordinateStateTypes m_CoordinateState = CoordinateStateTypes.Enabled;
        public CoordinateStateTypes CoordinateState
        {
            get { return m_CoordinateState; }
            set { m_CoordinateState = value; }
        }

        String m_HierarchyCaption = String.Empty;
        /// <summary>
        /// Название иерархии - используется ТОЛЬКО для отображения в гриде
        /// </summary>
        public String HierarchyCaption
        {
            get { return m_HierarchyCaption; }
            set { m_HierarchyCaption = value; }
        }

        public MemberWrap SourceMember
        {
            get { return m_SourceMember; }
            set { 
                m_SourceMember = value;
                // Изменился и кэпшен для элемента
                Raise_PropertyChanged("SourceMemberCaption");
            }
        }

        public MemberWrap DestMember
        {
            get { return m_DestMember; }
            set
            {
                m_DestMember = value;
                // Изменился и кэпшен для элемента
                Raise_PropertyChanged("DestMemberCaption");
            }
        }

        public string SourceMemberUniqueName
        {
            get
            {
                if (SourceMember != null)
                {
                    if (SourceMember.IsDefaultMember)
                    {
                        return HierarchyUniqueName + "." + DEFAULTMEMBER;
                    }
                    if (SourceMember.IsUnknownMember)
                    {
                        return HierarchyUniqueName + "." + UNKNOWNMEMBER;
                    }
                    return SourceMember.UniqueName;
                }
                return String.Empty;
            }
        }

        const String DEFAULTMEMBER = "DEFAULTMEMBER";
        const String UNKNOWNMEMBER = "UNKNOWNMEMBER";

        public String SourceMemberCaption
        {
            get {
                if (SourceMember != null)
                {
                    if (SourceMember.IsDefaultMember)
                    {
                        return DEFAULTMEMBER;
                    }
                    if (SourceMember.IsUnknownMember)
                    {
                        return UNKNOWNMEMBER;
                    }
                    return SourceMember.Caption;
                }
                return String.Empty; 
            }
            set
            {
            }
        }

        public String DestMemberUniqueName
        {
            get
            {
                if (DestMember != null)
                {
                    if (DestMember.IsDefaultMember)
                    {
                        return HierarchyUniqueName + "." + DEFAULTMEMBER;
                    }
                    if (DestMember.IsUnknownMember)
                    {
                        return HierarchyUniqueName + "." + UNKNOWNMEMBER;
                    }
                    return DestMember.UniqueName;
                }
                return String.Empty;
            }
        }

        public String DestMemberCaption
        {
            get
            {
                if (DestMember != null)
                {
                    if (DestMember.IsDefaultMember)
                    {
                        return DEFAULTMEMBER;
                    }
                    if (DestMember.IsUnknownMember)
                    {
                        return UNKNOWNMEMBER;
                    }
                    return DestMember.Caption;
                }
                return String.Empty;
            }
            set
            {
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void Raise_PropertyChanged(String propName)
        {
            if (!String.IsNullOrEmpty(propName))
            {
                PropertyChangedEventHandler handler = PropertyChanged;
                if (handler != null)
                {
                    handler(this, new PropertyChangedEventArgs(propName));
                }
            }
        }
    }

    public enum CoordinateStateTypes
    { 
        Unknown,
        Enabled,
        Disabled,
        Readonly
    }
}
