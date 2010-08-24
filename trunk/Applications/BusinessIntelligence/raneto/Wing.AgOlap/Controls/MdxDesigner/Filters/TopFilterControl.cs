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
using Ranet.Olap.Core.Metadata;
using Ranet.AgOlap.Controls.MdxDesigner.Wrappers;
using System.Threading;

namespace Ranet.AgOlap.Controls.MdxDesigner.Filters
{
    public class TopFilterControl : FilterControlBase
    {
        TopFilterTypeCombo comboFilterType;
        NumericUpDown numCount;
        TopFilterTargetCombo comboFilterTarget;
        MeasuresCombo comboMeasure;

        public TopFilterControl()
        {
            Grid LayoutRoot = new Grid();

            LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition());
            LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition());
            LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition());
            LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Star) });

            LayoutRoot.RowDefinitions.Add(new RowDefinition());
            LayoutRoot.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(24) });
            LayoutRoot.RowDefinitions.Add(new RowDefinition());

            // Тип фильтра
            comboFilterType = new TopFilterTypeCombo();
            LayoutRoot.Children.Add(comboFilterType);
            Grid.SetRow(comboFilterType, 1);

            // Количество записей
            numCount = new NumericUpDown() { Margin = new Thickness(5,0,0,0) };
            numCount.Minimum = 0;
            numCount.Increment = 1;
            numCount.Value = 10;
            LayoutRoot.Children.Add(numCount);
            Grid.SetRow(numCount, 1);

            // Расставляем контролы в зависимости от культуры
            String culture = Thread.CurrentThread.CurrentUICulture.Name.ToLower();
            if (culture == "ru" || culture == "ru-ru")
            {
                // Количество
                numCount.Margin = new Thickness(0);
                Grid.SetColumn(numCount, 0);
                // Тип фильтра
                LayoutRoot.ColumnDefinitions[1].Width = new GridLength(2, GridUnitType.Star);
                comboFilterType.Margin = new Thickness(5, 0, 0, 0);
                Grid.SetColumn(comboFilterType, 1);

                // Что отбираем
                LayoutRoot.ColumnDefinitions[2].Width = new GridLength(3, GridUnitType.Star);
            }
            else
            {
                // Тип фильтра
                LayoutRoot.ColumnDefinitions[0].Width = new GridLength(2, GridUnitType.Star);
                comboFilterType.Margin = new Thickness(0);
                Grid.SetColumn(comboFilterType, 0);
                // Количество
                numCount.Margin = new Thickness(5, 0, 0, 0);
                Grid.SetColumn(numCount, 1);

                // Что отбираем
                LayoutRoot.ColumnDefinitions[2].Width = new GridLength(2, GridUnitType.Star);
            }

            // Что отбираем
            comboFilterTarget = new TopFilterTargetCombo() { Margin = new Thickness(5, 0, 0, 0) };
            LayoutRoot.Children.Add(comboFilterTarget);
            Grid.SetColumn(comboFilterTarget, 2);
            Grid.SetRow(comboFilterTarget, 1);

            // Текст "по"
            TextBlock lblBy = new TextBlock() { Text = Localization.Filter_Label_By, TextAlignment = TextAlignment.Center, VerticalAlignment = VerticalAlignment.Bottom, Margin = new Thickness(5, 0, 0, 0) };
            LayoutRoot.Children.Add(lblBy);
            Grid.SetColumn(lblBy, 3);
            Grid.SetRow(lblBy, 1);

            // Мера куба
            comboMeasure = new MeasuresCombo() { Margin = new Thickness(5, 0, 0, 0) }; 
            LayoutRoot.Children.Add(comboMeasure);
            Grid.SetColumn(comboMeasure, 4);
            Grid.SetRow(comboMeasure, 1);
            
            //Width = 500;
            this.Content = LayoutRoot;
        }

        Top_FilterWrapper m_Filter = new Top_FilterWrapper();
        public Top_FilterWrapper Filter
        {
            get {
                m_Filter.FilterType = comboFilterType.CurrentType;
                m_Filter.FilterTarget = comboFilterTarget.CurrentType;
                m_Filter.Count = Convert.ToInt32(numCount.Value);
                if (comboMeasure.CurrentMeasure != null)
                    m_Filter.MeasureUniqueName = comboMeasure.CurrentMeasure.UniqueName;
                else
                    m_Filter.MeasureUniqueName = String.Empty;
                return m_Filter;
            }
        }

        public void Initialize(CubeDefInfo cubeInfo)
        {
            comboMeasure.Initialize(cubeInfo);
        }

        public void Initialize(CubeDefInfo cubeInfo, Top_FilterWrapper wrapper)
        {
            comboMeasure.Initialize(cubeInfo);
            Initialize(wrapper);
        }

        public void Initialize(Top_FilterWrapper wrapper)
        {
            if (wrapper != null)
            {
                comboFilterType.SelectItem(wrapper.FilterType);
                numCount.Value = wrapper.Count;
                comboFilterTarget.SelectItem(wrapper.FilterTarget);
                comboMeasure.SelectItem(wrapper.MeasureUniqueName);
            }
        }
    }
}
