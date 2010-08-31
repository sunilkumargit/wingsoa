﻿/*
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
using Wing.AgOlap.Controls.MdxDesigner.Filters;

namespace Wing.AgOlap.Controls.PivotGrid.Conditions
{
    public class CellValueConditionControl : UserControl
    {
        CellConditionTypeCombo comboFilterType;
        NumericUpDown numCount_1;
        TextBlock lblAnd;
        NumericUpDown numCount_2;

        public CellValueConditionControl()
        {
            Grid LayoutRoot = new Grid();

            LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition());
            LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition());

            LayoutRoot.RowDefinitions.Add(new RowDefinition());
            LayoutRoot.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(24) });
            LayoutRoot.RowDefinitions.Add(new RowDefinition());

            // Тип фильтра
            comboFilterType = new CellConditionTypeCombo() { Margin = new Thickness(5, 0, 0, 0) };
            comboFilterType.SelectionChanged += new SelectionChangedEventHandler(comboFilterType_SelectionChanged);
            LayoutRoot.Children.Add(comboFilterType);
            Grid.SetRow(comboFilterType, 1);
            Grid.SetColumn(comboFilterType, 1);

            // Количество записей
            numCount_1 = new NumericUpDown() { Margin = new Thickness(5, 0, 0, 0) };
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
            if (comboFilterType.CurrentType == CellConditionType.Between ||
                comboFilterType.CurrentType == CellConditionType.NotBetween)
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

        public event EventHandler EditEnd;

        void comboFilterType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Refresh();

            EventHandler handler = EditEnd;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Текущее условие
        /// </summary>
        public CellConditionType ConditionType
        {
            get { return comboFilterType.CurrentType; }
            set
            {
                comboFilterType.SelectItem(value);
                Refresh();
            }
        }

        /// <summary>
        /// Значение 1
        /// </summary>
        public double Value1
        {
            get { return numCount_1.Value; }
            set
            {
                numCount_1.Value = value;
            }
        }

        /// <summary>
        /// Значение 2
        /// </summary>
        public double Value2
        {
            get { return numCount_2.Value; }
            set
            {
                numCount_2.Value = value;
            }
        }

    }
}