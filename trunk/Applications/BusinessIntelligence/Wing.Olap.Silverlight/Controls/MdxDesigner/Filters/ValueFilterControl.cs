/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
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
using Wing.Olap.Core.Metadata;
using Wing.Olap.Controls.MdxDesigner.Wrappers;

namespace Wing.Olap.Controls.MdxDesigner.Filters
{
    public class ValueFilterControl : FilterControlBase
    {
        MeasuresCombo comboMeasure;
        ValueFilterTypeCombo comboFilterType;
        NumericUpDown numCount_1;
        TextBlock lblAnd;
        NumericUpDown numCount_2;

        public ValueFilterControl()
        {
            Grid LayoutRoot = new Grid();

            LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Star) });
            LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Star) });
            LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition());
            LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition());

            LayoutRoot.RowDefinitions.Add(new RowDefinition());
            LayoutRoot.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(24) });
            LayoutRoot.RowDefinitions.Add(new RowDefinition());

            // Мера куба
            comboMeasure = new MeasuresCombo() { Margin = new Thickness(0) };
            LayoutRoot.Children.Add(comboMeasure);
            Grid.SetColumn(comboMeasure, 0);
            Grid.SetRow(comboMeasure, 1);

            // Тип фильтра
            comboFilterType = new ValueFilterTypeCombo() { Margin = new Thickness(5, 0, 0, 0) };
            comboFilterType.SelectionChanged += new SelectionChangedEventHandler(comboFilterType_SelectionChanged);
            LayoutRoot.Children.Add(comboFilterType);
            Grid.SetRow(comboFilterType, 1);
            Grid.SetColumn(comboFilterType, 1);

            // Количество записей
            numCount_1 = new NumericUpDown() { Margin = new Thickness(5,0,0,0) };
            numCount_1.DecimalPlaces = 2;
            numCount_1.Increment = 1;
            numCount_1.Minimum = int.MinValue;
            numCount_1.Maximum = int.MaxValue;
            numCount_1.Value = 0;
            LayoutRoot.Children.Add(numCount_1);
            Grid.SetRow(numCount_1, 1);
            Grid.SetColumn(numCount_1, 2);

            // Текст "и"
            lblAnd = new TextBlock() { Text = Localization.Filter_Label_And, TextAlignment = TextAlignment.Center, VerticalAlignment = VerticalAlignment.Bottom, Margin = new Thickness(5, 0, 0, 0) };
            LayoutRoot.Children.Add(lblAnd);
            Grid.SetColumn(lblAnd, 3);
            Grid.SetRow(lblAnd, 1);

            // Количество записей
            numCount_2 = new NumericUpDown() { Margin = new Thickness(5, 0, 0, 0) };
            numCount_2.DecimalPlaces = 2;
            numCount_2.Increment = 1;
            numCount_2.Minimum = int.MinValue;
            numCount_2.Maximum = int.MaxValue;
            numCount_2.Value = 1;
            LayoutRoot.Children.Add(numCount_2);
            Grid.SetRow(numCount_2, 1);
            Grid.SetColumn(numCount_2, 4);

            this.Content = LayoutRoot;

            Refresh();
        }

        void Refresh()
        {
            if (comboFilterType.CurrentType == ValueFilterTypes.Between ||
                comboFilterType.CurrentType == ValueFilterTypes.NotBetween)
            {
                Grid.SetColumnSpan(numCount_1, 1);
                lblAnd.Visibility = Visibility.Visible;
                numCount_2.Visibility = Visibility.Visible;
            }
            else
            {
                Grid.SetColumnSpan(numCount_1, 3);
                lblAnd.Visibility = Visibility.Collapsed;
                numCount_2.Visibility = Visibility.Collapsed;
            }
        }

        void comboFilterType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Refresh();
        }

        Value_FilterWrapper m_Filter = new Value_FilterWrapper();
        public Value_FilterWrapper Filter
        {
            get
            {
                m_Filter.FilterType = comboFilterType.CurrentType;
                m_Filter.Num1 = Convert.ToInt32(numCount_1.Value);
                m_Filter.Num2 = Convert.ToInt32(numCount_2.Value);
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

        public void Initialize(CubeDefInfo cubeInfo, Value_FilterWrapper wrapper)
        {
            comboMeasure.Initialize(cubeInfo);
            Initialize(wrapper);
        }

        public void Initialize(Value_FilterWrapper wrapper)
        {
            if (wrapper != null)
            {
                comboFilterType.SelectItem(wrapper.FilterType);
                numCount_1.Value = wrapper.Num1;
                numCount_2.Value = wrapper.Num2;
                comboMeasure.SelectItem(wrapper.MeasureUniqueName);
            }
        }
    }
}