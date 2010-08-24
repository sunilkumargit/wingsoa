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
using Ranet.Olap.Core.Metadata;
using Ranet.AgOlap.Controls.MdxDesigner.Wrappers;

namespace Ranet.AgOlap.Controls.MdxDesigner.Filters
{
    public class FilterControl : UserControl
    {
        FilterFamilyCombo comboFilterFamily;
        Grid grdFilterContainer;

        public FilterControl()
        {
            Grid grdLayoutRoot = new Grid();

            grdLayoutRoot.RowDefinitions.Add(new RowDefinition(){ Height = GridLength.Auto });
            grdLayoutRoot.RowDefinitions.Add(new RowDefinition(){ Height = GridLength.Auto });
            grdLayoutRoot.RowDefinitions.Add(new RowDefinition(){ Height = GridLength.Auto });
            grdLayoutRoot.RowDefinitions.Add(new RowDefinition(){ Height = GridLength.Auto });

            grdLayoutRoot.ColumnDefinitions.Add(new ColumnDefinition());

            Grid grdFilterType = new Grid();
            grdFilterType.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            grdFilterType.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(200) });
            grdFilterType.ColumnDefinitions.Add(new ColumnDefinition());
            // Заголовок
            TextBlock lblFilterType = new TextBlock() { VerticalAlignment = VerticalAlignment.Center, Text = Localization.FilterControl_Label_FilterType, Margin = new Thickness(0, 0, 0, 0) };
            grdFilterType.Children.Add(lblFilterType);
            Grid.SetRow(lblFilterType, 0);
            Grid.SetColumn(lblFilterType, 0);

            // Выбор типа фильтра
            comboFilterFamily = new FilterFamilyCombo() { Margin = new Thickness(5,0,0,0) };
            comboFilterFamily.SelectionChanged += new EventHandler(comboFilterFamily_SelectionChanged);
            grdFilterType.Children.Add(comboFilterFamily);
            Grid.SetRow(comboFilterFamily, 0);
            Grid.SetColumn(comboFilterFamily, 1);

            grdLayoutRoot.Children.Add(grdFilterType);
            Grid.SetRow(grdFilterType, 0);

            // Заголовок
            TextBlock lblFilterSettings = new TextBlock() { Text = Localization.FilterControl_Label_FilterSettings, Margin = new Thickness(0, 5, 0, 3) };
            grdLayoutRoot.Children.Add(lblFilterSettings);
            Grid.SetRow(lblFilterSettings, 2);

            // Фильтр
            grdFilterContainer = new Grid();
            grdLayoutRoot.Children.Add(grdFilterContainer);
            Grid.SetRow(grdFilterContainer, 3);

            this.Content = grdLayoutRoot;
            this.IsEnabledChanged += new DependencyPropertyChangedEventHandler(FilterControl_IsEnabledChanged);
            BuildFilter();
        }

        void FilterControl_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            m_TopFilterControl.IsEnabled = IsEnabled;
            m_ValueFilterControl.IsEnabled = IsEnabled;
            m_LabelFilterControl.IsEnabled = IsEnabled;
        }

        TopFilterControl m_TopFilterControl = new TopFilterControl();
        ValueFilterControl m_ValueFilterControl = new ValueFilterControl();
        LabelFilterControl m_LabelFilterControl = new LabelFilterControl();

        CubeDefInfo m_CubeInfo;
        public void Initialize(CubeDefInfo cubeInfo)
        {
            m_CubeInfo = cubeInfo;
            m_TopFilterControl.Initialize(cubeInfo);
            m_ValueFilterControl.Initialize(cubeInfo);
        }

        public void Initialize(Filter_FilterWrapper filter)
        {
            m_TopFilterControl.Initialize(filter.TopFilter);
            m_ValueFilterControl.Initialize(filter.ValueFilter);
            m_LabelFilterControl.Initialize(filter.LabelFilter);
            comboFilterFamily.SelectItem(filter.CurrentFilter);
        }

        void comboFilterFamily_SelectionChanged(object sender, EventArgs e)
        {
            BuildFilter();
        }

        void BuildFilter()
        {
            grdFilterContainer.Children.Clear();
            switch (comboFilterFamily.CurrentType)
            {
                case FilterFamilyTypes.TopFilter:
                    grdFilterContainer.Children.Add(m_TopFilterControl);
                    break;
                case FilterFamilyTypes.ValueFilter:
                    grdFilterContainer.Children.Add(m_ValueFilterControl);
                    break;
                case FilterFamilyTypes.LabelFilter:
                    grdFilterContainer.Children.Add(m_LabelFilterControl);
                    break;
            }
        }

        /// <summary>
        /// Фильтр (Top, по значению либо по подписи)
        /// </summary>
        public Filter_FilterWrapper Filter
        {
            get
            {
                Filter_FilterWrapper m_Filter = new Filter_FilterWrapper();
                m_Filter.CurrentFilter = comboFilterFamily.CurrentType;
                m_Filter.TopFilter = m_TopFilterControl.Filter;
                m_Filter.ValueFilter = m_ValueFilterControl.Filter;
                m_Filter.LabelFilter = m_LabelFilterControl.Filter;
                return m_Filter;
            }
        }
    }
}
